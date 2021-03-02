/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

//The following are just a namespaces allocation statements.
using System;
using System.Collections.Generic;
using System.IO;

namespace MnM.GWS.Standard { }
namespace MnM.GWS.Advanced { }

namespace MnM.GWS
{
    public partial class NativeFactory : IFactory
    {
        #region VARIABLES
        public static readonly IFactory Instance = new NativeFactory();
        protected static readonly bool initialized;
        #endregion

        #region CONSTRUCTORS
        NativeFactory()
        {
            Initialize();
        }
        static NativeFactory()
        {
            InitializeWindowingSystem(ref initialized);
        }
        partial void Initialize();
        static partial void InitializeWindowingSystem(ref bool initialized);
        partial void Initialize()
        {
#if!(Standard || Advanced)
            throw new Exception(
                "You must implement abstract layer by yourself or get implemented Standard or " +
                "Advanced version from http://mnminfotech.co.uk by placing requirement.");
#endif
        }
        #endregion

        #region IMAGE PROCESSOR
        public IImageProcessor newImageProcessor()
        {
            IImageProcessor imageProcessor = null;
            newImageProcessor(ref imageProcessor);
            return imageProcessor;
        }
        partial void newImageProcessor(ref IImageProcessor imageProcessor);
        partial void newImageProcessor(ref IImageProcessor imageProcessor) =>
            imageProcessor = new StbImageProcessor();
        #endregion

        #region CONVERTER
        public IConverter newConverter()
        {
            IConverter converter = null;
            newConverter(ref converter);
            return converter;
        }
        partial void newConverter(ref IConverter converter);
        partial void newConverter(ref IConverter converter) =>
            converter = new Converter();
        #endregion

        #region MISC
        public IAnimatedGifFrame newAnimatedGifFrame(byte[] data, int delay)
        {
            IAnimatedGifFrame animatedGifFrame = null;
            newAnimatedGifFrame(ref animatedGifFrame, data, delay);
            return animatedGifFrame;
        }
        partial void newAnimatedGifFrame(ref IAnimatedGifFrame animatedGifFrame, byte[] data, int delay);
        partial void newAnimatedGifFrame(ref IAnimatedGifFrame animatedGifFrame, byte[] data, int delay)
        {
            animatedGifFrame= STBImage.GetAnimatedGifFrame(data, delay);
        }
        #endregion

#if GWS || Window
        #region IMAGE
        public IImage newImage(int width, int height)
        {
            IImage image = null;
            newImage(ref image, width, height);
            return image;
        }
        partial void newImage(ref IImage image, int width, int height);

        public unsafe IImage newImage(IntPtr pixels, int width, int height)
        {
            IImage image = null;
            newImage(ref image, pixels, width, height);
            return image;
        }
        partial void newImage(ref IImage image, IntPtr pixels, int width, int height);

        public IImage newImage(int[] pixels, int width, int height, bool makeCopy = false)
        {
            IImage image = null;
            newImage(ref image, pixels, width, height, makeCopy);
            return image;
        }
        partial void newImage(ref IImage image, int[] pixels, int width, int height, bool makeCopy = false);

        public IImage newImage(int width, int height, byte[] pixels)
        {
            IImage image = null;
            newImage(ref image, width, height, pixels);
            return image;
        }
        partial void newImage(ref IImage image, int width, int height, byte[] pixels);
        #endregion

        #region CANVAS
        public ICanvas newCanvas(IRenderTarget window)
        {
            ICanvas canvas = null;
            newCanvas(ref canvas, window);
            return canvas;
        }
        partial void newCanvas(ref ICanvas canvas, IRenderTarget window);
        #endregion

        #region NATIVE WINDOW
        public INativeTarget newNativeTarget(int x, int y, int w, int h)
        {
            INativeTarget target = null;
            newNativeTarget(ref target, x, y, w, h);
            return target;
        }
        partial void newNativeTarget(ref INativeTarget target, int x, int y, int w, int h);
        #endregion

        #region FORM
        public IForm newForm(int x, int y, int w, int h)
        {
            IForm form = null;
            newForm(ref form, x, y, w, h);
            return form;
        }
        partial void newForm(ref IForm form, int x, int y, int w, int h);
        partial void newForm(ref IForm form, int x, int y, int w, int h)
        {
            form = new NativeForm(x, y, w, h);
        }

