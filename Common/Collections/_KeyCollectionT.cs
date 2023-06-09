/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Collections.Generic;

namespace MnM.GWS
{
    #region IKEYCOLLECITON
    public interface IKeyCollection<TKey, TItem> : IMiniKeyCollection<TKey, TItem>, IInsertRange<TItem>
    {
        /// <summary>
        /// Clears all items in this collection.
        /// </summary>
        new void Clear();

        /// <summary>
        /// Remvoes item located at given index in this collection.
        /// </summary>
        /// <param name="index">Index of item to be removed.</param>
        new bool RemoveAt(int index);
    }
    #endregion

    public abstract class _KeyCollection<TKey, TItem> : _MiniKeyCollection<TKey, TItem>,
        IKeyCollection<TKey, TItem>
    {        
        #region CONSTRUCTORS
        protected _KeyCollection():
            base()
        {
        }
        protected _KeyCollection(int capacity):
            base(capacity)
        {
        }
        #endregion

        #region INSERT
        public void InsertRange(int position, IEnumerable<TItem> items) =>
            _InsertRange(position, items);
        public void InsertRange(int position, params TItem[] items) =>
            InsertRange(position, (IEnumerable<TItem>)items);
        #endregion

        #region INDEX OF
        public int IndexOf(TItem item)
        {
            if (item == null)
                return -1;
            int INDEX = -1;
            var KEY = KeyOf(item);
            if (KEY == null)
                return INDEX;
            if (KeyItems.TryGetValue(KEY, out INDEX))
                return INDEX;
            return INDEX;
        }
        #endregion

        #region MAKE ZERO LENGTH
        public virtual void MakeZeroLength()
        {
            Length = 0;
            Data = new TItem[Length];
        }
        #endregion
    }
}
