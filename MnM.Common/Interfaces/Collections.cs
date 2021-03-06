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
    #region ISPAN
    /// <summary>
    /// Interface ISpan
    /// </summary>
    public interface ISpan : IReadOnlyList<int>
    {
        /// <summary>
        /// 
        /// </summary>
        int Start { get; set; }

        /// <summary>
        /// 
        /// </summary>
        int End { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(int item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Contains(ISpan range);
    }
    #endregion

    #region PAIR
    /// <summary>
    /// Interface IPair
    /// </summary>
    /// <seealso cref="System.IComparable" />
    public interface IPair : IComparable
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        object Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; set; }

        /// <summary>
        /// Gets the pseudo keys.
        /// </summary>
        /// <value>The pseudo keys.</value>
        IList PseudoKeys { get; }

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, IPair entity, MatchBy option);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity1">The entity1.</param>
        /// <param name="entity2">The entity2.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(MultCriteria criteria, IPair entity1, IPair entity2, MatchBy option);

        /// <summary>
        /// Determines whether [has type equal] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if [has type equal] [the specified other]; otherwise, <c>false</c>.</returns>
        bool HasTypeEqual(IPair other);
    }

    /// <summary>
    /// Interface IPair
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <seealso cref="MnM.Collections.IPair" />
    /// <seealso cref="System.IComparable{MnM.Collections.IPair{K, V}}" />
    /// <seealso cref="System.IEquatable{MnM.Collections.IPair{K, V}}" />
    public interface IPair<K, V> : IPair, IComparable<IPair<K, V>>, IEquatable<IPair<K, V>>
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        new K Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        new V Value { get; set; }

        /// <summary>
        /// Gets the pseudo keys.
        /// </summary>
        /// <value>The pseudo keys.</value>
        new IList<K> PseudoKeys { get; }

        /// <summary>
        /// Gets a value indicating whether [read only key].
        /// </summary>
        /// <value><c>true</c> if [read only key]; otherwise, <c>false</c>.</value>
        bool ReadOnlyKey { get; }

        /// <summary>
        /// Gets a value indicating whether [read only value].
        /// </summary>
        /// <value><c>true</c> if [read only value]; otherwise, <c>false</c>.</value>
        bool ReadOnlyValue { get; }

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, IPair<K, V> entity, MatchBy option);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity1">The entity1.</param>
        /// <param name="entity2">The entity2.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(MultCriteria criteria, IPair<K, V> entity1, IPair<K, V> entity2, MatchBy option);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(MultCriteria criteria, K item1, K item2);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, K item);

        /// <summary>
        /// Matches the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(K item);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="val1">The value1.</param>
        /// <param name="val2">The value2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(MultCriteria criteria, V val1, V val2);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="val">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, V val);

        /// <summary>
        /// Matches the specified item.
        /// </summary>
        /// <param name="val">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(V val);

        /// <summary>
        /// Matches the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(IEnumerable<K> items);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="keys">The keys.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, IEnumerable<K> keys);

        /// <summary>
        /// Determines whether [has type equal] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if [has type equal] [the specified other]; otherwise, <c>false</c>.</returns>
        bool HasTypeEqual(IPair<K, V> other);
    }
    #endregion

    #region READONLY LIST
    public interface IReadOnlyList : IEnumerable
    {
        /// <summary>
        /// Gets the number of elements contained in the Collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        // Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns></returns>
        object this[int index] { get; }
    }
    #endregion

    #region IARRAY<T>
    /// <summary>
    /// Interface IArray
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IArray<T>
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        T[] Data { get; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }

        void Resize(int length);
    }
    #endregion

    #region IREADONLYLIST<K, V, T>
    public interface IReadOnlyList<K, V, T> : IReadOnlyList<T> where T : IPair<K, V>
    { }
    #endregion

    #region IITERATOR<T>
    public interface IIterator<T> : IReadOnlyList<T>, IReadOnlyList
    {
        #region PROPERTIES
        /// <summary>
        /// Gets the number of elements contained in the Collection.
        /// </summary>
        new int Count { get; }

        /// <summary>
        // Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns></returns>
        new T this[int index] { get; set; }

        bool IsReadOnly { get; }

        T First { get; }

        T Last { get; }
        #endregion

        #region TO ARRAY
        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns>T[].</returns>
        T[] ToArray();

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns>T[].</returns>
        T[] ToArray(ISpan range);
        #endregion

        #region INDEX OK   
        /// <summary>
        /// Verifies the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="System.Exception">Index is out of bound of array</exception>
        bool IndexOK(int index);
        #endregion

        #region COPY TO    
        int CopyTo(ref T[] target, ISpan range, int arrayIndex);
        #endregion
    }
    #endregion

    #region ILEXICON
    public interface ILexicon<K, V, T> :
        ICollection<T>, IIterator<T>, IReadOnlyList<K, V, T> where T : IPair<K, V>
    {
        #region PROPERTIES
        new int Count { get; }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        IIterator<K> Keys { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        IIterator<V> Values { get; }
        #endregion
    }
    #endregion

    #region IIOBJECT- DICTIONARY
    public interface IObjDictionary<TObj, TKey> : IEnumerable<TObj>, IDisposable where TObj : IID<TKey>
    {
        #region COUNT OF
        /// <summary>
        /// Returns a count of specified object type in Factory's object store.
        /// </summary>
        /// <typeparam name="T">Any TObj type which implements IID<TKey> interface.</typeparam>
        /// <returns></returns>
        int CountOf<T>() where T : TObj;

        /// <summary>
        /// Returns a count of specified object type in Factory's object store using filter applied through condition specified.
        /// </summary>
        /// <typeparam name="T">Any TObj type which implements IID<TKey> interface.</typeparam>
        /// <param name="condition">A condition to filter objects before counting</param>
        /// <returns></returns>
        int CountOf<T>(Predicate<T> condition) where T : TObj;
        #endregion

        #region CONTAINS
        /// <summary>
        /// Tests if the specified key exists in a storage unit assigned for specified type.
        /// </summary>
        /// <param name="key">Key to search storage for</param>
        /// <returns></returns>
        bool Contains(TKey key);
        #endregion

        #region REPLACE
        /// <summary>
        /// Replace a current version of object with specified one. Search is performed using id of object.
        /// This mkaes sure updated version of the instance is held in GWS storage unit for a particular type.
        /// </summary>
        /// <param name="obj">Storeable object which has to be the replaced value of whatever is stored in storage unit now</param>
        void Replace(TObj obj);
        #endregion

        #region ADD - REMOVE
        /// <summary>
        /// Adds object to the specified storage unit in factory.
        /// </summary>
        /// <param name="obj">Object to store</param>
        U Add<U>(U obj) where U : TObj;

        /// <summary>
        /// Removes object specified from the specified store unit in factory.
        /// </summary>
        /// <param name="obj">Object to remove</param>
        bool Remove(TObj obj);

        /// <summary>
        /// Removes object after finding it using key as id from the specified storage unit in factory.
        /// </summary>
        /// <param name="id"></param>
        bool Remove(TKey id);
        #endregion

        #region GET SINGLE
        /// <summary>
        /// Gets the exisiting object with agiven key from the storage unit of specified type from factory. 
        /// </summary>
        /// <typeparam name="U">Any TObj type which implements IID<TKey> interface.</typeparam>
        /// <param name="key">Key by which object will be identified in the storage</param>
        /// <returns></returns>
        U Get<U>(TKey key) where U : TObj;

        /// <summary>
        /// Gets the exisiting object with agiven key from the storage unit of specified type from factory. 
        /// </summary>
        /// <param name="key">Key by which object will be identified in the storage</param>
        /// <returns></returns>
        TObj Get(TKey key);

        /// <summary>
        /// Gets the exisiting object with agiven key from the storage unit of specified type from factory. 
        /// </summary>
        /// <typeparam name="U">Any TObj type which implements IID<TKey> interface.</typeparam>
        /// <param name="key"></param>
        /// <param name="obj">Object returned if found</param>
        /// <returns>True if found otherwise false</returns>
        bool Get<U>(TKey key, out U obj) where U : TObj;

        /// <summary>
        /// Gets the exisiting object with agiven key from the storage unit of specified type from factory. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj">Object returned if found</param>
        /// <returns>True if found otherwise false</returns>
        bool Get(TKey key, out TObj obj);

        /// <summary>
        /// Gets the first exisiting object which satisfies a condition specified from the storage unit of specified type of factory. 
        /// </summary>
        /// <typeparam name="U">Any TObj type which implements IID<TKey> interface.</typeparam>
        /// <param name="condition">A condition which must be satisfied by objects to get qualified</param>
        /// <returns></returns>
        U Get<U>(Predicate<U> condition) where U : TObj;

        /// <summary>
        /// Gets the first exisiting object which satisfies a condition specified from the storage unit of specified type of factory. 
        /// </summary>
        /// <param name="condition">A condition which must be satisfied by objects to get qualified</param>
        /// <returns></returns>
        TObj Get(Predicate<TObj> condition);
        #endregion

        #region GET ALL
        /// <summary>
        /// Gets the exisiting object which satisfy a condition specified from the storage unit of specified type from factory. 
        /// </summary>
        /// <typeparam name="U">Any TObj type which implements IID<TKey> interface.</typeparam>
        /// <param name="condition">A condition which must be satisfied by objects to get qualified</param>
        /// <returns>collection of qualified objects</returns>
        IEnumerable<U> GetAll<U>(Predicate<U> condition) where U : TObj;

        /// <summary>
        /// Gets the exisiting object which satisfy a condition specified from the storage unit of specified type from factory. 
        /// </summary>
        /// <param name="condition">A condition which must be satisfied by objects to get qualified</param>
        /// <returns>collection of qualified objects</returns>
        IEnumerable<TObj> GetAll(Predicate<TObj> condition);
        #endregion
    }
    public interface IObjDictionary<T> : IObjDictionary<T, uint> where T : IID
    { }
    #endregion

    #region IGWSCOLLECTION<T>
    public interface IGwsCollection<T> : IList<T>, IIterator<T>
    {
        #region PROPERTIES
        int Capacity { get; set; }
        #endregion

        #region INSERT RANGE
        void InsertRange(int index, IEnumerable<T> items);
        #endregion

        #region REMOVE LAST
        void RemoveLast();
        #endregion

        #region ADD RANGE
        void AddRange(IEnumerable<T> items);
        #endregion

        #region COPY TO
        void CopyTo(T[] array, int arrayIndex, int length);
        #endregion

        #region SORT
        void Sort();
        void Sort(IComparer<T> comparer);
        void Sort(Comparison<T> comparer);
        #endregion

        #region TRIM
        void Trim();
        #endregion
    }
    #endregion

    #region IPROXY-COLLECTION
    public interface IProxyCollection<T, U> : IReadOnlyList<T>, IArray<T>
    {
        void Add(U subItem);
        bool Remove(U subItem);
        void RemoveAt(int index);
        void AddRange(IEnumerable<U> subItems);
        void AddRange(params U[] subItems);
        int IndexOf(U subItem);
        void Clear();
    }
    #endregion

    #region ILIMITED-COLLECTION
    public interface IMiniCollection<T> : IReadOnlyList<T>, IArray<T>
    {
        void Add(T item);
        bool Remove(T item);
        void RemoveAt(int index);
        void AddRange(IEnumerable<T> item);
        void AddRange(params T[] items);
        int IndexOf(T item);
        bool Contains(T item);
        void Clear();
    }
    #endregion

    #region IKEYCOLLECITON
    public interface IKeyCollection<TKey, TITem>: IMiniCollection<TITem>
    {
        TITem FindItem(TKey key);
    }
    #endregion

#if Collections
    #region IITEMADDRESS
    public interface IItemAddress
    {
        /// <summary>
        /// Gets the index of the item.
        /// </summary>
        /// <value>The index of the item.</value>
        int ItemIndex { get; }

        /// <summary>
        /// Gets the index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        int PageIndex { get; }

        /// <summary>
        /// Gets the index of the visible.
        /// </summary>
        /// <value>The index of the visible.</value>
        int VisibleIndex { get; }
    }
    #endregion

    #region IFILTER
    /// <summary>
    /// Interface IFilter
    /// </summary>
    /// <seealso cref="MnM.Collections.IIterator" />
    public interface IFilter : IReadOnlyList
    {
        /// <summary>
        /// Applies the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value">The value.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        void Apply(Criteria criteria, object value, bool invert = false);

        /// <summary>
        /// Applies the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        void Apply(MultCriteria criteria, object value1, object value2, bool invert = false);
    }
    #endregion

    #region IFILTER<T>
    public interface IFilter<T> : IIterator<T>, IFilter
    {
    #region PROPERTIES
        /// <summary>
        /// Gets a value indicating whether this <see cref="AbstractFilter{T}"/> is applied.
        /// </summary>
        /// <value><c>true</c> if applied; otherwise, <c>false</c>.</value>
        bool Applied { get; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        IList<T> Source { get; set; }

        /// <summary>
        /// Gets or sets the indices.
        /// </summary>
        /// <value>The indices.</value>
        int[] Results { get; set; }

        /// <summary>
        /// Gets or sets the index of the current.
        /// </summary>
        /// <value>The index of the current.</value>
        int CurrentIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the current.
        /// </summary>
        /// <value>The index of the current.</value>
        int? NextStrart { get; }
    #endregion

    #region APPLY
        /// <summary>
        /// Applies the specified indexer.
        /// </summary>
        /// <param name="indexer">The indexer.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        void Apply(int[] indexer, bool invert = false);

        /// <summary>
        /// Applies the specified indexer.
        /// </summary>
        /// <param name="indexer">The indexer.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        void Apply(IList<int> indexer, bool invert = false);

        /// <summary>
        /// Applies the specified start.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        void Apply(int start, int count, bool invert = false);

        /// <summary>
        /// Applies the specified range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        void Apply(Span range, bool invert = false);

        /// <summary>
        /// Applies the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value">The value.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        void Apply(Criteria criteria, T value, bool invert = false);

        /// <summary>
        /// Applies the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        void Apply(MultCriteria criteria, T value1, T value2, bool invert = false);
    #endregion

    #region INVERT
        /// <summary>
        /// Inverts this instance.
        /// </summary>
        void Invert();
    #endregion

    #region CLEAR
        /// <summary>
        /// Clears the specified raise change event.
        /// </summary>
        /// <param name="raiseChangeEvent">if set to <c>true</c> [raise change event].</param>
        void Clear(bool raiseChangeEvent = true);
    #endregion

    #region LOOKUP INDICES
        IEnumerable<IItemAddress> LookupIndices();
    #endregion

    #region GET ACTUAL INDEX
        /// <summary>
        /// Gets the actual index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.Int32.</returns>
        int GetActualIndex(int index);
    #endregion

    #region NEW
        /// <summary>
        /// News the specified indexer.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="indexer">The indexer.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        /// <returns>TFilter.</returns>
        TFilter New<TFilter>(int[] indexer, bool invert = false)
            where TFilter : IFilter<T>;

        /// <summary>
        /// News the specified start.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        /// <returns>TFilter.</returns>
        TFilter New<TFilter>(int start, int count, bool invert = false)
            where TFilter : IFilter<T>;

        /// <summary>
        /// News the specified range.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="range">The range.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        /// <returns>TFilter.</returns>
        TFilter New<TFilter>(ISpan range, bool invert = false)
            where TFilter : IFilter<T>;

        /// <summary>
        /// News the specified criteria.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value">The value.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        /// <returns>TFilter.</returns>
        TFilter New<TFilter>(Criteria criteria, T value, bool invert = false)
            where TFilter : IFilter<T>;

        /// <summary>
        /// News the specified criteria.
        /// </summary>
        /// <typeparam name="TFilter">The type of the t filter.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="invert">if set to <c>true</c> [invert].</param>
        /// <returns>TFilter.</returns>
        TFilter New<TFilter>(MultCriteria criteria, T value1, T value2, bool invert = false)
            where TFilter : IFilter<T>;
    #endregion
    }
    #endregion

    #region IITEMLIST<T>
    public interface IItemList<T> : IIterator<T>, IList<T>
#if Advanced
        , ICloneable
#endif
    {
    #region PROPERTIES
        /// <summary>
        /// Gets the number of elements contained in the Collection.
        /// </summary>
        new int Count { get; }

        /// <summary>
        // Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns></returns>
        new T this[int index] { get; set; }

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        /// <exception cref="System.Exception">capacity can not be less than existing count</exception>
        int Capacity { get; set; }

#if Advanced
        IFilter<T> Filter { get; }
#endif
    #endregion

    #region ADD
        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        void AddRange(params T[] collection);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        void AddRange(IEnumerable<T> collection);
    #endregion

    #region INSERT RANGE
        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="collection">The collection.</param>
        void InsertRange(int position, IEnumerable<T> collection);

        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="collection">The collection.</param>
        void InsertRange(int position, params T[] collection);
    #endregion

    #region REMOVE ALL
        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>System.Int32.</returns>
        int RemoveAll(Predicate<T> match);

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>System.Int32.</returns>
        int RemoveAll(params T[] items);
    #endregion

    #region SORT
        void Sort();

        /// <summary>
        /// Sorts the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="System.Exception">Comparer is null</exception>
        void Sort(IComparer<T> comparer);

        /// <summary>
        /// Sorts the specified comparison.
        /// </summary>
        /// <param name="comparison">The comparison.</param>
        /// <exception cref="System.Exception">Comparison object is null</exception>
        void Sort(Comparison<T> comparison);
    #endregion

    #region REVERSE
        /// <summary>
        /// Reverses this instance.
        /// </summary>
        void Reverse();
    #endregion

    #region TRIM
        /// <summary>
        /// Trims the excess.
        /// </summary>
        void Trim();
    #endregion
    }
    #endregion

    #region ICHECKLIST<T>
#if Advanced
    public interface ICheckList<T> : IItemList<T>
    {
    #region PROPERTIES
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AbstractList{T}"/> is distinctive.
        /// </summary>
        /// <value><c>true</c> if distinctive; otherwise, <c>false</c>.</value>
        bool Distinctive { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [check flag enabled].
        /// </summary>
        /// <value><c>true</c> if [check flag enabled]; otherwise, <c>false</c>.</value>
        bool CheckFlagEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether [pending update].
        /// </summary>
        /// <value><c>true</c> if [pending update]; otherwise, <c>false</c>.</value>
        bool PendingUpdate { get; }
    #endregion

    #region CHECK-UNCHECK
        /// <summary>
        /// Gets the checked.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool GetChecked(int index);

        /// <summary>
        /// Sets the checked.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        void SetChecked(int index, bool value);

        /// <summary>
        /// Checkeds the indices.
        /// </summary>
        /// <returns>System.Int32[].</returns>
        int[] CheckedIndices();

        /// <summary>
        /// Checkeds the items.
        /// </summary>
        /// <returns>T[].</returns>
        T[] CheckedItems();

        /// <summary>
        /// Uns the check all.
        /// </summary>
        void UnCheckAll();

        /// <summary>
        /// Checks all.
        /// </summary>
        void CheckAll();

        /// <summary>
        /// Toggles the checked.
        /// </summary>
        void ToggleChecked();
    #endregion

    #region ADD
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="Checked">if set to <c>true</c> [checked].</param>
        void Add(T value, bool Checked);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="Checked">if set to <c>true</c> [checked].</param>
        void AddRange(IEnumerable<T> collection, bool Checked);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="length">The length.</param>
        void AddRange(IEnumerable<T> collection, int itemIndex, int length);

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <param name="length">The length.</param>
        /// <param name="Checked">if set to <c>true</c> [checked].</param>
        void AddRange(IEnumerable<T> collection, int itemIndex, int length, bool Checked);
    #endregion

    #region INSERT RANGE
        /// <summary>
        /// Inserts the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="value">The value.</param>
        /// <param name="Checked">if set to <c>true</c> [checked].</param>
        void Insert(int position, T value, bool Checked);

        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="Checked">if set to <c>true</c> [checked].</param>
        void InsertRange(int position, IEnumerable<T> collection, bool Checked);

        /// <summary>
        /// Inserts the range.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="Checked">if set to <c>true</c> [checked].</param>
        /// <param name="collection">The collection.</param>
        void InsertRange(int position, bool Checked, params T[] collection);
    #endregion

    #region REMOVE ALL
        /// <summary>
        /// Removes the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        void Remove(Criteria criteria, T item);

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="count">The count.</param>
        void RemoveRange(int position, int count);

        /// <summary>
        /// Removes the range from.
        /// </summary>
        /// <param name="position">The position.</param>
        void RemoveRangeFrom(int position);

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <param name="range">The range.</param>
        void RemoveRange(ISpan range);

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        /// <returns>System.Int32.</returns>
        int RemoveAll(Criteria criteria, T item);

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <returns>System.Int32.</returns>
        int RemoveAll(MultCriteria criteria, T item1, T item2);
    #endregion

    #region REVERSE
        /// <summary>
        /// Reverses the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        void Reverse(int index, int count);
    #endregion

    #region REFRESH
        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        void Refresh();
    #endregion
    }
#endif
    #endregion
#endif

#if (GWS || Window)
    #region IOBJECT-COLLECTION
    /// <summary>
    /// Object containing a collection of objects of type T.
    /// </summary>
    /// <typeparam name="T">Type of Object used in collection.</typeparam>
    public interface IObjCollection<T> : IEnumerable<T>, IDisposable where T : IRenderable
    {
        #region PROPERTIES
        /// <summary>
        /// Returns number of objects in collection.
        /// </summary>
        int ObjectCount { get; }

        /// <summary>
        /// Returns object from collection with given ID.
        /// </summary>
        /// <param name="id">ID of object to return.</param>
        /// <returns></returns>
        T this[uint id] { get; }

        /// <summary>
        /// Returns object from collection with given ID.
        /// </summary>
        /// <param name="name">Name of object to return.</param>
        /// <returns></returns>
        T this[string name] { get; }
        #endregion

        #region CONTAINS
        /// <summary>
        /// Tests for existence of object T in collection.
        /// </summary>
        /// <param name="item">Object to test for.</param>
        /// <returns>True if object present in collection.</returns>
        bool Contains(T item);

        /// <summary>
        /// Tests objects in the collection for matching object ID 
        /// </summary>
        /// <param name="itemID">ID of object required.</param>
        /// <returns>True if object present.</returns>
        bool Contains(uint itemID);
        #endregion

        #region ADD
        /// <summary>
        /// Adds a shape object to this collection.
        /// </summary>
        /// <typeparam name="T">A shape object to be added of type specifie by T</typeparam>
        /// <param name="settings">The settings context for the shape.</param>
        /// <returns>Returns the same Shape object which is added . 
        /// this lets user to pass something like new shape(....) and then used it further more.
        /// </returns></returns>
        U Add<U>(U shape, ISettings settings, bool? suspendUpdate = null) where U : T;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="U">A shape object to be added of type specifie by T</typeparam>
        /// <param name="shape"></param>
        /// <returns></returns>
        U Add<U>(U shape) where U : T;

        /// <summary>
        /// Adds a shape objects to this collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controls">Shape objects to add.</param>
        void AddRange<U>(IEnumerable<U> controls) where U : T;
        #endregion

        #region REMOVE
        /// <summary>
        /// Remove specified object from collection (if allowed).
        /// </summary>
        /// <param name="item">Object to remove.</param>
        bool Remove(T item);

        /// <summary>
        /// Remove all values from the collection.
        /// </summary>
        void RemoveAll();
        #endregion
    }

    /// <summary>
    /// Object containing a collection of objects of type IRenderable.
    /// </summary>
    public partial interface IObjCollection : IObjCollection<IRenderable>
    {
        /// <summary>
        /// Returs an existing drawing information object which holds latest drawing information for a given element.
        /// </summary>
        /// <param name="shape">The element for which the drawing information is sought for</param>
        /// <returns></returns>
        ISettings this[IRenderable shape] { get; }

        #region METHODS
        /// <summary>
        /// Gets all elements in this colleciton satisfying given condition.
        /// </summary>
        /// <typeparam name="T">Type of IRenderable.</typeparam>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <returns></returns>
        IEnumerable<IRenderable> Query(Predicate<ISettings> condition = null);

        /// <summary>
        /// Gets the first element in this colleciton satisfying given condition.
        /// </summary>
        /// <typeparam name="T">Type of IRenderable.</typeparam>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <returns></returns>
        IRenderable QueryFirst(Predicate<ISettings> condition = null);

        /// <summary>
        /// Gets all drawn information in this colleciton satisfying given condition.
        /// </summary>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <returns></returns>
        IEnumerable<IShape> QueryDraw(Predicate<ISettings> condition = null);

        /// <summary>
        /// Gets drawn information of first element in this colleciton satisfying given condition.
        /// </summary>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <returns></returns>
        IShape QueryFirstDraw(Predicate<ISettings> condition = null);
        #endregion
    }
    #endregion

    #region ICONTROL COLLECTION
    /// <summary>
    /// Represents an object which holds a control collection to maintain its child controls.
    /// </summary>
    public partial interface IContainer : IObjCollection, IRefreshable
    {       
    }
    #endregion

    #region IANIMATIONCOLLECTION
    public interface IAnimations : IKeyCollection<uint, IAnimation>, ITimerBase
    {
        IAnimationHost Host { get; }
        int RefreshInterval { get; }

        event EventHandler<IEventArgs<long>> AnimationLoopComplete;
    }
    #endregion
#endif
}
