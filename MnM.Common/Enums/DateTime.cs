/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
namespace MnM.GWS
{
#if Dates || Advanced
    using System;

    #region CHANGE TYPE
    /// <summary>
    /// Enum ChangeType
    /// </summary>
    public enum DateChangeType
    {
        /// <summary>
        /// The date replacement
        /// </summary>
        DateReplacement,
        /// <summary>
        /// The day increment
        /// </summary>
        DayIncrement,
        /// <summary>
        /// The day replacement
        /// </summary>
        DayReplacement,
        /// <summary>
        /// The month increment
        /// </summary>
        MonthIncrement,
        /// <summary>
        /// The month replacement
        /// </summary>
        MonthReplacement,
        /// <summary>
        /// The year increment
        /// </summary>
        YearIncrement,
        /// <summary>
        /// The year replacement
        /// </summary>
        YearReplacement
    }
    #endregion

    #region DATE INTERVAL
    /// <summary>
    /// Enum DateInterval
    /// </summary>
    public enum DateInterval
    {
        /// <summary>
        /// The year
        /// </summary>
        Year,
        /// <summary>
        /// The month
        /// </summary>
        Month,
        /// <summary>
        /// The weekday
        /// </summary>
        Weekday,
        /// <summary>
        /// The day
        /// </summary>
        Day,
        /// <summary>
        /// The hour
        /// </summary>
        Hour,
        /// <summary>
        /// The minute
        /// </summary>
        Minute,
        /// <summary>
        /// The second
        /// </summary>
        Second
    }
    #endregion

    #region HMDifference
    /// <summary>
    /// Enum HMDifference
    /// </summary>
    public enum HMDifference
    {
        /// <summary>
        /// As it is
        /// </summary>
        AsItIs,
        /// <summary>
        /// The positive
        /// </summary>
        Positive,
        /// <summary>
        /// The zero if negative
        /// </summary>
        ZeroIFNegative
    }
    #endregion

    #region RETURNDATE
    /// <summary>
    /// Enum ReturnDate
    /// </summary>
    public enum ReturnDate
    {
        /// <summary>
        /// The smaller
        /// </summary>
        Smaller,
        /// <summary>
        /// The bigger
        /// </summary>
        Bigger,
        /// <summary>
        /// The near match
        /// </summary>
        NearMatch
    }
    #endregion
#endif
}
