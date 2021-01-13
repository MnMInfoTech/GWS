/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
#if Window
using System;

namespace MnM.GWS
{
    public abstract partial class _Window : _Host, IWindow
    {
        #region VARIABLES
        readonly ICanvas Primary;
        ICanvas Current;

        protected bool previousCursorVisible = true;
        protected bool cursorVisible;
        protected readonly IRenderTarget UnderlyingWindow;
        bool firstShow;
        readonly DrawEventArgs DrawEventArgs = new DrawEventArgs();
        readonly EventInfo Event = new EventInfo();
        protected Rectangle bounds;
        protected bool focused;
        IExternalWindow Target;
        #endregion

        #region CONSTRUCTORS
        _Window(int width, int height)
        {
            bounds = new Rectangle(0, 0, width, height);
        }
        protected _Window(IExternalWindow target) :
            this(target.Width, target.Height)
        {
            if (!Initialize(externalWindow: target))
                throw new Exception("Window could not be initialized!");
            Target = target;
            UnderlyingWindow = Target;
            Initialize(out Primary, null);
        }
        protected _Window(string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null,
            RendererFlags? renderFlags = null) :
            this(width ?? 100, height ?? 100)
        {
            if (!Initialize(title, width, height, x, y, flags, display, renderFlags: renderFlags))
                throw new Exception("Window could not be initialized!");

            UnderlyingWindow = Factory.newRenderTarget(this);
            Initialize(out Primary, flags);
        }

        protected abstract bool Initialize(string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null,
            IExternalWindow externalWindow = null, RendererFlags? renderFlags = null);

        void Initialize(out ICanvas Canvas, GwsWindowFlags? flags)
        {
            Canvas = Factory.newCanvas(UnderlyingWindow);
            Current = Canvas;
            Initialize2(Canvas);
            GwsWindowFlags = flags ?? 0;
            if (GwsWindowFlags.HasFlag(GwsWindowFlags.OpenGL))
                GLContext = Factory.newGLContext(this);
            WindowID = Factory.GetWindowID(Handle);
            Name = "Window" + WindowID;
            this.Register();
        }
        partial void Initialize2(ICanvas Canvas);
        #endregion

        #region PROPERTIES
        protected sealed override ICanvas Buffer => Current;
        public sealed override string ID => Name;
        public sealed override Rectangle Bounds => bounds;
        public override IPenContext Background
        {
            get => UnderlyingWindow.Background;
            set
            {
                UnderlyingWindow.Background = value;
            }
        }
        public GwsWindowFlags GwsWindowFlags { get; private set; }
        public IGLContext GLContext { get; private set; }
        public int WindowID { get; private set; }
        public int X => bounds.X;
        public int Y => bounds.Y;
        public abstract Size MinSize { get; set; }
        public abstract Size MaxSize { get; set; }
        public bool IsWindow => true;
        public bool IsMouseDragging { get; protected set; }
        public bool Transparent => Transparency != 0f;
        public virtual IScreen Screen { get; set; }
        public virtual float Transparency { get; set; }
        public virtual WindowState WindowState { get; protected set; }
        public virtual WindowBorder WindowBorder { get; protected set; }
        public override string Text { get; set; }
        public virtual VectorF Scale { get; set; }
        public virtual bool CursorVisible { get; set; }
        protected CursorType? ResizeCursor { get; set; }
        public abstract uint PixelFormat { get; }
        public abstract ISound Sound { get; }
        public abstract bool Visible { get; set; }
        public abstract bool Enabled { get; set; }
        #endregion

        #region SHOW - HIDE
        public override void Show() =>
            ChangeVisible(true);
        public override void Hide() =>
            ChangeVisible(false);
        protected virtual void ChangeVisible(bool value)
        {
            var e = Factory.EmptyArgs;
            if (!firstShow && value)
            {
                firstShow = true;
                OnFirstShown(e);
            }
            OnVisibleChanged(e);
        }
        #endregion

        #region SHOW - HIDE - SET CURSOR
        public abstract void SetCursor(int x, int y);
        public abstract void ShowCursor();
        public abstract void HideCursor();
        #endregion

        #region POINT TO CLIENT - SCREEN
        public bool Contains(float x, float y) =>
            x >= X && y >= Y && x <= X + Width && y <= Y + Height;
        public abstract void ContainMouse(bool flag);
        #endregion

        #region CHANGE SCREEN
        public abstract void ChangeScreen(int screenIndex);
        #endregion

        #region CHANGE STATE
        public abstract void ChangeState(WindowState state);
        #endregion

        #region CHANGE BORDER
        public abstract void ChangeBorder(WindowBorder border);
        #endregion

