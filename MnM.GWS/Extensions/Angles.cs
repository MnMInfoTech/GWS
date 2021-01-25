/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Linq;

namespace MnM.GWS
{
    public static partial class Angles
    {
        #region VARAIBLES AND CONSTS

        /// <summary>
        /// Math.PI
        /// </summary>
        public const float PI = 3.14159265358979323846264338327950288419716939937510f;

        /// <summary>
        /// PI/180
        /// </summary>
        public const float Radian = PI / 180f;

        /// <summary>
        /// 180/PI
        /// </summary>
        public const float Radinv = 180f / PI;

        /// <summary>
        /// PI * 2
        /// </summary>
        public const float PIx2 = 2 * PI;

        /// <summary>
        /// For integer rotation.
        /// </summary>
        public const int BigExp = 20;

        /// <summary>
        /// Exponent of BigExp.
        /// </summary>
        public const int Big = (1 << BigExp);

        /// <summary>
        /// Represents 90 degree angle.
        /// </summary>
        public const float A90 = 90f;

        /// <summary>
        /// 1f/ 90f
        /// </summary>
        public const float Inv90 = 1f / 90f;

        /// <summary>
        /// Scale factor at 180 degree.
        /// </summary>
        public const float Scale180 = (A90 - 180) * Inv90;

        /// <summary>
        /// Negligible degree difference.
        /// </summary>
        const float EPSILON = .0001f;

        /// <summary>
        /// 0.1% of a degree
        /// </summary>
        const float AngleEpsilon = 0.001f * Radian;     // 0.1% of a degree


        static readonly Dictionary<int, Lot<float, float>> sincosDictionary = new Dictionary<int, Lot<float, float>>(720);
        static readonly Dictionary<int, Lot<int, int>> sinCosiDictionary = new Dictionary<int, Lot<int, int>>(720);
        const float sin001 = 1.74532925E-05f, cos001 = 1;
        const int sini001 = 18, cosi001 = 1048576;

        //const float kaapa = .5522847498307933984022516322796f;
        #endregion

        #region CONSTRUCTOR
        static Angles()
        {
            float sin, cos;
            for (int i = -360; i < 360; i += 1)
            {
                GetSinCos(i, out sin, out cos);
                sincosDictionary[i] = Lot.Create(sin, cos);
                sinCosiDictionary[i] = Lot.Create((sin * Big).Round(), (cos * Big).Round());
            }
        }
        internal static void Initialize() { }
        #endregion

        #region BASE ROTATE FUNCTIONS
        /// <summary>
        /// Rotates a point on around the center co-ordinates specified.
        /// </summary>
        /// <param name="angle">Angle to rotate point.</param>
        /// <param name="x">X co-ordinate of point.</param>
        /// <param name="y">Y co-ordinate of point.</param>
        /// <param name="cx">x position of the centre which to rotate the point by.</param>
        /// <param name="cy">y position of the centre which to rotate the point by.</param>
        /// <param name="x1">The x position of the rotated point.</param>
        /// <param name="y1">The y position of the rotated point.</param>
        /// <param name="type">True for clockwise rotation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(float x, float y, float angle, float cx, float cy, out float x1, out float y1, bool antiClock = false, SkewType skewType = 0)
        {
            if (angle == 0 || angle == 360 || angle == -360)
            {
                x1 = x;
                y1 = y;
                return;
            }

            float sx = 1, sy = 1;
            if (skewType != 0)
                skewType = GetScale(angle, skewType, out sx, out sy);

            if (sx != 1)
                x = (x - cx) * sx + cx;
            if (sy != 1)
                y = (y - cy) * sy + cy;

            if (skewType == SkewType.Horizontal || skewType == SkewType.Vertical)
            {
                x1 = x;
                y1 = y;
                return;
            }

            SinCos(angle, out float Sin, out float Cos);
            if (antiClock)
                Sin = -Sin;

            x -= cx;
            y -= cy;

            x1 = x * Cos - y * Sin;
            y1 = x * Sin + y * Cos;

            x1 += cx;
            y1 += cy;
        }

