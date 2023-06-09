/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MnM.GWS
{
    /// <summary>
    /// Provides action delegate for processing collection of axial lines and collection 
    /// of axial-points/ vectors/ lines for variety of purposes.
    /// </summary>
    /// <param name="lines">Axial lines to process. Normally for filing purposes.
    /// Can be null in order to skip the filling operation</param>
    /// <param name="items">Collection of lines/ Vectors/ Axis points to process.
    /// Can be null in order to skip the drawing operation.</param>
    /// <param name="parameters">Collection of inline action parameters to influence filling and drawing process.</param>
    public delegate bool RenderAction
    (
        IEnumerable<IScanLine> lines,
        IEnumerable<IScanPoint> items,
        IImageData imageSource,
        params IInLineParameter[] parameters
    );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="processArgs"></param>
    /// <returns></returns>
    public delegate U Process<T, U>(T processArgs) where T : IProcessArgs;


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="modifyCommand"></param>
    public delegate void ActionAt<T>(int index, T value, ModifyCommand modifyCommand);

    /// <summary>
    /// Provides to a void method which is currently assigned to this delegate.
    /// </summary>
    public delegate void VoidMethod();

    /// <summary>
    /// Provides to a method which is currently assigned to this delegate and returns value of type T.
    /// </summary>
    public delegate T ReturnMethod<T>();

    /// <summary>
    /// Delegate CustomizeFormat
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>System.String.</returns>
    public delegate string CustomizeFormat(object element);

    /// <summary>
    /// Delegate ObjectInputHandling
    /// </summary>
    /// <typeparam name="TSender">The type of the t sender.</typeparam>
    /// <param name="sender">The sender.</param>
    /// <param name="item">The item.</param>
    /// <param name="position">The position.</param>
    /// <param name="isHandled">if set to <c>true</c> [is handled].</param>
    public delegate void ObjectInputHandling(object sender,
        object item, int position, ref bool isHandled);

    /// <summary>
    /// Copy one memory block to another,
    /// </summary>
    /// <param name="sourceIndex">Pixel location of source where copy starts.</param>
    /// <param name="destinationIndex">Pixel location of destination where paste starts.</param>
    /// <param name="copyLength">Length of pixels to be copied.</param>
    /// <param name="xPosition">Position of X axis for the scan line copied.</param>
    /// <param name="yPosition">Position of Y axis for the scan line copied.</param>
    public delegate void BlockCopy(int sourceIndex, int destinationIndex,
        int copyLength, int xPosition, int yPosition, CopyCommand command);

    /// <summary>
    /// Copy one memory block to another,
    /// </summary>
    /// <param name="sourceIndex">Pixel location of source where copy starts.</param>
    /// <param name="destinationIndex">Pixel location of destination where paste starts.</param>
    /// <param name="copyLength">Length of pixels to be copied.</param>
    /// <param name="xPosition">Position of X axis for the scan line copied.</param>
    /// <param name="yPosition">Position of Y axis for the scan line copied.</param>
    public delegate void ConditionalCopy(int sourceIndex, int destinationIndex,
        int copyLength, int xPosition, int yPosition, int condition, NumCriteria criteria);

    /// <summary>
    /// Provides pixel information to process further for variety of purposes.
    /// </summary>
    /// <param name="x">X co-ordinate of pixel.</param>
    /// <param name="y">Y co-ordinate of pixel.</param>
    public delegate void VectorAction<T>(T x, T y);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void EventHandler<T>(object sender, T args) where T: IEventArgs;
}
