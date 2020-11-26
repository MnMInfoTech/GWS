/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
    public abstract partial class _Host : IHost
    {
        #region VARIABLES
        readonly KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs();
        readonly DrawEventArgs DrawEventArgs = new DrawEventArgs();
        #endregion

        #region PROPERTIES
        public abstract string Text { get; set; }
        public bool IsDisposed { get; private set; }
        public abstract string ID { get; }
        public string Name { get; protected set; }
        public RendererFlags RendererFlags { get; protected set; }
        public virtual bool FocusOnHover { get; set; }
        public virtual int TabIndex { get; set; }
        public abstract IObjCollection Objects { get; }
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;
        public IPenContext Background
        {
            set => Buffer.Background = value;
        }
        public IReadable BackgroundPen =>
            Buffer.BackgroundPen;
        public bool IsContainer =>
            true;
        public abstract IntPtr Handle { get; }
        public abstract Rectangle Bounds { get; }
        public abstract bool Focused { get; }
        public Rectangle InvalidatedArea =>
            Buffer.InvalidatedArea;

        protected abstract ISurface Buffer { get; }
#if Advanced
        public Rectangle ClipRectangle
        {
            get => Buffer.ClipRectangle;
            set => Buffer.ClipRectangle = value;
        }
        public bool Clipped => Buffer.Clipped;
#endif
        #endregion

        #region RENDER
        public void Render(IRenderable Renderable, IContext anyContext = null) =>
            Buffer.Render(Renderable, anyContext);
        #endregion

        #region UPDATE - INVALIDATE
        public virtual void Update(DrawCommand command = 0) =>
            Buffer.Update(command);
        public virtual void Invalidate(int x, int y, int width, int height) =>
            Buffer.Invalidate(x, y, width, height);
        #endregion

        #region COPY TO
        public virtual Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int destLen, int destW, int destX, int destY, DrawCommand command)
        {
            var copy = this.CompitibleRc(copyX, copyY, copyW, copyH);
            return Buffer.CopyTo(copy.X, copy.Y, copy.Width, copy.Height, destination, destLen, destW, destX, destY, command);
        }

        public virtual Rectangle CopyTo(IBlockable block, int destX, int destY, int copyX, int copyY,
            int copyW, int copyH, DrawCommand command)
        {
            var copy = this.CompitibleRc(copyX, copyY, copyW, copyH);
            return Buffer.CopyTo(block, destX, destY, copy.X, copy.Y, copy.Width, copy.Height, command);
        }
        #endregion

        #region TO BRUSH
#if Advanced
        public ITextureBrush ToBrush(Rectangle? copyArea = null) =>
            Buffer.ToBrush(copyArea);
#endif
        #endregion

        #region SHOW - HIDE
        public abstract void Show();
        public abstract void Hide();
        #endregion

        #region FOCUS
        public abstract bool Focus();
        #endregion

        #region REFRESH
        public virtual void Refresh()
        {
            if (Width == 0 || Height == 0)
                return;

            DrawEventArgs.Surface = this;
            OnPaint(DrawEventArgs);
            Buffer.Invalidate(0, 0, Width, Height);
            Buffer.Update();
        }
        #endregion

        #region RESIZE
        public abstract void Resize(int? width = null, int? height = null);
        #endregion

        #region FIND ELEMENT
#if Advanced
        public IRenderable FindElement(int x, int y) =>
            Buffer?.FindElement(x, y);
