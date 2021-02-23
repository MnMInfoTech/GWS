/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    #region IPOINT
    public interface IPoint : IDrawParams
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

    #region ISIZE
    /// <summary>
    /// Represents a location.
    /// </summary>
    public interface ISize : IDrawParams
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

    #region ISIZEF
    /// <summary>
    /// Represents a location.
    /// </summary>
    public interface ISizeF
    {
        /// <summary>
        /// Gets width of this object.
        /// </summary>
        float Width { get; }

        /// <summary>
        /// Gets height of this object.
        /// </summary>
        float Height { get; }
    }
    #endregion

    #region IPOINTF
    public interface IPointF
    {
        /// <summary>
        /// Gets X co-ordinate of the location this object.
        /// </summary>
        float X { get; }

        /// <summary>
        /// Gets Y co-ordinate of the location of this object.
        /// </summary>
        float Y { get; }
    }
    #endregion

    #region IRECTANGLEF
    public interface IRectangleF : ISizeF, IPointF
    { }
    #endregion

    #region IRECTANGLE
    public interface IRectangle : IPoint, ISize, IDrawParams, IBoundable
    { }
    #endregion

    #region IPERIMETER
    /// <summary>
    /// Represents an object which has an area with perimeter and information about IDs of process and shape currently being rendered.
    /// </summary>
    public interface IPerimeter : IProcessID, IShapeID, IType, IBoundable
    { }
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

    #region IBOUNDARY
    public interface IBoundary : IPerimeter, INotifiable
    {
        /// <summary>
        /// Gets or sets GWS assigned life priority for the object while rendering.
        /// </summary>
        new byte Type { get; set; }
    }
    #endregion

#if (GWS || Window)

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

    #region IBOUNDS
    public interface IBoundsHolder: IDrawParams
    {
        /// <summary>
        /// Gets bounds of current shape associated with current rendering process.
        /// </summary>
        IRectangle Bounds { get; }
    }
    #endregion 
#endif
}
