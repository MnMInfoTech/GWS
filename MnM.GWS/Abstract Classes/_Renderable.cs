using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public abstract class _Renderable: IRenderable, IRecentlyDrawn
    {
        public string ID { get; protected set; }
        public virtual RectangleF Bounds { get; }
        public Rectangle RecentlyDrawn { get; set; }
    }
}
