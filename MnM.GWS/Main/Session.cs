/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window

namespace MnM.GWS
{
    public sealed class Session: Boundary, ISession
    {
        #region VARIABLES
        /// <summary>
        /// X co-ordinate of the Destination of shape rendering.
        /// </summary>
        public int DstX;

        /// <summary>
        /// Y co-ordinate of the Destination of shape rendering.
        /// </summary>
        public int DstY;
        #endregion

        #region CONSTRUCTORS
        public Session(){ }
        public Session(uint shapeID, int processID, int dstX = 0, int dstY = 0)
        {
            ShapeID = shapeID;
            ProcessID = processID;
            DstX = dstX;
            DstY = dstY;
        }
        public Session(uint shapeID)
        {
            ShapeID = shapeID;
        }
        public Session(int processID, int dstX = 0, int dstY = 0)
        {
            ProcessID = processID;
            DstX = dstX;
            DstY = dstY;
        }
        public Session(int x, int y, int w, int h, uint shapeID, int processID, int dstX = 0, int dstY = 0)
            : base(x, y, w, h)
        {
            ShapeID = shapeID;
            ProcessID = processID;
            DstX = dstX;
            DstY = dstY;
        }
        public Session(IBoundable perimeter) :
            base(perimeter)
        {
            if (perimeter is IDstPoint)
            {
                DstX = ((IDstPoint)perimeter).X;
                DstY = ((IDstPoint)perimeter).Y;
            }
        }
        #endregion

        #region PROPERTIES
        int IDstPoint.X { get => DstX; set => DstX = value; }
        int IDstPoint.Y { get => DstY; set => DstY = value; }
        int IPoint.X => DstX;
        int IPoint.Y => DstY;
        uint ISession.ShapeID 
        { 
            get => ShapeID; 
            set => ShapeID = value; 
        }
        int ISession.ProcessID { get => ProcessID; set => ProcessID = value; }
        #endregion

        #region CLONE
        public object Clone()
        {
            var session = new Session(this);
            return session;
        }
        #endregion
    }
}
#endif
