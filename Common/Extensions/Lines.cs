/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public static partial class Lines
    {
        #region VARAIBLES & CONSTS
        public const float START_EPSILON = 0.05f;
        public const float END_EPSILON = 0.05f;
        public const float EPSILON = 0.0001f;
        public const float ParallelLineEPSILON = 0.001f;
        public const float LINE_TOLERANCE = 0.5f;
        #endregion

        #region LOCATION
        /// <summary>
        /// Returns a point given distance away in the line.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point of the line.</param>
        /// <param name="y1">Y co-ordinate of start point of the line.</param>
        /// <param name="m">Slope of the line.</param>
        /// <param name="distance">Distance to find a point from start point of the line.</param>
        /// <param name="steep">Steep ness of the line.</param>
        /// <param name="x2">X co-ordinate of reult point in the line.</param>
        /// <param name="y2">X co-ordinate of reult point in the line.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FindPoint(float x1, float y1, float m, float distance, bool steep, out float x2, out float y2)
        {
            x2 = x1;
            y2 = y1;
            var c = y1 - m * x1;
            if (steep)
            {
                y2 += distance;
                x2 = (y2 - c) / m;
            }
            else
            {
                x2 += distance;
                y2 = m * x2 + c;
            }
        }

        /// <summary>
        /// Returns a point given distance away in the line.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point of the line.</param>
        /// <param name="y1">Y co-ordinate of start point of the line.</param>
        /// <param name="m">Slope of the line.</param>
        /// <param name="distance">Distance to find a point from start point of the line.</param>
        /// <param name="steep">Steep ness of the line.</param>
        /// <param name="x2">X co-ordinate of reult point in the line.</param>
        /// <param name="y2">X co-ordinate of reult point in the line.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FindPoint(float x1, float y1, float m, float distance, bool steep, out int x2, out int y2)
        {
            var _x2 = x1;
            var _y2 = y1;
            var c = y1 - m * x1;
            if (steep)
            {
                _y2 += distance;
                _x2 = (_y2 - c) / m;
            }
            else
            {
                _x2 += distance;
                _y2 = m * _x2 + c;
            }
            x2 = _x2.Round();
            y2 = _y2.Round();
        }

        /// <summary>
        /// Returns a point given distance away in the line.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point of the line.</param>
        /// <param name="y1">Y co-ordinate of start point of the line.</param>
        /// <param name="ox2">X co-ordinate of end point of the line.</param>
        /// <param name="oy2">Y co-ordinate of start point of the line.</param>
        /// <param name="distance">Distance to find a point from start point of the line.</param>
        /// <param name="x2">X co-ordinate of reult point in the line.</param>
        /// <param name="y2">X co-ordinate of reult point in the line.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FindPoint(float x1, float y1, float ox2, float oy2, float distance, out float x2, out float y2)
        {
            var m = Vectors.Slope(x1, y1, ox2, oy2, out bool steep);
            FindPoint(x1, y1, m, distance, steep, out x2, out y2);
        }

        /// <summary>
        /// Returns a point given distance away in the line.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point of the line.</param>
        /// <param name="y1">Y co-ordinate of start point of the line.</param>
        /// <param name="ox2">X co-ordinate of end point of the line.</param>
        /// <param name="oy2">Y co-ordinate of start point of the line.</param>
        /// <param name="distance">Distance to find a point from start point of the line.</param>
        /// <param name="x2">X co-ordinate of reult point in the line.</param>
        /// <param name="y2">X co-ordinate of reult point in the line.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FindPoint(float x1, float y1, float ox2, float oy2, float distance, out int x2, out int y2)
        {
            var m = Vectors.Slope((float)x1, y1, ox2, oy2, out bool steep);
            FindPoint(x1, y1, m, distance, steep, out float _x2, out float _y2);
            x2 = _x2.Round();
            y2 = _y2.Round(); 
        }
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Parallel(ref float x1, ref float y1, ref float x2, ref float y2,
            float deviation, float length = 0, int roundingDigits = 0)
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Parallel(float lx1, float ly1, float lx2, float ly2,
            float deviation, out float x1, out float y1, out float x2, out float y2, float length = 0, int roundingDigits = 0)
        {
            x1 = lx1;
            y1 = ly1;
            x2 = lx2;
            y2 = ly2;
            Parallel(ref x1, ref y1, ref x2, ref y2, deviation, length, roundingDigits);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Parallel(float lx1, float ly1, float lx2, float ly2, int deviation, out int x1, out int y1, out int x2, out int y2, int length = 0)
        {
            Parallel(lx1, ly1, lx2, ly2, deviation, out float _x1, out float _y1, out float _x2, out float _y2, length);
            x1 = _x1.Round();
            y1 = _y1.Round();
            x2 = _x2.Round();
            y2 = _y2.Round();
        }
        #endregion

        #region ROUNDING OF LINE CO-ORDINATES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Perpendicular(float x1, float y1, float x2, float y2, float x, float y)
        {
            var k = ((y2 - y1) * (x - x1) - (x2 - x1) * (y - y1)) / (Numbers.Sqr(y2 - y1) + Numbers.Sqr(x2 - x1));
            var x4 = x - k * (y2 - y1);
            var y4 = y + k * (x2 - x1);
            return new VectorF(x4, y4);
        }
        #endregion

        #region CREATE AXIS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRotation AngleFrom(VectorF p1, VectorF p2, VectorF p3, VectorF p4)
        {
            var m = p1 - p2;
            var n = p3 - p4;
            float theta1 = (float)Math.Atan2(m.Y, m.X);
            float theta2 = (float)Math.Atan2(n.Y, n.X);
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLineAngle(float x1, float y1, float x2, float y2)
        {
            return (float)( Math.Atan2(y2 - y1, x2 - x1) * Angles.Radinv + 180);            
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetLineAngle(VectorF p, VectorF q)
        {
            return GetLineAngle(p.X, p.Y, q.X, q.Y);
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
        public static sbyte GetDirection(float lX1, float lY1, float lX2, float lY2, int axis, bool horizontal)
        {
            if (float.IsNaN(lX1) || float.IsNaN(lY1) || float.IsNaN(lX2) || float.IsNaN(lY2))
                return 0;

            var d1 = horizontal ? Math.Abs(lY1 - axis) : Math.Abs(lX1 - axis);
            var d2 = horizontal ? Math.Abs(lY2 - axis) : Math.Abs(lX2 - axis);
            var sign = horizontal ? lY1 < lY2 : lX1 < lX2;
            var sign1 = d1 < d2;
            if (sign == sign1)
                return 1;
            return -1;
        }
        #endregion

        #region MAKE DRAWABLE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MakeDrawable(bool Scanning, ref float x1, ref float y1, ref float x2, ref float y2, float? slope,
            out float m, out float c, Size clip = default(Size))
        {
            if (slope != null)
            {
                m = slope.Value;
            }
            else
            {
                var dx = x2 - x1;
                var dy = y2 - y1;
                m = dy;
                if (dx != 0)
                    m /= dx;
            }
            c = y1 - m * x1;

            int minX, minY, maxX, maxY;

            minX = -Vectors.UHD8kWidth;
            minY = -Vectors.UHD8kHeight;
            if (Scanning)
            {
                maxX = Vectors.UHD8kWidth;
                maxY = Vectors.UHD8kHeight;
            }
            else
            {
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
                x1 = (-c) / m;
            }
            if (y2 < minY)
            {
                y2 = 0;
                x2 = (-c) / m;
            }
            if (x1 > maxX)
            {
                x1 = maxX;
                y1 = m * x1 + c;
            }
            if (x2 > maxX)
            {
                x2 = maxX;
                y2 = m * x2 + c;
            }
            if (y1 > maxY)
            {
                y1 = maxY;
                x1 = (y1 - c) / m;
            }
            if (y2 > maxY)
            {
                y2 = maxY;
                x2 = (y2 - c) / m;
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DrawParams(float x1, float y1, float x2, float y2, bool horizontal, bool Scanning,
            out float m, out int step, out int Start, out int End, out float initialValue,
            out int lineLength, out float c, Size clip = default(Size))
        {
            m = 0;
            initialValue = 0;
            step = Start = End = 0;
            lineLength = 0;
            c = 0;

            if (float.IsNaN(x1) || float.IsNaN(y1) || float.IsNaN(x2) || float.IsNaN(y2))
            {
                return false;
            }

            Lines.MakeDrawable(Scanning, ref x1, ref y1, ref x2, ref y2, null, out m, out c, clip);

            step = 1;
            float min = horizontal ? y1 : x1;
            float max = horizontal ? y2 : x2;
            if (min < 0 && max < 0 && !Scanning)
            {
                return false;
            }
            if (horizontal)
                m = 1 / m;
            var tc = (horizontal ? -c * m : c);

            var negative = min > max;
            Start = (int)min;
            End = (int)max;
            if (Scanning)
            {
                if (min - Start != 0)
                    ++Start;
                if (max - End != 0)
                    ++End;
            }
            else
            {
                if (min - Start >= START_EPSILON)
                    ++Start;
                if (max - End >= END_EPSILON)
                    ++End;
            }
            initialValue = Start * m + tc;
            lineLength = negative ? Start - End : End - Start;
            if (negative)
            {
                m = -m;
                step = -step;
            }
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DrawParams(int ix1, int iy1, int ix2, int iy2, out bool horizontalScan,
            out int calculatedSlope, out int step, out int start, out int end, out int initialValue, Size clip = default(Size))
        {
            Lines.MakeDrawable(ref ix1, ref iy1, ref ix2, ref iy2);

            calculatedSlope = Vectors.Slope(ix1, iy1, ix2, iy2, out horizontalScan);
            step = 1;
            start = horizontalScan ? iy1 : ix1;
            end = horizontalScan ? iy2 : ix2;  
            initialValue = ((horizontalScan ? ix1 : iy1) << Vectors.BigExp);

            if (start < 0 && end < 0)
            {
                return false;
            }
            if (start > end)
                step = -step;

            return true;
        }
        #endregion

        #region > OR < THAN POINT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGreaterThan(float lX1, float lY1, float lX2, float lY2, float x, float y)
        {
            var c1 = (lX2 - lX1) * (y - lY1);
            var c2 = (lY2 - lY1) * (x - lX1);
            var result = c1 < c2;
            return result;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLessThan(float lX1, float lY1, float lX2, float lY2, float x, float y)
        {
            var c1 = (lX2 - lX1) * (y - lY1);
            var c2 = (lY2 - lY1) * (x - lX1);
            var result = c1 > c2;
            return result;
        }
        #endregion

        #region ROTATE
        /// <summary>
        /// Rotates a line by given rotation.
        /// If rotating and angle does not have any center, center of a line will be used to rotate otherwise center of angle will be used.
        /// </summary>
        /// <param name="degree">Rotation which to rotate the point by.</param>
        /// <param name="x1">X co-ordinate of start point of line.</param>
        /// <param name="y1">Y co-ordinate of start point of line.</param>
        /// <param name="x2">X co-ordinate of end point of line.</param>
        /// <param name="y2">Y co-ordinate of end point of line.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateLine(this IDegree degree, ref int x1, ref int y1, ref int x2, ref int y2,
            bool antiClock = false, bool noSkew = false)
        {
            bool isRotated = degree.EffectiveCenter(Numbers.Middle(x1, x2),
                Numbers.Middle(y1, y2), out int cx, out int cy);
            if (!isRotated)
                return;
            degree.Rotate(x1, y1, out x1, out y1, cx, cy, antiClock, noSkew);
            degree.Rotate(x2, y2, out x2, out y2, cx, cy, antiClock, noSkew);
        }

        /// <summary>
        /// Rotates a line by given rotation.
        /// If rotating and angle does not have any center, center of a line will be used to rotate otherwise center of angle will be used.
        /// </summary>
        /// <param name="degree">Rotation which to rotate the point by.</param>
        /// <param name="x1">X co-ordinate of start point of line.</param>
        /// <param name="y1">Y co-ordinate of start point of line.</param>
        /// <param name="x2">X co-ordinate of end point of line.</param>
        /// <param name="y2">Y co-ordinate of end point of line.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateLine(this IDegree degree, ref float x1, ref float y1, ref float x2, ref float y2,
            bool antiClock = false, bool noSkew = false)
        {
            bool isRotated = degree.EffectiveCenter(Numbers.Middle(x1, x2),
                Numbers.Middle(y1, y2), out float cx, out float cy);
            if (!isRotated)
                return;
            degree.Rotate(x1, y1, out x1, out y1, cx, cy, antiClock, noSkew);
            degree.Rotate(x2, y2, out x2, out y2, cx, cy, antiClock, noSkew);
        }
        #endregion
    }
}
