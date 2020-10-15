/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.IO;

namespace MnM.GWS
{
    public static partial class Factory
    {
        #region INSTANCE VARIABLE
        internal static bool IsClosing;
        readonly static string sysFontpath;

#if (Window)
        static IWindowFactory Instance;
#else
       static  IFactory Instance;
#endif
        public static IEventArgs EmptyArgs = new _EventArgs();
        #endregion

        #region CONSTRUCTOR
        static Factory()
        {
            sysFontpath = AppContext.BaseDirectory + "UbuntuMono-Regular.ttf";
        }
        #endregion

        #region ATTACH
#if (GWS || Window)
        internal static void Attach(IFactory factory)
        {
            if (factory == null)
                return;
            Instance?.Dispose();
            Instance = null;
#if Window
            Instance = factory as IWindowFactory;
#else
            Instance = factory;
#endif
            Operations.Converters["GWS"] = factory.newConverter();
            Pens.Attach(Instance);
            FontRenderer = Instance.newGlyphRenderer();
            ImageProcessor = Instance.newImageProcessor();
            ShapeParser = Instance.newShapeParser();
            using (var fontStream = System.IO.File.OpenRead(sysFontpath))
            {
                SystemFont = Instance.newFont(fontStream, 12);
            }
            //this will initialize AngleHelper
            Angles.Initialize();

            //this will initialize CurveHlepr
            Curves.Initialize();

            //this will initialize PointHelper
            Vectors.Initialize();

            //this will initialize ColorHelper.
            Colors.Initialize();
        }
#endif
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets the default font renderer provide by the GWS.
        /// </summary>
        public static IGlyphRenderer FontRenderer { get; private set; }

        /// <summary>
        /// Returns a default system font available in GWS.
        /// The font is: UbuntuMono-Regular and is covered under UBUNTU FONT LICENCE Version 1.0.
        /// </summary>
        public static IFont SystemFont { get; private set; }

        /// <summary>
        /// Returns true if currently underlying instance is null.
        /// </summary>
        public static bool IsDisposed => Instance == null;

        /// <summary>
        /// Returns the default image processor available in GWS for reading and writing image files and memory buffers.
        /// the GWS uses STBIMage internally. For more info on STBImage visit: https://github.com/nothings/stb
        /// </summary>
        public static IImageProcessor ImageProcessor { get; private set; }

        /// <summary>
        /// Returns an instance of shape parser coming from attached factory.
        /// </summary>
        public static IShapeParser ShapeParser { get; private set; }
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
        public static Rgba newColor(byte r, byte g, byte b, byte a = 255)
        {
            return Instance.newColor(r, g, b, a);
        }
        #endregion

        #region SURFACE
        /// <summary>
        /// Creates a new GWS Buffer object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <returns>IBuffer</returns>
        public static ISurface newSurface(int width, int height)
        {
            return Instance.newSurface(width, height);
        }

        /// <summary>
        /// Creates a new GWS Buffer object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">Pointer containing data to use. Please note that the array will be converted to int[] first.
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <param name="makeCopy">If true then copy the buffer array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel pointer supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        public static ISurface newSurface(IntPtr pixels, int width, int height)
        {
            return Instance.newSurface(pixels, width, height);
        }

        /// <summary>
        /// Creates a new GWS Buffer object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        public static ISurface newSurface(int[] pixels, int width, int height, bool makeCopy = false)
        {
            return Instance.newSurface(pixels, width, height, makeCopy);
        }

        /// <summary>
        /// Creates a new GWS Buffer object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        public static ISurface newSurface(int width, int height, byte[] pixels, bool makeCopy = false)
        {
            return Instance.newSurface(width, height, pixels, makeCopy);
        }