        /// <summary>
        /// Rotates a point on around the center co-ordinates specified.
        /// Source: https://www.pyromuffin.com/2018/07/rotating-vector-using-integer-math.html
        /// </summary>
        /// <param name="x">X co-ordinate of point.</param>
        /// <param name="y">Y co-ordinate of point.</param>
        /// <param name="angle">Angle to rotate point.</param>
        /// <param name="cx">x position of the centre which to rotate the point by.</param>
        /// <param name="cy">y position of the centre which to rotate the point by.</param>
        /// <param name="x1">The x position of the rotated point.</param>
        /// <param name="y1">The y position of the rotated point.</param>
        /// <param name="type">True for clockwise rotation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(int x, int y, int cx, int cy, float angle, out int x1, out int y1, bool antiClock = false, SkewType skewType = 0)
        {
            if (angle == 0 || angle == 360 || angle == -360)
            {
                x1 = x;
                y1 = y;
                return;
            }

            float sx = 1, sy = 1;
            if (skewType != 0)
                skewType = GetScale(angle, skewType, out sx, out sy);

            if (sx != 1)
                x = ((x - cx) * sx + cx).Round();
            if (sy != 1)
                y = ((y - cy) * sy + cy).Round();

            if (skewType == SkewType.Horizontal || skewType == SkewType.Vertical)
            {
                x1 = x;
                y1 = y;
                return;
            }

            SinCos(angle, out int Sini, out int Cosi);
            if (antiClock)
                Sini = -Sini;

            x -= cx;
            y -= cy;
            x1 = (x * Cosi - y * Sini) >> BigExp;
            y1 = (x * Sini + y * Cosi) >> BigExp;
            x1 += cx;
            y1 += cy;
        }
        #endregion

        #region GET SKEW SCALLING
        public static SkewType GetScale(float degree, SkewType? skewType, out float ScaleX, out float ScaleY)
        {
            ScaleX = ScaleY = 0;

            if (degree == 0 || degree == 360 || degree == -360)
                return 0;

            if (degree < 0)
                degree += 360;
            if (degree > 360)
                degree %= 360;

            var Skew = skewType ?? 0;
            float scale = degree;

            if (degree > 180)
                scale = scale - 180;

            scale = A90 - scale;
            scale *= Inv90;

            if (degree > 180)
                scale *= Scale180;

            ScaleX = ScaleY = 1;

            if (Skew.HasFlag(SkewType.Horizontal) ||
                Skew.HasFlag(SkewType.Diagonal))
                ScaleX = scale;

            if (Skew.HasFlag(SkewType.Vertical))
                ScaleY = scale;

            return Skew;
        }
        #endregion

        #region SIN COS
        /// <summary>
        /// Returns the Sin and Cosine values for the angle.
        /// </summary>
        /// <param name="angle">Angle in degrees for lookup/calculation.</param>
        /// <param name="sin">Returns Sin of angle.</param>
        /// <param name="cos">Returns Cosine of angle.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(float angle, out float sin, out float cos)
        {
            if (angle == 0.001f)
            {
                sin = sin001;
                cos = cos001;
                return;
            }
            if (angle >= 360)
                angle %= 360;
            int iAngle = (int)angle;

            if (sinCosiDictionary.ContainsKey(iAngle) && Math.Abs(angle - iAngle) < EPSILON)
            {
                sin = sincosDictionary[iAngle].Item1;
                cos = sincosDictionary[iAngle].Item2;
                return;
            }
            GetSinCos(angle, out sin, out cos);
        }

