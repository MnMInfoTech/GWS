/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    #region FACTORY
    public partial interface IFactory : IAttachment
    {
        #region IMAGE PROCESSOR
        /// <summary>
        /// Creates a new image processor. By default, GWS uses STBImage. For more info on STBImage visit: https://github.com/nothings/stb
        /// </summary>
        /// <returns>IImageProcessor</returns>
        IImageProcessor newImageProcessor();
        #endregion

        #region ANIMATED GIF FRAME
        /// <summary>
        /// Creates a new animated gif class.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IAnimatedGifFrame newAnimatedGifFrame(byte[] data, int delay);
        #endregion
    }
#if (GWS || Window)
    partial interface IFactory
    {
        #region TO PEN
        /// <summary>
        /// Creates a instance of Pen or Brush from the given context.
        /// </summary>
        /// <param name="context">Context to convert to IPen instance.</param>
        /// <param name="w">Width of IPen instance. Will be default if not supplied.</param>
        /// <param name="h">Height of IPen instance. Will be default if not supplied.</param>
        /// <returns>IPen</returns>
        IReadable ToPen(IPenContext context, int? w = null, int? h = null);
        #endregion

        #region SURFACE
        /// <summary>
        /// Creates a new Surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <returns>IBuffer</returns>
        ISurface newSurface(int width, int height);

        /// <summary>
        /// Creates a new Surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">Pointer containing data to use. Please note that the array will be converted to int[] first.
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <param name="makeCopy">If true then copy the buffer array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel pointer supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        ISurface newSurface(IntPtr pixels, int width, int height);

        /// <summary>
        /// Creates a new Surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        ISurface newSurface(int[] pixels, int width, int height, bool makeCopy = false);

        /// <summary>
        /// Creates a new Surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        ISurface newSurface(int width, int height, byte[] pixels, bool makeCopy = false);
        #endregion

        #region IMAGE
        /// <summary>
        /// Creates a new Image object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <returns>IBuffer</returns>
        IImage newImage(int width, int height);

        /// <summary>
        /// Creates a new Image object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">Pointer containing data to use. Please note that the array will be converted to int[] first.
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <returns>IBuffer</returns>
        IImage newImage(IntPtr pixels, int width, int height);

        /// <summary>
        /// Creates a new Image object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        IImage newImage(int[] pixels, int width, int height, bool makeCopy = false);

        /// <summary>
        /// Creates a new Surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        IImage newImage(int width, int height, byte[] pixels, bool makeCopy = false);
        #endregion

        #region CANVAS
        /// <summary>
        /// Creates a new Canvas object attached with given window.
        /// </summary>
        /// <param name="window">Window which this object belongs to.</param>
        /// <returns></returns>
        ICanvas newCanvas(IRenderTarget window);
        #endregion

        #region NATIVE WINDOW
        /// <summary>
        /// Gets direct form associated with native form or control.
        /// For example, System.Windows.Form or System.Windows.Control 
        /// if using Microsoft Windows.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns>I Form</returns>
        INativeTarget newNativeTarget(int x, int y, int w, int h);
        #endregion

        #region FORM
        /// <summary>
        /// Gets GWS form associated with native form or control.
        /// For example, System.Windows.Form or System.Windows.Control 
        /// if using Microsoft Windows.
        /// </summary>
        /// <param name="target">Target window For example, System.Windows.Form or System.Windows.Control</param>
        /// <returns>IForm</returns>
        IForm newForm(INativeTarget target);
        #endregion

        #region BRUSH - PEN
        /// <summary>
        /// Creates a new brush of certain width and height using specified fill style.
        /// </summary>
        /// <param name="style">Fill style to be used to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        IBrush newBrush(BrushStyle style, int width, int height);

        /// <summary>
        /// Creates a texture brush from image data source.
        /// </summary>
        /// <param name="data">Image data source</param>
        /// <param name="width">Width of source.</param>
        /// <param name="height">Height of source.</param>
        /// <returns></returns>
        ITextureBrush newBrush(IntPtr data, int width, int height);

        /// <summary>
        /// Creates a new pe from given color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        IReadable newPen(int color);
        #endregion

        #region COLOR
        /// <summary>
        /// Creates a new Colour structure with Red, Green, Blue and Alpha components.
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <param name="a">Alpha component</param>
        /// <returns></returns>
        Rgba newColor(byte r, byte g, byte b, byte a = 255);
        #endregion

        #region POLY FILL
        /// <summary>
        /// Creates new instance of IPolygonFill which can fill a polygon structure with specified PolyFill enum option.
        /// </summary>
        /// <returns>IPolygonFill</returns>
        IPolyFill newPolyFill();
        #endregion

        #region FONT
        /// <summary>
        /// Creates a new font with given parameters.
        /// </summary>
        /// <param name="fontStream">A stream containig font data</param>
        /// <param name="fontSize">Size of the font to be used to create any glyph required.</param>
        /// <returns>IFont object</returns>
        IFont newFont(Stream fontStream, int fontSize);
        #endregion

        #region LINE
        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        ILine newLine(float x1, float y1, float x2, float y2);
        #endregion

        #region CURVE
        /// <summary>
        /// Creates a new circle or ellipse or pie or an arc specified by the bounding area, start and end angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="rotation">Angle to apply rotation while rendering the arc/pie</param>
        ICurve newCurve(float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, CurveType type = 0, Rotation rotation = default(Rotation), VectorF scale = default(VectorF));

        /// <summary>
        /// Creates a curve replicationg data provided by conic parameter and specified array of three VectorF instances representing pie trianlge.
        /// </summary>
        /// <param name="conic">A conic whose perimeter will be used.</param>
        /// <param name="pieTriangle">Array of three VectorF instances representing pie trianlge.</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        ICurve newCurve(IConic conic, VectorF[] pieTriangle, CurveType type);
        #endregion

        #region CONIC
        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="rotation">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">Start angle from where a curve start</param>
        /// <param name="tiltAngle">Tilt angle is a deviation from 0 angle conic.</param>
        IConic newConic(Rotation rotation, float x, float y, float width, float height, float startAngle = 0, float endAngle = 0, float tiltAngle = 0);
        #endregion

        #region TETRAGON
        /// <summary>
        /// Creates a tetragon specified by four points and applies an angle of rotation if supplied.
        /// </summary>
        /// <param name="first">First point.</param>
        /// <param name="second">Second point.</param>
        /// <param name="third">Third point.</param>
        /// <param name="fourth">Fourth point.</param>
        ITetragon newTetragon(VectorF first, VectorF second, VectorF third, VectorF fourth);
        #endregion

        #region BEZIER
        /// <summary>
        /// Creates a bezier defined by either pointsData (float values) or pixels (points) and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="type">Type of bezier to create.</param>
        /// <param name="pointValues">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="points">Points which defines perimiter of the bezier.</param>
        IBezier newBezier(BezierType type, ICollection<float> pointValues, IList<VectorF> points);
        #endregion

        #region TRIANGLE
        /// <summary>
        /// Creates a new trianle formed by three points specified by x1, y1, x2, y2, x3, y3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="x1">X corodinate of the first point</param>
        /// <param name="y1">Y corodinate of the first point</param>
        /// <param name="x2">X corodinate of the second point</param>
        /// <param name="y2">Y corodinate of the second point</param>
        /// <param name="x3">X corodinate of the third point</param>
        /// <param name="y3">Y corodinate of the third point</param>
        ITriangle newTriangle(float x1, float y1, float x2, float y2, float x3, float y3);
        #endregion

        #region BOX
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        IBox newBox(int x, int y, int width, int height);
        #endregion

        #region BOXF
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        IBoxF newBoxF(float x, float y, float width, float height);
        #endregion

        #region BOUNDARY
        /// <summary>
        /// Gets a new boundary object.
        /// </summary>
        /// <returns></returns>
        IBoundary newBoundary();
        #endregion

        #region SHAPE
        /// <summary>
        /// Returns an instance of IShape.
        /// </summary>
        /// <param name="shape">Points to form a shape.</param>
        /// <param name="name">NAme of shape</param>
        /// <returns></returns>
        IFigure newFigure(IEnumerable<VectorF> shape, string name);
        #endregion

        #region GLYPHS
        /// <summary>
        /// Returns an instance of IGlyphs.
        /// </summary>
        /// <param name="text">Text of the glyphs.</param>
        /// <param name="area">Area of the glyphs.</param>
        /// <param name="resultGlyphs">Glyphs collection of the glyphs.</param>
        /// <param name="minHBY">Minimum horizontal bearing of the glyphs.</param>
        /// <returns></returns>
        IGlyphs newGlyphs(string text, RectangleF area, IList<IGlyph> resultGlyphs, float minHBY);
        #endregion

        #region ROUNDBOX
        /// <summary>
        /// Creates a new rouded box with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        IRoundBox newRoundBox(float x, float y, float w, float h, float cornerRadius, bool positiveLocation = false);
        #endregion

        #region POLYGON
        /// <summary>
        /// Creates a new polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon.</param>
        /// <param name="angle">Angle to apply rotation while rendering the polygon</param>
        IPolygon newPolygon(IList<VectorF> polyPoints);
        #endregion

        #region TEXT
        /// <summary>
        /// Cretes new text object with given parameters.
        /// </summary>
        /// <param name="glyphs">A list of processed glyphs collection from font</param>
        /// <param name="drawStyle">A specific drawstyle to use to measure and draw glyphs if desired so</param>
        /// <param name="dstX">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="dstY">X cordinate of destination location where glyphs to be drawn</param>
        IText newText(IList<IGlyph> glyphs, ITextStyle drawStyle = null, int? dstX = null, int? dstY = null);

        /// <summary>
        /// Cretes new text object with given parameters.
        /// </summary>
        /// <param name="font">the font object to be used to get glyphs</param>
        /// <param name="text">A text string to process to obtain glyphs collection from font</param>
        /// <param name="dstX">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="dstY">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="drawStyle">A specific drawstyle to use to measure and draw glyphs if desired so</param>
        IText newText(IFont font, string text, int dstX, int dstY, ITextStyle drawStyle = null);
        #endregion

        #region CONVERTER
        IConverter newConverter();
        #endregion

        #region SHAPE PARSER
        /// <summary>
        /// Creates new shape parser for preparing settings for a given shape rendering.
        /// </summary>
        /// <returns></returns>
        IShapeParser newShapeParser();
        #endregion
    }
