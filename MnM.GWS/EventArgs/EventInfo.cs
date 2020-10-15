/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
namespace MnM.GWS
{
    public class EventInfo : IEventInfo
    {
        public static readonly EventInfo Empty = new EventInfo();
        internal EventInfo()
        {
            Status = EventUseStatus.Unused;
        }
        public EventInfo(object sender, IEventArgs args, GwsEvent type)
        {
            Sender = sender;
            Args = args;
            Type = type;
            Status = EventUseStatus.Unused;
        }
        public object Sender { get; set; }
        public IEventArgs Args { get; set; }
        public GwsEvent Type { get; set; }
        public EventUseStatus Status { get; set; }
    }
}
