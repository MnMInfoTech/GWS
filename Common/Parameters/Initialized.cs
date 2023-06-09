/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public interface IInitialized : IProperty<bool>
    { }

    public struct Initialized : IInitialized
    {
        public readonly bool Value;
        public readonly static Initialized True = new Initialized(true);
        public readonly static Initialized False = new Initialized(false);

        Initialized(bool value)
        {
            Value = value;
        }
        object IValue.Value => Value;
        bool IValue<bool>.Value => Value;
    }
}