#endif
    #endregion

    #region WINDOW FACTORY
#if (GWS && Window)
    partial interface IFactory
    {
        #region PROPERTIES
        /// <summary>
        /// Gets the primary scrren available with system default resoultion in the operating system.
        /// </summary>
        IScreen PrimaryScreen { get; }

        /// <summary>
        /// Gets the default window creatio flags from the operating system.
        /// </summary>
        int DefaultWinFlag { get; }

        /// <summary>
        /// Gets the flags required to create full screen desktop.
        /// </summary>
        int FullScreenWinFlag { get; }

        /// <summary>
        /// Returns all the availble scrrens with possible resoultion provided by operating system.
        /// </summary>
        IScreens AvailableScreens { get; }

        /// <summary>
        /// Gets the array of available pixelformats offered by the operating system.
        /// </summary>
        uint[] PixelFormats { get; }

        /// <summary>
        /// Gets default primary pixel format offered by the operating system.
        /// </summary>
        uint PixelFormat { get; }

        /// <summary>
        /// Indicates if a connection to by the operating system is established or not.
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// Gets the type of underlying operating system.
        /// </summary>
        OS OS { get; }

        /// <summary>
        /// Gets the latest error occured while interacting with by the operating system.
        /// </summary>
        string LastError { get; }
        #endregion

        #region WINDOW
        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="x">X coordinate of location of the window</param>
        /// <param name="y">Y coordinate of location of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="display"></param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <returns></returns>
        IWindow newWindow(string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null,
            RendererFlags? renderFlags = null);

        /// <summary>
        /// Create a new window from an existing window by refercing the exisitng window's handle.
        /// </summary>
        /// <param name="externalWindow">External window</param>
        /// <returns></returns>
        IWindow newWindow(IExternalTarget externalWindow);
        #endregion

        #region OPENGL CONTEXT
        /// <summary>
        /// Creates OpenGL context for the given window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        IGLContext newGLContext(IWindow window);
        #endregion

        #region GET WINDOW ID
        /// <summary>
        /// Gets the unique window id associated with the window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        int GetWindowID(IntPtr window);
        #endregion

        #region SAVE IMAGE AS BITMAP
        /// <summary>
        /// Saves specified buffer as an image file on disk on specified file path.
        /// </summary>
        /// <param name="image">Image which is to be saved</param>
        /// <param name="file">Path of a file where data is to be saved</param>
        /// <returns>Returns true, if operation is succesful otherwise false.</returns>
        bool SaveAsBitmap(IBlockable image, string file, Command command = Command.Screen);

        /// <summary>
        /// Saves specified buffer as an image file on disk on specified file path.
        /// </summary>
        /// <param name="Pixels"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="file"></param>
        /// <returns>Returns true, if operation is succesful otherwise false.</returns>
        bool SaveAsBitmap(IntPtr Pixels, int width, int height, string file);
        #endregion

        #region CURSOR TYPE ENUM CONVERSION
        /// <summary>
        /// Converts GWS cursor types to native operating system's cursor types.
        /// </summary>
        /// <param name="cursorType"></param>
        /// <returns></returns>
        int ConvertToSystemCursorID(CursorType cursorType);
        #endregion

        #region TEXTURE
        /// <summary>
        /// Crates a new texture from a given window.
        /// </summary>
        /// <param name="window">Window from which texture is to be created</param>
        /// <param name="w">Width of the texture</param>
        /// <param name="h">Height of the texture</param>
        /// <param name="isPrimary">Defines if its a primary one for the window</param>
        /// <param name="textureAccess">Defines the way texture can be accessed. Default is streaming.</param>
        /// <returns></returns>
        ITexture newTexture(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false, TextureAccess? textureAccess = null);

        /// <summary>
        /// Crates a new texture from a given window. Then copies dat from given buffer.
        /// </summary>
        /// <param name="window">Window from which texture is to be created</param>
        /// <param name="source">Buffer source to copy data from onto surface</param>
        /// <param name="isPrimary">Define if its a primary one for the window</param>
        /// <param name="textureAccess">Defines the way texture can be accessed. Default is streaming.</param>
        /// <returns></returns>
        ITexture newTexture(IRenderWindow window, ICopyable source, bool isPrimary = false, TextureAccess? textureAccess = null);
        #endregion

        #region RENDER TARGET
        IRenderTarget newRenderTarget(IRenderWindow window);
        #endregion

        #region PUSH, PUMP, POLL EVENTS
        /// <summary>
        /// Push the specified event to the active window.
        /// </summary>
        /// <param name="e">Event to push on</param>
        void PushEvent(IEvent e);

        /// <summary>
        /// Instructs Window manager to start pumping events to eligble window.
        /// </summary>
        void PumpEvents();

        /// <summary>
        /// Gives current event happeed on active window.
        /// </summary>
        /// <param name="e">Event which is just happened</param>
        /// <returns></returns>
        bool PollEvent(out IEvent e);
        #endregion

        #region SET CURSOR POSITION
        /// <summary>
        /// Sets window's cusor's position to specified x and y coordinates.
        /// </summary>
        /// <param name="x">X coordinate of the location where cursor should be placed</param>
        /// <param name="x">Y coordinate of the location where cursor should be placed</param>
        void SetCursorPos(int x, int y);
        #endregion

        #region MISC
        /// <summary>
        /// Disables the existing scrren saver of the operating system.
        /// </summary>
        void DisableScreenSaver();
        #endregion

        #region WAV PLAYER
        /// <summary>
        /// Createa new Wav player.
        /// </summary>
        /// <returns></returns>
        ISound newWavPlayer();
        #endregion
    }
#endif
    #endregion
}
