/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if Window
    public abstract class _Texture : ITexture
    {
        #region VARIABLES
        protected readonly IRenderWindow Window;
        protected volatile int width, height, length;
        protected volatile bool Disposed;
        protected volatile bool locked;
        protected bool IsResizing;
        #endregion

        #region CONSTRUCTORS
        protected unsafe _Texture(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false,
            uint? pixelFormat = null, TextureAccess? textureAccess = null)
        {
            Window = window;
            IsPrimary = isPrimary;
            width = w ?? Window.Width;
            height = h ?? Window.Height;
            Handle = CreateHandle(pixelFormat, textureAccess, Width, Height, out Size s);
            width = s.Width;
            height = s.Height;
            length = width * height;
            ID = this.NewID();
#if Advanced
#endif
        }
        protected unsafe _Texture(IRenderWindow window, ICopyable source, bool isPrimary = false,
            uint? pixelFormat = null, TextureAccess? textureAccess = null) :
            this(window, source.Width, source.Height, isPrimary, pixelFormat, textureAccess)
        {
            var rc = this.CompitibleRc(0, 0, source.Width, source.Height);
            CopyFrom(source, 0, 0, rc.X, rc.Y, rc.Width, rc.Height, DrawCommand.SuspendUpdate);
        }
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
        #endregion

        #region COPY FROM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void CopyFrom(IBlockable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH, DrawCommand command = 0);
        #endregion
       
        #region BIND - UNBIND
        public abstract void Bind();
        public abstract void Unbind();
        #endregion

        #region UPLOAD
        public abstract void Upload(Rectangle area);
        #endregion

        #region RESIZE
        public unsafe void Resize(int? width = null, int? height = null)
        {
            DestoryTextureHandle(Handle);
            Handle = CreateHandle(null, null, width ?? Width, height ?? Height, out Size s);
            this.width = s.Width;
            this.height = s.Height;
            CopyFrom(Window, 0, 0, 0, 0, s.Width, s.Height);
        }
        #endregion

        #region CREATE - DESTORY
        protected abstract IntPtr CreateHandle(uint? format, TextureAccess? access, int w, int h, out Size size);
        protected abstract void DestoryTextureHandle(IntPtr handle);
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            Disposed = true;
            DestoryTextureHandle(Handle);
        }
        #endregion
    }
#endif
}
