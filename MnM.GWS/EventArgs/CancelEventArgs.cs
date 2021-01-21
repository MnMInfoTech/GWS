/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;

namespace MnM.GWS
{
    public class CancelEventArgs : EventArgs, ICancelEventArgs
    {
        public static new readonly CancelEventArgs Empty = new CancelEventArgs();
        public bool Cancel { get; set; }
    }

    public class CancelEventArgs<T> : CancelEventArgs
    {
        protected internal CancelEventArgs() { }
        public CancelEventArgs(T args)
        {
            Args = args;
        }
        public T Args { get; protected internal set; }
    }
    public class CancelEventArgs<T1, T2> : CancelEventArgs<T1>
    {
        protected internal CancelEventArgs() { }
        public CancelEventArgs(T1 args1, T2 args2) :
            base(args1)
        {
            Args2 = args2;
        }

        public T2 Args2 { get; protected internal set; }
    }
    public class CancelEventArgs<T1, T2, T3> : CancelEventArgs<T1, T2>
    {
        protected internal CancelEventArgs() { }
        public CancelEventArgs(T1 args1, T2 args2, T3 args3) :
            base(args1, args2)
        {
            Args3 = args3;
        }
        public T3 Args3 { get; protected internal set; }
    }

}
