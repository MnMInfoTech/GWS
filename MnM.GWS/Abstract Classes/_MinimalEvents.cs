/* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if(GWS || Window)
using System;

namespace MnM.GWS
{
    public abstract class _MinimalEvents: IMinimalEvents, IDisposable , IEventPusher
    {
        #region VARIABLES
        readonly KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs();
        #endregion

        public abstract string ID { get; }

        #region PUSH EVENT
        public virtual void PushEvent(IEventInfo e)
        {
            switch ((GwsEvent)e.Type)
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
                default:
                    return;
            }
            OnEventPushed(e);
        }
        protected virtual void OnEventPushed(IEventInfo e) =>
            EventPushed?.Invoke(this, e);

        public virtual event EventHandler<IEventInfo> EventPushed;
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

        protected virtual void OnMouseMove(IMouseEventArgs e) =>
            MouseMove?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseMove;

        protected virtual void OnMouseEnter(IMouseEventArgs e) =>
            Enter?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> Enter;

        protected virtual void OnMouseLeave(IMouseEventArgs e) =>
            Leave?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> Leave;

        protected virtual void OnAppClicked(IMouseEventArgs e) =>
            AppClicked?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> AppClicked;

        protected virtual void OnResize(ISizeEventArgs e) =>
            Resized?.Invoke(this, e);
        public event EventHandler<ISizeEventArgs> Resized;

        protected virtual void OnPaint(IDrawEventArgs e) =>
            Paint?.Invoke(this, e);
        public event EventHandler<IDrawEventArgs> Paint;

        protected virtual void OnLostFocus(ICancelEventArgs e) =>
            LostFocus?.Invoke(this, e);
        public event EventHandler<ICancelEventArgs> LostFocus;

        protected virtual void OnGotFocus(ICancelEventArgs e) =>
            GotFocus?.Invoke(this, e);
        public event EventHandler<ICancelEventArgs> GotFocus;
        #endregion

        #region DISPOSE
        public abstract void Dispose();
        #endregion
    }
}
#endif
