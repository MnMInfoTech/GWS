/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    public interface IMaxSize : ISize, IProperty, IValid
    { }

    /// <summary>
    /// Represents dimension in terms of width and height.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MaxSize : IMaxSize
    {
        #region VARIABLES
        /// <summary>
        /// Value of Width.
        /// </summary>
        public int Width;

        /// <summary>
        /// Value of height.
        /// </summary>
        public int Height;

        /// <summary>
        /// Empty instance of this object.
        /// </summary>
        public readonly static MaxSize Empty = new MaxSize();
        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Creates new instance using given values of width and height.
        /// </summary>
        /// <param name="width">Width value.</param>
        /// <param name="height">Height value.</param>
        public MaxSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates new instance using given values of width and height.
        /// </summary>
        /// <param name="width">Width value.</param>
        /// <param name="height">Height value.</param>
        public MaxSize(float width, float height)
        {
            Width = (int)width;
            Height = (int)height;
            if (width - Width != 0)
                ++Width;
            if (height - Height != 0)
                ++Height;
        }

        public MaxSize(ISize rc) :
            this(rc.Width, rc.Height)
        { }
        #endregion

        #region PROPERTIES
        int ISize.Width => Width;
        int ISize.Height => Height;
        object IValue.Value => this;
        public bool Valid => Width > 0 && Height > 0;
        #endregion

        public override string ToString()
        {
            return string.Format("Width : {0}, Height: {1}", Width, Height);
        }
    }

}
