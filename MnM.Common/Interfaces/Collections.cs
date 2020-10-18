/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
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
        U Add<U>(U obj) where U: TObj;

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
    public interface IObjDictionary<T> : IObjDictionary<T, string> where T : IID
    { }
    #endregion

    #region IGWSCOLLECTION<T>
    public interface IGwsCollection<T>: IList<T>, IIterator<T>
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
    public interface IObjCollection<T> : IEnumerable<T>, IDisposable where T : IID
    {
        /// <summary>
        /// Gets parent container which this collection belongs to.
        /// </summary>
        ISurface Parent { get; }

        /// <summary>
        /// Returns number of objects in collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns object from collection with given ID.
        /// </summary>
        /// <param name="id">ID of object to return.</param>
        /// <returns></returns>
        T this[string id] { get; }

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
        bool Contains(string itemID);

        /// <summary>
        /// Adds a shape object to this collection.
        /// </summary>
        /// <typeparam name="T">A shape object to be added of type specifie by TShape</typeparam>
        /// <param name="context">The drawing context for the shape i.e a pen or color or a brush or even an another graphics or buffer object from which a data can be read.</param>
        /// <returns>Returns the same Shape object which is added . 
        /// this lets user to pass something like new shape(....) and then used it further more.
        /// for example: var ellipse = Add(Factory.newEllipse(10,10,100,200), Colour.Red, null, null);
        /// </returns>
        U Add<U>(U shape, IReadContext context) where U : T;

        /// <summary>
        /// Add object to collection (if allowed).
        /// </summary>
        /// <param name="item">Object to add to collection.</param>
        U Add<U>(U item) where U : T;

        /// <summary>
        /// Adds a shape objects to this collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controls">Shape objects to add.</param>
        void AddRange<U>(IEnumerable<U> controls) where U : T;

        /// <summary>
        /// Remove specified object from collection (if allowed).
        /// </summary>
        /// <param name="item">Object to remove.</param>
        bool Remove(T item);

        /// <summary>
        /// Remove all values from the collection.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Object containing a collection of objects of type IRenderable.
    /// </summary>
    public interface IObjCollection : IObjCollection<IRenderable>, IID
#if Advanced
        , IEventPusher, IInteractable
#endif
    {
        #region PROPERTIES

        /// <summary>
        /// Retries drawing information such as last drawn area, fill mode, stroke mode etc. etc for a given element.
        /// </summary>
        /// <param name="shape">The element for which the drawing information is sought for</param>
        /// <returns></returns>
#if Advanced
        IDrawInfo2
#else
        IDrawInfo
#endif
            this[IRenderable shape]
        { get; }

        /// <summary>
        /// Indicates if the collection is currently adding an element or not.
        /// </summary>
        bool AddMode { get; }

        /// <summary>
        /// Gives enumearable of items held at the current page in this collection.
        /// Note: For standard version there is only one page so maintaing a something like a tab control is not possible in the version.
        /// Advanced version has support for multiple pages.
        /// </summary>
        IEnumerable<IRenderable> Items { get; }

#if Advanced
        IEnumerable<IDrawInfo2>
#else
        IEnumerable<IDrawInfo>
#endif
            InfoItems
        { get; }
        #endregion

        #region METHODS
        /// <summary>
        /// Draws any element on this path. This renderer has a built-in support for the following kind of elements:
        /// 1. IShape
        /// 2. IDrawable
        /// Please note that in case your element does not implement any of the above, you must provide your own rendering routine
        /// by overriding Render3 in GwsRenderer method. Once you have handled it return true otherwise an exception wiil be raised.
        /// </summary>
        /// <param name="shape">Element which is to be rendered</param>
        void Draw(IRenderable shape);

        /// <summary>
        /// Creates a new drawing information object for a given element after it is added in this collection to hold the current drawing information.
        /// </summary>
        /// <param name="shape">Element - which draw information is sought for.</param>
        /// <returns></returns>
#if Advanced
        IDrawInfo2
#else
        IDrawInfo
#endif
            NewDrawInfo(IRenderable shape);

        /// <summary>
        /// Creates a new drawing information object for an element which has given ID after it is added in this collection to hold the current drawing information.
        /// </summary>
        /// <param name="shapeID">ID of an element - which drawing information object is sought for.</param>
        /// <returns>IDrawInfo object.</returns>
#if Advanced
        IDrawInfo2
#else
        IDrawInfo
#endif
            NewDrawInfo(string shapeID);

        /// <summary>
        /// Returs an existing drawing information object for a given element after it is added in this collection to hold the current drawing information.
        /// </summary>
        /// <param name="shape">Element - which draw information is sought for.</param>
        /// <returns></returns>
#if Advanced
        IDrawInfo2
#else
        IDrawInfo
#endif
        GetInfo(string shape);

        /// <summary>
        /// Removes an element wchich has given ID from this collection if it exist.
        /// </summary>
        /// <param name="shapeID"></param>
        /// <returns></returns>
        bool Remove(string shapeID);

        /// <summary>
        /// Removes all existing elements from the current page of this collection.
        /// </summary>
        void RemoveAll();

        /// <summary>
        /// Re-display the given element in the current page of this collection.
        /// </summary>
        /// <param name="shape"></param>
        void Refresh(IRenderable shape);

        /// <summary>
        /// Gets all elements in this colleciton satisfying given condition.
        /// </summary>
        /// <typeparam name="T">Type of IRenderable.</typeparam>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(Predicate<
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
#endif
            > condition) where T : IRenderable;

        /// <summary>
        /// Gets all drawn information in this colleciton satisfying given condition.
        /// </summary>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <returns></returns>
        IList<IDrawnInfo> QueryDraw(Predicate<
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
#endif
            > condition);

        /// <summary>
        /// Gets the first element in this colleciton satisfying given condition.
        /// </summary>
        /// <typeparam name="T">Type of IRenderable.</typeparam>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <returns></returns>
        T QueryFirst<T>(Predicate<
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
#endif
            > condition) where T : IRenderable;

        /// <summary>
        /// Gets drawn information of first element in this colleciton satisfying given condition.
        /// </summary>
        /// <param name="condition">Condition to be satisfied.</param>
        /// <returns></returns>
        IDrawnInfo QueryFirstDraw(Predicate<
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
#endif
            > condition);

        #endregion

        #region ADVANCED VERSION
#if Advanced
        /// <summary>
        /// Returns the number of pages available in this collection.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Returns the index of the current page of this collection.
        /// </summary>
        int CurrentPage { get; }

        /// <summary>
        /// Gets th actual location when the drag operation started.
        /// </summary>
        Vector DragLocation { get; }

        /// <summary>
        /// Gets current status in relation to mouse dragging routine.
        /// </summary>
        MouseDrag MouseDrag { get; }

        /// <summary>
        /// Sets a page as specified by the index to be the active page int this collection.
        /// </summary>
        /// <param name="page">Index of the page intended to be the current one.</param>
        /// <param name="silent">Specifies whether or not it should do the change without any notification</param>
        void SetCurrentPage(int page, bool silent = false);

        /// <summary>
        /// Sets number of pages to be available in this collection.
        /// </summary>
        /// <param name="noOfPages">Numer of pages to be available for use</param>
        void SetPages(int noOfPages);

        /// <summary>
        /// Moves a given element in the collection to new x,y co-ordinates.
        /// </summary>
        /// <param name="shape">IElement to move.</param>
        /// <param name="drawX">Null or new x co-ordinate.</param>
        /// <param name="drawY">Null or new y co-ordinate.</param>
        void Move(IRenderable shape, int? drawX = null, int? drawY = null);

        /// <summary>
        /// Resize a given element in the collection.
        /// </summary>
        /// <param name="shape">Element to resize.</param>
        /// <param name="size">New size - representing same scale for width and height</param>
        /// <param name="clear">If true then it applies the background and thus wiping the drawing with in.</param>
        void Resize(IRenderable shape, float size, bool clear = false);

        /// <summary>
        /// Resize a given element in the collection.
        /// </summary>
        /// <param name="shape">Element to resize.</param>
        /// <param name="width">New width</param>
        /// <param name="width">New height</param>
        /// <param name="clear">If true then it applies the background and thus wiping the drawing with in.</param>
        void Resize(IRenderable shape, float width, float height, bool clear = false);

        /// <summary>
        /// Sets focus to a giave element in this collection so that it can receive user inputs.
        /// </summary>
        /// <param name="shape">An element to get focus</param>
        /// <returns>False if the element can not be focused.</returns>
        bool Focus(IRenderable shape);

        /// <summary>
        /// Bring the shape to front of the IContext supplied to it.
        /// </summary>
        /// <param name="shape">IElement to bring to front.</param>
        void BringToFront(IRenderable shape);

        /// <summary>
        /// Sends the Shape to the Back of the drawings on the IContext supplied.
        /// </summary>
        /// <param name="shape"></param>
        void SendToBack(IRenderable shape);

        /// <summary>
        /// Moves the give shape to another page of this collection.
        /// </summary>
        /// <param name="shape">shape which to change page number for.</param>
        /// <param name="newOrder">New page number for the shape.</param>
        bool MoveToPage(IRenderable shape, int pageNumber);

        /// <summary>
        /// Changes ZOrder of given shape in this collection.
        /// </summary>
        /// <param name="shape">shape which to change zorder for.</param>
        /// <param name="newOrder">New zorder for the shape.</param>
        int ChangeZOrder(IRenderable shape, int newOrder);

        /// <summary>
        /// Changes TabIndex of given shape in this collection.
        /// </summary>
        /// <param name="shape">Shape which to change zorder for.</param>
        /// <param name="newTabIndex">New zorder for the shape.</param>
        int ChangeTabIndex(IFocusable shape, int newTabIndex);

        /// <summary>
        /// Enable the interactive properties of the given Shape.
        /// </summary>
        /// <param name="shape">Shape to be given user input.</param>
        void Enable(IRenderable shape);

        /// <summary>
        /// Disable user interaction with the given shape.
        /// </summary>
        /// <param name="shape">IElement of shape that has user interaction disabled.</param>

        void Disable(IRenderable shape);

        /// <summary>
        /// Draw the given shape in the provided IContext.
        /// </summary>
        /// <param name="shape">IElement of the shape to be displayed</param>
        void Show(IRenderable shape);

        /// <summary>
        /// Hides the given shape in the provided IContext.
        /// </summary>
        /// <param name="shape">IElement of the shape to be removed from the IContext.</param>
        void Hide(IRenderable shape);

        /// <summary>
        /// Re-display all existing elements including those which are hidden in the current pageof this collection.
        /// </summary>
        void ShowAll();

        /// <summary>
        /// Hides all existing elments in the current page of this collection.
        /// </summary>
        void HideAll();

        /// <summary>
        /// Invokes an event whenever current page in this collection is changed.
        /// </summary>
        event EventHandler<IEventArgs> PageChanged;
#endif
        #endregion
    }
    #endregion

    #region ICONTROL COLLECTION
    /// <summary>
    /// Represents an object which holds a control collection to maintain its child controls.
    /// </summary>
    public interface IContainer: ISurface
    {
        /// <summary>
        /// A collection that keeps all child controls to be maintained.
        /// </summary>
        IObjCollection Controls { get; }
    }
    #endregion

