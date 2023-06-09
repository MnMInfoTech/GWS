
#if Window && SDL
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MnM.GWS.SDL;

namespace MnM.GWS
{
    partial class Factory
    {
        internal sealed class SdlWindow : IExOSWindow
        {
            #region VARIABLES
            const int defaultWidth = 300;
            const int defaultHeight = 300;

            string title;
            char[] DecodeTextBuffer = new char[32];
            float opacity = 1f;
            WindowFlags windowFlags;
            ModifierKeys ModifierKey;
            MouseButton MouseButton;
            int DesktopID;
            MemoryOccupation MemoryOccupation;
            IExRenderTarget Target;
            int X, Y, Width, Height;
            IGLContext GLContext;
            ISound Sound;
            IntPtr Handle;
            string ID;
            IScreen Screen;
            Size ScreenSize;

            #region EVENT ARGS
            readonly IExMouseEventArgs MouseDownArgs = new MouseEventArgs();
            readonly IExMouseEnterLeaveEventArgs MouseEnterLeaveArgs = new MouseEnterLeaveEventArgs();
            readonly IExMouseEventArgs MouseUpArgs = new MouseEventArgs();
            readonly IExMouseEventArgs MouseMoveArgs = new MouseEventArgs();
            readonly IExMouseWheelEventArgs MouseWheelArgs = new MouseWheelEventArgs();

            readonly IExKeyEventArgs KeyDownArgs = new KeyEventArgs();
            readonly IExKeyEventArgs KeyUpArgs = new KeyEventArgs();
            readonly IExKeyPressEventArgs KeyPressArgs = new KeyPressEventArgs();
            readonly FileDropEventArgs FileDropArgs = new FileDropEventArgs();
            readonly ICancelEventArgs CancelArgs = new CancelEventArgs();
            #endregion
            #endregion

            #region PROPERTIES           
            OS IOSMinimalWindow.OS => SdlAPI.OS;
            string IOSMinimalWindow.LastError => SdlAPI.GetError();
            uint IOSMinimalWindow.PixelFormat => SdlAPI.pixelFormat;
            IScreen IOSMinimalWindow.Screen => Screen;
            float IOSMinimalWindow.Transparency
            {
                get => 1f - opacity;
                set
                {
                    opacity = 1f - value;
                    SdlAPI.SetWindowOpacity(Handle, opacity);
                }
            }
            IMemoryOccupation IMemoryOccupier.MemoryOccupation => MemoryOccupation;
            IGLContext IOSMinimalWindow.GLContext => GLContext;
            ISound IOSMinimalWindow.Sound => Sound;
            IntPtr IHandle.Handle => Handle;
            string IID<string>.ID => ID;
            int IPoint.X => X;
            int IPoint.Y => Y;
            int ISize.Width => Width;
            int ISize.Height => Height;

            string ITextDisplayer.Text
            {
                get => title;
                set
                {
                    if (title == value)
                        return;
                    title = value;
                    SdlAPI.SetWindowTitle(Handle, title);
                }
            }
            string ITextHolder.Text => title;
            bool IFocus.Focused
            {
                get
                {
                    var f = SdlAPI.GetWindowFlags(Handle);
                    return ((f & WindowFlags.InputFocus) == WindowFlags.InputFocus);
                }
            }
            Location ILocationHolder.Location
            {
                get => new Location(X, Y);
                set
                {
                    SdlAPI.SetWindowPosition(Handle, X, Y);
                    SdlAPI.GetWindowPosition(Handle, out X, out Y);
                }
            }
            bool IOSWindow.IsDisposed => Handle == IntPtr.Zero;

            ModifierKeys IExOSWindow.ModifierKey { get => ModifierKey; set => ModifierKey = value; }
            MouseButton IExOSWindow.MouseButton { get => MouseButton; set => MouseButton = value; }
            ModifierKeys IInputStateHolder.ModifierKey { get => ModifierKey; }
            MouseButton IInputStateHolder.MouseButton { get => MouseButton; }
            IRenderTarget IRenderTargetHolder.Target => Target;
            int IEventPoller.BitsPerPixel => Screen.BitsPerPixel;
            ISize IEventPoller.WorkingSize => (ScreenSize);
            bool IFocusable.Focusable => Handle != IntPtr.Zero;
            #endregion

