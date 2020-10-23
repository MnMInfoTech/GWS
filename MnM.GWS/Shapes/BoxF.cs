/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    /// <summary>
    /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
    /// Also Oppsites sides have an agle of 90 degree between them.
    /// Sides are represented in points consist of float X & Y values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BoxF : IShape, IEquatable<BoxF>, IID
    {
        #region VARIABLES
        public float X;
        public float Y;
        public float Width;
        public float Height;
        byte valid;
        string id;
        const string toStr = "x:{0}, y:{1}, w:{2}, h:{3}";
        public static readonly BoxF Empty = new BoxF();
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public BoxF(float x, float y, float w, float h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            valid = Application.True;
            id = null;
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="r">Area to copy bounds from.</param>
        public BoxF(Rectangle r) :
            this(r.X, r.Y, r.Width, r.Height)
        { }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public BoxF(RectangleF r) :
            this(r.X, r.Y, r.Width, r.Height)
        { }
        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public BoxF(VectorF xy, SizeF wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public BoxF(Vector xy, Size wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a new square with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the box.</param>
        /// <param name="y">Far top horizontal co-rodinate of the box.</param>
        /// <param name="w">Width of the box.</param>
        public BoxF(float x, float y, float w) :
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
        public static BoxF FromLTRB(float x, float y, float right, float bottom, bool correct = true)
        {
            if (!correct)
                return new BoxF(x, y, right - x, bottom - y);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return new BoxF(x, y, w, h);
        }
        #endregion

        #region PROPERTIES
        public float Right => X + Width;
        public float Bottom => Y + Height;

        /// <summary>
        /// X co-ordinate of center of this object.
        /// </summary>
        public float Cx => X + Width / 2;

        /// <summary>
        /// Y co-ordinate of center of this object.
        /// </summary>
        public float Cy => Y + Height / 2;

        public IntPtr Handle => this.ToPtr();
        RectangleF IBoundsF.Bounds => new RectangleF(X, Y, Width, Height);
        string IID<string>.ID
        {
            get
            {
                if (id == null)
                    id = "RectF".NewID();
                return id;
            }
        }
        public string Name => "RectF";
        #endregion

        #region CONTAINS
        public bool Contains(float x, float y)
        {
            return x >= X && y >= Y && x <= Right && y <= Bottom;
        }
        public bool Contains(float x, float y, int offsetX, int offsetY)
        {
            x -= offsetX;
            y -= offsetY;
            return x >= X && y >= Y && x <= Right && y <= Bottom;
        }
        #endregion

        #region EQUALITY
        public bool Equals(BoxF other)
        {
            if (valid == 0 && other.valid == 0)
                return true;
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }
        public override bool Equals(object obj)
        {
            if (obj is BoxF)
                return Equals((BoxF)obj);
            else if (obj is Rectangle)
                return Equals((BoxF)obj);
            return false;
        }
        public override int GetHashCode()
        {
            return new { X, Y, Width, Height }.GetHashCode();
        }
        #endregion

        #region OPERATORS
        public static implicit operator bool(BoxF r) =>
            r.valid != 0;
        public static bool operator ==(BoxF a, BoxF b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(BoxF a, BoxF b)
        {
            return !a.Equals(b);
        }
        #endregion

        #region IENUMERABLE
        public IEnumerable<VectorF> Figure() => this;
        public IEnumerator<VectorF> GetEnumerator()
        {
            yield return new VectorF(X, Y);
            yield return new VectorF(X, Bottom);
            yield return new VectorF(Right, Bottom);
            yield return new VectorF(Right, Y);
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, X, Y, Width, Height);
        }
        public bool Valid => Valid;
    }
}
