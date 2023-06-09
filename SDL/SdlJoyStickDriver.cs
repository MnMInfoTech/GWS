/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if Window && SDL
using MnM.GWS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MnM.GWS.SDL
{
    static partial class SdlJoystickDriver
    {
        const string libSDL = SdlAPI.libSDL;

#region variables

        private const float RangeMultiplier = 1.0f / 32768.0f;
        //internal static readonly API.EventFilter eventFilterDelegateUnsafe = filterInputEvents;
        //private static readonly IntPtr eventFilterDelegate;

        private static int count;


        // For IJoystickDriver2 implementation
        private static readonly PrimitiveList<JoystickDevice> joysticks = new PrimitiveList<JoystickDevice>(4);

        private static readonly Dictionary<int, int> sdl_instanceid_to_joysticks = new Dictionary<int, int>();

        static int last_controllers_instance = 0;
        static readonly PrimitiveList<Sdl2GamePad> controllers = new PrimitiveList<Sdl2GamePad>(4);
        static readonly Dictionary<int, int> sdl_instanceid_to_controllers = new Dictionary<int, int>();
#endregion

#region contructor/ destructor
        //internal JoystickDriver() { }
        //~JoystickDriver()
        //{
        //    dispose(false);
        //}
#endregion

        static internal int filterInputEvents(Event ev)
        {
            try
            {
                switch (ev.Type)
                {
                    case EventType.JOYDEVICEADDED:
                    case EventType.JOYDEVICEREMOVED:
                        ProcessJoystickEvent(ev.JoyDeviceEvent);
                        break;

                    case EventType.JOYAXISMOTION:
                        ProcessJoystickEvent(ev.JoyAxisEvent);
                        break;

                    case EventType.JOYBALLMOTION:
                        ProcessJoystickEvent(ev.JoyBallEvent);
                        break;

                    case EventType.JOYBUTTONDOWN:
                    case EventType.JOYBUTTONUP:
                        ProcessJoystickEvent(ev.JoyButtonEvent);
                        break;

                    case EventType.JOYHATMOTION:
                        ProcessJoystickEvent(ev.JoyHatEvent);
                        break;

                    case EventType.CONTROLLERDEVICEADDED:
                    case EventType.CONTROLLERDEVICEREMOVED:
                        ProcessControllerEvent(ev.ControllerDeviceEvent);
                        break;

                    case EventType.CONTROLLERAXISMOTION:
                        ProcessControllerEvent(ev.ControllerAxisEvent);
                        break;

                    case EventType.CONTROLLERBUTTONDOWN:
                    case EventType.CONTROLLERBUTTONUP:
                        ProcessControllerEvent(ev.ControllerButtonEent);
                        break;
                }
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.ToString());
            }
            return 0;
        }

#region public methods
        static JoystickState GetState(int index)
        {
            JoystickState state = new JoystickState();
            if (isJoystickValid(index))
            {
                JoystickDevice joystick = joysticks[index];

                for (int i = 0; i < joystick.Axis.Count; i++)
                {
                    state.SetAxis(i, (short)(joystick.Axis[i] * short.MaxValue + 0.5f));
                }

                for (int i = 0; i < joystick.Button.Count; i++)
                {
                    state.SetButton(i, joystick.Button[i]);
                }

                for (int i = 0; i < joystick.Details.HatCount; i++)
                {
                    state.SetHat((JoystickHat)((byte)JoystickHat.Hat0 + i), joystick.Details.Hat[i]);
                }

                state.SetIsConnected(joystick.Details.IsConnected);
                state.SetPacketNumber(joystick.Details.PacketNumber);
            }

            return state;
        }
        static JoystickCapabilities GetCapabilities(int index)
        {
            if (isJoystickValid(index))
            {
                JoystickDevice joystick = joysticks[index];

                return new JoystickCapabilities(
                    joystick.Axis.Count,
                    joystick.Button.Count,
                    joystick.Details.HatCount,
                    joystick.Details.IsConnected);
            }
            return new JoystickCapabilities();
        }
        static Guid GetGuid(int index)
        {
            Guid guid = new Guid();
            if (isJoystickValid(index))
            {
                JoystickDevice joystick = joysticks[index];

                return joystick.Details.Guid;
            }
            return guid;
        }
#endregion

#region hid
        // Author:
        //       Stefanos A. <stapostol@gmail.com>
        //
        // Copyright (c) 2014 Stefanos Apostolopoulos
        //
        // Permission is hereby granted, free of charge, to any person obtaining a copy
        // of this software and associated documentation files (the "Software"), to deal
        //  the Software without restriction, including without limitation the rights
        // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        // copies of the Software, and to permit persons to whom the Software is
        // furnished to do so, subject to the following conditions:
        //
        // The above copyright notice and this permission notice shall be included in
        // all copies or substantial portions of the Software.
        //
        // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
        // THE SOFTWARE.
        //
        /// <summary>
        /// Scales the specified value linearly between min and max.
        /// </summary>
        /// <param name="value">The value to scale</param>
        /// <param name="value_min">The minimum expected value (inclusive)</param>
        /// <param name="value_max">The maximum expected value (inclusive)</param>
        /// <param name="result_min">The minimum output value (inclusive)</param>
        /// <param name="result_max">The maximum output value (inclusive)</param>
        /// <returns>The value, scaled linearly between min and max</returns>
        public static int ScaleValue(int value, int value_min, int value_max,
            int result_min, int result_max)
        {
            if (value_min >= value_max || result_min >= result_max)
            {
                throw new ArgumentOutOfRangeException();
            }
            Clamp(value, value_min, value_max);

            int range = result_max - result_min;
            long temp = (value - value_min) * range; // need long to avoid overflow
            return (int)(temp / (value_max - value_min) + result_min);
        }
        public static int Clamp(int n, int min, int max)
        {
            return Math.Max(Math.Min(n, max), min);
        }
        public static int TranslateJoystickAxis(HIDPage page, int usage)
        {
            switch (page)
            {
                case HIDPage.GenericDesktop:
                    switch ((HIDUsageGD)usage)
                    {
                        case HIDUsageGD.X:
                            return 0;
                        case HIDUsageGD.Y:
                            return 1;
                        case HIDUsageGD.Z:
                            return 2;
                        case HIDUsageGD.Rz:
                            return 3;
                        case HIDUsageGD.Rx:
                            return 4;
                        case HIDUsageGD.Ry:
                            return 5;
                        case HIDUsageGD.Slider:
                            return 6;
                        case HIDUsageGD.Dial:
                            return 7;
                        case HIDUsageGD.Wheel:
                            return 8;
                    }
                    break;

                case HIDPage.Simulation:
                    switch ((HIDUsageSim)usage)
                    {
                        case HIDUsageSim.Rudder:
                            return 9;
                        case HIDUsageSim.Throttle:
                            return 10;
                    }
                    break;
            }
            return 0;
        }

#endregion

#region private methods
        static JoystickDevice openJoystick(int id)
        {
            JoystickDevice joystick = null;
            int num_axes = 0;
            int num_buttons = 0;
            int num_hats = 0;
            int num_balls = 0;

            IntPtr handle = JoystickOpen(id);
            if (handle != IntPtr.Zero)
            {
                num_axes = JoystickNumAxes(handle);
                num_buttons = JoystickNumButtons(handle);
                num_hats = JoystickNumHats(handle);
                num_balls = JoystickNumBalls(handle);

                joystick = new JoystickDevice(id, num_axes, num_buttons);
                joystick.Description = JoystickName(handle);
                joystick.Details.Handle = handle;
                joystick.Details.InstanceId = JoystickInstanceID(handle);
                joystick.Details.Guid = JoystickGetGUID(handle).ToGuid();
                joystick.Details.HatCount = num_hats;
                joystick.Details.BallCount = num_balls;
            }
            return joystick;
        }
        static bool isJoystickValid(int id) =>
            id >= 0 && id < joysticks.Count;

        static bool isJoystickInstanceValid(int instance_id)
        {
            return sdl_instanceid_to_joysticks.ContainsKey(instance_id);
        }
        static HatLocation translateHat(HatPosition value)
        {
            if ((value & HatPosition.LeftUp) == HatPosition.LeftUp)
            {
                return HatLocation.UpLeft;
            }

            if ((value & HatPosition.RightUp) == HatPosition.RightUp)
            {
                return HatLocation.UpRight;
            }

            if ((value & HatPosition.LeftDown) == HatPosition.LeftDown)
            {
                return HatLocation.DownLeft;
            }

            if ((value & HatPosition.RightDown) == HatPosition.RightDown)
            {
                return HatLocation.DownRight;
            }

            if ((value & HatPosition.Up) == HatPosition.Up)
            {
                return HatLocation.Up;
            }

            if ((value & HatPosition.Right) == HatPosition.Right)
            {
                return HatLocation.Right;
            }

            if ((value & HatPosition.Down) == HatPosition.Down)
            {
                return HatLocation.Down;
            }

            if ((value & HatPosition.Left) == HatPosition.Left)
            {
                return HatLocation.Left;
            }

            return HatLocation.Centered;
        }
        static bool isControllerValid(int id) =>
            id >= 0 && id < controllers.Count;

        static bool IsControllerInstanceValid(int instance_id)
        {
            return sdl_instanceid_to_controllers.ContainsKey(instance_id);
        }

        static GamePadAxes getBoundAxes(IntPtr gamecontroller)
        {
            GamePadAxes axes = 0;
            axes |= isAxisBind(gamecontroller, GameControllerAxis.LeftX) ? GamePadAxes.LeftX : 0;
            axes |= isAxisBind(gamecontroller, GameControllerAxis.LeftY) ? GamePadAxes.LeftY : 0;
            axes |= isAxisBind(gamecontroller, GameControllerAxis.RightX) ? GamePadAxes.RightX : 0;
            axes |= isAxisBind(gamecontroller, GameControllerAxis.RightY) ? GamePadAxes.RightY : 0;
            axes |= isAxisBind(gamecontroller, GameControllerAxis.TriggerLeft) ? GamePadAxes.LeftTrigger : 0;
            axes |= isAxisBind(gamecontroller, GameControllerAxis.TriggerRight) ? GamePadAxes.RightTrigger : 0;
            return axes;
        }

        static PadButtons getBoundButtons(IntPtr gamecontroller)
        {
            PadButtons buttons = 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.A) ? PadButtons.A : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.B) ? PadButtons.B : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.X) ? PadButtons.X : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.Y) ? PadButtons.Y : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.START) ? PadButtons.Start : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.BACK) ? PadButtons.Back : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.LEFTSHOULDER) ? PadButtons.LeftShoulder : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.RIGHTSHOULDER) ? PadButtons.RightShoulder : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.LEFTSTICK) ? PadButtons.LeftStick : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.RIGHTSTICK) ? PadButtons.RightStick : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.GUIDE) ? PadButtons.BigButton : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.DPAD_DOWN) ? PadButtons.DPadDown : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.DPAD_UP) ? PadButtons.DPadUp : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.DPAD_LEFT) ? PadButtons.DPadLeft : 0;
            buttons |= isButtonBind(gamecontroller, GameControllerButton.DPAD_RIGHT) ? PadButtons.DPadRight : 0;
            return buttons;
        }

        static bool isAxisBind(IntPtr gamecontroller, GameControllerAxis axis)
        {
            GameControllerButtonBind bind =
               GameControllerGetBindForAxis(gamecontroller, axis);
            return bind.BindType == GameControllerBindType.Axis;
        }

        static bool isButtonBind(IntPtr gamecontroller, GameControllerButton button)
        {
            GameControllerButtonBind bind =
                GameControllerGetBindForButton(gamecontroller, button);
            return bind.BindType == GameControllerBindType.Button;
        }

        static GamePadAxes translateAxis(GameControllerAxis axis)
        {
            switch (axis)
            {
                case GameControllerAxis.LeftX:
                    return GamePadAxes.LeftX;

                case GameControllerAxis.LeftY:
                    return GamePadAxes.LeftY;

                case GameControllerAxis.RightX:
                    return GamePadAxes.RightX;

                case GameControllerAxis.RightY:
                    return GamePadAxes.RightY;

                case GameControllerAxis.TriggerLeft:
                    return GamePadAxes.LeftTrigger;

                case GameControllerAxis.TriggerRight:
                    return GamePadAxes.RightTrigger;

                default:
                    throw new ArgumentOutOfRangeException(
                        String.Format("[SDL] Unknown axis {0}", axis));
            }
        }

        static PadButtons translateButton(GameControllerButton button)
        {
            switch (button)
            {
                case GameControllerButton.A:
                    return PadButtons.A;

                case GameControllerButton.B:
                    return PadButtons.B;

                case GameControllerButton.X:
                    return PadButtons.X;

                case GameControllerButton.Y:
                    return PadButtons.Y;

                case GameControllerButton.LEFTSHOULDER:
                    return PadButtons.LeftShoulder;

                case GameControllerButton.RIGHTSHOULDER:
                    return PadButtons.RightShoulder;

                case GameControllerButton.LEFTSTICK:
                    return PadButtons.LeftStick;

                case GameControllerButton.RIGHTSTICK:
                    return PadButtons.RightStick;

                case GameControllerButton.DPAD_UP:
                    return PadButtons.DPadUp;

                case GameControllerButton.DPAD_DOWN:
                    return PadButtons.DPadDown;

                case GameControllerButton.DPAD_LEFT:
                    return PadButtons.DPadLeft;

                case GameControllerButton.DPAD_RIGHT:
                    return PadButtons.DPadRight;

                case GameControllerButton.BACK:
                    return PadButtons.Back;

                case GameControllerButton.START:
                    return PadButtons.Start;

                case GameControllerButton.GUIDE:
                    return PadButtons.BigButton;

                default:
                    return 0;
            }
        }
