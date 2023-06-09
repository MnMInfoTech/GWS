/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IPROXY-COLLECTION
    public interface IProxyCollection<TItem, TSubItem> : IMiniCollection<TItem>, IAddRange<TItem, TSubItem>, IInsertRange<TItem, TSubItem>, IReplace<TSubItem>
    {
        IReadOnlyList<TSubItem> SubItems { get; }
      
        new TItem this[int index] { get; set; }

        TSubItem this[TItem item] { get; }

        /// <summary>
        /// Adds the given subitem at the end of this collection.
        /// </summary>
        /// <param name="subItem">Item to add.</param>
        void Add(TSubItem subItem);

        /// <summary>
        /// Removes the given subitem from this collection.
        /// </summary>
        /// <param name="subItem">Item to remove.</param>
        /// <returns>True if operation is successful otherwise not.</returns>
        bool Remove(TSubItem subItem);

        /// <summary>
        /// Finds index of the specified item in this colleciton.
        /// </summary>
        /// <param name="subItem">Item to find index for.</param>
        /// <returns>Index of the item if found otherwise -1.</returns>
        int IndexOf(TSubItem subItem);

        /// <summary>
        /// Checks if the specified item exists in this collection or not.
        /// </summary>
        /// <param name="subItem">Item to find index for.</param>
        /// <returns>True if the exis otherwise false.</returns>
        bool Contains(TSubItem subItem);

        /// <summary>
        /// Inserts the specified item at given position in this colleciton.
        /// </summary>
        /// <param name="position">Position to insert item at.</param>
        /// <param name="subItem">ITem to insert.</param>
        void Insert(int position, TSubItem subItem);

        /// <summary>
        /// Swaps the givem item with exisitng item in this collection.
        /// </summary>
        /// <param name="oldSubItem">Existing item to perform swap operation.</param>
        /// <param name="newSubItem">New item to perform swap operation.</param>
        /// <returns>True if operation is successful otherwise not.</returns>
        bool Swap(TSubItem oldSubItem, TSubItem newSubItem);

        /// <summary>
        /// Changes ZOrder of given shape in this collection.
        /// </summary>
        /// <param name="item">Item which to change index for.</param>
        /// <param name="newIndex">New index for the item.</param>
        bool Relocate(TSubItem item, int newIndex);
    }
    #endregion

    public abstract class _ProxyCollection<TItem, TSubItem>: PrimitiveList<TItem>, IProxyCollection<TItem, TSubItem>
    {
        #region CONSTRUCTORS
        public _ProxyCollection():
            base()
        {
            SubItems = new ReadOnlyList(this);
        }
        public _ProxyCollection(int capacity) :
            base(capacity)
        {
            SubItems = new ReadOnlyList(this);
        }
        public _ProxyCollection(IEnumerable<TSubItem> collection) : this()
        {
            SubItems = new ReadOnlyList(this);
            AddRange(collection);
        }
        public _ProxyCollection(IEnumerable<TItem> collection) : 
            base(collection)
        {
            SubItems = new ReadOnlyList(this);
        }
        #endregion

        #region PROPERTIES
        public IReadOnlyList<TSubItem> SubItems { get; private set; }
        public abstract TSubItem this[TItem item] { get; }
        #endregion

        #region ADD
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Add(TSubItem subItem) =>
            Add(newItemFrom(subItem));
      
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void AddRange(IEnumerable<TSubItem> subItems) =>
            AddRange(subItems.Select(x => newItemFrom(x)));

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(params TSubItem[] subItems) =>
            AddRange((IEnumerable<TSubItem>)subItems);
        #endregion

        #region INSERT RANGE
        public virtual void InsertRange(int position, IEnumerable<TSubItem> subItems)
        {
            if (subItems == null) return;
            var _items = subItems.Select(x => newItemFrom(x));

            int sCount = _items.Count();

            if (position > Length)
                position = Length;

            var temp = new TItem[Math.Max(Data.Length, (Length + sCount) * 2)];
            Array.Copy(Data, 0, temp, 0, position);
            int j = position;

            var array = temp;
            foreach (var item in _items)
                array[j++] = item;

            Array.Copy(Data, position, temp, position + sCount, Length - position);
            Data = temp;
            Length += sCount;
        }
        public void InsertRange(int position, params TSubItem[] subItems) =>
            InsertRange(position,(IEnumerable<TSubItem>)subItems);
        #endregion

        #region SWAP
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Swap(TSubItem oldItem, TSubItem newItem)
        {
            int i = IndexOf(oldItem);
            if (i == -1)
                return false;
            int j = IndexOf(oldItem);
            if (j == -1)
                return false;
            var item = Data[i];
            Data[i] = Data[j];
            Data[j] = item;
            return true;
        }
        #endregion

        #region CHANGE INDEX
        public bool MoveByKey(TSubItem item, int newIndex)
        {
            var oldIndex = IndexOf(item);
           return Relocate(oldIndex, newIndex);
        }
        #endregion

        #region INSERT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Insert(int position, TSubItem subItem)
        {
            Insert(position, newItemFrom(subItem));
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

        #region REMOVE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Remove(TSubItem subItem)
        {
            var index = Data.FirstMatchIndex(x => subItem.Equals(this[x]), end: Length);
            if (index == -1)
                return false;
            return RemoveAt(index); 
        }
        #endregion

        #region RELOCATE
        public bool Relocate(TSubItem item, int newIndex)
        {
            var i = IndexOf(item);
            if (i == -1)
                return false;

            ReplaceAt(newIndex, Data[i]);
            return true;
        }
        #endregion

        #region NEW ITEM FROM SUB ITEM
        protected abstract TItem newItemFrom(TSubItem subItem);
        #endregion

        #region REPLACE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Replace(TSubItem oldSubItem, TSubItem newSubItem)
        {
            var i = IndexOf(oldSubItem);
            if (i == -1)
                return false;
            int j = IndexOf(newSubItem);
            TItem item = j == -1 ? newItemFrom(newSubItem) : Data[j];
            ReplaceAt(i, item);
            return true;
        }
        #endregion

        class ReadOnlyList : ReadOnlyList<TSubItem>
        {
            _ProxyCollection<TItem, TSubItem> Items;
            public ReadOnlyList(_ProxyCollection<TItem, TSubItem> items)
            {
                Items = items;
            }

            public override TSubItem this[int index]
            {
                get => Items [Items.Data[index]];
                protected set { }
            }
            public override int Count => 
                Items.Count;
        }
    }
}
