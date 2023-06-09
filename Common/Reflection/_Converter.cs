/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

// Author: Mukesh Adhvaryu.
using System;

namespace MnM.GWS
{
    public abstract class _Converter : IConverter
    {
        public bool ConvertTo<T>(string expression, out T result)
        {
            Type t = typeof(T);
            return ProcessUnknown(t, expression, out result);
        }
        protected abstract bool ProcessUnknown<T>(Type t, string expression, out T result);
        public abstract bool ConvertTo<T>(object value, out T result);
    }
}
