/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
    public class Host: _Host
    {
        #region VARIABLES
        protected readonly IWindowControl Control;
        Rectangle bounds;
        ICanvas Canvas;
        string id;
        #endregion

        #region CONSTRUCTOR
        public Host(IWindowControl control)
        {
            Control = control;
            bounds = new Rectangle(0, 0, control.Width, control.Height);
            Canvas = Factory.newCanvas(this);
            Control.Assign(this);
            id = "Parent".NewID();
        }
        #endregion

        #region PROPERTIES
        public override string ID =>
            id;
        public override IntPtr Handle => 
            Control.Handle;
        public override Rectangle Bounds => 
            bounds;
        public override bool Focused =>
            Control.Focused;
        public override string Text
        {
            get => Control.Text;
            set => Control.Text = value;
        }
        protected sealed override ISurface Buffer =>
            Canvas;
        public sealed override IObjCollection Controls => 
            Canvas.Controls;
        #endregion

        #region UPLOAD
        public override void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH)
        {
            source.CopyTo(Canvas, dstX, dstY, srcX, srcY, srcW, srcH);
            Control.Invalidate(dstX, dstY, srcW, srcH);
            Control.Refresh();
        }
        #endregion

        #region SHOW - HIDE
        public override void Show()
        {
            Control.Show();
        }

        public override void Hide()
        {
            Control.Hide();
        }
        #endregion

        #region FOCUS
        public override bool Focus() => 
            Control.Focus();
        #endregion

        #region RESIZE
        public override void Resize(int? width = null, int? height = null)
        {
            Canvas.Resize(width, height);
            OnResize(new SizeEventArgs(Canvas.Width, Canvas.Height));
            if (Control.Width != Canvas.Width || Control.Height != Canvas.Height)
                (Control as IResizable)?.Resize();
        }
        #endregion

        #region PUSH EVENT
        public sealed override void PushEvent(IEventInfo e)
        {
            Control.PushEvent(e);
            if (e.Status == EventUseStatus.Used)
                return;
            base.PushEvent(e);
        }
        #endregion

        #region EVENT OVERRIDING
        public sealed override event EventHandler<IKeyEventArgs> KeyUp
        {
            add => Control.KeyUp += value;
            remove => Control.KeyUp -= value;
        }
        public sealed override event EventHandler<IKeyEventArgs> KeyDown
        {
            add => Control.KeyDown += value;
            remove => Control.KeyDown -= value;
        }
        public sealed override event EventHandler<IKeyPressEventArgs> KeyPress
        {
            add => Control.KeyPress += value;
            remove => Control.KeyPress -= value;
        }
        public sealed override event EventHandler<IMouseEventArgs> MouseWheel
        {
            add => Control.MouseWheel += value;
            remove => Control.MouseWheel -= value;
        }
        public sealed override event EventHandler<IMouseEventArgs> Enter
        {
            add => Control.Enter += value;
            remove => Control.Enter -= value;
        }
        public sealed override event EventHandler<IMouseEventArgs> Leave
        {
            add => Control.Leave += value;
            remove => Control.Leave -= value;
        }

        public sealed override event EventHandler<IMouseEventArgs> MouseDown
        {
            add => Control.MouseDown += value;
            remove => Control.MouseDown -= value;
        }
        public sealed override event EventHandler<IMouseEventArgs> MouseUp
        {
            add => Control.MouseUp += value;
            remove => Control.MouseUp -= value;
        }
        public sealed override event EventHandler<IMouseEventArgs> MouseClick
        {
            add => Control.MouseClick += value;
            remove => Control.MouseClick -= value;
        }
        public sealed override event EventHandler<IMouseEventArgs> MouseMove
        {
            add => Control.MouseMove += value;
            remove => Control.MouseMove -= value;
        }
        public sealed override event EventHandler<IMouseEventArgs> AppClicked
        {
            add => Control.AppClicked += value;
            remove => Control.AppClicked -= value;
        }
        public sealed override event EventHandler<ISizeEventArgs> Resized
        {
            add => Control.Resized += value;
            remove => Control.Resized -= value;
        }
        public sealed override event EventHandler<IDrawEventArgs> Paint
        {
            add => Control.Paint += value;
            remove => Control.Paint -= value;
        }
        public sealed override event EventHandler<IEventInfo> EventPushed
        {
            add => Control.EventPushed += value;
            remove => Control.EventPushed -= value;
        }
        #endregion
    }
}
