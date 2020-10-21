/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
    using System;
    using System.IO;

    #region FACTORY
#if (GWS || Window)
    public partial interface IFactory : IAttachment
    {
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

        #region CANVAS
        /// <summary>
        /// Creates a new Canvas object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <returns>IBuffer</returns>
        ICanvas newCanvas(int width, int height);

        /// <summary>
        /// Creates a new Canvas object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">Pointer containing data to use. Please note that the array will be converted to int[] first.
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <param name="makeCopy">If true then copy the buffer array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel pointer supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        unsafe ICanvas newCanvas(IntPtr pixels, int width, int height);

        /// <summary>
        /// Creates a new Canvas object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        ICanvas newCanvas(int[] pixels, int width, int height, bool makeCopy = false);

        /// <summary>
        /// Creates a new Canvas object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        ICanvas newCanvas(int width, int height, byte[] pixels, bool makeCopy = false);

        /// <summary>
        /// Creates a new Canvas object attached with given window.
        /// </summary>
        /// <param name="window">Window which this object belongs to.</param>
        /// <returns></returns>
        ICanvas newCanvas(IRenderTarget window);
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
        IPen newPen(int color);
        #endregion

        #region TO PEN
        /// <summary>
        /// Creates a instance of Pen or Brush from the given context.
        /// </summary>
        /// <param name="context">Context to convert to IPen instance.</param>
        /// <param name="w">Width of IPen instance. Will be default if not supplied.</param>
        /// <param name="h">Height of IPen instance. Will be default if not supplied.</param>
        /// <returns>IPen</returns>
        IPen ToPen(IReadContext context, int? w = null, int? h = null);
        #endregion

        #region OBJECT COLLECTION
        IObjCollection newObjectCollection(ISurface buffer);
        #endregion

        #region BUFFER COLLECTION
#if Advanced
        /// <summary>
        /// Creates a collection to hold buffers to enable user to maintain and use multiple buffers with any parent window and graphics.
        /// </summary>
        /// <returns>IBufferCollection</returns>
        IBufferCollection newBufferCollection();

        /// <summary>
        /// Creates a collection to hold buffers to enable user to maintain and use multiple buffers with any parent window and graphics.
        /// </summary>
        /// <param name="primary">Primary buffer for this instance to use.
        /// If no null value is provided then ChangePrimary method will not be able to change the primary buffer value.
        /// As we have already provided dedicated primary buffer here.</param>
        /// <returns>IBufferCollection</returns>
        IBufferCollection newBufferCollection(ICanvas primary);

        /// <summary>
        /// Creates a collection to hold buffers to enable user to maintain and use multiple buffers with any parent window and graphics.
        /// </summary>
        /// <param name="capacity">Initiali capacity of the collection. the default is 4</param>
        /// <returns></returns>
        IBufferCollection newBufferCollection(int capacity);
#endif
        #endregion

        #region POLY FILL
        /// <summary>
        /// Creates new instance of IPolygonFill which can fill a polygon structure with specified PolyFill enum option.
        /// </summary>
        /// <returns>IPolygonFill</returns>
        IPolyFill newPolyFill();
        #endregion

        #region FONT, GLYPH, TEXT, RENDERER
        /// <summary>
        /// Creates a new Glyph renderer.
        /// </summary>
        /// <returns>IGlyphRenderer</returns>
        IGlyphRenderer newGlyphRenderer();

        /// <summary>
        /// Creates a new font with given parameters.
        /// </summary>
        /// <param name="fontStream">A stream containig font data</param>
        /// <param name="fontSize">Size of the font to be used to create any glyph required.</param>
        /// <returns>IFont object</returns>
        IFont newFont(Stream fontStream, int fontSize);
        #endregion

        #region MISC
        /// <summary>
        /// Creates a new animated gif class.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IAnimatedGifFrame newAnimatedGifFrame(byte[] data, int delay);
        #endregion

        #region IMAGE PROCESSOR
        /// <summary>
        /// Creates a new image processor. By default, GWS uses STBImage. For more info on STBImage visit: https://github.com/nothings/stb
        /// </summary>
        /// <returns>IImageProcessor</returns>
        IImageProcessor newImageProcessor();
        #endregion

        #region CONVERTER
        IConverter newConverter();
        #endregion

        #region PEN STORE
        /// <summary>
        /// Gets currently attached Pen store in GWS.
        /// </summary>
        IPens newPenStore();
        #endregion

        #region SHAPE PARSER
        /// <summary>
        /// Creates new shape parser for preparing settings for a given shape rendering.
        /// </summary>
        /// <returns></returns>
        IShapeParser newShapeParser();
        #endregion

        #region FOR ADVANCED VERSION
#if Advanced
        #region SIMPLE POPUP
        /// <summary>
        /// Creates a new simple popup using string values converted to simple popup items.
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        ISimplePopup newSimplePopup(params string[] items);

        /// <summary>
        /// Creates a new simple popup of specified width and height using string values converted to simple popup items.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        ISimplePopup newSimplePopup(int width, int height, params string[] items);
        #endregion

        #region SIMPLE LABLE
        /// <summary>
        /// Creates a new simple label.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        ISimpleLabel newSimpleLabel(string text = null, IFont font = null);
        #endregion

        #region SIMPLE BUTTON
        /// <summary>
        /// Creates a new simple button.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        ISimpleButton newSimpleButton(string text = null, IFont font = null, IBlock image = null);
        #endregion
#endif
        #endregion

    }
#endif
    #endregion

    #region WINDOW FACTORY
#if (GWS && Window)
    public interface IWindowFactory : IFactory
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
        IWindow newWindow(IWindowControl externalWindow);
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
        bool SaveAsBitmap(ICopyable image, string file);

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
        ITexture newTexture(IHost window, int? w = null, int? h = null, bool isPrimary = false, TextureAccess? textureAccess = null);

        /// <summary>
        /// Crates a new texture from a given window. Then copies dat from given buffer.
        /// </summary>
        /// <param name="window">Window from which texture is to be created</param>
        /// <param name="source">Buffer source to copy data from onto surface</param>
        /// <param name="isPrimary">Define if its a primary one for the window</param>
        /// <param name="textureAccess">Defines the way texture can be accessed. Default is streaming.</param>
        /// <returns></returns>
        ITexture newTexture(IHost window, ICopyable source, bool isPrimary = false, TextureAccess? textureAccess = null);
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
