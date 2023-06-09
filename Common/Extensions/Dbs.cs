/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace MnM.GWS
{
    public static partial class Dbs
    {
        #region MEMBERS
        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>IList&lt;System.String&gt;.</returns>
        public static IList<string> Members(this object element)
        {
            #region ISource
#if Data
                    if (element is ISourceBoundControl)
                    {
                        var source = (element as ISourceBoundControl).Source?.UnderlyingSource;
                        return source?.Members();
                    }
                    else if (element is ISource)
                    {
                        return (element as ISource).UnderlyingSource?.Members();
                    }
#endif
            #endregion

            #region data table
            if (element is DataTable)
            {
                var dt = element as DataTable;
                var collection = new PrimitiveList<string>(dt.Columns.Count);
                foreach (DataColumn item in dt.Columns)
                {
                    collection.Add(item.ColumnName);
                }
                return collection;
            }
            #endregion

            #region datarow
            else if (element is DataRow)
            {
                return (element as DataRow).Table.Members();
            }
            #endregion

            #region datarowview
            else if (element is DataRowView)
            {
                return (element as DataRowView).DataView.Table.Members();
            }
            #endregion

            #region Idatareader
            else if (element is IDataReader)
            {
                return (element as IDataReader).GetSchemaTable().Members();
            }
            #endregion

            #region Idatarecord
            else if (element is IDataRecord)
            {
                var record = element as IDataRecord;
                var collection = new System.Collections.Generic.List<string>(record.FieldCount);
                for (int i = 0; i < record.FieldCount; i++)
                {
                    var name = record.GetName(i);
                    collection.Add(name);
                }
                return collection;
            }
            #endregion

            #region IListsource
            else if (element is IListSource)
            {
                (element as IListSource).GetList().Members();
            }
            #endregion

            #region PropertyDescriptorCollection
            else if (element is PropertyDescriptorCollection)
            {
                var properties = element as PropertyDescriptorCollection;
                var collection = new System.Collections.Generic.List<string>(properties.Count);

                foreach (PropertyDescriptor item in properties)
                {
                    collection.Add(item.Name);
                }
                return collection;
            }
            #endregion

            #region ITypedList for binding source, data view etc
            else if (element is ITypedList)
            {
                return (element as ITypedList).GetItemProperties(null).Members();
            }
            #endregion

            #region customtypedescriptor
            else if (element is ICustomTypeDescriptor)
            {
                return (element as ICustomTypeDescriptor).GetProperties().Members();
            }
            #endregion

            #region ienumerable
            else if (element is IEnumerable)
            {
                foreach (var item in (element as IEnumerable))
                {
                    if (item == null) continue;
                    return item.Members();
                }
                return Factory.newGenre(element).Members();
            }
            #endregion

            #region type
            else if (element is Type)
            {
                var properties = (element as Type).MyProperties();
                var collection = new PrimitiveList<string>(properties.Length);
                foreach (var item in properties)
                {
                    collection.Add(item.Name);
                }
                return collection;
            }
            #endregion

            #region genre
            else if (element is IGenre)
            {
                var genre = element as IGenre;
                if (genre.HasTEnumerableInterface)
                {
                    IGenre tGenre;
                    if (genre.GetTListOrIEnumerableGenricParams(out tGenre))
                    {
                        return Members(tGenre.Value);
                    }
                }
                else Members(genre.Value);
            }
            #endregion

            #region primitive data type
            else if (element.IsPrimitive())
            {
                var dic = new PrimitiveList<string>(1);
                dic.Add("Value");
                return dic;
            }
            #endregion

            else if (element == null)
                return Factory.newGenre(element).Members();
            return Members(element.GetType());
        }
        #endregion

        #region OK
        /// <summary>
        /// Determines whether the specified column is ok.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns><c>true</c> if the specified column is ok; otherwise, <c>false</c>.</returns>
        public static bool OK(this DataRow row, string column)
        {
            bool notok = !row.Table.Columns.Contains(column) ||
                row[column] is System.DBNull ||
                row[column] == null;
            return !notok;
        }

        /// <summary>
        /// Determines whether the specified column is ok.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns><c>true</c> if the specified column is ok; otherwise, <c>false</c>.</returns>
        public static bool OK(this DataRow row, int column)
        {
            bool notok = column >= row.ItemArray.Length ||
                row[column] is System.DBNull ||
                row[column] == null;
            return !notok;
        }
        #endregion

        #region VALUE OF
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ValueOf<T>(this DataRow row, string column, out T value)
        {
            value = default(T);

            if (!row.OK(column)) return false;

            if (typeof(T).BaseType == typeof(Enum))
            {
                bool ok;
                value = Enums.EnumValue<T>(row[column].ToString(), out ok);
                return ok;
            }
            else
            {
                return row[column].ConvertTo(out value, null);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>T.</returns>
        public static T ValueOf<T>(this DataRow row, string column)
        {
            T value = default(T);
            if (!row.OK(column)) return value;
            if (typeof(T).BaseType == typeof(Enum))
            {
                value = Enums.EnumValue<T>(row[column].ToString());
            }
            else
            {
                value = row[column].ConvertTo<T>();
            }
            return value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ValueOf<T>(this DataRow row, int column, out T value)
        {
            value = default(T);

            if (!row.OK(column)) return false;

            if (typeof(T).BaseType == typeof(Enum))
            {
                bool ok;
                value = Enums.EnumValue<T>(row[column].ToString(), out ok);
                return ok;
            }
            else
            {
                return row[column].ConvertTo(out value, null);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>T.</returns>
        public static T ValueOf<T>(this DataRow row, int column)
        {
            T value = default(T);
            if (!row.OK(column)) return value;

            if (typeof(T).BaseType == typeof(Enum))
            {
                value = Enums.EnumValue<T>(row[column].ToString());
            }
            else
            {
                value = row[column].ConvertTo<T>();
            }
            return value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ValueOf<T>(this DataRowView row, string column, out T value)
        {
            return row.Row.ValueOf(column, out value);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>T.</returns>
        public static T ValueOf<T>(this DataRowView row, string column)
        {
            return row.Row.ValueOf<T>(column);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool ValueOf<T>(this DataRowView row, int column, out T value)
        {
            return row.Row.ValueOf(column, out value);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>T.</returns>
        public static T ValueOf<T>(this DataRowView row, int column)
        {
            return row.Row.ValueOf<T>(column);
        }
        #endregion

        #region SET VALUE OF
        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf<T>(this DataRow row, string column, T value)
        {
            try
            {
                if (!row.Table.Columns.Contains(column)) return false;
                row[column] = value;
                return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf<T>(this DataRow row, string column, object value)
        {
            try
            {
                if (!row.Table.Columns.Contains(column)) return false;
                T result;
                if (value.ConvertTo(out result, row.Table.Columns[column].DataType))
                {
                    row[column] = result;
                    return true;
                }
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf<T>(this DataRow row, int column, object value)
        {
            try
            {
                if (column >= row.Table.Columns.Count) return false;
                T result;
                if (value.ConvertTo(out result, row.Table.Columns[column].DataType))
                {
                    row[column] = result;
                    return true;
                }
                else return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf<T>(this DataRow row, int column, T value)
        {
            try
            {
                if (column >= row.Table.Columns.Count) return false;
                row[column] = value;
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf(this DataRow row, int column, object value)
        {
            try
            {
                if (column >= row.Table.Columns.Count) return false;
                object o;
                bool ok = value.ConvertTo(out o, row.Table.Columns[column].DataType);
                if (ok) row[column] = o;
                return ok;
            }
            catch { return false; }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf(this DataRow row, string column, object value)
        {
            try
            {
                if (!row.Table.Columns.Contains(column)) return false;
                object o;
                bool ok = value.ConvertTo(out o, row.Table.Columns[column].DataType);
                if (ok) row[column] = o;
                return ok;
            }
            catch { return false; }
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf<T>(this DataRowView row, string column, T value)
        {
            return row.Row.SetValueOf(column, value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf<T>(this DataRowView row, string column, object value)
        {
            return row.Row.SetValueOf<T>(column, value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf<T>(this DataRowView row, int column, object value)
        {
            return row.Row.SetValueOf<T>(column, value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf<T>(this DataRowView row, int column, T value)
        {
            return row.Row.SetValueOf<T>(column, value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf(this DataRowView row, int column, object value)
        {
            return row.Row.SetValueOf(column, value);
        }

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        internal static bool SetValueOf(this DataRowView row, string column, object value)
        {
            return row.Row.SetValueOf(column, value);
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="colDelimiter">The col delimiter.</param>
        /// <param name="rowDelimiter">The row delimiter.</param>
        /// <param name="distinct">if set to <c>true</c> [distinct].</param>
        /// <param name="ignoreNullValues">if set to <c>true</c> [ignore null values].</param>
        /// <returns>System.String.</returns>
        internal static string GetString(this IDataReader reader, string colDelimiter = ",",
            string rowDelimiter = "\r", bool distinct = false, bool ignoreNullValues = false)
        {
            object[] values = new object[reader.FieldCount];
            colDelimiter = colDelimiter ?? ",";
            rowDelimiter = rowDelimiter ?? "\r";

            if (distinct)
            {
                var list = new HashSet<string>();
                string value = null;

                while (reader.Read())
                {
                    reader.GetValues(values);

                    if (!ignoreNullValues)
                        value = string.Join(colDelimiter, values);
                    else
                        value = string.Join(colDelimiter, values.Where(v => v != null));

                    if (!list.Contains(value))
                        list.Add(value);

                }
                return string.Join(rowDelimiter, list);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                while (reader.Read())
                {
                    sb.Append(rowDelimiter);
                    reader.GetValues(values);

                    if (!ignoreNullValues)
                        sb.Append(string.Join(colDelimiter, values));
                    else
                        sb.Append(string.Join(colDelimiter, values.Where(v => v != null)));
                }
                if (sb.Length > 0)
                {
                    if (rowDelimiter != null) sb.Remove(0, rowDelimiter.Length);
                }
                return sb.ToString();
            }
        }
        #endregion
    }
}
#endif