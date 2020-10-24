/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
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

        #region OBJECT COLLECTION
        public abstract IObjCollection newObjectCollection(IBuffer buffer);
        #endregion

        #region BUFFER COLLECTION
#if Advanced
        public abstract IBufferCollection newBufferCollection();
        public abstract IBufferCollection newBufferCollection(int capacity);
        public abstract IBufferCollection newBufferCollection(ICanvas primary);
#endif
#endregion

        #region POLY FILL
        public virtual IPolyFill newPolyFill() =>
            new PolyFill();
        #endregion

        #region BRUSH - PEN
        public abstract IBrush newBrush(BrushStyle style, int width, int height);
        public abstract ITextureBrush newBrush(IntPtr data, int width, int height);

        public virtual IPen newPen(int color) =>
            Pen.CreateInstance(color);

        public abstract IPen ToPen(IReadContext context, int? w = null, int? h = null);
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
        public virtual IShapeParser newShapeParser() =>
            new ShapeParser();
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
            public override IPen ToPen(IReadContext context, int? w = null, int? h = null) =>
                Instance.ToPen(context, w, h);

            protected override bool IsDisposed => Instance.isDisposed;
        }
    }
}
