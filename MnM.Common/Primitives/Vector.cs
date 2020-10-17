/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
#if (GWS || Window)
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector : IOffset
    {
        #region VARIABLES
        public int X, Y;
        public byte Quadratic;
        byte Valid;

        const string toStr = "x:{0}, y:{1}";
        public readonly static Vector Empty = new Vector();
        #endregion

        #region CONSTRUCTORS
        public Vector(int value)
        {
            X = value;
            Y = value;
            if (value != int.MinValue)
                Valid = 1;
            else
                Valid = 0;
            Quadratic = 0;
        }

        public Vector(int x, int y, byte quadratic = 0)
        {
            X = x;
            Y = y;
            if (x != int.MinValue && y != int.MinValue)
                Valid = 1;
            else
                Valid = 0;
            Quadratic = quadratic;
        }
        public Vector(int val, int axis, bool horizontal, byte quadratic)
        {
            Vectors.ToXY(ref val, ref axis, ref horizontal, out X, out Y);
            this = new Vector(X, Y, quadratic);
        }
        public Vector(int val, int axis, bool horizontal) : this()
        {
            Vectors.ToXY(ref val, ref axis, ref horizontal, out X, out Y);
            this = new Vector(X, Y);
        }
        public Vector(Vector p) :
            this(p.X, p.Y, p.Quadratic)
        { }
        public Vector(Vector p, byte quadratic) :
            this(p.X, p.Y, quadratic)
        { }
        #endregion

        #region PROPERTIES
        public static Vector One => new Vector(1, 1);
        public static Vector UnitX => new Vector(1, 0);
        public static Vector UnitY => new Vector(1, 0);
        public Vector Yx => new Vector(Y, X);
        int IOffset.X => X;
        int IOffset.Y => Y;
        #endregion

        #region ASSIGN
        public void Assign(int x, int y, byte? quadratic = null)
        {
            X = x;
            Y = y;
            if (quadratic != null)
                Quadratic = quadratic.Value;
        }
        #endregion

        #region EQUALITY
        public static bool operator ==(Vector p1, Vector p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(Vector p1, Vector p2)
        {
            return !p1.Equals(p2);
        }
        public override int GetHashCode()
        {
            return new { X, Y }.GetHashCode();
        }

        public bool Equals(Vector other)
        {
            if (ReferenceEquals(other, null))
                return false;
            return other.X == X && other.Y == Y;
        }
        public override bool Equals(object obj)
        {
            if (obj is Vector)
                return Equals((Vector)obj);

            return false;
        }
        #endregion

        #region OPERATOR OVERLOADING
        public static implicit operator bool(Vector p) =>
            p.Valid == 1;

        public static implicit operator Size(Vector p) =>
            new Size(p.X, p.Y);

        public static Vector operator +(Vector p1, Vector p2) =>
            new Vector(p1.X + p2.X, p1.Y + p2.Y, p1.Quadratic);

        public static Vector operator -(Vector p1, Vector p2) =>
            new Vector(p1.X - p2.X, p1.Y - p2.Y, p1.Quadratic);

        public static Vector operator *(Vector p1, Vector p2) =>
            new Vector(p1.X * p2.X, p1.Y * p2.Y, p1.Quadratic);

        public static Vector operator /(Vector p1, Vector p2) =>
            new Vector(p1.X / p2.X, p1.Y / p2.Y, p1.Quadratic);

        public static Vector operator *(Vector p1, int b) =>
            new Vector(p1.X * b, p1.Y * b, p1.Quadratic);

        public static Vector operator +(Vector p1, int b) =>
            new Vector(p1.X + b, p1.Y + b, p1.Quadratic);

        public static Vector operator -(Vector p1, int b) =>
            new Vector(p1.X - b, p1.Y - b, p1.Quadratic);

        public static Vector operator /(Vector p1, int b) =>
            new Vector(p1.X / b, p1.Y / b, p1.Quadratic);
        #endregion

        #region CONVERSION 
        public static implicit operator VectorF(Vector p) =>
            new VectorF(p.X, p.Y, p.Quadratic);
        #endregion

        #region COMPARER
        public static Comparison<Vector> ClockWise = new Comparison<Vector>
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
#endif
}
