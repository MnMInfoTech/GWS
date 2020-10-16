/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
#if (GWS || Window)
    #region IELEMENT
    /// <summary>
    /// Represents an object which has a place in GWS object eco system.
    /// This is an entry point interface to be in the GWS object eco system.
    /// A minimum required interface to inherit in order to make your control work in the GWS.
    /// It must have an ID, a name Name and area to work upon.
    /// </summary>
    public interface IElement : IID, IRenderable, IRecognizable, IBounds
#if Advanced
        , IEventPusher
#endif
    { }
    #endregion

    public interface IDependentObject : IDependent, ISize,
        IBounds, IMinimalEvents, IInvalidatable, IFocusable, 
        IRefreshable, IVisible2, IBackground, IForeground
    { }

    #region IWINCONTROL
    public interface IWindowControl : IDependentObject,
        IHandle, IEventPusher, IClearable
    {
        /// <summary>
        /// Gets or sets the text of this control.
        /// </summary>
        string Text { get; set; }

        bool IsDisposed { get; }
    }
    #endregion

    #region IOBJECT
    public interface IObject : IDependentObject, IElement, IObjectHandle, IRefreshable
#if Advanced
        , IEventPusher
#endif
    { 
        Vector Location { get; set; }
        Size Size { get; set; }
    }
    #endregion

#if Advanced
    #region ICONTROL
    /// <summary>
    /// Represents an object which is in fact a full fledged control. It is a fully expanded version of IElement interface.
    /// All the UI controls such as ITextBox, IListBox etc. etc. derives from this.
    /// </summary>
    public interface IControl : IObject, IWindowable, IMouseDrag, IEventPusher
    {
        /// <summary>
        /// Gets or sets ZOrder for this object.
        /// </summary>
        int ZOrder { get; set; }

        /// <summary>
        /// Gets or sets current page for this object in parent collection.
        /// </summary>
        int Page { get; set; }

        new Rectangle Bounds { get; }

        event EventHandler<IEventArgs> AbilityChanged;
    }
    #endregion

    #region IFRAME
    public interface IFrame : IElement, IWindowable, IHost, IInteractable
    { }
    #endregion

    #region IANIMATOR
    /// <summary>
    /// Represents an animator popup with built-in as well as userdefined animation capabilities.
    /// </summary>
    public interface IAnimator : IPopupObject
    {
        AnimationMode Mode { get; set; }
    }
    #endregion

    #region ITOOLTIP-CONTROL
    public interface IToolTipControl : IPopupObject
    {
        /// <summary>
        /// Gets the display.
        /// </summary>
        /// <value>The display.</value>
        ContentAlignment ContentDisplay { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [display tool tip].
        /// </summary>
        /// <value><c>true</c> if [display tool tip]; otherwise, <c>false</c>.</value>
        bool DisplayToolTip { get; set; }

        /// <summary>
        /// Shows the tool tip.
        /// </summary>
        /// <param name="tipText">The tip text.</param>
        /// <param name="p">The p.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="font">The font.</param>
        void ShowToolTip(string tipText, Vector p = default, int? duration = null, IFont font = null);

        /// <summary>
        /// Hides the tool tip.
        /// </summary>
        void HideToolTip();
    }
    #endregion

    #region IDDCCONTROL
    public interface IDDControl : IID
    {
        /// <summary>
        /// Gets or sets a value indicating whether [drop down style].
        /// </summary>
        /// <value><c>true</c> if [drop down style]; otherwise, <c>false</c>.</value>
        bool IsDDCStyle { get; set; }

        ///// <summary>
        ///// Gets or sets the size of the collapse.
        ///// </summary>
        ///// <value>The size of the collapse.</value>
        //Size CollapseSize { get; set; }

        Rectangle GapArea { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [dropped down].
        /// </summary>
        /// <value><c>true</c> if [dropped down]; otherwise, <c>false</c>.</value>
        bool DroppedDown { get; set; }

        Rectangle DropDownButton { get; }
    }
    #endregion

    #region IPOPUP-OBJECT
    /// <summary>
    /// Represents an object which has a capability to serve as temporary object on screen.
    /// This kind of objects gets preccesedence over other elements when it comes to receiving user inputs.
    /// </summary>
    public interface IPopupObject : IElement, IWipeable, ISize, IDisposable
    {
        /// <summary>
        /// True if this popup is visible.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Gets or sets a default background pen to be used while showing this popup.
        /// </summary>
        IReadContext Background { get; set; }

        /// <summary>
        /// Gets or sets a default foreground pen to be used while drawing a bunch of popup items on screen.
        /// </summary>
        IReadContext Foreground { get; set; }

        /// <summary>
        /// Gets or sets a flag to determine if this popup shoud hide on any item selection.
        /// </summary>
        bool HideOnClick { get; set; }
    }

    /// <summary>
    /// Represents a popup object of certain T type elements.
    /// </summary>
    /// <typeparam name="T">Type of items</typeparam>
    /// <typeparam name="U">EvenArgs class of Argument of type T</typeparam>
    public interface IPopupObject<T, U> : IPopupObject, IReadOnlyList<T>
    {
        /// <summary>
        /// Get a last clicket item.
        /// </summary>
        T ClickedItem { get; }

        /// <summary>
        /// Gets an item currently mouse is hovering on.
        /// </summary>
        T HoveredItem { get; }

        /// <summary>
        /// Copies items to the another item array.
        /// </summary>
        /// <param name="array"> Array the items to be copied to</param>
        /// <param name="arrayIndex">Index from which it should start paste items</param>
        void CopyTo(T[] array, int arrayIndex);

        /// <summary>
        /// Find an item on a given mouse coordinates along with the index it is situated in this popup object.
        /// </summary>
        /// <param name="e">Mouse coordinte argmetns</param>
        /// <param name="item">Item found if at all</param>
        /// <param name="index">Inde xof an item found</param>
        /// <returns></returns>
        bool FindItem(IMouseEventArgs e, out T item, out int index);

        /// <summary>
        /// Fires when a mouse hovers on an item.
        /// </summary>
        event EventHandler<U> Hover;

        /// <summary>
        /// Fires when a mouse is clicked on an item.
        /// </summary>
        event EventHandler<U> Click;
    }
    #endregion

    #region ISIMPLE-POPUP
    /// <summary>
    /// Represents a single layered popup object. i.e popup without any sub popup.
    /// </summary>
    public interface ISimplePopup : IPopupObject<ISimplePopupItem, ISimplePopupItemEventArgs>
    {
        /// <summary>
        /// Gets or sets a flag indicating if this popup should resize automatically according to the size of its items.
        /// </summary>
        bool AutoSize { get; set; }

        /// <summary>
        /// Gets or set a left margin of its items from the left and top of this popup.
        /// </summary>
        Vector LTMargin { get; set; }

        /// <summary>
        /// Gets or sets a min size of this popup.
        /// </summary>
        Size MinSize { get; set; }

        /// <summary>
        /// Gets or sets a font object to draw text of the items.
        /// </summary>
        IFont Font { get; set; }
    }
    #endregion

    #region ISIMPLE-LABEL
    public interface ISimpleLabel : IObject
    {
        string Text { get; set; }
        IFont Font { get; set; }
    }
    #endregion

    #region ISIMPLE BUTTON
    public interface ISimpleButton: ISimpleLabel
    {
        IBlock Image { get; set; }
        ImagePosition ImageAlingn { get; set; }
        IReadContext HoverStyle { get; set; }
    }
    #endregion
#endif
#endif
}
