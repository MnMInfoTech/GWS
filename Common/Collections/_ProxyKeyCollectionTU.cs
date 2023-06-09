/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IPROXYKEY-COLLECTION<TKEY, TITEM>
    public interface IProxyKeyCollection<TKey, TItem> :
        IKeyCollection<TKey, TItem>, IProxyCollection<TItem, TKey>
    {
        new TItem this[int index] { get; }

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

    public abstract class _ProxyKeyCollection<TKey, TItem> : 
        _KeyCollection<TKey, TItem>, IProxyKeyCollection<TKey, TItem>
    {
        #region CONSTRUCTORS
        public _ProxyKeyCollection() :
            base()
        {
            SubItems = new ReadOnlyList(this);
        }
        public _ProxyKeyCollection(int capacity) :
            base(capacity)
        {
            SubItems = new ReadOnlyList(this);
        }
        #endregion

        #region PROPERTIES
        public IReadOnlyList<TKey> SubItems { get; private set; }
        public TKey this[TItem item] => KeyOf(item);
        #endregion

        #region NEW ITEM FROM KEY
        protected abstract TItem newItemFrom(TKey subItem);
        #endregion

        #region ADD
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TKey subItem)
        {
            _Add(newItemFrom(subItem));
        }
       
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<TKey> subItems)
        {
            if (subItems == null)
                return;
            _AddRange(subItems.Select(x => newItemFrom(x)));
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(params TKey[] subItems) =>
            AddRange((IEnumerable<TKey>)subItems);
        #endregion

        #region INSERT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int position, TKey subItem)
        {
            _Insert(position, newItemFrom(subItem));
        }
        #endregion

        #region INSERT RANGE
        public void InsertRange(int position, IEnumerable<TKey> items)
        {
            if (items == null)
                return;
            _InsertRange(position, items.Select(x => newItemFrom(x)));
        }

        public void InsertRange(int position, params TKey[] items)=>
            InsertRange(position, (IEnumerable<TKey>)items);
        #endregion

        #region REMOVE
        public bool Remove(TKey subItem) =>
            RemoveByKey(subItem);
        #endregion

        #region REPLACE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Replace(TKey oldSubItem, TKey newSubItem)
        {
            if (!KeyItems.ContainsKey(oldSubItem))
                return false;
            var index = KeyItems[oldSubItem];
            ReplaceAt(index, newItemFrom(newSubItem));
            return true;
        }
        #endregion

        #region CONTAINS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TKey item)
        {
            return KeyItems.ContainsKey(item);
        }
        #endregion

        #region INDEX OF
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(TKey key)
        {
            if (!KeyItems.TryGetValue(key, out int index))
                return -1;
            return index;
        }
        #endregion

        #region CHILD CLASS
        class ReadOnlyList : ReadOnlyList<TKey>
        {
            _ProxyKeyCollection<TKey, TItem> Items;
            public ReadOnlyList(_ProxyKeyCollection<TKey, TItem> items)
            {
                Items = items;
            }

            public override TKey this[int index]
            {
                get => Items.KeyOf(Items.Data[index]);
                protected set { }
            } 
            public override int Count => Items.Count;
        }
        #endregion
    }
}
