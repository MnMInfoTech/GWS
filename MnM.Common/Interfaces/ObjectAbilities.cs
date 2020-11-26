/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window) 
    using System;
    using System.Collections.Generic;

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

    #region IMIXABLE-BLOCK
    public unsafe interface IMixableBlock : IBlockable
    {
        int* Pixels(bool ForegroundBuffer = true);
        byte* AlphaValues(bool ForegroundBuffer = true);
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
        unsafe void ReadLine(int start, int end, int axis, bool horizontal, out int* pixels, out int srcIndex, out int length);
    }
    #endregion

    #region IWRITEABLE
    public interface IWritable : IBlockable, ICopyable, IClearable, IReceiver
#if Advanced
        , IElementFinder, IClippable
#endif
    {
        /// <summary>
        /// Gets curretly invalidated area of this object.
        /// </summary>
        Rectangle InvalidatedArea { get; }

#if Advanced
        /// <summary>
        /// Gets readable Target associated with this object;
        /// </summary>
        IReadable Target { get; }
#endif

        /// <summary>
        /// Renders any element on the given path. This renderer has a built-in support for the following kind of elements:
        /// 1. ICustomDrawable
        /// 2. IShape
        /// 3. IFigurable
        /// 4. ISelfDrawable - (Pro version).
        /// 5. IAreaDrawable.
        /// Please note that in case your element does not implement any of the above, you must provide your own rendering routine.
        /// Once you have handled it return true otherwise false.
        /// </summary>
        /// <param name="Renderable">Renderable object which is to be rendered</param>
        /// <param name="anyContext">A context which can be a Pen, Rgba color, Brush or RenderInfo object.</param>
        /// <returns>Returns true if this renderer was able to successfully render the element otherwise false.</returns>
        void Render(IRenderable Renderable, IContext anyContext = null);

        /// <summary>
        /// Writes pixel to this block at given axial position using specified color.
        /// </summary>
        /// <param name="val">Position on axis - X cordinate if horizontal otherwise Y.</param>
        /// <param name="axis">Position of axis -Y cordinate if horizontal otherwise X.</param>
        /// <param name="horizontal">Axis orientation - horizontal if true otherwise vertical.</param>
        /// <param name="color">Color to write at given location.</param>
        ///<param name="Alpha">Value by which blending should happen if at all it is supplied.</param>
        void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, DrawCommand drawCommand);

        /// <summary>
        /// Writes line to the this block at given position specified by x and y parameters by reading specified source
        /// starting from give source index upto the length specified.
        /// </summary>
        /// <param name="pixels">Source memory block to copy data from.</param>
        /// <param name="srcIndex">Location in source memory block from which copy should begin.</param>
        /// <param name="srcW">Width of source memory block.</param>
        /// <param name="length">Length up to which source should be read for writing.</param>
        /// <param name="horizontal"></param>
        /// <param name="x">X co-ordinate of the location where writing begins.</param>
        /// <param name="y">Y co-ordinate of the location where writing begins.</param>
        ///<param name="Alpha">Value by which blending should happen if at all it is supplied</param>
        unsafe void WriteLine(int* pixels, int srcIndex, int srcW, int length, bool horizontal,
            int x, int y, float? Alpha, byte* imageAlphas, DrawCommand drawCommand);
    }
    #endregion

    #region IRENDERABLE
    /// <summary>
    ///Marker interface -  Represents an object which has a unique ID and can be rendered on screen.
    public interface IRenderable : IID, IBoundsF
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
        IEnumerable<VectorF> Figure();
    }
    #endregion

    #region ICUTOMDRAWABLE
    public interface ICustomDrawable: IRenderable
    {
        /// <summary>
        /// Draws itself to the buffer by creating readable pen from the given context.
        /// If the context itself is pen and if it needs to be changed then this is possible by
        /// Calling GetPen method in renderer and then using it in renderer.
        /// </summary>
        /// <param name="buffer">Buffer to draw this object to.</param>
        /// <param name="Settings">A valid Settings to control drawing.</param>
        /// <returns>True if sucessfull otherwise false.</returns>
        bool Draw(IWritable buffer, IRenderInfo Settings);
    }
    #endregion

    #region IDRAWABLE
    /// <summary>
    /// An object which can be drawable to a given pixel target such as surface with a given pixel source.
    /// A smalll entities like point or entities which requires special drawing routine for example ISLine
    /// Inherit this interface if your shape/element is special in terms of drawing routine.
    /// </summary>
    public interface IDrawable : ICustomDrawable, IFigurable
    { }
    #endregion

    #region IDRAW
    public interface IDrawable2: IRenderable
    {
        /// <summary>
        /// Gets dedicated settings object associated with this control.
        /// </summary>
#if Advanced
        IRenderInfo2
#else
        IRenderInfo 
#endif
            Settings
        { get; }

        /// <summary>
        /// Draws itself to the buffer by creating readable pen from the given context.
        /// If the context itself is pen and if it needs to be changed then this is possible by
        /// Calling SetPen method in renderer and then using current pen in renderer through 
        /// CurrentPen property.
        /// </summary>
        void Draw();

        /// <summary>
        /// Returs current pen appropriated according to the current state of object.
        /// </summary>
        /// <returns>IPen</returns>
        /// <param name="Settings">Render settings to be used to get th pen. </param>
        /// <returns>IReadable - Pen</returns>
        IReadable GetPen(IRenderInfo Settings);
    }
    #endregion

    #region IAREADRAWABLE
    public interface IAreaDrawable: ICustomDrawable
    {
        Rectangle CopyArea { get; set; }
    }
    #endregion

    #region IOBJECTDRAWARE
    public interface IObjectAware
    {
        /// <summary>
        /// Tells this object that a control is being drawn now.
        /// </summary>
        IDrawable2 Control { get;
#if Advanced
            set;
#endif
        }
    }
    #endregion

    #region IBACKGROUND
    public interface IBackground
    {
        /// <summary>
        /// Sets background for this object.
        /// </summary>
        IPenContext Background { set; }

        /// <summary>
        /// Gets current background pen of this object.
        /// </summary>
        IReadable BackgroundPen { get; }
    }
    #endregion

    #region IFOREGROUND
    public interface IForeground
    {
        /// <summary>
        /// Sets foreground for this object.
        /// </summary>
        IPenContext Foreground { set; }

        /// <summary>
        /// Gets current foreground pen of this object.
        /// </summary>
        IReadable ForegroundPen { get; }
    }
    #endregion

    #region IHOVER-BACKGROUND
    public interface IHoverBackground
    {
        /// <summary>
        /// Sets background for this object.
        /// </summary>
        IPenContext HoverBackground { set; }

        /// <summary>
        /// Gets current background pen of this object.
        /// </summary>
        IReadable HoverBackgroundPen { get; }
    }
    #endregion

    #region IHOVR-FOREGROUND
    public interface IHoverForeground
    {
        /// <summary>
        /// Sets hovering foreground for this object.
        /// </summary>
        IPenContext HoverForeground { set; }

        /// <summary>
        /// Gets current foreground pen of this object.
        /// </summary>
        IReadable HoverForegroundPen { get; }
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
        Rectangle ClipRectangle { get; set; }
    }
    #endregion

    #region ICLEARABLE
    public interface IClearable
    {
        /// <summary>
        /// Clears data blocks covered by area specified by x, y, width and height paramters.
        /// </summary>
        /// <param name="x">Left most corner of region which is to be cleared.</param>
        /// <param name="y">Top most corner of region which is to be cleared.</param>
        /// <param name="width">Width of region which is to be cleared.</param>
        /// <param name="height">Height of region which is to be cleared.</param>
        /// <param name="command">A command to control clearing operation.</param>
        void Clear(int x, int y, int width, int height, DrawCommand command = 0);
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
        /// Invalidates data blocks covered by area specified by x, y, width and height parameters for later update.
        /// </summary>
        /// <param name="x">Left most corner of region which is to be updated.</param>
        /// <param name="y">Top most corner of region which is to be updated.</param>
        /// <param name="width">Width of region which is to be updated.</param>
        /// <param name="height">Height of region which is to be updated.</param>
        void Invalidate(int x, int y, int width, int height);

        /// <summary>
        /// Updates invalidated area on screen.
        /// </summary>
        void Update(DrawCommand command = DrawCommand.None);
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
        /// <returns></returns>
        unsafe Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int dstLen, int dstW, int dstX, int dstY,
            DrawCommand command = 0);

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render this memory block on</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        /// <param name="command">Draw command to control the copy operation.</param>
        Rectangle CopyTo(IBlockable block, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, DrawCommand command = 0);
    }
    #endregion

    #region IRECEIVABLE
    public interface IReceiver: IBlockable
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
        /// <param name="command">Draw command to to control copy task</param>
        void Receive(IntPtr source, int srcW, int srcH, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, DrawCommand command = 0);
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
        Size RotateAndScale(out IntPtr Data, Rotation angle, bool antiAliased = true, float scale = 1);

        /// <summary>
        /// Returns a flipped version of this object alogwith size of it.
        /// </summary>
        /// <param name="flipMode"></param>
        /// <returns>Flipped copy of this object.</returns>
        Size Flip(out IntPtr Data, Flip flipMode);
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
    public interface IWindowable : IRefreshable, ISize, IFocusable, IMoveable, IPoint,
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
