/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MnM.GWS
{
    public sealed class Timer : ITimer
    {
        #region VARIABLES
        volatile int interval = 5;
        volatile bool Running = false;
        volatile bool TickOn = false;
        volatile uint lastReading;
        Stopwatch Watch;
        readonly ElpasedEventArgs DefaultElpasedEventArgs = new ElpasedEventArgs(0);
        public event EventHandler<IElpasedTimeEventArgs> Tick;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public Timer(int interval = 50) 
        {
            this.interval = interval;
            Watch = new Stopwatch();
        }
        #endregion

        #region PROPERTIES
        public string ID { get; private set; }
        public int Interval
        {
            get => interval;
            set
            {
                if (interval < 5)
                    return;
                interval = value;
            }
        }
        public Unit Unit
        {
            get => DefaultElpasedEventArgs.Unit;
            set => DefaultElpasedEventArgs.Unit = value;
        }
        public bool IsRunning => Running;
        public uint LastReading => lastReading;
        #endregion

        #region START - STOP
        public void Start()
        {
            if (Running)
                return;
            lastReading = 0;
            Watch.Restart();
            Running = true;
            FireEvent(); 
        }
        public void Stop()
        {
            Watch.Stop();
            Watch.Reset();
            Running = false;
            lastReading = 0;
        }
        #endregion

        #region FIRE EVENT
        async void FireEvent()
        {
            await Task.Factory.StartNew(() =>
            {
                while (Running && Tick != null)
                {
                    if (Watch.ElapsedMilliseconds >= interval && !TickOn)
                    {
                        TickOn = true;
                        lastReading = 0;

                        switch (DefaultElpasedEventArgs.Unit)
                        {
                            case Unit.MilliSecond:
                            default:
                                lastReading = (uint)Watch.ElapsedMilliseconds;
                                break;
                            case Unit.Tick:
                                lastReading = (uint)Watch.ElapsedTicks;
                                break;
                            case Unit.Second:
                                lastReading = (uint)(Watch.ElapsedMilliseconds / 1000);
                                break;
                        }
                        Watch.Restart();
                        DefaultElpasedEventArgs.ElapsedTime = lastReading;
                        Tick(this, DefaultElpasedEventArgs);
                        TickOn = false;
                    }
                }
            });           
        }
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            Watch = null;
            Tick = null;
        }
        #endregion

        #region EVENTARGS CLASS
        class ElpasedEventArgs : IElpasedTimeEventArgs
        {
            public ElpasedEventArgs(uint time, Unit unit = Unit.MilliSecond)
            {
                ElapsedTime = time;
                Unit = unit;
            }
            public uint ElapsedTime { get; internal set; }
            public Unit Unit { get; internal set; }
        }
        #endregion

        #region TO STRING
        public override string ToString()
        {
            return "{0} takes " + lastReading + " " + Unit.ToString() + "s.";
        }
        #endregion
    }
}