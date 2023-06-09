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
    public static partial class GWSEnumerable
    {
        #region APPEND - PREPEND ITEMS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> AppendItems<T>(this IEnumerable<T> source, IEnumerable<T> newElements)
        {
            if (source != null)
            {
                foreach (var item in source)
                    yield return item;
            }
            if (newElements != null)
            {
                foreach (var item in newElements)
                    yield return item;
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> PrependItems<T>(this IEnumerable<T> source, IEnumerable<T> newElements)
        {
            if (newElements != null)
            {
                foreach (var item in newElements)
                    yield return item;
            }
            if (source != null)
            {
                foreach (var item in source)
                    yield return item;
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> AppendItem<T>(this IEnumerable<T> source, T newElement)
        {
            if (source != null)
            {
                foreach (var item in source)
                    yield return item;
            }
            if (newElement != null)
                yield return newElement;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> PrependItem<T>(this IEnumerable<T> source, T newElement)
        {
            if (newElement != null)
                yield return newElement;
            if (source != null)
            {
                foreach (var item in source)
                    yield return item;
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> AppendItems<T>(this IEnumerable<T> source, params T[] newElements)
        {
            if (source != null)
            {
                foreach (var item in source)
                    yield return item;
            }
            if (newElements != null && newElements.Length > 0)
            {
                foreach (var item in newElements)
                    yield return item;
            }
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> PrependItems<T>(this IEnumerable<T> source, params T[] newElements)
        {
            if (newElements != null && newElements.Length > 0)
            {
                foreach (var item in newElements)
                    yield return item;
            }
            if (source != null)
            {
                foreach (var item in source)
                    yield return item;
            }
        }
        #endregion

        #region TO ENUMERABLE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ToIEnumerable<T>(params T[] values) =>
            values;
        #endregion

        #region TO LIST
        public static IList<T> ToList<T>(this IEnumerable<T> items) =>
            new PrimitiveList<T>(items);
        #endregion

        #region TO READ-ONLY LIST
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<T> AsReadOnly<T>(this IEnumerable<T> collection) =>
            new ReadOnlyList<T, T>(collection);
        #endregion

        #region INIT ARRAY
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] InitializeArray<T>(int count, T @default)
        {
            var array = new T[count];
            for (int i = 0; i < count; i++)
                array[i] = @default;
            return array;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int[] InitializeArray(int count, int @default)
        {
            var array = new int[count];
            fixed (int* p = array)
                for (int i = 0; i < count; i++)
                    p[i] = @default;
            return array;
        }
        #endregion

        #region CHANGE ALL
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeAll<T>(ref T[] arr, T @default)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = @default;
            }
        }
        #endregion

        #region CORRECT LENGTH
        /// <summary>
        /// Corrects the length.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <param name="listLength">Length of the list.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectLength(ref int start, ref int length, int listLength)
        {
            if (start >= listLength || listLength <= 0)
            {
                start = 0; length = 0;
                return;
            }
            if (length < 0) { length = listLength; }
            start = System.Math.Max(0, start);
            listLength -= start;
            listLength = System.Math.Max(0, listLength);
            length = System.Math.Max(0, System.Math.Min(length, listLength));
            start = System.Math.Min(start, System.Math.Max(0, start + length - 1));
        }

        /// <summary>
        /// Corrects the length.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectLength(ref int start, ref int length)
        {
            start = System.Math.Max(0, start);
            length = System.Math.Max(0, length);
            start = System.Math.Min(start, System.Math.Max(0, start + length - 1));
        }

        /// <summary>
        /// Corrects the length.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <param name="listLength">Length of the list.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectLength(int start, ref int length, int listLength)
        {
            if (start >= listLength || listLength <= 0)
            {
                start = 0; length = 0;
                return;
            }
            if (length < 0) { length = listLength; }
            start = System.Math.Max(0, start);
            listLength -= start;
            listLength = System.Math.Max(0, listLength);
            length = System.Math.Max(0, System.Math.Min(length, listLength));
            start = System.Math.Min(start, System.Math.Max(0, start + length - 1));
        }

        /// <summary>
        /// Corrects the length.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="listLength">Length of the list.</param>
        /// <returns>Tuple&lt;System.Int32, System.Int32&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lot<int, int> CorrectLength(ISpan range, int listLength)
        {
            if (range == null)
                return Lot<int, int>.Empty;
            var len = range.Count;
            return CorrectLength(range.Start, len, listLength);
        }

        /// <summary>
        /// Corrects the length.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>Tuple&lt;System.Int32, System.Int32&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lot<int, int> CorrectLength(ISpan range)
        {
            if (range == null) return Lot<int, int>.Empty;
            var len = range.Count;
            return CorrectLength(range.Start, len);
        }

        /// <summary>
        /// Corrects the length.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <param name="listLength">Length of the list.</param>
        /// <returns>Tuple&lt;System.Int32, System.Int32&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lot<int, int> CorrectLength(int start, int length, int listLength)
        {
            CorrectLength(ref start, ref length, listLength);
            return Lot.Create(start, length);
        }

        /// <summary>
        /// Corrects the length.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <returns>Tuple&lt;System.Int32, System.Int32&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lot<int, int> CorrectLength(int start, int length)
        {
            CorrectLength(ref start, ref length);
            return Lot.Create(start, length);
        }
        #endregion

        #region CORRECT INDEX
        /// <summary>
        /// Corrects the index.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="listLength">Length of the list.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectIndex(ref int start, ref int end, int listLength)
        {
            if (start >= listLength || listLength <= 0)
            {
                start = 0; end = 0;
                return;
            }
            if (end < 0) { end = listLength - 1; }

            start = System.Math.Max(0, start);
            listLength -= start;
            listLength = System.Math.Max(0, listLength);
            end = System.Math.Max(0, System.Math.Min(end, listLength - 1));
            start = System.Math.Min(start, System.Math.Max(0, start + listLength - 1));
        }

        /// <summary>
        /// Corrects the index.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectIndex(ref int start, ref int end)
        {
            var len = start + end + 1;
            start = System.Math.Max(0, start);
            len = System.Math.Max(0, len);
            end = System.Math.Max(0, System.Math.Min(end, start + len - 1));
            start = System.Math.Min(start, System.Math.Max(0, start + len - 1));
        }

        /// <summary>
        /// Corrects the index.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="listLength">Length of the list.</param>
        /// <returns>Tuple&lt;System.Int32, System.Int32&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lot<int, int> CorrectIndex(int start, int end, int listLength)
        {
            CorrectIndex(ref start, ref end, listLength);
            return Lot.Create(start, end);
        }

        /// <summary>
        /// Corrects the index.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>Tuple&lt;System.Int32, System.Int32&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lot<int, int> CorrectIndex(int start, int end)
        {
            CorrectIndex(ref start, ref end);
            return Lot.Create(start, end);
        }

        /// <summary>
        /// Corrects the index.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="listLength">Length of the list.</param>
        /// <returns>Tuple&lt;System.Int32, System.Int32&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lot<int, int> CorrectIndex(ISpan range, int listLength)
        {
            if (range == null)
                return Lot<int, int>.Empty;
            return CorrectIndex(range.Start, range.End, listLength);
        }

        /// <summary>
        /// Corrects the index.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <returns>Tuple&lt;System.Int32, System.Int32&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Lot<int, int> CorrectIndex(ISpan range)
        {
            if (range == null)
                return Lot<int, int>.Empty;
            return CorrectIndex(range.Start, range.End);
        }
        #endregion

        #region COUNT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Count(this IEnumerable enumerable)
        {
            if (enumerable is ICount)
                return ((ICount)enumerable).Count;
            if (enumerable is IReadOnlyList)
                return ((IReadOnlyList)enumerable).Count;
            if (enumerable is ICollection)
                return ((ICollection)enumerable).Count;
            int i = 0;
            foreach (var item in enumerable)
                ++i;
            return i;
        }
        public static int Count<T>(this IEnumerable<T> enumerable)
        {
            if(enumerable is IReadOnlyCollection<T>)
                return ((IReadOnlyCollection<T>)enumerable).Count;
            if (enumerable is ICount)
                return ((ICount)enumerable).Count;
            if (enumerable is IReadOnlyList)
                return ((IReadOnlyList)enumerable).Count;

            if (enumerable is ICollection<T>)
                return ((ICollection<T>)enumerable).Count;
            if (enumerable is ICollection)
                return ((ICollection)enumerable).Count;
            int i = 0;
            foreach (var item in enumerable)
                ++i;
            return i;
        }
        #endregion

        #region IN REVERSE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> InReverse<T>(this IList<T> collection) =>
            collection.InReverse(collection.Count - 1);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> InReverse<T>(this IList<T> collection, int start)
        {
            for (int i = start; i >= 0; i--)
                yield return collection[i];
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> InReverse<T>(this IReadOnlyList<T> collection) =>
            collection.InReverse(collection.Count - 1);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> InReverse<T>(this IReadOnlyList<T> collection, int start)
        {
            for (int i = start; i >= 0; i--)
                yield return collection[i];
        }
        #endregion

        #region IN FORWARD
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> InForward<T>(this IList<T> collection, int start)
        {
            for (int i = start; i < collection.Count; i++)
                yield return collection[i];
        }
        #endregion

        #region FIRST MATCH
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FirstMatch<T>(this IList<T> collection,
            Predicate<T> condition, out T result, out int idx,
            int start = 0, bool reverse = false, int? end = null)
        {
            int last;
            if (reverse)
            {
                last = end ?? 0;
                for (int i = start; i >= end; i--)
                {
                    if (condition(collection[i]))
                    {
                        result = collection[i];
                        idx = i;
                        return true;
                    }
                }
            }
            else
            {
                last = end ?? (collection.Count - 1) - start;
                for (int i = start; i <= last; i++)
                {
                    if (condition(collection[i]))
                    {
                        result = collection[i];
                        idx = i;
                        return true;
                    }
                }
            }
            idx = -1;
            result = default(T);
            return false;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FirstMatchIndex<T>(this IList<T> collection,
            Predicate<T> condition, out int result, int start = 0, bool reverse = false, int? end = null) =>
            collection.FirstMatch(condition, out T t, out result, start, reverse, end);

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FirstMatchIndex<T>(this IList<T> collection,
           Predicate<T> condition, int start = 0, bool reverse = false, int? end = null)
        {
            collection.FirstMatch(condition, out T t, out int result, start, reverse, end);
            return result;
        }
        #endregion

        #region LAST IN LIST
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(this IList<T> list) =>
            list[list.Count - 1];

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Last<T>(this IReadOnlyList<T> list) =>
            list[list.Count - 1];
        #endregion

        #region REMOVE LAST
        public static void RemoveLast<T>(this IList<T> list) =>
            list.RemoveAt(list.Count - 1);
        #endregion

        #region JOIN
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Join<T>(this T[] first, T[] second, int firstLength = -1, int secondLength = -1)
        {
            firstLength = (firstLength < 0) ? first.Length : Math.Min(firstLength, first.Length);
            secondLength = (secondLength < 0) ? second.Length : Math.Min(secondLength, second.Length);

            var length = firstLength + secondLength;
            var result = new T[length];
            Array.Copy(first, 0, result, 0, firstLength);
            Array.Copy(second, firstLength, result, firstLength, secondLength);
            return result;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Join<T>(this ICollection<T> first, ICollection<T> second)
        {
            var length = first.Count + second.Count;
            var result = new T[length];
            first.CopyTo(result, 0);
            second.CopyTo(result, first.Count);
            return result;
        }
        #endregion

        #region VALUE
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The index.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.Exception">Object is set to null</exception>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetValue<T>(this IEnumerable<T> collection, int index)
        {
            if (collection == null) { throw new Exception("Object is set to null"); }
            if (collection is IList<T>)
            {
                return (collection as IList<T>)[index];
            }
            else
            {
                return collection.ElementAt(index);
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.Exception">Object is set to null</exception>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue<T>(this IEnumerable<T> collection, int index, T value)
        {
            if (collection == null) { throw new Exception("Object is set to null"); }
            if (collection is IList<T>)
            {
                (collection as IList<T>)[index] = value;
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="System.Exception">
        /// Object is set to null
        /// or
        /// Index is out of range
        /// </exception>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetValue(this IEnumerable collection, int index)
        {
            if (collection == null) { throw new Exception("Object is set to null"); }
            if (collection is IList)
            {
                return (collection as IList)[index];
            }
            else
            {
                int i = 0;
                IEnumerator ie = collection.GetEnumerator();
                while (ie.MoveNext())
                {
                    if (index == i) { return ie.Current; }
                    i++;
                }
                throw new Exception("Index is out of range");
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.Exception">
        /// Collection is null
        /// or
        /// Enumerable is read only
        /// </exception>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(this IEnumerable collection, int index, object value)
        {
            if (collection == null) { throw new Exception("Collection is null"); }
            if (collection is IList)
            {
                if ((collection as IList).IsReadOnly)
                    throw new Exception("Enumerable is read only");
                (collection as IList)[index] = value;
            }
        }
        #endregion

        #region FIND - IENUMERABLE
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="array"></param>
        /// <param name="storeAt"></param>
        /// <param name="value"></param>
        /// <param name="range"></param>
        /// <param name="last"></param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Find<T>(this IEnumerable<T> items, ref Entry<T>[] array, int storeAt, T value, ISpan range, bool last = false)
        {
            var result = items.FindItem(range, x => value.Equals(x), last);
            array[storeAt] = result;
        }

        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="match">The match.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entry<T> Find<T>(this IEnumerable<T> collection, Predicate<T> match, bool last = false) =>
            collection.FindItem(null, match, last);

        /// <summary>
        /// Finds the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entry<T> Find<T>(this IEnumerable<T> collection, Criteria criteria, T item, bool last = false) =>
            collection.FindItem(null, (x => Operations.Compare(x, criteria, item)), last);

        /// <summary>
        /// Finds the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<T>(this IEnumerable<T> collection, MultCriteria criteria, T item1, T item2, bool last = false)
        {
            return collection.FindItem(null, (x) =>
               (Operations.CompareRange(x, criteria, item1, item2)), last);
        }

        /// <summary>
        /// Finds the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<T>(this IEnumerable<T> collection, Criteria criteria, object item, bool last = false)
        {
            T val;
            if (item.TryConvert(out val, 0))
                return collection.Find(criteria, val, last);
            else
                return Entry<T>.Blank;
        }

        /// <summary>
        /// Finds the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<T>(this IEnumerable<T> collection, MultCriteria criteria, object item1, object item2, bool last = false)
        {
            T val1, val2;
            if (item1.TryConvert(out val1, 0) && item2.TryConvert(out val2, 0))
                return collection.Find(criteria, val1, val2, last);
            else
                return Entry<T>.Blank;
        }
        /// <summary>
        /// Gets the <see cref="Entry{T}"/> with the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<T>(this IEnumerable<T> items, T item, bool last = false) =>
            items.FindItem(null, (x) => (Operations.Compare(x, 0, item)), last: last);

        /// <summary>
        /// Gets the <see cref="Entry{T}"/> with the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="start">The start.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<T>(this IEnumerable<T> items, T item, int start, bool last = false)
        {
            return items.FindItem(new Span(start), (x => item.Equals(x)), last);
        }

        /// <summary>
        /// Gets the <see cref="Entry{T}"/> with the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <param name="startIndex">The start index.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<T>(this IEnumerable<T> items, Predicate<T> match, int startIndex, bool last = false) =>
            items.FindItem(new Span(startIndex), match, last: last);

        /// <summary>
        /// Gets the <see cref="Entry{T}"/> with the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<T>(this IEnumerable<T> items, Predicate<T> match, int startIndex, int count, bool last = false) =>
            items.FindItem(new Span(startIndex), match, last: last);

        /// <summary>
        /// Gets entry with the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IItem.</returns>
        public static Entry<T> Find<T>(this IEnumerable<T> items, object item, bool last = false)
        {
            T val;
            if (item.TryConvert(out val, 0))
                return items.FindItem(null, x => val.Equals(x), last: last);
            return Entry<T>.Blank;
        }
       
        /// <summary>
        /// Gets entry with the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="start">The start.</param>
        /// <returns>IItem.</returns>
        public static Entry<T> Find<T>(this IEnumerable<T> items, object item, int start, bool last = false)
        {
            T val;
            if (item.TryConvert(out val, 0))
                return items.FindItem(new Span(start), x => val.Equals(x), last: last);
            return Entry<T>.Blank;
        }
        #endregion

        #region FIND - IFIND<K, V, T>
        public static void Find<K, V, T>(this IEnumerable<T> items, ref Entry<T>[] array, int storeAt, T value, ISpan range, bool last = false) where T : IPair<K, V>
        {
            var result = items.FindItem(range, x => value.Equals(x), last);
            array[storeAt] = result;
        }

        /// <summary>
        /// Gets the <see cref="Entry{T}"/> with the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        /// <param name="option">The option.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<K, V, T>(this IReadOnlyList<K, V, T> items, Criteria criteria,
            T item, MatchBy option, bool last = false) where T : IPair<K, V> =>
            items.FindItem(null, (x) => (x.Match(criteria, item, option)), last);

        /// <summary>
        /// Gets the <see cref="Entry{T}"/> with the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<K, V, T>(this IReadOnlyList<K, V, T> items, Criteria criteria, T item, bool last = false) where T : IPair<K, V> =>
            items.FindItem(null, (x) => (x.Match(criteria, item, MatchBy.Item)), last);

        /// <summary>
        /// Gets the <see cref="Entry{T}"/> with the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="key1">The item1.</param>
        /// <param name="key2">The item2.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<K, V, T>(this IReadOnlyList<K, V, T> items, MultCriteria criteria, K key1, K key2, bool last = false) where T : IPair<K, V> =>
            items.FindItem(null, (x) => (x.Match(criteria, key1, key2)), last);

        /// <summary>
        /// Gets the <see cref="Entry{T}"/> with the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="key">The item.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<K, V, T>(this IReadOnlyList<K, V, T> items, Criteria criteria, K key, bool last = false) where T : IPair<K, V> =>
            items.FindItem(null, (x) => (x.Match(criteria, key)), last);

        /// <summary>
        /// Gets the <see cref="Entry{T}"/> with the specified item.
        /// </summary>
        /// <param name="key">The item.</param>
        /// <returns>Item&lt;T&gt;.</returns>
        public static Entry<T> Find<K, V, T>(this IReadOnlyList<K, V, T> items, K key, bool last = false) where T : IPair<K, V> =>
            items.FindItem(null, (x) => (x.Match(key)), last);

        /// <summary>
        /// Gets entry with the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <param name="option">The option.</param>
        /// <returns>IItem.</returns>
        public static Entry<T> Find<K, V, T>(this IReadOnlyList<K, V, T> items, MultCriteria criteria,
            IPair item1, IPair item2, MatchBy option, bool last = false) where T : IPair<K, V> =>
            items.FindItem(null, (x) => (x.Match(criteria, item1, item2, option)), last);

        /// <summary>
        /// Gets entry with the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        /// <param name="option">The option.</param>
        /// <returns>IItem.</returns>
        public static Entry<T> Find<K, V, T>(this IReadOnlyList<K, V, T> items, Criteria criteria,
            IPair item, MatchBy option, bool last = false) where T : IPair<K, V> =>
            items.FindItem(null, (x) => (x.Match(criteria, item, option)), last);

        /// <summary>
        /// Gets entry with the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="any">The item.</param>
        /// <returns>IItem.</returns>
        public static Entry<T> Find<K, V, T>(this IReadOnlyList<K, V, T> items, Criteria criteria, object any, bool last = false) where T : IPair<K, V>
        {
            K key; V value; T item;

            if (any.TryConvert(out key, criteria))
                return items.FindItem(null, (x) => (x.Match(criteria, key)), last);

            else if (any.TryConvert(out value, 0))
                return items.FindItem(null, (x) => (x.Match(criteria, value)), last);

            else if (any.TryConvert(out item, 0))
                return items.FindItem(null, (x) => (x.Match(criteria, item, MatchBy.Item)), last);
            return Entry<T>.Blank;
        }

        /// <summary>
        /// Gets entry with the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item1">The item1.</param> 
        /// <param name="item2">The item2.</param>
        /// <returns>IItem.</returns>
        public static Entry<T> Find<K, V, T>(this IReadOnlyList<K, V, T> items, MultCriteria criteria, object any1, object any2, bool last = false) where T : IPair<K, V>
        {
            K key1, key2; V val1, val2; T item1, item2;

            if (any1.TryConvert(out key1, 0) && any2.TryConvert(out key2, 0))
                return items.FindItem(null, (x) => (x.Match(criteria, key1, key2)), last);
            else if (any1.TryConvert(out val1, 0) && any2.TryConvert(out val2, 0))
                return items.FindItem(null, (x) => (x.Match(criteria, val1, val2)), last);
            else if (any1.TryConvert(out item1, 0) && any2.TryConvert(out item2, 0))
                return items.FindItem(null, (x) => (x.Match(criteria, item1, item2, MatchBy.Item)), last);

            return Entry<T>.Blank;
        }
        #endregion

        #region TO ARRAY
        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns>T[].</returns>
        public static T[] ToArray<T>(this IEnumerable<T> source)
        {
            T[] arr = null;
            source.CopyTo(ref arr, null, 0);
            return arr;
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns>T[].</returns>
        public static T[] ToArray<T>(this IEnumerable<T> source, ISpan range)
        {
            T[] arr = null;
            source.CopyTo(ref arr, range, 0);
            return arr;
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>T[].</returns>
        public static T[] ToArray<T>(this IEnumerable<T> source, int start, int count)
        {
            T[] arr = null;
            Span range = new Span(start, count);
            source.CopyTo(ref arr, range, 0);
            return arr;
        }
        #endregion

        #region COPY TO    
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="range"></param>
        /// <param name="arrayIndex"></param>
        /// <returns></returns>
        public static int CopyTo<T>(this IEnumerable<T> source, ref T[] target, ISpan range, int arrayIndex)
        {
            if (target == null)
                target = new T[0];


            int len = target.Length; T[] array;

            if (arrayIndex >= 0 && arrayIndex <= target.Length)
            {
                int resize;

                if (source is IArrayHolder<T>)
                {
                    int start = 0;

                    array = (source as IArrayHolder<T>).Data;
                    var count = (source as IArrayHolder<T>).Count;

                    if (range != null)
                    {
                        range = new Span(range, count);
                        start = range.Start; count = range.Count;
                    }
                    if (arrayIndex + count > target.Length)
                    {
                        resize = arrayIndex + count - target.Length;
                        Array.Resize(ref target, target.Length + resize);
                    }
                    Array.Copy(array, start, target, arrayIndex, count);
                }
                else if (source is IList<T> || source is IReadOnlyList<T>)
                {
                    IList<T> list = source as IList<T>;

                    if (range != null)
                    {
                        Span r = new Span(range, list.Count);
                        if (arrayIndex + r.Count > target.Length)
                        {
                            resize = arrayIndex + r.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        foreach (var i in r.ToEnumerable())
                        {
                            target[arrayIndex] = list[i]; arrayIndex++;
                        }
                    }
                    else
                    {
                        if (arrayIndex + list.Count > target.Length)
                        {
                            resize = arrayIndex + list.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        for (int i = 0; i < list.Count; i++)
                        {
                            target[arrayIndex] = list[i]; arrayIndex++;
                        }
                    }
                }
                else if (source is IReadOnlyList<T>)
                {
                    IReadOnlyList<T> list = source as IReadOnlyList<T>;

                    if (range != null)
                    {
                        Span r = new Span(range, list.Count);
                        if (arrayIndex + r.Count > target.Length)
                        {
                            resize = arrayIndex + r.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        foreach (var i in r.ToEnumerable())
                        {
                            target[arrayIndex] = list[i]; arrayIndex++;
                        }
                    }
                    else
                    {
                        if (arrayIndex + list.Count > target.Length)
                        {
                            resize = arrayIndex + list.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        for (int i = 0; i < list.Count; i++)
                        {
                            target[arrayIndex] = list[i]; arrayIndex++;
                        }
                    }
                }
                else
                {
                    if (range == null)
                    {
                        array = Enumerable.ToArray(source);
                    }
                    else
                    {
                        array = source.Skip(range.Start).Take(range.Count).ToArray();
                    }
                    resize = arrayIndex + array.Length - target.Length;
                    Array.Resize(ref target, target.Length + resize);
                    Array.Copy(array, 0, target, arrayIndex, array.Length);
                }
            }
            return target.Length - len;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="start">The start.</param>
        public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int start)
        {
            source.CopyTo(ref array, null, start);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int start, int count)
        {
            source.CopyTo(ref array, new Span(start, count), start);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        public static void CopyTo<T>(this IEnumerable<T> source, T[] array)
        {
            source.CopyTo(ref array, null, 0);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <param name="count">The count.</param>
        public static void CopyTo<T>(this IEnumerable<T> source, int index, T[] array, int arrayIndex, int count)
        {
            source.CopyTo(ref array, new Span(index, count), arrayIndex);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="range">The range.</param>
        /// <param name="arrayindex">The arrayindex.</param>
        public static void CopyTo<T>(this IEnumerable<T> source, T[] array, ISpan range, int arrayindex)
        {
            source.CopyTo(ref array, range, arrayindex);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="range">The range.</param>
        public static void CopyTo<T>(this IEnumerable<T> source, T[] array, ISpan range)
        {
            source.CopyTo(ref array, range, 0);
        }
        #endregion

        #region GET DATA
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] GetData<T>(this IIndexer<T> source, int defaultLength = 0)
        {
            if (source == null || source.Count == 0)
                return new T[defaultLength];
            if (source is IConvertible<T[]>)
                return ((IConvertible<T[]>)source).Convert();
            if (source is IArrayHolder<T>)
                return ((IArrayHolder<T>)source).Data;

            T[] arr = new T[source.Count];
            for (int i = 0; i < arr.Length; i++)
                arr[i] = source[i];
            return arr;
        }
        #endregion

        #region TO ENUMERABLE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> ToEnumerable<T>(this IIndexer<T> source, int defaultLength = 0)
        {
            if (source is IConvertible<T[]>)
            {
                var data = ((IConvertible<T[]>)source).Convert();
                foreach (var item in data)
                {
                    yield return item;
                }
            }
            if (source is IArrayHolder<T>)
            {
                var data = ((IArrayHolder<T>)source).Data;
                int i = source.Count;
                foreach (var item in data)
                {
                    if (--i < 0)
                        break;
                    yield return item;
                }
            }
            else
            {
                var count = source.Count;
                for (int i = 0; i < count; i++)
                    yield return source[i];
            }
        }
        #endregion

        #region WHERE
        /// <summary>
        /// Wheres the specified indices.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="indices">The indices.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, IEnumerable<int> indices, CriteriaMode mode)
        {
            bool[] indexer = new bool[source.Count()];
            bool except = (mode == CriteriaMode.Exclude) ? true : false;
            foreach (var item in indices)
            {
                indexer[item] = true;
            }
            return new ReadOnlyList<T, T>(source.Where((x, i) => (indexer[i] == !except)));
        }

        /// <summary>
        /// Wheres the specified indices.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="indices">The indices.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, IEnumerable<int> indices)
        {
            return source.Where(indices, 0);
        }

        /// <summary>
        /// Wheres the specified mode.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="indices">The indices.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, CriteriaMode mode, params int[] indices)
        {
            return source.Where(indices, mode);
        }

        /// <summary>
        /// Wheres the specified start.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, int start, int count, CriteriaMode mode)
        {
            bool except = (mode == CriteriaMode.Exclude) ? true : false;
            if (count > source.Count()) count = source.Count();
            Span r = new Span(start, count);
            return new ReadOnlyList<T, T>(source.Where
                ((x, i) => (r.Contains(i) != except)));
        }

        /// <summary>
        /// Wheres the specified start.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, int start, int count)
        {
            return source.Where(start, count, 0);
        }

        /// <summary>
        /// Wheres the specified range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="range">The range.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, ISpan range, CriteriaMode mode)
        {
            range = new Span(range, source.Count());
            return source.Where(range.Start, range.Count, mode);
        }

        /// <summary>
        /// Wheres the specified range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="range">The range.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, ISpan range)
        {
            range = new Span(range, source.Count());
            return new ReadOnlyList<T, T>(source.Where((x, i) => (range.Contains(i))));
        }

        /// <summary>
        /// Wheres the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value">The value.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, Criteria criteria, T value)
        {
            return new ReadOnlyList<T, T>(source.Where
                ((x) => (Operations.Compare(x, criteria, value))));
        }

        /// <summary>
        /// Wheres the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value">The value.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, Criteria criteria, object value)
        {
            T val;
            if (value.TryConvert(out val, criteria))
            {
                return source.Where(criteria, val);
            }
            else { return new ReadOnlyList<T, T>(new T[0]); }
        }

        /// <summary>
        /// Wheres the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, MultCriteria criteria, T value1, T value2)
        {
            return new ReadOnlyList<T, T>(source.Where
                ((x) => (Operations.CompareRange(x, criteria, value1, value2))));
        }

        /// <summary>
        /// Wheres the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>Wrapper&lt;T&gt;.</returns>
        public static IReadOnlyList<T> Where<T>(this IEnumerable<T> source, MultCriteria criteria, object value1, object value2)
        {
            T val1, val2;
            if (value1.TryConvert(out val1, 0) && value2.TryConvert(out val2, 0))
            {
                return new ReadOnlyList<T, T>(source.Where
                    ((x) => (Operations.CompareRange(x, criteria, val1, val2))));
            }
            else
            { return new ReadOnlyList<T, T>(new T[0]); }
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="condition">The condition.</param>
        /// <returns>System.Int32.</returns>
        public static int IndexOf<T>(this IEnumerable<T> list, Predicate<T> condition)
        {
            int i = -1;
            return list.Any(x => { i++; return condition(x); }) ? i : -1;
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public static int IndexOf<T>(this IEnumerable<T> list, T value)
        {
            int i = -1;
            return list.Any(x => { i++; return Equals(x, value); }) ? i : -1;
        }
        #endregion

        #region private methods
        /// <summary>
        /// Determines whether [is array index] [the specified item].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="item">The item.</param>
        /// <param name="range">The range.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="last">if set to <c>true</c> [last].</param>
        /// <param name="isArray">if set to <c>true</c> [is array].</param>
        /// <returns>Item&lt;T&gt;.</returns>
        static Entry<T> IsArrayIndex<T>(this IEnumerable<T> collection,
            ref T item, ISpan range, Criteria criteria, bool last, out bool isArray)
        {
            int position = -1;

            if ((collection is T[] || collection is IArrayHolder<T>) && criteria == Criteria.Equal)
            {
                T[] array; int start = 0, count = 0;

                if (collection is T[])
                {
                    array = collection as T[];
                    count = array.Length;
                }
                else
                {
                    array = (collection as IArrayHolder<T>).Data;
                    count = (collection as IArrayHolder<T>).Count;
                }

                if (range != null) { start = range.Start; }
                if (last)
                {
                    position = Array.LastIndexOf(array, item, start, count);
                }
                else
                {
                    position = Array.IndexOf(array, item, start, count);
                }
                isArray = true;

                if (position != -1)
                {
                    return new Entry<T>(array[position], position);
                }
                else { return Entry<T>.Blank; }
            }
            else
            {
                isArray = false;
                return Entry<T>.Blank;
            }
        }

        /// <summary>
        /// Finds the specified range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="range">The range.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="last">if set to <c>true</c> [last].</param>
        /// <returns>Item&lt;T&gt;.</returns>
        static Entry<T> FindItem<T>(this IEnumerable<T> collection, ISpan range, Predicate<T> predicate, bool last)
        {
            int index = -1;
            T value = default(T);

            if (collection is IList<T>)
            {
                IList<T> list = collection as IList<T>;
                if (range == null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (predicate(list[i]))
                        {
                            index = i;
                            value = list[index];
                            if (!last) break;
                        }
                    }
                }
                else
                {
                    var rng = new Span(range, list.Count);

                    foreach (var i in rng.ToEnumerable())
                    {
                        if (predicate(list[i]))
                        {
                            index = i;
                            value = list[index];
                            if (!last) break;
                        }
                    }
                }
            }
            else if (collection is IReadOnlyList<T>)
            {
                IReadOnlyList<T> list = collection as IReadOnlyList<T>;
                if (range == null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (predicate(list[i]))
                        {
                            index = i;
                            value = list[index];
                            if (!last) break;
                        }
                    }
                }
                else
                {
                    var rng = new Span(range, list.Count);

                    foreach (var i in rng.ToEnumerable())
                    {
                        if (predicate(list[i]))
                        {
                            index = i;
                            value = list[index];
                            if (!last) break;
                        }
                    }
                }
            }
            else
            {
                int i = -1;

                if (range == null)
                {
                    foreach (var thing in collection)
                    {
                        ++index;
                        if (predicate(thing))
                        {
                            i = index;
                            value = thing;
                            if (!last) break;
                        }
                    }
                    index = i;
                }
                else
                {
                    var r = new Span(range, collection.Count());

                    int start = r.Start;
                    int end = r.End;
                    foreach (var thing in collection)
                    {
                        ++index;
                        if (index > end) { break; }
                        if (index >= start && predicate(thing))
                        {
                            i = index; value = thing;
                            if (!last) { break; }
                        }
                    }
                    index = i;
                }
            }
            if (index == -1)
                return Entry<T>.Blank;
            return new Entry<T>(value, index);
        }

        /// <summary>
        /// Finds the specified range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="range">The range.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="last">if set to <c>true</c> [last].</param>
        /// <returns>Item&lt;T&gt;.</returns>
        static Entry<T> FindItem<K, V, T>(this IEnumerable<T> collection, ISpan range, Predicate<IPair<K, V>> predicate, bool last) where T : IPair<K, V>
        {
            int index = -1;
            T value = default(T);

            if (collection is IList<T>)
            {
                IList<T> list = collection as IList<T>;
                if (range == null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (predicate(list[i]))
                        {
                            index = i;
                            value = list[index];
                            if (!last) break;
                        }
                    }
                }
                else
                {
                    var rng = new Span(range, list.Count);

                    foreach (var i in rng.ToEnumerable())
                    {
                        if (predicate(list[i]))
                        {
                            index = i;
                            value = list[index];
                            if (!last) break;
                        }
                    }
                }
            }
            else if (collection is IReadOnlyList<T>)
            {
                IReadOnlyList<T> list = collection as IReadOnlyList<T>;
                if (range == null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (predicate(list[i]))
                        {
                            index = i;
                            value = list[index];
                            if (!last) break;
                        }
                    }
                }
                else
                {
                    var rng = new Span(range, list.Count);

                    foreach (var i in rng.ToEnumerable())
                    {
                        if (predicate(list[i]))
                        {
                            index = i;
                            value = list[index];
                            if (!last) break;
                        }
                    }
                }
            }
            else
            {
                int i = -1;

                if (range == null)
                {
                    foreach (var thing in collection)
                    {
                        ++index;
                        if (predicate(thing))
                        {
                            i = index;
                            value = thing;
                            if (!last) break;
                        }
                    }
                    index = i;
                }
                else
                {
                    var r = new Span(range, collection.Count());

                    int start = r.Start;
                    int end = r.End;
                    foreach (var thing in collection)
                    {
                        ++index;
                        if (index > end) { break; }
                        if (index >= start && predicate(thing))
                        {
                            i = index; value = thing;
                            if (!last) { break; }
                        }
                    }
                    index = i;
                }
            }
            if (index == -1)
                return Entry<T>.Blank;
            return new Entry<T>(value, index);
        }
        #endregion
    }
}
