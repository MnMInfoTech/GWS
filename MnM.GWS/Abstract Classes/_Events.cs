using System;

namespace MnM.GWS
{
    public abstract class _Events: _MinimalEvents2, IMinimalEvents, IMinimalWindowEvents
    {
        #region VARIABLES
        protected readonly KeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs();
        #endregion

        #region PUSH EVENT
        public override void PushEvent(IEventInfo e)
        {
            switch ((GwsEvent)e.Type)
            {
                case GwsEvent.Shown:
                case GwsEvent.Hidden:
                    OnVisibleChanged(e.Args);
                    break;
                case GwsEvent.FocusGained:
                    if (!(e.Args is ICancelEventArgs))
                        e = new EventInfo(e.Sender, new CancelEventArgs(), (int)GwsEvent.FocusGained);
                    OnGotFocus(e.Args as ICancelEventArgs);
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
                    break;
            }
            if (e.Handled)
                return;
            base.PushEvent(e);
        }
        #endregion

        #region EVENT DECLARATION
        protected virtual void OnLostFocus(ICancelEventArgs e) =>
            LostFocus?.Invoke(this, e);
        public event EventHandler<ICancelEventArgs> LostFocus;
      
        protected virtual void OnGotFocus(ICancelEventArgs e) =>
            GotFocus?.Invoke(this, e);
        public event EventHandler<ICancelEventArgs> GotFocus;

        protected virtual void OnMouseDoubleClick(IMouseEventArgs e) =>
            MouseDoubleClick?.Invoke(this, e);
        public event EventHandler<IMouseEventArgs> MouseDoubleClick;
      
        protected virtual void OnVisibleChanged(IEventArgs e) =>
            VisibleChanged?.Invoke(this, e);
        public event EventHandler<IEventArgs> VisibleChanged;

        protected virtual void OnMinimized(IEventArgs e) =>
            Minimized?.Invoke(this, e);
        public event EventHandler<IEventArgs> Minimized;

        protected virtual void OnMaximized(IEventArgs e) =>
            Maximized?.Invoke(this, e);
        public event EventHandler<IEventArgs> Maximized;

        protected virtual void OnRestored(IEventArgs e) =>
            Restored?.Invoke(this, e);
        public event EventHandler<IEventArgs> Restored;

        protected virtual void OnFirstShown(IEventArgs e) =>
            FirstShown?.Invoke(this, e);
        public event EventHandler<IEventArgs> FirstShown;
        #endregion
    }
}
