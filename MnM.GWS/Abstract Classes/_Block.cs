/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class _Block : IBlock
    {
        #region VARIABLES
        protected int width, height, length;
        protected bool isDisposed;

        protected bool AntiAliased;
        protected bool CheckForCloseness;
        protected bool LineOnly;
        protected bool EndsOnly;
        protected IPen BkgPen;
        protected IPen FrgPen;
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

        #region CLONE
        public abstract object Clone();
        #endregion
    }
}
