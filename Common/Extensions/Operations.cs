/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Collections;
using System.Collections.Generic;

#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
using System.Data;
using System.ComponentModel;
#endif

using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MnM.GWS
{
    public static partial class Operations
    {
        #region VARIABLES/ CONSTS
        static System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

        /// <summary>
        /// The find all binding
        /// </summary>
        public const BindingFlags findAllBinding =
            BindingFlags.Static | BindingFlags.Instance |
            BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>
        /// The find constructor
        /// </summary>
        public const BindingFlags findConstructor =
             BindingFlags.Instance |
             BindingFlags.NonPublic | BindingFlags.Public;


#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// The no case
        /// </summary>
        public const StringComparison noCase = StringComparison.InvariantCultureIgnoreCase;
#endif
        public static readonly Dictionary<string, IConverter> Converters = new Dictionary<string, IConverter>(4);
        const int MaxAnsiCode = 255;
        #endregion

        #region COMPARE
        /// <summary>
        /// Compares the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">The left.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Compare<T>(this T left, Criteria criteria, object right) =>
            Operator<T>.Compare(left, right, criteria);

        /// <summary>
        /// Compares the specified criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">The left.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Compare<T>(this T left, Criteria criteria, T right) =>
            Operator<T>.Compare(left, right, criteria);

        /// <summary>
        /// Compares the range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">The left.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CompareRange<T>(this T left, MultCriteria criteria, T value1, T value2) =>
            Operator<T>.CompareRange(left, criteria, value1, value2);

        /// <summary>
        /// Compares the range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">The left.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CompareRange<T>(this T left, MultCriteria criteria, object value1, object value2) =>
            Operator<T>.CompareRange(left, criteria, value1, value2);

        /// <summary>
        /// Compares the specified right.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Compare<T>(this T left, T right) =>
            Operator<T>.Compare(left, right, Criteria.Equal);

        /// <summary>
        /// Operates the specified mop.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">The left.</param>
        /// <param name="mop">The mop.</param>
        /// <param name="right">The right.</param>
        /// <returns>T.</returns>
        public static T Operate<T>(this T left, MathOperator mop, T right) =>
            Operator<T>.Operate(left, mop, right);

        /// <summary>
        /// Operates the specified mop.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="left">The left.</param>
        /// <param name="mop">The mop.</param>
        /// <param name="right">The right.</param>
        /// <returns>T.</returns>
        public static T Operate<T>(this T left, MathOperator mop, object right) =>
            Operator<T>.Operate(left, mop, right);
        #endregion

        #region ADD KEYWORD TO KEYWORDS
        public static void Add(this IKeywords keywords, ExprType value, params string[] keys) =>
            keywords.Add(keywords.NewKeyword(value, keys));
        public static void Add(this IKeywords keywords, ExprType value, IEnumerable<string> keys) =>
            keywords.Add(keywords.NewKeyword(value, keys));
        public static void AddClass<T>(this IKeywords keywords, params string[] keys)
            where T : class
        {
            var t = typeof(T);
            keywords.Add(ExprType.Class, GWSEnumerable.PrependItem(GWSEnumerable.PrependItem(keys, t.Name), t.FullName));
        }
        public static void AddStruct<T>(this IKeywords keywords, params string[] keys)
            where T : struct
        {
            var t = typeof(T);
            keywords.Add(ExprType.Class, GWSEnumerable.PrependItem(GWSEnumerable.PrependItem(keys, t.Name), t.FullName));
        }
        public static void AddArray<T>(this IKeywords keywords, params string[] keys)
        {
            var t = typeof(T);
            keywords.Add(ExprType.Array, GWSEnumerable.PrependItem(GWSEnumerable.PrependItem(keys, t.Name), t.FullName));
        }
        public static void AddEnumerable<E, T>(this IKeywords keywords, params string[] keys)
            where E : IEnumerable<T>
        {
            var t = typeof(T);
            keywords.Add(ExprType.Array, GWSEnumerable.PrependItem(GWSEnumerable.PrependItem(keys, t.Name), t.FullName));
            var e = typeof(E);
            keywords.Add(ExprType.Class, GWSEnumerable.PrependItem(GWSEnumerable.PrependItem(keys, t.Name), t.FullName));
        }
        public static void AddKeyword<T>(this IKeywords keywords, ExprType type, params string[] keys)
        {
            var t = typeof(T);
            keywords.Add(type, GWSEnumerable.PrependItem(GWSEnumerable.PrependItem(keys, t.Name), t.FullName));
        }
        #endregion

        #region GET/SET MEMBER VALUE
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object.</returns>
        public static object GetValue(this object source, string memberName, int? index = null)
        {
            object value = null;
            if (string.IsNullOrEmpty(memberName)) return null;

            if (index == null)
            {
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                if (source is DataRow)
                {
                    Dbs.ValueOf((DataRow)source, memberName, out value);
                    return value;
                }
                else if (source is DataRowView)
                {
                    Dbs.ValueOf((DataRowView)source, memberName, out value);
                    return value;
                }
                else if (source is ICustomTypeDescriptor)
                {
                    var property = (source as ICustomTypeDescriptor).GetProperties().Find(memberName, true);
                    if (property != null)
                        return property.GetValue(source);
                }
#endif
                if (source.IsPrimitive())
                    return source.ToString();
                else
                {
                    var property = source.MyProperty(memberName, true);
                    if (property != null)
                    {
                        try
                        {
                            return property.GetValue(source, null);
                        }
                        catch { return null; }
                    }
                }
            }
            else
            {
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                if (source is DataTable)
                {
                    Dbs.ValueOf((source as DataTable).Rows[index.Value], memberName, out value);
                    return value;
                }
                else if (source is DataView)
                {
                    Dbs.ValueOf((source as DataView).Table.Rows[index.Value], memberName, out value);
                    return value;
                }
                else if (source is IListSource)
                {
                    var _source = source as IListSource;
                    if (_source.ContainsListCollection)
                        return _source.GetList()[index.Value].GetValue(memberName);
                }
#endif
                if (source is IList)
                    return (source as IList)[index.Value].GetValue(memberName);
                else if (source is IEnumerable)
                {
                    if (source is string) return source as string;
                    var ienumerable = (source as IEnumerable).OfType<object>();
                    var _value = ienumerable.ElementAt(index.Value);
                    return _value.GetValue(memberName);
                }
            }
            return value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="index">The index.</param>
        /// <returns>T.</returns>
        public static T GetValue<T>(this object source, string memberName, int? index = null) =>
            source.GetValue(memberName, index).ConvertTo<T>();

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this object source, string memberName, bool distinct = true)
        {
            IEnumerable items = null;
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            if (source is DataTable)
            {
                items = (source as DataTable).Rows;
                goto PROCESS;
            }
            if (source is DataView)
            {
                items = (source as DataView).Table.Rows;
                goto PROCESS;
            }
            if (source is IListSource)
            {
                var _source = source as IListSource;
                if (_source.ContainsListCollection)
                {
                    items = _source.GetList();
                    goto PROCESS;
                }
            }
#endif
            if (source is IEnumerable)
                items = (source as IEnumerable);

            PROCESS:
            if (items == null)
                return new object[0];

            if (distinct)
            {
                var sortedList = new SortedList<object, object>();
                foreach (var item in items)
                {
                    object value = item.GetValue(memberName);
                    if
                    (
                        value == null
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                            || value == DBNull.Value
#endif
                    )
                    {
                        continue;
                    }
                    if (!sortedList.ContainsKey(value))
                        sortedList.Add(value, null);
                }
                return sortedList.Keys.ToArray();
            }
            else
            {
                var list = new PrimitiveList<object>();
                foreach (var item in items)
                {
                    object value = item.GetValue(memberName);
                    if
                    (
                        value == null
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                        || value == DBNull.Value
#endif
                    )
                    {
                        continue;
                    }

                    list.Add(value);
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this object source, string memberName, int start, int count)
        {
            IEnumerable items = null;
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            if (source is DataTable)
            {
                items = (source as DataTable).Rows;
                goto PROCESS;
            }
            if (source is DataView)
            {
                items = (source as DataView).Table.Rows;
                goto PROCESS;
            }
            if (source is IListSource)
            {
                var _source = source as IListSource;
                if (_source.ContainsListCollection)
                {
                    items = _source.GetList();
                    goto PROCESS;
                }
            }
#endif

            if (source is IEnumerable)
                items = (source as IEnumerable);
            PROCESS:
            if (items == null)
                return new object[0];

            return items.OfType<object>().Where((x, i) =>
                i >= start && i < start + count).Select(y => y.GetValue(memberName)).ToArray();

        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <param name="memberNames">The member names.</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this object source, int? index = null, params string[] memberNames)
        {
            var results = new object[memberNames.Length];
            for (int i = 0; i < memberNames.Length; i++)
                results[i] = source.GetValue(memberNames[i], index);

            return results;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberNames">The member names.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this object source, IEnumerable<string> memberNames, int? index = null)
        {
            var result = new PrimitiveList<object>();

            foreach (var item in memberNames)
                result.Add(source.GetValue(item, index));

            return result.ToArray();
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public static string GetText(this object source, string memberName, int? index = null) =>
            source.GetValue(memberName, index) + "";

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object.</returns>
        public static object GetValue(this ISourceBoundControl source, string memberName, int? index = null) =>
            source.Source.GetValue(memberName, index);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="index">The index.</param>
        /// <returns>T.</returns>
        public static T GetValue<T>(this ISourceBoundControl source, string memberName, int? index = null) =>
             source.Source.GetValue<T>(memberName, index);

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this ISourceBoundControl source, string memberName, bool distinct = true) =>
             source.Source.GetValues(memberName, distinct);


        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this ISourceBoundControl source, string memberName, int start, int count) =>
            source.Source.GetValues(memberName, start, count);

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <param name="memberNames">The member names.</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this ISourceBoundControl source, int? index = null, params string[] memberNames) =>
             source.Source.GetValues(index, memberNames);

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberNames">The member names.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this ISourceBoundControl source, IEnumerable<string> memberNames, int? index = null) =>
            source.Source.GetValues(memberNames, index);

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public static string GetText(this ISourceBoundControl source, string memberName, int? index = null) =>
            source.Source.GetText(memberName, index);

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object.</returns>
        public static object GetValue(this ISource source, string memberName, int? index = null)
        {
            if (source == null || source.UnderlyingSource == null) return null;
            return source.UnderlyingSource.GetValue(memberName, index);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="index">The index.</param>
        /// <returns>T.</returns>
        public static T GetValue<T>(this ISource source, string memberName, int? index = null)
        {
            if (source == null || source.UnderlyingSource == null) return default(T);
            return source.UnderlyingSource.GetValue<T>(memberName, index);
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this ISource source, string memberName, bool distinct = true)
        {
            if (source == null || source.UnderlyingSource == null) return null;
            return source.UnderlyingSource.GetValues(memberName, distinct);
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this ISource source, string memberName, int start, int count)
        {
            if (source == null || source.UnderlyingSource == null) return null;
            return source.UnderlyingSource.GetValues(memberName, start, count);
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="index">The index.</param>
        /// <param name="memberNames">The member names.</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this ISource source, int? index = null, params string[] memberNames)
        {
            if (source == null || source.UnderlyingSource == null) return null;
            return source.UnderlyingSource.GetValues(index, memberNames);
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberNames">The member names.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Object[].</returns>
        public static object[] GetValues(this ISource source, IEnumerable<string> memberNames, int? index = null)
        {
            if (source == null || source.UnderlyingSource == null) return null;
            return source.UnderlyingSource.GetValues(memberNames, index);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public static string GetText(this ISource source, string memberName, int? index = null)
        {
            if (source == null || source.UnderlyingSource == null) return null;
            return source.UnderlyingSource.GetText(memberName, index);
        }


        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        public static void SetValue(this object source, string memberName, object value, int? index = null)
        {
            if (string.IsNullOrEmpty(memberName)) return;

            if (index == null)
            {
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                if (source is DataRow)
                {
                    Dbs.SetValueOf((source as DataRow), memberName, value);
                    return;
                }
                else if (source is DataRowView)
                {
                    Dbs.SetValueOf((source as DataRow), memberName, value);
                    return;
                }

                if (source is ICustomTypeDescriptor)
                {
                    var property = (source as ICustomTypeDescriptor).GetProperties().Find(memberName, true);
                    if (property != null)
                    {
                        property.SetValue(source, value);
                        return;
                    }
                }
#endif


                if (source.IsPrimitive())
                    value.ConvertTo<object>(out source, source.GetType());
                else
                {
                    var property = source.MyProperty(memberName, true);
                    if (property != null) property.SetValue(source, value, null);
                }
            }
            else
            {
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                if (source is DataTable)
                {
                    Dbs.SetValueOf((source as DataTable).Rows[index.Value], memberName, value);
                    return;
                }
                else if (source is DataView)
                {
                    Dbs.SetValueOf((source as DataView).Table.Rows[index.Value], memberName, value);
                    return;
                }
                if (source is IListSource)
                {
                    var _source = source as IListSource;
                    if (_source.ContainsListCollection)
                    {
                        _source.GetList()[index.Value].SetValue(memberName, value);
                        return;
                    }
                }

#endif

                if (source is IList)
                    (source as IList)[index.Value].SetValue(memberName, value);
                else if (source is IEnumerable)
                {
                    var ienumerable = (source as IEnumerable).OfType<object>();
                    ienumerable.SetValue(index.Value, value);
                }
            }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        public static void SetValue(this ISourceBoundControl source, string memberName, object value, int? index = null) =>
            source.Source.SetValue(memberName, value, index);

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <param name="value">The value.</param>
        /// <param name="index">The index.</param>
        public static void SetValue(this ISource source, string memberName, object value, int? index = null)
        {
            if (source == null || source.UnderlyingSource == null) return;
            source.UnderlyingSource.SetValue(memberName, value, index);
        }
        #endregion

        #region IS NULL
        /// <summary>
        /// Determines whether [is null so as] [the specified other].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if [is null so as] [the specified other]; otherwise, <c>false</c>.</returns>
        public static bool IsNullSoAs<T>(this T value, T other)
        {
            return object.Equals(value, null) && object.Equals(other, null);
        }

        /// <summary>
        /// Determines whether the specified value is null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the specified value is null; otherwise, <c>false</c>.</returns>
        public static bool IsNull<T>(this T value)
        {
            return object.Equals(value, null);
        }

        /// <summary>
        /// Nots the null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool NotNull<T>(this T value)
        {
            return !object.Equals(value, null);
        }
        #endregion

        #region IS PRIMITIVE
        /// <summary>
        /// Determines whether the specified object is primitive.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if the specified object is primitive; otherwise, <c>false</c>.</returns>
        public static bool IsPrimitive(this object obj)
        {
            return (obj is bool || obj is char || obj is sbyte ||
                obj is byte || obj is short || obj is ushort || obj is int ||
                obj is uint || obj is long || obj is ulong || obj is float ||
                obj is double || obj is decimal || obj is DateTime || obj is string);
        }

        /// <summary>
        /// Determines whether this instance is primitive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</returns>
        public static bool IsPrimitive<T>()
        {
            var t = typeof(T);
            return (t == typeof(bool) || t == typeof(char) || t == typeof(sbyte) ||
                t == typeof(byte) || t == typeof(short) || t == typeof(ushort) || t == typeof(int) ||
                t == typeof(uint) || t == typeof(long) || t == typeof(ulong) || t == typeof(Single) ||
                t == typeof(float) | t == typeof(double) || t == typeof(decimal) ||
                t == typeof(DateTime) || t == typeof(string));
        }
        #endregion

        #region BINDING FLAGS
        /// <summary>
        /// Values the specified flagtype.
        /// </summary>
        /// <param name="flagtype">The flagtype.</param>
        /// <returns>BindingFlags.</returns>
        public static BindingFlags Value(this BindingFlagType flagtype)
        {
            switch (flagtype)
            {
                case BindingFlagType.AllMember:
                    return BindingFlags.Public | BindingFlags.Instance |
                        BindingFlags.NonPublic;
                case BindingFlagType.AllProperties:
                    return BindingFlags.Public | BindingFlags.Instance |
                        BindingFlags.NonPublic | BindingFlags.Static;
                case BindingFlagType.AllStatic:
                    return BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
                case BindingFlagType.PublicInstance:
                default:
                    return BindingFlags.Public | BindingFlags.Instance;
                case BindingFlagType.NonPublicInstance:
                    return BindingFlags.Instance | BindingFlags.NonPublic;
                case BindingFlagType.PublicStatic:
                    return BindingFlags.Public | BindingFlags.Static;
                case BindingFlagType.NonPublicStatic:
                    return BindingFlags.Static | BindingFlags.NonPublic;
                case BindingFlagType.TypeInitializer:
                    return BindingFlags.Static | BindingFlags.Instance;
                case BindingFlagType.PublicNonInherited:
                    return BindingFlags.Public | BindingFlags.Instance |
                       BindingFlags.NonPublic | BindingFlags.DeclaredOnly;


#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                case BindingFlagType.ExactMatching:
                    return BindingFlags.Public | BindingFlags.Instance |
                       BindingFlags.NonPublic | BindingFlags.ExactBinding;
#endif
            }
        }
        #endregion

        #region CONSTRUCT HASH CODE
        /// <summary>
        /// Constructs the hash code.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Int32.</returns>
        public static int ConstructHashCode(int a, int b)
        {
            var A = a >= 0 ? 2 * a : -2 * a - 1;
            var B = b >= 0 ? 2 * b : -2 * b - 1;
            return A >= B ? A * A + A + B : A + B * B;
        }
        #endregion

        #region EQUALS ANY OF
        /// <summary>
        /// Equalses any of.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="controls">The controls.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool EqualsAnyOf(this IHandle control, params IHandle[] controls)
        {
            if (control == null || controls.Length == 0) return false;
            foreach (var item in controls)
            {
                if (item == control) return true;
            }
            return false;
        }
        #endregion

        #region TYPE CODE
        /// <summary>
        /// To the type.
        /// </summary>
        /// <param name="typecode">The typecode.</param>
        /// <returns>Type.</returns>
        public static Type ToType(this TypeCode typecode)
        {
            switch (typecode)
            {
                case TypeCode.Boolean:
                    return typeof(bool);
                case TypeCode.Byte:
                    return typeof(byte);
                case TypeCode.Char:
                    return typeof(char);


#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
                case TypeCode.DBNull:
                    return typeof(DBNull);
#endif
                case TypeCode.DateTime:
                    return typeof(DateTime);
                case TypeCode.Decimal:
                    return typeof(decimal);
                case TypeCode.Double:
                    return typeof(double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return typeof(Int16);
                case TypeCode.Int32:
                    return typeof(Int32);
                case TypeCode.Int64:
                    return typeof(Int64);
                case TypeCode.Object:
                    return typeof(object);
                case TypeCode.SByte:
                    return typeof(sbyte);
                case TypeCode.Single:
                    return typeof(Single);
                case TypeCode.String:
                    return typeof(string);
                case TypeCode.UInt16:
                    return typeof(UInt16);
                case TypeCode.UInt32:
                    return typeof(UInt32);
                case TypeCode.UInt64:
                    return typeof(UInt64);
                default:
                    return null;
            }
        }

        /// <summary>
        /// To the type code.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>TypeCode.</returns>
        public static TypeCode ToTypeCode(this Type type)
        {
            return System.Type.GetTypeCode(type);
        }

        /// <summary>
        /// Determines whether the specified type is numeric.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is numeric; otherwise, <c>false</c>.</returns>
        public static bool IsNumeric(this TypeCode type)
        {
            switch (type)
            {
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether [is date time] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is date time] [the specified type]; otherwise, <c>false</c>.</returns>
        public static bool IsDateTime(this TypeCode type)
        {
            switch (type)
            {
                case TypeCode.DateTime:
                    return true;
                default:
                    return false;
            }
        }
        #endregion

        #region INIT ARRAY
        /// <summary>
        /// Initializes the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="length">The length.</param>
        /// <returns>T[].</returns>
        public static T[] InitArray<T>(int length) where T : new()
        {
            var array = new T[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = new T();
            }
            return array;
        }
        /// <summary>
        /// Gets the objet array list.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>List&lt;System.Object&gt;.</returns>
        public static PrimitiveList<object> GetObjetArrayList(IEnumerable collection)
        {
            var array = new PrimitiveList<object>();

            foreach (var item in collection)
                array.Add(item);
            return array;
        }
        #endregion

        #region INVERT CRITERIA
        /// <summary>
        /// Inverts the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Criteria.</returns>
        public static Criteria Invert(this Criteria criteria)
        {
            int i = criteria.EnumValue<int>();
            return (Criteria)(-i - 1);
        }

        /// <summary>
        /// Inverts the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>MultCriteria.</returns>
        public static MultCriteria Invert(this MultCriteria criteria)
        {
            int i = criteria.EnumValue<int>();
            return (MultCriteria)(-i - 1);
        }
        #endregion

        #region STRUCT TO PTR
        public static IntPtr ToPtr<T>(this T obj)
        {
            if (obj == null)
                return IntPtr.Zero;
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf<T>());
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }

        public static T ToObj<T>(this IntPtr ptr)
        {
            return Marshal.PtrToStructure<T>(ptr);
        }
        public static void FreePtr(this IntPtr ptr)
        {
            Marshal.FreeHGlobal(ptr);
        }
        #endregion

        #region ARRAY TO POINTER
        public static IntPtr ToPtr<T>(this T[] Data, out GCHandle handle)
        {
            handle = GCHandle.Alloc(Data, GCHandleType.Pinned);
            return Marshal.UnsafeAddrOfPinnedArrayElement(Data, 0);
        }
        #endregion

        #region FREE ARRAY POINTER
        public static void Free(this GCHandle handle)
        {
            if (handle.IsAllocated)
                handle.Free();
        }
        #endregion

        #region UTF8
        public unsafe static IntPtr AllocUTF8(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return IntPtr.Zero;
            byte[] bytes = Encoding.UTF8.GetBytes(str + '\0');
            fixed (byte* b = bytes)
            {
                return new IntPtr(b);
            }
        }
        public static byte[] UTF8_ToNative(this string s)
        {
            if (s == null)
                return null;
            return Encoding.UTF8.GetBytes(s + '\0');
        }
        public static unsafe string UTF8_ToManaged(this IntPtr s, bool freePtr = false)
        {
            if (s == IntPtr.Zero)
                return null;
            byte* ptr = (byte*)s;
            while (*ptr != 0)
                ptr++;

            byte[] bytes = new byte[ptr - (byte*)s];
            Marshal.Copy(s, bytes, 0, bytes.Length);
            string result = Encoding.UTF8.GetString(bytes);
            return result;
        }
        public static int ToUnicode(this char character)
        {
            UTF32Encoding encoding = new UTF32Encoding();
            byte[] bytes = encoding.GetBytes(character.ToString().ToCharArray());
            return BitConverter.ToInt32(bytes, 0);
        }
        public static string IntPtrToString(this IntPtr ptr) =>
            Marshal.PtrToStringAnsi(ptr);
        public static bool ContainsUnicodeCharacter(this string input) =>
            input.Any(c => c > MaxAnsiCode);
        #endregion

        #region GET GCHANDLE
        public static void GetPinnedArray(int width, int height, out int[] data, out IntPtr pixels, out GCHandle handle)
        {
            data = new int[width * height];
            handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            pixels = handle.AddrOfPinnedObject();
        }
        #endregion

        #region GET READONLY PREDEFINED ITEMS
        public static IEnumerable<N> PredefinedItemsOf<N>()
        {
            TypeInfo info = typeof(N).GetTypeInfo();

            var fields = info.GetFields(BindingFlags.Static | BindingFlags.Public);
            var list = fields.Select(x => x.GetValue(null)).OfType<N>();
            return list;
        }
        #endregion

        #region INHERITS
        public static bool Inherits<TBaseType, TCurrentType>()
        {
            var tcurrent = typeof(TCurrentType);
            var tbase = typeof(TBaseType);
            return tbase.GetTypeInfo().IsAssignableFrom(tcurrent);
            //return typeof(TCurrentType).GetTypeInfo().IsAssignableTo(typeof(TBaseType).GetTypeInfo());
        }
        public static bool Inherits<TBaseType, TCurrentType>(TCurrentType @object)
        {
            var tcurrent = (@object).GetType();
            var tbase = typeof(TBaseType);
            return tbase.GetTypeInfo().IsAssignableFrom(tcurrent);
            //return (@object).GetType().GetTypeInfo().IsAssignableTo(typeof(TBaseType).GetTypeInfo());
        }
        #endregion

        /// <summary>
        /// Class Operation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal static class Operator<T>
        {
            #region VARIABLES
            enum Status
            {
                None,
                NotSupported,
                Supported
            }

            /// <summary>
            /// The operation
            /// </summary>
            static Func<T, T, T>[] operation = new Func<T, T, T>[6];

            /// <summary>
            /// The comparison
            /// </summary>
            static Func<T, T, bool>[] comparison = new Func<T, T, bool>[13];

            /// <summary>
            /// The otasks
            /// </summary>
            static Status[] otasks = new Status[6];

            /// <summary>
            /// The ctasks
            /// </summary>
            static Status[] ctasks = new Status[13];
            #endregion

            #region PRIVATE METHODS
            /// <summary>
            /// Creates the operator.
            /// </summary>
            /// <param name="OperatorType">Type of the operator.</param>
            static void CreateOperator(MathOperator OperatorType)
            {
                Func<T, T, T> func = null;

                int i = OperatorType.EnumValue<int>();

                var paratype1 = typeof(T);
                var paratype2 = typeof(T);
                var left = Expression.Parameter(paratype1, "left");
                var right = Expression.Parameter(paratype2, "right");
                try
                {
                    switch (OperatorType)
                    {
                        case MathOperator.Add:
                            func = Expression.Lambda<Func<T, T, T>>
                                (Expression.AddChecked(left, right), left, right).Compile();
                            break;
                        case MathOperator.Divide:
                            func = Expression.Lambda<Func<T, T, T>>
                                (Expression.Divide(left, right), left, right).Compile();
                            break;
                        case MathOperator.Multiply:
                            func = Expression.Lambda<Func<T, T, T>>
                                (Expression.MultiplyChecked(left, right), left, right).Compile();
                            break;
                        case MathOperator.Subtract:
                            func = Expression.Lambda<Func<T, T, T>>
                                (Expression.SubtractChecked(left, right), left, right).Compile();
                            break;
                        case MathOperator.Modulo:
                            func = Expression.Lambda<Func<T, T, T>>
                                (Expression.Modulo(left, right), left, right).Compile();
                            break;
                        case MathOperator.Negate:
                            func = Expression.Lambda<Func<T, T, T>>
                                (Expression.Negate(left), left, left).Compile();
                            break;
                        default:
                            break;
                    }
                    if (i < 6) { otasks[i] = Status.Supported; operation[i] = func; }
                }
                catch
                {
                    if (i < 6) { otasks[i] = Status.NotSupported; operation[i] = null; }
                }
            }
            /// <summary>
            /// Creates the operator.
            /// </summary>
            /// <param name="OperatorType">Type of the operator.</param>
            /// <param name="id">The identifier.</param>
            static void CreateOperator(Criteria OperatorType, int id)
            {
                Func<T, T, bool> func = null;

                //var paratype1 = typeof(T);
                //var paratype2 = typeof(T);
                //var left = LambdaExpression.Parameter(paratype1, "left");
                //var right = LambdaExpression.Parameter(paratype2, "right");
                try
                {
                    switch (OperatorType)
                    {
                        case Criteria.Equal:
                        case Criteria.NotEqual:
                            //func = LambdaExpression.Lambda<Func<T, T, bool>>
                            //(LambdaExpression.Equal(left, right), left, right).Compile();
                            func = new Func<T, T, bool>(Operator<T>.Equal);
                            break;
                        case Criteria.GreaterThan:
                        case Criteria.NotGreaterThan:
                            //func = LambdaExpression.Lambda<Func<T, T, bool>>
                            //       (LambdaExpression.GreaterThan(left, right), left, right).Compile();
                            func = new Func<T, T, bool>(Operator<T>.Greater);
                            break;
                        case Criteria.LessThan:
                        case Criteria.NotLessThan:
                            //func = LambdaExpression.Lambda<Func<T, T, bool>>
                            //      (LambdaExpression.LessThan(left, right), left, right).Compile();
                            func = new Func<T, T, bool>(Operator<T>.Less);
                            break;
                        case Criteria.Occurs:
                        case Criteria.NotOccurs:
                            func = new Func<T, T, bool>(Operator<T>.Occurs);
                            break;
                        case Criteria.BeginsWith:
                        case Criteria.NotBeginsWith:
                            func = new Func<T, T, bool>(Operator<T>.Begins);
                            break;
                        case Criteria.EndsWith:
                        case Criteria.NotEndsWith:
                            func = new Func<T, T, bool>(Operator<T>.Ends);
                            break;
                        case Criteria.OccursNoCase:
                        case Criteria.NotOccursNoCase:
                            func = new Func<T, T, bool>(Operator<T>.OccursIgnoreCase);
                            break;
                        case Criteria.BeginsWithNoCase:
                        case Criteria.NotBeginsWithNoCase:
                            func = new Func<T, T, bool>(Operator<T>.BeginsIgnoreCase);
                            break;
                        case Criteria.EndsWithNoCase:
                        case Criteria.NotEndsWithNoCase:
                            func = new Func<T, T, bool>(Operator<T>.EndsIgnoreCase);
                            break;
                        case Criteria.StringEqual:
                        case Criteria.NotStrEqual:
                            func = new Func<T, T, bool>(Operator<T>.StrEqual);
                            break;
                        case Criteria.StringEqualNoCase:
                        case Criteria.NotStrEqualNoCase:
                            func = new Func<T, T, bool>(Operator<T>.StrEqualIgnoreCase);
                            break;
                        case Criteria.StringNumGreaterThan:
                            func = new Func<T, T, bool>(Operator<T>.StrGreater);
                            break;
                        case Criteria.StringNumLessThan:
                            func = new Func<T, T, bool>(Operator<T>.StrLess);
                            break;
                        default:
                            if (id <= 12) { ctasks[id] = Status.NotSupported; comparison[id] = null; }
                            break;
                    }

                    if (id <= 12) { ctasks[id] = Status.Supported; comparison[id] = func; }
                }
                catch
                {
                    //switch (OperatorType)
                    //{
                    //    case Criteria.Equal:
                    //        func = new Func<T, T, bool>(Operator<T>.Equal);
                    //        ctasks[id] = Status.Supported; comparison[id] = func;
                    //        break;
                    //    case Criteria.GreaterThan:
                    //        func = new Func<T, T, bool>(Operator<T>.Greater);
                    //        ctasks[id] = Status.Supported; comparison[id] = func;
                    //        break;
                    //    case Criteria.LessThan:
                    //        func = new Func<T, T, bool>(Operator<T>.Less);
                    //        ctasks[id] = Status.Supported; comparison[id] = func;
                    //        break;
                    //    default:
                    //        if (id <= 12) { ctasks[id] = Status.NotSupported; comparison[id] = null; }
                    //        break;
                    //}
                }
            }
            /// <summary>
            /// Equals the specified a.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool Equal(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ && b_)
                {
                    return true;
                }
                else if (a_)
                {
                    return false;
                }
                else if (a is IEquatable<T>)
                {
                    return (a as IEquatable<T>).Equals(b);
                }
                else if (a is IComparable<T>)
                {
                    return (a as IComparable<T>).CompareTo(b) == 0;
                }
                else if (a is IComparable)
                {
                    return (a as IComparable).CompareTo(b) == 0;
                }
                else if (object.ReferenceEquals(a, b))
                {
                    return true;
                }
                else
                {
                    return a.Equals(b);
                }
            }
            /// <summary>
            /// Greaters the specified a.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool Greater(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ && b_) { return false; }
                else if (a_) { return false; }
                else if (a is IComparable<T>)
                {
                    return (a as IComparable<T>).CompareTo(b) > 0;
                }
                else if (a is IComparable)
                {
                    return (a as IComparable).CompareTo(b) > 0;
                }
                else { return false; }
            }
            /// <summary>
            /// Lesses the specified a.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool Less(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ && b_) { return false; }
                else if (a_) { return true; }
                else if (a is IComparable<T>)
                {
                    return (a as IComparable<T>).CompareTo(b) < 0;
                }
                else if (a is IComparable)
                {
                    return (a as IComparable).CompareTo(b) < 0;
                }
                else { return false; }
            }
            /// <summary>
            /// Occurses the specified a.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool Occurs(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ || b_) { return false; }
                return a.ToString().IndexOf(b.ToString()) != -1;
            }
            /// <summary>
            /// Occurses the ignore case.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool OccursIgnoreCase(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ || b_) { return false; }
                return a.ToString().ToUpper().IndexOf(b.ToString().ToUpper()) != -1;
            }
            /// <summary>
            /// Beginses the specified a.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool Begins(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ || b_) { return false; }
                return a.ToString().StartsWith(b.ToString());
            }
            /// <summary>
            /// Beginses the ignore case.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool BeginsIgnoreCase(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ || b_) { return false; }
                return a.ToString().ToUpper().StartsWith(b.ToString().ToUpper());
            }
            /// <summary>
            /// Endses the specified a.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool Ends(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ || b_) { return false; }
                return a.ToString().EndsWith(b.ToString());
            }
            /// <summary>
            /// Endses the ignore case.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool EndsIgnoreCase(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ || b_) { return false; }
                return a.ToString().ToUpper().EndsWith(b.ToString().ToUpper());
            }
            /// <summary>
            /// Strings the equal.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool StrEqual(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ || b_) { return false; }
                return a.ToString() == (b.ToString());
            }
            /// <summary>
            /// Strings the equal ignore case.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool StrEqualIgnoreCase(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ || b_) { return false; }
                return a.ToString().ToUpper() == (b.ToString().ToUpper());
            }
            /// <summary>
            /// Strings the greater.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool StrGreater(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ && b_) { return false; }
                else if (a_) { return false; }
                else if (b_) { return true; }
                else
                {
                    return Numbers.NumCompare(a.ToString(), b.ToString()) > 0;
                }
            }
            /// <summary>
            /// Strings the less.
            /// </summary>
            /// <param name="a">a.</param>
            /// <param name="b">The b.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            static bool StrLess(T a, T b)
            {
                bool a_ = (object)a == null;
                bool b_ = (object)b == null;

                if (a_ && b_) { return false; }
                else if (b_) { return false; }
                else if (a_) { return true; }
                else
                {
                    return Numbers.NumCompare(a.ToString(), b.ToString()) < 0;
                }
            }
            #endregion

            #region PUBLIC METHODS
            /// <summary>
            /// Operates the specified left.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="mop">The mop.</param>
            /// <param name="right">The right.</param>
            /// <returns>T.</returns>
            public static T Operate(T left, MathOperator mop, T right)
            {
                int i = mop.EnumValue<int>();
                if (i < 6)
                {
                    if (otasks[i] == 0) { CreateOperator(mop); }
                    if (otasks[i] == Status.Supported)
                    {
                        try { return operation[i](left, right); }
                        catch { }
                    }
                }
                return left;
            }

            /// <summary>
            /// Operates the specified left.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="mop">The mop.</param>
            /// <param name="right">The right.</param>
            /// <returns>T.</returns>
            public static T Operate(T left, MathOperator mop, object right)
            {
                T val;
                if (right.ConvertTo(out val, null))
                    return Operate(left, mop, val);

                return left;
            }

            /// <summary>
            /// Compares the specified left.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <param name="criteria">The criteria.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public static bool Compare(T left, T right, Criteria criteria)
            {
                int val = criteria.EnumValue<int>();
                int id = (val < 0) ? -val - 1 : val;

                if (id <= 12)
                {
                    if (ctasks[id] == 0) { CreateOperator(criteria, id); }
                    if (ctasks[id] == Status.Supported)
                    {
                        try
                        {
                            if (val < 0)
                            {
                                return !comparison[id](left, right);
                            }
                            else
                            {
                                var func = comparison[id];
                                return func(left, right);
                            }
                        }
                        catch {; }
                    }
                }
                return false;
            }

            /// <summary>
            /// Compares the range.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="criteria">The criteria.</param>
            /// <param name="value1">The value1.</param>
            /// <param name="value2">The value2.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public static bool CompareRange(T left, MultCriteria criteria, T value1, T value2)
            {
                if (criteria == MultCriteria.Between)
                {
                    return Compare(left, value1, Criteria.NotLessThan) &&
                        Compare(left, value2, Criteria.NotGreaterThan);
                }
                else if (criteria == MultCriteria.NotBetween)
                {
                    return Compare(left, value1, Criteria.LessThan) &&
                        Compare(left, value2, Criteria.GreaterThan);
                }
                return false;
            }

            /// <summary>
            /// Compares the range.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="criteria">The criteria.</param>
            /// <param name="value1">The value1.</param>
            /// <param name="value2">The value2.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public static bool CompareRange(T left, MultCriteria criteria, object value1, object value2)
            {
                if (criteria == MultCriteria.Between)
                {
                    return Compare(left, value1, Criteria.NotLessThan) &&
                        Compare(left, value2, Criteria.NotGreaterThan);
                }
                else if (criteria == MultCriteria.NotBetween)
                {
                    return Compare(left, value1, Criteria.LessThan) &&
                        Compare(left, value2, Criteria.GreaterThan);
                }
                return false;
            }

            /// <summary>
            /// Compares the specified left.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public static bool Compare(T left, T right)
            {
                return Compare(left, right, Criteria.Equal);
            }

            /// <summary>
            /// Compares the specified left.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <param name="criteria">The criteria.</param>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public static bool Compare(T left, object right, Criteria criteria)
            {
                T val;
                if (right.ConvertTo(out val, null))
                {
                    return Compare(left, val, criteria);
                }
                else if (right != null)
                {
                    switch (criteria)
                    {
                        case Criteria.Occurs:
                        case Criteria.BeginsWith:
                        case Criteria.EndsWith:
                        case Criteria.OccursNoCase:
                        case Criteria.BeginsWithNoCase:
                        case Criteria.EndsWithNoCase:
                        case Criteria.StringEqual:
                        case Criteria.StringEqualNoCase:
                        case Criteria.StringNumGreaterThan:
                        case Criteria.StringNumLessThan:
                        case Criteria.NotOccurs:
                        case Criteria.NotBeginsWith:
                        case Criteria.NotEndsWith:
                        case Criteria.NotOccursNoCase:
                        case Criteria.NotBeginsWithNoCase:
                        case Criteria.NotEndsWithNoCase:
                        case Criteria.NotStrEqual:
                        case Criteria.NotStrEqualNoCase:
                        case Criteria.NotStringGreaterThan:
                        case Criteria.NotStringLessThan:
                            return Operator<string>.Compare(left.ToString(), right.ToString(), criteria);
                        default:
                            break;
                    }
                }
                return false;
            }
            #endregion
        }
    }

    public static partial class Operations
    {
        #region VARIABLES
        static Type listType = typeof(PrimitiveList<>);

        /// <summary>
        /// The common
        /// </summary>
        static readonly Type[] common = new Type[]
        {
            typeof ( IFunction ) ,
            typeof ( IList<> ) ,
            typeof ( IList ) ,
            typeof ( IEnumerable<> ) ,
            typeof ( IEnumerable )
        };
        #endregion

        #region HYBRID GENRE
        /// <summary>
        /// Hybrids the IGenre.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="option">The option.</param>
        /// <returns>IIGenre.</returns>
        public static IGenre HybridGenre(this IGenre genre, IGenre other, ExcludeNestedParams option)
        {
            return Factory.newGenre(genre.HybridGenreType(other, option), true, false);
        }

        /// <summary>
        /// Hybrids the IGenre.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="option">The option.</param>
        /// <param name="usethese">The usethese.</param>
        /// <returns>IIGenre.</returns>
        public static IGenre HybridGenre(this IGenre genre, IGenre other, ExcludeNestedParams option, params int[] usethese)
        {
            return Factory.newGenre(genre.HybridGenreType(other, option, usethese), true, false);
        }

        /// <summary>
        /// Hybrids the IGenre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <returns>IIGenre.</returns>
        public static IGenre HybridGenre(this IGenre genre, bool ignorenestedparams, params IGenre[] paramlist)
        {
            return Factory.newGenre(genre.HybridGenreType(ignorenestedparams, paramlist), true, false);
        }

        /// <summary>
        /// Hybrids the IGenre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <returns>IIGenre.</returns>
        public static IGenre HybridGenre(this IGenre genre, bool ignorenestedparams, IList<IGenre> paramlist)
        {
            return Factory.newGenre(genre.HybridGenreType(ignorenestedparams, paramlist), true, false);
        }

        /// <summary>
        /// Hybrids the IGenre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <param name="usethese">The usethese.</param>
        /// <returns>IIGenre.</returns>
        public static IGenre HybridGenre(this IGenre genre, bool ignorenestedparams, IList<IGenre> paramlist, params int[] usethese)
        {
            return Factory.newGenre(genre.HybridGenreType(ignorenestedparams, paramlist, usethese), true, false);
        }

        /// <summary>
        /// Hybrids the type of the IGenre.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="option">The option.</param>
        /// <param name="usethese">The usethese.</param>
        /// <returns>Type.</returns>
        public static Type HybridGenreType(this IGenre genre, IGenre other, ExcludeNestedParams option, params int[] usethese)
        {
            if (!genre.IsGeneric)
                return null;

            else if (usethese.Length == 0)
            {
                return genre.HybridGenreType(other, option);
            }
            else
            {
                int count = genre.GenricParams.Count;
                string[] abc = genre.GenricParams.Keys.ToArray();
                int start, ostart;
                if (other.GenricParams != null)
                {
                    switch (option)
                    {
                        case ExcludeNestedParams.NoExclusion:
                        default:
                            start = 0; ostart = 0;
                            break;
                        case ExcludeNestedParams.HostGenre:
                            start = genre.NestedCount; ostart = 0;
                            break;
                        case ExcludeNestedParams.OtherGenre:
                            start = 0; ostart = other.NestedCount;
                            break;
                        case ExcludeNestedParams.BothGenre:
                            start = genre.NestedCount;
                            ostart = other.NestedCount;
                            break;
                    }

                    for (int i = 0; i < usethese.Length; i++)
                    {
                        if (start >= count) { break; }
                        int index = usethese[i];

                        if (index >= ostart && other.GenricParams.Count > index &&
                                      other.GenricParams[index] != null)
                        {
                            abc[start] = other.GenricParams[usethese[i]].Key;
                        }
                        ++start;
                    }
                }
                string s = genre.GenericName + "[" + string.Join(",", abc) + "]";
                return Type.GetType(s, true);
            }
        }

        /// <summary>
        /// Hybrids the type of the IGenre.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="option">The option.</param>
        /// <returns>Type.</returns>
        public static Type HybridGenreType(this IGenre genre, IGenre other, ExcludeNestedParams option)
        {
            if (genre.IsGeneric == false) { return null; }
            else
            {
                int count = genre.GenricParams.Count;
                string[] abc = genre.GenricParams.Keys.ToArray();
                int start, ostart;
                if (other.GenricParams != null)
                {

                    switch (option)
                    {
                        case ExcludeNestedParams.NoExclusion:
                        default:
                            start = 0; ostart = 0;
                            break;
                        case ExcludeNestedParams.HostGenre:
                            start = genre.NestedCount; ostart = 0;
                            break;
                        case ExcludeNestedParams.OtherGenre:
                            start = 0; ostart = other.NestedCount;
                            break;
                        case ExcludeNestedParams.BothGenre:
                            start = genre.NestedCount;
                            ostart = other.NestedCount;
                            break;
                    }
                    for (int i = start, j = ostart; i < count; i++, j++)
                    {
                        if (j < other.GenricParams.Count && other.GenricParams[j] != null)
                        {
                            abc[i] = other.GenricParams[j].Key;
                        }
                    }
                }

                string s = genre.GenericName + "[" + string.Join(",", abc) + "]";
                return Type.GetType(s, true);
            }
        }

        /// <summary>
        /// Hybrids the type of the IGenre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <returns>Type.</returns>
        public static Type HybridGenreType(this IGenre genre, bool ignorenestedparams, params IGenre[] paramlist)
        {
            if (genre.GenricParams == null)
                return null;

            int start = (ignorenestedparams) ? genre.NestedCount : 0;
            int count = genre.GenricParams.Count;
            string[] abc = genre.GenricParams.Keys.ToArray();

            for (int i = 0; i < paramlist.Length; i++)
            {
                if (start > count) { break; }
                if (paramlist[i] != null)
                {
                    abc[start] = paramlist[i].Key;
                }
                start++;
            }
            string s = genre.GenericName + "[" + string.Join(",", abc) + "]";
            return Type.GetType(s, true);
        }

        /// <summary>
        /// Hybrids the type of the IGenre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <returns>Type.</returns>
        public static Type HybridGenreType(this IGenre genre, bool ignorenestedparams, IList<IGenre> paramlist)
        {
            if (genre.GenricParams == null)
                return null;

            int start = (ignorenestedparams) ? genre.NestedCount : 0;
            int count = genre.GenricParams.Count;
            string[] abc = genre.GenricParams.Keys.ToArray();

            for (int i = 0; i < paramlist.Count; i++)
            {
                if (start > count) { break; }
                if (paramlist[i] != null)
                {
                    abc[start] = paramlist[i].Key;
                }
                start++;
            }
            string s = genre.GenericName + "[" + string.Join(",", abc) + "]";
            return Type.GetType(s, true);
        }

        /// <summary>
        /// Hybrids the type of the IGenre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <param name="usethese">The usethese.</param>
        /// <returns>Type.</returns>
        public static Type HybridGenreType(this IGenre genre, bool ignorenestedparams, IList<IGenre> paramlist, params int[] usethese)
        {
            if (genre.GenricParams == null)
                return null;

            int start = (ignorenestedparams) ? genre.NestedCount : 0;
            int count = genre.GenricParams.Count;
            string[] abc = genre.GenricParams.Keys.ToArray();

            for (int i = 0; i < usethese.Length; i++)
            {
                if (start >= count) { break; }
                int index = usethese[i];

                if (paramlist.Count > index && paramlist[index] != null)
                {
                    abc[start] = paramlist[usethese[i]].Key;
                }
                ++start;
            }
            string s = genre.GenericName + "[" + string.Join(",", abc) + "]";
            return Type.GetType(s, true);
        }
        #endregion

        #region CASCADE GENRE
        /// <summary>
        /// Gets the t list or i enumerable genric parameters.
        /// </summary>
        /// <param name="genericparam">The genericparam.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetTListOrIEnumerableGenricParams(this IGenre genre, out IGenre genericparam)
        {
            if (genre.ListInterfaces[1] != null && genre.ListInterfaces[1].GenricParams != null)
            {
                genericparam = genre.ListInterfaces[1].GenricParams[0];
                return true;
            }
            else if (genre.ListInterfaces[3] != null && genre.ListInterfaces[3].GenricParams != null)
            {
                genericparam = genre.ListInterfaces[3].GenricParams[0];
                return true;
            }
            else { genericparam = null; }
            return false;
        }

        /// <summary>
        /// Cascades the genre.
        /// </summary>
        /// <returns>IGenre.</returns>
        public static IGenre CascadeGenre(this IGenre genre)
        {
            if (genre.IsGeneric && genre.HasTListInterface)
            {
                return genre.HybridGenre(false, genre);
            }
            else if (genre.IsArray && genre.Value.GetArrayRank() == 1)
            {
                return Factory.newGenre(genre.Value.MakeArrayType(1), true, false);
            }
            else
            {
                return genre;
            }
        }
        #endregion

        #region MATCH GENRE TLIST/ IEnumerable
        /// <summary>
        /// Matches the t list interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="GenericListParam">The generic list parameter.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchTListInterface(this IGenre genre, IGenre other, out IGenre GenericListParam)
        {
            bool success = genre.ListInterfaces[1] != null &&
                           genre.ListInterfaces[1].GenricParams[0] != null &&
                           other.ListInterfaces[1] != null &&
                           other.ListInterfaces[1].GenricParams[0] != null;
            if (success)
            {
                success = other.ListInterfaces[1].GenricParams[0] ==
                   genre.ListInterfaces[1].GenricParams[0];
                if (success)
                {
                    GenericListParam = genre.ListInterfaces[1].GenricParams[0];
                    return true;
                }
            }
            GenericListParam = null;
            return success;
        }

        /// <summary>
        /// Matches the t list interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchTListInterface(this IGenre genre, IGenre other)
        {
            bool success = genre.ListInterfaces[1] != null &&
                           genre.ListInterfaces[1].GenricParams[0] != null &&
                           other.ListInterfaces[1] != null &&
                           other.ListInterfaces[1].GenricParams[0] != null;
            if (success)
            {
                success = other.ListInterfaces[1].GenricParams[0] ==
                   genre.ListInterfaces[1].GenricParams[0];
            }
            return success;
        }

        /// <summary>
        /// Matches the t list interface parameter.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchTListInterfaceParam(this IGenre genre, IGenre other)
        {
            if (genre.ListInterfaces[1] != null &&
                genre.ListInterfaces[1].GenricParams[0] != null)
            {
                return genre.ListInterfaces[1].GenricParams[0].Equals(other);
            }
            return false;
        }

        /// <summary>
        /// Matches the t enumerable interface parameter.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchTEnumerableInterfaceParam(this IGenre genre, IGenre other)
        {
            if (genre.ListInterfaces[3] != null &&
                genre.ListInterfaces[3].GenricParams[0] != null)
            {
                return genre.ListInterfaces[3].GenricParams[0].Equals(other);
            }
            return false;
        }

        /// <summary>
        /// Matches the i list interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchIListInterface(this IGenre genre, IGenre other)
        {
            bool success = other.ListInterfaces[2] != null &&
               genre.ListInterfaces[2] != null;
            return success;
        }

        /// <summary>
        /// Matches the t enumerable interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="GenericListParam">The generic list parameter.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchTEnumerableInterface(this IGenre genre, IGenre other, out IGenre GenericListParam)
        {
            bool success = genre.ListInterfaces[3] != null &&
                           genre.ListInterfaces[3].GenricParams[0] != null &&
                           other.ListInterfaces[3] != null &&
                           other.ListInterfaces[3].GenricParams[0] != null;
            if (success)
            {
                success = other.ListInterfaces[3].GenricParams[0] ==
                   genre.ListInterfaces[3].GenricParams[0];
                if (success)
                {
                    GenericListParam = genre.ListInterfaces[3].GenricParams[0];
                    return true;
                }
            }
            GenericListParam = null;
            return success;
        }

        /// <summary>
        /// Matches the t enumerable interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchTEnumerableInterface(this IGenre genre, IGenre other)
        {
            bool success =
                     genre.ListInterfaces[3] != null &&
                     genre.ListInterfaces[3].GenricParams[0] != null &&
                     other.ListInterfaces[3] != null &&
                     other.ListInterfaces[3].GenricParams[0] != null;
            ;
            if (success)
            {
                success = other.ListInterfaces[3].GenricParams[0] ==
                   genre.ListInterfaces[3].GenricParams[0];
            }
            return success;
        }

        /// <summary>
        /// Matches the i fx interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchIFxInterface(this IGenre genre, IGenre other)
        {
            bool success = other.ListInterfaces[0] != null &&
               genre.ListInterfaces[0] != null;
            return success;
        }

        /// <summary>
        /// Matches the i enumerable interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool MatchIEnumerableInterface(this IGenre genre, IGenre other)
        {
            bool success = other.ListInterfaces[4] != null &&
               genre.ListInterfaces[4] != null;
            return success;
        }
        #endregion

        #region ARRAY FROM GENRE
        /// <summary>
        /// Mies the array.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>Array.</returns>
        public static Array MyArray(this IGenre genre, int length)
        {
            return (Array)genre.Value.MakeArrayType(1).MyInstance(length);
        }

        /// <summary>
        /// Mies the array.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>Array.</returns>
        public static Array MyArray(this IGenre genre, IEnumerable items)
        {
            Type maintype = Type.GetType(genre.Key + "[]");
            Array a = (Array)genre.Value.MakeArrayType(1).MyInstance(items.Count());
            IGenre gnr = Factory.newGenre(items);
            int i = 0;
            if (gnr.IsGeneric && gnr.ListInterfaces[1].GenricParams[0].Equals(genre))
            {
                foreach (var item in items)
                {
                    try { a.SetValue(item, i); }
                    catch {; }
                }
            }
            return a;
        }
        #endregion

        #region MY INSTANCE
        /// <summary>
        /// Creates self instance.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Object.</returns>
        public static object MyInstance(this IGenre genre, params object[] arguments)
        {
            if (genre.IsAbstract || genre.IsInterface)
                return null;
            return genre.Value.MyInstance(arguments);
        }

        /// <summary>
        /// Creates self instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public static T MyInstance<T>(this IGenre genre, params object[] arguments)
        {
            if (genre.IsAbstract || genre.IsInterface)
                return default(T);
            return (T)genre.Value.MyInstance(arguments);
        }

        /// <summary>
        /// Creates self instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mi">The mi.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public static T MyInstance<T>(this IGenre genre, InstanceType mi, params object[] arguments)
        {
            if (genre.IsAbstract || genre.IsInterface)
                return default(T);
            return (T)genre.Value.MyInstance(mi, arguments.GetTypes(), arguments);
        }

        /// <summary>
        /// Creates self instance.
        /// </summary>
        /// <param name="mi">The mi.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Object.</returns>
        public static object MyInstance(this IGenre genre, InstanceType mi, params object[] arguments)
        {
            if (genre.IsAbstract || genre.IsInterface)
                return null;
            return genre.Value.MyInstance(mi, arguments.GetTypes(), arguments);
        }
        #endregion

        #region LIST FROM SEED GENRE
        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns>IList.</returns>
        public static IList MyList(this IGenre genre, GetList kind)
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + genre.Key + "]");
            return (IList)maintype.MyInstance();
        }

        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kind">The kind.</param>
        /// <returns>T.</returns>
        public static T MyList<T>(this IGenre genre, GetList kind) where T : IEnumerable
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + genre.Key + "]");
            return maintype.MyInstance<T>();
        }

        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>IList.</returns>
        public static IList MyList(this IGenre genre, GetList kind, params object[] arguments)
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + genre.Key + "]");
            return (IList)maintype.MyInstance(arguments);
        }

        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kind">The kind.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public static T MyList<T>(this IGenre genre, GetList kind, params object[] arguments) where T : IEnumerable
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + genre.Key + "]");
            return maintype.MyInstance<T>(arguments);
        }

        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kind">The kind.</param>
        /// <param name="t">The t.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public static T MyList<T>(this IGenre genre, GetList kind, Type[] t, params object[] arguments) where T : IEnumerable
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + genre.Key + "]");
            return maintype.MyInstance<T>(t, arguments);
        }

        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>IList.</returns>
        public static IList MyList(this IGenre genre, IEnumerable collection)
        {
            GetList kind = GetList.MnMList; bool success = false;

            if (collection is IReadOnlyList)
            {
                kind = GetList.MnMList;
                success = true;
            }
            if (success)
            {
                Type maintype = Type.GetType(GetBlank(kind) + "[" + genre.Key + "]");
                return (IList)maintype.MyInstance(collection);
            }
            else { return null; }
        }

        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        public static IList<T> MyList<T>(this IGenre genre, IEnumerable<T> source)
        {
            GetList kind = GetList.MnMList;

            if (source is System.Collections.Generic.List<T>)
                kind = GetList.List;
            else if (source is Stack<T>)
                kind = GetList.Stack;
            else if (source is Queue<T>)
                kind = GetList.Queue;
            else if (source is HashSet<T>)
                kind = GetList.HashSet;
            else if (source is IReadOnlyList<T>)
                return genre.mnmList<T>((source as IReadOnlyList<T>));

            return genre.MyList<IList<T>>(kind, source);
        }

        /// <summary>
        /// MNMs the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        static IList<T> mnmList<T>(this IGenre genre, IReadOnlyList<T> source)
        {
            var instance = Factory.newGenre(source).MyInstance<IList<T>>(source);
            if (instance == null)
            {
                instance = Factory.newGenre(source).MyInstance<IList<T>>();
                if (instance != null && !instance.IsReadOnly)
                {
                    foreach (var item in source)
                        instance.Add(item);
                }
            }
            return instance;
        }
        #endregion

        #region LIST GENRE FROM GENRE
        /// <summary>
        /// Mies the list genre.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns>IGenre.</returns>
        public static IGenre MyListGenre(this IGenre genre, GetList kind)
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + genre.Key + "]");
            return Factory.newGenre(maintype, false, false);
        }

        /// <summary>
        /// Mies the list genre.
        /// </summary>
        /// <returns>IGenre.</returns>
        public static IGenre MyListGenre(this IGenre genre)
        {
            if (genre.IsGeneric && genre.HasTListInterface)
            {
                return genre.HybridGenre(genre, ExcludeNestedParams.BothGenre);
            }
            else if (genre.IsArray)
            {
                return Factory.newGenre(genre.Value.MakeArrayType(1), false, false);
            }
            else
            {
                return genre.MyListGenre(GetList.MnMList);
            }
        }
        #endregion

        #region GET BLANK
        public static string GetBlank(GetList kind)
        {
            Type tp;
            switch (kind)
            {
                case GetList.MnMList:
                default:
                    tp = listType;
                    break;
                case GetList.List:
                    tp = typeof(System.Collections.Generic.List<>);
                    break;
                case GetList.Stack:
                    tp = typeof(Stack<>);
                    break;
                case GetList.Queue:
                    tp = typeof(Queue<>);
                    break;
                case GetList.HashSet:
                    tp = typeof(HashSet<>);
                    break;
            }
            return tp.Namespace + "." + tp.Name;
        }
        #endregion

        #region VERIFY AS

        /// <summary>
        /// Verifies as.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool VerifyAs<T>(this object item)
        {
            return ((item is T ||
                (item == null && !typeof(T).GetTypeInfo().IsValueType)) &&
                typeof(T) != typeof(object));
        }
        #endregion

        #region GET BASE TYPES & INTERFACE TYPES
        /// <summary>
        /// Gets the base types.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>Type[].</returns>
        public static Type[] GetBaseTypes(this Type target)
        {
            if (target == null) { return null; }
            Type t = target.GetTypeInfo().BaseType;
            var l = new PrimitiveList<Type>();

            while (t != null && t != typeof(object))
            {
                l.Add(t);
                t = t.GetTypeInfo().BaseType;
            }
            return l.ToArray();
        }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <param name="type">The t.</param>
        /// <param name="option">The option.</param>
        /// <param name="types">The types.</param>
        /// <returns>Type[].</returns>
        public static Type[] GetInterfaces(this Type type, ExtractInterfaces option, params Type[] types)
        {
            var mytypes = new PrimitiveList<Type>(5);
            Type[] interfaces;

            switch (option)
            {
                case ExtractInterfaces.TheseOnly:
                    interfaces = type.GetTypeInfo().GetInterfaces();
                    foreach (Type t in types)
                    {
                        mytypes.Add(interfaces.FirstOrDefault(x => x.GetTypeInfo().IsGenericType ? x.GetGenericTypeDefinition() == t : x == t));

                    }
                    break;
                case ExtractInterfaces.ExcludeThese:
                    interfaces = type.GetTypeInfo().GetInterfaces();
                    foreach (Type t in interfaces)
                    {
                        if (!types.Any(x => x.GetTypeInfo().IsGenericType ? x.GetGenericTypeDefinition() == t : x == t))
                            mytypes.Add(t);
                    }
                    break;
                case ExtractInterfaces.AllPrioritizeThese:
                    interfaces = type.GetInterfaces(ExtractInterfaces.TheseOnly, types);
                    mytypes.AddRange(interfaces);
                    interfaces = type.GetInterfaces(ExtractInterfaces.ExcludeThese, types);
                    if (interfaces.Length > 0)
                        mytypes.AddRange(interfaces);
                    break;
                default:
                    break;
            }
            return mytypes.ToArray();
        }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <param name="type">The t.</param>
        /// <param name="option">The option.</param>
        /// <param name="types">The types.</param>
        /// <returns>Type[].</returns>
        public static Type[] GetInterfaces(this Type type, params Type[] types)
        {
            var mytypes = new PrimitiveList<Type>(5);
            Type[] interfaces;
            interfaces = type.GetTypeInfo().GetInterfaces();
            foreach (Type t in interfaces)
            {
                if (!types.Any(x => x.GetTypeInfo().IsGenericType ? x.GetGenericTypeDefinition() == t : x == t))
                    mytypes.Add(t);
            }
            return mytypes.ToArray();
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <param name="makeTheseByRef">The make these by reference.</param>
        /// <returns>Type[].</returns>
        public static Type[] GetTypes(this object[] arguments, params int[] makeTheseByRef)
        {
            if (arguments.Length > 0)
            {
                Type[] t = new Type[arguments.Length];

                if (makeTheseByRef != null && makeTheseByRef.Length > 0)
                {
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        if (makeTheseByRef.Contains(i))
                        {
                            if (arguments[i] == null)
                                t[i] = Factory.newGenre(arguments[i], false, false).Value.MakeByRefType();
                            else
                                t[i] = arguments[i].GetType().MakeByRefType();
                        }
                        else if (arguments[i] == null)
                            t[i] = Factory.newGenre(arguments[i], false, false).Value;
                        else
                            t[i] = arguments[i].GetType();
                    }
                }
                else
                {
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        if (arguments[i] == null)
                            t[i] = Factory.newGenre(arguments[i], false, false).Value;
                        else
                            t[i] = arguments[i].GetType();
                    }
                }
                return t;
            }
            else
                return new Type[0];
        }

        /// <summary>
        /// Gets the types.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>Type[].</returns>
        public static Type[] GetTypes(this ParameterInfo[] arguments)
        {
            if (arguments == null)
                return null;

            Type[] types = new Type[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
                types[i] = arguments[i].ParameterType;
            return types;
        }
        #endregion

        #region MY INSTANCE
        /// <summary>
        /// Mies the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flagtype">The flagtype.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Object.</returns>
        public static object MyInstance(this Type type, BindingFlagType flagtype, params object[] args)
        {
            try
            {
                var argTypes = args.GetTypes();
                var info = type.GetTypeInfo();
                ConstructorInfo ci = info.GetConstructor(argTypes);
                if (ci != null)
                    return ci.Invoke(args);
                ci = info.GetConstructor(new Type[] { typeof(object[]) });
                if (ci != null)
                    return ci.Invoke(new object[] { args });
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Mies the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flagtype">The flagtype.</param>
        /// <param name="types">The types.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Object.</returns>
        public static object MyInstance(this Type type, BindingFlagType flagtype, Type[] types, object[] args)
        {
            try
            {
                var info = type.GetTypeInfo();
                ConstructorInfo ci = info.GetConstructor(types);
                if (ci != null)
                    return ci.Invoke(args);

                ci = info.GetConstructor(new Type[] { typeof(object[]) });
                if (ci != null)
                    return ci.Invoke(new object[] { args });
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Mies the instance.
        /// </summary>
        /// <param name="myType">My type.</param>
        /// <param name="mi">The mi.</param>
        /// <param name="t">The t.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Object.</returns>
        public static object MyInstance(this Type type, InstanceType mi, Type[] types, object[] arguments)
        {
            ConstructorInfo ci = null;
            var info = type.GetTypeInfo();
            try
            {
                ci = info.GetConstructor(types);
            }
            catch { }

            if (ci == null)
            {
                ci = info.GetConstructor(new Type[] { typeof(object[]) });
                if (ci != null) { arguments = new object[] { arguments }; }
            }
            if (ci != null)
            {
                object o = ci.Invoke(arguments);
                switch (mi)
                {
                    case InstanceType.Normal:
                    default:
                        break;
                    case InstanceType.Reference:
                        o.GetType().MakeByRefType();
                        break;
                    case InstanceType.Array:
                        o.GetType().MakeArrayType();
                        break;
                    case InstanceType.Pointer:
                        o.GetType().MakePointerType();
                        break;
                }
                return o;
            }
            return null;
        }

        /// <summary>
        /// Mies the instance.
        /// </summary>
        /// <param name="myType">My type.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Object.</returns>
        public static object MyInstance(this Type myType, params object[] arguments)
        {
            return myType.MyInstance(BindingFlagType.AllMember, arguments.GetTypes(), arguments);
        }

        /// <summary>
        /// Mies the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myType">My type.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>

        public static T MyInstance<T>(this Type myType, params object[] arguments)
        {
            try
            {
                return (T)myType.MyInstance(BindingFlagType.AllMember, arguments.GetTypes(), arguments);
            }
            catch { return default(T); }
        }

        /// <summary>
        /// Mies the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="myType">My type.</param>
        /// <param name="t">The t.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public static T MyInstance<T>(this Type myType, Type[] t, params object[] arguments)
        {
            try
            {
                return (T)myType.MyInstance(BindingFlagType.AllMember, t, arguments);
            }
            catch { return default(T); }
        }
        #endregion

        #region MY PROPERTY
        /// <summary>
        /// Mies the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>PropertyInfo.</returns>
        public static PropertyInfo MyProperty<T>(this T source, string name,
            bool ignoreCase, params object[] arguments)
        {
            if (source == null) return null;
            return source.GetType().MyProperty(name, ignoreCase, arguments);
        }

        /// <summary>
        /// Mies the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns>PropertyInfo.</returns>
        public static PropertyInfo MyProperty<T>(this T source, string name, bool ignoreCase)
        {
            if (source == null) return null;
            return source.GetType().MyProperty(name, ignoreCase);
        }

        /// <summary>
        /// Mies the property.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>PropertyInfo.</returns>
        public static PropertyInfo MyProperty(this Type t, string name, bool ignoreCase, params object[] arguments)
        {
            if (t == null) return null;
            Type[] types = arguments.GetTypes();
            Entry<PropertyInfo> property;
            var ti = t.GetTypeInfo();
            if (!ignoreCase)
            {
                property = ti.GetProperty(name, types);
            }
            else
            {
                BindingFlags flag = BindingFlagType.AllProperties.Value();
                var properties = ti.GetProperties(flag);
                property = Array.Find(properties, (x) => (x.Matched(name, ignoreCase, types)));
            }
            return (!property) ? property.Value : null;
        }

        /// <summary>
        /// Mies the property.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns>PropertyInfo.</returns>
        public static PropertyInfo MyProperty(this Type t, string name, bool ignoreCase)
        {
            if (t == null) return null;
            Entry<PropertyInfo> property;
            var ti = t.GetTypeInfo();
            if (!ignoreCase)
            {
                property = ti.GetProperty(name);
            }
            else
            {
                BindingFlags flag = BindingFlagType.AllProperties.Value();
                var properties = ti.GetProperties(flag);
                property = property = Array.Find(properties, (x) => (x.Matched(name, ignoreCase)));
            }
            return (property) ? property.Value : null;
        }

        /// <summary>
        /// Mies the property names.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>System.String[].</returns>
        public static string[] MyPropertyNames<T>()
        {
            return typeof(T).MyPropertyNames();
        }

        /// <summary>
        /// Mies the property names.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns>System.String[].</returns>
        public static string[] MyPropertyNames<T>(this T t)
        {
            if (t == null) return null;
            return t.GetType().MyPropertyNames();
        }

        /// <summary>
        /// Mies the property names.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>System.String[].</returns>
        public static string[] MyPropertyNames(this Type t)
        {
            if (t == null) return null;
            var properties = t.GetTypeInfo().GetProperties(BindingFlagType.AllProperties.Value());
            string[] names = new string[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                names[i] = properties[i].Name;
            }
            return names;
        }

        /// <summary>
        /// Mies the properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>PropertyInfo[].</returns>
        public static PropertyInfo[] MyProperties<T>()
        {
            return typeof(T).MyProperties();
        }

        /// <summary>
        /// Mies the properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns>PropertyInfo[].</returns>
        public static PropertyInfo[] MyProperties<T>(this T t)
        {
            if (t == null) return null;
            return t.GetType().MyProperties();
        }

        /// <summary>
        /// Mies the properties.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>PropertyInfo[].</returns>
        public static PropertyInfo[] MyProperties(this Type t)
        {
            if (t == null) return null;
            return t.GetTypeInfo().GetProperties(BindingFlagType.AllProperties.Value());
        }
        #endregion
    }

    partial class Operations
    {
        #region ADD ASSEMBLEY TO KEYWORDS

        public static IEnumerable<IGenre> AddAssembley(this IKeywords keywords, Assembly assembly, Type whereBaseTypeIsThis = null)
        {
            var types = assembly.GetTypes().Select(t => t.GetTypeInfo());
            Predicate<TypeInfo> p;

            if (whereBaseTypeIsThis != null)
                p = (t => t.GetInterface(whereBaseTypeIsThis.Name, true) != null
                && !t.IsAbstract && t.IsPublic);
            else
                p = (t => !t.IsAbstract && t.IsPublic);

            var glist = types.Where(x => p(x)).Select(y => keywords.NewGenre(y.AsType()));

            var namespaces = glist.Select(x => x.Namespace).Distinct().
                Select(y => keywords.NewKeyword(ExprType.NameSpace, y));

            keywords.AddRange(namespaces);
            return glist;
        }
        public static void AddAssembley(this IKeywords keywords, ref IGenreCollection genres, Assembly assembly, Type whereBaseTypeIsThis = null)
        {
            var list = keywords.AddAssembley(assembly, whereBaseTypeIsThis);
            genres.AddRange(list);
        }
        public static IEnumerable<IGenre> AddAssembley(this IKeywords keywords, Type assemblyOfThis, Type whereBaseTypeIsThis = null) =>
            keywords.AddAssembley(assemblyOfThis.GetTypeInfo().Assembly, whereBaseTypeIsThis);
        public static IEnumerable<IGenre> AddAssembley<AssemblyOfThis, IncludeOnlyDerivedTypeOfThis>(this IKeywords keywords) =>
           keywords.AddAssembley(typeof(AssemblyOfThis).GetTypeInfo().Assembly, typeof(IncludeOnlyDerivedTypeOfThis));
        public static IEnumerable<IGenre> AddAssembley<AssemblyOfThis>(this IKeywords keywords) =>
            keywords.AddAssembley(typeof(AssemblyOfThis).GetTypeInfo().Assembly);
        public static IKeyword GetKeyword(this IKeywords keywords, IGenreCollection genres, string expression, string nameSpace = null)
        {
            if (!string.IsNullOrEmpty(nameSpace))
                expression = nameSpace + "." + expression;

            Entry<IKeyword> key = keywords.Find(Criteria.StringEqualNoCase, expression);
            if (key)
                return key.Value;
            else
            {
                Entry<IGenre> function = genres.Find(Criteria.StringEqualNoCase, expression);
                if (function)
                    return keywords.NewKeyword(ExprType.Function, function.Value.Key);
            }
            return null;
        }
        #endregion

        #region CONVERSION
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        public static T ConvertTo<T>(this object value)
        {
            if (value == null) return default(T);
            value.ConvertTo(out T result, typeof(T), default(T));
            return result;
        }

        public static bool ConvertTo<T>(this object value, out T result)
        {
            if (value == null)
            {
                result = default(T);
                return false;
            }
            return  value.ConvertTo(out result, typeof(T), default(T));
        }
        /// <summary>
        /// Converts to.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <param name="tp">The tp.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static unsafe bool ConvertTo<T>(this object value, out T result, Type intendedType, T defaultValue = default(T))
        {
            result = defaultValue;
            if (value == null)
                return false;

            #region verify as T
            if (value.VerifyAs<T>())
            {
                result = (T)value;
                return true;
            }
            else if (value is IConvertible<T>)
            {
                result = (((IConvertible<T>)value).Convert());
                return true;
            }
            #endregion

            if (intendedType == null)
                intendedType = typeof(T);

            #region if same type return value itself
            if (value.GetType() == intendedType)
            {
                result = (T)(value);
                return true;
            }
            #endregion

            #region string
            if (value is string)
            {
                var ok = (value as string).ConvertFromString(out result);
                if (ok) return true;
            }
            #endregion

            #region  datetime - double
            else if (intendedType == typeof(double) && value is DateTime)
            {
                result = (T)(object)((DateTime)value).Ticks;
                return true;
            }
            else if (intendedType == typeof(DateTime) && value is double)
            {
                result = (T)(object)new DateTime(1899, 12, 30).AddDays((double)value);
                return true;
            }
            #endregion

            #region IEnumerable cast
            else if (value is IEnumerable)
            {
                if (Factory.newGenre(result, false, false).HasIEnumerableInterface && intendedType != typeof(string))
                    value = (value as IEnumerable);
                else if (value is byte[])
                {
                    if (intendedType == typeof(string))
                    {
                        result = (T)(object)System.Convert.ToBase64String((byte[])value);
                        return true;
                    }
                }
                else if (value is int[])
                {
                    if (intendedType == typeof(string))
                    {
                        var values = (int[])value;

                        fixed (int* p = values)
                        {
                            byte[] bytes = new byte[values.Length * 4];
                            fixed (byte* b = bytes)
                                Blocks.Copy((byte*)p, 0, b, 0, bytes.Length);
                            result = (T)(object)System.Convert.ToBase64String(bytes);
                            return true;
                        }
                    }
                }
                else if (intendedType == typeof(string))
                {
                    if (value is Enum)
                    {
                        result = (T)(object)Enums.EnumName((Enum)value);
                        return true;
                    }
                    else if (value != null)
                    {
                        result = (T)(object)value.ToString();
                        return true;
                    }
                }
            }
            #endregion

            #region Type to Genre / typecode & vise versa
            else if (value is Type)
            {
                var type = (Type)value;
                if (intendedType == typeof(IGenre) || type.GetTypeInfo().IsAssignableFrom(typeof(IGenre)))
                {
                    result = (T)(object)Factory.newGenre(type, false, false);
                    return true;
                }
                if (intendedType == typeof(TypeCode))
                {
                    result = (T)(object)type.ToTypeCode();
                    return true;
                }
                else if (intendedType == typeof(string))
                {
                    result = (T)(object)Factory.newGenre(type, false, false).TypeName;
                    return true;
                }
            }
            else if (value is IGenre)
            {
                if (intendedType == typeof(Type))
                {
                    result = (T)(object)(value as IGenre).Value;
                    return true;
                }
                else if (intendedType == typeof(TypeCode))
                {
                    result = (T)(object)(value as IGenre).Value.ToTypeCode();
                    return true;
                }
                else if (intendedType == typeof(string))
                {
                    result = (T)(object)(value as IGenre).TypeName;
                    return true;
                }
            }
            else if (value is TypeCode)
            {
                if (intendedType == typeof(IGenre) ||
                    value.GetType().GetTypeInfo().IsAssignableFrom(typeof(IGenre)))
                {
                    result = (T)(object)Factory.newGenre(((TypeCode)value).ToType(), false, false);
                    return true;
                }
                if (intendedType == typeof(Type))
                {
                    result = (T)(object)((TypeCode)value).ToType();
                    return true;
                }

                if (intendedType == typeof(string))
                {
                    result = (T)(object)Enums.EnumName(((TypeCode)value));
                    return true;
                }
            }
            #endregion

            #region nullable of generic
            else
            {
                var info = intendedType.GetTypeInfo();
                if (info.IsValueType && info.IsGenericType && intendedType.ToString().StartsWith("System.Nullable`1"))
                    intendedType = Factory.newGenre(intendedType).GenricParams[0].Value;
            }
            #endregion

            if (Converters.Count != 0)
            {
                foreach (var converter in Converters.Values)
                {
                    if (converter.ConvertTo(value, out result))
                        return true;
                }
            }

            #region AT LAST USE SYSTEM.ICONVERTIBLE
            if (value is IConvertible)
            {
                try
                {
                    result = (T)(object)System.Convert.ChangeType(value, intendedType);
                    return true;
                }
                catch { return false; }
            }
            else
            {
                try
                {
                    result = (T)(object)value;
                    return true;
                }
                catch { return false; }
            }
            #endregion
        }

        /// <summary>
        /// Converts the file to bytes.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] ConvertFileToBytes(this string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        byte[] bytes = new byte[fileStream.Length];
                        fileStream.Read(bytes, 0, (int)fileStream.Length);
                        return bytes;
                    }
                }
                catch
                {

                    return null;
                }
            }
            return null;
        }

        #endregion

        #region INVOKE METHOD
        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="Name">The name.</param>
        /// <param name="types">The types.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Object.</returns>
        internal static object InvokeMethod(this object o, string Name, Type[] types, params object[] arguments)
        {
            Type t = (o is Type) ? (Type)o : o.GetType();
            var info = t.GetTypeInfo();
            MethodInfo m = info.GetMethod(Name, types);
            if (m != null)
            {
                if (m.IsStatic)
                {
                    try
                    {
                        return m.Invoke(null, arguments);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        return m.Invoke(o, arguments);
                    }
                    catch { }
                }
            }
            return null;
        }

        /// <summary>
        /// Seeks the method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o">The o.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Arguments">The arguments.</param>
        /// <returns>MethodInfo.</returns>
        public static MethodInfo SeekMethod<T>(this T o, string Name, params Type[] Arguments)
        {
            Type t = (o is Type) ? o as Type : typeof(T);
            var info = t.GetTypeInfo();
            MethodInfo m = info.GetMethod(Name, Arguments);
            return m;
        }
        #endregion

        /// <summary>
        /// Properties the values.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Tuple&lt;System.String, System.Object&gt;[].</returns>
        public static Tuple<string, object>[] PropertyValues(this object source)
        {
            var type = source.GetType();
            var properties = type.GetTypeInfo().GetProperties(findAllBinding);

#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
            return properties.Select(
              (x) => new Tuple<string, object>(x.Name, x.GetValue(source, findAllBinding, null, null, Culture)
              )).ToArray();
#else
            return properties.Select(
             (x) => new Tuple<string, object>(x.Name, x.GetValue(source)
             )).ToArray();
#endif
        }

        #region private/ conversion methods
        /// <summary>
        /// Tries the convert.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool TryConvert<T>(this object value, out T result, Criteria criteria)
        {
            switch (criteria)
            {
                case Criteria.Equal:
                case Criteria.GreaterThan:
                case Criteria.LessThan:
                case Criteria.NotEqual:
                case Criteria.NotGreaterThan:
                case Criteria.NotLessThan:
                    return value.ConvertTo(out result, null);
                default:
                    if (value == null)
                    {
                        result = default(T);
                        return false;
                    }
                    else if (!value.ConvertTo(out result, null))
                        return value.ToString().ConvertTo(out result, null);
                    else
                        return true;
            }
        }

        /// <summary>
        /// Seeks the method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Name">The name.</param>
        /// <param name="Arguments">The arguments.</param>
        /// <returns>MethodInfo.</returns>
        static MethodInfo SeekMethod<T>(string Name, params Type[] Arguments)
        {
            MethodInfo m = typeof(T).GetTypeInfo().GetMethod(Name, Arguments);
            return m;
        }

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Name">The name.</param>
        /// <param name="types">The types.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Object.</returns>
        static object InvokeMethod<T>(string Name, Type[] types, params object[] arguments)
        {
            MethodInfo m = (typeof(T)).GetTypeInfo().GetMethod(Name, types);
            if (m != null)
            {
                try
                { return m.Invoke(null, arguments); }
                catch { }
            }
            return null;
        }

        /// <summary>
        /// Exists the method.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Arguments">The arguments.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        static bool ExistMethod(this object o, string Name, params Type[] Arguments)
        {
            return o.SeekMethod(Name, Arguments) != null;
        }

        /// <summary>
        /// Matcheds the specified types.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="types">The types.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        static bool Matched(this PropertyInfo p, params Type[] types)
        {
            ParameterInfo[] parameters = p.GetIndexParameters();
            if (types == null && parameters == null) return true;
            if (types.Length != parameters.Length) return false;
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] != parameters[i].ParameterType) return false;
            }
            return true;
        }

        /// <summary>
        /// Matcheds the specified name.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="types">The types.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        static bool Matched(this PropertyInfo p, string name, bool ignoreCase, params Type[] types)
        {
            Criteria crit = (!ignoreCase) ? Criteria.StringEqual : Criteria.StringEqualNoCase;
            if (!p.Name.Compare(crit, name)) return false;

            ParameterInfo[] parameters = p.GetIndexParameters();
            if (types == null && parameters == null) return true;
            if (types.Length != parameters.Length) return false;
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] != parameters[i].ParameterType) return false;
            }
            return true;
        }

        /// <summary>
        /// Matcheds the specified name.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        static bool Matched(this PropertyInfo p, string name, bool ignoreCase)
        {
            Criteria crit = (!ignoreCase) ? Criteria.StringEqual : Criteria.StringEqualNoCase;
            return (p.Name.Compare(crit, name));
        }

        /// <summary>
        /// Converts from string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        static bool ConvertFromString<T>(this string value, out T result)
        {
            result = default(T);
            if (string.IsNullOrEmpty(value)) return false;
            value = value.Trim();
            Type t = typeof(T);

            #region type
            if (t == typeof(Type))
            {
                result = (T)(object)Type.GetType(value, false);
                return true;
            }
            #endregion

            #region byte
            if (t == typeof(byte))
            {
                if (byte.TryParse(value, out byte i16))
                {
                    result = (T)(object)i16;
                    return true;
                }
            }
            #endregion

            #region sbyte
            if (t == typeof(sbyte))
            {
                if (sbyte.TryParse(value, out sbyte i16))
                {
                    result = (T)(object)i16;
                    return true;
                }
            }
            #endregion

            #region short
            if (t == typeof(short))
            {
                if (short.TryParse(value, out short i16))
                {
                    result = (T)(object)i16;
                    return true;
                }
            }
            #endregion

            #region ushort
            if (t == typeof(ushort))
            {
                if (ushort.TryParse(value, out ushort ui16))
                {
                    result = (T)(object)ui16;
                    return true;
                }
            }
            #endregion

            #region int
            if (t == typeof(int))
            {
                if (int.TryParse(value, out int i))
                {
                    result = (T)(object)i;
                    return true;
                }
            }
            #endregion

            #region uint
            if (t == typeof(uint))
            {
                if (uint.TryParse(value, out uint ui32))
                {
                    result = (T)(object)ui32;
                    return true;
                }
            }
            #endregion

            #region long
            else if (t == typeof(long))
            {
                if (long.TryParse(value, out long l))
                {
                    result = (T)(object)l;
                    return true;
                }
            }
            #endregion

            #region ulong
            else if (t == typeof(ulong))
            {
                if (ulong.TryParse(value, out ulong ui64))
                {
                    result = (T)(object)ui64;
                    return true;
                }
            }
            #endregion

            #region float
            else if (t == typeof(float))
            {
                if (float.TryParse(value, out float s))
                {
                    result = (T)(object)s;
                    return true;
                }
            }
            #endregion

            #region double
            else if (t == typeof(double))
            {
                if (double.TryParse(value, out double d))
                {
                    result = (T)(object)d;
                    return true;
                }
            }
            #endregion

            #region bool
            else if (t == typeof(bool))
            {
                if (string.Equals(value, "true") ||
                    string.Equals(value, "True") ||
                    string.Equals(value, "TRUE") ||
                    string.Equals(value, "Yes") ||
                    string.Equals(value, "yes") ||
                    string.Equals(value, "YES") ||
                string.Equals(value, "-1"))
                {
                    result = (T)(object)true;
                    return true;
                }
            }
            #endregion

            #region datetime
            else if (t == typeof(DateTime))
            {
                if (DateTime.TryParse(value, out DateTime dt))
                {
                    result = (T)(object)dt;
                    return true;
                }
            }
            #endregion

            #region enum
            if (t == typeof(Enum))
            {
                result = Enums.EnumValue<T>(value);
                return true;
            }
            #endregion

            #region nullable of generic
            else
            {
                var info = t.GetTypeInfo();
                if (info.IsValueType && info.IsGenericType && t.ToString().StartsWith("System.Nullable`1"))
                {
                    t = Factory.newGenre(t).GenricParams[0].Value;
                    result = (T)(object)Convert.ChangeType(value, t);
                    return true;
                }
            }
            #endregion

            #region else
            if (Converters.Count != 0)
            {
                foreach (var converter in Converters.Values)
                {
                    if (converter.ConvertTo(value, out result))
                        return true;
                }
            }
            #endregion
            return false;
        }
        #endregion
    }
}
