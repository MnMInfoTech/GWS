/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region IPOLYGON
    /// <summary>
    /// Represent an object which is a polygon i.e made of a collection of straight lines.
    /// In GWS, having a collection of points arranged in a sequential manner - one after another, 
    /// defines a polygon. Bezier is a curve, but GWS breakes it into the straight lines so for
    /// the drawing purpose it becomes polygon without having close ends i.e first point joins the last one.
    /// All the shapes which offer closed area are in fact have the first point joined with the last.
    /// GWS, does not break the curves except bezier i.e Ellipse, Circle, Pie, Arc in straight lines and 
    /// that is why there is a separate drawing routine for them.
    /// </summary>
    public interface IPolygon : IShape, IFigure, IHitTestable
    { }
    #endregion

    #region POLYGON
    /// <summary>
    /// Represent an object which is a polygon i.e made of a collection of straight lines.
    /// In GWS, having a collection of points arranged in a sequential manner - one after another, 
    /// defines a polygon. Bezier is a curve, but GWS breakes it into the straight lines so for
    /// the drawing purpose it becomes polygon without having close ends i.e first point joins the last one.
    /// All the shapes which offer closed area are in fact have the first point joined with the last.
    /// GWS, does not break the curves except bezier i.e Ellipse, Circle, Pie, Arc in straight lines and 
    /// that is why there is a separate drawing routine for them.
    /// </summary>
    public struct Polygon : IPolygon, IExResizable
    {
        #region VARIABLES
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

        public readonly VectorF[] Points;

        static string tostr = "{0}, x: {1}, y: {2}, w: {3}, h: {4}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon.</param>
        /// <param name="angle">Angle to apply rotation while rendering the polygon</param>
        public Polygon(IEnumerable<VectorF> polyPoints) : this()
        {
            if (polyPoints is VectorF[])
                Points = (VectorF[])polyPoints;
            else if(polyPoints is IArrayHolder<VectorF>)
            {
                var arr = (IArrayHolder<VectorF>)polyPoints;
                Points = new VectorF[arr.Count];
                Array.Copy(arr.Data, Points, Points.Length);
            }
            else
                Points = polyPoints.ToArray();
            var Bounds = polyPoints.ToArea().Expand();
            X = Bounds.X;
            Y = Bounds.Y;
            Width = Bounds.Width;
            Height = Bounds.Height;
        }
     
        /// <summary>
        /// Creates a new polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x, y.</param>
        /// <param name="angle">Angle to apply rotation while rendering the polygon</param>
        public Polygon(params float[] polyPoints) :
            this(polyPoints.ToVectorF())
        { }

        public Polygon(IPolygonalF points) : this()
        {
            Points = points.GetPoints();

            bool ptfound = false;
            bool sizefound = false;

            if (points is IPoint)
            {
                X = ((IPoint)points).X;
                Y = ((IPoint)points).Y;
                ptfound = true;
            }
            if (points is ISize)
            {
                Width = points.Width;
                Height = points.Height;
                sizefound = true;
            }
            if (!ptfound || !sizefound)
            {
                Points.MinMax(out float x, out float y, out float maxx, out float maxy);
                X = (int)x;
                Y = (int)y;
                Width = (int)(maxx - x);
                Height = (int)(maxy - y);
            }
        }
       
        Polygon(IPolygon polygon, int w, int h): this()
        {
            Points = polygon.GetPoints().Scale(new Scale(polygon, w, h)).ToArray();
            X = polygon.X;
            Y = polygon.Y;
            Width = w;
            Height = h;
        }
        Polygon(IPolygon polygon)
        {
            X = 0;
            Y = 0;
            Width = polygon.Width;
            Height = polygon.Height;
            var subtract = new Point(polygon.X, polygon.Y);
            Points = polygon.GetPoints().Select(pt => pt - subtract).ToArray();
        }
        #endregion

        #region PROPERTIES
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

        #region CONTAINS
        public unsafe bool Contains(float x, float y)
        {
            int i, j;
            int nvert = Points.Length;
            bool c = false;
            fixed (VectorF* pt = Points)
            {
                for (i = 0, j = nvert - 1; i < nvert; j = i++)
                {
                    if (((pt[i].Y > y) != (pt[j].Y > y)) &&
                     (x < (pt[j].X - pt[i].X) * (y - pt[i].Y) / (pt[j].Y - pt[i].Y) + pt[i].X))
                        c = !c;
                }
            }
            return c;
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
            return new Polygon(this, w, h);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new Polygon(this);
        }
        #endregion

        public override string ToString()
        {
            return string.Format(tostr, "Polygon", X, Y, Width, Height);
        }
    }
   #endregion
}
#endif
