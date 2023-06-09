/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static partial class Numbers
    {
        #region FLAG CONSTS
        /// <summary>
        /// 0
        /// </summary>
        public const byte Flag0 = 0x0;
        /// <summary>
        /// 
        /// </summary>
        public const byte Flag1 = 0x1;
        /// <summary>
        /// 
        /// </summary>
        public const byte Flag2 = 0x2;
        public const byte Flag3 = 0x4;
        public const byte Flag4 = 0x8;
        public const byte Flag5 = 0x10;
        public const byte Flag6 = 0x20;
        public const byte Flag7 = 0x40;
        public const byte Flag8 = 0x80;
        public const ushort Flag9 = 0x100;
        public const ushort Flag10 = 0x200;
        public const ushort Flag11 = 0x400;
        public const ushort Flag12 = 0x800;
        public const ushort Flag13 = 0x1000;
        public const ushort Flag14 = 0x2000;
        public const ushort Flag15 = 0x4000;
        public const ushort Flag16 = 0x8000;
        public const uint Flag17 = 0x10000;
        public const uint Flag18 = 0x20000;
        public const uint Flag19 = 0x40000;
        public const uint Flag20 = 0x80000;
        public const uint Flag21 = 0x100000;
        public const uint Flag22 = 0x200000;
        public const uint Flag23 = 0x400000;
        public const uint Flag24 = 0x800000;
        public const uint Flag25 = 0x1000000;
        public const uint Flag26 = 0x2000000;
        public const uint Flag27 = 0x4000000;
        public const uint Flag28 = 0x8000000;
        public const uint Flag29 = 0x10000000;
        public const uint Flag30 = 0x20000000;
        public const uint Flag31 = 0x40000000;
        public const uint Flag32 = 0x80000000;
        public const ulong Flag33 = 0x100000000;
        public const ulong Flag34 = 0x200000000;
        public const ulong Flag35 = 0x400000000;
        public const ulong Flag36 = 0x800000000;
        public const ulong Flag37 = 0x1000000000;
        public const ulong Flag38 = 0x2000000000;
        public const ulong Flag39 = 0x4000000000;
        public const ulong Flag40 = 0x8000000000;
        public const ulong Flag41 = 0x10000000000;
        public const ulong Flag42 = 0x20000000000;
        public const ulong Flag43 = 0x40000000000;
        public const ulong Flag44 = 0x80000000000;
        public const ulong Flag45 = 0x100000000000;
        public const ulong Flag46 = 0x200000000000;
        public const ulong Flag47 = 0x400000000000;
        public const ulong Flag48 = 0x800000000000;
        public const ulong Flag49 = 0x1000000000000;
        public const ulong Flag50 = 0x2000000000000;
        public const ulong Flag51 = 0x4000000000000;
        public const ulong Flag52 = 0x8000000000000;
        public const ulong Flag53 = 0x10000000000000;
        public const ulong Flag54 = 0x20000000000000;
        public const ulong Flag55 = 0x40000000000000;
        public const ulong Flag56 = 0x80000000000000;
        public const ulong Flag57 = 0x100000000000000;
        public const ulong Flag58 = 0x200000000000000;
        public const ulong Flag59 = 0x400000000000000;
        public const ulong Flag60 = 0x800000000000000;
        public const ulong Flag61 = 0x1000000000000000;
        public const ulong Flag62 = 0x2000000000000000;
        public const ulong Flag63 = 0x4000000000000000;
        public const ulong Flag64 = 0x8000000000000000;
        #endregion

        #region ORDER
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Order(ref int lower, ref int upper)
        {
            if (lower <= upper)
                return;
            var l = lower;
            lower = upper;
            upper = l;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Order(ref byte lower, ref byte upper)
        {
            if (lower <= upper)
                return;
            var l = lower;
            lower = upper;
            upper = l;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Order(ref int? lower, ref int? upper)
        {
            if (lower <= upper)
                return;
            var l = lower;
            lower = upper;
            upper = l;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Order(ref double lower, ref double upper)
        {
            if (lower <= upper)
                return;
            var l = lower;
            lower = upper;
            upper = l;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Order(ref float lower, ref float upper)
        {
            if (lower <= upper)
                return;
            var l = lower;
            lower = upper;
            upper = l;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Order(ref float? lower, ref float? upper)
        {
            if (lower <= upper)
                return;
            var l = lower;
            lower = upper;
            upper = l;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void OrderY(ref float x1, ref float y1, ref float x2, ref float y2)
        {
            if (y1 > y2)
            {
                Swap(ref y1, ref y2);
                Swap(ref x1, ref x2);
            }
        }
        #endregion

        #region SWAP
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T x, ref T y)
        {
            var temp = x;
            x = y;
            y = temp;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(ref T x1, ref T y1, ref T x2, ref T y2)
        {
            var temp = x1;
            x1 = x2;
            x2 = temp;
            temp = y1;
            y1 = y2;
            y2 = temp;
        }
        #endregion

        #region ASSIGN
        public static void NoNullAssign<T>(this T? newValue, ref T current) where T : struct
        {
            if (newValue == null)
                return;
            current = newValue.Value;
        }
        public static void NoNullAssign<T>(this T newValue, ref T current) where T : class
        {
            if (newValue == null)
                return;
            current = newValue;
        }
        #endregion

        #region ROUNDING
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ceiling(this float val)
        {
            return (int)Math.Ceiling(val);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this float val)
        {
            return (int)Math.Round(val);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this float val, int digits)
        {
            return (int)Math.Round(val, digits);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RoundF(this float val, int digits)
        {
            return (float)Math.Round(val, digits);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Floor(this float val)
        {
            return (int)Math.Floor(val);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Floor(this double val)
        {
            return (int)Math.Floor(val);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this float val, MidpointRounding r)
        {
            return (int)Math.Round(val, r);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ceiling(this double val)
        {
            return (int)Math.Ceiling(val);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Round(this double val)
        {
            return (int)Math.Round(val);
        }
        #endregion

        #region ABS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(this float value)
        {
            if (value < 0)
                value = -value;
            return value;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int value)
        {
            if (value < 0)
                value = -value;
            return value;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Abs(this double value)
        {
            if (value < 0)
                value = -value;
            return value;
        }
        #endregion

        #region INT EQUAL
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IntEqual(this float a, int b)
        {
            return ((int)a == b || a.Round() == b || a.Ceiling() == b);
        }
        #endregion

        #region FRACTION
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float? Fraction(this float p)
        {
            var f = p - (int)p;
            if (f == 0)
                return null;
            return f;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float? FractionInv(this float p)
        {
            var f = p - (int)p;
            if (f == 0)
                return null;
            return 1 - f;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Fractional(this float p)
        {
            var f = p - (int)p;
            return (f != 0);
        }
        #endregion

        #region SQUARE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sqr(this int number) =>
            number * number;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sqr(this double number) =>
            number * number;

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sqr(this float number) =>
            number * number;
        #endregion

        #region AVERAGE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Avg(params float[] numbers)
        {
            if (numbers.Length == 0)
                return 0f;
            var num = 0f;
            foreach (var n in numbers)
                num += n;
            return num / numbers.Length;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Avg(params int[] numbers)
        {
            if (numbers.Length == 0)
                return 0;
            var num = 0;
            foreach (var n in numbers)
                num += n;
            return num / numbers.Length;
        }
        #endregion

        #region MIN - MAX
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MinFrom(this IEnumerable<float> numbers, int start, int counter)
        {
            int i = -1;
            int j = start;
            var num = -1f;

            foreach (var n in numbers)
            {
                ++i;
                if (i < start)
                    continue;
                if (i == j)
                {
                    j += counter;
                    if (num == -1f || n < num)
                        num = n;
                }
            }
            return num;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MaxFrom(this IEnumerable<float> numbers, int start, int counter)
        {
            int i = -1;
            int j = start;
            var num = 0f;

            foreach (var n in numbers)
            {
                ++i;
                if (i < start)
                    continue;
                if (i == j)
                {
                    j += counter;
                    if (num == 0 || n > num)
                        num = n;
                }
            }
            return num;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MinFrom(this IEnumerable<int> numbers, int start, int counter)
        {
            int i = -1;
            int j = start;
            var num = -1;

            foreach (var n in numbers)
            {
                ++i;
                if (i < start)
                    continue;
                if (i == j)
                {
                    j += counter;
                    if (num == -1 || n < num)
                        num = n;
                }
            }
            return num;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MaxFrom(this IEnumerable<int> numbers, int start, int counter)
        {
            int i = -1;
            int j = start;
            var num = 0;

            foreach (var n in numbers)
            {
                ++i;
                if (i < start)
                    continue;
                if (i == j)
                {
                    j += counter;
                    if (num == 0 || n > num)
                        num = n;
                }
            }
            return num;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(params float[] numbers) =>
            (numbers as IEnumerable<float>).Min();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(params float[] numbers) =>
            (numbers as IEnumerable<float>).Max();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(params int[] numbers) =>
            (numbers as IEnumerable<int>).Min();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(params int[] numbers) =>
            (numbers as IEnumerable<int>).Max();
        #endregion

        #region IS WITHIN
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithIn(this float value, float min, float max)
        {
            Order(ref min, ref max);
            return value >= min && value <= max;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithIn(this int value, float min, float max)
        {
            Order(ref min, ref max);
            return value >= min && value <= max;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithIn(this float? value, float min, float max)
        {
            Order(ref min, ref max);
            return value >= min && value <= max;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithIn(this double value, double min, double max)
        {
            Order(ref min, ref max);
            return value >= min && value <= max;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithIn(this double? value, double min, double max)
        {
            Order(ref min, ref max);
            return value >= min && value <= max;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithIn(this int value, int min, int max)
        {
            Order(ref min, ref max);
            return value >= min && value <= max;
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWithIn(this int? value, int min, int max)
        {
            Order(ref min, ref max);
            return value >= min && value <= max;
        }
        #endregion

        #region IN BETWEEN
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InBetween(this float value, float min, float max)
        {
            Order(ref min, ref max);
            return value > min && value < max;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InBetween(this double value, double min, double max)
        {
            Order(ref min, ref max);
            return value > min && value < max;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InBetween(this int value, int min, int max)
        {
            Order(ref min, ref max);
            return value > min && value < max;
        }
        #endregion

        #region MIDDLE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Middle(float min, float max)
        {
            Order(ref min, ref max);
            return min + (max - min) / 2f;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Middle(int min, int max)
        {
            Order(ref min, ref max);
            return min + (max - min) / 2;
        }
        #endregion

        #region STAY WITHIN
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Confine(float min, float max, ref float value)
        {
            if (min == 0 && max == 0)
                return;

            Order(ref min, ref max);
            if (value < min)
                value = min;
            if (value > max)
                value = max;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Confine(int min, int max, ref int value)
        {
            if (min == 0 && max == 0)
                return;

            Order(ref min, ref max);
            if (value < min)
                value = min;
            if (value > max)
                value = max;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Confine(byte min, byte max, ref byte value)
        {
            if (min == 0 && max == 0)
                return;

            Order(ref min, ref max);
            if (value < min)
                value = min;
            if (value > max)
                value = max;
        }
        #endregion

        #region POSITIVE LENGTH
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PositiveLength(ref int start, ref int end, out int length)
        {
            Order(ref start, ref end);
            length = 0;
            if (start < 0)
            {
                length = start;
                start = 0;
            }
            length += end - start;

            if (length == 0)
                length = 1;
            if (length < 0)
                return false;
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool PositiveLength(ref float start, ref float end, out float length)
        {
            Order(ref start, ref end);
            length = 0;
            if (start < 0)
            {
                length = start;
                start = 0;
            }
            length += end - start;

            if (length == 0)
                length = 1;
            if (length < 0)
                return false;
            return true;
        }
        #endregion

        #region IS NUMERIC - IS DATE
        public static bool IsNumeric(this object value)
        {
            if (
                value is double || value is Single || value is decimal
                || value is Int16 || value is Int32 || value is Int64
                || value is UInt16 || value is UInt32 || value is UInt64)
                return true;
            else
            {
                var o = value?.ToString()?.TrimStart();
                if (string.IsNullOrEmpty(o))
                    return false;

                bool point = false;

                foreach (var item in o)
                {
                    if (item == ',')
                        continue;
                    if (item == '.')
                    {
                        if (point)
                            return false;
                        else
                        {
                            point = true;
                            continue;
                        }
                    }
                    else if (!char.IsDigit(item))
                        return false;
                }
                return true;
            }
        }
        #endregion

        #region TO NUMBER
        public static bool ToNumber(this object value, out double result)
        {
            result = 0;
            if (!value.IsNumeric())
                return false;
            result = double.Parse(value.ToString());
            return true;
        }
        public static bool ToNumber(this object value, out float result)
        {
            result = 0;
            if (!value.IsNumeric())
                return false;
            result = Single.Parse(value.ToString());
            return true;
        }
        public static bool ToNumber(this object value, out decimal result)
        {
            result = 0;
            if (!value.IsNumeric())
                return false;
            result = decimal.Parse(value.ToString());
            return true;
        }

        public static bool ToNumber(this object value, out short result)
        {
            result = 0;
            if (!value.IsNumeric())
                return false;
            result = Int16.Parse(value.ToString());
            return true;
        }
        public static bool ToNumber(this object value, out int result)
        {
            result = 0;
            if (!value.IsNumeric())
                return false;
            result = Int32.Parse(value.ToString());
            return true;
        }
        #endregion

        #region FIX
        public static double Fix(this double Value)
        {
            if (Value >= 0)
                return System.Math.Floor(Value);
            else
                return System.Math.Ceiling(Value);
        }
        public static float Fix(this float Value)
        {
            if (Value >= 0)
                return (float)System.Math.Floor(Value);
            else
                return (float)System.Math.Ceiling(Value);
        }
        #endregion

        #region HASH HELPER
        // Licensed to the.NET Foundation under one or more agreements.
        // The .NET Foundation licenses this file to you under the MIT license.
        // See the LICENSE file  the project root for more information.
        public static int Combine(int h1, int h2)
        {
            // RyuJIT optimizes this to use the ROL instruction
            // Related GitHub pull request: dotnet/coreclr#1830
            uint rol5 = ((uint)h1 << 5) | ((uint)h1 >> 27);
            return ((int)rol5 + h1) ^ h2;
        }
        #endregion

        #region NUMERIC STRING COMPARE
        public static int NumCompare(string a, string b) =>
            NumericStringComparer.Compare(a, b);
        #endregion

        #region PRIVAE METHODS
        internal static int? parseBool(this string input)
        {
            if (string.IsNullOrEmpty(input)) return null;
            string[] result = new string[0];

            switch (input.ToUpper())
            {
                case "0":
                case "NO":
                case "FALSE":
                    return 0;
                case "-1":
                case "YES":
                case "TRUE":
                    return -1;
                default:
                    return null;
            }
        }
        #endregion

        /// <summary>
        /// Class StringNumericComparer. This class cannot be inherited.
        /// </summary>
        /// <seealso cref="System.Collections.Generic.IComparer{System.String}" />
        sealed class NumericStringComparer : IComparer<string>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NumericStringComparer"/> class.
            /// </summary>
            public NumericStringComparer() { }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
            int IComparer<string>.Compare(string x, string y)
            {
                return Compare(x, y);
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.</returns>
            public static int Compare(string x, string y)
            {
                //get rid of special cases
                if ((x == null) && (y == null)) return 0;
                else if (x == null) return -1;
                else if (y == null) return 1;

                if ((x.Equals(String.Empty) && (y.Equals(String.Empty)))) return 0;
                else if (x.Equals(String.Empty)) return -1;
                else if (y.Equals(String.Empty)) return -1;

                //WE style, special case
                bool sp1 = Char.IsLetterOrDigit(x, 0);
                bool sp2 = Char.IsLetterOrDigit(y, 0);
                if (sp1 && !sp2) return 1;
                if (!sp1 && sp2) return -1;

                int i1 = 0, i2 = 0; //current index
                int r = 0; // temp result
                while (true)
                {
                    bool c1 = Char.IsDigit(x, i1);
                    bool c2 = Char.IsDigit(y, i2);
                    if (!c1 && !c2)
                    {
                        bool letter1 = Char.IsLetter(x, i1);
                        bool letter2 = Char.IsLetter(y, i2);
                        if ((letter1 && letter2) || (!letter1 && !letter2))
                        {
                            if (letter1 && letter2)
                            {
                                r = Char.ToLower(x[i1]).CompareTo(Char.ToLower(y[i2]));
                            }
                            else
                            {
                                r = x[i1].CompareTo(y[i2]);
                            }
                            if (r != 0) return r;
                        }
                        else if (!letter1 && letter2) return -1;
                        else if (letter1 && !letter2) return 1;
                    }
                    else if (c1 && c2)
                    {
                        r = CompareNum(x, ref i1, y, ref i2);
                        if (r != 0) return r;
                    }
                    else if (c1)
                    {
                        return -1;
                    }
                    else if (c2)
                    {
                        return 1;
                    }
                    i1++;
                    i2++;
                    if ((i1 >= x.Length) && (i2 >= y.Length))
                    {
                        return 0;
                    }
                    else if (i1 >= x.Length)
                    {
                        return -1;
                    }
                    else if (i2 >= y.Length)
                    {
                        return -1;
                    }
                }
            }

            /// <summary>
            /// Compares the number.
            /// </summary>
            /// <param name="s1">The s1.</param>
            /// <param name="i1">The i1.</param>
            /// <param name="s2">The s2.</param>
            /// <param name="i2">The i2.</param>
            /// <returns>System.Int32.</returns>
            private static int CompareNum(string s1, ref int i1, string s2, ref int i2)
            {
                int nzStart1 = i1, nzStart2 = i2; // nz = non zero
                int end1 = i1, end2 = i2;

                ScanNumEnd(s1, i1, ref end1, ref nzStart1);
                ScanNumEnd(s2, i2, ref end2, ref nzStart2);
                int start1 = i1; i1 = end1 - 1;
                int start2 = i2; i2 = end2 - 1;

                int nzLength1 = end1 - nzStart1;
                int nzLength2 = end2 - nzStart2;

                if (nzLength1 < nzLength2) return -1;
                else if (nzLength1 > nzLength2) return 1;

                for (int j1 = nzStart1, j2 = nzStart2; j1 <= i1; j1++, j2++)
                {
                    int r = s1[j1].CompareTo(s2[j2]);
                    if (r != 0) return r;
                }
                // the nz parts are equal
                int length1 = end1 - start1;
                int length2 = end2 - start2;
                if (length1 == length2) return 0;
                if (length1 > length2) return -1;
                return 1;
            }

            /// <summary>
            /// Scans the number end.
            /// </summary>
            /// <param name="s">The s.</param>
            /// <param name="start">The start.</param>
            /// <param name="end">The end.</param>
            /// <param name="nzStart">The nz start.</param>
            private static void ScanNumEnd(string s, int start, ref int end, ref int nzStart)
            {
                nzStart = start;
                end = start;
                bool countZeros = true;
                while (Char.IsDigit(s, end))
                {
                    if (countZeros && s[end].Equals('0'))
                    {
                        nzStart++;
                    }
                    else countZeros = false;
                    end++;
                    if (end >= s.Length) break;
                }
            }
        }
    }
}
