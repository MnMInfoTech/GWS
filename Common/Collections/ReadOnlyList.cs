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
    public abstract class ReadOnlyList<T> : IReadOnlyList, IReadOnlyList<T>, IReverseIEnumerable<T>
    {
        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class.
        /// </summary>
        protected ReadOnlyList()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize() { }
        #endregion

        #region PROPERTIES
        public abstract T this[int index] { get; protected set; }
        public abstract int Count { get; }
        public virtual bool IsReadOnly => true;
        public T First => this[0];
        public T Last => this[Count - 1];
        object IReadOnlyList.this[int index] => this[index];
        #endregion

        #region IN-REVERSE
        public IEnumerable<T> InReverse()
        {
            int len = Count - 1;
            for (int i = len; i >= 0; i--)
            {
                yield return this[i];
            }
        }
        #endregion

        #region ENUMERATOR
        public IEnumerator<T> GetEnumerator()
        {
            int len = Count;
            for (int i = 0; i < len; i++)
                yield return this[i];
        }
        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();
        #endregion
    }
}
