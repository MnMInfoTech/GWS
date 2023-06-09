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
    #region IINDEXED COLLECTION<T,U>
    public partial interface IObjectCollection<T, U> : IPrimitiveObjectCollection<T, U>, IAddRange<T>,
        IInsertRange<T>, IRemoveRange<T>, ISwap<T>
        where T: IObject
    {
        
    }
    #endregion

    #region IEx INDEXED COLLECTION<T,U>
    internal partial interface IExObjectCollection<T, U> : IObjectCollection<T, U>, IExPrimitiveObjectCollection<T, U>
        where T : IObject
        where U : T
    { }
    #endregion

    #region IEx OBJECT COLLECTION<T, U>
    internal abstract class ExObjectCollection<T, U> : ExPrimitiveObjectCollection<T, U>, IExObjectCollection<T, U> 
        where T : IObject
        where U : T
    {
        #region CONSTRUCTORS
        public ExObjectCollection(int capacity = 0) :
            base(capacity)
        { }
        public ExObjectCollection(IEnumerable<T> collection) :
            base(collection)
        { }
        #endregion

        #region ADD RANGE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                return;
            ADD_RANGE(items.OfType<U>().Where(x => !Exists(x)));
        }
        public void AddRange(params T[] items) =>
            AddRange((IEnumerable<T>)items);
        internal virtual bool ADD_RANGE(IEnumerable<U> items)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (Data.Length <= Length)
                    Array.Resize(ref Data, Length + ResizeUnit);
                Data[Length++] = item;
                SetPosition(item, Length);
                ++i;
            }
            return i > 0;
        }
        #endregion

        #region INSERT RANGE
        public void InsertRange(int position, IEnumerable<T> items)
        {
            if (items == null) return;
            INSERT_RANGE(position, items.OfType<U>().Where(x => !Exists(x)));
        }

        public void InsertRange(int position, params T[] items) =>
            InsertRange(position, (IEnumerable<T>)items);

        internal virtual bool INSERT_RANGE(int position, IEnumerable<U> items)
        {
            int sCount = items.Count();
            if (sCount == 0)
                return false;
            
            if (position > Length)
                position = Length;

            var temp = new U[Math.Max(Data.Length, (Length + sCount) * 2)];
            Array.Copy(Data, 0, temp, 0, position);
            int j = position;

            var array = temp;
            foreach (var item in items)
                array[j++] = item;

            Array.Copy(Data, position, temp, position + sCount, Length - position);
            Data = temp;
            Length += sCount;
            for (int i = position; i < Length; i++)
                SetPosition(Data[i], i + 1);
            return true;
        }
        #endregion

        #region REMOVE RANGE
        public void RemoveRange(int startIndex, int count)
        {
            if (startIndex < 0)
                startIndex = 0;
            GWSEnumerable.CorrectLength(ref startIndex, ref count, Length);
            var indices = System.Linq.Enumerable.Range(startIndex, count).ToArray();
            if (indices.Length > 0)
                REMOVE_RANGE(indices);
        }
        public void RemoveAll(params T[] items)
        {
            var indices = items.Select(
                item => IndexOf(item)).Where(i => i != -1).ToArray();
            if (indices.Length > 0)
                REMOVE_RANGE(indices);
        }
        internal virtual void REMOVE_RANGE(int[] indices)
        {
            foreach (var i in indices)
               SetPosition(Data[i], ZERO);
            Data = Data.Where(indices, CriteriaMode.Exclude).ToArray();
            Length = Data.Length;
            for (int i = 0; i < Length; i++)
               SetPosition(Data[i], i + 1);
        }
        #endregion

        #region SWAP
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Swap(T item1, T item2)
        {
            var i = IndexOf(item1);
            var j = IndexOf(item2);
            if (i == -1 || j == -1)
                return false;
            SWAP(i, j);
            return true;
        }
        internal virtual void SWAP(int index1, int index2)
        {
            var item = Data[index1];
            SetPosition(item, index2 + 1);
            SetPosition(Data[index2], index1 + 1);
            Data[index1] = Data[index2];
            Data[index2] = item;
        }
        #endregion

        #region RELOCATE
        public bool Relocate(T item, int newIndex)
        {
            var oldIndex = IndexOf(item);
            return RELOCATE(oldIndex, newIndex);
        }
        internal virtual bool RELOCATE(int oldIndex, int newIndex)
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
            SetPosition(item, replace + 1);
            Data[replace] = item;
            var last = replace + (end - start);
            for (int i = replace + 1; i <= last; i++)
                SetPosition(Data[i], i + 1);
            return true;
        }
        #endregion
    }
    #endregion
}
