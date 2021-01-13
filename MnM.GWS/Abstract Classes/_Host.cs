/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
#if GWS || Window
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract partial class _Host : IHost
    {
        #region VARIABLES
        readonly KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs();
        readonly DrawEventArgs DrawEventArgs = new DrawEventArgs();
        protected volatile bool isDisposed;
        #endregion

        #region PROPERTIES
        public abstract string Text { get; set; }
        public bool IsDisposed => isDisposed;
        public abstract string ID { get; }
        public string Name { get; protected set; }
        public RendererFlags RendererFlags { get; protected set; }
        public virtual bool FocusOnHover { get; set; }
        public virtual int TabIndex { get; set; }
        public IObjCollection Objects => Buffer.Objects;
        public abstract IPenContext Background { get; set; }
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;
        public bool IsContainer =>
            true;
        public abstract IntPtr Handle { get; }
        public abstract Rectangle Bounds { get; }
        public abstract bool Focused { get; }
        protected abstract ICanvas Buffer { get; }
        #endregion

        #region UPDATE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Update(Command command = 0, IRectangle boudary = null) =>
            Buffer.Update(command, boudary);
        #endregion

        #region COPY TO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual IRectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination,
            int destLen, int destW, int destX, int destY, Command command = 0, string shapeID = null)
        {
            return Buffer.CopyTo(copyX, copyY, copyW, copyH, destination, destLen, destW, destX, destY, command, shapeID);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual IRectangle CopyTo(IBlockable block, int destX, int destY, int copyX, int copyY,
            int copyW, int copyH, Command command)
        {
            return Buffer.CopyTo(block, destX, destY, copyX, copyY, copyW, copyH, command);
        }
        #endregion

        #region SHOW - HIDE
        public abstract void Show();
        public abstract void Hide();
        #endregion

        #region FOCUS
        public abstract bool Focus();
        #endregion

        #region REFRESH
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Refresh()
        {
            if (Width == 0 || Height == 0)
                return;
            DrawEventArgs.Graphics = this;
            OnPaint(DrawEventArgs);
            Buffer.Update(0, new Rectangle(0, 0, Width, Height));
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

        #region CLEAR
        void IClearable.Clear(int x, int y, int width, int height, Command command) =>
            Buffer.Clear(x, y, width, height, command);
        #endregion

        #region COPY FROM
        IRectangle IPastable.CopyFrom(IntPtr source, int srcW, int srcH, int dstX, int dstY,
            int copyX, int copyY, int copyW, int copyH, Command command, string ShapeID, IntPtr alphaBytes) =>
            Buffer.CopyFrom(source, srcW, srcH, dstX, dstY, copyX, copyY, copyW, copyH, command, ShapeID, alphaBytes);
        #endregion

        #region CONSOLIDATE
        public IRectangle Consolidate(int copyX, int copyY, int copyW, int copyH, IntPtr destination,
            int dstLen, int dstW, int dstX, int dstY, IImageData backBuffer, Command Command, IntPtr? Pen, string shapeID) =>
            Buffer.Consolidate(copyX, copyY, copyW, copyH, destination, dstLen, dstW, dstX, dstY, backBuffer, Command, Pen, shapeID);
        #endregion

        #region PUSH EVENT
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void PushEvent(IEventInfo e)
        {
#if Advanced
            Objects?.PushEvent(e);
            if (e.Handled)
                return;
#endif
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
                case GwsEvent.Shown:
                case GwsEvent.Hidden:
                    OnVisibleChanged(e.Args);
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
                    HandleAdditionalEvents(e);
                    break;
            }
            if (e.Handled)
                return;
            OnEventPushed(e);
        }

        protected virtual void OnEventPushed(IEventInfo e) =>
            EventPushed?.Invoke(this, e);

        public virtual event EventHandler<IEventInfo> EventPushed;

        partial void HandleAdditionalEvents(IEventInfo e);
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
        protected virtual void OnFirstShown(IEventArgs e) =>
            FirstShown?.Invoke(this, e);
        #endregion

        #region DISPOSE
        public abstract void Dispose();
        #endregion

        #region IBUFFER
        bool IWritable.CanNotWrite => 
            Buffer.CanNotWrite;
        int ILength.Length =>
            Buffer.Length;
        void IWritable.WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, Command command, string ShapeID, INotifier boundary) =>
           Buffer.WritePixel(val, axis, horizontal, color, Alpha, command, ShapeID, boundary);

        unsafe void IWritable.WriteLine(int* source, int srcIndex, int srcW, int length, bool horizontal,
            int x, int y, float? Alpha, byte* imageAlphas, Command command, string ShapeID, INotifier boundary) =>
            Buffer.WriteLine(source, srcIndex, srcW, length, horizontal, x, y, Alpha, imageAlphas, command, ShapeID, boundary);

        void IWritable.ClearIndices() =>
            Buffer.ClearIndices();
        #endregion
    }
}
#endif