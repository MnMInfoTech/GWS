/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    #region IARRAY<T>
    /// <summary>
    /// Interface IArray
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IArrayHolder<T> : ICount
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        T[] Data { get; }
    }
    #endregion

    public interface IArray<T>: ISize, ISizeHolder, ICount, IHandle, IDisposable
    {
        /// <summary>
        /// Clears data blocks covered by area specified by x, y, width and height parameters.
        /// </summary>
        /// <param name="dstX">Left most corner of region which is to be cleared.</param>
        /// <param name="dstY">Top most corner of region which is to be cleared.</param>
        /// <param name="width">Width of region which is to be cleared.</param>
        /// <param name="height">Height of region which is to be cleared.</param>
        void Clear(int dstX, int dstY, int width, int height);
    }
    public sealed class Array<T>: IArray<T>, IArrayHolder<T>, IExResizable 
    {
        #region VARIABLES
        GCHandle gcHandle;
        public T[] Data;
        public volatile int Width, Height, Length;
        public volatile IntPtr Handle;
        readonly int OriginalWidth, OriginalHeight;
        #endregion

        #region CONSTRUCTORS
        public Array(int w, int h)
        {
            Width = OriginalWidth = w;
            Height = OriginalHeight = h;
            Length = w * h;
            Data = new T[Length];
            gcHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
            Handle = Marshal.UnsafeAddrOfPinnedArrayElement(Data, 0);
        }
        ~Array()
        {
            if (gcHandle.IsAllocated)
                gcHandle.Free();
        }
        #endregion

        #region PROPERTIES
        int ISize.Width => Width;
        int ISize.Height => Height;
        T[] IArrayHolder<T>.Data => Data;
        int ICount.Count => Length;
        IntPtr IHandle.Handle => Handle;

        public Size Size
        {
            get => new Size(Width, Height);
            set
            {
                if (!value)
                    return;
                ((IExResizable)this).Resize(value.Width, value.Height, out _);
            }
        }
        #endregion

        #region RESIZE
        public object Resize(int w, int h, out bool success, ResizeCommand command = 0)
        {
            success = false;
            if ((w == 0 && h == 0) ||
                (w == Width && h == Height))
                return this;

            if (w > Vectors.UHD8kWidth)
                w = Vectors.UHD8kWidth;
            if (h > Vectors.UHD8kHeight)
                h = Vectors.UHD8kHeight;

            bool SizeOnlyToFit = (command & ResizeCommand.SizeOnlyToFit) ==  ResizeCommand.SizeOnlyToFit;
            bool AutoReSizeContent = !SizeOnlyToFit && (command & ResizeCommand.AutoReSizeContent) == ResizeCommand.AutoReSizeContent;
            bool NotLessThanOriginal = !SizeOnlyToFit && (command & ResizeCommand.NotLessThanOriginal) == ResizeCommand.NotLessThanOriginal;
            
            if (SizeOnlyToFit && Width > w && Height > h)
                return this;
            if (SizeOnlyToFit)
            {
                if (w < Width)
                    w = Width;
                if (h < Height)
                    h = Height;
            }
            if (NotLessThanOriginal)
            {
                if (w < OriginalWidth)
                    w = OriginalWidth;
                if (h < OriginalHeight)
                    h = OriginalHeight;
            }
            if (gcHandle.IsAllocated)
                gcHandle.Free();

            if (AutoReSizeContent && 
                Data is int[]  ||
                Data is byte[] ||
                Data is uint[]
            )
            {
                Interpolation interpolation = 0;
                if ((command & ResizeCommand.AutoReSizeContentBicubic) == ResizeCommand.AutoReSizeContentBicubic)
                {
                    interpolation = Interpolation.Bicubic;
                }
                else if ((command & ResizeCommand.AutoReSizeContentBilinear) == ResizeCommand.AutoReSizeContentBilinear)
                {
                    interpolation = Interpolation.Bilinear;
                }
                var t = Factory.ImageProcessor.RotateAndScale
                (
                    Handle, Width, Height, 0, 0, Width, Height, interpolation,
                    SizeOnlyToFit, null, new Scale(Width, Height, w, h)
                ).Result;

                w = t.Item1.Width;
                h = t.Item1.Height;
                Array.Clear(Data, 0, Data.Length);
                Width = w;
                Height = h;
                Length = Width * Height;
                Data = new T[Length];
                gcHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
                Handle = Marshal.UnsafeAddrOfPinnedArrayElement(Data, 0);
                Blocks.Copy(t.Item2, 0, Handle, 0, w * h);
            }
            else
            {
                Blocks.ResizedData(ref Data, w, h, Width, Height);
                Width = w;
                Height = h;
                gcHandle = GCHandle.Alloc(Data, GCHandleType.Pinned);
                Handle = Marshal.UnsafeAddrOfPinnedArrayElement(Data, 0);
                Length = Width * Height;
            }
            success = true;
            return this;
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(int dstX, int dstY, int width, int height)
        {
            int copyX = dstX;
            int copyY = dstY;
            int copyW = width;
            int copyH = height;
            T[] src = new T[copyW * copyH];
            Blocks.CopyBlock(src, new Rectangle(copyX, copyY, copyW, copyH), copyW, copyH, Data, dstX, dstY, Width);
        }
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            if (gcHandle.IsAllocated)
                gcHandle.Free();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
