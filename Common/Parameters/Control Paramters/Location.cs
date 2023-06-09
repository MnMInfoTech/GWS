/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    public interface ILocation : IPoint, IProperty
    { }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct Location : ILocation
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

        public readonly static Location Empty = new Location();
        #endregion

        #region CONSTRUCTORS
        public Location(int value) :
            this(value, value)
        { }

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Location(float x, float y)
        {
            X = (int)x;
            Y = (int)y;
        }
        public Location(int val, int axis, bool horizontal)
        {
            Vectors.ToXY(ref val, ref axis, ref horizontal, out X, out Y);
            this = new Location(X, Y);
        }
        public Location(IPoint p) :
            this(p?.X ?? 0, p?.Y ?? 0)
        { }
        public Location(IPoint pt, int x, int y) : this()
        {
            if (pt != null)
            {
                X = pt.X;
                Y = pt.Y;
            }
            X += x;
            Y += y;
        }
        public Location(IPoint pt1, IPoint pt2) : this()
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
        public static Location One => new Location(1, 1);
        public static Location UnitX => new Location(1, 0);
        public static Location UnitY => new Location(1, 0);
        public Location Yx => new Location(Y, X);
        public static Location Maximum => new Location(int.MaxValue, int.MaxValue);
        public static Location Minimum => new Location(int.MinValue, int.MinValue);

        int IPoint.X => X;
        int IPoint.Y => Y;
        object IValue.Value => this;
        #endregion

        #region EQUALITY
        public static bool operator ==(Location p1, Location p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(Location p1, Location p2)
        {
            return !p1.Equals(p2);
        }
        public override int GetHashCode()
        {
            return new { X, Y }.GetHashCode();
        }

        public bool Equals(Location other)
        {
            if (ReferenceEquals(other, null))
                return false;
            return other.X == X && other.Y == Y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Location)
                return Equals((Location)obj);

            return false;
        }
        #endregion

        #region OPERATOR OVERLOADING
        public static implicit operator Size(Location p) =>
            new Size(p.X, p.Y);

        public static Location operator +(Location p1, IPoint p2) =>
            new Location(p1.X + p2.X, p1.Y + p2.Y);

        public static Location operator -(Location p1, IPoint p2) =>
            new Location(p1.X - p2.X, p1.Y - p2.Y);

        public static Location operator *(Location p1, IPoint p2) =>
            new Location(p1.X * p2.X, p1.Y * p2.Y);

        public static Location operator /(Location p1, IPoint p2) =>
            new Location(p1.X / p2.X, p1.Y / p2.Y);

        public static Location operator *(Location p1, int b) =>
            new Location(p1.X * b, p1.Y * b);

        public static Location operator +(Location p1, int b) =>
            new Location(p1.X + b, p1.Y + b);

        public static Location operator -(Location p1, int b) =>
            new Location(p1.X - b, p1.Y - b);

        public static Location operator /(Location p1, int b) =>
            new Location(p1.X / b, p1.Y / b);

        public static Location operator -(Location p1) =>
            new Location(-p1.X, -p1.Y);

        public static implicit operator bool(Location pt) =>
            pt.X != 0 || pt.Y != 0;
        #endregion

        #region COMPARER
        public static Comparison<Location> ClockWise = new Comparison<Location>
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
