/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS && Window)
using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public abstract class _Sound : ISound
    {
        public bool Loop { get; set; }
        public abstract void Load(string file);
        public abstract bool Play();
        public bool IsDisposed { get; protected set; }

        public abstract void Pause();
        public abstract void Stop();

        public abstract void Dispose();
        public abstract void Quit();
    }
}
#endif
