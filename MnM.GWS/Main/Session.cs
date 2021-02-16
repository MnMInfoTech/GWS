/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public sealed partial class Session: ReadSession, ISession
    {
        #region VARIABLES
        /// <summary>
        /// ID of shape being rendered.
        /// </summary>
        public uint ShapeID;

        /// <summary>
        /// ID of process being used to render the shape.
        /// </summary>
        public int ProcessID;

        /// <summary>
        /// X co-ordinate of the Destination of shape rendering.
        /// </summary>
        public int DstX;

        /// <summary>
        /// Y co-ordinate of the Destination of shape rendering.
        /// </summary>
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
        public Session(IBoundable perimeter) : this()
        {
            perimeter.GetBounds(out X1, out Y1, out int w, out int h);
            X2 = X1 + w;
            Y2 = Y1 + h;

            if (perimeter is IProcessID)
                ProcessID = ((IProcessID)perimeter).ProcessID;
           
            if(perimeter is IShapeID)
                ShapeID = ((IShapeID)perimeter).ShapeID;

            if(perimeter is IDstPoint)
            {
                DstX = ((IDstPoint)perimeter).X;
                DstY = ((IDstPoint)perimeter).Y;
            }
        }
        #endregion

        #region PROPERTIES
        public bool Valid => X2 > 0 && Y2 > 0;
        uint IShapeID.ShapeID => ShapeID;
        int IProcessID.ProcessID => ProcessID;
        int IDstPoint.X { get => DstX; set => DstX = value; }
        int IDstPoint.Y { get => DstY; set => DstY = value; }
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
        public void GetBounds(out int x, out int y, out int w, out int h)
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
            w = x2 - x;
            h = y2 - y;
        }
        #endregion

        #region CLONE
        protected override ReadSession newInstance() =>
            new Session();
        protected override void CopyTo(ReadSession session)
        {
            base.CopyTo(session);
            Session session1 = (Session)session;
            session1.ShapeID = ShapeID;
            session1.ProcessID = ProcessID;
            session1.DstX = DstX;
            session1.DstY = DstY;
            session1.X1 = X1;
            session1.Y1 = Y1;
            session1.X2 = X2;
            session1.Y2 = Y2;
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
#endif
