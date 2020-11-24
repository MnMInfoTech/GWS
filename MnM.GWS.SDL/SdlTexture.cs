/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if Window
using System;
using System.Runtime.CompilerServices;

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
            const byte o = 0;
            #endregion

            #region CONSTRUCTORS
            public SdlTexture(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false, uint? pixelformat = null, TextureAccess? textureAccess = null) :
                base(window, w, h, isPrimary, pixelformat, textureAccess)
            { }
            public SdlTexture(IRenderWindow window, ICopyable source, bool isPrimary = false, uint? pixelformat = null, TextureAccess? textureAccess = null) :
                base(window, source, isPrimary, pixelformat, textureAccess)
            { }
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

            #region COPY FROM
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override unsafe void CopyFrom(IBlockable source, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, bool updateImmediate = true)
            {
                var dstRC = this.CompitibleRc(dstX, dstY, copyW, copyH);
                IntPtr textureData;
                int lockedLength;
                Lock(dstRC, out textureData, out lockedLength);
                if (source is ICopyable)
                {
                   ((ICopyable) source).CopyTo(copyX, copyY, dstRC.Width, dstRC.Height, textureData, lockedLength, Width, 0, 0);
                }
                else if(source is IPixels)
                {
                    int* src = (int*)(((IPixels)source).Source);
                    int* dst = (int*)textureData;
                    BlockCopy action = (srcIndex, dstIndex, copyLength, x, y) => Blocks.Copy(src, srcIndex, dst, dstIndex, copyLength);
                    Blocks.CopyBlock2(copyX, copyY, copyW, copyH, source.Length, source.Width, source.Height, 0, 0, width, lockedLength, action);
                }
                Unlock();
                if (updateImmediate)
                    Upload(dstRC);
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
                    SdlFactory.LockTexture(Handle, IntPtr.Zero, out textureData, out texturePitch);
                    lockedArea = new Rectangle(0, 0, Width, Height);
                }
                else
                {
                    lockedArea = Rects.CompitibleRc(Width, Height, copyRc.X, copyRc.Y, copyRc.Width, copyRc.Height);
                    if (copyRc.Width == 0 || copyRc.Height == 0)
                        return Rectangle.Empty;
                    var copyHandle = new Rectangle(copyRc).ToPtr();
                    SdlFactory.LockTexture(Handle, copyHandle, out textureData, out texturePitch);
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
                SdlFactory.UnlockTexture(Handle);
                locked = false;
            }
            #endregion

            #region BIND - UNBIND
            public sealed override void Bind() =>
                SdlFactory.SetRenderTarget(Renderer, Handle);
            public sealed override void Unbind() =>
                SdlFactory.SetRenderTarget(Renderer, IntPtr.Zero);
            #endregion

            #region UPLOAD
            public override void Upload(Rectangle area)
            {
                SdlFactory.RenderCopyTexture(Renderer, Handle, area, area);
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
                SdlFactory.QueryTexture(Handle, out f, out acc, out w, out h);
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
