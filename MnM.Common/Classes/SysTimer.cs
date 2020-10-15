/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Threading;


namespace MnM.GWS
{
    /// <summary>
    /// Hight resolution non overlapping, multi thread ok timer - https://stackoverflow.com/a/41697139/548894
    /// </summary>
    /// <remarks>
    public class SysTimer: _Timer
    {
        #region VARIABLES
        /// <summary>
        ///  Execution thread
        /// </summary>
        Thread thread;

        bool JoinThreadOnStopping;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="joinThreadOnStopping">If true joins with current thread on stopping otherwise not.</param>
        public SysTimer(int interval = 50, bool joinThreadOnStopping = true): 
            base(interval)
        {
            JoinThreadOnStopping = joinThreadOnStopping;
        }
        #endregion

        public sealed override void Start()
        {
            if (Running) return;
            base.Start();
            Running = true;
            thread = new Thread(()=>
            {
                while (Running && Enabled)
                {
                    if (Due)
                        FireEvent();
                }
            })
            {
                IsBackground = true,
            };
            thread.Start();
        }
        public sealed override void Stop()
        {
            base.Stop();
            // Even if _thread.Join may take time it is guaranteed that 
            // Elapsed event is never called overlapped with different threads
            if (JoinThreadOnStopping && Thread.CurrentThread != thread)
            {
                thread.Join();
            }
        }
    }
}