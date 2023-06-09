/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static class Enums
    {
        #region CONSTANT LISTING
        /// <summary>
        /// https://stackoverflow.com/questions/41477862/linq-get-the-static-class-constants-as-list
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="classType"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<NamedValue<V>> ListConstansts<V>(this Type classType)  
        {
            TypeInfo info = classType.GetTypeInfo();
            
            var fields = info.GetFields(BindingFlags.Static | BindingFlags.Public);            
            var list = fields.Where(x=>x.IsStatic).Select(x => Lot.Create(x.Name, x.GetValue(null)));
            var items = list.Where(x => x.Item2 is V ).Select(x => NamedValue.Create(x.Item1, (V)x.Item2));
            return items;
        }


        /// <summary>
        /// https://stackoverflow.com/questions/41477862/linq-get-the-static-class-constants-as-list
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="classType"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<NamedValue<V>> ListConstansts<V>(this Type classType, Predicate<V> condition)
        {
            TypeInfo info = classType.GetTypeInfo();
            var fields = info.GetFields(BindingFlags.Static | BindingFlags.Public);
            var list = fields.Select(x => NamedValue.Create(x.Name, x.GetValue(null)));

            var items = list.Where(x => x.Value is V && condition((V)x.Value)).Select(x => NamedValue.Create(x.Name, (V)x.Value));
            return items;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/41477862/linq-get-the-static-class-constants-as-list
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="classType"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<NamedValue<V>> ListConstansts<V>(this Type classType, Predicate<string> condition)
        {
            TypeInfo info = classType.GetTypeInfo();
            var fields = info.GetFields(BindingFlags.Static | BindingFlags.Public);
            var list = fields.Select(x => NamedValue.Create(x.Name, x.GetValue(null)));

            var items = list.Where(x => x.Value is V && condition(x.Name)).Select(x => NamedValue.Create(x.Name, (V)x.Value));
            return items;
        }

        /// <summary>
        /// https://stackoverflow.com/questions/41477862/linq-get-the-static-class-constants-as-list
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="classType"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<NamedValue<V>> ListConstansts<V>(this Type classType, ICollection<V> filter, bool inverse = false)
        {
            if(inverse)
                return ListConstansts<V>(classType, (value) => !filter.Contains(value));
            return ListConstansts<V>(classType, (value) => filter.Contains(value));
        }

        /// <summary>
        /// https://stackoverflow.com/questions/41477862/linq-get-the-static-class-constants-as-list
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="classType"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<NamedValue<V>> ListConstansts<V>(this Type classType, ICollection<string> filter, bool inverse = false)
        {
            if (inverse)
                return ListConstansts<V>(classType, (value) => !filter.Contains(value));
            return ListConstansts<V>(classType, (value) => filter.Contains(value));
        }

        /// <summary>
        /// https://stackoverflow.com/questions/41477862/linq-get-the-static-class-constants-as-list
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="classType"></param>
        /// <returns></returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDictionary<V, string> ConstantDir<V>(this Type classType)
        {
            TypeInfo info = classType.GetTypeInfo();

            var fields = info.GetFields(BindingFlags.Static | BindingFlags.Public);
            var list = fields.Where(x => x.IsStatic);
            var items = new Dictionary<V, string>(fields.Length);
            foreach (var item in list)
                items[(V)item.GetValue(null)] = item.Name;
            return items;
        }
        #endregion

        #region INCLUDEs - EXCLUDES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Includes<T>(this T value, T valueToCheck)
            where T : Enum
        {
            var v1 = Convert.ToInt32(value);
            var v2 = Convert.ToInt32(valueToCheck);
            return (v1 & v2) == v2;
        }

        /// <summary>
        /// Includeses the specified value set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="valueSet">The value set.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Includes<T>(this T value, params T[] valueSet)
            where T : Enum
        {
            if (valueSet.Length == 0) return false;
            var val = Convert.ToInt32(value);
            var items = valueSet.Select(x => Convert.ToInt32(x));
            foreach (var item in items)
            {
                if ((item & val) == val)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Excludeses the specified first occurance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="firstOccurance">The first occurance.</param>
        /// <param name="valueSet">The value set.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Includes<T>(this T value, out T firstOccurance, params T[] valueSet)
            where T : Enum
        {
            firstOccurance = default(T);
            if (valueSet.Length == 0) return false;
            if (valueSet.Length == 0) return false;
            foreach (var item in valueSet)
            {
                if (item.HasFlag(value))
                {
                    firstOccurance = item;
                    return true;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether [is one of] [the specified value set].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="valueSet">The value set.</param>
        /// <returns><c>true</c> if [is one of] [the specified value set]; otherwise, <c>false</c>.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOneOf<T>(this T value, params T[] valueSet)
            where T : Enum
        {
            foreach (var item in valueSet)
            {
                if (Equals(value, item)) return true;
            }
            return false;
        }

        /// <summary>
        /// Excludeses the specified value set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="valueSet">The value set.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Excludes<T>(this T value, params T[] valueSet)
            where T : Enum
        {
            if (valueSet.Length == 0) return false;
            foreach (var item in valueSet)
            {
                if (item.HasFlag(value))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Excludeses the specified first occurance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="firstOccurance">The first occurance.</param>
        /// <param name="valueSet">The value set.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Excludes<T>(this T value, out T firstOccurance, params T[] valueSet)
            where T : Enum
        {
            firstOccurance = default(T);
            if (valueSet.Length == 0) return false;
            if (valueSet.Length == 0) return false;
            foreach (var item in valueSet)
            {
                if (item.HasFlag(value))
                {
                    firstOccurance = item;
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region GET NAME
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string EnumName(this Enum value)
        {
            return value.ToString();
        }
        #endregion

        #region GET VALUE
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnumValue<T>(string value) 
        {
            if (string.IsNullOrEmpty(value)) return default(T);
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnumValue<T>(int value) where T:Enum
        {
            try
            {
                return (T)(object)value;
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="success">if set to <c>true</c> [success].</param>
        /// <returns>T.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnumValue<T>(string value, out bool success)  
        {
            success = false;
            try
            {
                T val = (T)Enum.Parse(typeof(T), value, true);
                success = true;
                return val;
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnumValue<T>(this object value) where T:Enum
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>T.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T EnumValue<T>(this Enum value)
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch { }
            return default(T);
        }
        #endregion

        #region CONCAT ENUM NAME/VALUE
        /// <summary>
        /// Gets the concate value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameprefix">The nameprefix.</param>
        /// <param name="namesuffix">The namesuffix.</param>
        /// <returns>T.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ConcateEnumValue<T>(this T nameprefix, T namesuffix) where T : Enum
        {
            string s = EnumName(nameprefix) + EnumName(namesuffix);
            return (T)Enum.Parse(typeof(T), s, true);
        }

        /// <summary>
        /// Gets the name of the concate.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>System.String.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcateEnumName<T>(this T prefix, T suffix) where T : Enum
        {
            return prefix.EnumName() + suffix.EnumName();
        }

        /// <summary>
        /// Gets the concate value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nameprefix">The nameprefix.</param>
        /// <param name="namesuffix">The namesuffix.</param>
        /// <returns>T.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ConcateEnumValue<T>(this Enum nameprefix, Enum namesuffix)
        {
            string s = EnumName(nameprefix) + EnumName(namesuffix);
            return (T)Enum.Parse(typeof(T), s, true);
        }

        /// <summary>
        /// Gets the name of the concate.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>System.String.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ConcateEnumName(this Enum prefix, Enum suffix)
        {
            return prefix.EnumName() + suffix.EnumName();
        }
        #endregion

        #region ENUM SOURCE LIST
#if Data
        /// <summary>
        /// Sets the enum source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        public static void SetEnumSource<T>(this IRowSourceBoundControl list) where T : Enum
        {
            list.Source.UnderlyingSource = EnumTag<T>.GetList();
            list.Source.DisplayMember = "Name";
            list.Source.ValueMember = "Value";
        }
#endif
        #endregion
    }
}
