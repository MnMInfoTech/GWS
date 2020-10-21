using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MnM.GWS.Desktop
{
#if (GWS || Window)
    public class GwsForm : Form, IWindowControl
    {
        #region VARIABLES
        readonly EventInfo eventInfo = new EventInfo();
        const int dx = 602, dy = 200, dw = 404, dh = 506;
        readonly EventArgs<IContainer> parentArgs = new EventArgs<IContainer>();
        readonly KeyPressEventArgs KeyPressEventArgs = new KeyPressEventArgs();
        protected ISurface Window;
#if Advanced
        protected int[] Data;
        protected byte[] Alphas;
#endif
        #endregion

        #region CONSTRUCTORS
        public GwsForm() :
            this(dx, dy, dw, dh)
        { }
        public GwsForm(int width, int height) :
            this(dx, dy, width, height)
        { }
        public GwsForm(int x, int y, int width, int height) : base()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(x, y);
            Size = new System.Drawing.Size(width, height);
        }
        #endregion

        #region PROPERTIES
        public string ID => base.Name;
        RectangleF IBoundsF.Bounds { get; }
        Rectangle IHostable.Bounds { get; }
        public int X => Bounds.X;
        public int Y => Bounds.Y;

        public IReadContext Background
        {
            get => (Window as IBackground)?.Background;
            set
            {
                if (!(Window is IBackground))
                    return;
                (Window as IBackground).Background = value;
            }
        }
        public bool FocusOnHover { get; set; }
        public IReadContext Foreground { get; set; }
        Rectangle IBounds.Bounds =>
            new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
        ISurface IHostable.Window => Window;
#if Advanced
        int[] IRenderTarget.Data => Data;
        byte[] IRenderTarget.Alphas => Alphas;
#endif
        #endregion

        #region ASSIGN PARENT
        public void Assign(ISurface window)
        {
            if (window == null)
                return;

            this.Window = window;

            if (window is IContainer)
            {
                parentArgs.Args = (IContainer)window;
                OnParentChanged(parentArgs);
            }
        }

        protected virtual void OnParentChanged(IEventArgs<IContainer> e)
        {
            Background = Rgba.ActiveCaption;
            ParentChanged?.Invoke(this, e);
        }
        #endregion

        #region DRAW
        public bool Draw(IBlock writable, IReadContext context, out IPen pen)
        {
            pen = null;
            return true;
        }
        IEnumerable<VectorF> IDrawable.ToShape() => null;
        #endregion

        #region UPDATE
        public void Invalidate(int x, int y, int width, int height, bool updateImmedaite = false)
        {
            Invalidate(new Rectangle(x, y, width, height));
            Update();
        }
        #endregion

        #region COPY FROM
        public void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region PAINT CONTROL
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (Window == null)
                return;
            var size = Window.Portion(out IntPtr Data, e.ClipRectangle.X,
                e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
            if (Data == IntPtr.Zero)
                return;
            var bitmap = new Bitmap(size.Width, size.Height, size.Width * 4, PixelFormat.Format32bppArgb, Data);
            e.Graphics.DrawImage(bitmap, e.ClipRectangle.X, e.ClipRectangle.Y);
        }

        public new event EventHandler<IEventArgs<IContainer>> ParentChanged;
        #endregion

        #region CLEAR
        public Rectangle Clear(bool updateImmediate = false) =>
            Window.Clear(updateImmediate);
        public Rectangle Clear(int x, int y, int width, int height, bool updateImmediate = false) =>
            Window.Clear(x, y, width, height, updateImmediate);
        #endregion

        #region RESIZE
        #endregion

        #region PUSH EVENT
        public virtual void PushEvent(IEventInfo e)
        {
            switch (e.Type)
            {
                case GwsEvent.KEYDOWN:
                    OnKeyDown(e.Args as IKeyEventArgs);
                    break;
                case GwsEvent.KEYUP:
                    OnKeyDown(e.Args as IKeyEventArgs);
                    break;
                case GwsEvent.TEXTINPUT:
                    var txtInput = e.Args as ITextInputEventArgs;
                    if (txtInput != null)
                    {
                        foreach (var item in txtInput.Characters)
                        {
                            KeyPressEventArgs.KeyChar = item;
                            OnKeyPress(KeyPressEventArgs);
                        }
                    }
                    break;
                case GwsEvent.MOUSEMOTION:
                    OnMouseMove(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.MOUSEBUTTONDOWN:
                    OnMouseDown(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.MOUSEBUTTONUP:
                    IMouseEventArgs me = e.Args as IMouseEventArgs;
                    OnMouseUp(me);
                    OnMouseClick(me);
                    break;
                case GwsEvent.MOUSEWHEEL:
                    OnMouseWheel(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.ENTER:
                    OnMouseEnter(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.LEAVE:
                    OnMouseLeave(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.SIZE_CHANGED:
                    break;
                case GwsEvent.RESIZED:
                    OnResize(e.Args as ISizeEventArgs);
                    break;
                case GwsEvent.PAINT:
                    OnPaint(e.Args as IDrawEventArgs);
                    break;
                case GwsEvent.APPCLICK:
                    OnAppClicked(e.Args as IMouseEventArgs);
                    break;
                default:
                    e.Status = EventUseStatus.Unused;
                    break;
            }
            if (e.Status == EventUseStatus.Used)
                return;
            OnEventPushed(e);
        }
        protected virtual void OnEventPushed(IEventInfo e) =>
            EventPushed?.Invoke(this, e);

        public virtual event EventHandler<IEventInfo> EventPushed;
        #endregion

        #region EVENTS EXPOSING METHODS
        protected virtual void OnKeyDown(IKeyEventArgs e) =>
            KeyDown?.Invoke(this, e);
        protected virtual void OnKeyUp(IKeyEventArgs e) =>
            KeyUp?.Invoke(this, e);
        protected virtual bool OnKeyPress(IKeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
            return true;
        }
        protected virtual void OnMouseWheel(IMouseEventArgs e) =>
            MouseWheel?.Invoke(this, e);
        protected virtual void OnMouseDown(IMouseEventArgs e) =>
            MouseDown?.Invoke(this, e);
        protected virtual void OnMouseUp(IMouseEventArgs e) =>
            MouseUp?.Invoke(this, e);
        protected virtual void OnMouseClick(IMouseEventArgs e) =>
            MouseClick?.Invoke(this, e);
        protected virtual void OnMouseMove(IMouseEventArgs e) =>
            MouseMove?.Invoke(this, e);
        protected virtual void OnMouseEnter(IMouseEventArgs e) =>
            Enter?.Invoke(this, e);

        protected virtual void OnMouseLeave(IMouseEventArgs e) =>
            Leave?.Invoke(this, e);

        protected virtual void OnAppClicked(IMouseEventArgs e) =>
            AppClicked?.Invoke(this, e);


        protected virtual void OnResize(ISizeEventArgs e) =>
            Resized?.Invoke(this, e);
        protected virtual void OnPaint(IDrawEventArgs e) =>
            Paint?.Invoke(this, e);
        #endregion

        #region EVENT DECLARATION
        public new event EventHandler<IKeyEventArgs> KeyDown;
        public new event EventHandler<IKeyEventArgs> KeyUp;
        public new event EventHandler<IKeyPressEventArgs> KeyPress;
        public new event EventHandler<IMouseEventArgs> MouseWheel;
        public new event EventHandler<IMouseEventArgs> MouseDown;
        public new event EventHandler<IMouseEventArgs> MouseUp;
        public new event EventHandler<IMouseEventArgs> MouseClick;
        public new event EventHandler<IMouseEventArgs> MouseMove;

        public new event EventHandler<IMouseEventArgs> Enter;
        public new event EventHandler<IMouseEventArgs> Leave;

        public event EventHandler<IMouseEventArgs> AppClicked;
        public event EventHandler<ISizeEventArgs> Resized;
        public new event EventHandler<IDrawEventArgs> Paint;

        public bool Antialiased { get; set; }
        public int Length { get; }
        bool IWritable.Antialiased { get; }

        public void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha)
        {
            throw new NotImplementedException();
        }

        public unsafe void WriteLine(int* source, int srcIndex, int srcW, int length, bool horizontal, int x, int y, float? Alpha)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region DISPOSE
        #endregion
    }
#endif
}
