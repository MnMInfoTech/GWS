/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Diagnostics;

namespace MnM.GWS
{
    public static class Benchmarks
    {
        #region EXECUTE
        public static string Execute(VoidMethod method, out long executionTime, string description = "Method Execution", TimeUnit unit = 0)
        {
            executionTime = 0;
            if (method == null)
                return "No method present";
            long time = Application.ElapsedTicks.ToTimeUnit(unit);
            method();
            executionTime = Application.ElapsedTicks.ToTimeUnit(unit); 
            executionTime -= time;
            return description + "  takes: " + executionTime + " " + unit.ToString() + "s";
        }
        public static string Execute(VoidMethod method, string description = "Method Execution", TimeUnit unit = 0) =>
            Execute(method, out long i, description, unit);
        public static string Execute<T>(ReturnMethod<T> method, out long executionTime, out T value, string description = "Method Execution", TimeUnit unit = 0)
        {
            executionTime = 0;
            value = default(T);
            if (method == null)
                return "No method present";
            long time = Application.ElapsedTicks.ToTimeUnit(unit);
            value  = method();
            executionTime = Application.ElapsedTicks.ToTimeUnit(unit);
            executionTime -= time;
            return description + " takes: " + executionTime + " " + unit.ToString();
        }
        public static T Execute<T>(ReturnMethod<T> method, out long executionTime, out string message, string description = "Method Execution", TimeUnit unit = 0)
        {
            message = Execute(method, out executionTime, out T value, description, unit);
            return value;
        }
        public static T Execute<T>(ReturnMethod<T> method, out string message, string description = "Method Execution", TimeUnit unit = 0)
        {
            message = Execute(method, out _, out T value, description, unit);
            return value;
        }
        #endregion
    }
}
