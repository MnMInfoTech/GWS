/* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if Window
using System;
using System.Runtime.CompilerServices;

using MnM.GWS.SDL;

namespace MnM.GWS
{
#if HideSdlObjects
    partial class NativeFactory
    {
#else
    public
#endif
    class SdlWindowSurface : IRenderTarget
    {
        #region VARIABLES
        /// <summary>
        /// Width of this object.
        /// </summary>
        protected int width;

        /// <summary>
        /// Height of this object
        /// </summary>
        protected int height;

        /// <summary>
        /// Length of one dimensional memory block this object represents.
        /// </summary>
        protected int length;

        /// <summary>
        /// Render Window - attached to this target.
        /// </summary>
        readonly IRenderWindow Window;

        /// <summary>
        /// 
        /// </summary>
        IntPtr Surface;
        #endregion

        #region CONSTRUCTORS
        public SdlWindowSurface(IRenderWindow window)
        {
            Window = window;
            this.Surface = GetHandle(Window.Handle);
            ID = "Target".NewName();
            width = window.Width;
            height = window.Height;
            length = width * height;

            Source = Surface.ToObj<SdlSurfaceInfo>().Pixels;
        }
        #endregion

        #region PROPERTIES
        public unsafe IntPtr Source { get; private set; }
        public int Width => width;
        public int Height => height;
        public int Length => length;
        public bool IsDisposed { get; private set; }
        public string ID { get; private set; }
        unsafe int* Screen => (int*)Source;
        #endregion

        #region GET HANDLE
        IntPtr GetHandle(IntPtr handle) =>
        NativeFactory.GetWindowSurface(handle);
        #endregion

        #region COPY FROM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe IRectangle CopyFrom(IntPtr source, int srcW, int srcH, int dstX, int dstY, IRectangle copyArea,
            Command Command, IntPtr alphaBytes = default(IntPtr))
        {
            if (IsDisposed)
                return Rectangle.Empty;
            int copyX = copyArea.X;
            int copyY = copyArea.Y;
            int copyW = copyArea.Width;
            int copyH = copyArea.Height;

            var dstRc = Blocks.CopyBlock((int*)source, copyX, copyY, copyW, copyH, srcW * srcH, srcW,
                srcH, Screen, dstX, dstY, Width, Length, Command, (byte*)alphaBytes);

            Update(Command, dstRc);
            return dstRc;
        }
        #endregion

        #region COPY TO       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe IRectangle CopyTo(IntPtr destination, int dstLen, int dstW, int dstX, int dstY, IRectangle copyArea, Command Command = 0)
        {
            if (IsDisposed)
                return Rectangle.Empty;
            int copyX = copyArea.X;
            int copyY = copyArea.Y;
            int copyW = copyArea.Width;
            int copyH = copyArea.Height;

            return Blocks.CopyBlock(Screen, copyX, copyY, copyW, copyH, length, width, height,
                (int*)destination, dstX, dstY, dstW, dstLen, Command, null);
        }
        #endregion

        #region CLEAR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe IRectangle Clear(int clearX, int clearY, int clearW, int clearH, Command Command = 0)
        {
            bool SuspendUpdate = (Command & Command.SuspendUpdate) == Command.SuspendUpdate;
            var rc = Blocks.CopyBlock(null, clearX, clearY, clearW, clearH, Length,
                Width, Height, Screen, clearX, clearY, Width, Length, Command, null);

            if (!SuspendUpdate)
                Update(0, rc);
            return rc;
        }
        #endregion

        #region UPDATE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Update(Command Command, IRectangle RecentlyDrawn = null)
        {
            if (RecentlyDrawn == null || !RecentlyDrawn.Valid)
                return;
            
            Rectangle rc;
            if (RecentlyDrawn is IBoundary)
                rc = new Rectangle(((IBoundary)RecentlyDrawn).GetBounds(6, 6));
            else
                rc = new Rectangle(RecentlyDrawn);

            NativeFactory.UpdateWindow(Window.Handle, rc);
        }
        #endregion

        #region RESIZE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Resize(int? newWidth = null, int? newHeight = null)
        {
            if ((newWidth == null && newHeight == null) ||
                (newWidth == width && newHeight == height))
                return;

            Surface = NativeFactory.GetWindowSurface(Window.Handle);
            SdlSurfaceInfo sdlSurface = Surface.ToObj<SdlSurfaceInfo>();
            Source = sdlSurface.Pixels;

            width = sdlSurface.Width;
            height = sdlSurface.Height;
            length = width * height;
            if (Window == null)
                return;
            var rc = Window.CopyTo(Source, length, width, 0, 0, new Rectangle(0, 0, width, height), Command.Backdrop);
            NativeFactory.UpdateWindow(Window.Handle, new Rectangle(rc.X, rc.Y, rc.Width, rc.Height));
        }
        #endregion

        #region DISPOSE
        public void Dispose()
        {
        }
        #endregion
    }

#if HideSdlObjects
    }
#endif
}
#endif
