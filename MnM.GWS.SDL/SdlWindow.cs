/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if Window
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using static MnM.GWS.Application;

namespace MnM.GWS
{
#if AllHidden
    partial class SdlFactory
    {
#else
    public
#endif
        sealed class SdlWindow : _Window, IWindow
        {
            #region VARIABLES
            const int defaultWidth = 300;
            const int defaultHeight = 300;

            IntPtr handle;
            int ClickTimeStamp;
            bool disabled, visible;
            WindowFlags windowFlags;
            VectorF scale;

            // private Icon icon;
            //RendererFlags renderFlags;
            IntPtr sdlCursor = IntPtr.Zero;
            string title;
            bool fullScreen;
            bool isSingleThreaded;
            char[] DecodeTextBuffer = new char[32];
            float opacity = 1f;
            ISound sound;
            WindowState windowState, pWindowState;
            WindowBorder windowBorder = WindowBorder.Resizable;
            Size minSize, maxSize;
            bool Valid;
            bool firstShow;
            #endregion

            #region CONSTRUCTORS
            public SdlWindow(string title = null, int? width = null, int? height = null,
                int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null,
                RendererFlags? renderFlags = null) :
                base(title, width, height, x, y, flags, display, renderFlags)
            { }

            public SdlWindow(IExternalWindow control) :
                base(control)
            { }

            protected sealed override bool Initialize(string title = null, int? width = null, int? height = null,
                int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null,
                IExternalWindow externalWindow = null,  RendererFlags? renderFlags = null)
            {
                sound = Factory.newWavPlayer();
                mouseState.SetIsConnected(true);
                keyState.SetIsConnected(true);
                pMouseState.SetIsConnected(true);

                base.Screen = display ?? Factory.AvailableScreens.Primary;
                this.title = title ?? "MnM Window";
                var gwsWindowFlags = flags ?? 0;

                windowFlags = GetFlags(gwsWindowFlags);

                isSingleThreaded = gwsWindowFlags.HasFlag(GwsWindowFlags.MultiThread);

                SdlFactory.DisableScreenSaverEx();

                try
                {

                    if (!windowFlags.HasFlag(WindowFlags.Shown))
                        windowFlags |= WindowFlags.Hidden;
                    if (!windowFlags.HasFlag(WindowFlags.NoBorders))
                        windowFlags |= WindowFlags.Resizable;
                    //windowFlags |= WindowFlags.Maximized;

                    if (externalWindow != null)
                    {
                        handle = SdlFactory.CreateWindowFrom(externalWindow.Handle);
                    }
                    else
                    {
                        int px = Screen.Bounds.X + x ?? 0;
                        int py = Screen.Bounds.Y + y ?? 0;
                        int wd = width ?? defaultWidth;
                        int ht = height ?? defaultHeight;

                        if (windowFlags.HasFlag(WindowFlags.FullScreen) ||
                            windowFlags.HasFlag(WindowFlags.FullScreenDesktop) ||
                            windowFlags.HasFlag(WindowFlags.Maximized))
                        {
                            windowFlags = windowFlags & ~WindowFlags.FullScreen;
                            windowFlags = windowFlags & ~WindowFlags.FullScreenDesktop;
                            windowFlags = windowFlags & ~WindowFlags.Resizable;
                            windowFlags = windowFlags & ~WindowFlags.Maximized;


                            px = 0; py = 0;
                            //wd = Screen.Width - 30;
                            //ht = Screen.Height - 30;
                            fullScreen = true;
                        }
                        else
                        {
                            px = (Screen.Bounds.Width - wd) / 2 - px / 2;
                            py = (Screen.Bounds.Height - ht) / 2 - py / 2;
                        }

                        handle = SdlFactory.CreateWindow(title, px, py, wd, ht, (int)windowFlags);
                    }

                    Valid = true;
                    visible = windowFlags.HasFlag(WindowFlags.Shown) ? true : false;

                    SdlFactory.GetWindowSize(Handle, out int w, out int h);
                    SdlFactory.GetWindowPosition(Handle, out int _x, out int _y);
                    bounds = new Rectangle(_x, _y, w, h);

                    RendererFlags = renderFlags ?? RendererFlags.Accelarated;
                    return true;
                }
                catch
                {
                    Valid = false;
                    return false;
                }
            }
            #endregion

