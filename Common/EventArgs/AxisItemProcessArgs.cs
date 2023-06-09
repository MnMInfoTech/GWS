/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    /// <summary>
    /// Reprents an object which provides in line information of each point
    /// being traverse while in scane point processing loop.
    /// </summary>
    public unsafe interface IAxisPointProcessArgs : IProcessArgs
    {
        /// <summary>
        /// Gets start point where pen reading begins at. 
        /// </summary>
        int PenX { get; }

        /// <summary>
        /// Gets Y start axis point where pen reading begins at. 
        /// </summary>
        int PenY { get; }
    }

    /// <summary>
    /// Reprents an object which provides in line information of each line  
    /// being traverse while in scane line processing loop.
    /// </summary>
    public unsafe interface IAxisLineProcessArgs : IProcessArgs
    {
        /// <summary>
        /// Gets start point where pen reading begins at. 
        /// </summary>
        int PenStart { get; }

        /// <summary>
        /// Gets Y start axis point where pen reading begins at. 
        /// </summary>
        int PenAxis { get; }

        /// <summary>
        /// Gets linear length up to which rendering should expand.
        /// </summary>
        int CopyLength { get; }
    }

    unsafe class AxisPointProcessArgs : ProcessArgs, IAxisPointProcessArgs
    {
        internal int PenX;
        internal int PenY;
        int IAxisPointProcessArgs.PenX { get => PenX; }
        int IAxisPointProcessArgs.PenY { get => PenY; }
    }

    unsafe class AxisLineProcessArgs : ProcessArgs, IAxisLineProcessArgs
    {
        internal int PenStart;
        internal int PenAxis;
        public int copyLength;

        int IAxisLineProcessArgs.PenStart { get => PenStart; }
        int IAxisLineProcessArgs.PenAxis { get => PenAxis; }
        int IAxisLineProcessArgs.CopyLength { get => copyLength; }
    }
}
