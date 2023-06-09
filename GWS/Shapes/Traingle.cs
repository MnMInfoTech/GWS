/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;

namespace MnM.GWS
{
    #region ITRIANGLE
    public interface ITriangle : IShape, IFigure, ICount
    {
        VectorF P1 { get; }
        VectorF P2 { get; }
        VectorF P3 { get; }
    }
    #endregion

    #region TRIANGLE
    /// <summary>
    /// Represent an object which has three points to offer.
    /// This object must have collection of three points.
    /// </summary>
    public struct Triangle : ITriangle, IHitTestable, IExResizable
    {
        #region VARIABLES
        /// <summary>
        /// 1st point of triangle.
        /// </summary>
        public readonly VectorF P1;

        /// <summary>
        /// 2nd point of triangle.
        /// </summary>
        public readonly VectorF P2;

        /// <summary>
        /// 3rd point of triangle.
        /// </summary>
        public readonly VectorF P3;

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
        readonly float Area;

        const string toStr = "{0}, x1: {1}, y1: {2}, x2: {3}, y2: {4}, x3: {5}, y3: {6}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new trianle formed by three points specified by x1, y1, x2, y2, x3, y3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="x1">X corodinate of the first point</param>
        /// <param name="y1">Y corodinate of the first point</param>
        /// <param name="x2">X corodinate of the second point</param>
        /// <param name="y2">Y corodinate of the second point</param>
        /// <param name="x3">X corodinate of the third point</param>
        /// <param name="y3">Y corodinate of the third point</param>
        public Triangle(float x1, float y1, float x2, float y2, float x3, float y3) : this()
        {
            P1 = new VectorF(x1, y1);
            P2 = new VectorF(x2, y2);
            P3 = new VectorF(x3, y3);
            Curves.Order3Points(ref P1, ref P2, ref P3);
            var Bounds = Vectors.ToArea(P1, P2, P3).Expand();
            X = Bounds.X;
            Y = Bounds.Y;
            Width = Bounds.Width;
            Height = Bounds.Height;
            Area = GetArea(P1, P2, P3);
        }

        /// <summary>
        /// Creates a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the traingle</param>
        public Triangle(Vector p1, Vector p2, Vector p3) :
            this(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y)
        { }

        /// <summary>
        /// Creates a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="angle"></param>
        public Triangle(VectorF p1, VectorF p2, VectorF p3) :
            this(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y)
        { }

        /// <summary>
        /// Creates a trianle formed by three points - start and end points of line and another point p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="l">Line to supplying start and end points.</param>
        /// <param name="p3">the third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the traingle</param>
        public Triangle(ILine l, Vector p3) :
            this(l.X1, l.Y1, l.X2, l.Y2, p3.X, p3.Y)
        { }

        /// <summary>
        /// Creates a trianle formed by three points - start and end points of line and another point p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="l">Line to supplying start and end points.</param>
        /// <param name="p3">the third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the traingle</param>
        public Triangle(ILine l, VectorF b) :
            this(l.X1, l.Y1, l.X2, l.Y2, b.X, b.Y)
        { }

        public Triangle(float x1, float y1, float w, float h, Position position) :
            this(x1, y1, w, h, position, 0)
        { }

        public Triangle(float x1, float y1, float w, float h, Position position, float shrink) : this()
        {
            var cx = x1 + w / 2f;
            var cy = y1 + h / 2f;

            float x = x1 + shrink;
            float y = y1 + shrink;
            w -= shrink;
            h-= shrink;
            float r = x1 + w;
            float b = y1 + h;

            switch (position)
            {
                case Position.Default:
                case Position.Left:
                default:
                    this = new Triangle(x, cy, r, y, r, b);
                    break;
                case Position.Top:
                    this = new Triangle(cx, y, r, b, x, b);
                    break;
                case Position.Right:
                    this = new Triangle(r, cy, x, b, x, y);
                    break;
                case Position.Bottom:
                    this = new Triangle(cx, b, r, y, x, y);
                    break;
            }
        }
        public Triangle(float x1, float y1, float w, float h, Position position, IScale scale) : this()
        {
            Rectangles.Scale(ref x1, ref y1, ref w, ref h, scale);
            this = new Triangle(x1, y1, w, h, position);
        }
        public Triangle(int x1, int y1, int w, int h, Position position, IScale scale) : this()
        {
            Rectangles.Scale(ref x1, ref y1, ref w, ref h, scale);
            this = new Triangle(x1, y1, w, h, position);
        }

        Triangle(Triangle triangle, int w, int h)
        {
            X = triangle.X; Y = triangle.Y; Width = w; Height = h;
            var Points = new VectorF[] { triangle.P1, triangle.P2, triangle.P3 }.Scale(new Scale(triangle, w, h)).ToArray();
            P1 = Points[0];
            P2 = Points[1];
            P3 = Points[2];
            Area = GetArea(P1, P2, P3);
        }
        Triangle(Triangle triangle)
        {
            X = 0; Y = 0; Width = triangle.Width; Height = triangle.Height;
            var subtract = new VectorF(triangle.X, triangle.Y);
            P1 = triangle.P1 -subtract;
            P2 = triangle.P2 - subtract;
            P3 = triangle.P3 - subtract;
            Area = GetArea(P1, P2, P3);
        }
        #endregion

        #region PROPERTIES
        VectorF ITriangle.P1 => P1;
        VectorF ITriangle.P2 => P2;
        VectorF ITriangle.P3 => P3;
        public bool Valid => Width >= 0 && Height >= 0;
        int ICount.Count => 3;
        bool IOriginCompatible.IsOriginBased => X == 0 && Y == 0;
        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
        #endregion

        #region CONTAINS
        public bool Contains(float x, float y) =>
            Contains(new VectorF(x, y));
        public bool Contains(VectorF p)
        {
            var a1 = GetArea(p, P1, P2);
            var a2 = GetArea(p, P1, P3);
            var a3 = GetArea(p, P3, P2);
            return Area == a1 + a2 + a3;
        }
        #endregion

        #region GET AREA
        static float GetArea(VectorF p1, VectorF p2, VectorF p3)
        {
            return Math.Abs((p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y)) / 2f);
        }
        #endregion

        #region PERIMETER
        VectorF[] IPolygonal<VectorF>.GetPoints()
        {
           return new VectorF[] { P1, P2, P3 };                 
        }
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
            return new Triangle(this, w, h);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new Triangle(this);
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, "Triangle", P1.X, P1.Y, P2.X, P2.Y, P3.X, P3.Y);
        }
    }
    #endregion
}
#endif