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
#if Data
    /// <summary>
    /// Enum Usage
    /// </summary>
    public enum Usage
    {
        /// <summary>
        /// Value of Field marked with this usage will get Inserted and Updated but will not be used as
        /// Criteria in Update or Delete operations.
        /// </summary>
        NonKey,
        /// <summary>
        /// Value of Field marked with this usage will get Inserted.
        /// However, the Field will only be used as Criteria for Update
        /// and Delete operations
        /// </summary>
        Key,
        /// <summary>
        /// Value of Field marked with this usage is used in Update or Delete operations
        /// as Criteria only. This field will be ignored in Insert operation.
        /// </summary>
        CriteriaOnly,
        /// <summary>
        /// Value of Field marked with this usage is used in Update or Delete operations
        /// as Criteria only. AutoIncrement Field will have this usage.
        /// </summary>
        Identity,
        /// <summary>
        /// Field marked with this usage is both AutoIncremental as well as Primary Key.
        /// Only be used in Update or Delete Operations
        /// </summary>
        IdentityKey,
        /// <summary>
        /// Value of Field marked with this usage will completely ignored in all
        /// operations. Computed field which does not exist in table can have
        /// this usage.
        /// </summary>
        None,
    }
#endif
}
