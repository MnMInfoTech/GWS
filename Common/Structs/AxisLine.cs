/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region IAXIS LINE
    /// <summary>
    /// Represents an object whis has an axis and a series of positons (values) lying on that axis.
    /// </summary>
    public interface IAxisLine : IScanLine, IAxis, IEndPoints, ICount
    { }
    #endregion

    /// <summary>
    /// Reprents an AxisLine structure which can also have a lenght to stretch.
    /// </summary>
    public struct AxisLine: IAxisLine, ISpan<int>, IAlpha
    {
        const byte MAX = 255;

        #region VARIABLES
        /// <summary>
        /// Axial value : if IsHorizontal is trule then X otherwise Y.
        /// </summary>
        public readonly int Start;

        /// <summary>
        /// Axis value : if IsHorizontal is trule then Y otherwise X.
        /// </summary>
        public readonly int Axis;

        /// <summary>
        /// Specifies rendering option for this line.
        /// </summary>
        public readonly LineFill Draw;

        /// <summary>
        /// <summary>
        /// Length of stretch for this point.
        /// </summary>
        /// </summary>
        public readonly int End;

        /// <summary>
        /// Alpha value to antialias entire point up to the stretch specified.
        /// </summary>
        public readonly byte Alpha;
        #endregion

        const string tostr = "Val1: {0}, Val2: {1}, Axis: {2}, Draw: {3}, Alpha: {4}";

        #region CONSTRUCTORS
        public AxisLine(float val, int axis, LineFill draw, int stretch = 0) : this()
        {
            Start = (int)val;
            Axis = axis;
            Draw = draw;
            Alpha = MAX;
            if (stretch < 0)
                stretch = -stretch;
            End = Start + stretch;
        }
        public AxisLine(int val1, int val2, int axis, LineFill draw, byte alpha):
            this(val1, val2, axis, draw)
        {
            Alpha = alpha;
        }
        public AxisLine(int val1, int val2, int axis, LineFill draw) 
        {
            Start = val1;
            Axis = axis;
            Draw = draw;
            Alpha = MAX;
            var stretch = (val2 - val1);
            if (stretch < 0)
                stretch = -stretch;
            End = Start + stretch;
        }
        #endregion

        #region PROPERTIES
        public int this[int index] 
        {
            get
            {
                if (index > 0)
                    return End;
                return Start;
            }
        }
        public bool IsHorizontal => (Draw & LineFill.Vertical) != LineFill.Vertical;
        public int Count => 2;
        int IAxis.Axis => Axis;
        LineFill IAxis.Draw => Draw;
        byte IAlpha.Alpha => Alpha;
        IEnumerable<IAxisPoint> IEndPoints.Points
        {
            get
            {
                float s = Start;
                if (Alpha != MAX)
                    s += Colours.Alphas[Alpha];
                yield return new AxisPoint(s, Axis, Draw);
                s = End;
                if (Alpha != MAX)
                    s += Colours.Alphas[Alpha];
                yield return new AxisPoint(s, Axis, Draw);
            }
        }
        int ISpan<int>.Start => Start;
        int ISpan<int>.End => End;
        #endregion

        public override string ToString()
        {
            return string.Format(tostr, Start, End, Axis, Draw, Alpha);
        }
    }

    /// <summary>
    /// Reprents an AxisLine structure which can also have a lenght to stretch.
    /// </summary>
    public struct AxisLineF : IAxisLine, ISpan<float>
    {
        #region VARIABLES
        /// <summary>
        /// Axial value : if IsHorizontal is trule then X otherwise Y.
        /// </summary>
        public readonly float Start;

        /// <summary>
        /// Axis value : if IsHorizontal is trule then Y otherwise X.
        /// </summary>
        public readonly int Axis;

        /// <summary>
        /// Specifies rendering option for this line.
        /// </summary>
        public readonly LineFill Draw;

        /// <summary>
        /// <summary>
        /// Length of stretch for this point.
        /// </summary>
        /// </summary>
        public readonly float End;
        #endregion

        const string tostr = "Val1: {0}, Val2: {1}, Axis: {2}, Draw: {3}";

        #region CONSTRUCTORS
        public AxisLineF(float val, int axis, LineFill draw, float stretch = 0) : this()
        {
            if (stretch < 0)
                stretch = -stretch;
            Start = val;
            End = val + stretch;
            Axis = axis;
            Draw = draw;           
        }
        public AxisLineF(float val1, float val2, int axis, LineFill draw) 
        {
            Start = val1;
            End = val2;
            Axis = axis;
            Draw = draw;
            if (Start > End)
            {
                var temp = Start;
                Start = End;
                End = temp;
            }
        }
        #endregion

        #region PROPERTIES
        public float this[int index]
        {
            get
            {
                if (index > 0)
                    return End;
                return Start;
            }
        }
        public bool IsHorizontal => (Draw & LineFill.Vertical) != LineFill.Vertical;
        public int Count => 2;
        int IAxis.Axis => Axis;
        LineFill IAxis.Draw => Draw;
        IEnumerable<IAxisPoint> IEndPoints.Points
        {
            get
            {
                yield return new AxisPoint(Start, Axis, Draw);
                yield return new AxisPoint(End, Axis, Draw);
            }
        }
        float ISpan<float>.Start => Start;
        float ISpan<float>.End => End;
        #endregion

        public override string ToString()
        {
            return string.Format(tostr, Start, End, Axis, Draw);
        }
    }
}
