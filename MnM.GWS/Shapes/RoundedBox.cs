/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    public struct RoundBox : IRoundBox
    {
        #region VARIABLES
        public readonly float X;
        public readonly float Y;
        public readonly float Width;
        public readonly float Height;
        public readonly float CornerRadius;

        const string toStr = "x:{0}, y:{1}, width:{2}, height:{3}, cornerRadius: {4}";
        IList<VectorF> points;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new rouded box with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public RoundBox(float x, float y, float w, float h, float cornerRadius, bool positiveLocation = false) : this()
        {
            ID = "RoundBox".NewID();
            X = x;
            Y = y;
            Width = w;
            Height = h;

            if (positiveLocation)
            {
                if (X < 0)
                {
                    Width += X;
                    X = 0;
                }
                if (Y < 0)
                {
                    Height += Y;
                    Y = 0;
                }
            }

            CornerRadius = cornerRadius;
            points = Curves.RoundedBoxPoints(X, Y, Width, Height, CornerRadius);
        }
        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to match bounds from.</param>
        public RoundBox(Rectangle area, float cornerRadius) : 
            this(area.X, area.Y, area.Width, area.Height, cornerRadius)
        { }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public RoundBox(RectangleF area, float cornerRadius) : 
            this(area.X, area.Y, area.Width, area.Height, cornerRadius)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public RoundBox(VectorF xy, SizeF wh, float cornerRadius) :
            this(xy.X, xy.Y, wh.Width, wh.Height, cornerRadius)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public RoundBox(Vector xy, Size wh, float cornerRadius) :
            this(xy.X, xy.Y, wh.Width, wh.Height, cornerRadius)
        { }

        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static RoundBox FromLTRB(float x, float y, float right, float bottom, float cornerRadius, bool correct = true)
        {
            if (!correct)
                return new RoundBox(x, y, right - x, bottom - y, cornerRadius);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return new RoundBox(x, y, w, h, cornerRadius);
        }
        #endregion

        #region PROPERTIES
        public RectangleF Bounds => new RectangleF(X, Y, Width, Height);
        public float Right => X + Width;
        public float Bottom => Y + Height;
        public bool IsEmpty =>
            Width == 0 && Height == 0;
        public bool IsSquare => Width == Height;
        public string ID { get; private set; }
        public string Name => "RoundBox";
        float IRoundBox.CornerRadius => CornerRadius;
        public QuadType Type => QuadType.RoundBox;
        RectangleF IBoundsF.Bounds => Bounds;
        #endregion

        #region IEnumerable<VectorF>
        public IEnumerator<VectorF> GetEnumerator() => points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, X, Y, Width, Height, CornerRadius);
        }
    }
}
