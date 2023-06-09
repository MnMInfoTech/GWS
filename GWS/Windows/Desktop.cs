/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IDESKTOP-WINDOW
    /// <summary>
    /// Additional properties and methods of Windows that buffers do not have.
    /// </summary>
    public partial interface IDesktop : IOSMinimalWindow, IBaseWindow, IRenderWindow, IHandle, IID<string>, IFocus, ITextDisplayer 
    {
        /// <summary>
        /// Gets or set visibility of this window.
        /// </summary>
        new bool Visible { get; set; }

        /// Gets or set ability of this window.
        new bool Enabled { get; set; }

        /// <summary>
        /// Close the window and manage memory.
        /// </summary>
        void Close();

        new event EventHandler<IDrawEventArgs> PaintImages;
    }
    #endregion

    #region IEx-DESKTOP
    internal partial interface IExDesktop : IDesktop, IExBaseWindow, IExRenderWindow, IEventProcessor
    { }
    #endregion
}
namespace MnM.GWS
{
    #region DESKTOP
#if DevSupport
    public
#else
    internal
#endif
    partial class Desktop : BaseWindow<IView>, IExDesktop
    {
        #region VARIABLES
        const int formX = 602, formY = 200, formW = 404, formH = 506;
        IExOSWindow OSWindow;
        protected bool IsEventPusher;
        IExView PrimaryView;
        IExView CurrentView;
        GwsWindowFlags flags;
        #endregion

        #region CONSTRUCTOR
        Desktop(int width, int height) :
            base(width, height)
        {
#if MS
            WaitTime = 50;
#endif
            OSWindow = (IExOSWindow)Factory.newOSWindow();

        }
        public Desktop(string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null,
            RendererFlags? renderFlags = null) :
            this(width ?? 100, height ?? 100)
        {
            int iw = width ?? formW;
            int ih = height ?? formH;

            size = new Size(iw, ih);

            if (!OSWindow.PsuedoConstructor(this, out this.flags, title, iw, ih, x, y, flags, renderFlags: renderFlags))
                throw new Exception("Window could not be initialized!");
            //location = new Location(ix, iy);

            PrimaryView = Initialize(flags);
            if ((Flags & GwsWindowFlags.ShowImmediate) == GwsWindowFlags.ShowImmediate) 
                Show();

            memoryOccupation = new MemoryOccupation(PrimaryView.MemoryOccupation);
            memoryOccupation.KB += OSWindow.Target.MemoryOccupation.KB;
        }

        IExView Initialize(GwsWindowFlags? flags)
        {
            this.flags = flags ?? 0;
            bool mutiWindow = (Flags & GwsWindowFlags.MultiWindow) == GwsWindowFlags.MultiWindow;

            var factoryView = Factory.newView(OSWindow.Target, mutiWindow);
            if (!(factoryView is IExView))
            {
                throw new Exception("View created from factory is not compatible for this window!");
            }
            var view = (IExView)factoryView;
            CurrentView = view;
#if Window
            this.Register(OSWindow);
#endif
            if ((this.flags & GwsWindowFlags.Shown) == GwsWindowFlags.Shown)
                CurrentView.Update(ViewState.Hidden, ModifyCommand.Remove);
            else
                CurrentView.Update(ViewState.Hidden, ModifyCommand.Add);

            return view;
        }
        #endregion

