/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
    public class DrawEventArgs : EventArgs, IDrawEventArgs
    {
        public ISurface Surface { get; internal set; }
        internal DrawEventArgs() { }

        public DrawEventArgs(ISurface graphics)
        {
            Surface = graphics;
        }
        void IDisposable.Dispose()
        {
            Surface = null;
        }
    }
}
