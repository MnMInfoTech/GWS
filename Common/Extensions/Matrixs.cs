/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)

namespace MnM.GWS
{
    using System;
    using System.Runtime.CompilerServices;
    public static partial class Matrixs
    {
        #region CREATE TRANSLATION
        /// <summary>
        /// Creates a translation matrix from the given vector.
        /// </summary>
        /// <param name="position">The translation position.</param>
        /// <returns>A translation matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateTranslation<T>(this VectorF position) where T : IMatrix3x2, new()
        {
            T result = new T();

            result.M00 = 1.0f;
            result.M01 = 0.0f;
            result.M10 = 0.0f;
            result.M11 = 1.0f;

            result.M20 = position.X;
            result.M21 = position.Y;

            return result;
        }

        /// <summary>
        /// Creates a translation matrix from the given X and Y components.
        /// </summary>
        /// <param name="xPosition">The X position.</param>
        /// <param name="yPosition">The Y position.</param>
        /// <returns>A translation matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateTranslation<T>(float xPosition, float yPosition) where T : IMatrix3x2, new()
        {
            T result = new T();

            result.M00 = 1.0f;
            result.M01 = 0.0f;
            result.M10 = 0.0f;
            result.M11 = 1.0f;

            result.M20 = xPosition;
            result.M21 = yPosition;

            return result;
        }
        #endregion

        #region CREATE SCALE
        /// <summary>
        /// Creates a scale matrix from the given X and Y components.
        /// </summary>
        /// <param name="xScale">Value to scale by on the X-axis.</param>
        /// <param name="yScale">Value to scale by on the Y-axis.</param>
        /// <returns>A scaling matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateScale<T>(float xScale, float yScale) where T : IMatrix3x2, new()
        {
            T result = new T();

            result.M00 = xScale;
            result.M01 = 0.0f;
            result.M10 = 0.0f;
            result.M11 = yScale;
            result.M20 = 0.0f;
            result.M21 = 0.0f;
            return result;
        }

        /// <summary>
        /// Creates a scale matrix that is offset by a given center point.
        /// </summary>
        /// <param name="xScale">Value to scale by on the X-axis.</param>
        /// <param name="yScale">Value to scale by on the Y-axis.</param>
        /// <param name="centerPoint">The center point.</param>
        /// <returns>A scaling matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateScale<T>(float xScale, float yScale, VectorF centerPoint) where T : IMatrix3x2, new()
        {
            T result = new T();

            float tx = centerPoint.X * (1 - xScale);
            float ty = centerPoint.Y * (1 - yScale);

            result.M00 = xScale;
            result.M01 = 0.0f;
            result.M10 = 0.0f;
            result.M11 = yScale;
            result.M20 = tx;
            result.M21 = ty;
            return result;
        }

        /// <summary>
        /// Creates a scale matrix from the given vector scale.
        /// </summary>
        /// <param name="scale">The scale to use.</param>
        /// <returns>A scaling matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateScale<T>(this IScale scale) where T : IMatrix3x2, new()
        {
            T result = new T();

            result.M00 = scale.X;
            result.M01 = 0.0f;
            result.M10 = 0.0f;
            result.M11 = scale.Y;
            result.M20 = 0.0f;
            result.M21 = 0.0f;

            return result;
        }

        /// <summary>
        /// Creates a scale matrix from the given vector scale with an offset from the given center point.
        /// </summary>
        /// <param name="scale">The scale to use.</param>
        /// <param name="centerPoint">The center offset.</param>
        /// <returns>A scaling matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateScale<T>(this IScale scale, VectorF centerPoint) where T : IMatrix3x2, new()
        {
            T result = new T();

            float tx = centerPoint.X * (1 - scale.X);
            float ty = centerPoint.Y * (1 - scale.Y);

            result.M00 = scale.X;
            result.M01 = 0.0f;
            result.M10 = 0.0f;
            result.M11 = scale.Y;
            result.M20 = tx;
            result.M21 = ty;

            return result;
        }

        /// <summary>
        /// Creates a scale matrix that scales uniformly with the given scale.
        /// </summary>
        /// <param name="scale">The uniform scale to use.</param>
        /// <returns>A scaling matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateScale<T>(float scale) where T : IMatrix3x2, new()
        {
            T result = new T();
            result.M00 = scale;
            result.M01 = 0.0f;
            result.M10 = 0.0f;
            result.M11 = scale;
            result.M20 = 0.0f;
            result.M21 = 0.0f;

            return result;
        }