        #region PROPERTIES   
        public string ID => OSWindow.ID;
        public OS OS => OSWindow.OS;
        public float Transparency { get => OSWindow.Transparency; set => OSWindow.Transparency = value; }
        public sealed override string Text
        {
            get => OSWindow.Text;
            set => OSWindow.Text = value;
        }
        public sealed override bool Focused => base.Focused && OSWindow.Focused;
        public IntPtr Handle => OSWindow.Handle;
        internal sealed override ViewState ViewState
        {
            get => CurrentView.ViewState;
            set => CurrentView.Update(value, ModifyCommand.Replace);
        }
        public IScreen Screen => OSWindow.Screen;
        public GwsWindowFlags Flags { get => flags; protected set => flags = value; }
        public sealed override IControlCollection Controls => View.Controls;
        protected sealed override IView View => CurrentView;
        ModifierKeys IInputStateHolder.ModifierKey => OSWindow.ModifierKey;
        MouseButton IInputStateHolder.MouseButton => OSWindow.MouseButton;
#if Window
        public uint PixelFormat => OSWindow.PixelFormat;
        public IGLContext GLContext => OSWindow.GLContext;
        public ISound Sound => OSWindow.Sound;
        public string LastError => OSWindow.LastError;
        public sealed override Location Location 
        {
            get => location;
            set 
            { 
                if (!Valid)
                    return;
                location = value;
                OSWindow.Location = location;
                location = new Location(OSWindow);
            }
        }
#endif
        #endregion

        #region RESIZE
        protected override bool ResizeActual(int w, int h, ResizeCommand resizeCommand)
        {
            size = new Size(w, h);
            ((IExResizable)View).Resize(w, h, out bool success, resizeCommand);
            if(success)
            {
                ((IExResizable)OSWindow.Target).Resize(w, h, out _, resizeCommand);
            }
            Resize2(w, h);
            SizeArgs.Reset(w, h);
            OnResize(SizeArgs);
            memoryOccupation = new MemoryOccupation(CurrentView.MemoryOccupation);
            memoryOccupation.KB += OSWindow.Target.MemoryOccupation.KB;
            return true;
        }
        partial void Resize2(int w, int h);
        #endregion

        #region REFRESH
        public override bool Refresh(UpdateCommand command = 0)
        {
            if (!Visible)
                return false;
            return View.Refresh(command);
        }
        #endregion

        #region FOCUS
        public override void BringToFront()
        {
            if (!Enabled || !Visible)
                return;
            OSWindow.BringToFront();
        }
        public override void SendToBack()
        {
            if (!Enabled || !Visible)
                return;
            OSWindow.SendToBack();
        }
        #endregion

        #region FOCUS
        public override bool Focus()
        {
            if (!Enabled || !Visible)
                return false;
            bool ok = OSWindow.Focus();
            if (!ok)
                return false;
            return true;
        }
        #endregion

        #region SHOW - HIDE
        public override void Show()
        {
            CancelArgs.Cancel = false;
            OnVisibleChanged(CancelArgs);
            if (!CancelArgs.Cancel)
                OSWindow.Show();
            CurrentView.Update(ViewState.Hidden, ModifyCommand.Remove);
        }
        public override void Hide(bool forceFully = false)
        {
            CancelArgs.Cancel = false;
            OnVisibleChanged(CancelArgs);
            if (!CancelArgs.Cancel)
                OSWindow.Hide();
            CurrentView.Update(ViewState.Hidden, ModifyCommand.Add);
        }
        #endregion

        #region CONTAINS
        public override bool Contains(float x, float y)
        {
            return x >= X && y >= Y && x <= X + Width && y <= Y + Height;
        }
        #endregion

