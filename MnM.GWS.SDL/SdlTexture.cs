/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if Window
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS.SDL
{
    class SdlTexture : ITexture, ITexture2
    {
        #region VARIABLES
        protected IntPtr Renderer;
        protected readonly IRenderWindow Window;
        protected volatile int width, height, length;
        protected volatile bool Disposed;
        protected volatile bool locked;
        protected bool IsResizing;
        BlendMode mode;
        const byte o = 0;
        #endregion

        #region CONSTRUCTORS
        public SdlTexture(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false,
            uint? pixelformat = null, TextureAccess? textureAccess = null)
        {
            Window = window;
            IsPrimary = isPrimary;
            width = w ?? Window.Width;
            height = h ?? Window.Height;
            Handle = CreateHandle(pixelformat ?? NativeFactory.pixelFormat, textureAccess, Width, Height, out Size s);
            width = s.Width;
            height = s.Height;
            length = width * height;
            ID = this.NewID();
        }
        public SdlTexture(IRenderWindow window, ICopyable source, bool isPrimary = false,
            uint? pixelformat = null, TextureAccess? textureAccess = null) :
            this(window, null, null, isPrimary, pixelformat, textureAccess)
        { }
        #endregion

        #region PROPERTIES
        public string ID { get; private set; }
        public bool IsPrimary { get; private set; }
        public IntPtr Handle { get; private set; }

        public int Width =>
            width;
        public int Height =>
            height;
        public int Length =>
            length;
        public bool IsDisposed =>
            Window.IsDisposed || Disposed;
        public RendererFlags RendererFlags =>
            Window.RendererFlags;

        public BlendMode Mode
        {
            get => mode;
            set
            {
                mode = value;
                NativeFactory.SetTextureBlendMod(Handle, value);
            }
        }
        public FlipMode Flip { get; set; }

        public byte Alpha
        {
            get => NativeFactory.GetTextureAlpha(Handle);
            set => NativeFactory.SetTextureAlpha(Handle, value);
        }
        public int ColorMode
        {
            get => NativeFactory.GetTextureColorMod(Handle);
            set => NativeFactory.SetTextureColorMod(Handle, value);
        }
        #endregion

        #region COPY FROM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void CopyFrom(IBlockable source, int dstX, int dstY, IRectangle copyArea, Command command)
        {
            int copyX = copyArea.X;
            int copyY = copyArea.Y;
            int copyW = copyArea.Width;
            int copyH = copyArea.Height;

            var dstRC = this.CompitibleRc(dstX, dstY, copyW, copyH);
            IntPtr textureData;
            int lockedLength;
            Lock(dstRC, out textureData, out lockedLength);
            if (source is ICopyable)
            {
                ((ICopyable)source).CopyTo(textureData, lockedLength, Width, 0, 0, new Rectangle(copyX, copyY, dstRC.Width, dstRC.Height), 0);
            }
            else if (source is IPixels)
            {
                int* src = (int*)(((IPixels)source).Source);
                int* dst = (int*)textureData;
                BlockCopy action = (srcIndex, dstIndex, copyLength, x, y, cmd) =>
                Blocks.Copy(src, srcIndex, dst, dstIndex, copyLength, cmd);
                Blocks.CopyBlock(copyX, copyY, copyW, copyH, source.Length, source.Width, source.Height, 0, 0, width, lockedLength, action, command);
            }
            Unlock();
            if ((command & Command.SuspendUpdate) != Command.SuspendUpdate)
                Upload(dstRC);
        }
        #endregion

        #region RESIZE
        public unsafe void Resize(int? width = null, int? height = null)
        {
            DestoryTextureHandle(Handle);
            Handle = CreateHandle(null, null, width ?? Width, height ?? Height, out Size s);
            this.width = s.Width;
            this.height = s.Height;
            CopyFrom(Window, 0, 0, new Rectangle(0, 0, s.Width, s.Height), 0);
        }
        #endregion

        #region LOCK - UNLOCK
        Rectangle Lock(Rectangle copyRc, out IntPtr textureData, out int lockedLength)
        {
            if (locked)
                Unlock();

            textureData = IntPtr.Zero;
            lockedLength = 0;
            int texturePitch;
            Rectangle lockedArea;

            if (copyRc == null)
            {
                NativeFactory.LockTexture(Handle, IntPtr.Zero, out textureData, out texturePitch);
                lockedArea = new Rectangle(0, 0, Width, Height);
            }
            else
            {
                lockedArea = Rects.CompitibleRc(Width, Height, copyRc.X, copyRc.Y, copyRc.Width, copyRc.Height);
                if (copyRc.Width == 0 || copyRc.Height == 0)
                    return Rectangle.Empty;
                var copyHandle = new Rectangle(copyRc).ToPtr();
                NativeFactory.LockTexture(Handle, copyHandle, out textureData, out texturePitch);
                copyHandle.FreePtr();
            }
            locked = true;
            lockedLength = lockedArea.Height * texturePitch;
            return lockedArea;
        }
        void Unlock()
        {
            if (!locked)
                return;
            NativeFactory.UnlockTexture(Handle);
            locked = false;
        }
        #endregion

        #region BIND - UNBIND
        public void Bind() =>
            NativeFactory.SetRenderTarget(Renderer, Handle);
        public void Unbind() =>
            NativeFactory.SetRenderTarget(Renderer, IntPtr.Zero);
        #endregion

        #region UPLOAD
        public void Upload(Rectangle area)
        {
            NativeFactory.RenderCopyTexture(Renderer, Handle, area, area);
            NativeFactory.UpdateRenderer(Renderer);
        }
        #endregion

        #region CREATE - DESTROY
        protected IntPtr CreateHandle(uint? format, TextureAccess? access, int w, int h, out Size size)
        {
            Renderer = NativeFactory.GetRenderer(Window.Handle);
            if (Renderer == IntPtr.Zero)
                Renderer = NativeFactory.CreateRenderer(Window.Handle, -1, Window.RendererFlags);
            var acc = access ?? TextureAccess.Streaming;
            var f = format ?? NativeFactory.pixelFormat;
            var handle = NativeFactory.CreateTexture(Renderer, f, acc, w, h);
            NativeFactory.QueryTexture(Handle, out f, out acc, out w, out h);
            size = new Size(w, h);
            NativeFactory.SetTextureBlendMod(handle, BlendMode.None);
            return handle;
        }
        protected void DestoryTextureHandle(IntPtr handle)
        {
            if (handle != IntPtr.Zero)
                NativeFactory.DestroyTexture(handle);
        }
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            Disposed = true;
            DestoryTextureHandle(Handle);
        }
        #endregion
    }
}
#endif