        /// <summary>
        /// Returns the Sin and Cosine values for the angle.
        /// </summary>
        /// <param name="angle">Angle in degrees for lookup/calculation.</param>
        /// <param name="sin">Returns integer Sin of angle i.e (Math.Sin * (1 << 20)).Round().</param>
        /// <param name="cos">Returns integer Cosine of angle i.e (Math.Cos * (1 << 20)).Round().</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(float angle, out int sin, out int cos)
        {
            if (angle == 0.001f)
            {
                sin = sini001;
                cos = cosi001;
                return;
            }
            if (angle >= 360)
                angle %= 360;
            int iAngle = (int)angle;
            if (sinCosiDictionary.ContainsKey(iAngle) && Math.Abs(angle - iAngle) < EPSILON)
            {
                sin = sinCosiDictionary[iAngle].Item1;
                cos = sinCosiDictionary[iAngle].Item2;
                return;
            }

            GetSinCos(angle, out float s, out float c);
            sin = (s * Big).Round();
            cos = (c * Big).Round();
        }
        /// <summary>
        /// Returns the Sin and Cosine values for the angle.
        /// </summary>
        /// <param name="angle">Angle in degrees for lookup/calculation.</param>
        /// <param name="sin">Returns Sin of angle.</param>
        /// <param name="cos">Returns Cosine of angle,</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void GetSinCos(float angle, out float sin, out float cos)
        {
            float radians = angle * Radian;
            radians = (float)Math.IEEERemainder(radians, PI * 2);
            if (radians > -AngleEpsilon && radians < AngleEpsilon)
            {
                // Exact case for zero rotation.
                cos = 1;
                sin = 0;
            }
            else if (radians > Math.PI / 2 - AngleEpsilon && radians < Math.PI / 2 + AngleEpsilon)
            {
                // Exact case for 90 degree rotation.
                cos = 0;
                sin = 1;
            }
            else if (radians < -Math.PI + AngleEpsilon || radians > Math.PI - AngleEpsilon)
            {
                // Exact case for 180 degree rotation.
                cos = -1;
                sin = 0;
            }
            else if (radians > -Math.PI / 2 - AngleEpsilon && radians < -Math.PI / 2 + AngleEpsilon)
            {
                // Exact case for 270 degree rotation.
                cos = 0;
                sin = -1;
            }
            else
            {
                // Arbitrary rotation.
                cos = (float)Math.Cos(radians);
                sin = (float)Math.Sin(radians);
            }
        }
        #endregion

        #region GET ANGLE
        /// <summary>
        /// Find the angle of a point in relation to a central point.
        /// </summary>
        /// <param name="x">x position of point.</param>
        /// <param name="y">y position of point.</param>
        /// <param name="cx">x position center.</param>
        /// <param name="cy">y position center.</param>
        /// <param name="inDegree">Returns degrees if true and radians otherwise.</param>
        /// <param name="convertToPositiveIfNegative">Returns Positive only values if true.</param>
        /// <returns>Angle in relation to the centre.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngle(float x, float y, float cx, float cy)
        {
            float angle = (float)Math.Atan2(y - cy, x - cx);
            angle *= Angles.Radinv;
            if (angle < 0)
                angle += 360;
            return angle;
        }
        #endregion

        #region FLIP
        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="y">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FlipVertical(int y, int height)
        {
            y -= height;
            if (y < 0)
                y = Math.Abs(y);
            return y;
        }

        /// <summary>
        /// Flip along the y axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="x">Point to be flipped (y is unchanged).</param>
        /// <param name="width">Width of rectangle.</param>
        /// <returns>New value of points x co-ordinate.</returns>
        public static int FlipHorizontal(int x, int width)
        {
            x -= width;
            if (x < 0)
                x = Math.Abs(x);
            return x;
        }

        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="y">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float FlipVertical(float y, float height)
        {
            y -= height;
            if (y < 0)
                y = Math.Abs(y);
            return y;
        }

        /// <summary>
        /// Flip along the y axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="x">Point to be flipped (y is unchanged).</param>
        /// <param name="width">Width of rectangle.</param>
        /// <returns>New value of points x co-ordinate.</returns>
        public static float FlipHorizontal(float x, float width)
        {
            x -= width;
            if (x < 0)
                x = Math.Abs(x);
            return x;
        }
        #endregion

        #region ROTATE 180
        /// <summary>
        /// Rotates a point on the shape 180 degrees around the center of its rectangular bounds.
        /// </summary>
        /// <param name="x">x position of point on the shape.</param>
        /// <param name="y">y position of point on the shape.</param>
        /// <param name="height"></param>
        /// <param name="newX">Rotated x co-ordinate of point.</param>
        /// <param name="newY">Rotated y co-ordinate of point.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate180(int x, int y, int height, out int newX, out int newY)
        {
            y -= height;
            if (y < 0)
                y = Math.Abs(y);
            newX = x;
            newY = y;
        }

