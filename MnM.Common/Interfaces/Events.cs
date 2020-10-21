/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
#if Texts || GWS || Window 
    #region IKEYPRESSEVENT-ARGS
    /// <summary>
    /// Represents an argument object to relay key press information.
    /// </summary>
    public interface IKeyPressEventArgs : IEventArgs
    {
        /// <summary>
        /// Gets character on keyboard that is pressed.
        /// </summary>
        char KeyChar { get; set; }
    }
    #endregion

    #region IKEYEVENT-ARGS
    /// <summary>
    /// Represents an argument object to relay key input information.
    /// </summary>
    public interface IKeyEventArgs : IInputEventArgs
    {
        /// <summary>
        /// The code of key pressed or released.
        /// </summary>
        Key KeyCode { get; }

        /// <summary>
        /// Relative Scancode for the key.
        /// </summary>
        int ScanCode { get; }

        /// <summary>
        /// Indicates whether an Alt key is pressed or released.
        /// </summary>
        bool Alt { get; }

        /// <summary>
        /// Indicates whether a Control key is pressed or released.
        /// </summary>
        bool Control { get; }

        /// <summary>
        /// Indicates whether a Shift key is pressed or released.
        /// </summary>
        bool Shift { get; }

        /// <summary>
        /// Indicates whether any modifier key is pressed or released.
        /// </summary>
        Key Modifiers { get; }

        /// <summary>
        /// Current state of key - i.e up or down.
        /// </summary>
        KeyState State { get; }

        /// <summary>
        /// If true, supress the current key press - i.e chosing to ignore it.
        /// </summary>
        bool SupressKeypress { get; set; }
    }
    #endregion

    #region ITEXTINPUT-EVENT-ARGS
    /// <summary>
    /// Reprsents an argument object which contains information of text input.
    /// </summary>
    public interface ITextInputEventArgs : IEventArgs
    {
        /// <summary>
        /// Collection of characters entered.
        /// </summary>
        char[] Characters { get; }
    }
    #endregion
#endif