#endif
        #endregion

        #region PUSH EVENT
        public virtual void PushEvent(IEventInfo e)
        {
#if Advanced
            Objects?.PushEvent(e);
            if (e.Status == EventUseStatus.Used)
                return;
#endif

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
                            keyPressEventArgs.KeyChar = item;
                            OnKeyPress(keyPressEventArgs);
                        }
                    }
                    break;
                case GwsEvent.MOUSEMOTION:
                    OnMouseMove(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.ENTER:
                    OnMouseEnter(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.LEAVE:
                    OnMouseLeave(e.Args as IMouseEventArgs);
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
                //Minimal Events ends here...
                case GwsEvent.SHOWN:
                case GwsEvent.HIDDEN:
                    OnVisibleChanged(e.Args);
                    break;
                case GwsEvent.MOVED:
                    OnMoved(e.Args);
                    break;
                case GwsEvent.FOCUS_GAINED:
                    OnGotFocus(e.Args);
                    break;
                case GwsEvent.FOCUS_LOST:
                    if (!(e.Args is ICancelEventArgs))
                        e = new EventInfo(e.Sender, new CancelEventArgs(), GwsEvent.FOCUS_LOST);
                    OnLostFocus(e.Args as ICancelEventArgs);
                    break;
                case GwsEvent.CONTROLLERAXISMOTION:
                case GwsEvent.JOYAXISMOTION:
                    OnJoystickMove(e.Args as IJoystickAxisEventArgs);
                    break;
                case GwsEvent.JOYBUTTONDOWN:
                    OnJoystickDown(e.Args as IJoystickButtonEventArgs);
                    break;
                case GwsEvent.JOYBUTTONUP:
                    OnJoystickUp(e.Args as IJoystickButtonEventArgs);
                    break;
                case GwsEvent.FINGERDOWN:
                    OnTouchBegan(e.Args as ITouchEventArgs);
                    break;
                case GwsEvent.FINGERMOTION:
                    OnTouchBegan(e.Args as ITouchEventArgs);
                    break;
                case GwsEvent.FINGERUP:
                    OnTouchBegan(e.Args as ITouchEventArgs);
                    break;
                case GwsEvent.MINIMIZED:
                    OnMinimized(Factory.EmptyArgs);
                    break;
                case GwsEvent.MAXIMIZED:
                    OnMaximized(Factory.EmptyArgs);
                    break;
                case GwsEvent.RESTORED:
                    OnRestored(Factory.EmptyArgs);
                    break;
                case GwsEvent.LASTEVENT:
                case GwsEvent.FIRSTEVENT:
                    break;
                case GwsEvent.QUIT:
                case GwsEvent.EXPOSED:
                case GwsEvent.CLOSE:
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
            MouseWheel?.Invoke(this, e);
        protected virtual void OnMouseLeave(IMouseEventArgs e) =>
            MouseWheel?.Invoke(this, e);

        protected virtual void OnAppClicked(IMouseEventArgs e) =>
            AppClicked?.Invoke(this, e);


        protected virtual void OnResize(ISizeEventArgs e) =>
            Resized?.Invoke(this, e);
        protected virtual void OnPaint(IDrawEventArgs e) =>
            Paint?.Invoke(this, e);
        #endregion

        #region EVENT DECLARATION
        public virtual event EventHandler<IKeyEventArgs> KeyDown;
        public virtual event EventHandler<IKeyEventArgs> KeyUp;
        public virtual event EventHandler<IKeyPressEventArgs> KeyPress;
        public virtual event EventHandler<IMouseEventArgs> MouseWheel;
        public virtual event EventHandler<IMouseEventArgs> MouseDown;
        public virtual event EventHandler<IMouseEventArgs> MouseUp;
        public virtual event EventHandler<IMouseEventArgs> MouseClick;
        public virtual event EventHandler<IMouseEventArgs> MouseMove;
        public virtual event EventHandler<IMouseEventArgs> Enter;
        public virtual event EventHandler<IMouseEventArgs> Leave;
        public virtual event EventHandler<IMouseEventArgs> AppClicked;
        public virtual event EventHandler<ISizeEventArgs> Resized;
        public virtual event EventHandler<IDrawEventArgs> Paint;
        #endregion

        #region EVENT2 DECLARATION
        public virtual event EventHandler<ICancelEventArgs> LostFocus;
        public virtual event EventHandler<IEventArgs> GotFocus;
        public virtual event EventHandler<IEventArgs> Moved;
        public virtual event EventHandler<IKeyEventArgs> PreviewKeyDown;
        public virtual event EventHandler<IMouseEventArgs> MouseDoubleClick;
        public virtual event EventHandler<IEventArgs> VisibleChanged;
        public virtual event EventHandler<IEventArgs> FirstShown;
        public virtual event EventHandler<ITouchEventArgs> TouchBegan;
        public virtual event EventHandler<ITouchEventArgs> TouchMoved;
        public virtual event EventHandler<ITouchEventArgs> TouchEnded;
        public virtual event EventHandler<IMouseEventArgs> MouseDrag;
        public virtual event EventHandler<IMouseEventArgs> MouseDragBegin;
        public virtual event EventHandler<IMouseEventArgs> MouseDragEnd;
        public virtual event EventHandler<IJoystickButtonEventArgs> JoystickDown;
        public virtual event EventHandler<IJoystickButtonEventArgs> JoystickUp;
        public virtual event EventHandler<IJoystickAxisEventArgs> JoystickMove;
        public virtual event EventHandler<IEventArgs> JoystickConnected;
        public virtual event EventHandler<IEventArgs> JoystickDisconnected;
        public virtual event EventHandler<IEventArgs> Minimized;
        public virtual event EventHandler<IEventArgs> Maximized;
        public virtual event EventHandler<IEventArgs> Restored;
        #endregion

        #region EVENTS2 EXPOSING METHODS
        protected virtual void OnPreviewKeyDown(IKeyEventArgs e) =>
            PreviewKeyDown?.Invoke(this, e);

        protected virtual void OnMouseDoubleClick(IMouseEventArgs e) =>
            MouseDoubleClick?.Invoke(this, e);
        protected virtual void OnMouseDragBegin(IMouseEventArgs e) =>
            MouseDragBegin?.Invoke(this, e);
        protected virtual void OnMouseDragEnd(IMouseEventArgs e) =>
            MouseDragEnd?.Invoke(this, e);
        protected virtual void OnMouseDrag(IMouseEventArgs e) =>
            MouseDrag?.Invoke(this, e);
        protected virtual void OnVisibleChanged(IEventArgs e) =>
            VisibleChanged?.Invoke(this, e);
        protected virtual void OnFirstShown(IEventArgs e) =>
            FirstShown?.Invoke(this, e);
        protected virtual void OnGotFocus(IEventArgs e) =>
            GotFocus?.Invoke(this, e);
        protected virtual void OnLostFocus(ICancelEventArgs e) =>
            LostFocus?.Invoke(this, e);
        protected virtual void OnMoved(IEventArgs e) =>
            Moved?.Invoke(this, e);
        protected virtual void OnTouchBegan(ITouchEventArgs e) =>
            TouchBegan?.Invoke(this, e);
        protected virtual void OnTouchMoved(ITouchEventArgs e) =>
            TouchMoved?.Invoke(this, e);
        protected virtual void OnTouchEnded(ITouchEventArgs e) =>
            TouchEnded?.Invoke(this, e);
        protected virtual void OnJoystickDown(IJoystickButtonEventArgs e) =>
            JoystickDown?.Invoke(this, e);
        protected virtual void OnJoystickUp(IJoystickButtonEventArgs e) =>
            JoystickUp?.Invoke(this, e);
        protected virtual void OnJoystickMove(IJoystickAxisEventArgs e) =>
            JoystickMove?.Invoke(this, e);
        protected virtual void OnJoystickConnected(IEventArgs e) =>
            JoystickConnected?.Invoke(this, e);
        protected virtual void OnJoystickDisconnected(IEventArgs e) =>
            JoystickDisconnected?.Invoke(this, e);

        protected virtual void OnMaximized(IEventArgs e) =>
            Maximized?.Invoke(this, e);
        protected virtual void OnMinimized(IEventArgs e) =>
            Minimized?.Invoke(this, e);
        protected virtual void OnRestored(IEventArgs e) =>
            Restored?.Invoke(this, e);
        #endregion

        #region BUFFER EVENTS
#if Advanced
        protected virtual void OnBackgroundChanged(IEventArgs e)
        {
            BackgroundChanged?.Invoke(this, e);
        }
        public event EventHandler<IEventArgs> BackgroundChanged;
#endif
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            IsDisposed = true;
            Objects?.Dispose();
            (Buffer as IDisposable).Dispose();
        }
        #endregion
    }
    partial class _Host
    {
        #region IBUFFER
        int ILength.Length =>
            Buffer.Length;
#if Advanced
        unsafe int* IMixableBlock.Pixels(bool ForegroundBuffer) =>
            Buffer.Pixels(ForegroundBuffer);
        unsafe byte* IMixableBlock.AlphaValues(bool ForegroundBuffer) =>
            Buffer.AlphaValues(ForegroundBuffer);
        IDrawable2 IObjectAware.Control
        {
            get => Buffer.Control;
            set => Buffer.Control = value;
        }
        IReadable IWritable.Target =>
            Buffer.Target;
#endif
        void IWritable.WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, DrawCommand command) =>
           Buffer.WritePixel(val, axis, horizontal, color, Alpha, command);

        unsafe void IWritable.WriteLine(int* source, int srcIndex, int srcW, int length, bool horizontal,
            int x, int y, float? Alpha, byte* imageAlphas, DrawCommand command) =>
            Buffer.WriteLine(source, srcIndex, srcW, length, horizontal, x, y, Alpha, imageAlphas, command);

        void IClearable.Clear(int x, int y, int width, int height, DrawCommand command) =>
            Buffer.Clear(x, y, width, height, command);

        void IReceiver.Receive(IntPtr source, int srcW, int srcH, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, DrawCommand command) =>
            Buffer.Receive(source, srcW, srcH, dstX, dstY, copyX, copyY, copyW, copyH, command);

        object ICloneable.Clone() =>
            Buffer.Clone();
        #endregion
    }
}