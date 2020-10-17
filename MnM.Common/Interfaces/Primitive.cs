/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window)
    using System;
    using System.Collections.Generic;

    #region ISize
    /// <summary>
    /// Represents a location.
    /// </summary>
    public interface ISize
    {
        /// <summary>
        /// Gets width of this object.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets height of this object.
        /// </summary>
        int Height { get; }
    }
    #endregion

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

    #region SCALE
    public interface IScale
    {
        /// <summary>
        /// Gets value to scale X co-ordinte.
        /// </summary>
        float X { get; }

        /// <summary>
        /// Gets value to scale Y co-ordinte.
        /// </summary>
        float Y { get; }

        /// <summary>
        /// Indicates if this object has valid scalling factors.
        /// </summary>
        bool HasScale { get; }
    }
    #endregion

    #region ICOLOR
    public interface IColor
    {
        /// <summary>
        /// The color this object represents.
        /// </summary>
        int Color { get; }
    }
    #endregion

    #region ILOCATION
    public interface ILocation
    {
        /// <summary>
        /// Gets X co-ordinate of the location this object.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets Y co-ordinate of the location of this object.
        /// </summary>
        int Y { get; }
    }
    #endregion

    #region IBOUNDS
    /// <summary>
    /// Represents an object which has bounds.
    /// </summary>
    public interface IBounds
    {
        /// <summary>
        /// Gets bounds of this object.
        /// </summary>
        Rectangle Bounds { get; }
    }
    #endregion

    #region IBOUNDS
    /// <summary>
    /// Represents an object which has bounds.
    /// </summary>
    public interface IBoundsF
    {
        /// <summary>
        /// Gets bounds of this object.
        /// </summary>
        RectangleF Bounds { get; }
    }
    #endregion

#if Advanced
    #region ISIMPLE-POPUP-ITEM
    public interface ISimplePopupItem 
    {
        string Text { get; }
        bool Enabled { get; }
        Rectangle Bounds { get; }
    }
    #endregion
#endif

#endif
}
