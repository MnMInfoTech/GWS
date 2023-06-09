/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Threading.Tasks;

namespace MnM.GWS
{
    #region ICOUNTDOWN
    public interface ICountDown : ISchedule
    {
        event EventHandler<IEventArgs> Stopped;
    }
    #endregion

    #region COUNT-DOWN CLASS
    partial class Factory
    {
        sealed class CountDown : Schedule, ICountDown
        {
            #region VARIABLES
            public event EventHandler<IEventArgs> Stopped;
            #endregion

            #region CONSTRUCTORS
            public CountDown(int threshold, TimeUnit unit = TimeUnit.MilliSecond)
                : base(threshold, unit) 
            { }
            #endregion

            protected override void Invoke()
            {
                if ((status & LoopStatus.IsRunning) != LoopStatus.IsRunning ||
                    Application.ElapsedTicks - startTimeTicks < ticksPerLoop)
                    return;
                Stop();
                Stopped?.Invoke(this, Factory.DefaultArgs);
            }

            #region DISPOSE
            public override void Dispose()
            {
                base.Dispose(); 
                Stopped = null;
            }
            #endregion
        }
    }
    #endregion
}
