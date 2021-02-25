/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
namespace MnM.GWS
{
    public abstract class _Animation: IAnimation
    {
        #region VARIABLES
        protected volatile bool Running;
        public readonly Boundary Boundary;
        int id;
        static int uid;
        #endregion

        #region CONSTRUCTOR
        public _Animation()
        {
            Running = true;
            Boundary = new Boundary(Type);
            id = ++uid;
        }
        #endregion

        #region PROPERTIES
        public int ID => id;
        public bool IsRunning => Running;
        public abstract byte Type { get; }
        public abstract ISettings Settings { get; }
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        IBoundary IAnimation.Boundary => Boundary;
        #endregion

        #region SWITCH
        public virtual void Run(bool on)
        {
            Running = on;
        }
        #endregion
    }
}
