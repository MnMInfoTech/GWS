/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public interface IItemMargin : IPoint, IProperty
    { }

    #region ITEM MARGIN
    public struct ItemMargin : IItemMargin
    {
        public int X, Y;
        public static ItemMargin Empty = new ItemMargin();
        public ItemMargin(int x, int y)
        {
            X = x;
            Y = y;
        }
        public ItemMargin(IPoint p) :
            this()
        {
            if (p != null)
            {
                X = p.X;
                Y = p.Y;
            }
        }
        public ItemMargin(int value):
            this(value, value)
        {

        }
        

        int IPoint.X => X;
        int IPoint.Y => Y;
        object IValue.Value => this;

        #region OPERATOR OVERLOADING
        public static implicit operator Size(ItemMargin p) =>
            new Size(p.X, p.Y);

        public static ItemMargin operator +(ItemMargin p1, IPoint p2) =>
            new ItemMargin(p1.X + p2.X, p1.Y + p2.Y);

        public static ItemMargin operator -(ItemMargin p1, IPoint p2) =>
            new ItemMargin(p1.X - p2.X, p1.Y - p2.Y);

        public static ItemMargin operator *(ItemMargin p1, IPoint p2) =>
            new ItemMargin(p1.X * p2.X, p1.Y * p2.Y);

        public static ItemMargin operator /(ItemMargin p1, IPoint p2) =>
            new ItemMargin(p1.X / p2.X, p1.Y / p2.Y);

        public static ItemMargin operator *(ItemMargin p1, int b) =>
            new ItemMargin(p1.X * b, p1.Y * b);

        public static ItemMargin operator +(ItemMargin p1, int b) =>
            new ItemMargin(p1.X + b, p1.Y + b);

        public static ItemMargin operator -(ItemMargin p1, int b) =>
            new ItemMargin(p1.X - b, p1.Y - b);

        public static ItemMargin operator /(ItemMargin p1, int b) =>
            new ItemMargin(p1.X / b, p1.Y / b);

        public static ItemMargin operator -(ItemMargin p1) =>
            new ItemMargin(-p1.X, -p1.Y);

        public static implicit operator bool(ItemMargin pt) =>
            pt.X != 0 || pt.Y != 0;
        #endregion

        public override string ToString() =>
            string.Format("X:{0}, Y:{1}", X, Y);

    }
    #endregion
}
