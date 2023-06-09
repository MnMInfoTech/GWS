/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region ILINE
    public interface ILine : ILineSegment, IShape, IFigure, IScanPoint, ILineCommand
    { }
    #endregion

    #region LINE
    /// <summary>
    /// Represents an object which defines a line segment and its properties.
    /// </summary>
    public struct Line : ILine, IExDraw, IExResizable
    {
        #region VARIABLES
        /// <summary>
        /// X co-ordinate of start point.
        /// </summary>
        public readonly float X1;

        /// <summary>
        /// Y co-ordinate of start point.
        /// </summary>
        public readonly float Y1;

        /// <summary>
        /// X co-ordinate of end point.
        /// </summary>
        public readonly float X2;

        /// <summary>
        /// Y co-ordinate of end point.
        /// </summary>
        public readonly float Y2;

        LineCommand type;

        const string toStr = "x1: {0}; y1: {1}; x2: {2}, y2: {3}";
        public static readonly ILine Empty = new Line();
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point</param>
        /// <param name="y1">Y co-ordinate of start point</param>
        /// <param name="x2">X co-ordinate of end point</param>
        /// <param name="y2">Y co-ordinate of end point</param>
        public Line(float x1, float y1, float x2, float y2) : this()
        {
            type =  LineCommand.ValidLine;

            if ((float.IsNaN(x1) || float.IsNaN(y1) || float.IsNaN(x2) || float.IsNaN(y2)))
                type = 0;

            if (type != 0)
            {
                X1 = x1;
                Y1 = y1;
                X2 = x2;
                Y2 = y2;
            }
            if (type != 0 && x1 == 0 && y1 == 0 || x2 == 0 && y2 == 0)
                type |= LineCommand.OriginBasedLine;
        }

        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point</param>
        /// <param name="y1">Y co-ordinate of start point</param>
        /// <param name="x2">X co-ordinate of end point</param>
        /// <param name="y2">Y co-ordinate of end point</param>
        /// <param name="rotation">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public Line(float x1, float y1, float x2, float y2, IDegree rotation,
            float deviation, bool antiClock = false, bool noSkew = false) : this()
        {
            if (rotation != null && rotation.Valid)
                rotation.RotateLine(ref x1, ref y1, ref x2, ref y2, antiClock, noSkew);

            if (deviation != 0)
                Lines.Parallel(x1, y1, x2, y2, deviation, out x1, out y1, out x2, out y2);

            this = new Line(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point</param>
        /// <param name="y1">Y co-ordinate of start point</param>
        /// <param name="x2">X co-ordinate of end point</param>
        /// <param name="y2">Y co-ordinate of end point</param>
        /// <param name="rotation">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public Line(float x1, float y1, float x2, float y2, IDegree rotation, 
            bool antiClock = false, bool noSkew = false) : this()
        {
            if (rotation != null && rotation.Valid)
                rotation.RotateLine(ref x1, ref y1, ref x2, ref y2, antiClock, noSkew);

            this = new Line(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point</param>
        /// <param name="y1">Y co-ordinate of start point</param>
        /// <param name="x2">X co-ordinate of end point</param>
        /// <param name="y2">Y co-ordinate of end point</param>
        /// <param name="rotation">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public Line(float x1, float y1, float x2, float y2, IDegree rotation) : this()
        {
            if (rotation != null && rotation.Valid)
                rotation.RotateLine(ref x1, ref y1, ref x2, ref y2);

            this = new Line(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the start and end points specified by x1, y1, x2, y2  upto value of deviation parameter.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point</param>
        /// <param name="y1">Y co-ordinate of start point</param>
        /// <param name="x2">X co-ordinate of end point</param>
        /// <param name="y2">Y co-ordinate of end point</param>
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
        {
            if (type != 0)
                type |= l.LineCommand;
        }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified line upto value of deviation parameter.
        /// </summary>
        /// <param name="l">Reference line.</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public Line(ILine l) :
            this(l.X1, l.Y1, l.X2, l.Y2)
        {
            if (type != 0)
                type |= l.LineCommand;
        }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified line upto value of deviation parameter.
        /// </summary>
        /// <param name="l">Reference line.</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public Line(ILine l, IDegree rotation, bool antiClock = false, bool noSkew = false) :
            this(l.X1, l.Y1, l.X2, l.Y2, rotation, 0, antiClock, noSkew)
        {
            if (type != 0)
                type |= l.LineCommand;
        }

        /// <summary>
        /// Creates a line segment from the specified start and end points.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="angle">Angle to apply rotation on start and end point before creating the line segment</param>
        public Line(IPointF start, IPointF end, IDegree rotation, bool antiClock = false, bool noSkew = false) :
            this(start.X, start.Y, end.X, end.Y, rotation, 0, antiClock, noSkew)
        {
            if
            (
                start is IPointType
                && (((IPointType)start).Kind & PointKind.Bresenham) == PointKind.Bresenham 
                || end is IPointType
                && (((IPointType)end).Kind & PointKind.Bresenham) == PointKind.Bresenham
            )
            {
                type |= LineCommand.Bresenham;
            }
        }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified start and end points upto value of deviation parameter.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="rotation">Angle to apply rotation on start and end point before creating the line segment</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public Line(IPointF start, IPointF end, IDegree rotation, 
            float deviation, bool antiClock = false, bool noSkew = false) :
           this(start.X, start.Y, end.X, end.Y, rotation, deviation, antiClock, noSkew)
        {
            if
            (
                start is IPointType
                && (((IPointType)start).Kind & PointKind.Bresenham) == PointKind.Bresenham
                || end is IPointType
                && (((IPointType)end).Kind & PointKind.Bresenham) == PointKind.Bresenham
            )
            {
                type |= LineCommand.Bresenham;
            }
        }

        /// <summary>
        /// Creates a new line segment using the specified start and end points.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        public Line(IPointF start, IPointF end) :
            this(start.X, start.Y, end.X, end.Y)
        {
            if
            (
                start is IPointType
                && (((IPointType)start).Kind & PointKind.Bresenham) == PointKind.Bresenham
                || end is IPointType
                && (((IPointType)end).Kind & PointKind.Bresenham) == PointKind.Bresenham
            )
            {
                type |= LineCommand.Bresenham;
            }
        }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified start and end points upto value of deviation parameter.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public Line(IPointF start, IPointF end, float deviation) :
            this(start.X, start.Y, end.X, end.Y, deviation)
        {
            if
            (
                start is IPointType
                && (((IPointType)start).Kind & PointKind.Bresenham) == PointKind.Bresenham
                || end is IPointType
                && (((IPointType)end).Kind & PointKind.Bresenham) == PointKind.Bresenham
            )
            {
                type |= LineCommand.Bresenham;
            }
        }

        /// <summary>
        /// Creates a new line segment using the specified x1, y1 - start point and end point.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point.</param>
        /// <param name="y1">Y co-ordinate of start point.</param>
        /// <param name="end">End point x2, y2.</param>
        public Line(float x1, float y1, IPointF end) :
            this(x1, y1, end.X, end.Y)
        { }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the start and end points specified start and end points upto value of deviation parameter.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="rotation">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public Line(IPoint start, IPoint end, IDegree rotation, 
            float deviation, bool antiClock = false, bool noSkew = false) :
            this(start.X, start.Y, end.X, end.Y, rotation, deviation, antiClock, noSkew)
        {
            if
            (
                start is IPointType
                && (((IPointType)start).Kind & PointKind.Bresenham) == PointKind.Bresenham
                || end is IPointType
                && (((IPointType)end).Kind & PointKind.Bresenham) == PointKind.Bresenham
            )
            {
                type |= LineCommand.Bresenham;
            }
        }

        /// <summary>
        /// Creates a new line segment specified sby tart and end points;
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="rotation">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        public Line(IPoint start, IPoint end, IDegree rotation, bool antiClock = false, bool noSkew = false) :
            this(start.X, start.Y, end.X, end.Y, rotation, 0, antiClock, noSkew)
        {
            if
            (
                start is IPointType
                && (((IPointType)start).Kind & PointKind.Bresenham) == PointKind.Bresenham
                || end is IPointType
                && (((IPointType)end).Kind & PointKind.Bresenham) == PointKind.Bresenham
            )
            {
                type |= LineCommand.Bresenham;
            }
        }

        /// <summary>
        /// Creates a new line segment specified sby tart and end points;
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        public Line(IPoint start, IPoint end) :
            this(start.X, start.Y, end.X, end.Y)
        {
            if
            (
                start is IPointType
                && (((IPointType)start).Kind & PointKind.Bresenham) == PointKind.Bresenham
                || end is IPointType
                && (((IPointType)end).Kind & PointKind.Bresenham) == PointKind.Bresenham
            )
            {
                type |= LineCommand.Bresenham;
            }
        }

        /// <summary>
        /// Creates a new line segment using the specified x2, y2 - end point and start point.
        /// </summary>
        /// <param name="start">Start point x1, y1.</param>
        /// <param name="x2">X co-ordinate of end point.</param>
        /// <param name="y2">Y co-ordinate of end point.</param>
        public Line(IPoint start, float x2, float y2) :
           this(start.X, start.Y, x2, y2)
        {
            if
            (
                start is IPointType
                && (((IPointType)start).Kind & PointKind.Bresenham) == PointKind.Bresenham
            )
            {
                type |= LineCommand.Bresenham;
            }
        }


        /// <summary>
        /// Creates a new line segment using the specified x1, y1 - start point and end point.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point.</param>
        /// <param name="y1">Y co-ordinate of start point.</param>
        /// <param name="end">End point x2, y2.</param>
        public Line(float x1, float y1, IPoint end) :
            this(x1, y1, end.X, end.Y)
        {
            if
            (
                end is IPointType
                && (((IPointType)end).Kind & PointKind.Bresenham) == PointKind.Bresenham
            )
            {
                type |= LineCommand.Bresenham;
            }
        }
        #endregion

        #region PROPERTIES
        bool IValid.Valid => type != 0;
        float ILineSegment.X1 => X1;
        float ILineSegment.Y1 => Y1;
        float ILineSegment.X2 => X2;
        float ILineSegment.Y2 => Y2;
        bool IOriginCompatible.IsOriginBased => X1 == 0 && Y1 == 0 || X2 == 0 && Y2 == 0;
        int IPoint.X => (int)(X1 > X2 ? X2 : X1);
        int IPoint.Y => (int)(Y1 > Y2 ? Y2 : Y1);
        int ISize.Width
        {
            get
            {
                var fw = X2 - X1;
                if (fw < 0)
                    fw = -fw;
                var w = (int)fw;
                if (fw - w != 0)
                    ++w;
                if (w == 0)
                    return 1;
                return w;
            }
        }
        int ISize.Height
        {
            get
            {
                var fh = Y2 - Y1;
                if (fh < 0)
                    fh = -fh;
                var h = (int)fh;
                if (fh - h != 0)
                    ++h;
                if (h == 0)
                    return 1;
                return h;
            }
        }
        LineCommand ILineCommand.LineCommand => type;
        #endregion

        #region DRAW
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer renderer)
        {
            if (type == 0)
                return true;

            parameters.Extract(out IExSession session);

            bool OriginalFill = (session.Command & Command.OriginalFill) == Command.OriginalFill;

            if (session.Stroke != 0 && !OriginalFill)
            {
                renderer.RenderPolygon(this, session);
                return true;
            }
            var x1 = X1;
            var y1 = Y1;
            var x2 = X2;
            var y2 = Y2;

            if (session.Rotation != null && session.Rotation.Valid)
                Angles.Rotate(session.Rotation, ref x1, ref y1, ref x2, ref y2);

            if (session.Scale != null && session.Scale.HasScale)
            {
                x1 *= session.Scale.X;
                y1 *= session.Scale.Y;
                x2 *= session.Scale.X;
                y2 *= session.Scale.Y;
            }
            session.SetPen(this);
            var action = renderer.CreateRenderAction(session);
            action(null, new ILine[] { new Line(x1, y1, x2, y2) }, null);
            return true;
        }
        #endregion

        #region PERIMETER
        VectorF[] IPolygonal<VectorF>.GetPoints()
        {
            return new VectorF[] 
            { 
                new VectorF(X1, Y1),
                new VectorF(X2, Y2) 
            };
        }
        #endregion

        #region HAS VERTICAL SLOPE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSteep()
        {
            var dx = X2 - X1;
            var dy = Y2 - Y1;
            var pdy = dy;
            var pdx = dx;
            if (pdy < 0)
                pdy = -pdy;
            if (pdx < 0)
                pdx = -pdx;
            return (pdy > pdx);
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
            var scale = new Scale(this, w, h);
            return new Line(X1 * scale.X, Y1 * scale.Y, X2 * scale.X, Y2 * scale.Y);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            var x = (X1 > X2 ? X2 : X1);
            var y = (Y1 > Y2 ? Y2 : Y1);
            return new Line(X1 - x, Y1 - y, X2 - x, Y2 - y);
        }
        #endregion

        #region OPERATORS
        public static bool operator ==(Line a, Line b) =>
            a.Equals(b);
        public static bool operator !=(Line a, Line b) =>
            !a.Equals(b);

        public static implicit operator bool(Line l) =>
            l.type != 0;
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
    }
    #endregion
}
#endif
