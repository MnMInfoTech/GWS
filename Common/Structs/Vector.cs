/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    public interface IVector : IPoint, IPointType { }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector : IPoint, IPointType, IValid, IScanPoint
    {
        #region VARIABLES
        public int X, Y;
        public PointKind Kind;
        byte valid;
        
        const string toStr = "x:{0}, y:{1}";
        public readonly static Vector Empty = new Vector();
        #endregion

        #region CONSTRUCTORS
        public Vector(int value) : 
            this(value, value) { }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
            if (x != int.MinValue && y != int.MinValue)
                valid = 1;
            else
                valid = 0;
            Kind =  PointKind.Normal;
        }
        public Vector(int x, int y, PointKind kind)
        {
            X = x;
            Y = y;
            if (x != int.MinValue && y != int.MinValue)
                valid = 1;
            else
                valid = 0;
            Kind = kind;
        }

        public Vector(float x, float y)
        {
            X = x.Round();
            Y = y.Round();
            if (x != int.MinValue && y != int.MinValue)
                valid = 1;
            else
                valid = 0;
            Kind = PointKind.Normal;
        }

        public Vector(float x, float y, PointKind kind)
        {
            X = x.Round();
            Y = y.Round();
            if (x != int.MinValue && y != int.MinValue)
                valid = 1;
            else
                valid = 0;
            Kind = kind;
        }

        public Vector(int val, int axis, bool horizontal, PointKind kind)
        {
            Vectors.ToXY(ref val, ref axis, ref horizontal, out X, out Y);
            this = new Vector(X, Y, kind);
        }
        public Vector(int val, int axis, bool horizontal) : this()
        {
            Vectors.ToXY(ref val, ref axis, ref horizontal, out X, out Y);
            this = new Vector(X, Y);
        }
        public Vector(IScanPoint p) : this()
        { 
            if(p is IPoint)
            {
                var pt = (IPoint)p;
                this = new Vector(pt.X, pt.Y);
            }
            else if (p is IPointF)
            {
                var pt = (IPointF)p;
                this = new Vector(pt.X, pt.Y);
            }
            else if (p is IAxisPoint)
            {
                var pt = (IAxisPoint)p;
                if (pt.IsHorizontal)
                {
                    float x = pt.Val;
                    X = x.Round();
                    Y = pt.Axis; 
                }
                else
                {
                    float y = pt.Val;
                    Y = y.Round();
                    X = pt.Axis;
                }
                this = new Vector(X, Y);
            }
            if (p is IPointType)
                Kind = ((IPointType)p).Kind;
        }
        public Vector(IScanPoint p, PointKind kind) :
            this(p)
        {
            Kind = kind;
        }
        public Vector(IScanPoint pt, int x, int y): this()
        {
            this = new Vector(pt);
            X += x;
            Y += y;
        }
        Vector(PointKind type) : this()
        {
            Kind = type;
        }
        Vector(IScanPoint p1, IScanPoint p2, MathOperator mathOperator) : this()
        {
            var pt1 = new Vector(p1);
            var pt2 = new Vector(p2);
            int x1 = pt1.X, y1 = pt1.Y;
            int x2 = pt2.X, y2 = pt2.Y;
            Kind = pt1.Kind | pt2.Kind;

            switch (mathOperator)
            {
                case MathOperator.None:
                default:
                case MathOperator.Add:
                    this = new Vector(x1 + x2, y1 + y2, Kind);
                    break;
                case MathOperator.Multiply:
                    this = new Vector(x1 * x2, y1 * y2, Kind);
                    break;
                case MathOperator.Subtract:
                    this = new Vector(x1 - x2, y1 - y2, Kind);
                    break;
                case MathOperator.Divide:
                    if (x2 == 0)
                        x2 = 1;
                    if (y2 == 0)
                        y2 = 1;
                    this = new Vector(x1 / x2, y1 / y2, Kind);
                    break;
                case MathOperator.Modulo:
                    if (x2 == 0)
                        x2 = 1;
                    if (y2 == 0)
                        y2 = 1;
                    this = new Vector(x1 % x2, y1 % y2, Kind);
                    break;
            }
        }
        #endregion

        #region PROPERTIES
        public static Vector One => new Vector(1, 1);
        public static Vector UnitX => new Vector(1, 0);
        public static Vector UnitY => new Vector(1, 0);
        public Vector Yx => new Vector(Y, X);
        public static Vector Break = new Vector(PointKind.Break);
        public bool Valid => valid != 0;
        int IPoint.X => X;
        int IPoint.Y => Y;
        PointKind IPointType.Kind => Kind;
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
            p.valid == 1;

        public static implicit operator Size(Vector p) =>
            new Size(p.X, p.Y);

        public static Vector operator +(Vector p1, IScanPoint p2) =>
            new Vector(p1, p2, MathOperator.Add);

        public static Vector operator -(Vector p1, IScanPoint p2) =>
            new Vector(p1, p2, MathOperator.Subtract);

        public static Vector operator *(Vector p1, IScanPoint p2) =>
            new Vector(p1, p2, MathOperator.Multiply);

        public static Vector operator /(Vector p1, IScanPoint p2) =>
            new Vector(p1, p2, MathOperator.Divide);

        public static Vector operator *(Vector p1, int b) =>
            new Vector(p1.X * b, p1.Y * b, p1.Kind);

        public static Vector operator +(Vector p1, int b) =>
            new Vector(p1.X + b, p1.Y + b, p1.Kind);

        public static Vector operator -(Vector p1, int b) =>
            new Vector(p1.X - b, p1.Y - b, p1.Kind);

        public static Vector operator /(Vector p1, int b) =>
            new Vector(p1.X / b, p1.Y / b, p1.Kind);
        #endregion

        #region VECTOR MATH
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Add(IPoint p2) =>
            new Vector(X + p2.X, Y + p2.Y, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Subtract(IPoint p2) =>
            new Vector(X - p2.X, Y - p2.Y, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Multiply(IPoint p2) =>
            new Vector(X * p2.X, Y * p2.Y, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Divide(IPoint p2) =>
            new Vector(X / p2.X, Y / p2.Y, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Multiply(int b) =>
            new Vector(X * b, Y * b, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Add(int b) =>
            new Vector(X + b, Y + b, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Subtract(int b) =>
            new Vector(X - b, Y - b, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Divide(int b) =>
            new Vector(X / b, Y / b, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Normalize()
        {
            int ls = X * X + Y * Y;
            float invNorm = 1.0f / (float)Math.Sqrt(ls);

            return new Vector((X * invNorm).Round(), (Y * invNorm).Round(), Kind);
        }
        #endregion

        #region CONVERSION 
        public static explicit operator VectorF(Vector p) =>
            new VectorF(p.X, p.Y, p.Kind);
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
}
