/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/
#if (GWS || Window)
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IGRADIENTBRUSH
    public interface IGradientBrush : IBrush
    {
        /// <summary>
        /// 
        /// </summary>
        IBrushStyle Style { get; }
    }
    #endregion

    partial class Factory
    {
        sealed partial class GradientBrush : Brush, IGradientBrush
        {
            #region VARIABLES
            IBrushStyle Style;
            int[] OriginalData;
            float[] CircularDistances;
            float[] OriginalCircularDistances;
            string id;
            #endregion

            #region CREATE INSTANCE
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static IGradientBrush CreateInstance(IBrushStyle style, int width, int height)
            {
                var styl = style ?? BrushStyle.Black;
                var brush = new GradientBrush();
                brush.id = Application.NewID("GrdienBrush");
                brush.Style = styl;
                brush.Gradient = styl.Gradient;
                bool success = false;
                brush.ResizeInternally(width, height, 0, ref success);
                if (success)
                    brush.Store();
                return brush;
            }
            #endregion

            #region PROPERTIES
            protected override int SolidBrushColour => Style.StartColour;
            sbyte IBrush.Gradient => Gradient;
            IBrushStyle IGradientBrush.Style => Style;
            public override string ID => id;
            #endregion

            #region INDEX OF
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected sealed override int IndexOf(int x, int y, bool intCalculation, out int x0, out int y0, out float x3, out float y3)
            {
                int index, x1, y1;
                float position, Span, angle;
                x3 = y3 = 0;

                #region HANDLE ROTATION
                if (IsRotated)
                {
                    x1 = x - Cx;
                    y1 = y - Cy;

                    if (intCalculation)
                    {
                        x0 = ((x1 * Cosi + y1 * Sini) >> BigExp) + XCx;
                        y0 = ((y1 * Cosi - x1 * Sini) >> BigExp) + YCy;
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
                    x0 = x + BrushX;
                    y0 = y + BrushY;
                }
                #endregion

                #region CALCULATE INDEX
                if (x0 < 0) x0 = 0;
                if (y0 < 0) y0 = 0;

                switch (Gradient)
                {
                    case BrushType.Texture:
                        index = x0 + y0 * width;
                        break;
                    case BrushType.Solid:
                        index = 0;
                        break;

                    case BrushType.Horizontal:
                    case BrushType.HorizontalCentral:
                        index = x0;
                        break;

                    case BrushType.Circular:
                        x0 -= Rx;
                        y0 -= Ry;
                        index = length - 1 - (int)Math.Sqrt(x0 * x0 + y0 * y0);
                        if (index < 0)
                            index = 0;
                        break;

                    case BrushType.Elliptical:
                    case BrushType.MiddleCircular:
                    case BrushType.Conical:
                    case BrushType.Conical2:
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

                        if (Gradient == 9 || Gradient == 10)
                            break;

                        position = Span = CircularDistances[index];
                        position -= (float)Math.Sqrt(x0 * x0 + y0 * y0);

                        var ratio = length / Span;
                        index = (int)((ratio * position));

                        if (index < 0)
                            index = 0;
                        break;

                    case BrushType.Rectangular:
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

                    case BrushType.HorizontalSwitch:
                        index = x0;
                        if (y0 > Ry)
                        {
                            if (x0 > width)
                                x0 -= width;
                            index = width + x0;
                        }
                        break;

                    case BrushType.Vertical:
                    case BrushType.VerticalCentral:
                        index = y0;
                        break;

                    case BrushType.ForwardDiagonal:
                    case BrushType.DiagonalCentral:
                        index = (x0 + y0);
                        break;

                    case BrushType.BackwardDiagonal:
                        y0 -= height;
                        if (y0 < 0)
                            y0 = -y0;
                        index = (x0 + y0);
                        break;

                    default:
                        index = x0 + y0 * width;
                        GetIndexUserDefined(Gradient, x0, y0, ref index);
                        break;
                }

                if (index > length - 1)
                {
                    if (Gradient == BrushType.Texture)
                        index %= length;
                    else
                        index = length - 1;
                }
                #endregion

                return index;
            }
            #endregion

            #region PARTIAL METHODS
            partial void GetIndexUserDefined(int gradient, int x, int y, ref int index);
            partial void GetSizeUserDefined(int gradient, ref int w, ref int h, ref int size);
            partial void GetDataUserDefined(int gradient, int w, int h, int size, ref int[] data);
            #endregion

            #region RESIZE
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected sealed override void ResizeInternally(int w, int h, ResizeCommand resizeCommand, ref bool success)
            {
                if (w == 0)
                    w = 1;
                if (h == 0)
                    h = 1;
                if (w < 0)
                    w = -w;
                if (h < 0)
                    h = -h;
                var w1 = width - 1;
                var h1 = height - 1;

                if ((w == width && h == height) || (w1 == w && h1 == h))
                {
                    success = false;
                    return;
                }

                width = w;
                height = h;
                Rx = width / 2;
                Ry = height / 2;
                VectorF center = new VectorF(width / 2f, height / 2f);

                switch (Gradient)
                {
                    case BrushType.Solid:
                    case BrushType.Horizontal:
                        length = width + 1;
                        break;
                    case BrushType.HorizontalCentral:
                        length = this.width + 1;
                        break;
                    case BrushType.Vertical:
                        length = height + 1;
                        break;

                    case BrushType.VerticalCentral:
                        length = height + 1;
                        break;

                    case BrushType.ForwardDiagonal:
                    case BrushType.BackwardDiagonal:
                    case BrushType.DiagonalCentral:
                        length = (width + height) + 1;
                        break;

                    case BrushType.HorizontalSwitch:
                        length = (this.width) * 2 + 1;
                        break;

                    case BrushType.Circular:
                        length = Math.Max(width, height) / 2 + 1;
                        break;

                    case BrushType.Rectangular:
                        length = Math.Min(width, height) + 1;
                        break;

                    case BrushType.Elliptical:
                    case BrushType.MiddleCircular:
                        length = (int)Math.Ceiling(center.Length());
                        break;
                    case BrushType.Conical:
                    case BrushType.Conical2:
                        length = 361;
                        break;
                    default:
                        length = w * h;
                        GetSizeUserDefined(Gradient, ref this.width, ref this.height, ref length);
                        break;
                }

                if (Gradient == BrushType.Solid)
                {
                    Data = Style.GetColour(0, 1, false).Repeat(length);
                    success = true;
                    return;
                }

                bool centerToLeftRight = (Gradient > BrushType.BackwardDiagonal && Gradient < BrushType.Conical2) ||
                    Gradient == BrushType.Rectangular;
                bool linear = Gradient > BrushType.Solid && Gradient < 11 ||
                    Gradient == BrushType.Rectangular || Gradient == BrushType.Circular;
                bool radial = Gradient == BrushType.Elliptical || Gradient == BrushType.MiddleCircular;

                if (linear || radial)
                {
                    int start = 0;
                    int end = length, max = length;
                    Data = new int[length];
                    if (Data.Length == 0)
                    {
                        success = true;
                        return;
                    }
                    if (centerToLeftRight)
                    {
                        end = (int)(end / 2f) + 1;
                        max = (int)(max / 2f) + 1;
                    }
                    int len = Data.Length;

                    for (int i = start; i < end; i++)
                    {
                        int idx = i - start;
                        Data[idx] = Style.GetColour(i, max);

                        if (centerToLeftRight)
                            Data[len - 1 - idx] = Data[idx];
                    }
                    if (linear)
                    {
                        success = true;
                        return;
                    }
                }
                if (radial)
                {
                    CircularDistances = new float[361];
                    var cx = center.X;
                    var cy = center.Y;
                    bool pieAngle = Gradient == BrushType.Elliptical && cx != cy;
                    float x, y, cs, sin, cos, RxRy = cx * cy;

                    for (int i = 0; i < 361; i++)
                    {
                        Angles.SinCos(i, out sin, out cos);
                        if (!pieAngle)
                        {
                            x = cx * cos;
                            y = cy * sin;
                        }
                        else
                        {
                            var cycos = cy * cos;
                            cycos *= cycos;
                            var cxsin = cx * sin;
                            cxsin *= cxsin;
                            cs = (float)Math.Sqrt((cycos) + (cxsin));
                            x = (RxRy * cos) / cs;
                            y = (RxRy * sin) / cs;
                        }
                        CircularDistances[i] = (float)Math.Sqrt(x * x + y * y);
                    }
                    success = true;
                    return;
                }
                Data = new int[width * height];
                GetDataUserDefined(Gradient, width, height, length, ref Data);
                success = true;
            }
            #endregion

            #region STORE - RESTORE
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected sealed override void Store()
            {
                if (CircularDistances != null)
                {
                    OriginalCircularDistances = new float[CircularDistances.Length];
                    Array.Copy(CircularDistances, OriginalCircularDistances, CircularDistances.Length);
                }

                OriginalData = new int[length];
                Array.Copy(Data, OriginalData, length);
                OriginalSize = new Size(this.width, this.height);
                OriginalLength = length;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected sealed override void Restore()
            {
                if (length == Data.Length && width == OriginalSize.Width && height == OriginalSize.Height)
                    return;
                width = OriginalSize.Width;
                height = OriginalSize.Height;
                length = OriginalLength;
                Rx = width / 2;
                Ry = height / 2;

                if (OriginalData != null)
                {
                    Data = new int[length];
                    Array.Copy(OriginalData, Data, length);
                }
                if (OriginalCircularDistances != null)
                {
                    CircularDistances = new float[361];
                    Array.Copy(OriginalCircularDistances, CircularDistances, 361);
                }
            }
            #endregion

            #region CLONE
            public sealed override object Clone()
            {
                var brush = new GradientBrush();
                brush.Gradient = Gradient;
                brush.Style = (IBrushStyle)Style?.Clone();
                brush.width = width;
                brush.height = height;
                brush.length = length;
                brush.Rx = Rx;
                brush.Ry = Ry;
                brush.Data = new int[Data.Length];
                Array.Copy(Data, 0, brush.Data, 0, length);
                if (CircularDistances != null)
                {
                    brush.CircularDistances = new float[361];
                    Array.Copy(CircularDistances, 0, brush.CircularDistances, 0, 361);
                }
                brush.OriginalLength = OriginalLength;
                brush.OriginalSize = OriginalSize;
                brush.OriginalData = new int[OriginalLength];
                Array.Copy(OriginalData, 0, brush.OriginalData, 0, OriginalLength);

                brush.BrushX = BrushX;
                brush.BrushY = BrushY;
                brush.Cx = Cx;
                brush.Cy = Cy;
                brush.XCx = XCx;
                brush.YCy = YCy;
                brush.Cos = Cos;
                brush.Cosi = Cosi;
                brush.Sini = Sini;
                brush.Sin = Sin;
                brush.IsRotated = IsRotated;

                return brush;
            }
            #endregion

            #region DISPOSE
            protected sealed override void Dispose2()
            {
                CircularDistances = null;
                OriginalData = null;
                OriginalCircularDistances = null;
            }
            #endregion
        }
    }
}
#endif