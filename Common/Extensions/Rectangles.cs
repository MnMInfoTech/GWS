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
    public static partial class Rectangles
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CompitibleRc(int sW, int sH, 
            out int x0, out int y0, out int right, out int bottom, int x, int y, int width, int height)
        {
            right = bottom = 0;
            x0 = x;
            y0 = y;

            if (x0 < 0)
                x0 = 0;
            if (y0 < 0)
                y0 = 0;

            var srcW = Math.Min(width, sW);
            var srcH = Math.Min(height, sH);
            if (srcH < 0 || srcW < 0)
                return;

            right = x0 + srcW;
            bottom = y0 + srcH;
            if (right > sW)
                right = sW;
            if (bottom > sH)
                bottom = sH;
        }

        /// <summary>
        /// Returns an IRectangle that is compatible with the one required.
        /// </summary>
        /// <param name="srcW">Width reuired</param>
        /// <param name="srcH">Height Required</param>
        /// <param name="x">Proposed X position if any.</param>
        /// <param name="y">Proposed Y position if any.</param>
        /// <param name="w">Proposed width if any.</param>
        /// <param name="h">Prooposed height if any.</param>
        /// <returns>Returns Resultant Rectangle object.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CompitibleRc(int srcW, int srcH, ref int x, ref int y, ref int w, ref int h)
        {
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            w = Math.Min(w, srcW);
            h = Math.Min(h, srcH);
            if (h < 0 || w < 0)
            {
                w = h = x = y = 0;
                return;
            }
            var right = x + w;
            var bottom = y + h;
            if (right > srcW)
                right = srcW;
            if (bottom > srcH)
                bottom = srcH;
            w = right - x;
            h = bottom - y;
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle CompitibleRc(int x1, int y1, int w1, int h1,
            int x, int y, int width, int height, ObjType? type = null)
        {
            if (w1==0 && h1==0)
                return Rectangle.Empty;

            var r = x + width;
            var b = y + height;
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

            if (x < x1 || x > r1 || y < y1 || y > b1)
                return Rectangle.Empty;

            if (r > r1)
                r = r1;

            if (b > b1)
                b = b1;

            if (type != null)
                return UpdateArea.FromLTRB(x, y, r, b, type.Value);

            return Rectangle.FromLTRB(x, y, r, b);
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this IBounds rect, int x, int y, int w, int h)
        {
            if (rect == null || !rect.Valid)
                return false;
            rect.GetBounds(out int x1, out int y1, out int w1, out int h1);

            return x == x1 && y == y1 && w == w1 && h == h1;
        }

        /// <summary>
        /// Tests if given x, y, w and h parametrs are matching in value to X,Y,Width and Height parameters of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton to match.</param>
        /// <param name="y">Y co-ordinate of the location to match.</param>
        /// <param name="w">Width value to match.</param>
        /// <param name="h">Height value to match.</param>
        /// <returns>True if all parameters match otherwise false.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this IRectangleF rc, float x, float y, float w, float h)
        {
            if (rc == null || !rc.Valid)
                return false;
            return x == rc.X && y == rc.Y && w == rc.Width && h == rc.Height;
        }

        /// <summary>
        /// Tests if given x, y, w and h parametrs are matching in value to X,Y,Width and Height parameters of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton to match.</param>
        /// <param name="y">Y co-ordinate of the location to match.</param>
        /// <param name="w">Width value to match.</param>
        /// <param name="h">Height value to match.</param>
        /// <returns>True if all parameters match otherwise false.</returns>

        /// <summary>
        /// Tests if given rectangle is matching in values to another rectangle.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="rc1"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this IRectangleF rc, IRectangleF rc1)
        {
            if (rc == null && rc1 == null)
                return true;
            if (rc == null || rc1 == null)
                return false;

            return rc.Equals(rc1.X, rc1.Y, rc1.Width, rc1.Height);
        }


        /// <summary>
        /// Tests if given rectangle is matching in values to another rectangle.
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="rc1"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals(this IBounds rc, IBounds rc1)
        {
            if (rc == null && rc1 == null)
                return true;
            if (rc == null || rc1 == null)
                return false;

            int x, y, w, h, x1, y1, w1, h1;
            rc.GetBounds(out x, out y, out w, out h);
            rc1.GetBounds(out x1, out y1, out w1, out h1);

            return x == x1 && y == y1 && w == w1 && h == h1;
        }
        #endregion

        #region CONTAINS
        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the location.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this IBounds bounds, float x, float y)
        {
            if (bounds == null || !bounds.Valid)
                return false;  
            if(bounds is IHitTestable)
                return ((IHitTestable)bounds).Contains(x, y);

            int X, Y, W, H;
            bounds.GetBounds(out X, out Y, out W, out H);
            if (W <= 0 || H <= 0)
                return false;
            if (x < X || y < Y || x > (X + W) || y > (Y + H))
                return false;
            return true;
        }

        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the location.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this IBounds bounds, int x, int y)
        {
            if (bounds == null || !bounds.Valid)
                return false;   
            
            if (bounds is IHitTestable)
                return ((IHitTestable)bounds).Contains(x, y);

            int X, Y, W, H;
            bounds.GetBounds(out X, out Y, out W, out H);
            if (W <= 0 || H <= 0)
                return false;
            if (x < X || y < Y || x > (X + W) || y > (Y + H))
                return false;
            return true;
        }

        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the location.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this IBoundsF bounds, float x, float y)
        {
            if (bounds == null || !bounds.Valid)
                return false;  
            
            if (bounds is IHitTestable)
                return ((IHitTestable)bounds).Contains(x, y);

            float X, Y, W, H;
            bounds.GetBounds(out X, out Y, out W, out H);
            if (x < X || y < Y || x > (X + W) || y > (Y + H))
                return false;
            return true;
        }

        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the location.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this IBoundsF bounds, int x, int y)
        {
            if (bounds == null || !bounds.Valid)
                return false;  
            if (bounds is IHitTestable)
                return ((IHitTestable)bounds).Contains(x, y);

            float X, Y, W, H;
            bounds.GetBounds(out X, out Y, out W, out H);
            if (x < X || y < Y || x > (X + W) || y > (Y + H))
                return false;
            return true;
        }

        /// <summary>
        /// Tests if given location lies within the bounds specified through X, Y, W, H parameters.
        /// </summary>
        /// <param name="X">X co-ordinate of the bounds.</param>
        /// <param name="Y">Y co-ordinate of the bounds.</param>
        /// <param name="W">Width of the bounds.</param>
        /// <param name="H">Height of the bounds.</param>
        /// <param name="x">X co-ordinate of the location.</param>
        /// <param name="y">X co-ordinate of the location.</param>
        /// <returns>True if test is successful otherwise false.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(int X, int Y, int W, int H, float x, float y)
        {
            if (W == 0 || H == 0 || x < X || y < Y || x > (X + W) || y > (Y + H))
                return false;
            return true;
        }

        /// <summary>
        /// Tests if given location lies within the bounds specified through X, Y, W, H parameters.
        /// </summary>
        /// <param name="X">X co-ordinate of the bounds.</param>
        /// <param name="Y">Y co-ordinate of the bounds.</param>
        /// <param name="W">Width of the bounds.</param>
        /// <param name="H">Height of the bounds.</param>
        /// <param name="x">X co-ordinate of the location.</param>
        /// <param name="y">X co-ordinate of the location.</param>
        /// <returns>True if test is successful otherwise false.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(int X, int Y, int W, int H, int x, int y)
        {
            if (W == 0 || H == 0 || x < X || y < Y || x > (X + W) || y > (Y + H))
                return false;
            return true;
        }

        /// <summary>
        /// Tests if given location lies within the bounds specified through X, Y, W, H parameters.
        /// </summary>
        /// <param name="X">X co-ordinate of the bounds.</param>
        /// <param name="Y">Y co-ordinate of the bounds.</param>
        /// <param name="W">Width of the bounds.</param>
        /// <param name="H">Height of the bounds.</param>
        /// <param name="x">X co-ordinate of the location.</param>
        /// <param name="y">X co-ordinate of the location.</param>
        /// <returns>True if test is successful otherwise false.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(float X, float Y, float W, float H, float x, float y)
        {
            if (W == 0 || H == 0 || x < X || y < Y || x > (X + W) || y > (Y + H))
                return false;
            return true;
        }

        /// <summary>
        /// Tests if given location lies within the bounds specified through X, Y, W, H parameters.
        /// </summary>
        /// <param name="X">X co-ordinate of the bounds.</param>
        /// <param name="Y">Y co-ordinate of the bounds.</param>
        /// <param name="W">Width of the bounds.</param>
        /// <param name="H">Height of the bounds.</param>
        /// <param name="x">X co-ordinate of the location.</param>
        /// <param name="y">X co-ordinate of the location.</param>
        /// <returns>True if test is successful otherwise false.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(float X, float Y, float W, float H, int x, int y)
        {
            if (W == 0 || H == 0 || x < X || y < Y || x > (X + W) || y > (Y + H))
                return false;
            return true;
        }
        #endregion

        #region SCALE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(ref float X, ref float Y, ref float Width, ref float Height,
            IScale scale, IPointF center = null)
        {
            if (Width <= 0 && Height <= 0)
                return;
            if (scale == null || !scale.HasScale)
                return;

            var sx = scale.X;
            var sy = scale.Y;
            float r1 = X + Width, b1 = Y + Height;
            var c = center ?? new VectorF(X + Width / 2f, Y + Height / 2f);

            Vectors.Scale(X, Y, sx, sy, c.X, c.Y, out float x, out float y);
            Vectors.Scale(r1, b1, sx, sy, c.X, c.Y, out float r, out float b);
            X = x;
            Y = y;
            Width = r - x;
            Height = b - y;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(float X, float Y, float Width, float Height,
            out float resultX, out float resultY, out float resultWidth, out float resultHeight,
            IScale scale, IPointF center = null)
        {
            resultX = X;
            resultY = Y;
            resultWidth = Width;
            resultHeight = Height;

            if (Width <= 0 && Height <= 0)
                return;
            if (scale == null || !scale.HasScale)
                return;

            var sx = scale.X;
            var sy = scale.Y;
            float r1 = X + Width, b1 = Y + Height;
            var c = center ?? new VectorF(X + Width / 2f, Y + Height / 2f);

            float r2, b2;
            Vectors.Scale(X, Y, sx, sy, c.X, c.Y, out resultX, out resultY);
            Vectors.Scale(r1, b1, sx, sy, c.X, c.Y, out r2, out b2);
            resultWidth = r2 - resultX;
            resultHeight = b2 - resultY;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(ref int X, ref int Y, ref int Width, ref int Height, IScale scale,
            IPointF center = null)
        {
            if (Width <= 0 && Height <= 0)
                return;
            if (scale == null || !scale.HasScale)
                return;

            var sx = scale.X;
            var sy = scale.Y;
            float r1 = X + Width, b1 = Y + Height;
            var c = center ?? new VectorF(X + Width / 2f, Y + Height / 2f);

            Vectors.Scale(X, Y, sx, sy, c.X, c.Y, out float x, out float y);
            Vectors.Scale(r1, b1, sx, sy, c.X, c.Y, out float r, out float b);
            X = (int)x;
            Y = (int)y;
            float w2 = (r - x);
            float h2 = (b - y);
            int iw2 = w2.Round();
            int ih2 = h2.Round();
            Width = iw2;
            Height = ih2;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale( ref float X, ref float Y, ref float Width, ref float Height, IRotation rotation,
            IScale scale, IPointF center = null)
        {
            float r = X + Width, b = Y + Height;

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
                X = (X - CX) * sx + CX;
                Y = (Y - CY) * sy + CY;
                r = (r - CX) * sx + CX;
                b = (b - CY) * sy + CY;
            }
            if (HasSkewScale)
            {
                rotation?.EffectiveCenter(CX, CY, out CX, out CY);
                var skewX = rotation.Skew.X;
                var skewY = rotation.Skew.Y;

                X = (X - CX) * skewX + CX;
                Y = (Y - CY) * skewY + CY;
                r = (r - CX) * skewX + CX;
                b = (b - CY) * skewY + CY;
            }
            Exit:
            Width = r - X;
            Height = b - Y;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(ref int X, ref int Y, ref int Width, ref int Height,
            IRotation rotation, IScale scale, IPointF center = null)
        {
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
                X = (int)((X - CX) * sx + CX);
                Y = (int)((Y - CY) * sy + CY);
                r = (int)((r - CX) * sx + CX);
                b = (int)((b - CY) * sy + CY);
            }

            if (HasSkewScale)
            {
                rotation?.EffectiveCenter(CX, CY, out CX, out CY);

                var skewX = rotation.Skew.X;
                var skewY = rotation.Skew.Y;
                X = (int)((X - CX) * skewX + CX);
                Y = (int)((Y - CY) * skewY + CY);
                r = (int)((r - CX) * skewX + CX);
                b = (int)((b - CY) * skewY + CY);
            }
            Exit:
            Width = r - X;
            Height = b - Y;
        }
        #endregion

        #region ROTATE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateRectangle(ref float X, ref float Y, ref float W, ref float H, float degree,
            IPointF center = null, bool antiClock = false, ISkew Skew = null, SkewType? skewType = null,
            float? skewDegree = null)
        {
            var Cx = center?.X ?? X + W / 2f;
            var Cy = center?.Y ?? Y + H / 2f;
            var type = Skew?.Type ?? skewType ?? 0;
            Angles.Rotate(X, Y, degree, Cx, Cy, out float X1, out float Y1, antiClock, type, skewDegree);
            Angles.Rotate(X + W, Y, degree, Cx, Cy, out float X2, out float Y2, antiClock, type, skewDegree);
            Angles.Rotate(X + W, Y + H, degree, Cx, Cy, out float X3, out float Y3, antiClock, type, skewDegree);
            Angles.Rotate(X, Y + H, degree, Cx, Cy, out float X4, out float Y4, antiClock, type, skewDegree);
            Vectors.MinMax(out X, out Y, out float maxX, out float maxY,
                X1, Y1, X2, Y2, X3, Y3, X4, Y4);
            W = maxX - X;
            H = maxY - Y;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateRectangle(ref int X, ref int Y, ref int W, ref int H, float degree,
            IPointF center = null, bool antiClock = false, ISkew Skew = null, SkewType? skewType = null,
            float? skewDegree = null)
        {
            var Cx = center?.X ?? X + W / 2f;
            var Cy = center?.Y ?? Y + H / 2f;
            var type = Skew?.Type ?? skewType ?? 0;
            Angles.Rotate(X, Y, degree, Cx, Cy, out float X1, out float Y1, antiClock, type, skewDegree);
            Angles.Rotate(X + W, Y, degree, Cx, Cy, out float X2, out float Y2, antiClock, type, skewDegree);
            Angles.Rotate(X + W, Y + H, degree, Cx, Cy, out float X3, out float Y3, antiClock, type, skewDegree);
            Angles.Rotate(X, Y + H, degree, Cx, Cy, out float X4, out float Y4, antiClock, type, skewDegree);
            Vectors.MinMax(out float x, out float y, out float maxX, out float maxY,
                X1, Y1, X2, Y2, X3, Y3, X4, Y4);

            W = (maxX - X).Round();
            H = (maxY - Y).Round();
            X = x.Round();
            Y = y.Round();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateRectangle(int X, int Y, int W, int H, float degree, out Point pt, out Size size,
            IPointF center = null, bool antiClock = false, ISkew Skew = null, SkewType? skewType = null,
            float? skewDegree = null)
        {
            RotateRectangle(ref X, ref Y, ref W, ref H, degree, center, antiClock, Skew, skewType, skewDegree);
            pt = new Point(X, Y);
            size = new Size(W, H);
        }

        public static void RotateRectangle(int X, int Y, int W, int H, float degree, out int resultX, out int resultY,
            out int resultWidth, out int resultHeight,
            IPointF center = null, bool antiClock = false, ISkew Skew = null, SkewType? skewType = null,
            float? skewDegree = null)
        {
            resultX = X; resultY = Y;
            resultWidth = W; resultHeight = H;

            RotateRectangle(ref resultX, ref resultY, ref resultWidth, ref resultHeight, degree, center, antiClock, Skew, skewType, skewDegree);
        }

        public static void RotateRectangle(float X, float Y, float W, float H, float degree, out float resultX, out float resultY,
        out float resultWidth, out float resultHeight,
        IPointF center = null, bool antiClock = false, ISkew Skew = null, SkewType? skewType = null,
        float? skewDegree = null)
        {
            resultX = X; resultY = Y;
            resultWidth = W; resultHeight = H;

            RotateRectangle(ref resultX, ref resultY, ref resultWidth, ref resultHeight, degree, center, antiClock, Skew, skewType, skewDegree);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateRectangle(float X, float Y, float W, float H, float degree, out PointF pt, out SizeF size,
            IPointF center = null, bool antiClock = false, ISkew Skew = null, SkewType? skewType = null,
            float? skewDegree = null)
        {
            RotateRectangle(ref X, ref Y, ref W, ref H, degree, center, antiClock, Skew, skewType, skewDegree);
            pt = new PointF(X, Y);
            size = new SizeF(W, H);
        }

        public static void RotateRectangle(this IDegree degree, ref int X, ref int Y, ref int W, ref int H, bool antiClock = false, bool noSkew = false)
        {
            if (degree == null || !degree.Valid)
                return;
            ICentre center = degree is ICentreHolder ? ((ICentreHolder)degree).Centre : null;
            if (!noSkew && degree is ISkewHolder)
            {
                ISkew skew = degree is ISkewHolder ? ((ISkewHolder)degree).Skew : null;
                RotateRectangle(ref X, ref Y, ref W, ref H, degree.Angle, center, antiClock, skew);
            }
            else
            {
                RotateRectangle(ref X, ref Y, ref W, ref H, degree.Angle, center, antiClock, null);
            }
        }

        public static void RotateRectangle(this IDegree degree, ref float X, ref float Y, ref float W, ref float H, bool antiClock = false, bool noSkew = false)
        {
            if (degree == null || !degree.Valid)
                return;
            ICentre center = degree is ICentreHolder ? ((ICentreHolder)degree).Centre : null;
            if (!noSkew && degree is ISkewHolder)
            {
                ISkew skew = degree is ISkewHolder ? ((ISkewHolder)degree).Skew : null;
                RotateRectangle(ref X, ref Y, ref W, ref H, degree.Angle, center, antiClock, skew);
            }
            else
            {
                RotateRectangle(ref X, ref Y, ref W, ref H, degree.Angle, center, antiClock, null);
            }
        }
        public static void RotateRectangle(this IDegree degree, int X, int Y, int W, int H, out Point pt, out Size size, bool antiClock = false, bool noSkew = false)
        {
            if(degree?.Valid == true)
            {
                RotateRectangle(degree, ref X, ref Y, ref W, ref H, antiClock, noSkew);
            }
            pt = new Point(X, Y);
            size = new Size(W, H);
        }
        public static void RotateRectangle(this IDegree degree, float X, float Y, float W, float H, out PointF pt, out SizeF size, bool antiClock = false, bool noSkew = false)
        {
            if (degree?.Valid == true)
            {
                RotateRectangle(degree, ref X, ref Y, ref W, ref H, antiClock, noSkew);
            }
            pt = new PointF(X, Y);
            size = new SizeF(W, H);
        }
        #endregion

        #region GET SCAN LINES
        public static IEnumerable<IAxisLine> GetScanLines(this IBounds bounds)
        {
            if (bounds is IPolygonal)
            {
                var pts = ((IPolygonal)bounds).GetPoints();
                if (pts != null)
                {
                    var lines = pts.ToLineSegments(PointJoin.CircularJoin);
                    using (var scanner = Factory.newLineScanner())
                    {
                        scanner.Scan(lines);
                        foreach (var item in scanner.GetScanLines())
                            yield return item;
                    }
                }
            }
            else if (bounds is IPolygonalF)
            {
                var pts = ((IPolygonalF)bounds).GetPoints();
                var lines = pts.ToLineSegments(PointJoin.CircularJoin);
                using (var scanner = Factory.newLineScanner())
                {
                    scanner.Scan(lines);
                    foreach (var item in scanner.GetScanLines())
                        yield return item;
                }
            }
            bounds.GetBounds(out int x, out int y, out int w, out int h);
            for (int i = y; i < y + h; i++)
            {
                yield return new AxisLine(x, i, LineFill.Horizontal, w);
            }
        }
        #endregion
    }
}
