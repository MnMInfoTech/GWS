/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)
using System;

namespace MnM.GWS
{
    #region IFILEDROP-EVENTARGS
    public interface IFileDropEventArgs : IEventArgs
    {
        string FileName { get; set; }
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

    #region IGAMEPAD
    public interface IGamePad
    {
        /// <summary>
        /// Gets the <see cref="InputState"/> for the up button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the up button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        bool Up { get; }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the down button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the down button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        bool Down { get; }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the left button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the left button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        bool Left { get; }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the right button.
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
        InputState A { get; }
        InputState B { get; }
        InputState X { get; }
        InputState Y { get; }
        InputState Back { get; }
        InputState BigButton { get; }
        InputState LeftShoulder { get; }
        InputState LeftStick { get; }
        InputState RightShoulder { get; }
        InputState RightStick { get; }
        InputState Start { get; }
        bool IsAnyButtonPressed { get; }
    }
    #endregion

    #region IGAMEPAD-DPAD
    public interface IGamePadDPad
    {
        int Buttons { get; }
        InputState Up { get; }
        InputState Down { get; }
        InputState Left { get; }
        InputState Right { get; }
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
}
#endif