            #region PROPERTIES
            public sealed override IntPtr Handle => handle;
            public override bool Focused =>
                focused && !disabled && visible;
            public override bool Visible
            {
                get => visible;
                set => ChangeVisible(value);
            }
            public override bool Enabled
            {
                get => !disabled;
                set => disabled = !value;
            }
            public override float Transparency
            {
                get => 1f - opacity;
                set
                {
                    opacity = 1f - value;
                    SdlFactory.SetWindowOpacity(Handle, opacity);
                }
            }
            public override WindowState WindowState => windowState;
            public override WindowBorder WindowBorder => windowBorder;
            public override string Text
            {
                get
                {
                    if (Valid)
                        return title + "";
                    return string.Empty;
                }
                set
                {
                    if (Valid)
                    {
                        SdlFactory.SetWindowTitle(Handle, value);
                        title = value;
                    }
                }
            }
            public override VectorF Scale
            {
                get => scale;
                set
                {
                    var renderer = SdlFactory.GetRenderer(Handle);
                    SdlFactory.RenderSetScale(renderer, value.X, value.Y);
                    scale = value;
                }
            }
            public override bool CursorVisible
            {
                get => cursorVisible;
                set
                {
                    if (Valid && value != cursorVisible)
                    {
                        grabCursor(!value);
                        cursorVisible = value;
                    }
                }
            }
            public override uint PixelFormat => SdlFactory.pixelFormat;
            public CursorType Cursor { get; set; }
            public string ToolTipText { get; set; }
            public object Tag { get; set; }
            bool IsBeingClosed { get; set; }
            public override ISound Sound => sound;
            public override Size MinSize 
            {
                get => minSize;
                set
                {
                    minSize = value;
                    if (minSize)
                    {
                        if (Width < minSize.Width || Height < minSize.Height)
                        {
                            var Width = Math.Max(Bounds.Width, minSize.Width);
                            var Height = Math.Max(Bounds.Height, minSize.Height);
                            Resize(Width, Height);
                        }
                    }
                }
            }
            public override Size MaxSize
            {
                get => maxSize;
                set
                {
                    maxSize = value;
                    if (maxSize)
                    {
                        if (Width > maxSize.Width || Height > maxSize.Height)
                        {
                            var Width = Math.Min(Bounds.Width, maxSize.Width);
                            var Height = Math.Min(Bounds.Height, maxSize.Height);
                            Resize(Width, Height);
                        }
                    }
                }
            }
            #endregion

            #region FOCUS
            public bool Has(Vector p)
            {
                if (Transparency >= 1f)
                    return false;
                if (disabled)
                    return false;
                return Bounds.Contains(p.X, p.Y);
            }
            public override void BringToFront()
            {
                if (disabled || !Visible)
                    return;

                var window = GetWindow<IWindow>((x) => x.Focused);
                SdlFactory.RaiseWindow(Handle);
                if (window != null)
                {
                    if (!window.Equals(this))
                        window.Focus();
                }
            }
            public override void SendToBack()
            {
                var b = Bounds;
                var window = GetWindow<IWindow>((x) => x.Bounds.Intersects(b));
                window?.BringToFront();
            }
            public override void BringForward(int numberOfPlaces = 1)
            {
                //
            }
            public override void SendBackward(int numberOfPlaces = 1)
            {
                //
            }
            public override bool Focus()
            {
                if (disabled || !Visible)
                    return false;
                SdlFactory.RaiseWindow(Handle);
                return true;
            }
            #endregion

            #region CREATE GL CONTEXT
            //public GLContext CreateGLContext() =>
            //    GLContext.Create(this);
            #endregion

