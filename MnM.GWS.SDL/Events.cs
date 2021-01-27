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

#if Window
using System;
using System.Runtime.InteropServices;
using System.Text;

using MnM.GWS.Advanced;

namespace MnM.GWS.SDL
{
    class TickEventArgs : EventArgs, ITickEventArgs
    {
        const string toStr = "tick:{0}, lastTick:{1}, fps:{2}";

        private int lastTick;
        private int tick;
        private int fps;

        #region constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tick">
        /// The current tick.
        /// </param>
        /// <param name="lastTick">
        /// The tick count that it was at last frame.
        /// </param>
        /// <param name="fps">Frames per second</param>
        public TickEventArgs(int tick, int lastTick, int fps)
        {
            this.tick = tick;
            this.lastTick = lastTick;
            this.fps = fps;
        }

        #endregion

        #region properties
        /// <summary>
        /// Gets when the last frame tick occurred.
        /// </summary>
        public int LastTick => lastTick;

        /// <summary>
        /// Gets the FPS as of the event call. Events.FPS is an alternative.
        /// </summary>
        public int Fps => fps;

        /// <summary>
        /// Gets the current SDL tick time.
        /// </summary>
        public int Tick => tick;

        /// <summary>
        /// Gets the difference  time between the 
        /// current tick and the last tick.
        /// </summary>
        public int TicksElapsed => tick - this.lastTick;

        /// <summary>
        /// Seconds elapsed between the last tick and the current tick
        /// </summary>
        public float SecondsElapsed => TicksElapsed / 1000.0f;
        #endregion
    }
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
    class FrameEventArgs : EventArgs, IFrameEventArgs
    {
        private double elapsed;
        const string toStr = "elapsed:{0}";

        /// <summary>
        /// Constructs a new FrameEventArgs instance.
        /// </summary>
        public FrameEventArgs()
        { }

        /// <summary>
        /// Constructs a new FrameEventArgs instance.
        /// </summary>
        /// <param name="elapsed">The amount of time that has elapsed since the previous event,  seconds.</param>
        public FrameEventArgs(double elapsed)
        {
            Time = elapsed;
        }

