/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if AllHidden
    partial class NativeFactory
    {
#else
    public
#endif
        sealed class Pen : IPen, IColor, ISettingsReceiver
    {
        #region VARIABLES
        int w, h;
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
                Pen pen = new Pen(color);
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
        public bool Invert
        {
            get => Inversion;
            set => Inversion = value;
        }
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
        public unsafe void ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length, out byte[] pixelAlphas)
        {
            pixels = new int[0];
            srcIndex = 0;
            pixelAlphas = null;
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
        public unsafe IRectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination,
            int dstLen, int dstW, int dstX, int dstY, Command command = 0, string shapeID = null)
        {
            var data = color.Repeat(copyW * copyH + 1);
            var dst = (int*)destination;
            fixed (int* source = data)
            {
                int* src = source;
                return Blocks.CopyBlock(0, 0, copyW, copyH, data.Length, copyW, copyH, dstX, dstY, dstW, dstLen,
                     (srcIndex, dstIndex, w, x, y, cmd) =>
                     Blocks.Copy(src, srcIndex, dst, dstIndex, w, cmd, null, true), command);
            }
        }
        #endregion

        #region CLONE
        public object Clone()
        {
            var pen = new Pen(Color);
            pen.w = w;
            pen.h = h;
            return pen;
        }
        #endregion

        #region COPY SETTINGS
        public void Receive(IDrawParams settings, bool flushMode)
        {
            if (settings == null) goto Flush;

            if (flushMode)
                goto Flush;
            if(settings is IBounds)
            {
                var Settings = ((IBounds)settings).Bounds;
                w = Settings.Width;
                h = Settings.Height;
            }
            else if (settings is ISize)
            {
                var Settings = ((ISize)settings);
                w = Settings.Width;
                h = Settings.Height;
            }
            return;
        Flush:
            w = h = 0;
        }
        #endregion
    }
#if AllHidden
    }
#endif
}
#endif
