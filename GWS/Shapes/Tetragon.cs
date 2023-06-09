/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace MnM.GWS
{
    public interface ITetragon : IShape, IFigure, ICount
    { }

    /// <summary>
    /// Represents a closed object (Quardilateral) which has four sides.
    /// This defination is in accordance with the British English and not the US one.
    /// </summary>
    public struct Tetragon : ITetragon, IExResizable
    {
        #region VARIABLES
        public readonly VectorF[] Points;
        /// <summary>
        /// Far left horizontal corodinate of this object.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Far top vertical corodinate of this object.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// far right horizontal corodinate (X + Width) of this object.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Deviation from the far top vertical corodinate (Y) of this object.
        /// </summary>
        public readonly int Height;

        readonly sbyte Valid;

        public static readonly Tetragon Empty = new Tetragon();
        const string toStr = "{0}, x1: {1}, y1: {2}, x2: {3}, y2: {4}, x3: {5}, y3: {6}, x4: {7}, y4: {8}";
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
        public Tetragon(VectorF p1, VectorF p2, VectorF p3, VectorF p4) : this()
        {
            Points = new VectorF[] { p1, p2, p3, p4 };
            var Bounds = Points.ToArea().Expand();
            X = Bounds.X;
            Y = Bounds.Y;
            Width = Bounds.Width;
            Height = Bounds.Height;
            Valid = (sbyte)((X == 0 && Y == 0) ? -1 : 1);
            Type = QuadType.Trapezium;
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
                        new VectorF(x + w, y),
                        new VectorF(x + w, y + h),
                        new VectorF(x, y + h)
                };
            X = (int)x;
            Y = (int)y;
            Width = (int)w;
            if (w - Width !=0)
                Width++;
            Height = (int)h;
            if (h - Height !=0)
                ++Height;
            Type = QuadType.Rhombus;
            Valid = (sbyte)((X == 0 && Y == 0) ? -1 : 1);
        }

        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while creating the trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public Tetragon(ILine first, float deviation, FillCommand Command = 0, float skewBy = 0)
        {
            ILine second = new Line(first);
            bool StrokeInner = (Command & FillCommand.StrokeInner) == FillCommand.StrokeInner;
            bool StrokeOuter = (Command & FillCommand.StrokeOuter) == FillCommand.StrokeOuter && !StrokeInner;

            if (StrokeOuter)
                second = new Line(second, deviation);
            else if (StrokeInner)
                second = new Line(second, -deviation);
            else
            {
                first = new Line(first, -deviation / 2f);
                second = new Line(second, deviation / 2f);
            }

            deviation = Command == FillCommand.StrokeInner ? -deviation : deviation;

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
        public Tetragon(float x1, float y1, float x2, float y2, float deviation, FillCommand mode = 0, float skewBy = 0) :
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
            this = new Tetragon(first, parallelLineDeviation, FillCommand.StrokeOuter, parallelLineSizeDifference);
        }

        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) specified by start and end points forming a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">Start point of base line.</param>
        /// <param name="p2">End point of base line.</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="mode"> Stroke mode to apply while creating the trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public Tetragon(VectorF p1, VectorF p2, float deviation, FillCommand mode = 0, float skewBy = 0)
            : this(p1.X, p1.Y, p2.X, p2.Y, deviation, mode, skewBy)
        { }

        /// <summary>
        /// Creates a tetragon specified by points (minimum four are required) and apply an angle of rotation if supplied.
        /// </summary>
        /// <param name="points">Collection of points to create tetragon from.</param>
        /// <param name="angle">Angle to apply rotation while creating the trapezium</param>
        public Tetragon(IEnumerable<VectorF> points) : this()
        {
            if (points == null)
                return;
            if (points.Count() < 3)
                return;
            var pts = points.ToArray();
            if (pts.Length > 3)
                this = new Tetragon(pts[0], pts[1], pts[2], pts[3]);
            else
                this = new Tetragon(pts[0], pts[1], pts[2]);
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

        public Tetragon(IPolygonal tetragon) :
            this(tetragon.GetPoints().Select(p => new VectorF(p.X, p.Y)))
        { }

        public Tetragon(IPolygonalF tetragon) :
           this(tetragon.GetPoints())
        { }

        Tetragon(Tetragon tetragon, int w, int h)
        {
            X = tetragon.X; Y = tetragon.Y; Width = w; Height = h;
            Points = tetragon.Points.Scale(new Scale(tetragon, w, h)).ToArray();
            Valid = (sbyte)((X == 0 && Y == 0) ? -1 : 1);
            Type = tetragon.Type;
        }

        Tetragon(Tetragon tetragon)
        {
            X = 0;
            Y = 0;
            Width = tetragon.Width;
            Height = tetragon.Height;
            Type = tetragon.Type;
            Valid = tetragon.Valid;
            var subtract = new Point(tetragon.X, tetragon.Y);
            Points = tetragon.Points.Select(pt => pt - subtract).ToArray();
        }
        #endregion

        #region PROPERTIES
        public QuadType Type { get; private set; }
        bool IValid.Valid => Valid != 0;
        int ICount.Count => 4;
        bool IOriginCompatible.IsOriginBased => X == 0 && Y == 0;
        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
        #endregion

        #region CONTAINS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool Contains(float x, float y)
        {
            if (Width == 0 || Height == 0 || x < X || y < Y || x > X + Width || y > Y + Height)
                return false;

            if (Points != null)
            {
                fixed (VectorF* p = Points)
                {
                    return (Vectors.InTriangle(x, y, p[0].X, p[0].Y,
                        p[1].X, p[1].Y, p[2].X, p[2].Y) ||
                            Vectors.InTriangle(x, y, p[2].X, p[2].Y,
                            p[3].X, p[3].Y, p[0].X, p[0].Y));
                }
            }
            return true;
        }
        #endregion

        #region PERIMETER
        VectorF[] IPolygonal<VectorF>.GetPoints() => 
            Points;
        #endregion

        #region RESIZE
        object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            var iw = ((ISize)this).Width;
            var ih = ((ISize)this).Height;

            if
            (
               (w == iw && h == ih) ||
               (w == 0 && h == 0)
            )
            {
                return this;
            }

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

            if (SizeOnlyToFit && iw > w && ih > h)
                return this;

            if (SizeOnlyToFit)
            {
                if (w < iw)
                    w = iw;
                if (h < ih)
                    h = ih;
            }
            success = true;
            return new Tetragon(this, w, h);
         }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new Tetragon(this);
        }
        #endregion

        public static implicit operator bool(Tetragon t) => t.Valid != 0;

        public override string ToString()
        {
            return string.Format(toStr, "Tetragon", Points[0].X, Points[0].Y, 
                Points[1].X, Points[1].Y, Points[2].X, Points[2].Y, Points[3].X, Points[3].Y);
        }
    }
}
#endif