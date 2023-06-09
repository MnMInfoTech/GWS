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
    #region IPOINTF
    public interface IPointF 
    {
        /// <summary>
        /// Gets X co-ordinate of the location this object.
        /// </summary>
        float X { get; }

        /// <summary>
        /// Gets Y co-ordinate of the location of this object.
        /// </summary>
        float Y { get; }
    }
    #endregion

    [StructLayout(LayoutKind.Sequential)]
    public struct PointF : IPointF, IScanPoint, IEquatable<PointF>
    {
        #region VARIABLES
        /// <summary>
        /// Gets X co-ordinate of the location this object.
        /// </summary>
        public float X;

        /// <summary>
        /// Gets Y co-ordinate of the location of this object.
        /// </summary>
        public float Y;
        const string toStr = "x:{0}, y:{1}";

        public readonly static PointF Empty = new PointF();
        #endregion

        #region CONSTRUCTORS
        public PointF(float value) :
            this(value, value)
        {
        }
        public PointF(float x, float y) : this()
        {
            X = x;
            Y = y;
        }
        public PointF(float val, float axis, bool horizontal) :
            this(horizontal ? val : axis, horizontal ? axis : val)
        { }
        public PointF(IScanPoint p) :
            this()
        {
            if (p is IPointF)
            {
                var pt = (IPointF)p;
                this = new PointF(pt.X, pt.Y);
            }
            else if (p is IPoint)
            {
                var pt = (IPoint)p;
                this = new PointF(pt.X, pt.Y);
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
                this = new PointF(X, Y);
            }
        }

        public PointF(IScanPoint p, float x, float y) :
            this(p)
        {
            X += x;
            Y += y;
        }
        PointF(IScanPoint p1, IScanPoint p2, MathOperator mathOperator) : this()
        {
            var pt1 = new PointF(p1);
            var pt2 = new PointF(p2);
            float x1 = pt1.X, y1 = pt1.Y;
            float x2 = pt2.X, y2 = pt2.Y;

            switch (mathOperator)
            {
                case MathOperator.None:
                default:
                case MathOperator.Add:
                    this = new PointF(x1 + x2, y1 + y2);
                    break;
                case MathOperator.Multiply:
                    this = new PointF(x1 * x2, y1 * y2);
                    break;
                case MathOperator.Subtract:
                    this = new PointF(x1 - x2, y1 - y2);
                    break;
                case MathOperator.Divide:
                    if (x2 == 0)
                        x2 = 1;
                    if (y2 == 0)
                        y2 = 1;
                    this = new PointF(x1 / x2, y1 / y2);
                    break;
                case MathOperator.Modulo:
                    if (x2 == 0)
                        x2 = 1;
                    if (y2 == 0)
                        y2 = 1;
                    this = new PointF(x1 % x2, y1 % y2);
                    break;
            }
        }
        #endregion

        #region COMPARER
        public static Comparison<PointF> ClockWise = new Comparison<PointF>
            ((a, b) =>
            {
                return Math.Sign(Math.Atan2(a.Y, a.X) - Math.Atan2(b.Y, b.X));
            });
        #endregion

        #region PROPERTIES
        float IPointF.X => X;
        float IPointF.Y => Y;
        #endregion

        #region OPERATOR OVERLOADING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SizeF(PointF v) =>
            new SizeF(v.X, v.Y);

        public static PointF operator -(PointF v) =>
            new PointF(-v.X, -v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF operator +(PointF p1, IScanPoint p2) =>
            new PointF(p1, p2, MathOperator.Add);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF operator -(PointF p1, IScanPoint p2) =>
            new PointF(p1, p2, MathOperator.Subtract);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF operator *(PointF p1, IScanPoint p2) =>
            new PointF(p1, p2, MathOperator.Multiply);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF operator /(PointF p1, IScanPoint p2) =>
            new PointF(p1, p2, MathOperator.Divide);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF operator *(PointF p1, float b) =>
            new PointF(p1.X * b, p1.Y * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF operator +(PointF p1, float b) =>
            new PointF(p1.X + b, p1.Y + b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF operator -(PointF p1, float b) =>
            new PointF(p1.X - b, p1.Y - b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF operator /(PointF p1, float b) =>
            new PointF(p1.X / b, p1.Y / b);
        #endregion

        #region EQUALITY
        public static bool operator ==(PointF p1, PointF p2)
        {
            return p1.Equals(p2);
        }
        public static bool operator !=(PointF p1, PointF p2)
        {
            return !p1.Equals(p2);
        }

        public override int GetHashCode()
        {
            return new { X, Y }.GetHashCode();
        }

        public bool Equals(PointF other)
        {
            return other.X == X && other.Y == Y;
        }
        public bool Equals(float x, float y) =>
            X == x && Y == y;

        public override bool Equals(object obj)
        {
            if (obj is PointF)
                return Equals((PointF)obj);

            return false;
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, X, Y);
        }
    }
}