        public IForm newForm(INativeTarget target)
        {
            IForm form = null;
            newForm(ref form, target);
            return form;
        }
        partial void newForm(ref IForm form, INativeTarget target);
        partial void newForm(ref IForm form, INativeTarget target)
        {
            form = new NativeForm(target);
        }
        #endregion

        #region BRUSH
        public IBrush newBrush(BrushStyle style, int width, int height)
        {
            IBrush brush = null;
            newBrush(ref brush, style, width, height);
            return brush;
        }
        partial void newBrush(ref IBrush brush, BrushStyle style, int width, int height);
        public ITextureBrush newBrush(IntPtr data, int width, int height)
        {
            ITextureBrush brush = null;
            newBrush(ref brush, data, width, height);
            return brush;
        }
        partial void newBrush(ref ITextureBrush brush, IntPtr data, int width, int height);
        #endregion

        #region TO PEN
        public IReadable ToPen(IPenContext context, int? w = null, int? h = null)
        {
            IReadable brush = null;
            ToPen(ref brush, context, w, h);
            return brush;
        }
        partial void ToPen(ref IReadable readable, IPenContext context, int? w = null, int? h = null);
        #endregion

        #region FONT
        public IFont newFont(Stream fontStream, int fontSize)
        {
            IFont font = null;
            newFont(ref font, fontStream, fontSize);
            return font;
        }
        partial void newFont(ref IFont font, Stream fontStream, int fontSize);
        partial void newFont(ref IFont font, Stream fontStream, int fontSize)
        {
            font = new Font(fontStream, fontSize);
        }
        #endregion

        #region SHAPE PARSER
        public IShapeParser newShapeParser()
        {
            IShapeParser shapeParser = null;
            newShapeParser(ref shapeParser);
            return shapeParser;
        }
        partial void newShapeParser(ref IShapeParser shapeParser);
        #endregion

        #region LINE
        public ILine newLine(float x1, float y1, float x2, float y2)
        {
            ILine line = null;
            newLine(ref line, x1, y1, x2, y2);
            return line;
        }
        partial void newLine(ref ILine line, float x1, float y1, float x2, float y2);
        partial void newLine(ref ILine line, float x1, float y1, float x2, float y2)
        {
            line = new Line(x1, y1, x2, y2);
        }
        #endregion

        #region CURVE
        public ICurve newCurve(float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, CurveType type = 0, Rotation rotation = default(Rotation), VectorF scale = default(VectorF))
        {
            ICurve curve = null;
            newCurve(ref curve, x, y, width, height, startAngle, endAngle, type, rotation, scale);
            return curve;
        }
        partial void newCurve(ref ICurve curve, float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, CurveType type = 0, Rotation rotation = default(Rotation), VectorF scale = default(VectorF));
        partial void newCurve(ref ICurve curve, float x, float y, float width, float height, float startAngle, float endAngle, 
            CurveType type, Rotation rotation, VectorF scale)
        {
            curve = new Curve(x, y, width, height, startAngle, endAngle, type, rotation, scale);
        }

        public ICurve newCurve(IConic conic, VectorF[] pieTriangle, CurveType type)
        {
            ICurve curve = null;
            newCurve(ref curve, conic, pieTriangle, type);
            return curve;
        }
        partial void newCurve(ref ICurve curve, IConic conic, VectorF[] pieTriangle, CurveType type);
        partial void newCurve(ref ICurve curve, IConic conic, VectorF[] pieTriangle, CurveType type)
        {
            curve = new Curve(conic, pieTriangle, type);
        }
        #endregion

        #region CONIC
        public IConic newConic(Rotation rotation, float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, float tiltAngle = 0)
        {
            IConic conic = null;
            newConic(ref conic, rotation, x, y, width, height, startAngle, endAngle, tiltAngle);
            return conic;
        }
        partial void newConic(ref IConic conic, Rotation rotation, float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, float tiltAngle = 0);
        partial void newConic(ref IConic conic, Rotation rotation, float x, float y, float width, float height, float startAngle, float endAngle, float tiltAngle)
        {
            conic = new Conic(rotation, x, y, width, height, startAngle, endAngle, tiltAngle);
        }
        #endregion

