/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif

namespace MnM.GWS
{
    #region IACCUMULATIVE
    public interface IAccumulative
    {
        /// <summary>
        /// Gets or sets a flag to determine this object should be cleared or not before beginning of a new rendering operation.
        /// </summary>
        bool Accumulative { get; set; }
    }
    #endregion

    #region IPOLYGONAL<T>
    /// <summary>
    /// Represents an object which can be described in perimeter i.e. a hollow structure defined by sequential formation of points.
    /// </summary>
    public interface IPolygonal<T> : IObject where T : IScanPoint
    {
        /// <summary>
        /// Gets array of points of type specified as of T to represent fence of this perimeter.
        /// </summary>
        T[] GetPoints();
    }
    #endregion

    #region POLYGONALF
    public interface IPolygonalF : IPolygonal<VectorF>
    { }
    #endregion

    #region IPOLYGONAL
    public interface IPolygonal : IPolygonal<Point>
    { }
    #endregion

    #region ICOPYABLE
    /// <summary>
    /// Specifies copyable memory block with certain size and length.
    /// </summary>
    public interface ICopyable : ISize, IValid
    {
        /// <summary>
        /// Provides a paste routine to paste the specified chunk of data to a given destination pointer on a given location.
        /// </summary>
        /// <param name="destination">Specifies a pointer where the block should get copied</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line</param>
        /// <param name="parameters">Collection of parameters to control image drawing operation (Can be null).
        /// Expected parameters :
        /// Parameter of CopyCommand for example CopyCommand.Backdrop.Add()
        /// Instance of CopyArea structure as area to copy
        /// Instance of Point structure as destination.
        /// Instance of Offset stuct as offset to destination point
        /// Instance of Clip class as clipped area to limit paste operation area
        /// Instance of Rotation class to rotate copied data while pasting it to the destination.
        /// Instance of Scale class to scale copied data while pasting it to the destination.
        /// Instance of IPen interface to serve as background while pasting it to the destination.
        /// </param>
        ISize CopyTo(IntPtr destination, int dstLen, int dstW, IEnumerable<IParameter> parameters = null);
    }
    #endregion

    #region IEDGE-EXTRACTOR
    public interface IEdgeExtractable: ISize
    {
        VectorF[] GatherEdges(int X, int Y, int W, int H, params IParameter[] parameters);
    }
    #endregion

    #region IRESIZEABLE<T>
    /// <summary>
    /// Represents an object that can be resized.
    /// </summary>
    internal interface IExResizable: ISize
    {
        /// <summary>
        /// Resizes this object.
        /// </summary>
        /// <param name="width">Intended new width of the object.</param>
        /// <param name="height">Intended new height of object.</param>
        /// <param name="resizeCommand">Resize option to control this resize operation.</param>
        object Resize(int width, int height, out bool success , ResizeCommand resizeCommand = ResizeCommand.Free);
    }
    #endregion

    #region IOBJECT CHECKER
    public partial interface IObjectChecker
    {
        /// <summary>
        /// Moves cursor location to a given x and y coordinates.
        /// the test is applied on a last drawn area rather than an actual area of each element so 
        /// if an element is not drawn yet, it can not be found!
        /// </summary>
        /// <param name="x">X coordinate to set for</param>
        /// <param name="y">Y coordinate to set for</param>
        /// <param name="silent">If true, change event will not get fired.</returns>
        void SetCursor(int x, int y, bool silent = true);
    }
    #endregion

    #region IHITTESTABLE
    public interface IHitTestable
    {
        /// <summary>
        /// Tests if given location lies within the bounds of this object.
        /// </summary>
        /// <param name="x">X co-ordinate of the locaiton.</param>
        /// <param name="y">Y co-ordinate of the location.</param>
        /// <returns>True if the location lies within bounds of this object otherwise false.</returns>
        bool Contains(float x, float y);
    }
    #endregion

    #region IFLUSHABLE
    /// <summary>
    /// Represents an object which has ability to flush itself.
    /// </summary>
    public interface IFlushable
    {
        /// <summary>
        /// Flushes all current values.
        /// </summary>
        void Flush();
    }
    #endregion

