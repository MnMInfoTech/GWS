/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
#if HideGWSObjects
    partial class NativeFactory
    {
#else
    public
#endif
    partial struct Figure : IFigure
    {
        #region VARIABLES
        public readonly IEnumerable<VectorF> Points;
        public readonly string TypeName;
        uint id;
        #endregion

        #region CONSTRUCTORS
        public Figure(IEnumerable<VectorF> points, string shapeType): this()
        {
            Points = points;
            TypeName = shapeType;
            if (points is IID)
                id = ((IID)points).ID;
            Name = TypeName.NewName();
        }
        public Figure(IFigure shape): this()
        {
            Points = shape;
            TypeName = shape.TypeName;
            id = shape.ID;
            Name = TypeName.NewName();
        }
        #endregion

        #region ISHAPE
        public uint ID
        {
            get
            {
                if (id == 0)
                    id = this.NewID();
                return id;
            }
        }
        public IEnumerable<VectorF> Perimeter()
        {
            if (Points is IFigurable)
                return ((IFigurable)Points).Perimeter();
            return this;
        }
        IEnumerator<VectorF> IEnumerable<VectorF>.GetEnumerator() => Points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Points.GetEnumerator();
        string IRecognizable.TypeName => TypeName;
        public string Name { get; private set; }
        #endregion
    }
#if HideGWSObjects
    }
#endif
}
#endif
