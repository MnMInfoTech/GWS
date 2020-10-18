/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
#if(GWS || Window)
    public abstract class _MinimalEvents: IMinimalEvents, IDisposable , IEventPusher
    {
        #region VARIABLES
        readonly KeyPressEventArgs KeyPressEventArgs = new KeyPressEventArgs();
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

        #region DISPOSE
        public abstract void Dispose();
        #endregion
    }
#endif
}
