/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region ILEXICON
    public interface ILexicon<K, V, T> :
        IPrimitiveList<T>, IReadOnlyList<K, V, T> where T : IPair<K, V>
    {
        #region PROPERTIES
        new int Count { get; }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        IQueryableCollection<K> Keys { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        IQueryableCollection<V> Values { get; }
        #endregion

        /// <summary>
        /// Trims internal data array.
        /// </summary>
        void Trim();
    }
    #endregion

    public abstract partial class _Lexicon<K, V, T> : PrimitiveList<T>, ILexicon<K, V, T> where T : IPair<K, V>
    {
        #region VARIABLES
        KeyCollection keys;
        ValueCollection values;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="GenreCollection"/> class.
        /// </summary>
        public _Lexicon()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreCollection"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public _Lexicon(int capacity) : base(capacity) {; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreCollection"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public _Lexicon(IEnumerable<T> source) : base(source) { }
        #endregion

        #region PROPERTIES
        public T First => Data[0];

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public IQueryableCollection<K> Keys
        {
            get
            {
                if (keys == null)
                    keys = new KeyCollection(this);
                return keys;
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public IQueryableCollection<V> Values
        {
            get
            {
                if (values == null)
                    values = new ValueCollection(this);
                return values;
            }
        }
        #endregion

        #region INDEX OK
        public bool IndexOK(int index) => index >= 0 && index < Length;
        #endregion

        #region TRIM
        public void Trim()
        {
            Array.Resize(ref Data, Length);
        }
        #endregion

        #region NESTED CLASSES
        /// <summary>
        /// Class KeyCollection. This class cannot be inherited.
        /// </summary>
        sealed class KeyCollection : _QueryableCollection<K>
        {
            /// <summary>
            /// The parent
            /// </summary>
            IReadOnlyList<T> parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection"/> class.
            /// </summary>
            /// <param name="source">The source.</param>
            internal KeyCollection(IReadOnlyList<T> source)
            {
                parent = source;
            }

            /// <summary>
            /// Gets the count.
            /// </summary>
            /// <value>The count.</value>
            public override int Count
            {
                get { return parent.Count; }
            }
            /// <summary>
            /// Gets or sets the <see cref="K"/> at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>K.</returns>
            /// <exception cref="System.NotImplementedException"></exception>
            public K this[int index]
            {
                get => parent[index].Key;
            }
            /// <summary>
            /// Gets a value indicating whether this instance is read only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public override bool IsReadOnly
            {
                get { return true; }
            }

            #region GET VALUE
            protected override K GetValue(int index)
            {
                return parent[index].Key;
            }
            #endregion
        }

        /// <summary>
        /// Class ValueCollection. This class cannot be inherited.
        /// </summary>
        sealed class ValueCollection : _QueryableCollection<V>
        {
            /// <summary>
            /// The parent
            /// </summary>
            IList<T> parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValueCollection"/> class.
            /// </summary>
            /// <param name="source">The source.</param>
            internal ValueCollection(IList<T> source)
            {
                parent = source;
            }

            /// <summary>
            /// Gets the count.
            /// </summary>
            /// <value>The count.</value>
            public override int Count
            {
                get { return parent.Count; }
            }
            /// <summary>
            /// Gets or sets the <see cref="V"/> at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>V.</returns>
            /// <exception cref="System.NotImplementedException"></exception>
            public V this[int index] => parent[index].Value;

            /// <summary>
            /// Gets a value indicating whether this instance is read only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public override bool IsReadOnly
            {
                get { return true; }
            }

            #region GET VALUE
            protected override V GetValue(int index)
            {
                return parent[index].Value;
            }
            #endregion

        }
        #endregion
    }
}