        /// <summary>
        /// Creates a scale matrix that scales uniformly with the given scale with an offset from the given center.
        /// </summary>
        /// <param name="scale">The uniform scale to use.</param>
        /// <param name="centerPoint">The center offset.</param>
        /// <returns>A scaling matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateScale<T>(float scale, Vector centerPoint) where T : IMatrix3x2, new()
        {
            T result = new T();

            float tx = centerPoint.X * (1 - scale);
            float ty = centerPoint.Y * (1 - scale);

            result.M00 = scale;
            result.M01 = 0.0f;
            result.M10 = 0.0f;
            result.M11 = scale;
            result.M20 = tx;
            result.M21 = ty;

            return result;
        }
        #endregion

        #region CREATE SKEW
        /// <summary>
        /// Creates a skew matrix from the given angles  radians.
        /// </summary>
        /// <param name="radiansX">The X angle,  radians.</param>
        /// <param name="radiansY">The Y angle,  radians.</param>
        /// <returns>A skew matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateSkew<T>(float radiansX, float radiansY) where T : IMatrix3x2, new()
        {
            T result = new T();

            float xTan = (float)Math.Tan(radiansX);
            float yTan = (float)Math.Tan(radiansY);

            result.M00 = 1.0f;
            result.M01 = yTan;
            result.M10 = xTan;
            result.M11 = 1.0f;
            result.M20 = 0.0f;
            result.M21 = 0.0f;

            return result;
        }

        /// <summary>
        /// Creates a skew matrix from the given angles  radians and a center point.
        /// </summary>
        /// <param name="radiansX">The X angle,  radians.</param>
        /// <param name="radiansY">The Y angle,  radians.</param>
        /// <param name="centerPoint">The center point.</param>
        /// <returns>A skew matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateSkew<T>(float radiansX, float radiansY, VectorF centerPoint) where T : IMatrix3x2, new()
        {
            T result = new T();

            float xTan = (float)Math.Tan(radiansX);
            float yTan = (float)Math.Tan(radiansY);

            float tx = -centerPoint.Y * xTan;
            float ty = -centerPoint.X * yTan;

            result.M00 = 1.0f;
            result.M01 = yTan;
            result.M10 = xTan;
            result.M11 = 1.0f;
            result.M20 = tx;
            result.M21 = ty;

            return result;
        }
        #endregion

        #region CREATE ROTATION
        /// <summary>
        /// Creates a rotation matrix using the given rotation  radians.
        /// </summary>
        /// <param name="radians">The amount of rotation,  radians.</param>
        /// <returns>A rotation matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateRotation<T>(float radians) where T : IMatrix3x2, new()
        {
            T result = new T();
            Angles.SinCos(radians, out float cos, out float sin);

            // [  cos  sin ]
            // [ -sin  cos ]
            // [  x  y ]

            result.M00 = cos;
            result.M01 = sin;
            result.M10 = -sin;
            result.M11 = cos;
            result.M20 = 0.0f;
            result.M21 = 0.0f;

            return result;
        }

