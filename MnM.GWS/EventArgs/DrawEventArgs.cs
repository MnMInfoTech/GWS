/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window
using System;

namespace MnM.GWS
{
    public class DrawEventArgs : EventArgs, IDrawEventArgs
    {
        public IGraphics Graphics { get; internal set; }
        internal DrawEventArgs() { }

        public DrawEventArgs(IGraphics graphics)
        {
            Graphics = graphics;
        }
        void IDisposable.Dispose()
        {
            Graphics = null;
        }
    }
}
#endif
