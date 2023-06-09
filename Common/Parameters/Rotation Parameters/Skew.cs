/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    #region SKEW
    /// <summary>
    /// Represents a skew object which contains requisite information to skew an object.
    /// </summary>
    public interface ISkew: IScale, IRotationParameter
    {
        /// <summary>
        /// Rotation value in degree.
        /// </summary>
        float Degree { get; }

        /// <summary>
        /// Gets type of skew.
        /// </summary>
        SkewType Type { get; }
    }
    #endregion

    #region SKEW HOLDER
    public interface ISkewHolder
    {
        /// <summary>
        /// Gets or sets an associated skew object.
        /// </summary>
        ISkew Skew { get; }

        /// <summary>
        /// Set skew object for this object.
        /// </summary>
        /// <param name="skew"></param>
        void SetSkew(ISkew skew);
    }
    #endregion

    /// <summary>
    /// Represents a skew object which contains requisite information to skew an object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Skew : ISkew, IEquatable<ISkew>
    {
        #region VARAIBLES
        /// <summary>
        /// Type of skew.
        /// </summary>
        public readonly SkewType Type;

        /// <summary>
        /// Rotation value in degree.
        /// </summary>
        public readonly float Degree;

        /// <summary>
        /// Scale factor for X co-ordinate.
        /// </summary>
        public readonly float ScaleX;

        /// <summary>
        /// Scale factor for Y co-ordinate.
        /// </summary>
        public readonly float ScaleY;

        public readonly static Skew Empty = new Skew();

        const string toStr = "Skew (Type : {0}, Degree : {1}, Scale X : {2}, Scale Y : {3})";
        #endregion

        #region CONSTRUCTORS
        public Skew(float degree, SkewType skewType)
        {
            Degree = degree;
            Type = Angles.GetScale(degree, skewType, out ScaleX, out ScaleY);
        }
        public Skew(ISkew skew)
        {
            Degree = skew?.Degree ?? 0;
            Type = skew?.Type ?? 0;
            ScaleX = skew?.X ?? 0;
            ScaleY = skew?.Y ?? 0;
        }
        public Skew(ISkew skew, SkewType? type = null, float? skewDegree = null)
        {
            Degree = skewDegree ?? skew?.Degree ?? 0;
            Type = type ?? skew?.Type ?? 0;
            ScaleX = ScaleY = 0;

            if (type != null || skewDegree != null)
            {
                Type = Angles.GetScale(Degree, Type, out ScaleX, out ScaleY);
                return;
            }
            if (skew == null || !skew.HasScale)
                return;
            Degree = skew.Degree;
            Type = skew.Type;
            ScaleX = skew.X;
            ScaleY = skew.Y;
        }
        #endregion

        #region PROPERTIES
        public bool HasScale => Type != 0 && (ScaleX != 0 || ScaleY != 0);
        public bool HasAngle => Degree != 0 && Degree != 0.001f && Degree != 360 && Degree != -360;

        float IScale.X => ScaleX;
        float IScale.Y => ScaleY;
        SkewType ISkew.Type => Type;
        float ISkew.Degree => Degree;
        object IValue.Value => this;
        #endregion

        #region EQUALITY
        public bool Equals(ISkew other)
        {
            if (other == null)
                return false;
            return other.Type == Type && other.X == ScaleX && other.Y == ScaleY;
        }
        public override int GetHashCode()
        {
            return new { Type, ScaleX, ScaleY }.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is ISkew)
                return ((ISkew)obj).Equals(this);
            return false;
        }
        #endregion

        #region OPERATORS
        public static implicit operator bool(Skew angle) =>
            angle.HasScale;

        public static bool operator ==(Skew a, Skew b) =>
            a.Equals(b);
        public static bool operator !=(Skew a, Skew b) =>
            !a.Equals(b);
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, Type, Degree, ScaleX, ScaleY);
        }
    }

    partial class Parameters
    {
        #region TO SKEW
        public static ISkew ToSkew(this SkewType skewType, float degree) =>
            new Skew(degree, skewType);
        public static ISkew ToSkew(this SkewType? skewType, float degree) =>
            new Skew(degree, skewType ?? 0);
        public static ISkew ToSkew(this SkewType? skewType, float? degree = null) =>
            new Skew(degree ?? 0, skewType ?? 0);
        public static ISkew ToSkew(this ISkew skew) =>
            new Skew(skew);
        public static ISkew ToSkew(this ISkew skew, SkewType? type = null, float? skewDegree = null) =>
            new Skew(skew, type, skewDegree);
        #endregion
    }
}
