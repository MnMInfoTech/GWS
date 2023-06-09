/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;

namespace MnM.GWS
{
    public interface IPropertyChangedEventArgs : IEventArgs, IName 
    {
        IProperty Property { get; }
        bool Silent { get; }
    }

    internal interface IExPropertyChangedEventArgs : IPropertyChangedEventArgs
    {
        void Reset(IProperty property, bool silent, string name);
    }
    #region PROPERTY CHANGED EVENT ARGS
    public sealed class PropertyChangedEventArgs : EventArgs, IExPropertyChangedEventArgs 
    {
        public PropertyChangedEventArgs() { }
        public PropertyChangedEventArgs(IProperty property)
        {
            Property = property;
        }
        public IProperty Property { get; private set; }
        public bool Silent { get; private set; }
        public string Name { get; private set; }

        void IExPropertyChangedEventArgs.Reset(IProperty property, bool silent, string name)
        {
            Property = property;
            Silent = silent;
            Name = name;
        }
    }
    #endregion
}
