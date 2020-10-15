/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
#if (GWS || Window)
    public abstract class _Events : _MinimalEvents , IEvents,  IEventPusher
    {
        #region PUSH EVENT
        public override void PushEvent(IEventInfo e)
        {
            base.PushEvent(e);
            if (e.Status == EventUseStatus.Used)
                return;
            switch (e.Type)
            {
                case GwsEvent.SHOWN:
                case GwsEvent.HIDDEN:
                    OnVisibleChanged(e.Args);
                    break;
                case GwsEvent.MOVED:
                    OnMoved(e.Args as IEventArgs);
                    break;
                case GwsEvent.ENTER:
                    OnMouseEnter(e.Args as IMouseEventArgs);
                    break;
                case GwsEvent.LEAVE:
                    OnMouseLeave(e.Args as IMouseEventArgs);
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
                    break;
            }
            if (e.Status == EventUseStatus.Used)
                return;
            OnEventPushed(e);
        }
        #endregion

        #region EVENT2 DECLARATION
        public virtual event EventHandler<ICancelEventArgs> LostFocus;
        public virtual event EventHandler<IEventArgs> GotFocus;
        public virtual event EventHandler<IEventArgs> Moved;
        public virtual event EventHandler<IKeyEventArgs> PreviewKeyDown;
        public virtual event EventHandler<IMouseEventArgs> MouseDoubleClick;
        public virtual event EventHandler<IEventArgs> MouseEnter;
        public virtual event EventHandler<IEventArgs> MouseLeave;
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
        protected virtual void OnMouseEnter(IMouseEventArgs e) =>
            MouseEnter?.Invoke(this, e);
        protected virtual void OnMouseLeave(IMouseEventArgs e) =>
            MouseLeave?.Invoke(this, e);
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
    }
#endif
}
