/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
#if (GWS || Window) && !Advance

using System;
using System.Collections.Generic;
using System.Linq;

#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif

namespace MnM.GWS
{
    sealed class View : Canvas, IExView
    {
        #region VARIABLES
        MemoryOccupation MemoryOccupation;
        readonly IExDrawEventArgs DrawArgs = new DrawEventArgs();
        ControlCollection controls;
        #endregion

        #region CONTRUCTORS
        public View(IRenderTarget target, bool isMultiWindow = false) :
            base(target)
        {
            MemoryOccupation.BeginMonitoring(out long total);
            MemoryOccupation.EndMonitoring(total);
            controls = new ControlCollection(this);
        }
        #endregion

        #region PROPERTIES
        public IControlCollection Controls => controls;
        public bool Visible => Target.RenderWindow.Visible;
        public bool Focused => Target.RenderWindow.Focused;
        public bool SuspendLayout
        {
            get => (viewState & ViewState.SuspendedLayout) == ViewState.SuspendedLayout
                ;
            set
            {
                if (value)
                    viewState |= ViewState.SuspendedLayout;
                else
                    viewState &= ~ViewState.SuspendedLayout;
            }
        }
        #endregion

        #region IMPLICIT INTERFACE PROPERTY IMPLEMENTATION
        IExControlCollection IExControls.Controls => controls;
        IExView IViewHolder<IExView>.View => this;
        IMemoryOccupation IMemoryOccupier.MemoryOccupation =>
            MemoryOccupation;
        IExRenderWindow IExRenderWindowHolder.RenderWindow => Target.RenderWindow;
        IRenderWindow IRenderWindowHolder.RenderWindow => Target.RenderWindow;
        IRenderTarget IRenderTargetHolder.Target => Target;
        unsafe int* IExView.Pixels
        {
            get
            {
                fixed (int* p = Pixels)
                    return p;
            }
        }
        int IExView.OriginalWidth => OriginalWidth;
        int IExView.OriginalHeight => OriginalHeight;
        IntPtr ISource<IntPtr>.Source => Source;
        int IPoint.X => 0;
        int IPoint.Y => 0;
        #endregion

        #region GET - SET CURSOR
        public void SetCursor(int x, int y, bool silent = true) { }
        public void SetCursorPos(int x, int y, bool global = false)
        {
            viewState |= ViewState.Busy;
            Target.RenderWindow.SetCursorPos(x, y, global);
            viewState &= ~ViewState.Busy;
        }
        public void SetCursor(CursorType cursor) =>
             Target.RenderWindow.SetCursorType(cursor);

        public void GetCursorPos(out int x, out int y, bool global = false) =>
             Target.RenderWindow.GetCursorPos(out x, out y, global);
        #endregion

        #region REFRESH
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Refresh(UpdateCommand command = 0)
        {
            if (
            (viewState & ViewState.Disposed) == ViewState.Disposed)
                return false;

            bool persistTemporaryDrawings = (command & UpdateCommand.RestoreScreen) != UpdateCommand.RestoreScreen;
            bool FirstShow = (viewState & ViewState.FullyLoaded) != ViewState.FullyLoaded;
            viewState |= ViewState.FullyLoaded | ViewState.Busy;

            ITypedBounds all = new UpdateArea(0, 0, Width, Height);

            if (!persistTemporaryDrawings)
                this.Clear(all, Command.SkipDisplayUpdate.Add());

            UpdateCommand cmd = 0;
            var parameters = new IParameter[] { Command.SkipDisplayUpdate.Add() };
            if (FirstShow || !persistTemporaryDrawings)
            {
                foreach (var Shape in Controls.Query(reverse: false))
                    this.Draw(Shape, parameters);
                Update(all, cmd);
            }
            if (FirstShow || persistTemporaryDrawings)
            {
                DrawArgs.Renderer = this;

                Target.RenderWindow.OnPaintImages(DrawArgs);
                cmd = DrawArgs.Session.Command.ToEnum<UpdateCommand>();
                if (DrawArgs.Session.Boundaries.Count > 0 && (cmd & UpdateCommand.SkipDisplayUpdate) != UpdateCommand.SkipDisplayUpdate)
                {
                    foreach (var UpdateRect in DrawArgs.Session.Boundaries)
                        Update(UpdateRect, cmd);
                }
            }
            viewState &= ~ViewState.Busy;
            DrawArgs.Reset();
            return true;
        }
        #endregion

        #region RESIZE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal override object Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            if (
               (viewState & ViewState.Disposed) == ViewState.Disposed ||
               (w == Width && h == Height) ||
               (w == 0 && h == 0))
                return false;

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

            if (SizeOnlyToFit && Width > w && Height > h)
                return false;

