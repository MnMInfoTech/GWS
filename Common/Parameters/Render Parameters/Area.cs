/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    #region IAREA
    /// <summary>
    /// Represents an object which provides option to control screen shot taking operation.
    /// </summary>
    public interface IArea : IParameter, IPurposedBounds
    { }
    #endregion

    #region AREA
    public struct Area: IArea
    {
        readonly IBounds Bounds;
        public readonly Purpose Purpose;

        public Area(int x, int y, int w, int h, Purpose areaFor = Purpose.Copy)
        {
            Bounds = new Rectangle(x, y, w, h);
            Purpose = areaFor;
        }
        public Area(IBounds bounds, Purpose areaFor = Purpose.Copy) 
        {
            Bounds = bounds;
            Purpose = areaFor;
        }

        Purpose IEnumHolder<Purpose>.Kind => Purpose;
        public void GetBounds(out int x, out int y, out int w, out int h)
        {
            x = y = w = h = 0;
            Bounds?.GetBounds(out x, out y, out w, out h);
        }

        public bool Valid => Bounds?.Valid ?? false;

        public override string ToString()
        {
            return Bounds?.ToString() ?? "Invalid";
        }
    }
    #endregion
}
