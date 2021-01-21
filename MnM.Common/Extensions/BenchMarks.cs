/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System.Diagnostics;

namespace MnM.GWS
{
    public static class Benchmarks
    {
        #region BENCHMARKING
        static readonly Stopwatch Watch = new Stopwatch();
        #endregion

        #region EXECUTE
        public static string Execute(VoidMethod method, out long executionTime, string description = "Method Execution", Unit unit = 0)
        {
            executionTime = 0;
            if (method == null)
                return "No method present";
            Watch.Reset();
            Watch.Start();
            method();
            Watch.Stop();
            switch (unit)
            {
                case Unit.MilliSecond:
                default:
                    executionTime = Watch.ElapsedMilliseconds;
                    break;
                case Unit.MicroSecond:
                    executionTime = Watch.ElapsedMilliseconds * 1000;
                    break;
                case Unit.Tick:
                    executionTime = Watch.ElapsedTicks;
                    break;
                case Unit.Second:
                    executionTime = Watch.ElapsedMilliseconds / 1000;
                    break;
            }
            return description + "  takes: " + executionTime + " " + unit.ToString() + "s";
        }
        public static string Execute(VoidMethod method, string description = "Method Execution", Unit unit = 0) =>
            Execute(method, out long i, description, unit);
        public static string Execute<T>(ReturnMethod<T> method, out long executionTime, out T value, string description = "Method Execution", Unit unit = 0)
        {
            Watch.Reset();
            Watch.Start();
            value = method();
            Watch.Stop();
            switch (unit)
            {
                case Unit.MilliSecond:
                default:
                    executionTime = Watch.ElapsedMilliseconds;
                    break;
                case Unit.Tick:
                    executionTime = Watch.ElapsedTicks;
                    break;
                case Unit.Second:
                    executionTime = Watch.ElapsedMilliseconds / 1000;
                    break;
            }
            return description + " takes: " + executionTime + " " + unit.ToString();
        }
        public static T Execute<T>(ReturnMethod<T> method, out long executionTime, out string message, string description = "Method Execution", Unit unit = 0)
        {
            Watch.Reset();
            Watch.Start();
            T value = method();
            Watch.Stop();
            switch (unit)
            {
                case Unit.MilliSecond:
                default:
                    executionTime = Watch.ElapsedMilliseconds;
                    break;
                case Unit.Tick:
                    executionTime = Watch.ElapsedTicks;
                    break;
                case Unit.Second:
                    executionTime = Watch.ElapsedMilliseconds / 1000;
                    break;
            }
            message = description + " takes: " + executionTime + " " + unit.ToString();
            return value;
        }
        public static T Execute<T>(ReturnMethod<T> method, out string message, string description = "Method Execution", Unit unit = 0)
        {
            long executionTime = 0;
            Watch.Reset();
            Watch.Start();
            T value = method();
            Watch.Stop();
            switch (unit)
            {
                case Unit.MilliSecond:
                default:
                    executionTime = Watch.ElapsedMilliseconds;
                    break;
                case Unit.Tick:
                    executionTime = Watch.ElapsedTicks;
                    break;
                case Unit.Second:
                    executionTime = Watch.ElapsedMilliseconds / 1000;
                    break;
            }
            message = description + " takes: " + executionTime + " " + unit.ToString();
            return value;
        }
        #endregion

        #region EXECUTE
        public static string Execute(VoidMethod method, Stopwatch Watch, out long executionTime, string description = "Method Execution", Unit unit = 0)
        {
            executionTime = 0;
            if (method == null)
                return "No method present";
            Watch.Reset();
            Watch.Start();
            method();
            Watch.Stop();
            switch (unit)
            {
                case Unit.MilliSecond:
                default:
                    executionTime = Watch.ElapsedMilliseconds;
                    break;
                case Unit.MicroSecond:
                    executionTime = Watch.ElapsedMilliseconds * 1000;
                    break;
                case Unit.Tick:
                    executionTime = Watch.ElapsedTicks;
                    break;
                case Unit.Second:
                    executionTime = Watch.ElapsedMilliseconds / 1000;
                    break;
            }
            return description + "  takes: " + executionTime + " " + unit.ToString() + "s";
        }
        public static string Execute(VoidMethod method, Stopwatch Watch, string description = "Method Execution", Unit unit = 0) =>
            Execute(method, out long i, description, unit);
        public static string Execute<T>(ReturnMethod<T> method, Stopwatch Watch, out long executionTime, out T value, string description = "Method Execution", Unit unit = 0)
        {
            Watch.Reset();
            Watch.Start();
            value = method();
            Watch.Stop();
            switch (unit)
            {
                case Unit.MilliSecond:
                default:
                    executionTime = Watch.ElapsedMilliseconds;
                    break;
                case Unit.Tick:
                    executionTime = Watch.ElapsedTicks;
                    break;
                case Unit.Second:
                    executionTime = Watch.ElapsedMilliseconds / 1000;
                    break;
            }
            return description + " takes: " + executionTime + " " + unit.ToString();
        }
        public static T Execute<T>(ReturnMethod<T> method, Stopwatch Watch, out long executionTime, out string message, string description = "Method Execution", Unit unit = 0)
        {
            Watch.Reset();
            Watch.Start();
            T value = method();
            Watch.Stop();
            switch (unit)
            {
                case Unit.MilliSecond:
                default:
                    executionTime = Watch.ElapsedMilliseconds;
                    break;
                case Unit.Tick:
                    executionTime = Watch.ElapsedTicks;
                    break;
                case Unit.Second:
                    executionTime = Watch.ElapsedMilliseconds / 1000;
                    break;
            }
            message = description + " takes: " + executionTime + " " + unit.ToString();
            return value;
        }
        public static T Execute<T>(ReturnMethod<T> method, Stopwatch Watch, out string message, string description = "Method Execution", Unit unit = 0)
        {
            long executionTime = 0;
            Watch.Reset();
            Watch.Start();
            T value = method();
            Watch.Stop();
            switch (unit)
            {
                case Unit.MilliSecond:
                default:
                    executionTime = Watch.ElapsedMilliseconds;
                    break;
                case Unit.Tick:
                    executionTime = Watch.ElapsedTicks;
                    break;
                case Unit.Second:
                    executionTime = Watch.ElapsedMilliseconds / 1000;
                    break;
            }
            message = description + " takes: " + executionTime + " " + unit.ToString();
            return value;
        }
        #endregion
    }
}