        /// <summary>
        /// Creates a new GWS Graphics object and fills it with the data received from the disk image file located on a given path.
        /// </summary>
        /// <param name="path">Path of the disk image file to use as a initial source of Graphics object</param>
        /// <returns>IGraphics</returns>
        public static ISurface newSurface(string path)
        {
            var tuple = Renderer.ReadImage(path);
            return newSurface(tuple.Item2, tuple.Item3, tuple.Item1);
        }

        /// <summary>
        /// Creates a new GWS Graphics object and fills it with the supplied buffer. 
        /// Actually internal memory buffer is set to refer the buffer supplied.
        /// </summary>
        /// <param name="pixels">Byte array to use as internal memory buffer</param>
        /// <returns>Graphics object</returns>
        public static ISurface newSurface(byte[] pixels)
        {
            var tuple = Renderer.ReadImage(pixels);
            return newSurface(tuple.Item2, tuple.Item3, tuple.Item1);
        }
        #endregion

        #region CANVAS
        /// <summary>
        /// Creates a new GWS Buffer object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <returns>IBuffer</returns>
        public static ICanvas newCanvas(int width, int height)
        {
            return Instance.newCanvas(width, height);
        }

        /// <summary>
        /// Creates a new GWS Buffer object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">Pointer containing data to use. Please note that the array will be converted to int[] first.
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <param name="makeCopy">If true then copy the buffer array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel pointer supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        public static ICanvas newCanvas(IntPtr pixels, int width, int height)
        {
            return Instance.newCanvas(pixels, width, height);
        }

        /// <summary>
        /// Creates a new GWS Buffer object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        public static ICanvas newCanvas(int[] pixels, int width, int height, bool makeCopy = false)
        {
            return Instance.newCanvas(pixels, width, height, makeCopy);
        }

        /// <summary>
        /// Creates a new GWS Buffer object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="pixels">pixel array containing color data/// </param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>IBuffer</returns>
        public static ICanvas newCanvas(int width, int height, byte[] pixels, bool makeCopy = false)
        {
            return Instance.newCanvas(width, height, pixels, makeCopy);
        }

        /// <summary>
        /// Creates a new GWS Graphics object attached with given window.
        /// </summary>
        /// <param name="window">Window which this object belongs to.</param>
        /// <returns></returns>
        public static ICanvas newCanvas(IRenderTarget window)
        {
            return Instance.newCanvas(window);
        }

        /// <summary>
        /// Creates a new GWS Graphics object and fills it with the data received from the disk image file located on a given path.
        /// </summary>
        /// <param name="path">Path of the disk image file to use as a initial source of Graphics object</param>
        /// <returns>IGraphics</returns>
        public static ICanvas newCanvas(string path)
        {
            var tuple = Renderer.ReadImage(path);
            return newCanvas(tuple.Item2, tuple.Item3, tuple.Item1);
        }

        /// <summary>
        /// Creates a new GWS Graphics object and fills it with the supplied buffer. 
        /// Actually internal memory buffer is set to refer the buffer supplied.
        /// </summary>
        /// <param name="pixels">Byte array to use as internal memory buffer</param>
        /// <returns>Graphics object</returns>
        public static ICanvas newCanvas(byte[] pixels)
        {
            var tuple = Renderer.ReadImage(pixels);
            return newCanvas(tuple.Item2, tuple.Item3, tuple.Item1);
        }
        #endregion

        #region BRUSH - PEN
        /// <summary>
        /// Creates a new brush of certain width and height using specified fill style.
        /// </summary>
        /// <param name="style">Fill style to be used to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        public static IBrush newBrush(BrushStyle style, int width, int height)
        {
            return Instance.newBrush(style, width, height);
        }