#if (GWS || Window) 
    #region IMOUSEEVENT-ARGS
    /// <summary>
    /// Represents an argument object to relay mouse input information.
    /// </summary>
    public interface IMouseEventArgs : IInputEventArgs
    {
        /// <summary>
        /// X cor-ordinate of the location of mouse.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Y cor-ordinate of the location of mouse.
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Indicates state of mouse - i.e up or down or hovering or clicked etc.
        /// </summary>
        MouseState State { get; }

        /// <summary>
        /// Indicates which butten is currently in play i.e left, right or middle etc.
        /// </summary>
        MouseButton Button { get; }

        /// <summary>
        /// Indicates numner of clicks 1 for single and 2 for double.
        /// </summary>
        int Clicks { get; }

        /// <summary>
        ///Indicates a signed count of the number of detents the mouse wheel has rotated.   
        /// </summary>
        int Delta { get; }

        /// <summary>
        /// Indicates if mouse is cliecked.
        /// </summary>
        bool Clicked { get; }

        /// <summary>
        /// While dragging, this indicated X co-rdinate of start point of start of dragging.
        /// </summary>
        int DragStartX { get; }

        /// <summary>
        /// While dragging, this indicated Y co-rdinate of start point of start of dragging.
        /// </summary>
        int DragStartY { get; }

        /// <summary>
        /// Deviation of wheel in horizontal axis.
        /// </summary>
        int XDelta { get; }

        /// <summary>
        /// Deviation of wheel in vertical axis.
        /// </summary>
        int YDelta { get; }
    }
    #endregion

    #region ISIZEEVENT-ARGS
    public interface ISizeEventArgs : IEventArgs, ISize
    {
    }
    #endregion

    #region IPAINTEVENT-ARGS
    /// <summary>
    /// Represents an argument object to expose underlying graphics object to draw on.
    /// </summary>
    public interface IDrawEventArgs : IEventArgs, IDisposable
    {
        /// <summary>
        /// Underlying surface object to draw.
        /// </summary>
        IBlock Surface { get; }
    }
    #endregion

    #region ITICKEVENT-ARGS
    public interface ITickEventArgs : IEventArgs
    {
        int LastTick { get; }
        int Fps { get; }
        int Tick { get; }
        int TicksElapsed { get; }
        float SecondsElapsed { get; }
    }
    #endregion

    #region IFILEDROP-EVENTARGS
    public interface IFileDropEventArgs : IEventArgs
    {
        string FileName { get; set; }
    }
    #endregion

    #region IFRAME-EVENTARGS
    public interface IFrameEventArgs : IEventArgs
    {
        /// <summary>
        /// Gets a <see cref="System.Double"/> that indicates how many seconds of time elapsed since the previous event.
        /// </summary>
        double Time { get; }
    }
    #endregion

    #region IUPLOAD EVENT ARGS
    public interface IUploadEventArgs : IEventArgs
    {
        byte[] Data { get; }
        Rectangle Area { get; }
    }
    #endregion

    #region ITOUCHEVENT-ARGS
    /// <summary>
    /// Represents an argument object to relay touch information.
    /// </summary>
    public interface ITouchEventArgs : IEventArgs
    {
        /// <summary>
        /// X cor-ordinate of the location of touch.
        /// </summary>
        float X { get; }

        /// <summary>
        /// X cor-ordinate of the location of touch.
        /// </summary>
        float Y { get; }

        /// <summary>Index of the finger in case of multi-touch events</summary>
        long Finger { get; }
    }
    #endregion

    #region IJOYSTICK-EVENT-ARGS
    /// <summary>
    /// Represents a joy stick object. 
    /// </summary>
    public interface IJoystickDevice
    {
        /// <summary>
        /// Id of current button being used.
        /// </summary>
        int Which { get; }
    }
    #endregion

    #region IJOYSTICK-HAT-EVENT-ARGS
    /// <summary>
    /// 
    /// </summary>
    public interface IJoystickHatEventArgs : IJoystickDevice, IEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        int Index { get; }
        int Value { get; }
    }
    #endregion

    #region IJOYSTICHATEVENT-ARGS

    #endregion

    #region IJOYSTICK-BUTTON-EVENTARGS
    public interface IJoystickButtonEventArgs : IJoystickDevice, IEventArgs
    {
        int Button { get; }
        bool Pressed { get; }
    }
    #endregion

    #region IJOYSTICK-BALL-EVENTARGS
    public interface IJoystickBallEventArgs : IEventArgs
    {
        int Device { get; }
        int Button { get; }
        bool Pressed { get; }
        int XDelta { get; }
        int YDelta { get; }
    }
    #endregion

    #region IJOYSTICK-AXIS-EVENTARGS
    public interface IJoystickAxisEventArgs : IEventArgs
    {
        int Device { get; }
        int Axis { get; }
        float Value { get; }
        float Delta { get; }
    }
    #endregion

#if Advanced
    #region IMOUSE DRAG
    public interface IMouseDrag
    {
        bool IsMouseDragging { get; }
    }
    #endregion

    #region ISimplePopupItemEventArgs
    public interface ISimplePopupItemEventArgs : IEventArgs
    {
        ISimplePopupItem Item { get; set; }
        int Index { get; set; }
    }
    #endregion

#endif

#endif

