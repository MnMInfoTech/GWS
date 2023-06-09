/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    /// <summary>
    /// Represents an object whis has an axis and a position (value) lying on that axis.
    /// </summary>
    public interface IAxisPoint : IAxis, IScanPoint
    {
        /// <summary>
        /// Gets value at a given axis.
        /// </summary>
        float Val { get; }
    }

    public struct AxisPoint: IAxisPoint
    {
        #region VARIABLES
        public readonly float Val;
        public readonly int Axis;
        public readonly LineFill Draw;
        const string toStr = "Val: {0}, Axis: {1}, Draw: {2}";
        #endregion

        #region CONSTRUCTOR
        public AxisPoint(float val, int axis, LineFill draw)
        {
            Val = val;
            Axis = axis;
            Draw = draw;
        }
        #endregion

        #region PROPERTIES
        public bool IsHorizontal => (Draw & LineFill.Vertical) != LineFill.Vertical;
        float IAxisPoint.Val => Val;
        int IAxis.Axis => Axis;
        LineFill IAxis.Draw => Draw;
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, Val, Axis, Draw);
        }
    }
}
