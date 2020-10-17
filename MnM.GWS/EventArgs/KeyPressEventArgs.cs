/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
    public class KeyPressEventArgs : EventArgs, IKeyPressEventArgs
    {
        public KeyPressEventArgs()
        {

        }
        public KeyPressEventArgs(char keyChar)
        {
            KeyChar = keyChar;
        }
        public char KeyChar { get; set; }
    }
}
