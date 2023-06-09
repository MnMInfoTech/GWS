/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System.Collections.Generic;

namespace MnM.GWS
{
    #region IPROPERTY
    public interface IProperty : IValue, IParameter
    { }

    public interface IProperty<T> : IValue<T>, IProperty
    { }
    #endregion

    #region IPROPERTY-BAG
    public interface IPropertyBag
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of property.</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Get<T>() where T : IProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of property.</typeparam>
        /// <param name="value"></param>
        /// <param name="silent"></param>
        /// <returns></returns>
        bool Set<T>(T value, bool silent = true) where T : IProperty;

        bool Contains<T>() where T : IProperty;
    }
    #endregion

    #region IPROPERTY-BAG
#if DevSupport
    public
#else
    internal
#endif
    interface IExPropertyBag : IPropertyBag, IExRefreshProperties, IExGetProperties
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of property.</typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        void Add<T>() where T : IProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        bool Remove<T>() where T : IProperty;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Name"></param>
        void Add<T>(string Name);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        bool Remove<T>(string name) where T : IProperty;
    }
    #endregion

    #region PROPERTY-BAG HOLDER
    public interface IPropertyBagHolder
    {
        IPropertyBag Properties { get; }
    }
    #endregion

    #region IEx PROPERTY BAG HOLDER
#if DevSupport
    public
#else
    internal
#endif
    interface IExPropertyBagHolder : IPropertyBagHolder, IExRefreshProperties
    {
       new IExPropertyBag Properties { get; }
    }
    #endregion

    #region IPROPERTY REFRESHER
#if DevSupport
    public
#else
    internal
#endif
    interface IExRefreshProperties
    {
        IPrimitiveList<IParameter> RefreshProperties(IEnumerable<IParameter> parameters);
    }
    #endregion

    #region IExGET-PROPERTIES
#if DevSupport
    public
#else
    internal
#endif
    interface IExGetProperties
    {
        IEnumerable<IProperty> GetProperties(bool privateToo = false);
    }
    #endregion

    #region IExGET-PROPERTIES
#if DevSupport
    public
#else
    internal
#endif
    interface IExPropertyEnabledControl
    {
        void SetRemainingProperties(IPrimitiveList<IParameter> parameters);
        void OnPropertyChanged<TProperty>(TProperty Property, bool Silent, string Name) 
            where TProperty : IProperty;
    }
    #endregion

    #region IPROPERTY MANAGEMENT
#if DevSupport
    public
#else
    internal
#endif
    partial interface IPropertyManagement : IExRefreshProperties, IExGetProperties, IPropertyBagHolder
#if DevSupport
        ,IExPropertyBagHolder
#endif
    {
    }
    #endregion

    #region IEx PROPERTY MANAGEMENT
    internal interface IExPropertyManagement : IPropertyManagement, IExPropertyBagHolder
    { }
    #endregion
}
