/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS.Standard
{
    public partial class NativeFactory : _Factory
    {
#if !(Standard || Advanced)
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
        public override IPen ToPen(IReadContext context, int? w = null, int? h = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region OBJECT COLLECTION
        public override IObjCollection newObjectCollection(ISurface buffer)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IObjCollection"));
        }
        #endregion
#endif
    }
}
