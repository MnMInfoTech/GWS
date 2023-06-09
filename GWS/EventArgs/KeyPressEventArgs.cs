/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

#if (GWS || Window)
using System;

namespace MnM.GWS
{
    /// <summary>
    /// Represents an argument object to relay key press information.
    /// </summary>
    public interface IKeyPressEventArgs : IEventArgs, IChar
    {
    }
    public interface IExKeyPressEventArgs : IKeyPressEventArgs
    {
        new char KeyChar { get; set; }
    }
    public class KeyPressEventArgs : EventArgs, IExKeyPressEventArgs
    {
        public KeyPressEventArgs() :
            this((char)0)
        { }
        public KeyPressEventArgs(char keyChar)
        {
            KeyChar = keyChar;
        }
        public char KeyChar { get; private set; }
        char IExKeyPressEventArgs.KeyChar { get => KeyChar; set => KeyChar = value; }
    }
}
#endif
