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
    public abstract class _ProxyCollection<TItem, TSubItem>: IProxyCollection<TItem, TSubItem>
    {
        #region VARIABLES
        TItem[] iData;
        int Length;
        int position = -1;
        #endregion

        #region CONSTRUCTORS
        public _ProxyCollection()
        {
            Length = 0;
            iData = new TItem[4];
        }
        public _ProxyCollection(int capacity) : this()
        {
            Capacity = capacity;
        }
        public _ProxyCollection(int capacity, bool exactCapacity) : this()
        {
            if (exactCapacity)
                Resize(capacity);
            else
                Capacity = capacity;
        }
        public _ProxyCollection(IEnumerable<TSubItem> collection) : this()
        {
            AddRange(collection);
        }
        public _ProxyCollection(IEnumerable<TItem> collection) : this()
        {
            if (collection == null) return;
            int sCount = collection.Count();

            if (iData.Length <= Count + sCount)
                Resize((Count + sCount) * 2);

            if (collection is TItem[])
            {
                Array.Copy((TItem[])(object)collection, 0, iData, Count, sCount);
                Length += sCount;
            }
            else
            {
                foreach (var item in collection)
                    iData[Length++] = item;
            }
        }
        #endregion

        #region PROPERTIES
        public int Capacity
        {
            get => iData.Length;
            set
            {
                if (value < Count)
                    return;
                else if (value > Count)
                    Resize(Math.Max(value, Count * 2));
            }
        }
        public int Count => Length;
        public TItem this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();
                return iData[index];
            }
        }
        public TItem[] Data => iData;
        object IReadOnlyList.this[int index] => iData[index];
        #endregion

        #region ADD
        public virtual void Add(TSubItem subItem)
        {
            var item = NewItem(subItem);
            if (iData.Length <= Count)
                Resize(Count * 2);
            iData[Count] = item;
            Length++;
        }
        public virtual void AddRange(IEnumerable<TSubItem> subItems)
        {
            if (subItems == null) return;
            int sCount = subItems.Count();

            if (iData.Length <= Count + sCount)
                Resize((Count + sCount) * 2);

            if (subItems is TItem[])
            {
                Array.Copy((TItem[])(object)subItems, 0, iData, Count, sCount);
                Length += sCount;
            }
            else
            {
                foreach (var item in subItems)
                    iData[Length++] = NewItem(item);
            }
        }
        public void AddRange(params TSubItem[] subItems) =>
            AddRange((IEnumerable<TSubItem>)subItems);
        #endregion

        #region CLEAR
        public virtual void Clear()
        {
            Array.Clear(iData, 0, Length);
            Length = 0;
        }
        #endregion

        #region INDEX OF
        public int IndexOf(TSubItem subItem)
        {
            return iData.FirstMatchIndex(x => subItem.Equals(GetSubItem(x)));
        }
        #endregion

        #region CONTAINS
        public bool Contains(TSubItem subItem)
        {
            return iData.Any(x => subItem.Equals(GetSubItem(x)));
        }
        #endregion

        #region REMOVE
        public bool Remove(TSubItem subItem)
        {
            var index = iData.FirstMatchIndex(x => subItem.Equals(GetSubItem(x)));
            if (index == -1)
                return false;
            RemoveAt(index);
            return true;
        }
        public void RemoveAt(int index)
        {
            if (index == Count - 1)
            {
                iData[index] = default(TItem);
                --Length;
                return;
            }
            Array.Copy(iData, index + 1, iData, index, Length - (index + 1));
            Array.Clear(iData, Length - 1, 1);
            --Length;
        }
        #endregion

        #region RESIZE
        void Resize(int count)
        {
            Array.Resize(ref iData, count);
            if (Length > count)
                Length = count;
        }
        void IArray<TItem>.Resize(int length) =>
            Resize(length);
        #endregion

        #region ENUMERATOR
        public IEnumerator<TItem> GetEnumerator()
        {
            position = -1;
            for (int i = 0; i < Length; i++)
            {
                ++position;
                yield return iData[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion

        #region CONVERT
        protected abstract TItem NewItem(TSubItem subItem);
        protected abstract TSubItem GetSubItem(TItem item);
        #endregion
    }
}
