/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region ILENGTH
    public interface ILength
    {
        /// <summary>
        /// Length of this memory block.
        /// </summary>
        int Length { get; }
    }
    #endregion

    #region IBLOCKABLE
    /// <summary>
    /// Represents an object which can be represent a memory block.
    /// </summary>
    public interface IBlockable : ISize, ILength
    { }
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
        /// <param name="newWidth">the new width of the object.If null is passed or nothing is passed the object will keep the original width.</param>
        /// <param name="newHeight">this new height of object. If null is passed or nothing is passed the object will keep the original height.</param>
        void Resize(int? newWidth = null, int? newHeight = null);
    }
    #endregion

    #region ICLONEABLE2
    public interface ICloneable2 : ICloneable
    {
        object Clone(int width, int height);
    }
    public interface ICloneable<T> : ICloneable
    {
        new T Clone();
    }
    #endregion

    #region IDISPOSABLE
    public interface IDisposed : IDisposable
    {
        /// <summary>
        /// Indicates whether this object is disposed or not.
        /// </summary>
        bool IsDisposed { get; }
    }
    #endregion

    #region IROTATION
    public interface IRotatable : IDrawParams
    {
        /// <summary>
        /// Gets Rotaion object for rotate, skew and transform operations.
        /// </summary>
        Rotation Rotation { get; }
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

#if (GWS || Window)

    #region IRENDERABLE
    /// <summary>
    ///Marker interface -  Represents an object which has a unique ID and can be rendered on screen.
    public interface IRenderable : IID
    { }
    #endregion

    #region IFIGURABLE
    /// <summary>
    /// 
    /// </summary>
    public interface IFigurable: IRenderable
    {
        /// <summary>
        /// Converts this object to shape.
        /// Returns null if this object can not be converted to shape.
        /// </summary>
        /// <returns></returns>
        IEnumerable<VectorF> Perimeter();
    }
    #endregion

    #region IDRAWABLE
    public interface IDrawable: IRenderable
    {
        /// <summary>
        /// Draws itself to the buffer by creating readable pen from the given context.
        /// If the context itself is pen and if it needs to be changed then this is possible by
        /// Calling GetPen method in renderer and then using it in renderer.
        /// </summary>
        /// <param name="buffer">Buffer to draw this object to.</param>
        /// <param name="Settings">A valid Settings to control drawing.</param>
        /// <returns>True if sucessfull otherwise false.</returns>
        bool Draw(IWritable buffer, ISettings Settings);
    }
    #endregion

    #region IDRAWABLE2
    public interface IDrawable2 : 
        IDrawable, IForeground, IBackground, IChild
    { }
    #endregion

    #region ISETTINGS-RECEIVER
    /// <summary>
    /// Represents an object which supports offset rendering/ reading.
    /// </summary>
    public interface ISettingsReceiver : IDrawParams
    {
        /// <summary>
        /// Copies setting from another settings object.
        /// </summary>
        /// <param name="settings">Settings object to copy data from. If null then all current settings will be flushed.</param>
        void Receive(IDrawParams settings, bool flushMode = false);
    }
    #endregion

    #region IREADABLE
    public interface IReadable : IBlockable, IPenContext, IID, ICopyable
    {
        bool Invert { get; set; }

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
        /// <param name="srcAlphas">Resultant alpha values for respective pixels of memory block.
        /// Unless this object is texture brush and Advanced version this will be null.</param>
        void ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length, out byte[] srcAlphas);
    }
    #endregion

    #region IWRITABLE
    public interface IWritable : IBlockable
    {
        /// <summary>
        /// Indicates if nothing can be written on this object now.
        /// </summary>
        bool CanNotWrite { get; }

        /// <summary>
        /// Writes pixel to this block at given axial position using specified color.
        /// </summary>
        /// <param name="val">Position on axis - X cordinate if horizontal otherwise Y.</param>
        /// <param name="axis">Position of axis -Y cordinate if horizontal otherwise X.</param>
        /// <param name="horizontal">Axis orientation - horizontal if true otherwise vertical.</param>
        /// <param name="color">Color to write at given location.</param>
        ///<param name="Alpha">Value by which blending should happen if at all it is supplied.</param>
        /// <param name="Command">Command to control pixel writing.</param>
        /// <param name="ShapeID">ID of shape which pixel is being written for.</param>
        void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, Command Command, string ShapeID, INotifier boundary);

        /// <summary>
        /// Writes line to the this block at given position specified by x and y parameters by reading specified source
        /// starting from give source index upto the length specified.
        /// </summary>
        /// <param name="colors">Source memory block to copy data from.</param>
        /// <param name="srcIndex">Location in source memory block from which copy should begin.</param>
        /// <param name="srcW">Width of source memory block.</param>
        /// <param name="length">Length up to which source should be read for writing.</param>
        /// <param name="horizontal"></param>
        /// <param name="x">X co-ordinate of the location where writing begins.</param>
        /// <param name="y">Y co-ordinate of the location where writing begins.</param>
        ///<param name="Alpha">Value by which blending should happen if at all it is supplied</param>
        /// <param name="Command">Command to control pixel line writing.</param>
        /// <param name="ShapeID">ID of shape which pixel line is being written for.</param>
        unsafe void WriteLine(int* colors, int srcIndex, int srcW, int length, bool horizontal,
            int x, int y, float? Alpha, byte* imageAlphas, Command Command, string ShapeID, INotifier boundary);

        /// <summary>
        /// Clears indices of last drawn pixels. 
        /// Only useful when a shape is drawn with Distinct flag of enum : Command.
        /// </summary>
        void ClearPixelRecord();
    }
    #endregion
     
    #region INOTIFIER
    public interface INotifier
    {
        /// <summary>
        /// Incorporates given perimeter specified by x1, y1, x2, y2 parameters.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        void Notify(int x1, int y1, int x2, int y2);
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

    #region ICOPYABLE
    /// <summary>
    /// Specifies copyable memory block with certain size and length.
    /// </summary>
    public interface ICopyable : IBlockable
    {
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
        /// <param name="command">Draw command to control the copy operation.</param>
        /// <param name="alphaBytes">Alpha channel information(optional).</param>
        /// <returns>Area covered by this operation.</returns>
        unsafe IRectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int dstLen, int dstW, int dstX, int dstY,
            Command command, string ShapeID = null);
    }
    #endregion

    #region IPASTABLE
    public interface IPastable : IBlockable
    {
        /// <summary>
        /// Copies portion of data specified by copyX, copyY, copyW, copyH parameters from a given memory block and 
        /// pastes it onto this texture at given loaction specified by dstX and dstY parameters.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="srcW"></param>
        /// <param name="srcH"></param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        /// <param name="Command">Draw command to to control copy task</param>
        /// <param name="ShapeID">ID of shape which pixels are being received from.</param>
        /// <param name="alphaBytes">Alpha channel information (optional).</param>
        IRectangle CopyFrom(IntPtr source, int srcW, int srcH, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH,
            Command Command, string ShapeID, IntPtr alphaBytes = default(IntPtr));
    }
    #endregion

    #region ICLEARABLE
    public interface IClearable
    {
        /// <summary>
        /// Clears data blocks covered by area specified by x, y, width and height paramters.
        /// </summary>
        /// <param name="clearX">Left most corner of region which is to be cleared.</param>
        /// <param name="clearY">Top most corner of region which is to be cleared.</param>
        /// <param name="clearW">Width of region which is to be cleared.</param>
        /// <param name="clearH">Height of region which is to be cleared.</param>
        /// <param name="command">A command to control clearing operation.</param>
        IRectangle Clear(int clearX, int clearY, int clearW, int clearH, Command command = 0);
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

    #region IUPDATABLE
    /// <summary>
    /// Represents an object which has a capability to update or invalidate the screen display. For example render window.
    /// It also supports selective update of certain area invaldated currently.
    /// </summary>
    public interface IUpdatable : ISize
    {
        /// <summary>
        /// Updates invalidated area on screen.
        /// </summary>
        /// <param name="command">Command to control this Update task.</param>
        /// <param name="boundary">Area to update.</param>
        void Update(Command command = 0, IRectangle boundary = null);
    }
    #endregion

    #region ITITLE DISPLAYER
    public interface ITextDisplayer
    {
        /// <summary>
        /// Gets or sets a value displayed in a title bar or area of this object.
        /// </summary>
        string Text { get; set; }
    }
    #endregion

    #region IBACKGROUND
    public interface IBackgroundPen
    {
        int[] PenData { get; }
    }

    public interface IBackground
    {
        /// <summary>
        /// Sets background for this object.
        /// </summary>
        IPenContext Background { get; set; }
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

    #region ISHOWABLE2
    public interface IShowable2
    {
        /// <summary>
        /// Shows this object on screen.
        /// </summary>
        void Show(int x, int y);
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
        IRectangle ClipRectangle { get; set; }
    }
    #endregion

    #region IELMENTFINDER
    public interface IElementFinder
    {
        /// <summary>
        /// Finds an element from this collection if it exists on a given x and y coordinates.
        /// the test is applied on a last drawn area rather than an actual area of each element so if an element is not drawn yet, 
        /// it can not be found!
        /// </summary>
        /// <param name="x">X coordinate to search for</param>
        /// <param name="y">Y coordinate to search for</param>
        /// <returns></returns>
        IRenderable FindElement(int x, int y);
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
        Size RotateAndScale(out IntPtr Data, Rotation angle, bool antiAliased = true, float scale = 1);

        /// <summary>
        /// Returns a flipped version of this object alogwith size of it.
        /// </summary>
        /// <param name="flipMode"></param>
        /// <returns>Flipped copy of this object.</returns>
        Size Flip(out IntPtr Data, FlipMode flipMode);
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
        IEventPusher ActiveObject { get; set; }

        /// <summary>
        /// Gets th actual location when the drag operation started.
        /// </summary>
        Vector DragLocation { get; }

        /// <summary>
        /// Gets current status in relation to mouse dragging routine.
        /// </summary>
        MouseDrag MouseDrag { get; }

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
    public interface IWindowable : IRefreshable, ISize, IFocusable, IMoveable, IPoint,
        IResizable, IOVerlap, IMinMaxSizable, IShowable, IHideable, IVisible2, IMinimalEvents, IDisposable
#if Advanced
        , IEvents
#endif
    {
    }
    #endregion

    #region IITEMIZED
    public interface IItemized
    {
        /// <summary>
        /// Gets child item at given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Item as given index in current page. Returns null if this object does not have children.</returns>
        IRenderable GetItem(int index);
    }

    public interface IItemized<T> : IItemized where T: IRenderable
    {
        /// <summary>
        /// Gets child item at given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>Item as given index in current page. Returns null if this object does not have children.</returns>
        new T GetItem(int index);
    }
    #endregion

    #region IWIPEABLE
    /// <summary>
    /// Represents an object which can be drawn and then wiped off completely restoring the state of screen
    /// exactly the same before it was drawn.
    /// Use this interface only for object for which you know drawn area before hand.
    /// </summary>
    public interface IWipeable : IDrawable2, IShowable2, IHideable
    {
        /// <summary>
        /// True if this popup is visible.
        /// </summary>
        bool Visible { get; }

        /// <summary>
        /// Gets the bounds of this object.
        /// </summary>
        Rectangle Bounds { get; }
    }
    #endregion
#endif
}
