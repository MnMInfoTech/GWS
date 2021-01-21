/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
namespace MnM.GWS
{
#if Functions
    #region CLONING
    /// <summary>
    /// Enum ToClone
    /// </summary>
    public enum Cloning
    {
        /// <summary>
        /// The values
        /// </summary>
        Values,

        /// <summary>
        /// The structure
        /// </summary>
        Structure,

        /// <summary>
        /// The both
        /// </summary>
        Both
    }
    #endregion

    #region FX ENTITY
    /// <summary>
    /// Enum EntityType
    /// </summary>
    public enum FxEntity
    {
        /// <summary>
        /// The output
        /// </summary>
        Output,

        /// <summary>
        /// The input
        /// </summary>
        Input,

        /// <summary>
        /// The both
        /// </summary>
        Both,

        /// <summary>
        /// The none
        /// </summary>
        None
    }
    #endregion

    #region SERIES CHANGE
    /// <summary>
    /// Enum SeriesChange
    /// </summary>
    public enum SeriesChange
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0x0,
        /// <summary>
        /// The added
        /// </summary>
        Added = 0x1,
        /// <summary>
        /// The edited
        /// </summary>
        Edited = 0x2,
        /// <summary>
        /// The both
        /// </summary>
        Both = Added | Edited
    }
    #endregion
#endif

}
