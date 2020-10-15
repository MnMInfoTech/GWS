/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    public static class IDGenerator
    {
        static readonly Dictionary<string, int> newIDs = new Dictionary<string, int>(100);
      
        #region NEW ID
        /// <summary>
        /// Gets a new ID for a given object type.
        /// </summary>
        /// <param name="objType">Type name</param>
        /// <returns></returns>
        public static string NewID(this string objType)
        {
            if (objType == null)
                return null;
            if (!newIDs.ContainsKey(objType))
                newIDs.Add(objType, 0);

            var newID = ++newIDs[objType];
            newIDs[objType] = newID;
            return objType + newID;
        }

        /// <summary>
        /// Resets an internal counter of IDs for a given object type.
        /// </summary>
        /// <param name="objType">Type name</param>
        /// <returns></returns>
        public static void ResetID(this string objType)
        {
            if (objType == null)
                return;
            if (!newIDs.ContainsKey(objType))
                return;

            newIDs[objType] = 0;
        }

        /// <summary>
        /// Returns unique ID for a given type.
        /// </summary>
        /// <param name="objType">Type of object unique id is sought for</param>
        /// <returns>New ID</returns>
        public static string NewID(this Type objType)
        {
            if (objType == null)
                return null;

            return NewID(objType.FullName);
        }

        /// <summary>
        /// Returns unique ID for a given objType which is a class name usually.
        /// </summary>
        /// <param name="objType">Class name usually</param>
        /// <returns>New ID</returns>
        public static string NewID(this object o)
        {
            if (o == null)
                return null;
            return NewID(o.GetType());
        }
        #endregion

    }
}
