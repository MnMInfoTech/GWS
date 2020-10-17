/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
    public class InvalidateEventArgs
    {
        internal InvalidateEventArgs() { }
        public InvalidateEventArgs(Rectangle rectangle, bool addMode = false)
        {
            AddMode = addMode;
            Area = rectangle;
        }
        public bool AddMode { get; internal set; }
        public Rectangle Area { get; internal set; }
    }
}
