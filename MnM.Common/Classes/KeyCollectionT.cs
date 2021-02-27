using System;
/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class KeyCollection<TKey, TItem> : MiniCollection<TItem>, IKeyCollection<TKey, TItem>
    {
        Dictionary<TKey, int> Keys;

        #region CONSTRUCTORS
        public KeyCollection()
        {
            Keys = new Dictionary<TKey, int>();
        }
        public KeyCollection(int capacity)
        {
            Keys = new Dictionary<TKey, int>(capacity);
        }
        #endregion

        #region PROPERTIES
        public override TItem this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();
                return iData[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();
                Keys.Remove(Key(iData[index]));
                iData[index] = value;
                Keys[Key(value)] = index;
            }
        }
        #endregion

        #region ADD
        public override void Add(TItem item)
        {
            Keys[Key(item)] = Count;
            base.Add(item);
        }
        public override void AddRange(IEnumerable<TItem> items)
        {
            int count = Count;
            base.AddRange(items);
            for (int i = count; i < Count; i++)
            {
                Keys[Key(iData[i])] = i;
            }
        }
        #endregion

        #region REMOVE
        public override bool Remove(TItem item)
        {
            if (!Keys.ContainsKey(Key(item)))
                return false;
            var index = Keys[Key(item)];
            base.RemoveAt(index);
            Keys.Remove(Key(item));
            for (int i = index; i < Count; i++)
                Keys[Key(iData[i])] = i;
            return true;
        }
        public override void RemoveAt(int index)
        {
            Keys.Remove(Key(iData[index]));
            base.RemoveAt(index);
            for (int i = index; i < Count; i++)
                Keys[Key(iData[i])] = i;
        }
        #endregion

        #region INDEX OF
        public override int IndexOf(TItem item)
        {
            if (!Keys.ContainsKey(Key(item)))
                return -1;
            return Keys[Key(item)];
        }
        #endregion

        #region CONTAINS
        public override bool Contains(TItem item)
        {
            return Keys.ContainsKey(Key(item));
        }
        #endregion

        #region CLEAR
        public override void Clear()
        {
            base.Clear();
            Keys.Clear();
        }
        #endregion

        #region GET KEY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TKey Key(TItem item);
        #endregion

        #region FIND ITEM
        public TItem FindItem(TKey key)
        {
            if (!Keys.ContainsKey(key))
                return default(TItem);
           return iData[Keys[key]];
        }
        #endregion
    }
}