            #region SHOW HIDE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void ChangeVisible(bool value)
            {
                if (Valid)
                {
                    visible = value;

                    if (value)
                    {
                        windowFlags |= WindowFlags.Shown;
                        windowFlags = windowFlags & ~WindowFlags.Hidden;
                        if (fullScreen)
                            SdlFactory.MaximizeWindow(Handle);
                        else
                            SdlFactory.ShowWindow(Handle);

                        if (!firstShow)
                        {
                            OnLoad(Factory.EmptyArgs);
                            firstShow = true;
                        }
                    }
                    else
                    {
                        SdlFactory.HideWindow(Handle);
                        windowFlags |= WindowFlags.Hidden;
                        windowFlags = windowFlags & ~WindowFlags.Shown;
                    }
                }
                base.ChangeVisible(value);
            }
            #endregion

            #region CLOSE
            protected override void OnClosed(IEventArgs e)
            {
                Valid = false;
                sound?.Dispose();
                if (Handle != IntPtr.Zero)
                {
                    cursorVisible = true;
                    var renderer = SdlFactory.GetRenderer(Handle);
                    SdlFactory.DestroyRenderer(renderer);
                    SdlFactory.DestroyWindow(Handle);
                }
                handle = IntPtr.Zero;
                base.OnClosed(e);
            }
            #endregion

            #region RESIZE
            public override void Resize(int? width = null, int? height = null)
            {
                var w = width ?? Width;
                var h = height ?? Height;

                if ((w != Width || h != Height))
                {
                    SdlFactory.SetWindowSize(Handle, w, h);
                }
                base.Resize(Width, Height);
            }
            #endregion

            #region PRIVATE METHODS
            void grabCursor(bool grab)
            {
                SdlFactory.ShowCursor(!grab ? 0 : 1);
                SdlFactory.SetWindowGrab(Handle, grab);
                SdlFactory.SetRelativeMouseMode(grab);
                if (!grab)
                {
                    // Move the cursor to the current position
                    //  order to avoid a sudden jump when it
                    // becomes visible again
                    float scale = Width / (float)Height;
                    SdlFactory.WarpMouseInWindow(Handle, (int)Math.Round(mouseState.X / scale), (int)Math.Round(mouseState.Y / scale));
                }
            }
            #endregion

            #region CURSOR
            public override void SetCursor(int x, int y)
            {
                SdlFactory.ShowCursor(1);
                SdlFactory.SetCursorPosition(x, y);
            }
            public override void ShowCursor() =>
                SdlFactory.ShowCursor(1);
            public override void HideCursor() =>
                SdlFactory.ShowCursor(0);
            public override void ContainMouse(bool flag) =>
                SdlFactory.SetWindowGrab(Handle, flag);
            #endregion

            #region CHANGE
            public override void ChangeScreen(int screenIndex) { }
            public override void ChangeState(WindowState value)
            {
                if (windowState == value || !Valid)
                    return;

                switch (value)
                {
                    case WindowState.Maximized:
                        SdlFactory.MaximizeWindow(Handle);
                        windowState = WindowState.Maximized;
                        break;

                    case WindowState.Minimized:
                        SdlFactory.MinimizeWindow(Handle);
                        windowState = WindowState.Minimized;
                        break;

                    case WindowState.Normal:
                        SdlFactory.RestoreWindow(Handle);
                        break;
                }
                if (!CursorVisible)
                    grabCursor(true);
                OnWindowStateChanged(Factory.EmptyArgs);

            }
            public override void ChangeBorder(WindowBorder value)
            {
                if (WindowBorder == value || !Valid)
                    return;
                switch (value)
                {
                    case WindowBorder.Resizable:
                        SdlFactory.SetWindowBordered(Handle, true);
                        windowBorder = WindowBorder.Resizable;
                        break;

                    case WindowBorder.Hidden:
                        SdlFactory.SetWindowBordered(Handle, false);
                        windowBorder = WindowBorder.Hidden;
                        break;

                    case WindowBorder.Fixed:
                        break;
                }
                OnWindowBorderChanged(Factory.EmptyArgs);
            }
            public override void Move(int? x = null, int? y = null)
            {
                if (!Valid)
                    return;

                if (x == null && y == null)
                    return;
                var _x = x ?? X;
                var _y = y ?? Y;
                bounds = new Rectangle(_x, _y, bounds.Width, bounds.Height);
                SdlFactory.SetWindowPosition(Handle, X, Y);
            }
            #endregion

