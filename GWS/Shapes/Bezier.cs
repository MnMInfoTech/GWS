/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice must not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if GWS || Window
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region IBEZIER
    public interface IBezier : IShape, IFigure
    {
        BezierType Option { get; }
    }
    #endregion

    #region BEZIER
    /// <summary>
    /// Represents an object which have properties of bezier curve.
    /// For drawing purpose, GWS breakes the curve in straight line segments.
    /// In GWS, a bezier can be drawn by offering minimum 3 points. 
    /// However there isn't any specific number of points required except minimum 3 to draw a curve.
    /// </summary>
    public struct Bezier : IBezier, IExResizable
    {
        #region VARAIBLES
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

        readonly VectorF[] Points;
        readonly static string toStr = "{0}, x: {1}, y: {2}, w: {3}, h: {4}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a bezier defined by points and specified by type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        public Bezier(params float[] points) :
            this(0, points as IList<float>)
        { }

        /// <summary>
        /// Creates a bezier defined by points and specified by type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        public Bezier(BezierType type, params float[] points) :
            this(type, points as IList<float>)
        { }

        /// <summary>
        /// Creates a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="type">Type of bezier to create.</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        public Bezier(BezierType type, IList<float> points) :
            this(type, points, null)
        { }

        /// <summary>
        /// Creates a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="type">Type of bezier to create.</param>
        /// <param name="points">Points which defines perimiter of the bezier.</param>
        /// <param name="angle">Angle to apply rotation while creating the bezier</param>
        public Bezier(BezierType type, IList<VectorF> points) :
            this(type, null, points)
        { }

        /// <summary>
        /// Creates a bezier defined by either pointsData (float values) or pixels (points) and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="type">Type of bezier to create.</param>
        /// <param name="pointValues">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="points">Points which defines perimiter of the bezier.</param>
        public Bezier(BezierType type, ICollection<float> pointValues, IList<VectorF> points) : this()
        {
            Option = type;
            if (pointValues != null)
                Points = pointValues.ToVectorF().ToArray();
            else
                Points = points.ToArray();
            var Bounds = Points.ToArea().Expand();
            X = Bounds.X;
            Y = Bounds.Y;
            Width = Bounds.Width;
            Height = Bounds.Height;
        }

        Bezier(Bezier bezier, int w, int h)
        {
            Option = bezier.Option;
            X= bezier.X;
            Y = bezier.Y;
            Width = bezier.Width;
            Height = bezier.Height;
            Points = bezier.Points.Scale(new Scale(bezier, w, h)).ToArray();
        }

        Bezier(IBezier bezier)
        {
            Option = bezier.Option;
            X = Y = 0;
            Width = bezier.Width;
            Height = bezier.Height;
            var subtract = new Point(bezier.X, bezier.Y);
            Points = bezier.GetPoints().Select(pt => pt - subtract).ToArray();
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Specified which option is used to interpret the points for accumulating bezier points.
        /// We have two options : Cubic (taking a group of 4 points) or Multiple (4, 7, 10, 13 ... so on).
        /// Please not that if only three points are provided then its a Quadratic Bezier.
        /// </summary>
        public BezierType Option { get; private set; }
        public bool Valid => Width >= 0 && Height >= 0;
        bool IOriginCompatible.IsOriginBased => X == 0 && Y == 0;
        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
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
            return new Bezier(this, w, h);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new Bezier(this);
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, "Bezier", X, Y, Width, Height);
        }
    }  
    #endregion
}
#endif