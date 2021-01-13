/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MnM.GWS.Desktop
{
    public abstract partial class _NativeTarget : Form, INativeTarget
    {
        #region VARIABLES
        /// <summary>
        /// 
        /// </summary>
        protected INativeForm Window;

        /// <summary>
        /// Background pen to provide background for this object.
        /// </summary>
        IReadable BkgPen;

        /// <summary>
        /// 
        /// </summary>
        protected volatile Bitmap Bitmap;

        /// <summary>
        /// 
        /// </summary>
        protected readonly Array<int> Pointer;

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
        /// Indicates if this object is currently being resized or not.
        /// </summary>
        protected volatile bool IsResizing;

        protected readonly int originalWidth, originalHeight;

#if Advanced
        protected byte[] Locking;
#endif

        protected const byte o = 0;

        #region EVENT ARGS
        readonly MsKeyEventArgs keyEventArgs = new MsKeyEventArgs();
        readonly MsMouseEventArgs mouseEventArgs = new MsMouseEventArgs();
        readonly MsKeyPressEventArgs keyPressEventArgs = new MsKeyPressEventArgs();

        readonly EventInfo keyeventInfo = new EventInfo();
        readonly EventInfo mouseeventInfo = new EventInfo();
        readonly EventInfo keypresseventInfo = new EventInfo();
        readonly EventInfo loadEventInfo = new EventInfo();
        #endregion
        #endregion

        #region CONSTRUCTORS
        internal _NativeTarget(int x, int y, int w, int h)
        {
            Pointer = new Array<int>(w, h);
            Bitmap = new Bitmap(w, h, w * 4, PixelFormat.Format32bppArgb, Pointer.Handle);
            width = Pointer.Width;
            height = Pointer.Height;
            length = Pointer.Length;
            originalWidth = width;
            originalHeight = height;
            DoubleBuffered = true;
            BackColor = Color.White;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(x, y);
            Size = new System.Drawing.Size(w, h);
        }
        #endregion

        #region PROPERTIES
        public string ID => Name;
        public IPenContext Background
        {
            get => BkgPen;
            set
            {
                if (value == null)
                {
                    (BkgPen as IDisposable)?.Dispose();
                    BkgPen = null;
                }
                else
                    BkgPen = value.ToPen(Width, Height);
                OnBackgroundChanged(Factory.EmptyArgs);
                Clear(0, 0, width, height, Command.Backdrop);
            }
        }
        IntPtr IPixels.Source => Pointer.Handle;
        int ISize.Width => width;
        int ISize.Height => height;
        int ILength.Length => length;
        INativeForm INativeTarget.Form
        {
            set
            {
                Window = value;
                if (Window == null)
                    return;
                if (Window.Width != width || Window.Height != height)
                {
                    ((IResizable)this).Resize(Window.Width, Window.Height);
                }
            }
        }
        public int[] PenData { get; private set; }
        protected unsafe int* Screen => (int*)Pointer.Handle;
        public override string Text
        {
            get => base.Text;
            set
            {
                if (InvokeRequired)
                    SetTextSafe(value);
                else
                    base.Text = value;
            }
        }
#if Advanced
        byte[] IRenderTarget.Locking => Locking;
#endif
        #endregion

        #region PAINT
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.DrawImage(Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }
        #endregion

        #region RESIZE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IResizable.Resize(int? newWidth, int? newHeight)
        {
            Size = new System.Drawing.Size(newWidth ?? Size.Width, newHeight ?? Size.Height);
            OnResizeEnd(EventArgs.Empty);
        }
        protected override void OnResizeBegin(EventArgs e)
        {
            IsResizing = true;
            base.OnResizeBegin(e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void OnResizeEnd(EventArgs e)
        {
            int w = Size.Width;
            int h = Size.Height;
            if (w <= originalWidth && h <= originalHeight)
            {
                base.OnResizeEnd(e);
                return;
            }
            w = Math.Max(w, originalWidth);
            h = Math.Max(h, originalHeight);
            width = w;
            height = h;
            length = width * height;
            Window?.Resize(Size.Width, Size.Height);
            Pointer.Resize(Size.Width, Size.Height);
            Bitmap = new Bitmap(Pointer.Width, Pointer.Height, Pointer.Width * 4,
                PixelFormat.Format32bppArgb, Pointer.Handle);
            width = Pointer.Width;
            height = Pointer.Height;
            length = Pointer.Length;
            (BkgPen as IResizable)?.Resize(width, height);
            OnBackgroundChanged(Factory.EmptyArgs);
            IsResizing = false;
            Clear(0, 0, width, height, Command.Backdrop);
            base.OnResizeEnd(e);
        }
        #endregion

        #region UPDATE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Update(Command Command, IRectangle RecentlyDrawn = null);
        #endregion

        #region EVENT BINDING
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            loadEventInfo.Sender = this;
            loadEventInfo.Args = Factory.EmptyArgs;
            loadEventInfo.Type = 57;//FirstShown;
            Window?.PushEvent(loadEventInfo);
        }
        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            keyEventArgs.keyCode = e.KeyCode.ToGWSKey();
            keyEventArgs.scanCode = e.KeyValue;
            keyEventArgs.keyState = KeyState.Up;
            keyeventInfo.Sender = this;
            keyeventInfo.Args = keyEventArgs;
            keyeventInfo.Type = 5;//KeyUp;
            Window?.PushEvent(keyeventInfo);
        }
        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            keyEventArgs.keyCode = e.KeyCode.ToGWSKey();
            keyEventArgs.scanCode = e.KeyValue;
            keyEventArgs.keyState = KeyState.Down;
            keyeventInfo.Sender = this;
            keyeventInfo.Args = keyEventArgs;
            keyeventInfo.Type = 4;//KeyDown;
            Window?.PushEvent(keyeventInfo);
        }
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            mouseEventArgs.mouseState = MouseState.Up;
            mouseEventArgs.button = e.Button.ToGwsButton();
            mouseEventArgs.x = e.X;
            mouseEventArgs.y = e.Y;
            mouseEventArgs.delta = e.Delta;

            mouseeventInfo.Sender = this;
            mouseeventInfo.Args = mouseEventArgs;
            mouseeventInfo.Type = 9;//MouseUp;
            Window?.PushEvent(mouseeventInfo);
        }
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            mouseEventArgs.mouseState = MouseState.Down;
            mouseEventArgs.button = e.Button.ToGwsButton();
            mouseEventArgs.x = e.X;
            mouseEventArgs.y = e.Y;
            mouseEventArgs.delta = e.Delta;

            mouseeventInfo.Sender = this;
            mouseeventInfo.Args = mouseEventArgs;
            mouseeventInfo.Type = 8;//MouseDown;
            Window?.PushEvent(mouseeventInfo);
        }
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            mouseEventArgs.mouseState = MouseState.Move;
            mouseEventArgs.button = e.Button.ToGwsButton();
            mouseEventArgs.x = e.X;
            mouseEventArgs.y = e.Y;
            mouseEventArgs.delta = e.Delta;

            mouseeventInfo.Sender = this;
            mouseeventInfo.Args = mouseEventArgs;
            mouseeventInfo.Type = 7;//MouseMotion;
            Window?.PushEvent(mouseeventInfo);
        }
        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            mouseEventArgs.mouseState = MouseState.Wheel;
            mouseEventArgs.button = e.Button.ToGwsButton();
            mouseEventArgs.x = e.X;
            mouseEventArgs.y = e.Y;
            mouseEventArgs.delta = e.Delta;

            mouseeventInfo.Sender = this;
            mouseeventInfo.Args = mouseEventArgs;
            mouseeventInfo.Type = 10;//MouseWheel;
            Window?.PushEvent(mouseeventInfo);
        }
        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            mouseEventArgs.mouseState = MouseState.Click;
            mouseEventArgs.button = e.Button.ToGwsButton();
            mouseEventArgs.x = e.X;
            mouseEventArgs.y = e.Y;
            mouseEventArgs.delta = e.Delta;

            mouseeventInfo.Sender = this;
            mouseeventInfo.Args = mouseEventArgs;
            mouseeventInfo.Type = 56;//MouseClick;
            Window?.PushEvent(mouseeventInfo);
        }
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
            keyPressEventArgs.keyChar = e.KeyChar;
            keypresseventInfo.Sender = this;
            keypresseventInfo.Args = keyPressEventArgs;
            keypresseventInfo.Type = 6; //TextInput;
            Window?.PushEvent(keypresseventInfo);
        }
        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            Pointer.Dispose();
        }
        #endregion

        #region COPY FROM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract unsafe IRectangle CopyFrom(IntPtr source, int srcW, int srcH, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH,
            Command Command, string ShapeID, IntPtr alphaBytes = default(IntPtr));
        #endregion

        #region COPY TO       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract unsafe IRectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr dest,
            int dstLen, int dstW, int dstX, int dstY, Command Command = 0, string shapeID = null);
        #endregion

        #region CLEAR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract unsafe void Clear(int clearX, int clearY, int clearW, int clearH, Command Command = 0);
        #endregion

        #region BACKGROUND CHANGED
        protected virtual unsafe void OnBackgroundChanged(IEventArgs e)
        {
            int* pen = null;

            if (BkgPen != null)
            {
                PenData = new int[length];
                fixed (int* p = PenData)
                    pen = p;
                if (BkgPen != null)
                    BkgPen.CopyTo(0, 0, width, height, (IntPtr)pen, length, width, 0, 0, Command.Opaque);
            }
            else
                PenData = null;

            BackgroundChanged?.Invoke(this, e);
        }
        public event EventHandler<IEventArgs> BackgroundChanged;
        #endregion

        #region INVOKE DELGATES
        void invalidateSafe(System.Drawing.Rectangle rectangle) =>
            Invalidate(rectangle);
        void updateSafe() =>
            Update();
        void setTextSafe(string text) =>
            base.Text = text;

        protected void InvalidateSafe(System.Drawing.Rectangle rectangle)
        {
            Invoke(new DelInvalidate(invalidateSafe), rectangle);
        }
        protected void UpdateSafe()
        {
            Invoke(new DelUpdate(updateSafe));
        }
        protected void SetTextSafe(string text)
        {
            Invoke(new DelTextChange(setTextSafe), text);
        }
        delegate void DelInvalidate(System.Drawing.Rectangle rectangle);
        delegate void DelUpdate();
        delegate void DelTextChange(string text);
        #endregion
    }
}
