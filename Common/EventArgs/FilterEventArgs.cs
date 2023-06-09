using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public interface IFilterEventArgs
    {
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        FilterStatus Status { get; }

        /// <summary>
        /// Gets the status time.
        /// </summary>
        /// <value>The status time.</value>
        DateTime StatusTime { get; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>The duration.</value>
        long Duration { get; }

        string Message { get; set;}
    }

    /// <summary>
    /// Class FilterEventArgs.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class FilterEventArgs : EventArgs, IFilterEventArgs
    {
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        public FilterStatus Status { get; protected internal set; }

        /// <summary>
        /// Gets the status time.
        /// </summary>
        /// <value>The status time.</value>
        public DateTime StatusTime { get; protected internal set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>The duration.</value>
        public long Duration { get; protected internal set; }

        public string Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterEventArgs"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="statusTime">The status time.</param>
        /// <param name="duration">The duration.</param>
        public FilterEventArgs(FilterStatus status = FilterStatus.None,
            DateTime? statusTime = null, long duration = 0)
        {
            Status = status;
            StatusTime = statusTime ?? DateTime.Now;
            Duration = duration;
        }

        protected internal void Reset(FilterStatus status = FilterStatus.None,
            DateTime? statusTime = null, long duration = 0)
        {
            Status = status;
            StatusTime = statusTime ?? DateTime.Now;
            Duration = duration;
        }
    }

}
