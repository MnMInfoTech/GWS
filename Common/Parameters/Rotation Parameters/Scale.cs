/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;

namespace MnM.GWS
{
    #region ISCALE
    public interface IScale : IProperty
    {
        /// <summary>
        /// Gets value to scale X co-ordinate.
        /// </summary>
        float X { get; }

        /// <summary>
        /// Gets value to scale Y co-ordinate.
        /// </summary>
        float Y { get; }

        /// <summary>
        /// Indicates if this object has valid scaling factors.
        /// </summary>
        bool HasScale { get; }
    }
    #endregion

    public struct Scale : IScale, IEquatable<IScale>
    {
        #region VARIABLES
        /// <summary>
        /// Scale factor for X co-ordinate.
        /// </summary>
        public readonly float X;

        /// <summary>
        /// Scale factor for Y co-ordinate.
        /// </summary>
        public readonly float Y;

        public static readonly Scale Empty = new Scale();
        #endregion

        #region CONSTRUCTORS
        public Scale(float? scaleX, float? scaleY)
        {
            X = scaleX ?? 0;
            Y = scaleY ?? 0;
        }
        public Scale(float scaleX, float scaleY)
        {
            X = scaleX;
            Y = scaleY;
        }
        public Scale(IPointF p)
        {
            X = p.X;
            Y = p.Y;
        }
        public Scale(IPoint p)
        {
            X = p.X;
            Y = p.Y;
        }
        public Scale(float value)
        {
            X = Y = value;
        }
        public Scale(ISize currentSize, int newWidth, int newHeight)
        {
            X = newWidth / (float)currentSize.Width;
            Y = newHeight / (float)currentSize.Height;
        }
        public Scale(int currentWidth, int currentHeight, int newWidth, int newHeight)
        {
            X = newWidth / (float)currentWidth;
            Y = newHeight / (float)currentHeight;
        }
        #endregion

        #region PROPERTIES
        public bool HasScale => ((X != 0 || Y != 0) && (X != 1 || Y != 1));
        float IScale.X => X;
        float IScale.Y => Y;
        object IValue.Value => this;
        #endregion

        #region EQUALITY
        public bool Equals(IScale other)
        {
            return other.X == X && other.Y == Y;
        }
        public override int GetHashCode()
        {
            return new { X, Y }.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IScale)
                return ((IScale)obj).Equals(this);
            return false;
        }
        #endregion

        #region OPERATORS
        public static implicit operator bool(Scale angle) =>
            angle.HasScale;

        public static bool operator ==(Scale a, Scale b) =>
            a.Equals(b);
        public static bool operator !=(Scale a, Scale b) =>
            !a.Equals(b);
        #endregion

        public override string ToString()
        {
            return "ScaleX: " + X + ", ScaleY: " + Y;
        }
    }

}
