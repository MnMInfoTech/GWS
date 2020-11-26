/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.IO;

//The following are just a namespaces allocation statements.
namespace MnM.GWS.Standard { }
namespace MnM.GWS.Advanced { }


#if Standard
namespace MnM.GWS.Standard
#elif Advanced
namespace MnM.GWS.Advanced
#else
namespace MnM.GWS
#endif
{
    /*Please remove this file when you use either standard or advaced version of GWS implementation
     * This is a dummy file just tells you to either obtain standrard or advanced version of GWS from M&M Info-Tech or
     * to implement your own version.
    */
#if !(Standard || Advanced)
    public partial class NativeFactory : _Factory
    {
        static string goforStandard = @"
            Implement your own version of {0} interface and supply instance here!. Alteranatively, you can download fully implemented standard version from http://mnminfotech.co.uk.";

        public static readonly IFactory Instance = new NativeFactory();
        protected NativeFactory() { }

        #region SURFACE
        public override ISurface newSurface(int width, int height)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ISurface"));
        }
        public override unsafe ISurface newSurface(IntPtr pixels, int width, int height)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ISurface"));
        }
        public override ISurface newSurface(int[] pixels, int width, int height, bool makeCopy = false)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ISurface"));
        }
        public override ISurface newSurface(int width, int height, byte[] pixels, bool makeCopy = false)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ISurface"));
        }
        #endregion

        #region CANVAS
        public override ICanvas newCanvas(int width, int height)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ICanvas"));
        }
        public override unsafe ICanvas newCanvas(IntPtr pixels, int width, int height)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ICanvas"));
        }
        public override ICanvas newCanvas(int[] pixels, int width, int height, bool makeCopy = false)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ICanvas"));
        }
        public override ICanvas newCanvas(int width, int height, byte[] pixels, bool makeCopy = false)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ICanvas"));
        }

        public override ICanvas newCanvas(IRenderTarget window)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ICanvas"));
        }
        #endregion

        #region RENDER TARGET
        public override IRenderTarget newRenderTarget(IRenderWindow window)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IRenderTarget"));
        }
        #endregion

        #region FORM
#if NATIVE
        public override IForm newForm(int x, int y, int w, int h)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IForm"));
        }
#endif
        #endregion

        #region RENDER INFO
        public override IRenderInfo newRenderInfo(string shapeID)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IRenderInfo"));
        }
        #endregion

        #region BRUSH
        public override IBrush newBrush(BrushStyle style, int width, int height)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IBrush"));
        }
        public override ITextureBrush newBrush(IntPtr data, int width, int height)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IBrush"));
        }
        #endregion

        #region TO PEN
        public override IReadable ToPen(IPenContext context, int? w = null, int? h = null)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ISurface, ICanvas, IBrush, IObjCollection"));
        }
        #endregion

        #region OBJECT COLLECTION
        public override IObjCollection newObjectCollection(IWritable buffer)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IObjCollection"));
        }
        #endregion

        #region FONT
        public override IFont newFont(Stream fontStream, int fontSize)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IFont"));
        }

        public override IShapeParser newShapeParser()
        {
            throw new NotImplementedException(string.Format(goforStandard, "IShapeParser"));
        }
        #endregion

        #region LINE
        public override ILine newLine(float x1, float y1, float x2, float y2)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ILine"));
        }
        #endregion

        #region CURVE
        public override ICurve newCurve(float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, CurveType type = 0, Rotation rotation = default(Rotation), VectorF scale = default(VectorF))
        {
            throw new NotImplementedException(string.Format(goforStandard, "ICurve"));
        }

        public override ICurve newCurve(IConic conic, VectorF[] pieTriangle, CurveType type)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ICurve"));
        }
        #endregion

        #region CONIC
        public override IConic newConic(Rotation rotation, float x, float y, float width, float height, 
            float startAngle = 0, float endAngle = 0, float tiltAngle = 0)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IConic"));
        }
        #endregion

        #region TETRAGON
        public override ITetragon newTetragon(VectorF first, VectorF second, VectorF third, VectorF fourth)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ITetragon"));
        }
        #endregion

        #region BEZIER
        public override IBezier newBezier(BezierType type, ICollection<float> pointValues, IList<VectorF> points)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IBezier"));
        }
        #endregion

        #region TRIANGLE
        public override ITriangle newTriangle(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            throw new NotImplementedException(string.Format(goforStandard, "ITriangle"));
        }
        #endregion

        #region BOX
        public override IBox newBox(int x, int y, int width, int height)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IBox"));
        }
        #endregion

        #region BOXF
        public override IBoxF newBoxF(float x, float y, float width, float height)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IBoxF"));
        }
        #endregion

        #region SHAPE
        public override IShape newShape(IEnumerable<VectorF> shape, string name)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IShape"));
        }
        #endregion

        #region GLYPHS
        public override IGlyphs newGlyphs(string text, RectangleF area, IList<IGlyph> resultGlyphs, float minHBY)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IGlyphs"));
        }
        #endregion

        #region ROUNDBOX
        public override IRoundBox newRoundBox(float x, float y, float w, float h, float cornerRadius, bool positiveLocation = false)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IRoundBox"));
        }
        #endregion

        #region POLYGON
        public override IPolygon newPolygon(IList<VectorF> polyPoints)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IPolygon"));
        }
        #endregion

        #region TEXT
        public override IText newText(IList<IGlyph> glyphs, ITextStyle drawStyle = null, int? dstX = null, int? dstY = null)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IText"));
        }

        public override IText newText(IFont font, string text, int dstX, int dstY, ITextStyle drawStyle = null)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IText"));
        }
        #endregion
    }
#endif
    }
