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
#if AllHidden
    partial class NativeFactory
    {
#else
    public
#endif
     partial  struct Triangle : ITriangle 
    {
        public readonly float X, Y, Width, Height;
        
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
            Centre = (P1 + P2 + P3) / 3;
            var Bounds = Vectors.ToArea(P1, P2, P3);
            X = Bounds.X;
            Y = Bounds.Y;
            Width = Bounds.Width;
            Height = Bounds.Height;
            ID = "Triangle".NewID();
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
        #endregion

        #region PROPERTIES
        public VectorF P1 { get; private set; }
        public VectorF P2 { get; private set; }
        public VectorF P3 { get; private set; }

        public VectorF Centre { get; private set; }
        public float Area { get; private set; }
        public string Name => "Triangle";
        public Rotation Rotation { get; private set; }
        public string ID { get; private set; }
        float IPointF.X => X;
        float IPointF.Y => Y;
        float ISizeF.Width => Width;
        float ISizeF.Height => Height;
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

        #region IEnumerable<VectorF>
        public IEnumerable<VectorF> Perimeter() => this;
        public IEnumerator<VectorF> GetEnumerator()
        {
            yield return P1;
            yield return P2;
            yield return P3;
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion
    }
#if AllHidden
    }
#endif
}
#endif