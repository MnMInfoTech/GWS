using System;
/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public class MiniCollection<T>: IMiniCollection<T>
    {
        #region VARIABLES
        T[] iData;
        int Length;
        int position = -1;
        #endregion

        #region CONSTRUCTORS
        public MiniCollection()
        {
            Length = 0;
            iData = new T[4];
        }
        public MiniCollection(int capacity) : this()
        {
            Capacity = capacity;
        }
        public MiniCollection(int capacity, bool exactCapacity) : this()
        {
            if (exactCapacity)
                Resize(capacity);
            else
                Capacity = capacity;
        }
        public MiniCollection(IEnumerable<T> collection) : this()
        {
            AddRange(collection);
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
        public T this[int index]
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
                iData[index] = value;
            }
        }
        public T[] Data => iData;
        T IReadOnlyList<T>.this[int index] => this[index];
        object IReadOnlyList.this[int index] => this[index];
        #endregion

        #region ADD
        public virtual void Add(T item)
        {
            if (iData.Length <= Count)
                Resize(Count * 2);
            iData[Count] = item;
            Length++;
        }
        public virtual void AddRange(IEnumerable<T> items)
        {
            if (items == null) return;
            int sCount = items.Count();

            if (iData.Length <= Count + sCount)
                Resize((Count + sCount) * 2);

            if (items is T[])
            {
                Array.Copy((T[])items, 0, iData, Count, sCount);
                Length += sCount;
            }
            else
            {
                foreach (var item in items)
                    iData[Length++] = item;
            }
        }
        public void AddRange(params T[] items) =>
            AddRange((IEnumerable<T>)items);
        #endregion

        #region CLEAR
        public virtual void Clear()
        {
            Array.Clear(iData, 0, Length);
            Length = 0;
        }
        #endregion

        #region INDEX OF
        public int IndexOf(T item)
        {
            return iData.FirstMatchIndex(x => item.Equals(x));
        }
        #endregion

        #region CONTAINS
        public bool Contains(T item)
        {
            return iData.Any(x => item.Equals(x));
        }
        #endregion

        #region REMOVE
        public bool Remove(T item)
        {
            var index = iData.FirstMatchIndex(x => item.Equals(x));
            if (index == -1)
                return false;
            RemoveAt(index);
            return true;
        }
        public void RemoveAt(int index)
        {
            if (index == Count - 1)
            {
                iData[index] = default(T);
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
        void IArray<T>.Resize(int length) =>
            Resize(length);
        #endregion

        #region ENUMERATOR
        public IEnumerator<T> GetEnumerator()
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
    }
}
