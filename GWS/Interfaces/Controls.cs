/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.ComponentModel;
#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif

namespace MnM.GWS
{
    #region IWIDGET
    public partial interface IWidget : IItem, IVisibilityManagement, IEnable2,
        IDrawnArea, IName2, INotToBeImplementedOutsideGWS
    { }
    #endregion

    #region IExWIDGET
    internal partial interface IExWidget : IWidget, IExItem, IExDraw, IExDrawnArea, IDisposable

#if !Advance
        , IExRefreshProperties
#endif
    { }
    #endregion

    #region IPOPUP
    public interface IPopup : IIndependent, IHideable
    {
        /// <summary>
        /// Gets or sets a flag to determine if this popup should hide on any item selection.
        /// </summary>
        bool HideOnClick { get; set; }
    }
    #endregion

    #region IEx-POPUP
    internal interface IExPopup : IPopup, IExEventPusher
    { }
    #endregion

    #region ICUT
    /// <summary>
    /// Represents an object which has a capability to apply a cut to any axial line in order to fragment and omit an unwanted portion.
    /// </summary>
    public interface ICut
    {
        /// <summary>
        /// Indicates if this object is in-fact forms a closed loop i.e an ellipse.
        /// This property indicates true if no start and end angle is provided i.e both of them are zero or one of them is 0 and other one is 360 degree.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets the currently associated function to check if certain point is ok with in the context of this object.
        /// </summary>    
        /// <param name="val">Position of reading on axis i.e X coordinate if horizontal otherwise Y</param>
        /// <param name="axis">Position of reading on axis i.e Y coordinate if horizontal otherwise X</param>
        /// <param name="horizontal">Direction of reading: if true horizontally otherwise vertically</param>
        /// <returns>True if conditional logic validates to true otherwise false.</returns>
        bool CheckPixel(float val, int axis, bool horizontal);

        /// <summary>
        /// Performs ray tracing on cutlines and adds result values to the list.
        /// </summary>
        /// <param name="axis">Position of reading on axis i.e Y coordinate if horizontal otherwise X</param>
        /// <param name="horizontal">Direction of reading: if true horizontally otherwise vertically</param>
        /// <param name="list"></param>
        void AddValsSafe(int axis, bool horizontal, ICollection<float> list);
    }
    #endregion

    #region ITEXT DISPLAYER
    public interface ITextDisplayer : ITextHolder
    {
        /// <summary>
        /// Gets or sets a value displayed in a title bar or area of this object.
        /// </summary>
        new string Text { get; set; }
    }
    #endregion
}
#endif
