/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
    public abstract class _Host : _Events, IHost
    {
        #region VARIABLES
        readonly KeyPressEventArgs KeyPressEventArgs = new KeyPressEventArgs();
        readonly DrawEventArgs DrawEventArgs = new DrawEventArgs();
        #endregion

        #region PROPERTIES
        public abstract string Text { get; set; }
        public bool IsDisposed { get; private set; }
        public abstract string ID { get; }
        public string Name { get; protected set; }
        public RendererFlags RendererFlags { get; protected set; }
        public virtual bool FocusOnHover { get; set; }
        public virtual int TabIndex { get; set; }
        public abstract IObjCollection Objects { get; }
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;
        public virtual IReadContext Background
        {
            get => Buffer.Background;
            set
            {
                    Buffer.Background = value;
            }
        }
        public virtual IReadContext Foreground
        {
            get => (Buffer as IForeground)?.Foreground;
            set
            {
                if (Buffer is IForeground)
                    (Buffer as IForeground).Foreground = value;
            }
        }
        public abstract IntPtr Handle { get; }
        public abstract Rectangle Bounds { get; }
        public abstract bool Focused { get; }
        protected abstract IBlock Buffer { get; }
        int ICopyable.Length => Buffer.Length;
        public bool Antialiased {

            get => Buffer.Antialiased;
            set { }
        }
        #endregion

        #region PUSH EVENT
        public override void PushEvent(IEventInfo e)
        {
#if Advanced
            Objects?.PushEvent(e);
            if (e.Status == EventUseStatus.Used)
                return;
#endif
            base.PushEvent(e);
        }
        #endregion

        #region UPDATE - INVALIDATE
        public virtual void Update() =>
            (Buffer as IUpdatable)?.Update();
        public virtual void Invalidate(int x, int y, int width, int height, bool updateImmedaite = false) =>
            Buffer.Invalidate(x, y, width, height, updateImmedaite);
        #endregion

        #region COPY FROM
        public abstract void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH, bool updateImmediate = true);
        #endregion

        #region COPY TO
        public virtual Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int destLen, int destW, int destX, int destY)
        {
            var copy = this.CompitibleRc(copyX, copyY, copyW, copyH);
            return Buffer.CopyTo(copy.X, copy.Y, copy.Width, copy.Height, destination, destLen, destW, destX, destY);
        }

        public virtual Rectangle CopyTo(IWritable block, int destX, int destY, int copyX, int copyY,
            int copyW, int copyH, bool updateImmediate = true)
        {
            var copy = this.CompitibleRc(copyX, copyY, copyW, copyH);
            return Buffer.CopyTo(block, destX, destY, copy.X, copy.Y, copy.Width, copy.Height, updateImmediate);
        }
        #endregion

        #region SHOW - HIDE
        public abstract void Show();
        public abstract void Hide();
        #endregion

        #region FOCUS
        public abstract bool Focus();
        #endregion

        #region REFRESH
        public virtual void Refresh()
        {
            if (Width == 0 || Height == 0)
                return;

            DrawEventArgs.Surface = this;
            OnPaint(DrawEventArgs);
            var r = Buffer.Settings.RecentlyDrawn;
            if (r)
                Buffer.Invalidate(r.X, r.Y, r.Width, r.Height, true);
        }
        #endregion

        #region CLEAR
        public virtual Rectangle Clear(bool updateImmediate = false)
        {
            return (Buffer as IClearable)?.Clear(updateImmediate) ?? Rectangle.Empty;
        }
        public virtual Rectangle Clear(int x, int y, int width, int height, bool updateImmediate = false)
        {
            return (Buffer as IClearable)?.Clear(x, y, width, height, updateImmediate)?? Rectangle.Empty;
        }
        #endregion

        #region RESIZE
        public abstract void Resize(int? width = null, int? height = null);
        #endregion

        #region DISPOSE
        public override void Dispose()
        {
            IsDisposed = true;
            Objects?.Dispose();
            (Buffer as IDisposable)?.Dispose();
        }
        #endregion

        #region FIND ELEMENT
#if Advanced
        public IRenderable FindElement(int x, int y) =>
            Buffer?.FindElement(x, y);
#endif
        #endregion

        #region IBUFFER
        int IWritable.Length =>
            Buffer.Length;
        int IBlock.Length => 
            Buffer.Length;

        bool IWritable.Antialiased => 
            Buffer.Antialiased;
#if Advanced

        IObjectDraw IObjectDrawer.ObjectDraw =>
            Buffer.ObjectDraw;

        unsafe byte* IAlphaSource.SourceAlphas
        {
            set => Buffer.SourceAlphas = value;
        }
        IDrawSettings2 IDrawController.Settings => 
            Buffer.Settings;
#else
        IDrawSettings IDrawController.Settings => 
            Buffer.Settings;
#endif
         void IWritable.WritePixel(int val, int axis, bool horizontal, int color, float? Alpha) =>
            Buffer.WritePixel(val, axis, horizontal, color, Alpha);

        unsafe void IWritable.WriteLine(int* source, int srcIndex, int srcW, int length, bool horizontal,
            int x, int y, float? Alpha) =>
            Buffer.WriteLine(source, srcIndex, srcW, length, horizontal, x, y, Alpha);

        public Size RotateAndScale(out int[] Data, Rotation angle, bool antiAliased = true, float scale = 1)
        {
            if(Buffer is IScalable)
            {
                return ((IScalable)Buffer).RotateAndScale(out Data, angle, antiAliased, scale);
            }
            Data = new int[0];
            return Size.Empty;
        }
        public Size Flip(out int[] Data, Flip flipMode) 
        {
            if (Buffer is IScalable)
            {
                return ((IScalable)Buffer).Flip(out Data, flipMode);
            }
            Data = new int[0];
            return Size.Empty;
        }

        void IRenderSession.Begin(IRenderable renderable, out IPen pen)
        {
            pen = null;
            (Buffer as IRenderSession)?.Begin(renderable, out pen);
        }
        void IRenderSession.End(IPen pen) =>
            (Buffer as IRenderSession)?.End(pen);

        object ICloneable.Clone() =>
            Buffer.Clone();
        #endregion
    }
}
