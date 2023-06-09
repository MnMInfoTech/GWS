/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if(GWS || Window)
namespace MnM.GWS
{
    #region CHAR-LIMIT
    public interface ICharLimit : IInLineParameter
    {
        /// <summary>
        /// Gets a number of how many characters can be allowed to display.
        /// 0 means all.
        /// </summary>
        int MaxCharDisplay { get; }
    }
    #endregion

    #region CHAR LIMIT
    public struct CharLimit : ICharLimit
    {
        #region VARIABLES
        readonly int MaxCharDisplay;
        public static CharLimit Empty = new CharLimit();
        #endregion

        #region CONSTRUCTORS
        public CharLimit(int maxCharDisplay)
        {
            MaxCharDisplay = maxCharDisplay;
        }
        #endregion

        #region PROPERTIES
        int ICharLimit.MaxCharDisplay => MaxCharDisplay;
        #endregion

        public override string ToString() =>
            MaxCharDisplay.ToString();
    }
    #endregion
}
#endif