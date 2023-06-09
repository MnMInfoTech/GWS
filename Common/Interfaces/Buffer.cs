/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region IIMAGE-CONTEXT
    /// <summary>
    /// This is a marker interface which represents an object which can be converted to a valid image source.
    /// GWS provides in-built functionality to convert IPenContext object to a valid image source.
    /// If you intent to inherit this interface on some custom object then you must override Factory.DealWithUnknownImageSource method for
    /// successful conversion.
    /// </summary>
    public interface IImageContext 
    { }
    #endregion

    #region IPENCONTEXT
    /// <summary>
    /// This is a marker interface which represents an object which can be converted to a buffer pen.
    /// </summary>
    public interface IPenContext : IImageContext, IProperty, IInLineParameter
    { }
    #endregion

    #region IPEN
    /// <summary>
    /// Represents an object which can be used to draw a shape on IGraphics surface.
    /// </summary>
    public interface IPen : IPenContext
    {
        /// <summary>
        /// Reads a pixel after applying applying offset and rotation transformation (if exists) to get the correct co-ordinate.
        /// </summary>
        /// <param name="x">X co-ordinate of the location to read pixel from.</param>
        /// <param name="y">Y co-ordinate of the location to read pixel from.</param>
        /// <returns>Pixel value.</returns>
        int ReadPixel(int x, int y);

        /// <summary>
        /// Reads an axial line after applying applying offset and rotation transformation (if exists).
        /// </summary>
        /// <param name="start">Start of an axial line to read from this object - X co-ordinate if horizontal otherwise Y co-ordinate.</param>
        /// <param name="end">End of an axial line to read from this object - Y co-ordinate if not horizontal otherwise X co-ordinate.</param>
        /// <param name="axis">Axis value of line to read from this object -  Y co-ordinate if horizontal otherwise X co-ordinate.</param>
        /// <param name="horizontal">Direction of axial line if true then horizontal otherwise vertiacal.</param>
        /// <param name="pixels">Resultant memory block.</param>
        /// <param name="srcIndex">Location in the resultant memory block from where reading shoud start.</param>
        /// <param name="length">Length up to which the block should be read.</param>
        void ReadLine(int start, int end, int axis, bool horizontal,
            out int[] pixels, out int srcIndex, out int length);
    }
    #endregion

    #region IIMAGE DATA
    /// <summary>
    /// Represents an object which offers data source which can be rendered on target
    /// of type IRenderer. Any object which implements IBlock or ICopyable which both
    /// implements this interface will be dealt automatically by GWS to perform render operation.
    /// </summary>
    public interface IImageData : IImageContext, ISource<IntPtr>
    { }
    #endregion

    #region IRENDERER
    /// <summary>
    /// Represents an object which provides a render action for executing render operation.
    /// </summary>
    public interface IRenderer : ISize, IValid, INotToBeImplementedOutsideGWS
    {
        /// <summary>
        /// Returns an action delegate for rendering a collection of lines one after another 
        /// on specified this object using writable pen provided by session object.
        /// </summary>
        /// <param name="Parameters">Collection of parameters to control and assist rendering operation.</param>
        /// <returns>DrawAction created by this object.</returns>
        RenderAction CreateRenderAction(IEnumerable<IParameter> Parameters = null);
    }
    #endregion

    #region IEx RENDER
    internal partial interface IExRenderer : IRenderer, IExUpdatable<ViewState, ModifyCommand>, IExViewState
    {
        IPoint UniversalDrawOffset { get; set; }
    }
    #endregion

    #region IMARKER<T>
    /// <summary>
    /// Represents an object which allows conditional marking of some data it holds.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IMarker<T>
    {
        /// <summary>
        /// Marks the data of type T satisfying the condition if specified with given value.
        /// </summary>
        /// <param name="lines">Scan lines to provide area for marking.</param>
        /// <param name="value">Value which data to be marked with.</param>
        /// <param name="XOR"></param>
        /// <param name="modifyCommand"></param>
        /// <param name="offX"></param>
        /// <param name="offY"></param>
        /// <param name="userX"></param>
        /// <param name="userY"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        int[] Mark(IEnumerable<IScanLine> lines, T value, bool XOR, ModifyCommand modifyCommand = ModifyCommand.Replace, 
            IOffset offset = null, IPoint userPoint = null, Func<int, bool> condition = null);
    }
    #endregion

    #region IIMAGE
    /// <summary>
    /// Represents an object which offers data source which can be rendered on target
    /// of type IRenderer. Any object which implements IBlock or ICopyable which both
    /// implements this interface will be dealt automatically by GWS to perform render operation.
    /// </summary>
    public interface IImage : IRenderer, IImageSource
    { }
    #endregion

    #region IBACKGROUND
    public interface IBackgroundPenHolder
    {
        /// <summary>
        /// Gets background pen constructed from background context supplied to this window.
        /// </summary>
        IPen BackgroundPen { get; }
    }
    #endregion

    #region IBACKGROUNDPEN
    public interface IBackgroundContextSetter: IBackgroundPenHolder
    {
        /// <summary>
        /// sets background context to construct background of this window.
        /// </summary>
        IPenContext BackgroundContext { set; }
    }
    #endregion
}
