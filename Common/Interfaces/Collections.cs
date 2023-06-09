/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections;
using System.Collections.Generic;
#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif

namespace MnM.GWS
{
    #region IQUERABLELIST<T>
    public interface IQueryableCollection<T> : IReadOnlyList, IReadOnlyList<T>, IReverseIEnumerable<T>, IIndexer<T>, IQueryable<T>, IQueryable, ICount, IDisposable
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        new T this[int index] { get; }

        /// <summary>
        /// Gets the count of elements in this object.
        /// </summary>
        new int Count { get; }

        /// <summary>
        /// Gets readonly status of this list i.e if items are replaceable or not.
        /// </summary>
        bool IsReadOnly { get; }
    }
    #endregion

    #region IMINICOLLECTION<T>
    public interface IMiniCollection<T> : IQueryableCollection<T>, ICount,
        IAdd<T>, IAddRange<T>, ISwap<T>, IRemove<T>, IExist<T>
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        new T this[int index] { get; set; }

    }
    #endregion

    #region IOBSERVABLE-LIST
    public interface IObservableList<T>: IReadOnlyList<T>
    {
        /// <summary>
        /// Occurs when list changed.
        /// </summary>
        event EventHandler<IListChangeEventArgs> ListChanged;
    }
    #endregion

    #region IQUERYABLE<T>
    public interface IQueryable
    {
        #region QUERY
        /// <summary>
        /// Gets all elements in this collection satisfying given condition.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <param name="reverse">If true returns result in descending order of indices of elements.</param>
        /// <param name="conditionalIndex">Conditional Index to be used for further query this collection using index of each element.</param>
        /// <param name="conditionalIndexCriteria">Conditional index comparison criteria.</param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(Predicate<T> condition = null, bool reverse = false,
            int? conditionalIndex = null, NumCriteria? conditionalIndexCriteria = null);
        #endregion
    }
    #endregion

    #region IQUERYABLE<T>
    public interface IQueryable<T>
    {
        #region QUERY
        /// <summary>
        /// Gets all elements in this collection satisfying given condition.
        /// </summary>
        /// <typeparam name="T">Type of element.</typeparam>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <param name="reverse">If true returns result in descending order of indices of elements.</param>
        /// <param name="conditionalIndex">Conditional Index to be used for further query this collection using index of each element.</param>
        /// <param name="conditionalIndexCriteria">Conditional index comparison criteria.</param>
        /// <returns></returns>
        IEnumerable<T> Query(Predicate<T> condition = null, bool reverse = false,
            int? conditionalIndex = null, NumCriteria? conditionalIndexCriteria = null);
        #endregion
    }
    #endregion

    #region ICOUNT
    public interface ICount
    {
        /// <summary>
        /// Gets the count of elements in this object.
        /// </summary>
        int Count { get; }
    }
    #endregion

    #region ICOUNTER
    public interface ICounter: ICount
    {
        /// <summary>
        /// Updates count with new value.
        /// </summary>
        /// <param name="newValue">New value which the count should be updated with.</param>
        void Update(int newValue);

        /// <summary>
        /// Replaces count with new value.
        /// </summary>
        /// <param name="newValue">New value which the count should be replaced with.</param>
        void Reset(int newValue);
    }
    #endregion

    #region IINDEXER<T>
    public interface IIndexer<T>: ICount
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        T this[int index] { get; }
    }
    #endregion

    #region IINDEXER<T>
    public interface IExIndexer<T> : IIndexer<T>
    {
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index"> The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        new T this[int index] { get; set; }
    }
    #endregion

    #region READONLY LIST
    public interface IReadOnlyList : ICount, IEnumerable
    {
        /// <summary>
        // Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns></returns>
        object this[int index] { get; }
    }
    #endregion

    #region IREADONLYLIST<K, V, T>
    public interface IReadOnlyList<K, V, T> : IReadOnlyList<T> where T : IPair<K, V>
    { }
    #endregion

    #region IITERATOR
    /// <summary>
    /// 
    /// </summary>
    internal partial interface IIterator : IReadOnlyList, ICloneable 
    {
        Array ToArray();

        Array ToArray(ISpan range);
    }
    #endregion

    #region ISWAP<T>
    public interface ISwap<T>
    {
        /// <summary>
        /// Swaps the given item with existing item in this collection.
        /// </summary>
        /// <param name="oldItem">Existing item to perform swap operation.</param>
        /// <param name="newItem">New item to perform swap operation.</param>
        /// <returns>True if operation is successful otherwise not.</returns>
        bool Swap(T oldItem, T newItem);

        /// <summary>
        /// Changes ZOrder of given shape in this collection.
        /// </summary>
        /// <param name="item">Item which to change index for.</param>
        /// <param name="newIndex">New index for the item.</param>
        bool Relocate(T item, int newIndex);
    }
    #endregion

    #region SWAP<TKEY, TITEM>
    public interface ISwap<TKey, TItem>
    {
        /// <summary>
        /// Swaps items related to the given keys.
        /// </summary>
        /// <param name="key1">First key to find related item for.</param>
        /// <param name="key2">Second key to find related item for.</param>
        /// <returns>True if operation is successful otherwise false.</returns>
        bool Swap(TKey key1, TKey key2);

        /// <summary>
        /// Replace an item related to the given key with the specified item in this collection.
        /// </summary>
        /// <param name="key">Key to find related item.</param>
        /// <param name="item">Item to replace current item with.</param>
        /// <returns>True if operation is successful otherwise false.</returns>
        bool Replace(TKey key, TItem item);

        /// <summary>
        /// Changes ZOrder of given shape in this collection.
        /// </summary>
        /// <param name="newIndex">New index for the item with specified key.</param>
        /// <param name="key">Key of the item which to change index for.</param>
        bool Relocate(TKey key, int newIndex);
    }
    #endregion

    #region FIND<TKEY, TITEM>
    public interface IFind<TKey, TItem> 
    {
        /// <summary>
        /// Finds item related to the given key from this collection.
        /// </summary>
        /// <param name="key">Key to find related item.</param>
        /// <returns></returns>
        TItem Find(TKey key);

        /// <summary>
        /// Finds item related to the given key from this collection.
        /// </summary>
        /// <param name="key">Key to find related item.</param>
        /// <param name="item">returned item if found.</param>
        /// <returns>True if item is found otherwise false.</returns>
        bool Find(TKey key, out TItem item);

        /// <summary>
        /// Finds index of item related to the given key from this collection.
        /// </summary>
        /// <param name="key">Key to find index of related item.</param>
        /// <param name="nonZeroBased">If true, returns index increased by 1.</param>
        /// <returns>Index of the item for the given key.</returns>
        int FindIndex(TKey key);

        /// <summary>
        /// Finds index of item related to the given key from this collection.
        /// </summary>
        /// <param name="key">Key to find index of related item.</param>
        /// <returns>True if item is found otherwise false.</returns>
        bool FindIndex(TKey key, out int index);

        /// <summary> 
        /// Confirms whether the given key exits in this collection or not.
        /// </summary>
        /// <param name="key">Key to check existance for.</param>
        /// <returns></returns>
        bool Exists(TKey key);

        /// <summary>
        ///  Determines whether the collection contains a specific item or not.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(TItem item);
    }
    #endregion

    #region INDEX OF<T>
    public interface IIndexOf<T>
    {
        int IndexOf(T item);
        bool IndexOf(T item, out int index);
    }
    #endregion

    #region ADD<T>
    public interface IAdd<T>
    {
        /// <summary>
        /// Add specified item in this collection.
        /// </summary>
        /// <param name="item">Item to be added in this collection.</param>
        void Add(T item);

        /// <summary>
        /// Inserts specified item at given position in this collection.
        /// </summary>
        /// <param name="position">Position which the item should be inserted into this collection at.</param>
        /// <param name="item"></param>
        void Insert(int position, T item);
    }
    #endregion

    #region ADD<TITEM, TSUBITEM>
    public interface IAdd<TItem, TSubItem>: IAdd<TItem> 
    {
        /// <summary>
        /// Add specified item in this collection.
        /// </summary>
        /// <param name="item">Item to be added in this collection.</param>
        void Add(TSubItem item);

        /// <summary>
        /// Inserts specified item at given position in this collection.
        /// </summary>
        /// <param name="position">Position which the item should be inserted into this collection at.</param>
        /// <param name="item"></param>
        void Insert(int position, TSubItem item);
    }
    #endregion

    #region ADD RANGE<T>
    public interface IAddRange<T>
    {
        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void AddRange(IEnumerable<T> items);

        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void AddRange(params T[] items);
    }
    #endregion

    #region ADD RANGE<T, U>
    public interface IAddRange<TItem, TSubItem>: IAddRange<TItem>
    {
        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void AddRange(IEnumerable<TSubItem> items);

        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void AddRange(params TSubItem[] items);
    }
    #endregion

    #region INSERT RANGE<T>
    public interface IInsertRange<T>
    {
        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="position">Position which the item should be inserted into this collection at.</param>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void InsertRange(int position, IEnumerable<T> items);

        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="position">Position which the item should be inserted into this collection at.</param>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void InsertRange(int position, params T[] items);
    }
    #endregion

    #region INSERT RANGE<T, U>
    public interface IInsertRange<TItem, TSubItem>
    {
        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="position">Position which the item should be inserted into this collection at.</param>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void InsertRange(int position, IEnumerable<TSubItem> items);

        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="position">Position which the item should be inserted into this collection at.</param>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void InsertRange(int position, params TSubItem[] items);
    }
    #endregion

    #region IREPLACE<T>
    public interface IReplace<T>
    {
        /// <summary>
        /// Replaces the existing item with the specified item in this collection.
        /// </summary>
        /// <param name="oldSubItem">Existing item to replace.</param>
        /// <param name="newSubItem">New item to replace.</param>
        /// <returns>True if operation is successful otherwise not.</returns>
        bool Replace(T oldSubItem, T newSubItem);
    }
    #endregion

    #region REMOVE<TITEM>
    public interface IRemove<T>: IClearable<bool>
    {
        /// <summary>
        /// Removes the first occurrence of a specific object from this collection.
        /// </summary>
        /// <param name="item">The object to remove from this list.</param>
        /// <returns>
        /// true if item was successfully found and then removed from this list;
        /// otherwise, false.
        /// </returns>
        bool Remove(T item);

        /// <summary>
        /// Removes item located at given index in this collection.
        /// </summary>
        /// <param name="index">Position of item to be removed.</param>
        bool RemoveAt(int index);

        /// <summary>
        /// Removes the last item in this collection.
        /// </summary>
        void RemoveLast();
    }
    #endregion

    #region REMOVE<TKEY, TITEM>
    public interface IRemove<TKey, TItem> : IRemove<TItem>
    {
        /// <summary>
        /// Remvoes item related to the given key from this collection.
        /// </summary>
        /// <param name="key">Key to find related item.</param>
        /// <returns></returns>
        bool RemoveByKey(TKey key);
    }
    #endregion

    #region REMOVE RANGE<T>
    public interface IRemoveRange<T>
    {
        void RemoveRange(int startIndex, int count);
        void RemoveAll(params T[] items);
    }
    #endregion
     
    #region KEYVALUESETTER
    public interface IKeyValueSetter<TKey, TValue>
    {
        TValue this[TKey key, bool silent = true] { set; }
        T Get<T>(TKey key);
    }
    #endregion

    #region IREVERSE-ENUMERATION
    /// <summary>
    /// Represents a collection which can be enumerated in reverse.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReverseIEnumerable<T>
    {
        IEnumerable<T> InReverse();
    }
    #endregion

    #region IEXIST<T>
    public interface IExist<T>
    {
        /// <summary>
        /// Lets the user know if given item belongs to this object.
        /// </summary>
        /// <param name="item">Item to check for existence in this object.</param>
        /// <returns>True if item exist otherwise false.</returns>
        bool Exists(T item);
    }
    #endregion

    #region ICAPACITY
    public interface IResizeUnit
    {
        /// <summary>
        /// Gets or sets number by which internal array of list need to grow if it reaches existing capacity.
        /// </summary>
        int ResizeUnit { get; set; }

        /// <summary>
        /// Sets length of internal array in this list to 0. 
        /// </summary>
        void MakeZeroLength();
    }
    #endregion
}
