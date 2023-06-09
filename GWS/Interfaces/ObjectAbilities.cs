/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MnM.GWS
{
    #region IOVERLAP
    /// <summary>
    /// Represents an object which can be drawn either on top or behind the other objects it overlaps with.
    /// </summary>
    public interface IOverlap
    {
        /// <summary>
        /// Bring the object upfront so nothing overlaps its area.
        /// </summary>
        void BringToFront();

        /// <summary>
        /// Sends the object to the bottom of the stack of objects so every other one overlaps it.
        /// </summary>
        void SendToBack();
    }
    #endregion

    #region ISHOWABLE
    public interface IShowable
    {
        /// <summary>
        /// Shows this object on screen.
        /// </summary>
        void Show();
    }
    #endregion

    #region IHIDABLE
    public interface IHideable
    {
        /// <summary>
        /// Hides this object from screen.
        /// </summary>
        void Hide(bool forcefully = false);
    }
    #endregion

    #region ISHOWABLE2
    public interface IShowable2
    {
        /// <summary>
        /// Shows this object on screen on given location.
        /// </summary>
        void Show(int x, int y);
    }
    #endregion

    #region IWIPEABLE
    /// <summary>
    /// Represents an object which can be drawn and then wiped off completely restoring the state of screen
    /// exactly the same before it was drawn.
    /// Use this interface only for object for which you know drawn area before hand.
    /// </summary>
    public interface IWipeable : IShowable2, IHideable, IVisible, IEnable
    { }
    #endregion

    #region IINPUTHOLDER
    public interface IInputStateHolder
    {
        ModifierKeys ModifierKey { get; }
        MouseButton MouseButton { get; }
    }
    #endregion

    #region IFOCUS MANAGEMENT
    public interface IFocusManagement : IFocusable, IFocusEvents
    { }
    #endregion

    #region ILAYOUT SUPPORT
    public interface ILayoutSupport
    {
        /// <summary>
        /// Gets or sets a flag to influence an ability of window to reflect changes in properties.
        /// </summary>
        bool SuspendLayout { get; set; }
    }
    #endregion

    #region IPROPERTY MANAGEMENT
    partial interface IPropertyManagement : IPropertyEvents
    { }
    #endregion
}
#endif

