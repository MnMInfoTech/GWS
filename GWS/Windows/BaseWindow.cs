/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)

using System;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region IBASE WINDOW
    public partial interface IBaseWindow : IParent, ICopyable, IRotatableSource, IImageData,
        IEdgeExtractable, IDisposable, IPoint, IHitTestable, ITextHolder, IFocusable, 
        ILocationHolder, IOverlap, IShowable, IHideable, IEnable, IWindowEvents,
        ICopyableObject, IPropertyBagHolder, IMessageBoxHost, ICursorManager, ISizeHolder, IMemoryOccupier
    {
        /// <summary>
        /// Indicates if this window is disposed or not.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Indicates if this window is busy at the moment or not.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Indicates if this window is minimized.
        /// </summary>
        bool IsMinimized { get; }

        /// <summary>
        /// Indicates if this window is maximized.
        /// </summary>
        bool IsMaximized { get; }
    }
    #endregion

    #region IEx BASE WINDOW
    internal partial interface IExBaseWindow : IBaseWindow, IExParent, IExResizable, IExtracter<IntPtr>,
        IAdvancePaintable, ILastMousePosition, IExPropertyManagement, IExPropertyEnabledControl
    { }
    #endregion
}

namespace MnM.GWS
{
#if DevSupport
    public
#else
    internal
#endif
     abstract partial class BaseWindow<TView> : IExBaseWindow,
        IExEventPusher, IExViewState where TView : class, IView
    {
        #region VARIABLES
        protected Location location;
        protected Size size;

        protected MemoryOccupation memoryOccupation;
        protected Point LastMousePosition;

        internal readonly IExPropertyCollection Bag;

        long DoubleClickStamp;
        protected int WaitTime = -1;

        protected readonly ICancelEventArgs CancelArgs = new CancelEventArgs();

        internal readonly IExKeyPressEventArgs KeyPressArgs = new KeyPressEventArgs();
        internal readonly IExSizeEventArgs SizeArgs = new SizeEventArgs();
        internal readonly IExPropertyChangedEventArgs PropertyArgs = new PropertyChangedEventArgs();

        #endregion

        #region CONSTRUCTOR
        protected BaseWindow(int w, int h)
        {
            Bag = new PropertyCollection(this);
            size = new Size(w, h);
            Bag.Add<MinSize>();
            Bag.Add<MaxSize>();
            Bag.Add<TextProperty>();
            Bag.Add<IFont>();
            PseudoConstructor();
        }
        protected BaseWindow(int x, int y, int w, int h) :
            this(w, h)
        {
            location = new Location(x, y);
        }
        partial void PseudoConstructor();
        #endregion

        #region PROPERTIES
        public abstract string Text { get; set; }
        public abstract IControlCollection Controls { get; }
        public IMemoryOccupation MemoryOccupation => memoryOccupation;
        public int X => location.X;
        public int Y => location.Y;
        public int Width => size.Width;
        public int Height => size.Height;
        public virtual bool Valid => Width > 0 && Height > 0;
        public IPropertyBag Properties => Bag;
        public virtual Location Location
        {
            get => location;
            set
            {
                location = value;
            }
        }
        public Size Size
        {
            get => size;
            set
            {
                if (!value)
                    return;
                ((IExResizable)this).Resize(value.Width, value.Height, out _);
            }
        }
        public bool SuspendLayout
        {
            get => (ViewState & ViewState.SuspendedLayout) == ViewState.SuspendedLayout;
            set
            {
                if (value)
                    ViewState |= ViewState.SuspendedLayout;
                else
                    ViewState &= ~ViewState.SuspendedLayout;
            }
        }
        public virtual bool Focused
        {
            get
            {
                var vs = ViewState;
                return (ViewState & ViewState.Disposed) != ViewState.Disposed
                && (vs & ViewState.Hidden) != ViewState.Hidden
                && (vs & ViewState.Disabled) != ViewState.Disabled
                && (vs & ViewState.Disposed) != ViewState.Disposed;
            }
        }
        public virtual bool Focusable
        {
            get
            {
                var vs = ((IExViewState)View).ViewState;
                return (ViewState & ViewState.Disposed) != ViewState.Disposed
                && (vs & ViewState.Hidden) != ViewState.Hidden
                && (vs & ViewState.Disabled) != ViewState.Disabled
                && (vs & ViewState.Disposed) != ViewState.Disposed;
            }
        }
        public virtual bool Visible
        {
            get
            {
                var vs = ((IExViewState)View).ViewState;
                return (ViewState & ViewState.Disposed) != ViewState.Disposed &&
                        (vs & ViewState.Hidden) != ViewState.Hidden &&
                    (vs & ViewState.Disposed) != ViewState.Disposed;
            }
            set
            {
                if ((ViewState & ViewState.Disposed) == ViewState.Disposed)
                    return;
                var vs = ((IExViewState)View).ViewState;
                bool IsVisible = (vs & ViewState.Hidden) != ViewState.Hidden;
                if (value == IsVisible)
                    return;

                if (value)
                    Show();
                else
                    Hide();
                ((IExUpdatable<ViewState, ModifyCommand>)View)?.Update
                    (ViewState.Hidden, value ? ModifyCommand.Add : ModifyCommand.Remove);
            }
        }
        public virtual bool Enabled
        {
            get
            {
                var vs = ((IExViewState)View).ViewState;
                return (ViewState & ViewState.Disposed) == ViewState.Disposed &&
                (vs & ViewState.Disabled) != ViewState.Disabled;
            }
            set
            {
                if ((ViewState & ViewState.Disposed) == ViewState.Disposed)
                    return;
                var vs = ((IExViewState)View).ViewState;
                bool IsEnabled = (vs & ViewState.Disabled) != ViewState.Disabled;
                if (value == IsEnabled)
                    return;
                ((IExUpdatable<ViewState, ModifyCommand>)View).Update
                    (ViewState.Disabled, value ? ModifyCommand.Add : ModifyCommand.Remove);

                if ((vs & ViewState.Hidden) != ViewState.Hidden)
                    Refresh();
            }
        }
        public virtual bool IsDisposed => (ViewState & ViewState.Disposed) == ViewState.Disposed;
        public virtual bool IsBusy => (ViewState & ViewState.Busy) == ViewState.Busy;
        public virtual bool IsMinimized => (ViewState & ViewState.Minimized) == ViewState.Minimized;
        public virtual bool IsMaximized => (ViewState & ViewState.Maximized) == ViewState.Maximized;

        protected abstract TView View { get; }
        internal abstract ViewState ViewState { get; set; }
        #endregion

        #region IMPLICIT INTERFACE IMPLEMENTED PROPERTIES
        IExView IViewHolder<IExView>.View => (IExView)View;
        IPoint ILastMousePosition.LastMousePosition => LastMousePosition;
        IExPropertyBag IExPropertyBagHolder.Properties => Bag;
        IExControlCollection IExControls.Controls => (IExControlCollection)Controls;
        IntPtr ISource<IntPtr>.Source => View.Source;
        ViewState IExViewState.ViewState => ViewState;
        IPoint IExRenderer.UniversalDrawOffset
        {
            get => ((IExRenderer)View).UniversalDrawOffset;
            set
            {
                ((IExRenderer)View).UniversalDrawOffset = value;
            }
        }
        #endregion

        #region CONTAINS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        public virtual bool Contains(float x, float y)
        {
            if (Width == 0 || Height == 0)
                return false;
            if (x < X || y < Y || x > (X + Width) || y > (Y + Height))
                return false;
            return true;
        }
        #endregion

        #region EXISTS
        public virtual bool Exists(IObject obj) =>
            View.Exists(obj);
        #endregion

        #region CREATE ACTION
        RenderAction IRenderer.CreateRenderAction(IEnumerable<IParameter> Parameters)
        {
            Parameters.Extract(out IExSession session);

#if Advance
            if (this is IExWindowIndex)
            {
                session.WindowIndex = ((IExWindowIndex)this).Position;
            }
#endif
            return View.CreateRenderAction(session);
        }
        #endregion

        #region CLEAR
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(IEnumerable<IParameter> Parameters)
        {
            Parameters.Extract(out IExSession session);
#if Advance
            if (this is IExWindowIndex)
            {
                session.WindowIndex = ((IExWindowIndex)this).Position;
            }
#endif
            View.Clear(session);
        }
        #endregion

        #region COPY TO
        public ISize CopyTo(IntPtr destination, int dstLen, int dstW,
            IEnumerable<IParameter> Parameters = null)
        {
            Parameters.Extract(out IExSession session);
#if Advance
            if (this is IExWindowIndex)
            {
                session.WindowIndex = ((IExWindowIndex)this).Position;
            }
#endif
            return View.CopyTo(destination, dstLen, dstW, session);
        }
        #endregion

        #region UPDATE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update(IBounds area, UpdateCommand command = 0)
        {
            View.Update(area, command);
        }
        #endregion

        #region TAKE SCREEN SHOT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool TakeScreenShot<T>(T surface, IEnumerable<IParameter> parameters = null)
            where T : IImageContext, ISource<IntPtr>, IRenderer
        {
            if (
                (ViewState & ViewState.Disposed) == ViewState.Disposed ||
                (ViewState & ViewState.Minimized) == ViewState.Minimized)
                return (false);

            int x = 0, y = 0, w = Width, h = Height;
            if (this is IWindow)
            {
                x = X;
                y = Y;
            }

            IArea clip = parameters?.LastOrDefault(p => p is IArea) as IArea;
            if (clip != null && clip.Valid)
                clip.GetBounds(out x, out y, out w, out h);

            var copyArea = new Area(x, y, w, h++);

            #region SET SURFACE
            if (surface is IExResizable)
                ((IExResizable)surface).Resize(++w, h, out _, ResizeCommand.SizeOnlyToFit);
            #endregion

            var Parameters = parameters.AppendItems(copyArea, Command.CopyRGBOnly.Add());
            var action = surface.CreateRenderAction(Parameters);
            action(null, null, View);
            return (true);
        }
        #endregion

        #region RESIZE
        object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            if ((ViewState & ViewState.Disposed) == ViewState.Disposed
                || (w == size.Width && h == size.Height) || (w == 0 && h == 0))
                return this;

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

            if (SizeOnlyToFit && Width > w && Height > h)
                return this;

            if (SizeOnlyToFit)
            {
                if (w < Width)
                    w = Width;
                if (h < Height)
                    h = Height;
            }

            if (w > Vectors.UHD8kWidth)
                w = Vectors.UHD8kWidth;
            if (h > Vectors.UHD8kHeight)
                h = Vectors.UHD8kHeight;

            var minSize = Bag.Get<MinSize>();
            var maxSize = Bag.Get<MaxSize>();
            if (!minSize.Valid && !maxSize.Valid)
            {
                goto RESIZE;
            }

            if
            (
                (maxSize.Valid && w > maxSize.Width && h > maxSize.Height) ||
                (minSize.Valid && w < minSize.Width && h < minSize.Height)
            )
            {
                return this;
            }

            if (minSize.Valid)
            {
                w = Math.Max(minSize.Width, w);
                h = Math.Max(minSize.Height, h);
            }
            if (maxSize.Valid)
            {
                w = Math.Min(maxSize.Width, w);
                h = Math.Min(maxSize.Height, h);
            }

            RESIZE:
            success = ResizeActual(w, h, resizeCommand);
            return this;
        }
        protected abstract bool ResizeActual(int w, int h, ResizeCommand resizeCommand);
        #endregion

