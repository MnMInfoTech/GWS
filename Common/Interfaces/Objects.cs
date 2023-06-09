/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Collections.Generic;
#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif

namespace MnM.GWS
{
    #region IOBJECT
    /// <summary>
    /// Marker interface
    /// </summary>
    public interface IObject : IPoint, ISize, IValid
    { }
    #endregion

    #region ISHAPE
    /// <summary>
    /// Represents an object which has a place in GWS object eco system.
    /// This is an entry point interface to be in the GWS object eco system.
    /// A minimum required interface to inherit in order to make your control work in the GWS.
    /// It must have an ID and area to work upon.
    /// </summary>
    public interface IShape : IObject, IOriginCompatible
    { }
    #endregion

    #region IFIGURE
    public interface IFigure : IPolygonalF, IShape
    { }
    #endregion

    #region IITEM
    public partial interface IItem : IObject, IOriginCompatible, IHitTestable, IPosition<gint>, IName
    { }
    #endregion

    #region IEx ITEM
    internal partial interface IExItem : IItem, IExPosition<gint>, IExDraw, IName2
    {
        IObject UnderlyingItem { get; }
    }
    #endregion

    #region IPOPUP-ITEM
    public interface ITextItem : IShape, IPropertyBagHolder, ILocationHolder, IValue<string>, ITextHolder
    {
        new string Value { get; set; }
        bool Initialized { get; }
        IRectangle Measure(IFont font = null, IEnumerable<IParameter> parameters = null);
    }
    #endregion
}
