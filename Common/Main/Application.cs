/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MnM.GWS
{
    public static partial class Application
    {
        #region VARIABLES
        static Dictionary<string, int> NameIDs;
        internal static int Dpi = 96;

        public const bool Runtime =
#if RunTime
            true;
#else
            false;
#endif

        static volatile LoopStatus status;
        static ushort doubleClickSpeed;
        #endregion

        #region CONSTS
        public const StringComparison NoCase = StringComparison.CurrentCultureIgnoreCase;
        public const string ImplementedInAdvanceVersionOnly =
            "Sorry this is only implemented  the Advance version! For more information - visit www.mnminfotech.co.uk";
        public const byte YES = 1;
        public const byte NO = 0;
        const ushort defaultDblClickSpeed = 300;

        public const int TicksPerMillisecond = (int)TimeSpan.TicksPerMillisecond;
        public const int TicksPerSecond = (int)TimeSpan.TicksPerSecond;
        public const int TicksPerMicroSecond = (int)(TicksPerMillisecond / 1000);
        public const int TicksPerMinute = (int)TimeSpan.TicksPerSecond * 60;
        public const long TicksPerHour = TimeSpan.TicksPerSecond * 3600;
        public const long TicksPerDay = TimeSpan.TicksPerSecond * 3600 * 24;

        const double MillisecondPerTicks = 1d/ TicksPerMillisecond;
        const double SecondPerTicks = 1d / TicksPerSecond;
        const double MicroSecondPerTicks = 1d / TicksPerMicroSecond;
        const double MinutePerTicks = 1d / TicksPerMinute;
        const double HourPerTicks = 1d / TicksPerHour;
        const double DayPerTicks = 1d/ TicksPerDay;
        #endregion

        #region CONSTRUCTORS
        static Application()
        {
            NameIDs = new Dictionary<string, int>(100);
            doubleClickSpeed = defaultDblClickSpeed;
            PsuedoConstructor();
        }
        static partial void PsuedoConstructor();
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Elapsed time in milliseconds.
        /// </summary>
        public static long ElapsedMilliseconds => (long)System.DateTime.Now.TimeOfDay.TotalMilliseconds;
        public static long ElapsedTicks => System.DateTime.Now.Ticks;
        public static int DPI => Dpi;

        /// <summary>
        /// Elapsed time ticks.
        /// </summary>
        public static bool IsRunning => (status & LoopStatus.IsRunning) == LoopStatus.IsRunning;
        public static bool IsSchedulerRunning => (status & LoopStatus.IsBusyRunning) == LoopStatus.IsBusyRunning;

        public static ushort DoubleClikSpeed
        {
            get => doubleClickSpeed;
            set
            {
                if (value == 0)
                    value = defaultDblClickSpeed;
                else if (value < 50)
                    value = 50;
                doubleClickSpeed = value;
            }
        }
        #endregion

        #region COVERT TO TICKS
        public static long ToTicks(this long interval, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.MilliSecond:
                    return interval * TicksPerMillisecond;
                case TimeUnit.Second:
                    return interval * TicksPerSecond;
                case TimeUnit.Minute:
                    return interval * TicksPerMinute;
                case TimeUnit.Hour:
                    return interval * TicksPerHour;
                case TimeUnit.Day:
                    return interval * TicksPerDay;
                case TimeUnit.MicroSecond:
                    return interval * TicksPerMicroSecond;
                default:
                    return interval;
            }
        }
        public static long ToTimeUnit(this long ticks, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.MilliSecond:
                    return (long)(ticks * MillisecondPerTicks);
                case TimeUnit.Second:
                    return (long)(ticks * SecondPerTicks);
                case TimeUnit.Minute:
                    return (long)(ticks * MinutePerTicks);
                case TimeUnit.Hour:
                    return (long)(ticks * HourPerTicks);
                case TimeUnit.Day:
                    return (long)(ticks * DayPerTicks);
                case TimeUnit.MicroSecond:
                    return (long)(ticks * MicroSecondPerTicks);
                default:
                    return ticks;
            }
        }

        #endregion

        #region NEW ID
        /// <summary>
        /// 
        /// </summary>
        /// <param name="renderable"></param>
        /// <returns></returns>
        public static string NewID(string typeName)
        {
            if (NameIDs.ContainsKey(typeName))
            {
                int id = NameIDs[typeName];
                NameIDs[typeName] = ++id;
                return typeName + id;
            }
            NameIDs[typeName] = 1;
            return typeName + 1;
        }
        public static string NewID(string typeName, out int counter)
        {
            if (NameIDs.ContainsKey(typeName))
            {
                counter = NameIDs[typeName];
                NameIDs[typeName] = ++counter;
                return typeName + counter;
            }
            counter = 1;
            NameIDs[typeName] = counter;
            return typeName + counter;
        }
        #endregion

        #region TYPE-NAME FROM ID
        public static string TypeName(this string ID)
        {
            if (string.IsNullOrEmpty(ID))
                return "";

            int i = -1;
            char c;
            do
            {
                c = ID[++i];
            } 
            while (char.IsLetter(c));

            return ID.Substring(0, i);
        }
        #endregion

#if GWS || Window
#endif
        #region QUIT
        public static void Quit()
        {
            status &= ~LoopStatus.IsRunning;
            Factory.factory.Dispose();
#if Window
            foreach (var item in Windows.Values)
                item.Dispose();

            Windows = null;
#elif MS
            System.Windows.Forms.Application.Exit();
#endif
        }
        #endregion
    }
}