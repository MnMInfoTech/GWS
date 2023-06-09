/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window

namespace MnM.GWS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    public static partial class Lines
    {
        #region INTERSECTION
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(this ILine first, ILine second, out VectorF P)
        {
            var p1 = new VectorF(first.X1, first.Y1);
            var p2 = new VectorF(first.X2, first.Y2);
            var p3 = new VectorF(second.X1, second.Y1);
            var p4 = new VectorF(second.X2, second.Y2);
            bool ok = Vectors.Intersects(p1, p2, p3, p4, out P);
            return ok;
        }
        public static bool Intersects(this ILine first, ILine second, out VectorF P, out bool parallel)
        {
            var p1 = new VectorF(first.X1, first.Y1);
            var p2 = new VectorF(first.X2, first.Y2);
            var p3 = new VectorF(second.X1, second.Y1);
            var p4 = new VectorF(second.X2, second.Y2);
            bool ok = Vectors.Intersects(p1, p2, p3, p4, out P, out parallel);
            return ok;
        }
        #endregion

        #region TO AREA
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF ToArea(this IEnumerable<ILine> lines)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = 0;
            float maxY = 0;

            foreach (var n in lines)
            {
                if (n==null || !n.Valid)
                    continue;
                var x = Math.Min(n.X1, n.X2);
                var y = Math.Min(n.Y1, n.Y2);
                var r = Math.Max(n.X1, n.X2);
                var b = Math.Max(n.Y1, n.Y2);

                if (x < minX)
                    minX = x;

                if (y < minY)
                    minY = y;

                if (maxX < r)
                    maxX = r;

                if (maxY < b)
                    maxY = b;
            }

            if (minX == float.MaxValue || minY == float.MaxValue)
                return new RectangleF(0, 0, 0, 0);

            return new RectangleF(minX, minY, maxX, maxY);
        }
        #endregion

        #region > OR < THAN POINT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThan(this ILine l, float x, float y)
        {
            var c1 = (l.X2 - l.X1) * (y - l.Y1);
            var c2 = (l.Y2 - l.Y1) * (x - l.X1);
            var result = c1 < c2;
            return result;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThan(this ILine l, float x, float y)
        {
            var c1 = (l.X2 - l.X1) * (y - l.Y1);
            var c2 = (l.Y2 - l.Y1) * (x - l.X1);
            var result = c1 > c2;
            return result;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThan(this ILine l, VectorF p) =>
            l.IsLessThan(p.X, p.Y);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThan(this ILine l, VectorF p) =>
            l.IsGreaterThan(p.X, p.Y);
        #endregion

        #region SLOPE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this ILine l) =>
            Vectors.Slope(l.X1, l.Y1, l.X2, l.Y2);
        #endregion

        #region DISTANCE BETWEEN PARALLEL LINES
        /// <summary>
        /// Source: https://www.geeksforgeeks.org/distance-between-two-parallel-lines/
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceFromParallel(this ILine l1, ILine l2)
        {
            var m1 = Slope(l1);
            var m2 = Slope(l2);
            var c1 = l1.Y1 - m1 * l1.X1;
            var c2 = l2.X1 - m2 * l2.X1;
            return Math.Abs(c2 - c1) / ((m1 * m1) - 1);
        }
        #endregion

        #region ANGLE BETWEEN 2 LINES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRotation AngleFrom(this ILine l1, ILine l2)
        {
            float theta1 = (float)Math.Atan2(l1.Y2 - l1.Y1, l1.X2 - l1.X1);
            float theta2 = (float)Math.Atan2(l2.Y2 - l2.Y1, l2.X2 - l2.X1);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            return new Rotation(angle);
        }
        #endregion

        #region SCAN VAL SAFE
        /// <summary>
        /// Scans the given line and solves standard line equation y = mx + c to get x or y co-ordinate depending on axis direction.
        /// The axis value must lies within line span otherwise value will be an invalid - NaN and false is returned.
        /// </summary>
        /// <param name="l">Line to scan.</param>
        /// <param name="axis">Axis value- Y co-ordinate if horizontal and X co-ordinate otherwise.</param>
        /// <param name="horizontal">Axial direction.</param>
        /// <param name="value">If axis is within line span - value is obtained by solving standard line equation y = Mx + C.</param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Scan(this ILine l, float axis, bool horizontal, out float value)
        {
            value = float.NaN;

            if (l==null || !l.Valid)
                return false;

            bool exit;
            var Type = l.Type(out _, out float M, out float C);
            if (horizontal)
            {
                if (Type == LineDirection.Horizontal)
                    return false;
                if (l.Y1 > l.Y2)
                    exit = axis < l.Y2.Ceiling() || axis >= l.Y1.Ceiling();
                else
                    exit = axis < l.Y1.Ceiling() || axis >= l.Y2.Ceiling();
                if (exit)
                    return false;

                value = (axis - C) / M;
            }
            else
            {
                if (Type == LineDirection.Vertical)
                    return false;
                if (l.X1 > l.X2)
                    exit = axis < l.X2.Ceiling() || axis >= l.X1.Ceiling();
                else
                    exit = axis < l.X1.Ceiling() || axis >= l.X2.Ceiling();
                if (exit)
                    return false;
                value = M * axis + C;
            }
            return true;
        }
        #endregion

        #region GET DIRECTION
        /// <summary>
        /// Gets the direction of line at particur spot. i.e. in order to reach the spot, 
        /// one has to move upward or downward from within rise / run of this line.
        /// </summary>
        /// <param name="l">Line to scan.</param>
        /// <param name="axis">Spot is Y co-ordinate if horizontal otherwise X co-ordinate.</param>
        /// <param name="horizontal">Scan direction - along the Y axis if horizontal otherwise X axis.</param>
        /// <returns>Direction of line 0 if line is invaid, 1 if downward, -1 if upward.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte GetDirection(this ILine l, int axis, bool horizontal)
        {
            if (l==null ||!l.Valid)
                return 0;

            var d1 = horizontal ? Math.Abs(l.Y1 - axis) : Math.Abs(l.X1 - axis);
            var d2 = horizontal ? Math.Abs(l.Y2 - axis) : Math.Abs(l.X2 - axis);
            var sign = horizontal ? l.Y1 < l.Y2 : l.X1 < l.X2;
            var sign1 = d1 < d2;
            if (sign == sign1)
                return 1;
            return -1;
        }
        #endregion

        #region PARALLEL
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsParallel(this ILine l1, ILine l2)
        {
            return IsParallel(l1.X1, l1.Y1, l1.X2, l1.Y2, l2.X1, l2.Y1, l2.X2, l2.Y2);
        }
        #endregion

        #region LINE TYPE
        /// <summary>
        /// Returns Type of the line specified by x1, y1, x2, y2 co-ordinates.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point of the line.</param>
        /// <param name="y1">Y co-ordinate of start point of the line.</param>
        /// <param name="x2">X co-ordinate of end point of the line.</param>
        /// <param name="y2">Y co-ordinate of end point of the line.</param>
        /// <param name="steep">Steepness of the line i.e. line is wider (width) or longer (height).</param>
        /// <param name="m">Value of M as in y = M * x + C.</param>
        /// <param name="c">Value of C as in y = M * x + C.</param>
        /// <returns>Type of this line i.e. horizontal or vertical or a point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LineDirection Type(int x1, int y1, int x2, int y2, out SlopeType steep, out float m, out float c)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            m = dy;
            if (dx != 0)
                m /= dx;
            c = y1 - m * x1;

            dy = Math.Abs(dy);
            dx = Math.Abs(dx);

            if (dy > dx)
                steep = SlopeType.Steep;
            else
                steep = SlopeType.NonSteep;

            LineDirection t = 0;

            if (x1 == x2 && y1 == y2)
                t = LineDirection.Point;
            else if (x1 == x2)
                t = LineDirection.Horizontal;
            else if (y1 == y2)
                t = LineDirection.Vertical;
            return t;
        }

        /// <summary>
        /// Returns Type of the line specified by x1, y1, x2, y2 co-ordinates.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point of the line.</param>
        /// <param name="y1">Y co-ordinate of start point of the line.</param>
        /// <param name="x2">X co-ordinate of end point of the line.</param>
        /// <param name="y2">Y co-ordinate of end point of the line.</param>
        /// <param name="steep">Steepness of the line i.e. line is wider (width) or longer (height).</param>
        /// <param name="m">Value of M as in y = M * x + C.</param>
        /// <param name="c">Value of C as in y = M * x + C.</param>
        /// <returns>Type of this line i.e. horizontal or vertical or a point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LineDirection Type(float x1, float y1, float x2, float y2, 
            out SlopeType steep, out float m, out float c)
        {
            LineDirection t = LineDirection.Diagonal;
            var dx = x2 - x1;
            var dy = y2 - y1;
            m = dy;
            if (dx != 0)
                m /= dx;
            c = y1 - m * x1;

            dy = Math.Abs(dy);
            dx = Math.Abs(dx);

            if (dy > dx)
                steep = SlopeType.Steep;
            else
                steep = SlopeType.NonSteep;

            if (dx <= EPSILON)
                t = LineDirection.Vertical;
            if (dy <= EPSILON)
            {
                if (t == LineDirection.Vertical)
                    t = LineDirection.Point;
                else
                    t = LineDirection.Horizontal;
            }
            return t;
        }

        /// <summary>
        /// Returns Type of the line specified by x1, y1, x2, y2 co-ordinates.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point of the line.</param>
        /// <param name="y1">Y co-ordinate of start point of the line.</param>
        /// <param name="x2">X co-ordinate of end point of the line.</param>
        /// <param name="y2">Y co-ordinate of end point of the line.</param>
        /// <param name="steep">Steepness of the line i.e. line is wider (width) or longer (height).</param>
        /// <returns>Type of this line i.e. horizontal or vertical or a point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LineDirection Type(float x1, float y1, float x2, float y2,
            out SlopeType steep)
        { 
            LineDirection type = LineDirection.Diagonal;
            var dx = x2 - x1;
            var dy = y2 - y1;

            var pdy = Math.Abs(dy);
            var pdx = Math.Abs(dx);

            if (pdy > pdx)
                steep = SlopeType.Steep;
            else
                steep = SlopeType.NonSteep;

            if (pdx <= EPSILON)
                type = LineDirection.Vertical;
            if (pdy <= EPSILON)
            {
                if (type == LineDirection.Vertical)
                    type = LineDirection.Point;
                else
                    type = LineDirection.Horizontal;
            }
            return type;
        }

        /// <summary>
        /// Returns Type of the line specified by x1, y1, x2, y2 co-ordinates.
        /// </summary>
        /// <param name="line">Line to get type and other characteristics for.</param>
        /// <param name="steep">Steepness of the line i.e. line is wider (width) or longer (height).</param>
        /// <param name="m">Value of M as in y = M * x + C.</param>
        /// <param name="c">Value of C as in y = M * x + C.</param>
        /// <returns>Type of this line i.e. horizontal or vertical or a point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LineDirection Type(this ILine line, out SlopeType steep, out float m, out float c) =>
            Type(line.X1, line.Y1, line.X2, line.Y2, out steep, out m, out c);

        /// <summary>
        /// Returns Type of the line specified by x1, y1, x2, y2 co-ordinates.
        /// </summary>
        /// <param name="line">Line to get type and other characteristics for.</param>
        /// <param name="steep">Steepness of the line i.e. line is wider (width) or longer (height).</param>
        /// <returns>Type of this line i.e. horizontal or vertical or a point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LineDirection Type(this ILine line, out SlopeType steep) =>
            Type(line.X1, line.Y1, line.X2, line.Y2, out steep);
        #endregion

        #region ROTATE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ILine Rotate(this ILine l, IDegree rotation, bool antiClock = false, bool noSkew = false)
        {
            if (rotation != null && rotation.Valid)
                return new Line(l.X1, l.Y1, l.X2, l.Y2, rotation, antiClock, noSkew);
            return new Line(l.X1, l.Y1, l.X2, l.Y2);
        }
        #endregion

        #region OFFSET & SHRINK
        public static ILine Offset(this ILine l, float offsetX, float offsetY) =>
            new Line(l.X1 + offsetX, l.Y1 + offsetY, l.X2 + offsetX, l.Y2 + offsetY);
        #endregion

        #region MOVE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ILine Move(this ILine l, float move)
        {
            var p = Vectors.Move(l.X1, l.Y1, l.X2, l.Y2, move);
            return new Line(p.X, p.Y, l.X2, l.Y2);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ILine Extend(this ILine l, float deviation)
        {
            Vectors.Extend(l.X1, l.Y1, l.X2, l.Y2, out VectorF a, out VectorF b, deviation, true);
            return new Line(a, b);
        }
        #endregion

        #region FIND POINT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF FindPoint(this ILine l, float distance)
        {
            FindPoint(l.X1, l.Y1, l.X2, l.Y2, distance, out float x, out float y);
            return new VectorF(x, y);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FindPoint(this ILine l, float distance, out float x, out float y)
        {
            FindPoint(l.X1, l.Y1, l.X2, l.Y2, distance, out x, out y);
        }
        #endregion

        #region PERPENDICULAR
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Perpendicular(this ILine line, VectorF point) =>
            Perpendicular(line.X1, line.Y1, line.X2, line.Y2, point.X, point.Y);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ILine PerpendicularLine(this ILine line, VectorF p)
        {
            var p1 = line.Perpendicular(p);
            return new Line(p, p1);
        }
        #endregion

        #region CREATE AXIS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ILine Axis(this ILine l)
        {
            Axis(l.X1, l.Y1, l.X2, l.Y2, out float x1, out float y1, out float x2, out float y2);
            return new Line(x1, y1, x2, y2);
        }
        #endregion

        #region MIRROR LINE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ILine Mirror(this ILine l, bool startPointCommon = false)
        {
            l.Type(out _, out float m, out float c);
            m = -m;
            float nx1, ny1, nx2, ny2;
            if (startPointCommon)
            {
                ny1 = l.Y1;
                nx1 = l.X1;
                nx2 = l.X2;
                ny2 = m * nx2 + c;
            }
            else
            {
                c = l.Y2 - m * l.X2;
                ny1 = l.Y2;
                nx1 = l.X2;
                nx2 = l.X1;
                ny2 = m * nx2 + c;
            }
            return new Line(nx1, ny1, nx2, ny2);
        }
        #endregion

        #region JOIN ENDS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ILine[] JoinEnds(this ILine l1, ILine l2, bool opposite = false)
        {
            if (l2==null || !l2.Valid)
                return new ILine[] { new Line(l1.X1, l1.Y1, l1.X2, l1.Y2) };

            var v1 = new VectorF(l1.X1, l1.Y1);
            var v2 = new VectorF(l1.X2, l1.Y2);
            var v3 = new VectorF(l2.X1, l2.Y1);
            var v4 = new VectorF(l2.X2, l2.Y2);
            var distance1 = v1.Distance(v3);
            var distance2 = v1.Distance(v4);

            if (opposite)
            {
                if (distance1 < distance2)
                    Numbers.Swap(ref v3, ref v4);
            }
            else
            {
                if (distance1 > distance2)
                    Numbers.Swap(ref v3, ref v4);
            }
            return new ILine[] { new Line(v1, v3), new Line(v2, v4) };
        }
        #endregion

        #region STROKE SIDES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<ILine> StrokedSides(this ILine line, float stroke,
            IRotation rotation = null, bool expandCentrally = false)
        {
            if (line == null || !line.Valid)
            {
                return new ILine[0];
            }
            ILine first, second;

            if (!expandCentrally)
            {
                first = new Line(line);
                second = new Line(first, stroke);
            }
            else
            {
                first = new Line(line, -stroke / 2f);
                second = new Line(line, stroke / 2f);
            }
            if (rotation != null && rotation.Valid)
            {
                first = first.Rotate(rotation);
                second = second.Rotate(rotation);
            }

            var third = new Line(first.X1, first.Y1, second.X1, second.Y1);
            var fourth = new Line(first.X2, first.Y2, second.X2, second.Y2);
            return new ILine[] { first, second, third, fourth };
        }
        #endregion

        #region MIN - MAX
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(this IEnumerable<ILine> collection, out float minX, out float minY, out float maxX, out float maxY)
        {
            minX = minY = float.MaxValue;
            maxX = maxY = float.MinValue;
            bool ok = false;
            foreach (var l in collection)
            {
                if (l==null || !l.Valid)
                    continue;
                float x, y, r, b;

               x = l.X1 > l.X2 ? l.X1 - l.X2 : l.X2 - l.X1;
               y = l.Y1 > l.Y2 ? l.Y1 - l.Y2 : l.Y2 - l.Y1;
                r = l.X1 > l.X2 ? l.X2 : l.X1;
                b = l.Y1 > l.Y2 ? l.Y2 : l.Y1;

                ok = true;

                if (x < minX)
                    minX = x;
                if (y < minY)
                    minY = y;
                if (r > maxX)
                    maxX = r;
                if (b > maxY)
                    maxY = b;
            }
            if (!ok)
                minX = maxX = minY = maxY = 0;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(out float minX, out float minY, out float maxX, out float maxY, params ILine[] collection) =>
            MinMax(collection as IEnumerable<ILine>, out minX, out minY, out maxX, out maxY);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static bool MinMax(this IEnumerable<IAxisLine> lines, out int minX, out int minY, out int maxX, out int maxY)
        //{
        //    float x1 = float.MaxValue;
        //    float x2 = 0;
        //    int y1 = int.MaxValue;
        //    int y2 = 0;

        //    float y3 = int.MaxValue;
        //    float y4 = 0;
        //    int x3 = int.MaxValue;
        //    int x4 = 0;

        //    foreach (var item in lines.Where(l => l.IsHorizontal))
        //        item.MinMax(ref x1, ref x2, ref y1, ref y2);

        //    foreach (var item in lines.Where(l => !l.IsHorizontal))
        //        item.MinMax(ref y3, ref y4, ref x3, ref x4);


        //    if (x1 < x3)
        //        minX = (int)x1;
        //    else
        //        minX = x3;

        //    if (x2 > x4)
        //        maxX = (int)x2;
        //    else
        //        maxX = x4;

        //    if (y1 < y3)
        //        minY = y1;
        //    else
        //        minY = (int)y3;

        //    if (y2 > y4)
        //        maxY = y2;
        //    else
        //        maxY = (int)y4;

        //    if (maxX == 0 || maxY == 0)
        //        return false;

        //    return true;
        //}
        #endregion
    }
}
#endif
