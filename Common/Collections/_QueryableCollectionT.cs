/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public abstract class _QueryableCollection<T>: IQueryableCollection<T>
    {
        #region PROPERTIES
        public abstract int Count { get; }
        public abstract bool IsReadOnly { get; }
        protected virtual bool CheckForDisposabilityOfIndividualItems => false;
        object IReadOnlyList.this[int index] => GetValue(index);
        T IQueryableCollection<T>.this[int index] => GetValue(index);
        T IReadOnlyList<T>.this[int index] => GetValue(index);
        T IIndexer<T>.this[int index] => GetValue(index);
        #endregion

        #region QUERY
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<T> Query(Predicate<T> condition = null, bool reverse = false,
            int? conditionalIndex = null, NumCriteria? conditionalIndexCriteria = null)
        {
            var fx = (condition ?? (s => true));
            int count = Count;
            bool hasCondition = conditionalIndex != null;
            var idx = conditionalIndex ?? -1;
            var criteria = conditionalIndexCriteria ?? NumCriteria.Equal;
            hasCondition = hasCondition && criteria != NumCriteria.None;
            var items = reverse ? InReverse() : this;
            int step = reverse? -1: 1;
            var i = reverse? count: -1;

            foreach (var item in items)
            {
                i += step;

                if (!fx(item))
                    continue;

                if (hasCondition)
                {
                    switch (criteria)
                    {
                        case NumCriteria.None:
                        default:
                            break;
                        case NumCriteria.Equal:
                            if (i != idx)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (i <= idx)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (i >= idx)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (i == idx)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (i > idx)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (i < idx)
                                continue;
                            break;
                    }
                }

                yield return item;
            }
        }

        public IEnumerable<TObject> Query<TObject>(Predicate<TObject> condition = null, bool reverse = false,
            int? conditionalIndex = null, NumCriteria? conditionalIndexCriteria = null)
        {
            var fx = (condition ?? (s => true));
            int count = Count;
            bool hasCondition = conditionalIndex != null;
            var idx = conditionalIndex ?? -1;
            var criteria = conditionalIndexCriteria ?? NumCriteria.Equal;
            hasCondition = hasCondition && criteria != NumCriteria.None;
            var items = reverse ? InReverse() : this;
            int step = reverse ? -1 : 1;
            var i = reverse ? count : -1;
            var Items = items.OfType<TObject>();

            foreach (var item in Items)
            {
                i += step;

                if (!fx(item))
                    continue;

                if (hasCondition)
                {
                    switch (criteria)
                    {
                        case NumCriteria.None:
                        default:
                            break;
                        case NumCriteria.Equal:
                            if (i != idx)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (i <= idx)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (i >= idx)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (i == idx)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (i > idx)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (i < idx)
                                continue;
                            break;
                    }
                }

                yield return item;
            }
        }
        #endregion

        #region GET - SET VALUE
        protected abstract T GetValue(int index);
        #endregion

        #region IENUMERABLE
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            int len = Count;
            for (int i = 0; i < len; i++)
            {
                yield return GetValue(i);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        #region IN-REVERSE
        public IEnumerable<T> InReverse()
        {
            int len = Count - 1;
            for (int i = len; i >= 0; i--)
            {
                yield return GetValue(i);
            }
        }
        #endregion

        #region DOSPOSE
        public virtual void Dispose()
        {
            if (Operations.Inherits<IDisposable, T>())
            {
                var enumerator = this.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (current == null)
                        continue;
                    ((IDisposable)current).Dispose();
                }
            }
            else if (CheckForDisposabilityOfIndividualItems)
            {
                var enumerator = this.GetEnumerator();
                while (enumerator.MoveNext())
                {
                     var Current = enumerator.Current;
                     if(Current is IDisposable)
                        ((IDisposable)Current).Dispose();
                }
            }
        }
        #endregion
    }
}
