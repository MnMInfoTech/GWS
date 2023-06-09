/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
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

        #region SERIALIZER
        ISerializer newSerializer();
        #endregion

        #region CONVERTER
        IConverter newConverter();
        #endregion

        #region GENRE
        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        IGenre newGenre();
        #endregion

        #region TO PEN
        /// <summary>
        /// Creates a instance of Pen or Brush from the given context.
        /// </summary>
        /// <param name="context">Context to convert to IPen instance.</param>
        /// <param name="w">Width of IPen instance. Will be default if not supplied.</param>
        /// <param name="h">Height of IPen instance. Will be default if not supplied.</param>
        /// <param name="h">Returns .</param>
        /// <returns>IReadable</returns>
        IPen ToPen(IPenContext context, int? w = null, int? h = null, ResizeCommand resizeMode = 0);
        #endregion

        #region TIMER
        ITimer newTimer(int interval = 5, TimeUnit unit = 0);
        #endregion

        #region COUNTDOWN
        ICountDown newCountDown(int threshold, TimeUnit unit = TimeUnit.MilliSecond);
        #endregion

        #region BOUNDARY
        IBoundary newBoundary(BoundaryKind kind = BoundaryKind.Boundary);
        IBoundary newBoundary(int x, int y, int w, int h, BoundaryKind kind = BoundaryKind.Boundary);
        IBoundary newBoundary(IBounds parameter, BoundaryKind kind = BoundaryKind.Boundary);
        IBoundary newBoundary(ObjType type, BoundaryKind kind = BoundaryKind.Boundary);
        IBoundary newBoundary(ObjType type, int[] boundaryData, BoundaryKind kind = BoundaryKind.Boundary);
        IBoundary newBoundary(int[] boundaryData, BoundaryKind kind = BoundaryKind.Boundary);
        #endregion

        #region LINE SCANNER
        /// <summary>
        /// Create new instance of object implementing ILineScanner interface for line scanning purposes.
        /// </summary>
        /// <returns>Instance of ILineScanner object.</returns>
        ILineScanner newLineScanner();
        #endregion

        #region DEAL WITH UNKNOWN IMAGESOURCE
        /// <summary>
        /// While rendering an image source, if it can not be converted to gws processable data,
        /// it will be passed here for parsing so that it can be prcoessed.
        /// </summary>
        /// <param name="source">Unknown Image-source to deal with.</param>
        /// <param name="result">Parsed Result in pointer block.</param>
        /// <param name="resultWidth">Width of the parsed result.</param>
        /// <param name="resultHeight">Height of the parsed result.</param>
        void DealWithUnknownImageSource
            (IImageContext source, ref IntPtr result, ref int resultWidth, ref int resultHeight);
        #endregion

        #region HANDLE DRAWING OF UNKNOWN OBJECT
        /// <summary>
        /// Provides a user defined mechanism to draw custom shapes which are not defined 
        /// at GWS native level. GWS recognizes shapes such as curve, line etc.
        /// It automatically draws objects inheriting IImageSource interface directly or indirectly.
        /// It also automatically draws shapes inheriting IPolygonalF interface directly or indirectly.
        /// For anything else provide your version of drawing.
        /// </summary>
        /// <param name="unknown">Unknown object intended to be rendered on a given renderer surface.</param>
        /// <param name="renderer">Buffer to draw this object to.</param>
        /// <param name="parameters">Collection of parameters to influence or control this drawing process.</param>
        /// <returns>True if operation is handled within this method otherwise returns false.</returns>
        bool HandleRenderingOfUnknownObject(IObject unknown, IRenderer renderer, IEnumerable<IParameter> parameters);
        #endregion
    }
    #endregion

    public abstract partial class Factory : IFactory
    {
        static internal IFactory factory;
        protected static readonly bool initialized;
        protected static volatile bool ApplicationRunning = false;
        public readonly static IEventArgs DefaultArgs = new EventArgs();

        #region CONSTRUCTOR
        static Factory()
        {
            InitializeWindowingSystem(ref initialized);
        }
        protected Factory() { }
        static partial void InitializeWindowingSystem(ref bool initialized);
        #endregion

        #region ATTACH
        public static void Attach<T>() where T : class, IFactory, new()
        {
            factory?.Dispose();
            factory = null;
            factory = new T();
            Operations.Converters["GWS"] = factory.newConverter();
            ImageProcessor = factory.newImageProcessor();

            //this will initialize AngleHelper
            Angles.Initialize();

            //this will initialize PointHelper
            Vectors.Initialize();

            PsuedoConstructorForGWSStandard();
            PsuedoConstrctorForGWSAdvanced();
        }
        static partial void PsuedoConstructorForGWSStandard();
        static partial void PsuedoConstrctorForGWSAdvanced();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Returns true if currently underlying instance is null.
        /// </summary>
        public static bool IsDisposed => factory == null;

        /// <summary>
        /// Returns the default image processor available in GWS for reading and writing image files and memory buffers.
        /// the GWS uses STBIMage internally. For more info on STBImage visit: https://github.com/nothings/stb
        /// </summary>
        public static IImageProcessor ImageProcessor { get; private set; }
        #endregion

        #region IMAGE PROCESSOR
        IImageProcessor IFactory.newImageProcessor()
        {
            IImageProcessor imageProcessor = null;
            newImageProcessor(ref imageProcessor);
            return imageProcessor;
        }

        protected virtual void newImageProcessor(ref IImageProcessor imageProcessor) =>
           imageProcessor = STBImageProcessor.Instance;

        /// <summary>
        /// Creates a new image processor. By default, GWS uses STBImage. For more info on STBImage visit: https://github.com/nothings/stb
        /// </summary>
        /// <returns>IImageProcessor</returns>
        public static IImageProcessor newImageProcessor()
        {
            return factory.newImageProcessor();
        }
        #endregion

        #region NEW SERIALIZER
        public static ISerializer newSerializer()
        {
            return factory.newSerializer();
        }
        ISerializer IFactory.newSerializer() =>
            new Serializer();
        #endregion

        #region CONVERTER
        public static IConverter newConverter() =>
            factory.newConverter();

        IConverter IFactory.newConverter()
        {
            IConverter converter = null;
            newConverter(ref converter);
            return converter;
        }
        protected virtual void newConverter(ref IConverter converter) =>
            converter = new Converter();
        #endregion

        #region GENRE
        IGenre IFactory.newGenre() =>
            new Genre();

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        public static IGenre newGenre() =>
            factory.newGenre();

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="getbasetypes">if set to <c>true</c> [getbasetypes].</param>
        /// <param name="option">The option.</param>
        /// <param name="interfaces">The interfaces.</param>
        public static IGenre newGenre(Type t, bool getbasetypes, ExtractInterfaces option, params Type[] interfaces)
        {
            var Genre = newGenre();
            Genre.Initialize(t);
            Genre.Interfaces = new GenreCollection(t.GetInterfaces(option, interfaces));

            if (getbasetypes)
                Genre.BaseTypes = new GenreCollection(t.GetBaseTypes());
            return Genre;

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="getbasetypes">if set to <c>true</c> [getbasetypes].</param>
        /// <param name="getinterfaces">if set to <c>true</c> [getinterfaces].</param>
        public static IGenre newGenre(Type t, bool getbasetypes, bool getinterfaces)
        {
            var Genre = newGenre();
            Genre.Initialize(t);
            if (getinterfaces)
            {
                Genre.Interfaces = new GenreCollection(t.GetInterfaces());
            }
            if (getbasetypes)
            {
                Genre.BaseTypes = new GenreCollection(t.GetBaseTypes());
            }
            return Genre;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="getbasetypes">if set to <c>true</c> [getbasetypes].</param>
        public static IGenre newGenre(Type t, bool getbasetypes)
        {
            var Genre = newGenre();
            Genre.Initialize(t);
            if (getbasetypes)
                Genre.BaseTypes = new GenreCollection(t.GetBaseTypes());
            return Genre;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        public static IGenre newGenre(Type t)
        {
            var Genre = newGenre();
            Genre.Initialize(t);
            return Genre;
        }

        public static IGenre newGenre<T>()
        {
            return newGenre(typeof(T), true, true);
        }
        public static IGenre newGenre<T>(T source)
        {
            if (source is Type)
                return newGenre(source as Type, true, true);
            else if (source != null)
            {
                if (source is IEnumerable)
                    return newGenre(source.GetType(), true, true);
                else
                    return newGenre(source.GetType(), true, true);
            }
            else
                return newGenre(typeof(T), true, true);
        }
        public static IGenre newGenre<T>(T source, bool getbasetypes, bool getinterfaces)
        {
            if (source is Type)
                return newGenre(source as Type, getbasetypes, getinterfaces);
            else if (source != null)
            {
                if (source is IEnumerable)
                    return newGenre(source.GetType(), getbasetypes, getinterfaces);
                else
                    return newGenre(source.GetType(), getbasetypes, getinterfaces);
            }
            else
                return newGenre(typeof(T), getbasetypes, getinterfaces);
        }
        #endregion

        #region TO PEN
        IPen IFactory.ToPen(IPenContext context, int? w, int? h, ResizeCommand resizeMode)
        {
            IPen pen = null;
            if (context is IPen)
            {
                pen = (IPen)context;
                if ((resizeMode & ResizeCommand.NewInstance) == ResizeCommand.NewInstance &&
                    pen is ICloneable)
                    pen = (IPen)((ICloneable)pen).Clone();
                if ((resizeMode & ResizeCommand.SizeOnlyToFit) != ResizeCommand.SizeOnlyToFit &&
                    pen is IExResizable && (w != null || h != null))
                {
                    var Pen = (IExResizable)pen;
                    Pen.Resize(w ?? Pen.Width, h ?? Pen.Height, out _, resizeMode);
                }
            }
            else if (context is IColour)
                pen = new Rgba(((IColour)context).Colour);
            else
                newPen(ref pen, context, w, h, resizeMode);

            if (pen == null)
                pen = Rgba.Empty;
            return pen;
        }
        protected virtual void newPen(ref IPen pen, IPenContext context, int? w = null, int? h = null, ResizeCommand resizeMode = 0) =>
            ToPen(ref pen, context, w, h, resizeMode);
        partial void ToPen(ref IPen pen, IPenContext context, int? w, int? h, ResizeCommand resizeMode);
        #endregion

        #region TIMER
        public static ITimer newTimer(int interval) =>
            factory.newTimer(interval, 0);
        public static ITimer newTimer(int interval, TimeUnit unit) =>
            factory.newTimer(interval, unit);
        ITimer IFactory.newTimer(int interval, TimeUnit unit) =>
           new Timer(interval, unit);
        #endregion

        #region COUNTDOWN
        public static ICountDown newCountDown(int threshold, TimeUnit unit = TimeUnit.MilliSecond) =>
           factory.newCountDown(threshold, unit);
        public static ICountDown newCountDown(int threshold) =>
                factory.newCountDown(threshold, TimeUnit.MilliSecond);
        public static ICountDown newCountDown() =>
            factory.newCountDown(1000, TimeUnit.MilliSecond);
        public static ICountDown newCountDown(TimeUnit unit) =>
            factory.newCountDown(1000, unit);
        ICountDown IFactory.newCountDown(int threshold, TimeUnit unit) =>
            new CountDown(threshold, unit);
        #endregion

        #region BOUNDARY
        IBoundary IFactory.newBoundary(BoundaryKind kind) =>
            new Boundary(kind);
        IBoundary IFactory.newBoundary(int x, int y, int w, int h, BoundaryKind kind) =>
            new Boundary(x, y, w, h, kind);
        IBoundary IFactory.newBoundary(IBounds parameter, BoundaryKind kind) =>
            new Boundary(parameter, kind);
        IBoundary IFactory.newBoundary(ObjType type, BoundaryKind kind) =>
            new Boundary(type, kind);
        IBoundary IFactory.newBoundary(ObjType type, int[] boundaryData, BoundaryKind kind) =>
            new Boundary(type, boundaryData, kind);
        IBoundary IFactory.newBoundary(int[] boundaryData, BoundaryKind kind) =>
            new Boundary(boundaryData, kind);

        public static IBoundary newBoundary(BoundaryKind kind = BoundaryKind.Boundary) =>
           factory.newBoundary(kind);
        public static IBoundary newBoundary(int x, int y, int w, int h, BoundaryKind kind = BoundaryKind.Boundary) =>
            factory.newBoundary(x, y, w, h, kind);
        public static IBoundary newBoundary(IBounds parameter, BoundaryKind kind = BoundaryKind.Boundary) =>
            factory.newBoundary(parameter, kind);
        public static IBoundary newBoundary(ObjType type, BoundaryKind kind = BoundaryKind.Boundary) =>
            factory.newBoundary(type, kind);

        public static IBoundary newBoundary(ObjType type, int[] boundaryData, BoundaryKind kind) =>
            new Boundary(type, boundaryData, kind);
        public static IBoundary newBoundary(int[] boundaryData, BoundaryKind kind) =>
            new Boundary(boundaryData, kind);
        #endregion

        #region LINE SCANNER
        /// <summary>
        /// Create new instance of object implementing ILineScanner interface for line scanning purposes.
        /// </summary>
        /// <returns>Instance of ILineScanner object.</returns>
        public static ILineScanner newLineScanner() =>
            factory.newLineScanner();

        ILineScanner IFactory.newLineScanner() =>
            new LineScanner();
        #endregion

        #region DEAL WITH UNKNOWN IMAGESOURCE
        /// <summary>
        /// While rendering an image source, if it can not be converted to gws processable data,
        /// it will be passed here for parsing so that it can be prcoessed.
        /// </summary>
        /// <param name="source">Unknown Image-source to deal with.</param>
        /// <param name="result">Parsed Result in pointer block.</param>
        /// <param name="resultWidth">Width of the parsed result.</param>
        /// <param name="resultHeight">Height of the parsed result.</param>
        protected virtual void HandleUnknownImageSource
            (IImageContext source, ref IntPtr result, ref int resultWidth, ref int resultHeight)
        { }
        void IFactory.DealWithUnknownImageSource
            (IImageContext source, ref IntPtr result, ref int resultWidth, ref int resultHeight) =>
            HandleUnknownImageSource(source, ref result, ref resultWidth, ref resultHeight);
        #endregion

        #region DRAW UNKNOWN OBJECT
        bool IFactory.HandleRenderingOfUnknownObject(IObject unknown, IRenderer renderer, IEnumerable<IParameter> parameters) =>
            HandleRenderingOfUnknownObject(unknown, renderer, parameters);

        /// <summary>
        /// Provides a user defined mechanism to draw custom shapes which are not defined 
        /// at GWS native level. GWS recognizes shapes such as curve, line etc.
        /// It automatically draws objects inheriting IImageSource interface directly or indirectly.
        /// It also automatically draws shapes inheriting IPolygonalF interface directly or indirectly.
        /// For anything else provide your version of drawing.
        /// </summary>
        /// <param name="object"></param>
        /// <param name="info"></param>
        protected virtual bool HandleRenderingOfUnknownObject(IObject unknown, IRenderer renderer, IEnumerable<IParameter> parameters) => true;
        #endregion

        #region MISC
        IAnimatedGifFrame IFactory.newAnimatedGifFrame(byte[] data, int delay)
        {
            IAnimatedGifFrame animatedGifFrame = null;
            newAnimatedGifFrame(ref animatedGifFrame, data, delay);
            return animatedGifFrame;
        }
        protected virtual void newAnimatedGifFrame(ref IAnimatedGifFrame animatedGifFrame, byte[] data, int delay)
        {
            animatedGifFrame = STBImageProcessor.GetAnimatedGifFrame(data, delay);
        }
        public static IAnimatedGifFrame newAnimatedGifFrame(byte[] data, int delay) =>
            factory.newAnimatedGifFrame(data, delay);
        #endregion

        #region DISPOSE
        void IDisposable.Dispose()
        {
            ImageProcessor.Dispose();
            Operations.Converters.Remove("GWS");
            Dispose2();
        }
        partial void Dispose2();
        #endregion
    }
}
