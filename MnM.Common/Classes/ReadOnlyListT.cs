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
    public class ReadOnlyList<T> : IReadOnlyList<T>
    {
        IList<T> items;

        public ReadOnlyList(IEnumerable<T> items)
        {
            if (items is IList<T>)
                this.items = items as IList<T>;
            else
            {
                this.items = items.ToArray();
            }
        }

        public T this[int index] => items[index];
        public int Count => items.Count;
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
    }
}
