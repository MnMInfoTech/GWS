/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    #region IOFFSET
    /// <summary>
    /// Represents an object which has an information about destination.
    /// when using, please mind that any subsequent addition of this 
    /// object into parameter list will do the values addition and not value replacement.
    /// i.e if previous offset is (1, 1) and you include another one say(4,4)
    /// the resultant offset when parsed would be (5, 5).
    /// </summary>
    public interface IOffset : IPoint, IInLineParameter
    { }
    #endregion

    #region IOFFSET-HOLDER
    public interface IOffsetHolder
    {
        IOffset Offset { get; }
    }
    #endregion

    [StructLayout(LayoutKind.Sequential)]
    public struct Offset : IOffset
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

        public readonly static Offset Empty = new Offset();
        #endregion

        #region CONSTRUCTORS
        public Offset(int value) :
            this(value, value)
        { }

        public Offset(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Offset(float x, float y)
        {
            X = x.Round();
            Y = y.Round();
        }

        public Offset(int val, int axis, bool horizontal)
        {
            Vectors.ToXY(ref val, ref axis, ref horizontal, out X, out Y);
            this = new Offset(X, Y);
        }
        public Offset(IPoint p) :
            this(p?.X ?? 0, p?.Y ?? 0)
        { }
        public Offset(IPoint pt, int x, int y) : this()
        {
            if (pt != null)
            {
                X = pt.X;
                Y = pt.Y;
            }
            X += x;
            Y += y;
        }
        public Offset(IPoint pt1, IPoint pt2) : this()
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
        public static Offset One => new Offset(1, 1);
        public static Offset UnitX => new Offset(1, 0);
        public static Offset UnitY => new Offset(1, 0);
        public Offset Yx => new Offset(Y, X);
        public static Offset Maximum => new Offset(int.MaxValue, int.MaxValue);
        public static Offset Minimum => new Offset(int.MinValue, int.MinValue);

        int IPoint.X => X;
        int IPoint.Y => Y;
        #endregion

        #region EQUALITY
        public static bool operator ==(Offset p1, Offset p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(Offset p1, Offset p2)
        {
            return !p1.Equals(p2);
        }
        public override int GetHashCode()
        {
            return new { X, Y }.GetHashCode();
        }

        public bool Equals(Offset other)
        {
            if (ReferenceEquals(other, null))
                return false;
            return other.X == X && other.Y == Y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Offset)
                return Equals((Offset)obj);

            return false;
        }
        #endregion

        #region OPERATOR OVERLOADING
        public static implicit operator Size(Offset p) =>
            new Size(p.X, p.Y);
        public static explicit operator Location(Offset p) =>
            new Location(p.X, p.Y);

        public static Offset operator +(Offset p1, Offset p2) =>
            new Offset(p1.X + p2.X, p1.Y + p2.Y);

        public static Offset operator -(Offset p1, Offset p2) =>
            new Offset(p1.X - p2.X, p1.Y - p2.Y);

        public static Offset operator *(Offset p1, Offset p2) =>
            new Offset(p1.X * p2.X, p1.Y * p2.Y);

        public static Offset operator /(Offset p1, Offset p2) =>
            new Offset(p1.X / p2.X, p1.Y / p2.Y);

        public static Offset operator *(Offset p1, int b) =>
            new Offset(p1.X * b, p1.Y * b);

        public static Offset operator +(Offset p1, int b) =>
            new Offset(p1.X + b, p1.Y + b);

        public static Offset operator -(Offset p1, int b) =>
            new Offset(p1.X - b, p1.Y - b);

        public static Offset operator /(Offset p1, int b) =>
            new Offset(p1.X / b, p1.Y / b);

        public static Offset operator -(Offset p1) =>
            new Offset(-p1.X, -p1.Y);

        public static implicit operator bool(Offset pt) =>
            pt.X != 0 || pt.Y != 0;
        #endregion

        #region COMPARER
        public static Comparison<Offset> ClockWise = new Comparison<Offset>
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