#if Advanced
    #region IBUFFER-COLLECTION
    public interface IBuffers : IDisposable
    {
        /// <summary>
        /// Gets the number of buffers present in the collection.
        /// </summary>
        int BufferCount { get; }

        /// <summary>
        /// Gets index of currently active buffer or sets a buffer on a given index in the collection active. 
        /// If -1 is specified then main buffer gets activated as current buffer.
        /// </summary>
        int BufferIndex { get; }

        /// <summary>
        /// Adds new buffer in the collection.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        int AddBuffer();

        /// <summary>
        /// Removes buffer from the collection at a given buffer index.
        /// </summary>
        /// <param name="bufferIndex"></param>
        void RemoveBuffer(int index);

        /// <summary>
        /// Lets user to switch to a buffer at a given index. i.e. to make that buffer currently active buffer for the purpose of performing pixel operations.
        /// </summary>
        /// <param name="index"></param>
        void SwitchToBuffer(int index);

        /// <summary>
        /// Lets user to switch to the primary buffer which actually not part of the collection itself.
        /// i.e. to make the primary buffer currently active buffer for the purpose of performing pixel operations.
        /// </summary>
        void SwitchToMainBuffer();

        /// <summary>
        /// Resizes all buffers to match the size of primary buffer.
        /// </summary>
        void ResizeBuffers();

        /// <summary>
        /// Removes all buffers from the collection except the main buffer.
        /// </summary>
        void RemoveBuffers();
    }

    public interface IBufferCollection : IBuffers, IEnumerable<ICanvas>, IDisposable
    {
        /// <summary>
        /// Gets the active buffer in this collection now.
        /// </summary>
        ICanvas Current { get; }
        
        /// <summary>
        /// Lets user to change the primary buffer held in this object. Please note that it is not a part of collection itself.
        /// </summary>
        /// <param name="primary">The buffer intended to be the primary buffer for this collection</param>
        void ChangePrimary(ICanvas primary);

        /// <summary>
        /// This event gets invoked whenever curret active buffer gets changed.
        /// </summary>
        event EventHandler<IEventArgs> BufferChanged;
    }
    #endregion
#endif

#endif
}
