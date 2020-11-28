using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public abstract class _Renderable: IRenderable, IRecentlyDrawn
    {
        protected string id;
        public string ID
        {
            get
            {
                if (id == null)
                {
                    if(this is IRecognizable)
                       id = ((IRecognizable)this).NewID();
                    else
                        id = GetType().Name.NewID();
                }
                return id;
            }
        }
        public abstract RectangleF Bounds { get; }
        public virtual Rectangle RecentlyDrawn { get; set; }
    }
}
