/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Collections.Generic;

namespace MnM.GWS
{
    public struct Entry<T> : IConvertible<bool>, IConvertible<int>, IEqualityComparer<Entry<T>>
    {
        int? index;
        T value;
        static Entry<T> blank;

        static Entry()
        {
            blank = new Entry<T>(default(T), null);
        }
        public Entry(T value, int? index = null)
        {
            this.value = value;
            this.index = index;
        }

        public int Index
        {
            get => index ?? -1;
            set => index = value;
        }
        public T Value =>
            value;
        public static Entry<T> Blank =>
            blank;

        public override int GetHashCode()
        {
            if (value == null) return base.GetHashCode();
            else return value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entry<T>)) return false;
            return base.Equals((Entry<T>)obj);
        }

        public static implicit operator Entry<T>(T value) =>
            new Entry<T>(value);

        public static implicit operator bool(Entry<T> value) =>
            !(value.index < 0);

        bool IConvertible<bool>.Convert() =>
            index >= 0;
        int IConvertible<int>.Convert() =>
            Index;
        bool IEqualityComparer<Entry<T>>.Equals(Entry<T> x, Entry<T> y) =>
            x.GetHashCode() == y.GetHashCode();
        int IEqualityComparer<Entry<T>>.GetHashCode(Entry<T> obj)
        {
            if (obj.value == null) return base.GetHashCode();
            return obj.value.GetHashCode();
        }
    }
}
