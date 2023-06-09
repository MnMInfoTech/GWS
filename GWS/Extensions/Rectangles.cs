/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    partial class Rectangles
    {
        #region COMPITIBLE RC
        /// <summary>
        /// Returns an IRectangle that is compatible with the one required.
        /// </summary>
        /// <param name="srcW">Width reuired</param>
        /// <param name="srcH">Height Required</param>
        /// <param name="copyX">Proposed X position if any.</param>
        /// <param name="copyY">Proposed Y position if any.</param>
        /// <param name="copyW">Proposed width if any.</param>
        /// <param name="copyH">Prooposed height if any.</param>
        /// <returns>Returns Resultant Rectangle object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle CompitibleRc(int srcW, int srcH, int copyX, int copyY, int copyW, int copyH)
        {
            var x0 = copyX;
            var y0 = copyY;

            if (x0 < 0)
                x0 = 0;
            if (y0 < 0)
                y0 = 0;

            var w = Math.Min(copyW, srcW);
            var h = Math.Min(copyH, srcH);
            if (h < 0 || w < 0)
                return Rectangle.Empty;

            var right = x0 + w;
            var bottom = y0 + h;
            if (right > srcW)
                right = srcW;
            if (bottom > srcH)
                bottom = srcH;
            return Rectangle.FromLTRB(x0, y0, right, bottom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sz">Size object defining rectangle.</param>
        /// <param name="x0">Proposed X position if any.</param>
        /// <param name="y0">Proposed Y position if any.</param>
        /// <param name="width">Proposed width if any.</param>
        /// <param name="height">Prioposed height if any.</param>
        /// <returns>Returns a rexctangle compatible with the specified ISize object using the provided parameters (if any) or and empty Irectangle object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle CompitibleRc(this ISize sz, int x0, int y0, int width, int height)
        {
            if (sz is IColour)
            {
                return new Rectangle(x0, y0, width, height);
            }
            else
                return CompitibleRc(sz.Width, sz.Height, x0, y0, width, height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle CompitibleRc(this IBounds rect, int x, int y, int width, int height, ObjType? type = null)
        {
            if (rect == null || !rect.Valid)
                return Rectangle.Empty;

            var r = x + width;
            var b = y + height;

            int x1, y1, w1, h1;

            rect.GetBounds(out x1, out y1, out w1, out h1);

            var r1 = x1 + w1;
            var b1 = y1 + h1;

            if (x < x1)
            {
                var diff = x1 - x;
                r -= diff;
                x = x1;
            }
            if (y < y1)
            {
                var diff = y1 - y;
                b -= diff;
                y = y1;
            }

            if (!rect.Contains(x, y))
                return Rectangle.Empty;

            if (r > r1)
                r = r1;

            if (b > b1)
                b = b1;

            if (type != null)
                return UpdateArea.FromLTRB(x, y, r, b, type.Value);

            else if (rect is IType)
                return UpdateArea.FromLTRB(x, y, r, b, ((IType)rect).Type);

            return Rectangle.FromLTRB(x, y, r, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle CompitibleRc(this IBounds rc1, IBounds rc2)
        {
            if (rc1 == null || rc2 == null || !rc1.Valid || !rc2.Valid)
                return Rectangle.Empty;

            int x, y, w, h;
            rc2.GetBounds(out x, out y, out w, out h);

            ObjType? type = null;

            if (rc1 is IType)
                type = ((IType)rc1).Type;
            else if (rc2 is IType)
                type = ((IType)rc2).Type;
            return rc1.CompitibleRc(x, y, w, h, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle CompitibleRc(int x1, int y1, int w1, int h1, IBounds rect, ObjType? type = null)
        {   
            if (rect == null || !rect.Valid)
                return Rectangle.Empty;
            int x, y, w, h;
            rect.GetBounds(out  x, out y, out w, out h);
            return CompitibleRc(x1, y1, w1, h1, x, y, w, h, type);
        }
        #endregion

        #region HYBRID
        /// <summary>
        /// Return a IRectangle object defining an area containing the areas of two specified rectangles.
        /// </summary>
        /// <param name="rc1">First rectangle to be merged.</param>
        /// <param name="rc2">Second rectangle to be merged.</param>
        /// <returns>IRectangle with area containing the orriginal rectangles.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle Hybrid(this IBounds rc1, IBounds rc2)
        {
            ObjType? type = null;
            if (rc1 is IType)
                type = ((IType)rc1).Type;
            else if (rc2 is IType)
                type = ((IType)rc2).Type;

            if (rc1 == null || !rc1.Valid)
            {
                if (type != null)
                    return new UpdateArea(rc2, type);
                return new Rectangle(rc2);
            }
            if (rc2 == null || !rc2.Valid)
            {
                if (type != null)
                    return new UpdateArea(rc1, type);
                return new Rectangle(rc1);
            }

            int x, y, w, h;
            rc2.GetBounds(out x, out y, out w, out h);
            return rc1.Hybrid(x, y, w, h, type);
          
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle Hybrid(this IBounds rc1, int x1, int y1, int w1, int h1, ObjType? type = null)
        {
            if (rc1 is IType && type == null)
                type = ((IType)rc1).Type;

            if (rc1 == null || !rc1.Valid)
            {
                if (type != null)
                    return new UpdateArea(x1, y1, w1, h1, type.Value);
                return new Rectangle(x1, y1, w1, h1);
            }

            int x, y, w, h, r, b, r1, b1;
            rc1.GetBounds(out x, out y, out w, out h);
            r = x + w;
            b = y + h;
            r1 = x1 + w1;
            b1 = y1 + h1;
            if (x1 < x)
                x = x1;
            if (y1 < y)
                y = y1;
            if (r1 > r)
                r = r1;
            if (b1 > b)
                b = b1;

            if (type != null)
                return UpdateArea.FromLTRB(x, y, r, b, type.Value);

            else if (rc1 is IType)
                return UpdateArea.FromLTRB(x, y, r, b, ((IType)rc1).Type);

            return Rectangle.FromLTRB(x, y, r, b);
        }

        /// <summary>
        /// Return a IRectangle object defining an area containing the areas of two specified rectangles.
        /// </summary>
        /// <param name="rc1">First rectangle to be merged.</param>
        /// <param name="rc2">Second rectangle to be merged.</param>
        /// <returns>IRectangle with area containing the orriginal rectangles.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITypedBounds Hybrid(this ITypedBounds rc1, IBounds rc2)
        {
            if (rc1 == null || !rc1.Valid)
                return new UpdateArea(rc2);
            if (rc2 == null || !rc2.Valid)
                return new UpdateArea(rc1);

            int x, y, w, h;
            rc2.GetBounds(out x, out y, out w, out h);
            ObjType? type = rc1?.Type;
            if (rc2 is IType)
                type = ((IType)rc2).Type;
            return rc1.Hybrid(x, y, w, h, type);

        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITypedBounds Hybrid(this ITypedBounds rc1, int x1, int y1, int w1, int h1, ObjType? type = null)
        {
            type = type ?? rc1?.Type;

            if (rc1 == null || !rc1.Valid)
            {
                return new UpdateArea(x1, y1, w1, h1, type);
            }

            int x, y, w, h, r, b, r1, b1;
            rc1.GetBounds(out x, out y, out w, out h);
            r = x + w;
            b = y + h;
            r1 = x1 + w1;
            b1 = y1 + h1;
            if (x1 < x)
                x = x1;
            if (y1 < y)
                y = y1;
            if (r1 > r)
                r = r1;
            if (b1 > b)
                b = b1;
            return UpdateArea.FromLTRB(x, y, r, b, type);
        }

        /// <summary>
        /// Return a IRectangleF object defining an area containing the areas of two specified rectangles.
        /// </summary>
        /// <param name="areaG1">First rectangle to be merged.</param>
        /// <param name="areaG2">Second rectangle to be merged.</param>
        /// <returns>IRectangleF with area containing the orriginal rectangles.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Hybrid(this IRectangleF areaG1, IRectangleF areaG2)
        {
            if (areaG1 == null || !areaG1.Valid)
                return new RectangleF(areaG2);
            if (areaG2 == null || !areaG2.Valid)
                return new RectangleF(areaG1);

            float x, y, w, h, x1, y1, w1, h1, r, b, r1, b1;
            x = areaG1.X;
            y = areaG1.Y;
            w = areaG1.Width;
            h = areaG1.Height;

            x1 = areaG2.X;
            y1 = areaG2.Y;
            w1 = areaG2.Width;
            h1 = areaG2.Height;

            r = x + w;
            b = y + h;

            r1 = x1 + w1;
            b1 = y1 + h1;
            if (x1 < x)
                x = x1;
            if (y1 < y)
                y = y1;
            if (r1 > r)
                r = r1;
            if (b1 > b)
                b = b1;
            return RectangleF.FromLTRB(x, y, r, b);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Hybrid(this IRectangleF areaG1, float x1, float y1, float w1, float h1)
        {
            if (areaG1 == null || !areaG1.Valid)
                return new Rectangle(x1, y1, w1, h1);

            float x, y, w, h, r, b, r1, b1;
            areaG1.GetBounds(out x, out y, out w, out h);
            r = x + w;
            b = y + h;
            r1 = x1 + w1;
            b1 = y1 + h1;
            if (x1 < x)
                x = x1;
            if (y1 < y)
                y = y1;
            if (r1 > r)
                r = r1;
            if (b1 > b)
                b = b1;
            return RectangleF.FromLTRB(x, y, r, b);
        }
        #endregion

        #region ROUNDING
        /// <summary>
        /// Returns integer dimensions for the rectangle in IRectangleF provided.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding up.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <param name="x">x position of Top Left corner rounded up to an Integer..</param>
        /// <param name="y">y position of the Top Left corner rounded up to an Integer..</param>
        /// <param name="w">Width rounded up to an Integer.</param>
        /// <param name="h">Height rounded up to an Integer.</param>
        public static void Ceiling(this IRectangleF rc, out int x, out int y, out int w, out int h)
        {
            x = rc.X.Ceiling();
            y = rc.Y.Ceiling();
            w = rc.Width.Ceiling();
            h = rc.Height.Ceiling();
        }

        /// <summary>
        /// Returns integer dimensions for the rectangle in IRectangleF provided.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding to nearest integer.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <param name="x">x position of Top Left corner rounded to the nearest Integer.</param>
        /// <param name="y">y position of the Top Left corner rounded to the nearest Integer.</param>
        /// <param name="w">Width rounded to the nearest Integer.</param>
        /// <param name="h">Height rounded to the nearest Integer.</param>
        public static void Round(this IRectangleF rc, out int x, out int y, out int w, out int h)
        {
            x = rc.X.Round();
            y = rc.Y.Round();
            w = rc.Width.Round();
            h = rc.Height.Round();
        }

        /// <summary>
        /// Returns integer dimensions for the rectangle in IRectangleF provided.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <param name="x">x position of Top Left corner rounded down.</param>
        /// <param name="y">y position of the Top Left corner rounded down.</param>
        /// <param name="w">Width rounded down.</param>
        /// <param name="h">Height rounded down.</param>
        public static void Floor(this IRectangleF rc, out int x, out int y, out int w, out int h)
        {
            x = rc.X.Floor();
            y = rc.Y.Floor();
            w = rc.Width.Floor();
            h = rc.Height.Floor();
        }

        /// <summary>
        /// Returns integer dimensions for the rectangle in IRectangleF provided.
        /// Converts contained rectangle's (x,y), of top left corner, to integer values by rounding up.
        /// Converts width and height to integer dimensions by rounding down.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <param name="x">x position of Top Left corner rounded up.</param>
        /// <param name="y">y position of the Top Left corner rounded up.</param>
        /// <param name="w">Width rounded down.</param>
        /// <param name="h">Height rounded down.</param>
        public static void Shrink(this IRectangleF rc, out int x, out int y, out int w, out int h)
        {
            x = rc.X.Ceiling();
            y = rc.Y.Ceiling();
            w = rc.Width.Floor();
            h = rc.Height.Floor();
        }

        /// <summary>
        /// Returns integer dimensions for the rectangle in IRectangleF provided.
        /// Converts contained rectangle's (x,y), of top left corner, to integer values by rounding down.
        /// Converts width and height to integer dimensions by rounding up.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <param name="x">x position of Top Left corner rounded down.</param>
        /// <param name="y">y position of the Top Left corner rounded down.</param>
        /// <param name="w">Width rounded up.</param>
        /// <param name="h">Height rounded up.</param>
        public static void Expand(this IRectangleF rc, out int x, out int y, out int w, out int h)
        {
            x = rc.X.Floor();
            y = rc.Y.Floor();
            w = rc.Width.Ceiling();
            h = rc.Height.Ceiling();
        }
        #endregion

        #region INTERSECTS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Intersects(this IBounds a, IBounds b)
        {
            if (a == null || b == null)
                return false;

            if(a is IHitTestable)
            {
                IHitTestable rc = (IHitTestable)a;
                if (b is IPolygonal)
                {
                    var pts = ((IPolygonal)b).GetPoints();
                    if (pts != null)
                    {
                        foreach (var p in pts)
                        {
                            if (!p)
                                continue;
                            if (rc.Contains(p.X, p.Y))
                                return true;
                        }
                        return false;
                    }
                }
                if (b is IPolygonalF)
                {
                    var pts = ((IPolygonalF)b).GetPoints();
                    if (pts != null)
                    {
                        foreach (var p in pts)
                        {
                            if (!p)
                                continue;
                            if (rc.Contains(p.X, p.Y))
                                return true;
                        }
                        return false;
                    }
                }
            }
            if(b is IHitTestable)
            {
                IHitTestable rc = (IHitTestable)b;
                if (a is IPolygonal)
                {
                    var pts = ((IPolygonal)a).GetPoints();
                    if (pts != null)
                    {
                        foreach (var p in pts)
                        {
                            if (!p)
                                continue;
                            if (rc.Contains(p.X, p.Y))
                                return true;
                        }
                        return false;
                    }
                }
                if (a is IPolygonalF)
                {
                    var pts = ((IPolygonalF)a).GetPoints();
                    if (pts != null)
                    {
                        foreach (var p in pts)
                        {
                            if (!p)
                                continue;
                            if (rc.Contains(p.X, p.Y))
                                return true;
                        }
                        return false;
                    }
                }
            }
            if(a is IPolygonalF && b is IPolygonalF)
            {
                var pts = ((IPolygonalF)a).GetPoints();
                var other = ((IPolygonalF)b).GetPoints();
                if (pts != null && other != null)
                    return ((IPolygonalF)a).GetPoints().Contains(((IPolygonalF)b).GetPoints());
            }
            else if(a is IPolygonal && b is IPolygonal)
            {
                var pts = ((IPolygonal)a).GetPoints();
                var other = ((IPolygonal)b).GetPoints();
                if (pts != null && other != null)
                    return ((IPolygonal)a).GetPoints().Contains(((IPolygonal)b).GetPoints());
            }

            int aX, aY, aW, aH, bX, bY, bW, bH;
            a.GetBounds(out aX, out aY, out aW, out aH);
            b.GetBounds(out bX, out bY, out bW, out bH);

            var result = Intersects(aX, aY, aW, aH, bX, bY, bW, bH);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(this IBounds a, int x, int y, int w, int h)
        {
            if (a == null)
                return false;
            if(a is IHitTestable)
            {
                var rc = (IHitTestable)a;
                if (rc.Contains(x, y) ||
                        rc.Contains(x + w, y) ||
                        rc.Contains(x + w, y + h) ||
                        rc.Contains(x, y + h))
                {
                    return true;
                }
                return false;
            }
            if (a is IPolygonalF)
            {
                var pts = ((IPolygonalF)a).GetPoints();
                if (pts != null)
                {
                    return pts.Contains(x, y) ||
                        pts.Contains(x + w, y) ||
                        pts.Contains(x + w, y + h) ||
                        pts.Contains(x, y + h);
                }
            }
            if (a is IPolygonal)
            {
                var pts = ((IPolygonal)a).GetPoints();
                if (pts != null)
                {
                    return pts.Contains(x, y) ||
                        pts.Contains(x + w, y) ||
                        pts.Contains(x + w, y + h) ||
                        pts.Contains(x, y + h);
                }
            }

            int aX, aY, aW, aH;
            a.GetBounds(out aX, out aY, out aW, out aH);
            return Intersects(aX, aY, aW, aH, x, y, w, h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Intersects(this IBoundsF a, IBoundsF b)
        {
            if (a == null || b == null)
                return false;

            if (a is IHitTestable)
            {
                IHitTestable rc = (IHitTestable)a;
                if (b is IPolygonal)
                {
                    var pts = ((IPolygonal)b).GetPoints();
                    if (pts != null)
                    {
                        foreach (var p in pts)
                        {
                            if (!p)
                                continue;
                            if (rc.Contains(p.X, p.Y))
                                return true;
                        }
                        return false;
                    }
                }
                if (b is IPolygonalF)
                {
                    var pts = ((IPolygonalF)b).GetPoints();
                    if (pts != null)
                    {
                        foreach (var p in pts)
                        {
                            if (!p)
                                continue;
                            if (rc.Contains(p.X, p.Y))
                                return true;
                        }
                        return false;
                    }
                }
            }
            if (b is IHitTestable)
            {
                IHitTestable rc = (IHitTestable)b;
                if (a is IPolygonal)
                {
                    var pts = ((IPolygonal)a).GetPoints();
                    if (pts != null)
                    {
                        foreach (var p in pts)
                        {
                            if (!p)
                                continue;
                            if (rc.Contains(p.X, p.Y))
                                return true;
                        }
                        return false;
                    }
                }
                if (a is IPolygonalF)
                {
                    var pts = ((IPolygonalF)a).GetPoints();
                    if (pts != null)
                    {
                        foreach (var p in pts)
                        {
                            if (!p)
                                continue;
                            if (rc.Contains(p.X, p.Y))
                                return true;
                        }
                        return false;
                    }
                }
            }
            if (a is IPolygonalF && b is IPolygonalF)
            {
                var pts = ((IPolygonalF)a).GetPoints();
                var other = ((IPolygonalF)b).GetPoints();
                if (pts != null && other != null)
                    return ((IPolygonalF)a).GetPoints().Contains(((IPolygonalF)b).GetPoints());
            }
            else if (a is IPolygonal && b is IPolygonal)
            {
                var pts = ((IPolygonal)a).GetPoints();
                var other = ((IPolygonal)b).GetPoints();
                if (pts != null && other != null)
                    return ((IPolygonal)a).GetPoints().Contains(((IPolygonal)b).GetPoints());
            }
            float aX, aY, aW, aH, bX, bY, bW, bH;
            a.GetBounds(out aX, out aY, out aW, out aH);
            b.GetBounds(out bX, out bY, out bW, out bH);

            return Intersects(aX, aY, aW, aH, bX, bY, bW, bH);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(this IBoundsF a, float x, float y, float w, float h)
        {
            if (a == null)
                return false;
            if (a is IHitTestable)
            {
                var rc = (IHitTestable)a;
                if (rc.Contains(x, y) ||
                        rc.Contains(x + w, y) ||
                        rc.Contains(x + w, y + h) ||
                        rc.Contains(x, y + h))
                {
                    return true;
                }
                return false;
            }
            if (a is IPolygonalF)
            {
                var pts = ((IPolygonalF)a).GetPoints();
                if (pts != null)
                {
                    return pts.Contains(x, y) ||
                        pts.Contains(x + w, y) ||
                        pts.Contains(x + w, y + h) ||
                        pts.Contains(x, y + h);
                }
            }
            if (a is IPolygonal)
            {
                var pts = ((IPolygonal)a).GetPoints();
                if (pts != null)
                {
                    return pts.Contains(x, y) ||
                        pts.Contains(x + w, y) ||
                        pts.Contains(x + w, y + h) ||
                        pts.Contains(x, y + h);
                }
            }

            float aX, aY, aW, aH;
            a.GetBounds(out aX, out aY, out aW, out aH);
            return Intersects(aX, aY, aW, aH, x, y, w, h);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(int aX, int aY, int aW, int aH, int bX, int bY, int bW, int bH)
        {
            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return false;
            if (aX == bX && aY == bY && aW == bW && aH == bH)
                return true;

            bool xOverlap = aX >= bX && aX <= (bX + bW) ||
                 bX >= aX && bX <= (aX + aW);

            bool yOverlap = aY >= bY && aY <= (bY + bH) ||
                 bY >= aY && bY <= (aY + aH);

            if (!xOverlap || !yOverlap)
                return false;

            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(float aX, float aY, float aW, float aH, float bX, float bY, float bW, float bH)
        {
            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return false;

            if (aX == bX && aY == bY && aW == bW && aH == bH)
                return true;

            bool xOverlap = aX >= bX && aX <= (bX + bW) ||
                 bX >= aX && bX <= (aX + aW);

            bool yOverlap = aY >= bY && aY <= (bY + bH) ||
                 bY >= aY && bY <= (aY + aH);

            if (!xOverlap || !yOverlap)
                return false;

            return true;
        }
        #endregion

        #region INTERSECT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBounds Intersect(this IBounds first, IBounds second)
        {
            if (first == null || !first.Valid || second == null || !second.Valid)
                return Rectangle.Empty;

            int aX, aY, aW, aH, bX, bY, bW, bH, x1, y1, x2, y2;
            first.GetBounds(out aX, out aY, out aW, out aH);
            second.GetBounds(out bX, out bY, out bW, out bH);

            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return Rectangle.Empty;

            x1 = aX;
            y1 = aY;

            if (x1 < bX)
                x1 = bX;
            if (y1 < bY)
                y1 = bY;

            x2 = aX + aW;
            y2 = aY + aH;
            if (x2 > bX + bW)
                x2 = bX + bW;
            if (y2 > bY + bH)
                y2 = bY + bH;

            if (x2 >= x1 && y2 >= y1)
            {
                return UpdateArea.FromLTRB(first, x1, y1, x2, y2);
            }
            return Rectangle.Empty;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Intersect(this IRectangleF first, IRectangleF second)
        {
            if (first == null || !first.Valid || second == null || !second.Valid)
                return RectangleF.Empty;

            float aX, aY, aW, aH, bX, bY, bW, bH, x1, y1, x2, y2;
            first.GetBounds(out aX, out aY, out aW, out aH);
            second.GetBounds(out bX, out bY, out bW, out bH);

            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return RectangleF.Empty;

            x1 = aX;
            y1 = aY;

            if (x1 < bX)
                x1 = bX;
            if (y1 < bY)
                y1 = bY;

            x2 = aX + aW;
            y2 = aY + aH;
            if (x2 > bX + bW)
                x2 = bX + bW;
            if (y2 > bY + bH)
                y2 = bY + bH;

            if (x2 >= x1 && y2 >= y1)
            {
                return RectangleF.FromLTRB(x1, y1, x2 - x1, y2 - y1);
            }
            return RectangleF.Empty;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBounds Intersect(this IBounds first, IBounds second, int expand)
        {
            if (first == null || !first.Valid || second == null || !second.Valid)
                return Rectangle.Empty;

            int aX, aY, aW, aH, bX, bY, bW, bH, x1, y1, x2, y2;
            first.GetBounds(out aX, out aY, out aW, out aH);
            second.GetBounds(out bX, out bY, out bW, out bH);

            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return Rectangle.Empty;

            x1 = aX;
            y1 = aY;

            if (x1 < bX)
                x1 = bX;
            if (y1 < bY)
                y1 = bY;

            x2 = aX + aW;
            y2 = aY + aH;
            if (x2 > bX + bW)
                x2 = bX + bW;
            if (y2 > bY + bH)
                y2 = bY + bH;

            if (x2 >= x1 && y2 >= y1)
            {
                x1 -= expand;
                y1 -= expand;
                if (x1 < 0)
                    x1 = 0;
                if (y1 < 0)
                    y1 = 0;
                x2 += expand;
                y2 += expand;
                return UpdateArea.FromLTRB(first, x1, y1, x2, y2);
            }
            return Rectangle.Empty;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Intersect(this IBounds first, IBounds second, 
            out int x, out int y, out int w, out int h)
        {
            x = w = y = h = 0;
            if (first == null || !first.Valid || second == null || !second.Valid)
                return;

            int aX, aY, aW, aH, bX, bY, bW, bH;
            first.GetBounds(out aX, out aY, out aW, out aH);
            second.GetBounds(out bX, out bY, out bW, out bH);

            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return;

            x = aX;
            y = aY;

            if (x < bX)
                x = bX;
            if (y < bY)
                y = bY;

            w = aX + aW;
            h = aY + aH;
            if (w > bX + bW)
                w = bX + bW;
            if (h > bY + bH)
                h = bY + bH;

            if (w >= x && h >= y)
            {
                w -= x;
                h -= y;
                return;
            }
            x = w = y = h = 0;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Intersect(this IBounds first, IBounds second, int expand, 
            out int x, out int y, out int w, out int h)
        {
            x = w = y = h = 0;
            if (first == null || !first.Valid || second == null || !second.Valid)
                return;

            int aX, aY, aW, aH, bX, bY, bW, bH;
            first.GetBounds(out aX, out aY, out aW, out aH);
            second.GetBounds(out bX, out bY, out bW, out bH);

            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return;

            x = aX;
            y = aY;

            if (x < bX)
                x = bX;
            if (y < bY)
                y = bY;

            w = aX + aW;
            h = aY + aH;
            if (w > bX + bW)
                w = bX + bW;
            if (h > bY + bH)
                h = bY + bH;

            if (w >= x && h >= y)
            {
                x -= expand;
                y -= expand;
                if (x < 0)
                    x = 0;
                if (y < 0)
                    y = 0;
                w += expand;
                h += expand;

                w -= x;
                h -= y;
                return;
            }
            x = w = y = h = 0;
        }
        #endregion

        #region CONTAINS OTHER RECT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this IBounds parent, IBounds child)
        {
            if (parent == null || child == null || !parent.Valid || !child.Valid)
                return false;
            int parentX, parentY, parentW, parentH, childX, childY, childW, childH;
            parent.GetBounds(out parentX, out parentY, out parentW, out parentH);
            parent.GetBounds(out childX, out childY, out childW, out childH);

            var chr = childX + childW;
            var chb = childY + childH;
            var par = parentX + parentW;
            var pab = parentY + parentH;

            return childX >= parentX && childY >= parentY && chr <= par && chb <= pab;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this IRectangleF parent, IRectangleF child)
        {
            var chr = child.X + child.Width;
            var chb = child.Y + child.Height;
            var par = parent.X + parent.Width;
            var pab = parent.Y + parent.Height;

            return child.X >= parent.X && child.Y >= parent.Y && chr <= par && chb <= pab;
        }
        #endregion

        #region MIN MAX 
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(this IEnumerable<IBounds> collection, ref int minX, ref int minY, 
            ref int maxX, ref int maxY)
        {
            ResetMinMax(out minX, out minY, out maxX, out maxY);

            foreach (var p in collection)
                p.CorrectMinMax(ref minX, ref minY, ref maxX, ref maxY);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectMinMax(int x, int y, ref int minX, ref int minY, ref int maxX, ref int maxY)
        {
            if (x < minX)
                minX = x;
            if (y < minY)
                minY = y;
            if (x > maxX)
                maxX = x;
            if (y > maxY)
                maxY = y;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectMinMax(this IBounds rc, ref int minX, ref int minY, ref int maxX, ref int maxY)
        {
            if (rc == null || !rc.Valid)
                return;
            int x, y, w, h;
            rc.GetBounds(out x, out y, out w, out h);

            CorrectMinMax(x, y, ref minX, ref minY, ref maxX, ref maxY);
            CorrectMinMax(x + w, y + h, ref minX, ref minY, ref maxX, ref maxY);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetMinMax(out int minX, out int minY, out int maxX, out int maxY)
        {
            minX = minY = int.MaxValue;
            maxX = maxY = 0;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(this IEnumerable<IRectangleF> collection, ref float minX, ref float minY, ref float maxX, ref float maxY) 
        {
            ResetMinMax(out minX, out minY, out maxX, out maxY);

            foreach (var p in collection)
                p.CorrectMinMax(ref minX, ref minY, ref maxX, ref maxY);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectMinMax(float x, float y, ref float minX, ref float minY, ref float maxX, ref float maxY)
        {
            if (x < minX)
                minX = x;
            if (y < minY)
                minY = y;
            if (x > maxX)
                maxX = x;
            if (y > maxY)
                maxY = y;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectMinMax(this IRectangleF rc, ref float minX, ref float minY, ref float maxX, ref float maxY)
        {
            if (rc == null || !rc.Valid)
                return;
            float x = rc.X, y = rc.Y, w = rc.Width, h = rc.Height;
            CorrectMinMax(x, y, ref minX, ref minY, ref maxX, ref maxY);
            CorrectMinMax(x + w, y + h, ref minX, ref minY, ref maxX, ref maxY);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetMinMax(out float minX, out float minY, out float maxX, out float maxY)
        {
            minX = minY = int.MaxValue;
            maxX = maxY = 0;
        }
        #endregion

        #region SCALE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Scale(this IRectangleF bounds, IScale scale, IPointF center = null)
        {
            if (bounds == null || !bounds.Valid)
                return RectangleF.Empty;

            if (scale == null || !scale.HasScale)
                return new RectangleF(bounds);

            var sx = scale.X;
            var sy = scale.Y;
            var c = center ?? bounds.Center();
            var x1 = bounds.X;
            var y1 = bounds.Y;
            var r1 = x1 + bounds.Width;
            var b1 = y1 + bounds.Height;

            Vectors.Scale(x1, y1, sx, sy, c.X, c.Y, out float x, out float y);
            Vectors.Scale(r1, b1, sx, sy, c.X, c.Y, out float r, out float b);
            return RectangleF.FromLTRB(x, y, r, b);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle Scale(this IBounds bounds, IScale scale, IPointF center = null)
        {
            if (bounds == null || !bounds.Valid)
                return Rectangle.Empty;
            if (scale == null || !scale.HasScale)
                return new Rectangle(bounds);

            var sx = scale.X;
            var sy = scale.Y;
            float x, y, r, b;
            int x1, y1, w1, h1, r1, b1;
            var c = center ?? bounds.Center();
            bounds.GetBounds(out x1, out y1, out w1, out h1);
            r1 = x1 + w1;
            b1 = y1 + h1;

            Vectors.Scale(x1, y1, sx, sy, c.X, c.Y, out x, out y);
            Vectors.Scale(r1, b1, sx, sy, c.X, c.Y, out r, out b);
            return Rectangle.FromLTRB(x, y, r, b);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Scale(float X, float Y, float Width, float Height,
            IScale scale, IPointF center = null)
        {
            if (Width <= 0 && Height <= 0)
                return RectangleF.Empty;
            if (scale == null || !scale.HasScale)
                return new RectangleF(X, Y, Width, Height);

            var sx = scale.X;
            var sy = scale.Y;
            float r1 = X + Width, b1 = Y + Height;
            var c = center ?? new VectorF(X + Width / 2f, Y + Height / 2f);

            Vectors.Scale(X, Y, sx, sy, c.X, c.Y, out float x, out float y);
            Vectors.Scale(r1, b1, sx, sy, c.X, c.Y, out float r, out float b);
            return RectangleF.FromLTRB(x, y, r, b);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle Scale(int X, int Y, int Width, int Height, IScale scale, IPointF center = null)
        {
            if (Width <= 0 && Height <= 0)
                return Rectangle.Empty;
            if (scale == null || !scale.HasScale)
                return new Rectangle(X, Y, Width, Height);

            var sx = scale.X;
            var sy = scale.Y;
            float r1 = X + Width, b1 = Y + Height;
            var c = center ?? new VectorF(X + Width / 2f, Y + Height / 2f);

            Vectors.Scale(X, Y, sx, sy, c.X, c.Y, out float x, out float y);
            Vectors.Scale(r1, b1, sx, sy, c.X, c.Y, out float r, out float b);
            return Rectangle.FromLTRB(x, y, r, b);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Scale(this IRectangleF rect, IRotation rotation, IScale scale, IPointF center = null)
        {
            return Scale(rect.X, rect.Y, rect.Width, rect.Height, rotation, scale, center);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle Scale(this IBounds rect, IRotation rotation, IScale scale, IPointF center = null)
        {
            rect.GetBounds(out int x, out int y, out int w, out int h);
            return Scale(x, y, w, h, rotation, scale, center);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Scale(float X, float Y, float Width, float Height, IRotation rotation,
            IScale scale, IPointF center = null)
        {
            float l = X, t = Y, r = X + Width, b = Y + Height;

            bool HasScale = scale != null && scale.HasScale;
            bool HasSkewScale = rotation?.Skew != null && rotation?.Skew?.HasScale == true;
            if (!HasScale && !HasSkewScale)
                goto Exit;

            float CX = center?.X ?? X + Width / 2f;
            float CY = center?.Y ?? Y + Height / 2f;

            if (HasScale)
            {
                var sx = scale.X;
                var sy = scale.Y;
                l = (l - CX) * sx + CX;
                t = (t - CY) * sy + CY;
                r = (r - CX) * sx + CX;
                b = (b - CY) * sy + CY;
            }
            if (HasSkewScale)
            {
                rotation?.EffectiveCenter(CX, CY, out CX, out CY);
                var skewX = rotation.Skew.X;
                var skewY = rotation.Skew.Y;

                l = (l - CX) * skewX + CX;
                t = (t - CY) * skewY + CY;
                r = (r - CX) * skewX + CX;
                b = (b - CY) * skewY + CY;
            }
        Exit:
            return RectangleF.FromLTRB(l, t, r, b);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle Scale(int X, int Y, int Width, int Height,
            IRotation rotation, IScale scale, IPointF center = null)
        {
            int l = X;
            int t = Y;
            int r = X + Width;
            int b = Y + Height;

            bool HasScale = scale != null && scale.HasScale;
            bool HasSkewScale = rotation?.Skew != null && rotation?.Skew?.HasScale == true;
            if (!HasScale && !HasSkewScale)
                goto Exit;

            float Cx = center?.X ?? X + Width / 2f;
            float Cy = center?.Y ?? Y + Height / 2f;
            int CX = Cx.Round();
            int CY = Cy.Round();

            if (HasScale)
            {
                var sx = scale.X;
                var sy = scale.Y;
                l = (int)((l - CX) * sx + CX);
                t = (int)((t - CY) * sy + CY);
                r = (int)((r - CX) * sx + CX);
                b = (int)((b - CY) * sy + CY);
            }

            if (HasSkewScale)
            {
                rotation?.EffectiveCenter(CX, CY, out CX, out CY);

                var skewX = rotation.Skew.X;
                var skewY = rotation.Skew.Y;
                l = (int)((l - CX) * skewX + CX);
                t = (int)((t - CY) * skewY + CY);
                r = (int)((r - CX) * skewX + CX);
                b = (int)((b - CY) * skewY + CY);
            }
        Exit:
            return Rectangle.FromLTRB(l, t, r, b);
        }
        #endregion

        #region ROTATE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<VectorF> Rotate(float x, float y, float w, float h, IDegree degree, IPointF center = null, bool noSkew = false)
        {
            var p = new VectorF[4];
            p[0] = new VectorF(x, y);
            p[1] = new VectorF(x, y + h);
            p[2] = new VectorF(x + w, y + h);
            p[3] = new VectorF(x + w, y);

            if (degree == null || !degree.Valid)
                return p;

            IPointF c = center ?? new VectorF(x + w / 2f, y + h / 2f);
            return p.Rotate(degree, center: c, noSkew: noSkew);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector> Rotate(int x, int y, int w, int h, IDegree degree, IPointF center = null, bool noSkew = false)
        {
            var pts = new Vector[4];
            pts[0] = new Vector(x, y);
            pts[1] = new Vector(x, y + h);
            pts[2] = new Vector(x + w, y + h);
            pts[3] = new Vector(x + w, y);

            if (degree == null || !degree.Valid)
                return pts;
            IPointF c = center ?? new VectorF(x + w / 2f, y + h / 2f);
            float cx = c.X;
            float cy = c.Y;
            int Cx = cx.Round();
            int Cy = cy.Round();
            return pts.Rotate(degree,  false, new VectorF(Cx, Cy), noSkew: noSkew);
        }

        public static IEnumerable<VectorF> Rotate(this IBounds bounds, IDegree degree, IPointF center = null, bool noSkew = false)
        {
            if (bounds == null)
                return null;
            bounds.GetBounds(out int X, out int Y, out int Width, out int Height);
            if (degree == null || !degree.Valid)
            {
                 return new VectorF[]
                 {
                    new VectorF(X, Y),
                    new VectorF(X + Width, Y),
                    new VectorF(X + Width, Y + Height),
                    new VectorF(X, Y + Height)
                 };
            }
            if (bounds is IPolygonalF)
            {
                var p = ((IPolygonalF)bounds).GetPoints();

                var c = center?? bounds.Center();
                if (degree is ICentreHolder && ((ICentreHolder)degree)?.Centre != null)
                    c = ((ICentreHolder)degree).Centre;
                return p.Rotate(degree, center: c, noSkew: noSkew);
            }
            else if (bounds is IEnumerable<VectorF>)
            {
                var p = ((IEnumerable<VectorF>)bounds);
                var c = center ?? bounds.Center();
                if (degree is ICentreHolder && ((ICentreHolder)degree)?.Centre != null)
                    c = ((ICentreHolder)degree).Centre;
                return p.Rotate(degree, center: c, noSkew: noSkew);
            }
            return Rotate((float)X, Y, Width, Height, degree, center, noSkew: noSkew);
        }

        public static IEnumerable<VectorF> Rotate(this IRectangleF bounds, IDegree degree, IPointF center = null, bool noSkew = false)
        {
            if (bounds == null)
                return null;

            bounds.GetBounds(out float X, out float Y, out float Width, out float Height);
            if (degree == null || !degree.Valid)
            {
                return new VectorF[]
                {
                    new VectorF(X, Y),
                    new VectorF(X + Width, Y),
                    new VectorF(X + Width, Y + Height),
                    new VectorF(X, Y + Height)
                };
            }
            if (bounds is IPolygonalF)
            {
                var p = ((IPolygonalF)bounds).GetPoints();
                var c = center ?? bounds.Center();
                if (degree is ICentreHolder && ((ICentreHolder)degree)?.Centre != null)
                    c = ((ICentreHolder)degree).Centre;
                return p.Rotate(degree, center: c, noSkew: noSkew);
            }
            else if (bounds is IEnumerable<VectorF>)
            {
                var p = ((IEnumerable<VectorF>)bounds);
                var c = center ?? bounds.Center();
                if (degree is ICentreHolder && ((ICentreHolder)degree)?.Centre != null)
                    c = ((ICentreHolder)degree).Centre;
                return p.Rotate(degree, center: c, noSkew: noSkew);
            }
            return Rotate(X, Y, Width, Height, degree, center, noSkew: noSkew);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IEnumerable<VectorF> Rotate(float x, float y, float w, float h, float degree, IPointF center = null, 
            bool AntiClock = false, ISkew Skew = null, SkewType? skewType = null, float? skewDegree = null)
        {
            var pts = new VectorF[4];
            pts[0] = new VectorF(x, y);
            pts[1] = new VectorF(x, y + h);
            pts[2] = new VectorF(x + w, y + h);
            pts[3] = new VectorF(x + w, y);
            IPointF c = center ?? new VectorF(x + w / 2f, y + h / 2f);
            return pts.Rotate(degree, AntiClock, Skew, skewType, skewDegree, c);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IEnumerable<VectorF> Rotate(this IBounds bounds, float degree, IPointF center = null, 
            bool antiClock = false, ISkew Skew = null, SkewType? skewType = null, float? skewDegree = null)
        {
            bounds.GetBounds(out int x, out int y, out int w, out int h);
            return Rotate(x, y, w, h, degree, center, antiClock, Skew, skewType, skewDegree);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IEnumerable<VectorF> Rotate(this IBoundsF bounds, float degree, IPointF center = null, 
            bool antiClock = false, ISkew Skew = null, SkewType? skewType = null, float? skewDegree = null)
        {
            bounds.GetBounds(out float x, out float y, out float w, out float h);
            return Rotate(x, y, w, h, degree, center, antiClock, Skew, skewType, skewDegree);
        }
        #endregion

        #region GET STROKE AREAS
        /// <summary>
        /// Returns 2 IRectangles that define the area of the inside or outside of the outline with stroke applied. 
        /// </summary>
        /// <param name="x">x position of Top Left of rectangle containing the curve without stroke.</param>
        /// <param name="y">y position of Top Left of rectangle containing the curve without stroke.</param>
        /// <param name="w">Width of rectangle containing the curve without stroke.</param>
        /// <param name="h">Width  of rectangle containing the curve without stroke.</param>
        /// <param name="outer">IRectangle returned with dimensions that contain the Outer edge of the Stroke.</param>
        /// <param name="inner">IRectangle returned with dimensions that contain the Inner edge of the Stroke.</param>
        /// <param name="stroke">Stroke size.</param>
        /// <param name="Command">Enum defining stroke position in relation to the curve without stroke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetStrokeAreas(float x, float y, float w, float h, out RectangleF outer,
            out RectangleF inner, float stroke, FillCommand Command = 0)
        {
            outer = inner = new RectangleF(x, y, w, h);
            var r = inner.Right;
            var b = inner.Bottom;

            float s;
            bool StrokeInner = (Command & FillCommand.StrokeInner) == FillCommand.StrokeInner;
            bool StrokeOuter = (Command & FillCommand.StrokeOuter) == FillCommand.StrokeOuter && !StrokeInner;

            if (StrokeOuter || StrokeInner)
                s = stroke;
            else
                s = (stroke / 2f);

            if (StrokeOuter)
                outer = RectangleF.FromLTRB(x - s, y - s, r + s, b + s);
            else if(StrokeInner)
                inner = RectangleF.FromLTRB(x + s, y + s, r - s, b - s);
            else
            {
                outer = RectangleF.FromLTRB(x - s, y - s, r + s, b + s);
                inner = RectangleF.FromLTRB(x + s, y + s, r - s, b - s);
            }
        }

        /// <summary>
        /// Returns 2 IRectangleFs that define the area of the inside or outside of the outline with stroke applied. 
        /// </summary>
        /// <param name="area">Area occupied by the shape.</param>
        /// <param name="outerArea">IRectangleF returned with dimensions that contain the Outer edge of the Stroke.</param>
        /// <param name="innerArea">IRectangleF returned with dimensions that contain the Inner edge of the Stroke.</param>
        /// <param name="stroke">Stroke size.</param>
        /// <param name="mode">Enum defining stroke position in relation to the curve without stroke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetStrokeAreas(this IObject area, out RectangleF outerArea, out RectangleF innerArea,
            float stroke, FillCommand mode = 0)
        {
            int x, y, w, h;
            area.ToArea(out x, out y, out w, out h);
            GetStrokeAreas(x, y, w, h, out outerArea, out innerArea, stroke, mode);
        }

        /// <summary>
        /// Returns 2 IRectangles that define the area of the inside or outside of the outline with stroke applied. 
        /// </summary>
        /// <param name="x">x position of Top Left of rectangle containing the curve without stroke.</param>
        /// <param name="y">y position of Top Left of rectangle containing the curve without stroke.</param>
        /// <param name="w">Width of rectangle containing the curve without stroke.</param>
        /// <param name="h">Width  of rectangle containing the curve without stroke.</param>
        /// <param name="outer">IRectangle returned with dimensions that contain the Outer edge of the Stroke.</param>
        /// <param name="inner">IRectangle returned with dimensions that contain the Inner edge of the Stroke.</param>
        /// <param name="stroke">Stroke size.</param>
        /// <param name="Command">Enum defining stroke position in relation to the curve without stroke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetStrokeAreas(int x, int y, int w, int h, out Rectangle outer,
            out Rectangle inner, float stroke, FillCommand Command = 0)
        {
            outer = inner = new Rectangle(x, y, w, h);
            var r = inner.Right;
            var b = inner.Bottom;

            float s;
            bool StrokeInner = (Command & FillCommand.StrokeInner) == FillCommand.StrokeInner;
            bool StrokeOuter = (Command & FillCommand.StrokeOuter) == FillCommand.StrokeOuter && !StrokeInner;

            if (StrokeOuter || StrokeInner)
                s = stroke;
            else
                s = (stroke / 2f);

            if (StrokeOuter)
                outer = Rectangle.FromLTRB(x - s, y - s, r + s, b + s);
            else if (StrokeInner)
                inner = Rectangle.FromLTRB(x + s, y + s, r - s, b - s);
            else
            {
                outer = Rectangle.FromLTRB(x - s, y - s, r + s, b + s);
                inner = Rectangle.FromLTRB(x + s, y + s, r - s, b - s);
            }
        }

        /// <summary>
        /// Returns 2 IRectangleFs that define the area of the inside or outside of the outline with stroke applied. 
        /// </summary>
        /// <param name="area">Area occupied by the shape.</param>
        /// <param name="outerArea">IRectangleF returned with dimensions that contain the Outer edge of the Stroke.</param>
        /// <param name="innerArea">IRectangleF returned with dimensions that contain the Inner edge of the Stroke.</param>
        /// <param name="stroke">Stroke size.</param>
        /// <param name="mode">Enum defining stroke position in relation to the curve without stroke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetStrokeAreas(this IObject area, out Rectangle outerArea, out Rectangle innerArea,
            float stroke, FillCommand mode = 0)
        {
            int x, y, w, h;
            area.ToArea(out x, out y, out w, out h);
            GetStrokeAreas(x, y, w, h, out outerArea, out innerArea, stroke, mode);
        }
        #endregion

        #region GET OUT LINE AREAS
        /// <summary>
        /// Returns an array of four rectangles each represents stroke side in the following order:
        /// Left, Top, Right, Bottom.
        /// </summary>
        /// <param name="outer">Outer rectangle as parent.</param>
        /// <param name="inner">Inner rectange as child.</param>
        /// <returns>RectF[] with four elements.</returns>
        public static RectangleF[] GetOutLineAreas(this IRectangleF outer, IRectangleF inner)
        {
            float ix = inner.X, iy = inner.Y, iw = inner.Width, ih = inner.Height,
                ox = outer.X, oy = outer.Y, ow = outer.Width, oh = outer.Height;

            float ib = iy + ih;
            float ob = oy + oh;
            float ir = ix + iw;
            float or = ox + ow;

            RectangleF[] r = new RectangleF[4];
            r[0] = RectangleF.FromLTRB(ox, iy, ix, ib); //left
            r[1] = RectangleF.FromLTRB(ox, oy, or, iy); //top
            r[2] = RectangleF.FromLTRB(ir, iy, or, ib); //right
            r[3] = RectangleF.FromLTRB(ox, ib, or, ob); //right
            return r;
        }
        #endregion

        #region HYBRID BOUNDS
        /// <summary>
        /// Creates a IRectangleF which defines a rectangle containing the two areas defined by the bounds on the two list of points supplied..
        /// </summary>
        /// <param name="c1">List of points defining one area.</param>
        /// <param name="c2">List of point defining a second area.</param>
        /// <returns>IRectangleF defining a rectangle that contains both sets of points.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF HybridBounds<T>(this IEnumerable<T> c1, IEnumerable<T> c2)
            where T: IPointF =>
            c1.ToArea().Hybrid(c2.ToArea());

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF HybridBounds(this ILine c1, ILine c2)
        {
            float minX, minY, maxX, maxY;
            Vectors.MinMax(out minX, out minY, out maxX, out maxY,
                new VectorF(c1.X1, c1.Y1),
                new VectorF(c1.X2, c1.Y2),
                       new VectorF(c2.X1, c2.Y1),
                new VectorF(c2.X2, c2.Y2));
            return RectangleF.FromLTRB(minX, minY, maxX, maxY);
        }
        #endregion

        #region CHANGE
        /// <summary>
        /// Convert IRectangle with integer dimensions to IRectangleF with floating point dimension.
        /// </summary>
        /// <param name="r">IRectangle to convert.</param>
        /// <returns></returns>
        public static RectangleF ToRectF(this IBounds r) =>
            new RectangleF(r);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding up.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Ceiling(this IRectangleF r)
        {
            return new Rectangle(r.X.Ceiling(), r.Y.Ceiling(), r.Width.Ceiling(), r.Height.Ceiling());
        }

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding to nearest integer.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Round(this IRectangleF r)
        {
            return new Rectangle(r.X.Round(), r.Y.Round(), r.Width.Round(), r.Height.Round());
        }

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Floor(this IRectangleF r)
        {
            return new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding up the (x,y) of the top left corner 
        /// and rounding down the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Shrink(this IRectangleF r)
        {
            return new Rectangle(r.X.Ceiling(), r.Y.Ceiling(), (int)r.Width, (int)r.Height);
        }


        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Expand(this IBoundsF r) =>
            new Rectangle(r);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Expand(this IRectangleF r, int offSet)
        {
            float x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            return new Rectangle(x - offSet, y - offSet, w + offSet * 2, h + offSet * 2);
        }

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static RectangleF Expand(this IBoundsF rc, float l = 0, float t = 0, float r = 0, float b = 0)
        {
            float x, y, w, h;
            rc.GetBounds(out x, out y, out w, out h);
            return new RectangleF(x - l, y - t, w + r, h + b);
        }

        /// <summary>
        /// Returns expanded version of the given bounds.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="bounds">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle Expand(this IBounds bounds, int offSet)
        {
            int x, y, w, h;
            bounds.GetBounds(out x, out y, out w, out h);
            x -= offSet;
            y -= offSet;
            w += offSet * 2;
            h += offSet * 2;
            if (bounds is IType)
                return new UpdateArea(x, y, w, h, ((IType)bounds).Type);
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle Expand(this IBounds rc, int l = 0, int t = 0, int r = 0, int b = 0)
        {
            int x, y, w, h;
            rc.GetBounds(out x, out y, out w, out h);
            x -= l;
            y -= t;
            w += r;
            h += b;

            if (rc is IType)
                return new UpdateArea(x, y, w, h, ((IType)rc).Type);
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Returns expanded version of the given bounds.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="bounds">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITypedBounds Expand(this ITypedBounds bounds, int offSet)
        {
            int x, y, w, h;
            bounds.GetBounds(out x, out y, out w, out h);
            x -= offSet;
            y -= offSet;
            w += offSet * 2;
            h += offSet * 2;
            return new UpdateArea(x, y, w, h, bounds.Type);
        }

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITypedBounds Expand(this ITypedBounds rc, int l = 0, int t = 0, int r = 0, int b = 0)
        {
            int x, y, w, h;
            rc.GetBounds(out x, out y, out w, out h);
            x -= l;
            y -= t;
            w += r;
            h += b;
            return new UpdateArea(x, y, w, h, rc.Type);
        }
        #endregion

        #region EXCHANGE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExchangeLocation(ref Rectangle rc1, ref Rectangle rc2)
        {
            var x = rc2.X;
            var y=rc2.Y;

            rc2.X = rc1.X;
            rc2.Y = rc1.Y;

            rc1.X = x;
            rc1.Y = y;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExchangeSize(ref Rectangle rc1, ref Rectangle rc2)
        {
            var w = rc2.Width;
            var h = rc2.Height;

            rc2.Width = rc1.Width;
            rc2.Height = rc1.Height;

            rc1.Width = w;
            rc1.Height = h;
        }
        #endregion

        #region PROXIMITY - INTERSECT
        public static bool Proximity(this IBounds rect, Vector p, out Vector distancePoint)
        {
            bool ok = rect.Contains(p.X, p.Y);
            if (ok)
            {
                int x, y, w, h;
                rect.GetBounds(out x, out y, out w, out h);
                distancePoint = new Vector(x + w - p.X, y + h - p.Y);
            }
            else
                distancePoint = new Vector();
            return ok;
        }
        public static bool Proximity(this IRectangleF r, VectorF p, out VectorF distancePoint)
        {
            bool ok = r.Contains(p.X, p.Y);
            if (ok)
            {
                distancePoint = new VectorF(r.X + r.Width - p.X, r.Y + r.Height - p.Y);
            }
            else
                distancePoint = new VectorF();
            return ok;
        }
        #endregion

        #region FROM LTRB
        public static RectangleF FromLTRB(float left, float top, float right, float bottom, bool correct = true)
        {
            if (correct)
            {
                Numbers.Order(ref left, ref right);
                Numbers.Order(ref top, ref bottom);
            }
            var w = right - left;
            if (w == 0)
                w = 1;
            var h = bottom - top;
            if (h == 0)
                h = 1;
            return new RectangleF(left, top, w, h);
        }

        public static Rectangle FromLTRB(int left, int top, int right, int bottom, bool correct = true)
        {
            if (correct)
            {
                Numbers.Order(ref left, ref right);
                Numbers.Order(ref top, ref bottom);
            }
            var w = right - left;
            if (w == 0)
                w = 1;
            var h = bottom - top;
            if (h == 0)
                h = 1;
            return new Rectangle(left, top, w, h);
        }
        #endregion

        #region CLAMP
        public static RectangleF Clamp(this IRectangleF rect, Size max) =>
            Clamp(rect, max.Width, max.Height);
        public static RectangleF Clamp(this IRectangleF rect, float width, float height)
        {
            if (rect == null || !rect.Valid)
                return RectangleF.Empty;

            float x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height;

            if (width == 0)
                width = w;
            if (height == 0)
                height = h;

            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            if (w > width)
                w = width;
            if (h > height)
                h = height;
            return new RectangleF(x, y, w, h);
        }
        public static Rectangle Clamp(this IBounds rect, int width, int height)
        {
            if (rect == null || !rect.Valid)
                return Rectangle.Empty;

            int x, y, w, h;
            rect.GetBounds(out x, out y, out w, out h);

            if (width == 0)
                width = w;
            if (height == 0)
                height = h;

            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            if (w > width)
                w = width;
            if (h > height)
                h = height;
            return new Rectangle(x, y, w, h);
        }
        public static Rectangle Clamp(this IBounds rect, Size max) =>
            Clamp(rect, max.Width, max.Height);
        #endregion

        #region INFLATE
        public static Rectangle Inflate(this IBounds rect, int xUnit, int yUnit)
        {
            return Inflate(rect, xUnit, yUnit, xUnit, yUnit);
        }

        public static Rectangle Inflate(this IBounds rect, int xUnit, int yUnit, int rUnit, int bUnit)
        {
            int x, y, w, h;
            rect.GetBounds(out x, out y, out w, out h);

            x -= xUnit;
            y -= yUnit;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            var r = (x + w) + rUnit;
            var b = (y + h) + bUnit;

            return Rectangle.FromLTRB(x, y, r, b);
        }

        public static RectangleF Inflate(this IRectangleF rect, int xUnit, int yUnit)
        {
            return Inflate(rect, xUnit, yUnit, xUnit, yUnit);
        }

        public static RectangleF Inflate(this IRectangleF rect, int xUnit, int yUnit, int rUnit, int bUnit)
        {
            float x = rect.X, y = rect.Y, w = rect.Width, h = rect.Height;

            x -= xUnit;
            y -= yUnit;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            var r = (x + w) + rUnit;
            var b = (y + h) + bUnit;

            return RectangleF.FromLTRB(x, y, r, b);
        }
        #endregion

        #region NEIGHBOUR
        /// <summary>
        /// Neighbours the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="size">The size.</param>
        /// <param name="ignoreNegativePoints">if set to <c>true</c> [ignore negative points].</param>
        /// <returns>Rectangle.</returns>
        public static Rectangle Neighbour(this IBounds source, int increment = 1,
            RectMovement direction = RectMovement.Right, int? size = null, bool ignoreNegativePoints = true)
        {
            Rectangle result;
            switch (direction)
            {
                case RectMovement.Left:
                    result = source.Previous(increment, Direction.Horizontal, ignoreNegativePoints, size);
                    return result;
                case RectMovement.Up:
                    result = source.Previous(increment, Direction.Vertical, ignoreNegativePoints, size);
                    return result;
                case RectMovement.Right:
                    result = source.Next(increment, Direction.Horizontal, size);
                    return result;
            }
            result = source.Next(increment, Direction.Vertical, size);
            return result;
        }

        /// <summary>
        /// Neighbours the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="size">The size.</param>
        /// <param name="ignoreNegativePoints">if set to <c>true</c> [ignore negative points].</param>
        /// <returns>RectangleF.</returns>
        public static RectangleF Neighbour(this IBoundsF source, float increment = 1f,
            RectMovement direction = RectMovement.Right, float? size = null, bool ignoreNegativePoints = true)
        {
            RectangleF result;
            switch (direction)
            {
                case RectMovement.Left:
                    result = source.Previous(increment, Direction.Horizontal, ignoreNegativePoints, size);
                    return result;
                case RectMovement.Up:
                    result = source.Previous(increment, Direction.Vertical, ignoreNegativePoints, size);
                    return result;
                case RectMovement.Right:
                    result = source.Next(increment, Direction.Horizontal, size);
                    return result;
            }
            result = source.Next(increment, Direction.Vertical, size);
            return result;
        }

        /// <summary>
        /// Neighbours the series.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="boundary">The boundary.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="alternateSize">Size of the alternate.</param>
        /// <param name="itemCount">The item count.</param>
        /// <param name="exhaustByCount">if set to <c>true</c> [exhaust by count].</param>
        /// <returns>MnMCollection&lt;RectangleF&gt;.</returns>
        public static PrimitiveList<RectangleF> NeighbourSeries(this IBoundsF source,
            float boundary, float increment = 1f, RectMovement direction = RectMovement.Right,
            float alternateSize = 0f, int itemCount = -1, bool exhaustByCount = false)
        {
            PrimitiveList<RectangleF> result;
            switch (direction)
            {
                case RectMovement.Left:
                    result = source.PreviousSeries(boundary, increment,
                        Direction.Horizontal, alternateSize, itemCount, exhaustByCount);
                    return result;
                case RectMovement.Up:
                    result = source.PreviousSeries(boundary, increment,
                        Direction.Vertical, alternateSize, itemCount, exhaustByCount);
                    return result;
                case RectMovement.Right:
                    result = source.NextSeries(boundary, increment,
                        Direction.Horizontal, alternateSize, itemCount, exhaustByCount);
                    return result;
                case RectMovement.Down:
                default:
                    result = source.NextSeries(boundary, increment,
                        Direction.Vertical, alternateSize, itemCount, exhaustByCount);
                    return result;
            }
        }

        /// <summary>
        /// Neighbours the specified shift.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <param name="shift">The shift.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        /// <returns>RectMovement.</returns>
        internal static RectMovement Neighbour(this Direction orientation,
            PointShift shift = PointShift.Forward, bool invert = false)
        {
            RectMovement result;

            if (invert)
                orientation = (Direction)Math.Abs((int)orientation - 1);

            switch (shift)
            {
                case PointShift.Backward:
                    if (orientation == Direction.Horizontal)
                    {
                        result = RectMovement.Left;
                        return result;
                    }
                    result = RectMovement.Up;
                    return result;
            }
            if (orientation == Direction.Horizontal)
                result = RectMovement.Right;
            else
                result = RectMovement.Down;
            return result;
        }


        /// <summary>
        /// Neighbours the specified shift.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <param name="shift">The shift.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        /// <returns>RectMovement.</returns>
        internal static RectMovement Neighbour(this ItemSpread orientation,
            PointShift shift = PointShift.Forward, bool invert = false)
        {
            RectMovement result;

            if (invert)
                orientation = (ItemSpread)Math.Abs((int)orientation - 1);

            switch (shift)
            {
                case PointShift.Backward:
                    if (orientation == ItemSpread.Horizontal)
                    {
                        result = RectMovement.Left;
                        return result;
                    }
                    result = RectMovement.Up;
                    return result;
            }
            if (orientation == ItemSpread.Horizontal)
                result = RectMovement.Right;
            else
                result = RectMovement.Down;
            return result;
        }
        #endregion

        #region PREVIOUS - NEXT
        /// <summary>
        /// Nexts the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="size">The size.</param>
        /// <returns>Rectangle.</returns>
        private static Rectangle Next(this IBounds source, int increment = 1,
            Direction direction = Direction.Horizontal, int? size = null)
        {
            if (source == null || !source.Valid)
                return Rectangle.Empty;

            int y, x, h, w;
            int sx, sy, sw, sh;
            source.GetBounds(out sx, out sy, out sw, out sh);

            if (direction == Direction.Horizontal)
            {
                y = sy;
                x = sx + sw + increment;
                h = sh;
                w = (size ?? sw);
            }
            else
            {
                y = sy + sh + increment;
                x = sx;
                h = (size ?? sh);
                w = sw;
            }
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Nexts the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="size">The size.</param>
        /// <returns>RectangleF.</returns>
        private static RectangleF Next(this IBoundsF source, float increment = 1f,
            Direction direction = Direction.Horizontal, float? size = null)
        {
            if (source == null || !source.Valid)
                return RectangleF.Empty;
            float y, x, h, w;
            float sx, sy, sw, sh;
            source.GetBounds(out sx, out sy, out sw, out sh);

            if (direction == Direction.Horizontal)
            {
                y = sy;
                x = sx + sw + increment;
                h = sh;
                w = size ?? sw;
            }
            else
            {
                y = sy + sh + increment;
                x = sx;
                h = size ?? sh;
                w = sw;
            }
            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// Nexts the series.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="boundary">The boundary.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="alternateSize">Size of the alternate.</param>
        /// <param name="itemCount">The item count.</param>
        /// <param name="exhaustByCount">if set to <c>true</c> [exhaust by count].</param>
        /// <returns>MnMCollection&lt;RectangleF&gt;.</returns>
        private static PrimitiveList<RectangleF> NextSeries(this IBoundsF source, float boundary,
            float increment = 1f, Direction direction =
            Direction.Horizontal, float alternateSize = 0f, int itemCount = -1,
            bool exhaustByCount = false)
        {
            PrimitiveList<RectangleF> list = new PrimitiveList<RectangleF>();
            list.ResizeUnit = 8;

            if (source == null || !source.Valid)
            {
                return list;
            }


            RectangleF rc = new RectangleF(source);
            bool alt = true;
            var size = (direction == Direction.Horizontal) ?
                rc.Width : rc.Height;
            int i = 0;
            var maxSize = (direction == Direction.Horizontal) ?
               rc.X + rc.Width : rc.Y + rc.Height;

            while (maxSize <= boundary || (itemCount > 0 && i < itemCount))
            {
                list.Add(rc);
                i++;
                var newSize = (alternateSize > 0 && alt) ? alternateSize : size;
                rc = rc.Next(increment, direction, newSize);
                maxSize = (direction == Direction.Horizontal) ?
                    rc.X + rc.Width : rc.Y + rc.Height;
                alt = !alt;
                if (exhaustByCount && i >= itemCount) break;
            }
            return list;
        }

        /// <summary>
        /// Previouses the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="ignoreNegativePoints">if set to <c>true</c> [ignore negative points].</param>
        /// <param name="size">The size.</param>
        /// <returns>Rectangle.</returns>
        private static Rectangle Previous(this IBounds source, int increment = 1,
            Direction direction = Direction.Horizontal,
            bool ignoreNegativePoints = true, int? size = null)
        {
            if (source == null || !source.Valid)
                return Rectangle.Empty;

            if (source == null || !source.Valid)
                return Rectangle.Empty;
            int y, x, h, w, sx, sy, sw, sh, shift;

            source.GetBounds(out sx, out sy, out sw, out sh);

            if (direction == Direction.Horizontal)
            {
                var Size = size ?? sw;
                shift = sx - Size - increment;
                x = (ignoreNegativePoints ? Math.Abs(shift) : shift);
                y = sy;
                h = sh;
                w = Size;
            }
            else
            {
                var Size = size ?? sh;
                shift = sy - Size - increment;
                y = (ignoreNegativePoints ? Math.Abs(shift) : shift);
                x = sx;
                h = Size;
                w = sw;
            }
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Previouses the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="ignoreNegativePoints">if set to <c>true</c> [ignore negative points].</param>
        /// <param name="size">The size.</param>
        /// <returns>RectangleF.</returns>
        private static RectangleF Previous(this IBoundsF source, float increment = 1f,
            Direction direction = Direction.Horizontal,
            bool ignoreNegativePoints = true, float? size = null)
        {
            if (source == null || !source.Valid)
                return RectangleF.Empty;
            float y, x, h, w, sx, sy, sw, sh, shift;

            source.GetBounds(out sx, out sy, out sw, out sh);

            if (direction == Direction.Horizontal)
            {
                var Size = size ?? sw;
                shift = sx - Size - increment;
                x = (ignoreNegativePoints ? Math.Abs(shift) : shift);
                y = sy;
                h = sh;
                w = Size;
            }
            else
            {
                var Size = size ?? sh;
                shift = sy - Size - increment;
                y = (ignoreNegativePoints ? Math.Abs(shift) : shift);
                x = sx;
                h = Size;
                w = sw;
            }
            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// Previouses the series.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="boundary">The boundary.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="alternateSize">Size of the alternate.</param>
        /// <param name="itemCount">The item count.</param>
        /// <param name="exhaustByCount">if set to <c>true</c> [exhaust by count].</param>
        /// <returns>MnMCollection&lt;RectangleF&gt;.</returns>
        private static PrimitiveList<RectangleF> PreviousSeries(this IBoundsF source,
            float boundary, float increment = 1f, Direction direction = Direction.Horizontal,
            float alternateSize = 0f, int itemCount = -1, bool exhaustByCount = false)
        {
            PrimitiveList<RectangleF> list = new PrimitiveList<RectangleF>();
            list.ResizeUnit = 8;
            if (source == null || !source.Valid)
                return list;

            RectangleF rc = new RectangleF(source);
            int i = 0;
            bool alt = true;
            var size = (direction == Direction.Horizontal) ? rc.Width : rc.Height;
            var maxSize = (direction == Direction.Horizontal) ?
                rc.X : rc.Y;

            while (maxSize >= boundary || (itemCount > 0 && i < itemCount))
            {
                list.Add(rc);
                i++;
                var newSize = (alternateSize > 0 && alt) ? alternateSize : size;
                rc = rc.Previous(increment, direction, true, newSize);
                maxSize = (direction == Direction.Horizontal) ?
                    rc.X : rc.Y;
                alt = !alt;
                if (exhaustByCount && i >= itemCount) break;
            }
            return list;
        }
        #endregion

        #region PREVIOUS - NEXT
        /// <summary>
        /// Nexts the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="size">The size.</param>
        /// <returns>Rectangle.</returns>
        private static Rectangle Next(this IBounds source, int increment = 1,
            ItemSpread direction = ItemSpread.Horizontal, int? size = null)
        {
            if (source == null || !source.Valid)
                return Rectangle.Empty;

            int y, x, h, w;
            int sx, sy, sw, sh;
            source.GetBounds(out sx, out sy, out sw, out sh);

            if (direction == ItemSpread.Horizontal)
            {
                y = sy;
                x = sx + sw + increment;
                h = sh;
                w = (size ?? sw);
            }
            else
            {
                y = sy + sh + increment;
                x = sx;
                h = (size ?? sh);
                w = sw;
            }
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Nexts the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="size">The size.</param>
        /// <returns>RectangleF.</returns>
        private static RectangleF Next(this IBoundsF source, float increment = 1f,
            ItemSpread direction = ItemSpread.Horizontal, float? size = null)
        {
            if (source == null || !source.Valid)
                return RectangleF.Empty;
            float y, x, h, w;
            float sx, sy, sw, sh;
            source.GetBounds(out sx, out sy, out sw, out sh);

            if (direction == ItemSpread.Horizontal)
            {
                y = sy;
                x = sx + sw + increment;
                h = sh;
                w = size ?? sw;
            }
            else
            {
                y = sy + sh + increment;
                x = sx;
                h = size ?? sh;
                w = sw;
            }
            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// Nexts the series.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="boundary">The boundary.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="alternateSize">Size of the alternate.</param>
        /// <param name="itemCount">The item count.</param>
        /// <param name="exhaustByCount">if set to <c>true</c> [exhaust by count].</param>
        /// <returns>MnMCollection&lt;RectangleF&gt;.</returns>
        private static PrimitiveList<RectangleF> NextSeries(this IBoundsF source, float boundary,
            float increment = 1f, ItemSpread direction = ItemSpread.Horizontal, float alternateSize = 0f, int itemCount = -1,
            bool exhaustByCount = false)
        {
            PrimitiveList<RectangleF> list = new PrimitiveList<RectangleF>();
            list.ResizeUnit = 8;

            if (source == null || !source.Valid)
            {
                return list;
            }


            RectangleF rc = new RectangleF(source);
            bool alt = true;
            var size = (direction == ItemSpread.Horizontal) ?
                rc.Width : rc.Height;
            int i = 0;
            var maxSize = (direction == ItemSpread.Horizontal) ?
               rc.X + rc.Width : rc.Y + rc.Height;

            while (maxSize <= boundary || (itemCount > 0 && i < itemCount))
            {
                list.Add(rc);
                i++;
                var newSize = (alternateSize > 0 && alt) ? alternateSize : size;
                rc = rc.Next(increment, direction, newSize);
                maxSize = (direction == ItemSpread.Horizontal) ?
                    rc.X + rc.Width : rc.Y + rc.Height;
                alt = !alt;
                if (exhaustByCount && i >= itemCount) break;
            }
            return list;
        }

        /// <summary>
        /// Previouses the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="ignoreNegativePoints">if set to <c>true</c> [ignore negative points].</param>
        /// <param name="size">The size.</param>
        /// <returns>Rectangle.</returns>
        private static Rectangle Previous(this IBounds source, int increment = 1,
             ItemSpread direction = ItemSpread.Horizontal,
            bool ignoreNegativePoints = true, int? size = null)
        {
            if (source == null || !source.Valid)
                return Rectangle.Empty;

            if (source == null || !source.Valid)
                return Rectangle.Empty;
            int y, x, h, w, sx, sy, sw, sh, shift;

            source.GetBounds(out sx, out sy, out sw, out sh);

            if (direction == ItemSpread.Horizontal)
            {
                var Size = size ?? sw;
                shift = sx - Size - increment;
                x = (ignoreNegativePoints ? Math.Abs(shift) : shift);
                y = sy;
                h = sh;
                w = Size;
            }
            else
            {
                var Size = size ?? sh;
                shift = sy - Size - increment;
                y = (ignoreNegativePoints ? Math.Abs(shift) : shift);
                x = sx;
                h = Size;
                w = sw;
            }
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Previouses the specified increment.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="ignoreNegativePoints">if set to <c>true</c> [ignore negative points].</param>
        /// <param name="size">The size.</param>
        /// <returns>RectangleF.</returns>
        private static RectangleF Previous(this IBoundsF source, float increment = 1f,
             ItemSpread direction = ItemSpread.Horizontal,
            bool ignoreNegativePoints = true, float? size = null)
        {
            if (source == null || !source.Valid)
                return RectangleF.Empty;
            float y, x, h, w, sx, sy, sw, sh, shift;

            source.GetBounds(out sx, out sy, out sw, out sh);

            if (direction == ItemSpread.Horizontal)
            {
                var Size = size ?? sw;
                shift = sx - Size - increment;
                x = (ignoreNegativePoints ? Math.Abs(shift) : shift);
                y = sy;
                h = sh;
                w = Size;
            }
            else
            {
                var Size = size ?? sh;
                shift = sy - Size - increment;
                y = (ignoreNegativePoints ? Math.Abs(shift) : shift);
                x = sx;
                h = Size;
                w = sw;
            }
            return new RectangleF(x, y, w, h);
        }

        /// <summary>
        /// Previouses the series.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="boundary">The boundary.</param>
        /// <param name="increment">The increment.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="alternateSize">Size of the alternate.</param>
        /// <param name="itemCount">The item count.</param>
        /// <param name="exhaustByCount">if set to <c>true</c> [exhaust by count].</param>
        /// <returns>MnMCollection&lt;RectangleF&gt;.</returns>
        private static PrimitiveList<RectangleF> PreviousSeries(this IBoundsF source,
            float boundary, float increment = 1f, ItemSpread direction = ItemSpread.Horizontal,
            float alternateSize = 0f, int itemCount = -1, bool exhaustByCount = false)
        {
            PrimitiveList<RectangleF> list = new PrimitiveList<RectangleF>();
            list.ResizeUnit = 8;
            if (source == null || !source.Valid)
                return list;

            RectangleF rc = new RectangleF(source);
            int i = 0;
            bool alt = true;
            var size = (direction == ItemSpread.Horizontal) ? rc.Width : rc.Height;
            var maxSize = (direction == ItemSpread.Horizontal) ?
                rc.X : rc.Y;

            while (maxSize >= boundary || (itemCount > 0 && i < itemCount))
            {
                list.Add(rc);
                i++;
                var newSize = (alternateSize > 0 && alt) ? alternateSize : size;
                rc = rc.Previous(increment, direction, true, newSize);
                maxSize = (direction == ItemSpread.Horizontal) ?
                    rc.X : rc.Y;
                alt = !alt;
                if (exhaustByCount && i >= itemCount) break;
            }
            return list;
        }
        #endregion

        #region TO TYPE RECTANGLE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUpdateArea ToUpdateArea(this IBounds bounds)
        {
            if (bounds is IUpdateArea)
                return (IUpdateArea)bounds;
            if (bounds is IConvertible<IUpdateArea>)
                return ((IConvertible<IUpdateArea>)bounds).Convert();
            return new UpdateArea(bounds);
        }
        #endregion
    }
}
#endif
