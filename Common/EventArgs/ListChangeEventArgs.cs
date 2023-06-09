using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public interface IListChangeEventArgs : IEventArgs
    {
        /// <summary>
        /// Gets the new index.
        /// </summary>
        /// <value>The new index.</value>
        int NewIndex { get; }
      
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }

        /// <summary>
        /// Gets the changed indices.
        /// </summary>
        /// <value>The changed indices.</value>
        IEnumerable<int> ChangedIndices { get; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        /// <value>The operation.</value>
        ListOperation Operation { get; }        
    }

    /// <summary>
    /// Class CollectionChangedEventArgs.
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class ListChangedEventArgs : EventArgs, IListChangeEventArgs
    {
        public int NewIndex { get; protected internal set; }
        public int Count { get; protected internal set; }
        public IEnumerable<int> ChangedIndices { get; protected internal set; }
        public ListOperation Operation { get; protected internal set; }

        public ListChangedEventArgs() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChangedEventArgs"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="newIndex">The new index.</param>
        /// <param name="count">The count.</param>
        /// <param name="indices">The indices.</param>
        public ListChangedEventArgs
            (ListOperation operation, int newIndex = -1, int count = 1,
            IEnumerable<int> indices = null)
        {
            NewIndex = newIndex;
            Operation = operation;
            ChangedIndices = indices;
            Count = count;
        }

        public void Reset(ListOperation operation, int newIndex = -1, int count = 1,
            IEnumerable<int> indices = null)
        {
            NewIndex = newIndex;
            Operation = operation;
            ChangedIndices = indices;
            Count = count;
        }
    }

}
