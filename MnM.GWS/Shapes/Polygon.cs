/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;
using System.Collections;
using System.Collections.Generic;

using static MnM.GWS.Application;

namespace MnM.GWS
{
    public struct Polygon : IPolygon
    {
        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon.</param>
        /// <param name="angle">Angle to apply rotation while rendering the polygon</param>
        public Polygon(IList<VectorF> polyPoints) : this()
        {
            Points = polyPoints;
            Bounds = polyPoints.ToArea();
            ID = "Polygon".NewID();
        }
        /// <summary>
        /// Creates a new polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x, y.</param>
        /// <param name="angle">Angle to apply rotation while rendering the polygon</param>
        public Polygon(params float[] polyPoints) :
            this(polyPoints.ToPoints())
        { }
        #endregion

        #region PROPRTIES
        public IList<VectorF> Points { get; private set; }
        public RectangleF Bounds { get; private set; }
        public string Name => "Polygon";
        public string ID { get; private set; }
        RectangleF IBoundsF.Bounds => Bounds;
        #endregion

        #region DRAW
        public bool Contains(float x, float y)
        {
            int counter = 0;
            int i;
            double xinters;
            VectorF p1, p2;
            var N = Points.Count - 1;

            p1 = Points[0];
            for (i = 1; i <= N; i++)
            {
                p2 = Points[i % N];
                if (y > Math.Min(p1.Y, p2.Y))
                {
                    if (y <= Math.Max(p1.Y, p2.Y))
                    {
                        if (x <= Math.Max(p1.X, p2.X))
                        {
                            if (p1.Y != p2.Y)
                            {
                                xinters = (y - p1.Y) * (p2.X - p1.X) / (p2.Y - p1.Y) + p1.X;
                                if (p1.X == p2.X || x <= xinters)
                                    counter++;
                            }
                        }
                    }
                }
                p1 = p2;
            }
            return !(counter % 2 == 0);
        }
        #endregion

        #region IEnumerable<VectorF>
        IEnumerator<VectorF> IEnumerable<VectorF>.GetEnumerator() =>
            Points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            Points.GetEnumerator();
        #endregion
    }
}