        /// <summary>
        /// Gets a <see cref="System.Double"/> that indicates how many seconds of time elapsed since the previous event.
        /// </summary>
        public double Time
        {
            get => elapsed;
            internal set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException();
                elapsed = value;
            }
        }
    }
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

    class KeyEventArgs : EventArgs, IKeyEventArgs
    {
        const string toStr = "key:{0}";
        Key k;
        internal KeyState state;

        public KeyEventArgs() { }

        public KeyEventArgs(IKeyEventArgs args)
        {
            KeyCode = (SdlKeys)args.ScanCode;
            k = args.KeyCode;
            state = Keyboard.IsKeyDown(KeyCode) ? KeyState.Down : KeyState.Up;
        }

        public bool Alt =>
            Keyboard[SdlKeys.AltLeft] || Keyboard[SdlKeys.AltRight];
        public bool Control =>
            Keyboard[SdlKeys.ControlLeft] || Keyboard[SdlKeys.ControlRight];
        public bool Shift =>
            Keyboard[SdlKeys.ShiftLeft] || Keyboard[SdlKeys.ShiftRight];
        public SdlKeyModifiers Modifiers
        {
            get
            {
                SdlKeyModifiers mods = 0;
                mods |= Alt ? SdlKeyModifiers.Alt : 0;
                mods |= Control ? SdlKeyModifiers.Control : 0;
                mods |= Shift ? SdlKeyModifiers.Shift : 0;
                return mods;
            }
        }
        public bool IsRepeat { get; internal set; }
        public bool Enter { get; set; }
        public bool SupressKeypress { get; set; }

        internal Keyboard Keyboard { get; set; }
        internal SdlKeys KeyCode { get; set; }

        public static Key GetKey(int scancode)
        {
            Scancode s = (Scancode)scancode;

            switch (s)
            {
                case Scancode.ESCAPE:
                    return Key.Escape;

                // Function keys
                case Scancode.F1:
                    return Key.F1;
                case Scancode.F2:
                    return Key.F2;
                case Scancode.F3:
                    return Key.F3;
                case Scancode.F4:
                    return Key.F4;
                case Scancode.F5:
                    return Key.F5;
                case Scancode.F6:
                    return Key.F6;
                case Scancode.F7:
                    return Key.F7;
                case Scancode.F8:
                    return Key.F8;
                case Scancode.F9:
                    return Key.F9;
                case Scancode.F10:
                    return Key.F10;
                case Scancode.F11:
                    return Key.F11;
                case Scancode.F12:
                    return Key.F12;
                case Scancode.F13:
                    return Key.F13;
                case Scancode.F14:
                    return Key.F14;
                case Scancode.F15:
                    return Key.F15;
                case Scancode.F16:
                    return Key.F16;
                case Scancode.F17:
                    return Key.F17;
                case Scancode.F18:
                    return Key.F18;
                case Scancode.F19:
                    return Key.F19;
                case Scancode.F20:
                    return Key.F20;
                case Scancode.F21:
                    return Key.F21;
                case Scancode.F22:
                    return Key.F22;
                case Scancode.F23:
                    return Key.F23;
                case Scancode.F24:
                    return Key.F24;

                // Number keys (0-9)
                case Scancode.Num0:
                    return Key.NumPad0;
                case Scancode.Num1:
                    return Key.NumPad1;
                case Scancode.Num2:
                    return Key.NumPad2;
                case Scancode.Num3:
                    return Key.NumPad3;
                case Scancode.Num4:
                    return Key.NumPad4;
                case Scancode.Num5:
                    return Key.NumPad5;
                case Scancode.Num6:
                    return Key.NumPad6;
                case Scancode.Num7:
                    return Key.NumPad7;
                case Scancode.Num8:
                    return Key.NumPad8;
                case Scancode.Num9:
                    return Key.NumPad9;

                // Letters (A-Z)
                case Scancode.A:
                    return Key.A;
                case Scancode.B:
                    return Key.B;
                case Scancode.C:
                    return Key.C;
                case Scancode.D:
                    return Key.D;
                case Scancode.E:
                    return Key.E;
                case Scancode.F:
                    return Key.F;
                case Scancode.G:
                    return Key.G;
                case Scancode.H:
                    return Key.H;
                case Scancode.I:
                    return Key.I;
                case Scancode.J:
                    return Key.J;
                case Scancode.K:
                    return Key.K;
                case Scancode.L:
                    return Key.L;
                case Scancode.M:
                    return Key.M;
                case Scancode.N:
                    return Key.N;
                case Scancode.O:
                    return Key.O;
                case Scancode.P:
                    return Key.P;
                case Scancode.Q:
                    return Key.Q;
                case Scancode.R:
                    return Key.R;
                case Scancode.S:
                    return Key.S;
                case Scancode.T:
                    return Key.T;
                case Scancode.U:
                    return Key.U;
                case Scancode.V:
                    return Key.V;
                case Scancode.W:
                    return Key.W;
                case Scancode.X:
                    return Key.X;
                case Scancode.Y:
                    return Key.Y;
                case Scancode.Z:
                    return Key.Z;

                case Scancode.TAB:
                    return Key.Tab;
                case Scancode.CAPSLOCK:
                    return Key.CapsLock;
                case Scancode.LCTRL:
                    return Key.LControlKey;
                case Scancode.LSHIFT:
                    return Key.LShiftKey;
                case Scancode.LALT:
                    return Key.LAlt;
                case Scancode.MENU:
                    return Key.Menu;
                case Scancode.LGUI:
                    return Key.LWin;
                case Scancode.RGUI:
                    return Key.RWin;
                case Scancode.SPACE:
                    return Key.Space;
                case Scancode.RALT:
                    return Key.RALT;
                //case Code.:
                //    return Key.WinRight;
                case Scancode.APPLICATION:
                    return Key.Menu;
                case Scancode.RCTRL:
                    return Key.RControlKey;
                case Scancode.RSHIFT:
                    return Key.RShiftKey;
                case Scancode.RETURN:
                    return Key.Enter;
                case Scancode.BACKSPACE:
                    return Key.Back;

                case Scancode.SEMICOLON:
                    return Key.OemSemicolon;      // Varies by keyboard: return ;: on Win2K/US
                case Scancode.SLASH:
                    return Key.OemSlash;          // Varies by keyboard: return /? on Win2K/US
                case Scancode.GRAVE:
                    return Key.OemTilde;          // Varies by keyboard: return `~ on Win2K/US
                case Scancode.LEFTBRACKET:
                    return Key.OemOpenBrackets;    // Varies by keyboard: return [{ on Win2K/US
                case Scancode.BACKSLASH:
                    return Key.OemBackslash;      // Varies by keyboard: return \| on Win2K/US
                case Scancode.RIGHTBRACKET:
                    return Key.OemCloseBrackets;   // Varies by keyboard: return ]} on Win2K/US
                case Scancode.APOSTROPHE:
                    return Key.OemQuotes;          // Varies by keyboard: return '" on Win2K/US
                case Scancode.EQUALS:
                    return Key.Oemplus;
                case Scancode.COMMA:
                    return Key.Oemcomma;     // Invariant: : return
                case Scancode.MINUS:
                    return Key.OemMinus;     // Invariant: -
                case Scancode.PERIOD:
                    return Key.OemPeriod;    // Invariant: .

                case Scancode.HOME:
                    return Key.Home;
                case Scancode.END:
                    return Key.End;
                case Scancode.DELETE:
                    return Key.Delete;
                case Scancode.PAGEUP:
                    return Key.PageUp;
                case Scancode.PAGEDOWN:
                    return Key.PageDown;
                case Scancode.PAUSE:
                    return Key.Pause;
                case Scancode.NUMLOCKCLEAR:
                    return Key.NumLock;

                case Scancode.SCROLLLOCK:
                    return Key.Scroll;
                case Scancode.PRINTSCREEN:
                    return Key.PrintScreen;
                case Scancode.CLEAR:
                    return Key.Clear;
                case Scancode.INSERT:
                    return Key.Insert;

                case Scancode.SLEEP:
                    return Key.Sleep;

                // Keypad
                case Scancode.KP_0:
                    return Key.D0;
                case Scancode.KP_1:
                    return Key.D1;
                case Scancode.KP_2:
                    return Key.D2;
                case Scancode.KP_3:
                    return Key.D3;
                case Scancode.KP_4:
                    return Key.D4;
                case Scancode.KP_5:
                    return Key.D5;
                case Scancode.KP_6:
                    return Key.D6;
                case Scancode.KP_7:
                    return Key.D7;
                case Scancode.KP_8:
                    return Key.D8;
                case Scancode.KP_9:
                    return Key.D9;

                case Scancode.KP_DECIMAL:
                    return Key.Decimal;
                case Scancode.KP_PLUS:
                    return Key.Add;
                case Scancode.KP_MINUS:
                    return Key.OemMinus;
                case Scancode.KP_DIVIDE:
                    return Key.Divide;
                case Scancode.KP_MULTIPLY:
                    return Key.Multiply;
                case Scancode.KP_ENTER:
                    return Key.Enter;

                // Navigation
                case Scancode.UP:
                    return Key.Up;
                case Scancode.DOWN:
                    return Key.Down;
                case Scancode.LEFT:
                    return Key.Left;
                case Scancode.RIGHT:
                    return Key.Right;

                default:
                    return Key.NoName;
            }
        }

        #region interface
        Key IKeyEventArgs.KeyCode => k;
        int IKeyEventArgs.ScanCode => (int)KeyCode;
        Key IKeyEventArgs.Modifiers => GetKey((int)Modifiers);
        KeyState IKeyEventArgs.State => state;
        #endregion
    }
    //class KeyPressEventArgs : EventArgs, IKeyPressEventArgs
    //{
    //    /// <summary>
    //    /// Constructs a new instance.
    //    /// </summary>
    //    /// <param name="keyChar">The ASCII character that was typed.</param>
    //    public KeyPressEventArgs(char keyChar)
    //    {
    //        KeyChar = keyChar;
    //    }

    //    /// <summary>
    //    /// Gets a <see cref="System.Char"/> that defines the ASCII character that was typed.
    //    /// </summary>
    //    public char KeyChar { get; internal set; }
    //}

    class MouseEventArgs : EventArgs, IMouseEventArgs
    {
        private Mouse state;
        const string toStr = "x:{0}, y{1}";

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public MouseEventArgs()
        {
            state.SetIsConnected(true);
        }


        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        public MouseEventArgs(int x, int y)
            : this()
        {
            state.X = x;
            state.Y = y;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> instance to clone.</param>
        public MouseEventArgs(IMouseEventArgs args)
            : this(args.X, args.Y)
        {
        }

        public MouseEventArgs(int x, int y, MouseState state = 0,
            MouseButton button = 0, int clicks = 0, int delta = 0, Vector dragStart = default(Vector)) :
            this(x, y)
        {
            Status = state;
            Clicks = clicks;
            Clicked = clicks > 0;
            Delta = delta;
            DragStartX = dragStart.X;
            DragStartY = dragStart.Y;
        }

        internal void SetButton(MouseButton button, State state)
        {
            if (button < 0 || button > MouseButton.LastButton)
            {
                throw new ArgumentOutOfRangeException();
            }
            switch (state)
            {
                case State.Pressed:
                    this.state.EnableBit((int)button);
                    break;

                case State.Released:
                    this.state.DisableBit((int)button);
                    break;
            }
        }

        internal State GetButton(MouseButton button)
        {
            if (button < 0 || button > MouseButton.LastButton)
            {
                throw new ArgumentOutOfRangeException();
            }

            return
                state.ReadBit((int)button) ?
                State.Pressed : State.Released;
        }

        /// <summary>
        /// Gets the X position of the mouse for the event.
        /// </summary>
        public int X
        {
            get => state.X;
            internal set => state.X = value;
        }

        /// <summary>
        /// Gets the Y position of the mouse for the event.
        /// </summary>
        public int Y
        {
            get => state.Y;
            internal set => state.Y = value;
        }

        /// <summary>
        /// Gets a <see cref="MnM.GWS.VectorF"/> representing the location of the mouse for the event.
        /// </summary>
        public Vector Position => new Vector(X, Y);
        public MouseButton Button { get; internal set; }

        public virtual MouseState Status { get; internal set; }
        public int Clicks { get; internal set; }
        public virtual int Delta { get; internal set; }
        public bool Clicked { get; internal set; }
        public int DragStartX { get; internal set; }
        public int DragStartY { get; internal set; }
        public virtual int XDelta { get; internal set; }
        public virtual int YDelta { get; internal set; }
        public bool Enter { get; set; }

        public static MouseButton GetButton(int button)
        {
            _MouseButton b = (_MouseButton)button;
            switch (b)
            {
                case _MouseButton.Left:
                    return MouseButton.Left;
                case _MouseButton.Middle:
                    return MouseButton.Middle;
                case _MouseButton.Right:
                    return MouseButton.Right;
                case _MouseButton.X1:
                    return MouseButton.Button1;
                case _MouseButton.X2:
                    return MouseButton.Button2;
                default:
                    return (MouseButton)button;
            }
        }
        /// <summary>
        /// Gets the current <see cref="MnM.GWS.MouseStateInfo"/>.
        /// </summary>
        internal Mouse Mouse
        {
            get { return state; }
            set { state = value; }
        }

        public override string ToString() => string.Format(toStr, X, Y);
    }
    class MouseMoveEventArgs : MouseEventArgs
    {
        const string toStr = "x:{0}, y{1}, button:{2}, xDelta:{3}, yDelta:{4}";
        /// <summary>
        /// Constructs a new <see cref="MouseMoveEventArgs"/> instance.
        /// </summary>
        public MouseMoveEventArgs() { }

        /// <summary>
        /// Constructs a new <see cref="MouseMoveEventArgs"/> instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="xDelta">The change  X position produced by this event.</param>
        /// <param name="yDelta">The change  Y position produced by this event.</param>
        public MouseMoveEventArgs(int x, int y, int xDelta, int yDelta)
            : base(x, y)
        {
            XDelta = xDelta;
            YDelta = yDelta;
        }

        /// <summary>
        /// Constructs a new <see cref="MouseMoveEventArgs"/> instance.
        /// </summary>
        /// <param name="args">The <see cref="MouseMoveEventArgs"/> instance to clone.</param>
        public MouseMoveEventArgs(MouseMoveEventArgs args)
            : this(args.X, args.Y, args.XDelta, args.YDelta)
        {
        }

        public override MouseState Status
        {
            get => MouseState.Move;
            internal set { }
        }
    }
    class MouseButtonEventArgs : MouseEventArgs
    {
        const string toStr = "x:{0}, y:{1}, pressed:{2}";

        /// <summary>
        /// Constructs a new <see cref="MouseButtonEventArgs"/> instance.
        /// </summary>
        public MouseButtonEventArgs() { }

        /// <summary>
        /// Constructs a new <see cref="MouseButtonEventArgs"/> instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="button">The mouse button for the event.</param>
        /// <param name="pressed">The current state of the button.</param>
        public MouseButtonEventArgs(int x, int y, MouseButton button, bool pressed)
            : base(x, y)
        {
            Button = button;
            IsPressed = pressed;
        }

        /// <summary>
        /// Constructs a new <see cref="MouseButtonEventArgs"/> instance.
        /// </summary>
        /// <param name="args">The <see cref="MouseButtonEventArgs"/> instance to clone.</param>
        public MouseButtonEventArgs(MouseButtonEventArgs args)
            : this(args.X, args.Y, args.Button, args.IsPressed)
        {
        }

        public bool IsPressed
        {
            get { return GetButton(Button) == State.Pressed; }
            internal set { SetButton(Button, value ? State.Pressed : State.Released); }
        }

        public override string ToString()
        {
            return string.Format(toStr, X, Y, Button, IsPressed);
        }

        public override MouseState Status
        {
            get => IsPressed ? MouseState.Down : MouseState.Up;
            internal set { }
        }
    }
    class MouseWheelEventArgs : MouseEventArgs
    {
        /// <summary>
        /// Constructs a new <see cref="MouseWheelEventArgs"/> instance.
        /// </summary>
        public MouseWheelEventArgs() { }

        /// <summary>
        /// Constructs a new <see cref="MouseWheelEventArgs"/> instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="value">The value of the wheel.</param>
        /// <param name="delta">The change  value of the wheel for this event.</param>
        public MouseWheelEventArgs(int x, int y, int value, int delta)
            : base(x, y)
        {
            Mouse.SetScrollAbsolute(Mouse.Scroll.Xf, value);
            this.DeltaPrecise = delta;
        }

        /// <summary>
        /// Constructs a new <see cref="MouseWheelEventArgs"/> instance.
        /// </summary>
        /// <param name="args">The <see cref="MouseWheelEventArgs"/> instance to clone.</param>
        public MouseWheelEventArgs(MouseWheelEventArgs args)
            : this(args.X, args.Y, args.Value, args.Delta)
        {
        }

        /// <summary>
        /// Gets the value of the wheel  integer units.
        /// To support high-precision mice, it is recommended to use <see cref="ValuePrecise"/> instead.
        /// </summary>
        public int Value => (int)Math.Round(Mouse.Scroll.Yf, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Gets the change  value of the wheel for this event  integer units.
        /// To support high-precision mice, it is recommended to use <see cref="DeltaPrecise"/> instead.
        /// </summary>
        public override int Delta
        {
            get => (int)Math.Round(DeltaPrecise, MidpointRounding.AwayFromZero);
            internal set { }
        }
        /// <summary>
        /// Gets the precise value of the wheel  floating-point units.
        /// </summary>
        public float ValuePrecise
        {
            get { return Mouse.Scroll.Yf; }
        }

        /// <summary>
        /// Gets the precise change  value of the wheel for this event  floating-point units.
        /// </summary>
        public float DeltaPrecise { get; internal set; }

        public override MouseState Status
        {
            get => MouseState.Wheel;
            internal set { }
        }
    }
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

    class TextInputEventArgs: EventArgs, ITextInputEventArgs
    {
        internal TextInputEventArgs()
        {

        }
        public TextInputEventArgs(char[] chars)
        {
            Characters = chars;
        }
        public char[] Characters { get; internal set; }
    }
    struct Mouse : IEquatable<Mouse>
    {
        internal const int MaxButtons = 16; // we are storing  an ushort
        int x, y;
        private MouseScroll scroll;
        private ushort buttons;

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the specified
        /// <see cref="MnM.GWS.MouseButton"/> is pressed.
        /// </summary>
        /// <param name="button">The <see cref="MnM.GWS.MouseButton"/> to check.</param>
        /// <returns>True if key is pressed; false otherwise.</returns>
        public bool this[MouseButton button]
        {
            get => IsButtonDown(button);
            internal set
            {
                if (value)
                    EnableBit((int)button);
                else
                    DisableBit((int)button);
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this button is down.
        /// </summary>
        /// <param name="button">The <see cref="MnM.GWS.MouseButton"/> to check.</param>
        public bool IsButtonDown(MouseButton button) =>
            ReadBit((int)button);

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this button is up.
        /// </summary>
        /// <param name="button">The <see cref="MnM.GWS.MouseButton"/> to check.</param>
        public bool IsButtonUp(MouseButton button) =>
            !ReadBit((int)button);

        /// <summary>
        /// Gets the absolute wheel position  integer units.
        /// To support high-precision mice, it is recommended to use <see cref="WheelPrecise"/> instead.
        /// </summary>
        public int Wheel =>
            (int)Math.Round(scroll.Yf, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Gets the absolute wheel position  floating-point units.
        /// </summary>
        public float WheelPrecise =>
            scroll.Yf;

        /// <summary>
        /// Gets a <see cref="MnM.GWS.MouseScroll"/> instance,
        /// representing the current state of the mouse scroll wheel.
        /// </summary>
        public MouseScroll Scroll
        {
            get { return scroll; }
        }
        /// <summary>
        /// Gets an integer representing the absolute x position of the pointer,  window pixel coordinates.
        /// </summary>
        public int X
        {
            get => x;
            internal set => x = value;
        }

        /// <summary>
        /// Gets an integer representing the absolute y position of the pointer,  window pixel coordinates.
        /// </summary>
        public int Y
        {
            get => y;
            internal set => y = value;
        }
        internal Vector Position
        {
            get => new Vector(X, Y);
            set
            {
                x = value.X;
                y = value.Y;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the left mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public State LeftButton
        {
            get { return IsButtonDown(MouseButton.Left) ? State.Pressed : State.Released; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the middle mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public State MiddleButton
        {
            get { return IsButtonDown(MouseButton.Middle) ? State.Pressed : State.Released; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the right mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public State RightButton
        {
            get { return IsButtonDown(MouseButton.Right) ? State.Pressed : State.Released; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the first extra mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public State XButton1
        {
            get { return IsButtonDown(MouseButton.Button1) ? State.Pressed : State.Released; }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the second extra mouse button is pressed.
        /// This property is intended for XNA compatibility.
        /// </summary>
        public State XButton2
        {
            get { return IsButtonDown(MouseButton.Button2) ? State.Pressed : State.Released; }
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
        /// Gets the absolute wheel position  integer units. This property is intended for XNA compatibility.
        /// To support high-precision mice, it is recommended to use <see cref="WheelPrecise"/> instead.
        /// </summary>
        public int ScrollWheelValue
        {
            get { return Wheel; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected.
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected { get; internal set; }

        /// <summary>
        /// Checks whether two <see cref="Mouse" /> instances are equal.
        /// </summary>
        /// <param name="left">
        /// A <see cref="Mouse"/> instance.
        /// </param>
        /// <param name="right">
        /// A <see cref="Mouse"/> instance.
        /// </param>
        /// <returns>
        /// True if both left is equal to right; false otherwise.
        /// </returns>
        public static bool operator ==(Mouse left, Mouse right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="Mouse" /> instances are not equal.
        /// </summary>
        /// <param name="left">
        /// A <see cref="Mouse"/> instance.
        /// </param>
        /// <param name="right">
        /// A <see cref="Mouse"/> instance.
        /// </param>
        /// <returns>
        /// True if both left is not equal to right; false otherwise.
        /// </returns>
        public static bool operator !=(Mouse left, Mouse right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares to an object instance for equality.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare to.
        /// </param>
        /// <returns>
        /// True if this instance is equal to obj; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Mouse)
            {
                return this == (Mouse)obj;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a hashcode for the current instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Int32"/> represting the hashcode for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return buttons.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode() ^ scroll.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.GWS.MouseStateInfo"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.GWS.MouseStateInfo"/>.</returns>
        public override string ToString()
        {
            string b = Convert.ToString(buttons, 2).PadLeft(10, '0');
            return String.Format("[X={0}, Y={1}, Scroll={2}, Buttons={3}, IsConnected={4}]",
                X, Y, Scroll, b, IsConnected);
        }

        internal bool ReadBit(int offset)
        {
            ValidateOffset(offset);
            return (buttons & (1 << offset)) != 0;
        }

        internal void EnableBit(int offset)
        {
            ValidateOffset(offset);
            buttons |= unchecked((ushort)(1 << offset));
        }

        internal void DisableBit(int offset)
        {
            ValidateOffset(offset);
            buttons &= unchecked((ushort)(~(1 << offset)));
        }

        internal void MergeBits(Mouse other)
        {
            buttons |= other.buttons;
            SetScrollRelative(other.scroll.Xf, other.scroll.Yf);
            X += other.X;
            Y += other.Y;
            IsConnected |= other.IsConnected;
        }

        internal void SetIsConnected(bool value)
        {
            IsConnected = value;
        }

        internal void SetScrollAbsolute(float x, float y)
        {
            scroll.Xf = x;
            scroll.Yf = y;
        }

        internal void SetScrollRelative(float x, float y)
        {
            scroll.Xf += x;
            scroll.Yf += y;
        }

        private static void ValidateOffset(int offset)
        {
            if (offset < 0 || offset >= 16)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
        }
        /// <summary>
        /// Compares two MouseState instances.
        /// </summary>
        /// <param name="other">The instance to compare two.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public bool Equals(Mouse other)
        {
            return
                buttons == other.buttons &&
                X == other.X &&
                Y == other.Y &&
                Scroll == other.Scroll;
        }

        /// <summary>
        /// Represents the state of a mouse wheel.
        /// </summary>
        public struct MouseScroll : IEquatable<MouseScroll>
        {
            /// <summary>
            /// Gets the absolute horizontal offset of the wheel,
            /// or 0 if no horizontal scroll wheel exists.
            /// </summary>
            /// <value>The x.</value>
            public float Xf { get; internal set; }

            /// <summary>
            /// Gets the absolute vertical offset of the wheel,
            /// or 0 if no vertical scroll wheel exists.
            /// </summary>
            /// <value>The y.</value>
            public float Yf { get; internal set; }

            /// <param name="left">A <see cref="MouseScroll"/> instance to test for equality.</param>
            /// <param name="right">A <see cref="MouseScroll"/> instance to test for equality.</param>
            public static bool operator ==(MouseScroll left, MouseScroll right)
            {
                return left.Equals(right);
            }

            /// <param name="left">A <see cref="MouseScroll"/> instance to test for inequality.</param>
            /// <param name="right">A <see cref="MouseScroll"/> instance to test for inequality.</param>
            public static bool operator !=(MouseScroll left, MouseScroll right)
            {
                return !left.Equals(right);
            }

            /// <summary>
            /// Returns a <see cref="System.String"/> that represents the current <see cref="MnM.GWS.MouseScroll"/>.
            /// </summary>
            /// <returns>A <see cref="System.String"/> that represents the current <see cref="MnM.GWS.MouseScroll"/>.</returns>
            public override string ToString()
            {
                return string.Format("[X={0:0.00}, Y={1:0.00}]", Xf, Yf);
            }

            /// <summary>
            /// Serves as a hash function for a <see cref="MnM.GWS.MouseScroll"/> object.
            /// </summary>
            /// <returns>A hash code for this instance that is suitable for use  hashing algorithms and data structures such as a
            /// hash table.</returns>
            public override int GetHashCode()
            {
                return Xf.GetHashCode() ^ Yf.GetHashCode();
            }

            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="MnM.GWS.MouseScroll"/>.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="MnM.GWS.MouseScroll"/>.</param>
            /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
            /// <see cref="MnM.GWS.MouseScroll"/>; otherwise, <c>false</c>.</returns>
            public override bool Equals(object obj)
            {
                return
                    obj is MouseScroll &&
                    Equals((MouseScroll)obj);
            }

            /// <summary>
            /// Determines whether the specified <see cref="MnM.GWS.MouseScroll"/> is equal to the current <see cref="MnM.GWS.MouseScroll"/>.
            /// </summary>
            /// <param name="other">The <see cref="MnM.GWS.MouseScroll"/> to compare with the current <see cref="MnM.GWS.MouseScroll"/>.</param>
            /// <returns><c>true</c> if the specified <see cref="MnM.GWS.MouseScroll"/> is equal to the current
            /// <see cref="MnM.GWS.MouseScroll"/>; otherwise, <c>false</c>.</returns>
            public bool Equals(MouseScroll other)
            {
                return Xf == other.Xf && Yf == other.Yf;
            }
        }
    }
    struct Keyboard : IEquatable<Keyboard>
    {
        // Allocate enough ints to store all keyboard keys
        private const int IntSize = sizeof(int) * 8;

        private const int NumInts = ((int)SdlKeys.LastKey + IntSize - 1) / IntSize;
        // The following line triggers bogus CS0214  gmcs 2.0.1, sigh...
        private unsafe fixed int Keys[NumInts];

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the specified
        /// <see cref="MnM.GWS.Key"/> is pressed.
        /// </summary>
        /// <param name="key">The <see cref="MnM.GWS.Key"/> to check.</param>
        /// <returns>True if key is pressed; false otherwise.</returns>
        public bool this[SdlKeys key]
        {
            get { return IsKeyDown(key); }
            internal set { SetKeyState(key, value); }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether the specified
        /// <see cref="MnM.GWS.Key"/> is pressed.
        /// </summary>
        /// <param name="code">The scancode to check.</param>
        /// <returns>True if code is pressed; false otherwise.</returns>
        public bool this[short code]
        {
            get { return IsKeyDown((SdlKeys)code); }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this key is down.
        /// </summary>
        /// <param name="key">The <see cref="MnM.GWS.Key"/> to check.</param>
        public bool IsKeyDown(SdlKeys key)
        {
            return ReadBit((int)key);
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this scan code is down.
        /// </summary>
        /// <param name="code">The scan code to check.</param>
        public bool IsKeyDown(short code)
        {
            return code >= 0 && code < (short)SdlKeys.LastKey && ReadBit(code);
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this key is up.
        /// </summary>
        /// <param name="key">The <see cref="MnM.GWS.Key"/> to check.</param>
        public bool IsKeyUp(SdlKeys key)
        {
            return !ReadBit((int)key);
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this scan code is down.
        /// </summary>
        /// <param name="code">The scan code to check.</param>
        public bool IsKeyUp(short code)
        {
            return !IsKeyDown(code);
        }

        /// <summary>
        /// Gets a value indicating whether any key is down.
        /// </summary>
        /// <value><c>true</c> if any key is down; otherwise, <c>false</c>.</value>
        public bool IsAnyKeyDown
        {
            get
            {
                // If any bit is set then a key is down.
                unsafe
                {
                    fixed (int* k = Keys)
                    {
                        for (int i = 0; i < NumInts; ++i)
                        {
                            if (k[i] != 0)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets a <see cref="System.Boolean"/> indicating whether this keyboard
        /// is connected.
        /// </summary>
        public bool IsConnected { get; internal set; }

#if false
        // Disabled until the correct cross-platform API can be determined.
        public bool IsLedOn(KeyboardLeds led)
        {
            return false;
        }

        public bool IsLedOff(KeyboardLeds led)
        {
            return false;
        }
#endif

        /// <summary>
        /// Checks whether two <see cref="Keyboard" /> instances are equal.
        /// </summary>
        /// <param name="left">
        /// A <see cref="Keyboard"/> instance.
        /// </param>
        /// <param name="right">
        /// A <see cref="Keyboard"/> instance.
        /// </param>
        /// <returns>
        /// True if both left is equal to right; false otherwise.
        /// </returns>
        public static bool operator ==(Keyboard left, Keyboard right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks whether two <see cref="Keyboard" /> instances are not equal.
        /// </summary>
        /// <param name="left">
        /// A <see cref="Keyboard"/> instance.
        /// </param>
        /// <param name="right">
        /// A <see cref="Keyboard"/> instance.
        /// </param>
        /// <returns>
        /// True if both left is not equal to right; false otherwise.
        /// </returns>
        public static bool operator !=(Keyboard left, Keyboard right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares to an object instance for equality.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare to.
        /// </param>
        /// <returns>
        /// True if this instance is equal to obj; false otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Keyboard)
            {
                return this == (Keyboard)obj;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Generates a hashcode for the current instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.Int32"/> represting the hashcode for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            unsafe
            {
                fixed (int* k = Keys)
                {
                    int hashcode = 0;
                    for (int i = 0; i < NumInts; i++)
                    {
                        hashcode ^= (k + i)->GetHashCode();
                    }
                    return hashcode;
                }
            }
        }

        internal void SetKeyState(SdlKeys key, bool down)
        {
            if (down)
            {
                EnableBit((int)key);
            }
            else
            {
                DisableBit((int)key);
            }
        }

        internal bool ReadBit(int offset)
        {
            ValidateOffset(offset);

            int int_offset = offset / IntSize;
            int bit_offset = offset % IntSize;
            unsafe
            {
                fixed (int* k = Keys) { return (*(k + int_offset) & (1 << bit_offset)) != 0u; }
            }
        }

        internal void EnableBit(int offset)
        {
            ValidateOffset(offset);

            int int_offset = offset / IntSize;
            int bit_offset = offset % IntSize;
            unsafe
            {
                fixed (int* k = Keys) { *(k + int_offset) |= 1 << bit_offset; }
            }
        }

        internal void DisableBit(int offset)
        {
            ValidateOffset(offset);

            int int_offset = offset / IntSize;
            int bit_offset = offset % IntSize;
            if (int_offset >= 33554433)
                return;
            try
            {
                unsafe
                {
                    fixed (int* k = Keys) { *(k + int_offset) &= ~(1 << bit_offset); }
                }
            }
            catch { }
        }

        internal void MergeBits(Keyboard other)
        {
            unsafe
            {
                int* k2 = other.Keys;
                fixed (int* k1 = Keys)
                {
                    for (int i = 0; i < NumInts; i++)
                    {
                        *(k1 + i) |= *(k2 + i);
                    }
                }
            }
            IsConnected |= other.IsConnected;
        }

        internal void SetIsConnected(bool value)
        {
            IsConnected = value;
        }

        private static void ValidateOffset(int offset)
        {
            if (offset < 0 || offset >= NumInts * IntSize)
            {
                //throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Compares two KeyboardState instances.
        /// </summary>
        /// <param name="other">The instance to compare two.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public bool Equals(Keyboard other)
        {
            bool equal = true;
            unsafe
            {
                int* k2 = other.Keys;
                fixed (int* k1 = Keys)
                {
                    for (int i = 0; equal && i < NumInts; i++)
                    {
                        equal &= *(k1 + i) == *(k2 + i);
                    }
                }
            }
            return equal;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    struct Event: IEvent
    {
        [FieldOffset(0)]
        public EventType Type;

        [FieldOffset(0)]
        public WindowEvent Window;

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

        GwsEvent IEvent.Type
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
                        switch (Window.Event)
                        {
                            case WindowEventID.Shown:
                                return GwsEvent.Shown;
                            case WindowEventID.Hidden:
                                return GwsEvent.Hidden;
                            case WindowEventID.Exposed:
                                return GwsEvent.Hidden;
                            case WindowEventID.Moved:
                                return GwsEvent.Moved;
                            case WindowEventID.Resized:
                                return GwsEvent.Resized;
                            case WindowEventID.SizeChanged:
                                return GwsEvent.SizeChanged;
                            case WindowEventID.MiniMized:
                                return GwsEvent.Minimized;
                            case WindowEventID.Maximized:
                                return GwsEvent.Maximized;
                            case WindowEventID.Restored:
                                return GwsEvent.Restored;
                            case WindowEventID.Enter:
                                return GwsEvent.Enter;
                            case WindowEventID.Leave:
                                return GwsEvent.Leave;
                            case WindowEventID.FocusGained:
                                return GwsEvent.FocusGained;
                            case WindowEventID.FocusLost:
                                return GwsEvent.FocusLost;
                            case WindowEventID.Close:
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
                    case EventType.TEXTINPUT:
                        return GwsEvent.TextInput;
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
        IExEvent IEvent.Window => Window;
        public string ID => "Window" + Window.WindowID;
    }

    /* A video driver dependent event (event.syswm.*), disabled */
    [StructLayout(LayoutKind.Sequential)]
    struct SysWMEvent
    {
        public EventType type;
        public UInt32 timestamp;
        public IntPtr msg; /* SDL_SysWMmsg*, system-dependent*/
    }
    struct WindowEvent:IExEvent
    {
        public EventType Type;
        public uint Timestamp;
        public int WindowID;
        public WindowEventID Event;
        private byte padding1;
        private byte padding2;
        private byte padding3;
        public int Data1;
        public int Data2;

        GwsEvent IExEvent.Type
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
                        switch (Event)
                        {
                            case WindowEventID.Shown:
                                return GwsEvent.Shown;
                            case WindowEventID.Hidden:
                                return GwsEvent.Hidden;
                            case WindowEventID.Exposed:
                                return GwsEvent.Hidden;
                            case WindowEventID.Moved:
                                return GwsEvent.Moved;
                            case WindowEventID.Resized:
                                return GwsEvent.Resized;
                            case WindowEventID.SizeChanged:
                                return GwsEvent.SizeChanged;
                            case WindowEventID.MiniMized:
                                return GwsEvent.Minimized;
                            case WindowEventID.Maximized:
                                return GwsEvent.Maximized;
                            case WindowEventID.Restored:
                                return GwsEvent.Restored;
                            case WindowEventID.Enter:
                                return GwsEvent.Enter;
                            case WindowEventID.Leave:
                                return GwsEvent.Leave;
                            case WindowEventID.FocusGained:
                                return GwsEvent.FocusGained;
                            case WindowEventID.FocusLost:
                                return GwsEvent.FocusLost;
                            case WindowEventID.Close:
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
                    case EventType.TEXTINPUT:
                        return GwsEvent.TextInput;
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
        int IExEvent.WindowID => WindowID;
        WindowEventID IExEvent.Event => Event;
    }
    struct Keysym
    {
        public Scancode Scancode;
        public Keycode Sym;
        public Keymod Mod;
        [Obsolete]
        public uint Unicode;
    }
    struct KeyboardEvent
    {
        public EventType Type;
        public uint Timestamp;
        public int WindowID;
        public State State;
        public byte Repeat;
        private byte padding2;
        private byte padding3;
        internal Keysym Keysym;
    }
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
    struct TextInputEvent
    {
        public const int TextSize = 32;

        public EventType Type;
        public UInt32 Timestamp;
        public UInt32 WindowID;
        public unsafe fixed byte Text[TextSize];
    }

    [StructLayout(LayoutKind.Sequential)]
    struct SizeEvent
    {
        /// <summary>New width of the window</summary>
        public uint Width;

        /// <summary>New height of the window</summary>
        public uint Height;
    }
    struct DropEvent
    {
        public uint Type;
        public uint Timestamp;
        public IntPtr File;
        public int WindowID;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct QuitEvent
    {
        internal EventType Type;
        public uint Timestamp;
    }

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

    struct MouseButtonEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int WindowID;
        public uint Which;
        public _MouseButton Button;
        public State State;
        public byte Clicks;
        private byte padding1;
        public int X;
        public int Y;
    }
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
    struct MouseWheelEvent
    {
        public EventType Type;
        public uint Timestamp;
        public int WindowID;
        public uint Which;
        public int X;
        public int Y;

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
    struct ControllerButtonEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which;
        public GameControllerButton Button;
        public State State;
        private byte padding1;
        private byte padding2;
    }
    struct ControllerDeviceEvent
    {
        internal EventType Type;
        public uint Timestamp;

        /// <summary>
        /// The joystick device index for the ADDED event, instance id for the REMOVED or REMAPPED event
        /// </summary>
        public int Which;
    }

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
            return index >= 0 && index < NativeFactory.MaxAxisCount;
        }

        private bool IsDPadValid(int index)
        {
            return index >= 0 && index < NativeFactory.MaxDPadCount;
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
        /// Gets the <see cref="State"/> for the up button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the up button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        public State Up
        {
            get { return IsUp ? State.Pressed : State.Released; }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the down button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the down button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        public State Down
        {
            get { return IsDown ? State.Pressed : State.Released; }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the left button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the left button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        public State Left
        {
            get { return IsLeft ? State.Pressed : State.Released; }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the right button.
        /// </summary>
        /// <value><c>ButtonState.Pressed</c> if the right button is pressed; otherwise, <c>ButtonState.Released</c>.</value>
        public State Right
        {
            get { return IsRight ? State.Pressed : State.Released; }
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

    /// <summary>
    /// Describes the <see cref="State"/> of <see cref="GamePad"/> <see cref="PadButtons"/>.
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
        /// Gets the <see cref="State"/> for the A button.
        /// </summary>
        public State A
        {
            get { return GetButton(PadButtons.A); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the B button.
        /// </summary>
        public State B
        {
            get { return GetButton(PadButtons.B); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the X button.
        /// </summary>
        public State X
        {
            get { return GetButton(PadButtons.X); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the Y button.
        /// </summary>
        public State Y
        {
            get { return GetButton(PadButtons.Y); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the Back button.
        /// </summary>
        public State Back
        {
            get { return GetButton(PadButtons.Back); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the big button.
        /// This button is also known as Home or Guide.
        /// </summary>
        public State BigButton
        {
            get { return GetButton(PadButtons.BigButton); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the left shoulder button.
        /// </summary>
        public State LeftShoulder
        {
            get { return GetButton(PadButtons.LeftShoulder); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the left stick button.
        /// This button represents a left stick that is pressed in.
        /// </summary>
        public State LeftStick
        {
            get { return GetButton(PadButtons.LeftStick); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the right shoulder button.
        /// </summary>
        public State RightShoulder
        {
            get { return GetButton(PadButtons.RightShoulder); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the right stick button.
        /// This button represents a right stick that is pressed in.
        /// </summary>
        public State RightStick
        {
            get { return GetButton(PadButtons.RightStick); }
        }

        /// <summary>
        /// Gets the <see cref="State"/> for the starth button.
        /// </summary>
        public State Start
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
            if (A == State.Pressed)
            {
                sb.Append("A");
            }
            if (B == State.Pressed)
            {
                sb.Append("B");
            }
            if (X == State.Pressed)
            {
                sb.Append("X");
            }
            if (Y == State.Pressed)
            {
                sb.Append("Y");
            }
            if (Back == State.Pressed)
            {
                sb.Append("Bk");
            }
            if (Start == State.Pressed)
            {
                sb.Append("St");
            }
            if (BigButton == State.Pressed)
            {
                sb.Append("Gd");
            }
            if (Back == State.Pressed)
            {
                sb.Append("Bk");
            }
            if (LeftShoulder == State.Pressed)
            {
                sb.Append("L");
            }
            if (RightShoulder == State.Pressed)
            {
                sb.Append("R");
            }
            if (LeftStick == State.Pressed)
            {
                sb.Append("Ls");
            }
            if (RightStick == State.Pressed)
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

        private State GetButton(PadButtons b)
        {
            return (buttons & b) != 0 ? State.Pressed : State.Released;
        }
    }

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

    /// <summary>
    /// Describes the current state of a <see cref="JoystickDevice"/>.
    /// </summary>
    struct JoystickState : IEquatable<JoystickState>
    {
        // If we ever add more values to JoystickAxis or JoystickButton
        // then we'll need to increase these limits.
        internal const int MaxAxes = 64;
        internal const int MaxButtons = 64;
        internal const int MaxHats = (int)JoystickHat.Last + 1;

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
        /// Gets the current <see cref="State"/> of the specified button.
        /// </summary>
        /// <returns><see cref="State.Pressed"/> if the specified button is pressed; otherwise, <see cref="State.Released"/>.</returns>
        /// <param name="button">The button to query.</param>
        public State GetButton(int button)
        {
            return (buttons & ((long)1 << button)) != 0 ? State.Pressed : State.Released;
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
                JoystickHat hat = JoystickHat.Hat0 + i;
                equals &= GetHat(hat).Equals(other.GetHat(hat));
            }
            return equals;
        }
    }


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

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct HapticDirection
    {
        public byte type;
        public fixed int dir[3];
    }

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
    struct JoyButtonEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which;
        public byte Button;
        public State State;
        private byte padding1;
        private byte padding2;
    }
    struct JoyDeviceEvent
    {
        internal EventType Type;
        public uint Timestamp;
        public int Which;
    }
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

    enum HIDPage : ushort
    {
        Undefined = 0x00,
        GenericDesktop = 0x01,
        Simulation = 0x02,
        VR = 0x03,
        Sport = 0x04,
        Game = 0x05,
        // Reserved 0x06
        KeyboardOrKeypad = 0x07, // USB Device Class Definition for Human Interface Devices (HID). Note: the usage type for all key codes is Selector (Sel).
        LEDs = 0x08,
        Button = 0x09,
        Ordinal = 0x0A,
        Telephony = 0x0B,
        Consumer = 0x0C,
        Digitizer = 0x0D,
        // Reserved 0x0E
        PID = 0x0F, // USB Physical Interface Device definitions for force feedback and related devices.
        Unicode = 0x10,
        // Reserved 0x11 - 0x13
        AlphanumericDisplay = 0x14,
        // Reserved 0x15 - 0x7F
        // Monitor 0x80 - 0x83   USB Device Class Definition for Monitor Devices
        // Power 0x84 - 0x87     USB Device Class Definition for Power Devices
        PowerDevice = 0x84,                // Power Device Page
        BatterySystem = 0x85,              // Battery System Page
        // Reserved 0x88 - 0x8B
        BarCodeScanner = 0x8C, // (Point of Sale) USB Device Class Definition for Bar Code Scanner Devices
        WeighingDevice = 0x8D, // (Point of Sale) USB Device Class Definition for Weighing Devices
        Scale = 0x8D, // (Point of Sale) USB Device Class Definition for Scale Devices
        MagneticStripeReader = 0x8E,
        // ReservedPointofSalepages 0x8F
        CameraControl = 0x90, // USB Device Class Definition for Image Class Devices
        Arcade = 0x91, // OAAF Definitions for arcade and coinop related Devices
        // Reserved 0x92 - 0xFEFF
        // VendorDefined 0xFF00 - 0xFFFF
        VendorDefinedStart = 0xFF00
    }
    enum HIDUsageCD
    {
        ACPan = 0x0238,
        ConsumerControl = 0x01
    }
    enum HIDUsageGD : ushort
    {
        Pointer = 0x01, // Physical Collection
        Mouse = 0x02, // Application Collection
        // 0x03 Reserved
        Joystick = 0x04, // Application Collection
        GamePad = 0x05, // Application Collection
        Keyboard = 0x06, // Application Collection
        Keypad = 0x07, // Application Collection
        MultiAxisController = 0x08, // Application Collection
        // 0x09 - 0x2F Reserved
        X = 0x30, // Dynamic Value
        Y = 0x31, // Dynamic Value
        Z = 0x32, // Dynamic Value
        Rx = 0x33, // Dynamic Value
        Ry = 0x34, // Dynamic Value
        Rz = 0x35, // Dynamic Value
        Slider = 0x36, // Dynamic Value
        Dial = 0x37, // Dynamic Value
        Wheel = 0x38, // Dynamic Value
        Hatswitch = 0x39, // Dynamic Value
        CountedBuffer = 0x3A, // Logical Collection
        ByteCount = 0x3B, // Dynamic Value
        MotionWakeup = 0x3C, // One-Shot Control
        Start = 0x3D, // On/Off Control
        Select = 0x3E, // On/Off Control
        // 0x3F Reserved
        Vx = 0x40, // Dynamic Value
        Vy = 0x41, // Dynamic Value
        Vz = 0x42, // Dynamic Value
        Vbrx = 0x43, // Dynamic Value
        Vbry = 0x44, // Dynamic Value
        Vbrz = 0x45, // Dynamic Value
        Vno = 0x46, // Dynamic Value
        // 0x47 - 0x7F Reserved
        SystemControl = 0x80, // Application Collection
        SystemPowerDown = 0x81, // One-Shot Control
        SystemSleep = 0x82, // One-Shot Control
        SystemWakeUp = 0x83, // One-Shot Control
        SystemContextMenu = 0x84, // One-Shot Control
        SystemMainMenu = 0x85, // One-Shot Control
        SystemAppMenu = 0x86, // One-Shot Control
        SystemMenuHelp = 0x87, // One-Shot Control
        SystemMenuExit = 0x88, // One-Shot Control
        SystemMenu = 0x89, // Selector
        SystemMenuRight = 0x8A, // Re-Trigger Control
        SystemMenuLeft = 0x8B, // Re-Trigger Control
        SystemMenuUp = 0x8C, // Re-Trigger Control
        SystemMenuDown = 0x8D, // Re-Trigger Control
        // 0x8E - 0x8F Reserved
        DPadUp = 0x90, // On/Off Control
        DPadDown = 0x91, // On/Off Control
        DPadRight = 0x92, // On/Off Control
        DPadLeft = 0x93, // On/Off Control
        // 0x94 - 0xFFFF Reserved
        Reserved = 0xFFFF
    }
    enum HIDUsageSim : ushort
    {
        FlightSimulationDevice = 0x01, // Application Collection
        AutomobileSimulationDevice = 0x02, //             Application Collection
        TankSimulationDevice = 0x03, //             Application Collection
        SpaceshipSimulationDevice = 0x04, //             Application Collection
        SubmarineSimulationDevice = 0x05, //             Application Collection
        SailingSimulationDevice = 0x06, //             Application Collection
        MotorcycleSimulationDevice = 0x07, //             Application Collection
        SportsSimulationDevice = 0x08, //             Application Collection
        AirplaneSimulationDevice = 0x09, //             Application Collection
        HelicopterSimulationDevice = 0x0A, //             Application Collection
        MagicCarpetSimulationDevice = 0x0B, //             Application Collection
        BicycleSimulationDevice = 0x0C, //             Application Collection
        // 0x0D - 0x1F Reserved
        FlightControlStick = 0x20, //             Application Collection
        FlightStick = 0x21, //             Application Collection
        CyclicControl = 0x22, //             Physical Collection
        CyclicTrim = 0x23, //             Physical Collection
        FlightYoke = 0x24, //             Application Collection
        TrackControl = 0x25, //             Physical Collection
        // 0x26 - 0xAF Reserved
        Aileron = 0xB0, //             Dynamic Value
        AileronTrim = 0xB1, //             Dynamic Value
        AntiTorqueControl = 0xB2, //             Dynamic Value
        AutopilotEnable = 0xB3, //             On/Off Control
        ChaffRelease = 0xB4, //             One-Shot Control
        CollectiveControl = 0xB5, //             Dynamic Value
        DiveBrake = 0xB6, //             Dynamic Value
        ElectronicCountermeasures = 0xB7, //             On/Off Control
        Elevator = 0xB8, //             Dynamic Value
        ElevatorTrim = 0xB9, //             Dynamic Value
        Rudder = 0xBA, //             Dynamic Value
        Throttle = 0xBB, //             Dynamic Value
        FlightCommunications = 0xBC, //             On/Off Control
        FlareRelease = 0xBD, //             One-Shot Control
        LandingGear = 0xBE, //             On/Off Control
        ToeBrake = 0xBF, //             Dynamic Value
        Trigger = 0xC0, //             Momentary Control
        WeaponsArm = 0xC1, //             On/Off Control
        Weapons = 0xC2, //             Selector
        WingFlaps = 0xC3, //             Dynamic Value
        Accelerator = 0xC4, //             Dynamic Value
        Brake = 0xC5, //             Dynamic Value
        Clutch = 0xC6, //             Dynamic Value
        Shifter = 0xC7, //             Dynamic Value
        Steering = 0xC8, //             Dynamic Value
        TurretDirection = 0xC9, //             Dynamic Value
        BarrelElevation = 0xCA, //             Dynamic Value
        DivePlane = 0xCB, //             Dynamic Value
        Ballast = 0xCC, //             Dynamic Value
        BicycleCrank = 0xCD, //             Dynamic Value
        HandleBars = 0xCE, //             Dynamic Value
        FrontBrake = 0xCF, //             Dynamic Value
        RearBrake = 0xD0, //             Dynamic Value
        // 0xD1 - 0xFFFF Reserved
        Reserved = 0xFFFF
    }

}
#endif