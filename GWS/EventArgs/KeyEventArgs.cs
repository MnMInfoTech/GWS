/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)

namespace MnM.GWS
{
    /// <summary>
    /// Represents an argument object to relay key input information.
    /// </summary>
    public interface IKeyEventArgs : ICancelEventArgs
    {
        /// <summary>
        /// The code of key pressed or released.
        /// </summary>
        Key KeyCode { get; }

        /// <summary>
        /// Indicates if given modifier key is presses or not.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        bool this[ModifierKeys keys] { get; }

        /// <summary>
        /// Indicates whether any modifier key is pressed or released.
        /// </summary>
        ModifierKeys ModifierKeys { get; }

        /// <summary>
        /// Current state of key - i.e up or down.
        /// </summary>
        KeyState State { get; }
    }

    internal interface IExKeyEventArgs : IKeyEventArgs
    {
        new Key KeyCode { get; set; }
        new KeyState State { get; set; }
        new ModifierKeys ModifierKeys { get; set; }
    }

    public sealed class KeyEventArgs : CancelEventArgs, IExKeyEventArgs
    {
        #region VARIABLES
        const string toStr = "key:{0}, mod: {1}"; 
        #endregion

        public KeyEventArgs() { }

        public KeyEventArgs(IKeyEventArgs args)
        {
            KeyCode = args.KeyCode;
            State = args.State;
            ModifierKeys = args.ModifierKeys;
        }
        public KeyEventArgs(Key key, KeyState keyState, ModifierKeys modifierKeys)
        {
            State = keyState;
            KeyCode = key;
            ModifierKeys = modifierKeys;
        }

        #region PROPERTIES
        public Key KeyCode { get; private set; }
        public KeyState State { get; private set; }
        public ModifierKeys ModifierKeys { get; private set; }
        public bool this[ModifierKeys keys]=> (ModifierKeys & keys) == keys;
        Key IExKeyEventArgs.KeyCode { get => KeyCode; set => KeyCode = value; }
        KeyState IExKeyEventArgs.State { get => State; set => State = value; }
        ModifierKeys IExKeyEventArgs.ModifierKeys { get => ModifierKeys; set => ModifierKeys = value; }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, KeyCode, ModifierKeys);
        }
    }
}
#endif