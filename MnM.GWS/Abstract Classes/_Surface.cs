/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class _Surface : ISurface
    {
        #region VARIABLES
        protected readonly bool IsContainer;
        protected int width, height, length;
        protected bool isDisposed;

        protected bool AntiAliased;
        protected bool CheckForCloseness;
        protected bool LineOnly;
        protected bool EndsOnly;
        protected IPen BkgPen;
        #endregion

        #region CONTRUCTORS
        protected _Surface()
        {
            IsContainer = this is IContainer;
        }
        #endregion

        #region PROPERTIES
        public string ID { get; protected set; }
        public int Width => width;
        public int Height => height;
        public int Length => length;
        public virtual IReadContext Background
        {
            get => BkgPen ?? Pens.White;
            set
            {
                if (value == null)
                {
                    (BkgPen as IDisposable)?.Dispose();
                    BkgPen = null;
                    return;
                }
                BkgPen = value.ToPen(Width, Height);
            }
        }
        public bool IsDisposed => isDisposed;
        bool IWritable.Antialiased => AntiAliased;

        public abstract
#if Advanced
            IDrawSettings2
#else
            IDrawSettings
#endif
            Settings
        { get; }

        protected abstract unsafe int* source { get; }
#if Advanced
        public abstract IObjectDraw ObjectDraw { get; }
        public unsafe abstract byte* SourceAlphas { set; }
        protected abstract unsafe byte* alphas { get; }
#endif
        #endregion

        #region BEGIN - END
        public abstract void Begin(IRenderable renderable, out IPen pen);
        public abstract void End(IPen pen);
        #endregion

        #region BLEND
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int Blend(int dstColor, int srcColor, byte alpha)
        {
            if (alpha == 255)
                return srcColor;

            //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
            uint C1 = (uint)dstColor;
            uint C2 = (uint)srcColor;
            uint invAlpha = 255 - (uint)alpha;
            uint RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C2 & Colors.RBMASK))) >> 8;
            uint AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C2 & Colors.GMASK) >> 8)));
            int color = (int)((RB & Colors.RBMASK) | (AG & Colors.AGMASK));
            return color;
        }
        #endregion

        #region WRITE PIXEL
        public abstract void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha);
        #endregion

        #region WRITE LINE
        public abstract unsafe void WriteLine(int* source, int srcIndex, int srcW, int length, bool horizontal, int x, int y, float? Alpha);
        #endregion

        #region COPY TO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Rectangle CopyTo(IWritable block, int destX, int destY, int copyX, int copyY, int copyW, int copyH)
        {
            var copy = Rects.CompitibleRc(width, height, copyX, copyY, copyW, copyH);

#if Advanced
            if (block is IAlphaSource)
                ((IAlphaSource)block).SourceAlphas = alphas;
#endif
            Rectangle dstRc;
            var x = copy.X;
            var y = copy.Y;
            copyW = copy.Width;

            var b = y + copy.Height;
            if (y < 0)
            {
                b += y;
                y = 0;
            }
            int srcLen = length;
            int srcIndex = x + y * width;
            int srcW = width;
            var dy = destY;
            int* src = source;

            for (int j = y; j <= b; j++)
            {
                block.WriteLine(src, srcIndex, srcW, copyW, true, destX, dy++, null);
                srcIndex += srcW;
                if (srcIndex >= srcLen)
                    break;
            }
            dstRc = new Rectangle(destX, destY, copyW, dy - destY);

#if Advanced
            if (block is IAlphaSource)
                ((IAlphaSource)block).SourceAlphas = null;
#endif

            if (dstRc)
                block.Invalidate(dstRc.X, dstRc.Y, dstRc.Width, dstRc.Height, true);

            return dstRc;
        }
        public abstract Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int dstLen, int dstW, int dstX, int dstY);
        #endregion

        #region INVALIDATE
        public abstract void Invalidate(int x, int y, int width, int height, bool updateImmediate = false);
        #endregion

        #region FIND ELEMENT
#if Advanced
        public abstract IRenderable FindElement(int x, int y);
#endif
        #endregion

#if Advanced
        #region COPY FROM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH);
        #endregion
