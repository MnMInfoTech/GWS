/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public abstract class _Lexicon<K, V, T> : Collection<T>, ILexicon<K, V, T> where T: IPair<K, V>
    {
        #region VARIABLES
        KeyCollection keys;
        ValueCollection values;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="Genres"/> class.
        /// </summary>
        public _Lexicon() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genres"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public _Lexicon(int capacity) : base(capacity) {; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genres"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public _Lexicon(IEnumerable<T> source) : base(source) { }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        public IIterator<K> Keys
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
        public IIterator<V> Values
        {
            get
            {
                if (values == null)
                    values = new ValueCollection(this);
                return values;
            }
        }
        #endregion

        #region nested classes
        /// <summary>
        /// Class KeyCollection. This class cannot be inherited.
        /// </summary>
        sealed class KeyCollection : _Iterator<K>
        {
            /// <summary>
            /// The parent
            /// </summary>
            IList<T> parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="KeyCollection"/> class.
            /// </summary>
            /// <param name="source">The source.</param>
            internal KeyCollection(IList<T> source)
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
            public override K this[int index]
            {
                get => parent[index].Key;
                set => throw new NotImplementedException();
            }
            /// <summary>
            /// Gets a value indicating whether this instance is read only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public override bool IsReadOnly
            {
                get { return true; }
            }
        }

        /// <summary>
        /// Class ValueCollection. This class cannot be inherited.
        /// </summary>
        sealed class ValueCollection : _Iterator<V>
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
            public override V this[int index]
            {
                get => parent[index].Value;
                set => throw new NotImplementedException();
            }
            /// <summary>
            /// Gets a value indicating whether this instance is read only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public override bool IsReadOnly
            {
                get { return true; }
            }
        }
        #endregion
    }
}