        #region CLOSE - DISPOSE
        public void Close()
        {
            isDisposed = true;           
            this.Deregister();
            Objects?.Dispose();
            Primary.Dispose();
            Close2();
            OnClosed(Factory.EmptyArgs);
        }
        partial void Close2();
        public override void Dispose()
        {
            Close();
        }
        #endregion

        #region REFRESH
        public override void Refresh()
        {
            if (!Visible)
                return;
            base.Refresh();
        }
        #endregion

        #region RESIZE
        public override void Resize(int? width, int? height)
        {
            if (width == null && height == null)
                return;

            if (width == Width && Height == height)
                return;

            var w = width ?? Width;
            var h = height ?? Height;
            OnResize(new SizeEventArgs(w, h));
        }

        protected override void OnResize(ISizeEventArgs e)
        {
            bounds = new Rectangle(Bounds.X, Bounds.Y, e.Width, e.Height);
            Primary?.Resize(e.Width, e.Height);
            Resize2();
            UnderlyingWindow.Resize(e.Width, e.Height);
            base.OnResize(e);
        }
        partial void Resize2();
        #endregion

        #region MOVE
        public abstract void Move(int? x = null, int? y = null);
        #endregion

        #region ZORDER
        public abstract void BringToFront();
        public abstract void SendToBack();
        public abstract void BringForward(int numberOfPlaces = 1);
        public abstract void SendBackward(int numberOfPlaces = 1);
        #endregion

        #region IEVENT PROCESSING
        public bool ProcessEvent(IEvent @event)
        {
            var e = ParseEvent(@event);

            if (e == null)
                return false;
            var Event = new EventInfo();
            Event.Sender = this;
            Event.Args = e;
            Event.Type = (int)@event.Type;
            PushEvent(Event);
            //Event.Handled = false;
            return true;
        }
        public override void PushEvent(IEventInfo e)
        {
            switch ((GwsEvent)e.Type)
            {
                case GwsEvent.Close:
                    OnClosed(e.Args);
                    break;
                case GwsEvent.Resized:
                    //case GwsEvent.SIZE_CHANGED:
                    base.PushEvent(e);
                    break;
                default:
                    if (Target != null)
                        Target.PushEvent(e);
                    else
                        base.PushEvent(e);
                    break;
            }
        }
        protected abstract IEventArgs ParseEvent(IEvent @event);
        #endregion

        #region EVENT DECLARATION WINDOW
        public event EventHandler<IEventArgs> Load;
        public event EventHandler<IEventArgs> TitleChanged;
        public event EventHandler<IEventArgs> WindowBorderChanged;
        public event EventHandler<IEventArgs> WindowStateChanged;
        public event EventHandler<ICancelEventArgs> Closing;
        public event EventHandler<IEventArgs> Closed;
#if Advanced
        public event EventHandler<IEventArgs> BufferChanged;
#endif
        #endregion

        #region EVENT EXPOSING METHODS
        protected override void OnVisibleChanged(IEventArgs e)
        {
            base.OnVisibleChanged(e);
        }
        protected override void OnFirstShown(IEventArgs e)
        {
            Refresh();
            base.OnFirstShown(e);
        }
        protected override void OnGotFocus(IEventArgs e)
        {
            if (!previousCursorVisible)
            {
                previousCursorVisible = true;
                CursorVisible = false;
            }
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(ICancelEventArgs e)
        {
            base.OnLostFocus(e);
            if (e.Cancel)
            {
                focused = true;
                return;
            }
            if (!previousCursorVisible)
            {
                previousCursorVisible = true;
                CursorVisible = false;
            }
            focused = false;
        }
        protected virtual void OnTitleChanged(IEventArgs e) =>
            TitleChanged?.Invoke(this, e);
        protected virtual void OnWindowBorderChanged(IEventArgs e) =>
            WindowBorderChanged?.Invoke(this, e);
        protected virtual void OnWindowStateChanged(IEventArgs e) =>
            WindowStateChanged?.Invoke(this, e);
        protected virtual void OnClosed(IEventArgs e) =>
            Closed?.Invoke(this, e);
        protected virtual void OnClosing(ICancelEventArgs e) =>
            Closing?.Invoke(this, e);
        protected virtual void OnLoad(IEventArgs e) =>
            Load?.Invoke(this, e);
        #endregion

        #region BACKGROUND CHANGED
        private void BackgroundIsChanged(object sender, IEventArgs e) =>
            OnBackgroundChanged(e);
        protected virtual void OnBackgroundChanged(IEventArgs e)
        {
            BackgroundChanged?.Invoke(this, e);
        }
        public event EventHandler<IEventArgs> BackgroundChanged;
        #endregion
    }
}
#endif
