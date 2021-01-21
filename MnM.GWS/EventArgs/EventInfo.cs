/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
namespace MnM.GWS
{
    public class EventInfo : IEventInfo
    {
        public static readonly EventInfo Empty = new EventInfo();
        public EventInfo()
        {
        }
        public EventInfo(object sender, IEventArgs args, int type)
        {
            Sender = sender;
            Args = args;
            Type = type;
        }
        public object Sender { get; set; }
        public IEventArgs Args { get; set; }
        public int Type { get; set; }
        public bool Handled { get; set; }
    }
}