        /// <summary>
        /// Creates a rotation matrix using the given rotation  radians and a center point.
        /// </summary>
        /// <param name="radians">The amount of rotation,  radians.</param>
        /// <param name="centerPoint">The center point.</param>
        /// <returns>A rotation matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateRotation<T>(float radians, VectorF centerPoint) where T : IMatrix3x2, new()
        {
            T result = new T();

            Angles.SinCos(radians, out float cos, out float sin);

            float x = centerPoint.X * (1 - cos) + centerPoint.Y * sin;
            float y = centerPoint.Y * (1 - cos) - centerPoint.X * sin;

            // [  cos  sin ]
            // [ -sin  cos ]
            // [  x  y ]
            result.M00 = cos;
            result.M01 = sin;
            result.M10 = -sin;
            result.M11 = cos;
            result.M20 = x;
            result.M21 = y;

            return result;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateRotation<T>(this IRotation rotation, bool negative = false) where T : IMatrix3x2, new()
        {
            var degree = negative ? -rotation.Angle : rotation.Angle;
            Angles.SinCos(degree, out float sin, out float cos);
            return CreateRotation<T>(sin, cos);
        }

        /// <summary>
        /// Creates a rotation matrix using the given rotation  radians.
        /// </summary>
        /// <param name="radians">The amount of rotation,  radians.</param>
        /// <returns>A rotation matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T CreateRotation<T>(float sin, float cos) where T : IMatrix3x2, new()
        {
            T result = new T();
            // [  cos  sin ]
            // [ -sin  cos ]
            // [  x  y ]

            result.M00 = cos;
            result.M01 = sin;
            result.M10 = -sin;
            result.M11 = cos;
            result.M20 = 0.0f;
            result.M21 = 0.0f;
            return result;
        }
        #endregion

        #region INVERT
        /// <summary>
        /// Attempts to invert the given matrix. If the operation succeeds, the inverted matrix is stored  the result parameter.
        /// </summary>
        /// <param name="matrix">The source matrix.</param>
        /// <param name="result">The output matrix.</param>
        /// <returns>True if the operation succeeded, False otherwise.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Invert<T>(this T matrix, out T result) where T : IMatrix3x2, new()
        {
            float det = (matrix.M00 * matrix.M11) - (matrix.M10 * matrix.M01);
            result = new T();

            if ((float)Math.Abs(det) < float.Epsilon)
            {
                result.M00 = float.NaN;
                result.M01 = float.NaN;
                result.M10 = float.NaN;
                result.M11 = float.NaN;
                result.M20 = float.NaN;
                result.M21 = float.NaN;
                return false;
            }

            float invDet = 1.0f / det;

            result.M00 = matrix.M11 * invDet;
            result.M01 = -matrix.M01 * invDet;
            result.M10 = -matrix.M10 * invDet;
            result.M11 = matrix.M00 * invDet;
            result.M20 = (matrix.M10 * matrix.M21 - matrix.M20 * matrix.M11) * invDet;
            result.M21 = (matrix.M20 * matrix.M01 - matrix.M00 * matrix.M21) * invDet;

            return true;
        }
        #endregion

        #region LERP
        /// <summary>
        /// Linearly interpolates from matrix1 to matrix2, based on the third parameter.
        /// </summary>
        /// <param name="matrix1">The first source matrix.</param>
        /// <param name="matrix2">The second source matrix.</param>
        /// <param name="amount">The relative weighting of matrix2.</param>
        /// <returns>The interpolated matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Lerp<T>(this T matrix1, T matrix2, float amount) where T : IMatrix3x2, new()
        {
            T result = new T();

            // First row
            result.M00 = matrix1.M00 + (matrix2.M00 - matrix1.M00) * amount;
            result.M01 = matrix1.M01 + (matrix2.M01 - matrix1.M01) * amount;

            // Second row
            result.M10 = matrix1.M10 + (matrix2.M10 - matrix1.M10) * amount;
            result.M11 = matrix1.M11 + (matrix2.M11 - matrix1.M11) * amount;

            // Third row
            result.M20 = matrix1.M20 + (matrix2.M20 - matrix1.M20) * amount;
            result.M21 = matrix1.M21 + (matrix2.M21 - matrix1.M21) * amount;

            return result;
        }
        #endregion

        #region NEGATE
        /// <summary>
        /// Negates the given matrix by multiplying all values by -1.
        /// </summary>
        /// <param name="value">The source matrix.</param>
        /// <returns>The negated matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Negate<T>(this T value) where T : IMatrix3x2, new()
        {
            T result = new T();

            result.M00 = -value.M00;
            result.M01 = -value.M01;
            result.M10 = -value.M10;
            result.M11 = -value.M11;
            result.M20 = -value.M20;
            result.M21 = -value.M21;

            return result;
        }
        #endregion

        #region ADD
        /// <summary>
        /// Adds each matrix element  value1 with its corresponding element  value2.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>The matrix containing the summed values.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Add<T>(this T value1, T value2) where T : IMatrix3x2, new()
        {
            T result = new T();

            result.M00 = value1.M00 + value2.M00;
            result.M01 = value1.M01 + value2.M01;
            result.M10 = value1.M10 + value2.M10;
            result.M11 = value1.M11 + value2.M11;
            result.M20 = value1.M20 + value2.M20;
            result.M21 = value1.M21 + value2.M21;

            return result;
        }
        #endregion

        #region SUBTRACT
        /// <summary>
        /// Subtracts each matrix element  value2 from its corresponding element  value1.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>The matrix containing the resulting values.</returns>
        public static T Subtract<T>(this T value1, T value2) where T : IMatrix3x2, new()
        {
            T result = new T();

            result.M00 = value1.M00 - value2.M00;
            result.M01 = value1.M01 - value2.M01;
            result.M10 = value1.M10 - value2.M10;
            result.M11 = value1.M11 - value2.M11;
            result.M20 = value1.M20 - value2.M20;
            result.M21 = value1.M21 - value2.M21;

            return result;
        }
        #endregion

        #region MULTIPLY
        /// <summary>
        /// Multiplies two matrices together and returns the resulting matrix.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>The product matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Multiply<T>(this T value1, T value2) where T : IMatrix3x2, new()
        {
            T result = new T();

            // First row
            result.M00 = value1.M00 * value2.M00 + value1.M01 * value2.M10;
            result.M01 = value1.M00 * value2.M01 + value1.M01 * value2.M11;

            // Second row
            result.M10 = value1.M10 * value2.M00 + value1.M11 * value2.M10;
            result.M11 = value1.M10 * value2.M01 + value1.M11 * value2.M11;

            // Third row
            result.M20 = value1.M20 * value2.M00 + value1.M21 * value2.M10 + value2.M20;
            result.M21 = value1.M20 * value2.M01 + value1.M21 * value2.M11 + value2.M21;

            return result;
        }

        /// <summary>
        /// Scales all elements  a matrix by the given scalar factor.
        /// </summary>
        /// <param name="value1">The source matrix.</param>
        /// <param name="value2">The scaling value to use.</param>
        /// <returns>The resulting matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Multiply<T>(this T value1, float value2) where T : IMatrix3x2, new()
        {
            T result = new T();

            result.M00 = value1.M00 * value2;
            result.M01 = value1.M01 * value2;
            result.M10 = value1.M10 * value2;
            result.M11 = value1.M11 * value2;
            result.M20 = value1.M20 * value2;
            result.M21 = value1.M21 * value2;

            return result;
        }
        #endregion

        #region MATRIX TRANSFORM
        /// <param name="matrix">The transformation matrix.</param>
        /// <returns>The transformed vector.</returns>
        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="matrix"></param>
        /// <param name="newx"></param>
        /// <param name="newy"></param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform<T>(this T matrix, float x, float y,  out float newx, out float newy) where T : IMatrix3x2
        {
            //x*cos+y*-sin+x
            //x*sin+y*cos+y
            newx = x * matrix.M00 + y * matrix.M10 + matrix.M20;
            newy = x * matrix.M01 + y * matrix.M11 + matrix.M21;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform<T>(this T matrix, float x, float y,  out int newx, out int newy) where T : IMatrix3x2
        {
            newx = (x * matrix.M00 + y * matrix.M10 + matrix.M20).Round();
            newy = (x * matrix.M01 + y * matrix.M11 + matrix.M21).Round();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform<T>(this T matrix, int x, int y,out int newx, out int newy) where T : IMatrix3x2
        {
            newx = (x * matrix.M00 + y * matrix.M10 + matrix.M20).Round();
            newy = (x * matrix.M01 + y * matrix.M11 + matrix.M21).Round();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform<T>(this T matrix, int val, int axis, bool horizontal, out float newx, out float newy) where T : IMatrix3x2
        {
            Vectors.ToXY(val, axis, horizontal, out int x, out int y);
            matrix.Transform(x, y,  out newx, out newy);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Transform<T>(this T matrix, int val, int axis, bool horizontal,  out int newx, out int newy) where T : IMatrix3x2
        {
            Vectors.ToXY(val, axis, horizontal, out int x, out int y);
            matrix.Transform(x, y, out newx, out newy);
        }

        /// <summary>
        /// Transforms a vector by the given matrix.
        /// </summary>
        /// <param name="position">The source vector.</param>
        /// <param name="matrix">The transformation matrix.</param>
        /// <returns>The transformed vector.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF Transform<T>(this T matrix, Vector position) where T : IMatrix3x2
        {
            return new VectorF(
                position.X * matrix.M00 + position.Y * matrix.M10 + matrix.M20,
                position.X * matrix.M01 + position.Y * matrix.M11 + matrix.M21);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector TransformNormal<T>(this T matrix, Vector position) where T : IMatrix3x2
        {
            return new VectorF(
                position.X * matrix.M00 + position.Y * matrix.M10,
                position.X * matrix.M01 + position.Y * matrix.M11).Round();
        }
        #endregion
    }
}
#endif
