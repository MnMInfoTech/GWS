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
    #region IPRIMITIVE OBJECT COLLECTION<T,U>
    public interface IPrimitiveObjectCollection<T, U> : IQueryableCollection<T>, IAdd<T>,
        IRemove<T>, IExist<T>, IResizeUnit, IIndexOf<T>
        where T : IObject
    { }
    #endregion

    #region IEx IPRIMITIVE OBJECT COLLECTION<T,U>
    internal interface IExPrimitiveObjectCollection<T, U> : IPrimitiveObjectCollection<T, U>
        where T : IObject
        where U : T
    {
        new U this[int index] { get; set; }

        int GetPosition(T item);

        void SetPosition(U item, int value);
    }
    #endregion

    #region IEx IPRIMITIVE OBJECT COLLECTION<T, U>
    internal abstract class ExPrimitiveObjectCollection<T, U> : _QueryableCollection<T>, IExPrimitiveObjectCollection<T, U>, ICollection<T>
        where T : IObject
        where U : T
    {
        #region VARIABLES
        int resizeUnit;
        protected U[] Data;
        protected int Length;
        protected static int ZERO = 0;
        #endregion

        #region CONSTRUCTOR
        public ExPrimitiveObjectCollection(int capacity = 0)
        {
            Data = new U[capacity];
            resizeUnit = 4;
        }
        public ExPrimitiveObjectCollection(IEnumerable<T> collection)
        {
            Data = collection.OfType<U>().
                Select((x, i) =>
                {
                    var item = x;
                    SetPosition(item, i + 1);
                    return item;
                }
                ).ToArray();
            resizeUnit = 4;
        }
        #endregion

        #region PROPERTIES
        public int ResizeUnit
        {
            get => resizeUnit > 0 ? resizeUnit : Length;
            set
            {
                if (value < 0)
                    return;
                resizeUnit = value;
            }
        }
        public override int Count => Length;
        public T Last => Data[Length - 1];
        public override bool IsReadOnly => false;
        U IExPrimitiveObjectCollection<T, U>.this[int index]
        {
            get => Data[index];
            set => REPLACE_AT(index, value);
        }
        #endregion

        #region INDEX OF
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T item)
        {
            IndexOf(item, out int i);
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool IndexOf(T item, out int i)
        {
            i = -1;
            if (!(item is U))
                return false;
            i = GetPosition(item) - 1;
            if (i < 0 || i >= Length || !Equals(item, Data[i]))
                return false;
            return true;
        }
        #endregion

        #region GET VALUE
        protected override T GetValue(int index) =>
            Data[index];
        #endregion

        #region ADD
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            if (!(item is U) || Exists(item))
                return;
            ADD((U)item);
        }
        internal virtual void ADD(U item)
        {
            if (Data.Length <= Length)
                Array.Resize(ref Data, Length + ResizeUnit);
            Data[Length++] = item;
            SetPosition(item, Length);
        }
        #endregion

        #region INSERT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int position, T item)
        {
            if (!(item is U) || Exists(item))
                return;
            INSERT(position, (U)item);
        }
        internal virtual void INSERT(int position, U item)
        {
            if (Data.Length <= Length)
                Array.Resize(ref Data, Length + ResizeUnit);

            Array.Copy(Data, position, Data, position + 1, Length - position);
            Data[position] = item;
            SetPosition(item, position + 1);
            Length++;
            for (int i = position + 1; i < Length; i++)
                SetPosition(Data[i], i + 1);
        }
        #endregion

        #region REMOVE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
                return false;
            return REMOVE_AT(index);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveAt(int index)
        {
            if (index == -1 || index >= Length)
                return false;
            return REMOVE_AT(index);
        }
        internal virtual bool REMOVE_AT(int index)
        {
            if (index == Length - 1)
            {
                --Length;

                SetPosition(Data[Length], ZERO);
                Data[Length] = default(U);
                return true;
            }
            var widget = Data[index];
            SetPosition(widget, ZERO);
            var srcIndex = index + 1;
            var len = Length - srcIndex;
            if (len < 0)
            {
                len = srcIndex - Length;
            }

            Array.Copy(Data, srcIndex, Data, index, len);
            --Length;
            SetPosition(Data[Length], ZERO);
            Data[Length] = default(U);

            for (int i = index; i < Length; i++)
                SetPosition(Data[i], i + 1);

            return true;
        }
        //void IList<T>.RemoveAt(int index) =>
        //    RemoveAt(index);
        public void RemoveLast() =>
            RemoveAt(Length - 1);
        #endregion

        #region REPLACE
        internal virtual bool REPLACE_AT(int index, U item)
        {
            if (IndexOf(item) == index)
                return false;
            SetPosition(Data[index], ZERO);
            Data[index] = item;
            SetPosition(item, index + 1);
            return true;
        }
        #endregion

        #region CONTAINS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool ICollection<T>.Contains(T item) =>
            IndexOf(item, out _);
        #endregion

        #region EXISTS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Exists(T item) =>
            IndexOf(item, out _);
        #endregion

        #region CLEAR
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Clear()
        {
            for (int i = 0; i < Length; i++)
                SetPosition(Data[i], ZERO);
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
            for (int i = arrayIndex; i < Length; i++)
                SetPosition(Data[i], i + 1);
        }
        #endregion

        #region MAKE ZERO LENGTH
        public void MakeZeroLength()
        {
            for (int i = 0; i < Length; i++)
                SetPosition(Data[i], ZERO);
            Length = 0;
            Data = new U[Length];
        }
        #endregion

        #region INT32 TO AND FROM CONVERSION
        int IExPrimitiveObjectCollection<T, U>.GetPosition(T item) =>
            GetPosition(item);

        void IExPrimitiveObjectCollection<T, U>.SetPosition(U item, int value) =>
            SetPosition(item, value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract int GetPosition(T item);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal abstract void SetPosition(U item, int value);
        #endregion

        #region DISPOSE
        public override void Dispose()
        {
            base.Dispose();
            Data = null;
        }
        #endregion
    }
    #endregion
}
