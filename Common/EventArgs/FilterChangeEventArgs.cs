// ***********************************************************************
// Assembly         : MnM
// Author           : Mukesh Adhvaryu
// Created          : 04-17-2016
//
// Last Modified By : Mukesh Adhvaryu
// Last Modified On : 05-24-2016
// ***********************************************************************
// <copyright file="EventArgs-Delegates.cs" company="M&M Info-Tech Ltd. London, United Kingdom">
//     M&M INFO-TECH UK LTD 2016
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace MnM.GWS
{
    public interface IFilterChangeEventArgs : IEventArgs
    {
        /// <summary>
        /// Gets a value indicating whether [stop after operation].
        /// </summary>
        /// <value><c>true</c> if [stop after operation]; otherwise, <c>false</c>.</value>
        bool StopAfterOperation { get;  }
    }

    /// <summary>
    /// Class FilterChangeEventArgs.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class FilterChangeEventArgs : EventArgs, IFilterChangeEventArgs
    {
        /// <summary>
        /// Gets a value indicating whether [stop after operation].
        /// </summary>
        /// <value><c>true</c> if [stop after operation]; otherwise, <c>false</c>.</value>
        public bool StopAfterOperation { get; private set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterChangeEventArgs"/> class.
        /// </summary>
        /// <param name="stopAfterOperation">if set to <c>true</c> [stop after operation].</param>
        public FilterChangeEventArgs(bool stopAfterOperation)
        {
            this.StopAfterOperation = stopAfterOperation;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FilterChangeEventArgs"/> class.
        /// </summary>
        public FilterChangeEventArgs()
        {

        }

        /// <summary>
        /// Gets the empty.
        /// </summary>
        /// <value>The empty.</value>
        public new static FilterChangeEventArgs Empty
        {
            get { return new FilterChangeEventArgs(); }
        }
    }

    /// <summary>
    /// Class AutoValueAddEventsArgs.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class AutoValueAddEventsArgs : EventArgs
    {
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public string[] Values { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoValueAddEventsArgs"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public AutoValueAddEventsArgs(string[] values)
        {
            this.Values = values;
        }
    }

}