    #region IPARAM-RECEIVER
    public interface IParamReceiver<T, U>: IParameter 
        where T: IParameter
        where U: IParameter
    {
        /// <summary>
        /// Modifies this object with specified single parameter.
        /// </summary>
        /// <param name="parameter">Parameter which this object is to be modifed with.</param>
        T Modify(U parameter);
    }
    #endregion

    #region IMULTI-PARAM-RECEIVER
    public interface IMultiParamReceiver<T, U> : IParameter
        where T : IParameter
        where U : IParameter
    {
        /// <summary>
        /// Modifies this object with specified multiple parameters.
        /// </summary>
        /// <param name="parameters">Parameters which this object is to be modified with.</param>
        T Modify(IEnumerable<U> parameters);

        /// <summary>
        /// Modifies this object with specified multiple parameters.
        /// </summary>
        /// <param name="parameter">Parameters which this object is to be modified with.</param>
        T Modify(U parameter);
    }
    #endregion

    #region IMODIFY-COMMAND-HOLDER
    public interface IModifyCommandHolder
    {
        ModifyCommand ModifyCommand { get; }
    }
    #endregion

    #region IMODIFIER<T>
    public interface IModifier<T> : IModifyCommandHolder
    {
        T Modify(T other);
    }
    #endregion

    #region IREFRESHABLE
    /// <summary>
    /// Represents an object which can be redrawn on any parent window of which it is sort of part of. 
    /// i.e belonging to the control collection of that window.
    /// </summary>
    public interface IRefreshable
    {
        /// <summary>
        /// Initializes this object using given collection of parameters (optional).
        /// </summary>
        /// <param name="parameters">Optional colloection of parameters.</param>
        bool Refresh(UpdateCommand command = 0);
    }
    #endregion

    #region ICOPY
    public interface ICopy
    {
        /// <summary>
        /// Copies portion of data specified by copyX, copyY, copyW, copyH parameters from a given memory block and 
        /// pastes it onto this texture at given loaction specified by dstX and dstY parameters.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="copyArea">Area in source to copy.</param>
        /// <param name="command">Draw command to to control copy task</param>
        void CopyFrom(ICopyable source, IBounds copyArea, CopyCommand command = 0, IPoint dstPoint = null, UpdateCommand updateCommand = 0);
    }
    #endregion

    #region IPAGEABLE
    public interface IPageable: IItemSpread
    {
        /// <summary>
        /// Gets total number of items contained across all pages.
        /// </summary>
        int ItemCount { get; }

        /// <summary>
        /// Gets total number of items contained in this object and visible on screen.
        /// </summary>
        int VisibleCount { get; }

        /// <summary>
        /// Indicates if this object is expanded or not.
        /// </summary>
        bool IsExpanded { get; }

        /// <summary>
        /// Gets index of current page.
        /// </summary>
        int PageIndex { get; }
    }
    #endregion

    #region ICOPYABLE OBJECT
    public interface ICopyableObject : ISize
    {
        /// <summary>
        /// Takes a screen shot of this window to a given surface.
        /// </summary>
        /// <param name="surface">Surface object which will have resultant screen-shot image.
        /// <param name="parameters">Limits the area of screen-shot. 
        /// Expected parameters are:
        /// 1. ScreenShotType
        /// 2. Clip to crop the result image. Clip location must be in relation to location of DrawnBounds
        /// of this object.
        /// </param>        
        bool TakeScreenShot<T>(T surface, IEnumerable<IParameter> parameters = null)
            where T : IImageContext, ISource<IntPtr>, IRenderer;
    }
    #endregion

    #region ICLEARABLE <TPARAMETER>
    /// <summary>
    /// Represents an object which has something which can be cleared.
    /// The clearing process can be influenced by the parameter if supplied.
    /// </summary>
    public interface IClearable<TParameter>
    {
        /// <summary>
        /// Clears this object using parameters supplied.
        /// </summary>
        /// <param name="parameter">Parameter to influence the clearing process.
        /// </param>
        void Clear(TParameter parameter = default(TParameter));
    }
    #endregion

