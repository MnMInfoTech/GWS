/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if AllHidden
    partial class NativeFactory
    {
#else
    public
#endif
       partial class NativeForm : IForm
        {
            #region VARIABLES
            readonly ICanvas Canvas;
            readonly INativeTarget Target;

            readonly bool IsEventPusher;

            readonly KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs();
            readonly DrawEventArgs drawEventArgs = new DrawEventArgs();
            readonly EventInfo drawEventInfo = new EventInfo();

            const int formX = 602, formY = 200, formW = 404, formH = 506;
            #endregion

            #region CONSTRCUTORS
            public NativeForm(INativeTarget target)
            {
                Target = target;
                Canvas = Factory.newCanvas(target);
                Target.Form = this;
                IsEventPusher = Canvas.Objects is IEventPusher;
            }
            public NativeForm(int formW, int formH) :
                this(Factory.newNativeTarget(formW, formH))
            { }
            public NativeForm() :
                this(Factory.newNativeTarget(formW, formH))
            { }
            public NativeForm(int formX, int formY, int formW, int formH) :
                this(Factory.newNativeTarget(formX, formY, formW, formH))
            { }
            #endregion

            #region PROPERTIES
            public IPenContext Background
            {
                get => Canvas.Background;
                set => Canvas.Background = value;
            }
            public bool CanNotWrite => Canvas.CanNotWrite;
            public string ID => Target.ID;
            public bool IsDisposed => Target.IsDisposed;
            public IntPtr Handle => Target.Handle;
            public int Width => Canvas.Width;
            public int Height => Canvas.Height;
            public IObjCollection Objects => Canvas.Objects;
            public string Name => Target.Name;
            public string Text
            {
                get => Target.Text;
                set => Target.Text = value;
            }
            int ILength.Length => Canvas.Length;

#if Advanced
            public bool SupportBackgroundBuffer
            {
                get => Canvas.SupportBackgroundBuffer;
                set => Canvas.SupportBackgroundBuffer = value;
            }
            public bool Clipped => Canvas.Clipped;
            public IRectangle ClipRectangle
            {
                get => Canvas.ClipRectangle;
                set => Canvas.ClipRectangle = value;
            }
#endif
            #endregion

#if Advanced
            #region GET DATA
            public void GetData(out int[] Pixels, out byte[] Alphas, bool BackgroundBuffer = false)
            {
                Canvas.GetData(out Pixels, out Alphas, BackgroundBuffer);
            }
            #endregion
#endif
            #region CONSOLIDATE
            public IRectangle Consolidate(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int dstLen,
                int dstW, int dstX, int dstY, IImageData backBuffer, Command Command = Command.None, IntPtr? Pen = null, string shapeID = null)
            {
                return Canvas.Consolidate(copyX, copyY, copyW, copyH, destination, dstLen, dstW, dstX, dstY, backBuffer, Command, Pen, shapeID);
            }
            #endregion

            #region WRITE PIXEL
            public void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, Command Command, string ShapeID, INotifier boundary)
            {
                Canvas.WritePixel(val, axis, horizontal, color, Alpha, Command, ShapeID, boundary);
            }
            #endregion

            #region WRITE LINE
            public unsafe void WriteLine(int* colors, int srcIndex, int srcW, int length, bool horizontal, int x, int y, float? Alpha, byte* imageAlphas, Command Command, string ShapeID, INotifier boundary)
            {
                Canvas.WriteLine(colors, srcIndex, srcW, length, horizontal, x, y, Alpha, imageAlphas, Command, ShapeID, boundary);
            }
            #endregion

            #region CLEAR INDICES
            public void ClearPixelRecord()
            {
                Canvas.ClearPixelRecord();
            }
            #endregion

            #region COPY TO
            public IRectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int dstLen, int dstW, int dstX, int dstY, Command command, string ShapeID = null)
            {
                return Canvas.CopyTo(copyX, copyY, copyW, copyH, destination, dstLen, dstW, dstX, dstY, command, ShapeID);
            }
            #endregion

            #region DISPOSE
            public void Dispose()
            {
                Canvas.Dispose();
            }
            #endregion

            #region CLEAR
            public IRectangle Clear(int clearX, int clearY, int clearW, int clearH, Command command = Command.None)
            {
               return Canvas.Clear(clearX, clearY, clearW, clearH, command);
            }
            #endregion

            #region COPY FROM
            public IRectangle CopyFrom(IntPtr source, int srcW, int srcH, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, Command Command, string ShapeID, IntPtr alphaBytes = default)
            {
                return Canvas.CopyFrom(source, srcW, srcH, dstX, dstY, copyX, copyY, copyW, copyH, Command, ShapeID, alphaBytes);
            }
            #endregion

            #region RESIZE
            public void Resize(int? newWidth = null, int? newHeight = null)
            {
                Canvas.Resize(newWidth, newHeight);
            }
            #endregion

            #region UPDATE
            public void Update(Command command = Command.None, IRectangle boundary = null)
            {
                Canvas.Update(command, boundary);
            }
            #endregion

            #region REFRESH
            public void Refresh()
            {
                Canvas.Refresh();
            }
            #endregion

#if Advanced
            #region FIND ELEMENT
            public IRenderable FindElement(int x, int y)
            {
                return Canvas.FindElement(x, y);
            }
            #endregion
#endif

            #region BACKGROUND CHANGED
            public event EventHandler<IEventArgs> BackgroundChanged
            {
                add => Canvas.BackgroundChanged += value;
                remove => Canvas.BackgroundChanged -= value;
            }
            #endregion

            #region SHOW - HIDE
            public void Show() =>
                Target.Show();
            public void Hide() =>
                Target.Hide();
            #endregion

            #region PUSH EVENT
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public virtual void PushEvent(IEventInfo e)
            {
                if (IsEventPusher)
                    ((IEventPusher)Canvas.Objects).PushEvent(e);

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
#if AllHidden
    }
#endif
}
