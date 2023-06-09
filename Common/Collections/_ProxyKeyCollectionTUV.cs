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
    #region IPROXYKEY-COLLECTION<TKEY, TITEM, TSUBITEM>
    public interface IProxyKeyCollection<TKey, TItem, TSubItem> :
      IKeyCollection<TKey, TItem>, IProxyCollection<TItem, TSubItem>
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

    public abstract class _ProxyKeyCollection<TKey, TItem, TSubItem> : _KeyCollection<TKey, TItem>, 
        IProxyKeyCollection<TKey, TItem, TSubItem>
    {
        #region CONSTRUCTORS
        public _ProxyKeyCollection() :
            base()
        {
            SubItems = new _SubItems(this);
        }
        public _ProxyKeyCollection(int capacity) :
            base(capacity)
        {
            SubItems = new _SubItems(this);
        }
        #endregion

        #region PROPERTIES
        public IReadOnlyList<TSubItem> SubItems { get; private set; }
        public abstract TSubItem this[TItem item] { get; }
        #endregion

        #region NEW ITEM FROM SUB ITEM
        protected abstract TItem newItemFrom(TSubItem subItem);
        #endregion

        #region ADD
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(TSubItem subItem) =>
            Add(newItemFrom(subItem));

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<TSubItem> subItems) =>
            AddRange(subItems.Select(x => newItemFrom(x)));

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(params TSubItem[] subItems) =>
            AddRange(subItems as IEnumerable<TSubItem>);
        #endregion

        #region INSERT RANGE
        public void InsertRange(int position, IEnumerable<TSubItem> subItems) =>
            AddRange(subItems.Select(x => newItemFrom(x)));

        public void InsertRange(int position, params TSubItem[] subItems) =>
            AddRange(subItems.Select(x => newItemFrom(x)));
        #endregion

        #region INSERT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Insert(int position, TSubItem subItem)
        {
            base.Insert(position, newItemFrom(subItem));
        }
        #endregion

        #region SWAP
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Swap(TSubItem oldSubItem, TSubItem newSubItem)
        {
            var i = IndexOf(oldSubItem);
            if (i == -1)
                return false;
            var j = IndexOf(newSubItem);
            if (j == -1)
                return false;
            var key1 = KeyOf(Data[i]);
            var key2 = KeyOf(Data[j]);

            Swap2(i, key1, j, key2);
            return true;
        }
        #endregion

        #region REMOVE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveByKey(TSubItem subItem)
        {
            var index = IndexOf(subItem);
            if (index == -1)
                return false;
            return RemoveAt(index);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TSubItem subItem) =>
            RemoveByKey(subItem);
        #endregion

        #region MOVE
        public bool Relocate(TSubItem item, int newIndex)
        {
            var i = IndexOf(item);
            if (i == -1)
                return false;
            ReplaceAt(newIndex, Data[i]);
            return true;
        }
        #endregion

        #region REPLACE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Replace(TSubItem oldSubItem, TSubItem newSubItem)
        {
            var i = IndexOf(oldSubItem);
            if (i == -1)
                return false;
            int j = IndexOf(newSubItem);
            TItem item = j != -1 ? Data[j] : newItemFrom(newSubItem);
            ReplaceAt(i, item);
            return true;
        }
        #endregion

        #region INDEX OF
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int IndexOf(TSubItem subItem)
        {
            return Data.FirstMatchIndex(x => subItem.Equals(this[x]), end: Length);
        }
        #endregion

        #region CONTAINS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Contains(TSubItem subItem)
        {
            return Data.FirstMatchIndex(x => subItem.Equals(this[x]), end: Length) != -1;
        }
        #endregion

        #region CHILD CLASS
        class _SubItems : ReadOnlyList<TSubItem>
        {
            _ProxyKeyCollection<TKey, TItem, TSubItem> Items;
            public _SubItems(_ProxyKeyCollection<TKey, TItem, TSubItem> items)
            {
                Items = items;
            }

            public override TSubItem this[int index]
            {
                get => Items[Items.Data[index]];
                protected set { }
            }
            public override int Count => Items.Count;
        }
        #endregion
    }
}
