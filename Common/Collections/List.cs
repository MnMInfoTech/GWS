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
    #region IGWSLIST<T>
    public interface IGwsList<T> : IMiniCollection<T>, IInsertRange<T>
    {
        int Capacity { get; set; }

        /// <summary>
        /// Trims internal data array.
        /// </summary>
        void Trim();

        #region SORT
        void Sort();
        void Sort(IComparer<T> comparer);
        void Sort(Comparison<T> comparer);
        #endregion
    }
    #endregion

    public class List<T>: _QueryableCollection<T>, IGwsList<T>, IReadOnlyList, IArrayHolder<T>, IExIndexer<T>, IList<T>
    {
        #region VARIABLES
        protected T[] Data;
        protected int Length;
        #endregion

        #region CONSTRUCTORS
        public List()
        {
            Length = 0;
            Data = new T[4];
        }
        public List(int capacity) : this()
        {
            Capacity = capacity;
        }
        public List(int capacity, bool exactCapacity) : this()
        {
            if (exactCapacity)
                Resize(capacity);
            else
                Capacity = capacity;
        }
        public List(IEnumerable<T> collection) : this()
        {
            AddRange(collection);
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets the first element.
        /// </summary>
        public T First => this[0];

        /// <summary>
        /// Gets the last element.
        /// </summary>
        /// <value>The last.</value>
        public T Last => this[Length - 1];

        public virtual int Capacity
        {
            get => Data.Length;
            set
            {
                if (value < Count)
                    return;
                else if (value > Count)
                    Resize(Math.Max(value, Count * 2));
            }
        }
        public override int Count => Length;
        public T this[int index]
        {
            get
            {
                return Data[index];
            }
            set
            {
                ReplaceAt(index, value);
            }
        }
        public override bool IsReadOnly => false;
        T[] IArrayHolder<T>.Data => Data;
        #endregion

        #region GET VALUE
        protected override T GetValue(int index)
        {
            return Data[index];
        }
        #endregion

        #region ADD
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Add(T item)
        {
            if (Data.Length <= Length)
                Resize(Length * 2);
            Data[Length] = item;
            Length++;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void AddRange(IEnumerable<T> items)
        {
            if (items == null) return;
            int sCount = items.Count();

            if (Data.Length <= Count + sCount)
                Resize((Count + sCount) * 2);

            if (items is T[])
            {
                Array.Copy((T[])items, 0, Data, Count, sCount);
                Length += sCount;
            }
            else
            {
                foreach (var item in items)
                    Data[Length++] = item;
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(params T[] items) =>
            AddRange((IEnumerable<T>)items);
        #endregion

        #region SWAP
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Swap(T item1, T item2)
        {
            var i = IndexOf(item1);
            if (i == -1)
                return false;
            var j = IndexOf(item2);
            if (j == -1)
                return false;
            var item = Data[i];
            Data[i] = Data[j];
            Data[j] = item;
            return true;
        }
        #endregion

        #region RELOCATE
        public bool Relocate(T item, int newIndex)
        {
            var oldIndex = IndexOf(item);
            return Relocate(oldIndex, newIndex);
        }
        protected virtual bool Relocate(int oldIndex, int newIndex)
        {
            if (oldIndex == newIndex || oldIndex == -1)
                return false;
            if (oldIndex >= Length)
                oldIndex = Length - 1;

            var item = Data[oldIndex];
            int start = oldIndex;
            int end = newIndex;
            int replace = end;
            if (newIndex < oldIndex)
            {
                end = oldIndex;
                start = newIndex;
                replace = start;
            }
            Array.Copy(Data, start, Data, start + 1, end - start);
            Data[replace] = item;
            return true;
        }
        #endregion

        #region INSERT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Insert(int position, T item)
        {
            if (Data.Length <= Length) 
                Resize(Count * 2);

            Array.Copy(Data, position, Data, position + 1, Length - position);
            Data[position] = item;
            Length++;
        }
        #endregion

        #region INSERT RANGE
        public virtual void InsertRange(int index, IEnumerable<T> items)
        {
            if (items == null) return;
            int sCount = items.Count();

            var _data = new T[Math.Max(Data.Length, (Length + sCount) * 2)];
            Array.Copy(Data, 0, _data, 0, index);
            int j = index;
            foreach (var item in items)
                _data[j++] = item;

            Array.Copy(Data, index, _data, index + sCount, Length - index);
            Data = _data;
            Length += sCount;
        }
        public void InsertRange(int position, params T[] items) =>
            InsertRange(position, (IEnumerable<T>)items);
        #endregion

        #region REPLACE
        protected virtual void ReplaceAt(int index, T newItem)
        {
            Data[index] = newItem;
        }
        #endregion

        #region CLEAR
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Clear()
        {
            Array.Clear(Data, 0, Length);
            Length = 0;
        }
        void IClearable<bool>.Clear(bool parameter) =>
            Clear();
        #endregion

        #region INDEX OF
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int IndexOf(T item)
        {
            return Array.IndexOf(Data, item, 0, Length);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int IndexOf(Predicate<T> predicate)
        {
            return Array.FindIndex(Data, 0, Length, predicate);
        }
        #endregion

        #region EXISTS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Exists(T item)
        {
            return Array.IndexOf(Data, item, 0, Length) != -1;
        }

        bool ICollection<T>.Contains(T item)=>
            Exists(item);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Contains(Predicate<T> predicate)
        {
            return Array.FindIndex(Data, 0, Length, predicate) != -1;
        }
        #endregion

        #region REMOVE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            var index = IndexOf(item);
            return RemoveAt(index);
        }
      
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool RemoveAt(int index)
        {
            if (index == -1 || index >= Length)
                return false;
            if (index == Length - 1)
            {
                Data[index] = default(T);
                --Length;
                return true;
            }
            Array.Copy(Data, index + 1, Data, index, Length - (index + 1));
            Array.Clear(Data, Length - 1, 1);
            --Length;
            return true;
        }
        void IList<T>.RemoveAt(int index) =>
            RemoveAt(index);
        #endregion

        #region RESIZE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Resize(int count)
        {
            Array.Resize(ref Data, count);
            if (Length > count)
                Length = count;
        }
        #endregion

        #region REMOVE LAST
        public void RemoveLast() =>
            RemoveAt(Length - 1);
        #endregion

        #region COPY TO
        public void CopyTo(T[] array, int arrayIndex) 
        {
            if (arrayIndex + Length > array.Length)
                Array.Resize(ref array, arrayIndex + Length);
            Array.Copy(Data, 0, array, arrayIndex, Length);
        }

        public void CopyTo(T[] array, int arrayIndex, int length)
        {
            length = Math.Min(length, Count);
            if (arrayIndex + length > array.Length)
                Array.Resize(ref array, arrayIndex + length);
            Array.Copy(Data, 0, array, arrayIndex, length);
        }
        #endregion

        #region SORT
        public void Sort()
        {
            var data = new T[Count];
            CopyTo(data, 0);
            Array.Sort(data);
            Array.Copy(data, Data, data.Length);
        }
        public void Sort(IComparer<T> comparer)
        {
            var data = new T[Count];
            CopyTo(data, 0);
            Array.Sort(data, comparer);
            Array.Copy(data, Data, data.Length);
        }
        public void Sort(Comparison<T> comparer)
        {
            var data = new T[Count];
            CopyTo(data, 0);
            Array.Sort(data, comparer);
            Array.Copy(data, Data, data.Length);
        }
        #endregion

        #region TRIM
        public void Trim()
        {
            Array.Resize(ref Data, Length);
        }
        #endregion

        #region ENUMERATOR
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sealed override IEnumerator<T> GetEnumerator()
        {
            int i = -1;
            foreach (var item in Data)
            {
                ++i;
                if (i >= Length)
                    break;
                yield return item;
            }
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion
    }
}
