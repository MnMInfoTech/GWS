/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if(GWS || Window)

namespace MnM.GWS
{
    /// <summary>
    /// Represents an argument object to relay mouse input information.
    /// </summary>
    public interface IMouseEventArgs : ICancelEventArgs, IPoint
    {
        /// <summary>
        /// Indicates state of mouse - i.e up or down or hovering or clicked etc.
        /// </summary>
        MouseState Status { get; }

        /// <summary>
        /// Indicates which butten is currently in play i.e left, right or middle etc.
        /// </summary>
        MouseButton Button { get; }
    }

    internal interface IExMouseEventArgs: IMouseEventArgs, IExPoint
    {
        new MouseState Status { get; set; }
        new MouseButton Button { get; set; }
    }
    public class MouseEventArgs: CancelEventArgs, IExMouseEventArgs
    {
        const string toStr = "X:{0}, Y:{1}";

        public MouseEventArgs() { }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        public MouseEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }

        public MouseEventArgs(int x, int y, MouseState state):
            this(x, y)
        {
            Status = state;
        }
       
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> instance to clone.</param>
        public MouseEventArgs(IMouseEventArgs args)
            : this(args.X, args.Y)
        {
            Button = args.Button;
            Status = args.Status;
        }

        public MouseState Status  {get; protected set;}
        public MouseButton Button  {get; protected set;}
        public virtual int X { get; protected set; }
        public virtual int Y { get; protected set; }
        int IExPoint.X { get => X; set => X = value; }
        int IExPoint.Y { get => Y; set => Y = value; }
        MouseButton IExMouseEventArgs.Button { get => Button; set => Button = value; }
        MouseState IExMouseEventArgs.Status
        {
            get => Status;
            set
            {
                if ((value & MouseState.Down) == MouseState.Down)
                    value &= ~MouseState.Up;
                if ((value & MouseState.Up) == MouseState.Up)
                    value &= ~MouseState.Down;
                if ((value & MouseState.Enter) == MouseState.Enter)
                    value &= ~MouseState.Leave;

                Status |= value;
            }
        }
        public override string ToString() => string.Format(toStr, X, Y);

    }

    public interface IMouseWheelEventArgs : ICancelEventArgs, IPoint
    {
        /// <summary>
        /// Indicates state of mouse - i.e up or down or hovering or clicked etc.
        /// </summary>
        MouseState Status { get; }

        /// <summary>
        ///Indicates a signed count of the number of detents the mouse wheel has rotated
        ///in horizontal direction.
        /// </summary>
        int XMove { get; }

        /// <summary>
        ///Indicates a signed count of the number of detents the mouse wheel has rotated
        ///in vertical direction.   
        /// </summary>
        int YMove { get; }
    }

    internal interface IExMouseWheelEventArgs : IMouseWheelEventArgs, IExPoint
    {
        /// <summary>
        ///Indicates a signed count of the number of detents the mouse wheel has rotated
        ///in horizontal direction.
        /// </summary>
        new int XMove { get; set; }

        /// <summary>
        ///Indicates a signed count of the number of detents the mouse wheel has rotated
        ///in vertical direction.   
        /// </summary>
        new int YMove { get; set; }
    }
    public sealed class MouseWheelEventArgs : CancelEventArgs, IExMouseWheelEventArgs
    {
        const string toStr = "X:{0}, Y:{1}";

        public MouseWheelEventArgs() { }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        public MouseWheelEventArgs(int x, int y) 
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> instance to clone.</param>
        public MouseWheelEventArgs(IPoint args) 
        {
            X = args.X;
            Y = args.Y;
            if(args is IMouseWheelEventArgs)
            {
                XMove = ((IMouseWheelEventArgs)args).XMove;
                YMove = ((IMouseWheelEventArgs)args).YMove;
            }
        }

        public MouseState Status => MouseState.Wheel;

        public int X { get; private set; }
        public int Y { get; private set; }

        public int XMove { get; private set; }
        public int YMove { get; private set; }

        int IExPoint.X { get => X; set => X = value; }
        int IExPoint.Y { get => Y; set => Y = value; }

        int IExMouseWheelEventArgs.XMove { get => XMove; set => XMove = value; }
        int IExMouseWheelEventArgs.YMove { get => YMove; set => YMove = value; }

        public override string ToString() => string.Format(toStr, X, Y);
    }

    public interface IMouseEnterLeaveEventArgs: IMouseEventArgs
    {
        IObject OtherObject { get; }
    }
    internal interface IExMouseEnterLeaveEventArgs : IMouseEnterLeaveEventArgs, IExMouseEventArgs
    {
        new IObject OtherObject { get; set; }
    }

    public sealed class MouseEnterLeaveEventArgs: MouseEventArgs, IExMouseEnterLeaveEventArgs
    {
        IObject otherObject;

        #region CONSTRUCTORS
        public MouseEnterLeaveEventArgs() { }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        public MouseEnterLeaveEventArgs(int x, int y) :
            base(x, y)
        { }

        public MouseEnterLeaveEventArgs(int x, int y, MouseState state) :
            base(x, y, state)
        { }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> instance to clone.</param>
        public MouseEnterLeaveEventArgs(IMouseEventArgs args)
            : base(args)
        { }
        #endregion

        public IObject OtherObject => otherObject;
        IObject IExMouseEnterLeaveEventArgs.OtherObject { get => otherObject; set => otherObject = value; }
    }
}
#endif