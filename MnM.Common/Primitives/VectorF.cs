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
    public struct VectorF: IScale, IPointF
    {
        #region VARIABLES
        public float X, Y;
        public byte Quadratic;
        byte Valid;

        const string toStr = "x:{0}, y:{1}, Quadratic:{2}";

        public readonly static VectorF Empty = new VectorF();
        #endregion

        #region CONSTRUCTORS
        public VectorF(float value)
        {
            Quadratic = 0;
            if (!float.IsNaN(value))
                Valid = 1;
            else
                Valid = 0;
            X = value;
            Y = value;
        }
        public VectorF(float x, float y, byte quadratic = 0) : this()
        {
            Quadratic = quadratic;
            if (!float.IsNaN(x) && !float.IsNaN(y))
                Valid = 1;
            else
                Valid = 0;
            X = x;
            Y = y;
        }
        public VectorF(float val, float axis, bool horizontal, byte quadratic = 0) :
            this(horizontal ? val : axis, horizontal ? axis : val, quadratic)
        { }

        public VectorF(VectorF p, byte quadratic) :
            this(p.X, p.Y, quadratic)
        { }
        public VectorF(VectorF p) :
            this(p.X, p.Y)
        {
            Quadratic = p.Quadratic;
        }
        public VectorF(Vector p) :
            this(p.X, p.Y)
        {
                Quadratic = p.Quadratic;
        }
        #endregion

        #region COMPARER
        public static Comparison<VectorF> ClockWise = new Comparison<VectorF>
            ((a,b) => 
            {
                return Math.Sign(Math.Atan2(a.Y, a.X) - Math.Atan2(b.Y, b.X));
            });
        #endregion

        #region PROPERTIES
        public static VectorF UnitX = new VectorF(1f, 0);
        public static VectorF UnitY = new VectorF(0f, 1f);
        float IScale.X => X;
        float IScale.Y => Y;
        float IPointF.X => X;
        float IPointF.Y => Y;
        public bool HasScale => (X != 0 || Y != 0);
        #endregion

        #region ASSIGN
        public void Assign(float x, float y, byte? quadratic = null)
        {
            X = x;
            Y = y;
            if (quadratic != null)
                Quadratic = quadratic.Value;
        }
        #endregion

        #region ROUNDING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Ceiling()
        {
            if (Valid == 0)
                return new Vector();
            return new Vector(X.Ceiling(), Y.Ceiling(), Quadratic);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Round()
        {
            if (Valid==0)
                return new Vector();

            return new Vector(X.Round(), Y.Round(), Quadratic);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector Floor()
        {
            if (Valid == 0)
                return new Vector();

           return  new Vector((int)X, (int)Y, Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF CeilingF()
        {
            if (Valid == 0)
                return new VectorF();
            return new VectorF(X.Ceiling(), Y.Ceiling(), Quadratic);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF RoundF()
        {
            if (Valid == 0)
                return new VectorF();
            return new VectorF(X.Round(), Y.Round(), Quadratic);
        }
        public VectorF RoundF(int digits)
        {
            if (Valid == 0)
                return new VectorF();
            return new VectorF(X.Round(digits), Y.Round(digits), Quadratic);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VectorF FloorF()
        {
            if (Valid == 0)
                return new VectorF();

            return new VectorF((int)X, (int)Y, Quadratic);
        }
        #endregion

        #region OPERATOR OVERLOADING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(VectorF v)=>
            v.Valid == 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator SizeF(VectorF v) =>
            new SizeF(v.X, v.Y);
        
        public static VectorF operator -(VectorF v) =>
            new VectorF(-v.X, -v.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator +(VectorF p1, VectorF p2) =>
            new VectorF(p1.X + p2.X, p1.Y + p2.Y, (byte)(p1.Quadratic + p2.Quadratic));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator -(VectorF p1, VectorF p2) =>
            new VectorF(p1.X - p2.X, p1.Y - p2.Y, (byte)(p1.Quadratic + p2.Quadratic));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator *(VectorF p1, VectorF p2) =>
            new VectorF(p1.X * p2.X, p1.Y * p2.Y, (byte)(p1.Quadratic + p2.Quadratic));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator /(VectorF p1, VectorF p2) =>
            new VectorF(p1.X / p2.X, p1.Y / p2.Y, (byte)(p1.Quadratic + p2.Quadratic));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator *(VectorF p1, float b) =>
            new VectorF(p1.X * b, p1.Y * b, p1.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator +(VectorF p1, float b) =>
            new VectorF(p1.X + b, p1.Y + b, p1.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator -(VectorF p1, float b) =>
            new VectorF(p1.X - b, p1.Y - b, p1.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF operator /(VectorF p1, float b) =>
            new VectorF(p1.X / b, p1.Y / b, p1.Quadratic);
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
            if (ReferenceEquals(other, null))
                return false;
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
            return string.Format(toStr, X, Y, Quadratic);
        }
    }
}