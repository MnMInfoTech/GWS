/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if Collections || Texts
    #region ITEXTFORMATTER
    /// <summary>
    /// Structure TextFormatter.
    /// </summary>
    public interface ITextFormatter : ICloneable
    {
        /// <summary>
        /// Occurs when [before format].
        /// </summary>
        event CustomizeFormat BeforeFormat;

    #region properties
        /// <summary>
        /// Gets or sets the word splitter.
        /// </summary>
        /// <value>The word splitter.</value>
        string WordSplitter { get; set; }

        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the suffix.
        /// </summary>
        /// <value>The suffix.</value>
        string Suffix { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [no null as string].
        /// </summary>
        /// <value><c>true</c> if [no null as string]; otherwise, <c>false</c>.</value>
        bool NoNullAsString { get; set; }

        /// <summary>
        /// Gets or sets the type of the array format.
        /// </summary>
        /// <value>The type of the array format.</value>
        TextFormatType ArrayFormat { get; set; }

        /// <summary>
        /// Gets or sets the data member.
        /// </summary>
        /// <value>The data member.</value>
        string DataMember { get; set; }
    #endregion

        /// <summary>
        /// Initials the formatting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <returns>System.String.</returns>
        string InitialFormatting<T>(T element);
    }
    #endregion
#endif

#if Texts || Functions
    #region IRANGE
    public interface IRange : ISpan, IReadOnlyList
    {
    #region properties
        bool IsEmpty { get; }
        bool Backward { get; }
        bool Forward { get; }
    #endregion

    #region public methods
        void Set(int? start = null, int? end = null, bool raiseChangeEvent = true);
        bool Clear(bool refresh = true, int? start = null);
        void CopyFrom(ISpan range);
        bool SameAs(Tuple<int, int> r);
        bool SameAs(Lot<int, int> r);
        void CorrectLength(int listLength);
    #endregion
    }
    #endregion
#endif

#if Texts
    #region ISEARCHABLE
    public interface ISearchable
    {
    #region properties
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        string Text { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [match whole word].
        /// </summary>
        /// <value><c>true</c> if [match whole word]; otherwise, <c>false</c>.</value>
        bool MatchWholeWord { get; set; }

        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        /// <value>The comparison.</value>
        StringComparison Comparison { get; set; }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        /// <value>The start.</value>
        int Start { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; set; }

        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>The results.</value>
        IList<ISpan> Results { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        /// <value><c>true</c> if this instance is null; otherwise, <c>false</c>.</value>
        bool IsNull { get; }
    #endregion
    }
    #endregion

    #region IVALIDATEWORD
    public interface IValidatedWord
    {
        int Start { get; set; }
        int End { get; set; }

        string Input { get; set; }

        string Output { get; set; }
        int OutPutEnd { get; }
        bool Failure { get; set; }
        string Message { get; set; }
    }
    #endregion
#endif

#if (Texts && GWS && Advanced)
    public interface ICharPoint
    {
        IRectangle Bounds { get; }
        IRectangle RedrawBounds { get; }
        int Index { get; }
        CaretState State { get; }
        string Address { get; }
        IPoint UserPoint { get; }
        int X { get; }
        int Y { get; }
        int X1 { get; }
        int Y1 { get; }

        bool Forward { get; }
        bool Backward { get; }

        bool LeftAlign { get; }
        bool RightAlign { get; }

        bool KeyLeft { get; }
        bool KeyRight { get; }

        bool KeyUp { get; }
        bool KeyDn { get; }

        bool KeyPgUp { get; }
        bool KeyPgDn { get; }

        bool KeyHome { get; }
        bool KeyEnd { get; }

        bool ByMouse { get; }
        bool ByKey { get; }

        bool Selection { get; }

        bool SelectionClear { get; }

        bool MouseDrag { get; }
        bool MouseProxy { get; }
        bool MouseDirect { get; }

        bool HorizontalMove { get; }
        bool VerticalMove { get; }

        bool XForward { get; }
        bool XBackward { get; }

        bool YForward { get; }
        bool YBackward { get; }
    }
    public interface ITextMeasurement
    {
        IRectangle Bounds { get; set; }
        /// <summary>
        /// Gets or sets the text bounds.
        /// </summary>
        /// <value>The text bounds.</value>
        IRectangle TextBounds { get; set; }

        int CharIndex { get; set; }
        /// <summary>
        /// Gets or sets the index of the previous.
        /// </summary>
        /// <value>The index of the previous.</value>
        int PreviousIndex { get; set; }
        /// <summary>
        /// Gets or sets the character.
        /// </summary>
        /// <value>The character.</value>
        char Char { get; set; }
        /// <summary>
        /// Gets or sets the user point.
        /// </summary>
        /// <value>The user point.</value>
        IPoint UserPoint { get; set; }

        /// <summary>
        /// Copies from.
        /// </summary>
        /// <param name="tm">The tm.</param>
        void CopyFrom(ITextMeasurement tm);
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object Clone();

        void ResetBounds(IPoint p);
        /// <summary>
        /// Resets the bounds.
        /// </summary>
        /// <param name="lineX">The line x.</param>
        /// <param name="lineY">The line y.</param>
        void ResetBounds(int? lineX = null, int? lineY = null);
        /// <summary>
        /// Resets the empty bounds.
        /// </summary>
        /// <param name="rc">The rc.</param>
        void ResetEmptyBounds(IRectangle rc);
    }
#endif

}