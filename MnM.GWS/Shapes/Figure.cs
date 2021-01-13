/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
#if GWS || Window
using System.Collections;
using System.Collections.Generic;

#if Standard
namespace MnM.GWS.Standard
#elif Advanced
namespace MnM.GWS.Advanced
#else
namespace MnM.GWS
#endif
{
#if AllHidden
    partial class _Factory
    {
#else
    public
#endif
    partial struct Figure : IFigure
    {
        #region VARIABLES
        public readonly IEnumerable<VectorF> Points;
        public readonly string Name;
        #endregion

        #region CONSTRUCTORS
        public Figure(IEnumerable<VectorF> points, string shapeType)
        {
            Points = points;
            Name = shapeType;
            ID = (points is IID) ? (points as IID).ID :
                (Name?? "Shape").NewID(); ;
        }
        public Figure(IFigure shape): this()
        {
            Points = shape;
            Name = shape.Name;
            ID = shape.ID;
        }
        #endregion

        #region ISHAPE
        public string ID { get; private set; }
        public IEnumerable<VectorF> Perimeter()
        {
            if (Points is IFigurable)
                return ((IFigurable)Points).Perimeter();
            return this;
        }
        IEnumerator<VectorF> IEnumerable<VectorF>.GetEnumerator() => Points.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Points.GetEnumerator();
        string IRecognizable.Name => Name;
        #endregion
    }
#if AllHidden
    }
#endif
}
#endif
