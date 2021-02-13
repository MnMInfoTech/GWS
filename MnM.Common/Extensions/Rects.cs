/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static partial class Rects
    {
        #region COMPITIBLE
        /// <summary>
        /// Returns True if image width and height match the Rectangle width and height with (x,y)=(0,0) or the rectangle has not beem defined yet.
        /// </summary>
        /// <param name="imgW">Image width.</param>
        /// <param name="imgH">Image height.</param>
        /// <param name="x">Top Left x ordinate (0 or null).</param>
        /// <param name="y">Top Left y ordinate (0 or null).</param>
        /// <param name="width">Rectangle width (imgW or null).</param>
        /// <param name="height">Rectangle height (imgH or null).</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCompitibleRc(int imgW, int imgH, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            return (
                (x == 0 || x == null) &&
                (y == 0 || y == null) &&
                (width == imgW || width == 0 || width == null) &&
                (height == imgH || height == 0 || height == null));
        }

        /// <summary>
        /// Returns an IRectangle that is compatible with the one required.
        /// </summary>
        /// <param name="sW">Width reuired</param>
        /// <param name="sH">Height Required</param>
        /// <param name="x">Proposed X position if any.</param>
        /// <param name="y">Proposed Y position if any.</param>
        /// <param name="width">Proposed width if any.</param>
        /// <param name="height">Prioposed height if any.</param>
        /// <returns>Returns a rexctangle compatible with the sW and sH using the provided parameters (if any) or and empty Irectangle object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CompitibleRc(int sW, int sH, out int x0, out int y0, out int right, out int bottom,
            int? x = null, int? y = null, int? width = null, int? height = null)
        {
            right = bottom = 0;
            x0 = x ?? 0;
            y0 = y ?? 0;

            if (x0 < 0)
                x0 = 0;
            if (y0 < 0)
                y0 = 0;

            var srcW = Math.Min(width ?? sW, sW);
            var srcH = Math.Min(height ?? sH, sH);
            if (srcH < 0 || srcW < 0)
                return;

            right = x0 + srcW;
            bottom = y0 + srcH;
            if (right > sW)
                right = sW;
            if (bottom > sH)
                bottom = sH;
        }
        #endregion
    }
    
    #if GWS || Window
    public static partial class Rects
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
        public static Rectangle CompitibleRc(int srcW, int srcH, int? copyX = null, int? copyY = null, int? copyW = null, int? copyH = null)
        {
            var x0 = copyX ?? 0;
            var y0 = copyY ?? 0;

            if (x0 < 0)
                x0 = 0;
            if (y0 < 0)
                y0 = 0;

            var w = Math.Min(copyW ?? srcW, srcW);
            var h = Math.Min(copyH ?? srcH, srcH);
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
        public static Rectangle CompitibleRc(this ISize sz, int? x0 = null, int? y0 = null, int? width = null, int? height = null)
        {
            if (sz is IColor)
            {
                int w = width ?? sz.Width;
                int h = height ?? sz.Height;
                return CompitibleRc(w, h, x0, y0);
            }
            else
                return CompitibleRc(sz.Width, sz.Height, x0, y0, width, height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle CompitibleRc(this ISize sz, Rectangle rect) =>
            CompitibleRc(sz, rect.X, rect.Y, rect.Width, rect.Height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle CompitibleRc(this IRectangle rect, int? x0 = null, int? y0 = null, int? width = null, int? height = null)
        {
            var x = x0 ?? rect.X;
            var y = y0 ?? rect.Y;
            var w = width ?? rect.Width;
            var h = height ?? rect.Height;
            var r = x + w;
            var b = y + h;

            if (x < rect.X)
            {
                var diff = rect.X - x;
                r -= diff;
                x = rect.X;
            }
            if (y < rect.Y)
            {
                var diff = rect.Y - y;
                b -= diff;
                y = rect.Y;
            }

            if (!rect.Contains(x, y))
                return Rectangle.Empty;

            if (r > rect.Width)
                r = rect.Width;

            if (b > rect.Height)
                b = rect.Height;

            return Rectangle.FromLTRB(x, y, r, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle CompitibleRc(this IRectangle sz, Rectangle rect) =>
            sz.CompitibleRc(rect.X, rect.Y, rect.Width, rect.Height);
        #endregion

        #region COMPITIBLE PERIMETER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Perimeter CompitiblePerimeter(int srcW, int srcH, IBoundable perimeter)
        {
            if (perimeter == null)
                perimeter = new Perimeter(0, 0, srcW, srcH);
            perimeter.GetBounds(out int x0, out int y0, out int copyW, out int copyH);

            if (x0 < 0)
                x0 = 0;
            if (y0 < 0)
                y0 = 0;

            var w = Math.Min(copyW, srcW);
            var h = Math.Min(copyH, srcH);
            if (h < 0 || w < 0)
                return Perimeter.Empty;

            var right = x0 + w;
            var bottom = y0 + h;
            if (right > srcW)
                right = srcW;
            if (bottom > srcH)
                bottom = srcH;
            int processID = 0;
            uint shapeID = 0;
            if (perimeter is IProcessID)
                processID = ((IProcessID)perimeter).ProcessID;
            if (perimeter is IShapeID)
                shapeID = ((IShapeID)perimeter).ShapeID;
            return new Perimeter(x0, y0, right - x0, bottom - y0, processID, shapeID);
        }
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Perimeter CompitiblePerimeter(int srcW, int srcH, int x0, int y0, int copyW, int copyH, int processID = 0, uint shapeID = 0)
        {
            if (x0 < 0)
                x0 = 0;
            if (y0 < 0)
                y0 = 0;

            var w = Math.Min(copyW, srcW);
            var h = Math.Min(copyH, srcH);
            if (h < 0 || w < 0)
                return Perimeter.Empty;

            var right = x0 + w;
            var bottom = y0 + h;
            if (right > srcW)
                right = srcW;
            if (bottom > srcH)
                bottom = srcH;
            return new Perimeter(x0, y0, right - x0, bottom - y0, processID, shapeID);
        }
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Perimeter CompitiblePerimeter(this ISize size, int x0, int y0, int copyW, int copyH, int processID = 0, uint shapeID = 0) =>
            CompitiblePerimeter(size.Width, size.Height, x0, y0, copyW, copyH, processID, shapeID);
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Perimeter CompitiblePerimeter(this ISize size, IBoundable perimeter) =>
            CompitiblePerimeter(size.Width, size.Height, perimeter);
        #endregion

        #region UNION
        /// <summary>
        /// Return a IRectangle object defining an area containing the areas of two specified rectangles.
        /// </summary>
        /// <param name="areaG1">First rectangle to be merged.</param>
        /// <param name="areaG2">Second rectangle to be merged.</param>
        /// <returns>IRectangle with area containing the orriginal rectangles.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle Hybrid(this IRectangle areaG1, IRectangle areaG2)
        {
            var x = Math.Min(areaG1.X, areaG2.X);
            var y = Math.Min(areaG1.Y, areaG2.Y);
            var r = Math.Max(areaG1.X + areaG1.Width, areaG2.X + areaG2.Width);
            var b = Math.Max(areaG1.Y + areaG1.Height, areaG2.Y + areaG2.Height);
            return Rectangle.FromLTRB(x, y, r, b);
        }
        /// <summary>
        /// Return a IRectangleF object defining an area containing the areas of two specified rectangles.
        /// </summary>
        /// <param name="areaG1">First rectangle to be merged.</param>
        /// <param name="areaG2">Second rectangle to be merged.</param>
        /// <returns>IRectangleF with area containing the orriginal rectangles.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Hybrid(this IRectangleF areaG1, IRectangleF areaG2)
        {
            var x = Math.Min(areaG1.X, areaG2.X);
            var y = Math.Min(areaG1.Y, areaG2.Y);
            var r = Math.Max(areaG1.X + areaG1.Width, areaG2.X + areaG2.Width);
            var b = Math.Max(areaG1.Y + areaG1.Height, areaG2.Y + areaG2.Height);
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
        public static void Ceiling(this IRectangleF r, out int x, out int y, out int w, out int h)
        {
            x = r.X.Ceiling();
            y = r.Y.Ceiling();
            w = r.Width.Ceiling();
            h = r.Height.Ceiling();
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
        public static void Round(this IRectangleF r, out int x, out int y, out int w, out int h)
        {
            x = r.X.Round();
            y = r.Y.Round();
            w = r.Width.Round();
            h = r.Height.Round();
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
        public static void Floor(this IRectangleF r, out int x, out int y, out int w, out int h)
        {
            x = r.X.Floor();
            y = r.Y.Floor();
            w = r.Width.Floor();
            h = r.Height.Floor();
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
        public static void Shrink(this IRectangleF r, out int x, out int y, out int w, out int h)
        {
            x = r.X.Ceiling();
            y = r.Y.Ceiling();
            w = r.Width.Floor();
            h = r.Height.Floor();
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
        public static void Expand(this IRectangleF r, out int x, out int y, out int w, out int h)
        {
            x = r.X.Floor();
            y = r.Y.Floor();
            w = r.Width.Ceiling();
            h = r.Height.Ceiling();
        }
        #endregion

        #region GET
        /// <summary>
        /// Returns the floating point dimensions of the rectangle defined by IRectangleF.
        /// !!!!bounds do something special!!!!
        /// </summary>
        /// <param name="rc">IRectangleF to report on.</param>
        /// <param name="x">x position of Top Left corner.</param>
        /// <param name="y">y position of Top Left corner.</param>
        /// <param name="r">x position of bottom right corner.</param>
        /// <param name="b">y position of bottom right corner.</param>
        public static void Get(this IRectangle rc, out int x, out int y, out int r, out int b)
        {
            x = rc.X;
            y = rc.Y;
            r = x + rc.Width;
            b = y + rc.Height;
        }

        /// <summary>
        /// Returns the floating point dimensions of the rectangle defined by IRectangleF.
        /// !!!!bounds do something special!!!!
        /// </summary>
        /// <param name="rc">IRectangleF to report on.</param>
        /// <param name="x">x position of Top Left corner.</param>
        /// <param name="y">y position of Top Left corner.</param>
        /// <param name="r">x position of bottom right corner.</param>
        /// <param name="b">y position of bottom right corner.</param>
        public static void Get(this IRectangleF rc, out float x, out float y, out float r, out float b)
        {
            x = rc.X;
            y = rc.Y;
            r = x + rc.Width;
            b = y + rc.Height;
        }
        #endregion

        #region CONTAINS
        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        public static bool Contains(this IRectangle rectangle, int x, int y)
        {
            if (x < rectangle.X || y < rectangle.Y || x > rectangle.X + rectangle.Width || y > rectangle.Y + rectangle.Height)
                return false;
            return true;
        }

        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        public static bool Contains(this IRectangleF rectangle, float x, float y)
        {
            if (x < rectangle.X || y < rectangle.Y || x > rectangle.X + rectangle.Width || y > rectangle.Y + rectangle.Height)
                return false;
            return true;
        }

        /// <summary>
        /// Tests if given x lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <returns>True if the x lies within bounds of this object otherwise false.</returns>
        public static bool ContainsX(this IRectangle rect, int x) =>
            x >= rect.X && x <= rect.X + rect.Width;

        /// <summary>
        /// Tests if given y lies within the bounds of this object.
        /// </summary>
        /// <param name="y">Y co-ordinate of the locaiton.</param>
        /// <returns>True if the y lies within bounds of this object otherwise false.</returns>
        public static bool ContainsY(this IRectangle rect, int y) =>
             y >= rect.Y && y <= rect.Y + rect.Height;

        /// <summary>
        /// Tests if given x lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <returns>True if the x lies within bounds of this object otherwise false.</returns>
        public static bool ContainsX(this IRectangleF rect, float x) =>
            x >= rect.X && x <= rect.X + rect.Width;

        /// <summary>
        /// Tests if given y lies within the bounds of this object.
        /// </summary>
        /// <param name="y">Y co-ordinate of the locaiton.</param>
        /// <returns>True if the y lies within bounds of this object otherwise false.</returns>
        public static bool ContainsY(this IRectangleF rect, float y) =>
             y >= rect.Y && y <= rect.Y + rect.Height;
        #endregion

        #region EQUALS
        /// <summary>
        /// Tests if given x, y, w and h parametrs are matching in value to X,Y,Width and Height parameters of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton to match.</param>
        /// <param name="y">Y co-ordinate of the location to match.</param>
        /// <param name="w">Width value to match.</param>
        /// <param name="h">Height value to match.</param>
        /// <returns>True if all parameters match otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this IRectangle rc, int x, int y, int w, int h) =>
            x == rc.X && y == rc.Y && w == rc.Width && h == rc.Height;

        /// <summary>
        /// Tests if given x, y, w and h parametrs are matching in value to X,Y,Width and Height parameters of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton to match.</param>
        /// <param name="y">Y co-ordinate of the location to match.</param>
        /// <param name="w">Width value to match.</param>
        /// <param name="h">Height value to match.</param>
        /// <returns>True if all parameters match otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this IRectangleF rc, float x, float y, float w, float h) =>
            x == rc.X && y == rc.Y && w == rc.Width && h == rc.Height;

        /// <summary>
        /// Tests if given x, y, w and h parametrs are matching in value to X,Y,Width and Height parameters of this object.
        /// </summary>
        /// <param name="o">Other rectangle to match.</param>
        /// <returns>True if all parameters match otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this IRectangle rc, IRectangle o) =>
            o.X == rc.X && o.Y == rc.Y && o.Width == rc.Width && o.Height == rc.Height;

        /// <summary>
        /// Tests if given x, y, w and h parametrs are matching in value to X,Y,Width and Height parameters of this object.
        /// </summary>
        /// <param name="o">Other rectangle to match.</param>
        /// <returns>True if all parameters match otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this IRectangleF rc, IRectangleF o) =>
            o.X == rc.X && o.Y == rc.Y && o.Width == rc.Width && o.Height == rc.Height;
        #endregion

        #region INTERSECTS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(this IBoundable a, IBoundable b)  
        {
            if (a == null || b == null)
                return false;

            a.GetBounds(out int aX, out int aY, out int aW, out int aH);
            b.GetBounds(out int bX, out int bY, out int bW, out int bH);

            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return false;

            bool xOverlap = aX >= bX && aX <= (bX + bW) ||
                 bX >= aX && bX <= (aX + aW);

            bool yOverlap = aY >= bY && aY <= (bY + bH) ||
                 bY >= aY && bY <= (aY + aH);

            if (!xOverlap || !yOverlap)
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(this IRectangle a, IRectangle b)
        {
            if (a == null || b == null)
                return false;

            int aX = a.X;
            int aY = a.Y;
            int aW = a.Width;
            int aH = a.Height;

            int bX = b.X;
            int bY = b.Y;
            int bW = b.Width;
            int bH = b.Height;

            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return false;

            bool xOverlap = aX >= bX && aX <= (bX + bW) ||
                 bX >= aX && bX <= (aX + aW);

            bool yOverlap = aY >= bY && aY <= (bY + bH) ||
                 bY >= aY && bY <= (aY + aH);

            if (!xOverlap || !yOverlap)
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(this IRectangleF rect, IRectangleF other)
        {
            if (other.Width == 0 || other.Height == 0)
                return false;
            bool xOverlap = rect.X >= other.X && rect.X <= (other.X + other.Width) ||
                 other.X >= rect.X && other.X <= (rect.X + rect.Width);

            bool yOverlap = rect.Y >= other.Y && rect.Y <= (other.Y + other.Height) ||
                 other.Y >= rect.Y && other.Y <= (rect.Y + rect.Height);

            if (!xOverlap || !yOverlap)
                return false;

            return true;
        }
        #endregion

        #region INTERSECT
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Perimeter Intersect(this IBoundable a, IBoundable b)
        {
            if (a == null || b == null)
                return Perimeter.Empty;

            a.GetBounds(out int aX, out int aY, out int aW, out int aH);
            b.GetBounds(out int bX, out int bY, out int bW, out int bH);

            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return Perimeter.Empty;

            int x1 = Math.Max(aX, bX);
            int y1 = Math.Max(aY, bY);
            int x2 = Math.Min(aX + aW, bX + bW);
            int y2 = Math.Min(aY + aH, bY + bH);

            if (x2 >= x1 && y2 >= y1)
            {
                int processID = 0;
                uint shapeID = 0;
                if (a is IProcessID)
                    processID = ((IProcessID)a).ProcessID;
                if (a is IShapeID)
                    shapeID = ((IShapeID)a).ShapeID;
                return new Perimeter(x1, y1, x2 - x1, y2 - y1, processID, shapeID);
            }
            return Perimeter.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle Intersect(this IRectangle a, IRectangle b)
        {
            if (a == null || b == null)
                return Rectangle.Empty;

            int aX = a.X;
            int aY = a.Y;
            int aW = a.Width;
            int aH = a.Height;

            int bX = b.X;
            int bY = b.Y;
            int bW = b.Width;
            int bH = b.Height;

            if (aW == 0 || aH == 0 || bW == 0 || bH == 0)
                return Rectangle.Empty;

            int x1 = Math.Max(aX, bX);
            int y1 = Math.Max(aY, bY);
            int x2 = Math.Min(aX + aW, bX + bW);
            int y2 = Math.Min(aY + aH, bY + bH);

            if (x2 >= x1 && y2 >= y1)
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            return Rectangle.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Intersect(this IRectangleF a, IRectangleF b)
        {
            float x1 = Math.Max(a.X, b.X);
            float y1 = Math.Max(a.Y, b.Y);
            float x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1
                && y2 >= y1)
            {
                return new RectangleF(x1, y1, x2 - x1, y2 - y1);
            }
            return RectangleF.Empty;
        }
        #endregion

        #region CONTAINS OTHER RECT
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this IRectangle parent, IRectangle child)
        {
            var chr = child.X + child.Width;
            var chb = child.Y + child.Height;
            var par = parent.X + parent.Width;
            var pab = parent.Y + parent.Height;

            return child.X >= parent.X && child.Y >= parent.Y && chr <= par && chb <= pab;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(this IEnumerable<Rectangle> collection, ref int minX, ref int minY, ref int maxX, ref int maxY)
        {
            ResetMinMax(out minX, out minY, out maxX, out maxY);

            foreach (var p in collection)
                p.CorrectMinMax(ref minX, ref minY, ref maxX, ref maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectMinMax(this IRectangle rectangle, ref int minX, ref int minY, ref int maxX, ref int maxY)
        {
            if (rectangle == null)
                return;
            CorrectMinMax(rectangle.X, rectangle.Y, ref minX, ref minY, ref maxX, ref maxY);
            CorrectMinMax(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, ref minX, ref minY, ref maxX, ref maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetMinMax(out int minX, out int minY, out int maxX, out int maxY)
        {
            minX = minY = int.MaxValue;
            maxX = maxY = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MinMax(this IEnumerable<RectangleF> collection, ref float minX, ref float minY, ref float maxX, ref float maxY)
        {
            ResetMinMax(out minX, out minY, out maxX, out maxY);

            foreach (var p in collection)
                p.CorrectMinMax(ref minX, ref minY, ref maxX, ref maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectMinMax(this IRectangleF rectangle, ref float minX, ref float minY, ref float maxX, ref float maxY)
        {
            if (rectangle == null)
                return;
            CorrectMinMax(rectangle.X, rectangle.Y, ref minX, ref minY, ref maxX, ref maxY);
            CorrectMinMax(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, ref minX, ref minY, ref maxX, ref maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetMinMax(out float minX, out float minY, out float maxX, out float maxY)
        {
            minX = minY = int.MaxValue;
            maxX = maxY = 0;
        }
        #endregion

        #region MERGE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Merge(this INotifiable notifiable, IBoundable rc)
        {
            if (rc == null)
                return;
            rc.GetBounds(out int x, out int y, out int w, out int h);
            if (w == 0 || h == 0)
                return;
            notifiable.Notify(x, y, x + w, y + h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Merge(this INotifiable notifiable, IRectangle rc)
        {
            if (rc == null)
                return;
            if (rc.Width == 0 || rc.Height == 0)
                return;
            notifiable.Notify(rc.X, rc.Y, rc.X + rc.Width, rc.Y + rc.Height);
        }
        #endregion

        #region COPY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(this INotifiable notifiable, IBoundable rc)
        {
            notifiable.Notify(0, 0, 0, 0);
            if (rc == null)
                return;
            rc.GetBounds(out int x, out int y, out int w, out int h);
            if (w == 0 || h == 0)
                return;
            notifiable.Notify(x, y, x + w, y + h);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Copy(this INotifiable notifiable, IRectangle rc)
        {
            notifiable.Notify(0, 0, 0, 0);
            if (rc == null)
                return;
            if (rc.Width == 0 || rc.Height == 0)
                return;
            notifiable.Notify(rc.X, rc.Y, rc.X + rc.Width, rc.Y + rc.Height);
        }
        #endregion

        #region CLEAR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear(this INotifiable notifiable)
        {
            notifiable.Notify(0, 0, 0, 0);
        }
        #endregion

        #region TO RECTANGLE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle ToRectangle(this IPerimeter boundary, int xExpand = 0, int yExpand = 0)
        {
            if (boundary == null)
                return Rectangle.Empty;
            boundary.GetBounds(out int x, out int y, out int w, out int h, xExpand, yExpand);
            if (w == 0 && h == 0)
                return Rectangle.Empty;
            return new Rectangle(x, y, w, h);
        }
        #endregion

        #region TO PERIMETER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IPerimeter ToPerimeter(this IRectangle rc, int processID = 0)
        {
            if (rc == null)
                return Perimeter.Empty;
            return new Perimeter(rc.X, rc.Y, rc.Width, rc.Height, processID);
        }
        #endregion
    }

    partial class Rects
    {
        #region SCALE
        public static RectangleF Scale(this IRectangleF bounds, IScale scale, VectorF? center = null)
        {
            var sx = scale.X;
            var sy = scale.Y;
            float x, y, r, b;
            var c = center ?? bounds.Center();
            Vectors.Scale(bounds.X, bounds.Y, sx, sy, c.X, c.Y, out x, out y);
            Vectors.Scale(bounds.X + bounds.Width, bounds.Y + bounds.Height, sx, sy, c.X, c.Y, out r, out b);
            return RectangleF.FromLTRB(x, y, r, b);
        }
        public static Rectangle Scale(this IRectangle bounds, IScale scale, VectorF? center = null)
        {
            var sx = scale.X;
            var sy = scale.Y;
            float x, y, r, b;
            var c = center ?? bounds.Center();
            Vectors.Scale(bounds.X, bounds.Y, sx, sy, c.X, c.Y, out x, out y);
            Vectors.Scale(bounds.X + bounds.Width, bounds.Y + bounds.Height, sx, sy, c.X, c.Y, out r, out b);
            return Rectangle.FromLTRB(x, y, r, b);
        }

        public static RectangleF Scale(this IRectangleF rect, Rotation angle, IScale scale, out bool flatSkew)
        {
            flatSkew = false;

            if (!angle && !scale.HasScale)
            {
                return new RectangleF(rect);
            }
            SkewType skew = 0;

            if (angle)
            {
                skew = angle.Skew;
                flatSkew = skew == SkewType.Horizontal || skew == SkewType.Vertical;
            }

            RectangleF bounds = new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
            float Cx, Cy;
            float Sx = scale.X + 1;
            float Sy = scale.Y + 1;

            if (Sx != 1 || Sy != 1)
                bounds = rect.Scale(new VectorF(Sx, Sy));

            bool isRotated = angle.EffectiveCenter(rect, out Cx, out Cy);
            if (isRotated && skew != 0)
                bounds = rect.Scale(angle, new VectorF(Cx, Cy));

            return bounds;
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
        /// <param name="mode">Enum defining stroke position in relation to the curve without stroke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetStrokeAreas(float x, float y, float w, float h, out RectangleF outer,
            out RectangleF inner, float stroke, StrokeMode mode = StrokeMode.StrokeMiddle)
        {
            outer = inner = new RectangleF(x, y, w, h);
            var r = inner.Right;
            var b = inner.Bottom;

            float s;

            if (mode == StrokeMode.StrokeMiddle)
                s = (stroke / 2f);
            else
                s = (stroke);

            switch (mode)
            {
                case StrokeMode.StrokeMiddle:
                default:
                    outer = RectangleF.FromLTRB(x - s, y - s, r + s, b + s);
                    inner = RectangleF.FromLTRB(x + s, y + s, r - s, b - s);
                    break;
                case StrokeMode.StrokeOuter:
                    outer = RectangleF.FromLTRB(x - s, y - s, r + s, b + s);

                    break;
                case StrokeMode.StrokeInner:
                    inner = RectangleF.FromLTRB(x + s, y + s, r - s, b - s);
                    break;
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
        public static void GetStrokeAreas(this IRectangle area, out RectangleF outerArea, out RectangleF innerArea,
            float stroke, StrokeMode mode = StrokeMode.StrokeMiddle) =>
            GetStrokeAreas(area.X, area.Y, area.Width, area.Height, out outerArea, out innerArea, stroke, mode);

        /// <summary>
        /// Returns 2 IRectangleFs that define the area of the inside or outside of the outline with stroke applied. 
        /// </summary>
        /// <param name="area">Area occupied by the shape.</param>
        /// <param name="outerArea">IRectangleF returned with dimensions that contain the Outer edge of the Stroke.</param>
        /// <param name="innerArea">IRectangleF returned with dimensions that contain the Inner edge of the Stroke.</param>
        /// <param name="stroke">Stroke size.</param>
        /// <param name="mode">Enum defining stroke position in relation to the curve without stroke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetStrokeAreas(this IRectangleF area, out RectangleF outerArea, 
            out RectangleF innerArea, float stroke, StrokeMode mode = StrokeMode.StrokeMiddle) =>
            GetStrokeAreas(area.X, area.Y, area.Width, area.Height, out outerArea, out innerArea, stroke, mode);
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
            float ib = inner.Y + inner.Height;
            float ob = outer.Y + outer.Height;
            float ir = inner.X + inner.Width;
            float or = outer.X + outer.Width;

            RectangleF[] r = new RectangleF[4];
            r[0] = RectangleF.FromLTRB(outer.X, inner.Y, inner.X, ib); //left
            r[1] = RectangleF.FromLTRB(outer.X, outer.Y, or, inner.Y); //top
            r[2] = RectangleF.FromLTRB(ir, inner.Y, or, ib); //right
            r[3] = RectangleF.FromLTRB(outer.X, ib, or, ob); //right
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
        public static RectangleF HybridBounds(this IEnumerable<VectorF> c1, IEnumerable<VectorF> c2) =>
            c1.ToArea().Hybrid(c2.ToArea());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public static RectangleF ToRectangleF(this IRectangle r) =>
            new RectangleF(r.X, r.Y, r.Width, r.Height);

        /// <summary>
        /// Creates new IRectangle with same dimensions as given Integer version.
        /// </summary>
        /// <param name="r">IRectangle to convert</param>
        /// <returns></returns>
        public static RectangleF ToAreaF(this IRectangle r) =>
            new RectangleF(r.X, r.Y, r.Width, r.Height);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding up.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Ceiling(this IRectangleF r) =>
            new Rectangle(r.X.Ceiling(), r.Y.Ceiling(), r.Width.Ceiling(), r.Height.Ceiling());

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding to nearest integer.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Round(this IRectangleF r) =>
            new Rectangle(r.X.Round(), r.Y.Round(), r.Width.Round(), r.Height.Round());

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Floor(this IRectangleF r) =>
            new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding up the (x,y) of the top left corner 
        /// and rounding down the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Shrink(this IRectangleF r) =>
            new Rectangle(r.X.Ceiling(), r.Y.Ceiling(), (int)r.Width, (int)r.Height);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Expand(this IRectangleF r, int offSet = 0) =>
           new Rectangle((int)r.X - offSet, (int)r.Y - offSet, r.Width.Ceiling() + offSet, r.Height.Ceiling() + offSet);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Expand(this IRectangle r, int offSet = 0) =>
           new Rectangle((int)r.X - offSet, (int)r.Y - offSet, r.Width + offSet, r.Height + offSet);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Expand(this IRectangle r, int offsetX = 0, int offsetY = 0, int offsetW = 0, int offsetH = 0) =>
           new Rectangle(r.X - offsetX,  r.Y - offsetY, r.Width + offsetW, r.Height + offsetH);
        #endregion

        #region CENTER
        /// <summary>
        /// Gets the center of the rectangle.
        /// </summary>
        /// <param name="r">Rectangle to get center from</param>
        /// <returns></returns>
        public static VectorF Center(this IRectangleF r) =>
            new VectorF(r.X + r.Width / 2f, r.Y + r.Height / 2f);

        /// <summary>
        /// Gets the ccenter of the rectangle.
        /// </summary>
        /// <param name="r">RectangleF to get center from</param>
        /// <returns></returns>
        public static VectorF Center(this IRectangle r) =>
            new VectorF(r.X + r.Width / 2f, r.Y + r.Height / 2f);
        #endregion

        #region PROXIMITY - INTERSECT
        public static bool Proximity(this IRectangle rect, Vector p, out Vector distancePoint)
        {
            bool ok = rect.Contains(p.X, p.Y);
            if (ok)
                distancePoint = new Vector(rect.X + rect.Width - p.X, rect.Y + rect.Height - p.Y);
            else
                distancePoint = new Vector();
            return ok;
        }
        public static bool Proximity(this IRectangleF rect, VectorF p, out VectorF distancePoint)
        {
            bool ok = rect.Contains(p.X, p.Y);
            if (ok)
                distancePoint = new VectorF(rect.X + rect.Width - p.X, rect.Y + rect.Height - p.Y);
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
            if (width == 0)
                width = rect.Width;
            if (height == 0)
                height = rect.Height;

            var x = Math.Max(rect.X, 0);
            var y = Math.Max(rect.Y, 0);
            var w = Math.Max(rect.Width, 0);
            var h = Math.Max(rect.Height, 0);
            if (w > width)
                w = width;
            if (h > height)
                h = height;
            return new RectangleF(x, y, w, h);
        }
        public static Rectangle Clamp(this IRectangle rect, Size max) =>
            Clamp(rect, max.Width, max.Height);
        public static Rectangle Clamp(this IRectangle rect, int width, int height)
        {
            if (width == 0)
                width = rect.Width;
            if (height == 0)
                height = rect.Height;

            var x = Math.Max(rect.X, 0);
            var y = Math.Max(rect.Y, 0);
            var w = Math.Max(rect.Width, 0);
            var h = Math.Max(rect.Height, 0);
            if (w > width)
                w = width;
            if (h > height)
                h = height;
            return new Rectangle(x, y, w, h);
        }
        #endregion

        #region INFLATE
        public static Rectangle Inflate(this IRectangle rect, int xUnit, int yUnit)
        {
            var x = rect.X - xUnit;
            var y = rect.Y - yUnit;
            var r = (rect.X + rect.Width);
            var b = (rect.Y + rect.Height);
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            return Rectangle.FromLTRB(x, y, r, b);
        }

        public static Rectangle Inflate(this IRectangle rect, int xUnit, int yUnit, int rUnit, int bUnit)
        {
            var x = rect.X - xUnit;
            var y = rect.Y - yUnit;
            var r = (rect.X + rect.Width) + rUnit;
            var b = (rect.Y + rect.Height) + bUnit;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            return Rectangle.FromLTRB(x, y, r, b);
        }

        public static RectangleF Inflate(this IRectangleF rect, int xUnit, int yUnit)
        {
            var x = rect.X - xUnit;
            var y = rect.Y - yUnit;
            var r = (rect.X + rect.Width);
            var b = (rect.Y + rect.Height);
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            return RectangleF.FromLTRB(x, y, r, b);
        }

        public static RectangleF Inflate(this IRectangleF rect, int xUnit, int yUnit, int rUnit, int bUnit)
        {
            var x = rect.X - xUnit;
            var y = rect.Y - yUnit;
            var r = (rect.X + rect.Width) + rUnit;
            var b = (rect.Y + rect.Height) + bUnit;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            return RectangleF.FromLTRB(x, y, r, b);
        }
        #endregion
    }
#endif
}
