/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{   
    public static partial class Vectors
    {
        #region VARIABLES
        static readonly Collection<int> Slopes;
        public const int BigExp = 20;
        public const int Big = (1 << BigExp);
        public const int LOffset = 1;
        //public const int SOffset = LOffset * 2;
        #endregion

        #region CONSTRUCTOR
        static Vectors()
        {
            Slopes = new Collection<int>(3001);
            Slopes.Add(0);
            for (int i = 1; i < 3001; i++)
                Slopes.Add(Big / i);
        }
        internal static void Initialize() { }
        #endregion

        #region SCALE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(float X, float Y, float scaleX, float scaleY, float centerX, float centerY, out float x, out float y)
        {
            x = X;
            y = Y;

            if (scaleX == 1 && scaleY == 1 || scaleX == 0 && scaleX == 0)
                return;

            if (scaleX != 1)
                x -= centerX;

            if (scaleY != 1)
                y -= centerY;

            x *= scaleX;
            y *= scaleY;

            if (scaleX != 1)
                x += centerX;

            if (scaleY != 1)
                y += centerY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(int X, int Y, int scaleX, int scaleY, int centerX, int centerY, out int x, out int y)
        {
            x = X;
            y = Y;

            if (scaleX == 1 && scaleY == 1 || scaleX == 0 && scaleX == 0)
                return;


            if (scaleX != 1)
                x -= centerX;
            if (scaleY != 1)
                y -= centerY;

            x *= scaleX;
            y *= scaleY;

            if (scaleX != 1)
                x += centerX;

            if (scaleY != 1)
                y += centerY;
        }
        #endregion

        #region DISTANCE SQUARED
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(float p1X, float p1Y, float p2X, float p2Y)
        {
            float dx = p1X - p2X;
            float dy = p1Y - p2Y;

            return Numbers.Sqr(dx + dy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(float p1X, float p1Y, float p2X, float p2Y)
        {
            float dx = p1X - p2X;
            float dy = p1Y - p2Y;

            return (float)Math.Sqrt(Numbers.Sqr(dx + dy));
        }
        #endregion

        #region SLOPE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(float x1, float y1, float x2, float y2)
        {
            var DX = x2 - x1;
            var DY = y2 - y1;
            var Slope = DY;
            if (DX != 0)
                Slope /= DX;
            return Slope;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(float x1, float y1, float x2, float y2, out float C)
        {
            var DX = x2 - x1;
            var DY = y2 - y1;
            var Slope = DY;
            if (DX != 0)
                Slope /= DX;
            C = y1 - Slope * x1;
            return Slope;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(float x1, float y1, float x2, float y2, out bool steep)
        {
            var DX = x2 - x1;
            var DY = y2 - y1;
            var Slope = DY;
            if (DX != 0)
                Slope /= DX;
            steep = Math.Abs(DY) > Math.Abs(DX);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Slope(int x1, int y1, int x2, int y2, out bool Steep)
        {
            var dx = (x2 - x1);
            var dy = (y2 - y1);
            var DX = Math.Abs(dx);
            var DY = Math.Abs(dy);
            Steep = DY > DX;
            int Slope;

            if (Steep)
            {
                if (DY > 3000)
                    Slope = dx / (DY / Big);
                else
                    Slope = dx * Slopes[DY];
            }
            else
            {
                if (DX > 3000)
                    Slope = dy / (DX / Big);
                else

                    Slope = dy * Slopes[DX];
            }
            return Slope;
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
        public static void FourthPointOfRhombus(float x1, float y1, float x2, float y2, float x3, float y3, out float x, out float y)
        {
            x = x1 + x3 - x2;
            y = y1 + y3 - y2;
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
        public static float AreaOfTriangle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            return Math.Abs((x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2)) / 2f);
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
    }
}

namespace MnM.GWS
{
#if (GWS || Window)
    using System.Collections.Generic;
    using System.Linq;
    public static partial class Vectors
    {
        //8K UHD 7680x4320;
        public const int UHD8kWidth = 7680;
        public const int UHD8kHeight = 4320;

        #region CAST
        //public static T Cast<T>(this VectorF p) where T : VectorF, new()
        //{
        //    var t = new T();
        //    t.Assign(p.X, p.Y, p.Quadratic);
        //    return t;
        //}
        //public static T Cast<T>(this Vector p) where T : Vector, new()
        //{
        //    var t = new T();
        //    t.Assign(p.X, p.Y, p.Quadratic);
        //    return t;
        //}

        //public static IList<U> Cast<T, U>(this IEnumerable<T> list)
        //    where T : VectorF
        //    where U : VectorF, new()
        //{
        //    if (list is IList<U>)
        //        return list as IList<U>;
        //    return list.Select(x => x.Cast<U>()).ToArray();
        //}
        //public static IList<U> Cast2<T, U>(this IEnumerable<T> list)
        //    where T : Vector
        //    where U : Vector, new()
        //{
        //    if (list is IList<U>)
        //        return list as IList<U>;
        //    return list.Select(x => x.Cast<U>()).ToArray();
        //}
        #endregion

        #region DOT LENGTH
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this VectorF v, VectorF p2) =>
            v.X * p2.X + v.Y * p2.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSquared(this VectorF v) =>
            (v.X * v.X + v.Y * v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(this VectorF v) =>
          (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Dot(this Vector v, Vector p2) =>
            v.X * p2.X + v.Y * p2.Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LengthSquared(this Vector v) =>
            (v.X * v.X + v.Y * v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Length(this Vector v) =>
          (int)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        #endregion

        #region DISTANCE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(this VectorF p1, VectorF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;

            float ls = dx * dx + dy * dy;
            return (ls);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this VectorF p1, VectorF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;

            float ls = dx * dx + dy * dy;

            return (float)Math.Sqrt(ls);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DistanceSquared(this Vector p1, Vector p2)
        {
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;

            int ls = dx * dx + dy * dy;
            return (ls);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Distance(this Vector p1, Vector p2)
        {
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;

            int ls = dx * dx + dy * dy;

            return (int)Math.Sqrt(ls);
        }
        #endregion

        #region SLOPE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this VectorF p1, VectorF p2, out float c) =>
            Slope(p1.X, p1.Y, p2.X, p2.Y, out c);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this VectorF p1, VectorF p2) =>
            Slope(p1.X, p1.Y, p2.X, p2.Y, out bool steep);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this VectorF p1, VectorF p2, out bool steep) =>
            Slope(p1.X, p1.Y, p2.X, p2.Y, out steep);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this Vector p1, Vector p2, out bool steep) =>
            Slope(p1.X, p1.Y, p2.X, p2.Y, out steep);
        #endregion

        #region CLOSENESS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool VeryCloseTo(this VectorF v, VectorF other, float threshold = 1.5f)
        {
            float dx = v.X - other.X;
            float dy = v.Y - other.Y;
            return (float)(Numbers.Sqr(dx) + Numbers.Sqr(dy)) < threshold;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool VeryCloseTo(this Vector v, Vector other, float threshold = 1.5f)
        {
            float dx = v.X - other.X;
            float dy = v.Y - other.Y;
            return (float)(Numbers.Sqr(dx) + Numbers.Sqr(dy)) < threshold;
        }
        #endregion

        #region MIN AND MAX
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(this IEnumerable<VectorF> collection, out float minX, out float minY, out float maxX, out float maxY, SizeF? clip = null)
        {
            minX = minY = float.MaxValue;
            maxX = maxY = float.MinValue;
            bool pointFound = false;
            bool sizeFound = false;
            bool ok = false;

            if (collection is IPointF)
            {
                minX = ((IPointF)collection).X;
                minY = ((IPointF)collection).Y;
                pointFound = true;
            }
            else if(collection is IPoint)
            {
                minX = ((IPoint)collection).X;
                minY = ((IPoint)collection).Y;
                pointFound = true;
            }

            if (pointFound && collection is ISizeF)
            {
                maxX = ((ISizeF)collection).Width;
                maxY = ((ISizeF)collection).Height;
                if (maxX != 0 && maxY != 0)
                {
                    maxX += minX;
                    maxY += minY;
                    sizeFound = true;
                }
            }
            else if (pointFound && collection is ISize)
            {
                minX = ((ISize)collection).Width;
                minY = ((ISize)collection).Height;
                if (maxX != 0 && maxY != 0)
                {
                    maxX += minX;
                    maxY += minY;
                    sizeFound = true;
                }
            }
            if (pointFound && sizeFound)
            {
                ok = true;
                goto Clipping;
            }

            foreach (var p in collection)
            {
                if (p == null || !p)
                    continue;
                ok = true;
                if (p.X < minX)
                    minX = p.X;
                if (p.Y < minY)
                    minY = p.Y;
                if (p.X > maxX)
                    maxX = p.X;
                if (p.Y > maxY)
                    maxY = p.Y;
            }

            Clipping:
            if (clip != null && clip.Value)
            {
                minX = Math.Max(minX, 0);
                minY = Math.Max(minY, 0);
                maxX = Math.Max(maxX, 0);
                maxY = Math.Max(maxY, 0);

                if (maxX > clip.Value.Width)
                    maxX = clip.Value.Width;
                if (maxY > clip.Value.Height)
                    maxY = clip.Value.Height;
            }
            if (!ok)
                minX = maxX = minY = maxY = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(this IEnumerable<Vector> collection, out int minX, out int minY, out int maxX, out int maxY, Size? clip = null)
        {
            minX = minY = int.MaxValue;
            maxX = maxY = int.MinValue;
            bool pointFound = false;
            bool sizeFound = false;
            bool ok = false;

            if (collection is IPointF)
            {
                minX = (int)((IPointF)collection).X;
                minY = (int)((IPointF)collection).Y;
                pointFound = true;
            }
            else if (collection is IPoint)
            {
                minX = ((IPoint)collection).X;
                minY = ((IPoint)collection).Y;
                pointFound = true;
            }

            if (pointFound && collection is ISizeF)
            {
                maxX = (int)((ISizeF)collection).Width;
                maxY = (int)((ISizeF)collection).Height;
                if (maxX != 0 && maxY != 0)
                {
                    maxX += minX;
                    maxY += minY;
                    sizeFound = true;
                }
            }
            else if (pointFound && collection is ISize)
            {
                minX =  ((ISize)collection).Width;
                minY = ((ISize)collection).Height;
                if (maxX != 0 && maxY != 0)
                {
                    maxX += minX;
                    maxY += minY;
                    sizeFound = true;
                }
            }
            if (pointFound && sizeFound)
            {
                ok = true;
                goto Clipping;
            }

            foreach (var p in collection)
            {
                if (!p)
                    continue;
                ok = true;
                if (p.X < minX)
                    minX = p.X;
                if (p.Y < minY)
                    minY = p.Y;
                if (p.X > maxX)
                    maxX = p.X;
                if (p.Y > maxY)
                    maxY = p.Y;
            }

            Clipping:
            if (clip != null && (clip.Value.Width > 0 && clip.Value.Height > 0))
            {
                minX = Math.Max(minX, 0);
                minY = Math.Max(minY, 0);
                maxX = Math.Max(maxX, 0);
                maxY = Math.Max(maxY, 0);

                if (maxX > clip.Value.Width)
                    maxX = clip.Value.Width;
                if (maxY > clip.Value.Height)
                    maxY = clip.Value.Height;
            }
            if (!ok)
                minX = maxX = minY = maxY = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(out float minX, out float minY, out float maxX, out float maxY, params VectorF[] collection) =>
            MinMax(collection as IEnumerable<VectorF>, out minX, out minY, out maxX, out maxY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(SizeF clip, out float minX, out float minY, out float maxX, out float maxY, params VectorF[] collection) =>
            MinMax(collection as IEnumerable<VectorF>, out minX, out minY, out maxX, out maxY, clip);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(out int minX, out int minY, out int maxX, out int maxY, params Vector[] collection) =>
            MinMax(collection as IEnumerable<Vector>, out minX, out minY, out maxX, out maxY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(Size clip, out int minX, out int minY, out int maxX, out int maxY, params Vector[] collection) =>
            MinMax(collection as IEnumerable<Vector>, out minX, out minY, out maxX, out maxY, clip);
        #endregion

        #region MIN ONLY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(this IEnumerable<VectorF> collection, out float minX, out float minY)
        {
            minX = minY = float.MaxValue;
            bool ok = false;
            foreach (var p in collection)
            {
                if (p == null || !p)
                    continue;
                ok = true;
                if (p.X < minX)
                    minX = p.X;
                if (p.Y < minY)
                    minY = p.Y;
            }
            if (!ok)
            {
                minX = minY = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(this IEnumerable<Vector> collection, out int minX, out int minY)
        {
            minX = minY = int.MaxValue;
            bool ok = false;
            foreach (var p in collection)
            {
                if (!p)
                    continue;
                ok = true;
                if (minX == -1 || p.X < minX)
                    minX = p.X;
                if (minY == -1 || p.Y < minY)
                    minY = p.Y;
            }
            if (!ok)
            {
                minX = minY = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(out float minX, out float minY, params VectorF[] collection) =>
            Min(collection as IEnumerable<VectorF>, out minX, out minY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(out int minX, out int minY, params Vector[] collection) =>
            Min(collection as IEnumerable<Vector>, out minX, out minY);
        #endregion

        #region MAX ONLY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(this IEnumerable<VectorF> collection, out float maxX, out float maxY)
        {
            maxX = maxY = float.MinValue;
            bool ok = false;
            foreach (var p in collection)
            {
                if (p == null || !p)
                    continue;
                ok = true;
                if (p.X > maxX)
                    maxX = p.X;
                if (p.Y > maxY)
                    maxY = p.Y;
            }
            if (!ok)
            {
                maxX = maxY = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(this IEnumerable<Vector> collection, out int maxX, out int maxY)
        {
            maxX = maxY = -1;
            foreach (var p in collection)
            {
                if (p == null || !p)
                    continue;
                if (maxX == -1 || p.X > maxX)
                    maxX = p.X;
                if (maxY == -1 || p.Y > maxY)
                    maxY = p.Y;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(out float minX, out float minY, params VectorF[] collection) =>
            Max(collection as IEnumerable<VectorF>, out minX, out minY);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(out int minX, out int minY, params Vector[] collection)  =>
            Max(collection as IEnumerable<Vector>, out minX, out minY);
        #endregion

        #region CLEAN
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<VectorF> Clean(this IEnumerable<VectorF> data, PointJoin join) 
        {
            int count = data.Count();

            var list = new Collection<VectorF>(count);
            bool unique = join.HasFlag(PointJoin.NoRepeat);
            bool noTooClose = join.HasFlag(PointJoin.AvoidTooClose);

            VectorF p0, first;
            p0 = first = default(VectorF);

            foreach (var item in data)
            {
                if (!item)
                    continue;

                if (!first)
                    first = item;

                if (unique && item.Equals(p0))
                    continue;

                if (noTooClose)
                {
                    if (p0.VeryCloseTo(item))
                        continue;
                }
                list.Add(item);

                p0 = item;
            }
            return list;
        }
        #endregion

        #region PARALLEL
        public static void ParallelAt(float x1, float y1, VectorF p1, VectorF p2, float length, out float x2, out float y2) =>
            ParallelAt(x1, y1, Slope(p1, p2, out bool steep), length, steep, out x2, out y2);
        public static void ParallelAt(int x1, int y1, Vector p1, Vector p2, int length, out int x2, out int y2) =>
                ParallelAt(x1, y1, Slope(p1, p2, out bool steep), length, steep, out x2, out y2);

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

        #region > OR < THAN POINT
        public static bool IsGreaterThan(this VectorF p, VectorF p1, VectorF p2)
        {
            var c1 = (p2.X - p1.X) * (p.Y - p1.Y);
            var c2 = (p2.Y - p1.Y) * (p.X - p1.X);
            //if (Math.Abs(c1 - c2) < 0.01f)
            //    return true;
            return c1 < c2;
        }
        public static bool IsLessThan(this VectorF p, VectorF p1, VectorF p2)
        {
            var c1 = (p2.X - p1.X) * (p.Y - p1.Y);
            var c2 = (p2.Y - p1.Y) * (p.X - p1.X);
            //if (Math.Abs(c1 - c2) < 0.01f)
            //    return true;
            return c1 > c2;
        }
        public static bool IsGreaterThan(float x, float y, VectorF p1, VectorF p2)
        {
            var c1 = (p2.X - p1.X) * (y - p1.Y);
            var c2 = (p2.Y - p1.Y) * (x - p1.X);
            //if (Math.Abs(c1 - c2) < 0.01f)
            //    return true;
            return c1 < c2;
        }
        public static bool IsLessThan(float x, float y, VectorF p1, VectorF p2)
        {
            var c1 = (p2.X - p1.X) * (y - p1.Y);
            var c2 = (p2.Y - p1.Y) * (x - p1.X);
            //if (Math.Abs(c1 - c2) < 0.01f)
            //    return true;
            return c1 > c2;
        }
        #endregion

        #region IN CENTER OF TRIANGLE
        public static void InCenterOfTriangle(VectorF p1, VectorF p2, VectorF p3, out float cx, out float cy)
        {
            var a = p1.Distance(p2);
            var b = p2.Distance(p3);
            var c = p3.Distance(p1);
            cx = (a * p1.X + b * p2.X + c * p3.X) / (a + b + c);
            cy = (a * p1.Y + b * p2.Y + c * p3.Y) / (a + b + c);
        }
        #endregion

        #region POINT IN TRIANGLE
        public static bool InTriangle(this VectorF p, VectorF p1, VectorF p2, VectorF p3) =>
            InTriangle(p.X, p.Y, p1, p2, p3);
        public static bool InTriangle(float x, float y, VectorF p1, VectorF p2, VectorF p3)
        {
            var Area = AreaOfTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);
            /* Calculate area of triangle PBC */
            var A1 = AreaOfTriangle(x, y, p2.X, p2.Y, p3.X, p3.Y);

            /* Calculate area of triangle PAC */
            var A2 = AreaOfTriangle(p1.X, p1.Y, x, y, p3.X, p3.Y);

            /* Calculate area of triangle PAB */
            var A3 = AreaOfTriangle(p1.X, p1.Y, p2.X, p2.Y, x, y);

            /* Check if sum of A1, A2 and A3 is same as A */
            return Area == A1 + A2 + A3;
        }
        #endregion

        #region 3 PPOINTS MOTION - CLOCK WISE OR NOT
        public static bool ClockWise(this IList<VectorF> points) 
        {
            double sum = 0.0;
            for (int i = 0; i < points.Count; i++)
            {
                var v1 = points[i];
                var v2 = points[(i + 1) % points.Count];
                sum += (v2.X - v1.X) * (v2.Y + v1.Y);
            }
            return sum > 0.0;
        }
        public static bool Clockwise(params VectorF[] points) =>
            ClockWise(points as IList<VectorF>);
        #endregion

        #region VECTOR MATH
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF SquareRoot(this VectorF v) =>
            new VectorF((float)Math.Sqrt(v.X), (float)Math.Sqrt(v.Y), v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Normalize(this VectorF v)
        {
            float ls = v.X * v.X + v.Y * v.Y;
            float invNorm = 1.0f / (float)Math.Sqrt(ls);

            return new VectorF(v.X * invNorm, v.Y * invNorm, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Add(this VectorF v, VectorF p2) =>
            new VectorF(v.X + p2.X, v.Y + p2.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Subtract(this VectorF v, VectorF p2) =>
            new VectorF(v.X - p2.X, v.Y - p2.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Multiply(this VectorF v, VectorF p2) =>
            new VectorF(v.X * p2.X, v.Y * p2.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Divide(this VectorF v, VectorF p2) =>
            new VectorF(v.X / p2.X, v.Y / p2.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Multiply(this VectorF v, float b) =>
            new VectorF(v.X * b, v.Y * b, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Add(this VectorF v, float b) =>
            new VectorF(v.X + b, v.Y + b, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Subtract(this VectorF v, float b) =>
            new VectorF(v.X - b, v.Y - b, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Divide(this VectorF v, float b) =>
            new VectorF(v.X / b, v.Y / b, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Negate(this VectorF v) =>
            new VectorF(-v.X, -v.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Add(this Vector v, Vector p2) =>
            new Vector(v.X + p2.X, v.Y + p2.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Subtract(this Vector v, Vector p2) =>
            new Vector(v.X - p2.X, v.Y - p2.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Multiply(this Vector v, Vector p2) =>
            new Vector(v.X * p2.X, v.Y * p2.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Divide(this Vector v, Vector p2) =>
            new Vector(v.X / p2.X, v.Y / p2.Y, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Multiply(this Vector v, int b) =>
            new Vector(v.X * b, v.Y * b, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Add(this Vector v, int b) =>
            new Vector(v.X + b, v.Y + b, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Subtract(this Vector v, int b) =>
            new Vector(v.X - b, v.Y - b, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Divide(this Vector v, int b) =>
            new Vector(v.X / b, v.Y / b, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Normalize(this Vector v)
        {
            int ls = v.X * v.X + v.Y * v.Y;
            float invNorm = 1.0f / (float)Math.Sqrt(ls);

            return new Vector((v.X * invNorm).Round(), (v.Y * invNorm).Round(), v.Quadratic);
        }
        #endregion

        #region OFFSET
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Offset(this VectorF v, VectorF o)
        {
            return new VectorF(v.X + o.X, v.Y + o.Y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Offset(this VectorF v, float x, float y)
        {
            return new VectorF(v.X + x, v.Y + y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Offset(this Vector v, Vector o)
        {
            return new Vector(v.X + o.X, v.Y + o.Y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Offset(this Vector v, int x, int y)
        {
            return new Vector(v.X + x, v.Y + y, v.Quadratic);
        }
        #endregion

        #region  MIN - MAX - AVG
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Min(this VectorF v, VectorF p2)
        {
            return new VectorF(
                (v.X < p2.X) ? v.X : p2.X,
                (v.Y < p2.Y) ? v.Y : p2.Y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Max(this VectorF v, VectorF p2)
        {
            return new VectorF(
                (v.X > p2.X) ? v.X : p2.X,
                (v.Y > p2.Y) ? v.Y : p2.Y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Avg(this VectorF v, VectorF p2) =>
            new VectorF((v.X + p2.X) / 2, (v.Y + p2.Y) / 2, v.Quadratic);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Min(this Vector v, Vector p2)
        {
            return new Vector(
                (v.X < p2.X) ? v.X : p2.X,
                (v.Y < p2.Y) ? v.Y : p2.Y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Max(this Vector v, Vector p2)
        {
            return new Vector(
                (v.X > p2.X) ? v.X : p2.X,
                (v.Y > p2.Y) ? v.Y : p2.Y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Avg(this Vector v, Vector p2) =>
            new Vector((v.X + p2.X) / 2, (v.Y + p2.Y) / 2, v.Quadratic);
        #endregion

        #region REFLECT & TRANSFORM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Reflect(this VectorF v, VectorF normal)
        {
            float dot = v.X * normal.X + v.Y * normal.Y;

            return new VectorF(
                v.X - 2.0f * dot * normal.X,
                v.Y - 2.0f * dot * normal.Y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Reflect(this Vector v, Vector normal)
        {
            int dot = v.X * normal.X + v.Y * normal.Y;

            return new Vector(
                v.X - 2 * dot * normal.X,
                v.Y - 2 * dot * normal.Y, v.Quadratic);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector TransformNormal(this Vector v, IMatrix3x2 matrix)
        {
            return new Vector(
                (v.X * matrix.M00 + v.Y * matrix.M10).Round(),
                (v.X * matrix.M01 + v.Y * matrix.M11).Round(), v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Transform(this VectorF v, IMatrix3x2 matrix)
        {
            return new VectorF(
                v.X * matrix.M00 + v.Y * matrix.M10 + matrix.M20,
                v.X * matrix.M01 + v.Y * matrix.M11 + matrix.M21, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF TransformNormal(this VectorF v, IMatrix3x2 matrix)
        {
            return new VectorF(
                v.X * matrix.M00 + v.Y * matrix.M10,
                v.X * matrix.M01 + v.Y * matrix.M11, v.Quadratic);
        }
        #endregion

        #region CLAMP & LERP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            return new VectorF(x, y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Lerp(this VectorF v, VectorF p2, float amount)
        {
            return new VectorF(
                v.X + (p2.X - v.X) * amount,
                v.Y + (p2.Y - v.Y) * amount, v.Quadratic);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            return new Vector(x, y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Lerp(this Vector v, Vector p2, float amount)
        {
            return new VectorF(
                v.X + (p2.X - v.X) * amount,
                v.Y + (p2.Y - v.Y) * amount, v.Quadratic).Round();
        }
        #endregion

        #region TO AREA
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle ToArea(this IEnumerable<Vector> points)
        {
            points.MinMax(out int minx, out int miny, out int maxx, out int maxy);
            return Rects.FromLTRB(minx, miny, maxx, maxy, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF ToArea(this IEnumerable<VectorF> points)
        {
            points.MinMax(out float minx, out float miny, out float maxx, out float maxy);
            return Rects.FromLTRB(minx, miny, maxx, maxy, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF ToArea(params VectorF[] points) =>
            ToArea(points as IEnumerable<VectorF>);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle ToArea(params Vector[] points) =>
            ToArea(points as IEnumerable<Vector>);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF ToArea(params float[] points)
        {
            float minX, minY, maxX, maxY;
            minX = minY = maxX = maxY = -1;
            for (int i = 0; i < points.Length - 1; i += 2)
            {
                if (minX == -1 || points[i] < minX)
                    minX = points[i];
                if (maxX == -1 || points[i] > maxX)
                    maxX = points[i];
                if (minY == -1 || points[i + 1] < minY)
                    minY = points[i + 1];
                if (maxY == -1 || points[i + 1] > maxY)
                    maxY = points[i + 1];
            }
            return Rects.FromLTRB(minX, minY, maxX, maxY, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle ToArea(params int[] points)
        {
            int minX, minY, maxX, maxY;
            minX = minY = maxX = maxY = -1;
            for (int i = 0; i < points.Length - 1; i += 2)
            {
                if (minX == -1 || points[i] < minX)
                    minX = points[i];
                if (maxX == -1 || points[i] > maxX)
                    maxX = points[i];
                if (minY == -1 || points[i + 1] < minY)
                    minY = points[i + 1];
                if (maxY == -1 || points[i + 1] > maxY)
                    maxY = points[i + 1];
            }
            return Rects.FromLTRB(minX, minY, maxX, maxY, true);
        }
        #endregion

        #region CENTER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Center(this IEnumerable<Vector> points)
        {
            points.MinMax(out int minx, out int miny, out int maxx, out int maxy);
            return new Vector(minx + (maxx - minx) / 2, miny + (maxy - miny) / 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Center(this IEnumerable<VectorF> points)
        {
            points.MinMax(out float minx, out float miny, out float maxx, out float maxy);
            return new VectorF(minx + (maxx - minx) / 2, miny + (maxy - miny) / 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Center(params VectorF[] points) =>
            Center(points as IEnumerable<VectorF>);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Center(params Vector[] points) =>
            Center(points as IEnumerable<Vector>);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Center(params float[] points)
        {
            float minX, minY, maxX, maxY;
            minX = minY = maxX = maxY = -1;
            for (int i = 0; i < points.Length - 1; i += 2)
            {
                if (minX == -1 || points[i] < minX)
                    minX = points[i];
                if (maxX == -1 || points[i] > maxX)
                    maxX = points[i];
                if (minY == -1 || points[i + 1] < minY)
                    minY = points[i + 1];
                if (maxY == -1 || points[i + 1] > maxY)
                    maxY = points[i + 1];
            }
            return new VectorF(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector ToCenter(params int[] points)
        {
            int minX, minY, maxX, maxY;
            minX = minY = maxX = maxY = -1;
            for (int i = 0; i < points.Length - 1; i += 2)
            {
                if (minX == -1 || points[i] < minX)
                    minX = points[i];
                if (maxX == -1 || points[i] > maxX)
                    maxX = points[i];
                if (minY == -1 || points[i + 1] < minY)
                    minY = points[i + 1];
                if (maxY == -1 || points[i + 1] > maxY)
                    maxY = points[i + 1];
            }
            return new Vector(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);
        }
        #endregion

        #region AVG OF COLLECTION
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Avg(this IEnumerable<VectorF> collection) 
        {
            float x = 0, y = 0;
            int i = -1;

            foreach (var p in collection)
            {
                if (!p)
                    continue;
                ++i;
                x += p.X;
                y += p.Y;
            }
            return new VectorF(x / i, y / i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Avg(this IEnumerable<Vector> collection)
        {
            int x = 0, y = 0;
            int i = -1;

            foreach (var p in collection)
            {
                if (!p)
                    continue;
                ++i;
                x += p.X;
                y += p.Y;
            }
            return new Vector(x / i, y / i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Avg(params VectorF[] collection) =>
            Avg(collection as IEnumerable<VectorF>);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Avg(params Vector[] collection) =>
            Avg(collection as IEnumerable<Vector>);
        #endregion

        #region TO POINTS
        public static Collection<VectorF> ToPointsF(this IEnumerable<int> xyPairs)
        {
            if (xyPairs == null)
                return new Collection<VectorF>();
            Collection<VectorF> points;
            var len = xyPairs.Count();
            var previous = -1;
            points = new Collection<VectorF>(len);
            foreach (var item in xyPairs)
            {
                if (previous == -1)
                    previous = item;
                else
                {
                    points.Add(new VectorF(previous, item));
                    previous = -1;
                }
            }
            return points;
        }
        public static Collection<VectorF> ToPoints(this IEnumerable<float> xyPairs)
        {
            if (xyPairs == null)
                return new Collection<VectorF>();

            Collection<VectorF> points;
            var len = xyPairs.Count();
            var previous = -1f;

            points = new Collection<VectorF>(len);
            foreach (var item in xyPairs)
            {
                if (previous == -1)
                    previous = item;
                else
                {
                    points.Add(new VectorF(previous, item));
                    previous = -1;
                }
            }

            return points;
        }
        public static Collection<VectorF> ToPoints(params float[] xyPairs) =>
            ToPoints(xyPairs as IEnumerable<float>);

        public static IEnumerable<VectorF> ToPointF(this IEnumerable<Vector> points) =>
            points.Select(x => (VectorF)(x));
        #endregion

        #region SCALE     
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Scale(this VectorF v, float scaleX, float scaleY, float centerX, float centerY)
        {
            Scale(v.X, v.Y, scaleX, scaleY, centerX, centerY, out float x, out float y);
            return new VectorF(x, y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Scale(this VectorF v, float scaleX, float scaleY)
        {
            return new VectorF(v.X * scaleX, v.Y * scaleY, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Scale(this Vector v, float scaleX, float scaleY)
        {
            return new Vector((v.X * scaleX).Round(), (v.Y * scaleY).Round(), v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Scale(this Vector v, int scaleX, int scaleY)
        {
            return new Vector((v.X * scaleX), (v.Y * scaleY), v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Scale(this Vector v, int scaleX, int scaleY, int centerX, int centerY)
        {
            Scale(v.X, v.Y, scaleX, scaleY, centerX, centerY, out int x, out int y);
            return new Vector(x, y, v.Quadratic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Scale(this Vector v, float scaleX, float scaleY, float centerX, float centerY)
        {
            Scale(v.X, v.Y, scaleX, scaleY, centerX, centerY, out float x, out float y);
            return new Vector(x.Round(), y.Round(), v.Quadratic);
        }

        public static IEnumerable<VectorF> Scale(this IEnumerable<VectorF> source, float? scaleX = null, float? scaleY = null,
            VectorF? center = null)
        {
            if (scaleX == null && scaleY == null)
            {
                if (source is IEnumerable<VectorF>)
                    return source;
                return source.ToArray();
            }
            if (center == null)
            {
                var x = scaleX ?? 1;
                var y = scaleY ?? 1;
                return source.Select(p => p.Scale(x, y));
            }
            else
            {
                var x = scaleX ?? 1;
                var y = scaleY ?? 1;
                var cx = center.Value.X;
                var cy = center.Value.Y;
                return source.Select(p => p.Scale(x, y, cx, cy));
            }
        }

        public static IEnumerable<VectorF> Scale<T>(this IEnumerable<VectorF> source, IScale scale, VectorF? center = null)
        {
            var sx = scale.X;
            var sy = scale.Y;

            if ((sx == 0 && sy == 0) || (sx == 1 && sy == 1))
            {
                if (source is IEnumerable<VectorF>)
                    return source;
                return source.Select(x => new VectorF(x));
            }
            if (center == null)
            {
                return source.Select(p => p.Scale(sx, sy));
            }
            else
            {
                var cx = center.Value.X;
                var cy = center.Value.Y;
                IList<VectorF> items;

                if (source is IList<VectorF>)
                    items = source as IList<VectorF>;
                else
                    items = source.Select(x => new VectorF(x)).ToArray();

                for (int i = 0; i < items.Count; i++)
                {
                    var x = items[i].X;
                    var y = items[i].Y;

                    if (sx != 1)
                        x = (x - cx) * sx + cx;

                    if (sy != 1)
                        y = (y - cy) * sy + cy;

                    items[i] = new VectorF(x, y, items[i].Quadratic);
                }
                return items;
            }
        }

        public static IEnumerable<Vector> Scale(this IEnumerable<Vector> source, int? scaleX = null, int? scaleY = null,
            Vector? center = null)
        {
            if (scaleX == null && scaleY == null)
                return source;

            if (center == null)
            {
                var x = scaleX ?? 1;
                var y = scaleY ?? 1;
                return source.Select(p => p.Scale(x, y));
            }
            else
            {
                var x = scaleX ?? 1;
                var y = scaleY ?? 1;
                var cx = center.Value.X;
                var cy = center.Value.Y;
                return source.Select(p => p.Scale(x, y, cx, cy));
            }
        }

        public static IEnumerable<Vector> Scale(this IEnumerable<Vector> source, IScale scale, Vector? center = null)
        {
            var sx = scale.X;
            var sy = scale.Y;

            if ((sx == 0 && sy == 0) || (sx == 1 && sy == 1))
                return source;

            if (center == null)
            {
                return source.Select(p => p.Scale(sx, sy));
            }
            else
            {
                var cx = center.Value.X;
                var cy = center.Value.Y;
                var items = source.ToArray();

                for (int i = 0; i < items.Length; i++)
                {
                    float x = items[i].X;
                    float y = items[i].Y;

                    if (sx != 1)
                        x = (x - cx) * sx + cx;

                    if (sy != 1)
                        y = (y - cy) * sy + cy;

                    items[i] = new Vector(x.Round(), y.Round(), items[i].Quadratic);
                }
                return items;
            }
        }
        #endregion

        #region ROTATE AND SCALE
        public static IList<VectorF> RotateAndScale(this IEnumerable<VectorF> points, Rotation angle, IScale scale,
            out RectangleF bounds, out bool flatSkew, VectorF? Center = null)
        {
            flatSkew = false;
            bool hasAngle = angle;

            if (hasAngle)
            {
                var skew = angle.Skew;
                flatSkew = skew == SkewType.Horizontal || skew == SkewType.Vertical;
            }
            bounds = points.ToArea();
            var c = (bounds).Center();
            float Cx, Cy;
            float Sx = (scale?.X ?? 0) + 1;
            float Sy = (scale?.Y ?? 0) + 1;
            IList<VectorF> Original;

            if (Sx != 1 || Sy != 1)
            {
                Original = points.Scale(Sx, Sy, c).ToArray();
                bounds = bounds.Scale(new VectorF(Sx, Sy));
            }

            bool isRotated;

            if (Center == null && hasAngle)
                isRotated = angle.EffectiveCenter(bounds, out Cx, out Cy);
            else
                isRotated = angle.EffectiveCenter(Center.Value, out Cx, out Cy);

            if (isRotated)
            {
                Original = points.Rotate(angle, Cx, Cy);
                if (angle.HasScale)
                    bounds = bounds.Scale(angle, new VectorF(Cx, Cy));
            }
            else
            {
                if (points is IList<VectorF>)
                    Original = points as IList<VectorF>;
                else
                    Original = points.ToArray();
            }

            return Original;
        }
        #endregion

        #region GET CENTER
        public static VectorF FindCenterFrom(this VectorF p, VectorF q)
        {
            var cx = Math.Min(p.X, q.X) + Math.Abs(q.X - p.X) / 2;
            var cy = Math.Min(p.Y, q.Y) + Math.Abs(q.Y - p.Y) / 2;
            return new VectorF(cx, cy);
        }
        public static VectorF FindCenterFrom(this Vector p, Vector q)
        {
            var cx = Math.Min(p.X, q.X) + Math.Abs(q.X - p.X) / 2f;
            var cy = Math.Min(p.Y, q.Y) + Math.Abs(q.Y - p.Y) / 2f;
            return new VectorF(cx, cy);
        }
        #endregion

        #region STROKING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<VectorF> StrokePoints(this IEnumerable<VectorF> source, float stroke, PointJoin join, bool isCircular)
        {
            var pts = source.Clean(join);
            return StrokePoints2(pts, stroke, join, isCircular);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<VectorF> StrokePoints2(this IList<VectorF> pts, float stroke, PointJoin join, bool isCircular)
        {
            if (pts.Count == 0)
                return new VectorF[0];

            if (pts.Count == 1)
                return new VectorF[] { new VectorF(pts[0]) };

            if (pts.Count == 2)
            {
                Parallel(pts[0], pts[1], stroke, out VectorF a, out VectorF b);
                return new VectorF[] { a, b };
            }
            VectorF[] points = new VectorF[pts.Count];

            var triangle = TriangleStroke(pts[0], pts[1], pts[2], stroke);

            points[0] = triangle[0];

            int j = 0;
            for (int i = 2; i < pts.Count; i++)
            {
                triangle = TriangleStroke(pts[i - 2], pts[i - 1], pts[i], stroke);
                points[++j] = triangle[1];
            }
            points[points.Length - 1] = triangle[2];

            if (isCircular)
            {
                points[points.Length - 1] = TriangleStroke(pts[pts.Count - 2], pts[pts.Count - 1], pts[0], stroke)[1];
                points[0] = TriangleStroke(pts[pts.Count - 1], pts[0], pts[1], stroke)[1];
            }
            return points;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        #region FIND RHOMBUS POINT
        /// <summary>
        /// Finds missing 4th point of rhombus - derived from 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="first">1st point of rhombus</param>
        /// <param name="second">2nd point of rhombus.</param>
        /// <param name="third">3rd point of rhombus.</param>
        /// <returns>Missing 4th point of rhombus.</returns>
        public static VectorF FourthPointOfRhombus(this VectorF first, VectorF second, VectorF third)
        {
            FourthPointOfRhombus(first.X, first.Y, second.X, second.Y, third.X, third.Y, out float x, out float y);
            return new VectorF(x, y);
        }
        #endregion

        #region MOVE - EXTEND
        public static VectorF Move(VectorF p, VectorF q, float move)
        {
            return Move(p.X, p.Y, q.X, q.Y, move);
        }
        public static VectorF Move(float x1, float y1, float x2, float y2, float move)
        {
            var DX = x2 - x1;
            var DY = y2 - y1;
            var dist = (float)Math.Sqrt(DX * DX + DY * DY);
            var x = x1 + DX / dist * move;
            var y = y1 + DY / dist * move;
            return new VectorF(x, y);
        }
        public static void Extend(float x1, float y1, float x2, float y2, out VectorF A, out VectorF B, float deviation, bool bothDirection = false)
        {
            var DX = x2 - x1;
            var DY = y2 - y1;

            var dist = (float)Math.Sqrt(DX * DX + DY * DY);

            var xDist = (DX / dist * deviation);
            var yDist = (DY / dist * deviation);

            x2 -= xDist;
            y2 -= yDist;

            if (bothDirection)
            {
                x1 = (x1 + xDist);
                y1 = (y1 + yDist);
            }

            A = new VectorF(x1, y1);
            B = new VectorF(x2, y2);
        }
        public static void Extend(VectorF p, VectorF q, out VectorF A, out VectorF B, float deviation, bool bothDirection = false)
        {
            Extend(p.X, p.Y, q.X, q.Y, out A, out B, deviation, bothDirection);
        }
        #endregion

        #region IN CENTER OF TRIANGLE
        public static VectorF InCenterOfTriangle(VectorF p1, VectorF p2, VectorF p3)
        {
            InCenterOfTriangle(p1, p2, p3, out float x, out float y);
            return new VectorF(x, y);
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
        /// <returns>Return Vector containing x and y.</returns>
        public static Vector XYOf(this int index, int width)
        {
            XYOf(index, width, out int x, out int y);
            return new Vector(x, y);
        }
        #endregion

        #region ANGLE BETWEEN 2 Points
        public static Rotation AngleFromVerticalCounterPart(this VectorF p, VectorF q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(DY, 0);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }
        public static Rotation AngleFromHorizontalCounterPart(this VectorF p, VectorF q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(0, DX);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }

        public static Rotation AngleFromVerticalCounterPart(this Vector p, Vector q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(DY, 0);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }
        public static Rotation AngleFromHorizontalCounterPart(this Vector p, Vector q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(0, DX);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }
        #endregion

        #region FIND PERPENDICULAR
        public static VectorF FindPerpendicularTo(this VectorF p, VectorF lp1, VectorF lp2)
        {
            return Lines.Perpendicular(lp1.X, lp1.Y, lp2.X, lp2.Y, p.X, p.Y);
        }
        public static ILine FindPerpendicularLine(this VectorF p, VectorF lp1, VectorF lp2)
        {
            var p1 = Lines.Perpendicular(lp1.X, lp1.Y, lp2.X, lp2.Y, p.X, p.Y);
            return Factory.newLine(p, p1);
        }
        #endregion

        #region SOLVE CONIC FROM 5 POINTS 
        /// <summary>
        /// Gets conic constants from given five points. This is an efficient alternative to Crammer's equatin solving algorithm.      
        /// Source: https://phys.libretexts.org/Bookshelves/Astronomy_and_Cosmology_TextMaps/Map%3A_Celestial_Mechanics_(Tatum)/2%3A_Conic_Sections/2.8%3A_Fitting_a_Conic_Section_Through_Five_Points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="p5"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <param name="E"></param>
        /// <param name="F"></param>
        /// <param name="Cx"></param>
        /// <param name="Cy"></param>
        /// <returns></returns>
        public static ConicType SolveConicEquation(VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5,
            out float A, out float B, out float C, out float D, out float E, out float F, out float Cx, out float Cy, out float Width,
            out float Height, out float angle)
        {
            LinePair.ConicEquation(p1, p2, p3, p4, p5, out A, out B, out C, out D, out E, out F);

            float bsq4ac = (B * B) - (4 * A * C);
            Cx = (((2 * C * D) - (B * E)) / bsq4ac);
            Cy = (-(((B * D) - (2 * A * E)) / bsq4ac));

            angle = (float)(Math.Atan(B / (A - C)) / 2) * Angles.Radinv;

            Curves.TranslateConicToOrigin(A, B, C, ref D, ref E, ref F);

            ConicType type;

            if (bsq4ac < 0)
                type = ConicType.Ellipse;

            else if (bsq4ac == 0)
                type = ConicType.Parabola;
            else
                type = ConicType.Hyperbola;


            if (type == ConicType.Ellipse)
            {
                float sin, cos, sin2, cos2, sinSq, cosSq;
                Angles.SinCos(angle, out sin, out cos);

                sin2 = (float)Math.Sin(2 * angle * Angles.Radian);
                cos2 = (float)Math.Cos(2 * angle * Angles.Radian);
                sinSq = sin * sin;
                cosSq = cos * cos;

                float standard_A, standard_B, standard_C;
                standard_B = B * cos2 + (C - A) * sin2;  //should be equal to 0 or near it

                if (standard_B < 0 || standard_B > 0)
                {
                    int pp = 0;
                    //rotation issue
                }
                standard_A = A * cosSq + B * sin * cos + C * sinSq;
                standard_C = A * sinSq - B * sin * cos + C * cosSq;
                float aSqr = -F / standard_A;
                float bSqr = -F / standard_C;

                //multiply by two to get the non-semi axes
                Width = (float)Math.Sqrt(Math.Abs(aSqr)) * 2;
                Height = (float)Math.Sqrt(Math.Abs(bSqr)) * 2;
            }
            else
            {
                Width = Curves.DefaultConicWidth;
                Height = Curves.DefaultConicHeight;
            }
            return type;
        }
        #endregion

        #region TO LINES
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<ILine> ToLines(this IEnumerable<VectorF> data, PointJoin join, float stroke = 0)
        {
            int count = data.Count();

            var lines = new Collection<ILine>(count / 2 + 1);
            bool connectEach = join.HasFlag(PointJoin.ConnectEach);
            bool unique = join.HasFlag(PointJoin.NoRepeat);
            bool joinEnds = join.HasFlag(PointJoin.ConnectEnds);
            bool noTooClose = join.HasFlag(PointJoin.AvoidTooClose);

            VectorF p0, p1, first;
            p0 = p1 = first = VectorF.Empty;

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

                if (noTooClose)
                {
                    if (p0.VeryCloseTo(p1))
                        continue;
                }
                lines.Add(Factory.newLine(p0, p1, stroke));

                if (connectEach)
                    p0 = p1;
                else
                    p0 = VectorF.Empty;
            }

            if (connectEach && joinEnds && !Equals(p1, first))
            {
                var line = Factory.newLine(p1, first, stroke);
                lines.Add(line);
            }

            if (join.HasFlag(PointJoin.RemoveLast))
                lines.RemoveAt(lines.Count - 1);

            return lines;
        }
        #endregion
    }
#endif
}