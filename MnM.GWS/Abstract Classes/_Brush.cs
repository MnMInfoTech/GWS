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
        public abstract unsafe void ReadLine(int start, int end, int axis, bool horizontal, out int* src, out int srcIndex, out int copyLength);
        #endregion

        #region COPY TO
        public unsafe Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr dest, int destLen, int destW, int destX, int destY, DrawCommand command)
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
                ReadLine(x, r, y, true, out int* src, out int srcIndex, out length);
                if (destIndex + length >= destLen)
                    break;
                Blocks.Copy(src, srcIndex, dst, destIndex, length);
                destIndex += destW;
                ++i;
                ++y;
            }
            return new Rectangle(destX, destY, copyW, i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Rectangle CopyTo(IBlockable block, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, DrawCommand command)
        {
            if (block is IPixels)
            {
                return CopyTo(copyX, copyY, copyW, copyH, ((IImage)block).Source, block.Length, block.Width, dstX, dstY, command);
            }

            if (!(block is IWritable))
                return Rectangle.Empty;

            var surface = (IWritable)block;
            Rectangle dstRc = Rectangle.Empty;
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
            int destLen = surface.Length;
            var dy = dstY;
            int srcIndex, copylen;

            int i = 0;
            while (y < b)
            {
                ReadLine(x, r, y, true, out int* src, out srcIndex, out copylen);
                surface.WriteLine(src, srcIndex, copylen, copylen, true, dstX, dy++, null, null, command);
                ++i;
                ++y;
            }
            dstRc = Rectangle.FromLTRB(dstX, dstY, r, dy);
            if (dstRc && block is IUpdatable)
            {
                var updatable = (IUpdatable)surface;
                updatable.Invalidate(dstRc.X, dstRc.Y, dstRc.Width, dstRc.Height);
                updatable.Update(command);
            }
            return dstRc;
        }
        #endregion

        #region RESIZE
        public abstract void Resize(int? width = null, int? height = null);
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
