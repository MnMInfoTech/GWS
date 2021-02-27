/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if MS && (GWS || Window)
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using MnM.GWS.Desktop;

namespace MnM.GWS
{
#if HideNativeObjects
    partial class NativeFactory
    {
#else
    public
#endif
        sealed partial class MSTarget : _RenderTarget, INativeTarget
        {
            #region VARIABLES
            /// <summary>
            /// 
            /// </summary>
            INativeForm NativeForm;

            /// <summary>
            /// 
            /// </summary>
            volatile Bitmap Bitmap;

            /// <summary>
            /// 
            /// </summary>
            readonly Array<int> Pointer;

            readonly _Form form;
            #endregion

            #region CONSTRUCTORS
            public MSTarget(int x, int y, int w, int h) : 
                base(w, h)
            {
                Pointer = new Array<int>(w, h);
                Bitmap = new Bitmap(w, h, w * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, Pointer.Handle);
                width = Pointer.Width;
                height = Pointer.Height;
                length = Pointer.Length;
                form = new _Form(this, x, y, w, h);
                Window = form;
            }
            #endregion

            #region PROPERTIES
            public override IntPtr Source => Pointer.Handle;
            public IntPtr Handle => form.Handle;
            INativeForm INativeTarget.Form
            {
                get => NativeForm;
                set
                {
                    NativeForm = value;
                    if (NativeForm == null)
                        return;
                    if (NativeForm.Width != width || NativeForm.Height != height)
                    {
                        Resize(NativeForm.Width, NativeForm.Height);
                    }
                }
            }
            public string Text
            {
                get => form.Text;
                set => form.Text = value;
            }
            #endregion

            #region RESIZE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void ResizeSource(int w, int h)
            {
                width = w;
                height = h;
                length = width * height;
                Pointer.Resize(width, height);
                var all = new Rect(0, 0, width, height);
                Bitmap = new Bitmap(Pointer.Width, Pointer.Height, Pointer.Width * 4,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb, Pointer.Handle);
                NativeForm.CopyTo(Pointer.Handle, length, width, 0, 0, all, Command.Backdrop);
                Update(0, all);
            }
            #endregion

            #region UPDATE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Update(ulong command, IBoundable area)
            {
                if (area == null || !area.Valid)
                    return;
                int x, y, w, h;
                System.Drawing.Rectangle rc;
                area.GetBounds(out x, out y, out w, out h);
                rc = new System.Drawing.Rectangle(x, y, w, h);
                form.Update(rc);
            }
            #endregion

            #region SHOW - HIDE
            public void Show() => form.Show();
            public void Hide() => form.Hide();
            #endregion

            #region DISPOSE
            public override void Dispose()
            {
                base.Dispose();
                Bitmap.Dispose();
                Pointer.Dispose();
            }
            #endregion

            #region FORM
            class _Form : Form, IRenderWindow
            {
                MSTarget Target;
             
                #region EVENT ARGS
                readonly MsKeyEventArgs keyEventArgs = new MsKeyEventArgs();
                readonly MsMouseEventArgs mouseEventArgs = new MsMouseEventArgs();
                readonly MsKeyPressEventArgs keyPressEventArgs = new MsKeyPressEventArgs();

                readonly EventInfo keyeventInfo = new EventInfo();
                readonly EventInfo mouseeventInfo = new EventInfo();
                readonly EventInfo keypresseventInfo = new EventInfo();
                readonly EventInfo loadEventInfo = new EventInfo();
                #endregion

                public _Form(MSTarget target, int x, int y, int w, int h)
                {
                    Target = target;
                    DoubleBuffered = true;
                    StartPosition = FormStartPosition.Manual;
                    Location = new Point(x, y);
                    Size = new System.Drawing.Size(w, h);
                }

                #region PROPERTIES
                int ISize.Width => Target.width;
                int ISize.Height => Target.height;
                public int Length => Target.length;
                public RendererFlags RendererFlags => RendererFlags.Default;
                #endregion

