/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if Window
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract partial class _Window : _Events, IWindow
    {
#region VARIABLES
        readonly ICanvas Primary;
        ICanvas Current;
        protected bool IsEventPusher;
        protected bool previousCursorVisible = true;
        protected bool cursorVisible;
        bool firstShow;
        protected Rectangle bounds;
        protected bool focused;

        readonly DrawEventArgs drawEventArgs = new DrawEventArgs();
        readonly EventInfo Event = new EventInfo();
        protected volatile bool isDisposed;
        readonly IExternalTarget Target;
        readonly string typeName;
#endregion

#region CONSTRUCTORS
        _Window(int width, int height)
        {
            bounds = new Rectangle(0, 0, width, height);
            GetTypeName(out typeName);
        }
        protected _Window(IExternalTarget target) :
            this(target.Width, target.Height)
        {
            if (!Initialize(externalWindow: target))
                throw new Exception("Window could not be initialized!");
            Target = target;
            Initialize(out Primary, null, target);
            IsEventPusher = true;
        }
        protected _Window(string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null,
            RendererFlags? renderFlags = null) :
            this(width ?? 100, height ?? 100)
        {
            if (!Initialize(title, width, height, x, y, flags, display, renderFlags: renderFlags))
                throw new Exception("Window could not be initialized!");
            Initialize(out Primary, flags);
        }

        protected abstract bool Initialize(string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null,
            IExternalTarget externalWindow = null, RendererFlags? renderFlags = null);

        void Initialize(out ICanvas Canvas, GwsWindowFlags? flags, IRenderTarget target = null)
        {
            Canvas = Factory.newCanvas(target ?? Factory.newRenderTarget(this));
            Current = Canvas;
            InitializeBufferCollection();
            GwsWindowFlags = flags ?? 0;
            if (GwsWindowFlags.HasFlag(GwsWindowFlags.OpenGL))
                GLContext = Factory.newGLContext(this);
            WindowID = Factory.GetWindowID(Handle);
            Name = "Window" + WindowID;
            this.Register();
        }
        partial void InitializeBufferCollection();

        /// <summary>
        /// Gets type of this window.
        /// Please make sure that it matched with window id coming from event from PollEvent.
        /// </summary>
        /// <param name="typeName"></param>
        protected abstract void GetTypeName(out string typeName);
#endregion

#region PROPERTIES
        public virtual string Text { get; set; }
        public string Name { get; protected set; }
        public Rectangle Bounds => bounds;
        public GwsWindowFlags GwsWindowFlags { get; private set; }
        public IGLContext GLContext { get; private set; }
        public int WindowID { get; private set; }
        public int X => bounds.X;
        public int Y => bounds.Y;
        public IPenContext Background
        {
            set => Canvas.Background = value;
        }
        public int Width => bounds.Width;
        public int Height => bounds.Height;
        public bool IsContainer =>
            true;
        public bool IsDisposed => isDisposed;
        public virtual bool FocusOnHover { get; set; }
        public virtual int TabIndex { get; set; }
        public RendererFlags RendererFlags { get; protected set; }

        public bool IsWindow => true;
        public bool IsMouseDragging { get; protected set; }
        public bool Transparent => Transparency != 0f;
        public virtual IScreen Screen { get; set; }
        public virtual float Transparency { get; set; }
        public virtual WindowState WindowState { get; protected set; }
        public virtual WindowBorder WindowBorder { get; protected set; }
        public virtual VectorF Scale { get; set; }
        public virtual bool CursorVisible { get; set; }
        public string ID => typeName + WindowID;
        protected CursorType? ResizeCursor { get; set; }

        public abstract IntPtr Handle { get; }
        public abstract bool Focused { get; }
        public abstract Size MinSize { get; set; }
        public abstract Size MaxSize { get; set; }
        public abstract uint PixelFormat { get; }
        public abstract ISound Sound { get; }
        public abstract bool Visible { get; set; }
        public abstract bool Enabled { get; set; }
        protected ICanvas Canvas => Current;
        public bool Freezed
        {
            get => Canvas.Freezed;
        }
#endregion

#region RENDER
        public void Render(IRenderable Renderable, ISettings Settings)
        {
            Canvas.Render(Renderable, Settings);
        }
#endregion

#region UPDATE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Update(Command command, IPerimeter boudary) =>
            Canvas.Update(command, boudary);
#endregion

#region COPY TO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual IPerimeter CopyTo(IntPtr destination,
            int destLen, int destW, int destX, int destY, IPerimeter copyArea, Command command = 0)
        {
            return Canvas.CopyTo(destination, destLen, destW, destX, destY, copyArea, command);
        }
