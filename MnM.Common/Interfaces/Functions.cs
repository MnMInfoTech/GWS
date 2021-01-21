/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if Functions

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

#region ILIMIT
    public interface ILimit : IGroup
    {
#region PROPERTIES
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        string ID { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Limit"/> is protected.
        /// </summary>
        /// <value><c>true</c> if protected; otherwise, <c>false</c>.</value>
        bool Protected { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Limit"/> is equal.
        /// </summary>
        /// <value><c>true</c> if equal; otherwise, <c>false</c>.</value>
        bool Equal { get; }

        /// <summary>
        /// Gets the lower.
        /// </summary>
        /// <value>The lower.</value>
        int Lower { get; }

        /// <summary>
        /// Gets the upper.
        /// </summary>
        /// <value>The upper.</value>
        int Upper { get; }

        /// <summary>
        /// Gets the restrictions.
        /// </summary>
        /// <value>The restrictions.</value>
        Collection<int> Restrictions { get; }
#endregion

#region ALLOWS
        /// <summary>
        /// Allowses the specified value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Allows(int Value);

        /// <summary>
        /// Allowses the specified value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Allows(ref int Value);

        /// <summary>
        /// Restrictses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Restricts(int value);

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
        bool Contains(int value);
#endregion
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

#region IFUNCTION
    public interface IFunction : IReadOnlyList, ICloneable
    {
#region events
        event VoidMethod BeforeParse;
        event VoidMethod AfterParse;
#endregion

#region properties
        IBehavior Behavior { get; }

        IList<IFunction> Children { get; }

        IReadOnlyList<ISeries> SeriesList { get; }

        FxEntity CompitibleCloningEntity(ICloneType other);

        IList Input { get; }

        IReadOnlyList Arguments { get; }

        IReadOnlyList Results { get; }

        IReadOnlyList MathStream { get; }

        bool CloningIncludeStructure { get; }

        bool CloningIncludeValues { get; }
#endregion

#region series change
        bool AddGroup(int group);

        SeriesChange SetSeries(int count);

        SeriesChange SetSeries<G>(int start, IEnumerable<G> invokers) where G : IGroup;

        SeriesChange SetSeries<G>(int start, params G[] invokers) where G : IGroup;

        SeriesChange SetSeries<G>(params G[] invokers) where G : IGroup;

        SeriesChange SetSeries<G>(IEnumerable<G> invokers) where G : IGroup;
#endregion

#region casting
        Any CastAs<Any>(ICloneType replication) where Any : IFunction;

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
        string Expression(ITextFormatter format = null);

        string[] Statements(ITextFormatter argFormat = null, ITextFormatter resultFormat = null);

        string[] TextArray(ITextFormatter argFormat = null);
#endregion
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
    public interface ISeries : IReadOnlyList<ISpan>, IGroup
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

#region ADD
        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Add(ref int value);

        /// <summary>
        /// Adds the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Add(params int[] values);

        /// <summary>
        /// Adds the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Add(IGroup source);

        /// <summary>
        /// Add_s the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        void Add(int value);
#endregion

#region CHANGE
        /// <summary>
        /// Changes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Change(IGroup source);

        /// <summary>
        /// Changes the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Change(params int[] values);
#endregion

#region REMOVE
        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        void Remove(int index);

        /// <summary>
        /// Removes the last.
        /// </summary>
        void RemoveLast();
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

#region ICLONETYPE
    public interface ICloneType : ICloneable
    {
        /// <summary>
        /// Gets to clone.
        /// </summary>
        /// <value>To clone.</value>
        Cloning ToClone { get; }

        /// <summary>
        /// Gets the offers.
        /// </summary>
        /// <value>The offers.</value>
        FxEntity Offers { get; }

        /// <summary>
        /// Gets the accepts.
        /// </summary>
        /// <value>The accepts.</value>
        FxEntity Accepts { get; }

        /// <summary>
        /// Gets a value indicating whether [include structure].
        /// </summary>
        /// <value><c>true</c> if [include structure]; otherwise, <c>false</c>.</value>
        bool IncludeStructure { get; }

        /// <summary>
        /// Gets a value indicating whether [include values].
        /// </summary>
        /// <value><c>true</c> if [include values]; otherwise, <c>false</c>.</value>
        bool IncludeValues { get; }

        /// <summary>
        /// Hybrids the specified replicates.
        /// </summary>
        /// <param name="replicates">The replicates.</param>
        /// <returns>CloneType.</returns>
        ICloneType Hybrid(Cloning replicates);

        /// <summary>
        /// Hybrids the specified offers.
        /// </summary>
        /// <param name="offers">The offers.</param>
        /// <param name="accepts">The accepts.</param>
        /// <returns>CloneType.</returns>
        ICloneType Hybrid(FxEntity offers, FxEntity accepts);

        /// <summary>
        /// Matches the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>EntityType.</returns>
        FxEntity Match(ICloneType other);

        /// <summary>
        /// Matches the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns>EntityType.</returns>
        FxEntity Compitible(ICloneType other);

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>CloneType.</returns>
        new ICloneType Clone();
    }
#endregion
}
#endif
