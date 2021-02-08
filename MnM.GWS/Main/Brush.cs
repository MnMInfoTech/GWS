/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if HideGWSObjects
    partial class NativeFactory
    {
#else
    public
#endif
        partial class Brush : IBrush, ITextureBrush, IResizable
        {
            #region  VARIABLES
            protected int width, height, length, type;
            protected BrushStyle Style;
            volatile bool Invert;
            int Rx, Ry;
            bool MatchSize;
            int[] Data;
            float[] CircularDistances;
        volatile ReadChoice choice;
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
            static Brush()
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
                InitializeStatic();
            }
            static partial void InitializeStatic();
            Brush()
            {
                MatchSize = true;
                Initialize();
            }
            partial void Initialize();
            #endregion

            #region CREATE INSTANCE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Brush CreateInstance(BrushStyle style, int width, int height)
            {
                var styl = style ?? BrushStyle.Black;
                var ID = styl.ID;
                Brush brush;

                brush = new Brush();
                brush.Style = styl;
                brush.type = styl.Gradient;
                bool success = false;
                brush.ResizeInternally(width, height, ref success);
                if (success)
                    brush.Store();
                return brush;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe static Brush CreateInstance(IntPtr data, int width, int height)
            {
                var ID = "" + data;
                Brush brush;

                brush = new Brush();
                brush.type = -1;
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
            #endregion

            #region PROPERTIES
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
            public ReadChoice Choice
        {
            get => choice;
            set
            {
                choice = value;
                Invert = (choice & ReadChoice.InvertColor) == ReadChoice.InvertColor;
            }
        }
            unsafe IntPtr IPixels.Source
            {
                get
                {
                    fixed (int* p = Data)
                        return (IntPtr)p;
                }
            }
            #endregion

            #region READ PIXEL
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int ReadPixel(int x, int y)
            {
                int color = 0;
                ReadPixel2(x, y, ref color);
                return color;
            }
            unsafe partial void ReadPixel2(int x, int y, ref int color);
            #endregion

            #region READ LINE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ReadLine(int Start, int End, int Axis, bool Horizontal,
                out int[] pixels, out int srcIndex, out int copyLength)
            {
                pixels = null;
                srcIndex = 0;
                copyLength = 0;
                ReadLine2(Start, End, Axis, Horizontal, ref pixels, ref srcIndex, ref copyLength);
            }
            unsafe partial void ReadLine2(int Start, int End, int Axis, bool Horizontal,
                ref int[] pixels, ref int srcIndex, ref int copyLength);
            #endregion

            #region COPY TO
            public unsafe IPerimeter CopyTo(IntPtr dest, int dstLen, int dstW, int dstX, int dstY, IPerimeter copyArea, Command command = Command.Opaque)
            {
                int length;
                int* dst = (int*)dest;
                copyArea.GetBounds(out int copyX, out int copyY, out int copyW, out int copyH);

                var x = copyX;
                var r = x + copyW;
                var y = copyY;
                var b = y + copyH;

                if (y < 0)
                {
                    b += y;
                    y = 0;
                }

                int destIndex = dstX + dstY * dstW;
                int i = 0;
                while (y < b)
                {
                    ReadLine(x, r, y, true, out int[] source, out int srcIndex, out length);
                    if (destIndex + length >= dstLen)
                        break;
                    fixed (int* src = source)
                        Blocks.Copy(src, srcIndex, dst, destIndex, length, command, null, true);
                    destIndex += dstW;
                    ++i;
                    ++y;
                }
                return new Perimeter(dstX, dstY, copyW, i);
            }
            #endregion

            #region RESIZE
            public void Resize(int? width = null, int? height = null) =>
                Resize2(width, height);
            partial void Resize2(int? width = null, int? height = null);
            partial void ResizeInternally(int w, int h, ref bool success);
            #endregion

            #region COPY SETTINGS
            public void Receive(IDrawParams settings, bool flushMode = false)
            {
                Receive2(settings, flushMode);
            }
            partial void Receive2(IDrawParams settings, bool flushMode = false);
            #endregion

            #region STORE - RESTORE
            unsafe partial void Store();
            unsafe partial void Restore();
            #endregion

            #region CLONE
            public object Clone()
            {
                var brush = new Brush();
                brush.type = type;
                brush.Style = Style.Clone();
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
                CopyTo(brush);
                return brush;
            }
            public object Clone(int w, int h)
            {
                Brush brush = null;
                Clone(ref brush, w, h);
                return brush;
            }
            unsafe partial void Clone(ref Brush brush, int w, int h);
            partial void CopyTo(Brush brush);
            #endregion

            #region DISPOSE
            public void Dispose()
            {
                IsDisposed = true;
                width = height = length = 0;
                Data = null;
                CircularDistances = null;
                Dispose2();
            }
            partial void Dispose2();
            #endregion
        }
#if HideGWSObjects
    }
#endif
}
