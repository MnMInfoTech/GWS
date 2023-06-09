/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    public struct EnumTag<T> where T : Enum
    {
        public string Name { get; private set; }
        public T Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumTag{T}"/> struct.
        /// </summary>
        /// <param name="name_">The name_.</param>
        /// <param name="value_">The value_.</param>
        /// <exception cref="System.Exception">T must be enum</exception>
        public EnumTag(string name_, T value_)
        {
            Name = name_; Value = value_;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumTag{T}"/> struct.
        /// </summary>
        /// <param name="value_">The value_.</param>
        /// <exception cref="System.Exception">T must be enum</exception>
        public EnumTag(T value_)
        {
            Name = value_.ToString();
            Value = value_;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumTag{T}"/> struct.
        /// </summary>
        /// <param name="value_">The value_.</param>
        public EnumTag(string value_)
        {
            Name = null; Value = default(T);
            if (value_ != null)
            {
                Name = value_.Trim();
                try
                {
                    Value = (T)Enum.Parse(typeof(T), value_, true);
                }
                catch { }
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="EnumTag{T}"/> to <see cref="T"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator T(EnumTag<T> item)
        {
            return item.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="EnumTag{T}"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int(EnumTag<T> item)
        {
            return (int)(object)item.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T"/> to <see cref="EnumTag{T}"/>.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator EnumTag<T>(T item)
        {
            return new EnumTag<T>(item);
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns>List&lt;EnumTag&lt;T&gt;&gt;.</returns>
        public static IReadOnlyList<EnumTag<T>> GetList()
        {
            var list = new PrimitiveList<EnumTag<T>>();
            T[] values = (T[])Enum.GetValues(typeof(T));
            foreach (var item in values)
                list.Add(new EnumTag<T>(item));
            return list;
        }
    }
}