#if (GWS || Window)
    #region EVENT
    public interface IEvent
    {
        string ID { get; }
        GwsEvent Type { get; }
        IExEvent Window { get; }
    }
    #endregion

    #region EXTEVENT
    public interface IExEvent
    {
        GwsEvent Type { get; }
        int WindowID { get; }
        WindowEventID Event { get; }
    }
    #endregion

    #region IEVENT-PROCESSOR
    public interface IEventProcessor : IRecognizable, IDisposable
    {
        bool ProcessEvent(IEvent @event);
    }
    #endregion

    #region IEVENTPUSHER
    public interface IEventPusher
    {
        void PushEvent(IEventInfo e);
        event EventHandler<IEventInfo> EventPushed;
    }
    #endregion

    #region IEVENT INFO
    public interface IEventInfo
    {
        object Sender { get; set; }
        IEventArgs Args { get; set; }
        GwsEvent Type { get; }
        EventUseStatus Status { get; set; }
    }
    #endregion

    #region IMINIMALEVENTS
    public interface IMinimalEvents
    {
        event EventHandler<IKeyEventArgs> KeyDown;
        event EventHandler<IKeyEventArgs> KeyUp;
        event EventHandler<IKeyPressEventArgs> KeyPress;
        event EventHandler<IMouseEventArgs> MouseWheel;
        event EventHandler<IMouseEventArgs> MouseDown;
        event EventHandler<IMouseEventArgs> MouseUp;
        event EventHandler<IMouseEventArgs> MouseClick;
        event EventHandler<IMouseEventArgs> MouseMove;
        event EventHandler<IMouseEventArgs> Enter;
        event EventHandler<IMouseEventArgs> Leave;
        event EventHandler<IMouseEventArgs> AppClicked;
        event EventHandler<ISizeEventArgs> Resized;
        event EventHandler<IDrawEventArgs> Paint;
    }
    #endregion

    #region EVENTS
    public interface IEvents
    {
        event EventHandler<IEventArgs> Moved;
        event EventHandler<ICancelEventArgs> LostFocus;
        event EventHandler<IEventArgs> GotFocus;
        event EventHandler<IKeyEventArgs> PreviewKeyDown;
        event EventHandler<IMouseEventArgs> MouseDoubleClick;
        event EventHandler<IEventArgs> VisibleChanged;
        event EventHandler<IEventArgs> FirstShown;

        event EventHandler<ITouchEventArgs> TouchBegan;
        event EventHandler<ITouchEventArgs> TouchMoved;
        event EventHandler<ITouchEventArgs> TouchEnded;
        event EventHandler<IMouseEventArgs> MouseDrag;
        event EventHandler<IMouseEventArgs> MouseDragBegin;
        event EventHandler<IMouseEventArgs> MouseDragEnd;
        event EventHandler<IJoystickButtonEventArgs> JoystickDown;
        event EventHandler<IJoystickButtonEventArgs> JoystickUp;
        event EventHandler<IJoystickAxisEventArgs> JoystickMove;
        event EventHandler<IEventArgs> JoystickConnected;
        event EventHandler<IEventArgs> JoystickDisconnected;
        event EventHandler<IEventArgs> Minimized;
        event EventHandler<IEventArgs> Maximized;
        event EventHandler<IEventArgs> Restored;
    }
    #endregion
#endif

#if Window 
    #region IEVENTS3
    public interface IWindowEvents
    {
        event EventHandler<IEventArgs> Closed;
        event EventHandler<IEventArgs> Load;
        event EventHandler<IEventArgs> TitleChanged;
        event EventHandler<IEventArgs> WindowBorderChanged;
        event EventHandler<IEventArgs> WindowStateChanged;
    }
    #endregion
#endif

