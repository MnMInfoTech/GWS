/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MnM.GWS
{
    #region IPROPERTY-COLLECTION
    public interface IPropertyCollection : IPropertyBag, IReadOnlyList<IProperty>
    {
        bool IndexOf<T>(out int index) where T : IProperty;
    }

    internal interface IExPropertyCollection : IPropertyCollection, IExPropertyBag, IExRefreshProperties
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        bool Set<T>(T value) where T : IProperty;
    }
    #endregion

    #region PROPERTY COLLECTION CLASS
#if DevSupport
    public
#else
    internal
#endif
    class PropertyCollection : IExPropertyCollection
    {
        #region VARIABLES
        Dictionary<string, int> Indices;
        IProperty[] Data;
        int propertyIndex = -1;
        IExPropertyEnabledControl Control;
        #endregion

        #region CONSTRUCTORS
#if DevSupport
    public
#else
        internal
#endif
        PropertyCollection(IExPropertyEnabledControl control)
        {
            Control = control;
            Data = new IProperty[2];
            Indices = new Dictionary<string, int>();
        }
        #endregion

        #region PROPERTIES
        public int Count => Data.Length;
        public IProperty this[int index] => Data[index];
        #endregion

        #region GET 
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>() where T : IProperty
        {
            if (!IndexOf(typeof(T), out int i, out _) || i >= Data.Length)
                return default(T);
            var result = Data[i];
            if (result == null || result.Value == null)
                return default(T);
            return (T)result;
        }
        #endregion

        #region SET
        bool Set<T>(T value, out T property, out string name) where T:IProperty
        {
            property = value;
            name = null;
            var t = typeof(T);
            if(t.Name == "IProperty")
            {
                if(value.Value == null)
                    return false;
                t = value.Value.GetType();
            }
            if (!IndexOf(t, out int i, out name))
            {
                return false;
            }
            
            if (i >= Data.Length)
                Array.Resize(ref Data, i + 2);

            if (value is IModifier<T> && Data[i] != null)
                property = ((IModifier<T>)value).Modify((T)Data[i]);

            Data[i] = property;
            return true;
        }
        bool IExPropertyCollection.Set<T>(T value)=>
            Set(value, out _, out _);
        public bool Set<T>(T value, bool silent = true) where T : IProperty
        {
            if (!Set(value, out T property, out string name))
                return false;            
            Control.OnPropertyChanged(property, silent, name);
            return true;
        }
        #endregion

        #region ADD NAMES
        protected void add<T>() where T : IProperty => 
            ((IExPropertyCollection)this).Add<T>();
        void IExPropertyBag.Add<T>()
        {
            var name = typeof(T).Name;
            if (Indices.ContainsKey(name))
                return;
            Indices.Add(name, ++propertyIndex);
        }

        protected void add<T>(string Name) where T : IProperty =>
            ((IExPropertyCollection)this).Add<T>(Name);

        void IExPropertyBag.Add<T>(string Name)
        {
            if (string.IsNullOrEmpty(Name))
                return;

            bool found = IndexOf(typeof(T), out int i, out string name);
            if (name == Name)
                return;
            if (found)
            {
                Indices.Add(Name, i);
                return;
            }
            Indices.Add(name, ++propertyIndex);
        }

        protected bool remove<T>(string Name) where T : IProperty =>
            ((IExPropertyCollection)this).Remove<T>(Name);

        bool IExPropertyBag.Remove<T>(string Name)
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            if (!Indices.ContainsKey(Name))
                return false;
            var i = Indices[Name];
            if (i < Data.Length)
                Data[i] = null;
            return Indices.Remove(Name);
        }
        #endregion

        #region REMOVE NAME
        protected bool remove<T>() where T : IProperty =>
            ((IExPropertyCollection)this).Remove<T>();

        bool IExPropertyBag.Remove<T>()
        {
            var Name = typeof(T).Name;
            if (!Indices.ContainsKey(Name))
                return false;
            var i = Indices[Name];
            if (i < Data.Length)
                Data[i] = null;
            return Indices.Remove(Name);
        }
        #endregion

        #region INDEXOF
        public bool Contains<T>() where T : IProperty
        {
            return IndexOf(typeof(T), out _, out _);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IndexOf<T>(out int i) where T : IProperty
        {
            return IndexOf(typeof(T), out i, out _);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IndexOf(Type type, out int i, out string name)
        {
            var Type = type.GetTypeInfo();
            name = Type.Name;
            if (Indices.TryGetValue(name, out i))
            {
                return true;
            }
            if (Type.IsInterface)
            {
                name = Type.Name.Substring(1);
                if (Indices.TryGetValue(name, out i))
                {
                    return true;
                }
            }
            var interfaces = Type.GetInterfaces();
            foreach (var item in interfaces)
            {
                if (Indices.TryGetValue(item.Name, out i))
                {
                    name = Type.Name;
                    return true;
                }
            }
            i = -1;
            name = null;
            return false;
        }
        #endregion

        #region REFRESH
        IPrimitiveList<IParameter> IExRefreshProperties.RefreshProperties(IEnumerable<IParameter> parameters) =>
            RefreshProperties(parameters);
        protected IPrimitiveList<IParameter> RefreshProperties(IEnumerable<IParameter> parameters)
        {
            if (parameters == null)
                return null;

            #region EXTRACT PARAMETERS
            var rotParams = new PrimitiveList<IRotationParameter>();
            var remaining = new PrimitiveList<IParameter>();

            IRotation rotation = null;
            bool handled;

            foreach (var item in parameters)
            {
                if (item == null)
                    continue;

                handled = false;

                IProperty property = item as IProperty;

                if (property == null)
                    goto COLLECT_NON_PARSEABLE;

                if (property is IRotation)
                {
                    rotation = ((IRotation)property);
                    handled = true;
                }
                else if (property is IRotationParameter)
                {
                    rotParams.Add((IRotationParameter)property);
                    handled = true;
                }
                else
                {
                    handled = IndexOf(property.GetType(), out int i, out _);
                    if (handled)
                    {
                        if (i >= Data.Length)
                            Array.Resize(ref Data, i + 2);

                        Data[i] = property;
                    }
                }
                COLLECT_NON_PARSEABLE:
                if (!handled)
                    remaining.Add(property);
            }

            if (rotParams.Count > 0)
            {
                if (rotation == null)
                    rotation = new Rotation();
                rotation.Modify(rotParams);
            }

            if (rotation != null)
            {
                handled = IndexOf(rotation.GetType(), out int i, out _);
                if (handled)
                {
                    if (i >= Data.Length)
                        Array.Resize(ref Data, i + 1);

                    Data[i] = rotation;
                }
                else
                    remaining.Add(rotation);
            }
            #endregion

            if (remaining.Count > 0)
            {
                Control.SetRemainingProperties(remaining);
                return remaining;
            }
            return null;
        }
        #endregion

        #region GET PROPERTIES
        IEnumerable<IProperty> IExGetProperties.GetProperties(bool privateToo)
        {
            foreach (var item in Data)
            {
                if (item == null)
                    continue;
                yield return item;
            }
        }
        #endregion

        #region ENUMERATOR
        public IEnumerator<IProperty> GetEnumerator()
        {
            foreach (var item in Data)
            {
                if (item == null)
                    continue;
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion
    }
    #endregion
}
