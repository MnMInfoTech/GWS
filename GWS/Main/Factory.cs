/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if GWS || Window

using System;
using System.Collections.Generic;
using System.IO;

namespace MnM.GWS
{
    #region FACTORY
    partial interface IFactory
    {
        #region DESKTOP
        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="x">X coordinate of location of the window</param>
        /// <param name="y">Y coordinate of location of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <returns></returns>
        IDesktop newDesktop(string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null,
            RendererFlags? renderFlags = null);
        #endregion

        #region OSWINDOW
        /// <summary>
        /// Returns new OS Window.
        /// </summary>
        /// <returns></returns>
        IOSWindow newOSWindow();
        #endregion

        #region CANVAS
        /// <summary>
        /// Creates a new Surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <returns>Instance of ISurface</returns>
        ICanvas newCanvas(int width, int height);

        /// <summary>
        /// Creates a new Surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">Pointer containing data to use. Please note that the array will be converted to int[] first.
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="switchRBChannel">If true, Red and Blue channels will be swapped with eachother.</param>
        /// <returns>Instance of ISurface</returns>
        ICanvas newCanvas(IntPtr pixels, int width, int height, bool switchRBChannel = false);

        /// <summary>
        /// Creates a new Surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">pixel array containing colour data/// </param>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>Instance of ISurface</returns>
        ICanvas newCanvas(int[] pixels, int width, int height, bool makeCopy = false);

        /// <summary>
        /// Creates a new Surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="pixels">pixel array containing colour data/// </param>
        /// <param name="switchRBChannel">If true, Red and Blue channels will be swapped with eachother.</param>
        /// <returns>Instance of ISurface</returns>
        ICanvas newCanvas(int width, int height, byte[] pixels, bool switchRBChannel = false);
        #endregion

        #region VIEW
        /// <summary>
        /// Creates a new View object attached with given window with or without multiple virtual windows.
        /// </summary>
        /// <param name="window">Window which this object belongs to.</param>
        /// <param name="isMultiWindow">If true the view will allow multiple windows to be created.</param>
        /// <returns></returns>
        IView newView(IRenderTarget window, bool isMultiWindow = false);
        #endregion

        #region BRUSH
        /// <summary>
        /// Creates a new brush of certain width and height using specified fill style.
        /// </summary>
        /// <param name="style">Fill style to be used to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        IGradientBrush newBrush(IBrushStyle style, int width, int height);

        /// <summary>
        /// Creates a texture brush from image data source.
        /// </summary>
        /// <param name="data">Image data source</param>
        /// <param name="width">Width of source.</param>
        /// <param name="height">Height of source.</param>
        /// <returns></returns>
        ITextureBrush newBrush(IntPtr data, int width, int height);

        /// <summary>
        /// Creates a texture brush from image data source.
        /// </summary>
        /// <param name="data">Image data source</param>
        /// <param name="width">Width of source.</param>
        /// <param name="height">Height of source.</param>
        /// <returns></returns>
        ITextureBrush newBrush(byte[] data, int width, int height);

        /// <summary>
        /// Creates a texture brush from image data source.
        /// </summary>
        /// <param name="data">Image data source</param>
        /// <param name="width">Width of source.</param>
        /// <param name="height">Height of source.</param>
        /// <returns></returns>
        ITextureBrush newBrush(int[] data, int width, int height);
        #endregion

        #region POLY FILL
        /// <summary>
        /// Create new instance of object implementing IPolyFill interface for rendering purposes.
        /// </summary>
        /// <param name="renderAction">RenderAction delegate to render scan lines.</param>
        /// <returns>Instance of IPolyFill object.</returns>
        IPolygonFiller newPolyFill(RenderAction renderAction);
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

        #region CONTROL
        /// <summary>
        /// Creates a widget which hosts a primitivbe shape such as ellipse, triangle etc.
        /// </summary>
        /// <param name="child">Child object to be hosted.</param>
        /// <returns>Instance of object implementing IWidget interface</returns>
        IControl newControl(IShape child);

