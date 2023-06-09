/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    #region IBEHAVIOR
    public interface IBehavior
    {
        /// <summary>
        /// Gets or sets a value indicating whether [automatic group].
        /// </summary>
        /// <value><c>true</c> if [automatic group]; otherwise, <c>false</c>.</value>
        bool AutoGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Behavior"/> is hosting.
        /// </summary>
        /// <value><c>true</c> if hosting; otherwise, <c>false</c>.</value>
        bool Hosting { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        int Size { get; set; }
    }
    #endregion

    #region IFXPARAMS
    public interface IFxParam
    {
        int Index { get; }
        int SectionID { get; }
        string Key { get; }
        object Value { get; }
        bool Replace { get; }
    }
    #endregion

    #region IGROUP
    /// <summary>
    /// Interface IInvoker
    /// </summary>
    public interface IGroup
    {
        /// <summary>
        /// Gets the series.
        /// </summary>
        /// <value>The series.</value>
        ISeries Series { get; }

        /// <summary>
        /// Gets the limit.
        /// </summary>
        /// <value>The limit.</value>
        ILimit Limit { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IGroup"/> is circular.
        /// </summary>
        /// <value><c>true</c> if circular; otherwise, <c>false</c>.</value>
        bool Circular { get; }
    }
    #endregion

    #region ISERIES
    public partial interface ISeries : IReadOnlyList<ISpan>, IGroup
    {
        #region PROPERTIES
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        IReadOnlyList<int> Items { get; }

        int Cursor { get; }

        bool AsAdditional { get; }
        #endregion

        #region ACCEPTS
        /// <summary>
        /// Acceptses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Accepts(ref int value);

        /// <summary>
        /// Acceptses the specified limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Accepts(ILimit limit);
        #endregion

        #region RESET
        /// <summary>
        /// Resets this instance.
        /// </summary>
        void Reset();
        #endregion

        #region UPDATE
        /// <summary>
        /// Updates the specified force.
        /// </summary>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <returns>Span.</returns>
        ISpan Update(bool force);
        #endregion

        #region OCCUPATION
        /// <summary>
        /// Occupations the specified upto.
        /// </summary>
        /// <param name="upto">The upto.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.Int32.</returns>
        int Occupation(int upto, out int start, out int count);
        #endregion
    }
    #endregion

    #region IFUNCTION
    public partial interface IFunction: IReadOnlyList, ICloneable
    {
        event VoidMethod BeforeParse;
        event VoidMethod AfterParse;

        #region properties
        IBehavior Behavior { get; }

        IList<IFunction> Children { get; }

        IReadOnlyList<ISeries> SeriesList { get; }

        IList Input { get; }

        IReadOnlyList Arguments { get; }

        IReadOnlyList Results { get; }

        IReadOnlyList MathStream { get; }

        bool CloningIncludeStructure { get; }

        bool CloningIncludeValues { get; }
        #endregion

        #region series change
        bool AddGroup(int group);
        #endregion

        #region casting
        Any CastAs<Any>(bool includeMeAsChildFunction = false) where Any : IFunction;

        Any CastAs<Any>(ISpan range) where Any : IFunction;
        #endregion

        #region math
        Any Math<Any>(MathOperator mop, object b, Operand operand) where Any : IFunction;

        Any Math<Any>(MathOperator mop, object b) where Any : IFunction;
        #endregion

        #region parameter handling
        void AddParameter(int sectionID, int index, object value, bool replace = false);

        IEnumerable<IFxParam> GetParameters(int sectionID);

        IEnumerable<IFxParam> GetParameters();

        IFxParam GetParameter(int sectionID, int index);

        void RemoveParameters(int sectionID);

        void RemoveParameter(int sectionID, int index);

        void RemoveParameters();
        #endregion

        #region parse, merge, reset, update, ppend, flush, compute
        void Append(params IFunction[] parsers);

        void Compute(object Argument, ref object result);

        void Flush();

        void Merge(params IFunction[] parsers);

        bool Parse(params object[] objects);

        void Accumulate(params object[] objects);

        void Reset();

        void Update();
        #endregion

        #region expression
        string Expression();
        #endregion
    }
    #endregion
}
