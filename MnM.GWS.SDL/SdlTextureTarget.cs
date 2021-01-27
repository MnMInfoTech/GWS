using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MnM.GWS.SDL
{
    class SdlTextureTarget : ITextureTarget
    {
        #region VARIABLES
        protected IntPtr Renderer;
        protected readonly IRenderWindow Window;
        protected volatile int width, height, length;
        protected volatile bool Disposed;
        protected volatile bool locked;
        protected bool IsResizing;
        protected volatile int[] Data;
        const byte o = 0;
        #endregion

        #region CONSTRUCTORS
        public SdlTextureTarget(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false,
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
            Data = new int[length];
            ID = this.NewID();
        }
        public SdlTextureTarget(IRenderWindow window, ICopyable source, bool isPrimary = false,
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
        public unsafe IntPtr Source => (IntPtr)Screen;
        unsafe int* Screen
        {
            get
            {
                fixed (int* p = Data)
                    return p;
            }
        }
        public bool IsDisposed =>
            Window.IsDisposed || Disposed;
        public RendererFlags RendererFlags =>
            Window.RendererFlags;
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
                Update(0, dstRC);
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

            if (!copyRc)
            {
                NativeFactory.LockTexture(Handle, IntPtr.Zero, out textureData, out texturePitch);
                lockedArea = new Rectangle(0, 0, Width, Height);
            }
            else
            {
                lockedArea = Rects.CompitibleRc(Width, Height, copyRc.X, copyRc.Y, copyRc.Width, copyRc.Height);
                if (copyRc.Width == 0 || copyRc.Height == 0)
                    return Rectangle.Empty;
                NativeFactory.LockTexture(Handle, copyRc, out textureData, out texturePitch);
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

        #region UPDATE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Update(Command command = Command.None, IRectangle boundary = null)
        {
            Rectangle rc;
            if (boundary is IBoundary)
                rc = new Rectangle(((IBoundary)boundary).GetBounds(6, 6));
            else
                rc = new Rectangle(boundary);

            NativeFactory.UpdateTexture(Handle, IntPtr.Zero, Source, Width * 4);

            NativeFactory.RenderCopyTexture(Renderer, Handle, rc, rc);
            NativeFactory.UpdateRenderer(Renderer);
        }
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
