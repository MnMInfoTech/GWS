/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
#if Window
    public class Window: _Host
    {
        #region VARIABLES
        IHost Parent;
        IWindowControl Control;
        #endregion

        #region CONSTRUCTORS
        public Window(IWindowControl control)
        {
            Parent = Factory.newWindow(control);
            Control = control;
        }
        #endregion

        #region PROPERTIES
        public override string Text 
        { 
            get => Parent.Text; 
            set => Parent.Text = value;
        }
        public override string ID => Parent.ID;
        public override IntPtr Handle => Parent.Handle;
        public override Rectangle Bounds => Parent.Bounds;
        public override bool Focused => Parent.Focused;
        protected override ISurface Buffer => Parent;
        public override IObjCollection Controls => 
            Parent.Controls;
        #endregion

        #region USE EVENT
        public sealed override void PushEvent(IEventInfo e)
        {
            if (Parent == null)
                return;
#if Advanced
            Controls?.PushEvent(e);
            if (e.Status == EventUseStatus.Used)
                return;
#endif
            Control.PushEvent(e);
        }
        #endregion

        #region COPY FROM
        public override void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH) =>
            Parent.CopyFrom(source, dstX, dstY, srcX, srcY, srcW, srcH);
        #endregion

        #region SHOW - HIDE
        public override void Show() => 
            Parent.Show();
        public override void Hide() => 
            Parent.Hide();
        #endregion

        #region RESIZE
        public override void Resize(int? width = null, int? height = null) =>
            Parent.Resize(width, height);
        #endregion

        #region FOCUS
        public override bool Focus() => 
            Parent.Focus();
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
#endif
}
