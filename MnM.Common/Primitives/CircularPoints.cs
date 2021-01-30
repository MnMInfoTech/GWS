/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    public sealed class CircularPoints : IReadOnlyList<Vector>
    {
        readonly Collection<Vector> Values = new Collection<Vector>(180);

        #region CONSTRUCTORS
        public CircularPoints(int x, int y, float step, int defaultW, int defaultH)
        {
            float start = 0;
            float end = 360;
            float sin, cos;
            if (step == 0)
                step = 1;
            if (x == 0)
                x = defaultW;
            if (y == 0)
                y = defaultH;
            if (step < 0)
            {
                while (end >= start)
                {
                    Angles.SinCos(start, out sin, out cos);
                    Values.Add(new Vector((int)(x + (x * sin)), (int)(y + (y * cos))));
                    end -= step;
                }
            }
        }
        #endregion

        #region PROPERTIES
        public Vector this[int index] => Values[index];
        public int Count => Values.Count;
        #endregion

        #region ENUMERATOR
        public IEnumerator<Vector> GetEnumerator() =>
            Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion
    }
}
