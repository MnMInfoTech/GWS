/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window) 
    using System;
    using System.Collections.Generic;

    #region IRENDERABLE
    /// <summary>
    ///Marker interface -  Represents an object which has a unique ID and can be rendered on screen.
    public interface IRenderable : IID, IBoundsF
    { }
    #endregion

    #region IDRAWABLE
    /// <summary>
    /// An object which can be drawable to a given pixel target such as surface with a given pixel source.
    /// A smalll entities like point or entities which requires special drawing routine for example ISLine
    /// Inherit this interface if your shape/element is special in terms of drawing routine.
    /// </summary>
    public interface IDrawable : IRenderable
    {
        /// <summary>
        /// Draws itself to the buffer by creating readable pen from the given context.
        /// If the context itself is pen and if it needs to be changed then this is possible by
        /// Calling SetPen method in renderer and then using current pen in renderer through 
        /// CurrentPen property.
        /// </summary>
        /// <param name="buffer">Buffer to draw this object to.</param>
        /// <param name="readContext">Read context to create a valid pen to draw on buffer.</param>
        bool Draw(IWritable buffer, IReadContext readContext, out IPen Pen);

        /// <summary>
        /// Converts this object to shape.
        /// Returns null if this object can not be converted to shape.
        /// </summary>
        /// <returns></returns>
        IEnumerable<VectorF> ToShape();
    }
    #endregion

    #region ICUSTOM
    public interface ICustom : IRenderable 
    { }
    #endregion

    #region IHOSTABLE
    /// <summary>
    /// Represents an object which is dependent on parent window to exist.
    /// </summary>
    public interface IHostable : IDrawable, IBounds, ILocation, ISize, IRecognizable
    {
        /// <summary>
        /// Gets bounds of this object.
        /// </summary>
        new Rectangle Bounds { get; }

        /// <summary>
        /// Parent window this object belongs to.
        /// </summary>
        ISurface Window { get; }

        /// <summary>
        /// Assigns host window to this object. 
        /// </summary>
        /// <param name="window"></param>
        void Assign(ISurface window);
    }
    #endregion

    #region IVISIBLE
    public interface IVisible
    {
        /// <summary>
        /// Gets if an object is visble or not.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Gets if an object is enabled or not. However, only visible object can be treated enabled if it is enabled for the purpose of receiving inputs.
        /// </summary>
        bool Enabled { get; }
    }

    /// <summary>
    /// Indicates if an object can be shown and hidden as wells as disabled and enabled.
    /// </summary>
    public interface IVisible2 : IVisible
    {
        /// <summary>
        /// Gets or sets if an object is visble or not.
        /// </summary>
        new bool Visible { get; set; }

        /// <summary>
        /// Gets or sets if an object is enabled or not. However, only visible object can be treated enabled if it is enabled for the purpose of receiving inputs.
        /// </summary>
        new bool Enabled { get; set; }
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

    #region ISHOWABLE2
    public interface IShowable2
    {
        /// <summary>
        /// Shows this object on screen.
        /// </summary>
        void Show(int x, int y);
    }
    #endregion

    #region IHIDABLE
    public interface IHideable
    {
        /// <summary>
        /// Hides this object from screen.
        /// </summary>
        void Hide();
    }
    #endregion

    #region IRESIZEABLE
    /// <summary>
    /// Indicates if an object that can be resized.
    /// </summary>
    public interface IResizable
    {
        /// <summary>
        /// Resizes an object.
        /// </summary>
        /// <param name="width">the new width of the object.If null is passed or nothing is passed the object will keep the original width.</param>
        /// <param name="height">this new height of object. If null is passed or nothing is passed the object will keep the original height.</param>
        void Resize(int? width = null, int? height = null);
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
        /// Redraws itself using the drawsettings used when it is first added to the collection of parent window.
        /// </summary>
        void Refresh();
    }
    #endregion

    #region IFOCUSABLE
    /// <summary>
    /// Indicates if an object can receive or lose focus.
    /// </summary>
    public interface IFocusable
    {
        /// <summary>
        /// Gets or sets a flag to determine this object should be receive focus or not while hovering over.
        /// </summary>
        bool FocusOnHover { get; set; }

        /// <summary>
        /// Indicates if the object is currently focused.
        /// </summary>
        bool Focused { get; }

        /// <summary>
        /// Indicates the position of an object  relation to the others  a particular collection to receive focus.
        /// </summary>
        int TabIndex { get; set; }

        /// <summary>
        /// Bring focus to the object. Only one object can have focus at a time.
        /// </summary>
        /// <returns></returns>
        bool Focus();
    }
    #endregion

    #region IINVALIDATABLE
    /// <summary>
    /// Represents an object which has a capability to invalidate the screen display.For example render window.
    /// It also supports selective update of certain area.
    /// </summary>
    public interface IInvalidatable
    {
        /// <summary> 
        /// Invalidates data blocks covered by area specified by x, y, width and height parameters for later update.
        /// </summary>
        /// <param name="x">Left most corner of region which is to be updated.</param>
        /// <param name="y">Top most corner of region which is to be updated.</param>
        /// <param name="width">Width of region which is to be updated.</param>
        /// <param name="height">Height of region which is to be updated.</param>
        /// <param name="updateImmediate">If true, Update method will immediately called and screen will get updated otherwise not.</param>
        void Invalidate(int x, int y, int width, int height, bool updateImmediate = false);
    }
    #endregion

    #region IPOPUPABLE
    public interface IPopupable : IHostable, IShowable2, IHideable
    {
        /// <summary>
        /// True if this popup is visible.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Gets the bounds of this object.
        /// </summary>
        new Rectangle Bounds { get; }
    }
    #endregion

    #region IWIPEABLE
    /// <summary>
    /// Represents an object which can be drawn and then wiped off completely restoring the state of screen
    /// exactly the same before it was drawn.
    /// Use this interface only for object for which you know drawn area before hand.
    /// </summary>
    public interface IWipeable : IPopupable
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

    #region ICLIPPABLE
    public interface IClippable
    {
        /// <summary>
        /// Indicates if this object has a clip rect assigned now or not.
        /// </summary>
        bool Clipped { get; }

        /// <summary>
        /// Gets or sets an area to restrict write operations.
        /// </summary>
        Rectangle ClipRect { get; set; }
    }
    #endregion

    #region ICLEARABLE
    public interface IClearable
    {
        /// <summary>
        /// Clears data blocks covered by area specified by x, y, width and height paramters.
        /// </summary>
        /// <param name="updateImmediate">If true, scrreen will immediately get updated otherwise marked for later update.</param>
        /// <returns></returns>
        Rectangle Clear(bool updateImmediate = false);

        /// <summary>
        /// Clears data blocks covered by area specified by x, y, width and height paramters.
        /// </summary>
        /// <param name="x">Left most corner of region which is to be cleared.</param>
        /// <param name="y">Top most corner of region which is to be cleared.</param>
        /// <param name="width">Width of region which is to be cleared.</param>
        /// <param name="height">Height of region which is to be cleared.</param>
        /// <param name="updateImmediate">If true, scrreen will immediately get updated otherwise marked for later update.</param>
        /// <returns></returns>
        Rectangle Clear(int x, int y, int width, int height, bool updateImmediate = false);
    }
    #endregion

    #region IUPDATABLE
    /// <summary>
    /// Represents an object which has a capability to update the screen display.For example render window.
    /// It also supports selective update of certain area.
    /// </summary>
    public interface IUpdatable : ISize
    {
        /// <summary>
        /// Updates invalidated area on screen.
        /// </summary>
        void Update();
    }
    #endregion

    #region ICOPYABLE
    /// <summary>
    /// Specifies copyable memory block with certain size and length.
    /// </summary>
    public interface ICopyable : ISize
    {
        /// <summary>
        /// Length of an inner 1D memory block.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render this memory block on</param>
        /// <param name="destX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="destY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        Rectangle CopyTo(IWritable block, int destX, int destY, int copyX, int copyY, int copyW, int copyH);

        /// <summary>
        /// Provides a paste routine to paste the specified chunk of data to a given destination pointer on a given location.
        /// </summary>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        /// <param name="destination">Specifies a pointer where the block should get copied</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line</param>
        /// <param name="dstX">Specifies the X coordinate where the paste operation should commence</param>
        /// <param name="dstY">specifies the Y coordinate from where the paste operation should commence</param>
        /// <returns></returns>
        unsafe Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int dstLen, int dstW, int dstX, int dstY);
    }
    #endregion

    #region IDRAW-CONTROLLER
    public interface IDrawController
    {
#if Advanced
        /// <summary>
        /// Gets settings object to set rendering parameters.
        /// </summary>
        IDrawSettings2 Settings { get; }
#else
        /// <summary>
        /// Gets settings object to set rendering parameters.
        /// </summary>
        IDrawSettings Settings { get; }
#endif
    }
    #endregion

    #region IROTATABLE
    /// <summary>
    /// Represents an object which has an angle of rotation. It must have bounds.
    /// </summary>
    public interface IRotatable
    {
        /// <summary>
        /// Angle of rotation by which it has to rotate.
        /// Angle.Empty can be used to specify no rotation.
        /// Angle with 0 or 360 degree value are considered an Empty Angle - offers no rotation.
        /// </summary>
        Rotation Rotation { get; }
    }
    #endregion

    #region IROTATESCALABLE
    public interface IScalable
    {
        /// <summary>
        /// Returns a rotated and scalled memory block of this object alongwith size of it.
        /// </summary>
        /// <param name="angle">Angle of rotation to apply.</param>
        /// <param name="antiAliased">If true copy is antialised version of this object otherwise not.</param>
        /// <param name="scale">Scale to apply.</param>
        /// <returns>Rotated and scalled copy of this object.</returns>
        Size RotateAndScale(out int[] Data, Rotation angle, bool antiAliased = true, float scale = 1);

        /// <summary>
        /// Returns a flipped version of this object alogwith size of it.
        /// </summary>
        /// <param name="flipMode"></param>
        /// <returns>Flipped copy of this object.</returns>
        Size Flip(out int[] Data, Flip flipMode);
    }
    #endregion

    #region IMOVEABLE
    /// <summary>
    /// Indicates if an object can be moved.
    /// </summary>
    public interface IMoveable
    {
        /// <summary>
        /// Moves an object.
        /// </summary>
        /// <param name="x">the new x cordinate which the object is to be moved to. If null is passed or nothing is passed the object will keep the original x position.</param>
        /// <param name="y">the new y cordinate which the object is to be moved to. If null is passed or nothing is passed the object will keep the original y position.</param>
        void Move(int? x = null, int? y = null);
    }
    #endregion

    #region IMINMAXSIZABLE
    /// <summary>
    /// Represents an object of which minimum size can be changed.
    /// </summary>
    public interface IMinSizable
    {
        /// <summary>
        /// Gets or sets minimum size of this object.
        /// </summary>
        Size MinSize { get; set; }
    }

    /// <summary>
    /// Represents an object of which maximum size can be changed.
    /// </summary>
    public interface IMaxSizable
    {
        /// <summary>
        /// Gets or sets maximum size of this object.
        /// </summary>
        Size MaxSize { get; set; }
    }
    public interface IMinMaxSizable : IMinSizable, IMaxSizable
    { }
    #endregion

    #region IOVERLAP
    /// <summary>
    /// Represents an object which can be drawn either on top or behind the other objects it overlaps with.
    /// </summary>
    public interface IOVerlap
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

    #region IINTERACTIVE
    /// <summary>
    /// Represents an object which is capable of keeping track of an element with in its collection of child elements for
    /// the purpose of routing user inputs to it so that it can use them.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Gets or sets an active object from the perspective of handling user inputs.
        /// </summary>
        IEventPusher ActiveObject { get; }

        /// <summary>
        /// Gets latest element which the mouse last hovered on.
        /// </summary>
        IRenderable HoveredItem { get; }

        /// <summary>
        /// Gets the current element which the mouse is drawgging now.
        /// </summary>
        IRenderable DraggedItem { get; }
    }
    #endregion

    #region IWINDOWABLE
    public interface IWindowable : IRefreshable, ISize, IFocusable, IMoveable, ILocation,
        IResizable, IOVerlap, IMinMaxSizable, IShowable, IHideable, IVisible2, IMinimalEvents, IDisposable
#if Advanced
        , IEvents
#endif
    {
    }
    #endregion

    #region IDISPOSABLE
    public interface IDisposed
    {
        /// <summary>
        /// Indicates whether this object is disposed or not.
        /// </summary>
        bool IsDisposed { get; }
    }
    #endregion

    #region ICLONEABLE2
    public interface ICloneable2 : ICloneable
    {
        object Clone(int width, int height);
    }
    #endregion
#endif
}
