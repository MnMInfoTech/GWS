/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
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
            throw new NotImplementedException(string.Format(goforStandard, "ISurface, ICanvas, IBrush, IObjCollection"));
        }
        #endregion

        #region OBJECT COLLECTION
        public override IObjCollection newObjectCollection(IBuffer buffer)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IObjCollection"));
        }
        #endregion

        #region FONT
        public override IFont newFont(Stream fontStream, int fontSize)
        {
            throw new NotImplementedException(string.Format(goforStandard, "IFont"));
        }
        #endregion
    }
#endif
}
