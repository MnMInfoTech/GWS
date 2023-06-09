/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public interface IMargin : IPoint, IProperty
    { }

    public interface IMarginHolder
    {
        /// <summary>
        /// Sets a value to determine placement offset while placing this object on its container.
        /// </summary>
        IMargin Margin { get; set; }
    }
    #region MARGIN
    public struct Margin : IMargin
    {
        public int X, Y;
        public readonly static Margin Empty = new Margin();
        public Margin(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Margin(int value)
        {
            X =  Y = value;
        }

        public Margin(IPoint p):
            this()
        {
            if (p != null)
            {
                X = p.X;
                Y = p.Y;
            }
        }
        public Margin(IPoint pt1, IPoint pt2) : this()
        {
            if (pt1 != null)
            {
                X = pt1.X;
                Y = pt1.Y;
            }
            if (pt2 != null)
            {
                X += pt2.X;
                Y += pt2.Y;
            }
        }

        int IPoint.X => X;
        int IPoint.Y => Y;
        object IValue.Value => this;


        #region OPERATOR OVERLOADING
        public static implicit operator Size(Margin p) =>
            new Size(p.X, p.Y);

        public static Margin operator +(Margin p1, IPoint p2) =>
            new Margin(p1.X + p2.X, p1.Y + p2.Y);

        public static Margin operator -(Margin p1, IPoint p2) =>
            new Margin(p1.X - p2.X, p1.Y - p2.Y);

        public static Margin operator *(Margin p1, IPoint p2) =>
            new Margin(p1.X * p2.X, p1.Y * p2.Y);

        public static Margin operator /(Margin p1, IPoint p2) =>
            new Margin(p1.X / p2.X, p1.Y / p2.Y);

        public static Margin operator *(Margin p1, int b) =>
            new Margin(p1.X * b, p1.Y * b);

        public static Margin operator +(Margin p1, int b) =>
            new Margin(p1.X + b, p1.Y + b);

        public static Margin operator -(Margin p1, int b) =>
            new Margin(p1.X - b, p1.Y - b);

        public static Margin operator /(Margin p1, int b) =>
            new Margin(p1.X / b, p1.Y / b);

        public static Margin operator -(Margin p1) =>
            new Margin(-p1.X, -p1.Y);

        public static implicit operator bool(Margin pt) =>
            pt.X != 0 || pt.Y != 0;
        #endregion


        public override string ToString() =>
            string.Format("X:{0}, Y:{1}", X, Y);
    }
    #endregion
}
