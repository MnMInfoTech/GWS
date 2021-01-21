/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;

namespace MnM.GWS
{
    /// <summary>
    /// Reprents an AxisLine structure which can also have a lenght to stretch.
    /// </summary>
    public struct AxisLine
    {
        #region VARIABLES
        /// <summary>
        /// Axial value : if IsHorizontal is trule then X otherwise Y.
        /// </summary>
        public readonly int Val;

        /// <summary>
        /// Axis value : if IsHorizontal is trule then Y otherwise X.
        /// </summary>
        public readonly int Axis;

        /// <summary>
        /// Specifies the direction of axis point stretch whether it is horizontal or not.
        /// </summary>
        public readonly bool Horizontal;

        /// <summary>
        /// <summary>
        /// Length of stretch for this point.
        /// </summary>
        /// </summary>
        public readonly int Stretch;

        /// <summary>
        /// Alpha value to antialias entire point up to the stretch specified.
        /// </summary>
        public readonly float? Alpha;

        byte valid;
        #endregion

        const string tostr = "Val1: {0}, Val2: {1}, Axis: {2}";

        #region CONSTRUCTORS
        public AxisLine(float val, int axis, bool isHorizontal, int stretch = 0) : this()
        {
            Val = (int)val;
            Axis = axis;
            Horizontal = isHorizontal;
            Alpha = val-(int)val;
            if (Alpha == 0)
                Alpha = null;
            Stretch = stretch;
            if (float.IsNaN(val) || axis == int.MinValue)
                valid = 0;
            else
                valid = 1;
        }
        public AxisLine(int val1, int val2, int axis, bool isHorizontal, float? alpha = null) : this()
        {
            Val = val1;
            Axis = axis;
            Horizontal = isHorizontal;
            Alpha = alpha;
            Stretch = Math.Abs(val2 - val1);
            if (val1 == int.MinValue || axis == int.MinValue)
                valid = 0;
            else
                valid = 1;
        }
        #endregion

        public bool Valid => valid != 0;
        public override string ToString()
        {
            return string.Format(tostr, Val, Val + Stretch, Axis);
        }
    }
}
#endif
