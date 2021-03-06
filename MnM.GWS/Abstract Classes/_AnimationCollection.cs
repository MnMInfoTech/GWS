/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Diagnostics;

namespace MnM.GWS
{
    public abstract class _AnimationCollection : KeyCollection<uint, IAnimation>, IAnimations
    {
        #region VARIABLES
        protected const int BlinkerLapse = 15;
        protected readonly Stopwatch Watch = new Stopwatch();
        protected volatile bool Running;
        protected int BlinkerInterval = BlinkerLapse;
        protected readonly IBoundary[] Boundaries = new IBoundary[256];
        protected int interval;
        protected long elapsedTime;
        protected readonly EventArgs<long> AnimationArgs = new EventArgs<long>();
        #endregion

        #region CONSTRUCTORS
        protected _AnimationCollection()
        {
            Boundaries[ObjType.Blinker] = new Boundary(ObjType.Blinker);
            Boundaries[ObjType.Animation] = new Boundary(ObjType.Animation);
        }
        protected _AnimationCollection(int capacity):
            this()
        {
            Capacity = capacity;
        }
        #endregion

        #region PROPERTIES
        public int Interval
        {
            get => interval;
            set
            {
                interval = value;
                BlinkerInterval = Math.Max(interval, BlinkerLapse);
            }
        }
        public int RefreshInterval => BlinkerInterval;
        public bool IsRunning => Running;
        public long ElapsedTime => elapsedTime;
        public long CurrentWatchTime => Watch.ElapsedMilliseconds;
        public abstract IAnimationHost Host { get; }
        #endregion

        #region SWITCH
        public void Run(bool on)
        {
            Running = on;
            if (Running)
                Watch.Restart();
            else
                Watch.Reset();
            if (Running)
                Run();
        }
        #endregion
         
        #region RUN
        protected abstract void Run();
        #endregion

        #region GETKEY
        protected sealed override uint Key(IAnimation item) => item.ID;
        #endregion

        #region EVENTS
        protected virtual void OnAnimationLoopCompleted(IEventArgs<long> e) =>
            AnimationLoopComplete?.Invoke(this, e);
        public event EventHandler<IEventArgs<long>> AnimationLoopComplete;
        #endregion
    }
}
