/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public sealed class Block : IMemoryBlock
    {
        #region VARIABLES
        int width, height, length;

        int[] Data;
        bool AntiAliased;
        bool Distinct;
        bool Opaque, Back, SkipPenBackground;
        IPen BkgPen;
        LineCommand lineCommand;
        DrawCommand drawCommand;

        readonly HashSet<int> DrawnIndices = new HashSet<int>();
        const byte o = 0;
        #endregion

        #region CONSTRUCTORS
        public Block(int w, int h)
        {
            width = w;
            height = h;
            length = w * h;
            Data = new int[length];
        }
        #endregion

        #region PROPERTIES
        public int Width => width;
        public int Height => height;
        public int Length => length;
        bool IWritable.Antialiased => AntiAliased;
        public IReadContext Background
        {
            get => BkgPen;
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
        public LineCommand LineCommand
        {
            get => lineCommand;
            set
            {
                lineCommand = value;
                AntiAliased = !LineCommand.HasFlag(LineCommand.Breshenham);
                var distinct = LineCommand.HasFlag(LineCommand.Distinct);
                if (distinct != !Distinct)
                    DrawnIndices.Clear();
                Distinct = distinct;
            }
        }
        public DrawCommand DrawCommand
        {
            get => drawCommand;
            set
            {
                drawCommand = value;
                Opaque = Back = false;
                Opaque = value.HasFlag(DrawCommand.Opaque);
                Back = !Opaque && value.HasFlag(DrawCommand.Back);
            }
        }
        unsafe int* source
        {
            get
            {
                fixed (int* d = Data)
                    return d;
            }
        }
        #endregion

        #region BLEND
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Blend(int dstColor, int srcColor, byte alpha)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePixel(int val, int axis, bool horizontal, int srcColor, float? Alpha)
        {
            if (srcColor == 0 && !Opaque)
                return;

            int i;
            int x = horizontal ? val : axis;
            int y = horizontal ? axis : val;

            if (x < 0 || y < 0 || x >= width || y >= height)
                return;

            i = x + y * width;
            int dc = Data[i];

            if (Back && dc != 0)
                return;

            if (Distinct)
            {
                if (DrawnIndices.Contains(i))
                    return;
                DrawnIndices.Add(i);
            }
            byte alpha;

            float delta = Alpha ?? Colors.Alphas[(byte)((srcColor >> Colors.AShift) & 0xFF)];
            alpha = (byte)(delta * 255);

            if (alpha == 0)
                return;

            if (alpha != 255)
            {
                if (dc == 0)
                    dc = BkgPen?.ReadPixel(x, y) ?? 0;

                srcColor = Blend(dc, srcColor, alpha);
            }
            Data[i] = srcColor;
        }
        #endregion

        #region WRITE LINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteLine(int* src, int srcIndex, int srcW, int copyLength, bool horizontal, int x, int y, float? Alpha)
        {
            #region VARAIBLE INITIALIZATION
            if (copyLength <= 0)
                return;

            int* dst = source;
            int dstIndex = x + y * width;
            int dplus = horizontal ? 1 : width;
            int splus = horizontal || srcW == copyLength ? 1 : srcW;
            int last = dstIndex + dplus * copyLength;
            int j = srcIndex;

            int px = x;
            int py = y;
            int ix = horizontal ? 1 : 0;
            int iy = horizontal ? 0 : 1;
            bool hasBkg = BkgPen != null;

            int dc, sc;
            var NoBlend = Alpha == null;
            byte alpha = !NoBlend ? (byte)(Alpha.Value * 255) : o;
            #endregion

            #region WRITING LINE
            for (int i = dstIndex; i < last; i += dplus, j += splus, px += ix, py += iy)
            {
                if (i >= length) break;

                dc = dst[i];
                sc = src[j];

                if (sc == 0 && dc == 0)
                    continue;

                if (sc == 0)
                {
                    if (Opaque)
                        dst[i] = sc;
                    continue;
                }

                if (Back && dc != 0)
                    continue;

                if (!NoBlend && alpha < 2)
                    continue;

                if (NoBlend || alpha == 255)
                {
                    dst[i] = sc;
                    continue;
                }

                if (dc == 0 && hasBkg)
                    dc = BkgPen.ReadPixel(px, py);

                dst[i] = Blend(dc, sc, alpha);
                continue;
            }
            #endregion

            int w = horizontal ? copyLength : 1;
            int h = horizontal ? 1 : copyLength;
        }
        #endregion

        #region COPY TO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr dest, int dstLen, int dstW, int dstX, int dstY)
        {
            bool hasBkgPen = BkgPen != null;
            bool withBackgroundPixels = hasBkgPen && !SkipPenBackground;

            int bkgIndex = 0;
            int bkgLen = 0;
            int* dst = (int*)dest;
            int srcLen = Length;
            int srcW = Width;
            int srcH = Height;
            int[] bkg;
            int sc, bk;
            int* src = source;
            var copy = Rects.CompitibleRc(srcW, srcH, copyX, copyY, copyW, copyH);

            copyW = copy.Width;
            copyH = copy.Height;
            copyX = copy.X;
            copyY = copy.Y;

            if (copyX < 0)
            {
                copyW += copyX;
                copyX = 0;
            }
            if (copyY < 0)
            {
                copyH += copyY;
                copyY = 0;
            }
            var srcIndex = copyX + copyY * srcW;

            if (dstX < 0)
                dstX = 0;
            if (dstY < 0)
                dstY = 0;

            var dstIndex = dstX + dstY * dstW;

            if (copyW > srcW)
                copyW = srcW;
            if (copyH > srcH)
                copyH = srcH;

            if (srcIndex + copyW >= srcLen)
                copyW -= (srcIndex + copyW - srcLen);

            if (copyW <= 0)
                return Rectangle.Empty;

            if (dstIndex + copyW >= dstLen)
                copyW -= (dstIndex + copyW - dstLen);

            if (copyW <= 0)
                return Rectangle.Empty;

            int i = 0;
            int x = copyX;
            int y = copyY;
            int r = copyX + copyW;

            while (i < copyH)
            {
                if (srcIndex + copyW >= srcLen)
                    copyW -= (srcIndex + copyW - srcLen);
                if (copyW <= 0)
                    break;

                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;

                int didx = dstIndex;
                int sidx = srcIndex;

                if (withBackgroundPixels)
                {
                    BkgPen.ReadLine(x, r, copyY + i, true, out bkg, out bkgIndex, out bkgLen);
                    for (int j = didx; j < didx + copyW; j++, sidx++, bkgIndex++)
                    {
                        sc = src[sidx];
                        bk = bkg[bkgIndex];
                        if (sc == 0)
                            sc = bk;
                        dst[j] = sc;
                    }
                }
                else
                {
                    bk = 0;
                    for (int j = didx; j < didx + copyW; j++, sidx++)
                    {
                        sc = src[sidx];
                        dst[j] = sc;
                    }
                }
                srcIndex += srcW;
                dstIndex += dstW;
                ++i;
            }
            var result = new Rectangle(dstX, dstY, copyW, i);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Rectangle CopyTo(IWritable block, int destX, int destY, int copyX, int copyY, int copyW, int copyH, bool updateImmediate = true)
        {
            var copy = Rects.CompitibleRc(width, height, copyX, copyY, copyW, copyH);
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

            if (dstRc)
                block.Invalidate(dstRc.X, dstRc.Y, dstRc.Width, dstRc.Height, updateImmediate);

            return dstRc;
        }
        #endregion

        #region INVALIDATE
        public void Invalidate(int x, int y, int width, int height, bool updateImmediate = false) { }
        #endregion
    }

}
