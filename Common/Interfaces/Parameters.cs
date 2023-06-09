/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region IPARAMETER
    /// <summary>
    /// Marker interface - represents any object which serves as parameter in 
    /// rendering methods:
    /// FillAction.
    /// DrawAction.
    /// </summary>
    public interface IParameter { }
    #endregion

    #region IINLINE-PARAMETER
    /// <summary>
    /// Marker interface - represents any object which serves as parameter in 
    /// the following delegates:
    /// FillAction.
    /// DrawAction.
    /// </summary>
    public interface IInLineParameter : IParameter
    { }
    #endregion

    #region IUPDATE-PARAMETER
    /// <summary>
    /// Marker interface - represents any object which serves as parameter in 
    /// the following delegates:
    /// FillAction.
    /// DrawAction.
    /// </summary>
    public interface IUpdateParameter : IParameter
    { }
    #endregion

    #region IROTATION-PARAMETER
    /// <summary>
    /// Marker interface - represents any object which serves as an member elelemnt of IRotaton interface.
    /// </summary>
    public interface IRotationParameter : IParameter
    { }
    #endregion

    #region ICOMPOSITE PARAMETER
    /// <summary>
    /// Marker interface - represents any object which serves as an entity with composite parameters in 
    /// rendering methods:
    /// FillAction.
    /// DrawAction.
    /// </summary>
    public interface ICompositeParameter : IParameter, IEnumerable<IParameter>
    {
    }
    #endregion

    #region IEx COMPOSITE PARAMETER
#if DevSupport
    public
#else

    internal
#endif
        interface IExCompositeParameter : ICompositeParameter
    {
        void CopyFrom(IEnumerable<IParameter> other, bool incrementalOffset = false);
    }
    #endregion

    #region IUNUSED PARAMETERS
    public interface IUnusedParametersHolder
    {
        /// <summary> 
        /// Any remaining parameters which can not be parsed for extraction will
        /// be accumalted in this collection and left to be handled by the user accordingly.
        /// </summary>
        IPrimitiveList<IParameter> UnUsedParameters { get; }
    }
    #endregion
}
