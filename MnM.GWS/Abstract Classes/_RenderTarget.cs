using System;

namespace MnM.GWS
{
    public abstract class _RenderTarget: IRenderTarget
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

#if Advanced
        /// <summary>
        /// this array of byte will be used by Canvas object for direct screen.
        /// </summary>
        volatile byte[] flags;
#endif
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
#if Advanced
            flags = new byte[length];
#endif
        }
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
        public unsafe int[] Pen => PenData;
        public bool IsDisposed => isDisposed;

#if Advanced
        public unsafe IntPtr Flags
        {
            get
            {
                fixed (byte* b = flags)
                    return (IntPtr)b;
            }
        }
#endif
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
            if (oldWidth == width && oldHeight == height)
                return;
            if (BackgroundPen != null)
                (BackgroundPen as IResizable)?.Resize(w, h);
            ChangeBackground(BackgroundPen);

#if Advanced
            flags = flags.ResizedData(width, height, oldWidth, oldHeight);
#endif
        }
        protected abstract void ResizeSource(int w, int h);
        #endregion

        #region UPDATE
        public abstract void Update(ulong command = 0, IBoundable area = null);
        #endregion

        #region CLEAR
        public unsafe IBoundable Clear(IBoundable clearArea, ulong command = 0)
        {
            if (clearArea == null)
                return Perimeter.Empty;
            Perimeter perimeter = new Perimeter(clearArea);
            var Screen = (command & Command.ClearScreen) == Command.ClearScreen;
            bool RestorePen = (command & Command.WipeAnimation) == Command.WipeAnimation;
            bool ClearBackground = (command & Command.SkipBackground) != Command.SkipBackground;

            if (Screen)
            {
                return Blocks.CopyBlock(null, perimeter, length, width, height, (int*)Source, 0, 0, width, length, Command.Opaque);
            }
            if (RestorePen)
            {
                fixed (int* p = PenData)
                {
                    if (BackgroundPen != null)
                        return BackgroundPen.CopyTo((IntPtr)p, length, width, perimeter.X, perimeter.Y, perimeter, Command.Opaque);
                    else
                        return Blocks.CopyBlock(null, perimeter, length, width, height, p, 0, 0, width, length, Command.Opaque);
                }
            }
            return perimeter;
        }
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            isDisposed = true;

            (BackgroundPen as IDisposable)?.Dispose();
            PenData = null;
#if Advanced
            flags = null;
#endif
        }
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

