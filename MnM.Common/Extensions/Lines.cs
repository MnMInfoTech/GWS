/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if GWS || Window

    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    public static partial class Lines
    {
        #region VARAIBLES & CONSTS
        public const float EPSILON = .0001f;
        public const float ParallelLineEPSILON = 0.001f;
        #endregion

        #region PARALLEL
        /// <summary>
        /// Source Credit:https://rosettacode.org/wiki/Find_the_intersection_of_two_lines#C.23
        /// Indicates if two lines are parallel or not.
        /// </summary>
        /// <param name="s1X">X co-ordinate of start point of first line.</param>
        /// <param name="s1Y">Y co-ordinate of start point of first line.</param>
        /// <param name="e1X">X co-ordinate of end point of first line.</param>
        /// <param name="e1Y">Y co-ordinate of end point of first line.</param>
        /// <param name="s2X">X co-ordinate of start point of second line.</param>
        /// <param name="s2Y">Y co-ordinate of start point of second line.</param>
        /// <param name="e2X">X co-ordinate of end point of second line.</param>
        /// <param name="e2Y">X co-ordinate of end point of second line.</param>
        /// <returns>True if lines are papallel otherwise false.</returns>
        public static bool IsParallel(float s1X, float s1Y, float e1X, float e1Y, float s2X, float s2Y, float e2X, float e2Y)
        {
            float a1 = e1Y - s1Y;
            float b1 = s1X - e1X;

            float a2 = e2Y - s2Y;
            float b2 = s2X - e2X;

            float delta = a1 * b2 - a2 * b1;
            if (delta == 0)
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="deviation"></param>
        /// <param name="length"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Parallel(ref float x1, ref float y1, ref float x2, ref float y2, float deviation, float length = 0, int roundingDigits = 0)
        {
            if (deviation == 0)
                return;

            var w = x1 - x2;
            var h = y2 - y1;

            if (length == 0)
                length = (float)Math.Sqrt(Numbers.Sqr(w) + Numbers.Sqr(h));


            if (length == 0)
                length = 1;

            var dy = (deviation * h / length);
            var dx = (deviation * w / length);

            x1 += dy;
            x2 += dy;
            y1 += dx;
            y2 += dx;

            if (roundingDigits != 0)
                RoundLineCoordinates(ref x1, ref y1, ref x2, ref y2, roundingDigits);
        }

        /// <summary>
        /// Source Inspiration: https://stackoverflow.com/questions/2825412/draw-a-parallel-line Krumelur's answer
        /// </summary>
        /// <param name="lx1"></param>
        /// <param name="ly1"></param>
        /// <param name="lx2"></param>
        /// <param name="ly2"></param>
        /// <param name="deviation"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Parallel(float lx1, float ly1, float lx2, float ly2, float deviation, out float x1, out float y1, out float x2, out float y2, float length = 0, int roundingDigits = 0)
        {
            x1 = lx1;
            y1 = ly1;
            x2 = lx2;
            y2 = ly2;
            Parallel(ref x1, ref y1, ref x2, ref y2, deviation, length, roundingDigits);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Parallel(float lx1, float ly1, float lx2, float ly2, int deviation, out int x1, out int y1, out int x2, out int y2, int length = 0)
        {
            Parallel(lx1, ly1, lx2, ly2, deviation, out float _x1, out float _y1, out float _x2, out float _y2, length);
            x1 = _x1.Round();
            y1 = _y1.Round();
            x2 = _x2.Round();
            y2 = _y2.Round();
        }

        public static void ParallelAt(float x1, float y1, float m, float length, bool steep, out float x2, out float y2)
        {
            x2 = x1;
            y2 = y1;
            var c = y1 - m * x1;
            if (steep)
            {
                y2 += length;
                x2 = (y2 - c) / m;
            }
            else
            {
                x2 += length;
                y2 = m * x2 + c;
            }
        }
        public static void ParallelAt(float x1, float y1, float m, float length, bool steep, out int x2, out int y2)
        {
            ParallelAt(x1, y1, m, length, steep, out float _x2, out float _y2);
            x2 = _x2.Round();
            y2 = _y2.Round();
        }
        public static void ParallelAt(float x1, float y1, float ox2, float oy2, float length, out float x2, out float y2)
        {
            var m = Vectors.Slope(x1, y1, ox2, oy2, out bool steep);
            ParallelAt(x1, y1, m, length, steep, out x2, out y2);
        }
        public static void ParallelAt(int x1, int y1, int ox2, int oy2, int length, out int x2, out int y2)
        {
            var m = Vectors.Slope((float)x1, y1, ox2, oy2, out bool steep);
            ParallelAt(x1, y1, m, length, steep, out x2, out y2);
        }
        #endregion

        #region ROUNDING OF LINE CO-ORDINATES
        public static bool RoundLineCoordinates(ref float x1, ref float y1, ref float x2, ref float y2, int digits = 4)
        {
            if (float.IsNaN(x1) || float.IsNaN(y1) || float.IsNaN(x2) || float.IsNaN(y2) || ((x1 < 0 && x2 < 0) || (y1 < 0 && y2 < 0)))
                return false;
            if (x1 == x2)
            {
                x1 = x2 = x1.Round();
                return true;
            }
            x1 = (float)Math.Round(x1, digits);
            y1 = (float)Math.Round(y1, digits);
            x2 = (float)Math.Round(x2, digits);
            y2 = (float)Math.Round(y2, digits);
            return true;
        }
        #endregion

        #region MAKE DRAWABLE
        public static void MakeDrawable(bool Scanning, ref float x1, ref float y1, ref float x2, ref float y2, float? slope, 
            out float calculatedSlope, out float c, Size clip = default(Size))
        {
            if (slope != null)
            {
                calculatedSlope = slope.Value;
            }
            else
            {
                var dx = x2 - x1;
                var dy = y2 - y1;
                calculatedSlope = dy;
                if (dx != 0)
                    calculatedSlope /= dx;
            }
            c = y1 - calculatedSlope * x1;

            int minX, minY, maxX, maxY;
            if (Scanning)
            {
                minX = -Vectors.UHD8kWidth;
                minY = -Vectors.UHD8kHeight;
                maxX = Vectors.UHD8kWidth;
                maxY = Vectors.UHD8kHeight;
            }
            else
            {
                minX = 0;
                minY = 0;
                maxX = clip ? clip.Width : Vectors.UHD8kWidth;
                maxY = clip ? clip.Height : Vectors.UHD8kHeight;
            }

            if (x1 < minX)
            {
                x1 = 0;
                y1 = c;
            }
            else if (x2 < minX)
            {
                x2 = 0;
                y2 = c;
            }
            if (y1 < minY)
            {
                y1 = 0;
                x1 = (-c) / calculatedSlope;
            }
            if (y2 < minY)
            {
                y2 = 0;
                x2 = (-c) / calculatedSlope;
            }
            if (x1 > maxX)
            {
                x1 = maxX;
                y1 = calculatedSlope * x1 + c;
            }
            if (x2 > maxX)
            {
                x2 = maxX;
                y2 = calculatedSlope * x2 + c;
            }
            if (y1 > maxY)
            {
                y1 = maxY;
                x1 = (y1 - c) / calculatedSlope;
            }
            if (y2 > maxY)
            {
                y2 = maxY;
                x2 = (y2 - c) / calculatedSlope;
            }
        }
        public static void MakeDrawable(ref int ix1, ref int iy1, ref int ix2, ref int iy2, Size clip = default(Size))
        {
            float x1 = ix1;
            float y1 = iy1;
            float x2 = ix2;
            float y2 = iy2;
            MakeDrawable(false, ref x1, ref y1, ref x2, ref y2, null, out _, out _, clip);
            ix1 = x1.Round();
            iy1 = y1.Round();
            ix2 = x2.Round();
            iy2 = y2.Round();
        }
        #endregion

        #region DRAWABLE ESSENTIAL OF LINES
        public static bool DrawParams(float x1, float y1, float x2, float y2, bool horizontalScan, bool Scanning,
            out float calculatedSlope, out int step, out int start, out int end, out float initialValue, out bool direction, Size clip = default(Size))
        {
            if (float.IsNaN(x1) || float.IsNaN(y1) || float.IsNaN(x2) || float.IsNaN(y2))
            {
                calculatedSlope = 0;
                initialValue = 0;
                step = start = end = 0;
                direction = false;
                return false;
            }

            Lines.MakeDrawable(Scanning, ref x1, ref y1, ref x2, ref y2, null, out calculatedSlope, out float c, clip);

            step = 1;
            float min = horizontalScan ? y1 : x1;
            float max = horizontalScan ? y2 : x2;
            if (horizontalScan)
                calculatedSlope = 1 / calculatedSlope;
            var t = (horizontalScan ? -c * calculatedSlope : c);

            direction = min > max;
            bool Negative = direction;
            start = Scanning ? min.Ceiling() : min.Round();
            end = Scanning ? max.Ceiling() : max.Round();
            initialValue = start * calculatedSlope + t;

            if (Negative)
            {
                calculatedSlope = -calculatedSlope;
                step = -step;
            }
            return true;
        }

        public static bool DrawParams(int ix1, int iy1, int ix2, int iy2, out bool horizontalScan,
            out int calculatedSlope, out int step, out int start, out int end, out int initialValue, Size clip = default(Size))
        {
            Lines.MakeDrawable(ref ix1, ref iy1, ref ix2, ref iy2);

            calculatedSlope = Vectors.Slope(ix1, iy1, ix2, iy2, out horizontalScan);
            step = 1;
            start = horizontalScan ? iy1 : ix1;
            end = horizontalScan ? iy2 : ix2;
            initialValue = ((horizontalScan ? ix1 : iy1) << Vectors.BigExp);
            if (start > end)
                step = -step;

            return true;
        }
        #endregion

        #region PERPENDICULAR
        /// <summary>
        /// SOURCE: https://stackoverflow.com/questions/1811549/perpendicular-on-a-line-from-a-given-point
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static VectorF Perpendicular(float x1, float y1, float x2, float y2, float x, float y)
        {
            var k = ((y2 - y1) * (x - x1) - (x2 - x1) * (y - y1)) / (Numbers.Sqr(y2 - y1) + Numbers.Sqr(x2 - x1));
            var x4 = x - k * (y2 - y1);
            var y4 = y + k * (x2 - x1);
            return new VectorF(x4, y4);
        }
        #endregion

        #region CREATE AXIS
        public static void Axis(float x1, float y1, float x2, float y2, out float mx1, out float my1, out float mx2, out float my2)
        {
            var mx = Numbers.Middle(x1, x2);
            var my = Numbers.Middle(y1, y2);
            mx1 = mx - (y1 - my);
            my1 = my + (x1 - mx);
            mx2 = mx - (y2 - my);
            my2 = my + (x2 - mx);
        }
        #endregion

        #region ANGLE BETWEEN 2 LINES
        public static Rotation AngleFrom(VectorF p1, VectorF p2, VectorF p3, VectorF p4)
        {
            var m = p1 - p2;
            var n = p3 - p4;
            float theta1 = (float)Math.Atan2(m.Y, m.X);
            float theta2 = (float)Math.Atan2(n.Y, n.X);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            return new Rotation(angle);
        }
        public static Rotation AngleFromVerticalCounterPart(float x1, float y1, float x2, float y2)
        {
            float theta1 = (float)Math.Atan2(y2 - y1, x2 - x1);
            float theta2 = (float)Math.Atan2(y2 - y1, 0);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            return new Rotation(angle);
        }
        public static Rotation AngleFromHorizontalCounterPart(float x1, float y1, float x2, float y2)
        {
            float theta1 = (float)Math.Atan2(y2 - y1, x2 - x1);
            float theta2 = (float)Math.Atan2(y2 - y1, 0);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            return new Rotation(angle);
        }
        #endregion

        #region GET LINE ANGLE
        /// <summary>
        /// Get angle of a line.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static Rotation GetLineAngle(float x1, float y1, float x2, float y2)
        {
            bool Steep = Math.Abs(y1 - y2) > Math.Abs(x1 - x2);
            if (Steep)
                return AngleFromHorizontalCounterPart(x1, y1, x2, y2);
            else
                return AngleFromVerticalCounterPart(x1, y1, x2, y2);
        }
        public static Rotation GetLineAngle(VectorF p, VectorF q)
        {
            return GetLineAngle(p.X, p.Y, q.X, q.Y);
        }
        #endregion

        #region GET LINE TYPE
        public static LineType Type(float x1, float y1, float x2, float y2) =>
            Type(x1, y1, x2, y2, out _);
        public static LineType Type(float x1, float y1, float x2, float y2, out SlopeType steep)
        {
            return Type(x1, y1, x2, y2, out steep, out _);
        }
        public static LineType Type(float x1, float y1, float x2, float y2, out SlopeType steep, out float slope)
        {
            LineType t = LineType.Diagonal;
            var dx = x2 - x1;
            var dy = y2 - y1;
            slope = dy;
            if (dx != 0)
                slope /= dx;
            dy = Math.Abs(dy);
            dx = Math.Abs(dx);

            if (dy > dx)
                steep = SlopeType.Steep;
            else
                steep = SlopeType.NonSteep;

            if (dx <= EPSILON)
                t = LineType.Vertical;
            if (dy <= EPSILON)
            {
                if (t == LineType.Vertical)
                    t = LineType.Point;
                else
                    t = LineType.Horizontal;
            }
            return t;
        }
        public static void Type(int x1, int y1, int x2, int y2, out SlopeType steep, out float slope)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            slope = dy;
            if (dx != 0)
                slope /= dx;
            dy = Math.Abs(dy);
            dx = Math.Abs(dx);

            if (dy > dx)
                steep = SlopeType.Steep;
            else
                steep = SlopeType.NonSteep;
        }
        #endregion

        #region INTERSECTION
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public static RectangleF ToArea(this IEnumerable<ILine> lines)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = 0;
            float maxY = 0;

            foreach (var n in lines)
            {
                if (n == null)
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
        public static bool IsGreaterThan(this ILine l, float x, float y)
        {
            var c1 = (l.X2 - l.X1) * (y - l.Y1);
            var c2 = (l.Y2 - l.Y1) * (x - l.X1);
            var result = c1 < c2;
            return result;
        }
        public static bool IsLessThan(this ILine l, float x, float y)
        {
            var c1 = (l.X2 - l.X1) * (y - l.Y1);
            var c2 = (l.Y2 - l.Y1) * (x - l.X1);
            var result = c1 > c2;
            return result;
        }

        public static bool IsLessThan(this ILine l, VectorF p) =>
            l.IsLessThan(p.X, p.Y);
        public static bool IsGreaterThan(this ILine l, VectorF p) =>
            l.IsGreaterThan(p.X, p.Y);
        #endregion

        #region SLOPE
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
        public static Rotation AngleFrom(this ILine l1, ILine l2)
        {
            float theta1 = (float)Math.Atan2(l1.Y2 - l1.Y1, l1.X2 - l1.X1);
            float theta2 = (float)Math.Atan2(l2.Y2 - l2.Y1, l2.X2 - l2.X1);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            return new Rotation(angle);
        }
        public static Rotation AngleFromVerticalCounterPart(this ILine l) =>
            AngleFromVerticalCounterPart(l.X1, l.Y1, l.X2, l.Y2);
        public static Rotation AngleFromHorizontalCounterPart(this ILine l) =>
            AngleFromHorizontalCounterPart(l.X1, l.Y1, l.X2, l.Y2);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Scan(this ILine l, float axis, bool horizontal, out float value)
        {
            value = float.NaN;

            if (!l.Valid || l.Type == LineType.Point)
                return false;
            bool exit;

            if (horizontal)
            {
                if (l.Type == LineType.Horizontal)
                    return false;
                if (l.Y1 > l.Y2)
                    exit = axis < l.Y2.Ceiling() || axis >= l.Y1.Ceiling();
                else
                    exit = axis < l.Y1.Ceiling() || axis >= l.Y2.Ceiling();
                if (exit)
                    return false;

                value = (axis - l.C) / l.M;
            }
            else
            {
                if (l.Type == LineType.Vertical)
                    return false;
                if (l.X1 > l.X2)
                    exit = axis < l.X2.Ceiling() || axis >= l.X1.Ceiling();
                else
                    exit = axis < l.X1.Ceiling() || axis >= l.X2.Ceiling();
                if (exit)
                    return false;
                value = l.M * axis + l.C;
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
        public static sbyte GetDirection(this ILine l, int axis, bool horizontal)
        {
            if (!l.Valid)
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
        public static bool IsParallel(this ILine l1, ILine l2)
        {
            return IsParallel(l1.X1, l1.Y1, l1.X2, l1.Y2, l2.X1, l2.Y1, l2.X2, l2.Y2);
        }
        #endregion

        #region LINE TYPE
                public static LineType Type(this ILine line) =>
            Type(line.X1, line.Y1, line.X2, line.Y2, out _);
        #endregion
    }
#endif
}
