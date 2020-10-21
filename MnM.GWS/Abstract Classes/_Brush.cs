/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class _Brush : IBrush
    {
        #region  VARIABLES
        protected int[] Data;
        protected int width, height, length, type;
        protected float[] CircularDistances;
        protected BrushStyle Style;
        protected string id;

        protected bool IsRotated;
        protected int Cx, Cy;
        protected int Cosi, Sini;
        protected float Cos, Sin;
        protected bool AntiClock;
        protected int X, Y, R, B, PX, PY;
        protected int Rx, Ry;
        protected int XCx, YCy;
        protected bool Antialiased;
        protected bool InvertColor;

        protected bool MatchSize;
        #endregion

        #region MATH.ATAN2
        private const int atan2SIZE = 1024;
        // Output will swing from -STRETCH to STRETCH (default: Math.PI)
        // Useful to change to 1 if you would normally do "atan2(y, x) / Math.PI"

        // Inverse of SIZE
        private const int negativeatan2EZIS = -atan2SIZE;
        private static float[] ATAN2_TABLE_PPY = new float[atan2SIZE + 1];
        private static float[] ATAN2_TABLE_PPX = new float[atan2SIZE + 1];
        private static float[] ATAN2_TABLE_PNY = new float[atan2SIZE + 1];
        private static float[] ATAN2_TABLE_PNX = new float[atan2SIZE + 1];
        private static float[] ATAN2_TABLE_NPY = new float[atan2SIZE + 1];
        private static float[] ATAN2_TABLE_NPX = new float[atan2SIZE + 1];
        private static float[] ATAN2_TABLE_NNY = new float[atan2SIZE + 1];
        private static float[] ATAN2_TABLE_NNX = new float[atan2SIZE + 1];
        #endregion

        #region CONSTRUCTORS
        static _Brush()
        {
            for (int i = 0; i <= atan2SIZE; i++)
            {
                float f = (float)i / atan2SIZE;
                ATAN2_TABLE_PPY[i] = (float)(Math.Atan(f) * Angles.PI / Angles.PI);
                ATAN2_TABLE_PPX[i] = (Angles.PI * 0.5f - ATAN2_TABLE_PPY[i]);
                ATAN2_TABLE_PNY[i] = (-ATAN2_TABLE_PPY[i]);
                ATAN2_TABLE_PNX[i] = (ATAN2_TABLE_PPY[i] - Angles.PI * 0.5f);
                ATAN2_TABLE_NPY[i] = (Angles.PI - ATAN2_TABLE_PPY[i]);
                ATAN2_TABLE_NPX[i] = (ATAN2_TABLE_PPY[i] + Angles.PI * 0.5f);
                ATAN2_TABLE_NNY[i] = (ATAN2_TABLE_PPY[i] - Angles.PI);
                ATAN2_TABLE_NNX[i] = (-Angles.PI * 0.5f - ATAN2_TABLE_PPY[i]);
            }
        }
        protected _Brush()
        {
            MatchSize = true;
        }
        #endregion

        #region PROPERTIES
        public string ID =>
            id;
        public int Length =>
            length;
        public int Width =>
            width;
        public int Height =>
            height;
        public int Type
        {
            get => type;
            protected set => type = value;
        }
        public bool IsDisposed { get; private set; }
        BrushStyle IBrush.Style => Style;
        #endregion

        #region READ PIXEL
        public unsafe int ReadPixel(int x, int y)
        {
            if (type == 0)
                return Data[0];

            int x0, y0;
            if (IsRotated)
            {
                x -= Cx;
                y -= Cy;
                x0 = ((x * Cosi + y * Sini) >> Angles.BigExp) + XCx;
                y0 = ((y * Cosi - x * Sini) >> Angles.BigExp) + YCy;
            }
            else
            {
                x0 = x + X;
                y0 = y + Y;
            }

            #region CALCULATE INDEX
            if (x0 < 0) x0 = 0;
            if (y0 < 0) y0 = 0;

            int index;
            float position, Span;
            float angle;

            switch (type)
            {
                case -1: //BrushType.Texture:
                    index = x0 + y0 * width;
                    break;
                case 0: //BrushType.Solid:
                case 1: //BrushType.Horizontal:
                case 6:// BrushType.HorizontalCentral:
                    index = x0;
                    break;
                case 14:// BrushType.Ellipse:
                case 16:// BrushType.MiidleFlatCircular:
                case 9:// BrushType.Conic:
                case 10:// BrushType.Pie:
                    x0 -= Rx;
                    y0 -= Ry;
                    if (x0 == 0 && y0 == 0)
                    {
                        index = 225;
                        break;
                    }
                    else if (x0 >= 0)
                    {
                        if (y0 >= 0)
                        {
                            if (x0 >= y0)
                                angle = ATAN2_TABLE_PPY[(int)(atan2SIZE * y0 / x0 + 0.5f)];
                            else
                                angle = ATAN2_TABLE_PPX[(int)(atan2SIZE * x0 / y0 + 0.5f)];
                        }
                        else
                        {
                            if (x0 >= -y0)
                                angle = ATAN2_TABLE_PNY[(int)(negativeatan2EZIS * y0 / x0 + 0.5f)];
                            else
                                angle = ATAN2_TABLE_PNX[(int)(negativeatan2EZIS * x0 / y0 + 0.5f)];
                        }
                    }
                    else
                    {
                        if (y0 >= 0)
                        {
                            if (-x0 >= y0)
                                angle = ATAN2_TABLE_NPY[(int)(negativeatan2EZIS * y0 / x0 + 0.5f)];
                            else
                                angle = ATAN2_TABLE_NPX[(int)(negativeatan2EZIS * x0 / y0 + 0.5f)];
                        }
                        else
                        {
                            if (x0 <= y0) // (-x >= -y)
                                angle = ATAN2_TABLE_NNY[(int)(atan2SIZE * y0 / x0 + 0.5f)];
                            else
                                angle = ATAN2_TABLE_NNX[(int)(atan2SIZE * x0 / y0 + 0.5f)];
                        }
                    }

                    angle *= Angles.Radinv;

                    if (angle < 0)
                        angle += 360;
                    index = (int)angle;

                    if (type == 9 || type == 10)
                        break;

                    position = Span = CircularDistances[index];
                    position -= (float)Math.Sqrt(x0 * x0 + y0 * y0);

                    var ratio = length / Span;
                    index = (int)((ratio * position));

                    if (index < 0)
                        index = 0;
                    break;

                case 13:
                    x0 -= Rx;
                    y0 -= Ry;
                    index = length - 1 - (int)Math.Sqrt(x0 * x0 + y0 * y0);
                    if (index < 0)
                        index = 0;
                    break;
                case 15:// BrushType.Rectangular:
                    if (y0 > Ry)
                        y0 = height - y0;

                    position = width - y0 * 2 - 1;
                    if (x0 <= y0 || x0 >= y0 + position)
                        index = x0;
                    else
                        index = y0;
                    break;

                case 5:// BrushType.HorizontalSwitch:
                    index = x0;
                    if (y0 > Ry)
                    {
                        if (x0 > width)
                            x0 -= width;
                        index = width + x0;
                    }
                    break;
                case 2:// BrushType.Vertical:
                case 7:// BrushType.VerticalCentral:
                    index = y0;
                    break;
                case 3:// BrushType.ForwardDiagonal:
                case 8:// BrushType.DiagonalCentral:
                    index = (x0 + y0);
                    break;
                case 4:// BrushType.BackwardDiagonal:
                    y0 -= height;
                    if (y0 < 0)
                        y0 = Math.Abs(y0);
                    index = (x0 + y0);
                    break;
                default:
                    index = GetIndexUserDefined(x0, y0);
                    break;
            }
            if (index > length - 1)
            {
                if (type == -1)
                    index %= length;
                else
                    index = length - 1;
            }
            #endregion

            if (InvertColor)
                return Data[index] ^ 0xffffff;
            return Data[index];
        }
        #endregion

        #region GET INDEX
        protected abstract int GetIndexUserDefined(int x, int y);
        #endregion

        #region READ LINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int copyLength)
        {
            #region INITIALIZE OUT VARIABLES IN CASE WE HAVE TO RETURN WITHOUT FURTHER PROCESSING
            pixels = new int[0];
            srcIndex = 0;
            #endregion

            #region CHECK POSITIVE LENGTH
            if (start > end)
            {
                var temp = start;
                start = end;
                end = temp;
            }
            copyLength = 0;
            if (start < 0)
            {
                copyLength = start;
                start = 0;
            }
            copyLength += end - start;

            if (copyLength == 0)
                copyLength = 1;

            if (copyLength < 0)
                return;
            #endregion

            if (type == 0)
            {
                if (copyLength >= Data.Length)
                    copyLength = Data.Length - 1;

                if (InvertColor || !horizontal)
                {
                    int incr = horizontal ? 1 : width;
                    pixels = new int[copyLength];
                    for (int j = 0; j < copyLength; j++)
                    {
                        pixels[j] = Data[srcIndex];
                        srcIndex += incr;
                    }
                    srcIndex = 0;
                    return;
                }
                srcIndex = 0;
                pixels = Data;
                return;
            }

            bool inLoop = IsRotated || !horizontal || (type > 8 && type < 17);

            #region SET REQUISITE VARIABLES
            bool aa = Antialiased;
            int x0 = 0, y0 = 0;
            bool onlyWantToCalculateIndex = false;
            var intCalculation = type != -1 || !aa;
            float x3 = 0, y3 = 0;
            int x = horizontal ? start : axis;
            int y = horizontal ? axis : start;

            int ix = horizontal ? 1 : 0;
            int iy = horizontal ? 0 : 1;
            int i = 0;

            int x1, y1;
            uint alpha, invAlpha, C1, C2, RB, AG;
            float position, Span;
            float angle;
            int index;
            #endregion

            switch (inLoop)
            {
                case true:
                    pixels = new int[copyLength + 1];

                EnterLoop:
                    #region LOOP
                    while (i < copyLength)
                    {
                        /* If below is the case then we are here only to calculate index. we do not actually belong in this loop.
                         * So let us just calculate index and go back to where we belong.
                        */
                        if (onlyWantToCalculateIndex)
                            goto CalculateIndex;

                        #region HANDLE ROTATION
                        if (IsRotated)
                        {
                            x1 = x - Cx;
                            y1 = y - Cy;

                            if (intCalculation)
                            {
                                x0 = ((x1 * Cosi + y1 * Sini) >> Angles.BigExp) + XCx;
                                y0 = ((y1 * Cosi - x1 * Sini) >> Angles.BigExp) + YCy;
                            }
                            else
                            {
                                x3 = (x1 * Cos + y1 * Sin) + XCx;
                                y3 = (y1 * Cos - x1 * Sin) + YCy;
                                x0 = (int)x3;
                                y0 = (int)y3;
                            }
                        }
                        else
                        {
                            x0 = x + X;
                            y0 = y + Y;
                        }
                    #endregion

                    CalculateIndex:
                        #region CALCULATE INDEX
                        if (x0 < 0) x0 = 0;
                        if (y0 < 0) y0 = 0;

                        switch (type)
                        {
                            case -1:// BrushType.Texture:
                                index = x0 + y0 * width;
                                break;
                            case 0://BrushType.Solid
                                index = 0;
                                break;

                            case 1: //BrushType.Horizontal:
                            case 6:// BrushType.HorizontalCentral:
                                index = x0;
                                break;

                            case 13: //BrushType.Circular:
                                x0 -= Rx;
                                y0 -= Ry;
                                index = length - 1 - (int)Math.Sqrt(x0 * x0 + y0 * y0);
                                if (index < 0)
                                    index = 0;
                                break;

                            case 14://BrushType.Ellipse:
                            case 16:// BrushType.MiidleFlatCircular:
                            case 9:// BrushType.Conic0:
                            case 10:// BrushType.Conic90:
                                x0 -= Rx;
                                y0 -= Ry;
                                if (x0 == 0 && y0 == 0)
                                {
                                    index = 225;
                                    break;
                                }
                                else if (x0 >= 0)
                                {
                                    if (y0 >= 0)
                                    {
                                        if (x0 >= y0)
                                            angle = ATAN2_TABLE_PPY[(int)(atan2SIZE * y0 / x0 + 0.5f)];
                                        else
                                            angle = ATAN2_TABLE_PPX[(int)(atan2SIZE * x0 / y0 + 0.5f)];
                                    }
                                    else
                                    {
                                        if (x0 >= -y0)
                                            angle = ATAN2_TABLE_PNY[(int)(negativeatan2EZIS * y0 / x0 + 0.5f)];
                                        else
                                            angle = ATAN2_TABLE_PNX[(int)(negativeatan2EZIS * x0 / y0 + 0.5f)];
                                    }
                                }
                                else
                                {
                                    if (y0 >= 0)
                                    {
                                        if (-x0 >= y0)
                                            angle = ATAN2_TABLE_NPY[(int)(negativeatan2EZIS * y0 / x0 + 0.5f)];
                                        else
                                            angle = ATAN2_TABLE_NPX[(int)(negativeatan2EZIS * x0 / y0 + 0.5f)];
                                    }
                                    else
                                    {
                                        if (x0 <= y0) // (-x >= -y)
                                            angle = ATAN2_TABLE_NNY[(int)(atan2SIZE * y0 / x0 + 0.5f)];
                                        else
                                            angle = ATAN2_TABLE_NNX[(int)(atan2SIZE * x0 / y0 + 0.5f)];
                                    }
                                }

                                angle *= Angles.Radinv;

                                if (angle < 0)
                                    angle += 360;
                                index = (int)angle;

                                if (type == 9 || type == 10)
                                    break;

                                position = Span = CircularDistances[index];
                                position -= (float)Math.Sqrt(x0 * x0 + y0 * y0);

                                var ratio = length / Span;
                                index = (int)((ratio * position));

                                if (index < 0)
                                    index = 0;
                                break;

                            case 15:// BrushType.Rectangular:
                                if (Ry >= Rx)
                                {
                                    if (y0 > Ry)
                                        y0 = height - y0;

                                    position = width - y0 * 2 - 1;
                                    if (x0 <= y0 || x0 >= y0 + position)
                                        index = x0;
                                    else
                                        index = y0;
                                }
                                else
                                {
                                    if (x0 > Rx)
                                        x0 = width - x0;

                                    position = height - x0 * 2 - 1;
                                    if (y0 <= x0 || y0 >= x0 + position)
                                        index = y0;
                                    else
                                        index = x0;
                                }
                                break;

                            case 5:// BrushType.HorizontalSwitch:
                                index = x0;
                                if (y0 > Ry)
                                {
                                    if (x0 > width)
                                        x0 -= width;
                                    index = width + x0;
                                }
                                break;

                            case 2:// BrushType.Vertical:
                            case 7:// BrushType.VerticalCentral:
                                index = y0;
                                break;

                            case 3:// BrushType.ForwardDiagonal:
                            case 8:// BrushType.DiagonalCentral:
                                index = (x0 + y0);
                                break;

                            case 4:// BrushType.BackwardDiagonal:
                                y0 -= height;
                                if (y0 < 0)
                                    y0 = Math.Abs(y0);

                                index = (x0 + y0);
                                break;

                            default:
                                index = GetIndexUserDefined(x0, y0);
                                break;
                        }

                        if (index > length - 1)
                        {
                            if (type == -1)
                                index %= length;
                            else
                                index = length - 1;
                        }
                        #endregion

                        if (onlyWantToCalculateIndex)
                        {
                            /* We do not actually belong in this loop.
                            * So let us just go back to where we belong to.*/
                            onlyWantToCalculateIndex = false;
                            goto HorizontalLineCopy;
                        }

                        int Color;

                        if (!IsRotated || intCalculation || index == length - 1)
                        {
                            Color = Data[index];
                            goto assignColor;
                        }

                        float Dx = x3 - x0;
                        float Dy = y3 - y0;

                        if (Dx == 0 && Dy == 0)
                        {
                            Color = Data[index];
                            goto assignColor;
                        }

                        #region BI-LINEAR INTERPOLATION
                        int N = index + width;
                        bool Only2 = (N >= length || N + 1 >= length);

                        C1 = (uint)Data[index++];
                        C2 = (uint)Data[index];

                        alpha = (uint)(Dx * 255);
                        invAlpha = 255 - alpha;

                        if (alpha == 255)
                            C1 = C2;

                        else if (alpha != 0)
                        {
                            //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
                            RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C2 & Colors.RBMASK))) >> 8;
                            AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C2 & Colors.GMASK) >> 8)));
                            C1 = ((RB & Colors.RBMASK) | (AG & Colors.AGMASK));
                        }
                        if (Only2)
                        {
                            Color = (int)C1;
                            goto assignColor;
                        }

                        uint C3 = (uint)Data[N++];
                        uint C4 = (uint)Data[N];

                        if (alpha == 255)
                            C3 = C4;
                        else if (alpha != 0)
                        {
                            //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
                            RB = ((invAlpha * (C3 & Colors.RBMASK)) + (alpha * (C4 & Colors.RBMASK))) >> 8;
                            AG = (invAlpha * ((C3 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C4 & Colors.GMASK) >> 8)));
                            C3 = ((RB & Colors.RBMASK) | (AG & Colors.AGMASK));
                        }

                        alpha = (uint)(Dy * 255);
                        invAlpha = 255 - alpha;

                        if (alpha == 255)
                            Color = (int)C3;
                        else if (alpha != 0)
                        {
                            //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
                            RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C3 & Colors.RBMASK))) >> 8;
                            AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C3 & Colors.GMASK) >> 8)));
                            Color = (int)((RB & Colors.RBMASK) | (AG & Colors.AGMASK));
                        }
                        else
                            Color = (int)C1;
                        #endregion

                        assignColor:
                        if (InvertColor)
                            Color ^= 0xffffff;
                        pixels[i] = Color;

                        x += ix;
                        y += iy;
                        ++i;
                    }
                    #endregion

                    return;

                default:
                    x0 = x;
                    y0 = y;

                    x0 += X;
                    y0 += Y;
                    /* Here comes the weird part, we need to calculate index of location x0, y0
                     * but we do not want to get out of this method to call another method to do so.
                     * Saves a millisecond or two. So go to loop, enter the loop and from then jump to CalculateIndex label,
                     * Calculate index and come back here!*/
                    onlyWantToCalculateIndex = true;
                    goto EnterLoop;

                HorizontalLineCopy:
                    if (index < 0)
                    {
                        srcIndex = 0;
                        copyLength = 0;
                        return;
                    }

                    #region HORIZONTAL LINE COPY

                    // BrushType.Vertical || BrushType.VerticalCentral:
                    if (type == 2 || type == 7)
                    {
                        pixels = Data[index].Repeat(copyLength, InvertColor);
                        srcIndex = 0;
                    }
                    else
                    {
                        if (InvertColor)
                        {
                            if (index + copyLength > length)
                            {
                                var difference = index + copyLength - length;
                                copyLength -= difference;
                            }
                            pixels = new int[copyLength];
                            for (int j = 0; j < copyLength; j++)
                            {
                                pixels[j] = Data[index++];
                            }
                            srcIndex = 0;
                            return;
                        }
                        else
                        {
                            pixels = Data;
                            srcIndex = index;
                        }
                    }
                    //NOT BrushType.Vertical || BrushType.VerticalCentral || BrushType.Rectangular
                    if (type != 2 && type != 7 && type != 15)
                    {
                        if (srcIndex + copyLength > length)
                        {
                            var difference = srcIndex + copyLength - length;
                            copyLength -= difference;
                        }
                    }

                    if (pixels.Length == 0 || copyLength < 0)
                    {
                        srcIndex = 0;
                        copyLength = 0;
                        return;
                    }
                    #endregion
                    return;
            }
        }
        #endregion

        #region COPY TO
        public unsafe Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr dest, int destLen, int destW, int destX, int destY)
        {
            int length;
            int* dst = (int*)dest;

            var x = copyX;
            var r = x + copyW;
            var y = copyY;
            var b = y + copyH;

            if (y < 0)
            {
                b += y;
                y = 0;
            }

            int destIndex = destX + destY * destW;
            int i = 0;
            while (y < b)
            {
                ReadLine(x, r, y, true, out int[] source, out int srcIndex, out length);
                if (destIndex + length >= destLen)
                    break;
                fixed (int* src = source)
                    Blocks.Copy(src, srcIndex, dst, destIndex, length);
                destIndex += destW;
                ++i;
                ++y;
            }
            return new Rectangle(destX, destY, copyW, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Rectangle CopyTo(IWritable destination, int destX, int destY, int copyX, int copyY, int copyW, int copyH)
        {
            Rectangle destRc = Rectangle.Empty;
            Rectangle copyRc = new Rectangle(copyX, copyY, copyW, copyH);

            var x = copyRc.X;
            var r = x + copyRc.Width;
            var y = copyRc.Y;
            var b = y + copyRc.Height;

            if (y < 0)
            {
                b += y;
                y = 0;
            }
            copyRc = Rects.CompitibleRc(width, height, copyX, copyY, copyW, copyH);
            int destLen = destination.Length;
            var dy = destY;
            int srcIndex, copylen;

            int i = 0;
            while (y < b)
            {
                ReadLine(x, r, y, true, out int[] source, out srcIndex, out copylen);
                fixed (int* src = source)
                    destination.WriteLine(src, srcIndex, copylen, copylen, true, destX, dy++, null);
                ++i;
                ++y;
            }
            destRc = Rectangle.FromLTRB(x, y, r, dy);
            if (destRc)
                (destination).Invalidate(destRc.X, destRc.Y, destRc.Width, destRc.Height, true);

            return destRc;
        }
        #endregion

        #region RESIZE
#if Advanced
        public abstract void Resize(int? width = null, int? height = null);
#endif
        #endregion

        #region COPY SETTINGS
        public abstract void CopySettings(ISettable settings, bool flushMode = false);
        #endregion

        #region CLONE
        public object Clone()
        {
            var target = emptyInstance();
            CopyTo(target);
            return target;
        }
        public abstract object Clone(int w, int h);
        protected virtual void CopyTo(_Brush block)
        {
            block.type = type;
            block.Style = (BrushStyle)Style.Clone();
            block.id = id;

            block.X = X;
            block.Y = Y;
            block.Cx = Cx;
            block.Cy = Cy;
            block.XCx = XCx;
            block.YCy = YCy;
            block.Cos = Cos;
            block.Cosi = Cosi;
            block.Sini = Sini;
            block.Sin = Sin;
            block.IsRotated = IsRotated;
            block.AntiClock = AntiClock;

            block.width = width;
            block.height = height;
            block.length = length;

            block.Rx = Rx;
            block.Ry = Ry;
            block.Data = new int[Data.Length];

            Array.Copy(Data, 0, block.Data, 0, length);
            if (CircularDistances != null)
            {
                block.CircularDistances = new float[361];
                Array.Copy(CircularDistances, 0, block.CircularDistances, 0, 361);
            }
        }
        protected abstract _Brush emptyInstance();
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            IsDisposed = true;
            Data = null;
            width = height = length = 0;
        }
        #endregion
    }
}
