using System.Diagnostics;

namespace MnM.GWS
{
    public abstract class _Timer: ITimerBase
    {
        #region VARIABLES
        protected volatile int interval = 5;
        protected volatile bool Running = false;
        protected readonly Stopwatch Watch;
        protected long speed;
        protected long elapsedMilliseconds;
        #endregion

        #region CONSTRUCTOR
        protected _Timer(int interval = 50)
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
                if (interval < 1)
                    return;
                interval = value;
            }
        }
        public bool IsRunning => Running;
        public long Speed => speed;
        public long ElapsedTime => elapsedMilliseconds;
        #endregion

        #region START - STOP
        public void Switch(bool on)
        {
            speed = 0;
            elapsedMilliseconds = 0;
            Watch.Reset();

            if (on)
            {
                Watch.Restart();
                Process();
            }
            else
            {
                Watch.Stop();
                Running = false;
            }
        }
        #endregion

        #region FIRE EVENT
        protected abstract void Process();
        #endregion

        #region DISPOSE
        public virtual void Dispose() { }
        #endregion
    }
}
