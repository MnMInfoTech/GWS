/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class _Surface : ISurface
    {
        #region VARIABLES
        /// <summary>
        /// Indicates if this object is a container of shapes or not. i.e it implements IContainer interface or not.
        /// </summary>
        protected readonly bool IsContainer;

        /// <summary>
        /// Indicates if this object is a canvas or not.
        /// </summary>
        protected readonly bool IsCanvas;

        /// <summary>
        /// Indicates if this object is currently being resized or not.
        /// </summary>
        protected volatile bool IsResizing;

        /// <summary>
        /// Gets or sets a flag to determine if a child control is being rendered or not.
        /// </summary>
        protected volatile bool IsDrawingChild;

        /// <summary>
        /// ID of the shape currently is being rendered.
        /// </summary>
        protected volatile string ShapeID;

        /// <summary>
        /// ID of this object.
        /// </summary>
        protected string id;

        protected int width, height, length;
        protected bool isDisposed;


        protected volatile IReadable BkgPen;
        protected volatile IRenderTarget Target;


        protected volatile int recentlyDrawnX = int.MaxValue, recentlyDrawnY = int.MaxValue;
        protected volatile int recentlyDrawnR = 0, recentlyDrawnB = 0;

        protected volatile int minX = int.MaxValue, minY = int.MaxValue;
        protected volatile int maxX = 0, maxY = 0;

        protected readonly HashSet<int> DrawnIndices = new HashSet<int>();

        protected const byte o = 0;

        protected const int Zero = 0xffffff;
        #endregion

        #region COSTRUCTORS
        public _Surface(int width, int height, string id = null)
        {
            this.width = width;
            this.height = height;
            length = width * height;
            this.id = id ?? "Surface".NewID();
            IsCanvas = this is ICanvas;
            IsContainer = this is IContainer;
        }
        public _Surface(IRenderTarget window) :
            this(window.Width, window.Height)
        {
            Target = window;
        }
        #endregion

        #region PROPERTIES
        public string ID => id;
        public int Width => width;
        public int Height => height;
        public int Length => length;
        public bool IsDisposed => isDisposed;
        public Rectangle InvalidatedArea
        {
            get
            {
                if (maxX == 0 || maxY == 0)
                    return Rectangle.Empty;

                int x = minX - Vectors.LOffset;
                int y = minY - Vectors.LOffset;
                int r = maxX + Vectors.LOffset;
                int b = maxY + Vectors.LOffset;
                return Rectangle.FromLTRB(x, y, r, b);
            }
        }
        public IPenContext Background
        {
            set
            {
                if (value == null)
                {
                    (BkgPen as IDisposable)?.Dispose();
                    BkgPen = null;
                    return;
                }
                BkgPen = value.ToPen(width, height);

                if (IsContainer)
                    Invalidate(0, 0, width, height);
                OnBackgroundChanged(Factory.EmptyArgs);
            }
        }
        public IReadable BackgroundPen => BkgPen;
        protected unsafe abstract int* pixels(bool ForegroundBuffer = true);
        protected unsafe abstract byte* alphas(bool ForegroundBuffer = true);

#if Advanced
        IReadable IWritable.Target => Target;
        public abstract Rectangle ClipRectangle { get; set; }
        public abstract bool Clipped { get; }
        public abstract ISelfDrawable Control { get; set; }

        unsafe int* IMixableBlock.Pixels(bool ForegroundBuffer) =>
            pixels(ForegroundBuffer);

        unsafe byte* IMixableBlock.AlphaValues(bool ForegroundBuffer) =>
            alphas(ForegroundBuffer);
#endif
        #endregion

        #region RENDER
        public abstract void Render(IRenderable Renderable, IContext anyContext = null);
        #endregion

        #region WRITE PIXEL
        public abstract void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, DrawCommand drawCommand);
        #endregion

        #region WRITE LINE
        public abstract unsafe void WriteLine(int* pixels, int srcIndex, int srcW, int length, bool horizontal,
            int x, int y, float? Alpha, byte* imageAlphas, DrawCommand drawCommand);
        #endregion

        #region COPY TO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Rectangle CopyTo(IBlockable block, int destX, int destY, int copyX, int copyY, int copyW, int copyH,
            DrawCommand command = 0)
        {
            if (block is IPixels)
            {
                return CopyTo(copyX, copyY, copyW, copyH, ((IPixels)block).Source, block.Length, block.Width, destX, destY, command);
            }

            if (!(block is IWritable))
                return Rectangle.Empty;

            var surface = (IWritable)block;
            var copy = Rects.CompitibleRc(width, height, copyX, copyY, copyW, copyH);

            Rectangle dstRc;
            var x = copy.X;
            var y = copy.Y;
            copyW = copy.Width;

            var b = y + copy.Height;
            if (y < 0)
            {
                b += y;
                y = 0;
            }
            int srcLen = length;
            int srcIndex = x + y * width;
            int srcW = width;
            var dy = destY;

            bool Foregroundbuffer = true;
#if Advanced
            bool SwapOrder = (command & DrawCommand.SwapZOrder) == DrawCommand.SwapZOrder;
            bool BackgroundBuffer = (command & DrawCommand.BackgroundBuffer) == DrawCommand.BackgroundBuffer;
            Foregroundbuffer = BackgroundBuffer && !SwapOrder || !BackgroundBuffer && SwapOrder;
#endif
            int* pixels = this.pixels(Foregroundbuffer);
            byte* alphas = this.alphas(Foregroundbuffer);

            for (int j = y; j <= b; j++)
            {
                surface.WriteLine(pixels, srcIndex, srcW, copyW, true, destX, dy++, null, alphas, command);
                srcIndex += srcW;
                if (srcIndex >= srcLen)
                    break;
            }
            dstRc = new Rectangle(destX, destY, copyW, dy - destY);

            if (dstRc && block is IUpdatable)
            {
                var updatable = (IUpdatable)surface;
                updatable.Invalidate(dstRc.X, dstRc.Y, dstRc.Width, dstRc.Height);
                updatable.Update(command);
            }
            return dstRc;
        }

        public abstract Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination,
            int dstLen, int dstW, int dstX, int dstY, DrawCommand command = DrawCommand.None);
        #endregion

        #region RESIZE
        public abstract void Resize(int? width = null, int? height = null);
        #endregion

        #region CLEAR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Clear(int x, int y, int width, int height, DrawCommand command = 0);
        #endregion

        #region INVALIDATE - UPDATE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Invalidate(int x, int y, int w, int h)
        {
            if (x < recentlyDrawnX)
                recentlyDrawnX = x;
            if (y < recentlyDrawnY)
                recentlyDrawnY = y;

            if (x + w > recentlyDrawnR)
                recentlyDrawnR = x + w;
            if (y + h > recentlyDrawnB)
                recentlyDrawnB = y + h;

            if (x < minX)
                minX = x;
            if (y < minY)
                minY = y;

            if (x + w > maxX)
                maxX = x + w;
            if (h + y > maxY)
                maxY = h + y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Update(DrawCommand command = 0)
        {
            bool SuspendUpdate = (command & DrawCommand.SuspendUpdate) == DrawCommand.SuspendUpdate;
            int x = minX - Vectors.LOffset;
            int y = minY - Vectors.LOffset;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            int r = maxX + Vectors.LOffset;
            int b = maxY + Vectors.LOffset;

            if (!SuspendUpdate)
            {
                minX = minY = int.MaxValue;
                maxX = maxY = 0;
            }

            if (r != 0 && b != 0)
            {
                Target?.Invalidate(x, y, r - x, b - y);
                if (!SuspendUpdate)
                    Target?.Update(command);
            }

            recentlyDrawnX = recentlyDrawnY = int.MaxValue;
            recentlyDrawnR = recentlyDrawnB = 0;
        }
        #endregion

        #region CLONE
        public object Clone()
        {
            var surface = EmptyInstance(width, height);
            surface.width = width;
            surface.height = height;

            if (BkgPen is ICloneable)
                surface.BkgPen = ((ICloneable)BkgPen).Clone() as IReadable;

            surface.Target = Target;
            surface.recentlyDrawnX = recentlyDrawnX;
            surface.recentlyDrawnY = recentlyDrawnY;
            surface.recentlyDrawnR = recentlyDrawnR;
            surface.recentlyDrawnB = recentlyDrawnB;
            CopyToClone(surface);
            return surface;
        }
        protected abstract _Surface EmptyInstance(int width, int height);
        protected virtual void CopyToClone(_Surface surface) { }
        #endregion

        #region BLEND
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int Blend(int dstColor, int srcColor, byte alpha, bool invert = false)
        {
            if (alpha == 255)
                return srcColor;

            //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
            uint C1 = (uint)dstColor;
            uint C2 = (uint)srcColor;
            uint invAlpha = 255 - (uint)alpha;
            uint RB = ((invAlpha * (C1 & GWS.Colors.RBMASK)) + (alpha * (C2 & GWS.Colors.RBMASK))) >> 8;
            uint AG = (invAlpha * ((C1 & GWS.Colors.AGMASK) >> 8)) + (alpha * (GWS.Colors.ONEALPHA | ((C2 & GWS.Colors.GMASK) >> 8)));
            int color = (int)((RB & GWS.Colors.RBMASK) | (AG & GWS.Colors.AGMASK));
            if (invert)
                color = color ^ 0xffffff;
            return color;
        }
        #endregion

        #region EVENTS
        protected virtual void OnBackgroundChanged(IEventArgs e)
        {
            Target?.Invalidate(0, 0, Width, Height);
            Target?.Update();
            BackgroundChanged?.Invoke(this, e);
        }
        public virtual event EventHandler<IEventArgs> BackgroundChanged;
        #endregion

        #region FIND ELEMENT
#if Advanced
        public abstract IRenderable FindElement(int x, int y);
#endif
        #endregion

        #region TO BRUSH
#if Advanced
        public abstract ITextureBrush ToBrush(Rectangle? copyArea = null);
#endif
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            isDisposed = true;
            (BkgPen as IDisposable)?.Dispose();
            BkgPen = null;
        }
        #endregion
    }
}
