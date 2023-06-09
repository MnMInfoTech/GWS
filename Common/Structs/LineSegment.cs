using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    #region ILINE SEGMENT
    public interface ILineSegment : IScanPoint, IValid
    {
        /// <summary>
        /// Gets X coordinate of start point.
        /// </summary>
        float X1 { get; }

        /// <summary>
        /// Gets Y coordinate of start point.
        /// </summary>
        float Y1 { get; }

        /// <summary>
        /// Gets X coordinate of end point.
        /// </summary>
        float X2 { get; }

        /// <summary>
        /// Gets Y coordinate of end point.
        /// </summary>
        float Y2 { get; }
    }
    #endregion

    public struct LineSegment: ILineSegment
    {
        #region VARIABLES
        /// <summary>
        /// X coordinate of start point.
        /// </summary>
        public readonly float X1;

        /// <summary>
        /// Y coordinate of start point.
        /// </summary>
        public readonly float Y1;

        /// <summary>
        /// X coordinate of end point.
        /// </summary>
        public readonly float X2;

        /// <summary>
        /// Y coordinate of end point.
        /// </summary>
        public readonly float Y2;

        const string toStr = "{0}; x1: {1}; y1: {2}; x2: {3}, y2: {4}";
        public static readonly ILineSegment Empty = new LineSegment();
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X coordinate of start point</param>
        /// <param name="y1">Y coordinate of start point</param>
        /// <param name="x2">X coordinate of end point</param>
        /// <param name="y2">Y coordinate of end point</param>
        public LineSegment(float x1, float y1, float x2, float y2) : this()
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        /// <summary>
        /// Creates a line segment from the specified start and end points.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="angle">Angle to apply rotation on start and end point before creating the line segment</param>
        public LineSegment(IPointF start, IPointF end) :
            this(start.X, start.Y, end.X, end.Y)
        { }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the start and end points specified start and end points upto value of deviation parameter.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="rotation">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public LineSegment(IPoint start, IPoint end) :
            this(start.X, start.Y, end.X, end.Y)
        { }
        #endregion

        #region PROPERTIES
        bool IValid.Valid => !(float.IsNaN(X1) || float.IsNaN(Y1) || float.IsNaN(X2) || float.IsNaN(Y2));
        float ILineSegment.X1 => X1;
        float ILineSegment.Y1 => Y1;
        float ILineSegment.X2 => X2;
        float ILineSegment.Y2 => Y2;
        #endregion
    }
}
