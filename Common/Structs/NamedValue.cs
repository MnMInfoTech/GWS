/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public struct NamedValue<T>
    {
        public readonly string Name;
        public readonly T Value;
        static int uid;

        public NamedValue(string name, T value)
        {
            Name = name;
            Value = value;
        }
        public NamedValue(T value)
        {
            Name = "Item" + (++uid);
            Value = value;
        }
        public static explicit operator T(NamedValue<T> item) =>
            item.Value;

        public override string ToString()
        {
            return Name + " : " + Value;
        }
    }

    public static class NamedValue
    {
        public static NamedValue<T> Create<T>(string name, T value) =>
            new NamedValue<T>(name, value);
        public static NamedValue<T> Create<T>(T value) =>
        new NamedValue<T>(value);
    }
}
