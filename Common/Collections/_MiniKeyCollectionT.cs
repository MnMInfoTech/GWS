/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IMINI KEY COLLECITON
    public interface IMiniKeyCollection<TKey, TItem> : IMiniCollection<TItem>, 
       IFind<TKey, TItem>, ISwap<TKey, TItem>, IRemove<TKey, TItem>
    {
        /// <summary>
        /// Gets readonly list of all keys exist in this collection.
        /// </summary>
        IReadOnlyList<TKey> Keys { get; }
    }
    #endregion

    #region MINI KEY COLLECTION
    public abstract class _MiniKeyCollection<TKey, TItem>: _QueryableCollection<TItem>,
        IMiniKeyCollection<TKey, TItem>, IExIndexer<TItem>, IList<TItem>
    {
        #region VARIABLES
        protected Dictionary<TKey, int> KeyItems;
        protected TItem[] Data;
        protected int Length;
        protected int resizeUnit;
        #endregion

        #region CONSTRUCTORS
        protected _MiniKeyCollection()
        {
            KeyItems = new Dictionary<TKey, int>();
            Keys = new KeyList(this);
            Data = new TItem[4];
        }
        protected _MiniKeyCollection(int capacity) 
        {
            KeyItems = new Dictionary<TKey, int>(capacity);
            Keys = new KeyList(this);
            Data = new TItem[capacity * 2];
        }
        #endregion

        #region PROPERTIES
        public IReadOnlyList<TKey> Keys { get; private set; }
        public TItem this[int index] 
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
        public sealed override int Count => 
            Length;
        public override bool IsReadOnly => false;
        #endregion

        #region GET VALUE
        protected override TItem GetValue(int index)
        {
            return Data[index];
        }
        #endregion

        #region GET KEY
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract TKey KeyOf(TItem item);
        #endregion

        #region SYNC INDICES
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void SyncIndices(int start, int end)
        {
            if (start >= Data.Length || start > Length - 1)
                return;
            if (start < 0)
                start = 0;

            if (end >= Length)
                end = Length - 1;
            if (end < 0)
                return;
            var array = Data;
            if (start > end)
            {
                for (int i = start; i >= end; i--)
                {
                    var id = KeyOf(array[i]);
                    KeyItems[id] = i;
                }
            }
            else
            {
                for (int i = start; i <= end; i++)
                {
                    var id = KeyOf(array[i]);
                    KeyItems[id] = i;
                }
            }
        }
        #endregion

        #region ADD
        public void Add(TItem item) =>
            _Add(item);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool _Add(TItem item)
        {
            if (item == null)
                return false;
            var id = KeyOf(item);
            if (KeyItems.ContainsKey(id))
                return false;
            KeyItems[KeyOf(item)] = Length;
            if (Data.Length <= Length)
            {
                var resizeCount = resizeUnit == 0 ? Length * 2 : Length + resizeUnit;
                Resize(resizeCount);
            }
            Data[Length++] = item;
            return true;
        }

        public void AddRange(IEnumerable<TItem> items) =>
            _AddRange(items);

       public void AddRange(params TItem[] items) =>
            _AddRange(items);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void _AddRange(IEnumerable<TItem> items)
        {
            int count = Length;
            var _items = items.Where((item) => items != null &&
            KeyItems.ContainsKey(KeyOf(item)));
            int sCount = items.Count();

            if (Data.Length <= Length + sCount)
            {
                var resizeCount = resizeUnit == 0 ? (Count + sCount) * 2 : (Count + sCount) + resizeUnit;
                Resize(resizeCount);
            }

            if (items is TItem[])
            {
                Array.Copy((TItem[])items, 0, Data, Count, sCount);
                Length += sCount;
            }
            else
            {
                foreach (var item in items)
                    Data[Length++] = item;
            }
            SyncIndices(count, Length - 1);
        }
        #endregion

        #region INSERT
        public void Insert(int position, TItem item) =>
            _Insert(position, item);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual bool _Insert(int position, TItem item)
        {
            var id = KeyOf(Data[position]);
            if (KeyItems.ContainsKey(id))
                return false;
            if (Data.Length <= Length)
            {
                var resizeCount = resizeUnit == 0 ? Length * 2 : Length + resizeUnit;
                Resize(resizeCount);
            }

            Array.Copy(Data, position, Data, position + 1, Length - position);
            Data[position] = item;
            Length++;
            KeyItems[id] = position;
            SyncIndices(position + 1, Length);
            return true;
        }
        protected virtual void _InsertRange(int position, IEnumerable<TItem> items)
        {
            if (items == null) return;
            var _items = items.Where((item) => item != null && KeyItems.ContainsKey(KeyOf(item)));

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
            SyncIndices(position, Length - 1);
        }
        #endregion

        #region REMOVE  
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(TItem item)
        {
            if (item == null)
                return false;
            var key = KeyOf(item);
            if (key == null)
                return false;
            if (!KeyItems.TryGetValue(key, out int i))
                return false;
            return RemoveAt(i);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool RemoveByKey(TKey key)
        {
            if (!KeyItems.TryGetValue(key, out int index))
                return false;
            return RemoveAt(index);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool RemoveAt(int index)
        {
            if (!KeyItems.Remove(KeyOf(Data[index])))
                return false;

            if (index == Length - 1)
            {
                Data[index] = default(TItem);
                --Length;
                return true;
            }
            Array.Copy(Data, index + 1, Data, index, Length - (index + 1));
            Array.Clear(Data, Length - 1, 1);
            --Length;
            SyncIndices(index, Length - 1);
            return true;
        }

        public void RemoveLast() =>
            RemoveAt(Length - 1);
        void IList<TItem>.RemoveAt(int index) => 
            RemoveAt(index);
        #endregion

        #region SWAP
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Swap(TItem item1, TItem item2)
        {
            TKey key1 = KeyOf(item1);
            TKey key2 = KeyOf(item2);
            if (!KeyItems.TryGetValue(key1, out int i))
                return false;
            if (!KeyItems.TryGetValue(key2, out int j))
                return false;
            Swap2(i, key1, j, key2);
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Swap(TKey key1, TKey key2)
        {
            if (!KeyItems.TryGetValue(key1, out int i))
                return false;
            if (!KeyItems.TryGetValue(key2, out int j))
                return false;
            Swap2(i, key1, j, key2);
            return true;
        }
        protected virtual void Swap2(int i, TKey key1, int j, TKey key2)
        {
            var item = Data[i];
            Data[i] = Data[j];
            Data[j] = item;
            KeyItems[key1] = j;
            KeyItems[key2] = i;
        }
        #endregion

        #region RELOCATE
        public bool Relocate(TKey key, int newIndex)
        {
            if (!KeyItems.TryGetValue(key, out int oldIndex))
                return false;
            return Relocate(oldIndex, newIndex);
        }
        public bool Relocate(TItem item, int newIndex)
        {
            var key = KeyOf(item);
            if (!KeyItems.TryGetValue(key, out int oldIndex))
                return false;
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
            SyncIndices(oldIndex, newIndex);
            return true;
        }
        #endregion

        #region REPLACE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Replace(TKey key, TItem item)
        {
            if (!KeyItems.TryGetValue(key, out int i))
                return false;
            ReplaceAt(i, item);
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void ReplaceAt(int index, TItem newItem)
        {
            KeyItems.Remove(KeyOf(Data[index]));
            Data[index] = newItem;
            KeyItems[KeyOf(newItem)] = index;
        }
        #endregion

        #region FIND ITEM
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TItem Find(TKey key)
        {
            if (!KeyItems.TryGetValue(key, out int i))
                return default(TItem);
            return Data[i];
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Find(TKey key, out TItem item)
        {
            item = default(TItem);
            if (!KeyItems.TryGetValue(key, out int i))
                return false;
            item = Data[i];
            return true;
        }
        #endregion

        #region FIND INDEX
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(TKey key)
        {
            if (key == null)
            {
                return -1;
            }
            if (!KeyItems.TryGetValue(key, out int index))
                return - 1;
            return index;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool FindIndex(TKey key, out int index)
        {
            index = -1;
            if (key == null)
            {
                return false;
            }
            if (!KeyItems.TryGetValue(key, out index))
            {
                return false;
            }
            return true;
        }

        int IList<TItem>.IndexOf(TItem item)
        {
            FindIndex(KeyOf(item), out int index);
            return index;
        }
        #endregion

        #region EXISTS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual bool Exists(TKey key)
        {
            return KeyItems.ContainsKey(key);
        }
        bool ICollection<TItem>.Contains(TItem item) =>
            Exists(KeyOf(item));
        bool IFind<TKey, TItem>.Contains(TItem item) =>
            Exists(KeyOf(item));
        bool IExist<TItem>.Exists(TItem item) =>
            Exists(KeyOf(item));
        #endregion

        #region CLEAR
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Clear()
        {
            Array.Clear(Data, 0, Length);
            Length = 0;
            KeyItems.Clear();
        }
        void IClearable<bool>.Clear(bool parameter) =>
            Clear();
        #endregion

        #region COPY TO
        void ICollection<TItem>.CopyTo(TItem[] array, int arrayIndex)
        {
            if (arrayIndex + Length > array.Length)
                Array.Resize(ref array, arrayIndex + Length);
            Array.Copy(Data, 0, array, arrayIndex, Length);
        }
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

        #region DISPOSE
        public override void Dispose()
        {
            base.Dispose();
            Data = null;
        }
        #endregion
        sealed class KeyList : IReadOnlyList<TKey>
        {
            _MiniKeyCollection<TKey, TItem> Source;

            public KeyList(_MiniKeyCollection<TKey, TItem> source)
            {
                Source = source;
            }

            public TKey this[int index] =>
                Source.KeyOf(Source[index]);

            public int Count => Source.Count;

            public IEnumerator<TKey> GetEnumerator()
            {
                foreach (var item in Source)
                    yield return Source.KeyOf(item);
            }
            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }
    }
    #endregion
}