            var State = viewState;
            viewState |= ViewState.Disposed;
            var memOccupation = new MemoryOccupation();
            memOccupation.BeginMonitoring(out long total);

            bool NotLessThanOriginal = !SizeOnlyToFit && (resizeCommand & ResizeCommand.NotLessThanOriginal) == ResizeCommand.NotLessThanOriginal;

            if (SizeOnlyToFit)
            {
                if (w < Width)
                    w = Width;
                if (h < Height)
                    h = Height;
            }
            if (NotLessThanOriginal)
            {
                if (w < OriginalWidth)
                    w = OriginalWidth;
                if (h < OriginalHeight)
                    h = OriginalHeight;
            }

            int newW = w;
            int newH = h;

            int oldW = width;
            int oldH = height;

            Blocks.ResizedData(ref Pixels, newW, newH, oldW, oldH);

            width = newW;
            height = newH;
            memOccupation.EndMonitoring(total);
            MemoryOccupation = memOccupation;
            viewState = State;
            return true;
        }
        #endregion

        #region SET CURSOR TYPE
        public void SetCursorType(CursorType cursor) =>
            Target.RenderWindow.SetCursorType(cursor);
        #endregion

        #region EXISTS
        public bool Exists(IObject obj)
        {
            if (!(obj is IExControl))
                return false;
            return controls.Exists((IControl)obj);
        }
        #endregion

        #region CLONE
        public override object Clone()
        {
            if ((viewState & ViewState.Disposed) == ViewState.Disposed)
                return null;

            View target = new View(Target);

            Array.Copy(Pixels, target.Pixels, width * height);
            return target;
        }
        #endregion

        #region DISPOSE
        public override void Dispose()
        {
            if ((viewState & ViewState.Disposed) != ViewState.Disposed)
            {
                viewState |= ViewState.Disposed;
                Pixels = null;
                controls.Dispose();
            }
        }
        #endregion

        #region CONTROL COLLECTION
        sealed class ControlCollection : ExObjectCollection<IControl, IExControl>, IExControlCollection
        {
            View parent;

            #region CONSTRUCTOR
            public ControlCollection(View view)
            {
                parent = view;
            }
            #endregion

            #region PROPERTIES
            public IControl this[string name]
            {
                get
                {
                    if (name == null)
                        return null;

                    var i = Array.FindIndex(Data, (w) => w.Name == name);
                    if (i == -1) return null;
                    return Data[i];
                }
            }
            public IControl this[int index]
            {
                get => Data[index];
                set
                {
                    REPLACE_AT(index, (IExControl)value);
                }
            }

            IExControl IExControlCollection.this[int index]
            {
                get => Data[index];
                set
                {
                    REPLACE_AT(index, value);
                }
            }
            #endregion

            #region ADD
            public IControl Add(IEnumerable<IParameter> parameters, IShape shape)
            {
                if (shape == null)
                {
                    return default(IControl);
                }
                if (shape is IIndependent)
                {
                    if (shape is IExControl)
                        return (IExControl)(object)shape;
                    return default(IControl);
                }

                if (shape is IPosition<gint>)
                {
                    var i = ((IPosition<gint>)shape).Position - 1;
                    if (i >= 0 && i < Length && Equals(Data[i], shape))
                        return (IControl)(object)shape;
                }

                var widget = MAKE_COMPITIBLE(shape, parameters);
                if (!Exists(widget))
                    ADD(widget);
                return widget;
            }
            public IControl Add(IShape shape, params IParameter[] parameters) =>
                Add((IEnumerable<IParameter>)parameters, shape);
            public IControl Add(IEnumerable<IParameter> parameters, IControl Widget)
            {
                if (Widget == null)
                {
                    return default(IControl);
                }
                var widget = (IExControl)Widget;
                if (widget is IIndependent)
                {
                    if (widget is IExControl)
                        return (IExControl)(object)widget;
                    return default(IControl);
                }

                if (widget is IPosition<gint>)
                {
                    var i = ((IPosition<gint>)widget).Position - 1;
                    if (i >= 0 && i < Length && Equals(Data[i], widget))
                        return (IControl)(object)widget;
                }
                if (parameters != null)
                    widget.RefreshProperties(parameters);
                if (!Exists(widget))
                {
                    ADD(widget);
                }
                return widget;
            }
            public IControl Add(IControl widget, params IParameter[] parameters)
            {
                return Add((IEnumerable<IParameter>)parameters, widget);
            }

            internal override void ADD(IExControl widget)
            {
                if (widget is IIndependent)
                    return;

                base.ADD(widget);

                if (parent.Visible && (parent.viewState & ViewState.FullyLoaded) ==
                    ViewState.FullyLoaded)
                {
                    var parameters = new IParameter[] { Command.SkipDisplayUpdate.Add() };
                    parent.Draw(widget, parameters);
                }
            }
            #endregion

            #region ADD RANGE
            public void AddRange(IEnumerable<IShape> items)
            {
                var widgets = items.Where(s => s != null && !(s is IIndependent)).
                    Select(s => s is IExControl ? (IExControl)(object)s : MAKE_COMPITIBLE(s)).Where(s => !Exists(s));
                ADD_RANGE(widgets);
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddRange(params IShape[] items) =>
                AddRange((IEnumerable<IShape>)items);

            internal override bool ADD_RANGE(IEnumerable<IExControl> items)
            {
                int count = Length;
                base.ADD_RANGE(items);
                if (Length > count)
                {
                    var Parameters = new IParameter[] { Command.SkipDisplayUpdate.Add() };
                    for (int i = count; i < Length; i++)
                    {
                        var Shape = Data[i];
                        if (parent.Visible && (parent.viewState & ViewState.FullyLoaded) ==
                            ViewState.FullyLoaded)
                            parent.Draw(Shape, Parameters);
                    }
                }
                return Length > count;
            }
            #endregion

            #region CLEAR
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public sealed override void Clear()
            {
                parent.Clear(new UpdateArea(0, 0, parent.Width, parent.Height));
                base.Clear();
            }
            #endregion

            #region REMOVE
            internal override bool REMOVE_AT(int index)
            {
                var ok = base.RemoveAt(index);
                parent.Refresh(UpdateCommand.RestoreScreen);
                return ok;
            }
            #endregion

            #region INSERT
            public IControl Insert(int position, IShape shape, params IParameter[] parameters) =>
                Insert((IEnumerable<IParameter>)parameters, position, shape);
            public IControl Insert(IEnumerable<IParameter> parameters, int position, IShape shape)
            {
                if (shape == null)
                {
                    return default(IControl);
                }
                if (shape is IIndependent)
                {
                    if (shape is IExControl)
                        return (IExControl)(object)shape;
                    return default(IControl);
                }
                if (shape is IPosition<gint>)
                {
                    var i = ((IPosition<gint>)shape).Position - 1;
                    if (i >= 0 && i < Length && Equals(Data[i], shape))
                        return (IExControl)(object)shape;
                }
                var widget = MAKE_COMPITIBLE(shape, parameters);
                if (!Exists(widget))
                    INSERT(position, widget);
                return widget;
            }

            public IControl Insert(IEnumerable<IParameter> parameters, int position, IControl Widget)
            {
                if (Widget == null)
                {
                    return default(IControl);
                }
                var widget = (IExControl)Widget;

                if (widget is IIndependent)
                {
                    if (widget is IExControl)
                        return (IExControl)(object)widget;
                    return default(IControl);
                }
                if (widget is IPosition<gint>)
                {
                    var i = ((IPosition<gint>)widget).Position - 1;
                    if (i >= 0 && i < Length && Equals(Data[i], widget))
                        return (IExControl)(object)widget;
                }
                if (parameters != null)
                    widget.RefreshProperties(parameters);

                if (!Exists(widget))
                    INSERT(position, (IExControl)widget);
                return widget;
            }
            public IControl Insert(int position, IControl widget, params IParameter[] parameters)
            {
                return Insert((IEnumerable<IParameter>)parameters, position, widget);
            }


            internal override void INSERT(int position, IExControl widget)
            {
                if (widget is IPopup)
                    return;
                base.INSERT(position, widget);
                parent.Refresh(UpdateCommand.RestoreScreen);
            }
            #endregion

            #region REPLACE AT
            internal override bool REPLACE_AT(int index, IExControl item)
            {
                var ok = base.REPLACE_AT(index, item);
                parent.Refresh(UpdateCommand.RestoreScreen);
                return ok;
            }
            #endregion

            #region MAKE COMPITIBLE
            IExControl MAKE_COMPITIBLE(IShape @object, IEnumerable<IParameter> parameters = null)
            {
                if (@object is IExControl)
                {
                    if (parameters == null)
                        return (IExControl)@object;
                }
                var control = new Control(@object, parent, parameters);
                return control;
            }
            #endregion

            #region REFRESH
            public void Refresh(IControl widget)
            {
                ((IExDraw)widget).Draw(null, parent);
            }
            #endregion

            #region TO POSITION
            internal override int GetPosition(IControl item)
            {
                return item.Position;
            }

            internal override void SetPosition(IExControl item, int value)
            {
                item.Position = (gint)value;
            }
            #endregion
        }
        #endregion
    }
}
#endif
