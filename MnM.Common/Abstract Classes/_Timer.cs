/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Diagnostics;

namespace MnM.GWS
{
    /// <summary>
    /// Represents an object which allowers regualar activitiy on a specific time interval.
    /// </summary>
    public abstract class _Timer : ITimer
    {
        #region VARIABLES
        volatile int interval = 5;
        protected volatile bool Running = false;
        protected volatile bool Enabled;
        protected Stopwatch Watch;
        protected readonly IElpasedTimeEventArgs DefaultElpasedEventArgs = new ElpasedEventArgs(0);
        #endregion

        #region CONSTRUCTORS
        protected _Timer(int interval = 50)
        {
            Watch = new Stopwatch();
            ID = "Timer".NewID();

            if (interval < 5)
                interval = 5;
            this.interval = interval;
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
        public virtual void Start()
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
        public virtual void Stop()
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
        public virtual void FireEvent()
        {
            DefaultElpasedEventArgs.ElapsedTime = Watch.ElapsedMilliseconds;
            Tick(this, DefaultElpasedEventArgs);
            Watch.Restart();
        }
        public event EventHandler<IElpasedTimeEventArgs> Tick;
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            Watch = null;
            Tick = null;
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
