/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if Window
using System;
using System.Runtime.CompilerServices;

using static MnM.GWS.Application;

namespace MnM.GWS
{
#if AllHidden
    partial class SdlFactory
    {
#else
    public
#endif
        class SdlTexture : _Texture, ITexture2
        {
            #region VARIABLES
            protected IntPtr Renderer;
            BlendMode mode;
            #endregion

            #region CONSTRUCTORS
            public SdlTexture(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false, uint? pixelformat = null, TextureAccess? textureAccess = null) :
                base(window, w, h, isPrimary, pixelformat, textureAccess)
            {  }
            public SdlTexture(IRenderWindow window, ICopyable source, bool isPrimary = false, uint? pixelformat = null, TextureAccess? textureAccess = null) :
                base(window, source, isPrimary, pixelformat, textureAccess)
            {  }
            #endregion

            #region PROPERTIES
            public BlendMode Mode
            {
                get => mode;
                set
                {
                    mode = value;
                    SdlFactory.SetTextureBlendMod(Handle, value);
                }
            }
            public Flip Flip { get; set; }

            public byte Alpha
            {
                get => SdlFactory.GetTextureAlpha(Handle);
                set => SdlFactory.SetTextureAlpha(Handle, value);
            }
            public int ColorMode
            {
                get => SdlFactory.GetTextureColorMod(Handle);
                set => SdlFactory.SetTextureColorMod(Handle, value);
            }
            #endregion

            #region LOCK - UNLOCK
            protected override void Lock(IntPtr texture, IntPtr rect, out IntPtr textureData, out int texturePitch)
            {
                SdlFactory.LockTexture(texture, rect, out textureData, out texturePitch);
            }
            protected override void Unlock(IntPtr texture)
            {
                SdlFactory.UnlockTexture(texture);
            }
            #endregion

            #region BIND - UNBIND
            public sealed override void Bind() =>
                SdlFactory.SetRenderTarget(Renderer, Handle);
            public sealed override void Unbind() =>
                SdlFactory.SetRenderTarget(Renderer, IntPtr.Zero);
            #endregion

            #region COPY TO RENDERER
            protected override void CopyToRenderer(IntPtr texture, Rectangle sourceRc,  Rectangle destRc)
            {
                SdlFactory.RenderCopyTexture(Renderer, texture, sourceRc, destRc);
                SdlFactory.UpdateRenderer(Renderer);
            }
            #endregion

            #region CREATE - DESTROY
            protected override IntPtr CreateHandle(uint? format, TextureAccess? access, int w, int h, out Size size)
            {
                Renderer = SdlFactory.GetRenderer(Window.Handle);
                if (Renderer == IntPtr.Zero)
                    Renderer = SdlFactory.CreateRenderer(Window.Handle, -1, Window.RendererFlags);
                var acc = access ?? TextureAccess.Streaming;
                var f = format ?? SdlFactory.pixelFormat;
                var handle = SdlFactory.CreateTexture(Renderer, f, acc, w, h);
                SdlFactory.QueryTexture(Handle, out f, out  acc , out w, out h);
                size = new Size(w, h);
                SdlFactory.SetTextureBlendMod(handle, BlendMode.None);
                return handle;
            }
            protected override void DestoryTextureHandle(IntPtr handle)
            {
                if (handle != IntPtr.Zero)
                    SdlFactory.DestroyTexture(handle);
            }
            #endregion
        }
#if AllHidden
    }
#endif
}
#endif
