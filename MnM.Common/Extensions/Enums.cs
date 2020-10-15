/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
    public static class Enums
    {
        #region INCLUDE
        /// <summary>
        /// Includes the specified value2.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>T.</returns>
        public static T Include<T>(this T value1, T value2)      
        {
            return (T)(object)(Convert.ToInt32(value1) | Convert.ToInt32(value2));
        }

        public static bool Includes<T>(this T value, T valueToCheck) 
        {
            return value.HasFlag(valueToCheck);
        }

        /// <summary>
        /// Includeses the specified value set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="valueSet">The value set.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Includes<T>(this T value, params T[] valueSet)  
        {
            if (valueSet.Length == 0) return false;
            foreach (var item in valueSet)
            {
                if (value.HasFlag(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Includeses the specified first occurance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="firstOccurance">The first occurance.</param>
        /// <param name="valueSet">The value set.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Includes<T>(this Enum value,
            out T firstOccurance, params Enum[] valueSet)   
        {
            firstOccurance = default(T);
            if (valueSet.Length == 0) return false;
            foreach (var item in valueSet)
            {
                if (value.HasFlag(item))
                {
                    firstOccurance = (T)(object)item;
                    return true;
                }
            }
            return false;
        }

       public static bool HasFlag<T>(this T value, T check) 
        {
            var a = Convert.ToInt32(value);
            var b = Convert.ToInt32(check);
            return((a & b) == b);
        }

        /// <summary>
        /// Determines whether [is one of] [the specified value set].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="valueSet">The value set.</param>
        /// <returns><c>true</c> if [is one of] [the specified value set]; otherwise, <c>false</c>.</returns>
        public static bool IsOneOf<T>(this T value, params T[] valueSet)
        {
            foreach (var item in valueSet)
            {
                if (Equals(value, item)) return true;
            }
            return false;
        }
        public static bool IsOneOf<T>(this T? value, params T[] valueSet) where T : struct
        {
            foreach (var item in valueSet)
            {
                if (Equals(value, item)) return true;
            }
            return false;
        }
        #endregion

        #region EXCLUDE
        /// <summary>
        /// Excludes the specified value2.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>T.</returns>
        public static T Exclude<T>(this T value1, T value2)  
        {
            return (T)(object)(Convert.ToInt32(value1) & ~Convert.ToInt32(value2));
        }

        public static T Exclude<T>(this T value1, params T[] values)  
        {
            var v = Convert.ToInt32(value1);
            foreach (var v1 in values)
            {
                v = v & ~Convert.ToInt32(v1);
            }
            return (T)(object)v;
        }

        public static T Replace<T>(this T value, T oldValue, T newValue)  
        {
            return (T)(object)((Convert.ToInt32(value) & ~Convert.ToInt32(oldValue))
                | Convert.ToInt32(newValue));
        }

        /// <summary>
        /// Excludeses the specified value set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="valueSet">The value set.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool Excludes<T>(this T value, params T[] valueSet)
        {
            if (valueSet.Length == 0) return false;
            foreach (var item in valueSet)
                if (value.HasFlag(item)) 
                    return false;
            
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
        public static bool Excludes<T>(this T value, out T firstOccurance, params T[] valueSet)
        {
            firstOccurance = default(T);
            if (valueSet.Length == 0) return false;
            foreach (var item in valueSet)
            {
                if (value.HasFlag(item))
                {
                    firstOccurance =(T)(Object) item;
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
        public static T ConcateEnumValue<T>(this Enum nameprefix, Enum namesuffix)
        {
            string s = EnumName(nameprefix) + EnumName(namesuffix);
            if (typeof(T).BaseType == typeof(Enum))
            {
                return (T)Enum.Parse(typeof(T), s, true);
            }
            return default(T);
        }

        /// <summary>
        /// Gets the name of the concate.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="suffix">The suffix.</param>
        /// <returns>System.String.</returns>
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