        /// <summary>
        /// Creates a widget which hosts a primitivbe shape such as ellipse, triangle etc.
        /// </summary>
        /// <param name="child">Child object to be hosted.</param>
        /// <param name="parameters">Various setting parameters to influence drawing operation.</param>
        /// <returns>Instance of object implementing IWidget interface</returns>
        IControl newControl(IShape child, params IParameter[] parameters);

        /// <summary>
        /// Creates a widget which hosts a primitivbe shape such as ellipse, triangle etc.
        /// </summary>
        /// <param name="parameters">Various setting parameters to influence drawing operation.</param>
        /// <param name="child">Child object to be hosted.</param>
        /// <returns></returns>
        IControl newControl(IShape child, IEnumerable<IParameter> parameters);
        #endregion

        #region SHAPE PARSER
        /// <summary>
        /// Creates new shape parser for preparing settings for a given shape rendering.
        /// </summary>
        /// <returns></returns>
        IShapeParser newShapeParser();
        #endregion

        #region SHAPE RENDERER
        IGlyphRenderer newGlyphRenderer();
        #endregion

        #region IMAGE LIST
        /// <summary>
        /// Gets an instance of IImageList.
        /// </summary>
        /// <returns>IImageList</returns>
        IImageList newImageList();

        /// <summary>
        /// Gets an instance of IImageList.
        /// </summary>
        /// <param name="capacity">Initial capacity of the list.</param>
        /// <returns>IImageList</returns>
        IImageList newImageList(int capacity);
        #endregion
    }
    #endregion

    partial class Factory: IFactory
    {
        #region INSTANCE VARIABLE
        static string sysFontpath;
        #endregion

