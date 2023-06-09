
#if(GWS ||Window) 
using System;
using System.Collections.Generic;
#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif

namespace MnM.GWS
{
    #region ICONTROL
    public partial interface IControl : IWidget
    { }
    #endregion

    #region IExCONTROL
    internal partial interface IExControl : IControl, IExWidget
    { }
    #endregion

#if !Advance
    #region CONTROL
#if DevSupport || DLLSupport
    public
#else
    internal
#endif
    sealed class Control : IExControl 
    {
        #region VARIABLES
        IEnumerable<IParameter> Properties;
        IShape UnderlyingItem;
        Location location;
        Size size;

        string name;
        gint displayPosition;
        ObjState State;
        IExParent parent;

        IExBoundary boundary = (IExBoundary)Factory.newBoundary(BoundaryKind.Boundary);
        readonly ICancelEventArgs CancelArgs = new CancelEventArgs();
        #endregion

        #region CONSTRUCTORS
        public Control(IShape underlyingItem)
        {
            UnderlyingItem = underlyingItem;
            if (UnderlyingItem is IPoint)
                location = new Location((IPoint)UnderlyingItem);

            size = new Size(underlyingItem);
            boundary = (IExBoundary)Factory.newBoundary();
        }
        public Control(IShape underlyingItem, IEnumerable<IParameter> parameters) :
            this(underlyingItem)
        {
            Properties = parameters;
        }
        internal Control(IShape shape, IExParent parent) :
            this(shape)
        {
            this.parent = parent;
        }
        internal Control(IShape shape, IExParent view, IEnumerable<IParameter> parameters) :
            this(shape, parameters)
        {
            parent = view;
        }
        #endregion

        #region PROPERTIES
        public string Name
        {
            get => name;
            set => name = value;
        }
        public IUpdateArea DrawArea => boundary;
        public int X => location.X;
        public int Y => location.Y;
        public int Width => size.Width;
        public int Height => size.Height;
        public bool Visible
        {
            get => (State & ObjState.Hidden) != ObjState.Hidden;
            set
            {
                if (value && (State & ObjState.Hidden) != ObjState.Hidden
                    || !value && (State & ObjState.Hidden) == ObjState.Hidden)
                    return;

                if (value)
                {
                    CancelArgs.Cancel = false;
                    OnVisibleChanged(CancelArgs);
                    if (CancelArgs.Cancel)
                    {
                        State &= ~ObjState.Hidden;
                        return;
                    }
                    ((IExDraw)this).Draw(null, null);
                }
                else
                {
                    CancelArgs.Cancel = false;
                    OnVisibleChanged(CancelArgs);
                    if (CancelArgs.Cancel)
                    {
                        State |= ObjState.Hidden;
                        return;
                    }
                }
            }
        }
        public bool Enabled
        {
            get => (State & ObjState.Disabled) != ObjState.Disabled;
            set
            {
                if (value && (State & ObjState.Disabled) != ObjState.Disabled
                    || !value && (State & ObjState.Disabled) == ObjState.Disabled)
                    return;

                if (value)
                    State &= ~ObjState.Disabled;
                else
                    State |= ObjState.Disabled;

                ((IExDraw)this).Draw(null, null);
            }
        }
        public IParent Parent => parent;
        #endregion

        #region EXPLICT INTERFACE PROPERTY DECLARATION
        IObject IExItem.UnderlyingItem => UnderlyingItem;
        bool IOriginCompatible.IsOriginBased => location.X == 0 && location.Y == 0;
        IExBoundary IExDrawnArea.DrawnArea => boundary;
        IUpdateArea IDrawnArea.DrawnArea => boundary;
        bool IValid.Valid => size && ((State & ObjState.Disposed) != ObjState.Disposed);
        gint IPosition<gint>.Position => displayPosition;
        gint IExPosition<gint>.Position { get => displayPosition; set => displayPosition = value; }
        #endregion

        #region DRAW
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer renderer)
        {
            if ((State & ObjState.VisibilityJustChanged) == ObjState.VisibilityJustChanged)
            {
                State &= ~ObjState.VisibilityJustChanged;

                if ((State & ObjState.Hidden) == ObjState.Hidden)
                {
                    parent.Refresh();
                    return true;
                }
            }
            if ((State & ObjState.Hidden) == ObjState.Hidden)
                return true;
            var Parameters = parameters;
            if (Properties != null)
                Parameters = Properties.AppendItems(parameters);
            (renderer ?? parent).Draw(UnderlyingItem, Parameters);
            return true;
        }
        #endregion

        #region CONTAINS
        public bool Contains(float x, float y)
        {
            return boundary?.Contains(x, y) ?? false;
        }
        #endregion

        #region EVENTS
        void OnVisibleChanged(ICancelEventArgs e) =>
            VisibleChanged?.Invoke(this, e);
        public event EventHandler<ICancelEventArgs> VisibleChanged;
        #endregion

        #region REFRESH PROPERTIES
        IPrimitiveList<IParameter> IExRefreshProperties.RefreshProperties(IEnumerable<IParameter> parameters)
        {
            Properties = parameters;
            return null;
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            location = GWS.Location.Empty;
            return this;
        }
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            UnderlyingItem = null;
            Properties = null;
            parent = null;
            boundary = null;
        }
        #endregion

        #region OBJ STATE
        [Flags]
        enum ObjState : ushort
        {
            /// <summary>
            /// Default ability.
            /// </summary>
            None = Numbers.Flag0,

            /// <summary>
            /// Tells GWS that the object is in hidden state therefore unable to be focused.
            /// </summary>
            Hidden = Numbers.Flag1,

            /// <summary>
            /// Tells GWS that the object is in disabled state therefore unable to be focused.
            /// </summary>
            Disabled = Numbers.Flag2,

            /// <summary>
            /// Indicates that the view of the object is partially visible due to its bounds exceeding the parent container bounds.
            /// </summary>
            Registered = Numbers.Flag3,

            /// <summary>
            /// Tells GWS that redrawing of the object is required.
            /// </summary>
            NeedRefresh = Numbers.Flag4,

            /// <summary>
            /// Tells GWS that visibility of the object has just changed.
            /// Which usually means refreshing the object is required.
            /// </summary>
            VisibilityJustChanged = Numbers.Flag5,

            /// <summary>
            /// Tells GWS that since view is disposed rendering process is non executable.
            /// </summary>
            Disposed = Numbers.Flag6,

            /// <summary>
            /// Tells GWS the the window can not be resized.
            /// </summary>
            NonResizable = Numbers.Flag7,

            /// <summary>
            /// Tells GWS that control will automatically resize its content whenever resized method is executed.
            /// </summary>
            AutoResizeContent = Numbers.Flag8,

            /// <summary>
            /// Tells GWS that the object does not support hover effect.
            /// </summary>
            NonHoverable = Numbers.Flag9,

            /// <summary>
            /// Tells GWS that the object does not support click operation.
            /// </summary>
            NonClickable = Numbers.Flag10,

            /// <summary>
            /// Tells GWS that the object supports click effect.
            /// </summary>
            ButtonEffect = Numbers.Flag11,
        }
        #endregion
    }
    #endregion
#endif
}
#endif