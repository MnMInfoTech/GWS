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
        public static void Attach(IFactory factory)
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

        #region FONT
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

        #region LINE
        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        public static ILine newLine(float x1, float y1, float x2, float y2)
        {
            return Instance.newLine(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified start and end points upto value of deviation parameter.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public static ILine newLine(VectorF start, VectorF end, float deviation) =>
            newLine(start.X, start.Y, end.X, end.Y, deviation);

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified line upto value of deviation parameter.
        /// </summary>
        /// <param name="line">Reference line.</param>
        public static ILine newLine(ILine line) =>
            newLine(line.X1, line.Y1, line.X2, line.Y2);

        /// <summary>
        /// Creates a new parallel line segment which is deviated from the specified line upto value of deviation parameter.
        /// </summary>
        /// <param name="line">Reference line.</param>
        /// <param name="deviation">Deviation from the reference line.</param>
        public static ILine newLine(ILine line, float deviation) =>
            newLine(line.X1, line.Y1, line.X2, line.Y2, deviation);

        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public static ILine newLine(float x1, float y1, float x2, float y2,  Rotation angle, float deviation, bool? antiClock = null)
        {
            if (angle)
                angle.Rotate(ref x1, ref y1, ref x2, ref y2, antiClock);

            if (deviation != 0)
                Lines.Parallel(x1, y1, x2, y2, deviation, out x1, out y1, out x2, out y2);

           return newLine(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before creating the line segment</param>
        public static ILine newLine(float x1, float y1, float x2, float y2, Rotation angle, bool? antiClock = null)
        {
            if (angle)
                angle.Rotate(ref x1, ref y1, ref x2, ref y2, antiClock);

            return newLine(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new line segment with points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="deviation">Deviates the line segment to create a parallel one away from the original points specified</param>
        public static ILine newLine(float x1, float y1, float x2, float y2, float deviation)
        {
            if (deviation != 0)
                Lines.Parallel(x1, y1, x2, y2, deviation, out x1, out y1, out x2, out y2);

            return newLine(x1, y1, x2, y2);
        }

        /// <summary>
        /// Creates a new line segment using the specified start and end points.
        /// </summary>
        /// <param name="start">Start point - x1, y1.</param>
        /// <param name="end">End point x2, y2.</param>
        public static ILine newLine(VectorF start, VectorF end) =>
            newLine(start.X, start.Y, end.X, end.Y);
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
        public static ICurve newCurve(float x, float y, float width, float height, 
            float startAngle = 0, float endAngle = 0, CurveType type = 0, Rotation rotation = default(Rotation), VectorF scale = default(VectorF))
        {
            return Instance.newCurve(x, y, width, height, startAngle, endAngle, type, rotation, scale);
        }

        /// <summary>
        /// Creates a curve replicationg data provided by conic parameter and specified array of 3 elements representing pie trianlge.
        /// </summary>
        /// <param name="conic">A conic whose perimeter will be used.</param>
        /// <param name="pie"></param>
        /// <param name="type"></param>
        public static ICurve newCurve(IConic conic, VectorF[] pieTriangle, CurveType type)
        {
            return Instance.newCurve(conic, pieTriangle, type);
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 3 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="first">First point</param>
        /// <param name="second">Second point</param>
        /// <param name="third">Third point</param>
        /// <param name="rotation">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <returns>ICurve</returns>
        public static ICurve newCurve(VectorF first, VectorF second, VectorF third, CurveType type, 
            Rotation rotation = default(Rotation), VectorF scale = default(VectorF))
        {
            var Conic = newConic(first, second, third, type, rotation, scale);
            return newCurve(Conic, Conic.GetPieTriangle(type), type);

        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 4 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Third point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="rotation">Angle to apply rotation while rendering the arc/pie</param>
        public static ICurve newCurve(VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type = CurveType.Full,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) 
        {
            var Conic = newConic(p1, p2, p3, p4, type, rotation, Scale);
            return newCurve(Conic, Conic.GetPieTriangle(type), type);
        }

        /// <summary> 
        /// Creates a ellipse or pie or arc using the specified five points conic section and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="rotation">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        public static ICurve newCurve(VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5, CurveType type = CurveType.Pie,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) 
        {
            var Conic = newConic(p1, p2, p3, p4, p5, rotation, Scale);
            return newCurve(Conic, Conic.GetPieTriangle(type), type);
        }

        /// <summary>
        /// Creates a curve replicationg data provided by circle parameter.
        /// </summary>
        /// <param name="conic">A circle whiose identical copy is to be created</param>
        /// <param name="assignID">If true assign an unique id to the object</param>
        /// <returns>ICurve</returns>
        public static ICurve newCurve(IConic conic, Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) 
        {
            CurveType type = CurveType.Full;
            float startAngle = 0, endAngle = 0;

            if (conic is ICurve)
            {
                startAngle = (conic as ICurve).StartAngle;
                endAngle = (conic as ICurve).EndAngle;
                type = (conic as ICurve).Type;
            }
            var Conic = newConic(conic.Bounds, 0, 0, rotation, Scale);
            return newCurve(Conic, Conic.GetPieTriangle(type, startAngle, endAngle), type);
        }
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
        public static IConic newConic(Rotation rotation, float x, float y, float width, float height, float startAngle = 0, float endAngle = 0, float tiltAngle = 0)
        {
            return Instance.newConic(rotation, x, y, width, height, startAngle, endAngle, tiltAngle);
        }

        /// <summary>
        /// Creates a conic section from the given points with angle of rotation if supplied. Every conic will result in being either ellipse or arc or pie.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="rotation">Angle to apply rotation while rendering the arc/pie</param>
        public static IConic newConic(VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5, Rotation rotation = default(Rotation), VectorF Scale = default(VectorF))
        {
            IList<VectorF> Original = new VectorF[] { p1, p2, p3, p4, p5 };

            if ((!rotation || rotation.Skew == 0) && !Scale.HasScale)
                goto mks;

            Original = Original.RotateAndScale(rotation, Scale, out _, out bool flat);
            if (flat)
                rotation = Rotation.Empty;
            mks:
            float Cx, Cy, W, H;
            var angle = LinePair.ConicAngle(Original[0], Original[1], Original[2], Original[3], Original[4], out Cx, out Cy, out W, out H);

            var startAngle = Original[0].GetAngle(angle, Cx, Cy);
            var endAngle = Original[1].GetAngle(angle, Cx, Cy);
            return newConic(rotation, Cx - W / 2, Cy - H / 2, W, H, startAngle, endAngle, angle);
        }

        /// <summary>
        /// Creates a conic section which results in an ellipse from the given points with angle of rotation if supplied.
        /// Providing three points will always result in this conic being ellipse.
        /// Because other two points will be calculated in a way that it will result in a valid ellipse rather than parabola or hyperbola.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="rotation">Angle to apply rotation while rendering the conic</param>
        public static IConic newConic(VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF))
        {
            Curves.GetEllipseMakingPoints(ref p1, ref p2, ref p3, ref p4, type, out VectorF p5);
            return newConic(p1, p2, p3, p4, p5, rotation, Scale);
        }

        /// <summary>
        /// Creates a conic section which results in an ellipse from the given points with angle of rotation if supplied.
        /// Providing three points will always result in this conic being ellipse.
        /// Because other two points will be calculated in a way that it will result in a valid ellipse rather than parabola or hyperbola.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="rotation">Angle to apply rotation while rendering the conic</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        public static IConic newConic(VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Full,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF))
        {
            Curves.GetEllipseMakingPoints(ref p1, ref p2, ref p3, type, out VectorF p4, out VectorF p5);
            return newConic(p1, p2, p3, p4, p5, rotation, Scale);
        }

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
        public static IConic newConic(float x, float y, float width, float height, float startAngle = 0, float endAngle = 0,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) 
        {
            var bounds = new RectangleF(x, y, width, height);

            if ((!rotation || rotation.Skew == 0) && !Scale.HasScale)
                goto mks;
            bounds = bounds.Scale(rotation, Scale, out bool flat);
            if (flat)
                rotation = Rotation.Empty;
            mks:
            return newConic(rotation, bounds.X, bounds.Y, bounds.Width, bounds.Height, startAngle, endAngle);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="rotation"></param>
        public static IConic newConic(RectangleF bounds, float startAngle = 0, float endAngle = 0,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) =>
            newConic(bounds.X, bounds.Y, bounds.Width, bounds.Height, startAngle, endAngle, rotation, Scale);
        #endregion

        #region TETRAGON
        /// <summary>
        /// Creates a tetragon specified by four points and applies an angle of rotation if supplied.
        /// </summary>
        /// <param name="first">First point.</param>
        /// <param name="second">Second point.</param>
        /// <param name="third">Third point.</param>
        /// <param name="fourth">Fourth point.</param>
        public static ITetragon newTetragon(VectorF first, VectorF second, VectorF third, VectorF fourth)
        {
            return Instance.newTetragon(first, second, third, fourth);
        }

        /// <summary>
        /// Creates a rhombus specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of the bounding rectangle</param>
        /// <param name="y">Y cordinate of the bounding rectangle</param>
        /// <param name="w">Width of the bounding rectangle/param>
        /// <param name="h">Height the bounding rectangle</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        /// <param name="deviation">If not null, it replaces the value of width parameter</param>
        public static ITetragon newTetragon(float x, float y, float w, float h, float? deviation = null) 
        {
            w = deviation ?? w;
            return newTetragon(new VectorF(x, y), new VectorF(x, y + h), new VectorF(x + w, y + h), new VectorF(x + w, y));
        }

        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public static ITetragon newTetragon(ILine baseLine, float deviation, StrokeMode mode = StrokeMode.StrokeMiddle, float skewBy = 0)
        {
            ILine second = newLine(baseLine);

            if (mode == StrokeMode.StrokeMiddle)
            {
                baseLine = newLine(baseLine, -deviation / 2f);
                second = newLine(second, deviation / 2f);
            }
            else if (mode == StrokeMode.StrokeOuter)
                second = newLine(second, deviation);
            else if (mode == StrokeMode.StrokeInner)
                second = newLine(second, -deviation);

            deviation = mode == StrokeMode.StrokeInner ? -deviation : deviation;

            VectorF difference;
            bool Steep;

            var Start = new VectorF(baseLine.X1, baseLine.Y1);
            var End = new VectorF(baseLine.X2, baseLine.Y2);
            difference = Start - End;
            Steep = Math.Abs(difference.Y) > Math.Abs(difference.X);

            if (skewBy != 0)
            {
                if (skewBy >= deviation)
                    skewBy = deviation - 1;

                second = Steep ? second.Offset(0, skewBy) :
                    second.Offset(skewBy, 0);
            }

            return newTetragon(Start, End, new VectorF(second.X2, second.Y2), new VectorF(second.X1, second.Y1));
        }

        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) specified by base line x1, y1, x2, y2, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="x1">X corordinate of start point of base line.</param>
        /// <param name="y1">Y corordinate of start point of base line.</param>
        /// <param name="x2">X corordinate of end point of base line.</param>
        /// <param name="y2">Y corordinate of end point of base line.</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while creating the trapezium</param>
        /// <param name="mode"> Stroke mode to apply while creating the trapezium</param>
        public static ITetragon newTetragon(float x1, float y1, float x2, float y2, float deviation, StrokeMode mode = StrokeMode.StrokeMiddle, float skewBy = 0) =>
            newTetragon(newLine(x1, y1, x2, y2), deviation, mode, skewBy);

        /// <summary>
        /// Creates a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        public static ITetragon newTetragon(VectorF p1, VectorF p2, VectorF p3) =>
            newTetragon(p1, p2, p3, Vectors.FourthPointOfRhombus(p1, p2, p3));

        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) with angle of rotation if supplied.
        /// Baseline, parallelLineDeviation and parallelLineDeviation specified by values int the following manner:
        /// First four values defines base line x1, y1, x2, y2.
        /// Fifth value defines parallelLineDeviation.
        /// Sixth value (optional) defines parallelLineSizeDifference;
        /// </summary>
        /// <param name="angle">Angle to apply rotation while creating the trapezium</param>
        /// <param name="values">Values which defines trapezium formation.</param>
        public static ITetragon newTetragon(params float[] values)
        {
            var first = newLine(values[0], values[1], values[2], values[3]);
            float parallelLineDeviation = 30f;
            float parallelLineSizeDifference = 0;
            if (values.Length < 6)
                parallelLineDeviation = values[4];
            if (values.Length > 5)
                parallelLineSizeDifference = values[5];
            return  newTetragon(first, parallelLineDeviation, StrokeMode.StrokeOuter, parallelLineSizeDifference);
        }

        /// <summary>
        /// Creates a trapezium (defined as per the definition in British English) specified by start and end points forming a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">Start point of base line.</param>
        /// <param name="p2">End point of base line.</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="mode"> Stroke mode to apply while creating the trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public static ITetragon newTetragon(VectorF p1, VectorF p2, float deviation, StrokeMode mode = StrokeMode.StrokeMiddle, float skewBy = 0) =>
            newTetragon(p1.X, p1.Y, p2.X, p2.Y, deviation, mode, skewBy);

        /// <summary>
        /// Creates a tetragon specified by points (minimum four are required) and apply an angle of rotation if supplied.
        /// </summary>
        /// <param name="points">Collection of points to create tetragon from.</param>
        public static ITetragon newTetragon(IList<VectorF> points)
        {
            if (points == null || points.Count < 3)
                return null;

            if (points.Count > 3)
                return newTetragon(points[0], points[1], points[2], points[3]);
            else
                return newTetragon(points[0], points[1], points[2]);
        }

        /// <summary>
        /// Creates a rhombus identical to specified area and angle of rotation if supplied.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        /// <param name="deviation">If not null, it replaces the value of width parameter of area.</param>
        public static ITetragon newTetragon(Rectangle area, float? deviation = null) =>
            newTetragon(area.X, area.Y, area.Width, area.Height, deviation);

        /// <summary>
        /// Creates a rhombus identical to specified area and angle of rotation if supplied.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        /// <param name="deviation">If not null, it replaces the value of width parameter of area.</param>
        public static ITetragon newTetragon(RectangleF area, float? deviation = null) =>
            newTetragon(area.X, area.Y, area.Width, area.Height, deviation);
        #endregion

        #region BEZIER
        /// <summary>
        /// Creates a bezier defined by either pointsData (float values) or pixels (points) and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="type">Type of bezier to create.</param>
        /// <param name="pointValues">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="points">Points which defines perimiter of the bezier.</param>
        public static IBezier newBezier(BezierType type, ICollection<float> pointValues, IList<VectorF> points)
        {
           return Instance.newBezier(type, pointValues, points);
        }

        /// <summary>
        /// Creates a bezier defined by points and specified by type.
        /// </summary>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        public static IBezier newBezier(params float[] points) =>
            newBezier(0, points as IList<float>);

        /// <summary>
        /// Creates a bezier defined by points and specified by type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        public static IBezier newBezier(BezierType type, params float[] points) =>
            newBezier(type, points as IList<float>);

        /// <summary>
        /// Creates a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="type">Type of bezier to create.</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        public static IBezier newBezier(BezierType type, IList<float> points) =>
            newBezier(type, points, null);

        /// <summary>
        /// Creates a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="type">Type of bezier to create.</param>
        /// <param name="points">Points which defines perimiter of the bezier.</param>
        public static IBezier newBezier(BezierType type, IList<VectorF> points) =>
            newBezier(type, null, points);
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
        public static ITriangle newTriangle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            return Instance.newTriangle(x1, y1, x2, y2, x3, y3);
        }

        /// <summary>
        /// Creates a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        public static ITriangle newTriangle(Vector p1, Vector p2, Vector p3) =>
            newTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

        /// <summary>
        /// Creates a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        public static ITriangle newTriangle(VectorF p1, VectorF p2, VectorF p3) =>
            newTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

        /// <summary>
        /// Creates a trianle formed by three points - start and end points of line and another point p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="l">Line to supplying start and end points.</param>
        /// <param name="p3">the third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the traingle</param>
        public static ITriangle newTriangle(ILine l, Vector p3) =>
            newTriangle(l.X1, l.Y1, l.X2, l.Y2, p3.X, p3.Y);

        /// <summary>
        /// Creates a trianle formed by three points - start and end points of line and another point p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="l">Line to supplying start and end points.</param>
        /// <param name="p3">the third point</param>
        public static ITriangle newTriangle(ILine l, VectorF p3) =>
            newTriangle(l.X1, l.Y1, l.X2, l.Y2, p3.X, p3.Y);
        #endregion

        #region BOX
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        public static IBox newBox(int x, int y, int width, int height)
        {
            return Instance.newBox(x, y, width, height);
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="r">Area to copy bounds from.</param>
        public static IBox newBox(Rectangle r) =>
            newBox(r.X, r.Y, r.Width, r.Height);

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public static IBox newBox(Vector xy, Size wh) =>
            newBox(xy.X, xy.Y, wh.Width, wh.Height);

        /// <summary>
        /// Creates a new square with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the box.</param>
        /// <param name="y">Far top horizontal co-rodinate of the box.</param>
        /// <param name="w">Width of the box.</param>
        public static IBox newBox(int x, int y, int w) =>
            newBox(x, y, w, w);

        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static IBox FromLTRB(int x, int y, int right, int bottom, bool correct = true)
        {
            if (!correct)
                return newBox(x, y, right - x, bottom - y);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return newBox(x, y, w, h);
        }
        #endregion

        #region BOXF
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="width">Width of the rectangle.</param>
        /// <param name="height">Height of the rectangle.</param>
        public static IBoxF newBoxF(float x, float y, float width, float height)
        {
            return Instance.newBoxF(x, y, width, height);
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="r">Area to copy bounds from.</param>
        public static IBoxF newBoxF(Rectangle r) =>
            newBoxF(r.X, r.Y, r.Width, r.Height);

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public static IBoxF newBoxF(RectangleF r) =>
            newBoxF(r.X, r.Y, r.Width, r.Height);

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public static IBoxF newBoxF(VectorF xy, SizeF wh) =>
            newBoxF(xy.X, xy.Y, wh.Width, wh.Height);

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public static IBoxF newBoxF(Vector xy, Size wh) =>
            newBoxF(xy.X, xy.Y, wh.Width, wh.Height);

        /// <summary>
        /// Creates a new square with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the box.</param>
        /// <param name="y">Far top horizontal co-rodinate of the box.</param>
        /// <param name="w">Width of the box.</param>
        public static IBoxF newBoxF(float x, float y, float w) =>
            newBoxF(x, y, w, w);

        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static IBoxF FromLTRB(float x, float y, float right, float bottom, bool correct = true)
        {
            if (!correct)
                return newBoxF(x, y, right - x, bottom - y);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return newBoxF(x, y, w, h);
        }
        #endregion

        #region SHAPE
        /// <summary>
        /// Returns an instance of IShape.
        /// </summary>
        /// <param name="shape">Points to form a shape.</param>
        /// <param name="name">NAme of shape</param>
        /// <returns></returns>
        public static IShape newShape(IEnumerable<VectorF> shape, string name)
        {
            return Instance.newShape(shape, name);
        }
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
        public static IGlyphs newGlyphs(string text, RectangleF area, IList<IGlyph> resultGlyphs, float minHBY)
        {
           return Instance.newGlyphs(text, area, resultGlyphs, minHBY);
        }
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
        public static IRoundBox newRoundBox(float x, float y, float w, float h, float cornerRadius, bool positiveLocation = false)
        {
           return Instance.newRoundBox(x, y, w, h, cornerRadius, positiveLocation);
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to match bounds from.</param>
        public static IRoundBox newRoundBox(Rectangle area, float cornerRadius) =>
            newRoundBox(area.X, area.Y, area.Width, area.Height, cornerRadius);

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public static IRoundBox newRoundBox(RectangleF area, float cornerRadius) =>
            newRoundBox(area.X, area.Y, area.Width, area.Height, cornerRadius);

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public static IRoundBox newRoundBox(VectorF xy, SizeF wh, float cornerRadius) =>
            newRoundBox(xy.X, xy.Y, wh.Width, wh.Height, cornerRadius);

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public static IRoundBox newRoundBox(Vector xy, Size wh, float cornerRadius) =>
            newRoundBox(xy.X, xy.Y, wh.Width, wh.Height, cornerRadius);

        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static IRoundBox FromLTRB(float x, float y, float right, float bottom, float cornerRadius, bool correct = true)
        {
            if (!correct)
                return newRoundBox(x, y, right - x, bottom - y, cornerRadius);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return newRoundBox(x, y, w, h, cornerRadius);
        }
        #endregion

        #region POLYGON
        /// <summary>
        /// Creates a new polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon.</param>
        public static IPolygon newPolygon(IList<VectorF> polyPoints) =>
            Instance.newPolygon(polyPoints);

        /// <summary>
        /// Creates a new polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x, y.</param>
        public static IPolygon newPolygon(params float[] polyPoints) =>
            newPolygon(polyPoints.ToPoints());

        /// <summary>
        /// Creates a new polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x, y.</param>
        public static IPolygon newPolygon(params VectorF[] polyPoints) =>
            newPolygon(polyPoints as IList<VectorF>);
        #endregion

        #region TEXT
        /// <summary>
        /// Cretes new text object with given parameters.
        /// </summary>
        /// <param name="glyphs">A list of processed glyphs collection from font</param>
        /// <param name="drawStyle">A specific drawstyle to use to measure and draw glyphs if desired so</param>
        /// <param name="dstX">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="dstY">X cordinate of destination location where glyphs to be drawn</param>
        public static IText newText(IList<IGlyph> glyphs, ITextStyle drawStyle = null, int? dstX = null, int? dstY = null) =>
            Instance.newText(glyphs, drawStyle, dstX, dstY);

        /// <summary>
        /// Cretes new text object with given parameters.
        /// </summary>
        /// <param name="font">the font object to be used to get glyphs</param>
        /// <param name="text">A text string to process to obtain glyphs collection from font</param>
        /// <param name="dstX">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="dstY">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="drawStyle">A specific drawstyle to use to measure and draw glyphs if desired so</param>
        public static IText newText(IFont font, string text, int dstX, int dstY, ITextStyle drawStyle = null) =>
             Instance.newText(font, text,  dstX, dstY, drawStyle);
        #endregion

        class _EventArgs : EventArgs, IEventArgs { }
        public static void Dispose()
        {
            Instance.Dispose();
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
