/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    public class Collection<T> : IGwsCollection<T>, IArray<T>
    {
        #region VARIABLES
        T[] iData;
        int Length;
        int position = -1;
        #endregion

        #region CONSTRUCTORS
        public Collection()
        {
            Length = 0;
            iData = new T[4];
        }
        public Collection(int capacity) : this()
        {
            Capacity = capacity;
        }
        public Collection(int capacity, bool exactCapacity) : this()
        {
            if (exactCapacity)
                Resize(capacity);
            else
                Capacity = capacity;
        }
        public Collection(IEnumerable<T> collection) : this()
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
        public bool IsReadOnly => false;
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

        /// <summary>
        /// Gets the first element.
        /// </summary>
        public T First => this[0];

        /// <summary>
        /// Gets the last element.
        /// </summary>
        /// <value>The last.</value>
        public T Last => this[Count - 1];

        T IReadOnlyList<T>.this[int index]=>this[index];
        object IReadOnlyList.this[int index] => this[index];
        #endregion

        #region INDEX OF
        public int IndexOf(T item)
        {
            return Array.IndexOf(iData, item);
        }
        public int IndexOf(Predicate<T> predicate)
        {
            return iData.FirstMatchIndex(predicate);
        }
        #endregion

        #region CONTAINS
        public bool Contains(T item)
        {
            return Array.Exists(iData, (x => item.Equals(x)));
        }
        public bool Contains(Predicate<T> predicate)
        {
            return iData.FirstMatch(predicate, out _, out _);
        }
        #endregion

        #region INSERT
        public void Insert(int index, T item)
        {
            if (iData.Length <= Length) Resize(Count * 2);
            iData[Length] = item;
            ++Length;
        }
        public void InsertRange(int index, IEnumerable<T> items)
        {
            if (items == null) return;
            int sCount = items.Count();

            var _data = new T[Math.Max(iData.Length, (Length + sCount) * 2)];
            Array.Copy(iData, 0, _data, 0, index);
            int j = index;
            foreach (var item in items)
                _data[j++] = item;

            Array.Copy(iData, index, _data, index + sCount, Length - index);
            iData = _data;
            Length += sCount;
        }
        #endregion

        #region REMOVE
        public bool Remove(T item)
        {
            var i = Array.IndexOf(iData, item);
            if (i == -1)
                return false;
            RemoveAt(i);
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
        public void RemoveLast()
        {
            iData[Length - 1] = default(T);
            --Length;
        }
        #endregion

        #region ADD
        public void Add(T item)
        {
            if (iData.Length <= Count)
                Resize(Count * 2);
            iData[Count] = item;
            Length++;
        }
        public void AddRange(IEnumerable<T> items)
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
        #endregion

        #region CLEAR
        public void Clear()
        {
            Array.Clear(iData, 0, Length);
            Length = 0;
        }
        #endregion

        #region COPY TO
        public void CopyTo(T[] array, int arrayIndex) =>
            CopyTo(array, arrayIndex, Count);
        public void CopyTo(T[] array, int arrayIndex, int length)
        {
            length = Math.Min(length, Count);
            if (arrayIndex + length > array.Length)
                Array.Resize(ref array, arrayIndex + length);
            Array.Copy(iData, 0, array, arrayIndex, length);
        }
        public int CopyTo(ref T[] target, ISpan range, int arrayIndex)
        {
            if (target == null)
                target = new T[0];

            int len = target.Length;

            if (arrayIndex >= 0 && arrayIndex <= target.Length)
            {
                int start = 0, count = Count;

                if (range != null)
                {
                    range = new Span(range, count);
                    start = range.Start; count = range.Count;
                }
                if (arrayIndex + count > target.Length)
                {
                    int resize = arrayIndex + count - target.Length;
                    Array.Resize(ref target, target.Length + resize);
                }
                Array.Copy(iData, start, target, arrayIndex, count);
            }
            return target.Length - len;
        }
        #endregion

        #region TRIM
        public void Trim()
        {
            if (Count < iData.Length)
            {
                Array.Resize(ref iData, Count);
            }
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

        #region SORT
        public void Sort()
        {
            var data = new T[Count];
            CopyTo(data, 0);
            Array.Sort(data);
            Array.Copy(data, iData, data.Length);
        }
        public void Sort(IComparer<T> comparer)
        {
            var data = new T[Count];
            CopyTo(data, 0);
            Array.Sort(data, comparer);
            Array.Copy(data, iData, data.Length);
        }
        public void Sort(Comparison<T> comparer)
        {
            var data = new T[Count];
            CopyTo(data, 0);
            Array.Sort(data, comparer);
            Array.Copy(data, iData, data.Length);
        }
        #endregion

        #region TO ARRAY
        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns>T[].</returns>
        public T[] ToArray()
        {
            T[] arr = null;
            CopyTo(ref arr, new Span(0, Count), 0);
            return arr;
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns>T[].</returns>
        public T[] ToArray(ISpan range)
        {
            T[] arr = null;
            CopyTo(ref arr, range, 0);
            return arr;
        }
        #endregion

        #region INDEX OK
        /// <summary>
        /// Verifies the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="System.Exception">Index is out of bound of array</exception>
        public bool IndexOK(int index) =>
            index >= 0 && index <= Count - 1;
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
