/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public abstract class _Iterator<T> : IIterator<T>
    {
        #region VARIABLES
        /// <summary>
        /// The position
        /// </summary>
        [field: NonSerialized]
        protected int position;

        /// <summary>
        /// The _current index
        /// </summary>
        protected internal int indexNow;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="_Iterator{T}"/> class.
        /// </summary>
        protected _Iterator()
        {
            position = -1;
            initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void initialize() { }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets or sets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>T.</returns>
        public abstract T this[int index] { get; set; }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public abstract int Count { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public virtual bool IsReadOnly => true;

        /// <summary>
        /// Gets the first element.
        /// </summary>
        public T First => this[0];

        /// <summary>
        /// Gets the last element.
        /// </summary>
        /// <value>The last.</value>
        public T Last => this[Count - 1];

        /// <summary>
        /// Gets the items per page.
        /// </summary>
        /// <value>The items per page.</value>
        protected virtual int itemsPerPage => Math.Min(Count, 10);

        /// <summary>
        /// Gets or sets the index of the current.
        /// </summary>
        /// <value>The index of the current.</value>
        protected internal virtual int currentIndex
        {
            get => Math.Max(Math.Min(indexNow, Count - 1), 0);
            set
            {
                value = Math.Max(Math.Min(value, Count - 1), 0);
                if (value == indexNow) return;
                indexNow = value;
            }
        }

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        /// <value>The current item.</value>
        protected internal virtual T currentItem
        {
            get => (indexNow >= 0 && indexNow < Count) ? this[indexNow] : default(T);
            set => this[indexNow] = value;
        }

        protected internal int totalItems => itemsPerPage;
        object IReadOnlyList.this[int index] => this[index];
        #endregion

        #region TO ARRAY
        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns>T[].</returns>
        public T[] ToArray()
        {
            T[] arr = null;
            CopyTo(ref arr, new Span(0, Count), 0);
            return arr;
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns>T[].</returns>
        public T[] ToArray(ISpan range)
        {
            T[] arr = null;
            CopyTo(ref arr, range, 0);
            return arr;
        }
        #endregion

        #region INDEX OK
        /// <summary>
        /// Verifies the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="System.Exception">Index is out of bound of array</exception>
        public bool IndexOK(int index) =>
            index >= 0 && index <= Count - 1;
        #endregion

        #region COPY TO    
        public int CopyTo(ref T[] target, ISpan range, int arrayIndex)
        {
            if (target == null) 
                target = new T[0];


            int len = target.Length; T[] array;

            if (arrayIndex >= 0 && arrayIndex <= target.Length)
            {
                int resize = 0;

                if (this is IArray<T>)
                {
                    int start = 0, count = 0;

                    array = (this as IArray<T>).Data;
                    count = (this as IArray<T>).Count;

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
                else if (this is IList<T> || this is IReadOnlyList<T>)
                {
                    IList<T> list = this as IList<T>;

                    if (range != null)
                    {
                        Span r = new Span(range, list.Count);
                        if (arrayIndex + r.Count > target.Length)
                        {
                            resize = arrayIndex + r.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        foreach (var i in r)
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
                else if (this is IReadOnlyList<T>)
                {
                    IReadOnlyList<T> list = this as IReadOnlyList<T>;

                    if (range != null)
                    {
                        Span r = new Span(range, list.Count);
                        if (arrayIndex + r.Count > target.Length)
                        {
                            resize = arrayIndex + r.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        foreach (var i in r)
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
                        array = Enumerable.ToArray(this);
                    }
                    else
                    {
                        array = this.Skip(range.Start).Take(range.Count).ToArray();
                    }
                    resize = arrayIndex + array.Length - target.Length;
                    Array.Resize(ref target, target.Length + resize);
                    Array.Copy(array, 0, target, arrayIndex, array.Length);
                }
            }
            return target.Length - len;
        }
        #endregion

        #region IENUMERABLE
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            position = -1;
            int len = Count;
            for (int i = 0; i < len; i++)
            {
                ++position;
                yield return this[i];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
