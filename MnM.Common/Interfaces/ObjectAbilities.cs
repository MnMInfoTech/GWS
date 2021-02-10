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
    #endregion

    #region IDISPOSABLE
    public interface IDisposable2 : IDisposable
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

    #region IHIDABLE
    public interface IHideable
    {
        /// <summary>
        /// Hides this object from screen.
        /// </summary>
        void Hide();
    }
    #endregion

    #region IRECOGNIZABLE
    /// <summary>
    /// Represents an object which can be recognized by name in GWS.
    /// </summary>
    public interface IRecognizable
    {
        /// <summary>
        /// Type Name of this object.
        /// </summary>
        string TypeName { get; }
    }
    #endregion

#if (GWS || Window)

    #region IRENDERABLE
    /// <summary>
    ///Marker interface -  Represents an object which has a unique ID and can be rendered on screen.
    public interface IRenderable : IID, IRecognizable
    {        
        /// <summary>
        /// Name of this object.
        /// </summary>
        string Name { get; }
    }
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

    #region IRENDERABLE-BLOCK
    public interface IRenderableBlock: IDrawable
    {
        IPoint CopyPoint { get; set; }
        ISize CopySize { get; set; }
    }
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
    public interface IReadable : IBlockable, IPenContext, ICopyable
    {
        /// <summary>
        /// Reads a pixel after applying applying offset and rotation transformation (if exists) to get the correct co-ordinate.
        /// </summary>
        /// <param name="x">X co-ordinate of the location to read pixel from.</param>
        /// <param name="y">Y co-ordinate of the location to read pixel from.</param>
        /// <param name="session">Session object which provides information as to how to read data.</param>
        /// <returns>Pixel value.</returns>
        int ReadPixel(int x, int y, IReadSession session);

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
        /// <param name="session">Session object which records drawing area and has shape id and destination info.</param>
        void ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length, IReadSession session);
    }
    #endregion

    #region IWRITABLE
    public interface IWritable : IBlockable
    {
        /// <summary>
        /// Renders any element on this given object. This renderer has a built-in support for the following kind of elements:
        /// 1. IDrawable
        /// 2. IFigurable
        /// 3. IShape
        /// Please note that in case your element does not implement any of the above, you must provide your own rendering routine.
        /// Once you have handled it return true otherwise false.
        /// </summary>
        /// <param name="Renderable">Renderable object which is to be rendered</param>
        /// <param name="Settings">A context which can be a Pen, Rgba color, Brush or RenderInfo object.</param>
        /// <returns>Returns true if this renderer was able to successfully render the element otherwise false.</returns>
        void Render(IRenderable Renderable, ISettings Settings);

        /// <summary>
        /// Writes pixel to this block at given axial position using specified color.
        /// </summary>
        /// <param name="val">Position on axis - X cordinate if horizontal otherwise Y.</param>
        /// <param name="axis">Position of axis -Y cordinate if horizontal otherwise X.</param>
        /// <param name="horizontal">Axis orientation - horizontal if true otherwise vertical.</param>
        /// <param name="color">Color to write at given location.</param>
        ///<param name="Alpha">Value by which blending should happen if at all it is supplied.</param>
        /// <param name="Command">Command to control pixel writing.</param>
        /// <param name="session">Session object which records drawing area and has shape id and destination info.</param>
        void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, Command Command, ISession session);

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
        /// <param name="session">Boundary object which records drawing area and has shape id and destination info.</param>
        unsafe void WriteLine(int* colors, int srcIndex, int srcW, int length, bool horizontal,
            int x, int y, float? Alpha, byte* imageAlphas, Command Command, ISession  session);
    }
    #endregion

    #region IWRITABLE-BLOCK
    public interface IWritableBlock : IBlockable
    {
        /// <summary>
        /// Writes portion of data specified by copyX, copyY, copyW, copyH parameters from a given memory block and 
        /// pastes it onto this texture at given loaction specified by dstX and dstY parameters.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="srcW"></param>
        /// <param name="srcH"></param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyArea">Specifies the area to copy from this object.</param>
        /// <param name="Command">Draw command to to control copy task</param>
        /// <param name="alphaBytes">Alpha channel information (optional).</param>
        IPerimeter WriteBlock(IntPtr source, int srcW, int srcH, int dstX, int dstY, IPerimeter copyArea, Command Command = 0, IntPtr alphaBytes = default(IntPtr));
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
        /// <param name="destination">Specifies a pointer where the block should get copied</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line</param>
        /// <param name="dstX">Specifies the X coordinate where the paste operation should commence</param>
        /// <param name="dstY">Specifies the Y coordinate from where the paste operation should commence</param>
        /// <param name="copyArea">Specifies the area to copy from this object.</param>
        /// <param name="command">Draw command to control this operation.</param>
        /// <returns>Area covered by this operation.</returns>
        unsafe IPerimeter CopyTo(IntPtr destination, int dstLen, int dstW, int dstX, int dstY, IPerimeter copyArea, Command command = 0);
    }
    #endregion

    #region ICLEARABLE
    /// <summary>
    /// Represents an object which has clearable area.
    /// </summary>
    public interface IClearable
    {
        /// <summary>
        /// Clears data blocks covered by area specified by x, y, width and height paramters.
        /// </summary>
        /// <param name="clearArea">Area to be cleared.</param>
        /// <param name="command">A command to control clearing operation.</param>
        /// <param name="processID">ID of the process which initiated this operation.</param>
        IPerimeter Clear(IPerimeter clearArea, Command command = 0);
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
        /// <param name="processID">ID of the process which initiated this operation.</param>
        void Update(Command command, IPerimeter boundary);
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
        /// <param name="command">Command to control refresh task.</param>
        void Refresh(Command command = 0);
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
        IPerimeter ClipRectangle { get; set; }
    }
    #endregion

    #region ICOSOLIDATOR
    public interface IConsolidator 
    {
        /// <summary>
        /// Copies consolidated data to target destination. Very useful for mixing 2 images.
        /// Where this image serves as foreground image and backBuffer serves as background image. 
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="dstLen"></param>
        /// <param name="dstW"></param>
        /// <param name="dstX"></param>
        /// <param name="dstY"></param>
        /// <param name="backBuffer"></param>
        /// <param name="Command"></param>
        /// <param name="Pen"></param>
        /// <returns></returns>
        IPerimeter Consolidate(IntPtr destination, int dstLen,
            int dstW, int dstX, int dstY, IPerimeter copyArea, IMultiBuffered backBuffer, Command Command = Command.None, IntPtr? Pen = null);
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
    public interface IBackground
    {
        /// <summary>
        /// Sets background for this object.
        /// </summary>
        IPenContext Background { set; }
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

    #region IWINDOWABLE
    public interface IWindowable : IRefreshable, ISize, IFocusable, IMoveable, IPoint,
        IResizable, IOVerlap, IMinMaxSizable, IShowable, IHideable, IVisible2, IMinimalEvents, IDisposable
#if Advanced
        , IEvents
#endif
    {
    }
    #endregion

    #region IPAINTABLE
    public interface IPaintable
    {
        /// <summary>
        /// Invoke paint events to handle external paint routines defined by the user.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="processID">ID of the process which initiated this operation.</param>
        void InvokePaint(Command command, int processID = 0);
    } 
    #endregion
#endif
}
