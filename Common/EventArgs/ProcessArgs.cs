/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    #region IPROCESS
    public unsafe interface IProcessArgs : IAlpha
    {
        /// <summary>
        /// Gets X co-ordinate of start point where rendering begins at. 
        /// </summary>
        int DstX { get; }

        /// <summary>
        /// Gets Y co-ordinate of start point where rendering begins at. 
        /// </summary>
        int DstY { get; }

        /// <summary>
        /// Gets direction of rendering. If true horizonatl otherwise vertical.
        /// </summary>
        bool Horizontal { get; }
    }
    #endregion

    #region PROCESS
    abstract class ProcessArgs: IProcessArgs
    {
        internal int DstX, DstY;
        internal bool Horizontal;
        internal byte Alpha;

        int IProcessArgs.DstX => DstX;
        int IProcessArgs.DstY => DstY;
        bool IProcessArgs.Horizontal => Horizontal;
        byte IAlpha.Alpha => Alpha;
    }
    #endregion
}
