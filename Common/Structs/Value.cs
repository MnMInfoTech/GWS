/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public struct Value<T>
    {
        public Value(T val)
        {
            Current = Old = val;
        }
        public Value(T val, T old)
        {
            Current = val;
            Old = old;
        }

        public T Current { get; private set; }
        public T Old { get; }

        public static implicit operator T(Value<T> value) => value.Current;
        public static implicit operator Value<T>(T value) => new Value<T>(value);

        public void SetValue(T newValue, bool temporary)
        {
            if (temporary)
                this = new Value<T>(newValue, Current);

            else
                this = new Value<T>(newValue);
        }
        public void Restore()
        {
            this = new Value<T>(Old);
        }
    }

    public class ValueClass<T>
    {
        public ValueClass(T val)
        {
            Current = Old = val;
        }
        public ValueClass(T val, T old)
        {
            Current = val;
            Old = old;
        }

        public T Current { get; private set; }
        public T Old { get; private set; }

        public static implicit operator T(ValueClass<T> value) => value.Current;
        public static implicit operator ValueClass<T>(T value) => new ValueClass<T>(value);

        public void SetValue(T newValue, bool temporary)
        {
            if (!temporary)
                Old = newValue;
            Current = newValue;
        }
        public void Restore()
        {
            Current = Old;
        }
    }
}
