/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using static MnM.GWS.Application;

namespace MnM.GWS
{
    public struct Bezier : IBezier
    {
        readonly IList<VectorF> Points;

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
        /// <param name="angle">Angle to apply rotation while creating the bezier</param>
        public Bezier(BezierType type, ICollection<float> pointValues, IList<VectorF> points) : this()
        {
            Option = type;
            if (pointValues != null)
                Points = pointValues.ToPoints();
            else
                Points = points.ToArray();
            Bounds = Points.ToArea();
            ID = Name.NewID();
        }
        #endregion

        #region PROPERTIES
        public BezierType Option { get; private set; }
        public RectangleF Bounds { get; private set; }
        RectangleF IBoundsF.Bounds => Bounds;
        public string Name => "Bezier";
        public string ID { get; private set; }
        #endregion

        #region IEnumerable<IPosF>
        IEnumerator<VectorF> IEnumerable<VectorF>.GetEnumerator() =>
            Points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            Points.GetEnumerator();
        #endregion
    }
}
