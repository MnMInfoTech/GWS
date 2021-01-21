/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
#if Collections

    #region FILTER STATUS
    /// <summary>
    /// Enum FilterStatus
    /// </summary>
    public enum FilterStatus
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The started
        /// </summary>
        Started,
        /// <summary>
        /// The applied
        /// </summary>
        Applied,
    }
    #endregion

    #region COLLECTION OPERATION
    /// <summary>
    /// Enum CollectionOperation
    /// </summary>
    public enum CollectionOperation
    {
        /// <summary>
        /// The clear
        /// </summary>
        Clear,
        /// <summary>
        /// The reset
        /// </summary>
        Reset,
        /// <summary>
        /// The change of indices
        /// </summary>
        ChangeOfIndices,
        /// <summary>
        /// The add
        /// </summary>
        Add,
        /// <summary>
        /// The insert
        /// </summary>
        Insert,
        /// <summary>
        /// The remove
        /// </summary>
        Remove,
        /// <summary>
        /// The move
        /// </summary>
        Move,
        /// <summary>
        /// The change
        /// </summary>
        Change,
        /// <summary>
        /// The add range
        /// </summary>
        AddRange,
        /// <summary>
        /// The insert range
        /// </summary>
        InsertRange,
        /// <summary>
        /// The remove range
        /// </summary>
        RemoveRange,
        /// <summary>
        /// The filter change
        /// </summary>
        FilterChange,
        /// <summary>
        /// The check changed
        /// </summary>
        CheckChanged,
        /// <summary>
        /// The update
        /// </summary>
        Update,
        //BulkOperation
    }
    #endregion

    #region PROCESS-STATUS
    /// <summary>
    /// Enum ProcessStatus
    /// </summary>
    public enum ProcessStatus
    {
        /// <summary>
        /// The end
        /// </summary>
        End,
        /// <summary>
        /// The start
        /// </summary>
        Start,
        /// <summary>
        /// The in progess
        /// </summary>
        InProgess
    }
    #endregion

    #region CRITERIA MODE
    /// <summary>
    /// Enum CriteriaMode
    /// </summary>
    public enum CriteriaMode
    {
        /// <summary>
        /// The include
        /// </summary>
        Include,
        /// <summary>
        /// The exclude
        /// </summary>
        Exclude
    }
    #endregion
#endif
}
