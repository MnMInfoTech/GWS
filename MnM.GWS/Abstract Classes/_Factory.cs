/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.IO;
#if Advanced
using MnM.GWS.Advanced;
#endif

namespace MnM.GWS
{
    public abstract partial class _Factory : IFactory
    {
        #region VARIABLES
        bool isDisposed;
        #endregion

        #region COLOR
        public virtual Rgba newColor(byte r, byte g, byte b, byte a = 255) =>
            new Rgba(r, g, b, a);
        #endregion

        #region SURFACE
        public abstract ISurface newSurface(int width, int height);
        public abstract unsafe ISurface newSurface(IntPtr pixels, int width, int height);
        public abstract ISurface newSurface(int[] pixels, int width, int height, bool makeCopy = false);
        public abstract ISurface newSurface(int width, int height, byte[] pixels, bool makeCopy = false);
        #endregion

        #region CANVAS
        public abstract ICanvas newCanvas(int width, int height);
        public abstract unsafe ICanvas newCanvas(IntPtr pixels, int width, int height);
        public abstract ICanvas newCanvas(int[] pixels, int width, int height, bool makeCopy = false);
        public abstract ICanvas newCanvas(int width, int height, byte[] pixels, bool makeCopy = false);
        public abstract ICanvas newCanvas(IRenderTarget window);
        #endregion

        #region RENDER TARGET
#if Window
        public abstract IRenderTarget newRenderTarget(IRenderWindow window);
#endif
        #endregion

        #region FORM
#if NATIVE
        public abstract IForm newForm(int x, int y, int w, int h);
#endif
        #endregion

        #region OBJECT COLLECTION
        public abstract IObjCollection newObjectCollection(IWritable buffer);
        #endregion

        #region BUFFER COLLECTION
#if Advanced
        public abstract IBufferCollection newBufferCollection();
        public abstract IBufferCollection newBufferCollection(int capacity);
        public abstract IBufferCollection newBufferCollection(ISurface primary);
#endif
        #endregion

        #region RENDER INFO
        public abstract
#if Advanced
            IRenderInfo2 
#else
            IRenderInfo
#endif
            newRenderInfo(string shapeID);
        #endregion

        #region POLY FILL
        public virtual IPolyFill newPolyFill() =>
            new PolyFill();
        #endregion

        #region BRUSH - PEN
        public abstract IBrush newBrush(BrushStyle style, int width, int height);
        public abstract ITextureBrush newBrush(IntPtr data, int width, int height);

        public virtual IReadable newPen(int color) =>
            Pen.CreateInstance(color);

        public abstract IReadable ToPen(IPenContext context, int? w = null, int? h = null);
        #endregion

        #region FONT - GLYPH - GLYPHSLOT - TEXT - RENDERER
        public abstract IFont newFont(Stream fontStream, int fontSize);
        #endregion

        #region IMAGE PROCESSOR
        public virtual IImageProcessor newImageProcessor() =>
            new StbImageProcessor();
        #endregion

        #region CONVERTER
        public virtual IConverter newConverter() =>
            new Converter();
        #endregion

        #region PEN STORE
        /// <summary>
        /// Gets currently attached Pen store in GWS.
        /// </summary>
        public virtual IPens newPenStore() =>
            new Store(this);
        #endregion

        #region SHAPE PARSER
        public abstract IShapeParser newShapeParser();
        #endregion

        #region LINE
        public abstract ILine newLine(float x1, float y1, float x2, float y2);
        #endregion

        #region CURVE
        public abstract ICurve newCurve(float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, CurveType type = 0, Rotation rotation = default(Rotation), VectorF scale = default(VectorF));

        public abstract ICurve newCurve(IConic conic, VectorF[] pieTriangle, CurveType type);
        #endregion

        #region CONIC
        public abstract IConic newConic(Rotation rotation, float x, float y, float width, float height, float startAngle = 0, float endAngle = 0, float tiltAngle = 0);
        #endregion

        #region TETRAGON
        public abstract ITetragon newTetragon(VectorF first, VectorF second, VectorF third, VectorF fourth);
        #endregion

        #region BEZIER
        public abstract IBezier newBezier(BezierType type, ICollection<float> pointValues, IList<VectorF> points);
        #endregion

        #region TRIANGLE
        public abstract ITriangle newTriangle(float x1, float y1, float x2, float y2, float x3, float y3);
        #endregion

        #region BOX
        public abstract IBox newBox(int x, int y, int width, int height);
        #endregion

        #region BOXF
        public abstract IBoxF newBoxF(float x, float y, float width, float height);
        #endregion

        #region SHAPE
        public abstract IShape newShape(IEnumerable<VectorF> shape, string name);
        #endregion

        #region GLYPHS
        public abstract IGlyphs newGlyphs(string text, RectangleF area, IList<IGlyph> resultGlyphs, float minHBY);
        #endregion

        #region ROUNDBOX
        public abstract IRoundBox newRoundBox(float x, float y, float w, float h, float cornerRadius, bool positiveLocation = false);
        #endregion

        #region POLYGON
        public abstract IPolygon newPolygon(IList<VectorF> polyPoints);
        #endregion

        #region TEXT
        public abstract IText newText(IList<IGlyph> glyphs, ITextStyle drawStyle = null, int? dstX = null, int? dstY = null);

        public abstract IText newText(IFont font, string text, int dstX, int dstY, ITextStyle drawStyle = null);
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
    }

    partial class _Factory
    {
        class Store : _Pens
        {
            _Factory Instance;
            public Store(_Factory factory)
            {
                Instance = factory;
            }
            public override IReadable ToPen(IPenContext context, int? w = null, int? h = null) =>
                Instance.ToPen(context, w, h);

            protected override bool IsDisposed => Instance.isDisposed;
        }
    }
}
