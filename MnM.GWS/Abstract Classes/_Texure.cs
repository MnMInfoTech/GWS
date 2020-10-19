/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if Window
    public abstract class _Texture : ITexture
    {
        #region VARIABLES
        protected readonly IRenderWindow Window;
        bool locked;
        #endregion

        #region CONSTRUCTORS
        protected unsafe _Texture(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false, uint? pixelFormat = null, TextureAccess? textureAccess = null)
        {
            Window = window;
            IsPrimary = isPrimary;
            Width = w ?? Window.Width;
            Height = h ?? Window.Height;
            Handle = CreateHandle(pixelFormat, textureAccess, Width, Height, out Size s);
            Width = s.Width;
            Height = s.Height;
            ID = this.NewID();
        }
        protected unsafe _Texture(IRenderWindow window, ICopyable source, bool isPrimary = false, uint? pixelFormat = null, TextureAccess? textureAccess = null) :
            this(window, source.Width, source.Height, isPrimary, pixelFormat, textureAccess)
        {
            var rc = this.CompitibleRc(0, 0, source.Width, source.Height);
            IntPtr textureData;
            int lockedLength;
            Lock(rc, out textureData, out lockedLength);
            source.CopyTo(rc.X, rc.Y, rc.Width, rc.Height, textureData, lockedLength, Width, 0, 0);
            Unlock();
        }
        #endregion

        #region PROPERTIES
        public string ID { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsPrimary { get; private set; }
        public int Length => Width * Height;
        public IntPtr Handle { get; private set; }
        public bool IsDisposed { get; private set; }
        public RendererFlags RendererFlags => Window.RendererFlags;
        #endregion

        #region UPLOAD
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(ICopyable source, int srcX, int srcY, int srcW, int srcH) =>
            CopyFrom(source, srcX, srcY, srcX, srcY, srcW, srcH);
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH)
        {
            var dstRC = this.CompitibleRc(dstX, dstY, srcW, srcH);
            IntPtr textureData;
            int lockedLength;
            Lock(dstRC, out textureData, out lockedLength);
            source.CopyTo(srcX, srcY, dstRC.Width, dstRC.Height, textureData, lockedLength, Width, 0, 0);
            Unlock();
            CopyToRenderer(Handle, dstRC, dstRC);
        }
        #endregion

        #region LOCK - UNLOCK
        public Rectangle Lock(Rectangle copyRc, out IntPtr textureData, out int lockedLength)
        {
            if (locked)
                Unlock();

            textureData = IntPtr.Zero;
            lockedLength = 0;
            int texturePitch;
            Rectangle lockedArea;

            if (copyRc == null)
            {
                Lock(Handle, IntPtr.Zero, out textureData, out texturePitch);
                lockedArea = new Rectangle(0, 0, Width, Height);
            }
            else
            {
                lockedArea = Rects.CompitibleRc(Width, Height, copyRc.X, copyRc.Y, copyRc.Width, copyRc.Height);
                if (copyRc.Width == 0 || copyRc.Height == 0)
                    return Rectangle.Empty;
                var copyHandle = new Rectangle(copyRc).ToPtr();
                Lock(Handle, copyHandle, out textureData, out texturePitch);
                copyHandle.FreePtr();
            }
            locked = true;
            lockedLength = lockedArea.Height * texturePitch;
            return lockedArea;
        }
        public void Unlock()
        {
            if (!locked)
                return;
            Unlock(Handle);
            locked = false;
        }

        protected abstract void Lock(IntPtr texture, IntPtr copyRc, out IntPtr textureData, out int texturePitch);
        protected abstract void Unlock(IntPtr texture);
        #endregion

        #region BIND - UNBIND
        public abstract void Bind();
        public abstract void Unbind();
        #endregion

        #region COPY TO
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public unsafe Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr dest, int destLen, int destW, int destX, int destY)
        //{
        //    Rectangle copyRc = this.CompitibleRc(copyX, copyY, copyW, copyH);
        //    Lock(copyRc, out IntPtr textureData, out int lockedLength);
        //    var src = (int*)textureData;
        //    var dst = (int*)dest;
        //    copyW = copyRc.Width;
        //    copyH = copyRc.Height;

        //    var result = Blocks.CopyBlock(0,0,copyW, copyH, lockedLength, Width, Height,
        //        destX, destY, destW, destLen, (srcIndex, dstIndex, w, x, y) =>
        //    Blocks.Copy(src, srcIndex, dst, dstIndex, w));

        //    Unlock();
        //    return result;
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public unsafe Rectangle CopyTo(IWritable destination, int destX, int destY, int copyX, int copyY, int copyW, int copyH)
        //{
        //    Rectangle destRc = Rectangle.Empty;
        //    Rectangle copy = Rects.CompitibleRc(Width, Height, copyX, copyY, copyW, copyH);

        //    int destLen = destination.Length;
        //    int destW = destination.Width;
        //    var dy = destY;
        //    var x = copy.X;
        //    var r = x + copy.Width;
        //    var y = copy.Y;
        //    var b = y + copy.Height;
        //    copyW = copy.Width;
        //    copyH = copy.Height;

        //    if (y < 0)
        //    {
        //        b += y;
        //        y = 0;
        //    }

        //    int[] array = new int[copy.Width * copy.Height];
        //    int srcLen = array.Length;
        //    int srcIndex = 0;
        //    int copylen = copy.Width;

        //    fixed (int* src = array)
        //    {
        //        CopyTo(x, y, r - x, b - y, (IntPtr)src, srcLen, copylen, 0, 0);
        //        for (int j = y; j <= b; j++)
        //        {
        //            destination.WriteLine(src, srcIndex, copylen, copylen, true, destX, dy++, null);
        //            srcIndex += copylen;
        //            if (srcIndex >= srcLen)
        //                break;
        //        }
        //    }
        //    destRc = new Rectangle(destX, destY, copyW, dy - destY);
        //    destination.Invalidate(destRc.X, destRc.Y, destRc.Width, destRc.Height, true);
        //    return destRc;
        //}
        protected abstract void CopyToRenderer(IntPtr texture, Rectangle sourceRc, Rectangle destRc);
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            IsDisposed = true;
           
            DestoryTextureHandle(Handle);
        }
        #endregion

        #region RESIZE
        public unsafe void Resize(int? width = null, int? height = null)
        {
            DestoryTextureHandle(Handle);
            Handle = CreateHandle(null, null, width ?? Width, height ?? Height, out Size s);
            Width = s.Width;
            Height = s.Height;
            CopyFrom(Window, 0, 0, Width, Height);
        }
        #endregion

        #region CREATE - DESTORY
        protected abstract IntPtr CreateHandle(uint? format, TextureAccess? access, int w, int h, out Size size);
        protected abstract void DestoryTextureHandle(IntPtr handle);
        #endregion
    }
#endif
}
