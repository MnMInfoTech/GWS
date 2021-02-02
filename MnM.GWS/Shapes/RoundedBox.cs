/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
#if HideGWSObjects
    partial class NativeFactory
    {
#else
    public
#endif
       partial struct RoundBox : IRoundBox
        {
            #region VARIABLES
            public readonly float X;
            public readonly float Y;
            public readonly float Width;
            public readonly float Height;
            public readonly float CornerRadius;
        uint id;
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
            public RoundBox(float x, float y, float w, float h, float cornerRadius, RoundBoxOption option = 0) : this()
            {
                X = x;
                Y = y;
                Width = w;
                Height = h;
                CornerRadius = cornerRadius;
                points = Curves.RoundedBoxPoints(X, Y, Width, Height, CornerRadius, option);
            Name = TypeName.NewName();
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to match bounds from.</param>
        public RoundBox(Rectangle area, float cornerRadius, RoundBoxOption option = 0) :
                this(area.X, area.Y, area.Width, area.Height, cornerRadius, option)
            { }

            /// <summary>
            /// Creates a new rect identical to the area of specifed rectangle.
            /// </summary>
            /// <param name="area">Area to copy bounds from.</param>
            public RoundBox(RectangleF area, float cornerRadius, RoundBoxOption option = 0) :
                this(area.X, area.Y, area.Width, area.Height, cornerRadius, option)
            { }

            /// <summary>
            /// Creates a box matching the specifiedlocation and size.
            /// </summary>
            /// <param name="xy">Location of the box.</param>
            /// <param name="wh">Size of the box.</param>
            public RoundBox(VectorF xy, SizeF wh, float cornerRadius, RoundBoxOption option = 0) :
                this(xy.X, xy.Y, wh.Width, wh.Height, cornerRadius, option)
            { }

            /// <summary>
            /// Creates a box matching the specifiedlocation and size.
            /// </summary>
            /// <param name="xy">Location of the box.</param>
            /// <param name="wh">Size of the box.</param>
            public RoundBox(Vector xy, Size wh, float cornerRadius, RoundBoxOption option = 0) :
                this(xy.X, xy.Y, wh.Width, wh.Height, cornerRadius, option)
            { }

            /// <summary>
            /// Creates a new rect with specifed left, top, right and bottom parameters.
            /// </summary>
            /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
            /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
            /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
            /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
            /// <returns>RectF</returns>
            public static RoundBox FromLTRB(float x, float y, float right, float bottom, float cornerRadius, RoundBoxOption option = 0)
            {
                return new RoundBox(x, y, right - x, bottom - y, cornerRadius, option);
            }
            #endregion

            #region PROPERTIES
            public float Right => X + Width;
            public float Bottom => Y + Height;
            public bool IsEmpty =>
                Width == 0 && Height == 0;
            public bool IsSquare => Width == Height;
        public uint ID
        {
            get
            {
                if (id == 0)
                    id = this.NewID();
                return id;
            }
        }
        public string TypeName => "RoundBox";
        public string Name { get; private set; }
        float IRoundBox.CornerRadius => CornerRadius;
            public QuadType Type => QuadType.RoundBox;
            float IPointF.X => X;
            float IPointF.Y => Y;
            float ISizeF.Width => Width;
            float ISizeF.Height => Height;
            #endregion

            #region CONTAINS
            public bool Contains(float x, float y)
            {
                if (x < X || y < Y || x > X + Width || y > Y + Height)
                    return false;
                return true;
            }
            #endregion

            #region IEnumerable<VectorF>
            public IEnumerable<VectorF> Perimeter() => this;
            public IEnumerator<VectorF> GetEnumerator()
            {
                return points.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
            #endregion

            public override string ToString()
            {
                return string.Format(toStr, X, Y, Width, Height, CornerRadius);
            }
        }
#if HideGWSObjects
    }
#endif
}
#endif