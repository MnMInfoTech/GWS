/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public class LoopEventArgs : EventArgs
    {
        public bool Repeat { get; set; }
    }
}
