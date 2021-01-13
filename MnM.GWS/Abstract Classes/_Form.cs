/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if GWS || Window
    public abstract partial class _Form : IForm
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
        /// 
        /// </summary>
        protected INativeTarget Target;

        /// <summary>
        /// Indicates if this object is currently being resized or not.
        /// </summary>
        protected bool IsResizing;

        /// <summary>
        /// 
        /// </summary>
        protected readonly IObjCollection objects;

        readonly MnM.GWS.KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs();
        readonly DrawEventArgs drawEventArgs = new DrawEventArgs();
        readonly EventInfo drawEventInfo = new EventInfo();

        protected const int formX = 602, formY = 200, formW = 404, formH = 506;
        #endregion

        #region CONSTRUCTORS
        protected _Form(int formW, int formH)
        {
            width = formW;
            height = formH;
            length = width * height;
            objects = Factory.newObjectCollection(this);
        }
        #endregion

        #region PROPERTIES
        public int Width => width;
        public int Height => height;
        public int Length => length;
        public IObjCollection Objects => objects;
        public string ID => Target?.Name;
        public string Name =>
            Target.Name;
        public IPenContext Background
        {
            get => Target.Background;
            set
            {
                Target.Background = value;
            }
        }
        public string Text
        {
            get => Target.Text;
            set => Target.Text = value;
        }
        public bool IsDisposed => Target.IsDisposed;
        public bool CanNotWrite => Target.IsDisposed || IsResizing;
        #endregion

        #region WRITE PIXEL
        public abstract void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, Command Command, string ShapeID, INotifier boundary);
        #endregion

        #region WRITE LINE
        public abstract unsafe void WriteLine(int* colors, int srcIndex, int srcW, int length, bool horizontal, int x, int y,
            float? Alpha, byte* imageAlphas, Command Command, string ShapeID, INotifier boundary);
        #endregion

        #region CLEARINDICES
        public abstract void ClearIndices();
        #endregion

        #region CLEAR
        public abstract void Clear(int clearX, int clearY, int clearW, int clearH, Command command = 0);
        #endregion

        #region COPY FROM
        public abstract IRectangle CopyFrom(IntPtr source, int srcW, int srcH, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH,
            Command Command, string ShapeID, IntPtr alphaBytes = default);
        #endregion

        #region COPY TO
        public abstract IRectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int dstLen,
            int dstW, int dstX, int dstY, Command command = 0, string shapeID = null);
        #endregion

        #region CONSOLIDATE
        public abstract IRectangle Consolidate(int copyX, int copyY, int copyW, int copyH, IntPtr destination,
            int dstLen, int dstW, int dstX, int dstY, IImageData backBuffer, Command Command = Command.None, IntPtr? Pen = null, string shapeID = null);
        #endregion

        #region REFRESH
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Refresh()
        {
            if (Width == 0 || Height == 0)
                return;
            drawEventArgs.Graphics = this;
            OnPaint(drawEventArgs);
            Update(0, new Rectangle(0, 0, Width, Height));
        }
        #endregion

        #region RESIZE
        public abstract void Resize(int? newWidth = null, int? newHeight = null);
        #endregion

        #region UPDATE
        public abstract void Update(Command Command, IRectangle RecentlyDrawn = null);
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            Target.Dispose();
        }
        #endregion

        #region SHOW - HIDE
        public void Show() =>
            Target.Show();
        public void Hide() =>
            Target.Hide();
        #endregion

        #region PUSH - PROCESS EVENT
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushEvent(IEventInfo e)
        {
            VerifyEvent(e);
            if (e.Handled)
                return;
            var Type = (GwsEvent)e.Type;
            switch (Type)
            {
                case GwsEvent.KeyDown:
                    OnKeyDown(e.Args as IKeyEventArgs);
                    break;
                case GwsEvent.KeyUp:
                    OnKeyDown(e.Args as IKeyEventArgs);
                    break;
                case GwsEvent.TextInput:
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
                case GwsEvent.MouseMotion:
                    OnMouseMove(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.Enter:
                    OnMouseEnter(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.Leave:
                    OnMouseLeave(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.MouseDown:
                    OnMouseDown(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.MouseUp:
                    IMouseEventArgs me = e.Args as IMouseEventArgs;
                    OnMouseUp(me);
                    OnMouseClick(me);
                    break;
                case GwsEvent.MouseWheel:
                    OnMouseWheel(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.SizeChanged:
                    break;
                case GwsEvent.Resized:
                    OnResize(e.Args as ISizeEventArgs);
                    break;
                case GwsEvent.Paint:
                    OnPaint(e.Args as IDrawEventArgs);
                    break;
                case GwsEvent.AppClick:
                    OnAppClicked(e.Args as IMouseEventArgs);
                    break;
                //Minimal Events ends here...
                case GwsEvent.Shown:
                case GwsEvent.Hidden:
                    OnVisibleChanged(e.Args);
                    break;
                case GwsEvent.FirstShown:
                    OnFirstShown(e.Args);
                    break;
                case GwsEvent.FocusGained:
                    OnGotFocus(e.Args);
                    break;
                case GwsEvent.FocusLost:
                    if (!(e.Args is ICancelEventArgs))
                        e = new EventInfo(e.Sender, new CancelEventArgs(), (int)GwsEvent.FocusLost);
                    OnLostFocus(e.Args as ICancelEventArgs);
                    break;
                case GwsEvent.Minimized:
                    OnMinimized(Factory.EmptyArgs);
                    break;
                case GwsEvent.Maximized:
                    OnMaximized(Factory.EmptyArgs);
                    break;
                case GwsEvent.Restored:
                    OnRestored(Factory.EmptyArgs);
                    break;
                case GwsEvent.LASTEVENT:
                case GwsEvent.First:
                    break;
                case GwsEvent.Quit:
                case GwsEvent.Exposed:
                case GwsEvent.Close:
                default:
                    return;
            }
            OnEventPushed(e);
        }

        partial void VerifyEvent(IEventInfo e);
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

        #region MINIMAL WIDOW EVENTS DECLARATION
        public virtual event EventHandler<ICancelEventArgs> LostFocus;
        public virtual event EventHandler<IEventArgs> GotFocus;
        public virtual event EventHandler<IMouseEventArgs> MouseDoubleClick;
        public virtual event EventHandler<IEventArgs> VisibleChanged;
        public virtual event EventHandler<IEventArgs> Minimized;
        public virtual event EventHandler<IEventArgs> Maximized;
        public virtual event EventHandler<IEventArgs> Restored;
        public virtual event EventHandler<IEventArgs> FirstShown;
        #endregion

        #region MINIMAL WINDOWS EVENTS2 EXPOSING METHODS
        protected virtual void OnMouseDoubleClick(IMouseEventArgs e) =>
            MouseDoubleClick?.Invoke(this, e);
        protected virtual void OnVisibleChanged(IEventArgs e) =>
            VisibleChanged?.Invoke(this, e);
        protected virtual void OnGotFocus(IEventArgs e) =>
            GotFocus?.Invoke(this, e);
        protected virtual void OnLostFocus(ICancelEventArgs e) =>
            LostFocus?.Invoke(this, e);

        protected virtual void OnMaximized(IEventArgs e) =>
            Maximized?.Invoke(this, e);
        protected virtual void OnMinimized(IEventArgs e) =>
            Minimized?.Invoke(this, e);
        protected virtual void OnRestored(IEventArgs e) =>
            Restored?.Invoke(this, e);
        protected virtual void OnFirstShown(IEventArgs e)
        {
            drawEventArgs.Graphics = this;
            OnPaint(drawEventArgs);
            FirstShown?.Invoke(this, e);
        }
        #endregion
    }
#endif
}