            #region CONSTRUCTOR
            bool IExOSWindow.PsuedoConstructor(IRenderWindow window, out GwsWindowFlags Flags,
                string title, int? width, int? height, int? x, int? y, GwsWindowFlags? flags, RendererFlags? renderFlags)
            {
                MemoryOccupation = new MemoryOccupation();
                MemoryOccupation.BeginMonitoring(out long total);
                Flags = flags ?? 0;

                windowFlags = GetFlags(Flags);

                try
                {
                    if ((windowFlags & WindowFlags.Shown) != WindowFlags.Shown)
                        windowFlags |= WindowFlags.Hidden;
                    if ((windowFlags & WindowFlags.NoBorders) == WindowFlags.NoBorders)
                        windowFlags |= WindowFlags.Resizable;

                    //windowFlags |= WindowFlags.Maximized;
                    Screen = SdlAPI.Primary;
                    ScreenSize = new Size(Screen);

                    int px, py;
                    int wd = width ?? defaultWidth;
                    int ht = height ?? defaultHeight;


                    if ((windowFlags & WindowFlags.FullScreen) == WindowFlags.FullScreen ||
                        (windowFlags & WindowFlags.FullScreenDesktop) == WindowFlags.FullScreenDesktop ||
                        (windowFlags & WindowFlags.Maximized) == WindowFlags.Maximized)
                    {
                        windowFlags &=
                        ~(
                            WindowFlags.FullScreen
                            | WindowFlags.FullScreenDesktop
                            | WindowFlags.Resizable
                            | WindowFlags.Maximized
                            | WindowFlags.Minimized
                        );

                        px = 1; py = 30;
                        wd = Screen.Width;
                        ht = Screen.Height - 30;
                    }
                    else
                    {
                        px = x == null ? (Screen.Width - wd) / 2  : x.Value;
                        py = y == null ? (Screen.Height - ht) / 2 : y.Value;

                        if (px < 0)
                            px = -px;
                        if (py < 0)
                            py = -py;

                        if (py < 30)
                            py = 30;
                        if (px < 1)
                            px = 1;
                        
                        if(py + ht == Screen.Height - 40)
                        {
                            py -= 10;
                        }
                    }

                    Handle = SdlAPI.CreateWindow(title, px, py, wd, ht, (int)windowFlags);

                    SdlAPI.GetWindowSize(Handle, out int w, out int h);
                    SdlAPI.GetWindowPosition(Handle, out int _x, out int _y);

                    X = _x;
                    Y = _y;
                    Width = w;
                    Height = h;

                    SdlAPI.SetHintWithPriority("SDL_HINT_RENDER_VSYNC", "0", SdlHintPriority.HIGH);
                    //_Factory.SetHint("DL_FRAMEBUFFER_ACCELERATION", "1");
                    this.title = title ?? "SDL Window";

                    if ((windowFlags & WindowFlags.OpenGL) == WindowFlags.OpenGL)
                        GLContext = new GLContext(this);

                    DesktopID = SdlAPI.WindowID(Handle);
                    ID = "Window" + DesktopID;
                    Target = new SdlTarget(window);
                    Sound = new SdlSound();
                    MemoryOccupation.EndMonitoring(total);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            #endregion

            #region BRING TO FRONT - SEND TO BACK
            void IOverlap.BringToFront()
            {
                var window = Application.GetWindow<IDesktop>((x) => x.Focused);
                SdlAPI.RaiseWindow(Handle);
                if (window != null)
                {
                    if (!window.Equals(this))
                        window.Focus();
                }
            }
            void IOverlap.SendToBack()
            {
                var RC = new Rectangle(X, Y, Width, Height);
                var window = Application.GetWindow<IDesktop>((x) => new Rectangle(x.X, x.Y, x.Width, x.Height).Intersects(RC));
                window?.Focus();
            }
            #endregion

            #region FOCUS
            bool IFocusable.Focus()
            {
                SdlAPI.RaiseWindow(Handle);
                return true;
            }
            #endregion

            #region INPUT PROCESSING
            IEventArgs IEventParser.ParseEvent(IExternalEventInfo @event)
            {
                if (!(@event is Event))
                    return null;
                Event ev = (Event)@event;
                IEventArgs e = null;

                switch (ev.Type)
                {
                    case EventType.WINDOWEVENT:
                        e = processWindowEvent(ev.Window);
                        break;
                    case EventType.KEYPRESS:
                        e = processTextInputEvent(ev.TextInputEvent);
                        break;
                    case EventType.KEYDOWN:
                        e = processKeyEvent(ev);
                        break;
                    case EventType.KEYUP:
                        e = processKeyEvent(ev);
                        break;
                    case EventType.MOUSEBUTTONDOWN:
                    case EventType.MOUSEBUTTONUP:
                        e = processMouseButtonEvent(ev.MouseButtonEvent);
                        break;
                    case EventType.MOUSEMOTION:
                        e = processMouseMotionEvent(ev.MouseMotionEvent);
                        break;
                    case EventType.MOUSEWHEEL:
                        e = processMouseWheelEvent(ev.MouseWheelEvent);
                        break;
                    case EventType.DROPFILE:
                        e = processDropEvent(ev.DropEvent);
                        SdlAPI.Free(ev.DropEvent.File);
                        break;
                    default:
                        break;
                }
                return e;
            }
            #endregion

            #region PROPCESS WINDOW EVENT
            IEventArgs processWindowEvent(WinEvent e)
            {
                int x, y;
                var type = (WindowEvent)e.Event;

                switch (type)
                {
                    case WindowEvent.Close:
                        (Target.RenderWindow as IDisposable)?.Dispose();
                        return null;

                    case WindowEvent.MouseEnter:
                    case WindowEvent.MouseLeave:
                        MouseEnterLeaveArgs.Status = type == WindowEvent.MouseEnter? MouseState.Enter: MouseState.Leave;
                        MouseEnterLeaveArgs.Button = 0;
                        SdlAPI.GetCursorPosition(out x, out y);
                        MouseEnterLeaveArgs.X = x;
                        MouseEnterLeaveArgs.Y = y;
                        return MouseEnterLeaveArgs;

                    case WindowEvent.Exposed:
                        return null;

                    case WindowEvent.GotFocus:
                        return CancelArgs;

                    case WindowEvent.LostFocus:
                        return CancelArgs;

                    case WindowEvent.Hidden:
                        return Factory.DefaultArgs;

                    case WindowEvent.Shown:
                        return Factory.DefaultArgs;

                    case WindowEvent.Moved:
                        SdlAPI.GetWindowPosition(Handle, out x, out y);
                        X = x;
                        Y = y;
                        return Factory.DefaultArgs;

                    case WindowEvent.Resized:
                        var sizeArg = new SizeEventArgs(e.Data1, e.Data2);
                        return sizeArg;
                    case WindowEvent.Maximized:
                        return Factory.DefaultArgs;

                    case WindowEvent.MiniMized:
                        return Factory.DefaultArgs;

                    case WindowEvent.Restored:
                        return Factory.DefaultArgs;
                    default:
                        return null;
                }
            }
            #endregion

            #region PROPCESS KEY EVENT
            IEventArgs processKeyEvent(Event ev)
            {
                KeyState KeyState;
                IExKeyEventArgs e;
                if (ev.KeyboardEvent.State == InputState.Pressed)
                {
                    KeyState = KeyState.Down;
                    e = KeyDownArgs;
                }
                else
                {
                    KeyState = KeyState.Up;
                    e = KeyUpArgs;
                }
                var KeyCode = ToKeyCode((int)ev.KeyboardEvent.Keysym.Scancode);
                KeyState.ResetModifierKeys(KeyCode, ref ModifierKey);
                e.KeyCode = KeyCode;
                e.ModifierKeys = ModifierKey;
                e.State = KeyState;
                return e;
            }
            #endregion

            #region PROCESS TEXT INPUT EVENT
            unsafe IEventArgs processTextInputEvent(TextInputEvent ev)
            {
                KeyPressArgs.KeyChar = (char)ev.Text[0];
                return KeyPressArgs;
            }
            #endregion

            #region PROCESS MOUSE BUTTON EVENT
            IEventArgs processMouseButtonEvent(MouseButtonEvent ev)
            {
                MouseButton button = ToMouseButton((int)ev.Button);
                if (ev.Type == EventType.MOUSEBUTTONDOWN)
                {
                    var e = MouseDownArgs;
                    e.Button = button;
                    e.X = ev.X;
                    e.Y = ev.Y;
                    e.Status = MouseState.Down;
                    MouseButton = button;
                    return e;
                }
                else
                {
                    var e = MouseUpArgs;
                    e.Button = button;
                    e.X = ev.X;
                    e.Y = ev.Y;
                    e.Status = MouseState.Up;
                    MouseButton = 0;
                    return e;
                }
            }
            #endregion

            #region PROCESS MOUSE MOTION EVENT
            IEventArgs processMouseMotionEvent(MouseMotionEvent ev)
            {
                var e = MouseMoveArgs;
                _MouseButton Button;

                switch (ev.State)
                {
                    case ButtonFlags.Left:
                        Button = _MouseButton.Left;
                        break;
                    case ButtonFlags.Middle:
                        Button = _MouseButton.Middle;
                        break;
                    case ButtonFlags.Right:
                        Button = _MouseButton.Right;
                        break;
                    case ButtonFlags.X1:
                        Button = _MouseButton.X1;
                        break;
                    case ButtonFlags.X2:
                        Button = _MouseButton.X2;
                        break;
                    default:
                        Button = 0;
                        break;
                }
                MouseButton button = ToMouseButton((int)Button);

                e.X = ev.X;
                e.Y = ev.Y;
                MouseWheelArgs.X = ev.X;
                MouseWheelArgs.Y = ev.Y;
                e.Button = button;
                e.Status = MouseState.Move;
                return e;
            }
            #endregion

            #region PROCESS MOUSE WHEEL EVENT
            IEventArgs processMouseWheelEvent(MouseWheelEvent ev)
            {
                var e = MouseWheelArgs;
                e.XMove = ev.X;
                e.YMove = ev.Y;
                if (ev.X == 0 && ev.Y == 0)
                    return null;
                return e;
            }
            #endregion

            #region PROCESS FILE DROP EVENT
            IEventArgs processDropEvent(DropEvent ev)
            {
                var e = FileDropArgs;
                FileDropArgs.FileName = Marshal.PtrToStringUni(ev.File);
                return null;
                //OnFileDrop(e);
            }
            #endregion

            #region CHANGE SCREEN
            void IOSMinimalWindow.ChangeScreen(int screenIndex) { }
            #endregion

            #region SHOW - HIDE
            void IShowable.Show()
            {
                windowFlags |= WindowFlags.Shown;
                windowFlags = windowFlags & ~WindowFlags.Hidden;

                if ((windowFlags & WindowFlags.FullScreen) == WindowFlags.FullScreen)
                    SdlAPI.MaximizeWindow(Handle);
                else
                    SdlAPI.ShowWindow(Handle);
            }
            void IHideable.Hide(bool forceFully)
            {
                SdlAPI.HideWindow(Handle);
                windowFlags |= WindowFlags.Hidden;
                windowFlags = windowFlags & ~WindowFlags.Shown;
            }
            #endregion

            #region SHOW MESSAGE / INPUT BOX
            MsgBoxResult IMessageBoxHost.ShowMessageBox(string title, int x, int y, string text, MsgBoxButtons buttons)
            {
                //var data = new MessageBoxData();
                //data.window = handle;
                //data.title = title;
                //data.message = text;
                //int numButtons;
                //switch (buttons)
                //{
                //    case MsgBoxButtons.YesNo:
                //    default:
                //        numButtons = 2;
                //        break;
                //    case MsgBoxButtons.OkCancel:
                //        numButtons = 2;
                //        break;
                //    case MsgBoxButtons.Information:
                //        numButtons = 1;
                //        break;
                //    case MsgBoxButtons.Error:
                //        numButtons = 1;
                //        break;
                //    case MsgBoxButtons.AbortRetryIgnore:

                //        numButtons = 3;
                //        break;
                //}
                //data.numbuttons = numButtons;
                //MessageBoxButtonData[] Buttons =new MessageBoxButtonData[numButtons];
                //switch (buttons)
                //{
                //    case MsgBoxButtons.YesNo:
                //    default:
                //        Buttons[0]=new MessageBoxButtonData();
                //        Buttons[0].text = "YES";
                //        Buttons[1] = new MessageBoxButtonData();
                //        Buttons[1].text = "NO";
                //        break;
                //    case MsgBoxButtons.OkCancel:
                //        Buttons[0] = new MessageBoxButtonData();
                //        Buttons[0].text = "OK";
                //        Buttons[1] = new MessageBoxButtonData();
                //        Buttons[1].text = "CANCEL";
                //        break;
                //    case MsgBoxButtons.Information:
                //        Buttons[0] = new MessageBoxButtonData();
                //        Buttons[0].text = "OK";
                //        break;
                //    case MsgBoxButtons.Error:
                //        Buttons[0] = new MessageBoxButtonData();
                //        Buttons[0].text = "OK";
                //        break;
                //    case MsgBoxButtons.AbortRetryIgnore:
                //        Buttons[0] = new MessageBoxButtonData();
                //        Buttons[0].text = "ABORT";
                //        Buttons[1] = new MessageBoxButtonData();
                //        Buttons[1].text = "RETRY";
                //        Buttons[2] = new MessageBoxButtonData();
                //        Buttons[2].text = "CANCEL;";
                //        break;
                //}
                //for (int i = 0; i < numButtons; i++)
                //{
                //    Buttons[i].buttonid = i;
                //}

                //data.buttons = Buttons;

                //int id = 0;
                //FactoryBase.ShowMessageBox(data, ref id);

                //switch (id)
                //{
                //    case 0:
                //    default:
                //        switch (buttons)
                //        {
                //            case MsgBoxButtons.YesNo:
                //            default:
                //                result = MsgBoxResult.Yes;
                //                break;
                //            case MsgBoxButtons.OkCancel:
                //                result = MsgBoxResult.Ok;
                //                break;
                //            case MsgBoxButtons.Information:
                //                result = MsgBoxResult.Ok;
                //                break;
                //            case MsgBoxButtons.Error:
                //                result = MsgBoxResult.Ok;
                //                break;
                //            case MsgBoxButtons.AbortRetryIgnore:
                //                result = MsgBoxResult.Abort;
                //                break;
                //        }
                //        break;
                //    case 1:
                //        switch (buttons)
                //        {
                //            case MsgBoxButtons.YesNo:
                //            default:
                //                result = MsgBoxResult.No;
                //                break;
                //            case MsgBoxButtons.OkCancel:
                //                result = MsgBoxResult.Cancel;
                //                break;
                //            case MsgBoxButtons.AbortRetryIgnore:
                //                result = MsgBoxResult.Retry;
                //                break;
                //        }
                //        break;
                //    case 2:
                //        switch (buttons)
                //        {
                //            case MsgBoxButtons.YesNo:
                //            default:
                //                result = MsgBoxResult.No;
                //                break;
                //            case MsgBoxButtons.AbortRetryIgnore:
                //                result = MsgBoxResult.Ignore;
                //                break;
                //        }
                //        break;
                //}
                return 0;
            }
            Lot<MsgBoxResult, string> IMessageBoxHost.ShowInputBox(string title, int x, int y, string text, MsgBoxButtons buttons)
            {
                return Lot.Create<MsgBoxResult, string>();
            }
            #endregion

            #region GET - SET CURSOR POS
            void ICursorManager.GetCursorPos(out int x, out int y, bool global)
            {
                if (global)
                {
                    SdlAPI.GetCursorPositionGlobal(out x, out y);
                    return;
                }
                SdlAPI.GetCursorPosition(out x, out y);
            }

            /// <summary>
            /// <summary>
            /// Moves cursor location to a given x and y coordinates.        
            /// /// </summary>
            /// <param name="x">X coordinate to set mouse for</param>
            /// <param name="y">Y coordinate to set mouse for</param>
            void ICursorManager.SetCursorPos(int x, int y, bool global)
            {
                if (global)
                {
                    SdlAPI.SetCursorPosition(x, y);
                    return;
                }
                SdlAPI.SetCursorPosition(Handle, x, y);
            }

            void ICursorManager.SetCursorType(CursorType cursor)
            {
                int systemCursorID;
                switch (cursor)
                {
                    case CursorType.Arrow:
                    case CursorType.Default:
                        systemCursorID = 0;//SDL_SYSTEM_CURSOR_ARROW;
                        break;
                    case CursorType.IBeam:
                        systemCursorID = 1;// SystemCursor.SDL_SYSTEM_CURSOR_IBEAM;
                        break;
                    case CursorType.WaitCursor:
                        systemCursorID = 2;//SystemCursor.SDL_SYSTEM_CURSOR_WAIT;
                        break;
                    case CursorType.Cross:
                        systemCursorID = 3;//SDL_SYSTEM_CURSOR_CROSSHAIR;
                        break;
                    case CursorType.SizeNWSE:
                        systemCursorID = 5;// SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE;
                        break;
                    case CursorType.SizeNESW:
                        systemCursorID = 6;// SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW;
                        break;
                    case CursorType.SizeWE:
                        systemCursorID = 7;//SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE;
                        break;
                    case CursorType.SizeNS:
                        systemCursorID = 8;//SystemCursor.SDL_SYSTEM_CURSOR_SIZENS;
                        break;
                    case CursorType.SizeAll:
                        systemCursorID = 9;//SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL;
                        break;
                    case CursorType.No:
                        systemCursorID = 10;// SystemCursor.SDL_SYSTEM_CURSOR_NO;
                        break;
                    case CursorType.Hand:
                        systemCursorID = 11;// SystemCursor.SDL_SYSTEM_CURSOR_HAND;
                        break;
                    default:
                        systemCursorID = (int)cursor;
                        break;
                }
                var c = SdlAPI.CreateSystemCursor(systemCursorID);
                SdlAPI.SetWindowCursor(c);
            }

            #endregion

            #region NEW TEXTURE
            ITexture IOSMinimalWindow.newTexture(int? w, int? h, bool isPrimary, TextureAccess? textureAccess, RendererFlags? rendererFlags)
            {
                return new SdlTexture(Target.RenderWindow, w, h, isPrimary, SdlAPI.pixelFormat, textureAccess, rendererFlags);
            }
            #endregion

            #region NEW WAV PLAYER
            ISound IOSMinimalWindow.newWavPlayer() =>
                new SdlSound();
            #endregion

            #region POLL EVENT
            bool IEventPoller.PollEvent(out IExternalEventInfo e)
            {
                Event ev;
                var i = SdlAPI.PollEventEx(out ev);
                e = ev;
                return i != 0;
            }
            #endregion

            #region TO KEY CODE
            Key ToKeyCode(int scanCode)
            {
                Scancode s = (Scancode)scanCode;
                Key key;
                switch (s)
                {
                    case Scancode.ESCAPE:
                        key = Key.Escape;

                        // Function keys
                        break;
                    case Scancode.F1:
                        key = Key.F1;
                        break;
                    case Scancode.F2:
                        key = Key.F2;
                        break;
                    case Scancode.F3:
                        key = Key.F3;
                        break;
                    case Scancode.F4:
                        key = Key.F4;
                        break;
                    case Scancode.F5:
                        key = Key.F5;
                        break;
                    case Scancode.F6:
                        key = Key.F6;
                        break;
                    case Scancode.F7:
                        key = Key.F7;
                        break;
                    case Scancode.F8:
                        key = Key.F8;
                        break;
                    case Scancode.F9:
                        key = Key.F9;
                        break;
                    case Scancode.F10:
                        key = Key.F10;
                        break;
                    case Scancode.F11:
                        key = Key.F11;
                        break;
                    case Scancode.F12:
                        key = Key.F12;
                        break;
                    case Scancode.F13:
                        key = Key.F13;
                        break;
                    case Scancode.F14:
                        key = Key.F14;
                        break;
                    case Scancode.F15:
                        key = Key.F15;
                        break;
                    case Scancode.F16:
                        key = Key.F16;
                        break;
                    case Scancode.F17:
                        key = Key.F17;
                        break;
                    case Scancode.F18:
                        key = Key.F18;
                        break;
                    case Scancode.F19:
                        key = Key.F19;
                        break;
                    case Scancode.F20:
                        key = Key.F20;
                        break;
                    case Scancode.F21:
                        key = Key.F21;
                        break;
                    case Scancode.F22:
                        key = Key.F22;
                        break;
                    case Scancode.F23:
                        key = Key.F23;
                        break;
                    case Scancode.F24:
                        key = Key.F24;

                        // Number keys (0-9)
                        break;
                    case Scancode.Num0:
                        key = Key.NumPad0;
                        break;
                    case Scancode.Num1:
                        key = Key.NumPad1;
                        break;
                    case Scancode.Num2:
                        key = Key.NumPad2;
                        break;
                    case Scancode.Num3:
                        key = Key.NumPad3;
                        break;
                    case Scancode.Num4:
                        key = Key.NumPad4;
                        break;
                    case Scancode.Num5:
                        key = Key.NumPad5;
                        break;
                    case Scancode.Num6:
                        key = Key.NumPad6;
                        break;
                    case Scancode.Num7:
                        key = Key.NumPad7;
                        break;
                    case Scancode.Num8:
                        key = Key.NumPad8;
                        break;
                    case Scancode.Num9:
                        key = Key.NumPad9;

                        // Letters (A-Z)
                        break;
                    case Scancode.A:
                        key = Key.A;
                        break;
                    case Scancode.B:
                        key = Key.B;
                        break;
                    case Scancode.C:
                        key = Key.C;
                        break;
                    case Scancode.D:
                        key = Key.D;
                        break;
                    case Scancode.E:
                        key = Key.E;
                        break;
                    case Scancode.F:
                        key = Key.F;
                        break;
                    case Scancode.G:
                        key = Key.G;
                        break;
                    case Scancode.H:
                        key = Key.H;
                        break;
                    case Scancode.I:
                        key = Key.I;
                        break;
                    case Scancode.J:
                        key = Key.J;
                        break;
                    case Scancode.K:
                        key = Key.K;
                        break;
                    case Scancode.L:
                        key = Key.L;
                        break;
                    case Scancode.M:
                        key = Key.M;
                        break;
                    case Scancode.N:
                        key = Key.N;
                        break;
                    case Scancode.O:
                        key = Key.O;
                        break;
                    case Scancode.P:
                        key = Key.P;
                        break;
                    case Scancode.Q:
                        key = Key.Q;
                        break;
                    case Scancode.R:
                        key = Key.R;
                        break;
                    case Scancode.S:
                        key = Key.S;
                        break;
                    case Scancode.T:
                        key = Key.T;
                        break;
                    case Scancode.U:
                        key = Key.U;
                        break;
                    case Scancode.V:
                        key = Key.V;
                        break;
                    case Scancode.W:
                        key = Key.W;
                        break;
                    case Scancode.X:
                        key = Key.X;
                        break;
                    case Scancode.Y:
                        key = Key.Y;
                        break;
                    case Scancode.Z:
                        key = Key.Z;

                        break;
                    case Scancode.TAB:
                        key = Key.Tab;
                        break;
                    case Scancode.CAPSLOCK:
                        key = Key.CapsLock;
                        break;
                    case Scancode.LCTRL:
                        key = Key.LControlKey;
                        break;
                    case Scancode.LSHIFT:
                        key = Key.LShiftKey;
                        break;
                    case Scancode.LALT:
                        key = Key.LAlt;
                        break;
                    case Scancode.MENU:
                        key = Key.Menu;
                        break;
                    case Scancode.LGUI:
                        key = Key.LWin;
                        break;
                    case Scancode.RGUI:
                        key = Key.RWin;
                        break;
                    case Scancode.SPACE:
                        key = Key.Space;
                        break;
                    case Scancode.RALT:
                        key = Key.RALT;
                        //break; case Code.:
                        //    key = Key.WinRight;
                        break;
                    case Scancode.APPLICATION:
                        key = Key.Menu;
                        break;
                    case Scancode.RCTRL:
                        key = Key.RControlKey;
                        break;
                    case Scancode.RSHIFT:
                        key = Key.RShiftKey;
                        break;
                    case Scancode.RETURN:
                        key = Key.Enter;
                        break;
                    case Scancode.BACKSPACE:
                        key = Key.Back;

                        break;
                    case Scancode.SEMICOLON:
                        key = Key.OemSemicolon;      // Varies by keyboard: key = ;: on Win2K/US
                        break;
                    case Scancode.SLASH:
                        key = Key.OemSlash;          // Varies by keyboard: key = /? on Win2K/US
                        break;
                    case Scancode.GRAVE:
                        key = Key.OemTilde;          // Varies by keyboard: key = `~ on Win2K/US
                        break;
                    case Scancode.LEFTBRACKET:
                        key = Key.OemOpenBrackets;    // Varies by keyboard: key = [{ on Win2K/US
                        break;
                    case Scancode.BACKSLASH:
                        key = Key.OemBackslash;      // Varies by keyboard: key = \| on Win2K/US
                        break;
                    case Scancode.RIGHTBRACKET:
                        key = Key.OemCloseBrackets;   // Varies by keyboard: key = ]} on Win2K/US
                        break;
                    case Scancode.APOSTROPHE:
                        key = Key.OemQuotes;          // Varies by keyboard: key = '" on Win2K/US
                        break;
                    case Scancode.EQUALS:
                        key = Key.Oemplus;
                        break;
                    case Scancode.COMMA:
                        key = Key.Oemcomma;     // Invariant: : key =
                        break;
                    case Scancode.MINUS:
                        key = Key.OemMinus;     // Invariant: -
                        break;
                    case Scancode.PERIOD:
                        key = Key.OemPeriod;    // Invariant: .

                        break;
                    case Scancode.HOME:
                        key = Key.Home;
                        break;
                    case Scancode.END:
                        key = Key.End;
                        break;
                    case Scancode.DELETE:
                        key = Key.Delete;
                        break;
                    case Scancode.PAGEUP:
                        key = Key.PageUp;
                        break;
                    case Scancode.PAGEDOWN:
                        key = Key.PageDown;
                        break;
                    case Scancode.NUMLOCKCLEAR:
                        key = Key.NumLock;

                        break;
                    case Scancode.SCROLLLOCK:
                        key = Key.Scroll;
                        break;
                    case Scancode.PRINTSCREEN:
                        key = Key.PrintScreen;
                        break;
                    case Scancode.CLEAR:
                        key = Key.Clear;
                        break;
                    case Scancode.INSERT:
                        key = Key.Insert;

                        break;
                    case Scancode.SLEEP:
                        key = Key.Sleep;
                        break;
                    case Scancode.KP_0:
                        key = Key.D0;
                        break;
                    case Scancode.KP_1:
                        key = Key.D1;
                        break;
                    case Scancode.KP_2:
                        key = Key.D2;
                        break;
                    case Scancode.KP_3:
                        key = Key.D3;
                        break;
                    case Scancode.KP_4:
                        key = Key.D4;
                        break;
                    case Scancode.KP_5:
                        key = Key.D5;
                        break;
                    case Scancode.KP_6:
                        key = Key.D6;
                        break;
                    case Scancode.KP_7:
                        key = Key.D7;
                        break;
                    case Scancode.KP_8:
                        key = Key.D8;
                        break;
                    case Scancode.KP_9:
                        key = Key.D9;

                        break;
                    case Scancode.KP_DECIMAL:
                        key = Key.Decimal;
                        break;
                    case Scancode.KP_PLUS:
                        key = Key.Add;
                        break;
                    case Scancode.KP_MINUS:
                        key = Key.OemMinus;
                        break;
                    case Scancode.KP_DIVIDE:
                        key = Key.Divide;
                        break;
                    case Scancode.KP_MULTIPLY:
                        key = Key.Multiply;
                        break;
                    case Scancode.KP_ENTER:
                        key = Key.Enter;

                        // Navigation
                        break;
                    case Scancode.UP:
                        key = Key.Up;
                        break;
                    case Scancode.DOWN:
                        key = Key.Down;
                        break;
                    case Scancode.LEFT:
                        key = Key.Left;
                        break;
                    case Scancode.RIGHT:
                        key = Key.Right;
                        break;
                    case Scancode.MEDIASELECT:
                        key = Key.SelectMedia;
                        break;
                    case Scancode.VOLUMEDOWN:
                        key = Key.VolumeDown;
                        break;
                    case Scancode.VOLUMEUP:
                        key = Key.VolumeUp;
                        break;
                    case Scancode.AUDIOSTOP:
                        key = Key.MediaStop;
                        break;
                    case Scancode.AUDIOMUTE:
                        key = Key.VolumeMute;
                        break;
                    case Scancode.AUDIONEXT:
                        key = Key.MediaNextTrack;
                        break;
                    case Scancode.AUDIOPREV:
                        key = Key.MediaNextTrack;
                        break;
                    case Scancode.AUDIOPLAY:
                        key = Key.Play;
                        break;
                    case Scancode.PAUSE:
                        key = Key.MediaPlayPause;
                        break;
                    default:
                        key = Key.NoName;
                        break;
                }
                return key;
            }
            #endregion

            #region TO MOUSE BUTTON
            MouseButton ToMouseButton(int button)
            {
                _MouseButton b = (_MouseButton)button;
                MouseButton mouseButton;
                switch (b)
                {
                    case _MouseButton.Left:
                        mouseButton = MouseButton.Left;
                        break;
                    case _MouseButton.Middle:
                        mouseButton = MouseButton.Middle;
                        break;
                    case _MouseButton.Right:
                        mouseButton = MouseButton.Right;
                        break;
                    case _MouseButton.X1:
                        mouseButton = MouseButton.Button1;
                        break;
                    case _MouseButton.X2:
                        mouseButton = MouseButton.Button2;
                        break;
                    default:
                        mouseButton = 0;
                        break;
                }
                return mouseButton;
            }
            #endregion

            #region DISPOSE
            void IDisposable.Dispose()
            {
                Sound?.Dispose();
                Target?.Dispose();
                if (Handle != IntPtr.Zero)
                {
                    var renderer = SdlAPI.GetRenderer(Handle);
                    SdlAPI.DestroyRenderer(renderer);
                    SdlAPI.DestroyWindow(Handle);
                }
                Handle = IntPtr.Zero;
            }
            #endregion

            #region GET WINDOW FLAG
            static WindowFlags GetFlags(GwsWindowFlags flags)
            {
                WindowFlags flag = WindowFlags.Default;

                if ((flags & GwsWindowFlags.FullScreen) == GwsWindowFlags.FullScreen)
                    flag |= WindowFlags.FullScreen;

                if ((flags & GwsWindowFlags.Hidden) == GwsWindowFlags.Hidden)
                    flag |= WindowFlags.Hidden;

                if ((flags & GwsWindowFlags.HighDPI) == GwsWindowFlags.HighDPI)
                    flag |= WindowFlags.HighDPI;

                if ((flags & GwsWindowFlags.InputFocus) == GwsWindowFlags.InputFocus)
                    flag |= WindowFlags.InputFocus;

                if ((flags & GwsWindowFlags.Maximized) == GwsWindowFlags.Maximized)
                    flag |= WindowFlags.Maximized;

                if ((flags & GwsWindowFlags.Minimized) == GwsWindowFlags.Minimized)
                    flag |= WindowFlags.Minimized;

                if ((flags & GwsWindowFlags.MouseFocus) == GwsWindowFlags.MouseFocus)
                    flag |= WindowFlags.MouseFocus;

                if ((flags & GwsWindowFlags.NoBorders) == GwsWindowFlags.NoBorders)
                    flag |= WindowFlags.NoBorders;

                if ((flags & GwsWindowFlags.OpenGL) == GwsWindowFlags.OpenGL)
                    flag |= WindowFlags.OpenGL;

                if ((flags & GwsWindowFlags.Resizable) == GwsWindowFlags.Resizable)
                    flag |= WindowFlags.Resizable;

                if ((flags & GwsWindowFlags.Shown) == GwsWindowFlags.Shown)
                    flag |= WindowFlags.Shown;

                return flag;
            }
            #endregion

            #region TARGET CLASS
            sealed class SdlTarget : RenderTarget, IExRenderTarget
            {
                #region VARIABLES
                /// <summary>
                /// SDL surface object.
                /// </summary>
                IntPtr SurfaceHandle;

                /// <summary>
                /// SDL Surface object.
                /// </summary>
                SdlSurfaceInfo Surface;

                readonly string id;

                readonly object Sync = new object();
                #endregion

                #region CONSTRUCTORS
                internal SdlTarget(IRenderWindow window) :
                    base(window)
                {
                    SdlAPI.SetHint("SDL_HINT_FRAMEBUFFER_ACCELERATION", "X");
                    SurfaceHandle = SdlAPI.GetWindowSurface(window.Handle);
                    Surface = SurfaceHandle.ToObj<SdlSurfaceInfo>();
                    id = Application.NewID("RenderTaget");
                }
                #endregion

                #region PROPERTIES
                public override int Width => Surface.Width;
                public override int Height => Surface.Height;
                public override string ID => id;
                protected override IntPtr Source => Surface.Pixels;
                #endregion

                #region UPDATE  
                //[MethodImpl(MethodImplOptions.AggressiveInlining)]
                public override void Update(IBounds bounds, UpdateCommand command = UpdateCommand.Default)
                {
                    if ((RenderWindow.ViewState & ViewState.Disposed) == ViewState.Disposed ||
                        (command & UpdateCommand.SkipDisplayUpdate) == UpdateCommand.SkipDisplayUpdate)
                    {
                        return;
                    }
                    Rectangle rc;
                    if (bounds == null)
                        rc = new Rectangle(0, 0, Width, Height);
                    else
                    {
                        bounds.GetBounds(out int x, out int y, out int w, out int h);
                        --x;
                        --y;
                        ++w;
                        ++h;
                        if (x < 0)
                            x = 0;
                        if (y < 0)
                            y = 0;
                        rc = new Rectangle(x, y, w, h);
                    }
                    lock (Sync)
                    {
                        SdlAPI.UpdateWindow(RenderWindow.Handle, rc);
                    }
                }
                #endregion

                #region RESIZE
                //[MethodImpl(MethodImplOptions.AggressiveInlining)]
                protected override void ResizeInternal(int w, int h)
                {
                    SurfaceHandle = SdlAPI.GetWindowSurface(RenderWindow.Handle);
                    Surface = SurfaceHandle.ToObj<SdlSurfaceInfo>();
                }
                #endregion
                 
                #region DISPOSE
                public override void Dispose()
                {
                    base.Dispose();
                    SdlAPI.FreeSurface(SurfaceHandle);
                }
                #endregion
            }
            #endregion

            #region TEXTURE CLASS
            sealed class SdlTexture : ITexture, ITexture2, IExResizable
            {
                #region VARIABLES
                IntPtr Renderer;
                readonly IRenderWindow Window;
                volatile int width, height, length;
                uint PixelFormat;
                BlendMode mode;
                TextureAccess TextureAccess;
                RendererFlags RendererFlags;
                const byte o = 0;
                readonly IExBoundary Boundary = (IExBoundary)Factory.newBoundary();
                #endregion

                #region CONSTRUCTORS
                internal SdlTexture(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false,
                    uint? pixelformat = null, TextureAccess? textureAccess = null, RendererFlags? rendererFlags = null)
                {
                    RendererFlags = rendererFlags ?? 0;
                    Window = window;
                    IsPrimary = isPrimary;
                    width = w ?? Window.Width;
                    height = h ?? Window.Height;
                    Handle = CreateHandle(pixelformat ?? SdlAPI.pixelFormat, textureAccess, Width, Height, out Size s);
                    width = s.Width;
                    height = s.Height;
                    length = width * height;
                }
                #endregion

                #region PROPERTIES
                public bool IsPrimary { get; private set; }
                public IntPtr Handle { get; private set; }

                public int Width =>
                    width;
                public int Height =>
                    height;
                public BlendMode Mode
                {
                    get => mode;
                    set
                    {
                        mode = value;
                        SdlAPI.SetTextureBlendMod(Handle, value);
                    }
                }
                public FlipMode Flip { get; set; }

                public byte Alpha
                {
                    get => SdlAPI.GetTextureAlpha(Handle);
                    set => SdlAPI.SetTextureAlpha(Handle, value);
                }
                public int ColourMode
                {
                    get => SdlAPI.GetTextureColourMod(Handle);
                    set => SdlAPI.SetTextureColourMod(Handle, value);
                }
                public bool Valid => Width > 0 && Height > 0;
                public Size Size
                {
                    get => new Size(width, height);
                    set
                    {
                        if (!value)
                            return;
                        ((IExResizable)this).Resize(value.Width, value.Height, out _);
                    }
                }
                #endregion

                #region COPY FROM
                //[MethodImpl(MethodImplOptions.AggressiveInlining)]
                public unsafe void CopyFrom(ICopyable source, IBounds copyArea, CopyCommand command = 0, IPoint dstPoint = null, UpdateCommand updateCommand = 0)
                {
                    int x = 0, y = 0, w = source.Width, h = source.Height;
                    copyArea?.GetBounds(out x, out y, out w, out h);
                    int dstX = dstPoint?.X ?? x;
                    int dstY = dstPoint?.Y ?? y;
                    Blocks.CorrectRegion(ref x, ref y, ref w, ref h, source.Width, source.Height,
                        ref dstX, ref dstY, width, length, out _, out _);
                    var dstrc = new Rectangle(dstX, dstY, w, h);
                    var info = new IParameter[]
                    {
                        new Area(x,y,w,h),
                        command.Replace(),
                        new Point(dstX, dstY)
                    };
                    fixed (int* p = new int[w * h])
                    {
                        var dst = (IntPtr)p;
                        source.CopyTo(dst, w * h, w, info);
                        SdlAPI.UpdateTexture(Handle, dstrc, dst, w * 4);
                        Update(new UpdateArea(dstrc), updateCommand);
                    }
                }
                #endregion

                #region COPY TO
                public unsafe ISize CopyTo(IntPtr destination, int dstLen, int dstW,
                    IEnumerable<IParameter> parameters = null)
                {
                    int dstH = dstLen / dstW;

                    #region EXTRACT PARAMETRS
                    parameters.Extract(out IExSession session);
                    int dstX = session.UserPoint?.X ?? 0;
                    int dstY = session.UserPoint?.Y ?? 0;
                    int x = dstX;
                    int y = dstY;
                    int w = dstW;
                    int h = dstH;
                    #endregion

                    if(session.CopyArea!=null && session.CopyArea.Valid)
                        session.CopyArea.GetBounds(out x, out y, out w, out h);

                    Blocks.CorrectRegion(ref x, ref y, ref w, ref h, width, height,
                        ref dstX, ref dstY, dstW, dstLen, out _, out _);

#if Advance
                    if (session.Clip != null && session.Clip.Valid)
                    {
                        session.Clip.GetBounds(out int clipX, out int clipY, out int clipW, out int clipH);
                        int clipR = clipX + clipW;
                        int clipB = clipY + clipH;

                        if (dstX > clipR && dstY > clipB)
                            return ((ISize)GWS.Size.Empty);

                        if (dstY < clipY || dstY > clipB || dstX > clipR)
                            return ((ISize)GWS.Size.Empty);

                        var dstEX = dstX + w;
                        if (dstX < clipX)
                            dstX = clipX;
                        if (dstEX > clipR)
                            dstEX = clipR;
                        w = dstEX - dstX;
                    }
#endif
                    if (w < 0)
                        return ((ISize)GWS.Size.Empty);

                    var srcRc = new Rectangle(x, y, w, h);
                    int[] Data = new int[w * h];
                    var command = session.Command.ToEnum<CopyCommand>();
                    fixed (int* p = Data)
                    {
                        SdlAPI.SetRenderTarget(Renderer, Handle);
                        SdlAPI.ReadPixels(Renderer, srcRc, PixelFormat, p, w * 4);
                        Blocks.CopyBlock(p, new Rectangle(0, 0, w, h),
                            w * h, w, h, (int*)destination, dstX, dstY, dstW, dstLen, command);
                    }
                    return ((ISize)new Size(w, h));
                }
                #endregion

                #region RESIZE
                object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
                {
                    success = false;
                    if ((w == Width && h == Height) || (w == 0 && h == 0))
                        return this;
                    bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

                    if (SizeOnlyToFit && Width > w && Height > h)
                        return false;

                    if (SizeOnlyToFit)
                    {
                        if (w < Width)
                            w = Width;
                        if (h < Height)
                            h = Height;
                    }
                    if((resizeCommand & ResizeCommand.NewInstance) == ResizeCommand.NewInstance)
                    {
                        var texture = new SdlTexture(Window, width, height, false, PixelFormat, TextureAccess, RendererFlags);
                        success = true;
                        texture.CopyFrom(Window, new Rectangle(0, 0, texture.Width, texture.Height),0);
                        return texture;
                    }
                    DestoryTextureHandle(Handle);
                    Handle = CreateHandle(null, null, w, h, out Size s);
                    width = s.Width;
                    height = s.Height;
                    CopyFrom(Window, new Rectangle(0, 0, s.Width, s.Height), 0);
                    success = true;
                    return this;
                }
                #endregion

                #region LOCK - UNLOCK
                //void Lock(IBounds copyRc, out IntPtr textureData, out int lockedLength)
                //{
                //    if (locked)
                //        Unlock();

                //    textureData = IntPtr.Zero;
                //    lockedLength = 0;
                //    int texturePitch;
                //    Rectangle lockedArea;
                //    copyRc.GetBounds(out int x, out int y, out int w, out int h);

                //    if (w == 0 && h == 0)
                //    {
                //        NativeFactory.LockTexture(Handle, IntPtr.Zero, out textureData, out texturePitch);
                //        lockedArea = new Rectangle(0, 0, Width, Height);
                //    }
                //    else
                //    {
                //        lockedArea = Rects.CompitibleRc(Width, Height, x, y, w, h);
                //        if (lockedArea.Width == 0 || lockedArea.Height == 0)
                //            return;
                //        NativeFactory.LockTexture(Handle, lockedArea, out textureData, out texturePitch);
                //    }
                //    locked = true;
                //    lockedLength = lockedArea.Height * texturePitch;
                //}
                //void Unlock()
                //{
                //    if (!locked)
                //        return;
                //    NativeFactory.UnlockTexture(Handle);
                //}
                #endregion

                #region BIND - UNBIND
                public void Bind() =>
                    SdlAPI.SetRenderTarget(Renderer, Handle);
                public void Unbind() =>
                    SdlAPI.SetRenderTarget(Renderer, IntPtr.Zero);
                #endregion

                #region UPDATE
                //[MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Update(IBounds bounds, UpdateCommand command = 0)
                {
                    bool SuspendUpdate = (command & UpdateCommand.SkipDisplayUpdate) == UpdateCommand.SkipDisplayUpdate;
                    if (SuspendUpdate)
                    {
                        Boundary.Update(bounds);
                        return;
                    }
                    Rectangle rc;
                    if (bounds == null && Boundary.Valid)
                    {
                        rc = new Rectangle(Boundary);
                        Boundary.Update(Rectangle.Empty);
                        goto Display;
                    }
                    rc = new Rectangle(bounds);
                    Display:
                    SdlAPI.RendererCopyTexture(Renderer, Handle, rc, rc);
                    SdlAPI.UpdateRenderer(Renderer);
                }
                #endregion

                #region CREATE - DESTROY
                IntPtr CreateHandle(uint? format, TextureAccess? access, int w, int h, out Size size)
                {
                    Renderer = SdlAPI.GetRenderer(Window.Handle);
                    if (Renderer == IntPtr.Zero)
                        Renderer = SdlAPI.CreateRenderer(Window.Handle, -1, RendererFlags);
                    TextureAccess = access ?? TextureAccess.Streaming;
                    PixelFormat = format ?? SdlAPI.pixelFormat;
                    var handle = SdlAPI.CreateTexture(Renderer, PixelFormat, TextureAccess, w, h);
                    SdlAPI.QueryTexture(Handle, out PixelFormat, out TextureAccess, out w, out h);
                    size = new Size(w, h);
                    SdlAPI.SetTextureBlendMod(handle, BlendMode.None);
                    return handle;
                }
                void DestoryTextureHandle(IntPtr handle)
                {
                    if (handle != IntPtr.Zero)
                        SdlAPI.DestroyTexture(handle);
                }
                #endregion

                #region DISPOSE
                public void Dispose()
                {
                    DestoryTextureHandle(Handle);
                }
                #endregion
            }
            #endregion
        }
    }
}
#endif
