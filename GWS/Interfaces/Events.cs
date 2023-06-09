/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)
using System;

namespace MnM.GWS
{
    #region IDIMENSION EVENTS
    public interface IDimensionalEvents
    {
        event EventHandler<ISizeEventArgs> Resized;
        event EventHandler<IEventArgs> Moved;
    }
    #endregion

    #region IFOCUS EVENTS
    public interface IFocusEvents
    {
        event EventHandler<ICancelEventArgs> LostFocus;
        event EventHandler<ICancelEventArgs> GotFocus;
    }
    #endregion

    #region IPROPERTY EVENT
    public interface IPropertyEvents
    {
        event EventHandler<IPropertyChangedEventArgs> PropertyChanged;
    }
    #endregion

    #region IPARENT CHANGE EVENT
    public interface IParentChangeEvent
    {
        event EventHandler<IEventArgs> ParentChanged;
    }
    #endregion

    #region IMOUSE AND KEY EVENTS
    public interface IMouseKeyEvents
    {
        event EventHandler<IKeyEventArgs> PreviewKeyDown;
        event EventHandler<IKeyEventArgs> KeyDown;
        event EventHandler<IKeyEventArgs> KeyUp;
        event EventHandler<IKeyPressEventArgs> KeyPress;

        event EventHandler<IMouseEventArgs> MouseWheel;
        event EventHandler<IMouseEventArgs> MouseDown;
        event EventHandler<IMouseEventArgs> MouseUp;
        event EventHandler<IMouseEventArgs> MouseMove;
        event EventHandler<IMouseEventArgs> MouseClick;
        event EventHandler<IMouseEventArgs> MouseDoubleClick;
        event EventHandler<IMouseEnterLeaveEventArgs> MouseEnter;
        event EventHandler<IMouseEnterLeaveEventArgs> MouseLeave;
    }
    #endregion

    #region IJOYSTICKS EVENTS
    public interface IJoySticksEvents
    {
        event EventHandler<ITouchEventArgs> TouchBegan;
        event EventHandler<ITouchEventArgs> TouchMoved;
        event EventHandler<ITouchEventArgs> TouchEnded;
        event EventHandler<IJoystickButtonEventArgs> JoystickDown;
        event EventHandler<IJoystickButtonEventArgs> JoystickUp;
        event EventHandler<IJoystickAxisEventArgs> JoystickMove;
        event EventHandler<IEventArgs> JoystickConnected;
        event EventHandler<IEventArgs> JoystickDisconnected;
    }
    #endregion

    #region IWINDOW EVENTS
    public partial interface IWindowEvents : IMouseKeyEvents, IDimensionalEvents,
        IFocusEvents, IJoySticksEvents, IPropertyEvents
    {
        event EventHandler<IDrawEventArgs> PaintImages;
        event EventHandler<IEventArgs> Minimized;
        event EventHandler<IEventArgs> Maximized;
        event EventHandler<IEventArgs> Restored;
        event EventHandler<IEventArgs> Load;
        event EventHandler<IEventArgs> Closed;
        event EventHandler<IEventArgs> TitleChanged;
    }
    #endregion

    #region IADVANCE-PAINTABLE
    internal interface IAdvancePaintable
    {
        /// <summary>
        /// Invoke paint events to handle external paint routines defined by the user.
        /// </summary>
        void OnPaintImages(IDrawEventArgs e);
    }
    #endregion

    #region EVENT
    public interface IExternalEventInfo
    {
        string ID { get; }
        GwsEvent Type { get; }
        IWindowEventInfo Window { get; }
    }
    internal interface IExExternalEventInfo : IExternalEventInfo { }
    #endregion

    #region EXTEVENT
    public interface IWindowEventInfo
    {
        GwsEvent Type { get; }
        int WindowID { get; }
        int Event { get; }
    }
    #endregion

    #region IEVENT-PROCESSOR
    internal interface IEventProcessor : IID<string>, IDisposable, IEventParser
    {
        bool ProcessEvent(IExternalEventInfo @event);
    }
    #endregion

    #region IEVENT PARSER
#if (DevSupport || DLLSupport)
    public
#else
    internal
#endif
    interface IEventParser
    {
        IEventArgs ParseEvent(IExternalEventInfo @event);
    }
    #endregion

    #region IEVENTPOLLER
#if (DevSupport || DLLSupport)
    public
#else
    internal
#endif
    interface IEventPoller
    {
        /// <summary>
        /// X offset of visible screen.
        /// </summary>
        int BitsPerPixel { get; }

        /// <summary>
        /// Working size of the screen.
        /// </summary>
        ISize WorkingSize { get; }

        /// <summary>
        /// Gives current event happeed on active window.
        /// </summary>
        /// <param name="e">Event which is just happened</param>
        /// <returns></returns>
        bool PollEvent(out IExternalEventInfo e);
    }
    #endregion

    #region IExEVENTPUSHER
    internal interface IExEventPusher : IObject, IExist<IObject>
    {
        void PushEvent(IExEventInfo e);
    }
    #endregion
}
#endif