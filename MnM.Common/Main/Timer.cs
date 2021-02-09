/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Threading.Tasks;

namespace MnM.GWS
{
    public sealed class Timer : _Timer, ITimer
    {
        #region VARIABLES
        public event EventHandler<IEventArgs> Tick;
        Unit unit;
        bool ON;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        public Timer(int interval = 50) :
            base(interval)
        { }
        #endregion

        #region PROPERTIES
        public Unit Unit
        {
            get => unit;
            set => unit = value;
        }
        #endregion

        #region FIRE EVENT
        protected override async void FireEvent()
        {
            if (Running)
                return;
            Running = true;
            await Task.Factory.StartNew(() =>
            {
                while (Running && Tick != null)
                {
                    if (Watch.ElapsedMilliseconds >= interval && !ON)
                    {
                        ON = true;
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
                        ON = false;
                    }
                }
            });
            Running = false;
        }
      
        #endregion

        #region DISPOSE
        public override void Dispose()
        {
            base.Dispose();
            Tick = null;
        }
        #endregion

        #region TO STRING
        public override string ToString()
        {
            return "{0} takes " + Speed + " " + Unit.ToString() + "s.";
        }
        #endregion
    }
}