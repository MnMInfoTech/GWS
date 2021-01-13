/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if GWS || Window
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class _Brush : IBrush
    {
        #region  VARIABLES
        protected int width, height, length, type;
        protected BrushStyle Style;
        protected string id;
        protected bool InvertBrushColor;
#if Advanced
       protected bool IsResizing;
#endif
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
        public bool Invert
        {
            get => InvertBrushColor;
            set => InvertBrushColor = value;
        }
        #endregion

        #region GET INDEX
        protected abstract int GetIndexUserDefined(int x, int y);
        #endregion

        #region READ PIXEL
        public abstract int ReadPixel(int x, int y);
        #endregion

        #region READ LINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract unsafe void ReadLine(int Start, int End, int Axis, bool Horizontal, out int[] pixels, out int srcIndex, out int copyLength, out byte[] srcAlphas);
        #endregion

        #region COPY TO
        public unsafe IRectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr dest, int dstLen,
            int dstW, int dstX, int dstY, Command command = Command.Opaque, string shapeID = null)
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

            int destIndex = dstX + dstY * dstW;
            int i = 0;
            while (y < b)
            {
                ReadLine(x, r, y, true, out int[] source, out int srcIndex, out length, out _);
                if (destIndex + length >= dstLen)
                    break;
                fixed (int* src = source)
                    Blocks.Copy(src, srcIndex, dst, destIndex, length, command, null, true);
                destIndex += dstW;
                ++i;
                ++y;
            }
            return new Rectangle(dstX, dstY, copyW, i);
        }
        #endregion

        #region RESIZE
        public abstract void Resize(int? width = null, int? height = null);
        #endregion

        #region COPY SETTINGS
        public abstract void Receive(IDrawParams settings, bool flushMode = false);
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
            block.Style = Style.Clone();
            block.id = id;
            block.width = width;
            block.height = height;
            block.length = length;
        }
        protected abstract _Brush emptyInstance();
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            IsDisposed = true;
            width = height = length = 0;
        }
        #endregion
    }
}
#endif
