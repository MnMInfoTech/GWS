
#if GWS && MS
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MnM.GWS
{
    partial class Factory
    {
        internal class MSWindow : Form, IExOSWindow
        {
            #region VARIABLES
            ModifierKeys ModifierKey;
            MouseButton MouseButton;
            bool IsResizing = false;
            MemoryOccupation memoryOccupation;
            readonly IExKeyEventArgs keyEventArgs = new KeyEventArgs();
            readonly IExMouseEventArgs mouseEventArgs = new MouseEventArgs();
            readonly IExKeyPressEventArgs keyPressEventArgs = new KeyPressEventArgs();
            readonly IExMouseEnterLeaveEventArgs MouseEnterLeaveArgs = new MouseEnterLeaveEventArgs();

            readonly IExEventInfo keyeventInfo = new EventInfo();
            readonly IExEventInfo mouseeventInfo = new EventInfo();
            readonly IExEventInfo keypresseventInfo = new EventInfo();
            readonly IExEventInfo loadEventInfo = new EventInfo();
            IExRenderWindow Window;
            MSTarget Target;
            static readonly IScreen Screen = new GWSScreen(System.Windows.Forms.Screen.PrimaryScreen);
            #endregion

            #region PROPERTIES
            OS IOSMinimalWindow.OS => OS.Windows;
            float IOSMinimalWindow.Transparency { get; set; }
            string IID<string>.ID => Name;
            int IPoint.X => Location.X;
            int IPoint.Y => Location.Y;
            public IMemoryOccupation MemoryOccupation => memoryOccupation;
            ModifierKeys IExOSWindow.ModifierKey { get => ModifierKey; set => ModifierKey = value; }
            MouseButton IExOSWindow.MouseButton { get => MouseButton; set => MouseButton = value; }
            ModifierKeys IInputStateHolder.ModifierKey { get => ModifierKey; }
            MouseButton IInputStateHolder.MouseButton { get => MouseButton; }
            string ITextDisplayer.Text
            {
                get => base.Text;
                set
                {
                    SetTextSafe(value);
                }
            }
            IRenderTarget IRenderTargetHolder.Target => Target;
            IScreen IOSMinimalWindow.Screen => Screen;
            Location ILocationHolder.Location
            {
                get => new Location(Location.X, Location.Y);
                set
                {
                    Location = new System.Drawing.Point(value.X, value.Y);
                }
            }
            bool IFocusable.Focusable => !IsDisposed && Enabled;
            #endregion

            #region INITIALIZE
            bool IExOSWindow.PsuedoConstructor(IRenderWindow window,
                out GwsWindowFlags resultFlags, string title,
                int? width, int? height, int? x, int? y, GwsWindowFlags? flags, RendererFlags? renderFlags)
            {
                memoryOccupation = new MemoryOccupation();
                memoryOccupation.BeginMonitoring(out long total);
                if (!(window is IExRenderWindow))
                {
                    throw new ArgumentException("Given Render-Window is not compatible with this OS-Window!");
                }
                Window = (IExRenderWindow)window;
                resultFlags = 0;
                Location = new System.Drawing.Point(x ?? 0, y ?? 0);
                Size = new System.Drawing.Size(width ?? 400, height ?? 400);
                Text = title;
                DoubleBuffered = true;
                StartPosition = FormStartPosition.Manual;
                memoryOccupation.EndMonitoring(total);
                Target = new MSTarget(Window, this);
                return true;
            }
            #endregion

            #region RESIZE
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void OnResizeEnd(System.EventArgs e)
            {
                IsResizing = true;
                int w = Size.Width;
                int h = Size.Height;
                Window.Resize(w, h, out bool success);
                Window.Update(new Rectangle(0, 0, w, h), UpdateCommand.RestoreScreen);
                base.OnResizeEnd(e);
                IsResizing = false;
            }
            #endregion

            #region PARSE EVENT
            void SetTextSafe(string text)
            {
                Invoke(new DelTextChange(setTextSafe), text);
            }
            delegate void DelTextChange(string text);
            void setTextSafe(string text) =>
                base.Text = text;

            IEventArgs IEventParser.ParseEvent(IExternalEventInfo @event)
            {
                throw new NotImplementedException();
            }
            #endregion

            #region PAINT
            protected override void OnPaintBackground(PaintEventArgs e)
            {
                base.OnPaintBackground(e);
                e.Graphics.DrawImage(Target.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            }
            #endregion

            #region HIDE
            void IHideable.Hide(bool forcefully)
            {
                base.Hide();
            }
            #endregion

            #region SHOW MESSAGEBOX
            MsgBoxResult IMessageBoxHost.ShowMessageBox(string title, int x, int y, string text, MsgBoxButtons buttons = MsgBoxButtons.YesNo)
            {
                System.Windows.Forms.MessageBoxButtons Button = 0;
                switch (buttons)
                {
                    case MsgBoxButtons.YesNo:
                        Button = MessageBoxButtons.YesNo;
                        break;
                    case MsgBoxButtons.OkCancel:
                        Button = MessageBoxButtons.OKCancel;
                        break;
                    case MsgBoxButtons.Information:
                    case MsgBoxButtons.Error:
                        Button = MessageBoxButtons.OK;
                        break;
                    case MsgBoxButtons.AbortRetry:
                        Button = MessageBoxButtons.AbortRetryIgnore;
                        break;
                    default:
                        break;
                }
                var result = System.Windows.Forms.MessageBox.Show(this, text, title, Button);
                switch (result)
                {
                    case DialogResult.None:
                    default:
                        return MsgBoxResult.None;
                    case DialogResult.OK:
                        return MsgBoxResult.Ok;
                    case DialogResult.Cancel:
                        return MsgBoxResult.Cancel;
                    case DialogResult.Abort:
                        return MsgBoxResult.Abort;
                    case DialogResult.Retry:
                        return MsgBoxResult.Abort;
                    case DialogResult.Ignore:
                        return MsgBoxResult.Ignore;
                    case DialogResult.Yes:
                        return MsgBoxResult.Yes;
                    case DialogResult.No:
                        return MsgBoxResult.No;
                }
            }
            #endregion

            #region SHOW INPUT BOX
            Lot<MsgBoxResult, string> IMessageBoxHost.ShowInputBox(string title, int x, int y, string text, MsgBoxButtons buttons = MsgBoxButtons.YesNo)
            {
                return Lot.Create<MsgBoxResult, string>();
            }
            #endregion

            #region EVENT BINDING
            protected override void OnShown(System.EventArgs e)
            {
                base.OnShown(e);
                loadEventInfo.Reset(this, Factory.DefaultArgs, GwsEvent.Load);
                Window?.PushEvent(loadEventInfo);
            }
            protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
            {
                keyEventArgs.KeyCode = ToKeyCode((int)e.KeyCode);
                keyEventArgs.State = KeyState.Up;
                keyeventInfo.Reset(this, keyEventArgs, GwsEvent.KeyUp);
                Window?.PushEvent(keyeventInfo);
            }
            protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                keyEventArgs.KeyCode = ToKeyCode((int)e.KeyCode);
                keyEventArgs.State = KeyState.Down;
                keyeventInfo.Reset(this, keyEventArgs, GwsEvent.KeyDown);
                Window?.PushEvent(keyeventInfo);
            }
            protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
            {
                mouseEventArgs.Status = MouseState.Up;
                mouseEventArgs.Button = ToMouseButton(e.Button);
                mouseEventArgs.X = e.X;
                mouseEventArgs.Y = e.Y;
                mouseeventInfo.Reset(this, mouseEventArgs, GwsEvent.MouseUp);
                Window?.PushEvent(mouseeventInfo);
            }
            protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
            {
                mouseEventArgs.Status = MouseState.Down;
                mouseEventArgs.Button = ToMouseButton(e.Button);
                mouseEventArgs.X = e.X;
                mouseEventArgs.Y = e.Y;
                mouseeventInfo.Reset(this, mouseEventArgs, GwsEvent.MouseDown);
                Window?.PushEvent(mouseeventInfo);
            }
            protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
            {
                mouseEventArgs.Status = MouseState.Move;
                mouseEventArgs.Button = ToMouseButton(e.Button);
                mouseEventArgs.X = e.X;
                mouseEventArgs.Y = e.Y;
                mouseeventInfo.Reset(this, mouseEventArgs, GwsEvent.MouseMotion);
                Window?.PushEvent(mouseeventInfo);
            }
            protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
            {
                mouseEventArgs.Status = MouseState.Wheel;
                mouseEventArgs.Button = ToMouseButton(e.Button);
                mouseEventArgs.Y = e.Delta;
                mouseeventInfo.Reset(this, mouseEventArgs, GwsEvent.MouseWheel);
                Window?.PushEvent(mouseeventInfo);
            }
            protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
            {
                mouseEventArgs.Status = MouseState.Click;
                mouseEventArgs.Button = ToMouseButton(e.Button);
                mouseEventArgs.X = e.X;
                mouseEventArgs.Y = e.Y;

                mouseeventInfo.Reset(this, mouseEventArgs, GwsEvent.MouseClick);
                Window?.PushEvent(mouseeventInfo);
            }
            protected override void OnMouseEnter(System.EventArgs e)
            {
                MouseEnterLeaveArgs.Status = MouseState.Enter;
                MouseEnterLeaveArgs.Button = 0;
                MouseEnterLeaveArgs.X = System.Windows.Forms.Control.MousePosition.X;
                MouseEnterLeaveArgs.Y = System.Windows.Forms.Control.MousePosition.Y;
                mouseeventInfo.Reset(this, MouseEnterLeaveArgs, GwsEvent.MouseEnter);
                Window?.PushEvent(mouseeventInfo);
            }
            protected override void OnMouseLeave(System.EventArgs e)
            {
                MouseEnterLeaveArgs.Status = MouseState.Leave;
                MouseEnterLeaveArgs.Button = 0;
                MouseEnterLeaveArgs.X = System.Windows.Forms.Control.MousePosition.X;
                MouseEnterLeaveArgs.Y = System.Windows.Forms.Control.MousePosition.Y;
                mouseeventInfo.Reset(this, MouseEnterLeaveArgs, GwsEvent.MouseLeave);
                Window?.PushEvent(mouseeventInfo);
            }
            protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
            {
                keyPressEventArgs.KeyChar = e.KeyChar;
                keypresseventInfo.Reset(this, keyPressEventArgs, GwsEvent.KeyPress);
                Window?.PushEvent(keypresseventInfo);
            }
            protected override void OnClosed(System.EventArgs e)
            {
                base.OnClosed(e);
            }
            protected override void OnFormClosing(FormClosingEventArgs e)
            {
                base.OnFormClosing(e);
            }
            #endregion

            #region TO MOUSE BUTTON
            static MouseButton ToMouseButton(MouseButtons button)
            {
                switch (button)
                {
                    case MouseButtons.Left:
                        return MouseButton.Left;
                    case MouseButtons.None:
                        return MouseButton.None;
                    case MouseButtons.Right:
                        return MouseButton.Right;
                    case MouseButtons.Middle:
                        return MouseButton.Middle;
                    case MouseButtons.XButton1:
                        return MouseButton.Button1;
                    case MouseButtons.XButton2:
                        return MouseButton.Button2;
                    default:
                        return MouseButton.None;
                }
            }
            #endregion

            #region TO KEY-CODE
            Key ToKeyCode(int scanCode)
            {
                var Code = (Keys)scanCode;
                Key key = 0;
                switch (Code)
                {
                    case Keys.KeyCode:
                        key = Key.KeyCode;

                        break;
                    case Keys.Modifiers:
                        key = Key.Modifiers;

                        break;
                    case Keys.None:
                        key = Key.None;

                        break;
                    case Keys.LButton:
                        key = Key.LButton;

                        break;
                    case Keys.RButton:
                        key = Key.RButton;

                        break;
                    case Keys.Cancel:
                        key = Key.Cancel;

                        break;
                    case Keys.MButton:
                        key = Key.MButton;

                        break;
                    case Keys.XButton1:
                        key = Key.XButton1;

                        break;
                    case Keys.XButton2:
                        key = Key.XButton2;

                        break;
                    case Keys.Back:
                        key = Key.Back;

                        break;
                    case Keys.Tab:
                        key = Key.Tab;

                        break;
                    case Keys.LineFeed:
                        key = Key.LineFeed;

                        break;
                    case Keys.Clear:
                        key = Key.Clear;

                        break;
                    case Keys.Return:
                        key = Key.Return;


                        break;
                    case Keys.ShiftKey:
                        key = Key.ShiftKey;

                        break;
                    case Keys.ControlKey:
                        key = Key.ControlKey;

                        break;
                    case Keys.Menu:
                        key = Key.Menu;

                        break;
                    case Keys.Pause:
                        key = Key.Pause;

                        break;
                    case Keys.Capital:
                        key = Key.Capital;

                        break;
                    case Keys.KanaMode:
                        key = Key.KanaMode;

                        break;
                    case Keys.JunjaMode:
                        key = Key.JunjaMode;

                        break;
                    case Keys.FinalMode:
                        key = Key.FinalMode;

                        break;
                    case Keys.HanjaMode:
                        key = Key.HanjaMode;


                        break;
                    case Keys.Escape:
                        key = Key.Escape;

                        break;
                    case Keys.IMEConvert:
                        key = Key.IMEConvert;

                        break;
                    case Keys.IMENonconvert:
                        key = Key.IMENonconvert;

                        break;
                    case Keys.IMEAccept:
                        key = Key.IMEAccept;

                        break;
                    case Keys.IMEModeChange:
                        key = Key.IMEModeChange;

                        break;
                    case Keys.Space:
                        key = Key.Space;

                        break;
                    case Keys.Prior:
                        key = Key.Prior;

                        break;
                    case Keys.Next:
                        key = Key.Next;

                        break;
                    case Keys.End:
                        key = Key.End;

                        break;
                    case Keys.Home:
                        key = Key.Home;

                        break;
                    case Keys.Left:
                        key = Key.Left;

                        break;
                    case Keys.Up:
                        key = Key.Up;

                        break;
                    case Keys.Right:
                        key = Key.Right;

                        break;
                    case Keys.Down:
                        key = Key.Down;

                        break;
                    case Keys.Select:
                        key = Key.Select;

                        break;
                    case Keys.Print:
                        key = Key.Print;

                        break;
                    case Keys.Execute:
                        key = Key.Execute;

                        break;
                    case Keys.Snapshot:
                        key = Key.Snapshot;

                        break;
                    case Keys.Insert:
                        key = Key.Insert;

                        break;
                    case Keys.Delete:
                        key = Key.Delete;

                        break;
                    case Keys.Help:
                        key = Key.Help;

                        break;
                    case Keys.D0:
                        key = Key.D0;

                        break;
                    case Keys.D1:
                        key = Key.D1;

                        break;
                    case Keys.D2:
                        key = Key.D2;

                        break;
                    case Keys.D3:
                        key = Key.D3;

                        break;
                    case Keys.D4:
                        key = Key.D4;

                        break;
                    case Keys.D5:
                        key = Key.D5;

                        break;
                    case Keys.D6:
                        key = Key.D6;

                        break;
                    case Keys.D7:
                        key = Key.D7;

                        break;
                    case Keys.D8:
                        key = Key.D8;

                        break;
                    case Keys.D9:
                        key = Key.D9;

                        break;
                    case Keys.A:
                        key = Key.A;

                        break;
                    case Keys.B:
                        key = Key.B;

                        break;
                    case Keys.C:
                        key = Key.C;

                        break;
                    case Keys.D:
                        key = Key.D;

                        break;
                    case Keys.E:
                        key = Key.E;

                        break;
                    case Keys.F:
                        key = Key.F;

                        break;
                    case Keys.G:
                        key = Key.G;

                        break;
                    case Keys.H:
                        key = Key.H;

                        break;
                    case Keys.I:
                        key = Key.I;

                        break;
                    case Keys.J:
                        key = Key.J;

                        break;
                    case Keys.K:
                        key = Key.K;

                        break;
                    case Keys.L:
                        key = Key.L;

                        break;
                    case Keys.M:
                        key = Key.M;

                        break;
                    case Keys.N:
                        key = Key.N;

                        break;
                    case Keys.O:
                        key = Key.O;

                        break;
                    case Keys.P:
                        key = Key.P;

                        break;
                    case Keys.Q:
                        key = Key.Q;

                        break;
                    case Keys.R:
                        key = Key.R;

                        break;
                    case Keys.S:
                        key = Key.S;

                        break;
                    case Keys.T:
                        key = Key.T;

                        break;
                    case Keys.U:
                        key = Key.U;

                        break;
                    case Keys.V:
                        key = Key.V;

                        break;
                    case Keys.W:
                        key = Key.W;

                        break;
                    case Keys.X:
                        key = Key.X;

                        break;
                    case Keys.Y:
                        key = Key.Y;

                        break;
                    case Keys.Z:
                        key = Key.Z;

                        break;
                    case Keys.LWin:
                        key = Key.LWin;

                        break;
                    case Keys.RWin:
                        key = Key.RWin;

                        break;
                    case Keys.Apps:
                        key = Key.Apps;

                        break;
                    case Keys.Sleep:
                        key = Key.Sleep;

                        break;
                    case Keys.NumPad0:
                        key = Key.NumPad0;

                        break;
                    case Keys.NumPad1:
                        key = Key.NumPad1;

                        break;
                    case Keys.NumPad2:
                        key = Key.NumPad2;

                        break;
                    case Keys.NumPad3:
                        key = Key.NumPad3;

                        break;
                    case Keys.NumPad4:
                        key = Key.NumPad4;

                        break;
                    case Keys.NumPad5:
                        key = Key.NumPad5;

                        break;
                    case Keys.NumPad6:
                        key = Key.NumPad6;

                        break;
                    case Keys.NumPad7:
                        key = Key.NumPad7;

                        break;
                    case Keys.NumPad8:
                        key = Key.NumPad8;

                        break;
                    case Keys.NumPad9:
                        key = Key.NumPad9;

                        break;
                    case Keys.Multiply:
                        key = Key.Multiply;

                        break;
                    case Keys.Add:
                        key = Key.Add;

                        break;
                    case Keys.Separator:
                        key = Key.Separator;

                        break;
                    case Keys.Subtract:
                        key = Key.Subtract;

                        break;
                    case Keys.Decimal:
                        key = Key.Decimal;

                        break;
                    case Keys.Divide:
                        key = Key.Divide;

                        break;
                    case Keys.F1:
                        key = Key.F1;

                        break;
                    case Keys.F2:
                        key = Key.F2;

                        break;
                    case Keys.F3:
                        key = Key.F3;

                        break;
                    case Keys.F4:
                        key = Key.F4;

                        break;
                    case Keys.F5:
                        key = Key.F5;

                        break;
                    case Keys.F6:
                        key = Key.F6;

                        break;
                    case Keys.F7:
                        key = Key.F7;

                        break;
                    case Keys.F8:
                        key = Key.F8;

                        break;
                    case Keys.F9:
                        key = Key.F9;

                        break;
                    case Keys.F10:
                        key = Key.F10;

                        break;
                    case Keys.F11:
                        key = Key.F11;

                        break;
                    case Keys.F12:
                        key = Key.F12;

                        break;
                    case Keys.F13:
                        key = Key.F13;

                        break;
                    case Keys.F14:
                        key = Key.F14;

                        break;
                    case Keys.F15:
                        key = Key.F15;

                        break;
                    case Keys.F16:
                        key = Key.F16;

                        break;
                    case Keys.F17:
                        key = Key.F17;

                        break;
                    case Keys.F18:
                        key = Key.F18;

                        break;
                    case Keys.F19:
                        key = Key.F19;

                        break;
                    case Keys.F20:
                        key = Key.F20;

                        break;
                    case Keys.F21:
                        key = Key.F21;

                        break;
                    case Keys.F22:
                        key = Key.F22;

                        break;
                    case Keys.F23:
                        key = Key.F23;

                        break;
                    case Keys.F24:
                        key = Key.F24;

                        break;
                    case Keys.NumLock:
                        key = Key.NumLock;

                        break;
                    case Keys.Scroll:
                        key = Key.Scroll;

                        break;
                    case Keys.LShiftKey:
                        key = Key.LShiftKey;

                        break;
                    case Keys.RShiftKey:
                        key = Key.RShiftKey;

                        break;
                    case Keys.LControlKey:
                        key = Key.LControlKey;

                        break;
                    case Keys.RControlKey:
                        key = Key.RControlKey;

                        break;
                    case Keys.LMenu:
                        key = Key.LMenu;

                        break;
                    case Keys.RMenu:
                        key = Key.RMenu;

                        break;
                    case Keys.BrowserBack:
                        key = Key.BrowserBack;

                        break;
                    case Keys.BrowserForward:
                        key = Key.BrowserForward;

                        break;
                    case Keys.BrowserRefresh:
                        key = Key.BrowserRefresh;

                        break;
                    case Keys.BrowserStop:
                        key = Key.BrowserStop;

                        break;
                    case Keys.BrowserSearch:
                        key = Key.BrowserSearch;

                        break;
                    case Keys.BrowserFavorites:
                        key = Key.BrowserFavorites;

                        break;
                    case Keys.BrowserHome:
                        key = Key.BrowserHome;

                        break;
                    case Keys.VolumeMute:
                        key = Key.VolumeMute;

                        break;
                    case Keys.VolumeDown:
                        key = Key.VolumeDown;

                        break;
                    case Keys.VolumeUp:
                        key = Key.VolumeUp;

                        break;
                    case Keys.MediaNextTrack:
                        key = Key.MediaNextTrack;

                        break;
                    case Keys.MediaPreviousTrack:
                        key = Key.MediaPreviousTrack;

                        break;
                    case Keys.MediaStop:
                        key = Key.MediaStop;

                        break;
                    case Keys.MediaPlayPause:
                        key = Key.MediaPlayPause;

                        break;
                    case Keys.LaunchMail:
                        key = Key.LaunchMail;

                        break;
                    case Keys.SelectMedia:
                        key = Key.SelectMedia;

                        break;
                    case Keys.LaunchApplication1:
                        key = Key.LaunchApplication1;

                        break;
                    case Keys.LaunchApplication2:
                        key = Key.LaunchApplication2;

                        break;
                    case Keys.OemSemicolon:
                        key = Key.OemSemicolon;

                        break;
                    case Keys.Oemplus:
                        key = Key.Oemplus;

                        break;
                    case Keys.Oemcomma:
                        key = Key.Oemcomma;

                        break;
                    case Keys.OemMinus:
                        key = Key.OemMinus;

                        break;
                    case Keys.OemPeriod:
                        key = Key.OemPeriod;

                        break;
                    case Keys.OemQuestion:
                        key = Key.OemQuestion;

                        break;
                    case Keys.Oemtilde:
                        key = Key.OemTilde;

                        break;
                    case Keys.OemOpenBrackets:
                        key = Key.OemOpenBrackets;

                        break;
                    case Keys.OemPipe:
                        key = Key.OemPipe;

                        break;
                    case Keys.OemCloseBrackets:
                        key = Key.OemCloseBrackets;

                        break;
                    case Keys.OemQuotes:
                        key = Key.OemQuotes;

                        break;
                    case Keys.Oem8:
                        key = Key.Oem8;

                        break;
                    case Keys.OemBackslash:
                        key = Key.OemBackslash;

                        break;
                    case Keys.ProcessKey:
                        key = Key.ProcessKey;

                        break;
                    case Keys.Packet:
                        key = Key.Packet;

                        break;
                    case Keys.Attn:
                        key = Key.Attn;

                        break;
                    case Keys.Crsel:
                        key = Key.Crsel;

                        break;
                    case Keys.Exsel:
                        key = Key.Exsel;

                        break;
                    case Keys.EraseEof:
                        key = Key.EraseEof;

                        break;
                    case Keys.Play:
                        key = Key.Play;

                        break;
                    case Keys.Zoom:
                        key = Key.Zoom;

                        break;
                    case Keys.NoName:
                        key = Key.NoName;

                        break;
                    case Keys.Pa1:
                        key = Key.Pa1;

                        break;
                    case Keys.OemClear:
                        key = Key.OemClear;

                        break;
                    case Keys.Shift:
                        key = Key.Shift;

                        break;
                    case Keys.Control:
                        key = Key.Control;

                        break;
                    case Keys.Alt:
                        key = Key.Alt;
                        break;
                    default:
                        switch (Code)
                        {
                            case Keys.Enter:
                                key = Key.Enter;

                                break;
                            case Keys.CapsLock:
                                key = Key.CapsLock;

                                break;
                            case Keys.HangulMode:
                                key = Key.HangulMode;

                                break;
                            case Keys.KanjiMode:
                                key = Key.KanjiMode;

                                break;
                            case Keys.IMEAceept:
                                key = Key.IMEAceept;

                                break;
                            case Keys.PageUp:
                                key = Key.PageUp;

                                break;
                            case Keys.PageDown:
                                key = Key.PageDown;

                                break;
                            case Keys.PrintScreen:
                                key = Key.PrintScreen;

                                break;
                            case Keys.Oem1:
                                key = Key.Oem1;

                                break;
                            case Keys.Oem2:
                                key = Key.Oem2;

                                break;
                            case Keys.Oem3:
                                key = Key.Oem3;

                                break;
                            case Keys.Oem4:
                                key = Key.Oem4;

                                break;
                            case Keys.Oem5:
                                key = Key.Oem5;

                                break;
                            case Keys.Oem6:
                                key = Key.Oem6;

                                break;
                            case Keys.Oem7:
                                key = Key.Oem7;

                                break;
                            case Keys.Oem102:
                                key = Key.Oem102;
                                break;
                            default:
                                break;
                        }
                        break;
                }
                return key;
            }
            #endregion

            #region GET - SET CURSOR POS
            void ICursorManager.GetCursorPos(out int x, out int y, bool global)
            {
                var pt = Cursor.Position;
                if (!global)
                {
                    pt = this.PointToClient(pt);
                }
                x = pt.X;
                y = pt.Y;
            }

            void ICursorManager.SetCursorPos(int x, int y, bool global)
            {
                var pt = new System.Drawing.Point(x, y);
                if (!global)
                {
                    pt = this.PointToClient(pt);
                }
                Cursor.Position = pt;
            }
            #endregion

            #region SET CURSOR TYPE
            void ICursorManager.SetCursorType(CursorType cursor)
            {
                switch (cursor)
                {
                    case CursorType.Arrow:
                    case CursorType.Default:
                    default:
                        Cursor = Cursors.Arrow;//SDL_SYSTEM_CURSOR_ARROW;
                        break;
                    case CursorType.IBeam:
                        Cursor = Cursors.IBeam;// SystemCursor.SDL_SYSTEM_CURSOR_IBEAM;
                        break;
                    case CursorType.WaitCursor:
                        Cursor = Cursors.WaitCursor;//SystemCursor.SDL_SYSTEM_CURSOR_WAIT;
                        break;
                    case CursorType.Cross:
                        Cursor = Cursors.Cross; //SDL_SYSTEM_CURSOR_CROSSHAIR;
                        break;
                    case CursorType.SizeNWSE:
                        Cursor = Cursors.SizeNWSE;// SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE;
                        break;
                    case CursorType.SizeNESW:
                        Cursor = Cursors.SizeNESW;// SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW;
                        break;
                    case CursorType.SizeWE:
                        Cursor = Cursors.SizeWE;//SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE;
                        break;
                    case CursorType.SizeNS:
                        Cursor = Cursors.SizeNS;//SystemCursor.SDL_SYSTEM_CURSOR_SIZENS;
                        break;
                    case CursorType.SizeAll:
                        Cursor = Cursors.SizeAll;//SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL;
                        break;
                    case CursorType.No:
                        Cursor = Cursors.No;// SystemCursor.SDL_SYSTEM_CURSOR_NO;
                        break;
                    case CursorType.Hand:
                        Cursor = Cursors.Hand;// SystemCursor.SDL_SYSTEM_CURSOR_HAND;
                        break;
                }
            }
            #endregion

            #region TARGET CLASS
            sealed class MSTarget : RenderTarget
            {
                internal volatile Bitmap Bitmap;
                Array<int> Pointer;
                string id;
                MSWindow MSWindow;

                #region CONSTRUCTOR
                internal MSTarget(IRenderWindow window, MSWindow mSWindow) : base(window)
                {
                    MSWindow = mSWindow;
                    Pointer = new Array<int>(window.Width, window.Height);
                    Bitmap = new Bitmap(window.Width, window.Height, window.Width * 4, System.Drawing.Imaging.PixelFormat.Format32bppRgb, Pointer.Handle);
                    id = Application.NewID("MSTarget");
                }
                #endregion

                public override int Width => Pointer.Width;
                public override int Height => Pointer.Height;
                public override string ID => id;
                protected override IntPtr Source => Pointer.Handle;

                #region UPDATE
                //[MethodImpl(MethodImplOptions.AggressiveInlining)]
                public override void Update(IBounds bounds, UpdateCommand command = UpdateCommand.Default)
                {
                    if ((RenderWindow.ViewState & ViewState.Disposed) == ViewState.Disposed ||
                        (command & UpdateCommand.SkipDisplayUpdate) == UpdateCommand.SkipDisplayUpdate)
                    {
                        return;
                    }
                    System.Drawing.Rectangle rc;
                    if (bounds == null)
                        rc = new System.Drawing.Rectangle(0, 0, Width, Height);
                    else
                    {
                        bounds.GetBounds(out int x, out int y, out int w, out int h);
                        --x;
                        --y;
                        ++w;
                        ++h;
                        if (x < 0) x = 0;
                        if (y < 0) y = 0;
                        rc = new System.Drawing.Rectangle(x,y,w,h);
                    }
                    if (MSWindow.InvokeRequired)
                        UpdateSafe(rc);
                    else
                        updateSafe(rc);
                }
                void updateSafe(System.Drawing.Rectangle rectangle)
                {
                    MSWindow.Invalidate(rectangle);
                    MSWindow.Update();
                }
                void UpdateSafe(System.Drawing.Rectangle rectangle)
                {
                    if (MSWindow.IsDisposed)
                    {
                        return;
                    }
                    MSWindow.Invoke(new DelUpdate(updateSafe), rectangle);
                }
                delegate void DelUpdate(System.Drawing.Rectangle rectangle);
                #endregion

                #region RESIZE
                //[MethodImpl(MethodImplOptions.AggressiveInlining)]
                protected override void ResizeInternal(int w, int h)
                {
                    Pointer.Resize(w, h, out _, ResizeCommand.SizeOnlyToFit);
                    Bitmap = new Bitmap(w, h, w * 4,
                        System.Drawing.Imaging.PixelFormat.Format32bppRgb, Pointer.Handle);
                }
                #endregion
            }
            #endregion

            #region SCREEN CLASS
            class GWSScreen : IScreen
            {
                Screen Screen;

                internal GWSScreen(Screen screen)
                {
                    Screen = screen;
                  
                }

                public bool IsPrimary => Screen.Primary;
                public int BitsPerPixel => Screen.BitsPerPixel;
                public int X =>Screen.WorkingArea.X;
                public int Y =>Screen.WorkingArea.Y;
                public int Width => Screen.WorkingArea.Width;
                public int Height => Screen.WorkingArea.Height;

                public void Dispose()
                {
                    Screen = null;
                }

                public bool Contains(float x, float y) =>
                    Screen.WorkingArea.Contains((int)x, (int)y);
            }
            #endregion
        }
    }
}
#endif