            #region EVENT ARGS
            readonly MouseButtonEventArgs mouseDownArgs = new MouseButtonEventArgs();
            readonly MouseButtonEventArgs mouseUpArgs = new MouseButtonEventArgs();
            readonly MouseMoveEventArgs mouseMoveArgs = new MouseMoveEventArgs();
            readonly MouseWheelEventArgs mouseWheelArgs = new MouseWheelEventArgs();

            readonly KeyEventArgs keyDownArgs = new KeyEventArgs();
            readonly KeyEventArgs keyUpArgs = new KeyEventArgs();
            readonly KeyPressEventArgs keyPressArgs = new KeyPressEventArgs((char)0);
            readonly TextInputEventArgs textInputEventArgs = new TextInputEventArgs();
            readonly FileDropEventArgs fileDropArgs = new FileDropEventArgs();

            Mouse mouseState = new Mouse();
            Keyboard keyState = new Keyboard();

            Mouse pMouseState = new Mouse();
            #endregion

            #region INPUT PROCESSING
            public bool CanProcessEvent =>
                !IsDisposed && Visible && Enabled && Valid;
            protected override IEventArgs ParseEvent(IEvent @event)
            {
                if (IsDisposed || !Visible || !Enabled || !Valid)
                    return null;

                if (!(@event is Event))
                    return null;
                Event ev = (Event)@event;

                switch (ev.Type)
                {
                    case EventType.WINDOWEVENT:
                        return processWindowEvent(ev.Window);
                    case EventType.TEXTINPUT:
                        return processTextInputEvent(ev.TextInputEvent);
                    case EventType.KEYDOWN:
                    case EventType.KEYUP:
                        return processKeyEvent(ev);
                    case EventType.MOUSEBUTTONDOWN:
                    case EventType.MOUSEBUTTONUP:
                        return processMouseButtonEvent(ev.MouseButtonEvent);
                    case EventType.MOUSEMOTION:
                        return processMouseMotionEvent(ev.MouseMotionEvent);
                    case EventType.MOUSEWHEEL:
                        return processMouseWheelEvent(ev.MouseWheelEvent);
                    case EventType.DROPFILE:
                        var e = processDropEvent(ev.DropEvent);
                        SdlFactory.Free(ev.DropEvent.File);
                        return e;
                    default:
                        return null;
                }
            }
            IEventArgs processMouseButtonEvent(MouseButtonEvent ev)
            {
                MouseButton button = MouseEventArgs.GetButton((int)ev.Button);
                if (ev.Type == EventType.MOUSEBUTTONDOWN)
                {
                    mouseState[button] = true;

                    var e = mouseDownArgs;
                    e.Button = button;
                    e.IsPressed = true;
                    e.Mouse = mouseState;
                    e.State = MouseState.Down;

                    return e;
                }
                else
                {
                    mouseState[button] = false;

                    var e = mouseUpArgs;
                    e.Button = button;
                    e.IsPressed = false;
                    e.Mouse = mouseState;
                    e.State = MouseState.Up;

                    return e;

                    if (ev.Clicks > 1)
                    {
                        e.State = MouseState.DoubleClick;
                        OnMouseDoubleClick(e);
                        ClickTimeStamp = (int)ev.Timestamp;
                    }

                    //window.MonitorDblClick = true;

                    //if (window.ClickTimeStamp == 0)
                    //    window.ClickTimeStamp = (int)ev.Timestamp;
                    //else if (window.ClickTimeStamp - ev.Timestamp < 500)
                    //    window.OnMouseDoubleClick(button, (int)ev.Timestamp);
                }
            }
            IEventArgs processKeyEvent(Event ev)
            {
                bool keyPressed = ev.KeyboardEvent.State == State.Pressed;
                var key = (SdlKeys)ev.KeyboardEvent.Keysym.Scancode;

                if (keyPressed)
                {
                    keyState.SetKeyState(key, true);

                    var e = keyDownArgs;
                    e.Keyboard = keyState;
                    e.KeyCode = key;
                    e.IsRepeat = ev.KeyboardEvent.Repeat > 0;
                    return e;
                }
                else
                {
                    keyState.SetKeyState(key, false);

                    var e = keyUpArgs;
                    e.Keyboard = keyState;
                    e.KeyCode = key;
                    e.IsRepeat = false;
                    return e;
                }
            }