        #region REFRESH
        public abstract bool Refresh(UpdateCommand command = 0);
        #endregion

        #region ROTATE 
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Tuple<ISize, IntPtr> RotateAndScale(IEnumerable<IParameter> Parameters)
        {
            Parameters.Extract(out IExSession session);
#if Advance
            if (this is IExWindowIndex)
            {
                session.WindowIndex = ((IExWindowIndex)this).Position;
            }
#endif
            return View.RotateAndScale(session);
        }
        #endregion

        #region EXTRACT
        IntPtr IExtracter<IntPtr>.Extract(IExSession session, out int srcW, out int srcH) =>
            ((IExtracter<IntPtr>)View).Extract(session, out srcW, out srcH);
        #endregion

        #region FOCUS
        public abstract bool Focus();
        #endregion

        #region BRING TO FRONT
        public abstract void BringToFront();
        public abstract void SendToBack();
        #endregion

        #region SHOW-HIDE
        public abstract void Show();
        public abstract void Hide(bool forceFully = false);
        #endregion

        #region PUSH EVENT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DevSupport
            protected
#else
        internal
#endif
            virtual void PushEvent(IEventInfo eventInfo)
        {
            var e = (IExEventInfo)eventInfo;
            if (e.Handled)
                return;

            e.Handled = true;
            switch (e.Type)
            {
                //case GwsEvents.GotFocus:
                //case GwsEvents.LostFocus:
                //    return;

                #region CLOSE
                case GwsEvent.Close:
                    if (!Visible)
                        return;
                    OnClosed(e.Args);
                    return;
                #endregion

                #region RESIZE
                case GwsEvent.SizeChanged:
                case GwsEvent.Resized:
                    var sz = e.Args as ISizeEventArgs;
                    ((IExResizable)this).Resize(sz.Width, sz.Height, out _);
                    return;
                #endregion

                #region LOAD
                case GwsEvent.Load:
                    if ((((IExViewState)View).ViewState & ViewState.Hidden) == ViewState.Hidden)
                        return;
                    (View as IExParent)?.Update
                        (ViewState.Busy, ModifyCommand.Add);
                    OnLoad(e.Args);
                    if (this is IWindow)
                        View.Refresh();
                    else
                    {
#if !Advanced
                        View.Refresh();
#endif
                    }
                    (View as IExParent)?.Update
                        (ViewState.Busy, ModifyCommand.Remove);
                    break;
                #endregion

                #region SHOWN - HIDDEN
                case GwsEvent.Shown:
                    CancelArgs.Cancel = false;
                    OnVisibleChanged(CancelArgs);
                    if (!CancelArgs.Cancel && (((IExViewState)View).ViewState & ViewState.FullyLoaded) !=
                        ViewState.FullyLoaded)
                        goto case GwsEvent.Load;
                    break;
                case GwsEvent.Hidden:
                    CancelArgs.Cancel = false;
                    OnVisibleChanged(CancelArgs);
                    break;
                #endregion

                #region MINIMIZE
                case GwsEvent.Minimized:
                    if (!Visible)
                        return;
                    OnMinimized(Factory.DefaultArgs);
                    return;
                #endregion

                #region MAXIMIZED
                case GwsEvent.Maximized:
                    if (!Visible)
                        return;
                    OnMaximized(Factory.DefaultArgs);
                    return;
                #endregion

                #region RESTORED
                case GwsEvent.Restored:
                    if (!Visible)
                        return;
                    OnRestored(Factory.DefaultArgs);
                    return;
                #endregion

                #region NOT CATERED
                case GwsEvent.LASTEVENT:
                case GwsEvent.First:
                case GwsEvent.Quit:
                case GwsEvent.Exposed:
                    return;
                #endregion

                default:
                    e.Handled = false;
                    break;
            }

            IMouseEventArgs m;

            #region HANDLE DRAG OPERATION
            switch (e.Type)
            {
                case GwsEvent.MouseMotion:
                case GwsEvent.MouseDown:
                case GwsEvent.MouseUp:
                    m = e.Args as IMouseEventArgs;
                    if (m == null)
                        return;
                    if (m.Button == MouseButton.Left)
                    {
                        HandleDragOperation(e);
                        if (e.Handled)
                        {
                            e.Handled = false;
                            return;
                        }
                    }
                    break;
                case GwsEvent.MouseDrag:
                    if (!(e.Args is IDragEventArgs))
                        return;
                    HandleDragOperation(e);
                    if (e.Handled)
                    {
                        e.Handled = false;
                        return;
                    }
                    break;
                default:
                    break;
            }
            #endregion

            IKeyEventArgs k;
            ICancelEventArgs Cancel;

            e.Handled = true;

            switch (e.Type)
            {
                #region KEY DOWN
                case GwsEvent.KeyDown:
                    k = e.Args as IKeyEventArgs;
                    OnPreviewKeyDown(k);
                    if (k.Cancel)
                        return;
                    OnKeyDown(k);
                    if (k.Cancel)
                        return;
                    break;
                #endregion

                #region KEY UP
                case GwsEvent.KeyUp:
                    k = e.Args as IKeyEventArgs;
                    OnKeyUp(k);
                    break;
                #endregion

                #region TEXT INPUT
                case GwsEvent.KeyPress:
                    var txtInput = e.Args as IKeyPressEventArgs;
                    if (txtInput != null)
                    {
                        OnKeyPress(txtInput);
                    }
                    break;
                #endregion

                #region GOT FOCUS
                case GwsEvent.GotFocus:
                    Cancel = (e.Args is ICancelEventArgs) ? ((ICancelEventArgs)e.Args) : CancelArgs;
                    Cancel.Cancel = false;
                    OnGotFocus(Cancel);
                    break;
                #endregion

                #region LOST FOCUS
                case GwsEvent.LostFocus:
                    Cancel = (e.Args is ICancelEventArgs) ? ((ICancelEventArgs)e.Args) : CancelArgs;
                    Cancel.Cancel = false;
                    OnLostFocus(Cancel);
                    break;
                #endregion

                #region MOUSE MOTION
                case GwsEvent.MouseMotion:
                    m = e.Args as IMouseEventArgs;
                    if (m != null)
                        OnMouseMove(m);
                    break;
                #endregion

                #region MOUSE UP
                case GwsEvent.MouseUp:
                    m = e.Args as IMouseEventArgs;
                    if (m != null)
                    {
                        SetCursorType(CursorType.Arrow);
                        OnMouseUp(m);
                        OnMouseClick(m);
                        if (Application.ElapsedMilliseconds - DoubleClickStamp < Application.DoubleClikSpeed)
                            OnMouseDoubleClick(m);
                        DoubleClickStamp = Application.ElapsedMilliseconds;
                    }
                    break;
                #endregion

                #region MOUSE DOWN
                case GwsEvent.MouseDown:
                    m = e.Args as IMouseEventArgs;
                    if (m != null)
                        OnMouseDown(m);
                    break;
                #endregion

                #region MOUSE ENTER
                case GwsEvent.MouseEnter:
                    OnMouseEnter((IMouseEnterLeaveEventArgs)e.Args);
                    break;
                #endregion

                #region MOUSE LEAVE
                case GwsEvent.MouseLeave:
                    OnMouseLeave((IMouseEnterLeaveEventArgs)e.Args);
                    break;
                #endregion

                #region MOUSE WHEEL
                case GwsEvent.MouseWheel:
                    m = e.Args as IMouseEventArgs;
                    if (m != null)
                        OnMouseWheel(m);
                    break;
                #endregion

                #region RESIZE
                case GwsEvent.Resized:
                    if (e.Args is ISizeEventArgs)
                        OnResize((ISizeEventArgs)e.Args);
                    return;
                #endregion

                #region PAINT
                case GwsEvent.Paint:
                    var paintArg = e.Args as IDrawEventArgs;
                    if (paintArg == null || paintArg.Renderer != this)
                        return;
                    OnPaintImages(paintArg);
                    return;
                #endregion

                #region JOY MOTION
                case GwsEvent.ControllerAxisMotion:
                case GwsEvent.JoyAxisMotion:
                    if (e.Args is IJoystickAxisEventArgs)
                        OnJoystickMove((IJoystickAxisEventArgs)e.Args);
                    return;
                #endregion

                #region JOY BUTTON DOWN
                case GwsEvent.JoyButtonDown:
                    if (e.Args is IJoystickButtonEventArgs)
                        OnJoystickDown((IJoystickButtonEventArgs)e.Args);
                    return;
                #endregion

                #region JOY BUTTON UP
                case GwsEvent.JoyButtonUp:
                    if (e.Args is IJoystickButtonEventArgs)
                        OnJoystickUp((IJoystickButtonEventArgs)e.Args);
                    return;
                #endregion

                #region FINGER DOWN
                case GwsEvent.FingerDown:
                    if (e.Args is ITouchEventArgs)
                        OnTouchBegan((ITouchEventArgs)e.Args);
                    return;
                #endregion

                #region FINGER MOTION
                case GwsEvent.FingerMotion:
                    if (e.Args is ITouchEventArgs)
                        OnTouchMoved((ITouchEventArgs)e.Args);
                    return;
                #endregion

                #region FINGER UP
                case GwsEvent.FingerUp:
                    if (e.Args is ITouchEventArgs)
                        OnTouchEnded((ITouchEventArgs)e.Args);
                    return;
                #endregion

                #region MOVED
                case GwsEvent.Moved:
                    OnMoved(e.Args);
                    return;
                #endregion

                #region MISC
                case GwsEvent.SizeChanged:
                    return;
                #endregion

                default:
                    break;
            }

            e.Handled = false;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IExEventPusher.PushEvent(IExEventInfo e) =>
            PushEvent(e);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        partial void HandleDragOperation(IExEventInfo e);
        #endregion

        #region SET CURSOR
        public virtual void SetCursor(int x, int y, bool silent = true) =>
            View.SetCursor(x, y, silent);
        public virtual void SetCursorType(CursorType cursor) =>
            View.SetCursorType(cursor);
        public virtual void GetCursorPos(out int x, out int y, bool global = false) =>
            View.GetCursorPos(out x, out y, global);
        public virtual void SetCursorPos(int x, int y, bool global = false) =>
            View.SetCursorPos(x, y, global);
        #endregion

        #region PROPERTY RELATED
        protected virtual void OnPropertyChanged<T>(T Property, bool Silent, string Name) where T : IProperty
        {
            switch (Name)
            {
                case "MinSize":
                case "IMinSize":
                    Property.Get(out MinSize minSize);
                    if (minSize.Valid)
                    {
                        if (Width < minSize.Width || Height < minSize.Height)
                        {
                            var w = minSize.Width;
                            var h = minSize.Height;
                            w = Math.Max(minSize.Width, w);
                            h = Math.Max(minSize.Height, h);
                            ((IExResizable)this).Resize(w, h, out _);
                        }
                    }
                    goto RAISE_EVENT;

                case "MaxSize":
                case "IMaxSize":
                    Property.Get(out MaxSize maxSize);
                    if (maxSize.Valid == true)
                    {
                        if (Width > maxSize.Width || Height > maxSize.Height)
                        {
                            var w = maxSize.Width;
                            var h = maxSize.Height;
                            w = Math.Min(maxSize.Width, w);
                            h = Math.Min(maxSize.Height, h);
                            ((IExResizable)this).Resize(w, h, out _);
                        }
                    }
                    goto RAISE_EVENT;
                case "Font":
                case "IFont":
                    Property.Get(out IFont font);
                    if (font != null)
                    {
                        foreach (var item in Controls.
                            OfType<IPropertyBagHolder>().Where(c => c.Properties.Contains<TextProperty>()))
                            item.Properties.Set(font);
                    }
                    goto RAISE_EVENT;

                default:
                    break;
            }
            RAISE_EVENT:

            if (!Silent)
            {
                PropertyArgs.Reset(Property, Silent, Name);
                OnPropertyChanged(PropertyArgs);
            }
        }

        IPrimitiveList<IParameter> IExRefreshProperties.RefreshProperties(IEnumerable<IParameter> parameters) =>
            Bag.RefreshProperties(parameters);

        void IExPropertyEnabledControl.SetRemainingProperties(IPrimitiveList<IParameter> parameters) =>
            SetRemainingProperties(parameters);
        void IExPropertyEnabledControl.OnPropertyChanged<TProperty>(TProperty Property, bool Silent, string Name) =>
            OnPropertyChanged(Property, Silent, Name);

        protected virtual void SetRemainingProperties(IPrimitiveList<IParameter> parameters)
        {
            foreach (var parameter in parameters)
            {
                if (parameter is ILocation)
                {
                    location = new Location((ILocation)parameter);
                    continue;
                }
                if (parameter is IUserSize)
                {
                    size = new Size((ISize)parameter);
                    continue;
                }
            }
        }
        IEnumerable<IProperty> IExGetProperties.GetProperties(bool privateToo) =>
            GetProperties(privateToo);
        protected virtual IEnumerable<IProperty> GetProperties(bool privateToo = false)
        {
            if (privateToo)
            {
                yield return location;
                yield return size;
#if Advance

                if (this is IExWindowIndex)
                    yield return new WindowIndex(this);
#endif
            }
            foreach (var item in Bag)
                yield return item;
        }
        #endregion

        #region SHOW MESSAGE BOX
        public MsgBoxResult ShowMessageBox(string title, int x, int y, string text, MsgBoxButtons buttons = MsgBoxButtons.YesNo)
        {
#if Window && Advance
            IExMsgBox msgBox = new MsgBox();
            return msgBox.Show(this, title, x, y, text, buttons);
#else
            return View.Target.RenderWindow.ShowMessageBox(title, x, y, text, buttons);
#endif
        }
        #endregion

        #region SHOW INPUT BOX
        public Lot<MsgBoxResult, string> ShowInputBox(string title, int x, int y, string text, MsgBoxButtons buttons = MsgBoxButtons.YesNo)
        {
#if Window && Advance
            IExInputBox msgBox = new InputBox();
            return msgBox.Show(this, title, x, y, text, buttons);
#else
            return View.Target.RenderWindow.ShowInputBox(title, x, y, text, buttons);
#endif
        }
        #endregion

        #region GATHER EDGES
        public VectorF[] GatherEdges(int X, int Y, int W, int H, params IParameter[] parameters)
        {
            if (this is IWindow)
            {
                if (X > this.X)
                    X = this.X;
                if (Y > this.Y)
                    Y = this.Y;
                if (X + W > this.X + Width)
                    W = Width - this.X;
                if (Y + H > this.Y + Height)
                    H = Height - this.Y;
            }
            return View.GatherEdges(X, Y, W, H, parameters);
        }
        #endregion

        #region SET STATE
        void IExUpdatable<ViewState, ModifyCommand>.Update(ViewState state, ModifyCommand command)
        {
            switch (command)
            {
                case ModifyCommand.Replace:
                default:
                    ViewState = state;
                    break;
                case ModifyCommand.Add:
                    ViewState |= state;
                    break;
                case ModifyCommand.Remove:
                    ViewState &= ~state;
                    break;
            }
        }
        #endregion

        #region EVENTS DECLARATION
        protected virtual void OnKeyDown(IKeyEventArgs e) =>
            KeyDown?.Invoke(this, e);
        public event EventHandler<IKeyEventArgs> KeyDown;

        protected virtual void OnKeyUp(IKeyEventArgs e) =>
            KeyUp?.Invoke(this, e);
        public event EventHandler<IKeyEventArgs> KeyUp;

        protected virtual bool OnKeyPress(IKeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
            return true;
        }
        public event EventHandler<IKeyPressEventArgs> KeyPress;

        protected virtual void OnMouseWheel(IMouseEventArgs e) =>
            MouseWheel?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseWheel;

        protected virtual void OnMouseDown(IMouseEventArgs e) =>
            MouseDown?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseDown;

        protected virtual void OnMouseUp(IMouseEventArgs e) =>
            MouseUp?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseUp;

        protected virtual void OnMouseClick(IMouseEventArgs e) =>
            MouseClick?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseClick;

        protected virtual void OnMouseDoubleClick(IMouseEventArgs e) =>
            MouseDoubleClick?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseDoubleClick;

        protected virtual void OnMouseMove(IMouseEventArgs e) =>
            MouseMove?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseMove;

        protected virtual void OnMouseEnter(IMouseEnterLeaveEventArgs e) =>
            MouseEnter?.Invoke(this, e);
        public event EventHandler<IMouseEnterLeaveEventArgs> MouseEnter;

        protected virtual void OnMouseLeave(IMouseEnterLeaveEventArgs e) =>
            MouseLeave?.Invoke(this, e);
        public event EventHandler<IMouseEnterLeaveEventArgs> MouseLeave;

        protected virtual void OnResize(ISizeEventArgs e) =>
            Resized?.Invoke(this, e);
        public event EventHandler<ISizeEventArgs> Resized;

        protected virtual bool OnPaintImages(IDrawEventArgs e)
        {
            PaintImages?.Invoke(this, e);
            return (true);
        }
        public event EventHandler<IDrawEventArgs> PaintImages;
        void IAdvancePaintable.OnPaintImages(IDrawEventArgs e) =>
            OnPaintImages(e);
        protected virtual void OnLostFocus(ICancelEventArgs e) =>
            LostFocus?.Invoke(this, e);
        public event EventHandler<ICancelEventArgs> LostFocus;

        protected virtual void OnGotFocus(ICancelEventArgs e) =>
            GotFocus?.Invoke(this, e);
        public event EventHandler<ICancelEventArgs> GotFocus;

        protected virtual void OnVisibleChanged(ICancelEventArgs e) =>
            VisibleChanged?.Invoke(this, e);
        public event EventHandler<ICancelEventArgs> VisibleChanged;
        #endregion

        #region EVENT2 DECLARATION
        protected virtual void OnMoved(IEventArgs e) =>
            Moved?.Invoke(this, e);
        public event EventHandler<IEventArgs> Moved;

        protected virtual void OnPreviewKeyDown(IKeyEventArgs e) =>
            PreviewKeyDown?.Invoke(this, e);
        public event EventHandler<IKeyEventArgs> PreviewKeyDown;

        protected virtual void OnTouchBegan(ITouchEventArgs e) =>
            TouchBegan?.Invoke(this, e);
        public event EventHandler<ITouchEventArgs> TouchBegan;

        protected virtual void OnTouchMoved(ITouchEventArgs e) =>
            TouchMoved?.Invoke(this, e);
        public event EventHandler<ITouchEventArgs> TouchMoved;

        protected virtual void OnTouchEnded(ITouchEventArgs e) =>
            TouchEnded?.Invoke(this, e);
        public event EventHandler<ITouchEventArgs> TouchEnded;

        protected virtual void OnJoystickDown(IJoystickButtonEventArgs e) =>
            JoystickDown?.Invoke(this, e);
        public event EventHandler<IJoystickButtonEventArgs> JoystickDown;

        protected virtual void OnJoystickUp(IJoystickButtonEventArgs e) =>
            JoystickUp?.Invoke(this, e);
        public event EventHandler<IJoystickButtonEventArgs> JoystickUp;

        protected virtual void OnJoystickMove(IJoystickAxisEventArgs e) =>
            JoystickMove?.Invoke(this, e);
        public event EventHandler<IJoystickAxisEventArgs> JoystickMove;

        protected virtual void OnJoystickConnected(IEventArgs e) =>
            JoystickConnected?.Invoke(this, e);
        public event EventHandler<IEventArgs> JoystickConnected;

        protected virtual void OnJoystickDisconnected(IEventArgs e) =>
            JoystickDisconnected?.Invoke(this, e);
        public event EventHandler<IEventArgs> JoystickDisconnected;

        protected virtual void OnPropertyChanged(IPropertyChangedEventArgs e) =>
            PropertyChanged?.Invoke(this, e);
        public event EventHandler<IPropertyChangedEventArgs> PropertyChanged;
        #endregion

        #region EVENT4 DECLARATIONS
        protected virtual void OnMinimized(IEventArgs e) =>
           Minimized?.Invoke(this, e);
        public event EventHandler<IEventArgs> Minimized;

        protected virtual void OnMaximized(IEventArgs e) =>
            Maximized?.Invoke(this, e);
        public event EventHandler<IEventArgs> Maximized;

        protected virtual void OnRestored(IEventArgs e) =>
            Restored?.Invoke(this, e);
        public event EventHandler<IEventArgs> Restored;

        protected virtual void OnLoad(IEventArgs e) =>
            Load?.Invoke(this, e);
        public event EventHandler<IEventArgs> Load;

        protected virtual void OnTitaleChanged(IEventArgs e) =>
            TitleChanged?.Invoke(this, e);
        public event EventHandler<IEventArgs> TitleChanged;

        protected virtual void OnClosed(IEventArgs e) =>
            Closed?.Invoke(this, e);
        public event EventHandler<IEventArgs> Closed;
        #endregion

        #region DISPOSE
        public abstract void Dispose();
        #endregion
    }
}
#endif
