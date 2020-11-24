/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    public sealed class Array<T> : ISize, ILength, IResizable
    {
        #region VARIABLES
        GCHandle handle;
        public volatile T[] Data;
        public volatile int Width, Height, Length;
        public volatile IntPtr Handle;
        #endregion

        #region CONSTRUCTORS
        public Array(int w, int h)
        {
            Data = new T[w * h];
            handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
            Handle = Marshal.UnsafeAddrOfPinnedArrayElement(Data, 0);
            Width = w;
            Height = h;
            Length = w * h;
        }
        ~Array()
        {
            if (handle.IsAllocated)
                handle.Free();
        }
        #endregion

        #region PROPERTIES
        int ILength.Length => Length;
        int ISize.Width => Width;
        int ISize.Height => Height;
        #endregion

        #region RESIZE
        public void Resize(int? width = null, int? height = null)
        {
            if ((width == null && height == null) ||
                (width == Width && height == Height))
                return;
            if (handle.IsAllocated)
                handle.Free();

            var w = width ?? Width;
            var h = height ?? Height;
            var ow = Width;
            var oh = Height;
            Data = Data.ResizedData(w, h, ref ow, ref oh);
            Width = ow;
            Height = oh;
            handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
            Handle = Marshal.UnsafeAddrOfPinnedArrayElement(Data, 0);
            Length = Width * Height;
        }
        #endregion

        #region CLEAR
        /// <summary>
        /// Clears data blocks covered by area specified by x, y, width and height paramters.
        /// </summary>
        /// <param name="dstX">Left most corner of region which is to be cleared.</param>
        /// <param name="dstY">Top most corner of region which is to be cleared.</param>
        /// <param name="width">Width of region which is to be cleared.</param>
        /// <param name="height">Height of region which is to be cleared.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rectangle Clear(int dstX, int dstY, int width, int height)
        {
            var copy = this.CompitibleRc(dstX, dstY, width, height);
            T[] src = new T[copy.Width * copy.Height];
            int copyX = copy.X;
            int copyY = copy.Y;
            int copyW = copy.Width;
            int copyH = copy.Height;

            BlockCopy action = (srcIndex, dstIndex, copyLength, x, y) => Array.Copy(src, srcIndex, Data, dstIndex, copyLength);
            var dstRc = Blocks.CopyBlock(copyX, copyY, copyW, copyH, src.Length, copyW, copyH, dstX, dstY, Width, Length, action);
            return new Rectangle(dstX, dstY, copy.Width, copy.Height);
        }
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            if (handle.IsAllocated)
                handle.Free();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
