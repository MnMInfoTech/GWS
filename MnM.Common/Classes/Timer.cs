/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Threading.Tasks;

namespace MnM.GWS
{
    public sealed class Timer: _Timer
    {
        public Timer(int interval = 50): 
            base(interval)
        {
            this.Register();
        }

        public override void FireEvent()
        {
            if (Running)
                return;
            if (Due && Enabled)
            {
                Running = true;
                new Task(() => base.FireEvent()).RunSynchronously();
                Running = false;
            }
            Running = false;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.Deregister();
        }
    }
}
