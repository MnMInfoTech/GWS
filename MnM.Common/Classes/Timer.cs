/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MnM.GWS
{
    /// <summary>
    /// Represents an object which allowers regualar activitiy on a specific time interval.
    /// </summary>
    public sealed class Timer : ITimer
    {
        #region VARIABLES
        volatile int interval = 5;
        volatile bool Running = false;
        volatile bool Enabled;
        Stopwatch Watch;
        readonly IElpasedTimeEventArgs DefaultElpasedEventArgs = new ElpasedEventArgs(0);
        #endregion

        #region CONSTRUCTORS
        public Timer(int interval = 50)
        {
            Watch = new Stopwatch();
            ID = "Timer".NewID();

            if (interval < 5)
                interval = 5;
            this.interval = interval;
            this.Register();
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
        public bool Due => Enabled && Watch.ElapsedMilliseconds >= interval && Tick != null;
        #endregion

        #region START - RESTART
        public void Start()
        {
            Enabled = true;
            Watch.Restart();
            Running = false;
        }
        public void Restart()
        {
            Reset();
            Start();
        }
        #endregion

        #region STOP-RESET
        public void Stop()
        {
            Enabled = false;
            Watch.Stop();
            Watch.Reset();
            Running = false;
        }
        public void Reset()
        {
            var _enabled = Enabled;
            Enabled = false;
            Watch.Reset();
            Enabled = _enabled;
            Running = false;
        }
        #endregion

        #region FIRE EVENT
        public void FireEvent()
        {
            if (Running || !Enabled || !Due)
                return;
            Running = true;
            DefaultElpasedEventArgs.ElapsedTime = Watch.ElapsedMilliseconds;
            Tick(this, DefaultElpasedEventArgs);
            Watch.Restart();
            Running = false;
        }
        public event EventHandler<IElpasedTimeEventArgs> Tick;
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            Watch = null;
            Tick = null;
            this.Deregister();
        }
        #endregion

        #region EVENTARGS CLASS
        class ElpasedEventArgs : IElpasedTimeEventArgs
        {
            public ElpasedEventArgs()
            {

            }
            public ElpasedEventArgs(long time)
            {
                ElapsedTime = time;
            }
            public long ElapsedTime { get; set; }
        }
        #endregion
    }
}
