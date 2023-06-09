/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window

namespace MnM.GWS
{
    #region BRUSH TYPE
    public static partial class BrushType
    {
        /// <summary>
        /// For texture brushes only.
        /// </summary>
        public const sbyte Texture = -1;

        /// <summary>
        /// Solid fill with first colour specified.
        /// </summary>
        public const sbyte Solid = 0;

        /// <summary>
        /// Fill changes colour along the horizontal.
        /// </summary>
        public const sbyte Horizontal = 1;

        /// <summary>
        /// Fill changes colour along the vertical.
        /// </summary>
        public const sbyte Vertical = 2;

        /// <summary>
        /// Fill changes colour along the forward diagonal
        /// </summary>
        public const sbyte ForwardDiagonal = 3;

        /// <summary>
        /// Fill changes colour along the backward diagonal.
        /// </summary>
        public const sbyte BackwardDiagonal = 4;

        /// <summary>
        /// Fill changes colour from Left to right for the part of the shape in the top half of the enclosing rectangle.
        /// Fill goes right to left for bottom half of the enclosing rectangle.
        /// </summary>
        public const sbyte HorizontalSwitch = 5;

        /// <summary>
        /// Fill changes colour from left to central vertical and then from right to central vertical.
        /// </summary>
        public const sbyte HorizontalCentral = 6;

        /// <summary>
        /// Fill changes colour from top to central horizontal and then from bottom to central horizontal.
        /// </summary>
        public const sbyte VerticalCentral = 7;

        /// <summary>
        /// Fill changes colour from top left to central Forward diagonal and then from bottom right to central forward diagonal.
        /// </summary>
        public const sbyte DiagonalCentral = 8;

        /// <summary>
        /// Fill changes colour as it rotates around the centre of the rectangle enclosing the shape - starts at 0 degree. 
        /// The colour is symetric along the back diagonal.
        /// </summary>
        public const sbyte Conical = 9;

        /// <summary>
        /// Fill changes colour as it rotates around the centre, however operation starts at 90 degree. 
        /// The colour is symetric along the back diagonal.
        /// </summary>
        public const sbyte Conical2 = 10;

        /// <summary>
        /// Fill changes colour as it goes from outer most perimeter towards center referencing a biggest possible circle within perimeter. 
        /// </summary>
        public const sbyte Circular = 12;

        /// <summary>
        /// Fill changes colour as it goes from outer most perimeter towards center referencing a biggest possible ellipse within perimeter. 
        /// </summary>
        public const sbyte Elliptical = 13;

        /// <summary>
        /// Fill changes colour as it rotates around the centre in rectangular fashion. 
        /// </summary>
        public const sbyte Rectangular = 14;

        /// <summary>
        /// Fill changes colour as it rotates around the centre in rectangular fashion. 
        /// </summary>
        public const sbyte MiddleCircular = 15;
    }
    #endregion

    #region SHAPE TYPES
    public static partial class ShapeTypes
    {
        public const uint
            Square = 0,
            Rectangle = 1,
            Circle = 2,
            Ellipse = 3,
            Arc = 4,
            Pie = 5,
            Curve = 6,
            Line = 7,
            Triangle = 8,
            Polygon = 9,
            RoundedArea = 10,
            Rhombus = 11,
            Trapezium = 12,
            Bezier = 13,
            Text = 14,
            Capsule = 15;
    }
    #endregion
}
#endif