        /// <summary>
        /// Creates a texture brush from image data source.
        /// </summary>
        /// <param name="data">Image data source</param>
        /// <param name="width">Width of source.</param>
        /// <param name="height">Height of source.</param>
        /// <returns></returns>
        public static ITextureBrush newBrush(IntPtr data, int width, int height)
        {
            return Instance.newBrush(data, width, height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static IPen newPen(Rgba rgba)
        {
            return Instance.newPen(rgba.Color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static IPen newPen(IColor rgba)
        {
            return Instance.newPen(rgba.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static IPen newPen(int color)
        {
            return Instance.newPen(color);
        }
        #endregion

        #region OBJECT-COLLECTION
        /// <summary>
        /// Creates colletion that holds renderable controls on a given buffer.
        /// </summary>
        /// <param name="buffer">Parent buffer block.</param>
        /// <returns></returns>
        public static IObjCollection newObjectCollection(ICanvas buffer)
        {
            return Instance.newObjectCollection(buffer);
        }
        #endregion

        #region BUFFER COLLECTION
#if Advanced
        /// <summary>
        /// Creates a collection to hold buffers to enable user to maintain and use multiple buffers with any parent window and graphics.
        /// </summary>
        /// <returns>IBufferCollection</returns>
        public static IBufferCollection newBufferCollection() =>
            Instance.newBufferCollection();

        /// <summary>
        /// Creates a collection to hold buffers to enable user to maintain and use multiple buffers with any parent window and graphics.
        /// </summary>
        /// <param name="primary">Primary buffer for this instance to use.
        /// If no null value is provided then ChangePrimary method will not be able to change the primary buffer value.
        /// As we have already provided dedicated primary buffer here.</param>
        /// <returns>IBufferCollection</returns>
        public static IBufferCollection newBufferCollection(ICanvas primary) =>
            Instance.newBufferCollection(primary);

        /// <summary>
        /// Creates a collection to hold buffers to enable user to maintain and use multiple buffers with any parent window and graphics.
        /// </summary>
        /// <param name="capacity">Initiali capacity of the collection. the default is 4</param>
        /// <returns></returns>
        public static IBufferCollection newBufferCollection(int capacity) =>
            Instance.newBufferCollection(capacity);
#endif
#endregion

        #region POLY FILL
        /// <summary>
        /// Creates new instance of IPolygonFill which can fill a polygon structure with specified PolyFill enum option.
        /// </summary>
        /// <returns>IPolygonFill</returns>
        public static IPolyFill newPolyFill()
        {
            if (Instance == null)
                return null;
            return Instance.newPolyFill();
        }
        #endregion

        #region FONT, GLYPH, GLYPH RENDERER
        /// <summary>
        /// Creates a new Glyph renderer.
        /// </summary>
        /// <returns>IGlyphRenderer</returns>
        public static IGlyphRenderer newGlyphRenderer()
        {
            return Instance.newGlyphRenderer();
        }

        /// <summary>
        /// Creates a new font with given parameters.
        /// </summary>
        /// <param name="fontStream">A stream containig font data</param>
        /// <param name="fontSize">Size of the font to be used to create any glyph required.</param>
        /// <returns>IFont object</returns>
        public static IFont newFont(Stream fontStream, int fontSize)
        {
            return Instance.newFont(fontStream, fontSize);
        }
        #endregion

        #region IMAGE PROCESSOR
        /// <summary>
        /// Creates a new image processor. By default, GWS uses STBImage. For more info on STBImage visit: https://github.com/nothings/stb
        /// </summary>
        /// <returns>IImageProcessor</returns>
        public static IImageProcessor newImageProcessor()
        {
            return Instance.newImageProcessor();
        }

        public static IAnimatedGifFrame newAnimatedGifFrame(byte[] data, int delay)
        {
            return Instance.newAnimatedGifFrame(data, delay);
        }
        #endregion

        class _EventArgs : EventArgs, IEventArgs { }

#if Advanced
        #region SIMPLE POPUP
        public static ISimplePopup newSimplePopup(params string[] items) =>
            Instance.newSimplePopup(items);

        public static ISimplePopup newSimplePopup(int width, int height, params string[] items) =>
            Instance.newSimplePopup(width, height, items);
        #endregion

        #region SIMPLE LABLE
        public static ISimpleLabel newSimpleLabel(string text = null, IFont font = null) =>
            Instance.newSimpleLabel(text, font);
        #endregion
#endif
        public static void Dispose()
        {
            Instance.Dispose();
            FontRenderer?.Dispose();
            Pens.Dispose();
            SystemFont = null;
            ImageProcessor.Dispose();
            Operations.Converters.Remove("GWS");
        }
    }

#if Window
    partial class Factory
    {
        #region PROPERTIES
        /// <summary>
        /// Gets the primary scrren available with system default resoultion in the operating system.
        /// </summary>
        public static IScreen PrimaryScreen => Instance.PrimaryScreen;

        /// <summary>
        /// Gets the default window creatio flags from the operating system.
        /// </summary>
        public static int DefaultWinFlag => Instance.DefaultWinFlag;

        /// <summary>
        /// Gets the flags required to create full screen desktop.
        /// </summary>
        public static int FullScreenWinFlag => Instance.FullScreenWinFlag;

        /// <summary>
        /// Returns all the availble scrrens with possible resoultion provided by operating system.
        /// </summary>
        public static IScreens AvailableScreens => Instance.AvailableScreens;

        /// <summary>
        /// Gets the array of available pixelformats offered by the operating system.
        /// </summary>
        public static uint[] PixelFormats => Instance.PixelFormats;

        /// <summary>
        /// Gets default primary pixel format offered by the operating system.
        /// </summary>
        public static uint PixelFormat => Instance.PixelFormat;

        /// <summary>
        /// Indicates if a connection to by the operating system is established or not.
        /// </summary>
        public static bool Initialized => Instance.Initialized;

        /// <summary>
        /// Gets the type of underlying operating system.
        /// </summary>
        public static OS OS => Instance.OS;

        /// <summary>
        /// Gets the latest error occured while interacting with by the operating system.
        /// </summary>
        public static string LastError => Instance.LastError;
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
        public static IWindow newWindow(string title = null, int? width = null, int? height = null, int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null, RendererFlags? renderFlags = null)
        {
            return Instance.newWindow(title, width, height, x, y, flags, display, renderFlags);
        }

        /// <summary>
        /// Create a new window from an existing window by refercing the exisitng window's handle.
        /// </summary>
        /// <param name="externalWindow">External window</param>
        /// <returns></returns>
        public static IWindow newWindow(IWindowControl externalWindow)
        {
            return Instance.newWindow(externalWindow);
        }
        #endregion

        #region OPENGL CONTEXT
        /// <summary>
        /// Creates OpenGL context for the given window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static IGLContext newGLContext(IWindow window)
        {
            return Instance.newGLContext(window);
        }
        #endregion

        #region GET WINDOW ID
        /// <summary>
        /// Gets the unique window id associated with the window.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static int GetWindowID(IntPtr window)
        {
            return Instance.GetWindowID(window);
        }
        #endregion

        #region SAVE IMAGE AS BITMAP
        /// <summary>
        /// Saves specified buffer as an image file on disk on specified file path.
        /// </summary>
        /// <param name="image">Image which is to be saved</param>
        /// <param name="file">Path of a file where data is to be saved</param>
        /// <returns>Returns true, if operation is succesful otherwise false.</returns>
        public static bool SaveAsBitmap(ICopyable image, string file)
        {
            return Instance.SaveAsBitmap(image, file);
        }

        /// <summary>
        /// Saves specified buffer as an image file on disk on specified file path.
        /// </summary>
        /// <param name="Pixels"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="file"></param>
        /// <returns>Returns true, if operation is succesful otherwise false.</returns>
        public static bool SaveAsBitmap(IntPtr Pixels, int width, int height, string file)
        {
            return Instance.SaveAsBitmap(Pixels, width, height, file);
        }
        #endregion

        #region CURSOR TYPE ENUM CONVERSION
        /// <summary>
        /// Converts GWS cursor types to native operating system's cursor types.
        /// </summary>
        /// <param name="cursorType"></param>
        /// <returns></returns>
        public static int ConvertToSystemCursorID(CursorType cursorType)
        {
            return Instance.ConvertToSystemCursorID(cursorType);
        }
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
        public static ITexture newTexture(IHost window, int? w = null, int? h = null, bool isPrimary = false, TextureAccess? textureAccess = null)
        {
            return Instance.newTexture(window, w, h, isPrimary, textureAccess);
        }

        /// <summary>
        /// Crates a new texture from a given window. Then copies dat from given buffer.
        /// </summary>
        /// <param name="window">Window from which texture is to be created</param>
        /// <param name="source">Buffer source to copy data from onto surface</param>
        /// <param name="isPrimary">Define if its a primary one for the window</param>
        /// <param name="textureAccess">Defines the way texture can be accessed. Default is streaming.</param>
        /// <returns></returns>
        public static ITexture newTexture(IHost window, ICopyable source, bool isPrimary = false, TextureAccess? textureAccess = null)
        {
            return Instance.newTexture(window, source, isPrimary, textureAccess);
        }
        #endregion

        #region SET CURSOR POSITION
        /// <summary>
        /// Sets window's cusor's position to specified x and y coordinates.
        /// </summary>
        /// <param name="x">X coordinate of the location where cursor should be placed</param>
        /// <param name="x">Y coordinate of the location where cursor should be placed</param>
        public static void SetCursorPos(int x, int y)
        {
            Instance.SetCursorPos(x, y);
        }
        #endregion

        #region MISC
        /// <summary>
        /// Disables the existing scrren saver of the operating system.
        /// </summary>
        public static void DisableScreenSaver()
        {
            Instance.DisableScreenSaver();
        }
        #endregion

        #region PUSH, PUMP, POLL EVENTS
        /// <summary>
        /// Push the specified event to the active window.
        /// </summary>
        /// <param name="e">Event to push on</param>
        public static void PushEvent(IEvent e)
        {
            Instance.PushEvent(e);
        }

        /// <summary>
        /// Instructs Window manager to start pumping events to eligble window.
        /// </summary>
        public static void PumpEvents()
        {
            Instance.PumpEvents();
        }

        public static bool PollEvent(out IEvent e)
        {
            e = null;
            if (Instance == null)
                return false;
            return Instance.PollEvent(out e);
        }

        /// <summary>
        /// Gives current event happeed on active window.
        /// </summary>
        /// <param name="e">Event which is just happened</param>
        /// <returns></returns>
        public static ISound newWavPlayer()
        {
            return Instance.newWavPlayer();
        }
        #endregion
    }
#endif

    partial class Factory
    {
        #region BRUSH
        /// <summary>
        /// Creates a new brush of certain width and height using specified fill style.
        /// </summary>
        /// <param name="style">Fill style to be used to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        public static IBrush newBrush(BrushStyle style, float width, float height) =>
            Instance.newBrush(style, width.Ceiling(), height.Ceiling());

        /// <summary>
        /// Creates a new texture brush of certain width and height using specified fill style.
        /// </summary>
        /// <param name="style">Fill style to be used to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        public static unsafe ITextureBrush newBrush(string file)
        {
            var lot = ImageProcessor.Read(file);
            IntPtr data;
            fixed (byte* p = lot.Item1)
                data = (IntPtr)p;
            var textureBrush = Instance.newBrush(data, lot.Item2, lot.Item3);
            return textureBrush;
        }

        /// <summary>
        /// Creates a new brush of certain width and height using specified byte source.
        /// </summary>
        /// <param name="source">Memory block to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        public static unsafe ITextureBrush newBrush(byte[] source, int width, int height)
        {
            IntPtr data;
            fixed (byte* p = source)
                data = (IntPtr)p;
            var textureBrush = Instance.newBrush(data, width, height);
            return textureBrush;
        }

        /// <summary>
        /// Creates a new brush of certain width and height using specified int source.
        /// </summary>
        /// <param name="source">Memory block to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        public static unsafe ITextureBrush newBrush(int[] source, int width, int height)
        {
            IntPtr data;
            fixed (int* p = source)
                data = (IntPtr)p;
            var textureBrush = Instance.newBrush(data, width, height);
            return textureBrush;
        }
        #endregion

        #region VECTOR ARRAY
        /// <summary>
        /// Creates an array of Vectors of length specified.
        /// </summary>
        /// <param name="len">Length of array.</param>
        /// <returns></returns>
        public static Vector[] newPointArray(int len)
        {
            var points = new Vector[len];

            for (int i = 0; i < points.Length; i++)
                points[i] = new Vector();

            return points;
        }

        /// <summary>
        /// Creates an array of Vectors of length specified.
        /// </summary>
        /// <param name="len">Length of array.</param>
        /// <returns></returns>
        public static VectorF[] newPointFArray(int len)
        {
            var points = new VectorF[len];

            for (int i = 0; i < points.Length; i++)
                points[i] = new VectorF();
            return points;
        }
        #endregion

        #region FONT
        public static IFont newFont(string fontFile, int fontSize)
        {
            using (var fontStream = System.IO.File.OpenRead(fontFile))
            {
                return Instance.newFont(fontStream, fontSize);
            }
        }
        #endregion

#if Window
        #region WINDOW
        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="Instance">Instance of IWindowFactory</param>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="x">X coordinate of location of the window</param>
        /// <param name="y">Y coordinate of location of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <param name="display">Scrren with certain resoultion to display a window</param>
        /// <returns>IWindow</returns>
        public static IWindow newWindow(string title, int width, int height,
            int x, int y, GwsWindowFlags flags, RendererFlags renderFlags, IScreen display)
        {
            return Instance.newWindow(title, width, height, x, y, flags, display, renderFlags);
        }

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="Instance">Instance of IWindowFactory</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="x">X coordinate of location of the window</param>
        /// <param name="y">Y coordinate of location of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <param name="display">Scrren with certain resoultion to display a window</param>
        /// <returns>IWindow</returns>
        public static IWindow newWindow(int width, int height,
            int x, int y, GwsWindowFlags flags, RendererFlags renderFlags, IScreen display)
        {
            return Instance.newWindow(null, width, height, x, y, flags, display, renderFlags);
        }

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="Instance">Instance of IWindowFactory</param>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="x">X coordinate of location of the window</param>
        /// <param name="y">Y coordinate of location of the window</param>
        /// <returns>IWindow</returns>
        public static IWindow newWindow(string title, int width, int height,
            int x, int y)
        {
            return Instance.newWindow(title, width, height, x, y, null, null, null);
        }

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="Instance">Instance of IWindowFactory</param>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <param name="display">Scrren with certain resoultion to display a window</param>
        /// <returns>IWindow</returns>
        public static IWindow newWindow(string title, int width, int height,
            GwsWindowFlags flags, RendererFlags renderFlags, IScreen display)
        {
            return Instance.newWindow(title, width, height, null, null, flags, display, renderFlags);
        }

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="Instance">Instance of IWindowFactory</param>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <returns>IWindow</returns>
        public static IWindow newWindow(string title, int width, int height,
            GwsWindowFlags flags, RendererFlags renderFlags)
        {
            return Instance.newWindow(title, width, height, null, null, flags, null, renderFlags);
        }

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="Instance">Instance of IWindowFactory</param>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <returns>IWindow</returns>
        public static IWindow newWindow(string title, int width, int height,
            GwsWindowFlags flags)
        {
            return Instance.newWindow(title, width, height, null, null, flags, null, null);
        }
        #endregion
#endif
    }
}
