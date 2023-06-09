// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file  the project root for more information.
// Author: Manan Adhvaryu.

using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IMATRIX3x2
    /// <summary>
    /// A structure encapsulating a 3x2 matrix.
    /// </summary>
    public interface IMatrix3x2
    {
        #region PROPERTIES
        /// <summary>
        /// The first element of the first row
        /// </summary>
        float M00 { get; set; }

        /// <summary>
        /// The second element of the first row
        /// </summary>
        float M01 { get; set; }

        /// <summary>
        /// The first element of the second row
        /// </summary>
        float M10 { get; set; }

        /// <summary>
        /// The second element of the second row
        /// </summary>
        float M11 { get; set; }

        /// <summary>
        /// The first element of the third row
        /// </summary>
        float M20 { get; set; }

        /// <summary>
        /// The second element of the third row
        /// </summary>
        float M21 { get; set; }

        /// <summary>
        /// Returns whether the matrix is the identity matrix.
        /// </summary>
        bool IsIdentity { get; }

        /// <summary>
        /// Gets or sets the translation component of this matrix.
        /// </summary>
        VectorF Translation { get; set; }
        #endregion
    }
    #endregion

    /// <summary>
    /// A structure encapsulating a 3x2 matrix.
    /// </summary>
    public struct Matrix3x2 : IMatrix3x2, IEquatable<Matrix3x2>
    {
        #region VARIABLES
        /// <summary>
        /// The first element of the first row
        /// </summary>
        public float M00;
        /// <summary>
        /// The second element of the first row
        /// </summary>
        public float M01;
        /// <summary>
        /// The first element of the second row
        /// </summary>
        public float M10;
        /// <summary>
        /// The second element of the second row
        /// </summary>
        public float M11;
        /// <summary>
        /// The first element of the third row
        /// </summary>
        public float M20;
        /// <summary>
        /// The second element of the third row
        /// </summary>
        public float M21;

        public static readonly Matrix3x2 Empty = new Matrix3x2();

        /// <summary>
        /// Returns the multiplicative identity matrix.
        /// </summary>
        public static readonly Matrix3x2 Identity = new Matrix3x2(1f, 0f, 0f, 1f, 0f, 0f);
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Constructs a Matrix3x2 from the given components.
        /// </summary>
        public Matrix3x2(float m00, float m01, float m10, float m11, float m20, float m21)
        {
            M00 = m00;
            M01 = m01;
            M10 = m10;
            M11 = m11;
            M20 = m20;
            M21 = m21;
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Returns whether the matrix is the identity matrix.
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                return M00 == 1f && M11 == 1f && // Check diagonal element first for early out.
                                    M01 == 0f &&
                       M10 == 0f &&
                       M20 == 0f && M21 == 0f;
            }
        }

        /// <summary>
        /// Gets or sets the translation component of this matrix.
        /// </summary>
        public VectorF Translation
        {
            get
            {
                return new VectorF(M20, M21);
            }

            set
            {
                M20 = value.X;
                M21 = value.Y;
            }
        }
        #endregion

        #region OPERATORS
        /// <summary>
        /// Calculates the determinant for this matrix. 
        /// The determinant is calculated by expanding the matrix with a third column whose values are (0,0,1).
        /// </summary>
        /// <returns>The determinant.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetDeterminant()
        {
            // There isn't actually any such thing as a determinant for a non-square matrix,
            // but this 3x2 type is really just an optimization of a 3x3 where we happen to
            // know the rightmost column is always (0, 0, 1). So we expand to 3x3 format:
            //
            //  [ M11, M12, 0 ]
            //  [ M21, M22, 0 ]
            //  [ M31, M32, 1 ]
            //
            // Sum the diagonal products:
            //  (M11 * M22 * 1) + (M12 * 0 * M31) + (0 * M21 * M32)
            //
            // Subtract the opposite diagonal products:
            //  (M31 * M22 * 0) + (M32 * 0 * M11) + (1 * M21 * M12)
            //
            // Collapse out the constants and oh look, this is just a 2x2 determinant!

            return (M00 * M11) - (M10 * M01);
        }

        /// <summary>
        /// Negates the given matrix by multiplying all values by -1.
        /// </summary>
        /// <param name="value">The source matrix.</param>
        /// <returns>The negated matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x2 operator -(Matrix3x2 value)
        {
            Matrix3x2 m = new Matrix3x2();

            m.M00 = -value.M00;
            m.M01 = -value.M01;
            m.M10 = -value.M10;
            m.M11 = -value.M11;
            m.M20 = -value.M20;
            m.M21 = -value.M21;

            return m;
        }

        /// <summary>
        /// Adds each matrix element  value1 with its corresponding element  value2.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>The matrix containing the summed values.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x2 operator +(Matrix3x2 value1, Matrix3x2 value2)
        {
            Matrix3x2 m = new Matrix3x2();

            m.M00 = value1.M00 + value2.M00;
            m.M01 = value1.M01 + value2.M01;
            m.M10 = value1.M10 + value2.M10;
            m.M11 = value1.M11 + value2.M11;
            m.M20 = value1.M20 + value2.M20;
            m.M21 = value1.M21 + value2.M21;

            return m;
        }

        /// <summary>
        /// Subtracts each matrix element  value2 from its corresponding element  value1.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>The matrix containing the resulting values.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x2 operator -(Matrix3x2 value1, Matrix3x2 value2)
        {
            Matrix3x2 m = new Matrix3x2();

            m.M00 = value1.M00 - value2.M00;
            m.M01 = value1.M01 - value2.M01;
            m.M10 = value1.M10 - value2.M10;
            m.M11 = value1.M11 - value2.M11;
            m.M20 = value1.M20 - value2.M20;
            m.M21 = value1.M21 - value2.M21;

            return m;
        }

        /// <summary>
        /// Multiplies two matrices together and returns the resulting matrix.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>The product matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x2 operator *(Matrix3x2 value1, Matrix3x2 value2)
        {
            Matrix3x2 m = new Matrix3x2();

            // First row
            m.M00 = value1.M00 * value2.M00 + value1.M01 * value2.M10;
            m.M01 = value1.M00 * value2.M01 + value1.M01 * value2.M11;

            // Second row
            m.M10 = value1.M10 * value2.M00 + value1.M11 * value2.M10;
            m.M11 = value1.M10 * value2.M01 + value1.M11 * value2.M11;

            // Third row
            m.M20 = value1.M20 * value2.M00 + value1.M21 * value2.M10 + value2.M20;
            m.M21 = value1.M20 * value2.M01 + value1.M21 * value2.M11 + value2.M21;

            return m;
        }

        /// <summary>
        /// Scales all elements  a matrix by the given scalar factor.
        /// </summary>
        /// <param name="value1">The source matrix.</param>
        /// <param name="value2">The scaling value to use.</param>
        /// <returns>The resulting matrix.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Matrix3x2 operator *(Matrix3x2 value1, float value2)
        {
            Matrix3x2 m = new Matrix3x2();

            m.M00 = value1.M00 * value2;
            m.M01 = value1.M01 * value2;
            m.M10 = value1.M10 * value2;
            m.M11 = value1.M11 * value2;
            m.M20 = value1.M20 * value2;
            m.M21 = value1.M21 * value2;

            return m;
        }
        #endregion

        #region EQUALITY
        /// <summary>
        /// Returns a boolean indicating whether the given matrices are equal.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>True if the matrices are equal; False otherwise.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Matrix3x2 value1, Matrix3x2 value2)
        {
            return (value1.M00 == value2.M00 && value1.M11 == value2.M11 && // Check diagonal element first for early out.
                                                value1.M01 == value2.M01 &&
                    value1.M10 == value2.M10 &&
                    value1.M20 == value2.M20 && value1.M21 == value2.M21);
        }

        /// <summary>
        /// Returns a boolean indicating whether the given matrices are not equal.
        /// </summary>
        /// <param name="value1">The first source matrix.</param>
        /// <param name="value2">The second source matrix.</param>
        /// <returns>True if the matrices are not equal; False if they are equal.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Matrix3x2 value1, Matrix3x2 value2)
        {
            return (value1.M00 != value2.M00 || value1.M01 != value2.M01 ||
                    value1.M10 != value2.M10 || value1.M11 != value2.M11 ||
                    value1.M20 != value2.M20 || value1.M21 != value2.M21);
        }

        /// <summary>
        /// Returns a boolean indicating whether the matrix is equal to the other given matrix.
        /// </summary>
        /// <param name="other">The other matrix to test equality against.</param>
        /// <returns>True if this matrix is equal to other; False otherwise.</returns>
        public bool Equals(Matrix3x2 other)
        {
            return (M00 == other.M00 && M11 == other.M11 && // Check diagonal element first for early out.
                                        M01 == other.M01 &&
                    M10 == other.M10 &&
                    M20 == other.M20 && M21 == other.M21);
        }

        /// <summary>
        /// Returns a boolean indicating whether the given Object is equal to this matrix instance.
        /// </summary>
        /// <param name="obj">The Object to compare against.</param>
        /// <returns>True if the Object is equal to this matrix; False otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Matrix3x2)
            {
                return Equals((Matrix3x2)obj);
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return unchecked(M00.GetHashCode() + M01.GetHashCode() +
                             M10.GetHashCode() + M11.GetHashCode() +
                             M20.GetHashCode() + M21.GetHashCode());
        }
        #endregion

        /// <summary>
        /// Returns a String representing this matrix instance.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{ {{M11:{0} M12:{1}}} {{M21:{2} M22:{3}}} {{M31:{4} M32:{5}}} }}",
                                 M00, M01,
                                 M10, M11,
                                 M20, M21);
        }

        #region IMATRIX3x2
        float IMatrix3x2.M00
        {
            get => M00;
            set => M00 = value;
        }
        float IMatrix3x2.M01
        {
            get => M01;
            set => M01 = value;
        }
        float IMatrix3x2.M10
        {
            get => M10;
            set => M10 = value;
        }
        float IMatrix3x2.M11
        {
            get => M11;
            set => M11 = value;
        }
        float IMatrix3x2.M20
        {
            get => M20;
            set => M20 = value;
        }
        float IMatrix3x2.M21
        {
            get => M21;
            set => M21 = value;
        }
        bool IMatrix3x2.IsIdentity => IsIdentity;
        VectorF IMatrix3x2.Translation
        {
            get => Translation;
            set => Translation = new VectorF(value);
        }
        #endregion
    }
}