#endregion

#region FOCUS
        public abstract bool Focus();
#endregion

#region REFRESH
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Refresh(Command command = 0)
        {
            if (Width == 0 || Height == 0 || !Visible)
                return;
            Canvas.Refresh(command);
        }
#endregion

#region RAISE PAINT
        public void InvokePaint(Command command = 0, int processID = 0)
        {
            drawEventArgs.Graphics = Canvas;
            drawEventArgs.ProcessID = processID;
            OnPaint(drawEventArgs);
            if ((command & Command.InvalidateOnly) != Command.InvalidateOnly)
                Update(command, new Perimeter(0, 0, Width, Height, processID));
        }
#endregion

#region RESIZE
        public virtual void Resize(int? width, int? height)
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
            bounds = new Rectangle(bounds.X, bounds.Y, e.Width, e.Height);
            Primary?.Resize(e.Width, e.Height);
            Resize2();
            base.OnResize(e);
        }
        partial void Resize2();
#endregion

#region CLEAR
        public IPerimeter Clear(IPerimeter clear, Command command) =>
            Canvas.Clear(clear, command);
#endregion

#region CONSOLIDATE
        public IPerimeter Consolidate(IntPtr destination,
            int dstLen, int dstW, int dstX, int dstY, IPerimeter copyArea, IMultiBuffered backBuffer, Command Command, IntPtr? Pen) =>
            Canvas.Consolidate(destination, dstLen, dstW, dstX, dstY, copyArea, backBuffer, Command, Pen);
#endregion

#region ROTATE -FLIP
        public Size RotateAndScale(out IntPtr Data, Rotation angle, bool antiAliased = true, float scale = 1)
        {
            return Canvas.RotateAndScale(out Data, angle, antiAliased, scale);
        }

        public Size Flip(out IntPtr Data, FlipMode flipMode)
        {
            return Canvas.Flip(out Data, flipMode);
        }
#endregion

#region SHOW - HIDE
        public void Show() =>
            ChangeVisible(true);
        public void Hide() =>
            ChangeVisible(false);
        protected virtual void ChangeVisible(bool value)
        {
            var e = Factory.EmptyArgs;
            if (!firstShow && value)
            {
                firstShow = true;
                Refresh(Command.AddMode);
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
                case GwsEvent.Minimized:
                    OnMinimized(Factory.EmptyArgs);
                    break;
                case GwsEvent.Maximized:
                    OnMaximized(Factory.EmptyArgs);
                    break;
                case GwsEvent.Restored:
                    OnRestored(Factory.EmptyArgs);
                    break;

                case GwsEvent.Resized:
                    //case GwsEvent.SIZE_CHANGED:
                    base.PushEvent(e);
                    break;
                default:
#if Advanced
                    Canvas?.PushEvent(e);
                    if (e.Handled)
                        return;
#endif
                    if (IsEventPusher)
                        Target.PushEvent(e);
                    else
                        base.PushEvent(e);
                    break;
            }
        }
        protected abstract IEventArgs ParseEvent(IEvent @event);
#endregion

