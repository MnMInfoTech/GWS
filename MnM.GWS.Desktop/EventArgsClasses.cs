using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MnM.GWS.Desktop
{
    #region EVENT ARGS CLASSES
    public class MsKeyEventArgs : IKeyEventArgs
    {
        internal Key keyCode;
        internal int scanCode;
        internal KeyState keyState;

        internal MsKeyEventArgs() { }
        public MsKeyEventArgs(System.Windows.Forms.KeyEventArgs e, KeyState state)
        {
            keyCode = e.KeyCode.ToGWSKey();
            scanCode = e.KeyValue;
            keyState = state;
        }

        public Key KeyCode { get => keyCode; }
        public int ScanCode => scanCode;
        public bool Alt => keyCode == Key.Alt || keyCode == Key.LAlt || keyCode == Key.RALT;
        public bool Control =>
            keyCode == Key.Control || keyCode == Key.ControlKey || keyCode == Key.LControlKey || keyCode == Key.RControlKey;
        public bool Shift => keyCode == Key.Shift || keyCode == Key.ShiftKey || keyCode == Key.LShiftKey || keyCode == Key.RShiftKey;
        public Key Modifiers
        {
            get
            {
                Key mods = 0;
                mods |= Alt ? Key.Alt : 0;
                mods |= Control ? Key.Control : 0;
                mods |= Shift ? Key.Shift : 0;
                return mods;
            }
        }
        public KeyState State => keyState;
        public bool SupressKeypress { get; set; }
        public bool Enter { get; set; }
    }
    public class MsMouseEventArgs : IMouseEventArgs
    {
        const string toStr = "x:{0}, y:{1}, pressed:{2}";
        internal int x, y;
        internal MouseState mouseState;
        internal MouseButton button;
        internal int delta;
        internal int clicks;
        internal int xDelta, yDelta;
        internal int dragStartX, dragStartY;

        internal MsMouseEventArgs()
        {
        }

        public MsMouseEventArgs(int x, int y, MouseButtons button, MouseState state)
        {
            this.x = x;
            this.y = y;
            this.button = button.ToGwsButton();
            mouseState = state;
        }
        public MsMouseEventArgs(int x, int y, MouseButtons button, bool pressed = false)
        {
            this.x = x;
            this.y = y;
            this.button = button.ToGwsButton();
            mouseState = pressed ? MouseState.Down : MouseState.Up;
        }
        public MsMouseEventArgs(int x, int y, MouseButtons button, MouseState state, int delta)
        {
            this.x = x;
            this.y = y;
            this.button = button.ToGwsButton();
            mouseState = state;
            this.delta = delta;
            if (state == MouseState.DragBegin || state == MouseState.Down)
            {
                dragStartX = x;
                dragStartY = y;
            }
            else if (state == MouseState.Up)
            {
                xDelta = x - dragStartX;
                yDelta = y - dragStartY;
            }
        }

        public int X => x;
        public int Y => y;
        public MouseState State => mouseState;
        public MouseButton Button => button;
        public int Clicks => clicks;
        public int Delta => delta;
        public bool Clicked { get; }
        public int DragStartX => dragStartX;
        public int DragStartY => dragStartY;
        public int XDelta => xDelta;
        public int YDelta => yDelta;
        public bool Enter { get; set; }
    }
    public class MsKeyPressEventArgs : IKeyPressEventArgs
    {
        internal char keyChar;
        public MsKeyPressEventArgs()
        {

        }
        public MsKeyPressEventArgs(char key)
        {
            keyChar = key;
        }
        public char KeyChar { get => keyChar; set => keyChar = value; }
    }
    #endregion

}
