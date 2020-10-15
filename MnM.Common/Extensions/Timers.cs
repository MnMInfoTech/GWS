/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public static class Timers
    {
        static readonly Dictionary<string, ITimer> timers = new Dictionary<string, ITimer>(4);

        #region RUN TIMERS
        public static void RunTimers()
        {
            if (timers.Count > 0)
            {
                var timers = GetTimers<ITimer>(t => t.Due);
                foreach (var t in timers)
                    t.FireEvent();
            }
        }
        #endregion

        #region REGISTER - DEREGISTER
        public static void Register(this ITimer timer)
        {
            var name = timer.ID + "";
            if (timers.ContainsKey(name))
                return;
            timers.Add(name, timer);
        }
        public static void Deregister(this ITimer timer)
        {
            var name = timer.ID + "";
            timers.Remove(name);
        }
        #endregion

        #region GET TIMER
        public static bool GetTimer<T>(string name, out T timer) where T : ITimer
        {
            timer = default(T);

            if (!timers.TryGetValue(name + "", out ITimer ep))
                return false;

            if (ep is T)
            {
                timer = (T)ep;
                return true;
            }
            return false;
        }
        public static T GetTimer<T>(Predicate<T> condition) where T : ITimer
        {
            return timers.Values.OfType<T>().FirstOrDefault(x => condition(x));
        }
        #endregion

        #region GET TIMERS
        public static IEnumerable<T> GetTimers<T>(Predicate<T> condition) where T : ITimer
        {
            return timers.Values.OfType<T>().Where(x => condition(x));
        }
        #endregion

        #region EXISTS
        public static bool Exists(this ITimer timer) =>
            timers.ContainsKey(timer.ID + "");
        #endregion

        public static void Dispose()
        {
            var keys = timers.Keys.ToArray();
            foreach (var key in keys)
            {
                timers[key].Dispose();
            }
            timers.Clear();
        }
    }
}
