/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    public interface IRectangleF : IPointF, ISizeF, IBoundsF
    { }

    /// <summary>
    /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
    /// Sides are represented in points consist of integer X & Y values.
    /// Also Oppsite sides have an agle of 90 degree between them.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RectangleF : IRectangleF, IEquatable<IRectangleF>
    {
        #region VARIABLES
        public float X, Y, Width, Height;
        public static RectangleF Empty = new RectangleF();
        static string description = "X: {0}, Y: {1}, W: {2}, H: {3}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public RectangleF(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="r">Area to copy bounds from.</param>
        public RectangleF(IBoundsF r):
            this()
        {
            r?.GetBounds(out X, out Y, out Width, out Height);
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public RectangleF(IBounds r) :
            this()
        {
            if (r == null || !r.Valid)
            {
                return;
            }
            r.GetBounds(out int x, out int y, out int w, out int h);
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public RectangleF(VectorF xy, SizeF wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public RectangleF(Vector xy, Size wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a new square with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the box.</param>
        /// <param name="y">Far top horizontal co-rodinate of the box.</param>
        /// <param name="w">Width of the box.</param>
        public RectangleF(float x, float y, float w) :
            this(x, y, w, w)
        { }

        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static RectangleF FromLTRB(float x, float y, float right, float bottom, bool correct = true)
        {
            if (!correct)
                return new RectangleF(x, y, right - x, bottom - y);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return new RectangleF(x, y, w, h);
        }
        #endregion

        #region PROPERTIES
        public float Right => X + Width;
        public float Bottom => Y + Height;
        public bool Valid => Width >= 0 && Height >= 0;
        /// <summary>
        /// X co-ordinate of center of this object.
        /// </summary>
        public float Cx => X + Width / 2;

        /// <summary>
        /// Y co-ordinate of center of this object.
        /// </summary>
        public float Cy => Y + Height / 2;
        float IPointF.X => X;
        float IPointF.Y => Y;
        float ISizeF.Width => Width;
        float ISizeF.Height => Height;
        #endregion

        #region GET BOUNDS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetBounds(out float x, out float y, out float w, out float h)
        {
            if (Width <= 0 || Height <= 0)
            {
                x = y = w = h = 0;
                return;
            }
            x = X;
            y = Y;
            w = Width;
            h = Height;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetBounds(out int x, out int y, out int w, out int h)
        {
            if (Width <= 0 || Height <= 0)
            {
                x = y = w = h = 0;
                return;
            }
            x = (int)X;
            y = (int)Y;
            w = (int)Width;
            if (Width - w != 0)
                ++w;
            h = (int)Height;
            if (Height - h != 0)
                ++h;
        }
        #endregion

        #region EQUALITY
        public bool Equals(IRectangleF other)
        {
            if (other == null)
                return false;

            return Rectangles.Equals(other, X, Y, Width, Height);
        }
        public override bool Equals(object obj)
        {
            if (obj is IRectangleF)
                return Equals((IRectangleF)obj);
            else if (obj is RectangleF)
                return Equals((RectangleF)obj);
            return false;
        }
        public override int GetHashCode()
        {
            return new { X, Y, Width, Height }.GetHashCode();
        }
        #endregion

        #region OPERATORS
        public static implicit operator bool(RectangleF r) =>
            r.Valid;
        public static bool operator ==(RectangleF a, RectangleF b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(RectangleF a, RectangleF b)
        {
            return !a.Equals(b);
        }
        public static implicit operator RectangleF(Rectangle r) =>
            new RectangleF(r.X, r.Y, r.Width, r.Height);
        #endregion

        public override string ToString()
        {
            return string.Format(description, X, Y, Width, Height);
        }

    }
}
