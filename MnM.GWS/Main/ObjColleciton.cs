using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public sealed class ObjCollection: _ObjCollection<IGraphics>
    {
        Dictionary<uint, Shape> items;
        public ObjCollection(IGraphics graphics):
            base(graphics)
        {
            items = new Dictionary<uint, Shape>(100);
        }
        protected override Dictionary<uint, Shape> Items => items;
    }
}