#region WINDOW EVENT DECLARATION WINDOW
        protected virtual void OnTitaleChanged(IEventArgs e) =>
            TitleChanged?.Invoke(this, e);
        public event EventHandler<IEventArgs> TitleChanged;

        protected virtual void OnWindowBorderChanged(IEventArgs e) =>
            WindowBorderChanged?.Invoke(this, e);
        public event EventHandler<IEventArgs> WindowBorderChanged;

        protected virtual void OnWindowStateChanged(IEventArgs e) =>
            WindowStateChanged?.Invoke(this, e);
        public event EventHandler<IEventArgs> WindowStateChanged;

        protected virtual void OnClosing(ICancelEventArgs e) =>
            Closing?.Invoke(this, e);
        public event EventHandler<ICancelEventArgs> Closing;

        protected virtual void OnClosed(IEventArgs e) =>
            Closed?.Invoke(this, e);
        public event EventHandler<IEventArgs> Closed;
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

#region IIMAGE
        int ILength.Length =>
            Canvas.Length;
        
        void IWritable.WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, Command command, ISession boundary) =>
           Canvas.WritePixel(val, axis, horizontal, color, Alpha, command, boundary);
       
           unsafe void IWritable.WriteLine(int* source, int srcIndex, int srcW, int length, bool horizontal,
            int x, int y, float? Alpha, byte* imageAlphas, Command command, ISession boundary) =>
            Canvas.WriteLine(source, srcIndex, srcW, length, horizontal, x, y, Alpha, imageAlphas, command, boundary);
       
            IPerimeter IWritableBlock.WriteBlock(IntPtr source, int srcW, int srcH, int dstX, int dstY,
            IPerimeter copyArea, Command command, IntPtr alphaBytes) =>
            Canvas.WriteBlock(source, srcW, srcH, dstX, dstY, copyArea, command, alphaBytes);

        ReadChoice IReadable.Choice
        { 
            get => Canvas.Choice; 
            set => Canvas.Choice = value; 
        }

        int IReadable.ReadPixel(int x, int y) =>
            Canvas.ReadPixel(x, y);
        void IReadable.ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length) =>
            Canvas.ReadLine(start, end, axis, horizontal, out pixels, out srcIndex, out length);
#endregion
    }

    partial class _Window: IObjCollection
    {
#region PROPERTIES
        public int ObjectCount => Canvas.ObjectCount;
        public IRenderable this[uint id] => Canvas[id];
        public IRenderable this[string name] => Canvas[name];
        public ISettings this[IRenderable shape] => Canvas[shape];
#endregion

#region CONTAINS
        public bool Contains(IRenderable item)
        {
            return Canvas.Contains(item);
        }
        public bool Contains(uint itemID)
        {
            return Canvas.Contains(itemID);
        }
#endregion

#region ADD
        public U Add<U>(U shape, ISettings settings, bool? suspendUpdate = null) where U : IRenderable
        {
            return Canvas.Add(shape, settings, suspendUpdate);
        }
        public U Add<U>(U shape) where U : IRenderable
        {
            return Canvas.Add(shape);
        }
        public void AddRange<U>(IEnumerable<U> controls) where U : IRenderable
        {
            Canvas.AddRange(controls);
        }
#endregion

#region REMOVE
        public bool Remove(IRenderable item)
        {
            return Canvas.Remove(item);
        }
        public void RemoveAll()
        {
            Canvas.RemoveAll();
        }
#endregion

#region ENUMERATOR
        public IEnumerator<IRenderable> GetEnumerator()
        {
            return Canvas.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Canvas).GetEnumerator();
        }
#endregion

#region QUERY
        public IEnumerable<IRenderable> Query(Predicate<ISettings> condition = null)
        {
            return Canvas.Query(condition);
        }
        public IRenderable QueryFirst(Predicate<ISettings> condition = null)
        {
            return Canvas.QueryFirst(condition);
        }
        public IEnumerable<IShape> QueryDraw(Predicate<ISettings> condition = null)
        {
            return Canvas.QueryDraw(condition);
        }
        public IShape QueryFirstDraw(Predicate<ISettings> condition = null)
        {
            return Canvas.QueryFirstDraw(condition);
        }
#endregion
    }
}
#endif
