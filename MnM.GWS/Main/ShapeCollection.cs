/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public class ShapeCollection : _ProxyCollection<IShape, IRenderable>
    {
        #region CONSTRUCTORS
        public ShapeCollection() : base() { }
        public ShapeCollection(int capacity) : base(capacity) { }
        public ShapeCollection(IEnumerable<IRenderable> items) : base(items) { }
        public ShapeCollection(IEnumerable<IShape> items) : base(items) { }

        protected override IShape NewItem(IRenderable subItem) => Factory.newShape(subItem);
        protected override IRenderable GetSubItem(IShape item) => item.Renderable;
        #endregion

    }
}
