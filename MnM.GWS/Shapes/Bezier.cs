/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice must not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if GWS || Window
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if Standard
namespace MnM.GWS.Standard
#elif Advanced
namespace MnM.GWS.Advanced
#else
namespace MnM.GWS
#endif
{
#if AllHidden
    partial class _Factory
    {
#else
    public
#endif
    partial struct Bezier : IBezier
    {
        readonly IList<VectorF> Points;
        public readonly float X;
        public readonly float Y;
        public readonly float Width;
        public readonly float Height;

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
                Points = pointValues.ToPoints();
            else
                Points = points.ToArray();
            var Bounds = Points.ToArea();
            X = Bounds.X;
            Y = Bounds.Y;
            Width = Bounds.Width;
            Height = Bounds.Height;
            ID = Name.NewID();
        }
#endregion

#region PROPERTIES
        public BezierType Option { get; private set; }
        public string Name => "Bezier";
        public string ID { get; private set; }
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

#region IEnumerable<IPosF>
        public IEnumerable<VectorF> Perimeter() => this;
        IEnumerator<VectorF> IEnumerable<VectorF>.GetEnumerator() =>
            Points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            Points.GetEnumerator();
#endregion
    }
#if AllHidden
    }
#endif
}
#endif