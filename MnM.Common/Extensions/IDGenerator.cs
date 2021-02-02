/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    public static class IDGenerator
    {
        #region VARIABLES
        static volatile uint uniqueID;
        static readonly Dictionary<string, uint> newNames = new Dictionary<string, uint>(100);
        #endregion

        #region NEW NAME
        /// <summary>
        /// Gets a new ID for a given object type.
        /// </summary>
        /// <param name="objType">Type name</param>
        /// <returns></returns>
        public static string NewName(this string objType, bool increment = true)
        {
            if (objType == null)
                return null;
            if (!newNames.ContainsKey(objType)) 
                newNames.Add(objType, 0);
            uint newID;
            if (increment)
                newID = ++newNames[objType];
            else
                newID = newNames[objType];
            if (increment)
                newNames[objType] = newID;
            else
                ++newID;
            return objType + (newID);
        }

        /// <summary>
        /// Returns unique ID for a given type.
        /// </summary>
        /// <param name="objType">Type of object unique id is sought for</param>
        /// <returns>New ID</returns>
        public static string NewName(this Type objType, bool increment = true)
        {
            if (objType == null)
                return null;

            return NewName(objType.FullName, increment);
        }

        /// <summary>
        /// Returns unique ID for a given objType which is a class name usually.
        /// </summary>
        /// <param name="objType">Class name usually</param>
        /// <returns>New ID</returns>
        public static string NewName(this object o, bool increment = true)
        {
            if (o == null)
                return null;
            return NewName(o.GetType(), increment);
        }
        #endregion

        #region NEW ID
        /// <summary>
        /// Gets a shape specific incremental ID.
        /// </summary>
        /// <param name="renderable">Shape which the ID is sought for.</param>
        /// <returns></returns>
        public static uint NewID(this IRenderable renderable) =>
            ++uniqueID;

        /// <summary>
        /// Gets a shape specific available ID.
        /// </summary>
        /// <param name="renderable">Shape which the ID is sought for.</param>
        /// <returns></returns>
        public static uint AvailableID(this IRenderable renderable) =>
            uniqueID + 1;

        /// <summary>
        /// Gets a block specific incremental ID. Use this while drawing a block which you intend to move or resize or erase later.
        /// </summary>
        /// <returns></returns>
        internal static uint NewID() =>
            ++uniqueID;
        #endregion
    }
}