#endregion

#region methods
        //unsafe int filterInputEvents(IntPtr handle, IntPtr e)
        //{
        //    if (API.GetObject(handle, out JoystickDriver driver))
        //    {
        //        Event ev = *(Event*)e;
        //        return (driver).filterInputEvents(ev);
        //    }
        //    return 0;
        //}
        internal static void Dispose()
        {
            //lock (API.Sync)
            //{
            //    //API.DeleteEventWatch(eventFilterDelegate, Handle);
            //}
            foreach (var j in joysticks)
            {
                IntPtr handle = j.Details.Handle;
                JoystickClose(handle);
            }

            joysticks.Clear();
        }
#endregion

#region internal methods
        internal static void ProcessJoystickEvent(JoyDeviceEvent ev)
        {
            int id = ev.Which;
            if (id < 0)
            {
                return;
            }

            switch (ev.Type)
            {
                case EventType.JOYDEVICEADDED:
                    {
                        IntPtr handle = JoystickOpen(id);
                        if (handle != IntPtr.Zero)
                        {
                            JoystickDevice joystick = openJoystick(id);

                            int instance_id = joystick.Details.InstanceId;
                            int device_id = id;

                            if (joystick != null)
                            {
                                joystick.Details.IsConnected = true;
                                if (device_id < joysticks.Count)
                                {
                                    joysticks[device_id] = joystick;
                                }
                                else
                                {
                                    joysticks.Add(joystick);
                                }

                                sdl_instanceid_to_joysticks.Add(instance_id, device_id);
                            }
                        }
                    }
                    break;

                case EventType.JOYDEVICEREMOVED:
                    if (isJoystickInstanceValid(id))
                    {
                        int instance_id = id;
                        int device_id = sdl_instanceid_to_joysticks[instance_id];

                        JoystickDevice joystick = joysticks[device_id];
                        joystick.Details.IsConnected = false;

                        sdl_instanceid_to_joysticks.Remove(instance_id);
                    }
                    break;
            }
        }
        internal static void ProcessJoystickEvent(JoyAxisEvent ev)
        {
            int id = ev.Which;
            if (isJoystickInstanceValid(id))
            {
                int index = sdl_instanceid_to_joysticks[id];
                JoystickDevice joystick = joysticks[index];
                float value = ev.Value * RangeMultiplier;
                joystick.SetAxis(ev.Axis, value);
                joystick.Details.PacketNumber = Math.Max(0, unchecked(joystick.Details.PacketNumber + 1));
            }
        }
        internal static void ProcessJoystickEvent(JoyBallEvent ev)
        {
            int id = ev.Which;
            if (isJoystickInstanceValid(id))
            {
                int index = sdl_instanceid_to_joysticks[id];
                JoystickDevice joystick = joysticks[index];
                // Todo: does it make sense to support balls?
                joystick.Details.PacketNumber = Math.Max(0, unchecked(joystick.Details.PacketNumber + 1));
            }
        }
        internal static void ProcessJoystickEvent(JoyButtonEvent ev)
        {
            int id = ev.Which;
            if (isJoystickInstanceValid(id))
            {
                int index = sdl_instanceid_to_joysticks[id];
                JoystickDevice joystick = joysticks[index];
                joystick.SetButton(ev.Button, ev.State == InputState.Pressed);
                joystick.Details.PacketNumber = Math.Max(0, unchecked(joystick.Details.PacketNumber + 1));
            }
        }
        internal static void ProcessJoystickEvent(JoyHatEvent ev)
        {
            int id = ev.Which;
            if (isJoystickInstanceValid(id))
            {
                int index = sdl_instanceid_to_joysticks[id];
                JoystickDevice joystick = joysticks[index];
                if (ev.Hat >= 0 && ev.Hat < JoystickState.MaxHats)
                {
                    joystick.Details.Hat[ev.Hat] = new JoystickHatState(translateHat(ev.Value));
                }
                joystick.Details.PacketNumber = Math.Max(0, unchecked(joystick.Details.PacketNumber + 1));
            }
        }
        internal static void ProcessControllerEvent(ControllerDeviceEvent ev)
        {
            int id = ev.Which;
            if (id < 0)
            {
                return;
            }

            switch (ev.Type)
            {
                case EventType.CONTROLLERDEVICEADDED:
                    IntPtr handle = GameControllerOpen(id);
                    if (handle != IntPtr.Zero)
                    {
                        // The id variable here corresponds to a device_id between 0 and Sdl.NumJoysticks().
                        // It is only used  the ADDED event. All other events use an instance_id which increases
                        // monotonically  each ADDED event.
                        // The idea is that device_id refers to the n-th connected joystick, whereas instance_id
                        // refers to the actual hardware device behind the n-th joystick.
                        // Yes, it's confusing.
                        int device_id = id;
                        int instance_id = last_controllers_instance++;

                        Sdl2GamePad pad = new Sdl2GamePad(handle);

                        IntPtr joystick = GameControllerGetJoystick(handle);
                        if (joystick != IntPtr.Zero)
                        {
                            pad.Capabilities = new GamePadCapabilities(
                                GamePadType.GamePad,
                                getBoundAxes(joystick),
                                getBoundButtons(joystick),
                                true, true);
                            pad.State.SetConnected(true);

                            // Connect this device and add the relevant device index
                            if (controllers.Count <= id)
                            {
                                controllers.Add(pad);
                            }
                            else
                            {
                                controllers[device_id] = pad;
                            }

                            sdl_instanceid_to_controllers.Add(instance_id, device_id);
                        }
                    }
                    break;

                case EventType.CONTROLLERDEVICEREMOVED:
                    if (IsControllerInstanceValid(id))
                    {
                        int instance_id = id;
                        int device_id = sdl_instanceid_to_controllers[instance_id];

                        controllers[device_id].State.SetConnected(false);
                        sdl_instanceid_to_controllers.Remove(device_id);
                    }
                    break;

                case EventType.CONTROLLERDEVICEREMAPPED:
                    if (IsControllerInstanceValid(id))
                    {
                        // Todo: what should we do  this case?
                    }
                    break;
            }
        }
        internal static void ProcessControllerEvent(ControllerAxisEvent ev)
        {
            int instance_id = ev.Which;
            if (IsControllerInstanceValid(instance_id))
            {
                int id = sdl_instanceid_to_controllers[instance_id];
                controllers[id].State.SetAxis(translateAxis(ev.Axis), (short)ev.Value);
            }
        }
        internal static void ProcessControllerEvent(ControllerButtonEvent ev)
        {
            int instance_id = ev.Which;
            if (IsControllerInstanceValid(instance_id))
            {
                int id = sdl_instanceid_to_controllers[instance_id];
                controllers[id].State.SetButton(translateButton(ev.Button), ev.State == InputState.Pressed);
            }
        }
#endregion

