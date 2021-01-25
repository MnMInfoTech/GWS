/* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if(GWS || Window)
using System;

namespace MnM.GWS
{
    public abstract class _MinimalEvents2 : _MinimalEvents, IMinimalEvents2
    {
        public override void PushEvent(IEventInfo e)
        {
            base.PushEvent(e);
            if (e.Handled)
                return;
            switch ((GwsEvent)e.Type)
            {
                case GwsEvent.ControllerAxisMotion:
                case GwsEvent.JoyAxisMotion:
                    OnJoystickMove(e.Args as IJoystickAxisEventArgs);
                    break;
                case GwsEvent.JoyButtonDown:
                    OnJoystickDown(e.Args as IJoystickButtonEventArgs);
                    break;
                case GwsEvent.JoyButtonUp:
                    OnJoystickUp(e.Args as IJoystickButtonEventArgs);
                    break;
                case GwsEvent.FingerDown:
                    OnTouchBegan(e.Args as ITouchEventArgs);
                    break;
                case GwsEvent.FingerMotion:
                    OnTouchBegan(e.Args as ITouchEventArgs);
                    break;
                case GwsEvent.FingerUp:
                    OnTouchBegan(e.Args as ITouchEventArgs);
                    break;
                case GwsEvent.Moved:
                    OnMoved(e.Args);
                    break;
                default:
                    break;
            }
        }

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

        protected virtual void OnMouseDrag(IMouseEventArgs e) =>
            MouseDrag?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseDrag;

        protected virtual void OnMouseDragBegin(IMouseEventArgs e) =>
            MouseDragBegin?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseDragBegin;

        protected virtual void OnMouseDragEnd(IMouseEventArgs e) =>
            MouseDragEnd?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseDragEnd;

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
        #endregion
    }
}
#endif
