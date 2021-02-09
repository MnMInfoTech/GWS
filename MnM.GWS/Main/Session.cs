/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public sealed class Session: ISession
    {
        #region VARIABLES
        public uint ShapeID;
        public int ProcessID;
        public int DstX;
        public int DstY;

        /// X co-ordinate of recently drawn area of this object.
        /// </summary>
        int X1 = int.MaxValue;

        /// <summary>
        /// Y co-ordinate of recently drawn area of this object.
        /// </summary>
        int Y1 = int.MaxValue;

        /// <summary>
        /// Far right X co-ordinate of recently drawn area of this object.
        /// </summary>
        int X2 = 0;

        /// <summary>
        /// Far bottom Y co-ordinate of recently drawn area of this object.
        /// </summary>
        int Y2 = 0;

        static string description = "X: {0}, Y: {1}, R: {2}, B: {3}";
        #endregion

        #region CONSTRUCTORS
        public Session(){ }
        public Session(int x, int y, int w, int h, uint shapeID, int processID, int dstX = 0, int dstY = 0)
        {
            X1 = x;
            Y1 = y;
            X2 = X1 + w;
            Y2 = Y1 + h;
            ShapeID = shapeID;
            ProcessID = processID;
            DstX = dstX;
            DstY = dstY;
        }
        public Session(IPerimeter perimeter) : this()
        {
            perimeter.GetBounds(out X1, out Y1, out int w, out int h);
            X2 = X1 + w;
            Y2 = Y1 + h;
            ProcessID = perimeter.ProcessID;
            ShapeID = perimeter.ShapeID;
        }
        #endregion

        #region PROPERTIES
        public bool Valid => X2 != 0 && Y2 != 0;
        uint IShapeID.ShapeID => ShapeID;
        int IProcessID.ProcessID => ProcessID;
        int IDstPoint.DstX => DstX;
        int IDstPoint.DstY => DstY;
        #endregion

        #region NOTIFY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Notify(int x1, int y1, int x2, int y2)
        {
            if (x2 == 0 && y2 == 0)
            {
                X1 = Y1 = int.MaxValue;
                X2 = Y2 = 0;
                return;
            }
            if (x1 < X1)
                X1 = x1;
            if (y1 < Y1)
                Y1 = y1;
            if (x2 > X2)
                X2 = x2;
            if (y2 > Y2)
                Y2 = y2;
        }
        #endregion

        #region GET BOUNDS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetBounds(out int x, out int y, out int w, out int h, int xExpand = 0, int yExpand = 0)
        {
            if (X2 == 0 || Y2 == 0 || X2 - X1 <= 0 || Y2 - Y1 <= 0)
            {
                x = y = w = h = 0;
                return;
            }
            x = X1;
            y = Y1;
            var x2 = X2;
            var y2 = Y2;
            if (xExpand != 0)
            {
                x -= xExpand;
                if (x < 0) x = 0;
                x2 += xExpand;
            }
            if (yExpand != 0)
            {
                y -= yExpand;
                if (y < 0) y = 0;
                y2 += yExpand;
            }
            w = x2 - x;
            h = y2 - y;
        }
        #endregion

        #region TO STRING
        public override string ToString()
        {
            return string.Format(description, X1, Y1, X2, Y2);
        }
        #endregion
    }
}
