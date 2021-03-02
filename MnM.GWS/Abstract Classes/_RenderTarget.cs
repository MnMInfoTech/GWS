using System;

namespace MnM.GWS
{
    public abstract partial class _RenderTarget: IRenderTarget
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
        protected IRenderWindow Window;

        protected bool isDisposed;

        /// <summary>
        /// Background pen for this target.
        /// </summary>
        IReadable BackgroundPen;

        /// <summary>
        /// Pixels of background pen.
        /// </summary>
        volatile int[] PenData;
        #endregion

        #region CONSTRUCTORS
        protected _RenderTarget(int w, int h) 
        {
            Initialize(w, h);
        }
        protected _RenderTarget(IRenderWindow window)
        {
            Window = window;
            Initialize(window.Width, Window.Height);
        }
        protected void Initialize(int w, int h)
        {
            width = w;
            height = h;
            length = width * height;
            PenData = new int[length];
            InitializeFurther();        }
        partial void InitializeFurther();
        #endregion

        #region PROPERTIES
        public abstract IntPtr Source { get; }
        public int Width => width;
        public int Height => height;
        public int Length => length;
        public IPenContext Background
        {
            set
            {
                ChangeBackground(value);
            }
        }
        public unsafe int[] BackgroundData => PenData;
        public bool IsDisposed => isDisposed;
        #endregion

        #region RESIZE
        public void Resize(int? newWidth = null, int? newHeight = null)
        {
            if ((newWidth == null && newHeight == null) ||
                (newWidth == width && newHeight == height))
                return;

            var w = newWidth ?? width;
            var h = newHeight ?? height;
            var oldWidth = width;
            var oldHeight = height;
            ResizeSource(w, h);
            ResizeFurther(oldWidth, oldHeight, w, h);

            if (BackgroundPen != null)
                (BackgroundPen as IResizable)?.Resize(w, h);

            ChangeBackground(BackgroundPen);
        }
        protected abstract void ResizeSource(int w, int h);
        partial void ResizeFurther(int oldWidth, int oldHeight, int newWidth, int newHeight);
        #endregion

        #region UPDATE
        public abstract void Update(ulong command = 0, IBoundable area = null);
        #endregion

        #region CLEAR
        public unsafe IBoundable Clear(IBoundable clear, ulong command = 0)
        {
            if (clear == null)
                return Perimeter.Empty;
            var Screen = (command & Command.ClearScreen) == Command.ClearScreen;

            if (Screen)
                return Blocks.CopyBlock(null, clear, length, width, height, (int*)Source, 0, 0, width, length, Command.Opaque);

            IBoundable boundable = null;
            ClearFurther(ref boundable, clear, command);
            return boundable;
        }
        unsafe partial void ClearFurther(ref IBoundable result, IBoundable clear, ulong command = 0);
        #endregion

        #region READ PIXEL
        public unsafe int ReadPixel(int x, int y, IReadSession session)
        {
            if (length == 0)
                return 0;
            int i = x + y * width;
            if (i >= length)
                i = 0;
            bool Invert = (session.Choice & Command.InvertColor) == Command.InvertColor;

            int srcColor = ((int*)Source)[i];
            if (Invert)
                srcColor ^= Colors.Inversion;
            return srcColor;
        }
        #endregion

        #region READ LINE
        public unsafe void ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length, IReadSession session)
        {
            if (start > end)
            {
                int temp = end;
                end = start;
                start = end;
            }
            length = end - start;
            if (start < 0)
            {
                length += start;
                start = 0;
            }
            int srcCounter = horizontal ? 1 : width;
            pixels = new int[length];
            int* dst;
            fixed (int* p = pixels)
                dst = p;
            bool Invert = (session.Choice & Command.InvertColor) == Command.InvertColor;
            int* src = ((int*)Source);
            srcIndex = start + axis * width;
            int srcColor;

            for (int i = 0; i < length; i++)
            {
                srcColor = src[srcIndex];
                if (Invert)
                    srcColor ^= Colors.Inversion;
                dst[i] = srcColor;
                srcIndex += srcCounter;
            }
            srcIndex = 0;
        }
        #endregion

        #region COPY TO
        public unsafe IBoundable CopyTo(IntPtr dest, int dstLen, int dstW, int dstX, int dstY, IBoundable copyArea, IReadSession readSession)
        {
            int length;
            int* dst = (int*)dest;
            copyArea.GetBounds(out int copyX, out int copyY, out int copyW, out int copyH);

            var x = copyX;
            var r = x + copyW;
            var y = copyY;
            var b = y + copyH;

            if (y < 0)
            {
                b += y;
                y = 0;
            }

            int destIndex = dstX + dstY * dstW;
            int i = 0;

            var session = readSession?? ReadSession.Empty;
            while (y < b)
            {
                ReadLine(x, r, y, true, out int[] source, out int srcIndex, out length, session);
                if (destIndex + length >= dstLen)
                    break;
                fixed (int* src = source)
                    Blocks.Copy(src, srcIndex, dst, destIndex, length, session.Choice, null);
                destIndex += dstW;
                ++i;
                ++y;
            }
            return new Rect(dstX, dstY, copyW, i);
        }
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            isDisposed = true;

            (BackgroundPen as IDisposable)?.Dispose();
            PenData = null;
            DisposeFurther();
        }
        partial void DisposeFurther();
        #endregion

        #region INVOKE PAINT
        public void InvokePaint(ulong command = 0, int processID = 0) =>
            Window.InvokePaint(command, processID);
        #endregion

        #region BACKGROUND CHANGED
        unsafe void ChangeBackground(IPenContext penContext)
        {
            PenData = new int[length];
            if (penContext == null)
            {
                (BackgroundPen as IDisposable)?.Dispose();
                BackgroundPen = null;
                return;
            }
            IReadable Pen;
            if (penContext is IReadable)
                Pen = (IReadable)penContext;
            else
                Pen = penContext.ToPen(width, height);
            fixed (int* p = PenData)
            {
                Pen.CopyTo((IntPtr)p, length, Width, 0, 0, new Rect(0, 0, width, height), ReadSession.Empty);
            }
            BackgroundPen = Pen;
        }
        #endregion
    }
}