#endif
        #region CLONE
        public object Clone()
        {
            var surface = EmptyInstance(width, height);
            surface.width = width;
            surface.height = height;
            surface.Settings.CopySettings(Settings);

            if (BkgPen != null)
                surface.BkgPen = BkgPen?.Clone() as IPen;

            CopyToClone(surface);
            return surface;
        }
        protected abstract _Surface EmptyInstance(int width, int height);
        protected virtual void CopyToClone(_Surface buffer) { }
        #endregion

        #region ROTATE 
        /// <summary>
        /// Source: https://www.drdobbs.com/architecture-and-design/fast-bitmap-rotation-and-scaling/184416337
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="antiAliased"></param>
        public unsafe Size RotateAndScale(out int[] result, Rotation rotation, bool antiAliased, float scale = 1f)
        {
            Size size = new Size(width, height);
            if (scale <= 0) scale = 1;
            int dstW = width;
            int dstH = height;
            int srcW = width;
            int srcH = height;
            int* src = source;
            int srcLen = length;

            result = new int[dstW * dstH];
            fixed (int* dst = result)
            {
                if (!rotation && scale == 1)
                {
                    Blocks.Copy(src, 0, dst, 0, srcLen);
                    return size;
                }

                int srcCx = srcW / 2;
                int srcCy = srcH / 2;

                int dstCx = dstW / 2;
                int dstCy = dstH / 2;

                bool intCalculation = !antiAliased;

                float dstXf = 0;
                float dstYf = 0;
                int dstXi = 0;
                int dstYi = 0;
                float x3 = 0, y3 = 0;
                int x0 = 0, y0 = 0, xi = 0, yi = 0;
                int color = 0;

                int Sini = rotation.Sini;
                int Cosi = rotation.Cosi;

                float Sin = rotation.Sin;
                float Cos = rotation.Cos;

                if (scale != 1)
                {
                    Sin *= 1f / scale;
                    Cos *= 1f / scale;

                    Sini = (Sin * Angles.Big).Round();
                    Cosi = (Cos * Angles.Big).Round();
                }

                if (intCalculation)
                {
                    dstXi = -(dstCx * Cosi + dstCy * Sini);
                    dstYi = -(dstCx * -Sini + dstCy * Cosi);
                }
                else
                {
                    dstXf = srcCx - (dstCx * Cos + dstCy * Sin);
                    dstYf = srcCy - (dstCx * -Sin + dstCy * Cos);
                }

                for (int j = 0; j < dstH; j++)
                {
                    if (intCalculation)
                    {
                        xi = dstXi;
                        yi = dstYi;
                    }
                    else
                    {
                        x3 = dstXf;
                        y3 = dstYf;
                    }

                    int* pDst = dst + (dstW * j);

                    for (int i = 0; i < dstW; i++)
                    {
                        if (intCalculation)
                        {
                            x0 = srcCx + (xi >> Angles.BigExp);
                            y0 = srcCy + (yi >> Angles.BigExp);
                        }
                        else
                        {
                            x0 = (int)x3;
                            y0 = (int)y3;
                        }

                        if (x0 < 0 || y0 < 0 || x0 >= srcW || y0 >= srcH)
                        {
                            pDst++;
                            goto horizotalIncrement;
                        }

                        var index = x0 + (y0 * srcW);

                        if (intCalculation || (x0 - x3 == 0 && y3 - y0 == 0))
                        {
                            color = src[index];
                            if (color == 0)
                                color = *pDst;
                            goto assignColor;
                        }
                        float Dx = x3 - x0;
                        float Dy = y3 - y0;

                        #region BI-LINEAR INTERPOLATION
                        uint rb, ag, c3 = 0, c4 = 0;
                        int n = index + srcW;
                        bool only2 = (n >= srcLen || n + 1 >= srcLen);

                        uint c1 = (uint)src[index++];
                        uint c2 = (uint)src[index];
                        if (!only2)
                        {
                            c3 = (uint)src[n++];
                            c4 = (uint)src[n];
                        }
                        if (c1 == 0 || c1 == Colors.Transparent)
                            c1 = Colors.White;
                        if (c2 == 0 || c2 == Colors.Transparent)
                            c2 = Colors.White;

                        if (c3 == 0 || c3 == Colors.Transparent)
                            c3 = Colors.White;
                        if (c4 == 0 || c4 == Colors.Transparent)
                            c4 = Colors.White;

                        uint alpha = (uint)(Dx * 255);
                        uint invAlpha = 255 - alpha;

                        if (alpha == 255)
                            c1 = c2;

                        else if (alpha != 0)
                        {
                            rb = ((invAlpha * (c1 & Colors.RBMASK)) + (alpha * (c2 & Colors.RBMASK))) >> 8;
                            ag = (invAlpha * ((c1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((c2 & Colors.GMASK) >> 8)));
                            c1 = ((rb & Colors.RBMASK) | (ag & Colors.AGMASK));
                        }
                        if (only2)
                        {
                            color = (int)c1;
                            goto assignColor;
                        }

                        if (alpha == 255)
                            c3 = c4;
                        else if (alpha != 0)
                        {
                            rb = ((invAlpha * (c3 & Colors.RBMASK)) + (alpha * (c4 & Colors.RBMASK))) >> 8;
                            ag = (invAlpha * ((c3 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((c4 & Colors.GMASK) >> 8)));
                            c3 = ((rb & Colors.RBMASK) | (ag & Colors.AGMASK));
                        }

                        alpha = (uint)(Dy * 255);
                        invAlpha = 255 - alpha;

                        if (alpha == 255)
                            color = (int)c3;
                        else if (alpha != 0)
                        {
                            rb = ((invAlpha * (c1 & Colors.RBMASK)) + (alpha * (c3 & Colors.RBMASK))) >> 8;
                            ag = (invAlpha * ((c1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((c3 & Colors.GMASK) >> 8)));
                            color = (int)((rb & Colors.RBMASK) | (ag & Colors.AGMASK));
                        }
                        else
                            color = (int)c1;
                        #endregion

                        assignColor:
                        *pDst++ = color;

                    horizotalIncrement:
                        #region HORIZONTAL INCREMENT
                        if (intCalculation)
                        {
                            xi += Cosi;
                            yi -= Sini;
                        }
                        else
                        {
                            x3 += Cos;
                            y3 -= Sin;
                        }
                        #endregion
                    }

                    #region VERTICAL INCREMENT
                    if (intCalculation)
                    {
                        dstXi += Sini;
                        dstYi += Cosi;
                    }
                    else
                    {
                        dstXf += Sin;
                        dstYf += Cos;
                    }
                    #endregion
                }
            }
            return size;
        }
        #endregion

        #region FLIP
        public unsafe Size Flip(out int[] result, Flip flipMode)
        {
            int dstW = width;
            int dstH = height;
            int srcW = width;
            int srcH = height;
            int* src = source;
            int srcLen = length;
            result = new int[srcW * srcH];

            int i = 0;

            if (flipMode == GWS.Flip.Horizontal)
            {
                for (var y = srcH - 1; y >= 0; y--)
                {
                    for (var x = 0; x < srcW; x++)
                    {
                        var srcInd = y * srcW + x;
                        result[i] = src[srcInd];
                        i++;
                    }
                }
            }
            else
            {
                for (var y = 0; y < srcH; y++)
                {
                    for (var x = srcW - 1; x >= 0; x--)
                    {
                        var srcInd = y * srcW + x;
                        result[i] = src[srcInd];
                        i++;
                    }
                }
            }
            if (flipMode == GWS.Flip.Vertical)
                Numbers.Swap(ref srcW, ref srcH);

            return new Size(srcW, srcH);
        }
        #endregion

        #region CLEAR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle Clear(bool updateImmediate = false) =>
            Clear(0, 0, width, height, updateImmediate);
        public abstract Rectangle Clear(int x, int y, int width, int height, bool updateImmediate = false);
        #endregion

        #region UPDATE
        public abstract void Update();
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            isDisposed = true;
            (BkgPen as IDisposable)?.Dispose();
        }
        #endregion
    }
}