                #region PAINT
                protected override void OnPaintBackground(PaintEventArgs e)
                {
                    e.Graphics.DrawImage(Target.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
                }
                #endregion

                #region RESIZE
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                protected override void OnResizeEnd(EventArgs e)
                {
                    Target.NativeForm.Resize(Size.Width, Size.Height);
                }
                #endregion

                #region EVENT BINDING
                protected override void OnLoad(EventArgs e)
                {
                    base.OnLoad(e);
                    loadEventInfo.Sender = this;
                    loadEventInfo.Args = Factory.EmptyArgs;
                    loadEventInfo.Type = 57;//FirstShown;
                    Target.NativeForm?.PushEvent(loadEventInfo);
                }
                protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
                {
                    keyEventArgs.keyCode = e.KeyCode.ToGWSKey();
                    keyEventArgs.scanCode = e.KeyValue;
                    keyEventArgs.keyState = KeyState.Up;
                    keyeventInfo.Sender = this;
                    keyeventInfo.Args = keyEventArgs;
                    keyeventInfo.Type = 5;//KeyUp;
                    Target.NativeForm?.PushEvent(keyeventInfo);
                }
                protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
                {
                    keyEventArgs.keyCode = e.KeyCode.ToGWSKey();
                    keyEventArgs.scanCode = e.KeyValue;
                    keyEventArgs.keyState = KeyState.Down;
                    keyeventInfo.Sender = this;
                    keyeventInfo.Args = keyEventArgs;
                    keyeventInfo.Type = 4;//KeyDown;
                    Target.NativeForm?.PushEvent(keyeventInfo);
                }
                protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
                {
                    mouseEventArgs.mouseState = MouseState.Up;
                    mouseEventArgs.button = e.Button.ToGwsButton();
                    mouseEventArgs.x = e.X;
                    mouseEventArgs.y = e.Y;
                    mouseEventArgs.delta = e.Delta;

                    mouseeventInfo.Sender = this;
                    mouseeventInfo.Args = mouseEventArgs;
                    mouseeventInfo.Type = 9;//MouseUp;
                    Target.NativeForm?.PushEvent(mouseeventInfo);
                }
                protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
                {
                    mouseEventArgs.mouseState = MouseState.Down;
                    mouseEventArgs.button = e.Button.ToGwsButton();
                    mouseEventArgs.x = e.X;
                    mouseEventArgs.y = e.Y;
                    mouseEventArgs.delta = e.Delta;

                    mouseeventInfo.Sender = this;
                    mouseeventInfo.Args = mouseEventArgs;
                    mouseeventInfo.Type = 8;//MouseDown;
                    Target.NativeForm?.PushEvent(mouseeventInfo);
                }
                protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
                {
                    mouseEventArgs.mouseState = MouseState.Move;
                    mouseEventArgs.button = e.Button.ToGwsButton();
                    mouseEventArgs.x = e.X;
                    mouseEventArgs.y = e.Y;
                    mouseEventArgs.delta = e.Delta;

                    mouseeventInfo.Sender = this;
                    mouseeventInfo.Args = mouseEventArgs;
                    mouseeventInfo.Type = 7;//MouseMotion;
                    Target.NativeForm?.PushEvent(mouseeventInfo);
                }
                protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
                {
                    mouseEventArgs.mouseState = MouseState.Wheel;
                    mouseEventArgs.button = e.Button.ToGwsButton();
                    mouseEventArgs.x = e.X;
                    mouseEventArgs.y = e.Y;
                    mouseEventArgs.delta = e.Delta;

                    mouseeventInfo.Sender = this;
                    mouseeventInfo.Args = mouseEventArgs;
                    mouseeventInfo.Type = 10;//MouseWheel;
                    Target.NativeForm?.PushEvent(mouseeventInfo);
                }
                protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
                {
                    mouseEventArgs.mouseState = MouseState.Click;
                    mouseEventArgs.button = e.Button.ToGwsButton();
                    mouseEventArgs.x = e.X;
                    mouseEventArgs.y = e.Y;
                    mouseEventArgs.delta = e.Delta;

                    mouseeventInfo.Sender = this;
                    mouseeventInfo.Args = mouseEventArgs;
                    mouseeventInfo.Type = 56;//MouseClick;
                    Target.NativeForm?.PushEvent(mouseeventInfo);
                }
                protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
                {
                    keyPressEventArgs.keyChar = e.KeyChar;
                    keypresseventInfo.Sender = this;
                    keypresseventInfo.Args = keyPressEventArgs;
                    keypresseventInfo.Type = 6; //TextInput;
                    Target.NativeForm?.PushEvent(keypresseventInfo);
                }
                protected override void OnClosed(System.EventArgs e)
                {
                    base.OnClosed(e);
                    Target.Pointer.Dispose();
                }
                #endregion

                #region UPDATE
                public void Update(System.Drawing.Rectangle rc)
                {
                    if (InvokeRequired)
                    {
                        InvalidateSafe(rc);
                        UpdateSafe();
                    }
                    else
                    {
                        Invalidate(rc);
                        Update();
                    }
                }
                #endregion

                #region INVOKE DELGATES
                void invalidateSafe(System.Drawing.Rectangle rectangle) =>
                    Invalidate(rectangle);
                void updateSafe() =>
                    Update();
                void setTextSafe(string text) =>
                    base.Text = text;

                void InvalidateSafe(System.Drawing.Rectangle rectangle)
                {
                    Invoke(new DelInvalidate(invalidateSafe), rectangle);
                }
                void UpdateSafe()
                {
                    Invoke(new DelUpdate(updateSafe));
                }
                void SetTextSafe(string text)
                {
                    Invoke(new DelTextChange(setTextSafe), text);
                }
                delegate void DelInvalidate(System.Drawing.Rectangle rectangle);
                delegate void DelUpdate();
                delegate void DelTextChange(string text);
                #endregion

                #region DISPOSE
                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                        return;
                    base.Dispose(disposing);
                    Target.Dispose();
                }
                public void InvokePaint(ulong command, int processID = 0) =>
                    Target.NativeForm.InvokePaint(command, processID);
                #endregion
            }
            #endregion
        }
#if HideNativeObjects
    }
#endif
}
#endif