        #region TETRAGON
        public ITetragon newTetragon(VectorF first, VectorF second, VectorF third, VectorF fourth)
        {
            ITetragon tetragon = null;
            newTetragon(ref tetragon, first, second, third, fourth);
            return tetragon;
        }
        partial void newTetragon(ref ITetragon tetragon, VectorF first, VectorF second, VectorF third, VectorF fourth);
        partial void newTetragon(ref ITetragon tetragon, VectorF first, VectorF second, VectorF third, VectorF fourth)
        {
            tetragon = new Tetragon(first, second, third, fourth);
        }
        #endregion

        #region BEZIER
        public IBezier newBezier(BezierType type, ICollection<float> pointValues, IList<VectorF> points)
        {
            IBezier bezier = null;
            newBezier(ref bezier, type, pointValues, points);
            return bezier;
        }
        partial void newBezier(ref IBezier bezier, BezierType type, ICollection<float> pointValues, IList<VectorF> points);
        partial void newBezier(ref IBezier bezier, BezierType type, ICollection<float> pointValues, IList<VectorF> points)
        {
            bezier = new Bezier(type, pointValues, points);
        }
        #endregion

        #region TRIANGLE
        public ITriangle newTriangle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            ITriangle triangle = null;
            newTriangle(ref triangle, x1, y1, x2, y2, x3, y3);
            return triangle;
        }
        partial void newTriangle(ref ITriangle triangle, float x1, float y1, float x2, float y2, float x3, float y3);
        partial void newTriangle(ref ITriangle triangle, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            triangle = new Triangle(x1, y1, x2, y2, x3, y3);
        }
        #endregion

        #region BOX
        public IBox newBox(int x, int y, int width, int height)
        {
            IBox box = null;
            newBox(ref box, x, y, width, height);
            return box;
        }
        partial void newBox(ref IBox box, int x, int y, int width, int height);
        partial void newBox(ref IBox box, int x, int y, int width, int height)
        {
            box = new Box(x, y, width, height);
        }
        #endregion

        #region BOXF
        public IBoxF newBoxF(float x, float y, float width, float height)
        {
            IBoxF box = null;
            newBoxF(ref box, x, y, width, height);
            return box;
        }
        partial void newBoxF(ref IBoxF box, float x, float y, float width, float height);
        partial void newBoxF(ref IBoxF box, float x, float y, float width, float height)
        {
            box = new BoxF(x, y, width, height);
        }
        #endregion

        #region SHAPE
        public IFigure newFigure(IEnumerable<VectorF> shape, string name)
        {
            IFigure figure = null;
            newFigure(ref figure, shape, name);
            return figure;
        }
        partial void newFigure(ref IFigure figure, IEnumerable<VectorF> shape, string name);
        partial void newFigure(ref IFigure figure, IEnumerable<VectorF> shape, string name)
        {
            figure = new Figure(shape, name);
        }
        #endregion

        #region GLYPHS
        public IGlyphs newGlyphs(string text, RectangleF area, IList<IGlyph> resultGlyphs, float minHBY)
        {
            IGlyphs glyphs = null;
            newGlyphs(ref glyphs, text, area, resultGlyphs, minHBY);
            return glyphs;
        }
        partial void newGlyphs(ref IGlyphs glyphs, string text, RectangleF area, IList<IGlyph> resultGlyphs, float minHBY);
        partial void newGlyphs(ref IGlyphs glyphs, string text, RectangleF area, IList<IGlyph> resultGlyphs, float minHBY)
        {
            glyphs = new Glyphs(text, area, resultGlyphs, minHBY);
        }
        #endregion

        #region ROUNDBOX
        public IRoundBox newRoundBox(float x, float y, float w, float h, float cornerRadius, RoundBoxOption option)
        {
            IRoundBox box = null;
            newRoundBox(ref box, x, y, w, h, cornerRadius, option);
            return box;
        }
        partial void newRoundBox(ref IRoundBox box, float x, float y, float w, float h, float cornerRadius, RoundBoxOption option = 0);
        partial void newRoundBox(ref IRoundBox box, float x, float y, float w, float h, float cornerRadius, RoundBoxOption option)
        {
            box = new RoundBox(x, y, w, h, cornerRadius, option);
        }
        #endregion

