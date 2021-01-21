/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    /// <summary>
    /// Tetragon structure to create Rhombus, Box or Trapezium (defined as per the definition in British English).
    /// </summary>
#if AllHidden
    partial class NativeFactory
    {
#else
    public
#endif
     partial struct Tetragon : ITetragon, IPointF, ISizeF
    {
        #region VARIABLES
        public readonly IList<VectorF> Points;
        public readonly float X, Y, Width, Height;
        readonly byte Valid;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a tetragon specified by four points and applies an angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point.</param>
        /// <param name="p2">Second point.</param>
        /// <param name="p3">Third point.</param>
        /// <param name="p4">Fourth point.</param>
        /// <param name="angle">Angle to apply rotation while creating the trapezium</param>
        public Tetragon(VectorF p1, VectorF p2, VectorF p3, VectorF p4)
        {
            Points = new VectorF[] { p1, p2, p3, p4 };
            var Bounds = Points.ToArea();
            X = Bounds.X;
            Y = Bounds.Y;
            Width = Bounds.Width;
            Height = Bounds.Height;
            Valid = Application.True;
            Type = QuadType.Trapezium;
            ID = "Tetragon".NewID();
        }

        /// <summary>
        /// Creates a rhombus specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of the bounding rectangle</param>
        /// <param name="y">Y cordinate of the bounding rectangle</param>
        /// <param name="w">Width of the bounding rectangle/param>
        /// <param name="h">Height the bounding rectangle</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        /// <param name="deviation">If not null, it replaces the value of width parameter</param>
        public Tetragon(float x, float y, float w, float h, float? deviation = null) : this()
        {
            w = deviation ?? w;
            Points = new VectorF[]
                {
                        new VectorF(x, y),
                        new VectorF(x, y + h),
                        new VectorF(x + w, y + h),
                        new VectorF(x + w, y)
                };
            X = x;
            Y = y;
            Width = w;
            Height = h;
            Type = QuadType.Rhombus;
            Valid = Application.True;
            ID = "Tetragon".NewID();
        }

        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while creating the trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public Tetragon(ILine first, float deviation, StrokeMode mode = StrokeMode.StrokeMiddle, float skewBy = 0)
        {
            ILine second = new Line(first);

            if (mode == StrokeMode.StrokeMiddle)
            {
                first = new Line(first, -deviation / 2f);
                second = new Line(second, deviation / 2f);
            }
            else if (mode == StrokeMode.StrokeOuter)
                second = new Line(second, deviation);
            else if (mode == StrokeMode.StrokeInner)
                second = new Line(second, -deviation);

            deviation = mode == StrokeMode.StrokeInner ? -deviation : deviation;

            VectorF difference;
            bool Steep;

            var Start = new VectorF(first.X1, first.Y1);
            var End = new VectorF(first.X2, first.Y2);
            difference = Start - End;
            Steep = Math.Abs(difference.Y) > Math.Abs(difference.X);

            if (skewBy != 0)
            {
                if (skewBy >= deviation)
                    skewBy = deviation - 1;

                second = Steep ? second.Offset(0, skewBy) :
                    second.Offset(skewBy, 0);
            }

            this = new Tetragon(Start, End, new VectorF(second.X2, second.Y2), new VectorF(second.X1, second.Y1));
        }



        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) specified by base line x1, y1, x2, y2, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="x1">X corordinate of start point of base line.</param>
        /// <param name="y1">Y corordinate of start point of base line.</param>
        /// <param name="x2">X corordinate of end point of base line.</param>
        /// <param name="y2">Y corordinate of end point of base line.</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while creating the trapezium</param>
        /// <param name="mode"> Stroke mode to apply while creating the trapezium</param>
        public Tetragon(float x1, float y1, float x2, float y2, float deviation, StrokeMode mode = StrokeMode.StrokeMiddle, float skewBy = 0) :
            this(new Line(x1, y1, x2, y2), deviation, mode, skewBy)
        { }

        /// <summary>
        /// Creates a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        public Tetragon(VectorF p1, VectorF p2, VectorF p3) :
            this(p1, p2, p3, Vectors.FourthPointOfRhombus(p1, p2, p3))
        { }

        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) with angle of rotation if supplied.
        /// Baseline, parallelLineDeviation and parallelLineDeviation specified by values int the following manner:
        /// First four values defines base line x1, y1, x2, y2.
        /// Fifth value defines parallelLineDeviation.
        /// Sixth value (optional) defines parallelLineSizeDifference;
        /// </summary>
        /// <param name="angle">Angle to apply rotation while creating the trapezium</param>
        /// <param name="values">Values which defines trapezium formation.</param>
        public Tetragon(params float[] values)
        {
            var first = new Line(values[0], values[1], values[2], values[3]);
            float parallelLineDeviation = 30f;
            float parallelLineSizeDifference = 0;
            if (values.Length < 6)
                parallelLineDeviation = values[4];
            if (values.Length > 5)
                parallelLineSizeDifference = values[5];
            this = new Tetragon(first, parallelLineDeviation, StrokeMode.StrokeOuter, parallelLineSizeDifference);
        }

        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) specified by start and end points forming a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">Start point of base line.</param>
        /// <param name="p2">End point of base line.</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="mode"> Stroke mode to apply while creating the trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public Tetragon(VectorF p1, VectorF p2, float deviation, StrokeMode mode = StrokeMode.StrokeMiddle, float skewBy = 0)
            : this(p1.X, p1.Y, p2.X, p2.Y, deviation, mode, skewBy)
        { }

        /// <summary>
        /// Creates a tetragon specified by points (minimum four are required) and apply an angle of rotation if supplied.
        /// </summary>
        /// <param name="points">Collection of points to create tetragon from.</param>
        /// <param name="angle">Angle to apply rotation while creating the trapezium</param>
        public Tetragon(IList<VectorF> points) : this()
        {
            if (points == null || points.Count < 3)
                return;

            if (points.Count > 3)
                this = new Tetragon(points[0], points[1], points[2], points[3]);
            else
                this = new Tetragon(points[0], points[1], points[2]);
        }

        /// <summary>
        /// Creates a rhombus identical to specified area and angle of rotation if supplied.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        /// <param name="deviation">If not null, it replaces the value of width parameter of area.</param>
        public Tetragon(Rectangle area, float? deviation = null) :
            this(area.X, area.Y, area.Width, area.Height, deviation)
        { }

        /// <summary>
        /// Creates a rhombus identical to specified area and angle of rotation if supplied.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        /// <param name="deviation">If not null, it replaces the value of width parameter of area.</param>
        public Tetragon(RectangleF area, float? deviation = null) :
            this(area.X, area.Y, area.Width, area.Height, deviation)
        { }
        #endregion

        #region PROPERTIES
        public string Name
        {
            get
            {
                return Type + "";
            }
        }
        public string ID { get; private set; }
        public QuadType Type { get; private set; }
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
        public IEnumerator<VectorF> GetEnumerator() =>
            Points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion

        public static implicit operator bool(Tetragon t) => t.Valid != 0;
    }
#if AllHidden
    }
#endif
}
#endif