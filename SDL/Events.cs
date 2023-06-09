/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

/* We have used some part of the below class and structs definitions from the...
 * OpenTK - https://github.com/opentk/opentk
   *Copyright (c) 2006-2019 Stefanos Apostolopoulos for the Open Toolkit project.
   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
// Author: Manan Adhvaryu.

#if Window && SDL
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MnM.GWS.SDL
{
    #region FILEDROP-EVENTARGS
    class FileDropEventArgs : EventArgs, IFileDropEventArgs
    {
        const string toStr = "fileName:{0}";

        internal FileDropEventArgs() { }
        public FileDropEventArgs(string fileName)
        {
            FileName = fileName;
        }
        public string FileName { get; set; }
    }
    #endregion

    #region TOUCHEVENT-ARGS
    class TouchEventArgs : EventArgs, ITouchEventArgs
    {
        const string toStr = "x:{0}, y{1}, fingerId:{2}";

        public TouchEventArgs(float x, float y, long fingerId)
        {
            Finger = fingerId;
            X = x;
            Y = y;
        }
        internal TouchEventArgs(TouchFingerEvent e)
        {
            Finger = e.fingerId;
            X = e.x;
            Y = e.y;
        }
        public override string ToString()
        {
            return "[TouchEventArgs]" +
                   " Finger(" + Finger + ")" +
                   " X(" + X + ")" +
                   " Y(" + Y + ")";
        }

        /// <summary>Index of the finger  case of multi-touch events</summary>
        public long Finger { get; internal set; }

        /// <summary>X position of the touch, relative to the left of the owner window</summary>
        public float X { get; internal set; }

        /// <summary>Y position of the touch, relative to the top of the owner window</summary>
        public float Y { get; internal set; }
    }
    #endregion

    #region KEYPRESS-EVENTARGS
    class KeyPressEventArgs : EventArgs, IKeyPressEventArgs
    {
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="keyChar">The ASCII character that was typed.</param>
        public KeyPressEventArgs(char keyChar)
        {
            KeyChar = keyChar;
        }

        /// <summary>
        /// Gets a <see cref="System.Char"/> that defines the ASCII character that was typed.
        /// </summary>
        public char KeyChar { get; set; }
    }
    #endregion

    #region IJOYSTICK-BUTTON-EVENTARGS
    class JoystickButtonEventArgs : EventArgs, IJoystickButtonEventArgs
    {
        const string toStr = "button:{0}, pressed:{1}";
        /// <summary>
        /// Initializes a new instance of the <see cref="JoystickButtonEventArgs"/> class.
        /// </summary>
        /// <param name="button">The index of the joystick button for the event.</param>
        /// <param name="pressed">The current state of the button.</param>
        internal JoystickButtonEventArgs(int button, bool pressed)
        {
            this.Button = button;
            this.Pressed = pressed;
        }

        internal JoystickButtonEventArgs(int button, bool pressed, int device)
        {
            Button = button;
            Pressed = pressed;
            Which = device;
        }

        /// <summary>
        /// The index of the joystick button for the event.
        /// </summary>
        public int Button { get; internal set; }

        /// <summary>
        /// Gets a System.Boolean representing the state of the button for the event.
        /// </summary>
        public bool Pressed { get; internal set; }

        public int Which { get; internal set; }

        public override string ToString()
        {
            return string.Format(toStr, Button, Pressed);
        }
    }
    #endregion

    #region JOYSTICK-AXIS-EVENTARGS
    class JoystickAxisEventArgs : EventArgs, IJoystickAxisEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JoystickAxisEventArgs"/> class.
        /// </summary>
        /// <param name="axis">The index of the joystick axis that was moved.</param>
        /// <param name="value">The absolute value of the joystick axis.</param>
        /// <param name="delta">The relative change  value of the joystick axis.</param>
        public JoystickAxisEventArgs(int axis, float value, float delta)
        {
            this.Axis = axis;
            this.Value = value;
            this.Delta = delta;
        }

        public JoystickAxisEventArgs(int axis, float value, float delta, int device)
        {
            Axis = axis;
            Value = value;
            Delta = delta;
            Device = device;
        }

        /// <summary>
        /// Gets a System.Int32 representing the index of the axis that was moved.
        /// </summary>
        public int Axis { get; internal set; }

        /// <summary>
        /// Gets a System.Single representing the absolute position of the axis.
        /// </summary>
        public float Value { get; internal set; }

        /// <summary>
        /// Gets a System.Single representing the relative change  the position of the axis.
        /// </summary>
        public float Delta { get; internal set; }

        public int Device { get; internal set; }
    }
    #endregion

    #region EVENT
    [StructLayout(LayoutKind.Explicit)]
    struct Event : IExExternalEventInfo
    {
        [FieldOffset(0)]
        public EventType Type;

        [FieldOffset(0)]
        public WinEvent Window;

        [FieldOffset(0)]
        public KeyboardEvent KeyboardEvent;

        [FieldOffset(0)]
        public TextEditingEvent TextEditEvent;

        [FieldOffset(0)]
        public TextInputEvent TextInputEvent;

        [FieldOffset(0)]
        public MouseMotionEvent MouseMotionEvent;

        [FieldOffset(0)]
        public MouseButtonEvent MouseButtonEvent;

        [FieldOffset(0)]
        public MouseWheelEvent MouseWheelEvent;

        [FieldOffset(0)]
        public DropEvent DropEvent;

        [FieldOffset(0)]
        public QuitEvent QuitEvent;

        [FieldOffset(0)]
        public UserEvent UserEvent;

        [FieldOffset(0)]
        internal SysWMEvent Syswm;

        [FieldOffset(0)]
        public TouchFingerEvent TouchEvent;

        [FieldOffset(0)]
        public MultiGestureEvent MultiTouchEvent;

        [FieldOffset(0)]
        public DollarGestureEvent DollarGestureEvent;

        [FieldOffset(0)]
        public JoyAxisEvent JoyAxisEvent;

        [FieldOffset(0)]
        public JoyBallEvent JoyBallEvent;

        [FieldOffset(0)]
        public JoyHatEvent JoyHatEvent;

        [FieldOffset(0)]
        public JoyButtonEvent JoyButtonEvent;

        [FieldOffset(0)]
        public JoyDeviceEvent JoyDeviceEvent;

        [FieldOffset(0)]
        public ControllerAxisEvent ControllerAxisEvent;

        [FieldOffset(0)]
        public ControllerButtonEvent ControllerButtonEent;

        [FieldOffset(0)]
        public ControllerDeviceEvent ControllerDeviceEvent;

        // Ensure the structure is big enough
        // This hack is necessary to ensure compatibility
        // with different SDL versions, which might have
        // different sizeof(SDL_Event).
        [FieldOffset(0)]
        private unsafe fixed byte reserved[128];

        GwsEvent IExternalEventInfo.Type
        {
            get
            {
                switch (Type)
                {
                    case EventType.FIRSTEVENT:
                        return GwsEvent.First;
                    case EventType.QUIT:
                        return GwsEvent.Quit;
                    case EventType.WINDOWEVENT:
                        switch ((GWS.WindowEvent)Window.Event)
                        {
                            case GWS.WindowEvent.Shown:
                                return GwsEvent.Shown;
                            case GWS.WindowEvent.Hidden:
                                return GwsEvent.Hidden;
                            case GWS.WindowEvent.Exposed:
                                return GwsEvent.Hidden;
                            case GWS.WindowEvent.Moved:
                                return GwsEvent.Moved;
                            case GWS.WindowEvent.Resized:
                                return GwsEvent.Resized;
                            case GWS.WindowEvent.SizeChanged:
                                return GwsEvent.SizeChanged;
                            case GWS.WindowEvent.MiniMized:
                                return GwsEvent.Minimized;
                            case GWS.WindowEvent.Maximized:
                                return GwsEvent.Maximized;
                            case GWS.WindowEvent.Restored:
                                return GwsEvent.Restored;
                            case GWS.WindowEvent.MouseEnter:
                                return GwsEvent.MouseEnter;
                            case GWS.WindowEvent.MouseLeave:
                                return GwsEvent.MouseLeave;
                            case GWS.WindowEvent.GotFocus:
                                return GwsEvent.GotFocus;
                            case GWS.WindowEvent.LostFocus:
                                return GwsEvent.LostFocus;
                            case GWS.WindowEvent.Close:
                                return GwsEvent.Close;
                            default:
                                break;
                        }
                        return GwsEvent.WindowEvent;
                    case EventType.SYSWMEVENT:
                        return GwsEvent.SysWmEvent;
                    case EventType.KEYDOWN:
                        return GwsEvent.KeyDown;
                    case EventType.KEYUP:
                        return GwsEvent.KeyUp;
                    case EventType.TEXTEDITING:
                    case EventType.KEYPRESS:
                        return GwsEvent.KeyPress;
                    case EventType.MOUSEMOTION:
                        return GwsEvent.MouseMotion;
                    case EventType.MOUSEBUTTONDOWN:
                        return GwsEvent.MouseDown;
                    case EventType.MOUSEBUTTONUP:
                        return GwsEvent.MouseUp;
                    case EventType.MOUSEWHEEL:
                        return GwsEvent.MouseWheel;
                    case EventType.JOYAXISMOTION:
                        return GwsEvent.JoyAxisMotion;
                    case EventType.JOYBALLMOTION:
                        return GwsEvent.JoyBallMotion;
                    case EventType.JOYHATMOTION:
                        return GwsEvent.JoyHatMotion;
                    case EventType.JOYBUTTONDOWN:
                        return GwsEvent.JoyButtonDown;
                    case EventType.JOYBUTTONUP:
                        return GwsEvent.JoyButtonUp;
                    case EventType.JOYDEVICEADDED:
                        return GwsEvent.JoystickAdded;
                    case EventType.JOYDEVICEREMOVED:
                        return GwsEvent.JoystickRemoved;
                    case EventType.CONTROLLERAXISMOTION:
                        return GwsEvent.ControllerAxisMotion;
                    case EventType.CONTROLLERBUTTONDOWN:
                        return GwsEvent.ControllerButtonDown;
                    case EventType.CONTROLLERBUTTONUP:
                        return GwsEvent.ControllerButtonUp;
                    case EventType.CONTROLLERDEVICEADDED:
                        return GwsEvent.ControllerAdded;
                    case EventType.CONTROLLERDEVICEREMOVED:
                        return GwsEvent.ControllerRemoved;
                    case EventType.CONTROLLERDEVICEREMAPPED:
                        return GwsEvent.ControllerMapped;
                    case EventType.FINGERDOWN:
                        return GwsEvent.FingerDown;
                    case EventType.FINGERUP:
                        return GwsEvent.FingerUp;
                    case EventType.FINGERMOTION:
                        return GwsEvent.FingerMotion;
                    case EventType.DOLLARGESTURE:
                        return GwsEvent.DollarGesture;
                    case EventType.DOLLARRECORD:
                        return GwsEvent.DollarRecord;
                    case EventType.MULTIGESTURE:
                        return GwsEvent.MultiGesture;
                    case EventType.CLIPBOARDUPDATE:
                        return GwsEvent.ClipBoardUpdate;
                    case EventType.DROPFILE:
                        return GwsEvent.DropFile;
                    case EventType.USEREVENT:
                        return GwsEvent.UserEvent;
                    case EventType.LASTEVENT:
                        return GwsEvent.LASTEVENT;
                    case EventType.DROPTEXT:
                        return GwsEvent.DropText;
                    case EventType.DROPBEGIN:
                        return GwsEvent.DropBegin;
                    case EventType.DROPCOMPLETE:
                        return GwsEvent.DropComplete;
                    case EventType.AUDIODEVICEADDED:
                        return GwsEvent.AudioDeviceAdded;
                    case EventType.AUDIODEVICEREMOVED:
                        return GwsEvent.AudioDeviceRemoved;
                    case EventType.RENDER_TARGETS_RESET:
                    case EventType.RENDER_DEVICE_RESET:
                    default:
                        return (GwsEvent)Type;
                }
            }
        }
        IWindowEventInfo IExternalEventInfo.Window => Window;
        public string ID => "Window" + Window.WindowID;
    }
    #endregion

    #region SYSWM-EVENT
    /* A video driver dependent event (event.syswm.*), disabled */
    [StructLayout(LayoutKind.Sequential)]
    struct SysWMEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public IntPtr msg; /* SDL_SysWMmsg*, system-dependent*/
    }
    #endregion

    #region WINDOW-EVENT
    struct WinEvent : IWindowEventInfo
    {
        public EventType Type;
        public uint Timestamp;
        public int WindowID;
        public byte Event;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public int Data1;
        public int Data2;

        GwsEvent IWindowEventInfo.Type
        {
            get
            {
                switch (Type)
                {
                    case EventType.FIRSTEVENT:
                        return GwsEvent.First;
                    case EventType.QUIT:
                        return GwsEvent.Quit;
                    case EventType.WINDOWEVENT:
                        switch ((GWS.WindowEvent)Event)
                        {
                            case GWS.WindowEvent.Shown:
                                return GwsEvent.Shown;
                            case GWS.WindowEvent.Hidden:
                                return GwsEvent.Hidden;
                            case GWS.WindowEvent.Exposed:
                                return GwsEvent.Hidden;
                            case GWS.WindowEvent.Moved:
                                return GwsEvent.Moved;
                            case GWS.WindowEvent.Resized:
                                return GwsEvent.Resized;
                            case GWS.WindowEvent.SizeChanged:
                                return GwsEvent.SizeChanged;
                            case GWS.WindowEvent.MiniMized:
                                return GwsEvent.Minimized;
                            case GWS.WindowEvent.Maximized:
                                return GwsEvent.Maximized;
                            case GWS.WindowEvent.Restored:
                                return GwsEvent.Restored;
                            case GWS.WindowEvent.MouseEnter:
                                return GwsEvent.MouseEnter;
                            case GWS.WindowEvent.MouseLeave:
                                return GwsEvent.MouseLeave;
                            case GWS.WindowEvent.GotFocus:
                                return GwsEvent.GotFocus;
                            case GWS.WindowEvent.LostFocus:
                                return GwsEvent.LostFocus;
                            case GWS.WindowEvent.Close:
                                return GwsEvent.Close;
                            default:
                                break;
                        }
                        return GwsEvent.WindowEvent;
                    case EventType.SYSWMEVENT:
                        return GwsEvent.SysWmEvent;
                    case EventType.KEYDOWN:
                        return GwsEvent.KeyDown;
                    case EventType.KEYUP:
                        return GwsEvent.KeyUp;
                    case EventType.TEXTEDITING:
                    case EventType.KEYPRESS:
                        return GwsEvent.KeyPress;
                    case EventType.MOUSEMOTION:
                        return GwsEvent.MouseMotion;
                    case EventType.MOUSEBUTTONDOWN:
                        return GwsEvent.MouseDown;
                    case EventType.MOUSEBUTTONUP:
                        return GwsEvent.MouseUp;
                    case EventType.MOUSEWHEEL:
                        return GwsEvent.MouseWheel;
                    case EventType.JOYAXISMOTION:
                        return GwsEvent.JoyAxisMotion;
                    case EventType.JOYBALLMOTION:
                        return GwsEvent.JoyBallMotion;
                    case EventType.JOYHATMOTION:
                        return GwsEvent.JoyHatMotion;
                    case EventType.JOYBUTTONDOWN:
                        return GwsEvent.JoyButtonDown;
                    case EventType.JOYBUTTONUP:
                        return GwsEvent.JoyButtonUp;
                    case EventType.JOYDEVICEADDED:
                        return GwsEvent.JoystickAdded;
                    case EventType.JOYDEVICEREMOVED:
                        return GwsEvent.JoystickRemoved;
                    case EventType.CONTROLLERAXISMOTION:
                        return GwsEvent.ControllerAxisMotion;
                    case EventType.CONTROLLERBUTTONDOWN:
                        return GwsEvent.ControllerButtonDown;
                    case EventType.CONTROLLERBUTTONUP:
                        return GwsEvent.ControllerButtonUp;
                    case EventType.CONTROLLERDEVICEADDED:
                        return GwsEvent.ControllerAdded;
                    case EventType.CONTROLLERDEVICEREMOVED:
                        return GwsEvent.ControllerRemoved;
                    case EventType.CONTROLLERDEVICEREMAPPED:
                        return GwsEvent.ControllerMapped;
                    case EventType.FINGERDOWN:
                        return GwsEvent.FingerDown;
                    case EventType.FINGERUP:
                        return GwsEvent.FingerUp;
                    case EventType.FINGERMOTION:
                        return GwsEvent.FingerMotion;
                    case EventType.DOLLARGESTURE:
                        return GwsEvent.DollarGesture;
                    case EventType.DOLLARRECORD:
                        return GwsEvent.DollarRecord;
                    case EventType.MULTIGESTURE:
                        return GwsEvent.MultiGesture;
                    case EventType.CLIPBOARDUPDATE:
                        return GwsEvent.ClipBoardUpdate;
                    case EventType.DROPFILE:
                        return GwsEvent.DropFile;
                    case EventType.USEREVENT:
                        return GwsEvent.UserEvent;
                    case EventType.LASTEVENT:
                        return GwsEvent.LASTEVENT;
                    case EventType.DROPTEXT:
                        return GwsEvent.DropText;
                    case EventType.DROPBEGIN:
                        return GwsEvent.DropBegin;
                    case EventType.DROPCOMPLETE:
                        return GwsEvent.DropComplete;
                    case EventType.AUDIODEVICEADDED:
                        return GwsEvent.AudioDeviceAdded;
                    case EventType.AUDIODEVICEREMOVED:
                        return GwsEvent.AudioDeviceRemoved;
                    case EventType.RENDER_TARGETS_RESET:
                    case EventType.RENDER_DEVICE_RESET:
                    default:
                        return (GwsEvent)Type;
                }
            }
        }
        int IWindowEventInfo.WindowID => WindowID;
        int IWindowEventInfo.Event => Event;
    }
    #endregion

    #region KEYSYM
    struct Keysym
    {
        public Scancode Scancode;
        public Keycode Sym;
        public Keymod Mod;
        [Obsolete]
        public uint Unicode;
    }
    #endregion

    #region KEYBOARD-EVENT
    struct KeyboardEvent
    {
        public EventType Type;
        public uint Timestamp;
        public int WindowID;
        public InputState State;
        public byte Repeat;
        private byte padding2;
        private byte padding3;
        internal Keysym Keysym;
    }
    #endregion

    #region TEXT EDITING EVENT
    struct TextEditingEvent
    {
        public const int TextSize = 32;

        public EventType Type;
        public UInt32 Timestamp;
        public UInt32 WindowID;
        public unsafe fixed byte Text[TextSize];
        public Int32 Start;
        public Int32 Length;
    }
    #endregion

    #region TEXT INPUT EVENT
    struct TextInputEvent
    {
        public const int TextSize = 32;

        public EventType Type;
        public UInt32 Timestamp;
        public UInt32 WindowID;
        public unsafe fixed byte Text[TextSize];
    }
    #endregion

    #region SIZE EVENT
    [StructLayout(LayoutKind.Sequential)]
    struct SizeEvent
    {
        /// <summary>New width of the window</summary>
        public uint Width;

        /// <summary>New height of the window</summary>
        public uint Height;
    }
    #endregion

    #region DROP EVENT
    struct DropEvent
    {
        public uint Type;
        public uint Timestamp;
        public IntPtr File;
        public int WindowID;
    }
    #endregion

    #region QUIT EVENT
    [StructLayout(LayoutKind.Sequential)]
    struct QuitEvent
    {
        internal EventType Type;
        public uint Timestamp;
    }
    #endregion

    #region USER EVENT
    [StructLayout(LayoutKind.Sequential)]
    struct UserEvent
    {
        public UInt32 type;
        public UInt32 timestamp;
        public UInt32 windowID;
        public Int32 code;
        public IntPtr data1; /* user-defined */
        public IntPtr data2; /* user-defined */
    }
    #endregion

    #region MOUSE BUTTON EVENT
    struct MouseButtonEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int WindowID;
        public uint Which;
        public _MouseButton Button;
        public InputState State;
        public byte Clicks;
        private byte padding1;
        public int X;
        public int Y;
    }
    #endregion

    #region MOUSE MOTION EVENT
    struct MouseMotionEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int WindowID;
        public uint Which;
        internal ButtonFlags State;
        public int X;
        public int Y;
        public int Xrel;
        public int Yrel;
    }
    #endregion

    #region MOUSE WHEEL EVENT
    struct MouseWheelEvent
    {
        public EventType Type;
        public uint Timestamp;
        public int WindowID;
        public uint Which;
        public int X;
        public int Y;

        [Flags]
        public enum EventType : uint
        {
            /* Touch events */
            FingerDown = 0x700,
            FingerUp,
            FingerMotion,

            /* Gesture events */
            DollarGesture = 0x800,
            DollarRecord,
            MultiGesture,
        }

        public const uint TouchMouseID = 0xffffffff;
    }
    #endregion

    #region TOUCH FINGER EVENT
    [StructLayout(LayoutKind.Sequential)]
    struct TouchFingerEvent
    {
        public int type;
        public int timestamp;
        public long touchId; // SDL_TouchID
        public long fingerId; // SDL_GestureID
        public float x;
        public float y;
        public float dx;
        public float dy;
        public float pressure;
    }
    #endregion

    #region MULTIGESTURE EVENT
    [StructLayout(LayoutKind.Sequential)]
    struct MultiGestureEvent
    {
        public int type;
        public int timestamp;
        public long touchId; // SDL_TouchID
        public float dTheta;
        public float dDist;
        public float x;
        public float y;
        public short numFingers;
        public short padding;
    }
    #endregion

    #region DOLLAR GESTURE EVENT
    [StructLayout(LayoutKind.Sequential)]
    struct DollarGestureEvent
    {
        public uint type;
        public uint timestamp;
        public long touchId; // SDL_TouchID
        public long gestureId; // SDL_GestureID
        public int numFingers;
        public float error;
        public float x;
        public float y;
    }
    #endregion

    #region CONTROLLER-AXIS-EVENT
    struct ControllerAxisEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which;
        public GameControllerAxis Axis;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public short Value;
        private ushort padding4;
    }
    #endregion

    #region CONTROLLER BUTTON EVENT
    struct ControllerButtonEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which;
        public GameControllerButton Button;
        public InputState State;
        private byte padding1;
        private byte padding2;
    }
    #endregion

    #region CONTROL DEVICE EVENT
    struct ControllerDeviceEvent
    {
        internal EventType Type;
        public uint Timestamp;

        /// <summary>
        /// The joystick device index for the ADDED event, instance id for the REMOVED or REMAPPED event
        /// </summary>
        public int Which;
    }
    #endregion

    #region JOYAXIS EVENT
    struct JoyAxisEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which; // SDL_JoystickID
        public byte Axis;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public short Value;
        private short padding4;
    }
    #endregion

    #region JOYBALL EVENT
    struct JoyBallEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which;
        public byte Ball;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public short Xrel;
        public short Yrel;
    }
    #endregion

    #region JOY BUTTON EVENT
    struct JoyButtonEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which;
        public byte Button;
        public InputState State;
        private byte padding1;
        private byte padding2;
    }
    #endregion

    #region JOY DEVICE EVENT
    struct JoyDeviceEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which;
    }
    #endregion

    #region JOY HAT EVENT
    struct JoyHatEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which;
        public byte Hat;
        public HatPosition Value;
        private byte padding1;
        private byte padding2;
    }
    #endregion


    #region GAME-PAD-CAPABILITIES
    /// <summary>
    /// Describes the capabilities of a <c>GamePad</c> input device.
    /// </summary>
    struct GamePadCapabilities : IEquatable<GamePadCapabilities>, IGamePadCapabilities
    {
        private PadButtons buttons;
        private GamePadAxes axes;
        private byte gamepad_type;

        internal GamePadCapabilities(GamePadType type, GamePadAxes axes, PadButtons buttons, bool is_connected, bool is_mapped)
            : this()
        {
            gamepad_type = (byte)type;
            this.axes = axes;
            this.buttons = buttons;
            this.IsConnected = is_connected;
            this.IsMapped = is_mapped;
        }

        public int Buttons => (int)buttons;

        /// <summary>
        /// Gets a <see cref="GamePadType"/>  value describing the type of a <see cref="GamePad"/> input device.
        /// This value depends on the connected device and the drivers  use. If <c>IsConnected</c>
        /// is false, then this value will be <c>GamePadType.Unknown</c>.
        /// </summary>
        /// <value>The <c>GamePadType</c> of the connected input device.</value>
        public GamePadType GamePadType
        {
            get { return (GamePadType)gamepad_type; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// an up digital pad button.
        /// </summary>
        /// <value><c>true</c> if this instance has an up digital pad button; otherwise, <c>false</c>.</value>
        public bool HasDPadUpButton
        {
            get { return (buttons & PadButtons.DPadUp) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a down digital pad button.
        /// </summary>
        /// <value><c>true</c> if this instance has a down digital pad button; otherwise, <c>false</c>.</value>
        public bool HasDPadDownButton
        {
            get { return (buttons & PadButtons.DPadDown) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a left digital pad button.
        /// </summary>
        /// <value><c>true</c> if this instance has a left digital pad button; otherwise, <c>false</c>.</value>
        public bool HasDPadLeftButton
        {
            get { return (buttons & PadButtons.DPadLeft) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a right digital pad button.
        /// </summary>
        /// <value><c>true</c> if this instance has a right digital pad button; otherwise, <c>false</c>.</value>
        public bool HasDPadRightButton
        {
            get { return (buttons & PadButtons.DPadRight) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// an A button.
        /// </summary>
        /// <value><c>true</c> if this instance has an A button; otherwise, <c>false</c>.</value>
        public bool HasAButton
        {
            get { return (buttons & PadButtons.A) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a B button.
        /// </summary>
        /// <value><c>true</c> if this instance has a B button; otherwise, <c>false</c>.</value>
        public bool HasBButton
        {
            get { return (buttons & PadButtons.B) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a X button.
        /// </summary>
        /// <value><c>true</c> if this instance has a X button; otherwise, <c>false</c>.</value>
        public bool HasXButton
        {
            get { return (buttons & PadButtons.X) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a Y button.
        /// </summary>
        /// <value><c>true</c> if this instance has a Y button; otherwise, <c>false</c>.</value>
        public bool HasYButton
        {
            get { return (buttons & PadButtons.Y) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a left stick button.
        /// </summary>
        /// <value><c>true</c> if this instance has a left stick button; otherwise, <c>false</c>.</value>
        public bool HasLeftStickButton
        {
            get { return (buttons & PadButtons.LeftStick) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a right stick button.
        /// </summary>
        /// <value><c>true</c> if this instance has a right stick button; otherwise, <c>false</c>.</value>
        public bool HasRightStickButton
        {
            get { return (buttons & PadButtons.RightStick) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a left shoulder button.
        /// </summary>
        /// <value><c>true</c> if this instance has a left shoulder button; otherwise, <c>false</c>.</value>
        public bool HasLeftShoulderButton
        {
            get { return (buttons & PadButtons.LeftShoulder) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a right shoulder button.
        /// </summary>
        /// <value><c>true</c> if this instance has a right shoulder button; otherwise, <c>false</c>.</value>
        public bool HasRightShoulderButton
        {
            get { return (buttons & PadButtons.RightShoulder) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a back button.
        /// </summary>
        /// <value><c>true</c> if this instance has a back button; otherwise, <c>false</c>.</value>
        public bool HasBackButton
        {
            get { return (buttons & PadButtons.Back) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a big button. (also known as "guide" or "home" button).
        /// </summary>
        /// <value><c>true</c> if this instance has a big button; otherwise, <c>false</c>.</value>
        public bool HasBigButton
        {
            get { return (buttons & PadButtons.BigButton) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a start button.
        /// </summary>
        /// <value><c>true</c> if this instance has a start button; otherwise, <c>false</c>.</value>
        public bool HasStartButton
        {
            get { return (buttons & PadButtons.Start) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a left thumbstick with a x-axis.
        /// </summary>
        /// <value><c>true</c> if this instance has a left thumbstick with a x-axis; otherwise, <c>false</c>.</value>
        public bool HasLeftXThumbStick
        {
            get { return (axes & GamePadAxes.LeftX) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a left thumbstick with a y-axis.
        /// </summary>
        /// <value><c>true</c> if this instance has a left thumbstick with a y-axis; otherwise, <c>false</c>.</value>
        public bool HasLeftYThumbStick
        {
            get { return (axes & GamePadAxes.LeftY) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a right thumbstick with a x-axis.
        /// </summary>
        /// <value><c>true</c> if this instance has a right thumbstick with a x-axis; otherwise, <c>false</c>.</value>
        public bool HasRightXThumbStick
        {
            get { return (axes & GamePadAxes.RightX) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a right thumbstick with a y-axis.
        /// </summary>
        /// <value><c>true</c> if this instance has a right thumbstick with a y-axis; otherwise, <c>false</c>.</value>
        public bool HasRightYThumbStick
        {
            get { return (axes & GamePadAxes.RightY) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a left trigger.
        /// </summary>
        /// <value><c>true</c> if this instance has a left trigger; otherwise, <c>false</c>.</value>
        public bool HasLeftTrigger
        {
            get { return (axes & GamePadAxes.LeftTrigger) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a right trigger.
        /// </summary>
        /// <value><c>true</c> if this instance has a right trigger; otherwise, <c>false</c>.</value>
        public bool HasRightTrigger
        {
            get { return (axes & GamePadAxes.RightTrigger) != 0; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a low-frequency vibration motor.
        /// </summary>
        /// <value><c>true</c> if this instance has a low-frequency vibration motor; otherwise, <c>false</c>.</value>
        public bool HasLeftVibrationMotor
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a high-frequency vibration motor.
        /// </summary>
        /// <value><c>true</c> if this instance has a high frequency vibration motor; otherwise, <c>false</c>.</value>
        public bool HasRightVibrationMotor
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> has
        /// a microphone input.
        /// </summary>
        /// <value><c>true</c> if this instance has a microphone input; otherwise, <c>false</c>.</value>
        public bool HasVoiceSupport
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether this <c>GamePad</c> is
        /// currently connected.
        /// </summary>
        /// <value><c>true</c> if this instance is currently connected; otherwise, <c>false</c>.</value>
        public bool IsConnected { get; }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value describing whether a valid button configuration
        /// exists for this <c>GamePad</c>  the GamePad configuration database.
        /// </summary>
        public bool IsMapped { get; }

        /// <param name="left">A <see cref="GamePadCapabilities"/> structure to test for equality.</param>
        /// <param name="right">A <see cref="GamePadCapabilities"/> structure to test for equality.</param>
        public static bool operator ==(GamePadCapabilities left, GamePadCapabilities right)
        {
            return left.Equals(right);
        }

        /// <param name="left">A <see cref="GamePadCapabilities"/> structure to test for inequality.</param>
        /// <param name="right">A <see cref="GamePadCapabilities"/> structure to test for inequality.</param>
        public static bool operator !=(GamePadCapabilities left, GamePadCapabilities right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadCapabilities"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadCapabilities"/>.</returns>
        public override string ToString()
        {
            return String.Format(
                "{{Type: {0}; Axes: {1}; Buttons: {2}; {3}; {4}}}",
                GamePadType,
                Convert.ToString((int)axes, 2),
                Convert.ToString((int)buttons, 2),
                IsMapped ? "Mapped" : "Unmapped",
                IsConnected ? "Connected" : "Disconnected");
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="MnM.Sdl.GamePadCapabilities"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return
                buttons.GetHashCode() ^
                IsConnected.GetHashCode() ^
                IsMapped.GetHashCode() ^
                gamepad_type.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.Sdl.GamePadCapabilities"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.Sdl.GamePadCapabilities"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadCapabilities"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is GamePadCapabilities &&
                Equals((GamePadCapabilities)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MnM.Sdl.GamePadCapabilities"/> is equal to the current <see cref="MnM.Sdl.GamePadCapabilities"/>.
        /// </summary>
        /// <param name="other">The <see cref="MnM.Sdl.GamePadCapabilities"/> to compare with the current <see cref="MnM.Sdl.GamePadCapabilities"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MnM.Sdl.GamePadCapabilities"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadCapabilities"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(GamePadCapabilities other)
        {
            return
                buttons == other.buttons &&
                IsConnected == other.IsConnected &&
                IsMapped == other.IsMapped &&
                gamepad_type == other.gamepad_type;
        }

        public bool Equals(IGamePadCapabilities other)
        {
            return
                (int)buttons == other.Buttons &&
                IsConnected == other.IsConnected &&
                IsMapped == other.IsMapped &&
                gamepad_type == (byte)other.GamePadType;
        }
    }
    #endregion

    #region GAME-PAD-STATE
    /// <summary>
    /// Describes the current state of a <see cref="GamePad"/> device.
    /// </summary>
    struct GamePadState : IEquatable<GamePadState>, IGamePadState
    {
        private const float RangeMultiplier = 1.0f / (short.MaxValue + 1);

        private PadButtons buttons;
        private short left_stick_x;
        private short left_stick_y;
        private short right_stick_x;
        private short right_stick_y;
        private byte left_trigger;
        private byte right_trigger;

        /// <summary>
        /// Gets a <see cref="GamePadThumbSticks"/> structure describing the
        /// state of the <c>GamePad</c> thumb sticks.
        /// </summary>
        public GamePadThumbSticks ThumbSticks
        {
            get { return new GamePadThumbSticks(left_stick_x, left_stick_y, right_stick_x, right_stick_y); }
        }

        /// <summary>
        /// Gets a <see cref="GamePadButtons"/> structure describing the
        /// state of the <c>GamePad</c> buttons.
        /// </summary>
        public GamePadButtons Buttons
        {
            get { return new GamePadButtons(buttons); }
        }

        /// <summary>
        /// Gets a <see cref="GamePadDPad"/> structure describing the
        /// state of the <c>GamePad</c> directional pad.
        /// </summary>
        public GamePadDPad DPad
        {
            get { return new GamePadDPad(buttons); }
        }

        /// <summary>
        /// Gets a <see cref="GamePadTriggers"/> structure describing the
        /// state of the <c>GamePad</c> triggers.
        /// </summary>
        public GamePadTriggers Triggers
        {
            get { return new GamePadTriggers(left_trigger, right_trigger); }
        }

        /// <summary>
        /// Gets a value indicating whether this <c>GamePad</c> instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Gets the packet number for this <c>GamePadState</c> instance.
        /// Use the packet number to determine whether the state of a
        /// <c>GamePad</c> device has changed.
        /// </summary>
        public int PacketNumber { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadState"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadState"/>.</returns>
        public override string ToString()
        {
            return String.Format(
                "{{Sticks: {0}; Triggers: {1}; Buttons: {2}; DPad: {3}; IsConnected: {4}}}",
                ThumbSticks, Triggers, Buttons, DPad, IsConnected);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="MnM.Sdl.GamePadState"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return
                ThumbSticks.GetHashCode() ^ Buttons.GetHashCode() ^
                DPad.GetHashCode() ^ IsConnected.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.Sdl.GamePadState"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.Sdl.GamePadState"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadState"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is GamePadState &&
                Equals((GamePadState)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MnM.Sdl.GamePadState"/> is equal to the current <see cref="MnM.Sdl.GamePadState"/>.
        /// </summary>
        /// <param name="other">The <see cref="MnM.Sdl.GamePadState"/> to compare with the current <see cref="MnM.Sdl.GamePadState"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MnM.Sdl.GamePadState"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadState"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(GamePadState other)
        {
            return
                ThumbSticks == other.ThumbSticks &&
                Buttons == other.Buttons &&
                DPad == other.DPad &&
                IsConnected == other.IsConnected;
        }

        internal void SetAxis(GamePadAxes axis, short value)
        {
            if ((axis & GamePadAxes.LeftX) != 0)
            {
                left_stick_x = value;
            }

            if ((axis & GamePadAxes.LeftY) != 0)
            {
                left_stick_y = (short)~value;
            }

            if ((axis & GamePadAxes.RightX) != 0)
            {
                right_stick_x = value;
            }

            if ((axis & GamePadAxes.RightY) != 0)
            {
                right_stick_y = (short)~value;
            }

            if ((axis & GamePadAxes.LeftTrigger) != 0)
            {
                // Adjust from [-32768, 32767] to [0, 255]
                left_trigger = (byte)((value - short.MinValue) >> 8);
            }

            if ((axis & GamePadAxes.RightTrigger) != 0)
            {
                // Adjust from [-32768, 32767] to [0, 255]
                right_trigger = (byte)((value - short.MinValue) >> 8);
            }
        }

        internal void SetButton(PadButtons button, bool pressed)
        {
            if (pressed)
            {
                buttons |= button;
            }
            else
            {
                buttons &= ~button;
            }
        }

        internal void SetConnected(bool connected)
        {
            IsConnected = connected;
        }

        internal void SetTriggers(byte left, byte right)
        {
            left_trigger = left;
            right_trigger = right;
        }

        internal void SetPacketNumber(int number)
        {
            PacketNumber = number;
        }

        private bool IsAxisValid(GamePadAxes axis)
        {
            int index = (int)axis;
            return index >= 0 && index < SdlAPI.MaxAxisCount;
        }

        private bool IsDPadValid(int index)
        {
            return index >= 0 && index < SdlAPI.MaxDPadCount;
        }

        IGamePadThumbSticks IGamePadState.ThumbSticks => ThumbSticks;
        IGamePadButtons IGamePadState.Buttons => Buttons;
        IGamePadDPad IGamePadState.DPad => DPad;
        IGamePadTriggers IGamePadState.Triggers => Triggers;
        bool IEquatable<IGamePadState>.Equals(IGamePadState other)
        {
            return
                ThumbSticks.Equals(other.ThumbSticks) &&
                Buttons.Equals(other.Buttons) &&
                DPad.Equals(other.DPad) &&
                IsConnected == other.IsConnected;
        }
    }
    #endregion

    #region GAME-CONTROLLER BUTTON BIND
    [StructLayout(LayoutKind.Explicit)]
    struct GameControllerButtonBind
    {
        [FieldOffset(0)]
        public GameControllerBindType BindType;
        [FieldOffset(4)]
        public _MouseButton Button;
        [FieldOffset(4)]
        public GameControllerAxis Axis;
        [FieldOffset(4)]
        public int Hat;
        [FieldOffset(8)]
        public int HatMask;
    }
    #endregion

    #region GAMEPAD-PAD
    /// <summary>
    /// Describes the state of a <see cref="GamePad"/> directional pad.
    /// </summary>
    struct GamePadDPad : IEquatable<GamePadDPad>, IGamePadDPad
    {
        [Flags]
        private enum DPadButtons : byte
        {
            Up = PadButtons.DPadUp,
            Down = PadButtons.DPadDown,
            Left = PadButtons.DPadLeft,
            Right = PadButtons.DPadRight
        }

        private DPadButtons buttons;

        internal GamePadDPad(PadButtons state)
        {
            // DPad butons are stored  the lower 4bits
            // of the Buttons enumeration.
            buttons = (DPadButtons)((int)state & 0x0f);
        }

        public int Buttons => (int)buttons;

        /// <summary>
        /// Gets the <see cref="InputState"/> for the up button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the up button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        public InputState Up
        {
            get { return IsUp ? InputState.Pressed : InputState.Released; }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the down button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the down button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        public InputState Down
        {
            get { return IsDown ? InputState.Pressed : InputState.Released; }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the left button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the left button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        public InputState Left
        {
            get { return IsLeft ? InputState.Pressed : InputState.Released; }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the right button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the right button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        public InputState Right
        {
            get { return IsRight ? InputState.Pressed : InputState.Released; }
        }

        /// <summary>
        /// Gets a value indicating whether the up button is pressed.
        /// </summary>
        /// <value><c>true</c> if the up button is pressed; otherwise, <c>false</c>.</value>
        public bool IsUp
        {
            get { return (buttons & DPadButtons.Up) != 0; }
            internal set { SetButton(DPadButtons.Up, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the down button is pressed.
        /// </summary>
        /// <value><c>true</c> if the down button is pressed; otherwise, <c>false</c>.</value>
        public bool IsDown
        {
            get { return (buttons & DPadButtons.Down) != 0; }
            internal set { SetButton(DPadButtons.Down, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the left button is pressed.
        /// </summary>
        /// <value><c>true</c> if the left button is pressed; otherwise, <c>false</c>.</value>
        public bool IsLeft
        {
            get { return (buttons & DPadButtons.Left) != 0; }
            internal set { SetButton(DPadButtons.Left, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the right button is pressed.
        /// </summary>
        /// <value><c>true</c> if the right button is pressed; otherwise, <c>false</c>.</value>
        public bool IsRight
        {
            get { return (buttons & DPadButtons.Right) != 0; }
            internal set { SetButton(DPadButtons.Right, value); }
        }

        /// <param name="left">A <see cref="GamePadDPad"/> instance to test for equality.</param>
        /// <param name="right">A <see cref="GamePadDPad"/> instance to test for equality.</param>
        public static bool operator ==(GamePadDPad left, GamePadDPad right)
        {
            return left.Equals(right);
        }

        /// <param name="left">A <see cref="GamePadDPad"/> instance to test for inequality.</param>
        /// <param name="right">A <see cref="GamePadDPad"/> instance to test for inequality.</param>
        public static bool operator !=(GamePadDPad left, GamePadDPad right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadDPad"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadDPad"/>.</returns>
        public override string ToString()
        {
            return String.Format(
                "{{{0}{1}{2}{3}}}",
                IsUp ? "U" : String.Empty,
                IsLeft ? "L" : String.Empty,
                IsDown ? "D" : String.Empty,
                IsRight ? "R" : String.Empty);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="MnM.Sdl.GamePadDPad"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return buttons.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.Sdl.GamePadDPad"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.Sdl.GamePadDPad"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadDPad"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is GamePadDPad &&
                Equals((GamePadDPad)obj);
        }

        private void SetButton(DPadButtons button, bool value)
        {
            if (value)
            {
                buttons |= button;
            }
            else
            {
                buttons &= ~button;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="MnM.Sdl.GamePadDPad"/> is equal to the current <see cref="MnM.Sdl.GamePadDPad"/>.
        /// </summary>
        /// <param name="other">The <see cref="MnM.Sdl.GamePadDPad"/> to compare with the current <see cref="MnM.Sdl.GamePadDPad"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MnM.Sdl.GamePadDPad"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadDPad"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(GamePadDPad other)
        {
            return buttons == other.buttons;
        }
        public bool Equals(IGamePadDPad other)
        {
            return (int)buttons == other.Buttons;
        }
    }
    #endregion

    #region GAME THUMB STICKS
    /// <summary>
    /// Describes the current thumb stick state of a <see cref="GamePad"/> device
    /// </summary>
    struct GamePadThumbSticks : IEquatable<GamePadThumbSticks>, IGamePadThumbSticks
    {
        private const float ConversionFactor = 1.0f / short.MaxValue;
        private short left_x, left_y;
        private short right_x, right_y;

        internal GamePadThumbSticks(
            short left_x, short left_y,
            short right_x, short right_y)
        {
            this.left_x = left_x;
            this.left_y = left_y;
            this.right_x = right_x;
            this.right_y = right_y;
        }

        /// <summary>
        /// Gets a <see cref="PixelF"/> describing the state of the left thumb stick.
        /// </summary>
        public VectorF Left
        {
            get { return new VectorF(left_x * ConversionFactor, left_y * ConversionFactor); }
        }

        /// <summary>
        /// Gets a <see cref="PixelF"/> describing the state of the right thumb stick.
        /// </summary>
        public VectorF Right
        {
            get { return new VectorF(right_x * ConversionFactor, right_y * ConversionFactor); }
        }

        /// <param name="left">A <see cref="GamePadThumbSticks"/> instance to test for equality.</param>
        /// <param name="right">A <see cref="GamePadThumbSticks"/> instance to test for equality.</param>
        public static bool operator ==(GamePadThumbSticks left, GamePadThumbSticks right)
        {
            return left.Equals(right);
        }

        /// <param name="left">A <see cref="GamePadThumbSticks"/> instance to test for inequality.</param>
        /// <param name="right">A <see cref="GamePadThumbSticks"/> instance to test for inequality.</param>
        public static bool operator !=(GamePadThumbSticks left, GamePadThumbSticks right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadThumbSticks"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadThumbSticks"/>.</returns>
        public override string ToString()
        {
            return String.Format(
                "{{Left: ({0:f4}; {1:f4}); Right: ({2:f4}; {3:f4})}}",
                Left.X, Left.Y, Right.X, Right.Y);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="MnM.Sdl.GamePadThumbSticks"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return
                left_x.GetHashCode() ^ left_y.GetHashCode() ^
                right_x.GetHashCode() ^ right_y.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.Sdl.GamePadThumbSticks"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.Sdl.GamePadThumbSticks"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadThumbSticks"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is GamePadThumbSticks &&
                Equals((GamePadThumbSticks)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MnM.Sdl.GamePadThumbSticks"/> is equal to the current <see cref="MnM.Sdl.GamePadThumbSticks"/>.
        /// </summary>
        /// <param name="other">The <see cref="MnM.Sdl.GamePadThumbSticks"/> to compare with the current <see cref="MnM.Sdl.GamePadThumbSticks"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MnM.Sdl.GamePadThumbSticks"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadThumbSticks"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(GamePadThumbSticks other)
        {
            return
                left_x == other.left_x &&
                left_y == other.left_y &&
                right_x == other.right_x &&
                right_y == other.right_y;
        }
        public bool Equals(IGamePadThumbSticks other)
        {
            return
                left_x == other.Left.X &&
                left_y == other.Left.Y &&
                right_x == other.Right.X &&
                right_y == other.Right.Y;
        }

        VectorF IGamePadThumbSticks.Left => Left;
        VectorF IGamePadThumbSticks.Right => Right;
    }
    #endregion

    #region GAME PAD TRIGGERS
    /// <summary>
    /// Describes the state of a <see cref="GamePad"/> trigger buttons.
    /// </summary>
    struct GamePadTriggers : IEquatable<GamePadTriggers>, IGamePadTriggers
    {
        private const float ConversionFactor = 1.0f / byte.MaxValue;
        private byte left;
        private byte right;

        internal GamePadTriggers(byte left, byte right)
        {
            this.left = left;
            this.right = right;
        }

        /// <summary>
        /// Gets the offset of the left trigger button, between 0.0 and 1.0.
        /// </summary>
        public float Left
        {
            get { return left * ConversionFactor; }
        }

        /// <summary>
        /// Gets the offset of the left trigger button, between 0.0 and 1.0.
        /// </summary>
        public float Right
        {
            get { return right * ConversionFactor; }
        }

        /// <param name="left">A <see cref="GamePadTriggers"/> instance to test for equality.</param>
        /// <param name="right">A <see cref="GamePadTriggers"/> instance to test for equality.</param>
        public static bool operator ==(GamePadTriggers left, GamePadTriggers right)
        {
            return left.Equals(right);
        }

        /// <param name="left">A <see cref="GamePadTriggers"/> instance to test for equality.</param>
        /// <param name="right">A <see cref="GamePadTriggers"/> instance to test for equality.</param>
        public static bool operator !=(GamePadTriggers left, GamePadTriggers right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadTriggers"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadTriggers"/>.</returns>
        public override string ToString()
        {
            return String.Format(
                "({0:f2}; {1:f2})",
                Left, Right);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="MnM.Sdl.GamePadTriggers"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return
                left.GetHashCode() ^ right.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.Sdl.GamePadTriggers"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.Sdl.GamePadTriggers"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadTriggers"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is GamePadTriggers &&
                Equals((GamePadTriggers)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MnM.Sdl.GamePadTriggers"/> is equal to the current <see cref="MnM.Sdl.GamePadTriggers"/>.
        /// </summary>
        /// <param name="other">The <see cref="MnM.Sdl.GamePadTriggers"/> to compare with the current <see cref="MnM.Sdl.GamePadTriggers"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MnM.Sdl.GamePadTriggers"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadTriggers"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(GamePadTriggers other)
        {
            return
                left == other.left &&
                right == other.right;
        }
    }
    #endregion

    #region GAME PAD BUTTONS
    /// <summary>
    /// Describes the <see cref="InputState"/> of <see cref="GamePad"/> <see cref="PadButtons"/>.
    /// </summary>
    struct GamePadButtons : IEquatable<GamePadButtons>, IGamePadButtons
    {
        private PadButtons buttons;

        /// <summary>
        /// Initializes a new instance of the <see cref="MnM.Sdl.GamePadButtons"/> structure.
        /// </summary>
        /// <param name="state">A bitmask containing the button state.</param>
        public GamePadButtons(PadButtons state)
        {
            buttons = state;
        }

        public int Buttons => (int)buttons;

        /// <summary>
        /// Gets the <see cref="InputState"/> for the A button.
        /// </summary>
        public InputState A
        {
            get { return GetButton(PadButtons.A); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the B button.
        /// </summary>
        public InputState B
        {
            get { return GetButton(PadButtons.B); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the X button.
        /// </summary>
        public InputState X
        {
            get { return GetButton(PadButtons.X); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the Y button.
        /// </summary>
        public InputState Y
        {
            get { return GetButton(PadButtons.Y); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the Back button.
        /// </summary>
        public InputState Back
        {
            get { return GetButton(PadButtons.Back); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the big button.
        /// This button is also known as Home or Guide.
        /// </summary>
        public InputState BigButton
        {
            get { return GetButton(PadButtons.BigButton); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the left shoulder button.
        /// </summary>
        public InputState LeftShoulder
        {
            get { return GetButton(PadButtons.LeftShoulder); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the left stick button.
        /// This button represents a left stick that is pressed in.
        /// </summary>
        public InputState LeftStick
        {
            get { return GetButton(PadButtons.LeftStick); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the right shoulder button.
        /// </summary>
        public InputState RightShoulder
        {
            get { return GetButton(PadButtons.RightShoulder); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the right stick button.
        /// This button represents a right stick that is pressed in.
        /// </summary>
        public InputState RightStick
        {
            get { return GetButton(PadButtons.RightStick); }
        }

        /// <summary>
        /// Gets the <see cref="InputState"/> for the starth button.
        /// </summary>
        public InputState Start
        {
            get { return GetButton(PadButtons.Start); }
        }

        /// <summary>
        /// Gets a value indicating whether any button is pressed.
        /// </summary>
        /// <value><c>true</c> if any button is pressed; otherwise, <c>false</c>.</value>
        public bool IsAnyButtonPressed
        {
            get
            {
                // If any bit is set then a button is down.
                return buttons != 0;
            }
        }

        /// <param name="left">A <see cref="GamePadButtons"/> instance to test for equality.</param>
        /// <param name="right">A <see cref="GamePadButtons"/> instance to test for equality.</param>
        public static bool operator ==(GamePadButtons left, GamePadButtons right)
        {
            return left.Equals(right);
        }

        /// <param name="left">A <see cref="GamePadButtons"/> instance to test for inequality.</param>
        /// <param name="right">A <see cref="GamePadButtons"/> instance to test for inequality.</param>
        public static bool operator !=(GamePadButtons left, GamePadButtons right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadButtons"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.GamePadButtons"/>.</returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (A == InputState.Pressed)
            {
                sb.Append("A");
            }
            if (B == InputState.Pressed)
            {
                sb.Append("B");
            }
            if (X == InputState.Pressed)
            {
                sb.Append("X");
            }
            if (Y == InputState.Pressed)
            {
                sb.Append("Y");
            }
            if (Back == InputState.Pressed)
            {
                sb.Append("Bk");
            }
            if (Start == InputState.Pressed)
            {
                sb.Append("St");
            }
            if (BigButton == InputState.Pressed)
            {
                sb.Append("Gd");
            }
            if (Back == InputState.Pressed)
            {
                sb.Append("Bk");
            }
            if (LeftShoulder == InputState.Pressed)
            {
                sb.Append("L");
            }
            if (RightShoulder == InputState.Pressed)
            {
                sb.Append("R");
            }
            if (LeftStick == InputState.Pressed)
            {
                sb.Append("Ls");
            }
            if (RightStick == InputState.Pressed)
            {
                sb.Append("Rs");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="MnM.Sdl.GamePadButtons"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return buttons.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.Sdl.GamePadButtons"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.Sdl.GamePadButtons"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadButtons"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is GamePadButtons &&
                Equals((GamePadButtons)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MnM.Sdl.GamePadButtons"/> is equal to the current <see cref="MnM.Sdl.GamePadButtons"/>.
        /// </summary>
        /// <param name="other">The <see cref="MnM.Sdl.GamePadButtons"/> to compare with the current <see cref="MnM.Sdl.GamePadButtons"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MnM.Sdl.GamePadButtons"/> is equal to the current
        /// <see cref="MnM.Sdl.GamePadButtons"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(GamePadButtons other)
        {
            return buttons == other.buttons;
        }
        public bool Equals(IGamePadButtons other)
        {
            return (int)buttons == other.Buttons;
        }

        private InputState GetButton(PadButtons b)
        {
            return (buttons & b) != 0 ? InputState.Pressed : InputState.Released;
        }
    }
    #endregion

    #region GAME PAD CONFIG SOURCE
    struct GamePadConfigurationSource
    {
        private int? map_button;
        private int? map_axis;
        private JoystickHat? map_hat;
        private HatLocation? map_hat_position;

        /// <summary>
        /// Creates a new gamepad configuration source from an axis or a button
        /// </summary>
        /// <param name="isAxis">Whether this source is an axis or a button</param>
        /// <param name="index">The index of this source</param>
        public GamePadConfigurationSource(bool isAxis, int index)
            : this()
        {
            if (isAxis)
            {
                Type = ConfigurationType.Axis;
                Axis = index;
            }
            else
            {
                Type = ConfigurationType.Button;
                Button = index;
            }
        }

        /// <summary>
        /// Creates a new gamepad configuration source from a hat
        /// </summary>
        /// <param name="hat">The hat</param>
        /// <param name="pos">The starting hat position</param>
        public GamePadConfigurationSource(JoystickHat hat, HatLocation pos)
            : this()
        {
            Type = ConfigurationType.Hat;
            Hat = hat;
            map_hat_position = pos;
        }

        public ConfigurationType Type { get; private set; }

        /// <summary>
        /// Represents a gamepad axis
        /// </summary>
        public int Axis
        {
            get { return map_axis.Value; }
            private set { map_axis = value; }
        }

        /// <summary>
        /// Represents a gamepad button
        /// </summary>
        public int Button
        {
            get { return map_button.Value; }
            private set { map_button = value; }
        }

        /// <summary>
        /// Represents a gamepad hat
        /// </summary>
        public JoystickHat Hat
        {
            get { return map_hat.Value; }
            private set { map_hat = value; }
        }

        /// <summary>
        /// Represents the position of a gamepad hat
        /// </summary>
        public HatLocation HatPosition
        {
            get { return map_hat_position.Value; }
            private set { map_hat_position = value; }
        }
    }
    #endregion

    #region GAMEPAD CONFIG TARGET
    struct GamePadConfigurationTarget
    {
        private Nullable<PadButtons> map_button;
        private Nullable<GamePadAxes> map_axis;

        public GamePadConfigurationTarget(PadButtons button)
            : this()
        {
            Type = ConfigurationType.Button;
            map_button = button;
        }

        public GamePadConfigurationTarget(GamePadAxes axis)
            : this()
        {
            Type = ConfigurationType.Axis;
            map_axis = axis;
        }

        public ConfigurationType Type { get; private set; }

        public GamePadAxes Axis
        {
            get { return map_axis.Value; }
            private set { map_axis = value; }
        }

        public PadButtons Button
        {
            get { return map_button.Value; }
            private set { map_button = value; }
        }
    }
    #endregion

    #region JOYSTICK CAPABILITIES
    /// <summary>
    /// Describes the <c>JoystickCapabilities</c> of a <see cref="JoystickDevice"/>.
    /// </summary>
    struct JoystickCapabilities : IEquatable<JoystickCapabilities>
    {
        private byte axis_count;
        private byte button_count;
        private byte hat_count;

        internal JoystickCapabilities(int axis_count, int button_count, int hat_count, bool is_connected)
        {
            if (axis_count < 0 || axis_count > JoystickState.MaxAxes)
            {
            }
            if (button_count < 0 || button_count > JoystickState.MaxButtons)
            {
            }
            if (hat_count < 0 || hat_count > JoystickState.MaxHats)
            {
            }

            axis_count = clamp(axis_count, 0, JoystickState.MaxAxes);
            button_count = clamp(button_count, 0, JoystickState.MaxButtons);
            hat_count = clamp(hat_count, 0, JoystickState.MaxHats);

            this.axis_count = (byte)axis_count;
            this.button_count = (byte)button_count;
            this.hat_count = (byte)hat_count;
            this.IsConnected = is_connected;
        }

        static int clamp(int n, int min, int max)
        {
            return Math.Max(Math.Min(n, max), min);
        }

        internal void SetIsConnected(bool value)
        {
            IsConnected = value;
        }

        /// <summary>
        /// Gets the number of axes supported by this <see cref="JoystickDevice"/>.
        /// </summary>
        public int AxisCount
        {
            get { return axis_count; }
        }

        /// <summary>
        /// Gets the number of buttons supported by this <see cref="JoystickDevice"/>.
        /// </summary>
        public int ButtonCount
        {
            get { return button_count; }
        }

        /// <summary>
        /// Gets the number of hats supported by this <see cref="JoystickDevice"/>.
        /// </summary>
        public int HatCount
        {
            get { return hat_count; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="JoystickDevice"/> is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.JoystickCapabilities"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.JoystickCapabilities"/>.</returns>
        public override string ToString()
        {
            return String.Format(
                "{{Axes: {0}; Buttons: {1}; Hats: {2}; IsConnected: {3}}}",
                AxisCount, ButtonCount, HatCount, IsConnected);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="MnM.Sdl.JoystickCapabilities"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return
                AxisCount.GetHashCode() ^
                ButtonCount.GetHashCode() ^
                HatCount.GetHashCode() ^
                IsConnected.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.Sdl.JoystickCapabilities"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.Sdl.JoystickCapabilities"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="MnM.Sdl.JoystickCapabilities"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is JoystickCapabilities &&
                Equals((JoystickCapabilities)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MnM.Sdl.JoystickCapabilities"/> is equal to the current <see cref="MnM.Sdl.JoystickCapabilities"/>.
        /// </summary>
        /// <param name="other">The <see cref="MnM.Sdl.JoystickCapabilities"/> to compare with the current <see cref="MnM.Sdl.JoystickCapabilities"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MnM.Sdl.JoystickCapabilities"/> is equal to the current
        /// <see cref="MnM.Sdl.JoystickCapabilities"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(JoystickCapabilities other)
        {
            return
                AxisCount == other.AxisCount &&
                ButtonCount == other.ButtonCount &&
                HatCount == other.HatCount &&
                IsConnected == other.IsConnected;
        }
    }
    #endregion

    #region JOY STICK STATE
    /// <summary>
    /// Describes the current state of a <see cref="JoystickDevice"/>.
    /// </summary>
    struct JoystickState : IEquatable<JoystickState>
    {
        // If we ever add more values to JoystickAxis or JoystickButton
        // then we'll need to increase these limits.
        internal const int MaxAxes = 64;
        internal const int MaxButtons = 64;
        internal const byte MaxHats = (byte)(JoystickHat.Last + 1);

        private const float ConversionFactor = 1.0f / (short.MaxValue + 0.5f);

        private long buttons;
        private unsafe fixed short axes[MaxAxes];
        private JoystickHatState hat0;
        private JoystickHatState hat1;
        private JoystickHatState hat2;
        private JoystickHatState hat3;

        /// <summary>
        /// Gets a value between -1.0 and 1.0 representing the current offset of the specified axis.
        /// </summary>
        /// <returns>
        /// A value between -1.0 and 1.0 representing offset of the specified axis.
        /// If the specified axis does not exist, then the return value is 0.0. Use <see cref="JoystickDriver.GetCapabilities"/>
        /// to query the number of available axes.
        /// </returns>
        /// <param name="axis">The axis to query.</param>
        public float GetAxis(int axis)
        {
            return GetAxisRaw(axis) * ConversionFactor;
        }

        /// <summary>
        /// Gets the current <see cref="InputState"/> of the specified button.
        /// </summary>
        /// <returns><see cref="InputState.Pressed"/> if the specified button is pressed; otherwise, <see cref="InputState.Released"/>.</returns>
        /// <param name="button">The button to query.</param>
        public InputState GetButton(int button)
        {
            return (buttons & ((long)1 << button)) != 0 ? InputState.Pressed : InputState.Released;
        }

        /// <summary>
        /// Gets the hat.
        /// </summary>
        /// <returns>The hat.</returns>
        /// <param name="hat">Hat.</param>
        public JoystickHatState GetHat(JoystickHat hat)
        {
            switch (hat)
            {
                case JoystickHat.Hat0:
                    return hat0;
                case JoystickHat.Hat1:
                    return hat1;
                case JoystickHat.Hat2:
                    return hat2;
                case JoystickHat.Hat3:
                    return hat3;
                default:
                    return new JoystickHatState();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the specified button is currently pressed.
        /// </summary>
        /// <returns>true if the specified button is pressed; otherwise, false.</returns>
        /// <param name="button">The button to query.</param>
        public bool IsButtonDown(int button)
        {
            return (buttons & ((long)1 << button)) != 0;
        }

        /// <summary>
        /// Gets a value indicating whether the specified button is currently released.
        /// </summary>
        /// <returns>true if the specified button is released; otherwise, false.</returns>
        /// <param name="button">The button to query.</param>
        public bool IsButtonUp(int button)
        {
            return (buttons & ((long)1 << button)) == 0;
        }

        /// <summary>
        /// Gets a value indicating whether any button is down.
        /// </summary>
        /// <value><c>true</c> if any button is down; otherwise, <c>false</c>.</value>
        public bool IsAnyButtonDown
        {
            get
            {
                // If any bit is set then a button is down.
                return buttons != 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.JoystickState"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.JoystickState"/>.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < MaxAxes; i++)
            {
                sb.Append(" ");
                sb.Append(String.Format("{0:f4}", GetAxis(i)));
            }
            return String.Format(
                "{{Axes:{0}; Buttons: {1}; Hat: {2}; IsConnected: {3}}}",
                sb.ToString(),
                Convert.ToString(buttons, 2).PadLeft(16, '0'),
                hat0,
                IsConnected);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="MnM.Sdl.JoystickState"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            int hash = buttons.GetHashCode() ^ IsConnected.GetHashCode();
            for (int i = 0; i < MaxAxes; i++)
            {
                hash ^= GetAxisUnsafe(i).GetHashCode();
            }
            return hash;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.Sdl.JoystickState"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.Sdl.JoystickState"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="MnM.Sdl.JoystickState"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is JoystickState &&
                Equals((JoystickState)obj);
        }

        internal int PacketNumber { get; private set; }

        internal short GetAxisRaw(int axis)
        {
            short value = 0;
            if (axis >= 0 && axis < MaxAxes)
            {
                value = GetAxisUnsafe(axis);
            }
            return value;
        }

        internal void SetAxis(int axis, short value)
        {
            int index = axis;
            if (index < 0 || index >= MaxAxes)
            {
                throw new ArgumentOutOfRangeException("axis");
            }

            unsafe
            {
                fixed (short* paxes = axes)
                {
                    *(paxes + index) = value;
                }
            }
        }

        internal void ClearButtons()
        {
            buttons = 0;
        }

        internal void SetButton(int button, bool value)
        {
            if (button < 0 || button >= MaxButtons)
            {
                throw new ArgumentOutOfRangeException("button");
            }

            if (value)
            {
                buttons |= (long)1 << button;
            }
            else
            {
                buttons &= ~((long)1 << button);
            }
        }

        internal void SetHat(JoystickHat hat, JoystickHatState value)
        {
            switch (hat)
            {
                case JoystickHat.Hat0:
                    hat0 = value;
                    break;
                case JoystickHat.Hat1:
                    hat1 = value;
                    break;
                case JoystickHat.Hat2:
                    hat2 = value;
                    break;
                case JoystickHat.Hat3:
                    hat3 = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("hat");
            }
        }

        internal void SetIsConnected(bool value)
        {
            IsConnected = value;
        }

        internal void SetPacketNumber(int number)
        {
            PacketNumber = number;
        }

        private short GetAxisUnsafe(int index)
        {
            unsafe
            {
                fixed (short* paxis = axes)
                {
                    return *(paxis + index);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="MnM.Sdl.JoystickState"/> is equal to the current <see cref="MnM.Sdl.JoystickState"/>.
        /// </summary>
        /// <param name="other">The <see cref="MnM.Sdl.JoystickState"/> to compare with the current <see cref="MnM.Sdl.JoystickState"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MnM.Sdl.JoystickState"/> is equal to the current
        /// <see cref="MnM.Sdl.JoystickState"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(JoystickState other)
        {
            bool equals =
                buttons == other.buttons &&
                IsConnected == other.IsConnected;
            for (int i = 0; equals && i < MaxAxes; i++)
            {
                equals &= GetAxisUnsafe(i) == other.GetAxisUnsafe(i);
            }
            for (int i = 0; equals && i < MaxHats; i++)
            {
                JoystickHat hat = (JoystickHat)((byte)JoystickHat.Hat0 + i);
                equals &= GetHat(hat).Equals(other.GetHat(hat));
            }
            return equals;
        }
    }
    #endregion

    #region JOYSTICK HAT STATE
    /// <summary>
    /// Describes the state of a joystick hat.
    /// </summary>
    struct JoystickHatState : IEquatable<JoystickHatState>
    {
        internal JoystickHatState(HatLocation pos)
        {
            Position = pos;
        }

        /// <summary>
        /// Gets a <see cref="HatLocation"/> value indicating
        /// the position of this hat.
        /// </summary>
        /// <value>The position.</value>
        public HatLocation Position { get; }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating
        /// whether this hat lies  the top hemicircle.
        /// </summary>
        /// <value><c>true</c> if this hat lies  the top hemicircle; otherwise, <c>false</c>.</value>
        public bool IsUp
        {
            get
            {
                return
                    Position == HatLocation.Up ||
                    Position == HatLocation.UpLeft ||
                    Position == HatLocation.UpRight;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating
        /// whether this hat lies  the bottom hemicircle.
        /// </summary>
        /// <value><c>true</c> if this hat lies  the bottom hemicircle; otherwise, <c>false</c>.</value>
        public bool IsDown
        {
            get
            {
                return
                    Position == HatLocation.Down ||
                    Position == HatLocation.DownLeft ||
                    Position == HatLocation.DownRight;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating
        /// whether this hat lies  the left hemicircle.
        /// </summary>
        /// <value><c>true</c> if this hat lies  the left hemicircle; otherwise, <c>false</c>.</value>
        public bool IsLeft
        {
            get
            {
                return
                    Position == HatLocation.Left ||
                    Position == HatLocation.UpLeft ||
                    Position == HatLocation.DownLeft;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating
        /// whether this hat lies  the right hemicircle.
        /// </summary>
        /// <value><c>true</c> if this hat lies  the right hemicircle; otherwise, <c>false</c>.</value>
        public bool IsRight
        {
            get
            {
                return
                    Position == HatLocation.Right ||
                    Position == HatLocation.UpRight ||
                    Position == HatLocation.DownRight;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.JoystickHatState"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.Sdl.JoystickHatState"/>.</returns>
        public override string ToString()
        {
            return String.Format(
                "{{{0}{1}{2}{3}}}",
                IsUp ? "U" : String.Empty,
                IsLeft ? "L" : String.Empty,
                IsDown ? "D" : String.Empty,
                IsRight ? "R" : String.Empty);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="MnM.Sdl.JoystickHatState"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.Sdl.JoystickHatState"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.Sdl.JoystickHatState"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="MnM.Sdl.JoystickHatState"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            return
                obj is JoystickHatState &&
                Equals((JoystickHatState)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="MnM.Sdl.JoystickHatState"/> is equal to the current <see cref="MnM.Sdl.JoystickHatState"/>.
        /// </summary>
        /// <param name="other">The <see cref="MnM.Sdl.JoystickHatState"/> to compare with the current <see cref="MnM.Sdl.JoystickHatState"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="MnM.Sdl.JoystickHatState"/> is equal to the current
        /// <see cref="MnM.Sdl.JoystickHatState"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(JoystickHatState other)
        {
            return Position == other.Position;
        }
    }
    #endregion

    #region JOYSTICK GUID
    struct JoystickGuid
    {
        private long data0;
        private long data1;

        public Guid ToGuid()
        {
            byte[] data = new byte[16];

            unsafe
            {
                fixed (JoystickGuid* pdata = &this)
                {
                    Marshal.Copy(new IntPtr(pdata), data, 0, data.Length);
                }
            }

            // The Guid(byte[]) constructor swaps the first 4+2+2 bytes.
            // Compensate for that, otherwise we will not be able to match
            // the Guids  the configuration database.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(data, 0, 4);
                Array.Reverse(data, 4, 2);
                Array.Reverse(data, 6, 2);
            }

            return new Guid(data);
        }
    }
    #endregion

    #region HAPTIC DIRECTION
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct HapticDirection
    {
        public byte type;
        public fixed int dir[3];
    }
    #endregion

    #region HAPCTIC CONSTANT
    [StructLayout(LayoutKind.Sequential)]
    struct HapticConstant
    {
        // Header
        public ushort type;
        public HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Constant
        public short level;
        // Envelope
        public ushort attack_length;
        public ushort attack_level;
        public ushort fade_length;
        public ushort fade_level;
    }
    #endregion

    #region HAPTIC PERIODIC
    [StructLayout(LayoutKind.Sequential)]
    struct HapticPeriodic
    {
        // Header
        public ushort type;
        public HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Periodic
        public ushort period;
        public short magnitude;
        public short offset;
        public ushort phase;
        // Envelope
        public ushort attack_length;
        public ushort attack_level;
        public ushort fade_length;
        public ushort fade_level;
    }
    #endregion

    #region HAPTIC CONDITION
    [StructLayout(LayoutKind.Sequential)]
    unsafe struct HapticCondition
    {
        // Header
        public ushort type;
        public HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Condition
        public fixed ushort right_sat[3];
        public fixed ushort left_sat[3];
        public fixed short right_coeff[3];
        public fixed short left_coeff[3];
        public fixed ushort deadband[3];
        public fixed short center[3];
    }
    #endregion

    #region HAPTIC RAMP
    [StructLayout(LayoutKind.Sequential)]
    struct HapticRamp
    {
        // Header
        public ushort type;
        public HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Ramp
        public short start;
        public short end;
        // Envelope
        public ushort attack_length;
        public ushort attack_level;
        public ushort fade_length;
        public ushort fade_level;
    }
    #endregion

    #region HAPTIC LEFT RIGHT
    [StructLayout(LayoutKind.Sequential)]
    struct HapticLeftRight
    {
        // Header
        public ushort type;
        // Replay
        public uint length;
        // Rumble
        public ushort large_magnitude;
        public ushort small_magnitude;
    }
    #endregion

    #region HAPTIC CUSTOM
    [StructLayout(LayoutKind.Sequential)]
    struct HapticCustom
    {
        // Header
        public ushort type;
        public HapticDirection direction;
        // Replay
        public uint length;
        public ushort delay;
        // Trigger
        public ushort button;
        public ushort interval;
        // Custom
        public byte channels;
        public ushort period;
        public ushort samples;
        public IntPtr data; // Uint16*
                            // Envelope
        public ushort attack_length;
        public ushort attack_level;
        public ushort fade_length;
        public ushort fade_level;
    }
    #endregion

    #region HAPCTIC EFFECT
    [StructLayout(LayoutKind.Explicit)]
    struct HapticEffect
    {
        [FieldOffset(0)]
        public ushort type;
        [FieldOffset(0)]
        public HapticConstant constant;
        [FieldOffset(0)]
        public HapticPeriodic periodic;
        [FieldOffset(0)]
        public HapticCondition condition;
        [FieldOffset(0)]
        public HapticRamp ramp;
        [FieldOffset(0)]
        public HapticLeftRight leftright;
        [FieldOffset(0)]
        public HapticCustom custom;
    }
    #endregion
}
#endif