        #region POLYGON
        public IPolygon newPolygon(IList<VectorF> polyPoints)
        {
            IPolygon polygon = null;
            newPolygon(ref polygon, polyPoints);
            return polygon;
        }
        partial void newPolygon(ref IPolygon polygon, IList<VectorF> polyPoints);
        partial void newPolygon(ref IPolygon polygon, IList<VectorF> polyPoints)
        {
            polygon = new Polygon(polyPoints);
        }
        #endregion

        #region TEXT
        public IText newText(IList<IGlyph> glyphs, ITextStyle drawStyle = null, int? dstX = null, int? dstY = null)
        {
            IText text = null;
            newText(ref text, glyphs, drawStyle, dstX, dstY);
            return text;
        }
        partial void newText(ref IText text, IList<IGlyph> glyphs, ITextStyle drawStyle = null, int? dstX = null, int? dstY = null);
        partial void newText(ref IText text, IList<IGlyph> glyphs, ITextStyle drawStyle, int? dstX, int? dstY)
        {
            text = new Text(glyphs, drawStyle, dstX, dstY);
        }

        public IText newText(IFont font, string text, int dstX, int dstY, ITextStyle drawStyle = null)
        {
            IText glyphs = null;
            newText(ref glyphs, font, text, dstX, dstY, drawStyle);
            return glyphs;
        }
        partial void newText(ref IText glyphs, IFont font, string text, int dstX, int dstY, ITextStyle drawStyle = null);
        partial void newText(ref IText glyphs, IFont font, string text, int dstX, int dstY, ITextStyle drawStyle)
        {
            glyphs = new Text(font, text, dstX, dstY, drawStyle);
        }
        #endregion

        #region PEN
        public IPen newPen(int color)
        {
            IPen readable = null;
            newPen(ref readable, color);
            return readable;
        }
        partial void newPen(ref IPen readable, int color);
        partial void newPen(ref IPen readable, int color)
        {
            readable = Pen.CreateInstance(color);
        }
        #endregion

        #region POLY FILL
        public IPolyFill newPolyFill()
        {
            IPolyFill polyFill = null;
            newPolyFill(ref polyFill);
            return polyFill;
        }
        partial void newPolyFill(ref IPolyFill polyFill);
        partial void newPolyFill(ref IPolyFill polyFill)
        {
            polyFill = new PolyFill();
        }
        #endregion

        #region COLOR
        public Rgba newColor(byte r, byte g, byte b, byte a = 255) =>
            new Rgba(r, g, b, a);
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            Dispose2();
        }
        partial void Dispose2();
        #endregion
#endif
    }

