/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
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
}


namespace MnM.GWS
{
#if GWS || Window
    using System;
    using System.Collections.Generic;

    public static partial class Rects
    {
        #region COMPITIBLE RC
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
        public static Rectangle CompitibleRc(int sW, int sH, int? x = null, int? y = null, int? width = null, int? height = null)
        {
            var x0 = x ?? 0;
            var y0 = y ?? 0;

            if (x0 < 0)
                x0 = 0;
            if (y0 < 0)
                y0 = 0;

            var srcW = Math.Min(width ?? sW, sW);
            var srcH = Math.Min(height ?? sH, sH);
            if (srcH < 0 || srcW < 0)
                return Rectangle.Empty;

            var right = x0 + srcW;
            var bottom = y0 + srcH;
            if (right > sW)
                right = sW;
            if (bottom > sH)
                bottom = sH;
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
        public static Rectangle CompitibleRc(this Rectangle rect, int? x0 = null, int? y0 = null, int? width = null, int? height = null)
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
        public static Rectangle CompitibleRc(this Rectangle sz, Rectangle rect) =>
            sz.CompitibleRc(rect.X, rect.Y, rect.Width, rect.Height);
        #endregion

        #region UNION
        /// <summary>
        /// Return a IRectangle object defining an area containing the areas of two specified rectangles.
        /// </summary>
        /// <param name="areaG1">First rectangle to be merged.</param>
        /// <param name="areaG2">Second rectangle to be merged.</param>
        /// <returns>IRectangle with area containing the orriginal rectangles.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle Hybrid(this Rectangle areaG1, Rectangle areaG2)
        {
            var x = Math.Min(areaG1.X, areaG2.X);
            var y = Math.Min(areaG1.Y, areaG2.Y);
            var r = Math.Max(areaG1.Right, areaG2.Right);
            var b = Math.Max(areaG1.Bottom, areaG2.Bottom);
            return Rectangle.FromLTRB(x, y, r, b);
        }
        /// <summary>
        /// Return a IRectangleF object defining an area containing the areas of two specified rectangles.
        /// </summary>
        /// <param name="areaG1">First rectangle to be merged.</param>
        /// <param name="areaG2">Second rectangle to be merged.</param>
        /// <returns>IRectangleF with area containing the orriginal rectangles.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Hybrid(this RectangleF areaG1, RectangleF areaG2)
        {
            var x = Math.Min(areaG1.X, areaG2.X);
            var y = Math.Min(areaG1.Y, areaG2.Y);

            var r = Math.Max(areaG1.Right, areaG2.Right);
            var b = Math.Max(areaG1.Bottom, areaG2.Bottom);
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
        public static void Ceiling(this RectangleF r, out int x, out int y, out int w, out int h)
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
        public static void Round(this RectangleF r, out int x, out int y, out int w, out int h)
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
        public static void Floor(this RectangleF r, out int x, out int y, out int w, out int h)
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
        public static void Shrink(this RectangleF r, out int x, out int y, out int w, out int h)
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
        public static void Expand(this RectangleF r, out int x, out int y, out int w, out int h)
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
        public static void Get(this Rectangle rc, out int x, out int y, out int r, out int b)
        {
            x = rc.X;
            y = rc.Y;
            r = rc.Right;
            b = rc.Bottom;
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
        public static void Get(this RectangleF rc, out float x, out float y, out float r, out float b)
        {
            x = rc.X;
            y = rc.Y;
            r = rc.Right;
            b = rc.Bottom;
        }
        #endregion

        #region CONTAINS
        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        public static bool Contains(this Rectangle rect, int x, int y) =>
            x >= rect.X && y >= rect.Y && x <= rect.X + rect.Width && y <= rect.Y + rect.Height;

        /// <summary>
        /// Tests if given x lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <returns>True if the x lies within bounds of this object otherwise false.</returns>
        public static bool ContainsX(this Rectangle rect, int x) =>
            x >= rect.X && x <= rect.X + rect.Width;

        /// <summary>
        /// Tests if given y lies within the bounds of this object.
        /// </summary>
        /// <param name="y">Y co-ordinate of the locaiton.</param>
        /// <returns>True if the y lies within bounds of this object otherwise false.</returns>
        public static bool ContainsY(this Rectangle rect, int y) =>
             y >= rect.Y && y <= rect.Y + rect.Height;

        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        public static bool Contains(this RectangleF rect, float x, float y) =>
            x >= rect.X && y >= rect.Y && x <= rect.X + rect.Width && y <= rect.Y + rect.Height;

        /// <summary>
        /// Tests if given x lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <returns>True if the x lies within bounds of this object otherwise false.</returns>
        public static bool ContainsX(this RectangleF rect, float x) =>
            x >= rect.X && x <= rect.X + rect.Width;

        /// <summary>
        /// Tests if given y lies within the bounds of this object.
        /// </summary>
        /// <param name="y">Y co-ordinate of the locaiton.</param>
        /// <returns>True if the y lies within bounds of this object otherwise false.</returns>
        public static bool ContainsY(this RectangleF rect, float y) =>
             y >= rect.Y && y <= rect.Y + rect.Height;

        #endregion

        #region HAS
        /// <summary>
        /// Checks if specified point is in rectangle. 
        /// </summary>
        /// <param name="rect">Rectangle to check with</param>
        /// <param name="p">Point to check</param>
        /// <param name="checkRightUpto">If null check is considered upto far right of the rectangle. </param>
        /// <param name="checkBottomUpto">If null check is considered upto far bottom of the rectangle. </param>
        /// <returns></returns>
        public static bool Has(this Rectangle rect, Vector p, int? checkRightUpto = null, int? checkBottomUpto = null)
        {
            if (p == null)
                return false;
            return Has(rect, p.X, p.Y, checkRightUpto, checkBottomUpto);
        }

        /// <summary>
        /// Checks if specified if x0 & y0 is in rectangle. 
        /// </summary>
        /// <param name="rect">Rectangle to check with</param>
        /// <param name="x0">x coordinate to check if null then its x coordinate of rectangle</param>
        /// <param name="y0">y coordinate to check if null then its y coordinate of rectangle</param>
        /// <param name="checkRightUpto">If null check is considered upto far right of the rectangle. </param>
        /// <param name="checkBottomUpto">If null check is considered upto far bottom of the rectangle. </param>
        /// <returns></returns>
        public static bool Has(this Rectangle rect, int? x0 = null, int? y0 = null, int? checkRightUpto = null, int? checkBottomUpto = null)
        {
            if (rect.Width == 0 || rect.Height == 0) return false;
            var x = x0 ?? rect.X;
            var y = y0 ?? rect.Y;
            var r = checkRightUpto ?? rect.Right;
            var b = checkBottomUpto ?? rect.Bottom;

            return x >= rect.X && x <= r && y >= rect.Y && y <= b;
        }

        /// <summary>
        /// Checks if specified point is in rectangle. 
        /// </summary>
        /// <param name="rect">Rectangle to check with</param>
        /// <param name="p">Point to check</param>
        /// <param name="checkRightUpto">If null check is considered upto far right of the rectangle. </param>
        /// <param name="checkBottomUpto">If null check is considered upto far bottom of the rectangle. </param>
        /// <returns></returns>
        public static bool Has(this RectangleF rect, Vector p, int? checkRightUpto = null, int? checkBottomUpto = null)
        {
            if (p == null)
                return false;
            return Has(rect, p.X, p.Y, checkRightUpto, checkBottomUpto);
        }

        /// <summary>
        /// Checks if specified if x0 & y0 is in rectangle. 
        /// </summary>
        /// <param name="rect">Rectangle to check with</param>
        /// <param name="x0">x coordinate to check if null then its x coordinate of rectangle</param>
        /// <param name="y0">y coordinate to check if null then its y coordinate of rectangle</param>
        /// <param name="checkRightUpto">If null check is considered upto far right of the rectangle. </param>
        /// <param name="checkBottomUpto">If null check is considered upto far bottom of the rectangle. </param>
        /// <returns></returns>
        public static bool Has(this RectangleF rect, float? x0 = null, float? y0 = null, int? checkRightUpto = null, int? checkBottomUpto = null)
        {
            if (rect.Width == 0 || rect.Height == 0) return false;
            var x = x0 ?? rect.X;
            var y = y0 ?? rect.Y;
            var r = checkRightUpto ?? rect.Right;
            var b = checkBottomUpto ?? rect.Bottom;

            return x >= rect.X && x <= r && y >= rect.Y && y <= b;
        }

        /// <summary>
        /// Checks if specified point is in rectangle. 
        /// </summary>
        /// <param name="rect">Rectangle to check with</param>
        /// <param name="p">Point to check</param>
        /// <param name="checkRightUpto">If null check is considered upto far right of the rectangle. </param>
        /// <param name="checkBottomUpto">If null check is considered upto far bottom of the rectangle. </param>
        /// <returns></returns>
        public static bool Has(this RectangleF rect, VectorF p, int? checkRightUpto = null, int? checkBottomUpto = null)
        {
            if (p == null)
                return false;
            return Has(rect, p.X, p.Y, checkRightUpto, checkBottomUpto);
        }
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
        public static bool Equals(this Rectangle rc, int x, int y, int w, int h) =>
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
        public static bool Equals(this RectangleF rc, float x, float y, float w, float h) =>
            x == rc.X && y == rc.Y && w == rc.Width && h == rc.Height;

        /// <summary>
        /// Tests if given x, y, w and h parametrs are matching in value to X,Y,Width and Height parameters of this object.
        /// </summary>
        /// <param name="o">Other rectangle to match.</param>
        /// <returns>True if all parameters match otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this Rectangle rc, Rectangle o) =>
            o.X == rc.X && o.Y == rc.Y && o.Width == rc.Width && o.Height == rc.Height;

        /// <summary>
        /// Tests if given x, y, w and h parametrs are matching in value to X,Y,Width and Height parameters of this object.
        /// </summary>
        /// <param name="o">Other rectangle to match.</param>
        /// <returns>True if all parameters match otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this RectangleF rc, RectangleF o) =>
            o.X == rc.X && o.Y == rc.Y && o.Width == rc.Width && o.Height == rc.Height;
        #endregion

        #region INTERSECTS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(this Rectangle rect, Rectangle other)
        {
            bool xOverlap = rect.X.IsWithIn(other.X, other.Right) ||
                 other.X.IsWithIn(rect.X, rect.Right);

            bool yOverlap = rect.Y.IsWithIn(other.Y, other.Bottom) ||
                 other.Y.IsWithIn(rect.Y, rect.Bottom);

            return xOverlap && yOverlap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Intersects(this RectangleF rect, RectangleF other)
        {
            bool xOverlap = rect.X.IsWithIn(other.X, other.Right) ||
                 other.X.IsWithIn(rect.X, rect.Right);

            bool yOverlap = rect.Y.IsWithIn(other.Y, other.Bottom) ||
                 other.Y.IsWithIn(rect.Y, rect.Bottom);

            return xOverlap && yOverlap;
        }
        #endregion

        #region CONTAINS OTHER RECT
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this Rectangle parent, Rectangle child)
        {
            return child.X >= parent.X && child.Y >= parent.Y && child.Right <= parent.Right && child.Bottom <= parent.Bottom;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this RectangleF parent, RectangleF child)
        {
            return child.X >= parent.X && child.Y >= parent.Y && child.Right <= parent.Right && child.Bottom <= parent.Bottom;
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
        public static void CorrectMinMax(this Rectangle rectangle, ref int minX, ref int minY, ref int maxX, ref int maxY)
        {
            if (rectangle == null)
                return;
            CorrectMinMax(rectangle.X, rectangle.Y, ref minX, ref minY, ref maxX, ref maxY);
            CorrectMinMax(rectangle.Right, rectangle.Bottom, ref minX, ref minY, ref maxX, ref maxY);
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
        public static void CorrectMinMax(this RectangleF rectangle, ref float minX, ref float minY, ref float maxX, ref float maxY)
        {
            if (rectangle == null)
                return;
            CorrectMinMax(rectangle.X, rectangle.Y, ref minX, ref minY, ref maxX, ref maxY);
            CorrectMinMax(rectangle.Right, rectangle.Bottom, ref minX, ref minY, ref maxX, ref maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetMinMax(out float minX, out float minY, out float maxX, out float maxY)
        {
            minX = minY = int.MaxValue;
            maxX = maxY = 0;
        }
        #endregion
    }

    partial class Rects
    {
        #region SCALE
        public static RectangleF Scale(this RectangleF bounds, IScale scale, VectorF? center = null)
        {
            var sx = scale.X;
            var sy = scale.Y;
            float x, y, r, b;
            var c = center ?? bounds.Center();
            Vectors.Scale(bounds.X, bounds.Y, sx, sy, c.X, c.Y, out x, out y);
            Vectors.Scale(bounds.Right, bounds.Bottom, sx, sy, c.X, c.Y, out r, out b);
            return RectangleF.FromLTRB(x, y, r, b);
        }
        public static Rectangle Scale(this Rectangle bounds, IScale scale, VectorF? center = null)
        {
            var sx = scale.X;
            var sy = scale.Y;
            float x, y, r, b;
            var c = center ?? bounds.Center();
            Vectors.Scale(bounds.X, bounds.Y, sx, sy, c.X, c.Y, out x, out y);
            Vectors.Scale(bounds.Right, bounds.Bottom, sx, sy, c.X, c.Y, out r, out b);
            return Rectangle.FromLTRB(x, y, r, b);
        }

        public static RectangleF Scale(this RectangleF rect, Rotation angle, IScale scale, out bool flatSkew)
        {
            flatSkew = false;

            if (!angle && !scale.HasScale)
            {
                return new RectangleF(rect);
            }
            SkewType skew = 0;

            if(angle)
            {
                skew = angle.Skew;
                flatSkew = skew == SkewType.Horizontal || skew == SkewType.Vertical;
            }

            var bounds = rect;
            float Cx, Cy;
            float Sx = scale.X + 1;
            float Sy = scale.Y + 1;

            if (Sx != 1 || Sy != 1)
                bounds = bounds.Scale(new VectorF(Sx, Sy));

            bool isRotated = angle.EffectiveCenter(bounds, out Cx, out Cy);
            if (isRotated && skew != 0)
                bounds = bounds.Scale(angle, new VectorF(Cx, Cy));

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
        public static void GetStrokeAreas(this Rectangle area, out RectangleF outerArea, out RectangleF innerArea, float stroke, StrokeMode mode = StrokeMode.StrokeMiddle) =>
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
        public static void GetStrokeAreas(this RectangleF area, out RectangleF outerArea, out RectangleF innerArea, float stroke, StrokeMode mode = StrokeMode.StrokeMiddle) =>
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
        public static RectangleF[] GetOutLineAreas(this RectangleF outer, RectangleF inner)
        {
            RectangleF[] r = new RectangleF[4];
            r[0] = RectangleF.FromLTRB(outer.X, inner.Y, inner.X, inner.Bottom); //left
            r[1] = RectangleF.FromLTRB(outer.X, outer.Y, outer.Right, inner.Y); //top
            r[2] = RectangleF.FromLTRB(inner.Right, inner.Y, outer.Right, inner.Bottom); //right
            r[3] = RectangleF.FromLTRB(outer.X, inner.Bottom, outer.Right, outer.Bottom); //right
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
        public static RectangleF ToRectangleF(this Rectangle r) =>
            new RectangleF(r.X, r.Y, r.Width, r.Height);

        /// <summary>
        /// Creates new IRectangle with same dimensions as given Integer version.
        /// </summary>
        /// <param name="r">IRectangle to convert</param>
        /// <returns></returns>
        public static RectangleF ToAreaF(this Rectangle r) =>
            new RectangleF(r.X, r.Y, r.Width, r.Height);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding up.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Ceiling(this RectangleF r) =>
            new Rectangle(r.X.Ceiling(), r.Y.Ceiling(), r.Width.Ceiling(), r.Height.Ceiling());

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding to nearest integer.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Round(this RectangleF r) =>
            new Rectangle(r.X.Round(), r.Y.Round(), r.Width.Round(), r.Height.Round());

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Floor(this RectangleF r) =>
            new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding up the (x,y) of the top left corner 
        /// and rounding down the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Shrink(this RectangleF r) =>
            new Rectangle(r.X.Ceiling(), r.Y.Ceiling(), (int)r.Width, (int)r.Height);

        /// <summary>
        /// Converts IRectngleF to IRectangle.
        /// Converts contained rectangle with floating point dimensions to integer dimensions by rounding down the (x,y) of the top left corner 
        /// and rounding up the Height and Width.
        /// </summary>
        /// <param name="r">IRectangleF to convert</param>
        /// <returns>New IRectangle with integer dimensions.</returns>
        public static Rectangle Expand(this RectangleF r) =>
           new Rectangle((int)r.X, (int)r.Y, r.Width.Ceiling(), r.Height.Ceiling());
        #endregion

        #region CENTER
        /// <summary>
        /// Gets the center of the rectangle.
        /// </summary>
        /// <param name="r">Rectangle to get center from</param>
        /// <returns></returns>
        public static VectorF Center(this RectangleF r) =>
            new VectorF(r.X + r.Width / 2f, r.Y + r.Height / 2f);

        /// <summary>
        /// Gets the ccenter of the rectangle.
        /// </summary>
        /// <param name="r">RectangleF to get center from</param>
        /// <returns></returns>
        public static VectorF Center(this Rectangle r) =>
            new VectorF(r.X + r.Width / 2f, r.Y + r.Height / 2f);
        #endregion

        #region PROXIMITY - INTERSECT
        public static bool Proximity(this Rectangle rect, Vector p, out Vector distancePoint)
        {
            bool ok = (Has(rect, p));
            if (ok)
                distancePoint = new Vector(rect.X + rect.Width - p.X, rect.Y + rect.Height - p.Y);
            else
                distancePoint = new Vector();
            return ok;
        }
        public static bool Proximity(this RectangleF rect, VectorF p, out VectorF distancePoint)
        {
            bool ok = (Has(rect, p));
            if (ok)
                distancePoint = new VectorF(rect.X + rect.Width - p.X, rect.Y + rect.Height - p.Y);
            else
                distancePoint = new VectorF();
            return ok;
        }
        public static Rectangle Intersect(this Rectangle a, Rectangle b)
        {
            int x1 = Math.Max(a.X, b.X);
            int y1 = Math.Max(a.Y, b.Y);
            int x2 = Math.Min(a.Right, b.Right);
            int y2 = Math.Min(a.Bottom, b.Bottom);

            if (x2 >= x1
                && y2 >= y1)
            {
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
            return Rectangle.Empty;
        }
        public static RectangleF Intersect(this RectangleF a, RectangleF b)
        {
            float x1 = Math.Max(a.X, b.X);
            float x2 = Math.Min(a.X + a.Width, b.X + b.Width);
            float y1 = Math.Max(a.Y, b.Y);
            float y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);

            if (x2 >= x1
                && y2 >= y1)
            {

                return new RectangleF(x1, y1, x2 - x1, y2 - y1);
            }
            return RectangleF.Empty;
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
        public static RectangleF Clamp(this RectangleF rect, Size max) =>
            Clamp(rect, max.Width, max.Height);
        public static RectangleF Clamp(this RectangleF rect, float width, float height)
        {
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
        public static Rectangle Clamp(this Rectangle rect, Size max) =>
            Clamp(rect, max.Width, max.Height);
        public static Rectangle Clamp(this Rectangle rect, int width, int height)
        {
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
        public static Rectangle Inflate(this Rectangle rect, int xUnit, int yUnit)
        {
            var x = rect.X - xUnit;
            var y = rect.Y - yUnit;
            int r = rect.Right + xUnit;
            int b = rect.Bottom + yUnit;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            return Rectangle.FromLTRB(x, y, r, b);
        }
        #endregion
    }
#endif
}