    #region IEx CLEARABLE<TPARAMETER>
    /// <summary>
    /// Represents an object which has something which can be cleared.
    /// The clearing process can be influenced by the parameter if supplied.
    /// </summary>
    internal interface IExClearable<TParameter>
    {
        /// <summary>
        /// Clears this object using parameters supplied.
        /// </summary>
        /// <param name="parameter">Parameter to influence the clearing process.
        /// </param>
        void Clear(TParameter parameter = default(TParameter));
    }
    #endregion

    #region ICLEARABLE
    /// <summary>
    /// Represents an object which has clearable area.
    /// Expected parameters :
    /// Parameter of ClearCommand for example ClearCommands.Screen.Add()
    /// Instance of CopyArea structure as area to clear
    /// Instance of Point structure as destination.
    /// Instance of Offset structure as offset to destination point
    /// Instance of Clip class as clipped area to limit paste operation area
    /// Instance of Rotation class to rotate copied data while pasting it to the destination.
    /// Instance of Scale class to scale copied data while pasting it to the destination.
    /// Instance of IPen interface to serve as background while pasting it to the destination.
    /// </summary>
    public interface IClearable : IClearable<IEnumerable<IParameter>>
    { }
    #endregion

    #region IUPDATABLE<T, U>
    /// <summary>
    /// Represents an object which has a capability to update itself.  
    /// </summary>
    public interface IUpdatable<TValue, TParameter> 
    {
        /// <summary>
        /// Updates this object with given value.
        /// Operation is dependent on parameter supplied.
        /// </summary>
        /// <param name="value">Value by which this object to be updated.</param>
        /// <param name="parameter"></param>
        void Update(TValue value, TParameter parameter = default(TParameter));
    }
    #endregion

    #region IEx UPDATABLE<T, U>
    /// <summary>
    /// Represents an object which has a capability to update itself.  
    /// </summary>
    internal interface IExUpdatable<TValue, TParameter>
    {
        /// <summary>
        /// Updates this object with given value.
        /// Operation is dependent on parameter supplied.
        /// </summary>
        /// <param name="value">Value by which this object to be updated.</param>
        /// <param name="parameter"></param>
        void Update(TValue value, TParameter parameter = default(TParameter));
    }
    #endregion

    #region IUPDATABLE
    /// <summary>
    /// Represents an object which has a capability to update or invalidate the screen display. For example render window.
    /// It also supports selective update of certain area invalidated currently.
    /// </summary>
    public interface IUpdatable : IUpdatable<IBounds, UpdateCommand>, ISize
    { }
    #endregion

    #region IAUTOSIZABLE
    /// <summary>
    /// Represents an object which size can be set to be fitting its content.
    /// </summary>
    public interface IAutoSizable
    {
        /// <summary>
        /// Gets or sets a flag indicating if this popup should resize automatically according to the size of its items.
        /// </summary>
        bool AutoSize { get; set; }
    }
    #endregion

    #region IVISIBLE
    public interface IVisible
    {
        /// <summary>
        /// Gets visibility flag of this object.
        /// </summary>
        bool Visible { get; }
    }

    public interface IVisibilityManagement : IVisible
    {
        /// <summary>
        /// Gets or sets visibility flag of this object.
        /// </summary>
        new bool Visible { get; set; }
        event EventHandler<ICancelEventArgs> VisibleChanged;
    }
    #endregion

    #region IENABLE
    public interface IEnable
    {
        /// <summary>
        /// Gets if an object is enabled or not. However, only visible object can be treated enabled if it is enabled for the purpose of receiving inputs.
        /// </summary>
        bool Enabled { get; }
    }
    public interface IEnable2 : IEnable
    {
        /// <summary>
        /// Gets or sets if an object is enabled or not. However, only visible object can be treated enabled if it is enabled for the purpose of receiving inputs.
        /// </summary>
        new bool Enabled { get; set; }
    }
    #endregion

    #region ICLICKABLE
    public interface IClickable
    {
        event EventHandler<IEventArgs> Click;
    }
    #endregion

