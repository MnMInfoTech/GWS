/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;

namespace MnM.GWS
{
    /// <summary>
    /// Provides to a void method which is currently assigned to this delegete.
    /// </summary>
    public delegate void VoidMethod();

    /// <summary>
    /// Provides to a method which is currently assigned to this delegete and returns value of type T.
    /// </summary>
    public delegate T ReturnMethod<T>();

    /// <summary>
    /// Delegate CustomizeFormat
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>System.String.</returns>
    public delegate string CustomizeFormat(object element);

    /// <summary>
    /// Copy one memory block to another,
    /// </summary>
    /// <param name="sourceIndex">Pixel location of source where copy starts.</param>
    /// <param name="destinationIndex">Pixel location of destiantion where paste starts.</param>
    /// <param name="copyLength">Length of pixels to be copied.</param>
    /// <param name="xPosition">Position of X axis for the scan line copied.</param>
    /// <param name="yPosition">Position of Y axis for the scan line copied.</param>
    public delegate void BlockCopy(int sourceIndex, int destinationIndex, int copyLength, int xPosition, int yPosition);


#if GWS || Window
    /// <summary>
    /// Provides axial pixel information to process further for variety of purposes.
    /// </summary>
    /// <typeparam name="T">Expected type: float or integer.</typeparam>
    /// <param name="val">Value i.e. X co-ordinate if horizontal is true otherwise Y co-ordinate.</param>
    /// <param name="axis">Axis i.e. Y co-ordinate if horizontal is true otherwise X co-ordinate.</param>
    /// <param name="horizontal">Axial direction i.e. if true horizontal otherwise vertical</param>
    public delegate void PixelAction<T>(T val, int axis, bool horizontal, DrawCommand command);

    /// <summary>
    /// Provides pixel information to process further for variety of purposes.
    /// </summary>
    /// <param name="x">X co-ordinate of pixel.</param>
    /// <param name="y">Y co-ordinate of pixel.</param>
    public delegate void VectorAction<T>(T x, T y);

    /// <summary>
    /// Provides axial line information to process further for variety of purposes.
    /// </summary>
    /// <typeparam name="T">Expected type: float or integer.</typeparam>
    /// <param name="val1">Value1 i.e. X co-ordinate if horizontal is true otherwise Y co-ordinate.</param>
    /// <param name="axis">Axis i.e. Y co-ordinate if horizontal is true otherwise X co-ordinate.</param>
    /// <param name="horizontal">Axial direction i.e. if true horizontal otherwise vertical</param>
    /// <param name="val2">Value2 i.e. X co-ordinate if horizontal is true otherwise Y co-ordinate.</param>
    /// <param name="userData">Any data supplied by user.</param>
    /// <param name="command">Fill command to control aspects of filling line.</param>
    /// <param name="dstOffsetX">X co-ordinate value of any offset to apply while writing.</param>
    /// <param name="dstOffsetY">Y co-ordinate value of any offset to apply while writing.</param>
    public delegate void FillAction<T>(T val1, int axis, bool horizontal, T val2, float? userData, DrawCommand command);
#endif
}
