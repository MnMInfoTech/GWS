using System;
/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public class MiniCollection<T>: IMiniCollection<T>
    {
        #region VARIABLES
        protected T[] iData;
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
        public virtual T this[int index]
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
        #endregion

        #region ADD
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Add(T item)
        {
            if (iData.Length <= Count)
                Resize(Count * 2);
            iData[Count] = item;
            Length++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(params T[] items) =>
            AddRange((IEnumerable<T>)items);
        #endregion

        #region CLEAR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Clear()
        {
            Array.Clear(iData, 0, Length);
            Length = 0;
        }
        #endregion

        #region INDEX OF
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int IndexOf(T item)
        {
            return iData.FirstMatchIndex(x => item.Equals(x));
        }
        #endregion

        #region CONTAINS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Contains(T item)
        {
            return iData.Any(x => item.Equals(x));
        }
        #endregion

        #region REMOVE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Remove(T item)
        {
            var index = iData.FirstMatchIndex(x => item.Equals(x));
            if (index == -1)
                return false;
            RemoveAt(index);
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void RemoveAt(int index)
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
