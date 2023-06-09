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
    [StructLayout(LayoutKind.Sequential)]
    public struct VectorF : IPointF, IPointType, IScanPoint, IValid, IEquatable<VectorF>
    {
        #region VARIABLES
        public readonly float X, Y;
        public readonly PointKind Kind;
        byte valid;
        const string toStr = "x:{0}, y:{1}, Quadratic:{2}";
        public readonly static VectorF Empty = new VectorF();
        public static VectorF UnitX = new VectorF(1f, 0);
        public static VectorF UnitY = new VectorF(0f, 1f);
        public static VectorF Break = new VectorF(PointKind.Break);
        public static VectorF Segment = new VectorF(PointKind.Segment);
        public static VectorF Bresenham = new VectorF(PointKind.Bresenham);
        #endregion

        #region CONSTRUCTORS
        public VectorF(float value) :
            this(value, value)
        {
        }
        public VectorF(float x, float y) : this()
        {
            Kind = PointKind.Normal;
            if (float.IsNaN(x) || float.IsNaN(y))
            {
                valid = 0;
                return;
            }
            X = x;
            Y = y;
            valid = 1;
        }
        public VectorF(float x, float y, PointKind kind) : this()
        {
            Kind = kind;
            if (float.IsNaN(x) || float.IsNaN(y))
            {
                valid = 0;
                return;
            }
            X = x;
            Y = y;
            valid = 1;
        }
        public VectorF(float val, float axis, bool horizontal, PointKind kind = PointKind.Normal) :
            this(horizontal ? val : axis, horizontal ? axis : val, kind)
        { }

        public VectorF(IScanPoint p, PointKind kind) :
            this(p)
        {
            Kind = kind;
        }
        public VectorF(IScanPoint p) :
            this()
        {
            if (p is IPointF)
            {
                var pt = (IPointF)p;
                this = new VectorF(pt.X, pt.Y);
            }
            else if (p is IPoint)
            {
                var pt = (IPoint)p;
                this = new VectorF(pt.X, pt.Y);
            }
            else if (p is IAxisPoint)
            {
                var pt = (IAxisPoint)p;
                if (pt.IsHorizontal)
                {
                    X = pt.Val;
                    Y = pt.Axis;
                }
                else
                {
                    Y = pt.Val;
                    X = pt.Axis;
                }
                this = new VectorF(X, Y);
            }
            if (p is IPointType)
                Kind = ((IPointType)p).Kind;
        }
       
        public VectorF(IScanPoint p, float x, float y) :
            this(p)
        {
            X += x;
            Y += y;
        }
        VectorF(PointKind type) : this()
        {
            Kind = type;
        }
        VectorF(IScanPoint p1, IScanPoint p2, MathOperator mathOperator) : this()
        {
            var pt1 = new VectorF(p1);
            var pt2 = new VectorF(p2);
            float x1 = pt1.X, y1 = pt1.Y;
            float x2 = pt2.X, y2 = pt2.Y;
            Kind = pt1.Kind | pt2.Kind;

            switch (mathOperator)
            {
                case MathOperator.None:
                default:
                case MathOperator.Add:
                    this = new VectorF(x1 + x2, y1 + y2, Kind);
                    break;
                case MathOperator.Multiply:
                    this = new VectorF(x1 * x2, y1 * y2, Kind);
                    break;
                case MathOperator.Subtract:
                    this = new VectorF(x1 - x2, y1 - y2, Kind);
                    break;
                case MathOperator.Divide:
                    if (x2 == 0)
                        x2 = 1;
                    if (y2 == 0)
                        y2 = 1;
                    this = new VectorF(x1 / x2, y1 / y2, Kind);
                    break;
                case MathOperator.Modulo:
                    if (x2 == 0)
                        x2 = 1;
                    if (y2 == 0)
                        y2 = 1;
                    this = new VectorF(x1 % x2, y1 % y2, Kind);
                    break;
            }
        }

        VectorF(float x, float y, PointKind type1, PointKind type2) :
            this(x, y, type1 | type2)
        { }
        #endregion

        #region COMPARER
        public static Comparison<VectorF> ClockWise = new Comparison<VectorF>
            ((a, b) =>
            {
                return Math.Sign(Math.Atan2(a.Y, a.X) - Math.Atan2(b.Y, b.X));
            });
        #endregion

        #region PROPERTIES
        float IPointF.X => X;
        float IPointF.Y => Y;
        PointKind IPointType.Kind => Kind;
        public bool Valid => valid != 0;
        #endregion

        #region ROUNDING
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Ceiling()
        {
            return new Vector(X.Ceiling(), Y.Ceiling(), Kind);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Round()
        {
            return new Vector(X.Round(), Y.Round(), Kind);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Floor()
        {
            return new Vector((int)X, (int)Y, Kind);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF CeilingF()
        {
            return new VectorF(X.Ceiling(), Y.Ceiling(), Kind);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF RoundF()
        {
            return new VectorF(X.Round(), Y.Round(), Kind);
        }
        public VectorF RoundF(int digits)
        {
            return new VectorF(X.Round(digits), Y.Round(digits), Kind);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF FloorF()
        {
            return new VectorF((int)X, (int)Y, Kind);
        }
        #endregion

        #region OPERATOR OVERLOADING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(VectorF v) =>
           v.valid != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SizeF(VectorF v) =>
            new SizeF(v.X, v.Y);

        public static VectorF operator -(VectorF v) =>
            new VectorF(-v.X, -v.Y, v.Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator +(VectorF p1, IScanPoint p2) =>
            new VectorF(p1, p2, MathOperator.Add);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator -(VectorF p1, IScanPoint p2) =>
            new VectorF(p1, p2, MathOperator.Subtract);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator *(VectorF p1, IScanPoint p2) =>
            new VectorF(p1, p2, MathOperator.Multiply);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator /(VectorF p1, IScanPoint p2) =>
            new VectorF(p1, p2, MathOperator.Divide);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator *(VectorF p1, float b) =>
            new VectorF(p1.X * b, p1.Y * b, p1.Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator +(VectorF p1, float b) =>
            new VectorF(p1.X + b, p1.Y + b, p1.Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator -(VectorF p1, float b) =>
            new VectorF(p1.X - b, p1.Y - b, p1.Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator /(VectorF p1, float b) =>
            new VectorF(p1.X / b, p1.Y / b, p1.Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF SquareRoot() =>
            new VectorF((float)Math.Sqrt(X), (float)Math.Sqrt(Y), Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Normalize()
        {
            float ls = X * X + Y * Y;
            float invNorm = 1.0f / (float)Math.Sqrt(ls);

            return new VectorF(X * invNorm, Y * invNorm, Kind);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Add(IPointF p2) =>
            new VectorF(X + p2.X, Y + p2.Y, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Subtract(IPointF p2) =>
            new VectorF(X - p2.X, Y - p2.Y, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Multiply(IPointF p2) =>
            new VectorF(X * p2.X, Y * p2.Y, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Divide(IPointF p2) =>
            new VectorF(X / p2.X, Y / p2.Y, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Multiply(float b) =>
            new VectorF(X * b, Y * b, Kind);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Add(float b) =>
            new VectorF(X + b, Y + b, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Subtract(float b) =>
            new VectorF(X - b, Y - b, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Divide(float b) =>
            new VectorF(X / b, Y / b, Kind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF Negate() =>
            new VectorF(-X, -Y, Kind);
        #endregion

        #region EQUALITY
        public static bool operator ==(VectorF p1, VectorF p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(VectorF p1, VectorF p2)
        {
            return !p1.Equals(p2);
        }

        public override int GetHashCode()
        {
            return new { X, Y }.GetHashCode();
        }

        public bool Equals(VectorF other)
        {
            return other.X == X && other.Y == Y;
        }
        public bool Equals(float x, float y) =>
            X == x && Y == y;

        public override bool Equals(object obj)
        {
            if (obj is VectorF)
                return Equals((VectorF)obj);

            return false;
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, X, Y, Kind);
        }
    }
}