        #region PROCESS EVENT
        bool IEventProcessor.ProcessEvent(IExternalEventInfo @event)
        {
            if (!(@event is IExExternalEventInfo))
                return true;

            #region WHEN BUSY IGNORE INPUT EVENTS
            if ((CurrentView.ViewState & ViewState.Busy) == ViewState.Busy)
            {
                switch (@event.Type)
                {
                    case GwsEvent.First:
                    case GwsEvent.KeyDown:
                    case GwsEvent.KeyUp:
                    case GwsEvent.KeyPress:
                    case GwsEvent.MouseMotion:
                    case GwsEvent.MouseDown:
                    case GwsEvent.MouseUp:
                    case GwsEvent.MouseWheel:
                    case GwsEvent.JoyAxisMotion:
                    case GwsEvent.JoyBallMotion:
                    case GwsEvent.JoyHatMotion:
                    case GwsEvent.JoyButtonDown:
                    case GwsEvent.JoyButtonUp:
                    case GwsEvent.ControllerAxisMotion:
                    case GwsEvent.ControllerButtonDown:
                    case GwsEvent.ControllerButtonUp:
                    case GwsEvent.FingerDown:
                    case GwsEvent.FingerUp:
                    case GwsEvent.FingerMotion:
                    case GwsEvent.DollarGesture:
                    case GwsEvent.DollarRecord:
                    case GwsEvent.MultiGesture:
                    case GwsEvent.ClipBoardUpdate:
                    case GwsEvent.DropFile:
                    case GwsEvent.UserEvent:
                    case GwsEvent.DropText:
                    case GwsEvent.DropBegin:
                    case GwsEvent.DropComplete:
                    case GwsEvent.Shown:
                    case GwsEvent.Exposed:
                    case GwsEvent.Moved:
                    case GwsEvent.SizeChanged:
                    case GwsEvent.Minimized:
                    case GwsEvent.MouseEnter:
                    case GwsEvent.MouseLeave:
                    case GwsEvent.GotFocus:
                    case GwsEvent.LostFocus:
                    case GwsEvent.Paint:
                    case GwsEvent.MouseClick:
                    case GwsEvent.Load:
                    case GwsEvent.MouseDrag:
                    case GwsEvent.LASTEVENT:
                        return true;
                    default:
                        break;
                }
            }
            #endregion

            var e = OSWindow.ParseEvent(@event);
            if (e == null)
                return false;
            var Event = new EventInfo(this, e, @event.Type);
            PushEvent(Event);
            return true;
        }
        IEventArgs IEventParser.ParseEvent(IExternalEventInfo @event) =>
            OSWindow.ParseEvent(@event);
        #endregion

        #region GET - SET CURSOR
        public override void SetCursorType(CursorType cursor) =>
            OSWindow.SetCursorType(cursor);

        public override void GetCursorPos(out int x, out int y, bool global = false) =>
            OSWindow.GetCursorPos(out x, out y, global);

        public sealed override void SetCursorPos(int x, int y, bool global = false) =>
            OSWindow.SetCursorPos(x, y, global);
        #endregion

        #region ON PROPERTY CHANGED
        protected override void OnPropertyChanged<T>(T Property, bool Silent, string Name)
        {
            switch (Name)
            {
                case "TextProperty":
                case "ITextProperty":
                case "ITextHolder":
                    OSWindow.Text = Property.Value as string;
                    break;
                default:
                    break;
            }
            base.OnPropertyChanged(Property, Silent, Name);
        }
        #endregion

#if Window
        #region CHANGE SCREEN
        public void ChangeScreen(int screenIndex) =>
            OSWindow.ChangeScreen(screenIndex);
        #endregion

        #region NEW TEXTURE
        public ITexture newTexture(int? w = null, int? h = null, bool isPrimary = false,
            TextureAccess? textureAccess = null, RendererFlags? rendererFlags = null) =>
            OSWindow.newTexture(w, h, isPrimary, textureAccess, rendererFlags);
        #endregion

        #region NEW WAV PLAYER
        public ISound newWavPlayer() =>
            OSWindow.newWavPlayer();
        #endregion
#endif

        #region CLOSE
        public void Close()
        {
            Dispose();
        }
        #endregion

        #region DISPOSE
        public sealed override void Dispose()
        {
            if (OSWindow.IsDisposed)
                return;
            OnClosed(Factory.DefaultArgs);
            PrimaryView.Dispose();
            PrimaryView = null;
            Dispose2();
            this.Deregister();
            OSWindow.Dispose();
        }
        partial void Dispose2();
        #endregion

        #region TO STRING
        public override string ToString()
        {
            return string.Format("ID: {0}, X: {1}, Y: {2}, W: {3}, H: {4}", ID, X, Y, Width, Height);
        }
        #endregion
    }
    #endregion
}
#endif
