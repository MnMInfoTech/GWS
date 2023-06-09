/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    public interface IPoint
    {
        /// <summary>
        /// Gets X co-ordinate of the location this object.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets Y co-ordinate of the location of this object.
        /// </summary>
        int Y { get; }
    }
    internal interface IExPoint : IPoint
    {
        /// <summary>
        /// Gets or sets X co-ordinate of the location this object.
        /// </summary>
        new int X { get; set; }

        /// <summary>
        /// Gets or sets Y co-ordinate of the location of this object.
        /// </summary>
        new int Y { get; set; }
    }

    #region IUSER - POINT
    /// <summary>
    /// Represents an object which has an information about user defined destination.
    /// </summary>
    internal interface IUserPoint : IPoint, IParameter, IScanPoint
    { }
    #endregion

    [StructLayout(LayoutKind.Sequential)]
    public struct Point : IUserPoint 
    {
        #region VARIABLES
        /// <summary>
        /// Gets X co-ordinate of the location this object.
        /// </summary>
        public int X;

        /// <summary>
        /// Gets Y co-ordinate of the location of this object.
        /// </summary>
        public int Y;
        const string toStr = "x:{0}, y:{1}";
       
        public readonly static Point Empty = new Point();
        #endregion
         
        #region CONSTRUCTORS
        public Point(int value) :
            this(value, value)
        { }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Point(float x, float y)
        {
            X = (int)x;
            Y = (int)y;
        }

        public Point(int val, int axis, bool horizontal)
        {
            Vectors.ToXY(ref val, ref axis, ref horizontal, out X, out Y);
            this = new Point(X, Y);
        }
        public Point(IPoint p) :
            this(p?.X ?? 0, p?.Y ?? 0)
        { }
        public Point(IPoint pt, int x, int y) : this()
        {
            if (pt != null)
            {
                X = pt.X;
                Y = pt.Y;
            }
            X += x;
            Y += y;
        }
        public Point(IPoint pt, float x, float y) : this()
        {
            if (pt != null)
            {
                X = pt.X;
                Y = pt.Y;
            }
            X += (int)x;
            Y += (int)y;
        }
        public Point(IPoint pt1, IPoint pt2) : this()
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
        #endregion

        #region PROPERTIES
        public static Point One => new Point(1, 1);
        public static Point UnitX => new Point(1, 0);
        public static Point UnitY => new Point(1, 0);
        public Point Yx => new Point(Y, X);
        public static Point Maximum => new Point(int.MaxValue, int.MaxValue);
        public static Point Minimum => new Point(int.MinValue, int.MinValue);

        int IPoint.X => X;
        int IPoint.Y => Y;
        #endregion

        #region EQUALITY
        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(Point p1, Point p2)
        {
            return !p1.Equals(p2);
        }
        public override int GetHashCode()
        {
            return new { X, Y }.GetHashCode();
        }

        public bool Equals(Point other)
        {
            return other.X == X && other.Y == Y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Point)
                return Equals((Point)obj);

            return false;
        }
        #endregion

        #region OPERATOR OVERLOADING
        public static implicit operator Size(Point p) =>
            new Size(p.X, p.Y);

        public static Point operator +(Point p1, IPoint p2)
        {
            if (p2 == null)
                return p1;
            return new Point(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point operator -(Point p1, IPoint p2)
        {
            if (p2 == null)
                return p1;
           return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point operator *(Point p1, IPoint p2)
        {
            if (p2 == null)
                return p1;
            return new Point(p1.X * p2.X, p1.Y * p2.Y);
        }

        public static Point operator /(Point p1, IPoint p2)
        {
            if (p2 == null)
                return p1;
            var x = p2.X == 0 ? 1 : p2.X;
            var y = p2.Y == 0 ? 1 : p2.Y;
            return new Point(p1.X / x, p1.Y / y);
        }

        public static Point operator *(Point p1, int b) =>
            new Point(p1.X * b, p1.Y * b);

        public static Point operator +(Point p1, int b) =>
            new Point(p1.X + b, p1.Y + b);

        public static Point operator -(Point p1, int b) =>
            new Point(p1.X - b, p1.Y - b);

        public static Point operator /(Point p1, int b) =>
            new Point(p1.X / b, p1.Y / b);

        public static Point operator -(Point p1) =>
            new Point(-p1.X , -p1.Y);

        public static implicit operator bool(Point pt) =>
            pt.X != 0 || pt.Y != 0;
        #endregion

        #region COMPARER
        public static Comparison<Point> ClockWise = new Comparison<Point>
            ((a, b) =>
            {
                return Math.Sign(Math.Atan2(a.Y, a.X) - Math.Atan2(b.Y, b.X));
            });
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, X, Y);
        }
    }
}