        #region PSUEDO CONSTRUCTOR
        static partial void PsuedoConstructorForGWSStandard()
        {
            ShapeParser = factory.newShapeParser();
            sysFontpath = AppContext.BaseDirectory + "UbuntuMono-Regular.ttf";
            //sysFontpath = AppContext.BaseDirectory + "LinBiolinum_R_G.ttf";

            using (var fontStream = System.IO.File.OpenRead(sysFontpath))
            {
                SystemFont = factory.newFont(fontStream, 12);
                SystemFontBig = SystemFont.Clone() as IFont;
                SystemFontBig.Size = 24;
            }
            //this will initialize CurveHlepr
            Curves.Initialize();
            //this will initialize ColourHelper.
            Colours.Initialize();
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Returns a default system font available in GWS.
        /// The font is: UbuntuMono-Regular and is covered under UBUNTU FONT LICENCE Version 1.0.
        /// </summary>
        public static IFont SystemFont { get; private set; }

        /// <summary>
        /// Returns a default system font available in GWS.
        /// The font is: UbuntuMono-Regular and is covered under UBUNTU FONT LICENCE Version 1.0.
        /// </summary>
        public static IFont SystemFontBig { get; private set; }

        /// <summary>
        /// Returns an instance of shape parser coming from attached factory.
        /// </summary>
        public static IShapeParser ShapeParser { get; private set; }
        #endregion

        #region DESKTOP WINDOW
        IDesktop IFactory.newDesktop(string title, int? width, int? height,
            int? x, int? y, GwsWindowFlags? flags, RendererFlags? renderFlags)
        {
            IDesktop desktop = null;
            newDesktop(ref desktop, title, width, height, x, y, flags, renderFlags);
            return desktop;
        }
        protected virtual void newDesktop(ref IDesktop desktop, string title, int? width, int? height,
            int? x, int? y, GwsWindowFlags? flags, RendererFlags? renderFlags) =>
          desktop =  new Desktop(title, width, height, x, y, flags, renderFlags);

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="x">X coordinate of location of the window</param>
        /// <param name="y">Y coordinate of location of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <returns></returns>
        public static IDesktop newDesktop(
            string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null,
            GwsWindowFlags? flags = null,
            RendererFlags? renderFlags = null)
        {
            return factory.newDesktop(title, width, height, x, y, flags, renderFlags);
        }

        ///// <summary>
        ///// Create a new window from an existing window by refercing the exisitng window's handle.
        ///// </summary>
        ///// <param name="externalWindow">External window</param>
        ///// <returns></returns>
        //public static IDesktop newDesktop(IExternalTarget externalWindow)
        //{
        //    return factory.newDesktop(externalWindow);
        //}

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="x">X coordinate of location of the window</param>
        /// <param name="y">Y coordinate of location of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <returns>IWindow</returns>
        public static IDesktop newDesktop(string title, int width, int height,
            int x, int y, GwsWindowFlags flags, RendererFlags renderFlags)
        {
            return factory.newDesktop(title, width, height, x, y, flags, renderFlags);
        }

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="x">X coordinate of location of the window</param>
        /// <param name="y">Y coordinate of location of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <returns>IWindow</returns>
        public static IDesktop newDesktop(int width, int height,
            int x, int y, GwsWindowFlags flags, RendererFlags renderFlags)
        {
            return factory.newDesktop(null, width, height, x, y, flags, renderFlags);
        }

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="x">X coordinate of location of the window</param>
        /// <param name="y">Y coordinate of location of the window</param>
        /// <returns>IWindow</returns>
        public static IDesktop newDesktop(string title, int width, int height,
            int x, int y)
        {
            return factory.newDesktop(title, width, height, x, y, null, null);
        }

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <param name="renderFlags">Define flags to create renderer whenever requested.</param>
        /// <returns>IWindow</returns>
        public static IDesktop newDesktop(string title, int width, int height,
            GwsWindowFlags flags, RendererFlags renderFlags)
        {
            return factory.newDesktop(title, width, height, null, null, flags, renderFlags);
        }

        /// <summary>
        /// Creates a new window with specified parameters.
        /// </summary>
        /// <param name="title">Title of the window</param>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="flags">GwsWindowFlags to create a certain kind of window i.e fullscrren, resizeable etc.</param>
        /// <returns>IWindow</returns>
        public static IDesktop newDesktop(string title, int width, int height,
            GwsWindowFlags flags)
        {
            return factory.newDesktop(title, width, height, null, null, flags, null);
        }
        #endregion

        #region OSWINDOW
        /// <summary>
        /// Returns new OS Window.
        /// </summary>
        /// <returns></returns>
        IOSWindow IFactory.newOSWindow()
        {
            IOSWindow oSWindow = null;
            newOSWindow(ref oSWindow);
            return oSWindow;
        }
        protected virtual void newOSWindow(ref IOSWindow oSWindow)
        {
#if MS
            oSWindow = new Factory.MSWindow();
            return;
#elif Window && SDL
            oSWindow = new Factory.SdlWindow();
            return;
#endif
        }

        /// <summary>
        /// Returns new OS Window.
        /// </summary>
        /// <returns></returns>
        internal static IOSWindow newOSWindow() =>
            factory.newOSWindow();
        #endregion

        #region CANVAS
        protected abstract void newCanvas(ref ICanvas canvas, int width, int height);
        protected abstract void newCanvas(ref ICanvas canvas, IntPtr pixels, int width, int height, bool switchRBChannel = false);
        protected abstract void newCanvas(ref ICanvas canvas, int[] pixels, int width, int height, bool makeCopy = false);
        protected abstract void newCanvas(ref ICanvas canvas, int width, int height, byte[] pixels, bool switchRBChannel = false);

        ICanvas IFactory.newCanvas(int width, int height)
        {
            ICanvas canvas = null;
            newCanvas(ref canvas, width, height);
            return canvas;
        }
        ICanvas IFactory.newCanvas(IntPtr pixels, int width, int height, bool switchRBChannel)
        {
            ICanvas canvas = null;
            newCanvas(ref canvas, pixels, width, height, switchRBChannel);
            return canvas;
        }
        ICanvas IFactory.newCanvas(int[] pixels, int width, int height, bool makeCopy)
        {
            ICanvas canvas = null;
            newCanvas(ref canvas, pixels, width, height, makeCopy);
            return canvas;
        }
        ICanvas IFactory.newCanvas(int width, int height, byte[] pixels, bool switchRBChannel)
        {
            ICanvas canvas = null;
            newCanvas(ref canvas,  width, height, pixels, switchRBChannel);
            return canvas;
        }

        /// <summary>
        /// Creates a new surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// </param>
        /// <returns>Instance of ISurface</returns>
        public static ICanvas newCanvas(int width, int height) =>
            factory.newCanvas(width, height);

        /// <summary>
        /// Creates a new surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">Pointer containing data to use. Please note that the array will be converted to int[] first.
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="switchRBChannel">If true, Red and Blue channels will be swapped with eachother.</param>
        /// <returns>Instance of ISurface</returns>
        public static ICanvas newCanvas(IntPtr pixels, int width, int height, bool switchRBChannel = false) =>
            factory.newCanvas(pixels, width, height, switchRBChannel);

        /// <summary>
        /// Creates a new surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">pixel array containing colour data/// </param>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="makeCopy">If true then copy the pixel array into internal memory buffer
        /// otherwise set an internal menory buffer referring to the pixel array supplied.
        /// </param>
        /// <returns>Instance of ISurface</returns>
        public static ICanvas newCanvas(int[] pixels, int width, int height, bool makeCopy = false) =>
            factory.newCanvas(pixels, width, height, makeCopy);

        /// <summary>
        /// Creates a new surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="width">Required width</param>
        /// <param name="height">Requred height</param>
        /// <param name="pixels">pixel array containing colour data/// </param>
        /// <param name="switchRBChannel">If true, Red and Blue channels will be swapped with eachother.</param>
        /// <returns>Instance of ISurface</returns>
        public static ICanvas newCanvas(int width, int height, byte[] pixels, bool switchRBChannel = false) =>
            factory.newCanvas(width, height, pixels, switchRBChannel);


        /// <summary>
        /// Creates a new surface object of given width and height with pixels provided by buffer.
        /// </summary>
        /// <param name="pixels">pixel array containing colour data/// </param>
        /// <param name="switchRBChannel">If true, Red and Blue channels will be swapped with eachother.</param>
        /// <returns>Instance of ISurface</returns>
        public static ICanvas newCanvas(byte[] pixels, bool switchRBChannel = false)
        {
            ushort width, height;
            var i = 0;
            if
            (
                pixels[i++] != 'G'
                || pixels[i++] != 'W'
                || pixels[i++] != 'S'
            )
            {
                var len = pixels.Length;
                var f = (float)Math.Sqrt(len);
                width = (ushort)f;
                height = (ushort)f;
                if (f - height != 0)
                    ++height;
            }
            else
            {
                byte p1, p2;

                p1 = pixels[i++];
                p2 = pixels[i++];
                width = (ushort)(p1 + (p2 << 8));

                p1 = pixels[i++];
                p2 = pixels[i++];
                height = (ushort)(p1 + (p2 << 8));
            }
            return newCanvas(width, height, pixels, switchRBChannel);
        }
        /// <summary>
        /// Creates a new surface object and fills it with the data received from the disk image file located on a given path.
        /// </summary>
        /// <param name="path">Path of the disk image file to use as a initial source of Graphics object</param>
        /// <param name="switchRBChannel">If true, Red and Blue channels will be swapped with eachother.</param>
        /// <returns>Instance of ISurface</returns>
        public static ICanvas newCanvas(string path, bool switchRBChannel = false)
        {
            var tuple = ImageProcessing.ReadImage(path);
            return newCanvas(tuple.Item2, tuple.Item3, tuple.Item1, switchRBChannel);
        }

        /// <summary>
        /// Creates a new canvas object fitting the size of given shape.
        /// </summary>
        /// <param name="shape"></param>
        /// <returns></returns>
        public static ICanvas newCanvas(ISize size, IEnumerable<IParameter> parameters = null)
        {
            var canvas = (IExCanvas)factory.newCanvas(size.Width, size.Height);
            if (size is IImageSource)
            {
                var action = canvas.CreateRenderAction(parameters);
                action(null, null, (IImageSource)size);
            }
            else if (size is IObject)
                canvas.Render((IObject)size, parameters);
            return canvas;
        }
        #endregion

        #region VIEW
        protected abstract void newView(ref IView view, IRenderTarget window, bool isMultiWindow = false);
        IView IFactory.newView(IRenderTarget window, bool isMultiWindow)
        {
            IView view = null;
            newView(ref view, window, isMultiWindow);
            return view;
        }
        /// <summary>
        /// Creates a new View object attached with given window with or without multiple virtual windows.
        /// </summary>
        /// <param name="window">Window which this object belongs to.</param>
        /// <param name="isMultiWindow">If true the view will allow multiple windows to be created.</param>
        /// <returns></returns>
        public static IView newView(IRenderTarget window, bool isMultiWindow = false)
        {
            return factory.newView(window, isMultiWindow);
        }
        #endregion

        #region BRUSH
        IGradientBrush IFactory.newBrush(IBrushStyle style, int width, int height)
        {
            IGradientBrush gradientBrush = null;
            newBrush(ref gradientBrush, style, width, height);
            return gradientBrush;
        }
        protected virtual void newBrush(ref IGradientBrush gradientBrush,  IBrushStyle style, int width, int height)=>
            gradientBrush = GradientBrush.CreateInstance(style, width, height);

        ITextureBrush IFactory.newBrush(IntPtr data, int width, int height) =>
            TextureBrush.CreateInstance(data, width, height);
        ITextureBrush IFactory.newBrush(int[] data, int width, int height) =>
            TextureBrush.CreateInstance(data, width, height);

        ITextureBrush IFactory.newBrush(byte[] data, int width, int height) =>
            TextureBrush.CreateInstance(data, width, height);

        /// <summary>
        /// Creates a new brush of certain width and height using specified fill style.
        /// </summary>
        /// <param name="style">Fill style to be used to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        public static IGradientBrush newBrush(IBrushStyle style, int width, int height)
        {
            return factory.newBrush(style, width, height);
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
            return factory.newBrush(data, width, height);
        }

        /// <summary>
        /// Creates a new brush of certain width and height using specified int source.
        /// </summary>
        /// <param name="source">Memory block to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        public static ITextureBrush newBrush(int[] data, int width, int height)
        {
            return factory.newBrush(data, width, height);
        }

        /// <summary>
        /// Creates a new texture brush of certain width and height using specified fill style.
        /// </summary>
        /// <param name="style">Fill style to be used to fill the brush</param>
        /// <param name="width">Expected width of the brush</param>
        /// <param name="height">Expected height of the brush</param>
        /// <returns></returns>
        public static unsafe ITextureBrush newBrush(string file)
        {
            Tuple<byte[], int, int> lot;
            using (var stream = File.Open(file, FileMode.Open))
            {
                lot = ImageProcessor.Read(stream).Result;
            }
            IntPtr data;
            fixed (byte* p = lot.Item1)
                data = (IntPtr)p;
            var textureBrush = factory.newBrush(lot.Item1, lot.Item2, lot.Item3);
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
            var textureBrush = factory.newBrush(data, width, height);
            return textureBrush;
        }
        #endregion

        #region TO PEN
        partial void ToPen(ref IPen pen, IPenContext context, int? w, int? h, ResizeCommand resizeMode)
        {
            if (context is IBrushStyle)
                pen = ((IFactory)this).newBrush((IBrushStyle)context, w ?? 100, h ?? 100);
        }
        #endregion

        #region FONT
        IFont IFactory.newFont(Stream fontStream, int fontSize)
        {
            IFont font = null;
            newFont(ref font, fontStream, fontSize);
            return font;
        }
        protected virtual void newFont(ref IFont font, Stream fontStream, int fontSize) =>
           font = new Font(fontStream, fontSize);

        /// <summary>
        /// Creates a new font with given parameters.
        /// </summary>
        /// <param name="fontStream">A stream containig font data</param>
        /// <param name="fontSize">Size of the font to be used to create any glyph required.</param>
        /// <returns>IFont object</returns>
        public static IFont newFont(Stream fontStream, int fontSize)
        {
            return factory.newFont(fontStream, fontSize);
        }
        public static IFont newFont(string fontFile, int fontSize)
        {
            using (var fontStream = System.IO.File.OpenRead(fontFile))
            {
                return factory.newFont(fontStream, fontSize);
            }
        }
        #endregion

        #region POLY FILL
        IPolygonFiller IFactory.newPolyFill(RenderAction renderAction) =>
            new PolygonFiller(renderAction);

        /// <summary>
        /// Create new instance of object implementing IPolyFill interface for rendering purposes.
        /// </summary>
        /// <param name="renderAction">RenderAction delegate to render scan lines.</param>
        /// <returns>Instance of IPolyFill object.</returns>
        public static IPolygonFiller newPolyFill(RenderAction renderAction) =>
            factory.newPolyFill(renderAction);
        #endregion

        #region CONTROL
        IControl IFactory.newControl(IShape shape) =>
            new Control(shape);
        IControl IFactory.newControl(IShape shape, params IParameter[] parameters) =>
            new Control(shape, parameters);
        IControl IFactory.newControl(IShape shape, IEnumerable<IParameter> parameters) =>
            new Control(shape, parameters);
        public static IControl newControl(IShape shape) =>
            factory.newControl(shape);
        public static IControl newControl(IShape shape, params IParameter[] parameters) =>
            factory.newControl(shape, parameters);
        public static IControl newControl(IShape shape, IEnumerable<IParameter> parameters) =>
            factory.newControl(shape, parameters);
        #endregion

        #region SHAPE PARSER
        IShapeParser IFactory.newShapeParser() =>
            new ShapeParser();
        protected virtual void newShapeParser(ref IShapeParser shapeParser) =>
            shapeParser = new ShapeParser();
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

        #region GLYPH-RENDERER
        IGlyphRenderer IFactory.newGlyphRenderer()
        {
            IGlyphRenderer glyphRenderer = null;
            newGlyphRenderer(ref glyphRenderer);
            return glyphRenderer;
        }
        protected virtual void newGlyphRenderer(ref IGlyphRenderer glyphRenderer) =>
           glyphRenderer = new GlyphRenderer();

        public static IGlyphRenderer newGlyphRenderer() =>
            factory.newGlyphRenderer();
        #endregion

        #region IMAGE LIST
        IImageList IFactory.newImageList() =>
            new ImageList();
        IImageList IFactory.newImageList(int capacity) =>
            new ImageList(capacity);

        /// <summary>
        /// Gets an instance of IImageList.
        /// </summary>
        /// <returns>IImageList</returns>
        public static IImageList newImageList() =>
            factory.newImageList();

        /// <summary>
        /// Gets an instance of IImageList.
        /// </summary>
        /// <param name="capacity">Initial capacity of the list.</param>
        /// <returns>IImageList</returns>
        public static IImageList newImageList(int capacity) =>
            factory.newImageList(capacity);
        #endregion

        partial void Dispose2()
        {
            SystemFont = null;
        }   
    }
}
#endif

