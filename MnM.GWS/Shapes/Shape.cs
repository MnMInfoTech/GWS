/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    public struct Shape: IShape
    {
        #region VARIABLES
        public readonly IEnumerable<VectorF> Points;
        public readonly string Name;
        public readonly RectangleF Bounds;
        #endregion

        #region CONSTRUCTORS
        public Shape(IEnumerable<VectorF> points, string shapeType)
        {
            Points = points;
            Name = shapeType;
            Bounds = points.ToArea();
            ID = (points is IID) ? (points as IID).ID :
                "Shape".NewID(); ;
        }
        public Shape(IShape shape): this()
        {
            Points = shape;
            Name = shape.Name;
            Bounds = shape.Bounds;
            ID = shape.ID;
        }
        #endregion

        #region ISHAPE
        public string ID { get; private set; }
        public IEnumerable<VectorF> Figure() => this;
        IEnumerator<VectorF> IEnumerable<VectorF>.GetEnumerator() => Points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Points.GetEnumerator();
        RectangleF IBoundsF.Bounds => Bounds;
        string IRecognizable.Name => Name;
        #endregion
    }
}
