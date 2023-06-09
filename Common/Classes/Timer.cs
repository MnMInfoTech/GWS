/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;

namespace MnM.GWS
{
    #region ITIMER
    /// <summary>
    /// Represents an object which allowers regualar activitiy on a specific time interval.
    /// </summary>
    public interface ITimer : ISchedule
    {
        /// <summary>
        /// Tick event which gets invoked by the interval specified.
        /// </summary>
        event EventHandler<IEventArgs<long>> Tick;
    }
    #endregion

    partial class Factory
    {
        sealed class Timer : Schedule, ITimer
        {
            #region VARIABLES
            public event EventHandler<IEventArgs<long>> Tick;
            readonly EventArgs<long> e = new EventArgs<long>();
            #endregion

            #region CONSTRUCTORS
            /// <summary>
            /// 
            /// </summary>
            /// <param name="interval"></param>
            public Timer(int interval, TimeUnit unit = TimeUnit.MilliSecond) :
                base(interval, unit)
            { }
            #endregion

            #region PROCESS
            protected override void Invoke()
            {
                if ((status & LoopStatus.IsBusy) == LoopStatus.IsBusy || Tick == null)
                    return;

                var currentTime = Application.ElapsedTicks;
                if (currentTime - startTimeTicks >= ticksPerLoop)
                {
                    status |= LoopStatus.IsBusy;
                    e.Args = currentTime - startTimeTicks;
                    Tick(this, e);
                    startTimeTicks = Application.ElapsedTicks;
                    status &= ~LoopStatus.IsBusy;
                }
            }
            #endregion

            #region DISPOSE
            public override void Dispose()
            {
                base.Dispose();
                Tick = null;
            }
            #endregion
        }
    }
}