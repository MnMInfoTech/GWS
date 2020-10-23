/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    public struct Line : ILine
    {
        #region VARIABLES
        byte Valid;

        const string toStr = "{0}; {1}; {2}; {3}";
        public static readonly Line Empty = new Line();
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        public Line(float x1, float y1, float x2, float y2) : this()
        {
            Valid = Application.True;

            if ((float.IsNaN(x1) || float.IsNaN(y1) || float.IsNaN(x2) || float.IsNaN(y2)))
                Valid = Application.False;


            if (Valid != 0)
            {
                //LineHelper.RoundLineCoordinates(ref x1, ref y1, ref x2, ref y2, 4);

                M = Vectors.Slope(x1, y1, x2, y2, out float c);
                C = c;
                X1 = x1;
                Y1 = y1;
                X2 = x2;
                Y2 = y2;
                Bounds = Rects.FromLTRB(x1, y1, x2, y2, true);
                Type = this.Type();
            }
            else
            {
                Bounds = RectangleF.Empty;
                Type = LineType.Point;
            }
            ID = "Line".NewID();
        }

        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public Line(float x1, float y1, float x2, float y2, Rotation angle, float deviation, bool? antiClock = null) : this()
        {
            if (angle)
                angle.Rotate(ref x1, ref y1, ref x2, ref y2, antiClock);

            if (deviation != 0)
                Lines.Parallel(x1, y1, x2, y2, deviation, out x1, out y1, out x2, out y2);

            this = new Line(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public Line(float x1, float y1, float x2, float y2, Rotation angle, bool? antiClock = null) : this()
        {
            if (angle)
                angle.Rotate(ref x1, ref y1, ref x2, ref y2, antiClock);

            this = new Line(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public Line(float x1, float y1, float x2, float y2, Rotation angle) : this()
        {
            if (angle)
                angle.Rotate(ref x1, ref y1, ref x2, ref y2);

            this = new Line(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the start and end points specified by x1, y1, x2, y2  upto value of deviation parameter.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public Line(float x1, float y1, float x2, float y2, float deviation)
        {

            if (deviation != 0)
                Lines.Parallel(x1, y1, x2, y2, deviation, out x1, out y1, out x2, out y2);
            this = new Line(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified line upto value of deviation parameter.
        /// </summary>
        /// <param name="l">Reference line.</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public Line(ILine l, float deviation) :
            this(l.X1, l.Y1, l.X2, l.Y2, deviation)
        { }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified line upto value of deviation parameter.
        /// </summary>
        /// <param name="l">Reference line.</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public Line(ILine l) :
            this(l.X1, l.Y1, l.X2, l.Y2)
        { }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified line upto value of deviation parameter.
        /// </summary>
        /// <param name="l">Reference line.</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public Line(ILine l, Rotation angle, bool? antiClock  =null) :
            this(l.X1, l.Y1, l.X2, l.Y2, angle, 0, antiClock)
        { }

        /// <summary>
        /// Creates a line segment from the specified start and end points.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="angle">Angle to apply rotation on start and end point before creating the line segment</param>
        public Line(VectorF start, VectorF end, Rotation angle, bool? antiClock = null) :
            this(start.X, start.Y, end.X, end.Y, angle, 0, antiClock)
        { }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified start and end points upto value of deviation parameter.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="angle">Angle to apply rotation on start and end point before creating the line segment</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public Line(VectorF start, VectorF end, Rotation angle, float deviation, bool? antiClock = null) :
           this(start.X, start.Y, end.X, end.Y, angle, deviation, antiClock)
        { }

        /// <summary>
        /// Creates a new line segment using the specified start and end points.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        public Line(VectorF start, VectorF end) :
            this(start.X, start.Y, end.X, end.Y)
        { }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified start and end points upto value of deviation parameter.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public Line(VectorF start, VectorF end, float deviation) :
            this(start.X, start.Y, end.X, end.Y, deviation) { }

        /// <summary>
        /// Creates a new line segment using the specified x1, y1 - start point and end point.
        /// </summary>
        /// <param name="x1">X co-ordiante of start point.</param>
        /// <param name="y1">Y co-ordiante of start point.</param>
        /// <param name="end">End point x2, y2.</param>
        public Line(float x1, float y1, VectorF end) :
            this(x1, y1, end.X, end.Y)
        { }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the start and end points specified start and end points upto value of deviation parameter.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public Line(Vector start, Vector end, Rotation angle, float deviation, bool? antiClock = null) :
            this(start.X, start.Y, end.X, end.Y, angle, deviation, antiClock)
        { }

        /// <summary>
        /// Creates a new line segment specified sby tart and end points;
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        public Line(Vector start, Vector end, Rotation angle, bool? antiClock = null) :
            this(start.X, start.Y, end.X, end.Y, angle, 0, antiClock)
        { }

        /// <summary>
        /// Creates a new line segment specified sby tart and end points;
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        public Line(Vector start, Vector end) :
            this(start.X, start.Y, end.X, end.Y)
        { }

        /// <summary>
        /// Creates a new line segment using the specified x2, y2 - end point and start point.
        /// </summary>
        /// <param name="start">Start point x1, y1.</param>
        /// <param name="x2">X co-ordiante of end point.</param>
        /// <param name="y2">Y co-ordiante of end point.</param>
        public Line(Vector start, float x2, float y2) :
           this(start.X, start.Y, x2, y2) { }


        /// <summary>
        /// Creates a new line segment using the specified x1, y1 - start point and end point.
        /// </summary>
        /// <param name="x1">X co-ordiante of start point.</param>
        /// <param name="y1">Y co-ordiante of start point.</param>
        /// <param name="end">End point x2, y2.</param>
        public Line(float x1, float y1, Vector end) :
            this(x1, y1, end.X, end.Y)
        { }
        #endregion

        #region PROPERTIES
        public float X1 { get; private set; }
        public float Y1 { get; private set; }
        public float X2 { get; private set; }
        public float Y2 { get; private set; }
        public float M { get; private set; }
        public float C { get; private set; }

        bool ILine.Valid => Valid != 0;
        public RectangleF Bounds { get; private set; }
        public string Name => "Line";
        public string ID { get; private set; }
        public LineType Type { get; private set; }
        RectangleF IBoundsF.Bounds => Bounds;
        #endregion

        #region CONTAINS
        public bool Contains(float x, float y) =>
            (M * x + C) - y == 0;
        #endregion

        #region DRAW TO
        public bool Draw(IBuffer buffer, IReadContext readContext, out IPen Pen)
        {
            Pen = null;
            if (Valid == 0)
                return true;

            if (buffer.Settings.Stroke != 0 && buffer.Settings.FillMode != FillMode.Original)
                return false;


            var x1 = X1;
            var y1 = Y1;
            var x2 = X2;
            var y2 = Y2;
            var type = Type;

            if (buffer.Settings.Rotation)
            {
                Angles.Rotate(buffer.Settings.Rotation, ref x1, ref y1, ref x2, ref y2);
                type = Lines.Type(x1, y1, x2, y2);
            }

            Lines.MakeDrawable(false, ref x1, ref y1, ref x2, ref y2, null, out _, out _);
            var rc = RectangleF.FromLTRB(x1, y1, x2, y2, true);
            Pen = buffer.Settings.GetPen(this, readContext);
            bool aa = !buffer.Settings.LineCommand.HasFlag(LineCommand.Breshenham);

            if (type == LineType.Horizontal || type == LineType.Vertical || !aa)
            {
                buffer.CreateAction(Pen, out PixelAction<int> action);
                Renderer.ProcessLine(x1.Round(), y1.Round(), x2.Round(), y2.Round(), action, buffer.Settings.LineCommand);
            }
            else
            {
                buffer.CreateAction(Pen, out PixelAction<float> action);
                Renderer.ProcessLine(x1, y1, x2, y2, action, buffer.Settings.LineCommand);
            }
            return true;
        }
        #endregion

        #region TO SHAPE
        public IEnumerable<VectorF> Figure() => this;
        #endregion

        #region OPERATORS
        public static bool operator ==(ILine a, Line b) =>
            a.Equals(b);
        public static bool operator !=(ILine a, Line b) =>
            !a.Equals(b);

        public static implicit operator bool(Line l)=>
            l.Valid != 0;
        #endregion

        #region EQUALITY
        public override int GetHashCode()
        {
            return new { X1, Y1, X2, Y2 }.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is ILine))
                return false;
            return Equals((ILine)obj);
        }
        public bool Equals(ILine other)
        {
            return X1 == other.X1 && Y1 == other.Y1 && X2 == other.X2 && Y2 == other.Y2;
        }
        #endregion

        #region TO STRING
        public override string ToString() =>
            string.Format(toStr, X1.RoundF(2), Y1.RoundF(2), X2.RoundF(2), Y2.RoundF(2));
        #endregion

        #region IEnumerable<VectorF>
        public IEnumerator<VectorF> GetEnumerator()
        {
            yield return new VectorF(X1, Y1);
            yield return new VectorF(X2, Y2);
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion
    }
}
