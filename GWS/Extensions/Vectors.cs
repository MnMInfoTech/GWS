/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    partial class Vectors
    {
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this IPointF v, IPointF p2) =>
            v.X * p2.X + v.Y * p2.Y;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSquared(this IPointF v) =>
            (v.X * v.X + v.Y * v.Y);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(this IPointF v) =>
          (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Dot(this IPoint v, IPoint p2) =>
            v.X * p2.X + v.Y * p2.Y;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LengthSquared(this IPoint v) =>
            (v.X * v.X + v.Y * v.Y);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Length(this IPoint v) =>
          (int)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        #endregion

        #region DISTANCE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float DistanceSquared(this IPointF p1, IPointF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;

            float ls = dx * dx + dy * dy;
            return (ls);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Distance(this IPointF p1, IPointF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;

            float ls = dx * dx + dy * dy;

            return (float)Math.Sqrt(ls);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DistanceSquared(this IPoint p1, IPoint p2)
        {
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;

            int ls = dx * dx + dy * dy;
            return (ls);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Distance(this IPoint p1, IPoint p2)
        {
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;

            int ls = dx * dx + dy * dy;

            return (int)Math.Sqrt(ls);
        }
        #endregion

        #region SLOPE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this IPointF p1, IPointF p2, out float c) =>
            Slope(p1.X, p1.Y, p2.X, p2.Y, out c);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this IPointF p1, IPointF p2) =>
            Slope(p1.X, p1.Y, p2.X, p2.Y, out bool steep);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this IPointF p1, IPointF p2, out bool steep) =>
            Slope(p1.X, p1.Y, p2.X, p2.Y, out steep);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Slope(this IPoint p1, IPoint p2, out bool steep) =>
            Slope(p1.X, p1.Y, p2.X, p2.Y, out steep);
        #endregion

        #region MIN AND MAX
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax<T>(this IEnumerable<T> collection,
            out float minX, out float minY, out float maxX, out float maxY, SizeF? clip = null) where T : IPointF
        {
            minX = minY = float.MaxValue;
            maxX = maxY = 0;
            bool pointFound = false;
            bool sizeFound = false;
            bool ok = false;

            if (collection is IPointF)
            {
                minX = ((IPointF)collection).X;
                minY = ((IPointF)collection).Y;
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
                maxX = ((ISize)collection).Width;
                maxY = ((ISize)collection).Height;
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
                if (p == null || (p as IValid)?.Valid == false)
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

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax<T>(this IEnumerable<T> collection, out int minX, out int minY, 
            out int maxX, out int maxY, Size? clip = null) where T : IPoint
        {
            minX = minY = int.MaxValue;
            maxX = maxY = 0;
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
                maxX = ((ISize)collection).Width;
                maxY = ((ISize)collection).Height;
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
                if (p == null || (p as IValid)?.Valid == false)
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

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax<T>(out float minX, out float minY, out float maxX,
            out float maxY, params T[] collection) where T : IPointF =>
            MinMax(collection as IEnumerable<T>, out minX, out minY, out maxX, out maxY);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax<T>(SizeF clip, out float minX, out float minY, 
            out float maxX, out float maxY, params T[] collection) where T : IPointF =>
            MinMax(collection as IEnumerable<T>, out minX, out minY, out maxX, out maxY, clip);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax<T>(out int minX, out int minY, 
            out int maxX, out int maxY, params T[] collection) where T : IPoint =>
            MinMax(collection as IEnumerable<T>, out minX, out minY, out maxX, out maxY);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax<T>(Size clip, out int minX, out int minY, 
            out int maxX, out int maxY, params T[] collection) where T : IPoint =>
            MinMax(collection as IEnumerable<T>, out minX, out minY, out maxX, out maxY, clip);
        #endregion

        #region PARALLEL
        public static void ParallelAt(float x1, float y1, IPointF p1, IPointF p2, float length, out float x2, out float y2) =>
            ParallelAt(x1, y1, Slope(p1, p2, out bool steep), length, steep, out x2, out y2);
        public static void ParallelAt(int x1, int y1, IPoint p1, IPoint p2, int length, out int x2, out int y2) =>
                ParallelAt(x1, y1, Slope(p1, p2, out bool steep), length, steep, out x2, out y2);
        #endregion

        #region > OR < THAN POINT
        public static bool IsGreaterThan(this IPointF p, IPointF p1, IPointF p2)
        {
            var c1 = (p2.X - p1.X) * (p.Y - p1.Y);
            var c2 = (p2.Y - p1.Y) * (p.X - p1.X);
            //if (Math.Abs(c1 - c2) < 0.01f)
            //    return true;
            return c1 < c2;
        }
        public static bool IsLessThan(this IPointF p, IPointF p1, IPointF p2)
        {
            var c1 = (p2.X - p1.X) * (p.Y - p1.Y);
            var c2 = (p2.Y - p1.Y) * (p.X - p1.X);
            //if (Math.Abs(c1 - c2) < 0.01f)
            //    return true;
            return c1 > c2;
        }
        public static bool IsGreaterThan(float x, float y, IPointF p1, IPointF p2)
        {
            var c1 = (p2.X - p1.X) * (y - p1.Y);
            var c2 = (p2.Y - p1.Y) * (x - p1.X);
            //if (Math.Abs(c1 - c2) < 0.01f)
            //    return true;
            return c1 < c2;
        }
        public static bool IsLessThan(float x, float y, IPointF p1, IPointF p2)
        {
            var c1 = (p2.X - p1.X) * (y - p1.Y);
            var c2 = (p2.Y - p1.Y) * (x - p1.X);
            //if (Math.Abs(c1 - c2) < 0.01f)
            //    return true;
            return c1 > c2;
        }
        #endregion

        #region IN CENTER OF TRIANGLE
        public static void InCenterOfTriangle(IPointF p1, IPointF p2, IPointF p3, out float cx, out float cy)
        {
            var a = p1.Distance(p2);
            var b = p2.Distance(p3);
            var c = p3.Distance(p1);
            cx = (a * p1.X + b * p2.X + c * p3.X) / (a + b + c);
            cy = (a * p1.Y + b * p2.Y + c * p3.Y) / (a + b + c);
        }
        #endregion

        #region 3 PPOINTS MOTION - CLOCK WISE OR NOT
        public static bool ClockWise<T>(this IList<T> points) where T : IPointF
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
        public static bool Clockwise<T>(params T[] points) where T : IPointF =>
            ClockWise(points as IList<T>);
        #endregion

        #region OFFSET
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Offset(this IPointF v, IPointF o)
        {
            return new VectorF(v.X + o.X, v.Y + o.Y, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Offset(this IPointF v, float x, float y)
        {
            return new VectorF(v.X + x, v.Y + y, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Offset(this IPoint v, IPoint o)
        {
            return new Vector(v.X + o.X, v.Y + o.Y, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Offset(this IPoint v, int x, int y)
        {
            return new Vector(v.X + x, v.Y + y, v is IPointType ? ((IPointType)v).Kind : 0);
        }
        #endregion

        #region  MIN - MAX - AVG
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Min(this IPointF v, IPointF p2)
        {
            return new VectorF(
                (v.X < p2.X) ? v.X : p2.X,
                (v.Y < p2.Y) ? v.Y : p2.Y, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Max(this IPointF v, IPointF p2)
        {
            return new VectorF(
                (v.X > p2.X) ? v.X : p2.X,
                (v.Y > p2.Y) ? v.Y : p2.Y, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Avg(this IPointF v, IPointF p2) =>
            new VectorF((v.X + p2.X) / 2, (v.Y + p2.Y) / 2, v is IPointType ? ((IPointType)v).Kind : 0);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Min(this IPoint v, IPoint p2)
        {
            return new Vector(
                (v.X < p2.X) ? v.X : p2.X,
                (v.Y < p2.Y) ? v.Y : p2.Y, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Max(this IPoint v, IPoint p2)
        {
            return new Vector(
                (v.X > p2.X) ? v.X : p2.X,
                (v.Y > p2.Y) ? v.Y : p2.Y, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Avg(this IPoint v, IPoint p2) =>
            new Vector((v.X + p2.X) / 2, (v.Y + p2.Y) / 2, v is IPointType ? ((IPointType)v).Kind : 0);
        #endregion

        #region TO AREA
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToArea<T>(this IEnumerable<T> points, out Rectangle rect) where T : IPoint
        {
            points.MinMax(out int minx, out int miny, out int maxx, out int maxy);
            rect = Rectangles.FromLTRB(minx, miny, maxx, maxy, true);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF ToArea<T>(this IEnumerable<T> points) where T : IPointF
        {
            points.MinMax(out float minx, out float miny, out float maxx, out float maxy);
            return Rectangles.FromLTRB(minx, miny, maxx, maxy, true);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToArea<T>(this IEnumerable<T> points,
            out float x, out float y, out float w, out float h) where T : IPointF
        {
            points.MinMax(out x, out y, out w, out h);
            w -= x;
            h -= y;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToArea<T>(this IEnumerable<T> points,
            out int x, out int y, out int w, out int h) where T : IPointF
        {
            points.MinMax(out float minx, out float miny, out float maxx, out float maxy);
            x = (int)minx;
            y = (int)miny;
            w = (int)maxx;
            h = (int)maxy;
            if (maxx - w != 0)
                ++w;
            if (maxy - h != 0)
                ++h;
            w -= x;
            h -= y;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF ToArea<T>(params T[] points) where T: IPointF =>
            ToArea(points as IEnumerable<T>);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle ToArea(params Vector[] points)
        {
            ToArea(points as IEnumerable<Vector>, out Rectangle r);
            return r;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            return Rectangles.FromLTRB(minX, minY, maxX, maxY, true);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            return Rectangles.FromLTRB(minX, minY, maxX, maxY, true);
        }
        #endregion

        #region CENTER
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Center<T>(this IEnumerable<T> points, out Vector result) where T: IPoint
        {
            points.MinMax(out int minx, out int miny, out int maxx, out int maxy);
            result = new Vector(minx + (maxx - minx) / 2, miny + (maxy - miny) / 2);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Center<T>(this IEnumerable<T> points) where T: IPointF
        {
            points.MinMax(out float minx, out float miny, out float maxx, out float maxy);
            return new VectorF(minx + (maxx - minx) / 2, miny + (maxy - miny) / 2);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Center<T>(params T[] points) where T: IPointF =>
            Center(points as IEnumerable<T>);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Center(params Vector[] points)
        {
            Center(points as IEnumerable<Vector>, out Vector v);
            return v;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Avg<T>(this IEnumerable<T> collection) where T: IPointF
        {
            float x = 0, y = 0;
            int i = -1;

            foreach (var p in collection)
            {
                if (p == null || (p as IValid)?.Valid == false)
                    continue;
                ++i;
                x += p.X;
                y += p.Y;
            }
            return new VectorF(x / i, y / i);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Avg<T>(this IEnumerable<T> collection, out Vector v) where T: IPoint
        {
            int x = 0, y = 0;
            int i = -1;

            foreach (var p in collection)
            {
                if (p == null || (p as IValid)?.Valid == false)
                    continue;
                ++i;
                x += p.X;
                y += p.Y;
            }
            v = new Vector(x / i, y / i);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Avg<T>(params T[] collection) where T: IPointF  =>
            Avg(collection as IEnumerable<T>);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Avg(params Vector[] collection)
        {
            Avg(collection as IEnumerable<Vector>, out Vector v);
            return v;
        }
        #endregion

        #region TO POINTS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PrimitiveList<VectorF> ToVectorF(this IEnumerable<int> xyPairs, PointKind? kind = null)
        {
            if (xyPairs == null)
                return new PrimitiveList<VectorF>();
            PrimitiveList<VectorF> points;
            var len = xyPairs.Count();
            var previous = int.MaxValue;
            points = new PrimitiveList<VectorF>(len);
            var k = kind ?? 0;
            foreach (var item in xyPairs)
            {
                if (previous == int.MaxValue)
                    previous = item;
                else
                {
                    points.Add(new VectorF(previous, item, k));
                    previous = int.MaxValue;
                }
            }
            return points;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PrimitiveList<VectorF> ToVectorF(this IEnumerable<float> xyPairs, PointKind? kind = null)
        {
            if (xyPairs == null)
                return new PrimitiveList<VectorF>();

            PrimitiveList<VectorF> points;
            var len = xyPairs.Count();
            var previous = float.MaxValue;
            var k = kind ?? PointKind.Normal;
            points = new PrimitiveList<VectorF>(len);
            foreach (var item in xyPairs)
            {
                if (previous == float.MaxValue)
                    previous = item;
                else
                {
                    points.Add(new VectorF(previous, item, k));
                    previous = float.MaxValue;
                }
            }

            return points;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PrimitiveList<VectorF> ToVectorF(PointKind? kind, params float[] xyPairs) =>
            ToVectorF(xyPairs as IEnumerable<float>, kind);
        #endregion

        #region SCALE     
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Scale(this IPointF v, float scaleX, float scaleY, float centerX, float centerY)
        {
            Scale(v.X, v.Y, scaleX, scaleY, centerX, centerY, out float x, out float y);
            return new VectorF(x, y, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Scale(this IPointF v, float scaleX, float scaleY)
        {
            return new VectorF(v.X * scaleX, v.Y * scaleY, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Scale(this IPoint v, float scaleX, float scaleY)
        {
            return new Vector((v.X * scaleX).Round(), (v.Y * scaleY).Round(), v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Scale(this IPoint v, int scaleX, int scaleY)
        {
            return new Vector((v.X * scaleX), (v.Y * scaleY), v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Scale(this IPoint v, int scaleX, int scaleY, int centerX, int centerY)
        {
            Scale(v.X, v.Y, scaleX, scaleY, centerX, centerY, out int x, out int y);
            return new Vector(x, y, v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Scale(this IPoint v, float scaleX, float scaleY, float centerX, float centerY)
        {
            Scale(v.X, v.Y, scaleX, scaleY, centerX, centerY, out float x, out float y);
            return new Vector(x.Round(), y.Round(), v is IPointType ? ((IPointType)v).Kind : 0);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<VectorF> Scale(this IEnumerable<VectorF> source,
            float? sx = null, float? sy = null, VectorF? center = null)
        {
            if (sx == null && sy == null ||
                (sx == 0 || sx == 1) &&
                (sy == 0 || sy == 1))
                return source;
            return source.Scale(new Scale(sx, sy), center);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IEnumerable<VectorF> Scale(this IEnumerable<VectorF> source, IScale scale, VectorF? center = null)
        {
            if(scale is null || !scale.HasScale)
            {
                return source;
            }
            var sx = scale.X;
            var sy = scale.Y;

            var c = center ?? VectorF.Empty;
            var cx = c.X;
            var cy = c.Y;
            VectorF[] items;
            int count = 0;

            if (source is IArrayHolder<VectorF>)
                items = ((IArrayHolder<VectorF>)source).Data;
            else
                items = source.ToArray();
            count = items.Length;

            fixed (VectorF* pts = items)
            {
                for (int i = 0; i < count; i++)
                {
                    var x = pts[i].X;
                    var y = pts[i].Y;

                    if (sx != 1)
                        x = (x - cx) * sx + cx;

                    if (sy != 1)
                        y = (y - cy) * sy + cy;

                    pts[i] = new VectorF(x, y, items[i].Kind);
                }
            }
            return items;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector> Scale(this IEnumerable<Vector> source, int? sx = null, int? sy = null,
            Vector? center = null)
        {
            if (sx == null && sy == null ||
                (sx == 0 || sx == 1) && 
                (sy == 0 || sy == 1) )
                return source;           
                return source.Scale(new Scale(sx, sy), center);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IEnumerable<Vector> Scale(this IEnumerable<Vector> source, IScale scale, Vector? center = null)
        {
            if (scale == null || !scale.HasScale)
                return source;

            var sx = scale.X;
            var sy = scale.Y;

            var c = center ?? Vector.Empty;
            var cx = c.X;
            var cy = c.Y;
            Vector[] items;
            int count = 0;

            if (source is IArrayHolder<Vector>)
                items = ((IArrayHolder<Vector>)source).Data;
            else
                items = source.ToArray();
            count = items.Length;

            fixed (Vector* pts = items)
            {
                for (int i = 0; i < count; i++)
                {
                    float x = pts[i].X;
                    float y = pts[i].Y;

                    if (sx != 1)
                        x = (x - cx) * sx + cx;

                    if (sy != 1)
                        y = (y - cy) * sy + cy;

                    pts[i] = new Vector(x, y, pts[i].Kind);
                }
            }
            return items;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScaleCursor(this IBounds rc1, int x, int y, 
            IScale Scale, out int x0, out int y0, IPoint offset = null, bool inverse = false)
        {
            float m = x, n = y;

            bool HasScale = Scale != null && Scale.HasScale;

            if (HasScale)
            {
                rc1.GetBounds(out int x1, out int y1, out int w1, out int h1);
                IScale s = Scale;
                if (inverse)
                    s = new Scale(1f / Scale.X, 1f / Scale.Y);
                Rectangles.Scale(x1, y1, w1, h1, out float x2, out float y2, out float w2, out float h2, s);
                m = ((x - x1) / (float)w1) * w2 + x2;
                n = ((y - y1) / (float)h1) * h2 + y2;
            }
            if (offset != null)
            {
                m += offset.X;
                n += offset.Y;
            }
            x0 = m.Round();
            y0 = n.Round();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScaleCursor(this IBounds area, float x, float y,
            IScale Scale, out float x0, out float y0, IPoint offset = null, bool inverse = false)
        {
            x0 = x;
            y0 = y;
            bool HasScale = Scale != null && Scale.HasScale;

            if (HasScale)
            {
                area.GetBounds(out int x1, out int y1, out int w1, out int h1);
                IScale s = Scale;
                if (inverse)
                    s = new Scale(1f / Scale.X, 1f / Scale.Y);
                Rectangles.Scale(x1, y1, w1, h1, out float x2, out float y2, out float w2, out float h2, s);
                x0 = (x - x1) / w1 * w2 + x2;
                y0 = (y - y1) / h1 * h2 + y2;
            }
            if (offset != null)
            {
                x0 += offset.X;
                y0 += offset.Y;
            }
        }
        #endregion

        #region GET CENTER
        public static VectorF FindCenterFrom(this IPointF p, IPointF q)
        {
            var cx = Math.Min(p.X, q.X) + Math.Abs(q.X - p.X) / 2;
            var cy = Math.Min(p.Y, q.Y) + Math.Abs(q.Y - p.Y) / 2;
            return new VectorF(cx, cy);
        }
        public static VectorF FindCenterFrom(this IPoint p, IPoint q)
        {
            var cx = Math.Min(p.X, q.X) + Math.Abs(q.X - p.X) / 2f;
            var cy = Math.Min(p.Y, q.Y) + Math.Abs(q.Y - p.Y) / 2f;
            return new VectorF(cx, cy);
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF FourthPointOfRhombus(this IPointF first, IPointF second, IPointF third)
        {
            FourthPointOfRhombus(first.X, first.Y, second.X, second.Y, third.X, third.Y, out float x, out float y);
            return new VectorF(x, y);
        }

        /// <summary>
        /// Finds missing 4th point of rhombus - derived from 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="first">1st point of rhombus</param>
        /// <param name="second">2nd point of rhombus.</param>
        /// <param name="third">3rd point of rhombus.</param>
        /// <returns>Missing 4th point of rhombus.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector FourthPointOfRhombus(this IPoint first, IPoint second, IPoint third)
        {
            FourthPointOfRhombus(first.X, first.Y, second.X, second.Y, third.X, third.Y, out float x, out float y);
            return new Vector(x, y);
        }

        /// <summary>
        /// Finds missing 4th point of rhombus - derived from 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="first">1st point of rhombus</param>
        /// <param name="second">2nd point of rhombus.</param>
        /// <param name="third">3rd point of rhombus.</param>
        /// <returns>Missing 4th point of rhombus.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FourthPointOfRhombus(this IPoint first, IPoint second, IPoint third, out int x, out int y)
        {
            FourthPointOfRhombus(first.X, first.Y, second.X, second.Y, third.X, third.Y, out x, out y);
        }

        /// <summary>
        /// Finds missing 4th point of rhombus - derived from 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="first">1st point of rhombus</param>
        /// <param name="second">2nd point of rhombus.</param>
        /// <param name="third">3rd point of rhombus.</param>
        /// <returns>Missing 4th point of rhombus.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FourthPointOfRhombus(this IPointF first, IPointF second, IPointF third, out float x, out float y)
        {
            FourthPointOfRhombus(first.X, first.Y, second.X, second.Y, third.X, third.Y, out x, out y);
        }


        /// <summary>
        /// Finds missing 4th point of rhombus - derived from 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="first">1st point of rhombus</param>
        /// <param name="second">2nd point of rhombus.</param>
        /// <param name="third">3rd point of rhombus.</param>
        /// <returns>Missing 4th point of rhombus.</returns>
        /// <param name="boundingBox">Bounding box the result point must lie within.</param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF FourthPointOfRhombus(this IPointF first, IPointF second, IPointF third, IBounds boundingBox)
        {
            boundingBox.GetBounds(out int bX, out int bY, out int bWidth, out int bHeight);
            FourthPointOfRhombus(first.X, first.Y, second.X, second.Y,
                third.X, third.Y, bX, bY, bX + bWidth, bY + bHeight,
                out float x, out float y);
            return new VectorF(x, y);
        }

        /// <summary>
        /// Finds missing 4th point of rhombus - derived from 3 points supplied.
        /// Source: https://www.geeksforgeeks.org/find-missing-point-parallelogram/
        /// </summary>
        /// <param name="first">1st point of rhombus</param>
        /// <param name="second">2nd point of rhombus.</param>
        /// <param name="third">3rd point of rhombus.</param>
        /// <param name="boundsX">X co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="boundsY">Y co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="boundsR">Far right X co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <param name="boundsB">>Far bottom Y co-ordinate of bounding rectangle, result point must lie within.</param>
        /// <returns>Missing 4th point of rhombus.</returns>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF FourthPointOfRhombus(this IPointF first, IPointF second, IPointF third,
            float boundsX, float boundsY, float boundsR, float boundsB)
        {
            FourthPointOfRhombus(first.X, first.Y, second.X, second.Y,
                third.X, third.Y, boundsX, boundsY, boundsR,
                boundsB, out float x, out float y);
            return new VectorF(x, y);
        }
        #endregion

        #region CORRECT RHOMBUS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectRhombus(ref Point[] pts)
        {
            if (pts == null)
                return;
            int x, y;
            if (pts[0] == pts[1] || pts[0] == pts[2])
            {
                Vectors.FourthPointOfRhombus(pts[1], pts[2], pts[3], out x, out y);
                pts[0] = new Point(x, y);
            }
            else if (pts[0] == pts[3])
            {
                Vectors.FourthPointOfRhombus(pts[0], pts[1], pts[2], out x, out y);
                pts[3] = new Point(x, y);
            }
            else if (pts[1] == pts[2] || pts[1] == pts[3])
            {
                Vectors.FourthPointOfRhombus(pts[0], pts[2], pts[3], out x, out y);
                pts[1] = new Point(x, y);
            }
            else if (pts[2] == pts[3])
            {
                Vectors.FourthPointOfRhombus(pts[0], pts[1], pts[3], out x, out y);
                pts[2] = new Point(x, y);
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectRhombus(ref Vector[] pts)
        {
            if (pts == null)
                return;
            int x, y;
            if (pts[0] == pts[1] || pts[0] == pts[2])
            {
                Vectors.FourthPointOfRhombus(pts[1], pts[2], pts[3], out x, out y);
                pts[0] = new Vector(x, y);
            }
            else if (pts[0] == pts[3])
            {
                Vectors.FourthPointOfRhombus(pts[0], pts[1], pts[2], out x, out y);
                pts[3] = new Vector(x, y);
            }
            else if (pts[1] == pts[2] || pts[1] == pts[3])
            {
                Vectors.FourthPointOfRhombus(pts[0], pts[2], pts[3], out x, out y);
                pts[1] = new Vector(x, y);
            }
            else if (pts[2] == pts[3])
            {
                Vectors.FourthPointOfRhombus(pts[0], pts[1], pts[3], out x, out y);
                pts[2] = new Vector(x, y);
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectRhombus(ref VectorF[] pts)
        {
            if (pts == null)
                return;
            float x, y;
            if (pts[0] == pts[1] || pts[0] == pts[2])
            {
                Vectors.FourthPointOfRhombus(pts[1], pts[2], pts[3], out x, out y);
                pts[0] = new VectorF(x, y);
            }
            else if (pts[0] == pts[3])
            {
                Vectors.FourthPointOfRhombus(pts[0], pts[1], pts[2], out x, out y);
                pts[3] = new VectorF(x, y);
            }
            else if (pts[1] == pts[2] || pts[1] == pts[3])
            {
                Vectors.FourthPointOfRhombus(pts[0], pts[2], pts[3], out x, out y);
                pts[1] = new VectorF(x, y);
            }
            else if (pts[2] == pts[3])
            {
                Vectors.FourthPointOfRhombus(pts[0], pts[1], pts[3], out x, out y);
                pts[2] = new VectorF(x, y);
            }
        }
        #endregion

        #region MOVE - EXTEND
        public static VectorF Move(IPointF p, IPointF q, float move)
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
        public static void Extend(IPointF p, IPointF q, out VectorF A, out VectorF B, float deviation, bool bothDirection = false)
        {
            Extend(p.X, p.Y, q.X, q.Y, out A, out B, deviation, bothDirection);
        }
        #endregion

        #region IN CENTER OF TRIANGLE
        public static VectorF InCenterOfTriangle(IPointF p1, IPointF p2, IPointF p3)
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
        public static IRotation AngleFromVerticalCounterPart(this IPointF p, IPointF q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(DY, 0);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }
        public static IRotation AngleFromHorizontalCounterPart(this IPointF p, IPointF q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(0, DX);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }

        public static IRotation AngleFromVerticalCounterPart(this IPoint p, IPoint q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(DY, 0);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }
        public static IRotation AngleFromHorizontalCounterPart(this IPoint p, IPoint q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(0, DX);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }
        public static float AngleWith(float px, float py, float qx, float qy)
        {
            float dot = px * qx + py * qy;
            float det = px * qy - py * qx;
            float angle = (float)Math.Atan2(det, dot) * Angles.Radinv;
            return angle;
        }
        #endregion

        #region FIND PERPENDICULAR
        public static VectorF FindPerpendicularTo(this IPointF p, IPointF lp1, IPointF lp2)
        {
            return Lines.Perpendicular(lp1.X, lp1.Y, lp2.X, lp2.Y, p.X, p.Y);
        }
        public static ILine FindPerpendicularLine(this IPointF p, IPointF lp1, IPointF lp2)
        {
            var p1 = Lines.Perpendicular(lp1.X, lp1.Y, lp2.X, lp2.Y, p.X, p.Y);
            return new Line(p, p1);
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
        public static ConicType SolveConicEquation(IPointF p1, IPointF p2, IPointF p3, IPointF p4, IPointF p5,
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<ILine> ToLines(this IEnumerable<VectorF> data, PointJoin join, float stroke = 0)
        {
            int count = data.Count();
            var lines = new PrimitiveList<ILine>(count / 2 + 1);
            if (count == 2)
            {
                lines.Add(new Line(data.ElementAt(0), data.ElementAt(1)));
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
                if ((item.Kind & PointKind.Break)== PointKind.Break)
                {
                    if (
                        connectEach && joinEnds && !Equals(p1, first))
                    {
                        var line = new Line(p1, first, stroke);
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

                lines.Add(new Line(p0, p1, stroke));

                if (connectEach)
                    p0 = p1;
                else
                    p0 = VectorF.Empty;
            }

            if (connectEach && joinEnds && !Equals(p1, first))
            {
                var line = new Line(p1, first, stroke);
                lines.Add(line);
            }

            if ((join & PointJoin.RemoveLast) == PointJoin.RemoveLast)
                lines.RemoveAt(lines.Count - 1);

            return lines;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<ILine> ToLines(this IEnumerable<Vector> data, PointJoin join, float stroke = 0)
        {
            int count = data.Count();

            var lines = new PrimitiveList<ILine>(count / 2 + 1);
            if (count == 2)
            {
                lines.Add(new Line(data.ElementAt(0), data.ElementAt(1)));
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
                        var line = new Line(p1.X, p1.Y, first.X, first.Y, stroke);
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

                lines.Add(new Line(p0.X, p0.Y, p1.X, p1.Y, stroke));

                if (connectEach)
                    p0 = p1;
                else
                    p0 = Vector.Empty;
            }

            if (connectEach && joinEnds && !Equals(p1, first))
            {
                var line = new Line(p1.X, p1.Y, first.X, first.Y, stroke);
                lines.Add(line);
            }

            if ((join & PointJoin.RemoveLast) == PointJoin.RemoveLast)
                lines.RemoveAt(lines.Count - 1);

            return lines;
        }
        #endregion

        #region DE-COMPOSE
        public static unsafe VectorF[] DeCompose(this IEnumerable<VectorF> points, IEnumerable<int> contours, float height = 0)
        {
            var firstIndex = 0;
            var curveLevels = new int[32];
            var bezierArc = new VectorF[curveLevels.Length * 3 + 1];

            int[] Contours = null;
            VectorF[] Points = points is VectorF[]? (VectorF[]) points: points.ToArray();

            if (contours != null)
                Contours = contours is int[]? (int[])contours : contours.ToArray();

            if(Contours == null  || Contours.Length == 0)
                Contours = new int[] { Points.Length - 1 };

            PrimitiveList<VectorF> Result = new PrimitiveList<VectorF>(Points.Length);
            VectorF ActivePoint = VectorF.Empty;

            var ContoursLength = Contours.Length;

            fixed(int* ct = Contours)
            {
                fixed(VectorF* pts = Points)
                {
                    for (int i = 0; i < ContoursLength; i++)
                    {
                        // decompose the contour into drawing commands
                        int lastIndex = ct[i];
                        var pointIndex = firstIndex;
                        var start = pts[pointIndex];
                        var end = pts[lastIndex];
                        if ((start.Kind & PointKind.Control) == PointKind.Control)
                        {
                            // if first point is a control point, try using the last point
                            if ((end.Kind & PointKind.Control) == PointKind.Control)
                            {
                                start = end;
                                lastIndex--;
                            }
                            else
                            {
                                // if they're both control points, start at the middle
                                start = (start + end) / 2f;
                            }
                            pointIndex--;
                        }

                        // let's draw this contour
                        MoveTo(start, ref ActivePoint, Result);

                        var needClose = true;
                        while (pointIndex < lastIndex)
                        {
                            var point = pts[++pointIndex];
                            bool cp = (point.Kind & PointKind.Control) == PointKind.Control;
                            switch (cp)
                            {
                                case false:
                                default:
                                    LineTo(point, ref ActivePoint, Result);
                                    break;

                                case true:
                                    var control = point;
                                    var done = false;
                                    while (pointIndex < lastIndex)
                                    {
                                        var next = pts[++pointIndex];
                                        if ((next.Kind & PointKind.Control) != PointKind.Control)
                                        {
                                            CurveTo(control, next, ref ActivePoint, Result, curveLevels, bezierArc);
                                            done = true;
                                            break;
                                        }

                                        if ((next.Kind & PointKind.Control) != PointKind.Control)
                                            throw new Exception("Bad outline data.");
                                        var p = (control + next) / 2f;
                                        CurveTo(control, p, ref ActivePoint, Result, curveLevels, bezierArc);
                                        control = next;
                                    }

                                    if (!done)
                                    {
                                        // if we hit this point, we're ready to close out the contour
                                        CurveTo(control, start, ref ActivePoint, Result, curveLevels, bezierArc);
                                        needClose = false;
                                    }
                                    break;
                            }
                        }

                        if (needClose)
                        {
                            LineTo(start, ref ActivePoint, Result);
                        }
                        // next contour starts where this one left off
                        firstIndex = lastIndex + 1;
                        Result.Add(VectorF.Break);
                        ActivePoint = VectorF.Empty;
                        //
                    }
                }
            }

            VectorF[] result;
            if (height == 0)
                result = Result.ToArray();
            else
                result = Result.Select(p => p.FlipVertical(height)).ToArray();
            return result;
        }
        #region MOVE TO
        static void MoveTo(VectorF p, ref VectorF Start, ICollection<VectorF> points)
        {
            if(p)
                points.Add(p);
            Start = p;
        }
        #endregion

        #region CURVE TO
        static void CurveTo(VectorF controlPoint, VectorF endPoint, 
            ref VectorF Start, ICollection<VectorF> points, int[] curveLevels, VectorF[] bezierArc)
        {
            if (!Start)
                return;

            var levels = curveLevels;
            var arc = bezierArc;
            arc[0] = endPoint;
            arc[1] = controlPoint;
            arc[2] = Start;

            var dx = Math.Abs(arc[2].X + arc[0].X - 2 * arc[1].X);
            var dy = Math.Abs(arc[2].Y + arc[0].Y - 2 * arc[1].Y);

            if (dx < dy)
                dx = dy;

            // short cut for small arcs
            if (dx < 0.25f)
            {
                LineTo(arc[0], ref Start, points);
                return;
            }

            int level = 0;
            do
            {
                dx /= 4.0f;
                level++;
            } while (dx > 0.25f);

            int top = 0;
            int i = 0;
            levels[0] = level;

            while (top >= 0)
            {
                level = levels[top];
                if (level > 0)
                {
                    // split the arc
                    arc[i + 4] = arc[i + 2];
                    var b = arc[i + 1];
                    var a = new VectorF((arc[i + 2].X + b.X) / 2f, (arc[i + 2].Y + b.Y) / 2f, arc[i + 2].Kind);
                    arc[i + 3] = a;

                    b = new VectorF((arc[i].X + b.X) / 2f, (arc[i].Y + b.Y) / 2f, arc[i].Kind);
                    arc[i + 1] = b;
                    a = new VectorF((a.X + b.X) / 2f, (a.Y + b.Y) / 2f, a.Kind);
                    arc[i + 2] = a;
                    i += 2;
                    top++;
                    levels[top] = levels[top - 1] = level - 1;
                }
                else
                {
                    LineTo(arc[i], ref Start, points);
                    top--;
                    i -= 2;
                }
            }
        }
        #endregion

        #region SCAN
        static void LineTo(VectorF p, ref VectorF Start, ICollection<VectorF> points)
        {
            //if (!Start)
            //{
            //    Start = p;
            //    points.Add(p);
            //    return; 
            //}
            points.Add(p);
            Start = p;
        }
        #endregion
        #endregion
    }
}
#endif
