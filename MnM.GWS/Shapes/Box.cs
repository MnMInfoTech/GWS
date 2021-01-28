/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
#if HideGWSObjects
    partial class NativeFactory
    {
        /// <summary>
        /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
        /// Sides are represented in points consist of integer X & Y values.
        /// Also Oppsite sides have an agle of 90 degree between them.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
#else
    /// <summary>
    /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
    /// Sides are represented in points consist of integer X & Y values.
    /// Also Oppsite sides have an agle of 90 degree between them.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public
#endif
        partial struct Box : IBox, IEquatable<Rectangle>
        {
            #region VARIABLES
            /// <summary>
            /// Far left horizontal corodinate of this object.
            /// </summary>
            public int X;

            /// <summary>
            /// Far top vertical corodinate of this object.
            /// </summary>
            public int Y;

            /// <summary>
            /// far right horizontal corodinate (X + Width) of this object.
            /// </summary>
            public int Width;

            /// <summary>
            /// Deviation from the far top vertical corodinate (Y) of this object.
            /// </summary>
            public int Height;

            /// <summary>
            /// Empty instance of this object.
            /// </summary>
            public static readonly Box Empty = new Box();

            byte valid;
            int id;
            const string toStr = "x:{0}, y:{1}, w:{2}, h:{3}";
            #endregion

            #region CONSTRUCTORS
            /// <summary>
            /// Creates a new rect with specifed parameters.
            /// </summary>
            /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
            /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
            /// <param name="w">Width of the rectangle.</param>
            /// <param name="h">Height of the rectangle.</param>
            public Box(int x, int y, int w, int h): this()
            {
                X = x;
                Y = y;
                Width = w;
                Height = h;
                if (x == int.MinValue || y == int.MinValue || w <= 0 || h <= 0)
                    valid = Application.False;
                else
                    valid = Application.True;
            Name = TypeName.NewName();
        }

        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public Box(float x, float y, float w, float h) :
                this(x.Round(), y.Round(), w.Round(), h.Round())
            { }

            /// <summary>
            /// Creates a new rect identical to the area of specifed rectangle.
            /// </summary>
            /// <param name="area">Area to copy bounds from.</param>
            public Box(Rectangle area) :
                this(area.X, area.Y, area.Width, area.Height)
            { }

            /// <summary>
            /// Creates a new rect identical to the area of specifed rectangle.
            /// </summary>
            /// <param name="area">Area to copy bounds from.</param>
            public Box(RectangleF area) :
                    this(area.X, area.Y, area.Width, area.Height)
            { }

            /// <summary>
            /// Creates a box matching the specifiedlocation and size.
            /// </summary>
            /// <param name="xy">Location of the box.</param>
            /// <param name="wh">Size of the box.</param>
            public Box(VectorF xy, SizeF wh) :
                this(xy.X, xy.Y, wh.Width, wh.Height)
            { }

            /// <summary>
            /// Creates a box matching the specifiedlocation and size.
            /// </summary>
            /// <param name="xy">Location of the box.</param>
            /// <param name="wh">Size of the box.</param>
            public Box(Vector xy, Size wh) :
                this(xy.X, xy.Y, wh.Width, wh.Height)
            { }

            /// <summary>
            /// Creates a new square with specifed parameters.
            /// </summary>
            /// <param name="x">Far left horizontal co-rodinate of the box.</param>
            /// <param name="y">Far top horizontal co-rodinate of the box.</param>
            /// <param name="w">Width of the box.</param>
            public Box(float x, float y, float w) :
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
            public static Box FromLTRB(float x, float y, float right, float bottom, bool correct = true)
            {
                if (!correct)
                    return new Box(x, y, right - x, bottom - y);
                Numbers.Order(ref x, ref right);
                Numbers.Order(ref y, ref bottom);
                var w = right - x;
                if (w == 0)
                    w = 1;
                var h = bottom - y;
                if (h == 0)
                    h = 1;
                return new Box(x, y, w, h);
            }
            #endregion

            #region PROPERTIES
            /// <summary>
            /// far right horizontal corodinate (X + Width) of this object.
            /// </summary>
            public int Right => X + Width;

            /// <summary>
            /// far bottom horizontal corodinate (Y + Height) of this object.
            /// </summary>
            public int Bottom => Y + Height;

            /// <summary>
            /// X co-ordinate of center of this object.
            /// </summary>
            public int Cx => X + Width / 2;

            /// <summary>
            /// Y co-ordinate of center of this object.
            /// </summary>
            public int Cy => Y + Height / 2;

        public int ID
        {
            get
            {
                if (id == 0)
                    id = this.NewID();
                return id;
            }
        }
        public string TypeName => "Rect";
        public string Name { get; private set; }
        int IPoint.X => X;
            int IPoint.Y => Y;
            int ISize.Width => Width;
            int ISize.Height => Height;
            bool IRectangle.Valid => Width != 0 && Height != 0;
            #endregion

            #region CONTAINS
            public bool Contains(int x, int y)
            {
                return x >= X && y >= Y && x <= Right && y <= Bottom;
            }

            public bool ContainsX(int x)
            {
                return x >= X && x <= Right;
            }
            public bool ContainsY(int y)
            {
                return y >= Y && y <= Bottom;
            }
            #endregion

            #region EQUALITY
            public bool Equals(Rectangle other)
            {
                if (valid == 0 && other == null)
                    return true;
                if (other == null)
                    return false;
                return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
            }
            public override bool Equals(object obj)
            {
                if (obj is Rectangle)
                    return Equals((Rectangle)obj);
                return false;
            }
            public override int GetHashCode()
            {
                return new { X, Y, Width, Height }.GetHashCode();
            }
            #endregion

            #region OPERATORS
            public static implicit operator bool(Box r) =>
                r.valid != 0;
            public static bool operator ==(Box a, Rectangle b)
            {
                return a.Equals(b);
            }
            public static bool operator !=(Box a, Rectangle b)
            {
                return !a.Equals(b);
            }
            public static explicit operator RectangleF(Box r) =>
                new RectangleF(r.X, r.Y, r.Width, r.Height);

            public static explicit operator BoxF(Box r) =>
                new BoxF(r.X, r.Y, r.Width, r.Height);
            #endregion

            #region IENUMERABLE
            public IEnumerable<VectorF> Perimeter() => this;

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

            public override string ToString() =>
                string.Format(toStr, X, Y, Width, Height);
        }
#if HideGWSObjects
    }
#endif
}
#endif
