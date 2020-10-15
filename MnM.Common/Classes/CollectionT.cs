/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    public class Collection<T>: _Iterator<T>, IGwsCollection<T>, IArray<T>
    {
        #region VARIABLES
        T[] TData;
        int Length;
        #endregion

        #region CONSTRUCTORS
        public Collection() 
        {
            Length = 0;
            TData = new T[4];
        }
        public Collection(int capacity): this()
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
        public Collection(IEnumerable<T> collection):this()
        {
            AddRange(collection);
        }
        #endregion

        #region PROPERTIES
        public int Capacity
        {
            get => TData.Length;
            set
            {
                if (value < Count)
                    return;
                else if (value > Count)
                   Resize(Math.Max(value, Count * 2));
            }
        }
        public override int Count => Length;
        public override bool IsReadOnly => false;
        public override T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();
                return  TData[index];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();
                TData[index] = value;
            }
        }
        public T[] Data => TData;
        #endregion

        #region INDEX OF
        public int IndexOf(T item)
        {
            return Array.IndexOf(TData, item);
        }
        public bool Contains(T item)
        {
            return Array.Exists(TData, (x => item.Equals(x)));
        }
        #endregion

        #region INSERT
        public void Insert(int index, T item)
        {
            if (TData.Length <= Length) Resize(Count * 2);
            TData[Length] = item;
            ++Length;
        }
        public void InsertRange(int index, IEnumerable<T> items)
        {
            if (items == null) return;
            int sCount = items.Count();

            var _data = new T[Math.Max(TData.Length, (Length + sCount) * 2)];
            Array.Copy(TData, 0, _data, 0, index);
            int j = index;
            foreach (var item in items)
                _data[j++] = item;
            
            Array.Copy(TData, index, _data, index + sCount, Length - index);
            TData = _data;
            Length += sCount;
        }
        #endregion

        #region REMOVE
        public bool Remove(T item)
        {
            var i = Array.IndexOf(TData, item);
            if (i == -1)
                return false;
            RemoveAt(i);
            return true;
        }
        public void RemoveAt(int index)
        {
            if(index == Count - 1)
            {
                TData[index] = default(T);
                --Length;
                return;
            }
            Array.Copy(TData, index + 1, TData, index, Length - (index + 1));
            Array.Clear(TData, Length - 1, 1);
            --Length;
        }
        public void RemoveLast()
        {
            TData[Length - 1] = default(T);
            --Length;
        }
        #endregion

        #region ADD
        public void Add(T item)
        {
            if (TData.Length <= Count) 
                Resize(Count * 2);
            TData[Count] = item;
            Length++;
        }
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) return;
            int sCount = items.Count();

            if (TData.Length <= Count + sCount)
                Resize((Count + sCount) * 2);

            if (items is T[])
            {
                Array.Copy((T[])items, 0, TData, Count, sCount);
                Length += sCount;
            }
            else
            {
                foreach (var item in items)
                    TData[Length++] = item;
            }
        }
        #endregion

        #region CLEAR
        public void Clear()
        {
            Array.Clear(TData, 0, Length);
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
            Array.Copy(TData, 0, array, arrayIndex, length);
        }
        #endregion

        #region TRIM
        public void Trim()
        {
            if (Count < TData.Length)
            {
                Array.Resize(ref TData, Count);
            }
        }
        #endregion

        #region RESIZE
        void Resize(int count)
        {
            Array.Resize(ref TData, count);
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
            Array.Copy(data, TData, data.Length);
        }
        public void Sort(IComparer<T> comparer)
        {
            var data = new T[Count];
            CopyTo(data, 0);
            Array.Sort(data, comparer);
            Array.Copy(data, TData, data.Length);
        }
        public void Sort(Comparison<T> comparer)
        {
            var data = new T[Count];
            CopyTo(data, 0);
            Array.Sort(data, comparer);
            Array.Copy(data, TData, data.Length);
        }
        #endregion

        #region ENUMERATOR
        public override IEnumerator<T> GetEnumerator()
        {
            position = -1;
            for (int i = 0; i < Length; i++)
            {
                ++position;
                yield return TData[i];
            }
        }
        #endregion
    } 
}
