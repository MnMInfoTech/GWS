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

        /// <summary>
        /// Background pen flags.
        /// </summary>
        volatile byte[] bkgFlags;

        /// <summary>
        /// Alpha values for Background pixels.
        /// </summary>
        volatile byte[] bkgAlphas;
#endif

        /// <summary>
        /// 
        /// </summary>
        IPenContext penContext;

        /// <summary>
        /// Background pen for this target.
        /// </summary>
        IReadable Pen;

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
            bkgFlags = new byte[length];
            bkgAlphas = new byte[length];
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
                penContext = value;
                ChangeBackground(value, width, height);
            }
        }
        public IReadable BackgroundPen => Pen;
        public unsafe IntPtr BackgroundData 
        {
            get
            {
                fixed (int* b = PenData)
                    return (IntPtr)b;
            }
        }
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
        public unsafe IntPtr BackgroundFlags
        {
            get
            {
                fixed (byte* b = bkgFlags)
                    return (IntPtr)b;
            }
        }
        public unsafe IntPtr BackgroundAlphas
        {
            get
            {
                fixed (byte* b = bkgAlphas)
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
            PenData = PenData.ResizedData(width, height, oldWidth, oldHeight);

#if Advanced
            flags = flags.ResizedData(width, height, oldWidth, oldHeight);
            bkgAlphas = bkgAlphas.ResizedData(width, height, oldWidth, oldHeight);
            bkgFlags = bkgFlags.ResizedData(width, height, oldWidth, oldHeight);
#endif
        }
        protected abstract void ResizeSource(int w, int h);
        #endregion

        #region UPDATE
        public abstract void Update(ulong command = 0, IBoundable area = null);
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            isDisposed = true;

            (Pen as IDisposable)?.Dispose();
            PenData = null;
#if Advanced
            bkgFlags = bkgAlphas = null;
#endif
        }
        #endregion

        #region INVOKE PAINT
        public void InvokePaint(ulong command = 0, int processID = 0) =>
            Window.InvokePaint(command, processID);
        #endregion

        #region BACKGROUND CHANGED
        unsafe void ChangeBackground(IPenContext penContext, int w, int h)
        {
            int count = w * h;
            int[] data = new int[count];
            int* newPen;
            fixed (int* p = data)
                newPen = p;
            var area = new Rect(0, 0, w, h);
            Pen = penContext?.ToPen(w, h)?? null;
            Pen?.CopyTo((IntPtr)newPen, count, w, 0, 0, area, Command.Opaque);

#if Advanced
            byte* flags;
            fixed (byte* b = bkgFlags)
                flags = b;
            int* oldPen;
            fixed (int* p = PenData)
                oldPen = p;

            BlockCopy action = (sidx, didx, len, x, y, cmd) =>
            {
                int j = didx;
                for (int i = sidx; i < sidx + len; i++, j++)
                {
                    if (flags[j] == 0)
                        continue;
                    newPen[i] = oldPen[j];
                }
            };
            Blocks.CopyBlock(area, length, width, height, 0, 0, w, count, action, Command.Opaque);
#endif
            PenData = data;
            BackgroundChanged?.Invoke(this, Factory.EmptyArgs);
        }
        public event EventHandler<IEventArgs> BackgroundChanged;
        #endregion
    }
}
