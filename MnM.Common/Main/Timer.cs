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
        volatile uint speed;
        Stopwatch Watch;
        public event EventHandler<IEventArgs> Tick;
        Unit unit;
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
            get => unit;
            set => unit = value;
        }
        public bool IsRunning => Running;
        public uint Speed => speed;
        #endregion

        #region START - STOP
        public void Start()
        {
            if (Running)
                return;
            speed = 0;
            Watch.Restart();
            Running = true;
            FireEvent(); 
        }
        public void Stop()
        {
            Watch.Stop();
            Watch.Reset();
            Running = false;
            speed = 0;
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
                        speed = 0;

                        switch (unit)
                        {
                            case Unit.MilliSecond:
                            default:
                                speed = (uint)Watch.ElapsedMilliseconds;
                                break;
                            case Unit.Tick:
                                speed = (uint)Watch.ElapsedTicks;
                                break;
                            case Unit.Second:
                                speed = (uint)(Watch.ElapsedMilliseconds / 1000);
                                break;
                        }
                        Watch.Restart();
                        Tick(this, Factory.EmptyArgs);
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

        #region TO STRING
        public override string ToString()
        {
            return "{0} takes " + speed + " " + Unit.ToString() + "s.";
        }
        #endregion
    }
}