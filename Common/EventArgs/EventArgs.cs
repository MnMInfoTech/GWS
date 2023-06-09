/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    #region IEVENT-ARGS
    /// <summary>
    /// Base interface which represents EventArgs object.
    /// </summary>
    public interface IEventArgs { }
    #endregion

    public class EventArgs
        : System.EventArgs, IEventArgs
    {
        public static new EventArgs Empty = new EventArgs();
    }

    public interface ICancelEventArgs : IEventArgs
    {
        bool Cancel { get; set; }
        string MessageTitle { get; set; }
        string MessageText { get; set; }
    }
    public class CancelEventArgs : EventArgs, ICancelEventArgs
    {
        public static new readonly CancelEventArgs Empty = new CancelEventArgs();
        public bool Cancel { get; set; }
        public string MessageTitle { get; set; }
        public string MessageText { get; set; }
    }

    public interface IHandleEventArgs : IEventArgs
    {
        bool Handled { get; set; }
    }
    public class HandleEventArgs : EventArgs, IHandleEventArgs
    {
        public static new readonly HandleEventArgs Empty = new HandleEventArgs();
        public bool Handled { get; set; }
    }

    public interface IEventArgs<T> : IEventArgs
    {
        T Args { get; }
    }
    internal interface IExEventArgs<T> : IEventArgs<T>
    {
        new T Args { get; set; }
    }

    public interface IEventArgs<T1, T2> : IEventArgs
    {
        T1 Args1 { get; }
        T2 Args2 { get; }
    }

    public class EventArgs<T> : EventArgs, IExEventArgs<T>
    {
        protected internal EventArgs() { }
        public EventArgs(T args)
        {
            Args = args;
        }
        public T Args { get; protected internal set; }
        T IExEventArgs<T>.Args { get => Args; set => Args = value; }
    }
    public class EventArgs<T1, T2> : EventArgs<T1>, IEventArgs<T1, T2>
    {
        protected internal EventArgs() { }
        public EventArgs(T1 args1, T2 args2) :
            base(args1)
        {
            Args2 = args2;
        }

        public T2 Args2 { get; protected internal set; }
        T1 IEventArgs<T1, T2>.Args1 => Args;
    }
    public class EventArgs<T1, T2, T3> : EventArgs<T1, T2>
    {
        protected internal EventArgs() { }
        public EventArgs(T1 args1, T2 args2, T3 args3) :
            base(args1, args2)
        {
            Args3 = args3;
        }
        public T3 Args3 { get; protected internal set; }
    }
}
