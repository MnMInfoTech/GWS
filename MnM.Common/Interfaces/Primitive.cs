/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System.Collections.Generic;

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
    public interface IRectangle : ISize, IPoint, IDrawParams
    {
        /// <summary>
        /// Indicates if this object has valid perimiter or not.
        /// </summary>
        bool Valid { get; }
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

#if (GWS || Window)

    #region IBOUNDARY
    public interface IBoundary : IRectangle, INotifier, IEnumerable<Vector>, ICloneable<IBoundary>
    {
        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        bool Contains(int x, int y);

        /// <summary>
        /// Tests if given boundary intersects with this object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool Intersects<T>(T other) where T : IPoint, ISize;

        /// <summary>
        /// Retruns result of intersection of other boundary with this object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IRectangle Intersect<T>(T other) where T : IPoint, ISize;

        /// <summary>
        /// Merges give boundary perimeter with this object.
        /// </summary>
        /// <param name="boundary"></param>
        void Merge(IRectangle boundary);

        /// <summary>
        /// Replaces perimeter of this object by copying another boundary perimeter.
        /// </summary>
        /// <param name="boundary"></param>
        void Copy(IRectangle boundary);

        /// <summary>
        /// Gets current bounds of the perimeter.
        /// </summary>
        /// <param name="xExpand">Inflation unit by which horizontal expansion should occur.</param>
        /// <param name="yExpand">Inflation unit by which vertical expansion should occur.</param>
        /// <returns></returns>
        IRectangle GetBounds(int xExpand = 1, int yExpand = 1);

        /// <summary>
        /// Clears perimeters and resets it to default.
        /// </summary>
        void Clear();

        /// <summary>
        /// Draws border around this boundary using given command on specified buffer.
        /// </summary>
        /// <param name="buffer">Buffer which to draw boundary on.</param>
        /// <param name="command">Command to control boundary drawing.</param>
        void Draw(IWritable buffer, Command command = 0);
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

    #region IBOUNDS
    public interface IBounds: IDrawParams
    {
        /// <summary>
        /// Gets bounds of current shape associated with current rendering process.
        /// </summary>
        IRectangle Bounds { get; }
    }
    #endregion
#endif
}
