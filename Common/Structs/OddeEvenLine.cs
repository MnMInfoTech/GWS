/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    /// <summary>
    /// Reprents an ScanLine structure which can also have a lenght to stretch.
    /// </summary>
    public struct OddEvenLine : IAxisLine, ISpan<int>, IConvertible<int[]>, IAlpha
    {
        const byte MAX = 255;

        #region VARIABLES
        /// <summary>
        /// Axis value : if IsHorizontal is trule then Y otherwise X.
        /// </summary>
        public readonly int Axis;

        /// <summary>
        /// Specifies rendering option for this line.
        /// </summary>
        public readonly LineFill Draw;

        /// <summary>
        /// Alpha value to antialias entire point up to the stretch specified.
        /// </summary>
        public readonly byte Alpha;

        /// <summary>
        /// 
        /// </summary>
        readonly int[] Data;

        const string tostr = "Count: {0}, Axis: {1}, Values: {2}, Draw: {3}";
        #endregion

        #region CONSTRUCTORS
        public OddEvenLine(IEnumerable<int> data, int axis, LineFill draw, byte alpha) :
            this(data, axis, draw)
        { 
            Alpha = alpha;
        }
        public OddEvenLine(IEnumerable<int> data, int axis, LineFill draw)
        {
            Axis = axis;
            Data = data is int[]? (int[])data : data?.ToArray() ?? new int[0];
            Alpha = MAX;
            if (Data.Length == 2)
            {
                if (Data[0] > Data[1])
                {
                    var temp = Data[1];
                    Data[1] = Data[0];
                    Data[0] = temp;
                }
            }
            else if (Data.Length > 2)
            {
                System.Array.Sort(Data);
            }
            else if (Data.Length == 1)
            {
                var val = Data[0];
                Data = new int[2] { val, val };
            }
            Draw = draw;
        }
        public OddEvenLine(int axis, LineFill draw, params int[] data) :
            this(data, axis, draw)
        { }
        public OddEvenLine(int axis, byte alpha, LineFill draw, params int[] data) :
            this(data, axis, draw, alpha)
        { }
        public OddEvenLine(int val1, int axis, int val2, LineFill draw, byte alpha):
            this(val1, axis, val2, draw)
        {
            Alpha = alpha;
        }
        public OddEvenLine(int val1, int axis, int val2, LineFill draw)
        {
            Axis = axis;
            if (val1 > val2)
            {
                var temp = val1;
                val1 = val2;
                val2 = temp;
            }
            Data = new int[] { val1, val2 };
            Draw = draw;
            Alpha = MAX;
        }
        #endregion

        #region PROPERTIES
        public int this[int index]
        {
            get
            {
                return Data[index];
            }
        }
        public int Count => Data.Length;
        public bool IsHorizontal => (Draw & LineFill.Vertical) != LineFill.Vertical;
        int IAxis.Axis => Axis;
        LineFill IAxis.Draw => Draw;
        byte IAlpha.Alpha => Alpha;
        IEnumerable<IAxisPoint> IEndPoints.Points
        {
            get
            {
                foreach (var item in Data)
                {
                    yield return new AxisPoint(item, Axis, Draw);
                }
            }
        }
        int ISpan<int>.Start
        {
            get
            {
                if (Data == null || Data.Length == 0)
                    return 0;
                return Data[0];
            }
        }
        int ISpan<int>.End
        {
            get
            {
                if (Data == null || Data.Length == 0)
                    return 0;
                if (Data.Length == 1)
                    return Data[0];
                return Data[Data.Length - 1]; 
            }
        }
        #endregion

        #region CONVERT<INT>
        public int[] Convert() => Data;
        #endregion

        public override string ToString()
        {
            return string.Format(tostr, Data?.Length ?? 0, Axis, string.Join(",", Data), Draw);
        }

        int[] IConvertible<int[]>.Convert()
        {
            throw new System.NotImplementedException();
        }
    }


    /// <summary>
    /// Reprents an ScanLine structure which can also have a lenght to stretch.
    /// </summary>
    public struct OddEvenLineF: IAxisLine, ISpan<float>, IConvertible<float[]>
    { 
        #region VARIABLES
        /// <summary>
        /// Axis value : if IsHorizontal is trule then Y otherwise X.
        /// </summary>
        public readonly int Axis;

        /// <summary>
        /// Specifies rendering option for this line.
        /// </summary>
        public readonly LineFill Draw;

        /// <summary>
        /// 
        /// </summary>
        readonly float[] Data;

        const string tostr = "Count: {0}, Axis: {1}, Values: {2}, Draw: {3}";
        #endregion

        #region CONSTRUCTORS
        public OddEvenLineF(IEnumerable<float> data, int axis, LineFill draw)  
        {
            Axis = axis;
            Data = data is float[]? (float[])data: data?.ToArray()?? new float[0];
            if (Data.Length == 2)
            {
                if (Data[0] > Data[1])
                {
                    var temp = Data[1];
                    Data[1] = Data[0];
                    Data[0] = temp;
                }
            }
            else if (Data.Length > 2)
            {
                System.Array.Sort(Data);
            }
            else if(Data.Length == 1)
            {
                var val = Data[0];
                Data= new float[2] { val, val };
            }
            Draw = draw;
        }
        public OddEvenLineF(int axis, LineFill draw, params float[] data) :
            this(data, axis, draw)
        { }
        public OddEvenLineF(float val1, int axis, float val2, LineFill draw)
        {
            Axis = axis;
            if (val1 > val2)
            {
                var temp = val1;
                val1 = val2;
                val2 = temp;
            }
            Data = new float[] { val1, val2 };
            Draw = draw;
        }
        #endregion

        #region PROPERTIES
        public float this[int index]
        {
            get
            {
                return Data[index];
            }
        }
        public int Count => Data.Length;
        public bool IsHorizontal => (Draw & LineFill.Vertical) != LineFill.Vertical;
        int IAxis.Axis => Axis;
        LineFill IAxis.Draw => Draw;
        IEnumerable<IAxisPoint> IEndPoints.Points
        {
            get
            {
                foreach (var item in Data)
                {
                    yield return new AxisPoint(item, Axis, Draw);
                }
            }
        }
        float ISpan<float>.Start
        {
            get
            {
                if (Data == null || Data.Length == 0)
                    return 0;
                return Data[0];
            }
        }
        float ISpan<float>.End
        {
            get
            {
                if (Data == null || Data.Length == 0)
                    return 0;
                if(Data.Length  == 1)
                    return Data[0];
                return Data[Data.Length - 1];
            }
        }
        #endregion

        #region CONVERT<INT>
        public float[] Convert() => Data;
        #endregion

        public override string ToString()
        {
            return string.Format(tostr, Data?.Length ?? 0, Axis, string.Join(",", Data), Draw);
        }
    }
}

