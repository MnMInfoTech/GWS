/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;

namespace MnM.GWS
{
    public class Instance : Factory
    {
#if (GWS || Window)
        protected sealed override void newCanvas(ref ICanvas canvas, int width, int height) =>
           canvas = new Canvas(width, height);

        protected sealed override void newCanvas(ref ICanvas canvas, IntPtr pixels, int width, int height, bool switchRBChannel = false) =>
            canvas = new Canvas(pixels, width, height, switchRBChannel);

        protected sealed override void newCanvas(ref ICanvas canvas, int[] pixels, int width, int height, bool makeCopy = false) =>
            canvas = new Canvas(pixels, width, height, makeCopy);

        protected sealed override void newCanvas(ref ICanvas canvas, int width, int height, byte[] pixels, bool switchRBChannel = false) =>
            canvas = new Canvas(width, height, pixels, switchRBChannel);

        protected sealed override void newView(ref IView view, IRenderTarget window, bool isMultiWindow = false) =>
            view = new View(window, isMultiWindow);
#endif
    }
}
