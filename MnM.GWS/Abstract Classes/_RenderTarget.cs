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
                Pen.CopyTo((IntPtr)p, length, Width, 0, 0, new Rect(0, 0, width, height), Command.Opaque);
            BackgroundPen = Pen;
        }
        #endregion
    }
}