#if Window
    partial class NativeFactory
    {
        #region PROPERTIES
        public IScreen PrimaryScreen
        {
            get
            {
                IScreen screen = null;
                GetScreen(ref screen);
                return screen;
            }
        }
        partial void GetScreen(ref IScreen screen);

        public int DefaultWinFlag
        {
            get
            {
                int winflag = 0;
                GetDefaultWinFlag(ref winflag);
                return winflag;
            }
        }
        partial void GetDefaultWinFlag(ref int winflag);

        public int FullScreenWinFlag
        {
            get
            {
                int winflag = 0;
                GetScreenFlag(ref winflag);
                return winflag;
            }
        }
        partial void GetScreenFlag(ref int winflag);

        public IScreens AvailableScreens
        {
            get
            {
                IScreens screens = null;
                GetScreens(ref screens);
                return screens;
            }
        }
        partial void GetScreens(ref IScreens screens);

        public uint[] PixelFormats
        {
            get
            {
                uint[] pixelFormats = null;
                GetPixelFormats(ref pixelFormats);
                return pixelFormats;
            }
        }
        partial void GetPixelFormats(ref uint[] pixelFormats);

        public uint PixelFormat
        {
            get
            {
                uint pixelFormat = 0;
                GetPixelFormat(ref pixelFormat);
                return pixelFormat;
            }
        }
        partial void GetPixelFormat(ref uint pixelFormats);

        public bool Initialized => initialized;
        public OS OS
        {
            get
            {
                OS os = 0;
                GetOS(ref os);
                return os;
            }
        }
        partial void GetOS(ref OS os);
        public string LastError
        {
            get
            {
                string error = null;
                GetLastError(ref error);
                return error;
            }
        }
        partial void GetLastError(ref string error);
        #endregion

        #region TEXTURE
        public ITexture newTexture(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false, TextureAccess? textureAccess = null)
        {
            ITexture texture = null;
            newTexture(ref texture, window, w, h, isPrimary, textureAccess);
            return texture;
        }
        partial void newTexture(ref ITexture texture, IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false, TextureAccess? textureAccess = null);
       
        public ITexture newTexture(IRenderWindow window, ICopyable info, bool isPrimary = false, TextureAccess? textureAccess = null)
        {
            ITexture texture = null;
            newTexture(ref texture, window, info, isPrimary, textureAccess);
            return texture;
        }
        partial void newTexture(ref ITexture texture, IRenderWindow window, ICopyable info, bool isPrimary = false, TextureAccess? textureAccess = null);
        #endregion

        #region WINDOW
        public IWindow newWindow(string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null, RendererFlags? renderFlags = null)
        {
            IWindow window = null;
            newWindow(ref window, title, width, height, x, y, flags, display, renderFlags);
            return window;
        }
        partial void newWindow(ref IWindow window, string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null, RendererFlags? renderFlags = null);


        public IWindow newWindow(IExternalTarget control)
        {
            IWindow window = null;
            newWindow(ref window, control);
            return window;
        }
        partial void newWindow(ref IWindow window, IExternalTarget control);

        public int GetWindowID(IntPtr window)
        {
            int id = 0;
            GetWindowID2(ref id, window);
            return id;
        }
        partial void GetWindowID2(ref int id, IntPtr window);


        public void SetCursorPos(int x, int y) =>
            SetCursorPos2(x, y);
        partial void SetCursorPos2(int x, int y);

        public void DisableScreenSaver() =>
            DisableScreenSaver2();
        partial void DisableScreenSaver2();
        #endregion

        #region RENDER TARGET
        public IRenderTarget newRenderTarget(IRenderWindow window)
        {
            IRenderTarget target = null;
            newRenderTarget(ref target, window);
            return target;
        }
        partial void newRenderTarget(ref IRenderTarget target, IRenderWindow window);
        #endregion

        #region OPENGL CONTEXT
        public IGLContext newGLContext(IWindow window)
        {
            IGLContext target = null;
            newGLContext(ref target, window);
            return target;
        }

        partial void newGLContext(ref IGLContext gLContext, IWindow window);
        #endregion

        #region SAVE AS BITMAP
        public unsafe bool SaveAsBitmap(IBlockable image, string file, ulong command = Command.WriteToScreen)
        {
            if (image == null)
                return false;

            image.CopyTo(out IntPtr data, 0, 0, image.Width, image.Height, command);
            if (data == IntPtr.Zero)
                return false;
            bool success = false;
            SaveAsBitmap(ref success, data, image.Width, image.Height, file);
            return success;
        }
        partial void SaveAsBitmap(ref bool success, IBlockable image, string file, ulong command = Command.WriteToScreen);

        public unsafe bool SaveAsBitmap(IntPtr Pixels, int width, int height, string file)
        {
            if (Pixels == IntPtr.Zero)
                return false;

            bool success = false;
            SaveAsBitmap(ref success, Pixels, width, height, file);
            return success;
        }
        partial void SaveAsBitmap(ref bool success, IntPtr Pixels, int width, int height, string file);
        #endregion

        #region CURSOR
        public int ConvertToSystemCursorID(CursorType cursorType)
        {
            int cursorID = 0;
            GetCursorID(ref cursorID, cursorType);
            return cursorID;
        }
        partial void GetCursorID(ref int systemCursorID, CursorType cursorType);

        public void SetCursor(IntPtr cursor) =>
            SetCursor2(cursor);
        partial void SetCursor2(IntPtr cursor);
        #endregion

        #region EVENTS
        public void PushEvent(IEvent e) =>
            PushEvent2(e);
        partial void PushEvent2(IEvent e);
        public void PumpEvents() =>
            PumpEvents2();
        partial void PumpEvents2();

        public bool PollEvent(out IEvent e)
        {
            e = null;
            bool success = false;
            PollEvent2(ref success, ref e);
            return success;
        }
        partial void PollEvent2(ref bool success, ref IEvent e);
        #endregion

        #region WAV PLAYER
        public ISound newWavPlayer()
        {
            ISound sound = null;
            newWavPlayer(ref sound);
            return sound;
        }
        partial void newWavPlayer(ref ISound sound);
        #endregion
    }
#endif
}