#if (GWS || Window) 
    #region IGAMEPAD
    public interface IGamePad
    {
        /// <summary>
        /// Gets the <see cref="State"/> for the up button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the up button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        bool Up { get; }

        /// <summary>
        /// Gets the <see cref="State"/> for the down button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the down button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        bool Down { get; }

        /// <summary>
        /// Gets the <see cref="State"/> for the left button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the left button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        bool Left { get; }

        /// <summary>
        /// Gets the <see cref="State"/> for the right button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the right button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        bool Right { get; }

        /// <summary>
        /// Gets a value indicating whether the up button is pressed.
        /// </summary>
        /// <value><c>true</c> if the up button is pressed; otherwise, <c>false</c>.</value>
        bool IsUp { get; }

        /// <summary>
        /// Gets a value indicating whether the down button is pressed.
        /// </summary>
        /// <value><c>true</c> if the down button is pressed; otherwise, <c>false</c>.</value>
        bool IsDown { get; }

        /// <summary>
        /// Gets a value indicating whether the left button is pressed.
        /// </summary>
        /// <value><c>true</c> if the left button is pressed; otherwise, <c>false</c>.</value>
        bool IsLeft { get; }

        /// <summary>
        /// Gets a value indicating whether the right button is pressed.
        /// </summary>
        /// <value><c>true</c> if the right button is pressed; otherwise, <c>false</c>.</value>
        bool IsRight { get; }
    }
    #endregion

    #region IGAMEPAD-THUMBSTICKS
    public interface IGamePadThumbSticks : IEquatable<IGamePadThumbSticks>
    {
        VectorF Left { get; }
        VectorF Right { get; }
    }
    #endregion

    #region IGAMEPAD-CAPABILITIES
    public interface IGamePadCapabilities : IEquatable<IGamePadCapabilities>
    {
        GamePadType GamePadType { get; }

        int Buttons { get; }

        bool HasDPadUpButton { get; }
        bool HasDPadDownButton { get; }
        bool HasDPadLeftButton { get; }
        bool HasDPadRightButton { get; }
        bool HasAButton { get; }
        bool HasBButton { get; }
        bool HasXButton { get; }
        bool HasYButton { get; }
        bool HasLeftStickButton { get; }
        bool HasRightStickButton { get; }
        bool HasLeftShoulderButton { get; }
        bool HasRightShoulderButton { get; }
        bool HasBackButton { get; }
        bool HasBigButton { get; }
        bool HasStartButton { get; }
        bool HasLeftXThumbStick { get; }
        bool HasLeftYThumbStick { get; }
        bool HasRightXThumbStick { get; }
        bool HasRightYThumbStick { get; }
        bool HasLeftTrigger { get; }
        bool HasRightTrigger { get; }
        bool HasLeftVibrationMotor { get; }
        bool HasRightVibrationMotor { get; }
        bool HasVoiceSupport { get; }
        bool IsConnected { get; }
        bool IsMapped { get; }
    }
    #endregion

    #region IGAMEPAD-DRIVER
    public interface IGamePadDriver
    {
        IGamePadState GetState(int index);
        IGamePadCapabilities GetCapabilities(int index);
        string GetName(int index);
        bool SetVibration(int index, float left, float right);
    }
    #endregion

    #region IGAMEPAD-STATE
    public interface IGamePadState : IEquatable<IGamePadState>
    {
        IGamePadThumbSticks ThumbSticks { get; }
        IGamePadButtons Buttons { get; }
        IGamePadDPad DPad { get; }
        IGamePadTriggers Triggers { get; }
        bool IsConnected { get; }
        int PacketNumber { get; }
    }
    #endregion

    #region IGAMEPAD-BUTTONS
    public interface IGamePadButtons
    {
        int Buttons { get; }
        State A { get; }
        State B { get; }
        State X { get; }
        State Y { get; }
        State Back { get; }
        State BigButton { get; }
        State LeftShoulder { get; }
        State LeftStick { get; }
        State RightShoulder { get; }
        State RightStick { get; }
        State Start { get; }
        bool IsAnyButtonPressed { get; }
    }
    #endregion

    #region IGAMEPAD-DPAD
    public interface IGamePadDPad
    {
        int Buttons { get; }
        State Up { get; }
        State Down { get; }
        State Left { get; }
        State Right { get; }
        bool IsUp { get; }
        bool IsDown { get; }
        bool IsLeft { get; }
        bool IsRight { get; }
    }
    #endregion

    #region IGAMEPAD-TRIGGERS
    public interface IGamePadTriggers
    {
        float Left { get; }
        float Right { get; }
    }
    #endregion
#endif
}
