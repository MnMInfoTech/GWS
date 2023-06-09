/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

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
        const int BigExp = Vectors.BigExp;

        /// <summary>
        /// Exponent of BigExp.
        /// </summary>
        const int Big = Vectors.Big;

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
        /// <param name="antiClock">True for clockwise rotation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(float x, float y, float angle, float cx, float cy, 
            out float x1, out float y1, bool antiClock = false, SkewType skewType = 0, float? skewDegree = null)
        {
            float ScaleX = 1, ScaleY = 1;
            if (skewDegree != null && skewDegree != 0 && skewDegree != 360 && skewDegree != -360)
                GetScale(skewDegree.Value, skewType, out ScaleX, out ScaleY);

            if (ScaleX != 1)
                x = (x - cx) * ScaleX + cx;
            if (ScaleY != 1)
                y = (y - cy) * ScaleY + cy;

            if (angle == 0 || angle == 360 || angle == -360)
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
        /// <param name="degree">Angle to rotate point.</param>
        /// <param name="cx">x position of the centre which to rotate the point by.</param>
        /// <param name="cy">y position of the centre which to rotate the point by.</param>
        /// <param name="x1">The x position of the rotated point.</param>
        /// <param name="y1">The y position of the rotated point.</param>
        /// <param name="antiClock">True for clockwise rotation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(int x, int y, float degree, int cx, int cy, out int x1, out int y1,
            bool antiClock = false, SkewType skewType = 0, float? skewDegree = null)
        {
            float sx = 1, sy = 1;

            if (skewDegree != null && skewDegree != 0 && skewDegree != 360 && skewDegree != -360)
                GetScale(skewDegree.Value, skewType, out sx, out sy);

            if (sx != 1)
                x = ((x - cx) * sx + cx).Round();
            if (sy != 1)
                y = ((y - cy) * sy + cy).Round();

            if (degree == 0 || degree == 360 || degree == -360)
            {
                x1 = x;
                y1 = y;
                return;
            }

            SinCos(degree, out int Sini, out int Cosi);
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

        #region DEGREE ROTATE
        /// <summary>
        /// Returns new co-ordinates after rotating and skewing given point specified by x and y.
        /// </summary>
        /// <param name="rotation">Rotation which to rotate the point by.</param>
        /// <param name="x">X co-ordinate of the point to rotate.</param>
        /// <param name="y">Y co-ordinate of the point to rotate.</param>
        /// <param name="x1">X co-ordinate of resultant point.</param>
        /// <param name="y1">Y co-ordinate of resultant point.</param>
        /// <param name="cx">Optional X co-ordinate of center point to override center of rotation.</param>
        /// <param name="cy">Optional Y co-ordinate of center point to override center of rotation.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(this IDegree rotation, float x, float y, out float x1, out float y1,
            float? cx = null, float? cy = null, bool antiClock = false, bool noSkew = false)
        {
            x1 = x;
            y1 = y;
            if (rotation == null || !rotation.Valid)
                return;

            float degree = rotation.Angle;
            float Sin = 0, Cos = 1;
            float CX = cx ?? 0, CY = cy ?? 0;
            float SkewX = 1, SkewY = 1;
            bool HasSkewScale = false;
            bool diagonal = false;
            ISkew skew = null;
            ICentre center = null;

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

            if (rotation is ICentreHolder)
            {
                center = ((ICentreHolder)rotation).Centre;
            }
            else if (rotation is ICentre)
            {
                center = ((ICentre)rotation);
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
            if (HasAngle)
            {
                x -= CX;
                y -= CY;
                x1 = (x * Cos - y * Sin);
                y1 = (x * Sin + y * Cos);
                x1 += CX;
                y1 += CY;
                x = x1;
                y = y1;
            }
            if (HasSkewScale)
            {
                x = (x - CX) * SkewX + CX;
                y = (y - CY) * SkewY + CY;
            }
            x1 = x;
            y1 = y;
        }

        /// <summary>
        /// Returns new co-ordinates after rotating and skewing given point specified by x and y.
        /// </summary>
        /// <param name="rotation">Rotation which to rotate the point by.</param>
        /// <param name="x">X co-ordinate of the point to rotate.</param>
        /// <param name="y">Y co-ordinate of the point to rotate.</param>
        /// <param name="x1">X co-ordinate of resultant point.</param>
        /// <param name="y1">Y co-ordinate of resultant point.</param>
        /// <param name="cx">Optional X co-ordinate of center point to override center of rotation.</param>
        /// <param name="cy">Optional Y co-ordinate of center point to override center of rotation.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(this IDegree rotation, int x, int y, out int x1, out int y1,
            float? cx = null, float? cy = null, bool antiClock = false, bool noSkew = false)
        {
            x1 = x;
            y1 = y;
            if (rotation == null || !rotation.Valid)
                return;
            int Sin = 0, Cos = 1;
            float degree = rotation.Angle;
            float Cx = cx ?? 0, Cy = cy ?? 0;
            float SkewX = 1, SkewY = 1;
            bool HasSkewScale = false;
            bool diagonal = false;
            ISkew skew = null;
            ICentre center = null;

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

            if (rotation is ICentreHolder)
            {
                center = ((ICentreHolder)rotation).Centre;
            }
            else if (rotation is ICentre)
            {
                center = ((ICentre)rotation);
            }
            if (center != null)
            {
                Cx = center.X;
                Cy = center.Y;
            }
            if (HasAngle)
            {
                SinCos(degree, out Sin, out Cos);
                if (antiClock)
                    Sin = -Sin;
            }

            int CX = Cx.Round();
            int CY = Cy.Round();

            if (HasAngle)
            {
                x -= CX;
                y -= CY;
                x1 = (x * Cos - y * Sin) >> BigExp;
                y1 = (x * Sin + y * Cos) >> BigExp;
                x1 += CX;
                y1 += CY;
                x = x1;
                y = y1;
            }
            if (HasSkewScale)
            {
                x = (int)((x - CX) * SkewX + CX);
                y = (int)((y - CY) * SkewY + CY);
            }
            x1 = x;
            y1 = y;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Rotates a line by given rotation.
        /// If rotating and angle does not have any center, center of a line will be used to rotate otherwise center of angle will be used.
        /// </summary>
        /// <param name="rotation">Rotation which to rotate the point by.</param>
        /// <param name="x1">X co-ordinate of start point of line.</param>
        /// <param name="y1">Y co-ordinate of start point of line.</param>
        /// <param name="x2">X co-ordinate of end point of line.</param>
        /// <param name="y2">Y co-ordinate of end point of line.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        public static void Rotate(this IDegree rotation, ref float x1, ref float y1,
            ref float x2, ref float y2, bool antiClock = false, bool noSkew = false)
        {
            if (rotation == null || !rotation.Valid)
                return;
            bool isRotated = rotation.EffectiveCenter(Numbers.Middle(x1, x2),
                Numbers.Middle(y1, y2), out float cx, out float cy);
            if (!isRotated)
                return;
            rotation.Rotate(x1, y1, out x1, out y1, cx, cy, antiClock, noSkew);
            rotation.Rotate(x2, y2, out x2, out y2, cx, cy, antiClock, noSkew);
        }

        /// <summary>
        /// Returns new co-ordinates after rotating and skewing given point specified by x and y.
        /// </summary>
        /// <param name="rotation">Rotation which to rotate the point by.</param>
        /// <param name="x">X co-ordinate of the point to rotate.</param>
        /// <param name="y">Y co-ordinate of the point to rotate.</param>
        /// <param name="x1">X co-ordinate of resultant point.</param>
        /// <param name="y1">Y co-ordinate of resultant point.</param>
        /// <param name="center">Optional center point to override center of rotation.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(this IDegree rotation, float x, float y, IPointF center,
            out float x1, out float y1, bool antiClock = false, bool noSkew = false)
        {
            Rotate(rotation, x, y, out x1, out y1, center?.X, center?.Y, antiClock, noSkew);
        }

        /// <summary>
        /// Returns new co-ordinates after rotating and skewing given point specified by x and y.
        /// </summary>
        /// <param name="rotation">Rotation which to rotate the point by.</param>
        /// <param name="x">X co-ordinate of the point to rotate.</param>
        /// <param name="y">Y co-ordinate of the point to rotate.</param>
        /// <param name="x1">X co-ordinate of resultant point.</param>
        /// <param name="y1">Y co-ordinate of resultant point.</param>
        /// <param name="center">Optional center point to override center of rotation.</param>
        /// <param name="antiClock">If true rotation will be performed in anti-clock otherwise clockwise fashion.</param>
        /// <param name="noSkew">If true skew operation even if rotation has skew will not be carried out.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate(this IDegree rotation, int x, int y, IPointF center, out int x1, out int y1,
        bool antiClock = false, bool noSkew = false)
        {
            Rotate(rotation, x, y, out x1, out y1, center?.X, center?.Y, antiClock, noSkew);
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

            if ((Skew & SkewType.Horizontal) == SkewType.Horizontal ||
                (Skew & SkewType.Diagonal) == SkewType.Diagonal)
                ScaleX = scale;

            if ((Skew & SkewType.Vertical) == SkewType.Vertical)
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <param name="sin">Returns integer Sin of angle i.e (Math.Sin * (1 << 20)).Round().</param>
        /// <param name="cos">Returns integer Cosine of angle i.e (Math.Cos * (1 << 20)).Round().</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(float angle, out int sin, out int cos, float sinOffset, float cosOffset)
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
            s += sinOffset;
            c += cosOffset;

            sin = (s * Big).Round();
            cos = (c * Big).Round();
        }

        /// <summary>
        /// Returns the Sin and Cosine values for the angle.
        /// </summary>
        /// <param name="angle">Angle in degrees for lookup/calculation.</param>
        /// <param name="sin">Returns Sin of angle.</param>
        /// <param name="cos">Returns Cosine of angle,</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Rotate180(float x, float y, float height, out float newX, out float newY)
        {
            y -= height;
            if (y < 0)
                y = Math.Abs(y);//!!!!If the Rectangle centre is 0 then You get strange results. The image is folded on to itself about y = 0!!!!
            newX = x;
            newY = y;
        }
        #endregion

        #region GET SINCOS VALUES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int StoreAngles(float step, out float[] sinValues,
            out float[] cosValues, float sinOffset = 0, float cosOffset = 0)
        {
            if (step == 0)
                step = 1;
            float start = 0;
            float len = (360f / step);

            var length = len.Round();

            sinValues = new float[length+1];
            cosValues = new float[length+1];
            float sin, cos;
            int i = 0;
            fixed (float* sins = sinValues)
            {
                fixed (float* coss = cosValues)
                {
                    if (step > 0)
                    {
                        while (start < 360)
                        {
                            SinCos(start, out sin, out cos);
                            sins[i] = sin + sinOffset;
                            coss[i] = cos + cosOffset;
                            start += step;
                            ++i;
                        }
                    }
                    else
                    {
                        start = 359;
                        while (start >= 0)
                        {
                            SinCos(start, out sin, out cos);
                            sins[i] = sin + sinOffset;
                            coss[i] = cos + cosOffset;
                            start += step;
                            ++i;
                        }
                    }
                }
            }
            return length;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int StoreAngles(float step, out SinCos[] SinCosValues, float sinOffset = 0, float cosOffset = 0)
        {
            if (step == 0)
                step = 1;
            float start = 0;
            float len = (360f / step);

            int length = (int)len;
            if (len - length >= 05f)
                ++length;
            SinCosValues = new GWS.SinCos[length];
            int i = 0;
            fixed (SinCos* sincos = SinCosValues)
            {
                if (step > 0)
                {
                    while (start < 360)
                    {
                        sincos[i] = new SinCos(i, sinOffset, cosOffset);
                        start += step;
                        ++i;
                    }
                }
                else
                {
                    start = 359;
                    while (start >= 0)
                    {
                        sincos[i] = new SinCos(i, sinOffset, cosOffset);
                        start += step;
                        ++i;
                    }
                }
            }
            return length;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int StoreAngles(float step, out int[] sinValues,
            out int[] cosValues, float sinOffset = 0, float cosOffset = 0)
        {
            if (step == 0)
                step = 1;
            float start = 0;
            float len = (360f / step);

            var length = len.Round();

            sinValues = new int[length + 1];
            cosValues = new int[length + 1];
            int sin, cos;
            int i = 0;
            fixed (int* sins = sinValues)
            {
                fixed (int* coss = cosValues)
                {
                    if (step > 0)
                    {
                        while (start < 360)
                        {
                            SinCos(start, out sin, out cos, sinOffset, cosOffset);
                            sins[i] = sin;
                            coss[i] = cos;
                            start += step;
                            ++i;
                        }
                    }
                    else
                    {
                        start = 359;
                        while (start >= 0)
                        {
                            SinCos(start, out sin, out cos, sinOffset, cosOffset);
                            sins[i] = sin;
                            coss[i] = cos;
                            start += step;
                            ++i;
                        }
                    }
                }
            }
            return length;
        }

        #endregion

        #region ROTATION DIFFERENCE BY 2 DIFFERENT centres
        /// <param name="x">X co-ordinate of point.</param>
        /// <param name="y">Y co-ordinate of point.</param>
        /// <param name="degree">Angle to rotate point.</param>
        /// <param name="cx1">X co-ordinate of the first centre of rotation.</param>
        /// <param name="cy1">Y co-ordinate of the first centre of rotation.</param>
        /// <param name="cx2">X co-ordinate of the second centre of rotation.</param>
        /// <param name="cy2">Y co-ordinate of the second centre of rotation.</param>
        /// <param name="resultX">X co-ordinate of the resultant point.</param>
        /// <param name="resultY">Y co-ordinate of the resultant point.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Difference(int x, int y, float degree, int cx1, int cy1, int cx2, int cy2, 
            out int resultX, out int resultY, bool antiClock = false)
        {
            Rotate(x, y, degree, cx1, cy1, out int x1, out int y1, antiClock: antiClock);
            Rotate(x, y, degree, cx2 , cy2, out int x2, out int y2, antiClock: antiClock);
            resultX = x - (x2 - x1);
            resultY = y - (y2 - y1);
        }

        /// <param name="x">X co-ordinate of point.</param>
        /// <param name="y">Y co-ordinate of point.</param>
        /// <param name="degree">Angle to rotate point.</param>
        /// <param name="cx1">X co-ordinate of the first centre of rotation.</param>
        /// <param name="cy1">Y co-ordinate of the first centre of rotation.</param>
        /// <param name="cx2">X co-ordinate of the second centre of rotation.</param>
        /// <param name="cy2">Y co-ordinate of the second centre of rotation.</param>
        /// <param name="resultX">X co-ordinate of the resultant point.</param>
        /// <param name="resultY">Y co-ordinate of the resultant point.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Difference(float x, float y, float degree, float cx1, float cy1, float cx2, float cy2,
            out float resultX, out float resultY, bool antiClock = false)
        {
            Rotate(x, y, degree, cx1, cy1, out float x1, out float y1, antiClock: antiClock);
            Rotate(x, y, degree, cx2, cy2, out float x2, out float y2, antiClock: antiClock);
            resultX = x - (x2 - x1);
            resultY = y - (y2 - y1);
        }

        /// <param name="x">X co-ordinate of point.</param>
        /// <param name="y">Y co-ordinate of point.</param>
        /// <param name="degree">Angle to rotate point.</param>
        /// <param name="cx1">X co-ordinate of the first centre of rotation.</param>
        /// <param name="cy1">Y co-ordinate of the first centre of rotation.</param>
        /// <param name="cx2">X co-ordinate of the second centre of rotation.</param>
        /// <param name="cy2">Y co-ordinate of the second centre of rotation.</param>
        /// <param name="resultX">X co-ordinate of the resultant point.</param>
        /// <param name="resultY">Y co-ordinate of the resultant point.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Difference(int x, int y, float degree, float cx1, float cy1, float cx2, float cy2,
            out int resultX, out int resultY, bool antiClock = false)
        {
            Rotate(x, y, degree, cx1, cy1, out float x1, out float y1, antiClock: antiClock);
            Rotate(x, y, degree, cx2, cy2, out float x2, out float y2, antiClock: antiClock);
            var resX = x - (x2 - x1);
            var resY = y - (y2 - y1);

            resultX = resX.Round();
            resultY = resY.Round();
        }
        #endregion

        #region SIN COS
        /// <summary>
        /// Returns the Sin and Cosine values for the angle.
        /// Reverse Conversion => (co-ordinate * Cosi) << 20 && (co-ordinate * Sini) << 20;
        /// </summary>
        /// <param name="rotation">Angle object for lookup/calculation.</param>
        /// <param name="sin">Returns Sin of angle.</param>
        /// <param name="cos">Returns Cosine of angle.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(this IDegree rotation, out float sin, out float cos)
        {
            if (rotation == null || !rotation.HasAngle)
            {
                sin = 0;
                cos = 1;
                return;
            }
            SinCos(rotation.Angle, out sin, out cos);
        }

        /// <summary>
        /// Returns integer Sin and Cosine values for the angle.
        /// Reverse Conversion => (co-ordinate * Cosi) << 20 && (co-ordinate * Sini) << 20;
        /// </summary>
        /// <param name="angle">Angle object for lookup/calculation.</param>
        /// <param name="sin">Returns Sin of angle.</param>
        /// <param name="cos">Returns Cosine of angle.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(this IDegree rotation, out int sin, out int cos)
        {
            if (rotation == null || !rotation.HasAngle)
            {
                sin = 0;
                cos = 1048576;
                return;
            }
            SinCos(rotation.Angle, out sin, out cos);
        }
        #endregion

        #region GET EFFECTIVE CENTER
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EffectiveCenter(this IDegree center, float proposedCX, float proposedCy,
            out float Cx, out float Cy)
        {
            Cx = proposedCX;
            Cy = proposedCy;
            if (center == null)
                return true;
            ICentre c = null;

            if (center is ICentre)
                c = (ICentre)center;
            else if (center is ICentreHolder)
                c = ((ICentreHolder)center).Centre;

            if(c != null)
            {
                Cx = c.Cx;
                Cy = c.Cy;
            }
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EffectiveCenter(this IDegree center,
            int proposedCX, int proposedCy, out int Cx, out int Cy)
        {
            Cx = proposedCX;
            Cy = proposedCy;
            if (center == null)
                return true;
            ICentre c = null;

            if (center is ICentre)
                c = (ICentre)center;
            else if (center is ICentreHolder)
                c = ((ICentreHolder)center).Centre;

            if (c != null)
            {
                Cx = c.Cx.Round();
                Cy = c.Cy.Round();
            }
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EffectiveCenter(this IDegree angle, IPoint proposedCenter, out int Cx, out int Cy) =>
            EffectiveCenter(angle, proposedCenter.X, proposedCenter.Y, out Cx, out Cy);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EffectiveCenter(this IDegree angle, IBoundsF proposedBounds,
            out float Cx, out float Cy)
        {
            float x, y, w, h;
            proposedBounds.GetBounds(out x, out y, out w, out h);

            return EffectiveCenter(angle, x + w / 2f, y + h / 2f, out Cx, out Cy);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EffectiveCenter(this IDegree angle, IBounds proposedBounds, out int Cx, out int Cy)
        {
            int x, y, w, h;
            proposedBounds.GetBounds(out x, out y, out w, out h);
            return EffectiveCenter(angle, x + w / 2, y + h / 2, out Cx, out Cy);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EffectiveCenter(this IDegree angle, IBounds proposedBounds, out float Cx, out float Cy)
        {
            int x, y, w, h;
            proposedBounds.GetBounds(out x, out y, out w, out h);
            return EffectiveCenter(angle, x + w / 2f, y + h / 2f, out Cx, out Cy);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EffectiveCenter(this IDegree center, float proposedX, float proposedY,
            float proposedWidth, float proposedHeight, out float Cx, out float Cy)
        {
            return EffectiveCenter(center, proposedX + proposedWidth / 2f,
                proposedY + proposedHeight / 2f, out Cx, out Cy);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EffectiveCenter(this IDegree center, int proposedX, int proposedY,
            int proposedWidth, int proposedHeight, out float Cx, out float Cy)
        {
            return EffectiveCenter(center, proposedX + proposedWidth / 2f,
                proposedY + proposedHeight / 2f, out Cx, out Cy);
        }
        #endregion

    }
}
