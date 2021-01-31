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
        class NativeTarget : Form, INativeTarget
        {
            #region VARIABLES
            /// <summary>
            /// 
            /// </summary>
            protected INativeForm Window;

            /// <summary>
            /// 
            /// </summary>
            protected volatile Bitmap Bitmap;

            /// <summary>
            /// 
            /// </summary>
            protected readonly Array<int> Pointer;

            /// <summary>
            /// Width of this object.
            /// </summary>
            protected int width;

            /// <summary>
            /// Height of this object
            /// </summary>
            protected int height;

            /// <summary>
            /// Length of one dimensional memory block this object represents.
            /// </summary>
            protected int length;

            #region EVENT ARGS
            readonly MsKeyEventArgs keyEventArgs = new MsKeyEventArgs();
            readonly MsMouseEventArgs mouseEventArgs = new MsMouseEventArgs();
            readonly MsKeyPressEventArgs keyPressEventArgs = new MsKeyPressEventArgs();

            readonly EventInfo keyeventInfo = new EventInfo();
            readonly EventInfo mouseeventInfo = new EventInfo();
            readonly EventInfo keypresseventInfo = new EventInfo();
            readonly EventInfo loadEventInfo = new EventInfo();
            #endregion
            #endregion

            #region CONSTRUCTORS
            public NativeTarget(int x, int y, int w, int h)
            {
                Pointer = new Array<int>(w, h);
                Bitmap = new Bitmap(w, h, w * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, Pointer.Handle);
                width = Pointer.Width;
                height = Pointer.Height;
                length = Pointer.Length;
                DoubleBuffered = true;
                BackColor = Color.White;
                StartPosition = FormStartPosition.Manual;
                Location = new Point(x, y);
                Size = new System.Drawing.Size(w, h);
            }
            #endregion

            #region PROPERTIES
            public string ID => Name;
            IntPtr IPixels.Source => Pointer.Handle;
            int ISize.Width => width;
            int ISize.Height => height;
            int ILength.Length => length;
            INativeForm INativeTarget.Form
            {
                set
                {
                    Window = value;
                    if (Window == null)
                        return;
                    if (Window.Width != width || Window.Height != height)
                    {
                        ((IResizable)this).Resize(Window.Width, Window.Height);
                    }
                }
            }
            protected unsafe int* Screen => (int*)Pointer.Handle;
            public override string Text
            {
                get => base.Text;
                set
                {
                    if (InvokeRequired)
                        SetTextSafe(value);
                    else
                        base.Text = value;
                }
            }
            #endregion

            #region COPY FROM
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe IRectangle CopyFrom(IntPtr source, int srcW, int srcH, int dstX, int dstY, IRectangle copyArea,
                Command Command, IntPtr alphaBytes = default(IntPtr))
            {
                if (IsDisposed)
                    return Rectangle.Empty;
                int copyX = copyArea.X;
                int copyY = copyArea.Y;
                int copyW = copyArea.Width;
                int copyH = copyArea.Height;
                var dstRc = Blocks.CopyBlock((int*)source, copyX, copyY, copyW, copyH, srcW * srcH, srcW,
                    srcH, Screen, dstX, dstY, width, length, Command, (byte*)alphaBytes);

                Update(Command, dstRc);
                return dstRc;
            }
            #endregion

            #region COPY TO       
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized)]
            public unsafe IRectangle CopyTo(IntPtr destination,
                int dstLen, int dstW, int dstX, int dstY, IRectangle copyArea, Command Command = 0)
            {
                if (IsDisposed)
                    return Rectangle.Empty;
                int copyX = copyArea.X;
                int copyY = copyArea.Y;
                int copyW = copyArea.Width;
                int copyH = copyArea.Height;

                return Blocks.CopyBlock(Screen, copyX, copyY, copyW, copyH, length,
                    width, height, (int*)destination, dstX, dstY, dstW, dstLen, Command, null);
            }
            #endregion

            #region CLEAR
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe IRectangle Clear(int clearX, int clearY, int clearW, int clearH, Command Command = 0)
            {
                bool SuspendUpdate = (Command & Command.SuspendUpdate) == Command.SuspendUpdate;
                var rc = Blocks.CopyBlock(null, clearX, clearY, clearW, clearH, length,
                       Width, Height, Screen, clearX, clearY, Width, length, Command, null);

                if (!SuspendUpdate)
                    Update(0, rc);
                return rc;
            }
            #endregion

            #region PAINT
            protected override void OnPaintBackground(PaintEventArgs e)
            {
                e.Graphics.DrawImage(Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
            }
            #endregion

            #region RESIZE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void IResizable.Resize(int? newWidth, int? newHeight)
            {
                if (IsDisposed || newWidth == null && newHeight == null)
                    return;

                var w = newWidth ?? Width;
                var h = newHeight ?? Height;

                width = w;
                height = h;
                length = width * height;
                Pointer.Resize(width, height);
                Bitmap = new Bitmap(Pointer.Width, Pointer.Height, Pointer.Width * 4,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb, Pointer.Handle);
                Window.CopyTo(Pointer.Handle, length, width, 0, 0, new Rectangle(0, 0, width, height), Command.Backdrop);
                Update(0, new Rectangle(0, 0, width, height));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void OnResizeEnd(EventArgs e)
            {
                Window.Resize(Size.Width, Size.Height);
            }
            #endregion

            #region UPDATE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Update(Command Command, IRectangle RecentlyDrawn = null)
            {
                if (RecentlyDrawn == null || !RecentlyDrawn.Valid)
                    return;

                Rectangle rc;
                if (RecentlyDrawn is IBoundary)
                    rc = new Rectangle(((IBoundary)RecentlyDrawn).GetBounds(6, 6));
                else
                    rc = new Rectangle(RecentlyDrawn);

                if (InvokeRequired)
                {
                    InvalidateSafe(new System.Drawing.Rectangle(rc.X, rc.Y, rc.Width, rc.Height));
                    UpdateSafe();
                }
                else
                {
                    Invalidate(new System.Drawing.Rectangle(rc.X, rc.Y, rc.Width, rc.Height));
                    Update();
                }
            }
            #endregion

            #region EVENT BINDING
            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                loadEventInfo.Sender = this;
                loadEventInfo.Args = Factory.EmptyArgs;
                loadEventInfo.Type = 57;//FirstShown;
                Window?.PushEvent(loadEventInfo);
            }
            protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
            {
                keyEventArgs.keyCode = e.KeyCode.ToGWSKey();
                keyEventArgs.scanCode = e.KeyValue;
                keyEventArgs.keyState = KeyState.Up;
                keyeventInfo.Sender = this;
                keyeventInfo.Args = keyEventArgs;
                keyeventInfo.Type = 5;//KeyUp;
                Window?.PushEvent(keyeventInfo);
            }
            protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
            {
                keyEventArgs.keyCode = e.KeyCode.ToGWSKey();
                keyEventArgs.scanCode = e.KeyValue;
                keyEventArgs.keyState = KeyState.Down;
                keyeventInfo.Sender = this;
                keyeventInfo.Args = keyEventArgs;
                keyeventInfo.Type = 4;//KeyDown;
                Window?.PushEvent(keyeventInfo);
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
                Window?.PushEvent(mouseeventInfo);
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
                Window?.PushEvent(mouseeventInfo);
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
                Window?.PushEvent(mouseeventInfo);
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
                Window?.PushEvent(mouseeventInfo);
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
                Window?.PushEvent(mouseeventInfo);
            }
            protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
            {
                keyPressEventArgs.keyChar = e.KeyChar;
                keypresseventInfo.Sender = this;
                keypresseventInfo.Args = keyPressEventArgs;
                keypresseventInfo.Type = 6; //TextInput;
                Window?.PushEvent(keypresseventInfo);
            }
            protected override void OnClosed(System.EventArgs e)
            {
                base.OnClosed(e);
                Pointer.Dispose();
            }
            #endregion

            #region INVOKE DELGATES
            void invalidateSafe(System.Drawing.Rectangle rectangle) =>
                Invalidate(rectangle);
            void updateSafe() =>
                Update();
            void setTextSafe(string text) =>
                base.Text = text;

            protected void InvalidateSafe(System.Drawing.Rectangle rectangle)
            {
                Invoke(new DelInvalidate(invalidateSafe), rectangle);
            }
            protected void UpdateSafe()
            {
                Invoke(new DelUpdate(updateSafe));
            }
            protected void SetTextSafe(string text)
            {
                Invoke(new DelTextChange(setTextSafe), text);
            }
            delegate void DelInvalidate(System.Drawing.Rectangle rectangle);
            delegate void DelUpdate();
            delegate void DelTextChange(string text);
            #endregion
        }
#if HideNativeObjects
    }
#endif
}
#endif