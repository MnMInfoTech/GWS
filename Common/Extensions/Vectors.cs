/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static partial class Vectors
    {
        #region VARIABLES
        static readonly PrimitiveList<int> Slopes;
        public const int BigExp = 20;
        public const int Big = (1 << BigExp);
        public const int LOffset = 1;
        //8K UHD 7680x4320;
        public const int UHD8kWidth = 7680;
        public const int UHD8kHeight = 4320;
        #endregion

        #region CONSTRUCTOR
        static Vectors()
        {
            Slopes = new PrimitiveList<int>(3001);
            Slopes.Add(0);
            for (int i = 1; i < 3001; i++)
                Slopes.Add(Big / i);
        }
        internal static void Initialize() { }
        #endregion

        #region CLOSENESS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool VeryCloseTo(this VectorF v, VectorF other, float threshold = 1.5f)
        {
            float dx = v.X - other.X;
            float dy = v.Y - other.Y;
            return (float)(Numbers.Sqr(dx) + Numbers.Sqr(dy)) < threshold;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool VeryCloseTo(this Vector v, Vector other, float threshold = 1.5f)
        {
            float dx = v.X - other.X;
            float dy = v.Y - other.Y;
            return (float)(Numbers.Sqr(dx) + Numbers.Sqr(dy)) < threshold;
        }
        #endregion

        #region CLEAN
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PrimitiveList<VectorF> Clean(this IEnumerable<VectorF> data, PointJoin join, VectorF? offset = null)
        {
            int count = data.Count();

            var list = new PrimitiveList<VectorF>(count);
            bool unique = (join & PointJoin.NoRepeat) == PointJoin.NoRepeat;
            bool hasoffset = offset != null;
            VectorF off = offset ?? VectorF.Empty;
            VectorF p0, first;
            p0 = first = default(VectorF);

            foreach (var item in data)
            {
                if (!item)
                    continue;

                if (!first)
                    first = item;

                if (unique && p0 && item.Equals(p0))
                    continue;

                if (hasoffset)
                    list.Add(item + off);
                else
                    list.Add(item);

                p0 = item;
            }
            return list;
        }
        #endregion

        #region STROKING
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IList<VectorF> StrokePoints(this IEnumerable<VectorF> source, 
            float stroke, PointJoin join, bool isCircular)
        {
            VectorF[] pts;
            int Count = 0;
            if(source is IArrayHolder<VectorF>)
            {
                pts = ((IArrayHolder<VectorF>)source).Data;
                Count = ((IArrayHolder<VectorF>)source).Count;
            }
            else
            {
                pts = source.ToArray();
                Count = pts.Length;
            }
            //var pts = source.Clean(join);

            if (Count == 0 || Count == 1)
                return pts;

            if (Count == 2)
            {
                Parallel(pts[0], pts[1], stroke, out VectorF a, out VectorF b);
                return new VectorF[] { a, b };
            }

            VectorF[] points = new VectorF[Count];

            var triangle = TriangleStroke(pts[0], pts[1], pts[2], stroke);

            points[0] = triangle[0];

            int j = 0;
            int len = Count;
            fixed (VectorF* pt = pts)
            {
                fixed (VectorF* point = points)
                {
                    for (int i = 2; i < len; i++)
                    {
                        triangle = TriangleStroke(pts[i - 2], pts[i - 1], pts[i], stroke);
                        point[++j] = triangle[1];
                    }
                    point[points.Length - 1] = triangle[2];

                    if (isCircular)
                    {
                        point[points.Length - 1] = TriangleStroke(pts[Count - 2], pts[Count - 1], pts[0], stroke)[1];
                        point[0] = TriangleStroke(pts[Count - 1], pts[0], pts[1], stroke)[1];
                    }
                }
            }
            return points;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static VectorF[] TriangleStroke(VectorF p1, VectorF p2, VectorF p3, float stroke)
        {
            VectorF intersection, first, b, middle, last;

            Parallel(p1, p2, stroke, out first, out b, 0);
            Parallel(p2, p3, stroke, out middle, out last, 0);

            if (!Intersects(first, b, middle, last, out intersection))
                intersection = (b + middle) / 2;

            return new VectorF[] { first, intersection, last };
        }
        #endregion

        #region PARALLEL
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Parallel(VectorF p1, VectorF p2, float deviation, out VectorF p3, out VectorF p4, float length = 0)
        {
            float x1, y1, x2, y2;
            Lines.Parallel(p1.X, p1.Y, p2.X, p2.Y, deviation, out x1, out y1, out x2, out y2, length);
            p3 = new VectorF(x1, y1);
            p4 = new VectorF(x2, y2);
        }

        /// <summary>
        /// Source Credit:https://rosettacode.org/wiki/Find_the_intersection_of_two_lines#C.23
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="e1"></param>
        /// <param name="s2"></param>
        /// <param name="e2"></param>
        /// <param name="P"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        public static bool IsParallel(VectorF s1, VectorF e1, VectorF s2, VectorF e2) =>
            IsParallel(s1.X, s1.Y, e1.X, e1.Y, s2.X, s2.Y, e2.X, e2.Y);
        #endregion

        #region INTRSECTION
        /// <summary>
        /// Source Credit:https://rosettacode.org/wiki/Find_the_intersection_of_two_lines#C.23
        /// </summary>
        /// <param name="p1">Start point of first line</param>
        /// <param name="p2">End point of first line</param>
        /// <param name="p3">Start point of second line<</param>
        /// <param name="p4">End point of section line<</param>
        /// <param name="intersection">If segments intesect returns intesection point otherwise invalid point</param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(VectorF p1, VectorF p2, VectorF p3, VectorF p4, out VectorF intersection) =>
            Intersects(p1, p2, p3, p4, out intersection, out _);

        /// <summary>
        /// Source Credit:https://rosettacode.org/wiki/Find_the_intersection_of_two_lines#C.23
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="intersection"></param>
        /// <param name="parallel"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(VectorF p1, VectorF p2, VectorF p3, VectorF p4, out VectorF intersection, out bool parallel)
        {
            parallel = false;
            /* If any end point of first line equals any end point of second line than we already have an intersection. */
            if (p1.Equals(p3))
            {
                intersection = p1;
                return true;
            }
            if (p1.Equals(p4))
            {
                intersection = p1;
                return true;
            }

            if (p2.Equals(p3))
            {
                intersection = p2;
                return true;
            }
            if (p2.Equals(p4))
            {
                intersection = p2;
                return true;
            }
            float a1 = p2.Y - p1.Y;
            float b1 = p1.X - p2.X;
            float c1 = a1 * p1.X + b1 * p1.Y;

            float a2 = p4.Y - p3.Y;
            float b2 = p3.X - p4.X;
            float c2 = a2 * p3.X + b2 * p3.Y;

            float delta = a1 * b2 - a2 * b1;
            if (delta == 0)
            {
                parallel = true;
                intersection = VectorF.Empty;
                return false;
            }

            var x = (b2 * c1 - b1 * c2) / delta;
            var y = (a1 * c2 - a2 * c1) / delta;

            if (x < -UHD8kWidth ||
                x > UHD8kWidth ||
                y < -UHD8kHeight ||
                y > UHD8kHeight)
            {
                parallel = true;
                intersection = VectorF.Empty;
                return false;
            }
            intersection = new VectorF(x, y);
            return true;
        }

        #endregion

        #region SCALE
        /// <summary>
        /// Returns scaled version of specified co-ordinates in respect of scale ratios and given center co-ordinates.
        /// </summary>
        /// <param name="x">X co-ordinate of location to be scaled.</param>
        /// <param name="y">Y co-ordinate of location to be scaled.</param>
        /// <param name="sx">Ratio of scaling X co-ordinate.</param>
        /// <param name="sy">Ratio of scaling Y co-ordinate.</param>
        /// <param name="cx">X co-ordinate of center to be used in scaling.</param>
        /// <param name="cy">X co-ordinate of center to be used in scaling.</param>
        /// <param name="x1">Scaled version of specified X co-ordinate.</param>
        /// <param name="y1">Scaled version of specified Y co-ordinate.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(float x, float y, float sx, float sy, float cx, float cy, out float x1, out float y1)
        {
            x1 = x;
            y1 = y;

            if (sx == 1 && sy == 1 || sx == 0 && sx == 0)
                return;

            if (sx != 1)
            {
                x1 = (x1 - cx) * sx + cx;
            }
            if (sy != 1)
            {
                y1 = (y1 - cy) * sy + cy;
            }
        }

        /// <summary>
        /// Returns scaled version of specified co-ordinates in respect of scale ratios and given center co-ordinates.
        /// </summary>
        /// <param name="x">X co-ordinate of location to be scaled.</param>
        /// <param name="y">Y co-ordinate of location to be scaled.</param>
        /// <param name="sx">Ratio of scaling X co-ordinate.</param>
        /// <param name="sy">Ratio of scaling Y co-ordinate.</param>
        /// <param name="cx">X co-ordinate of center to be used in scaling.</param>
        /// <param name="cy">X co-ordinate of center to be used in scaling.</param>
        /// <param name="x1">Scaled version of specified X co-ordinate.</param>
        /// <param name="y1">Scaled version of specified Y co-ordinate.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(int x, int y, float sx, float sy, float cx, float cy, out int x1, out int y1)
        {
            x1 = x;
            y1 = y;

            if (sx == 1 && sy == 1 || sx == 0 && sx == 0)
                return;

            float _x1 = x;
            float _y1 = y;

            if (sx != 1)
            {
                _x1 = (_x1 - cx) * sx + cx;
                x1 = _x1.Round();
            }
            if (sy != 1)
            {
                _y1 = (_y1 - cy) * sy + cy;
                y1 = _y1.Round();
            }
        }

        /// <summary>
        /// Returns scaled version of specified co-ordinates in respect of scale ratios and given center co-ordinates.
        /// </summary>
        /// <param name="scale">Scale object which offers scaling ratios.</param>
        /// <param name="x">X co-ordinate of location to be scaled.</param>
        /// <param name="y">Y co-ordinate of location to be scaled.</param>
        /// <param name="cx">X co-ordinate of center to be used in scaling.</param>
        /// <param name="cy">X co-ordinate of center to be used in scaling.</param>
        /// <param name="x1">Scaled version of specified X co-ordinate.</param>
        /// <param name="y1">Scaled version of specified Y co-ordinate.</param>
        public static void Scale(this IScale scale, float x, float y, float cx, float cy, out float x1, out float y1)
        {
            x1 = x;
            y1 = y;
            if (scale == null || !scale.HasScale)
                return;
            Scale(x, y, scale.X, scale.Y, cx, cy, out x1, out y1);
        }

        /// <summary>
        /// Returns scaled version of specified co-ordinates in respect of scale ratios and given center co-ordinates.
        /// </summary>
        /// <param name="scale">Scale object which offers scaling ratios.</param>
        /// <param name="x">X co-ordinate of location to be scaled.</param>
        /// <param name="y">Y co-ordinate of location to be scaled.</param>
        /// <param name="cx">X co-ordinate of center to be used in scaling.</param>
        /// <param name="cy">X co-ordinate of center to be used in scaling.</param>
        /// <param name="x1">Scaled version of specified X co-ordinate.</param>
        /// <param name="y1">Scaled version of specified Y co-ordinate.</param>
        public static void Scale(this IScale scale, int x, int y, float cx, float cy, out int x1, out int y1)
        {
            x1 = x;
            y1 = y;
            if (scale == null || !scale.HasScale)
                return;
            Scale(x, y, scale.X, scale.Y, cx, cy, out x1, out y1);
        }

        /// <summary>
        /// Returns scaled version of specified co-ordinates in respect of scale ratios and given center co-ordinates.
        /// </summary>
        /// <param name="scale">Scale object which offers scaling ratios.</param>
        /// <param name="x">X co-ordinate of location to be scaled.</param>
        /// <param name="y">Y co-ordinate of location to be scaled.</param>
        /// <param name="cx">X co-ordinate of center to be used in scaling.</param>
        /// <param name="cy">X co-ordinate of center to be used in scaling.</param>
        /// <param name="x1">Scaled version of specified X co-ordinate.</param>
        /// <param name="y1">Scaled version of specified Y co-ordinate.</param>
        public static void Scale(this IScale scale, int x, int y, int cx, int cy, out int x1, out int y1)
        {
            x1 = x;
            y1 = y;
            if (scale == null || !scale.HasScale)
                return;
            Scale(x, y, scale.X, scale.Y, cx, cy, out x1, out y1);
        }
        #endregion

        #region DISTANCE SQUARED
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(float p1X, float p1Y, float p2X, float p2Y)
        {
            float dx = p1X - p2X;
            float dy = p1Y - p2Y;

            return Numbers.Sqr(dx + dy);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(float p1X, float p1Y, float p2X, float p2Y)
        {
            float dx = p1X - p2X;
            float dy = p1Y - p2Y;

            return (float)Math.Sqrt(Numbers.Sqr(dx + dy));
        }
        #endregion

        #region SLOPE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(float x1, float y1, float x2, float y2)
        {
            var DX = x2 - x1;
            var DY = y2 - y1;
            var Slope = DY;
            if (DX != 0)
                Slope /= DX;
            return Slope;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(float x1, float y1, float x2, float y2, out float Slope)
        {
            var dx = x2 - x1;
            var dy = y2 - y1;
            var m = dy;
            if (dx != 0)
                m /= dx;
            Slope = y1 - m * x1;
            return m;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(float x1, float y1, float x2, float y2, out bool Steep)
        {
            var DX = x2 - x1;
            var DY = y2 - y1;
            var Slope = DY;
            if (DX != 0)
                Slope /= DX;
            if (DX < 0)
                DX = -DX;
            if (DY < 0)
                DY = -DY;
            Steep = DY >= DX;
            return Slope;
        }

        /// <summary>
        /// Finds slope of a given line strictly for the purpose of drawing line only.
        /// Calculation is optimized for that. Do not use this slope as a standard slope in y = mx + c;
        /// It does not represent m and can not be used to calculate c or other x or y directly.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <returns>Integr slope of the line to run or rise while drawing.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Slope(int x1, int y1, int x2, int y2)
        {
            return Slope(x1, y1, x2, y2, out _);
        }

        /// <summary>
        /// Finds slope of a given line strictly for the purpose of drawing line only.
        /// Calculation is optimized for that. Do not use this slope as a standard slope in y = mx + c;
        /// It does not represent m and can not be used to calculate c or other x or y directly.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="Steep">Returns steepnes of line.</param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Slope(int x1, int y1, int x2, int y2, out bool Steep)
        {
            var dx = (x2 - x1);
            var dy = (y2 - y1);
            var DX = (dx);
            var DY = (dy);
            if (DX < 0)
                DX = -DX;
            if (DY < 0)
                DY = -DY;
            Steep = DY >= DX;
            int s = (Steep? DY : DX) / Big;
            if (s == 0)
                s = 1;
            int Slope;

            if (Steep)
            {
                if (DY > 3000)
                    Slope = dx / s;
                else
                    Slope = dx * Slopes[DY];
            }
            else
            {
                if (DX > 3000)
                    Slope = dy / s;
                else

                    Slope = dy * Slopes[DX];
            }
            return Slope;
        }

        /// <summary>
        /// Finds slope of a given line strictly for the purpose of drawing line only.
        /// Calculation is optimized for that. Do not use this slope as a standard slope in y = mx + c;
        /// It does not represent m and can not be used to calculate c or other x or y directly.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="Steep">Returns steepnes of line.</param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Slope(out bool Steep, float x1, float y1, float x2, float y2)
        {
            int ix1 = x1.Round();
            int iy1 = y1.Round();
            int ix2 = x2.Round();
            int iy2 = y2.Round();

            return Slope(ix1, iy1, ix2, iy2, out Steep);
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
        static bool IsParallel(float s1X, float s1Y, float e1X, float e1Y, float s2X, float s2Y, float e2X, float e2Y)
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

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ParallelAt(float x1, float y1, float m, float length, bool steep, out float x2, out float y2)
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ParallelAt(float x1, float y1, float m, float length, bool steep, out int x2, out int y2)
        {
            ParallelAt(x1, y1, m, length, steep, out float _x2, out float _y2);
            x2 = _x2.Round();
            y2 = _y2.Round();
        }
        #endregion

        #region FIND RHOMBUS POINT
        /// <summary>
        /// Finds co-ordinates of missing 4th point of rhombus - derived from co-ordinates of 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="x1">X co-ordinate of 1st of rhombus.</param>
        /// <param name="y1">Y co-ordinate of 1st point of rhombus.</param>
        /// <param name="x2">X co-ordinate of 2nd point of rhombus.</param>
        /// <param name="y2">Y co-ordinate of 2nd point of rhombus.</param>
        /// <param name="x3">X co-ordinate of 3rd point of rhombus.</param>
        /// <param name="y3">Y co-ordinate of 3rd point of rhombus.</param>
        /// <param name="x">X co-ordinate of missing 4th point of rhombus.</param>
        /// <param name="y">Y co-ordinate of missing 4th point of rhombus.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FourthPointOfRhombus(float x1, float y1, float x2, float y2, 
            float x3, float y3, out float x, out float y)
        {
            x = x1 + x2 - x3;
            y = y1 + y2 - y3;
        }

        /// <summary>
        /// Finds co-ordinates of missing 4th point of rhombus - derived from co-ordinates of 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="x1">X co-ordinate of 1st of rhombus.</param>
        /// <param name="y1">Y co-ordinate of 1st point of rhombus.</param>
        /// <param name="x2">X co-ordinate of 2nd point of rhombus.</param>
        /// <param name="y2">Y co-ordinate of 2nd point of rhombus.</param>
        /// <param name="x3">X co-ordinate of 3rd point of rhombus.</param>
        /// <param name="y3">Y co-ordinate of 3rd point of rhombus.</param>
        /// <param name="boundsX">X co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="boundsY">Y co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="boundsR">Far right X co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="boundsB">>Far bottom Y co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="x">X co-ordinate of missing 4th point of rhombus.</param>
        /// <param name="y">Y co-ordinate of missing 4th point of rhombus.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FourthPointOfRhombus(float x1, float y1, float x2, float y2,
            float x3, float y3, float boundsX, float boundsY, float boundsR, float boundsB, 
            out float x, out float y)
        {
            x = x1 + x2 - x3;
            y = y1 + y2 - y3;
            if (x + 1 >= boundsX && y + 1 >= boundsY && x <= boundsR && y <= boundsB)
                return;
            x = x1 + x3 - x2;
            y = y1 + y3 - y2;
            if (x + 1 >= boundsX && y + 1 >= boundsY && x <= boundsR && y <= boundsB)
                return;
            x = x2 + x3 - x1;
            y = y2 + y3 - y1;
        }

        /// <summary>
        /// Finds co-ordinates of missing 4th point of rhombus - derived from co-ordinates of 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="x1">X co-ordinate of 1st of rhombus.</param>
        /// <param name="y1">Y co-ordinate of 1st point of rhombus.</param>
        /// <param name="x2">X co-ordinate of 2nd point of rhombus.</param>
        /// <param name="y2">Y co-ordinate of 2nd point of rhombus.</param>
        /// <param name="x3">X co-ordinate of 3rd point of rhombus.</param>
        /// <param name="y3">Y co-ordinate of 3rd point of rhombus.</param>
        /// <param name="x">X co-ordinate of missing 4th point of rhombus.</param>
        /// <param name="y">Y co-ordinate of missing 4th point of rhombus.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FourthPointOfRhombus(int x1, int y1, int x2, int y2,
            int x3, int y3, out int x, out int y)
        {
            x = x1 + x2 - x3;
            y = y1 + y2 - y3;
        }

        /// <summary>
        /// Finds co-ordinates of missing 4th point of rhombus - derived from co-ordinates of 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="x1">X co-ordinate of 1st of rhombus.</param>
        /// <param name="y1">Y co-ordinate of 1st point of rhombus.</param>
        /// <param name="x2">X co-ordinate of 2nd point of rhombus.</param>
        /// <param name="y2">Y co-ordinate of 2nd point of rhombus.</param>
        /// <param name="x3">X co-ordinate of 3rd point of rhombus.</param>
        /// <param name="y3">Y co-ordinate of 3rd point of rhombus.</param>
        /// <param name="boundsX">X co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="boundsY">Y co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="boundsR">Far right X co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="boundsB">>Far bottom Y co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="x">X co-ordinate of missing 4th point of rhombus.</param>
        /// <param name="y">Y co-ordinate of missing 4th point of rhombus.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FourthPointOfRhombus(int x1, int y1, int x2, int y2,
            int x3, int y3, int boundsX, int boundsY, int boundsR, int boundsB,
            out int x, out int y)
        {
            x = x1 + x2 - x3;
            y = y1 + y2 - y3;
            if (x + 1 >= boundsX && y + 1 >= boundsY && x <= boundsR && y <= boundsB)
                return;
            x = x1 + x3 - x2;
            y = y1 + y3 - y2;
            if (x + 1 >= boundsX && y + 1 >= boundsY && x <= boundsR && y <= boundsB)
                return;
            x = x2 + x3 - x1;
            y = y2 + y3 - y1;
        }
        #endregion

        #region AREA OF TRIANGLE
        /// <summary>
        /// Gets area of triangle
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <returns>Area of triangle</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float AreaOfTriangle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            var result =((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) * 0.5f);
            if (result < 0)
                result = -result;
            return result;
        }
        #endregion

        #region POINT FROM INDEX
        /// <summary>
        /// Since width is known, this method reveals an X and Y coordinates of a given index.
        /// GWS defines blocks as a 1D array represent as 2D array of certain width upto certain height.
        /// So the equation for finding given index is x + y * width.
        /// This method is sort of an another representation of that equation.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void XYOf(this int index, int width, out int x, out int y)
        {
            y = index / width;
            x = index - y * width;
        }
        #endregion

        #region AXIAL POINT TO XY
        public static void ToXY(int val, int axis, bool horizontal, out int x, out int y)
        {
            x = horizontal ? val : axis;
            y = horizontal ? axis : val;
        }
        public static void ToXY(float val, float axis, bool horizontal, out float x, out float y)
        {
            x = horizontal ? val : axis;
            y = horizontal ? axis : val;
        }
        public static void ToXY(ref int val, ref int axis, ref bool horizontal, out int x, out int y)
        {
            x = horizontal ? val : axis;
            y = horizontal ? axis : val;
        }
        public static void ToXY(ref float val, ref float axis, ref bool horizontal, out float x, out float y)
        {
            x = horizontal ? val : axis;
            y = horizontal ? axis : val;
        }
        #endregion

        #region REFLECT & TRANSFORM
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Reflect(this VectorF v, VectorF normal)
        {
            float dot = v.X * normal.X + v.Y * normal.Y;

            return new VectorF(
                v.X - 2.0f * dot * normal.X,
                v.Y - 2.0f * dot * normal.Y, v.Kind);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Reflect(this Vector v, Vector normal)
        {
            int dot = v.X * normal.X + v.Y * normal.Y;

            return new Vector(
                v.X - 2 * dot * normal.X,
                v.Y - 2 * dot * normal.Y, v.Kind);
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector TransformNormal(this Vector v, IMatrix3x2 matrix)
        {
            return new Vector(
                (v.X * matrix.M00 + v.Y * matrix.M10).Round(),
                (v.X * matrix.M01 + v.Y * matrix.M11).Round(), v.Kind);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Transform(this VectorF v, IMatrix3x2 matrix)
        {
            return new VectorF(
                v.X * matrix.M00 + v.Y * matrix.M10 + matrix.M20,
                v.X * matrix.M01 + v.Y * matrix.M11 + matrix.M21, v.Kind);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF TransformNormal(this VectorF v, IMatrix3x2 matrix)
        {
            return new VectorF(
                v.X * matrix.M00 + v.Y * matrix.M10,
                v.X * matrix.M01 + v.Y * matrix.M11, v.Kind);
        }
        #endregion

        #region CLAMP & LERP
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Clamp(this VectorF v, VectorF min, VectorF max)
        {
            // This compare order is very important!!!
            // We must follow HLSL behavior  the case user specified min value is bigger than max value.
            float x = v.X;
            x = (min.X > x) ? min.X : x;  // max(x, minx)
            x = (max.X < x) ? max.X : x;  // min(x, maxx)

            float y = v.Y;
            y = (min.Y > y) ? min.Y : y;  // max(y, miny)
            y = (max.Y < y) ? max.Y : y;  // min(y, maxy)

            return new VectorF(x, y, v.Kind);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Lerp(this VectorF v, VectorF p2, float amount)
        {
            return new VectorF(
                v.X + (p2.X - v.X) * amount,
                v.Y + (p2.Y - v.Y) * amount, v.Kind);
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Clamp(this Vector v, Vector min, Vector max)
        {
            // This compare order is very important!!!
            // We must follow HLSL behavior  the case user specified min value is bigger than max value.
            int x = v.X;
            x = (min.X > x) ? min.X : x;  // max(x, minx)
            x = (max.X < x) ? max.X : x;  // min(x, maxx)

            int y = v.Y;
            y = (min.Y > y) ? min.Y : y;  // max(y, miny)
            y = (max.Y < y) ? max.Y : y;  // min(y, maxy)

            return new Vector(x, y, v.Kind);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Lerp(this Vector v, Vector p2, float amount)
        {
            return new VectorF(
                v.X + (p2.X - v.X) * amount,
                v.Y + (p2.Y - v.Y) * amount, v.Kind).Round();
        }
        #endregion

        #region MIN MAX
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(
          out float minX, out float minY, out float maxX, out float maxY, params float[] points)
        {
            minX = minY = float.MaxValue;
            maxX = maxY = 0;
            float previous = float.MaxValue;
            float x = 0, y;

            foreach (var p in points)
            {
                if (previous == float.MaxValue)
                {
                    x = p;
                    previous = 0;
                }
                else
                {
                    y = p;
                    previous = float.MaxValue;
                    if (x < minX)
                        minX = x;
                    if (y < minY)
                        minY = y;
                    if (x > maxX)
                        maxX = x;
                    if (y > maxY)
                        maxY = y;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(
            out int minX, out int minY, out int maxX, out int maxY, params int[] points)
        {
            minX = minY = int.MaxValue;
            maxX = maxY = 0;
            int previous = int.MaxValue;
            int x = 0, y;

            foreach (var p in points)
            {
                if (previous == int.MaxValue)
                {
                    x = p;
                    previous = 0;
                }
                else
                {
                    y = p;
                    previous = int.MaxValue;
                    if (x < minX)
                        minX = x;
                    if (y < minY)
                        minY = y;
                    if (x > maxX)
                        maxX = x;
                    if (y > maxY)
                        maxY = y;
                }
            }
        }
        #endregion

        #region CONTAINS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool Contains(this IEnumerable<VectorF> points, float x, float y)
        {
            if (points == null)
                return false;
            VectorF[] Points;
            if (points is VectorF[])
                Points = (VectorF[])points;
            else
                Points = points.ToArray();

            int i, j;
            int nvert = Points.Length;
            bool c = false;
            fixed (VectorF* pt = Points)
            {
                for (i = 0, j = nvert - 1; i < nvert; j = i++)
                {
                    if (((pt[i].Y > y) != (pt[j].Y > y)) &&
                     (x < (pt[j].X - pt[i].X) * (y - pt[i].Y) / (pt[j].Y - pt[i].Y) + pt[i].X))
                        c = !c;
                }
            }
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool Contains(this IEnumerable<Point> points, float x, float y)
        {
            if (points == null)
                return false;
            Point[] Points;
            if (points is Point[])
                Points = (Point[])points;
            else
                Points = points.ToArray();

            int i, j;
            int nvert = Points.Length;
            bool c = false;
            fixed (Point* pt = Points)
            {
                for (i = 0, j = nvert - 1; i < nvert; j = i++)
                {
                    if (((pt[i].Y > y) != (pt[j].Y > y)) &&
                     (x < (pt[j].X - pt[i].X) * (y - pt[i].Y) / (pt[j].Y - pt[i].Y) + pt[i].X))
                        c = !c;
                }
            }
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool Contains(this IEnumerable<Vector> points, float x, float y)
        {
            if (points == null)
                return false;
            Vector[] Points;
            if (points is Vector[])
                Points = (Vector[])points;
            else
                Points = points.ToArray();

            int i, j;
            int nvert = Points.Length;
            bool c = false;
            fixed (Vector* pt = Points)
            {
                for (i = 0, j = nvert - 1; i < nvert; j = i++)
                {
                    if (((pt[i].Y > y) != (pt[j].Y > y)) &&
                     (x < (pt[j].X - pt[i].X) * (y - pt[i].Y) / (pt[j].Y - pt[i].Y) + pt[i].X))
                        c = !c;
                }
            }
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool Contains<T>(this IEnumerable<VectorF> points, IEnumerable<T> other) 
            where T : IPointF
        {
            if (points == null || other == null)
                return false;
            VectorF[] Points;
            if (points is VectorF[])
                Points = (VectorF[])points;
            else
                Points = points.ToArray();

            int i, j;
            int nvert = Points.Length;
            bool c = false;
            float x, y;
            foreach (var item in other)
            {
                x = item.X;
                y = item.Y;
                fixed (VectorF* pt = Points)
                {
                    for (i = 0, j = nvert - 1; i < nvert; j = i++)
                    {
                        if (((pt[i].Y > y) != (pt[j].Y > y)) &&
                         (x < (pt[j].X - pt[i].X) * (y - pt[i].Y) / (pt[j].Y - pt[i].Y) + pt[i].X))
                            c = !c;
                    }
                }
            }
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool Contains<T>(this IEnumerable<Vector> points, IEnumerable<T> other)
            where T : IPoint
        {
            if (points == null || other == null)
                return false;

            Vector[] Points;
            if (points is Vector[])
                Points = (Vector[])points;
            else
                Points = points.ToArray();

            int i, j;
            int nvert = Points.Length;
            bool c = false;
            int x, y;
            foreach (var item in other)
            {
                x = item.X;
                y = item.Y;
                fixed (Vector* pt = Points)
                {
                    for (i = 0, j = nvert - 1; i < nvert; j = i++)
                    {
                        if (((pt[i].Y > y) != (pt[j].Y > y)) &&
                         (x < (pt[j].X - pt[i].X) * (y - pt[i].Y) / (pt[j].Y - pt[i].Y) + pt[i].X))
                            c = !c;
                    }
                }
            }
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static bool Contains<T>(this IEnumerable<Point> points, IEnumerable<T> other)
            where T : IPoint
        {
            if (points == null || other == null)
                return false;

            Point[] Points;
            if (points is Point[])
                Points = (Point[])points;
            else
                Points = points.ToArray();

            int i, j;
            int nvert = Points.Length;
            bool c = false;
            int x, y;
            foreach (var item in other)
            {
                x = item.X;
                y = item.Y;
                fixed (Point* pt = Points)
                {
                    for (i = 0, j = nvert - 1; i < nvert; j = i++)
                    {
                        if (((pt[i].Y > y) != (pt[j].Y > y)) &&
                         (x < (pt[j].X - pt[i].X) * (y - pt[i].Y) / (pt[j].Y - pt[i].Y) + pt[i].X))
                            c = !c;
                    }
                }
            }
            return c;
        }
        #endregion

        #region TO LINES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<ILineSegment> ToLineSegments(this IEnumerable<VectorF> data, PointJoin join)
        {
            int count = data.Count();
            var lines = new PrimitiveList<ILineSegment>(count / 2 + 1);
            if (count == 2)
            {
                lines.Add(new LineSegment(data.ElementAt(0), data.ElementAt(1)));
                return lines;
            }

            bool unique = (join & PointJoin.NoRepeat) == PointJoin.NoRepeat;
            bool connectEach = (join & PointJoin.ConnectEach) == PointJoin.ConnectEach;
            bool joinEnds = (join & PointJoin.ConnectEnds) == PointJoin.ConnectEnds;

            VectorF p0, p1, first;
            p0 = p1 = first = VectorF.Empty;

            foreach (var item in data)
            {
                if ((item.Kind & PointKind.Segment) == PointKind.Segment)
                {
                    first = p0 = p1 = VectorF.Empty;
                    continue;
                }
                if ((item.Kind & PointKind.Break) == PointKind.Break)
                {
                    if (
                        connectEach && joinEnds && !Equals(p1, first))
                    {
                        var line = new LineSegment(p1, first);
                        lines.Add(line);
                    }

                    if ((join & PointJoin.RemoveLast) == PointJoin.RemoveLast)
                        lines.RemoveAt(lines.Count - 1);

                    first = p0 = p1 = VectorF.Empty;
                    continue;
                }
                if (!item)
                    continue;

                p1 = item;

                if (!first)
                    first = p1;

                if (!p0)
                {
                    p0 = p1;
                    continue;
                }
                if (unique && p1.Equals(p0))
                    continue;

                lines.Add(new LineSegment(p0, p1));

                if (connectEach)
                    p0 = p1;
                else
                    p0 = VectorF.Empty;
            }

            if (connectEach && joinEnds && !Equals(p1, first))
            {
                var line = new LineSegment(p1, first);
                lines.Add(line);
            }

            if ((join & PointJoin.RemoveLast) == PointJoin.RemoveLast)
                lines.RemoveAt(lines.Count - 1);

            return lines;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<ILineSegment> ToLineSegments(this IEnumerable<Vector> data, PointJoin join)
        {
            int count = data.Count();

            var lines = new PrimitiveList<ILineSegment>(count / 2 + 1);
            if (count == 2)
            {
                lines.Add(new LineSegment(data.ElementAt(0), data.ElementAt(1)));
                return lines;
            }

            bool unique = (join & PointJoin.NoRepeat) == PointJoin.NoRepeat;
            bool connectEach = (join & PointJoin.ConnectEach) == PointJoin.ConnectEach;
            bool joinEnds = (join & PointJoin.ConnectEnds) == PointJoin.ConnectEnds;

            Vector p0, p1, first;
            p0 = p1 = first = Vector.Empty;

            foreach (var item in data)
            {
                if ((item.Kind & PointKind.Segment) == PointKind.Segment)
                {
                    first = p0 = p1 = Vector.Empty;
                    continue;
                }
                if ((item.Kind & PointKind.Break) == PointKind.Break)
                {
                    if (
                        connectEach && joinEnds && !Equals(p1, first))
                    {
                        var line = new LineSegment(p1.X, p1.Y, first.X, first.Y);
                        lines.Add(line);
                    }

                    if ((join & PointJoin.RemoveLast) == PointJoin.RemoveLast)
                        lines.RemoveAt(lines.Count - 1);

                    first = p0 = p1 = Vector.Empty;
                    continue;
                }

                if (!item)
                    continue;
                p1 = item;

                if (!first)
                    first = p1;

                if (!p0)
                {
                    p0 = p1;
                    continue;
                }
                if (unique && p1.Equals(p0))
                    continue;

                lines.Add(new LineSegment(p0.X, p0.Y, p1.X, p1.Y));

                if (connectEach)
                    p0 = p1;
                else
                    p0 = Vector.Empty;
            }

            if (connectEach && joinEnds && !Equals(p1, first))
            {
                var line = new LineSegment(p1.X, p1.Y, first.X, first.Y);
                lines.Add(line);
            }

            if ((join & PointJoin.RemoveLast) == PointJoin.RemoveLast)
                lines.RemoveAt(lines.Count - 1);

            return lines;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<ILineSegment> ToLineSegments(this IEnumerable<Point> data, PointJoin join)
        {
            int count = data.Count();

            var lines = new PrimitiveList<ILineSegment>(count / 2 + 1);
            if (count == 2)
            {
                lines.Add(new LineSegment(data.ElementAt(0), data.ElementAt(1)));
                return lines;
            }

            bool unique = (join & PointJoin.NoRepeat) == PointJoin.NoRepeat;
            bool connectEach = (join & PointJoin.ConnectEach) == PointJoin.ConnectEach;
            bool joinEnds = (join & PointJoin.ConnectEnds) == PointJoin.ConnectEnds;

            Point p0, p1, first;
            p0 = p1 = first = Point.Empty;

            foreach (var item in data)
            {
                if (!item)
                    continue;
                p1 = item;

                if (!first)
                    first = p1;

                if (!p0)
                {
                    p0 = p1;
                    continue;
                }
                if (unique && p1.Equals(p0))
                    continue;

                lines.Add(new LineSegment(p0.X, p0.Y, p1.X, p1.Y));

                if (connectEach)
                    p0 = p1;
                else
                    p0 = Point.Empty;
            }

            if (connectEach && joinEnds && !Equals(p1, first))
            {
                var line = new LineSegment(p1.X, p1.Y, first.X, first.Y);
                lines.Add(line);
            }

            if ((join & PointJoin.RemoveLast) == PointJoin.RemoveLast)
                lines.RemoveAt(lines.Count - 1);

            return lines;
        }
        #endregion

        #region GET SCAN LINES
        public static IEnumerable<IAxisLine> GetScanLines(this IEnumerable<Point> points)
        {
            var lines = points.ToLineSegments(PointJoin.CircularJoin);
            using (var scanner = Factory.newLineScanner())
            {
                scanner.Scan(lines);
                foreach (var item in scanner.GetScanLines())
                    yield return item;
            }
        }
        public static IEnumerable<IAxisLine> GetScanLines(this IEnumerable<VectorF> points)
        {
            var lines = points.ToLineSegments(PointJoin.CircularJoin);
            using (var scanner = Factory.newLineScanner())
            {
                scanner.Scan(lines);
                foreach (var item in scanner.GetScanLines())
                    yield return item;
            }
        }
        public static IEnumerable<IAxisLine> GetScanLines(this IEnumerable<Vector> points)
        {
            var lines = points.ToLineSegments(PointJoin.CircularJoin);
            using (var scanner = Factory.newLineScanner())
            {
                scanner.Scan(lines);
                foreach (var item in scanner.GetScanLines())
                    yield return item;
            }
        }
        #endregion

        #region CORRECT BOUNDARY
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectBoundary<T>(this IBounds bounds, IEnumerable<T> pts,
            ref int XOfMinY, ref int YOfMinX, ref int XOfMaxY, ref int YOfMaxX) where T : IPoint
        {
            if (pts != null)
            {
                bounds.GetBounds(out int MinX, out int MinY, out int MaxX, out int MaxY);
                MaxX += MinX;
                MaxY += MinY;

                foreach (var p in pts)
                {
                    var py = p.Y;
                    var px = p.X;
                    var pxc = py + 1;
                    var pyc = px + 1;
                    if (py == MinY || pyc == MinY)
                        XOfMinY = px;
                    if (px == MinX || pxc == MinX)
                        YOfMinX = py;
                    if (py == MaxY || pyc == MaxY)
                        XOfMaxY = px;
                    if (px == MaxX || pxc == MaxX)
                        YOfMaxX = py;
                }
            }
        }
        #endregion

        #region POINT IN TRIANGLE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InTriangle(this IPointF p, IPointF p1, IPointF p2, IPointF p3) =>
            InTriangle(p.X, p.Y, p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InTriangle(float x, float y, IPointF p1, IPointF p2, IPointF p3) =>
            InTriangle(x, y, p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InTriangle(float x, float y, float p1X, float p1Y, float p2X, float p2Y, float p3X, float p3Y)
        {
            var Area = AreaOfTriangle(p1X, p1Y, p2X, p2Y, p3X, p3Y);
            /* Calculate area of triangle PBC */
            var A1 = AreaOfTriangle(x, y, p2X, p2Y, p3X, p3Y);

            /* Calculate area of triangle PAC */
            var A2 = AreaOfTriangle(p1X, p1Y, x, y, p3X, p3Y);

            /* Calculate area of triangle PAB */
            var A3 = AreaOfTriangle(p1X, p1Y, p2X, p2Y, x, y);

            /* Check if sum of A1, A2 and A3 is same as A */
            return Area == A1 + A2 + A3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InTriangle(float x, float y, int p1X, int p1Y, int p2X, int p2Y, int p3X, int p3Y)
        {
            var Area = AreaOfTriangle(p1X, p1Y, p2X, p2Y, p3X, p3Y);
            /* Calculate area of triangle PBC */
            var A1 = AreaOfTriangle(x, y, p2X, p2Y, p3X, p3Y);

            /* Calculate area of triangle PAC */
            var A2 = AreaOfTriangle(p1X, p1Y, x, y, p3X, p3Y);

            /* Calculate area of triangle PAB */
            var A3 = AreaOfTriangle(p1X, p1Y, p2X, p2Y, x, y);

            /* Check if sum of A1, A2 and A3 is same as A */
            return Area == A1 + A2 + A3;
        }
        #endregion

        #region TO AXIS-POINTS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAxisPoint[] ToAxisPoints(float X, float Y, bool aliased)
        {
            int x0 = (int)X;
            int y0 = (int)Y;

            float alpha1 = X - x0;
            float alpha2 = Y - y0;

            if (aliased || alpha1 == 0 || alpha2 == 0)
            {
                bool horizontal = alpha1 != 0;

                if (horizontal)
                    return new IAxisPoint[] { new AxisPoint(X, y0, LineFill.Horizontal) };
                else
                    return new IAxisPoint[] { new AxisPoint(Y, x0, LineFill.Vertical) };
            }
            else
            {
                var invAlpha1 = 1 - alpha1;
                var invAlpha2 = 1 - alpha2;
                var pts = new IAxisPoint[4];
                pts[0] = new AxisPoint(x0 + invAlpha1 * invAlpha2, y0, LineFill.Horizontal);
                pts[1] = new AxisPoint(x0 + 1 + alpha1 * invAlpha2, y0, LineFill.Horizontal);
                pts[2] = new AxisPoint(y0 + 1 + invAlpha1 * alpha2, x0, LineFill.Vertical);
                pts[3] = new AxisPoint(y0 + 1 + alpha1 * alpha2, x0 + 1, LineFill.Vertical);
                return pts;
            }
        }

        public static IAxisPoint[] ToAxisPoints(this IPointF point, bool aliased) =>
            ToAxisPoints(point.X, point.Y, aliased);

        public static IAxisPoint[] ToAxisPoints(this IPoint point) =>
            ToAxisPoints(point.X, point.Y, true);
        #endregion

        #region CENTER
        /// <summary>
        /// Gets the center of the rectangle.
        /// </summary>
        /// <param name="r">Rectangle to get center from</param>
        /// <returns></returns>
        public static VectorF Center(this IRectangleF r)
        {
            if (r == null || !r.Valid)
                return VectorF.Empty;
            return new VectorF(r.X + r.Width / 2f, r.Y + r.Height / 2f);
        }
        /// <summary>
        /// Gets the center of the rectangle.
        /// </summary>
        /// <param name="r">Rectangle to get center from</param>
        /// <returns></returns>
        public static void Center(this IRectangleF r, out float cx, out float cy)
        {
            cx = cy = 0;
            if (r == null || !r.Valid)
                return;

            cx = r.X + r.Width / 2f;
            cy = r.Y + r.Height / 2f;
        }
        /// <summary>
        /// Gets the center of the rectangle.
        /// </summary>
        /// <param name="r">Rectangle to get center from</param>
        /// <returns></returns>
        public static void Center(this IBounds r, out float cx, out float cy)
        {
            cx = cy = 0;
            if (r == null || !r.Valid)
                return;
            int x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            cx = x + w / 2f;
            cy = y + h / 2f;
        }
        /// <summary>
        /// Gets the ccenter of the rectangle.
        /// </summary>
        /// <param name="r">RectangleF to get center from</param>
        /// <returns></returns>
        public static VectorF Center(this IBounds r)
        {
            if (r == null || !r.Valid)
                return VectorF.Empty;
            int x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            return new VectorF(x + w / 2f, y + h / 2f);
        }
        #endregion

    }
}