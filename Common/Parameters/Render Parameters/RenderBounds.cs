/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
namespace MnM.GWS
{
    public interface IRenderBounds: IPoint, ISize, IBounds, IParameter
    {
        void Set(IBounds newBounds);
        void Set(int x, int y, int w, int h);
        void Clear();
    }

    #region RENDER BOUNDS
    internal class RenderBounds: IRenderBounds
    {
        /// <summary>
        /// X co-ordinate of location of this object.
        /// </summary>
        public int X;

        /// <summary>
        /// Y co-ordinate of location of this object.
        /// </summary>
        public int Y;

        /// <summary>
        /// Width of this object.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of this object.
        /// </summary>
        public int Height;

        static string description = "X: {0}, Y: {1}, W: {2}, H: {3}";

        #region CONSTRUCTORS
        public RenderBounds() { }
        public RenderBounds(IBounds newBounds)
        {
            newBounds.GetBounds(out X, out Y, out Width, out Height);
        }
        public RenderBounds(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        #endregion

        #region PROPERTIES
        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
        bool IValid.Valid => Width != 0 && Height != 0;
        #endregion

        #region GET BOUNDS
        public void GetBounds(out int x, out int y, out int w, out int h)
        {
            x = X;
            y = Y;
            w = Width;
            h = Height;
        }
        #endregion

        #region SET
        public void Set(IBounds newBounds)
        {
            newBounds.GetBounds(out X, out Y, out Width, out Height);
        }

        public void Set(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
        #endregion

        #region CLEAR
        public void Clear()
        {
            X = Y = Width = Height = 0;
        }
        #endregion

        public override string ToString()
        {
            return string.Format(description, X, Y, Width, Height);
        }
    }
    #endregion
}
