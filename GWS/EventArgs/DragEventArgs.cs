/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if(GWS || Window)
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public partial interface IDragEventArgs : ICancelEventArgs, IPoint 
    {
        /// <summary>
        /// Gets data which is being dragged.
        /// </summary>
        object Data { get; set; }

        /// <summary>
        /// Clears all data in this argument.
        /// </summary>
        void Clear();
    }

    internal partial interface IExDragEventArgs : IDragEventArgs
    {
        new int X { get; set; }
        new int Y { get; set; }
    }
#if DevSupport
    public
#else
    internal
#endif
    sealed partial class DragEventArgs : CancelEventArgs, IExDragEventArgs
    {
        public DragEventArgs()
        {
             
        }
        public DragEventArgs(IDragEventArgs e)
        {
            if (e == null)
                return;
            X = e.X;
            Y = e.Y;
            Data = e.Data;
            CopyFrom(e);
        }
        public int X { get; private set; }
        public int Y { get; private set; }
        public object Data { get; set; }
        int IExDragEventArgs.X { get => X; set => X = value; }
        int IExDragEventArgs.Y { get => Y; set => Y = value; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            X = Y = 0;
            Data = null;
            MessageText = null;
            MessageTitle = null;
            Cancel = false;
            Clear2();
        }
        partial void Clear2();
        partial void CopyFrom(IDragEventArgs e);
    }
}
#endif