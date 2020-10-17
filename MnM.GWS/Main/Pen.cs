/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if AllHidden
    partial class _Factory
    {
#else
    public
#endif
        sealed class Pen : IPen, IColor, ISettings
        {
            #region VARIABLES
            int X, Y, w, h, R, B;
            int color;
            bool Inversion;
            #endregion

            #region CONSTRUCTOR
            Pen(int color)
            {
                this.color = color;
                ID = color.ToString();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Pen CreateInstance(int color)
            {
                var ID = color.ToString();
                Pen pen;

                if (!Pens.Get(ID, out pen))
                {
                    pen = new Pen(color);
                }
                return pen;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Pen CreateInstance(IColor color) =>
                CreateInstance(color.Color);
            #endregion

            #region PROPERTIES
            public string ID { get; private set; }
            public int Color => color;
            public int Type => 0;
            public int Width => w;
            public int Height => h;
            public int Length => w * h;
            #endregion

            #region READ PIXEL
            public int ReadPixel(int x, int y)
            {
                if (Inversion)
                    return color ^ 0xffffff;
                return color;
            }
            #endregion

            #region READLINE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe void ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length)
            {
                pixels = new int[0];
                srcIndex = 0;

                if (!Numbers.PositiveLength(ref start, ref end, out length))
                    goto mks;
                int c = color;
                if (Inversion)
                    c = color ^ 0xffffff;

                pixels = new int[length];

                fixed (int* d = pixels)
                {
                    for (int i = 0; i < length; i++)
                        d[i] = c;
                }
                return;
            mks:
                length = 0;
            }
            #endregion

            #region COPY TO
            public unsafe Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int destLen, int destW, int destX, int destY)
            {
                var data = color.Repeat(copyW * copyH + 1);
                var dst = (int*)destination;
                fixed (int* source = data)
                {
                    int* src = source;
                    return Blocks.CopyBlock(0, 0, copyW, copyH, data.Length, copyW, copyH, destX, destY, destW, destLen,
                         (srcIndex, dstIndex, w, x, y) =>
                         Blocks.Copy(src, srcIndex, dst, dstIndex, w));
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe Rectangle CopyTo(IWritable destination, int destX, int destY, int copyX, int copyY, int copyW, int copyH)
            {
                Rectangle destRc = Rectangle.Empty;
                Rectangle copy = new Rectangle(copyX, copyY, copyW, copyH);

                var x = copy.X;
                var r = x + copy.Width;
                var y = copy.Y;
                var b = y + copy.Height;
                copyW = copy.Width;
                copyH = copy.Height;

                if (y < 0)
                {
                    b += y;
                    y = 0;
                }
                int destLen = destination.Length;
                int destW = destination.Width;
                var dy = destY;
                var buffer = destination as ISurface;
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
                destRc = new Rectangle(destX, destY, copyW, i);
                if (destRc)
                    destination.Invalidate(destRc.X, destRc.Y, destRc.Width, destRc.Height, true);
                return destRc;
            }
            #endregion

            #region CONTAINS
            public bool Contains(int x, int y)
            {
                return x >= X && y >= Y && x <= R && y <= B;
            }
            #endregion

            #region CLONE
            public object Clone()
            {
                var pen = new Pen(Color);
                pen.X = X;
                pen.Y = Y;
                pen.w = w;
                pen.h = h;
                pen.R = R;
                pen.B = B;
                return pen;
            }
            #endregion

            #region COPY SETTINGS
            public void CopySettings(ISettable settings, bool flushMode)
            {
                if (settings == null) goto Flush;

                if (flushMode)
                    goto Flush;

                if (settings is IDrawInfo)
                {
                    var Settings = settings as IDrawInfo;
                    var bounds = Settings.Bounds;

                    X = bounds.X;
                    Y = bounds.Y;
                    w = bounds.Width;
                    h = bounds.Height;
                    R = bounds.Right;
                    B = bounds.Bottom;
                    Inversion = Settings.BrushCommand.HasFlag(BrushCommand.InvertColor);
                }
                return;
            Flush:
                X = Y = R = B = w = h = 0;
                Inversion = false;
            }
            #endregion
        }
#if AllHidden
    }
#endif
}
