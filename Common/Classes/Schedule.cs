/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Threading.Tasks;

namespace MnM.GWS
{
    #region ISCHEDULER
    public interface ISchedule: ILoop, IDisposable
    {
        /// <summary>
        /// Gets or sets interval by which the activity should be repeated.
        /// </summary>
        new long Interval { get; set; }

        /// <summary>
        /// Gets or sets unit of time measurement i.e milliseconds or seconds or microseconds.
        /// </summary>
        TimeUnit Unit { get; }

        /// <summary>
        /// Gets last recorded time in repated process cycle.
        /// </summary>
        long StartTime { get; }

        /// <summary>
        /// Removes this object from scheduler list and thus making it in-active.
        /// </summary>
        void Detach();
    }
    #endregion

    public abstract class Schedule : ISchedule
    {
        #region VARIABLES

        protected volatile LoopStatus status;
        long interval;
        protected long startTimeTicks, ticksPerLoop;
        TimeUnit unit;
        static IPrimitiveList<Schedule> Schedules = new SchedulerList();
        static LoopStatus State;
        #endregion

        #region CONSTRUCTOR
        protected Schedule(long interval = 5, TimeUnit unit = TimeUnit.MilliSecond)
        {
            this.unit = unit;
            this.interval = interval;
            ticksPerLoop = this.interval.ToTicks(unit);
        }
        #endregion

        #region PROPERTIES
        public long Interval
        {
            get => interval;
            set
            {
                interval = value;
                ticksPerLoop = this.interval.ToTicks(unit);
            }
        }
        public TimeUnit Unit => unit;
        public LoopStatus Status => status;
        public long StartTime => startTimeTicks;
        #endregion

        #region START-STOP
        public bool Start()
        {
            if ((status & LoopStatus.IsQueued) != LoopStatus.IsQueued)
            {
                Schedules.Add(this);
                status |= LoopStatus.IsQueued;
            }

            if ((status & LoopStatus.IsRunning) != LoopStatus.IsRunning)
            {
                status |= LoopStatus.IsRunning;
                startTimeTicks = Application.ElapsedTicks;
            }
            RunScheduler();
            return (status & LoopStatus.IsRunning) == LoopStatus.IsRunning;
        }
        public bool Stop()
        {
            if ((status & LoopStatus.IsRunning) != LoopStatus.IsRunning)
                return false;
            startTimeTicks = 0;
            status &= ~LoopStatus.IsRunning;
            return true;
        }
        #endregion

        #region DE-QUEUE
        public void Detach()
        {
            Schedules.Remove(this);
            status &= ~LoopStatus.IsQueued;
            if (Schedules.Count == 0)
                State &= ~LoopStatus.IsRunning;
        }
        #endregion

        #region UPDATE
        protected abstract void Invoke();
        #endregion

        #region RUN SCHEDULER
        static void RunScheduler()
        {
            if ((State & LoopStatus.IsRunning) == LoopStatus.IsRunning)
                return;
            State |= LoopStatus.IsRunning;
            Task.Run(() =>
            {
                while ((State & LoopStatus.IsRunning) == LoopStatus.IsRunning)
                {
                    if ((State & LoopStatus.IsSkipping) == LoopStatus.IsSkipping)
                        continue;
                    for (int i = 0; i < Schedules.Count; i++)
                    {
                        var item = Schedules[i];
                        if (item == null)
                            continue;
                        if ((item.status & LoopStatus.IsRunning) == LoopStatus.IsRunning)
                            item.Invoke();
                    }
                }
            });
        }
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            Schedules.Remove(this);
            Stop();
        }
        #endregion

        class SchedulerList : PrimitiveList<Schedule>
        {
            public sealed override void Add(Schedule item)
            {
                State |= LoopStatus.IsSkipping;
                base.Add(item);
                State &= ~LoopStatus.IsSkipping;
            }
            public sealed override bool RemoveAt(int index)
            {
                State |= LoopStatus.IsSkipping;
                base.RemoveAt(index);
                State &= ~LoopStatus.IsSkipping;
                return true;
            }
        }
    }
}