    #region IROTATESCALABLE
    public interface IRotatableSource
    {
        /// <summary>
        /// Rotates and scales memory block with specified rotation and scale factor with or without anti-aliasing.
        /// Source: https://www.drdobbs.com/architecture-and-design/fast-bitmap-rotation-and-scaling/184416337
        /// Original method does not do anti-alising but this one does.
        /// </summary>
        /// <param name="parameters">Collection of parameters to affect and control rotation and scaling operation.</param>
        /// <returns>esultant rotated and scalled data along with its size.</returns>
        Tuple<ISize, IntPtr> RotateAndScale(IEnumerable<IParameter> parameters);
    }
    #endregion

    #region IDRAW
    public interface IDraw
    {
        /// <summary>
        /// Represents an object which can be drawn drawn directly on its host surface.
        /// </summary>
        /// <param name="renderer">Buffer to draw this object to.</param>
        /// <param name="parameters">Collection of parameters to influence or control this drawing process.</param>
        /// <returns>True if operation is handled within this method otherwise returns false.</returns>
        bool Draw(IEnumerable<IParameter> parameters, IRenderer renderer);
    }
    #endregion

    #region IEx DRAW
    internal interface IExDraw
    {
        /// <summary>
        /// Represents an object which can be drawn drawn directly on its host surface.
        /// </summary>
        /// <param name="renderer">Buffer to draw this object to.</param>
        /// <param name="parameters">Collection of parameters to influence or control this drawing process.</param>
        /// <returns>True if operation is handled within this method otherwise returns false.</returns>
        bool Draw(IEnumerable<IParameter> parameters, IExRenderer renderer);
    }
    #endregion

    #region IDRAWN AREA
    public interface IDrawnArea
    {
        /// <summary>
        /// Gets area occupies after rendering this object to the display host.
        /// </summary>
        IUpdateArea DrawnArea { get; }
    }
    #endregion

    #region IEx DRAWN AREA
    internal interface IExDrawnArea : IDrawnArea
    {
        new IExBoundary DrawnArea { get; }
    }
    #endregion  

    #region ISETTINGS
    /// <summary>
    /// Represents an object which supports offset rendering/ reading.
    /// </summary>
    public interface ISettings : IParameter
    {
        /// <summary>
        /// Copies setting from another settings object.
        /// </summary>
        /// <param name="parameters">Collection of parameters to copy settings from.
        /// If null then all current settings will be flushed.</param>
        void ReceiveSettings(IEnumerable<IParameter> parameters = null);

        void FlushSettings();
    }
    #endregion

    #region IFOCUSABLE
    /// <summary>
    /// Represents an object which can receive or lose focus and can convey the current status i.e it is focused or not.
    /// </summary>
    public interface IFocusable : IFocus
    {
        /// <summary>
        /// Indicates if this object can receive focus at the moment.
        /// </summary>
        bool Focusable { get; }

        /// <summary>
        /// Bring focus to the object. Only one object can have focus at a time.
        /// </summary>
        /// <returns></returns>
        bool Focus();
    }
    #endregion

    #region IFOCUS
    public interface IFocus
    {
        /// <summary>
        /// Indicates if the object is currently focused.
        /// </summary>
        bool Focused { get; }
    }
    #endregion

    #region ISTATE
    internal interface IExViewState
    {
        /// <summary>
        /// Gets or sets state of this renderer. 
        /// IsResizing and IsDisposing flags can not be set publicly.
        /// </summary>
        ViewState ViewState { get; }
    }
    #endregion

    #region ITEXT HOLDER
    public interface ITextHolder
    {
        /// <summary>
        /// Gets a value displayed in a title bar or area of this object.
        /// </summary>
        string Text { get; }
    }
    #endregion

    #region ILOCATION HOLDER
    public interface ILocationHolder : IPoint
    {
        /// <summary>
        /// Gets or Sets location of this object.
        /// </summary>
        Location Location { get; set; }
    }
    #endregion

    #region ISIZE HOLDER
    public interface ISizeHolder : ISize
    {
        /// <summary>
        /// Gets or Sets size of this object.
        /// </summary>
        Size Size { get; set; }
    }
    #endregion
}