#region child classes
        class Sdl2JoystickDetails
        {
            public IntPtr Handle { get; set; }
            public Guid Guid { get; set; }
            public int InstanceId { get; set; }
            public int PacketNumber { get; set; }
            public int HatCount { get; set; }
            public int BallCount { get; set; }
            public bool IsConnected { get; set; }
            public readonly JoystickHatState[] Hat =
                new JoystickHatState[JoystickState.MaxHats];
        }
        class Sdl2GamePad
        {
            public IntPtr Handle { get; private set; }
            public GamePadState State;
            public GamePadCapabilities Capabilities;

            public Sdl2GamePad(IntPtr handle)
            {
                Handle = handle;
            }
        }
        class JoystickButtonCollection
        {
            private bool[] button_state;

            internal JoystickButtonCollection(int numButtons)
            {
                if (numButtons < 0)
                {
                    throw new ArgumentOutOfRangeException("numButtons");
                }

                button_state = new bool[numButtons];
            }

            /// <summary>
            /// Gets a System.Boolean indicating whether the JoystickButton with the specified index is pressed.
            /// </summary>
            /// <param name="index">The index of the JoystickButton to check.</param>
            /// <returns>True if the JoystickButton is pressed; false otherwise.</returns>
            public bool this[int index]
            {
                get { return button_state[index]; }
                internal set { button_state[index] = value; }
            }

            /// <summary>
            /// Gets a System.Int32 indicating the available amount of JoystickButtons.
            /// </summary>
            public int Count
            {
                get { return button_state.Length; }
            }
        }
        class JoystickAxisCollection
        {
            private float[] axis_state;

            internal JoystickAxisCollection(int numAxes)
            {
                if (numAxes < 0)
                {
                    throw new ArgumentOutOfRangeException("numAxes");
                }

                axis_state = new float[numAxes];
            }

            /// <summary>
            /// Gets a System.Single indicating the absolute position of the JoystickAxis with the specified index.
            /// </summary>
            /// <param name="index">The index of the JoystickAxis to check.</param>
            /// <returns>A System.Single  the range [-1, 1].</returns>
            public float this[int index]
            {
                get { return axis_state[index]; }
                internal set { axis_state[index] = value; }
            }

            /// <summary>
            /// Gets a System.Int32 indicating the available amount of JoystickAxes.
            /// </summary>
            public int Count
            {
                get { return axis_state.Length; }
            }
        }
        class JoystickDevice
        {
            private JoystickAxisEventArgs move_args = new JoystickAxisEventArgs(0, 0, 0);
            private JoystickButtonEventArgs button_args = new JoystickButtonEventArgs(0, false);
            internal readonly Sdl2JoystickDetails Details = new Sdl2JoystickDetails();

            internal JoystickDevice(int id, int axes, int buttons)
            {
                if (axes < 0)
                {
                    throw new ArgumentOutOfRangeException("axes");
                }

                if (buttons < 0)
                {
                    throw new ArgumentOutOfRangeException("buttons");
                }

                Id = id;
                Axis = new JoystickAxisCollection(axes);
                Button = new JoystickButtonCollection(buttons);
            }

            /// <summary>
            /// Gets a JoystickAxisCollection containing the state of each axis on this instance. Values are normalized  the [-1, 1] range.
            /// </summary>
            public JoystickAxisCollection Axis { get; }

            /// <summary>
            /// Gets JoystickButtonCollection containing the state of each button on this instance. True indicates that the button is pressed.
            /// </summary>
            public JoystickButtonCollection Button { get; }

            /// <summary>
            /// Gets a System.String containing a unique description for this instance.
            /// </summary>
            public string Description { get; internal set; }

            /// <summary>
            /// Gets a value indicating the InputDeviceType of this InputDevice.
            /// </summary>
            public InputType DeviceType
            {
                get { return InputType.Hid; }
            }

            /// <summary>
            /// Occurs when an axis of this JoystickDevice instance is moved.
            /// </summary>
            public EventHandler<JoystickAxisEventArgs> Move =
                delegate (object sender, JoystickAxisEventArgs e) { };

            /// <summary>
            /// Occurs when a button of this JoystickDevice instance is pressed.
            /// </summary>
            public EventHandler<JoystickButtonEventArgs> ButtonDown =
                delegate (object sender, JoystickButtonEventArgs e) { };

            /// <summary>
            /// Occurs when a button of this JoystickDevice is released.
            /// </summary>
            public EventHandler<JoystickButtonEventArgs> ButtonUp =
                delegate (object sender, JoystickButtonEventArgs e) { };

            internal int Id { get; set; }

            internal void SetAxis(int axis, float @value)
            {
                if ((int)axis < Axis.Count)
                {
                    move_args.Axis = axis;
                    move_args.Delta = move_args.Value - @value;
                    Axis[axis] = move_args.Value = @value;
                    Move(this, move_args);
                }
            }

            internal void SetButton(int button, bool @value)
            {
                if (button < Button.Count)
                {
                    if (Button[button] != @value)
                    {
                        button_args.Button = button;
                        Button[button] = button_args.Pressed = @value;
                        if (@value)
                        {
                            ButtonDown(this, button_args);
                        }
                        else
                        {
                            ButtonUp(this, button_args);
                        }
                    }
                }
            }

            public void DisPose()
            {

            }
        }
        class GameController
        {
            class GamePadDriver : IGamePadDriver
            {
                private readonly GamePadConfigurationDatabase database =
                    new GamePadConfigurationDatabase();

                private readonly Dictionary<Guid, GamePadConfiguration> configurations =
                    new Dictionary<Guid, GamePadConfiguration>();


                public GamePadState GetState(int index)
                {
                    JoystickState joy = SdlJoystickDriver.GetState(index);
                    GamePadState pad = new GamePadState();

                    if (joy.IsConnected)
                    {
                        pad.SetConnected(true);
                        pad.SetPacketNumber(joy.PacketNumber);

                        GamePadConfiguration configuration = GetConfiguration(SdlJoystickDriver.GetGuid(index));

                        foreach (GamePadConfigurationItem map in configuration)
                        {
                            switch (map.Source.Type)
                            {
                                case ConfigurationType.Axis:
                                    {
                                        // JoystickAxis -> Buttons/GamePadAxes mapping
                                        int source_axis = map.Source.Axis;
                                        short value = joy.GetAxisRaw(source_axis);

                                        switch (map.Target.Type)
                                        {
                                            case ConfigurationType.Axis:
                                                pad.SetAxis(map.Target.Axis, value);
                                                break;

                                            case ConfigurationType.Button:
                                                // Todo: if SDL2 GameController config is ever updated to
                                                // distinguish between negative/positive axes, then remove
                                                // Math.Abs below.
                                                // Button is considered press when the axis is >= 0.5 from center
                                                pad.SetButton(map.Target.Button, Math.Abs(value) >= short.MaxValue >> 1);
                                                break;
                                        }
                                    }
                                    break;

                                case ConfigurationType.Button:
                                    {
                                        // JoystickButton -> Buttons/GamePadAxes mapping
                                        int source_button = map.Source.Button;
                                        bool pressed = joy.GetButton(source_button) == InputState.Pressed;

                                        switch (map.Target.Type)
                                        {
                                            case ConfigurationType.Axis:
                                                // Todo: if SDL2 GameController config is ever updated to
                                                // distinguish between negative/positive axes, then update
                                                // the following line to support both.
                                                short value = pressed ?
                                                    short.MaxValue :
                                                    (map.Target.Axis & (GamePadAxes.LeftTrigger | GamePadAxes.RightTrigger)) != 0 ?
                                                        short.MinValue :
                                                        (short)0;
                                                pad.SetAxis(map.Target.Axis, value);
                                                break;

                                            case ConfigurationType.Button:
                                                pad.SetButton(map.Target.Button, pressed);
                                                break;
                                        }
                                    }
                                    break;

                                case ConfigurationType.Hat:
                                    {
                                        // JoystickHat -> Buttons/GamePadAxes mapping
                                        JoystickHat source_hat = map.Source.Hat;
                                        JoystickHatState state = joy.GetHat(source_hat);

                                        bool pressed = false;
                                        switch (map.Source.HatPosition)
                                        {
                                            case HatLocation.Down:
                                                pressed = state.IsDown;
                                                break;

                                            case HatLocation.Up:
                                                pressed = state.IsUp;
                                                break;

                                            case HatLocation.Left:
                                                pressed = state.IsLeft;
                                                break;

                                            case HatLocation.Right:
                                                pressed = state.IsRight;
                                                break;
                                        }

                                        switch (map.Target.Type)
                                        {
                                            case ConfigurationType.Axis:
                                                // Todo: if SDL2 GameController config is ever updated to
                                                // distinguish between negative/positive axes, then update
                                                // the following line to support both.
                                                short value = pressed ?
                                                    short.MaxValue :
                                                    (map.Target.Axis & (GamePadAxes.LeftTrigger | GamePadAxes.RightTrigger)) != 0 ?
                                                        short.MinValue :
                                                        (short)0;
                                                pad.SetAxis(map.Target.Axis, value);
                                                break;

                                            case ConfigurationType.Button:
                                                pad.SetButton(map.Target.Button, pressed);
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }

                    return pad;
                }

                public GamePadCapabilities GetCapabilities(int index)
                {
                    JoystickCapabilities joy = SdlJoystickDriver.GetCapabilities(index);
                    GamePadCapabilities pad;
                    if (joy.IsConnected)
                    {
                        GamePadConfiguration configuration = GetConfiguration(SdlJoystickDriver.GetGuid(index));
                        GamePadAxes mapped_axes = 0;
                        PadButtons mapped_buttons = 0;

                        foreach (GamePadConfigurationItem map in configuration)
                        {
                            switch (map.Target.Type)
                            {
                                case ConfigurationType.Axis:
                                    mapped_axes |= map.Target.Axis;
                                    break;

                                case ConfigurationType.Button:
                                    mapped_buttons |= map.Target.Button;
                                    break;
                            }
                        }

                        pad = new GamePadCapabilities(
                            GamePadType.GamePad, // Todo: detect different types
                            mapped_axes,
                            mapped_buttons,
                            joy.IsConnected,
                            configuration.Name != GamePadConfigurationDatabase.UnmappedName);
                    }
                    else
                    {
                        pad = new GamePadCapabilities();
                    }
                    return pad;
                }

                public string GetName(int index)
                {
                    JoystickCapabilities joy = SdlJoystickDriver.GetCapabilities(index);
                    string name = String.Empty;
                    if (joy.IsConnected)
                    {
                        GamePadConfiguration map = GetConfiguration(SdlJoystickDriver.GetGuid(index));
                        name = map.Name;
                    }
                    return name;
                }

                public bool SetVibration(int index, float left, float right)
                {
                    return false;
                }

                private GamePadConfiguration GetConfiguration(Guid guid)
                {
                    if (!configurations.ContainsKey(guid))
                    {
                        string config = database[guid];
                        GamePadConfiguration map = new GamePadConfiguration(config);
                        configurations.Add(guid, map);
                    }
                    return configurations[guid];
                }

                private bool IsMapped(GamePadConfigurationSource item)
                {
                    return item.Type != ConfigurationType.Unmapped;
                }

                internal class GamePadConfigurationDatabase
                {
                    internal const string UnmappedName = "Unmapped Controller";

                    private readonly Dictionary<Guid, string> Configurations = new Dictionary<Guid, string>();

                    internal GamePadConfigurationDatabase()
                    {
                        // Configuration database copied from SDL
                        // Simple DirectMedia Layer
                        // Copyright (C) 1997-2013 Sam Lantinga <slouken@libsdl.org>
                        //
                        // This software is provided 'as-is', without any express or implied
                        // warranty.  In no event will the authors be held liable for any damages
                        //    arising from the use of this software.
                        //
                        //    Permission is granted to anyone to use this software for any purpose,
                        //        including commercial applications, and to alter it and redistribute it
                        //        freely, subject to the following restrictions:
                        //
                        //       1. The origin of this software must not be misrepresented; you must not
                        // claim that you wrote the original software. If you use this software
                        //  a product, an acknowledgment  the product documentation would be
                        // appreciated but is not required.
                        // 2. Altered source versions must be plainly marked as such, and must not be
                        // misrepresented as being the original software.
                        // 3. This notice may not be removed or altered from any source distribution.
                        // Default (unknown) configuration
                        if (!SdlAPI.Initialized)
                        {
                            Add("00000000000000000000000000000000,Unmapped Controller,a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b10,leftshoulder:b4,leftstick:b8,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b9,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");
                        }
                        else
                        {
                            // Old SDL2 mapping for XInput devices (pre SDL-2.0.4)
                            Add("00000000000000000000000000000000,XInput Controller,a:b10,b:b11,back:b5,dpdown:b1,dpleft:b2,dpright:b3,dpup:b0,guide:b14,leftshoulder:b8,leftstick:b6,lefttrigger:a4,leftx:a0,lefty:a1,rightshoulder:b9,rightstick:b7,righttrigger:a5,rightx:a3,righty:a2,start:b4,x:b12,y:b13,");
                        }

                        // Windows - XInput
                        Add("78696e70757400000000000000000000,XInput Controller,a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b10,leftshoulder:b4,leftstick:b8,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b9,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");
                        // Windows - XInput (SDL2)
                        Add("78696e70757401000000000000000000,XInput Controller,a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b10,leftshoulder:b4,leftstick:b8,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b9,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");

                        // Windows
                        Add("8f0e1200000000000000504944564944,Acme,x:b2,a:b0,b:b1,y:b3,back:b8,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:b5,rightshoulder:b6,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a3,righty:a2,");
                        Add("341a3608000000000000504944564944,Afterglow PS3 Controller,a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b12,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b0,y:b3,");
                        Add("ffff0000000000000000504944564944,GameStop Gamepad,a:b0,b:b1,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b2,y:b3,");
                        Add("6d0416c2000000000000504944564944,Generic DirectInput Controller,a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b0,y:b3,");
                        Add("6d0419c2000000000000504944564944,Logitech F710 Gamepad,a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b0,y:b3,");
                        Add("88880803000000000000504944564944,PS3 Controller,a:b2,b:b1,back:b8,dpdown:h0.8,dpleft:h0.4,dpright:h0.2,dpup:h0.1,guide:b12,leftshoulder:b4,leftstick:b9,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b10,righttrigger:b7,rightx:a3,righty:a4,start:b11,x:b0,y:b3,");
                        Add("4c056802000000000000504944564944,PS3 Controller,a:b14,b:b13,back:b0,dpdown:b6,dpleft:b7,dpright:b5,dpup:b4,guide:b16,leftshoulder:b10,leftstick:b1,lefttrigger:b8,leftx:a0,lefty:a1,rightshoulder:b11,rightstick:b2,righttrigger:b9,rightx:a2,righty:a3,start:b3,x:b15,y:b12,");
                        Add("25090500000000000000504944564944,PS3 DualShock,a:b2,b:b1,back:b9,dpdown:h0.8,dpleft:h0.4,dpright:h0.2,dpup:h0.1,guide:,leftshoulder:b6,leftstick:b10,lefttrigger:b4,leftx:a0,lefty:a1,rightshoulder:b7,rightstick:b11,righttrigger:b5,rightx:a2,righty:a3,start:b8,x:b0,y:b3,");
                        Add("4c05c405000000000000504944564944,PS4 Controller,a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b12,leftshoulder:b4,leftstick:b10,lefttrigger:a3,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:a4,rightx:a2,righty:a5,start:b9,x:b0,y:b3,");
                        Add("6d0418c2000000000000504944564944,Logitech RumblePad 2 USB,x:b0,a:b1,b:b2,y:b3,back:b8,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:b6,rightshoulder:b5,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a3,");
                        Add("36280100000000000000504944564944,OUYA Controller,a:b0,b:b3,y:b2,x:b1,start:b14,guide:b15,leftstick:b6,rightstick:b7,leftshoulder:b4,rightshoulder:b5,dpup:b8,dpleft:b10,dpdown:b9,dpright:b11,leftx:a0,lefty:a1,rightx:a3,righty:a4,lefttrigger:b12,righttrigger:b13,");
                        Add("4f0400b3000000000000504944564944,Thrustmaster Firestorm Dual Power,a:b0,b:b2,y:b3,x:b1,start:b10,guide:b8,back:b9,leftstick:b11,rightstick:b12,leftshoulder:b4,rightshoulder:b6,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b5,righttrigger:b7,");
                        Add("00f00300000000000000504944564944,RetroUSB.com RetroPad,a:b1,b:b5,x:b0,y:b4,back:b2,start:b3,leftshoulder:b6,rightshoulder:b7,leftx:a0,lefty:a1,");
                        Add("00f0f100000000000000504944564944,RetroUSB.com Super RetroPort,a:b1,b:b5,x:b0,y:b4,back:b2,start:b3,leftshoulder:b6,rightshoulder:b7,leftx:a0,lefty:a1,");
                        Add("28040140000000000000504944564944,GamePad Pro USB,a:b1,b:b2,x:b0,y:b3,back:b8,start:b9,leftshoulder:b4,rightshoulder:b5,leftx:a0,lefty:a1,lefttrigger:b6,righttrigger:b7,");
                        Add("ff113133000000000000504944564944,SVEN X-PAD,a:b2,b:b3,y:b1,x:b0,start:b5,back:b4,leftshoulder:b6,rightshoulder:b7,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a4,lefttrigger:b8,righttrigger:b9,");
                        Add("8f0e0300000000000000504944564944,Piranha xtreme,x:b3,a:b2,b:b1,y:b0,back:b8,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b6,lefttrigger:b4,rightshoulder:b7,righttrigger:b5,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a3,righty:a2,");
                        Add("8f0e0d31000000000000504944564944,Multilaser JS071 USB,a:b1,b:b2,y:b3,x:b0,start:b9,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,");
                        Add("10080300000000000000504944564944,PS2 USB,a:b2,b:b1,y:b0,x:b3,start:b9,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b6,rightshoulder:b7,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a4,righty:a2,lefttrigger:b4,righttrigger:b5,");
                        Add("79000600000000000000504944564944,G-Shark GS-GP702,a:b2,b:b1,x:b3,y:b0,back:b8,start:b9,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a4,lefttrigger:b6,righttrigger:b7,");
                        Add("4b12014d000000000000504944564944,NYKO AIRFLO,a:b0,b:b1,x:b2,y:b3,back:b8,guide:b10,start:b9,leftstick:a0,rightstick:a2,leftshoulder:a3,rightshoulder:b5,dpup:h0.1,dpdown:h0.0,dpleft:h0.8,dpright:h0.2,leftx:h0.6,lefty:h0.12,rightx:h0.9,righty:h0.4,lefttrigger:b6,righttrigger:b7,");
                        Add("d6206dca000000000000504944564944,PowerA Pro Ex,a:b1,b:b2,x:b0,y:b3,back:b8,guide:b12,start:b9,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpdown:h0.0,dpleft:h0.8,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,");
                        Add("a3060cff000000000000504944564944,Saitek P2500,a:b2,b:b3,y:b1,x:b0,start:b4,guide:b10,back:b5,leftstick:b8,rightstick:b9,leftshoulder:b6,rightshoulder:b7,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,");
                        Add("4f0415b3000000000000504944564944,Thrustmaster Dual Analog 3.2,x:b1,a:b0,b:b2,y:b3,back:b8,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:b5,rightshoulder:b6,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a3,");
                        Add("6f0e1e01000000000000504944564944,Rock Candy Gamepad for PS3,a:b1,b:b2,x:b0,y:b3,back:b8,start:b9,guide:b12,leftshoulder:b4,rightshoulder:b5,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,");
                        Add("83056020000000000000504944564944,iBuffalo USB 2-axis 8-button Gamepad,a:b1,b:b0,y:b2,x:b3,start:b7,back:b6,leftshoulder:b4,rightshoulder:b5,leftx:a0,lefty:a1,");
                        Add("10080100000000000000504944564944,PS1 USB,a:b2,b:b1,x:b3,y:b0,back:b8,start:b9,leftshoulder:b6,rightshoulder:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a3,righty:a2,lefttrigger:b4,righttrigger:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,");

                        // Mac OS X
                        Add("0500000047532047616d657061640000,GameStop Gamepad,a:b0,b:b1,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b2,y:b3,");
                        Add("6d0400000000000016c2000000000000,Logitech F310 Gamepad (DInput),a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b0,y:b3,");
                        Add("6d0400000000000018c2000000000000,Logitech F510 Gamepad (DInput),a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b0,y:b3,");
                        Add("6d040000000000001fc2000000000000,Logitech F710 Gamepad (XInput),a:b0,b:b1,back:b9,dpdown:b12,dpleft:b13,dpright:b14,dpup:b11,guide:b10,leftshoulder:b4,leftstick:b6,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b7,righttrigger:a5,rightx:a3,righty:a4,start:b8,x:b2,y:b3,");
                        Add("6d0400000000000019c2000000000000,Logitech Wireless Gamepad (DInput),a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b0,y:b3,");
                        Add("4c050000000000006802000000000000,PS3 Controller,a:b14,b:b13,back:b0,dpdown:b6,dpleft:b7,dpright:b5,dpup:b4,guide:b16,leftshoulder:b10,leftstick:b1,lefttrigger:b8,leftx:a0,lefty:a1,rightshoulder:b11,rightstick:b2,righttrigger:b9,rightx:a2,righty:a3,start:b3,x:b15,y:b12,");
                        Add("4c05000000000000c405000000000000,PS4 Controller,a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b12,leftshoulder:b4,leftstick:b10,lefttrigger:a3,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:a4,rightx:a2,righty:a5,start:b9,x:b0,y:b3,");
                        Add("5e040000000000008e02000000000000,X360 Controller,a:b0,b:b1,back:b9,dpdown:b12,dpleft:b13,dpright:b14,dpup:b11,guide:b10,leftshoulder:b4,leftstick:b6,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b7,righttrigger:a5,rightx:a3,righty:a4,start:b8,x:b2,y:b3,");
                        Add("891600000000000000fd000000000000,Razer Onza Tournament,a:b0,b:b1,y:b3,x:b2,start:b8,guide:b10,back:b9,leftstick:b6,rightstick:b7,leftshoulder:b4,rightshoulder:b5,dpup:b11,dpleft:b13,dpdown:b12,dpright:b14,leftx:a0,lefty:a1,rightx:a3,righty:a4,lefttrigger:a2,righttrigger:a5,");
                        Add("4f0400000000000000b3000000000000,Thrustmaster Firestorm Dual Power,a:b0,b:b2,y:b3,x:b1,start:b10,guide:b8,back:b9,leftstick:b11,rightstick:,leftshoulder:b4,rightshoulder:b6,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b5,righttrigger:b7,");
                        Add("8f0e0000000000000300000000000000,Piranha xtreme,x:b3,a:b2,b:b1,y:b0,back:b8,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b6,lefttrigger:b4,rightshoulder:b7,righttrigger:b5,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a3,righty:a2,");
                        Add("0d0f0000000000004d00000000000000,HORI Gem Pad 3,a:b1,b:b2,y:b3,x:b0,start:b9,guide:b12,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,");
                        Add("79000000000000000600000000000000,G-Shark GP-702,a:b2,b:b1,x:b3,y:b0,back:b8,start:b9,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,leftx:a0,lefty:a1,rightx:a3,righty:a4,lefttrigger:b6,righttrigger:b7,");
                        Add("4f0400000000000015b3000000000000,Thrustmaster Dual Analog 3.2,x:b1,a:b0,b:b2,y:b3,back:b8,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:b5,rightshoulder:b6,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a3,");
                        Add("AD1B00000000000001F9000000000000,Gamestop BB-070 X360 Controller,a:b0,b:b1,back:b9,dpdown:b12,dpleft:b13,dpright:b14,dpup:b11,guide:b10,leftshoulder:b4,leftstick:b6,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b7,righttrigger:a5,rightx:a3,righty:a4,start:b8,x:b2,y:b3,");
                        Add("050000005769696d6f74652028303000,Wii Remote,a:b4,b:b5,y:b9,x:b10,start:b6,guide:b8,back:b7,dpup:b2,dpleft:b0,dpdown:b3,dpright:b1,leftx:a0,lefty:a1,lefttrigger:b12,righttrigger:,leftshoulder:b11,");
                        Add("83050000000000006020000000000000,iBuffalo USB 2-axis 8-button Gamepad,a:b1,b:b0,x:b3,y:b2,back:b6,start:b7,leftshoulder:b4,rightshoulder:b5,leftx:a0,lefty:a1,");

                        // Linux
                        Add("0500000047532047616d657061640000,GameStop Gamepad,a:b0,b:b1,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b2,y:b3,");
                        Add("03000000ba2200002010000001010000,Jess Technology USB Game Controller,a:b2,b:b1,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:,leftshoulder:b4,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,righttrigger:b7,rightx:a3,righty:a2,start:b9,x:b3,y:b0,");
                        Add("030000006d04000019c2000010010000,Logitech Cordless RumblePad 2,a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b0,y:b3,");
                        Add("030000006d0400001dc2000014400000,Logitech F310 Gamepad (XInput),a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b8,leftshoulder:b4,leftstick:b9,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b10,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");
                        Add("030000006d0400001ec2000020200000,Logitech F510 Gamepad (XInput),a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b8,leftshoulder:b4,leftstick:b9,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b10,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");
                        Add("030000006d04000019c2000011010000,Logitech F710 Gamepad (DInput),a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,leftshoulder:b4,leftstick:b10,lefttrigger:b6,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:b7,rightx:a2,righty:a3,start:b9,x:b0,y:b3,");
                        Add("030000006d0400001fc2000005030000,Logitech F710 Gamepad (XInput),a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b8,leftshoulder:b4,leftstick:b9,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b10,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");
                        Add("030000004c0500006802000011010000,PS3 Controller,a:b14,b:b13,back:b0,dpdown:b6,dpleft:b7,dpright:b5,dpup:b4,guide:b16,leftshoulder:b10,leftstick:b1,lefttrigger:b8,leftx:a0,lefty:a1,rightshoulder:b11,rightstick:b2,righttrigger:b9,rightx:a2,righty:a3,start:b3,x:b15,y:b12,");
                        Add("030000004c050000c405000011010000,Sony DualShock 4,a:b1,b:b2,y:b3,x:b0,start:b9,guide:b12,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a5,lefttrigger:b6,righttrigger:b7,");
                        Add("030000006f0e00003001000001010000,EA Sports PS3 Controller,a:b1,b:b2,y:b3,x:b0,start:b9,guide:b12,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,");
                        Add("03000000de280000ff11000001000000,Valve Streaming Gamepad,a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b8,leftshoulder:b4,leftstick:b9,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b10,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");
                        Add("030000005e0400008e02000014010000,X360 Controller,a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b8,leftshoulder:b4,leftstick:b9,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b10,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");
                        Add("030000005e0400008e02000010010000,X360 Controller,a:b0,b:b1,back:b6,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b8,leftshoulder:b4,leftstick:b9,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b10,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");
                        Add("030000005e0400001907000000010000,X360 Wireless Controller,a:b0,b:b1,back:b6,dpdown:b14,dpleft:b11,dpright:b12,dpup:b13,guide:b8,leftshoulder:b4,leftstick:b9,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b10,righttrigger:a5,rightx:a3,righty:a4,start:b7,x:b2,y:b3,");
                        Add("03000000100800000100000010010000,Twin USB PS2 Adapter,a:b2,b:b1,y:b0,x:b3,start:b9,guide:,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b6,rightshoulder:b7,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a3,righty:a2,lefttrigger:b4,righttrigger:b5,");
                        Add("03000000a306000023f6000011010000,Saitek Cyborg V.1 Game Pad,a:b1,b:b2,y:b3,x:b0,start:b9,guide:b12,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a4,lefttrigger:b6,righttrigger:b7,");
                        Add("030000004f04000020b3000010010000,Thrustmaster 2  1 DT,a:b0,b:b2,y:b3,x:b1,start:b9,guide:,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b6,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b5,righttrigger:b7,");
                        Add("030000004f04000023b3000000010000,Thrustmaster Dual Trigger 3-in-1,x:b0,a:b1,b:b2,y:b3,back:b8,start:b9,dpleft:h0.8,dpdown:h0.0,dpdown:h0.4,dpright:h0.0,dpright:h0.2,dpup:h0.0,dpup:h0.1,leftshoulder:h0.0,leftshoulder:b4,lefttrigger:b6,rightshoulder:b5,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a5,");
                        Add("030000008f0e00000300000010010000,GreenAsia Inc.    USB Joystick     ,x:b3,a:b2,b:b1,y:b0,back:b8,start:b9,dpleft:h0.8,dpdown:h0.0,dpdown:h0.4,dpright:h0.0,dpright:h0.2,dpup:h0.0,dpup:h0.1,leftshoulder:h0.0,leftshoulder:b6,lefttrigger:b4,rightshoulder:b7,righttrigger:b5,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a3,righty:a2,");
                        Add("030000008f0e00001200000010010000,GreenAsia Inc.      USB  Joystick  ,x:b2,a:b0,b:b1,y:b3,back:b8,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:b5,rightshoulder:b6,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a3,righty:a2,");
                        Add("030000005e0400009102000007010000,X360 Wireless Controller,a:b0,b:b1,y:b3,x:b2,start:b7,guide:b8,back:b6,leftstick:b9,rightstick:b10,leftshoulder:b4,rightshoulder:b5,dpup:b13,dpleft:b11,dpdown:b14,dpright:b12,leftx:a0,lefty:a1,rightx:a3,righty:a4,lefttrigger:a2,righttrigger:a5,");
                        Add("030000006d04000016c2000010010000,Logitech Logitech Dual Action,x:b0,a:b1,b:b2,y:b3,back:b8,start:b9,dpleft:h0.8,dpdown:h0.0,dpdown:h0.4,dpright:h0.0,dpright:h0.2,dpup:h0.0,dpup:h0.1,leftshoulder:h0.0,dpup:h0.1,leftshoulder:h0.0,leftshoulder:b4,lefttrigger:b6,rightshoulder:b5,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a3,");
                        Add("03000000260900008888000000010000,GameCube {WiseGroup USB box},a:b0,b:b2,y:b3,x:b1,start:b7,leftshoulder:,rightshoulder:b6,dpup:h0.1,dpleft:h0.8,rightstick:,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:a4,righttrigger:a5,");
                        Add("030000006d04000011c2000010010000,Logitech WingMan Cordless RumblePad,a:b0,b:b1,y:b4,x:b3,start:b8,guide:b5,back:b2,leftshoulder:b6,rightshoulder:b7,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a3,righty:a4,lefttrigger:b9,righttrigger:b10,");
                        Add("030000006d04000018c2000010010000,Logitech Logitech RumblePad 2 USB,x:b0,a:b1,b:b2,y:b3,back:b8,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:b6,rightshoulder:b5,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a3,");
                        Add("05000000d6200000ad0d000001000000,Moga Pro,a:b0,b:b1,y:b3,x:b2,start:b6,leftstick:b7,rightstick:b8,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:a5,righttrigger:a4,");
                        Add("030000004f04000009d0000000010000,Thrustmaster Run N Drive Wireless PS3,a:b1,b:b2,x:b0,y:b3,start:b9,guide:b12,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,");
                        Add("030000004f04000008d0000000010000,Thrustmaster Run N Drive  Wireless,a:b1,b:b2,x:b0,y:b3,start:b9,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a5,lefttrigger:b6,righttrigger:b7,");
                        Add("0300000000f000000300000000010000,RetroUSB.com RetroPad,a:b1,b:b5,x:b0,y:b4,back:b2,start:b3,leftshoulder:b6,rightshoulder:b7,leftx:a0,lefty:a1,");
                        Add("0300000000f00000f100000000010000,RetroUSB.com Super RetroPort,a:b1,b:b5,x:b0,y:b4,back:b2,start:b3,leftshoulder:b6,rightshoulder:b7,leftx:a0,lefty:a1,");
                        Add("030000006f0e00001f01000000010000,Generic X-Box pad,x:b2,a:b0,b:b1,y:b3,back:b6,guide:b8,start:b7,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:a2,rightshoulder:b5,righttrigger:a5,leftstick:b9,rightstick:b10,leftx:a0,lefty:a1,rightx:a3,righty:a4,");
                        Add("03000000280400000140000000010000,Gravis GamePad Pro USB ,x:b0,a:b1,b:b2,y:b3,back:b8,start:b9,leftshoulder:b4,lefttrigger:b6,rightshoulder:b5,righttrigger:b7,leftx:a0,lefty:a1,");
                        Add("030000005e0400008902000021010000,Microsoft X-Box pad v2 (US),x:b3,a:b0,b:b1,y:b4,back:b6,start:b7,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b5,lefttrigger:a2,rightshoulder:b2,righttrigger:a5,leftstick:b8,rightstick:b9,leftx:a0,lefty:a1,rightx:a3,righty:a4,");
                        Add("030000006f0e00001e01000011010000,Rock Candy Gamepad for PS3,a:b1,b:b2,x:b0,y:b3,back:b8,start:b9,guide:b12,leftshoulder:b4,rightshoulder:b5,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,");
                        Add("03000000250900000500000000010000,Sony PS2 pad with SmartJoy adapter,a:b2,b:b1,y:b0,x:b3,start:b8,back:b9,leftstick:b10,rightstick:b11,leftshoulder:b6,rightshoulder:b7,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b4,righttrigger:b5,");
                        Add("030000008916000000fd000024010000,Razer Onza Tournament,a:b0,b:b1,y:b3,x:b2,start:b7,guide:b8,back:b6,leftstick:b9,rightstick:b10,leftshoulder:b4,rightshoulder:b5,dpup:b13,dpleft:b11,dpdown:b14,dpright:b12,leftx:a0,lefty:a1,rightx:a3,righty:a4,lefttrigger:a2,righttrigger:a5,");
                        Add("030000004f04000000b3000010010000,Thrustmaster Firestorm Dual Power,a:b0,b:b2,y:b3,x:b1,start:b10,guide:b8,back:b9,leftstick:b11,rightstick:b12,leftshoulder:b4,rightshoulder:b6,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b5,righttrigger:b7,");
                        Add("03000000ad1b000001f5000033050000,Hori Pad EX Turbo 2,a:b0,b:b1,y:b3,x:b2,start:b7,guide:b8,back:b6,leftstick:b9,rightstick:b10,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a3,righty:a4,lefttrigger:a2,righttrigger:a5,");
                        Add("050000004c050000c405000000010000,PS4 Controller (Bluetooth),a:b1,b:b2,back:b8,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,guide:b12,leftshoulder:b4,leftstick:b10,lefttrigger:a3,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b11,righttrigger:a4,rightx:a2,righty:a5,start:b9,x:b0,y:b3,");
                        Add("060000004c0500006802000000010000,PS3 Controller (Bluetooth),a:b14,b:b13,y:b12,x:b15,start:b3,guide:b16,back:b0,leftstick:b1,rightstick:b2,leftshoulder:b10,rightshoulder:b11,dpup:b4,dpleft:b7,dpdown:b6,dpright:b5,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b8,righttrigger:b9,");
                        Add("03000000790000000600000010010000,DragonRise Inc.   Generic   USB  Joystick  ,x:b3,a:b2,b:b1,y:b0,back:b8,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:b6,rightshoulder:b5,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a3,righty:a4,");
                        Add("03000000666600000488000000010000,Super Joy Box 5 Pro,a:b2,b:b1,x:b3,y:b0,back:b9,start:b8,leftshoulder:b6,rightshoulder:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b4,righttrigger:b5,dpup:b12,dpleft:b15,dpdown:b14,dpright:b13,");
                        Add("05000000362800000100000002010000,OUYA Game Controller,a:b0,b:b3,dpdown:b9,dpleft:b10,dpright:b11,dpup:b8,guide:b14,leftshoulder:b4,leftstick:b6,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b7,righttrigger:a5,rightx:a3,righty:a4,x:b1,y:b2,");
                        Add("05000000362800000100000003010000,OUYA Game Controller,a:b0,b:b3,dpdown:b9,dpleft:b10,dpright:b11,dpup:b8,guide:b14,leftshoulder:b4,leftstick:b6,lefttrigger:a2,leftx:a0,lefty:a1,rightshoulder:b5,rightstick:b7,righttrigger:a5,rightx:a3,righty:a4,x:b1,y:b2,");
                        Add("030000008916000001fd000024010000,Razer Onza Classic Edition,x:b2,a:b0,b:b1,y:b3,back:b6,guide:b8,start:b7,dpleft:b11,dpdown:b14,dpright:b12,dpup:b13,leftshoulder:b4,lefttrigger:a2,rightshoulder:b5,righttrigger:a5,leftstick:b9,rightstick:b10,leftx:a0,lefty:a1,rightx:a3,righty:a4,");
                        Add("030000005e040000d102000001010000,Microsoft X-Box One pad,x:b2,a:b0,b:b1,y:b3,back:b6,guide:b8,start:b7,dpleft:h0.8,dpdown:h0.0,dpdown:h0.4,dpright:h0.0,dpright:h0.2,dpup:h0.0,dpup:h0.1,leftshoulder:h0.0,leftshoulder:b4,lefttrigger:a2,rightshoulder:b5,righttrigger:a5,leftstick:b9,rightstick:b10,leftx:a0,lefty:a1,rightx:a3,righty:a4,");
                        Add("03000000790000001100000010010000,RetroLink Saturn Classic Controller,x:b3,a:b0,b:b1,y:b4,back:b5,guide:b2,start:b8,leftshoulder:b6,rightshoulder:b7,leftx:a0,lefty:a1,");
                        Add("050000007e0500003003000001000000,Nintendo Wii U Pro Controller,a:b0,b:b1,x:b3,y:b2,back:b8,start:b9,guide:b10,leftshoulder:b4,rightshoulder:b5,leftstick:b11,rightstick:b12,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,dpup:b13,dpleft:b15,dpdown:b14,dpright:b16,");
                        Add("030000005e0400008e02000004010000,Microsoft X-Box 360 pad,a:b0,b:b1,x:b2,y:b3,back:b6,start:b7,guide:b8,leftshoulder:b4,rightshoulder:b5,leftstick:b9,rightstick:b10,leftx:a0,lefty:a1,rightx:a3,righty:a4,lefttrigger:a2,righttrigger:a5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,");
                        Add("030000000d0f00002200000011010000,HORI CO.,LTD. REAL ARCADE Pro.V3,x:b0,a:b1,b:b2,y:b3,back:b8,guide:b12,start:b9,leftshoulder:b4,lefttrigger:b6,rightshoulder:b5,righttrigger:b7,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,");
                        Add("030000000d0f00001000000011010000,HORI CO.,LTD. FIGHTING STICK 3,x:b0,a:b1,b:b2,y:b3,back:b8,guide:b12,start:b9,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:b6,rightshoulder:b5,righttrigger:b7,");
                        Add("03000000f0250000c183000010010000,Goodbetterbest Ltd USB Controller,x:b0,a:b1,b:b2,y:b3,back:b8,guide:b12,start:b9,dpleft:h0.8,dpdown:h0.0,dpdown:h0.4,dpright:h0.0,dpright:h0.2,dpup:h0.0,dpup:h0.1,leftshoulder:h0.0,leftshoulder:b4,lefttrigger:b6,rightshoulder:b5,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a2,righty:a3,");
                        Add("0000000058626f782047616d65706100,Xbox Gamepad (userspace driver),a:b0,b:b1,x:b2,y:b3,start:b7,back:b6,guide:b8,dpup:h0.1,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,leftshoulder:b4,rightshoulder:b5,lefttrigger:a5,righttrigger:a4,leftstick:b9,rightstick:b10,leftx:a0,lefty:a1,rightx:a2,righty:a3,");
                        Add("03000000ff1100003133000010010000,PC Game Controller,a:b2,b:b1,y:b0,x:b3,start:b9,back:b8,leftstick:b10,rightstick:b11,leftshoulder:b4,rightshoulder:b5,dpup:h0.1,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,leftx:a0,lefty:a1,rightx:a2,righty:a3,lefttrigger:b6,righttrigger:b7,");
                        Add("030000005e0400008e02000020200000,SpeedLink XEOX Pro Analog Gamepad pad,x:b2,a:b0,b:b1,y:b3,back:b6,guide:b8,start:b7,dpleft:h0.8,dpdown:h0.4,dpright:h0.2,dpup:h0.1,leftshoulder:b4,lefttrigger:a2,rightshoulder:b5,righttrigger:a5,leftstick:b9,rightstick:b10,leftx:a0,lefty:a1,rightx:a3,righty:a4,");
                        Add("030000006f0e00001304000000010000,Generic X-Box pad,x:b2,a:b0,b:b1,y:b3,back:b6,guide:b8,start:b7,dpleft:h0.8,dpdown:h0.0,dpdown:h0.4,dpright:h0.0,dpright:h0.2,dpup:h0.0,dpup:h0.1,leftshoulder:h0.0,leftshoulder:b4,lefttrigger:a2,rightshoulder:b5,righttrigger:a5,leftstick:a0,rightstick:a3,leftstick:b9,rightstick:b10,leftx:a0,lefty:a1,rightx:a3,righty:a4,");
                        Add("03000000a306000018f5000010010000,Saitek PLC Saitek P3200 Rumble Pad,x:b0,a:b1,b:b2,y:b3,back:b8,start:b9,dpleft:h0.8,dpdown:h0.0,dpdown:h0.4,dpright:h0.0,dpright:h0.2,dpup:h0.0,dpup:h0.1,leftshoulder:h0.0,leftshoulder:b4,lefttrigger:a2,rightshoulder:b6,rightshoulder:b5,righttrigger:b7,leftstick:b10,rightstick:b11,leftx:a0,lefty:a1,rightx:a3,righty:a4,");
                        Add("03000000830500006020000010010000,iBuffalo USB 2-axis 8-button Gamepad,a:b1,b:b0,x:b3,y:b2,back:b6,start:b7,leftshoulder:b4,rightshoulder:b5,leftx:a0,lefty:a1,");
                        Add("03000000c9110000f055000011010000,HJC Game GAMEPAD,leftx:a0,lefty:a1,dpdown:h0.4,rightstick:b11,rightshoulder:b5,rightx:a2,start:b9,righty:a3,dpleft:h0.8,lefttrigger:b6,x:b2,dpup:h0.1,back:b8,leftstick:b10,leftshoulder:b4,y:b3,a:b0,dpright:h0.2,righttrigger:b7,b:b1,");

                        // Android
                        Add("4e564944494120436f72706f72617469,NVIDIA Controller,a:b0,b:b1,dpdown:h0.4,dpleft:h0.8,dpright:h0.2,dpup:h0.1,leftshoulder:b9,leftstick:b7,lefttrigger:a4,leftx:a0,lefty:a1,rightshoulder:b10,rightstick:b8,righttrigger:a5,rightx:a2,righty:a3,start:b6,x:b2,y:b3,");

                    }

                    internal void Add(string config)
                    {
                        Guid guid = new Guid(config.Substring(0, 32));
                        if (!Configurations.ContainsKey(guid))
                        {
                            Configurations.Add(guid, config);
                        }
                        else
                        {
                            Configurations[guid] = config;
                        }
                    }

                    internal string this[Guid guid]
                    {
                        get
                        {
                            if (Configurations.ContainsKey(guid))
                            {
                                return Configurations[guid];
                            }
                            else
                            {
                                return Configurations[new Guid()]; // default configuration
                            }
                        }
                    }
                }

                IGamePadState IGamePadDriver.GetState(int index) => GetState(index);
                IGamePadCapabilities IGamePadDriver.GetCapabilities(int index) => GetCapabilities(index);
            }

            sealed class GamePadConfiguration : IEnumerable<GamePadConfigurationItem>
            {
                private static readonly char[] ConfigurationSeparator = new char[] { ',' };

                private readonly PrimitiveList<GamePadConfigurationItem> configuration_items =
                    new PrimitiveList<GamePadConfigurationItem>();

                public Guid Guid { get; private set; }

                public string Name { get; private set; }

                public GamePadConfiguration(string configuration)
                {
                    ParseConfiguration(configuration);
                }

                /// <summary>
                /// Parses a GamePad configuration string.
                /// This string must follow the rules for SDL2
                /// GameController outlined here:
                /// http://wiki.libsdl.org/SDL_GameControllerAddMapping
                /// </summary>
                /// <param name="configuration"></param>
                private void ParseConfiguration(string configuration)
                {
                    if (String.IsNullOrEmpty(configuration))
                    {
                        throw new ArgumentNullException();
                    }

                    // The mapping string has the format "GUID,name,config"
                    // - GUID is a unigue identifier returned by Joystick.GetGuid()
                    // - name is a human-readable name for the controller
                    // - config is a comma-separated list of configurations as follows:
                    //   - [gamepad axis or button name]:[joystick axis, button or hat number]
                    string[] items = configuration.Split(ConfigurationSeparator, StringSplitOptions.RemoveEmptyEntries);
                    if (items.Length < 3)
                    {
                        throw new ArgumentException();
                    }

                    GamePadConfiguration map = this;
                    map.Guid = new Guid(items[0]);
                    map.Name = items[1];
                    for (int i = 2; i < items.Length; i++)
                    {
                        string[] config = items[i].Split(':');
                        GamePadConfigurationTarget target = ParseTarget(config[0]);
                        GamePadConfigurationSource source = ParseSource(config[1]);
                        configuration_items.Add(new GamePadConfigurationItem(source, target));
                    }
                }

                /// <summary>
                /// Parses a gamepad configuration target string
                /// </summary>
                /// <param name="target">The string to parse</param>
                /// <returns>The configuration target (Button index, axis index etc.)</returns>
                private static GamePadConfigurationTarget ParseTarget(string target)
                {
                    switch (target)
                    {
                        // Buttons
                        case "a":
                            return new GamePadConfigurationTarget(PadButtons.A);
                        case "b":
                            return new GamePadConfigurationTarget(PadButtons.B);
                        case "x":
                            return new GamePadConfigurationTarget(PadButtons.X);
                        case "y":
                            return new GamePadConfigurationTarget(PadButtons.Y);
                        case "start":
                            return new GamePadConfigurationTarget(PadButtons.Start);
                        case "back":
                            return new GamePadConfigurationTarget(PadButtons.Back);
                        case "guide":
                            return new GamePadConfigurationTarget(PadButtons.BigButton);
                        case "leftshoulder":
                            return new GamePadConfigurationTarget(PadButtons.LeftShoulder);
                        case "rightshoulder":
                            return new GamePadConfigurationTarget(PadButtons.RightShoulder);
                        case "leftstick":
                            return new GamePadConfigurationTarget(PadButtons.LeftStick);
                        case "rightstick":
                            return new GamePadConfigurationTarget(PadButtons.RightStick);
                        case "dpup":
                            return new GamePadConfigurationTarget(PadButtons.DPadUp);
                        case "dpdown":
                            return new GamePadConfigurationTarget(PadButtons.DPadDown);
                        case "dpleft":
                            return new GamePadConfigurationTarget(PadButtons.DPadLeft);
                        case "dpright":
                            return new GamePadConfigurationTarget(PadButtons.DPadRight);

                        // Axes
                        case "leftx":
                            return new GamePadConfigurationTarget(GamePadAxes.LeftX);
                        case "lefty":
                            return new GamePadConfigurationTarget(GamePadAxes.LeftY);
                        case "rightx":
                            return new GamePadConfigurationTarget(GamePadAxes.RightX);
                        case "righty":
                            return new GamePadConfigurationTarget(GamePadAxes.RightY);

                        // Triggers
                        case "lefttrigger":
                            return new GamePadConfigurationTarget(GamePadAxes.LeftTrigger);
                        case "righttrigger":
                            return new GamePadConfigurationTarget(GamePadAxes.RightTrigger);


                        // Unmapped
                        default:
                            return new GamePadConfigurationTarget();
                    }
                }

                /// <summary>
                /// Creates a new gamepad configuration source from the given string
                /// </summary>
                /// <param name="item">The string to parse</param>
                /// <returns>The new gamepad configuration source</returns>
                private static GamePadConfigurationSource ParseSource(string item)
                {
                    if (String.IsNullOrEmpty(item))
                    {
                        return new GamePadConfigurationSource();
                    }

                    switch (item[0])
                    {
                        case 'a':
                            return new GamePadConfigurationSource(isAxis: true, index: ParseIndex(item));

                        case 'b':
                            return new GamePadConfigurationSource(isAxis: false, index: ParseIndex(item));

                        case 'h':
                            {
                                HatLocation position;
                                JoystickHat hat = ParseHat(item, out position);
                                return new GamePadConfigurationSource(hat, position);
                            }

                        default:
                            throw new InvalidOperationException("[Input] Invalid GamePad configuration value");
                    }
                }

                /// <summary>
                /// Parses a string  the format a#" where:
                /// - # is a zero-based integer number
                /// </summary>
                /// <param name="item">The string to parse</param>
                /// <returns>The index of the axis or button</returns>
                private static int ParseIndex(string item)
                {
                    // item is  the format "a#" where # a zero-based integer number
                    return Int32.Parse(item.Substring(1)); ;
                }

                /// <summary>
                /// Parses a string  the format "h#.#" where:
                /// - the 1st # is the zero-based hat id
                /// - the 2nd # is a bit-flag defining the hat position
                /// </summary>
                /// <param name="item">The string to parse</param>
                /// <param name="position">The hat position assigned via 'out'</param>
                /// <returns>The new joystick hat</returns>
                private static JoystickHat ParseHat(string item, out HatLocation position)
                {
                    JoystickHat hat = JoystickHat.Hat0;
                    int id = Int32.Parse(item.Substring(1, 1));
                    int pos = Int32.Parse(item.Substring(3));

                    position = HatLocation.Centered;
                    switch (pos)
                    {
                        case 1: position = HatLocation.Up; break;
                        case 2: position = HatLocation.Right; break;
                        case 3: position = HatLocation.UpRight; break;
                        case 4: position = HatLocation.Down; break;
                        case 6: position = HatLocation.DownRight; break;
                        case 8: position = HatLocation.Left; break;
                        case 9: position = HatLocation.UpLeft; break;
                        case 12: position = HatLocation.DownLeft; break;
                        default: position = HatLocation.Centered; break;
                    }

                    return (JoystickHat)((byte)hat + id);
                }

                public IEnumerator<GamePadConfigurationItem> GetEnumerator()
                {
                    return configuration_items.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }
            class GamePadConfigurationItem
            {
                public GamePadConfigurationItem(GamePadConfigurationSource source, GamePadConfigurationTarget target)
                {
                    Source = source;
                    Target = target;
                }

                public GamePadConfigurationSource Source { get; private set; }
                public GamePadConfigurationTarget Target { get; private set; }
            }
        }
#endregion
    }
    static partial class SdlJoystickDriver
    {
#region sdl joystick mapping
        [DllImport(libSDL, EntryPoint = "SDL_JoystickClose", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern void JoystickClose(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickEventState", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern EventState JoystickEventState(EventState enabled);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetAxis", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern short JoystickGetAxis(IntPtr joystick, int axis);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetButton", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern byte JoystickGetButton(IntPtr joystick, int button);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetGUID", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern JoystickGuid JoystickGetGUID(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickInstanceID", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int JoystickInstanceID(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickName", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern IntPtr JoystickNameInternal(IntPtr joystick);
        static string JoystickName(IntPtr joystick)
        {
            unsafe
            {
                return new string((char*)JoystickNameInternal(joystick));
            }
        }

        [DllImport(libSDL, EntryPoint = "SDL_JoystickNumAxes", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int JoystickNumAxes(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickNumBalls", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int JoystickNumBalls(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickNumButtons", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int JoystickNumButtons(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickNumHats", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int JoystickNumHats(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickOpenCallingConvention = CallingConvention.Cdecl, ", ExactSpelling = true)]
        static extern IntPtr JoystickOpen(int device_index);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickUpdate", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern void JoystickUpdate();

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetAxisInitialState", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int JoystickGetAxisInitialState(IntPtr joystick, int axis, out ushort state);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetBall", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int JoystickGetBall(IntPtr joystick, int ball, out int dx, out int dy);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetHat", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern byte JoystickGetHat(IntPtr joystick, int hat);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickNameForIndex", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr joystickNameForIndex(int device_index);
        static string JoystickNameForIndex(int device_index) =>
           Operations.UTF8_ToManaged(joystickNameForIndex(device_index));

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetDeviceGUID", CallingConvention = CallingConvention.Cdecl)]
        static extern Guid JoystickGetDeviceGUID(int device_index);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetGUIDString", CallingConvention = CallingConvention.Cdecl)]
        static extern void JoystickGetGUIDString(Guid guid, byte[] pszGUID, int cbGUID);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetGUIDFromString", CallingConvention = CallingConvention.Cdecl)]
        static extern Guid joystickGetGUIDFromString(byte[] pchGUID);
        static Guid JoystickGetGUIDFromString(string pchGuid) =>
           joystickGetGUIDFromString(SdlAPI.UTF8_ToNative(pchGuid));

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetDeviceVendor", CallingConvention = CallingConvention.Cdecl)]
        static extern ushort JoystickGetDeviceVendor(int device_index);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetDeviceProduct", CallingConvention = CallingConvention.Cdecl)]
        static extern ushort JoystickGetDeviceProduct(int device_index);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetDeviceProductVersion", CallingConvention = CallingConvention.Cdecl)]
        static extern ushort JoystickGetDeviceProductVersion(int device_index);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetDeviceType", CallingConvention = CallingConvention.Cdecl)]
        static extern JoystickType JoystickGetDeviceType(int device_index);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetDeviceInstanceID", CallingConvention = CallingConvention.Cdecl)]
        static extern int JoystickGetDeviceInstanceID(int device_index);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetVendor", CallingConvention = CallingConvention.Cdecl)]
        static extern ushort JoystickGetVendor(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetProduct", CallingConvention = CallingConvention.Cdecl)]
        static extern ushort JoystickGetProduct(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetProductVersion", CallingConvention = CallingConvention.Cdecl)]
        static extern ushort JoystickGetProductVersion(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickType", CallingConvention = CallingConvention.Cdecl)]
        static extern JoystickType JoystickGetType(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickGetAttached", CallingConvention = CallingConvention.Cdecl)]
        static extern int JoystickGetAttached(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickPowerLevel", CallingConvention = CallingConvention.Cdecl)]
        static extern JoystickPowerLevel JoystickCurrentPowerLevel(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickFromInstanceID", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr JoystickFromInstanceID(int joyid);

        [DllImport(libSDL, EntryPoint = "SDL_JoystickIsHaptic", CallingConvention = CallingConvention.Cdecl)]
        static extern int JoystickIsHaptic(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_LockJoysticks", CallingConvention = CallingConvention.Cdecl)]
        static extern void LockJoysticks();

        [DllImport(libSDL, EntryPoint = "SDL_NumJoysticks", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int NumJoysticks();

        [DllImport(libSDL, EntryPoint = "SDL_UnlockJoysticks", CallingConvention = CallingConvention.Cdecl)]
        static extern void UnlockJoysticks();

#region sdl Game controller mapping
        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetBindForAxis", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern GameControllerButtonBind GameControllerGetBindForAxis(IntPtr gamecontroller, GameControllerAxis axis);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetBindForButton", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern GameControllerButtonBind GameControllerGetBindForButton(
            IntPtr gamecontroller, GameControllerButton button);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerOpen", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern IntPtr GameControllerOpen(int joystick_index);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetJoystick", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern IntPtr GameControllerGetJoystick(IntPtr gamecontroller);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerEventState", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern EventState GameControllerEventState(EventState state);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetAxis", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern short GameControllerGetAxis(IntPtr gamecontroller, GameControllerAxis axis);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetButton", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern bool GameControllerGetButton(IntPtr gamecontroller, GameControllerButton button);


        [DllImport(libSDL, EntryPoint = "SDL_GameControllerName", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern IntPtr GameControllerNameInternal(IntPtr gamecontroller);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerAddMapping", CallingConvention = CallingConvention.Cdecl)]
        static extern int gameControllerAddMapping(byte[] mappingString);
        static int GameControllerAddMapping(string mappingString) =>
           gameControllerAddMapping(SdlAPI.UTF8_ToNative(mappingString));

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerNumMappings", CallingConvention = CallingConvention.Cdecl)]
        static extern int GameControllerNumMappings();

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerMappingForIndex", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr gameControllerMappingForIndex(int mapping_index);
        static string GameControllerMappingForIndex(int mapping_index) =>
           Operations.UTF8_ToManaged(gameControllerMappingForIndex(mapping_index));

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerAddMappingsFromRW", CallingConvention = CallingConvention.Cdecl)]
        static extern int gameControllerAddMappingsFromRW(IntPtr rw, int freerw);
        static int GameControllerAddMappingsFromFile(string file)
        {
            IntPtr rwops = SdlAPI.OpenFile(SdlAPI.UTF8_ToNative(file), SdlAPI.UTF8_ToNative("rb"));
            return gameControllerAddMappingsFromRW(rwops, 1);
        }

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerMappingForGUID", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr gameControllerMappingForGUID(Guid guid);
        static string GameControllerMappingForGUID(Guid guid) =>
           Operations.UTF8_ToManaged(gameControllerMappingForGUID(guid));

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerMapping", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr gameControllerMapping(IntPtr gamecontroller);
        static string GameControllerMapping(IntPtr gamecontroller) =>
           Operations.UTF8_ToManaged(gameControllerMapping(gamecontroller));

        [DllImport(libSDL, EntryPoint = "SDL_IsGameController", CallingConvention = CallingConvention.Cdecl)]
        static extern int IsGameController(int joystick_index);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerNameForIndex", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr gameControllerNameForIndex(int joystick_index);
        static string GameControllerNameForIndex(int joystick_index) =>
           Operations.UTF8_ToManaged(gameControllerNameForIndex(joystick_index));

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerName", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr gameControllerName(IntPtr gamecontroller);
        static string GameControllerName(IntPtr gamecontroller) =>
           Operations.UTF8_ToManaged(gameControllerName(gamecontroller));

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetVendor", CallingConvention = CallingConvention.Cdecl)]
        static extern ushort GameControllerGetVendor(IntPtr gamecontroller);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetProduct", CallingConvention = CallingConvention.Cdecl)]
        static extern ushort GameControllerGetProduct(IntPtr gamecontroller);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetProductVersion", CallingConvention = CallingConvention.Cdecl)]
        static extern ushort GameControllerGetProductVersion(IntPtr gamecontroller);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetAttached", CallingConvention = CallingConvention.Cdecl)]
        static extern int GameControllerGetAttached(IntPtr gamecontroller);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerUpdate", CallingConvention = CallingConvention.Cdecl)]
        static extern void GameControllerUpdate();

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetAxisFromString", CallingConvention = CallingConvention.Cdecl)]
        static extern GameControllerAxis gameControllerGetAxisFromString(byte[] pchString);
        static GameControllerAxis GameControllerGetAxisFromString(string pchString) =>
            gameControllerGetAxisFromString(SdlAPI.UTF8_ToNative(pchString));

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetStringForAxis", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr gameControllerGetStringForAxis(GameControllerAxis axis);
        static string GameControllerGetStringForAxis(GameControllerAxis axis) =>
           Operations.UTF8_ToManaged(gameControllerGetStringForAxis(axis));

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetButtonFromString", CallingConvention = CallingConvention.Cdecl)]
        static extern GameControllerButton gameControllerGetButtonFromString(byte[] pchString);
        static GameControllerButton GameControllerGetButtonFromString(string pchString) =>
           gameControllerGetButtonFromString(SdlAPI.UTF8_ToNative(pchString));

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerGetStringForButton", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr gameControllerGetStringForButton(GameControllerButton button);
        static string GameControllerGetStringForButton(GameControllerButton button) =>
           Operations.UTF8_ToManaged(gameControllerGetStringForButton(button));

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerClose", CallingConvention = CallingConvention.Cdecl)]
        static extern void GameControllerClose(IntPtr gamecontroller);

        [DllImport(libSDL, EntryPoint = "SDL_GameControllerClose", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GameControllerFromInstanceID(int joyid);

        [DllImport(libSDL, EntryPoint = "SDL_HapticClose", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern void HapticClose(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticDestroyEffect", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern void HapticDestroyEffect(IntPtr haptic, HapticEffects effect);

        [DllImport(libSDL, EntryPoint = "SDL_HapticEffectSupported", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int HapticEffectSupported(IntPtr haptic, ref HapticEffect effect);

        [DllImport(libSDL, EntryPoint = "SDL_HapticGetEffectStatus", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int HapticGetEffectStatus(IntPtr haptic, int effect);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_HapticIndex", ExactSpelling = true)]
        static extern int HapticIndex(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticName", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr hapticName(int device_index);
        static string HapticName(int device_index) =>
           Operations.UTF8_ToManaged(hapticName(device_index));

        [DllImport(libSDL, EntryPoint = "SDL_HapticNewEffect", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticNewEffect(IntPtr haptic, ref HapticEffect effect);

        [DllImport(libSDL, EntryPoint = "SDL_HapticNumAxes", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticNumAxes(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticNumEffects", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticNumEffects(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticNumEffectsPlaying", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticNumEffectsPlaying(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticOpen", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr HapticOpen(int device_index);

        [DllImport(libSDL, EntryPoint = "SDL_HapticOpened", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticOpened(int device_index);

        [DllImport(libSDL, EntryPoint = "SDL_HapticOpenFromJoystick", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr HapticOpenFromJoystick(IntPtr joystick);

        [DllImport(libSDL, EntryPoint = "SDL_HapticOpenFromMouse", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr HapticOpenFromMouse();

        [DllImport(libSDL, EntryPoint = "SDL_HapticPause", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticPause(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticQuery", CallingConvention = CallingConvention.Cdecl)]
        static extern uint HapticQuery(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticRumbleInit", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticRumbleInit(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticRumblePlay", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticRumblePlay(IntPtr haptic, float strength, uint length);

        [DllImport(libSDL, EntryPoint = "SDL_HapticRumbleStop", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticRumbleStop(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticRumbleSupported", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticRumbleSupported(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticRunEffect", CallingConvention = CallingConvention.Cdecl)]
        static extern int runEffect(IntPtr haptic, HapticRunEffect effect, uint iterations);

        [DllImport(libSDL, EntryPoint = "SDL_HapticSetAutocenter", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticSetAutocenter(IntPtr haptic, int autocenter);

        [DllImport(libSDL, EntryPoint = "SDL_HapticSetGain", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticSetGain(IntPtr haptic, int gain);

        [DllImport(libSDL, EntryPoint = "SDL_HapticStopAll", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticStopAll(IntPtr haptic);

        [DllImport(libSDL, EntryPoint = "SDL_HapticStopEffect", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticStopEffect(IntPtr haptic, HapticEffects effect);

        [DllImport(libSDL, EntryPoint = "SDL_HapticUnpause", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticUnpause(IntPtr haptic);


        [DllImport(libSDL, EntryPoint = "SDL_HapticUpdateEffect", CallingConvention = CallingConvention.Cdecl)]
        static extern int HapticUpdateEffect(IntPtr haptic, HapticEffects effect, ref HapticEffect IntPtr);

        [DllImport(libSDL, EntryPoint = "SDL_NumHaptics", CallingConvention = CallingConvention.Cdecl)]
        static extern int NumHaptics();
#endregion

#endregion
    }
}
#endif