/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static partial class Angles
    {
        #region ROTATE VECTOR  
        /// <summary>
        /// Rotates point on angle's center.
        /// </summary>
        /// <param name="rotation">>Angle to rotate point.</param>
        /// <param name="x">The x co-ordinate of rotated point.</param>
        /// <param name="y">The y co-ordinate of rotated point.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="quadratic">Point type of the resultant point..</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Rotated point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Rotate(this IDegree rotation, float x, float y,
            bool antiClock = false, PointKind quadratic = 0, bool noSkew = false)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(x, y, out x, out y, antiClock: antiClock, noSkew: noSkew);
            return new VectorF(x, y, quadratic);
        }

        /// <summary>
        /// Rotates point on angle's center.
        /// </summary>
        /// <param name="rotation">>Angle to rotate point.</param>
        /// <param name="x">The x co-ordinate of rotated point.</param>
        /// <param name="y">The y co-ordinate of rotated point.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="quadratic">Point type of the resultant point..</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Rotated point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Rotate(this IDegree rotation, int x, int y,
            bool antiClock = false, PointKind quadratic = 0, bool noSkew = false)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(x, y, out x, out y, antiClock: antiClock, noSkew: noSkew);
            return new Vector(x, y, quadratic);
        }

        /// <summary>
        /// Rotates point by given rotation.
        /// </summary>
        /// <param name="rotation">>Angle to rotate point.</param>
        /// <param name="pt">Point to be rotated.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Rotated point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Rotate(this IDegree rotation, IPointF pt, bool antiClock = false, bool noSkew = false)
        {
            if (pt == null)
                return VectorF.Empty;
            return rotation.Rotate(pt.X, pt.Y, antiClock, pt is IPointType ? ((IPointType)pt).Kind : 0, noSkew);
        }

        /// <summary>
        /// Rotates point on angle's center.
        /// </summary>
        /// <param name="rotation">>Angle to rotate point.</param>
        /// <param name="p">Point to rotate.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Rotated point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Rotate(this IDegree rotation, IPoint pt, bool antiClock = false, bool noSkew = false)
        {
            if (pt == null)
                return Vector.Empty;

            return rotation.Rotate(pt.X, pt.Y, antiClock, pt is IPointType ? ((IPointType)pt).Kind : 0, noSkew);
        }

        /// <summary>
        /// Rotates point around the center specified.
        /// </summary>
        /// <param name="rotation">>Angle to rotate point.</param>
        /// <param name="pt">Point to rotate.</param>
        /// <param name="center">The center to rotate point around.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Rotated point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Rotate(this IDegree rotation, IPointF pt, IPointF center, 
            bool antiClock = false, bool noSkew = false)
        {
            if (pt == null)
                return VectorF.Empty;
            rotation.Rotate(pt.X, pt.Y, out float x, out float y, center.X, center.Y, antiClock, noSkew);
            return new VectorF(x, y, pt is IPointType ? ((IPointType)pt).Kind : 0);
        }

        /// <summary>
        /// Rotates point around the center specified.
        /// </summary>
        /// <param name="rotation">>Angle to rotate point.</param>
        /// <param name="pt">Point to rotate.</param>
        /// <param name="center">The center to rotate point around.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Rotated point.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Rotate(this IDegree rotation, IPoint pt, IPoint center,
            bool antiClock = false, bool noSkew = false)
        {
            if (pt == null)
                return Vector.Empty;

            rotation.Rotate(pt.X, pt.Y, out int x, out int y, center.X, center.Y, antiClock, noSkew);
            return new Vector(x, y, pt is IPointType ? ((IPointType)pt).Kind : 0);
        }

        /// <summary>
        /// Rotates point around the center specified.
        /// </summary>
        /// <param name="rotation">>Angle to rotate point.</param>
        /// <param name="p">Point to rotate.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Rotated point.</returns>
        public static VectorF Rotate(this IPointF p, IDegree rotation, bool antiClock = false, bool noSkew = false) =>
            rotation.Rotate(p, antiClock, noSkew);

        /// <summary>
        /// Rotates point around the center specified.
        /// </summary>
        /// <param name="rotation">Rotation to be used for rotation.</param>
        /// <param name="p">Point to rotate.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Rotated point.</returns>
        public static Vector Rotate(this IPoint p, IDegree rotation, bool antiClock = false, bool noSkew = false) =>
            rotation.Rotate(p, antiClock, noSkew);
        #endregion

        #region ROTATE VECTORS
        /// <summary>
        /// Rotates given collection of points using specified rotation object.
        /// </summary>
        /// <param name="Source">Collection of points to be rotated.</param>
        /// <param name="rotation">Rotation to be used for rotation.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="center">Optional center of rotation.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Collection of rotated points.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<VectorF> Rotate(this IEnumerable<VectorF> Source, IDegree rotation,
            bool antiClock = false, IPointF center = null, bool noSkew = false)
        {
            if (rotation == null || !rotation.Valid)
                return Source;

            float Sin = 0, Cos = 1;
            float degree = rotation.Angle;
            float CX = 0, CY = 0;
            ISkew skew = null;
            ICentre Centre = center != null ? new Centre(center) : default(ICentre);
           
            float SkewX = 1, SkewY = 1;
            bool HasSkewScale = false;
            bool diagonal = false;

            if (rotation is ISkewHolder)
            {
                skew = ((ISkewHolder)rotation).Skew;
                HasSkewScale = skew?.HasScale == true && !noSkew;
                if (HasSkewScale)
                {
                    SkewX = skew.X;
                    SkewY = skew.Y;
                    diagonal = skew.Type == SkewType.Diagonal;
                }
            }
            bool HasAngle = rotation.HasAngle || diagonal;
            if (diagonal)
                degree += skew.Degree;
            if (Centre == null)
            {
                if (rotation is ICentreHolder)
                {
                    Centre = ((ICentreHolder)rotation).Centre;
                }
                else if (rotation is ICentre)
                {
                    Centre = ((ICentre)rotation);
                }
            }
            if (Centre != null)
            {
                CX = Centre.X;
                CY = Centre.Y;
            }
            if (HasAngle)
            {
                SinCos(degree, out Sin, out Cos);
                if (antiClock)
                    Sin = -Sin;
            }
            float x, y, x1, y1;
            PointKind type;

            return Source.Select((item) => 
            {
                if (!item)
                    return item;
                x = item.X;
                y = item.Y;
                type = item.Kind;

                if (HasSkewScale)
                {
                    x = (x - CX) * SkewX + CX;
                    y = (y - CY) * SkewY + CY;
                }
                if (HasAngle)
                {
                    x -= CX;
                    y -= CY;
                    x1 = (x * Cos - y * Sin);
                    y1 = (x * Sin + y * Cos);
                    x = x1 + CX;
                    y = y1 + CY;
                }
                return new VectorF(x, y, type); 
            });
        }

        /// <summary>
        /// Rotates given collection of points using specified rotation object.
        /// </summary>
        /// <param name="Source">Collection of points to be rotated.</param>
        /// <param name="rotation">Rotation to be used for rotation.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="Center">Optional center of rotation.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Collection of rotated points.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<Vector> Rotate(this IEnumerable<Vector> Source, IDegree rotation,
            bool antiClock = false, IPointF Center = null, bool noSkew = false)
        {
            if (rotation == null || !rotation.Valid)
                return Source;

            float Sin = 0, Cos = 1;
            float degree = rotation.Angle;
            float CX = 0, CY = 0;
            float SkewX = 1, SkewY = 1;
            bool HasSkewScale = false;
            bool diagonal = false;
            ISkew skew = null;
            ICentre center = Center != null ? new Centre(Center) : default(ICentre);

            if (rotation is ISkewHolder)
            {
                skew = ((ISkewHolder)rotation).Skew;
                HasSkewScale = skew?.HasScale == true && !noSkew;
                if (HasSkewScale)
                {
                    SkewX = skew.X;
                    SkewY = skew.Y;
                    diagonal = skew.Type == SkewType.Diagonal;
                }
            }
            bool HasAngle = rotation.HasAngle || diagonal;
            if (diagonal)
                degree += skew.Degree;
            if (center == null)
            {
                if (rotation is ICentreHolder)
                {
                    center = ((ICentreHolder)rotation).Centre;
                }
                else if (rotation is ICentre)
                {
                    center = ((ICentre)rotation);
                }
            }
            if (center != null)
            {
                CX = center.X;
                CY = center.Y;
            }
            if (HasAngle)
            {
                SinCos(degree, out Sin, out Cos);
                if (antiClock)
                    Sin = -Sin;
            }
            float x, y, x1, y1;
            PointKind type = PointKind.Normal;

            return Source.Select((item) =>
            {
                if (!item)
                    return item;
                x = item.X;
                y = item.Y;
                type = item.Kind;

                if (HasSkewScale)
                {
                    x = (x - CX) * SkewX + CX;
                    y = (y - CY) * SkewY + CY;
                }
                if (HasAngle)
                {
                    x -= CX;
                    y -= CY;
                    x1 = (x * Cos - y * Sin);
                    y1 = (x * Sin + y * Cos);
                    x = x1 + CX;
                    y = y1 + CY;
                }
                return new Vector(x, y, type);
            });

        }

        /// <summary>
        /// Rotates given collection of points using specified rotation object.
        /// </summary>
        /// <param name="Source">Collection of points to be rotated.</param>
        /// <param name="rotation">Rotation to be used for rotation.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="Center">Optional center of rotation.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Collection of rotated points.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<VectorF> Rotate(this IEnumerable<VectorF> Source,
            IDegree rotation, IScale scale, IPointF Center = null, bool antiClock = false, bool noSkew = false)
        {
            if ((rotation == null || !rotation.Valid) && (scale == null || !scale.HasScale))
                return Source;

            float Sin = 0, Cos = 1;
            float degree = rotation.Angle;
            float CX = 0, CY = 0;
            float SkewX = 1, SkewY = 1;
            bool HasSkewScale = false;
            bool diagonal = false;
            ISkew skew = null;
            ICentre center = Center != null ? new Centre(Center) : default(ICentre);

            if (rotation is ISkewHolder)
            {
                skew = ((ISkewHolder)rotation).Skew;
                HasSkewScale = skew?.HasScale == true && !noSkew;
                if (HasSkewScale)
                {
                    SkewX = skew.X;
                    SkewY = skew.Y;
                    diagonal = skew.Type == SkewType.Diagonal;
                }
            }
            bool HasAngle = rotation.HasAngle || diagonal;
            if (diagonal)
                degree += skew.Degree;
            if (center == null)
            {
                if (rotation is ICentreHolder)
                {
                    center = ((ICentreHolder)rotation).Centre;
                }
                else if (rotation is ICentre)
                {
                    center = ((ICentre)rotation);
                }
            }
            if (center != null)
            {
                CX = center.X;
                CY = center.Y;
            }
            if (HasAngle)
            {
                SinCos(degree, out Sin, out Cos);
                if (antiClock)
                    Sin = -Sin;
            }
            float x, y, x1, y1;
            PointKind type;
            float Sx = 0, Sy = 0;
          
            bool HasScale = false;
            if (scale != null)
            {
                Sx = scale.X;
                Sy = scale.Y;
                HasScale = scale.HasScale;
            }

            return Source.Select((item) =>
            {
                if (!item)
                    return item;
                x = item.X;
                y = item.Y;
                type = item.Kind;

                if (HasScale)
                {
                    x = (x - CX) * Sx + CX;
                    y = (y - CY) * Sy + CY;
                }
                if (HasSkewScale)
                {
                    x = (x - CX) * SkewX + CX;
                    y = (y - CY) * SkewY + CY;
                }
                if (HasAngle)
                {
                    x -= CX;
                    y -= CY;
                    x1 = (x * Cos - y * Sin);
                    y1 = (x * Sin + y * Cos);
                    x = x1 + CX;
                    y = y1 + CY;
                }
                return new VectorF(x, y, type);
            });
        }

        /// <summary>
        /// Rotates given collection of points using specified rotation object.
        /// </summary>
        /// <param name="Source">Collection of points to be rotated.</param>
        /// <param name="degree">Degree of rotation.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="Skew">Optional skew object to perform skewing operation.</param>
        /// <param name="skewType">Optional skew type option to perform skewing operation.</param>
        /// <param name="skewDegree">Optional degree of skew to perform skewing operation.</param>
        /// <param name="center">Optional center of rotation.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Collection of rotated points.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IEnumerable<VectorF> Rotate(this IEnumerable<VectorF> Source, float degree,
           bool antiClock = false, ISkew Skew = null, SkewType? skewType = null,
            float? skewDegree = null, IPointF center = null, bool noSkew = false)
        {
            ISkew skew = Skew.ToSkew(skewType, skewDegree);
            bool HasScale = skew.HasScale && !noSkew; 
            bool diagonal = HasScale && skew.Type == SkewType.Diagonal;
            bool HasAngle = (degree != 0 && degree != 360 && degree != -360) || diagonal;
            if (!HasAngle && !HasScale)
                return Source;

            float sx = skew.X;
            float sy = skew.Y;
            if (diagonal)
                degree += skew.Degree;
            float x, y, x1, y1;
            PointKind type;
            float Sin = 0, Cos = 1;

            if (HasAngle)
            {
                SinCos(degree, out Sin, out Cos);
                if (antiClock)
                    Sin = -Sin;
            }
            var CX = center?.X ?? 0;
            var CY = center?.Y ?? 0;

            return Source.Select((item) =>
            {
                if (!item)
                    return item;
                x = item.X;
                y = item.Y;
                type = item.Kind;

                if (HasScale)
                {
                    x = (x - CX) * sx + CX;
                    y = (y - CY) * sy + CY;
                }
                if (HasAngle)
                {
                    x -= CX;
                    y -= CY;
                    x1 = (x * Cos - y * Sin);
                    y1 = (x * Sin + y * Cos);
                    x = x1 + CX;
                    y = y1 + CY;
                }
                return new VectorF(x, y, type);
            });
        }

        /// <summary>
        /// Rotates given collection of points using specified rotation object.
        /// </summary>
        /// <param name="Source">Collection of points to be rotated.</param>
        /// <param name="rotation">Rotation to be used for rotation.</param>
        /// <param name="cx">X co-ordinate of center point to override center of rotation.</param>
        /// <param name="cy">Y co-ordinate of center point to override center of rotation.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns>Collection of rotated points.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<VectorF> Rotate(this IEnumerable<VectorF> Source, 
            IDegree rotation, float cx, float cy, bool antiClock = false, bool noSkew = false) =>
            Source.Rotate(rotation, antiClock, new VectorF(cx, cy), noSkew);
        #endregion

        #region ROTATE 180
        /// <summary>
        /// Rotates a point on the shape 180 degrees around the center of its rectangular bounds.
        /// </summary>
        /// <param name="p">PointF to rotate at 180 degree</param>
        /// <param name="height">Height by which to rotate the point</param>
        /// <returns>New PointF which rotated 180 degree</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Rotate180(this IPointF pt, float height)
        {
            var Y = pt.Y - height;
            if (Y < 0)
                Y = -Y;
            return new VectorF(pt.X, Y, pt is IPointType ? ((IPointType)pt).Kind : 0);
        }
        #endregion

        #region FLIP
        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="p">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF FlipVertical(this IPointF pt, float height)
        {
            if (pt == null)
                return VectorF.Empty;

            var Y = pt.Y - height;
            if (Y < 0)
                Y = -Y;
            return new VectorF(pt.X, Y, pt is IPointType ? ((IPointType)pt).Kind : 0);
        }

        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="p">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector FlipVertical(this IPoint pt, int height)
        {
            if (pt == null)
                return Vector.Empty;

            var Y = pt.Y- height;
            if (Y < 0)
                Y = -Y;
            return new Vector(pt.X, Y, pt is IPointType? ((IPointType)pt).Kind: 0);
        }

        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="p">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF FlipHorizontal(this IPointF pt, float width)
        {
            if (pt == null)
                return VectorF.Empty;
            var X = pt.X - width;
            if (X < 0)
                X = -X;
            return new VectorF(X, pt.Y, pt is IPointType ? ((IPointType)pt).Kind : 0);
        }

        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="p">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector FlipHorizontal(this IPoint pt, int width)
        {
            if (pt == null)
                return Vector.Empty;

            var X = pt.X - width;
            if (X < 0)
                X = -X;
            return new Vector(X, pt.Y, pt is IPointType ? ((IPointType)pt).Kind : 0);
        }
        #endregion

        #region GET ANGLE
        /// <summary>
        /// Find the angle of a given x and y coordinates in relation to a central point after reverting it back using current angle from its current rotation.
        /// </summary>
        /// <param name="rotation">Rtoation by which x and y are currently rotated.</param>
        /// <param name="x">x position of point.</param>
        /// <param name="y">y position of point.</param>
        /// <param name="cx">x position center.</param>
        /// <param name="cy">y position center.</param>
        /// <param name="seek"></param>
        /// <returns>Angle in relation to the centre.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngle(this IDegree rotation, float x, float y, float cx, float cy)
        {
            if (rotation.Valid)
                rotation.Rotate(x, y, out x, out y, cx, cy, true);
            float angle;
            angle = (float)Math.Atan2(y - cy, x - cx);

            angle *= Angles.Radinv;
            if (angle < 0)
                angle += 360;
            return angle;
        }

        /// <summary>
        /// Find the angle of a point in relation to a central point after reverting it back using current angle from its current rotation.
        /// </summary>
        /// <param name="p">Point to get angle for</param>
        /// <param name="currentAngle">ngle by which the point is currently rotated.</param>
        /// <param name="cx">X co-ordinate of center.</param>
        /// <param name="cy">Y co-ordinate of center.</param>
        /// <returns>ngle in relation to the centre.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngle(this IPointF p, float currentAngle, float cx, float cy)
        {
            float x = p.X;
            float y = p.Y;
            if (currentAngle != 0 && currentAngle != 360 && currentAngle != -360)
                Rotate(p.X, p.Y, currentAngle, cx, cy, out x, out y, true);
            return GetAngle(x, y, cx, cy);
        }

        /// <summary>
        /// Find the angle of a point in relation to a central point.
        /// </summary>
        /// <param name="p">Point to get angle for</param>
        /// <param name="cx">X co-ordinate of center.</param>
        /// <param name="cy">Y co-ordinate of center.</param>
        /// <returns>ngle in relation to the centre.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngle(this IPointF p, float cx, float cy)
        {
            return GetAngle(p.X, p.Y, cx, cy);
        }
        #endregion

        #region CALCULATE CENTER
        public static void RotateCenter(this IDegree rotation, ref float Cx, ref float Cy,
            float Rx, float Ry, out float X, out float Y)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(Cx, Cy, out Cx, out Cy);
            X = Cx - Rx;
            Y = Cy - Ry;
        }
        public static void RotateCenter(this IDegree rotation, ref int Cx, ref int Cy,
            int Rx, int Ry, out int X, out int Y)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(Cx, Cy, out Cx, out Cy);
            X = Cx - Rx;
            Y = Cy - Ry;
        }
        public static void RotateCenter(this IDegree rotation, ref int Cx, ref int Cy,
                float Rx, float Ry, out float X, out float Y)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(Cx, Cy, out Cx, out Cy);
            X = Cx - Rx;
            Y = Cy - Ry;
        }

        public static void RotateCenter(this IDegree rotation, ref int Cx, ref int Cy,
            float Rx, float Ry, out int X, out int Y)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(Cx, Cy, out Cx, out Cy);
            X = (int)(Cx - Rx);
            Y = (int)(Cy - Ry);
        }
        public static void RotateCenter(this IDegree rotation, ref int Cx, ref int Cy, float Rx, float Ry,
            int applicableCx, int applicableCy, out int X, out int Y)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(Cx, Cy, out Cx, out Cy, applicableCx, applicableCy);
            X = (int)(Cx - Rx);
            Y = (int)(Cy - Ry);
        }
        public static void RotateDest(this IDegree rotation, int Cx, int Cy, float Rx, float Ry, out int X, out int Y)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(Cx, Cy, out Cx, out Cy, noSkew: true);
            X = (int)(Cx - Rx);
            Y = (int)(Cy - Ry);
        }
        public static void RotateDest(this IDegree rotation, float Cx, float Cy,
            float Rx, float Ry, out int X, out int Y)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(Cx, Cy, out Cx, out Cy, noSkew: true);
            X = (int)(Cx - Rx);
            Y = (int)(Cy - Ry);
        }
        public static void RotateDest(this IDegree rotation, float Cx, float Cy,
            float Rx, float Ry, out float X, out float Y)
        {
            if (rotation != null && rotation.Valid)
                rotation.Rotate(Cx, Cy, out Cx, out Cy, noSkew: true);
            X = (Cx - Rx);
            Y = (Cy - Ry);
        }
    #endregion

        #region ROTATE-SINGLE
        /// <summary>
        /// Rotates given point int relation with given size of container.
        /// If rotation object does not have a center, centres calculated using
        /// container width and height will be used to rotate the point.
        /// </summary>
        /// <param name="Rotation">Rotation object to be used to rotate point.</param>
        /// <param name="containerWidth">Width of a container.</param>
        /// <param name="containerHeight">Height of a container.</param>
        /// <param name="p1X">X co-ordinate of the point.</param>
        /// <param name="p1Y">Y co-ordinate of the point.</param>
        /// <param name="resultX1">X co-ordinate of rotated resultant point of the point.</param>
        /// <param name="resultY1">X co-ordinate of rotated resultant point of the point.</param>
        /// <param name="antiClock">If true rotation will be performed anti-clock wise otherwise clock wise.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateSingle(this IDegree Rotation,
            int containerWidth, int containerHeight,
            int p1X, int p1Y, out int resultX1, out int resultY1, bool antiClock = false, bool noSkew = false)
        {
            if (Rotation == null || !Rotation.Valid)
            {
                resultX1 = p1X;
                resultY1 = p1Y;
                return;
            }
            var Wby2 = containerWidth * .5f;
            var Hby2 = containerHeight * .5f;

            Rotation.EffectiveCenter(p1X + Wby2, p1Y + Hby2, out float Cx, out float Cy);
            Rotation.Rotate(p1X, p1Y, out resultX1, out resultY1, Cx, Cy, antiClock: antiClock, noSkew: noSkew);
        }


        /// <summary>
        /// Rotates given point int relation with given size of container.
        /// If rotation object does not have a center, centres calculated using
        /// container width and height will be used to rotate the point.
        /// </summary>
        /// <param name="Rotation">Rotation object to be used to rotate point.</param>
        /// <param name="containerWidth">Width of a container.</param>
        /// <param name="containerHeight">Height of a container.</param>
        /// <param name="p1X">X co-ordinate of the point.</param>
        /// <param name="p1Y">Y co-ordinate of the point.</param>
        /// <param name="resultX1">X co-ordinate of rotated resultant point of the point.</param>
        /// <param name="resultY1">X co-ordinate of rotated resultant point of the point.</param>
        /// <param name="antiClock">If true rotation will be performed anti-clock wise otherwise clock wise.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateSingle(this IDegree Rotation,
            float containerWidth, float containerHeight,
            float p1X, float p1Y, out float resultX1, out float resultY1, bool antiClock = false, bool noSkew = false)
        {
            if (Rotation == null || !Rotation.Valid)
            {
                resultX1 = p1X;
                resultY1 = p1Y;
                return;
            }
            var Wby2 = containerWidth * .5f;
            var Hby2 = containerHeight * .5f;
            Rotation.EffectiveCenter(p1X + Wby2, p1Y + Hby2, out float Cx, out float Cy);
            Rotation.Rotate(p1X, p1Y, out resultX1, out resultY1, Cx, Cy, antiClock: antiClock, noSkew: noSkew);
        }

        public static Point RotateSingle(this IDegree Rotation,
            int containerWidth, int containerHeight,
            IPoint p, bool antiClock = false, bool noSkew = false)
        {
            if (Rotation == null || !Rotation.Valid)
            {
                return new Point(p);
            }

            var Wby2 = containerWidth * .5f;
            var Hby2 = containerHeight * .5f;
            Rotation.EffectiveCenter(p.X + Wby2, p.Y + Hby2, out float Cx, out float Cy);
            Rotation.Rotate(p.X, p.Y, out int x, out int y, Cx, Cy, antiClock: antiClock, noSkew: noSkew);
            return new Point(x, y);
        }
     
        public static VectorF RotateSingle(this IDegree Rotation,
            int containerWidth, int containerHeight,
            IPointF pt, bool antiClock = false, bool noSkew = false)
        {
            if (Rotation == null || !Rotation.Valid)
            {
                return new VectorF(pt.X, pt.Y, pt is IPointType ? ((IPointType)pt).Kind : 0);
            }
            var Wby2 = containerWidth * .5f;
            var Hby2 = containerHeight * .5f;
            Rotation.EffectiveCenter(pt.X + Wby2, pt.Y + Hby2, out float Cx, out float Cy);
            Rotation.Rotate(pt.X, pt.Y, out float x, out float y, Cx, Cy, antiClock: antiClock, noSkew: noSkew);
            return new VectorF(x, y, pt is IPointType ? ((IPointType)pt).Kind : 0);
        }

        /// <summary>
        /// Rotates given point int relation with given size of container.
        /// If rotation object does not have a center, centres calculated using
        /// container width and height will be used to rotate the point.
        /// </summary>
        /// <param name="Rotation">Rotation object to be used to rotate point.</param>
        /// <param name="containerWidth">Width of a container.</param>
        /// <param name="containerHeight">Height of a container.</param>
        /// <param name="p1X">X co-ordinate of the point.</param>
        /// <param name="p1Y">Y co-ordinate of the point.</param>
        /// <param name="resultX1">X co-ordinate of rotated resultant point of the point.</param>
        /// <param name="resultY1">X co-ordinate of rotated resultant point of the point.</param>
        /// <param name="antiClock">If true rotation will be performed anti-clock wise otherwise clock wise.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateSingle(float angle,
            float containerWidth, float containerHeight,
            float p1X, float p1Y, out float resultX1, out float resultY1, bool antiClock = false)
        {
            if (angle == 0 || angle == 360 || angle == -360)
            {
                resultX1 = p1X;
                resultY1 = p1Y;
                return;
            }
            var Wby2 = containerWidth * .5f;
            var Hby2 = containerHeight * .5f;
            Angles.Rotate(p1X, p1Y, angle, p1X + Wby2, p1Y + Hby2, out resultX1, out resultY1, antiClock: antiClock);
        }

        /// <summary>
        /// Rotates given point int relation with given size of container.
        /// If rotation object does not have a center, centres calculated using
        /// container width and height will be used to rotate the point.
        /// </summary>
        /// <param name="Rotation">Rotation object to be used to rotate point.</param>
        /// <param name="containerWidth">Width of a container.</param>
        /// <param name="containerHeight">Height of a container.</param>
        /// <param name="p1X">X co-ordinate of the point.</param>
        /// <param name="p1Y">Y co-ordinate of the point.</param>
        /// <param name="resultX1">X co-ordinate of rotated resultant point of the point.</param>
        /// <param name="resultY1">X co-ordinate of rotated resultant point of the point.</param>
        /// <param name="antiClock">If true rotation will be performed anti-clock wise otherwise clock wise.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateSingle(float angle,
            int containerWidth, int containerHeight,
            int p1X, int p1Y, out int resultX1, out int resultY1, bool antiClock = false)
        {
            if (angle == 0 || angle == 360 || angle == -360)
            {
                resultX1 = p1X;
                resultY1 = p1Y;
                return;
            }
            var Wby2 = (containerWidth * .5f).Round();
            var Hby2 = (containerHeight * .5f).Round();
            Angles.Rotate(p1X, p1Y, angle, p1X + Wby2, p1Y + Hby2, out resultX1, out resultY1, antiClock: antiClock);
        }
        #endregion

        #region ROTATE TOGETHER
        /// <summary>
        /// Rotates given two points int relation with given size of container.
        /// If rotation object does not have a center, centres calculated using
        /// container width and height will be used to rotate points.
        /// </summary>
        /// <param name="Rotation">Rotation object to be used to rotate points.</param>
        /// <param name="containerWidth">Width of a container.</param>
        /// <param name="containerHeight">Height of a container.</param>
        /// <param name="p1X">X co-ordinate of first point.</param>
        /// <param name="p1Y">Y co-ordinate of first point.</param>
        /// <param name="p2X">X co-ordinate of second point.</param>
        /// <param name="p2Y">Y co-ordinate of second point.</param>
        /// <param name="resultX1">X co-ordinate of rotated resultant point of first point.</param>
        /// <param name="resultX2">Y co-ordinate of rotated resultant point of first point.</param>
        /// <param name="resultY1">X co-ordinate of rotated resultant point of second point.</param>
        /// <param name="resultY2">Y co-ordinate of rotated resultant point of second point.</param>
        /// <param name="antiClock">If true rotation will be performed anti-clock wise otherwise clock wise.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateTogether(this IDegree Rotation, 
            int containerWidth, int containerHeight, 
            int p1X, int p1Y, int p2X, int p2Y, out int resultX1, out int resultY1,
            out int resultX2, out int resultY2, bool antiClock = false, bool noSkew = false)
        {
            if(Rotation == null || !Rotation.Valid)
            {
                resultX1 = p1X;
                resultY1 = p1Y;
                resultX2 = p2X;
                resultY2 = p2Y;
                return;
            }
            var Wby2 = containerWidth * .5f;
            var Hby2 = containerHeight * .5f;

            Rotation.EffectiveCenter(p1X + Wby2, p1Y + Hby2, out float Cx, out float Cy);
            Rotation.EffectiveCenter(p2X + Wby2, p2Y + Hby2, out float Cx1, out float Cy1);

            Rotation.Rotate(p2X, p2Y, out resultX2, out resultY2, Cx1, Cy1, antiClock: antiClock, noSkew: noSkew);
            Rotation.Rotate(p1X, p1Y, out resultX1, out resultY1, Cx, Cy, antiClock: antiClock, noSkew: noSkew);
        }

        /// <summary>
        /// Rotates given two points int relation with given size of container.
        /// If rotation object does not have a center, centres calculated using
        /// container width and height will be used to rotate points.
        /// </summary>
        /// <param name="Rotation">Rotation object to be used to rotate points.</param>
        /// <param name="containerWidth">Width of a container.</param>
        /// <param name="containerHeight">Height of a container.</param>
        /// <param name="p1X">X co-ordinate of first point.</param>
        /// <param name="p1Y">Y co-ordinate of first point.</param>
        /// <param name="p2X">X co-ordinate of second point.</param>
        /// <param name="p2Y">Y co-ordinate of second point.</param>
        /// <param name="resultX1">X co-ordinate of rotated resultant point of first point.</param>
        /// <param name="resultX2">Y co-ordinate of rotated resultant point of first point.</param>
        /// <param name="resultY1">X co-ordinate of rotated resultant point of second point.</param>
        /// <param name="resultY2">Y co-ordinate of rotated resultant point of second point.</param>
        /// <param name="antiClock">If true rotation will be performed anti-clock wise otherwise clock wise.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateTogether(this IDegree Rotation,
            float containerWidth, float containerHeight,
            float p1X, float p1Y, float p2X, float p2Y, out float resultX1, out float resultY1,
            out float resultX2, out float resultY2, bool antiClock = false, bool noSkew = false)
        {
            if (Rotation == null || !Rotation.Valid)
            {
                resultX1 = p1X;
                resultY1 = p1Y;
                resultX2 = p2X;
                resultY2 = p2Y;
                return;
            }
            var Wby2 = containerWidth * .5f;
            var Hby2 = containerHeight * .5f;

            Rotation.EffectiveCenter(p1X + Wby2, p1Y + Hby2, out float Cx, out float Cy);
            Rotation.EffectiveCenter(p2X + Wby2, p2Y + Hby2, out float Cx1, out float Cy1);

            Rotation.Rotate(p2X, p2Y, out resultX2, out resultY2, Cx1, Cy1, antiClock, noSkew);
            Rotation.Rotate(p1X, p1Y, out resultX1, out resultY1, Cx, Cy, antiClock, noSkew);
        }

        /// <summary>
        /// Rotates given two points int relation with given size of container.
        /// If rotation object does not have a center, centres calculated using
        /// container width and height will be used to rotate points.
        /// </summary>
        /// <param name="Rotation">Rotation object to be used to rotate points.</param>
        /// <param name="source">Collection of points to be rotated using given rotation object.</param>
        /// <param name="containerWidth">Width of a container.</param>
        /// <param name="containerHeight">Height of a container.</param>
        /// <param name="antiClock">If true rotation will be performed anti-clock wise otherwise clock wise.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<VectorF> RotateTogether(this IDegree Rotation, IEnumerable<VectorF> source,
            float containerWidth, float containerHeight, bool antiClock = false, bool noSkew = false)
        {
            if (Rotation == null || !Rotation.Valid)
            {
                return source; ;
            }
            var count = source.Count();
            ICentre Center = null;
            if (Rotation is ICentreHolder)
                Center = ((ICentreHolder)Rotation).Centre;
            else if (Rotation is ICentre)
                Center = (ICentre)Rotation;

            if (Center == null)
            {
                var Rx = containerWidth / 2f;
                var Ry = containerHeight / 2f;
               return source.Select( p => 
                    Rotation.Rotate(p, new VectorF(p.X + Rx, p.Y + Ry), antiClock, noSkew)).ToArray();
            }
            else
            {
                return source.Rotate(Rotation, antiClock: antiClock, noSkew: noSkew);
            }
        }

        /// <summary>
        /// Rotates given two points int relation with given size of container.
        /// If rotation object does not have a center, centres calculated using
        /// container width and height will be used to rotate points.
        /// </summary>
        /// <param name="Rotation">Rotation object to be used to rotate points.</param>
        /// <param name="containerWidth">Width of a container.</param>
        /// <param name="containerHeight">Height of a container.</param>
        /// <param name="antiClock">If true rotation will be performed anti-clock wise otherwise clock wise.</param>
        /// <returns></returns>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateTogether(this IDegree Rotation, 
            float containerWidth, float containerHeight, IPoint p1, IPoint p2, out IPoint rp1, 
            out IPoint rp2, bool antiClock = false, bool noSkew = false)
        {
            rp1 = p1;
            rp2 = p2;

            if (Rotation == null || !Rotation.Valid)
                return;

            RotateTogether(Rotation, containerWidth, containerHeight, p1.X, p1.Y, p2.X, p2.Y,
                out float p1x, out float p1y, out float p2x, out float p2y, antiClock, noSkew);
            rp1 = new Point(p1x, p1y);
            rp2 = new Point(p2x, p2y);
        }


        /// <summary>
        /// Rotates given two points int relation with given size of container.
        /// If rotation object does not have a center, centres calculated using
        /// container width and height will be used to rotate points.
        /// </summary>
        /// <param name="angle">Angle to be used to rotate points.</param>
        /// <param name="containerWidth">Width of a container.</param>
        /// <param name="containerHeight">Height of a container.</param>
        /// <param name="p1X">X co-ordinate of first point.</param>
        /// <param name="p1Y">Y co-ordinate of first point.</param>
        /// <param name="p2X">X co-ordinate of second point.</param>
        /// <param name="p2Y">Y co-ordinate of second point.</param>
        /// <param name="resultX1">X co-ordinate of rotated resultant point of first point.</param>
        /// <param name="resultX2">Y co-ordinate of rotated resultant point of first point.</param>
        /// <param name="resultY1">X co-ordinate of rotated resultant point of second point.</param>
        /// <param name="resultY2">Y co-ordinate of rotated resultant point of second point.</param>
        /// <param name="antiClock">If true rotation will be performed anti-clock wise otherwise clock wise.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RotateTogether(float angle,
            int containerWidth, int containerHeight,
            float p1X, float p1Y, float p2X, float p2Y, out float resultX1, out float resultY1,
            out float resultX2, out float resultY2, bool antiClock = false)
        {
            if (angle == 0 || angle == 360 || angle == -360)
            {
                resultX1 = p1X;
                resultY1 = p1Y;
                resultX2 = p2X;
                resultY2 = p2Y;
                return;
            }
            var Cx = containerWidth * .5f;
            var Cy = containerHeight * .5f;
            Angles.Rotate(p1X, p1Y, angle, p1X + Cx, p1Y + Cy, out resultX1, out resultY1, antiClock: antiClock);
            Angles.Rotate(p2X, p2Y, angle, p2X + Cx, p2Y + Cy, out resultX2, out resultY2, antiClock: antiClock);
        }
        #endregion
    }
}
#endif