            unsafe IEventArgs processTextInputEvent(TextInputEvent ev)
            {
                // Calculate the length of the typed text string
                int length;
                for (length = 0; length < TextInputEvent.TextSize && ev.Text[length] != '\0'; length++) { }

                // Make sure we have enough space to decode this string
                int decoded_length = Encoding.UTF8.GetCharCount(ev.Text, length);
                if (DecodeTextBuffer.Length < decoded_length)
                {
                    Array.Resize(
                        ref DecodeTextBuffer,
                        2 * Math.Max(decoded_length, DecodeTextBuffer.Length));
                }

                // Decode the string from UTF8 to .Net UTF16
                fixed (char* pBuffer = DecodeTextBuffer)
                {
                    decoded_length = System.Text.Encoding.UTF8.GetChars(
                        ev.Text,
                        length,
                        pBuffer,
                        DecodeTextBuffer.Length);
                }

                textInputEventArgs.Characters = new char[decoded_length];
                for (int i = 0; i < decoded_length; i++)
                {
                    textInputEventArgs.Characters[i] = DecodeTextBuffer[i];
                }
                return textInputEventArgs;
            }
            IEventArgs processMouseMotionEvent(MouseMotionEvent ev)
            {
                //float scale = w / (float)h;

                var x = ev.X - 0;// (int)Math.Round(ev.X);// * scale);
                var y = ev.Y - 0;// (int)Math.Round(ev.Y);// * scale);
                                 //GWS.Windows.MouseScale = scale;

                mouseState.X = x;
                mouseState.Y = y;

                var e = mouseMoveArgs;
                e.Mouse = mouseState;
                e.XDelta = mouseState.X - pMouseState.X;
                e.YDelta = mouseState.Y - pMouseState.Y;
                e.State = MouseState.Move;

                if (e.XDelta == 0 && e.YDelta == 0)
                {
                    return null;
                }

                pMouseState = mouseState;
                return (e);
            }
            IEventArgs processMouseWheelEvent(MouseWheelEvent ev)
            {
                mouseState.SetScrollRelative(ev.X, ev.Y);

                var e = mouseWheelArgs;
                e.Mouse = mouseState;
                e.DeltaPrecise = mouseState.Scroll.Yf - mouseState.Scroll.Yf;
                e.State = MouseState.Wheel;
                if (ev.X == 0 && ev.Y == 0)
                    return null;

                pMouseState = mouseState;
                return e;
            }
            IEventArgs processDropEvent(DropEvent ev)
            {
                var e = fileDropArgs;
                fileDropArgs.FileName = Marshal.PtrToStringUni(ev.File);
                return null;
                //OnFileDrop(e);
            }
            IEventArgs processWindowEvent(WindowEvent e)
            {
                switch (e.Event)
                {
                    case WindowEventID.Close:
                        Close();
                        return null;

                    case WindowEventID.Enter:
                        return mouseUpArgs;

                    case WindowEventID.Leave:
                        return mouseUpArgs;

                    case WindowEventID.Exposed:
                        return null;

                    case WindowEventID.FocusGained:
                        focused = true;
                        return Factory.EmptyArgs;

                    case WindowEventID.FocusLost:
                        focused = false;
                        return CancelEventArgs.Empty;

                    case WindowEventID.Hidden:
                        return Factory.EmptyArgs;

                    case WindowEventID.Shown:
                        return Factory.EmptyArgs;

                    case WindowEventID.Moved:
                        SdlFactory.GetWindowPosition(Handle, out int x, out int y);
                        bounds = new Rectangle(x, y, Bounds.Width, Bounds.Height);
                        return Factory.EmptyArgs;

                    case WindowEventID.Resized:
                        var sizeArg = new SizeEventArgs(e.Data1, e.Data2);
                        bounds = new Rectangle(Bounds.X, Bounds.Y, e.Data1,e.Data2);
                        return sizeArg;
                    case WindowEventID.Maximized:
                        windowState = WindowState.Maximized;
                        //window.Graphics.SyncOnResize(window);
                        return Factory.EmptyArgs;

                    case WindowEventID.MiniMized:
                        pWindowState = windowState;
                        windowState = WindowState.Minimized;
                        return Factory.EmptyArgs;

                    case WindowEventID.Restored:
                        windowState = pWindowState;
                        return Factory.EmptyArgs;
                    default:
                        return null;
                }
            }
            #endregion