        /// <summary>
        /// Rotates a point on the shape 180 degrees around the center of its rectangular bounds.
        /// </summary>
        /// <param name="x">x position of point on the shape.</param>
        /// <param name="y">y position of point on the shape.</param>
        /// <param name="height"></param>
        /// <param name="newX">Rotated x co-ordinate of point.</param>
        /// <param name="newY">Rotated y co-ordinate of point.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate180(float x, float y, float height, out float newX, out float newY)
        {
            y -= height;
            if (y < 0)
                y = Math.Abs(y);//!!!!If the Rectangle centre is 0 then You get strange results. The image is folded on to itself about y = 0!!!!
            newX = x;
            newY = y;
        }
        #endregion
    }

#if GWS || Window
    public static partial class Angles
    {
        #region ROTATION ROTATE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(this Rotation angle, float x, float y, out float x1, out float y1,
            float? cx = null, float? cy = null, bool? antiClock = null, SkewType? skewType = null)
        {
            var CX = cx ?? 0;
            var CY = cy ?? 0;
            bool AntiClock = antiClock ?? false;
            float Sin = angle.Sin;
            float Cos = angle.Cos;

            float sx = 1, sy = 1;
            SkewType skew = 0;

            if (skewType != null && skewType != 0)
                GetScale(angle.Degree, skewType, out sx, out sy);

            else if (angle)
            {
                if (cx == null)
                    CX = angle.Cx;
                if (cy == null)
                    CY = angle.Cy;

                if (angle.Skew != 0)
                {
                    sx = angle.ScaleX;
                    sy = angle.ScaleY;
                }
                skew = skewType ?? angle.Skew;
            }

            if (sx != 1)
                x = (x - CX) * sx + CX;
            if (sy != 1)
                y = (y - CY) * sy + CY;

            if (skew == SkewType.Horizontal || skew == SkewType.Vertical)
            {
                x1 = x;
                y1 = y;
                return;
            }

            if (AntiClock)
                Sin = -Sin;

            x -= CX;
            y -= CY;
            x1 = (x * Cos - y * Sin);
            y1 = (x * Sin + y * Cos);
            x1 += CX;
            y1 += CY;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(this Rotation angle, int x, int y, out int x1, out int y1, int? cx = null, int? cy = null,
            bool? antiClock = null, SkewType? skewType = null)
        {
            var CX = cx ?? 0;
            var CY = cy ?? 0;
            bool AntiClock = antiClock ?? false;
            int Sini = angle.Sini;
            int Cosi = angle.Cosi;

            float sx = 1, sy = 1;

            SkewType skew = 0;

            if (skewType != null && skewType != 0)
                GetScale(angle.Degree, skewType, out sx, out sy);

            else if (angle)
            {
                if (cx == null)
                    CX = angle.Cx;
                if (cy == null)
                    CY = angle.Cy;

                if (angle.Skew != 0)
                {
                    sx = angle.ScaleX;
                    sy = angle.ScaleY;
                }
                skew = skewType ?? angle.Skew;
            }

            if (sx != 1)
                x = ((x - CX) * sx + CX).Round();
            if (sy != 1)
                y = ((y - CY) * sy + CY).Round();

            if (skew == SkewType.Horizontal || skew == SkewType.Vertical)
            {
                x1 = x;
                y1 = y;
                return;
            }

            if (AntiClock)
                Sini = -Sini;

            x -= CX;
            y -= CY;
            x1 = (x * Cosi - y * Sini) >> BigExp;
            y1 = (x * Sini + y * Cosi) >> BigExp;
            x1 += CX;
            y1 += CY;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Rotates a line by given angle if angle is valid. 
        /// If rotating and angle does not have any center, center of a line will be used to rotate otherwise center of angle will be used.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point of line.</param>
        /// <param name="y1">Y co-ordinate of start point of line.</param>
        /// <param name="x2">X co-ordinate of end point of line.</param>
        /// <param name="y2">Y co-ordinate of end point of line.</param>
        /// <param name="type">True for clockwise rotation.</param>
        public static void Rotate(this Rotation angle, ref float x1, ref float y1, ref float x2, ref float y2,
            bool? antiClock = null, SkewType? skewType = null)
        {
            bool isRotated = angle.EffectiveCenter(Numbers.Middle(x1, x2), Numbers.Middle(y1, y2), out float cx, out float cy);
            if (!isRotated)
                return;
            angle.Rotate(x1, y1, out x1, out y1, cx, cy, antiClock, skewType);
            angle.Rotate(x2, y2, out x2, out y2, cx, cy, antiClock, skewType);
        }

        /// <summary>
        /// Rotates a line by given angle if angle is valid. 
        /// If rotating and angle does not have any center, center of a line will be used to rotate otherwise center of angle will be used.
        /// </summary>
        /// <param name="x1">X co-ordinate of start point of line.</param>
        /// <param name="y1">Y co-ordinate of start point of line.</param>
        /// <param name="x2">X co-ordinate of end point of line.</param>
        /// <param name="y2">Y co-ordinate of end point of line.</param>
        /// <param name="type">True for clockwise rotation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(this Rotation angle, ref int x1, ref int y1, ref int x2, ref int y2, bool? antiClock = null, SkewType? skewType = null)
        {
            bool isRotated = angle.EffectiveCenter(Numbers.Middle(x1, x2), Numbers.Middle(y1, y2), out int cx, out int cy);
            if (!isRotated)
                return;
            angle.Rotate(x1, y1, out x1, out y1, cx, cy, antiClock, skewType);
            angle.Rotate(x2, y2, out x2, out y2, cx, cy, antiClock, skewType);
        }
        #endregion

        #region ROTATE VECTOR  
        /// <summary>
        /// Rotates point on angle's center.
        /// </summary>
        /// <param name="angle">>Angle to rotate point.</param>
        /// <param name="x">The x co-ordinate of rotated point.</param>
        /// <param name="y">The y co-ordinate of rotated point.</param>
        /// <param name="type">True for clockwise rotation.</param>
        /// <returns>Rotated point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Rotate(this Rotation angle, float x, float y, bool? antiClock = null, SkewType? skewType = null, byte quadratic = 0)
        {
            if (angle)
                angle.Rotate(x, y, out x, out y, antiClock: antiClock, skewType: skewType);
            return new VectorF(x, y, quadratic);
        }

        /// <summary>
        /// Rotates point on angle's center.
        /// </summary>
        /// <param name="angle">>Angle to rotate point.</param>
        /// <param name="x">The x co-ordinate of rotated point.</param>
        /// <param name="y">The y co-ordinate of rotated point.</param>
        /// <param name="type">True for clockwise rotation.</param>
        /// <returns>Rotated point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Rotate(this Rotation angle, int x, int y, bool? antiClock = null, SkewType? skewType = null, byte quadratic = 0)
        {
            if (angle)
                angle.Rotate(x, y, out x, out y, antiClock: antiClock, skewType: skewType);
            return new Vector(x, y, quadratic);
        }

        /// <summary>
        /// Rotates point on angle's center.
        /// </summary>
        /// <param name="angle">>Angle to rotate point.</param>
        /// <param name="p">Point to rotate.</param>
        /// <param name="type">True for clockwise rotation.</param>
        /// <returns>Rotated point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Rotate(this Rotation angle, VectorF p, bool? antiClock = null, SkewType? skewType = null)
        {
            if (!p || !angle)
            {
                return p;
            }
            return angle.Rotate(p.X, p.Y, antiClock: antiClock, skewType: skewType, p.Quadratic);
        }

        /// <summary>
        /// Rotates point on angle's center.
        /// </summary>
        /// <param name="angle">>Angle to rotate point.</param>
        /// <param name="p">Point to rotate.</param>
        /// <param name="type">True for clockwise rotation.</param>
        /// <returns>Rotated point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Rotate(this Rotation angle, Vector p, bool? antiClock = null, SkewType? skewType = null)
        {
            if (!p || !angle)
            {
                return p;
            }
            return angle.Rotate(p.X, p.Y, antiClock: antiClock, skewType: skewType, p.Quadratic);
        }

        /// <summary>
        /// Rotates point around the center specified.
        /// </summary>
        /// <param name="angle">>Angle to rotate point.</param>
        /// <param name="p">Point to rotate.</param>
        /// <param name="center">The center to rotate point around.</param>
        /// <param name="type">True for clockwise rotation.</param>
        /// <returns>Rotated point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Rotate(this Rotation angle, VectorF p, VectorF center, bool? antiClock = null, SkewType? skewType = null)
        {
            if (!p || !angle)
            {
                return p;
            }
            angle.Rotate(p.X, p.Y, out float x, out float y, center.X, center.Y, antiClock, skewType);
            return new VectorF(x, y, p.Quadratic);
        }

        /// <summary>
        /// Rotates point around the center specified.
        /// </summary>
        /// <param name="angle">>Angle to rotate point.</param>
        /// <param name="p">Point to rotate.</param>
        /// <param name="center">The center to rotate point around.</param>
        /// <param name="type">True for clockwise rotation.</param>
        /// <returns>Rotated point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector Rotate(this Rotation angle, Vector p, Vector center, bool? antiClock = null, SkewType? skewType = null)
        {
            if (!p || !angle)
            {
                return p;
            }
            angle.Rotate(p.X, p.Y, out int x, out int y, center.X, center.Y, antiClock, skewType);
            return new Vector(x, y, p.Quadratic);
        }

        public static VectorF Rotate(this VectorF p, Rotation angle, bool? antiClock = null, SkewType? skewType = null) =>
            angle.Rotate(p, antiClock, skewType);

        public static Vector Rotate(this Vector p, Rotation angle, bool? antiClock = null, SkewType? skewType = null) =>
            angle.Rotate(p, antiClock, skewType);
        #endregion

        #region ROTATE VECTORS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<VectorF> Rotate(this IEnumerable<VectorF> Source, Rotation angle, bool? antiClock = null, SkewType? skewType = null)
        {
            if (angle)
                return Source.Select(x => angle.Rotate(x, antiClock, skewType)).ToArray();

            if (Source is IList<VectorF>)
                return Source as IList<VectorF>;
            return Source.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<Vector> Rotate(this Rotation angle, IEnumerable<Vector> Source, bool? antiClock = null, SkewType? skewType = null)
        {
            if (angle)
                return Source.Select(x => angle.Rotate(x, antiClock, skewType)).ToArray();

            if (Source is IList<Vector>)
                return Source as IList<Vector>;
            return Source.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<VectorF> Rotate(this IEnumerable<VectorF> Source, Rotation angle, VectorF center, bool? antiClock = null, SkewType? skewType = null)
        {
            if (angle)
                return Source.Select(p => angle.Rotate(p, center, antiClock, skewType)).ToArray();

            if (Source is IList<VectorF>)
                return Source as IList<VectorF>;

            return Source.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<Vector> Rotate(this Rotation angle, IEnumerable<Vector> Source, Vector center, bool? antiClock = null, SkewType? skewType = null)
        {
            if (angle)
                return Source.Select(p => angle.Rotate(p, center, antiClock, skewType)).ToArray();
            if (Source is IList<Vector>)
                return Source as IList<Vector>;
            return Source.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<VectorF> Rotate(this IEnumerable<VectorF> Source, Rotation angle, float cx, float cy,
            bool? antiClock = null, SkewType? skewType = null) =>
            Source.Rotate(angle, new VectorF(cx, cy), antiClock, skewType);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<Vector> Rotate(this Rotation angle, IEnumerable<Vector> Source, int cx, int cy,
            bool? antiClock = null, SkewType? skewType = null) =>
            angle.Rotate(Source, new Vector(cx, cy), antiClock, skewType);
        #endregion

        #region ROTATE 180
        /// <summary>
        /// Rotates a point on the shape 180 degrees around the center of its rectangular bounds.
        /// </summary>
        /// <param name="p">PointF to rotate at 180 degree</param>
        /// <param name="height">Height by which to rotate the point</param>
        /// <returns>New PointF which rotated 180 degree</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Rotate180(this VectorF p, float height)
        {
            var y = p.Y;
            y -= height;
            if (y < 0)
                y = Math.Abs(y);//!!!!If the Rectangle centre is 0 then You get strange results. The image is folded on to itself about y = 0!!!!
            return new VectorF(p.X, y, p.Quadratic);
        }
        #endregion

        #region FLIP
        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="p">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF FlipVertical(this VectorF p, float height)
        {
            float y = p.Y;
            y -= height;
            if (y < 0)
                y = Math.Abs(y);
            return new VectorF(p.X, y, p.Quadratic);
        }

        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="p">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector FlipVertical(this Vector p, int height)
        {
            int y = p.Y;
            y -= height;
            if (y < 0)
                y = Math.Abs(y);
            return new Vector(p.X, y);
        }


        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="p">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF FlipHorizontal(this VectorF p, float width)
        {
            float x = p.X;
            x -= width;
            if (x < 0)
                x = Math.Abs(x);
            return new VectorF(x, p.Y, p.Quadratic);
        }

        /// <summary>
        /// Flip along the x axis at the centre of the rectangle containing the shape.
        /// </summary>
        /// <param name="p">Point to be flipped (x is unchanged).</param>
        /// <param name="height">Height of rectangle.</param>
        /// <returns>New value of points y co-ordinate.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector FlipHorizontal(this Vector p, int width)
        {
            int x = p.X;
            x -= width;
            if (x < 0)
                x = Math.Abs(x);
            return new Vector(x, p.Y, p.Quadratic);
        }
        #endregion

        #region SIN COS
        /// <summary>
        /// Returns the Sin and Cosine values for the angle.
        /// Reverse Conversion => (co-ordinate * Cosi) << 20 && (co-ordinate * Sini) << 20;
        /// </summary>
        /// <param name="angle">Angle object for lookup/calculation.</param>
        /// <param name="sin">Returns Sin of angle.</param>
        /// <param name="cos">Returns Cosine of angle.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(this Rotation angle, out float sin, out float cos)
        {
            if (!angle)
            {
                sin = 0;
                cos = 1;
                return;
            }
            SinCos(angle.Degree, out sin, out cos);
        }
        /// <summary>
        /// Returns integer Sin and Cosine values for the angle.
        /// Reverse Conversion => (co-ordinate * Cosi) << 20 && (co-ordinate * Sini) << 20;
        /// </summary>
        /// <param name="angle">Angle object for lookup/calculation.</param>
        /// <param name="sin">Returns Sin of angle.</param>
        /// <param name="cos">Returns Cosine of angle.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(this Rotation angle, out int sin, out int cos)
        {
            if (!angle)
            {
                sin = 0;
                cos = 1048576;
                return;
            }
            SinCos(angle.Degree, out sin, out cos);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngle(this Rotation rotation, float x, float y, float cx, float cy)
        {
            if (rotation)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngle(this VectorF p, float currentAngle, float cx, float cy)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetAngle(this VectorF p, float cx, float cy)
        {
            return GetAngle(p.X, p.Y, cx, cy);
        }
        #endregion

        #region CALCULATE CENTER
        public static void RotateCenter(this Rotation angle, ref float Cx, ref float Cy, float Rx, float Ry, out float X, out float Y)
        {
            X = Cx - Rx;
            Y = Cy - Ry;
            if (!angle)
                return;
            angle.Rotate(Cx, Cy, out Cx, out Cy);
            X = Cx - Rx;
            Y = Cy - Ry;
        }
        public static void RotateCenter(this Rotation angle, ref int Cx, ref int Cy, float Rx, float Ry, out float X, out float Y)
        {
            X = Cx - Rx;
            Y = Cy - Ry;
            if (!angle)
                return;
            angle.Rotate(Cx, Cy, out Cx, out Cy);
            X = Cx - Rx;
            Y = Cy - Ry;
        }
        #endregion

        #region GET EFFECTIVE CENTER
        public static bool EffectiveCenter(this Rotation angle, float proposedCX, float proposedCy, out float Cx, out float Cy)
        {
            Cx = proposedCX;
            Cy = proposedCy;

            if (!angle)
                return false;

            if (angle)
            {
                if (angle.HasCenter)
                {
                    Cx = angle.Cx;
                    Cy = angle.Cy;
                }
            }
            return true;
        }
        public static bool EffectiveCenter(this Rotation angle, int proposedCX, int proposedCy, out int Cx, out int Cy)
        {
            Cx = proposedCX;
            Cy = proposedCy;

            if (!angle)
                return false;

            if (angle)
            {
                if (angle.HasCenter)
                {
                    Cx = angle.Cx;
                    Cy = angle.Cy;
                }
            }
            return true;
        }
        public static bool EffectiveCenter(this Rotation angle, VectorF proposedCenter, out float Cx, out float Cy) =>
            EffectiveCenter(angle, proposedCenter.X, proposedCenter.Y, out Cx, out Cy);
        public static bool EffectiveCenter(this Rotation angle, Vector proposedCenter, out int Cx, out int Cy) =>
            EffectiveCenter(angle, proposedCenter.X, proposedCenter.Y, out Cx, out Cy);
        public static bool EffectiveCenter(this Rotation angle, IRectangleF proposedBounds, out float Cx, out float Cy) =>
            EffectiveCenter(angle, proposedBounds.X + proposedBounds.Width / 2, proposedBounds.Y + proposedBounds.Height / 2, out Cx, out Cy);
        public static bool EffectiveCenter(this Rotation angle, IRectangle proposedBounds, out int Cx, out int Cy) =>
            EffectiveCenter(angle, proposedBounds.X + proposedBounds.Width / 2, proposedBounds.Y + proposedBounds.Height / 2, out Cx, out Cy);
        #endregion
    }
#endif
}
