/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.IO;

#if Standard
namespace MnM.GWS.Standard
#elif Advanced
namespace MnM.GWS.Advanced
#else
namespace MnM.GWS
#endif
{
    public abstract partial class _Factory : IFactory
    {
        #region VARIABLES
        bool isDisposed;
        #endregion

        #region IMAGE PROCESSOR
        public virtual IImageProcessor newImageProcessor() =>
            new StbImageProcessor();
        #endregion

        #region CONVERTER
        public virtual IConverter newConverter() =>
            new Converter();
        #endregion

        #region MISC
        public virtual IAnimatedGifFrame newAnimatedGifFrame(byte[] data, int delay) =>
            STBImage.GetAnimatedGifFrame(data, delay);
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            isDisposed = true;
            Dispose2();
        }
        protected virtual void Dispose2() { }
        #endregion

#if GWS || Window
        #region TO PEN
        public abstract IReadable ToPen(IPenContext context, int? w = null, int? h = null);
        #endregion

        #region SURFACE
        public abstract ISurface newSurface(int width, int height);
        public abstract unsafe ISurface newSurface(IntPtr pixels, int width, int height);
        public abstract ISurface newSurface(int[] pixels, int width, int height, bool makeCopy = false);
        public abstract ISurface newSurface(int width, int height, byte[] pixels, bool makeCopy = false);
        #endregion

        #region IMAGE
        public abstract IImage newImage(int width, int height);
        public abstract IImage newImage(IntPtr pixels, int width, int height);
        public abstract IImage newImage(int[] pixels, int width, int height, bool makeCopy = false);
        public abstract IImage newImage(int width, int height, byte[] pixels, bool makeCopy = false);
        #endregion

        #region CANVAS
        public abstract ICanvas newCanvas(IRenderTarget window);
        #endregion

        #region RENDER TARGET
#if Window
        public abstract IRenderTarget newRenderTarget(IRenderWindow window);
#endif
        #endregion

        #region FORM
        public abstract IForm newForm(int x, int y, int w, int h);
        #endregion

        #region NATIVE WINDOW
        public abstract INativeTarget newNativeTarget(int x, int y, int w, int h);
        #endregion

        #region OBJECT COLLECTION
        public abstract IObjCollection newObjectCollection(IImage buffer);
        #endregion

        #region SHAPE PARSER
        public abstract IShapeParser newShapeParser();
        #endregion

        #region BRUSH - PEN
        public abstract IBrush newBrush(BrushStyle style, int width, int height);
        public abstract ITextureBrush newBrush(IntPtr data, int width, int height);
        #endregion

        #region PEN
        public virtual IReadable newPen(int color) =>
            Pen.CreateInstance(color);
        #endregion

        #region POLY FILL
        public virtual IPolyFill newPolyFill() =>
            new PolyFill();
        #endregion

        #region COLOR
        public virtual Rgba newColor(byte r, byte g, byte b, byte a = 255) =>
            new Rgba(r, g, b, a);
        #endregion

        #region FONT
        public virtual IFont newFont(Stream fontStream, int fontSize) =>
            new Font(fontStream, fontSize);
        #endregion

        #region LINE
        public virtual ILine newLine(float x1, float y1, float x2, float y2) =>
            new Line(x1, y1, x2, y2);
        #endregion

        #region CURVE
        public virtual ICurve newCurve(float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, CurveType type = 0, Rotation rotation = default(Rotation), VectorF scale = default(VectorF)) =>
            new Curve(x, y, width, height, startAngle, endAngle, type, rotation, scale);

        public virtual ICurve newCurve(IConic conic, VectorF[] pieTriangle, CurveType type) =>
            new Curve(conic, pieTriangle, type);
        #endregion

        #region CONIC
        public virtual IConic newConic(Rotation rotation, float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, float tiltAngle = 0) =>
            new Conic(rotation, x, y, width, height, startAngle, endAngle, tiltAngle);
        #endregion

        #region TETRAGON
        public virtual ITetragon newTetragon(VectorF first, VectorF second, VectorF third, VectorF fourth) =>
            new Tetragon(first, second, third, fourth);
        #endregion

        #region BEZIER
        public virtual IBezier newBezier(BezierType type, ICollection<float> pointValues, IList<VectorF> points) =>
            new Bezier(type, pointValues, points);
        #endregion

        #region TRIANGLE
        public virtual ITriangle newTriangle(float x1, float y1, float x2, float y2, float x3, float y3) =>
            new Triangle(x1, y1, x2, y2, x3, y3);
        #endregion

        #region BOX
        public virtual IBox newBox(int x, int y, int width, int height) =>
            new Box(x, y, width, height);
        #endregion

        #region BOXF
        public virtual IBoxF newBoxF(float x, float y, float width, float height) =>
            new BoxF(x, y, width, height);
        #endregion

        #region FIGURE
        public virtual IFigure newFigure(IEnumerable<VectorF> shape, string name) =>
            new Figure(shape, name);
        #endregion

        #region GLYPHS
        public virtual IGlyphs newGlyphs(string text, RectangleF area, IList<IGlyph> resultGlyphs, float minHBY) =>
            new Glyphs(text, area, resultGlyphs, minHBY);
        #endregion

        #region ROUNDBOX
        public virtual IRoundBox newRoundBox(float x, float y, float w, float h, float cornerRadius, bool positiveLocation = false) =>
            new RoundBox(x, y, w, h, cornerRadius, positiveLocation);
        #endregion

        #region POLYGON
        public virtual IPolygon newPolygon(IList<VectorF> polyPoints) =>
            new Polygon(polyPoints);
        #endregion

        #region TEXT
        public virtual IText newText(IList<IGlyph> glyphs, ITextStyle drawStyle = null, int? dstX = null, int? dstY = null) =>
            new Text(glyphs, drawStyle, dstX, dstY);

        public virtual IText newText(IFont font, string text, int dstX, int dstY, ITextStyle drawStyle = null) =>
            new Text(font, text, dstX, dstY, drawStyle);
        #endregion

        #region BOUNDARY
        public virtual IBoundary newBoundary() =>
            new Boundary();
        #endregion

        #region PUSH, PUMP, POLL EVENTS
        public abstract void PushEvent(IEvent e);
        public abstract void PumpEvents();
        public abstract bool PollEvent(out IEvent e);
        #endregion
#endif
    }
}
