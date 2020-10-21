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
        bool locked;
        protected int width, height, length;
        protected bool isDisposed;
        bool AntiAliased;
        bool Distinct;
        bool Opaque, Back, Direct;
        IPen BkgPen;
        LineCommand lineCommand;
        DrawCommand drawCommand;

        readonly HashSet<int> DrawnIndices = new HashSet<int>();
        const byte o = 0;
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
            IntPtr textureData;
            int lockedLength;
            Lock(rc, out textureData, out lockedLength);
            source.CopyTo(rc.X, rc.Y, rc.Width, rc.Height, textureData, lockedLength, Width, 0, 0);
            Unlock();
        }
        #endregion

        #region PROPERTIES
        public string ID { get; private set; }
        public int Width => width;
        public int Height => height;
        public bool IsPrimary { get; private set; }
        public IntPtr Handle { get; private set; }
        public bool IsDisposed => Window.IsDisposed || isDisposed;
        public RendererFlags RendererFlags => Window.RendererFlags;
        public int Length => length;
        public IReadContext Background
        {
            get => BkgPen;
            set
            {
                if (value == null)
                {
                    (BkgPen as IDisposable)?.Dispose();
                    BkgPen = null;
                    return;
                }
                BkgPen = value.ToPen(Width, Height);
            }
        }
        public LineCommand LineCommand
        {
            get => lineCommand;
            set
            {
                lineCommand = value;
                AntiAliased = !LineCommand.HasFlag(LineCommand.Breshenham);
                var distinct = LineCommand.HasFlag(LineCommand.Distinct);
                if (distinct != !Distinct)
                    DrawnIndices.Clear();
                Distinct = distinct;
            }
        }
        public DrawCommand DrawCommand
        {
            get => drawCommand;
            set
            {
                drawCommand = value;
                Opaque = Back = Direct = false;
                Opaque = value.HasFlag(DrawCommand.Opaque);
                Back = !Opaque && value.HasFlag(DrawCommand.Back);
                Direct = value.HasFlag(DrawCommand.DirectOnScreen);
            }
        }
        bool IWritable.Antialiased => AntiAliased;
        #endregion

        #region COPY FROM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH, bool updateImmediate = true)
        {
            var dstRC = this.CompitibleRc(dstX, dstY, srcW, srcH);
            IntPtr textureData;
            int lockedLength;
            Lock(dstRC, out textureData, out lockedLength);
            source.CopyTo(srcX, srcY, dstRC.Width, dstRC.Height, textureData, lockedLength, Width, 0, 0);
            Unlock();
            if (updateImmediate)
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

        #region COPY TO RENDERER
        protected abstract void CopyToRenderer(IntPtr texture, Rectangle sourceRc, Rectangle destRc);
        #endregion

        #region WRITE PIXEL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WritePixel(int val, int axis, bool horizontal, int srcColor, float? Alpha)
        {
            if (srcColor == 0 && !Opaque)
                return;

            int i;
            int x = horizontal ? val : axis;
            int y = horizontal ? axis : val;

            if (x < 0 || y < 0 || x >= width || y >= height)
                return;


            IntPtr textureData;
            Lock(new Rectangle(x, y, 1, 1), out textureData, out _);

            i = x + y * width;
            int* Data = (int*)textureData;

            int dstColor = Data[0];

            if (Back && dstColor != 0)
                return;

            if (Distinct)
            {
                if (DrawnIndices.Contains(i))
                    return;
                DrawnIndices.Add(i);
            }
            byte alpha;

            float delta = Alpha ?? Colors.Alphas[(byte)((srcColor >> Colors.AShift) & 0xFF)];
            alpha = (byte)(delta * 255);

            if (alpha == 0)
                return;

            if (alpha != 255)
            {
                if (dstColor == 0)
                    dstColor = BkgPen?.ReadPixel(x, y) ?? 0;

                uint C1, C2, invAlpha, RB, AG;
                //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
                C1 = (uint)dstColor;
                C2 = (uint)srcColor;
                invAlpha = 255 - (uint)alpha;
                RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C2 & Colors.RBMASK))) >> 8;
                AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C2 & Colors.GMASK) >> 8)));
                srcColor = (int)((RB & Colors.RBMASK) | (AG & Colors.AGMASK));
            }
            Data[0] = srcColor;
        }
        #endregion

        #region WRITE LINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteLine(int* src, int srcIndex, int srcW, int copyLength, bool horizontal, int x, int y, float? Alpha)
        {
            #region VARAIBLE INITIALIZATION
            if (copyLength <= 0)
                return;
            IntPtr textureData;
            Lock(new Rectangle(x, y, horizontal ?  width : 1, horizontal ? 1 : height), out textureData, out _);

            int* dst = (int*) textureData;
            int dstIndex = 0;
            int dplus = 1;
            int splus = horizontal || srcW == copyLength ? 1 : srcW;
            int last = dstIndex + dplus * copyLength;
            int j = srcIndex;

            int px = x;
            int py = y;
            int ix = horizontal ? 1 : 0;
            int iy = horizontal ? 0 : 1;
            bool hasBkg = BkgPen != null;

            int dstColor, srcColor;
            var NoBlend = Alpha == null;
            byte alpha = !NoBlend ? (byte)(Alpha.Value * 255) : o;
            #endregion

            #region WRITING LINE
            for (int i = dstIndex; i < last; i += dplus, j += splus, px += ix, py += iy)
            {
                if (i >= length) break;

                dstColor = dst[i];
                srcColor = src[j];

                if (srcColor == 0 && dstColor == 0)
                    continue;

                if (srcColor == 0)
                {
                    if (Opaque)
                        dst[i] = srcColor;
                    continue;
                }

                if (Back && dstColor != 0)
                    continue;

                if (!NoBlend && alpha < 2)
                    continue;

                if (NoBlend || alpha == 255)
                {
                    dst[i] = srcColor;
                    continue;
                }

                if (dstColor == 0 && hasBkg)
                    dstColor = BkgPen.ReadPixel(px, py);

                uint C1, C2, invAlpha, RB, AG;
                //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
                C1 = (uint)dstColor;
                C2 = (uint)srcColor;
                invAlpha = 255 - (uint)alpha;
                RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C2 & Colors.RBMASK))) >> 8;
                AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C2 & Colors.GMASK) >> 8)));
                srcColor = (int)((RB & Colors.RBMASK) | (AG & Colors.AGMASK));
                dst[i] = srcColor;
            }
            #endregion
        }
        #endregion

        #region INVALIDATE
        public void Invalidate(int x, int y, int width, int height, bool updateImmediate = false)
        {
            CopyFrom(Window, x, y, x, y, width, height, updateImmediate);
        }
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            isDisposed = true;
            DestoryTextureHandle(Handle);
        }
        #endregion

        #region RESIZE
        public unsafe void Resize(int? width = null, int? height = null)
        {
            DestoryTextureHandle(Handle);
            Handle = CreateHandle(null, null, width ?? Width, height ?? Height, out Size s);
            this.width = s.Width;
            this.height = s.Height;
            CopyFrom(Window, 0, 0, 0, 0, Width, Height);
        }
        #endregion

        #region CREATE - DESTORY
        protected abstract IntPtr CreateHandle(uint? format, TextureAccess? access, int w, int h, out Size size);
        protected abstract void DestoryTextureHandle(IntPtr handle);
        #endregion
    }
#endif
}
