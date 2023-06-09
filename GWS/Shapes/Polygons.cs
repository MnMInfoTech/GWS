/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region IPOLYGON-COLLECTION
    public interface IPolygonCollection : IShape, IPolygonalF, 
      IProxyCollection<IPolygonalF, VectorF[]>
    {
        IParameter[] Parameters { get; set; }
    }
    #endregion

    #region POLYGON-COLLECTION
    public class PolygonCollection: _ProxyCollection<IPolygonalF, VectorF[]>, IPolygonCollection, IExResizable
    {
        IExBoundary boundary = (IExBoundary)Factory.newBoundary();
        bool isOriginBased;

        public PolygonCollection()
        {
        }
        public PolygonCollection(int capacity): base(capacity)
        {
        }
        public PolygonCollection(IEnumerable<IPolygonalF> items, 
            params IParameter[] settings): base(items)
        {
            Parameters = settings;
        }
        public bool Valid => true;
        public IParameter[] Parameters { get; set; }
        bool IOriginCompatible.IsOriginBased => isOriginBased;
        int IPoint.X => boundary.X;
        int IPoint.Y => boundary.Y;
        int ISize.Width => boundary.Width;
        int ISize.Height => boundary.Height;
        public override VectorF[] this[IPolygonalF item] => item.GetPoints();
        protected override IPolygonalF newItemFrom(VectorF[] subItem)
        {
            var f = new Polygon(subItem);
            boundary.Update(new int[] { f.X, f.Y, f.X + f.Width, f.Y + f.Height });
            return f;
        }

        #region RESIZE
        object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            var iw = ((ISize)this).Width;
            var ih = ((ISize)this).Height;

            if
            (
               (w == iw && h == ih) ||
               (w == 0 && h == 0)
            )
            {
                return this;
            }

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

            if (SizeOnlyToFit && iw > w && ih > h)
                return this;

            if (SizeOnlyToFit)
            {
                if (w < iw)
                    w = iw;
                if (h < ih)
                    h = ih;
            }
            for (int i = 0; i < Count; i++)
            {
                if(this[i] is IExResizable)
                    this[i] = (IPolygonalF)((IExResizable)this[i]).Resize(w, h, out _, resizeCommand);
            }
            success = true;
            return this;
        }
        #endregion

        #region PERIMETER
        VectorF[] IPolygonal<VectorF>.GetPoints()
        {
            var items = new PrimitiveList<VectorF>();
            foreach (var item in this)
            {
                items.AddRange(item.GetPoints());
                items.Add(VectorF.Break);
            }
            var arrays = ((IArrayHolder<VectorF>)items).Data;
            return arrays; 
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            var w = boundary.Width;
            var h = boundary.Height;
            boundary.Clear();
            boundary.Update(new int[] { 0, 0, w, h });

            for (int i = 0; i < Count; i++)
            {
                this[i] = (IPolygon)((IPolygon) this[i]).GetOriginBasedVersion();
            }
            isOriginBased = true;
            return this;
        }
        #endregion

    }
    #endregion
}
#endif
