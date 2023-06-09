/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/
#if (GWS || Window)
using System;

namespace MnM.GWS
{
    #region ITEXTURE-BRUSH
    public partial interface ITextureBrush : IBrush, IImageSource
    { }
    #endregion

    partial class Factory
    {
        sealed partial class TextureBrush : Brush, ITextureBrush
        {
            #region VARIABLES
            int[] OriginalData;
            string id;
            #endregion

            #region CREATE INSTANCE
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe static ITextureBrush CreateInstance(IntPtr data, int width, int height)
            {
                var brush = new TextureBrush();
                brush.id = Application.NewID("TextureBrush");
                brush.Gradient = BrushType.Texture;
                brush.width = width;
                brush.height = height;
                brush.length = width * height;
                brush.Rx = width / 2;
                brush.Ry = height / 2;
                brush.Data = new int[brush.length + 1];
                fixed (int* dst = brush.Data)
                    Blocks.Copy((int*)data, 0, dst, 0, brush.length);
                brush.Store();
                return brush;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe static ITextureBrush CreateInstance(int[] data, int width, int height)
            {
                var brush = new TextureBrush();
                brush.Gradient = BrushType.Texture;
                brush.width = width;
                brush.height = height;
                brush.length = width * height;
                brush.Rx = width / 2;
                brush.Ry = height / 2;
                brush.Data = data;
                brush.Store();
                return brush;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe static ITextureBrush CreateInstance(byte[] data, int width, int height)
            {
                fixed (byte* b = data)
                   return CreateInstance((IntPtr)b, width, height);
            }
            #endregion

            #region PROPERTIES
            protected override int SolidBrushColour => Data[0];
            public override string ID => id;
            unsafe IntPtr ISource<IntPtr>.Source
            { 
                get
                {
                    fixed (int* p = Data)
                        return (IntPtr)p;
                }
            }
            bool IOriginCompatible.IsOriginBased => true;
            int IPoint.X => 0;
            int IPoint.Y => 0;
            #endregion

            #region INDEX OF
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected sealed unsafe override int IndexOf(int x, int y, bool intCalculation, out int x0, out int y0, out float x3, out float y3)
            {
                int index, x1, y1;
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
                index = x0 + y0 * width;

                if (index > length - 1)
                    index %= length;
                #endregion

                return index;
            }
            #endregion

            #region RESIZE
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected sealed override void ResizeInternally(int w, int h, ResizeCommand resizeCommand, ref bool success)
            {
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

                bool AutoReSizeContent = (resizeCommand & ResizeCommand.AutoReSizeContent) == ResizeCommand.AutoReSizeContent;

                if (AutoReSizeContent
                )
                {
                    Interpolation interpolation = 0;
                    if ((resizeCommand & ResizeCommand.AutoReSizeContentBicubic) == ResizeCommand.AutoReSizeContentBicubic)
                    {
                        interpolation = Interpolation.Bicubic;
                    }
                    else if ((resizeCommand & ResizeCommand.AutoReSizeContentBilinear) == ResizeCommand.AutoReSizeContentBilinear)
                    {
                        interpolation = Interpolation.Bilinear;
                    }
                    var t = Factory.ImageProcessor.RotateAndScale
                    (
                        ((ISource<IntPtr>)this).Source, Width, Height, 0, 0, Width, Height, interpolation,
                        true, null, new Scale(Width, Height, w, h)
                    ).Result;
                    w = t.Item1.Width;
                    h = t.Item1.Height;

                    width = w;
                    height = h;
                    Data = new int[width * height];
                    Blocks.Copy(t.Item2, 0, ((ISource<IntPtr>)this).Source, 0, w * h);
                    success = true;
                }

                if (success)
                    return;
                if (w < width && h < height)
                {
                    if (OriginalSize)
                    {
                        if (w < OriginalSize.Width && h < OriginalSize.Height)
                            Restore();
                    }
                    success = true;
                    return;
                }
                int oldw, oldh;
                if (OriginalSize)
                {
                    oldw = OriginalSize.Width;
                    oldh = OriginalSize.Height;
                }
                else
                {
                    oldw = width;
                    oldh = height;
                }

                w = Math.Max(w, width);
                h = Math.Max(h, height);

                Data = Data.ResizedData(w, h, width, height);

                width = w;
                height = h;
                Rx = width / 2;
                Ry = height / 2;
                length = width * height;
                Rx = width / 2;
                Ry = height / 2;
                int sidx;
                int didx;
                int len;
                int idx;
                int last;

                if (width > oldw)
                {
                    sidx = 0;
                    len = width - oldw;
                    for (int i = 0; i < height; i++)
                    {
                        idx = sidx;
                        last = idx + width;
                        didx = idx + oldw;
                        Array.Copy(Data, idx, Data, didx, len);
                        while (didx < last)
                        {
                            Array.Copy(Data, idx, Data, didx, len);
                            didx += len;
                        }
                        sidx += width;
                    }
                }
                if (height > oldh)
                {
                    sidx = 0;
                    len = width;
                    didx = oldh * width;
                    for (int i = oldh; i < height; i++)
                    {
                        idx = sidx;
                        Array.Copy(Data, idx, Data, didx, len);
                        sidx += width;
                        didx += width;
                    }
                }
                success = true;
            }
            partial void ResizeTextureBrushInternally(int w, int h, ref bool sucess);
            #endregion

            #region STORE - RESTORE
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected sealed override void Store()
            {
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
            }
            #endregion

            #region GET DATA
            #endregion

            #region CLONE
            public sealed override object Clone()
            {
                var brush = new TextureBrush();
                brush.Gradient = Gradient;
                brush.width = width;
                brush.height = height;
                brush.length = length;
                brush.Rx = Rx;
                brush.Ry = Ry;
                brush.Data = new int[Data.Length];
                Array.Copy(Data, 0, brush.Data, 0, length);
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

            #region GET ORIGIN-BASED VERSION
            IOriginCompatible IOriginCompatible.GetOriginBasedVersion() => this;
            #endregion

            #region DISPOSE
            protected override void Dispose2()
            {
                OriginalData = null;
            }
            #endregion
        }
    }
}
#endif