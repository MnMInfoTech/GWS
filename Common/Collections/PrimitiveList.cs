/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MnM.GWS
{
    #region IPRIMITIVELIST<T>
    public interface IPrimitiveList<T> : IMiniCollection<T>, IResizeUnit, IDisposable
    { }
    #endregion

    #region PRIMITIVELIST<T>
    public class PrimitiveList<T> : _QueryableCollection<T>, IPrimitiveList<T>, IArrayHolder<T>, IExIndexer<T>, IList<T>
    {
        #region VARIABLES
        int resizeUnit;
        protected T[] Data;
        protected int Length;
        #endregion

        #region CONSTRUCTOR
        public PrimitiveList(int capacity = 0)
        {
            Data = new T[capacity];
            resizeUnit = 4;
        }
        public PrimitiveList(IEnumerable<T> collection)
        {
            Data = collection.ToArray();
            resizeUnit = 4;
        }
        #endregion

        #region PROPERTIES
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
        public int ResizeUnit 
        {
            get => resizeUnit > 0? resizeUnit: Length;
            set
            {
                if (value < 0)
                    return;
                resizeUnit = value;
            }
        }
        public override int Count => Length;
        public T Last => Data[Length - 1];
        T[] IArrayHolder<T>.Data => Data;
        public override bool IsReadOnly => false;
        #endregion

        #region GET VALUE
        protected override T GetValue(int index)
        {
            return Data[index];
        }
        #endregion

        #region INDEX OF
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual int IndexOf(T item)
        {
            return Array.IndexOf(Data, item, 0, Length);
        }
        #endregion

        #region ADD
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Add(T item)
        {
            if (Data.Length <= Length)
                Array.Resize(ref Data, Length + ResizeUnit);
            Data[Length++] = item;
        }
        #endregion

        #region ADD RANGE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void AddRange(IEnumerable<T> items)
        {
            if (items == null) return;
            int sCount = items.Count();

            if (Data.Length <= Count + sCount)
                Array.Resize(ref Data, Length + sCount + ResizeUnit); 

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
        public void AddRange(params T[] items) =>
            AddRange((IEnumerable<T>)items);
        #endregion

        #region INSERT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Insert(int position, T item)
        {
            if (Data.Length <= Length)
                Array.Resize(ref Data, Length + ResizeUnit);

            Array.Copy(Data, position, Data, position + 1, Length - position);
            Data[position] = item;
            Length++;
        }
        #endregion

        #region REMOVE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
                return false;
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
        public void RemoveLast() => RemoveAt(Length - 1);
        #endregion

        #region REPLACE
        protected virtual void ReplaceAt(int index, T newItem)
        {
            Data[index] = newItem;
        }
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

        #region EXISTS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Exists(T item)
        {
            return Array.IndexOf(Data, item, 0, Length) != -1;
        }
        bool ICollection<T>.Contains(T item)
        {
            return Array.IndexOf(Data, item, 0, Length) != -1;
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

        #region COPY TO
        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex + Length > array.Length)
                Array.Resize(ref array, arrayIndex + Length);
            Array.Copy(Data, 0, array, arrayIndex, Length);
        }
        #endregion

        #region MAKE ZERO LENGTH
        public virtual void MakeZeroLength()
        {
            Length = 0;
            Data = new T[Length];
        }
        #endregion
    }
    #endregion
}
