/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    public class ReadOnlyList<T, U> : IReadOnlyList<U> where T : U
    {
        readonly IReadOnlyList<T> Items;
        public ReadOnlyList(IEnumerable<T> items)
        {
            if (items is IReadOnlyList<T>)
                Items = (IReadOnlyList<T>)items;
            else
                Items = Items.ToArray();
        }

        public U this[int index] => Items[index];
        public int Count => Items.Count;
        public IEnumerator<U> GetEnumerator()
        {
            for (int i = 0; i < Items.Count; i++)
                yield return Items[i];
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
