/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

#if(GWS || Window)
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IEVENT INFO
    public interface IEventInfo
    {
        object Sender { get; }
        bool Handled { get; set; }
        IEventArgs Args { get; }
        GwsEvent Type { get; }
    }

    internal interface IExEventInfo : IEventInfo 
    {
        new object Sender { get; set; }
        new IEventArgs Args { get; set; }
        new GwsEvent Type { get; set; }

        void Reset(object sender, IEventArgs args, GwsEvent type);
        void Reset(object sender, IEventArgs args);
        void Reset(IEventArgs args);
        void Reset(GwsEvent type, IEventArgs args);
        void Reset(GwsEvent type);
    }
    #endregion

#if DevSupport
    public
#else
    internal
#endif
    sealed class EventInfo : IExEventInfo
    {
        #region VARIABLES
        internal static readonly EventInfo Empty = new EventInfo();
        object sender;
        IEventArgs args;
        GwsEvent type;
        bool handled;
        #endregion

        #region CONSTRUCTORS
        public EventInfo() { }
        public EventInfo(object sender, IEventArgs args, GwsEvent type)
        {
            this.sender = sender;
            this.args = args;
            this.type = type;
        }
        #endregion

        #region PROPERTIES
        public object Sender => sender;
        public bool Handled { get => handled; set => handled = value; }
        public IEventArgs Args => args;
        public GwsEvent Type => type;

        object IExEventInfo.Sender { get => sender; set => sender = value; }
        IEventArgs IExEventInfo.Args { get => args; set => args = value; }
        GwsEvent IExEventInfo.Type { get => type; set => type = value; }
        #endregion

        #region RESET
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IExEventInfo.Reset(object sender, IEventArgs args, GwsEvent type)
        {
            this.sender = sender;
            ((IExEventInfo)this).Reset(type, args);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IExEventInfo.Reset(object sender, IEventArgs args)
        {
            this.sender = sender;
            ((IExEventInfo)this).Reset(type, args);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IExEventInfo.Reset(IEventArgs args)
        {
            this.args = args;
            ((IExEventInfo)this).Reset(type);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IExEventInfo.Reset(GwsEvent type, IEventArgs args)
        {
            this.args = args;
            ((IExEventInfo)this).Reset(type);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        void IExEventInfo.Reset(GwsEvent type)
        {
            this.type = type;
            handled = false;
            if (args is ICancelEventArgs)
                ((ICancelEventArgs)args).Cancel = false;
        }
        #endregion
    }
}
#endif