            #region MISC
            /// <summary>
            /// Call this method to simulate KeyDown/KeyUp events
            /// on platforms that do not generate key events for
            /// modifier flags (e.g. Mac/Cocoa).
            /// Note: this method does not distinguish between the
            /// left and right variants of modifier keys.
            /// </summary>
            /// <param name="mods">Mods.</param>
            SdlKeys updateModifierFlags(SdlKeyModifiers mods, out bool modifierFound)
            {
                bool alt = (mods & SdlKeyModifiers.Alt) != 0;
                bool control = (mods & SdlKeyModifiers.Control) != 0;
                bool shift = (mods & SdlKeyModifiers.Shift) != 0;
                modifierFound = alt || control || shift;
                if (alt)
                {
                    return SdlKeys.AltLeft;
                }
                if (control)
                {
                    return (SdlKeys.ControlLeft);
                }

                if (shift)
                {
                    return (SdlKeys.ShiftLeft);
                }
                return 0;
            }

            static WindowFlags GetFlags(GwsWindowFlags flags)
            {
                WindowFlags flag = WindowFlags.Default;
                if (flags.HasFlag(GwsWindowFlags.Foreign))
                    flag |= WindowFlags.Foreign;

                if (flags.HasFlag(GwsWindowFlags.FullScreen))
                    flag |= WindowFlags.FullScreen;

                if (flags.HasFlag(GwsWindowFlags.FullScreenDesktop))
                    flag |= WindowFlags.FullScreenDesktop;

                if (flags.HasFlag(GwsWindowFlags.GrabInput))
                    flag |= WindowFlags.GrabInput;

                if (flags.HasFlag(GwsWindowFlags.Hidden))
                    flag |= WindowFlags.Hidden;

                if (flags.HasFlag(GwsWindowFlags.HighDPI))
                    flag |= WindowFlags.HighDPI;

                if (flags.HasFlag(GwsWindowFlags.InputFocus))
                    flag |= WindowFlags.InputFocus;

                if (flags.HasFlag(GwsWindowFlags.Maximized))
                    flag |= WindowFlags.Maximized;

                if (flags.HasFlag(GwsWindowFlags.Minimized))
                    flag |= WindowFlags.Minimized;

                if (flags.HasFlag(GwsWindowFlags.MouseFocus))
                    flag |= WindowFlags.MouseFocus;

                if (flags.HasFlag(GwsWindowFlags.NoBorders))
                    flag |= WindowFlags.NoBorders;

                if (flags.HasFlag(GwsWindowFlags.OpenGL))
                    flag |= WindowFlags.OpenGL;

                if (flags.HasFlag(GwsWindowFlags.Resizable))
                    flag |= WindowFlags.Resizable;

                if (flags.HasFlag(GwsWindowFlags.Shown))
                    flag |= WindowFlags.Shown;

                return flag;
            }

            #endregion
        }
#if AllHidden
    }
#endif
}
#endif
