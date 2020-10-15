/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MnM.GWS;

namespace MnM.GWS
{
    /// <summary>
    /// Class Strings.
    /// </summary>
    public static class Texts
    {
        #region consts
        public const char r = '\r';
        public const char n = '\n';
        public const char zero = '\0';
        public const char space = ' ';
        public const char underScore = '_';
        public const char comma = ',';
        public const char dot = '.';
        public const char semiColon = ';';
        public const char quote = ((char)34);

        /// <summary>
        /// The new line
        /// </summary>
        public static readonly string NewLine = System.Environment.NewLine;

        /// <summary>
        /// The quote
        /// </summary>
        public static readonly string Quote = ((char)34).ToString();

        /// <summary>
        /// The validation word breaker
        /// </summary>
        public static char[] ValidationWordBreaker = new char[] { comma, r };

        /// <summary>
        /// The selection word breaker
        /// </summary>
        public static readonly char[] SelectionWordBreaker = new char[]
            { space, comma, dot, semiColon, '-', underScore, r, n };

        /// <summary>
        /// The regex line splitter
        /// </summary>
        public static readonly string RegexLineSplitter = @"\r\n|\r|\n";

        /// <summary>
        /// The splitters
        /// </summary>
        public static readonly char[] Splitters = new char[] { r, n };

        /// <summary>
        /// The match word
        /// </summary>
        static string matchWord = @"\b{0}\b";

        /// <summary>
        /// The look ahead
        /// </summary>
        static string lookAhead = @"(?=.*\b{0}\b)";

        /// <summary>
        /// The find word
        /// </summary>
        static string findWord = @"([a-zA-Z0-9]+ ?)+?";

        /// <summary>
        /// The and lines
        /// </summary>
        static string andLines = @"^({0}).*[\r\n]*$";
        /// <summary>
        /// The or lines
        /// </summary>
        static string orLines = @"^.*({0}).*[\r\n]*$";

        /// <summary>
        /// The no case
        /// </summary>
        public const StringComparison noCase =
            StringComparison.InvariantCultureIgnoreCase;

        const string emailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
           + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
           + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        const string websitePattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";

        const string filePathPattern = @"^(?:[\w]\:|\\)(\\[a-z_\-\s0-9\.]+)+\.(txt|gif|pdf|doc|docx|xls|xlsx)$";
        #endregion

#if Collections || Texts
        #region TO TEXT
        /// <summary>
        /// To the text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <param name="format">The format.</param>
        /// <returns>System.String.</returns>
        public static string ToText<T>(this T element, ITextFormatter format)
        {
            bool isstring = (element is string);

            if (element is IEnumerable && !isstring)
            {
                TextFormatType ft;
                switch (format.ArrayFormat)
                {
                    case TextFormatType.VirtualTextBox:
                    case TextFormatType.TextBox:
                        ft = format.ArrayFormat;
                        break;
                    default:
                        ft = TextFormatType.Normal;
                        break;
                }
                ITextFormatter itemFormat = format.Clone() as ITextFormatter;
                itemFormat.ArrayFormat = ft;

                StringBuilder sb = new StringBuilder();
                IEnumerable elements = (IEnumerable)element;

                foreach (var item in elements)
                {
                    var isItemString = (item is string);

                    if (item is IEnumerable && !isItemString)
                        sb.Append(format.WordSplitter + ToText(item, format));
                    else
                        sb.Append(format.WordSplitter + finalformat(item, itemFormat, isItemString));
                }
                var len = (itemFormat.WordSplitter == null) ?
                    0 : itemFormat.WordSplitter.Length;

                if (sb.Length > 0)
                    sb.Remove(0, len);

                return finalformat(sb.ToString(), format, false);
            }
            else
                return finalformat(element, format, isstring);
        }
      
        /// <summary>
        /// Finalformats the specified format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element.</param>
        /// <param name="format">The format.</param>
        /// <param name="asstring">if set to <c>true</c> [asstring].</param>
        /// <returns>System.String.</returns>
        static string finalformat<T>(this T element, ITextFormatter format, bool asstring)
        {
            string prefix = "", suffix = "", s = "";

            bool asarray = true;
            try
            {
                s = format.InitialFormatting(element);
                if (asstring && !format.ArrayFormat.IsOneOf(TextFormatType.VirtualTextBox, TextFormatType.TextBox))
                {
                    prefix = Quote + format.Prefix;
                    suffix = format.Suffix + Quote;
                }
                else
                {
                    prefix = format.Prefix ?? "";
                    suffix = format.Suffix ?? "";
                    if (s.IndexOf(",") == -1)
                        asarray = false;
                }
                if (s.StartsWith(prefix) && s.EndsWith(suffix))
                {
                    prefix = ""; suffix = "";
                }

                switch (format.ArrayFormat)
                {
                    case TextFormatType.Array:
                        if (asarray) return "new[]{" + prefix + s + suffix + "}";
                        else return prefix + s + suffix;
                    case TextFormatType.Group:
                        if (asarray) return "{" + prefix + s + suffix + "}";
                        else return prefix + s + suffix;
                    case TextFormatType.Normal:
                        return prefix + s + suffix;
                    case TextFormatType.VirtualTextBox:
                        return prefix + s + suffix + r;
                    case TextFormatType.TextBox:
                        return prefix + s + suffix + r + n;
                    default:
                        return s;
                }
            }
            catch
            {
                return s;
            }

        }
        #endregion

        #region TO TEXT ARRAY
        /// <summary>
        /// To the text array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="format">The format.</param>
        /// <returns>System.String[].</returns>
        public static string[] ToTextArray<T>(this T elements, ITextFormatter format) where T : IEnumerable
        {
            bool isstring = (elements is string);

            var sb = new Collection<string>();

            if (!isstring)
            {
                foreach (var item in elements)
                {
                    if (item is IEnumerable && !(item is string))
                    {
                        sb.Add(ToText(item, format));
                    }
                    else
                    {
                        sb.Add(finalformat(item, format, (item is string)));
                    }
                }
            }
            else
            {
                sb.Add(finalformat(elements, format, isstring));
            }
            return sb.ToArray();
        }

        /// <summary>
        /// To the text array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The elements.</param>
        /// <param name="format">The format.</param>
        /// <param name="range">The range.</param>
        /// <returns>System.String[].</returns>
        public static string[] ToTextArray<T>(this T elements, ITextFormatter format, Span range)
            where T : IEnumerable
        {
            bool isstring = (elements is string);

            var r = new Span(range, elements.Count());

            var sb = new Collection<string>(r.Count);

            if (!isstring)
            {
                foreach (var i in r)
                {
                    var item = elements.GetValue(i);
                    if (item is IEnumerable && !(item is string))
                    {
                        sb.Add(ToText(item, format));
                    }
                    else
                    {
                        sb.Add(finalformat(item, format, (item is string)));
                    }
                }
            }
            else
            {
                string s = (elements as string).Substring(r.Start, r.Count);

                sb.Add(finalformat(s, format, isstring));
            }
            return sb.ToArray();
        }
        #endregion
#endif

#if Texts
        #region CONVERT CASE
        /// <summary>
        /// Converts the case.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="convertCase">The convert case.</param>
        /// <returns>System.String.</returns>
        public static string ConvertCase(this string text, CaseConversion convertCase = CaseConversion.Title)
        {
            if (text == null) return null;
            if (text.Length == 0) return text;
            switch (convertCase)
            {
                case CaseConversion.Sentence:
                    return text.Substring(0, 1).ToUpper() +
                        text.Substring(1).ToLower();
                case CaseConversion.Title:
                    TextInfo TextInfo =
                        CultureInfo.InvariantCulture.TextInfo;
                    return TextInfo.ToTitleCase
                        (text.Replace("_", "_ ")).Replace("_ ", "_");
                case CaseConversion.Upper:
                    return text.ToUpper();
                case CaseConversion.Lower:
                    return text.ToLower();
                case CaseConversion.Toggle:
                    char[] result = new char[text.Length];
                    for (int i = 0; i < text.Length; i++)
                    {
                        if (char.IsLetter(text[i]))
                        {
                            if (char.IsLower(text[i]))
                                result[i] = char.ToUpper(text[i]);
                            else result[i] = char.ToLower(text[i]);
                        }
                        else { result[i] = text[i]; }
                    }
                    return new string(result);
                default:
                    return text;
            }
        }

        /// <summary>
        /// Converts the case.
        /// </summary>
        /// <param name="e">The <see cref="IKeyPressEventArgs"/> instance containing the event data.</param>
        /// <param name="data">The data.</param>
        /// <param name="convertCase">The convert case.</param>
        public static void ConvertCase(this IKeyPressEventArgs e, StringBuilder data,
            CaseConversion convertCase = CaseConversion.Title)
        {
            if (data == null ||
                Convert.ToInt32(e.KeyChar) == 0 ||
                convertCase == CaseConversion.None)
                return;

            char nChar = Convert.ToChar(e.KeyChar);
            char mChar = nChar;

            switch (convertCase)
            {
                case CaseConversion.Lower:
                    mChar = char.ToLower(nChar);
                    break;
                case CaseConversion.Upper:
                    mChar = char.ToUpper(nChar);
                    break;
                case CaseConversion.Toggle:
                    if (char.IsLower(nChar))
                        mChar = char.ToUpper(nChar);
                    else
                        mChar = char.ToLower(nChar);
                    break;
                case CaseConversion.Title:
                    if (data.Length == 0)
                        mChar = char.ToUpper(nChar);
                    else
                    {
                        switch (data[data.Length - 1])
                        {
                            case space:
                            case underScore:
                            case r:
                            case n:
                            case dot:
                            case semiColon:
                                mChar = char.ToUpper(nChar);
                                break;
                            default:
                                mChar = char.ToLower(nChar);
                                break;
                        }
                    }
                    break;
                case CaseConversion.Sentence:
                    if (data.Length == 0)
                        mChar = char.ToUpper(nChar);
                    else
                        switch (data[data.Length - 1])
                        {
                            case r:
                            case n:
                            case dot:
                                mChar = char.ToUpper(nChar);
                                break;
                            case space:
                                if (data.Length > 1 && data[data.Length - 2] == dot)
                                    mChar = char.ToUpper(nChar);
                                else mChar = nChar;
                                break;
                            default:
                                mChar = char.ToLower(nChar);
                                break;
                        }
                    break;
            }
            e.KeyChar = mChar;
        }

        /// <summary>
        /// Converts the case.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="convertCase">The convert case.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public static void ConvertCase(this StringBuilder data,
            CaseConversion convertCase = CaseConversion.Title, int start = 0, int end = -1)
        {
            if (data == null ||
                convertCase == CaseConversion.None || data.Length == 0)
                return;

            bool found = false;
            if (end < 0) end = data.Length - 1;

            switch (convertCase)
            {
                #region sentence
                case CaseConversion.Sentence:
                    for (int i = start; i <= end; i++)
                    {
                        switch (data[i])
                        {
                            case r:
                            case n:
                            case dot:
                                found = true;
                                continue;
                            case space:
                                if (i > 0 && data[i - 1] == dot)
                                    found = true;
                                continue;
                            default:
                                if (found || i == 0)
                                    data[i] = char.ToUpper(data[i]);
                                found = false;
                                break;
                        }
                    }
                    break;
                #endregion
                #region title
                case CaseConversion.Title:
                    for (int i = start; i <= end; i++)
                    {
                        switch (data[i])
                        {
                            case space:
                            case underScore:
                            case r:
                            case n:
                            case dot:
                            case semiColon:
                                found = true;
                                continue;
                            default:
                                if (found) data[i] = char.ToUpper(data[i]);
                                found = false;
                                break;
                        }
                    }
                    break;
                #endregion
                #region upper
                case CaseConversion.Upper:
                    for (int i = start; i <= end; i++)
                    {
                        switch (data[i])
                        {
                            case space:
                            case underScore:
                            case r:
                            case n:
                                continue;
                            default:
                                data[i] = char.ToUpper(data[i]);
                                break;
                        }
                    }
                    break;
                #endregion
                #region lower
                case CaseConversion.Lower:
                    for (int i = start; i <= end; i++)
                    {
                        switch (data[i])
                        {
                            case space:
                            case underScore:
                            case r:
                            case n:
                                continue;
                            default:
                                data[i] = char.ToLower(data[i]);
                                break;
                        }
                    }
                    break;
                #endregion
                #region toggle
                case CaseConversion.Toggle:
                    for (int i = start; i <= end; i++)
                    {
                        switch (data[i])
                        {
                            case space:
                            case underScore:
                            case r:
                            case n:
                                continue;
                            default:
                                data[i] =
                                    char.IsLower(data[i]) ?
                                    char.ToUpper(data[i]) :
                                    char.ToLower(data[i]);
                                break;
                        }
                    }
                    break;
                #endregion
                default:
                    return;
            }
        }
        #endregion

        #region IS MATCHED
        /// <summary>
        /// Determines whether the specified search text is matched.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="count">The count.</param>
        /// <param name="compare">The compare.</param>
        /// <returns><c>true</c> if the specified search text is matched; otherwise, <c>false</c>.</returns>
        public static bool IsMatched(this string wholeText, string searchText,
            int startIndex, bool matchWholeWord, int count, StringComparison compare)
        {
            return wholeText.find(searchText, startIndex,
                matchWholeWord, count, compare) != null;
        }

        /// <summary>
        /// Determines whether the specified search text is matched.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="count">The count.</param>
        /// <param name="compare">The compare.</param>
        /// <returns><c>true</c> if the specified search text is matched; otherwise, <c>false</c>.</returns>
        public static bool IsMatched(this StringBuilder wholeText, string searchText,
            int startIndex, bool matchWholeWord, int count, StringComparison compare)
        {
            return wholeText.find(searchText, startIndex,
                matchWholeWord, count, compare) != null;
        }


        /// <summary>
        /// Determines whether the specified search is matched.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="search">The search.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns><c>true</c> if the specified search is matched; otherwise, <c>false</c>.</returns>
        public static bool IsMatched(this string wholeText, ISearchable search,
           int? start = null, int? count = null)
        {
            return wholeText.IsMatched
                (search.Text, start ?? search.Start,
                search.MatchWholeWord, count ?? search.Count, search.Comparison);
        }
        #endregion

        #region FIND - REPLACE
        public static string GetSearchCondition<T>(this IEnumerable<T> SearchList, bool forFilter = false) where T: ISearchable
        {
            if (SearchList == null ||
                !SearchList.Any()) return null;

            string searchText = null;

            foreach (var item in SearchList)
                searchText += "|(" + item.getPattern(forFilter) + ")";

            searchText = searchText.Remove(0, 1);
            return searchText;
        }

        #region find single
        /// <summary>
        /// Finds the specified search text.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="compare">The compare.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>Range.</returns>
        public static Range Find(this string wholeText, string searchText,
            bool matchWholeWord = false, StringComparison compare = noCase,
            int startIndex = 0, int count = -1)
        {
            return wholeText.find(searchText,
                    startIndex, matchWholeWord, count, compare);
        }

        /// <summary>
        /// Finds the specified search text.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="compare">The compare.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>Range.</returns>
        public static Range Find(this StringBuilder wholeText, string searchText,
            bool matchWholeWord = false, StringComparison compare = noCase,
            int startIndex = 0, int count = -1)
        {
            return wholeText.Find(searchText,
                matchWholeWord, compare, startIndex, count);
        }

        /// <summary>
        /// Finds the specified search.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="search">The search.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>Range.</returns>
        public static Range Find(this StringBuilder wholeText,
            ISearchable search, int? start = null, int? count = null)
        {
            return wholeText.Find
                (search.Text, search.MatchWholeWord, search.Comparison,
                start ?? search.Start, count ?? search.Count);
        }

        /// <summary>
        /// Finds the specified search.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="search">The search.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>Range.</returns>
        public static Range Find(this string wholeText, ISearchable search,
            int? start = null, int? count = null)
        {
            return wholeText.Find
                (search.Text, search.MatchWholeWord, search.Comparison,
                start ?? search.Start, count ?? search.Count);
        }

        /// <summary>
        /// Finds the specified search list.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchList">The search list.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>Range.</returns>
        public static Range Find<T>(this string wholeText, IEnumerable<T> searchList,
          int? start = null, int? count = null) where T : ISearchable
        {
            foreach (var search in searchList)
            {
                var range = wholeText.Find
                (search.Text, search.MatchWholeWord, search.Comparison,
                start ?? search.Start, count ?? search.Count);

                if (range != null) return range;
            }
            return null;
        }

        /// <summary>
        /// Finds the specified search text.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="count">The count.</param>
        /// <param name="compare">The compare.</param>
        /// <returns>Range.</returns>
        static Range find(this string wholeText, string searchText,
            int startIndex, bool matchWholeWord, int count, StringComparison compare)
        {
            if (string.IsNullOrEmpty(wholeText) ||
                string.IsNullOrEmpty(searchText) ||
                searchText.Length > wholeText.Length)
                return null;

            var target = wholeText.getSearchTargetText(startIndex, count);
            bool hasAnd = Regex.IsMatch(searchText, @"\s&\s");
            var pattern = searchText.getPattern(matchWholeWord, compare, hasAnd);

            if (hasAnd)
                target = target.Replace(r, n);

            var match = Regex.Match(target, pattern);
            if (!match.Success) return null;

            if (hasAnd)
            {
                var or = searchText.AndOR(AndOr.OR, matchWholeWord, compare.ignoreCase(), false);
                var m = Regex.Match(match.Value, or);
                if (m.Success)
                {
                    return new Range(m.Index + match.Index + startIndex,
                        m.Index + match.Index + startIndex + m.Length - 1);
                }
            }
            else
            {
                return new Range(match.Index + startIndex,
                    match.Index + startIndex + match.Length - 1);
            }
            return null;
        }

        /// <summary>
        /// Finds the specified search text.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="count">The count.</param>
        /// <param name="compare">The compare.</param>
        /// <returns>Range.</returns>
        static Range find(this StringBuilder wholeText, string searchText,
            int startIndex, bool matchWholeWord, int count, StringComparison compare)
        {
            if (wholeText.Length == 0 ||
                string.IsNullOrEmpty(searchText) ||
                searchText.Length > wholeText.Length)
                return null;

            var target = wholeText.getSearchTargetText(startIndex, count);
            bool hasAnd = Regex.IsMatch(searchText, @"\s&\s");
            var pattern = searchText.getPattern(matchWholeWord, compare, hasAnd);

            if (hasAnd)
                target = target.Replace(r, n);

            var match = Regex.Match(target, pattern);
            if (!match.Success) return null;

            if (hasAnd)
            {
                var or = searchText.AndOR(AndOr.OR, matchWholeWord, compare.ignoreCase(), false);
                var m = Regex.Match(match.Value, or);
                if (m.Success)
                {
                    return new Range(m.Index + match.Index + startIndex,
                        m.Index + match.Index + startIndex + m.Length - 1);
                }
            }
            else
            {
                return new Range(match.Index + startIndex,
                    match.Index + startIndex + match.Length - 1);
            }
            return null;
        }
        #endregion

        #region search all - core functions
        /// <summary>
        /// Searches all.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="count">The count.</param>
        /// <param name="compare">The compare.</param>
        /// <param name="filter">if set to <c>true</c> [filter].</param>
        /// <returns>IList&lt;IRange&gt;.</returns>
        static IList<ISpan> searchAll(this string wholeText, string searchText,
            int startIndex, bool matchWholeWord, int count, StringComparison compare, bool filter = false)
        {
            if (string.IsNullOrEmpty(wholeText) ||
                string.IsNullOrEmpty(searchText) ||
                searchText.Length > wholeText.Length)
                return null;

            var list = new Collection<ISpan>();

            var target = wholeText.getSearchTargetText(startIndex, count);

            bool hasAnd = Regex.IsMatch(searchText, @"\s&\s");
            var pattern = searchText.getPattern(matchWholeWord, compare, hasAnd || filter);

            if (hasAnd || filter)
                target = target.Replace(r, n);

            var matches = Regex.Matches(target, pattern).Cast<Match>();

            if (filter)
            {
                list.AddRange(matches.Select(m => new Range(m.Index, m.Index + m.Length - 1)));
            }
            else
            {
                if (hasAnd)
                {
                    var or = searchText.AndOR(AndOr.OR, matchWholeWord, compare.ignoreCase(), false);
                    foreach (var match in matches)
                    {
                        list.AddRange(Regex.Matches(match.Value, or).Cast<Match>().
                            Select(m => new Range(m.Index + match.Index +
                            startIndex, m.Index + match.Index + startIndex + m.Length - 1)));
                    }
                }
                else
                {
                    list.AddRange(matches.Select(m =>
                    new Range(m.Index + startIndex, m.Index + startIndex + m.Length - 1)));
                }
            }
            return list;
        }

        /// <summary>
        /// Searches all.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="count">The count.</param>
        /// <param name="compare">The compare.</param>
        /// <param name="filter">if set to <c>true</c> [filter].</param>
        /// <returns>IList&lt;IRange&gt;.</returns>
        static IList<ISpan> searchAll(this StringBuilder wholeText, string searchText,
            int startIndex, bool matchWholeWord, int count, StringComparison compare, bool filter = false)
        {
            var list = new Collection<ISpan>();

            var target = wholeText.getSearchTargetText(startIndex, count);

            bool hasAnd = Regex.IsMatch(searchText, @"\s&\s");
            var pattern = searchText.getPattern(matchWholeWord, compare, hasAnd || filter);

            if (hasAnd || filter)
                target = target.Replace(r, n);

            var matches = Regex.Matches(target, pattern).Cast<Match>();

            if (filter)
            {
                list.AddRange(matches.Select(m => new Range(m.Index, m.Index + m.Length - 1)));
            }
            else
            {
                if (hasAnd)
                {
                    var or = searchText.AndOR(AndOr.OR, matchWholeWord, compare.ignoreCase(), false);
                    foreach (var match in matches)
                    {
                        list.AddRange(Regex.Matches(match.Value, or).Cast<Match>().
                            Select(m => new Range(m.Index + match.Index +
                            startIndex, m.Index + match.Index + startIndex + m.Length - 1)));
                    }
                }
                else
                {
                    list.AddRange(matches.Select(m =>
                    new Range(m.Index + startIndex, m.Index + startIndex + m.Length - 1)));
                }
            }
            return list;
        }
        #endregion

        #region find all
        /// <summary>
        /// Finds all.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="compare">The compare.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>IList&lt;IRange&gt;.</returns>
        public static IList<ISpan> FindAll(this string wholeText, string searchText,
            bool matchWholeWord = false, StringComparison compare = noCase,
            int startIndex = 0, int count = -1)
        {
            return wholeText.searchAll(searchText,
                startIndex, matchWholeWord, count, compare);
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="compare">The compare.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>IList&lt;IRange&gt;.</returns>
        public static IList<ISpan> FindAll(this StringBuilder wholeText, string searchText,
            bool matchWholeWord = false, StringComparison compare = noCase,
            int startIndex = 0, int count = -1)
        {
            return wholeText.ToString().FindAll(searchText, matchWholeWord,
                compare, startIndex, count);
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="search">The search.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>IList&lt;IRange&gt;.</returns>
        public static IList<ISpan> FindAll(this StringBuilder wholeText,
            ISearchable search, int? start = null, int? count = null)
        {
            return wholeText.ToString().FindAll
                (search.Text, search.MatchWholeWord, search.Comparison,
                start ?? search.Start, count ?? search.Count);
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="search">The search.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>IList&lt;IRange&gt;.</returns>
        public static IList<ISpan> FindAll(this string wholeText, ISearchable search,
            int? start = null, int? count = null)
        {
            return wholeText.ToString().FindAll
                (search.Text, search.MatchWholeWord, search.Comparison,
                start ?? search.Start, count ?? search.Count);
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="search">The search.</param>
        /// <returns>IEnumerable&lt;IRange&gt;.</returns>
        #endregion

        #region filter all
        #region string
        /// <summary>
        /// Filters all.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="compare">The compare.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> FilterAll(this string wholeText, string searchText,
         bool matchWholeWord = false, StringComparison compare = noCase,
         int startIndex = 0, int count = -1)
        {
            return wholeText.searchAll(searchText,
                startIndex, matchWholeWord, count, compare, filter: true).
                Select(x => wholeText.Select(x));
        }

        /// <summary>
        /// Filters the alll.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="search">The search.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> FilterAlll(this string wholeText, ISearchable search,
            int? start = null, int? count = null)
        {
            return wholeText.ToString().searchAll
                (search.Text, start ?? search.Start, search.MatchWholeWord,
                count ?? search.Count, search.Comparison, filter: true).
                Select(x => wholeText.Select(x));
        }
        #endregion

        #region string builder
        /// <summary>
        /// Filters all.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="searchText">The search text.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="compare">The compare.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> FilterAll(this StringBuilder wholeText, string searchText,
            bool matchWholeWord = false, StringComparison compare = noCase,
            int startIndex = 0, int count = -1)
        {
            return wholeText.ToString().searchAll(searchText, startIndex,
                matchWholeWord, count, compare, filter: true).
                Select(x => wholeText.Select(x));
        }

        /// <summary>
        /// Filters all.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="search">The search.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public static IEnumerable<string> FilterAll(this StringBuilder wholeText,
            ISearchable search, int? start = null, int? count = null)
        {
            return wholeText.ToString().searchAll(search.Text, start ?? search.Start,
                search.MatchWholeWord, count ?? search.Count, search.Comparison, filter: true).
                Select(x => wholeText.Select(x));
        }
        #endregion

        #region text editor/ page

        /// <summary>
        /// Filters all.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
             #endregion
        #endregion

        #region replace all
        /// <summary>
        /// Replaces all.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="data">The data.</param>
        /// <param name="replaceText">The replace text.</param>
        public static void ReplaceAll(this IEnumerable<Range> list, ref StringBuilder data, string replaceText)
        {
            if (string.IsNullOrEmpty(replaceText)) replaceText = "";
            var items = list.OrderByDescending(x => x);
            foreach (var item in items)
            {
                if (item.Start >= data.Length || item.End > data.Length) continue;

                data.Remove(item.Start, item.Count + 1);
                data.Insert(item.Start, replaceText);
            }
        }

        /// <summary>
        /// Replaces all.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="data">The data.</param>
        /// <param name="replaceText">The replace text.</param>
        public static void ReplaceAll(this IEnumerable<Range> list, ref string data, string replaceText)
        {
            var sb = new StringBuilder(data);
            list.ReplaceAll(ref sb, replaceText);
            data = sb.ToString();
        }
        #endregion

        #region important search functions
        /// <summary>
        /// Ands the or.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="andOr">The and or.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="multiLine">if set to <c>true</c> [multi line].</param>
        /// <param name="forFilter">if set to <c>true</c> [for filter].</param>
        /// <returns>System.String.</returns>
        static string AndOR(this string search, AndOr andOr, bool matchWholeWord,
            bool ignoreCase = false, bool multiLine = false, bool forFilter = false)
        {
            if (string.IsNullOrEmpty(search)) return search;
            string text = search;
            string result = null;

            var wdFormat = (andOr == AndOr.OR ? matchWord : lookAhead);
            var lines = (andOr == AndOr.OR ? orLines : andLines);

            result += ignoreCase ? "(?i)" : "(?-i)";
            result += multiLine ? "(?m)" : "(?-m)";

            text = Regex.Replace(text, @"\s+[|]", "|");
            text = Regex.Replace(text, @"[|]+\s", "|");

            if (andOr == AndOr.OR)
                text = Regex.Replace(text, @"\s&\s", "|");
            else
                text = Regex.Replace(text, @"\s([&])\s", " ");

            if (matchWholeWord)
                text = Regex.Replace(text, findWord,
                    x => string.Format(wdFormat, x.Value.Trim()));
            if (forFilter)
            {
                result += string.Format(lines, text);
            }
            else
            {
                result += text;
            }
            return result;
        }

        /// <summary>
        /// Gets the search target text.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        static string getSearchTargetText(this string wholeText, int startIndex, int count)
        {
            string target;
            if (count > 0)
            {
                Enumerables.CorrectLength(ref startIndex, ref count, wholeText.Length);
                target = wholeText.Substring(startIndex, ref count);
            }
            else
            {
                target = wholeText.Substring(startIndex);
            }
            return target;
        }

        /// <summary>
        /// Gets the search target text.
        /// </summary>
        /// <param name="wholeText">The whole text.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        static string getSearchTargetText(this StringBuilder wholeText, int startIndex, int count)
        {
            string target;
            if (count > 0)
            {
                Enumerables.CorrectLength(ref startIndex, ref count, wholeText.Length);
                target = wholeText.Substring(startIndex, ref count);
            }
            else
            {
                target = wholeText.Substring(startIndex);
            }
            return target;
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <param name="searchText">The search text.</param>
        /// <param name="matchWholeWord">if set to <c>true</c> [match whole word].</param>
        /// <param name="compare">The compare.</param>
        /// <param name="forFilter">if set to <c>true</c> [for filter].</param>
        /// <returns>System.String.</returns>
        static string getPattern(this string searchText, bool matchWholeWord,
            StringComparison compare, bool forFilter = false)
        {
            string pattern = null;
            bool ignoreCase = compare.ignoreCase();

            if (Regex.IsMatch(searchText, @"\s&\s"))
                pattern = searchText.AndOR(AndOr.AND, matchWholeWord, ignoreCase: ignoreCase,
                    multiLine: true, forFilter: forFilter);
            else
                pattern = searchText.AndOR(AndOr.OR, matchWholeWord, ignoreCase: ignoreCase,
                    multiLine: true, forFilter: forFilter);

            return pattern;
        }

        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="forFilter">if set to <c>true</c> [for filter].</param>
        /// <returns>System.String.</returns>
        static string getPattern(this ISearchable search, bool forFilter = false)
        {
            return search.Text.getPattern(search.MatchWholeWord, search.Comparison, forFilter);
        }

        /// <summary>
        /// Ignores the case.
        /// </summary>
        /// <param name="compare">The compare.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        static bool ignoreCase(this StringComparison compare)
        {
            switch (compare)
            {
                case StringComparison.CurrentCultureIgnoreCase:
                case StringComparison.InvariantCultureIgnoreCase:
                    return true;
                default:
                    return false;
            }
        }

        //static string monoSearch(this string wholeText, int startIndex, int count)
        //{
        //    string result = null;
        //    var data = wholeText.getSearchTargetText(startIndex, count);
        //    var matches = Regex.Matches(data, RegexLineSplitter);
        //    var i = startIndex;
        //    foreach (Match match in matches)
        //    {
        //        var item = data.Select(i, firstCharIndex + match.Index - i);
        //        var measurement = hdc.charRectangle(item, container);
        //        lines.Add(new Span(charCount, measurement.CharIndex, item.Length));
        //        charCount = firstCharIndex + match.Index + match.Length;
        //        i = charCount;
        //    }
        //}

        //static IEnumerable<Range> monoFindAll(this string text, int textLocation, 
        //    string searchText, bool matchWholeWord, bool ignoreCase)
        //{

        //}
        #endregion
        #endregion

        #region SELECT / SUBSTRING
        /// <summary>
        /// Selects the specified range.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="range">The range.</param>
        /// <returns>System.String.</returns>
        public static string Select(this StringBuilder data, ISpan range, bool withLineFeed = false)
        {
            var t = Enumerables.CorrectLength(range, data.Length);
            if (withLineFeed)
                return string.Join("\r\n", Regex.Split(data.ToString(t.Item1, t.Item2), RegexLineSplitter));

            return data.ToString(t.Item1, t.Item2);
        }

        /// <summary>
        /// Selects the specified start.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>System.String.</returns>
        public static string Select(this StringBuilder data, int start, int end, bool withLineFeed = false)
        {
            Enumerables.CorrectIndex(ref start, ref end, data.Length);

            if (withLineFeed)
                return string.Join("\r\n", Regex.Split(data.ToString(start, end + 1), RegexLineSplitter));

            return data.ToString(start, end + 1);
        }

        /// <summary>
        /// Selects the specified start.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>System.String.</returns>
        public static string Select(this StringBuilder data, int start, ref int end, bool withLineFeed = false)
        {
            Enumerables.CorrectIndex(ref start, ref end, data.Length);
            if (withLineFeed)
                return string.Join("\r\n", Regex.Split(data.Select(start, end + 1), RegexLineSplitter));

            return data.Select(start, end + 1);
        }

        /// <summary>
        /// Selects the specified start.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>System.String.</returns>
        public static string Select(this string data, int start, ref int end, bool withLineFeed = false)
        {
            var t = Enumerables.CorrectLength(new Range(start, end), data.Length);
            if (withLineFeed)
                return string.Join("\r\n", Regex.Split(data.Substring(t.Item1, t.Item2), RegexLineSplitter));

            return data.Substring(t.Item1, t.Item2);
        }

        /// <summary>
        /// Selects the specified range.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="range">The range.</param>
        /// <returns>System.String.</returns>
        public static string Select(this string data, ISpan range, bool withLineFeed = false)
        {
            var t = Enumerables.CorrectLength(range, data.Length);
            if (withLineFeed)
                return string.Join("\r\n", Regex.Split(data.Substring(t.Item1, t.Item2), RegexLineSplitter));

            return data.Substring(t.Item1, t.Item2);
        }

        /// <summary>
        /// Substrings the specified index.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.String.</returns>
        public static string Substring(this StringBuilder data, int index, bool withLineFeed = false)
        {
            var length = data.Length;
            Enumerables.CorrectLength(ref index, ref length, data.Length);
            if (withLineFeed)
                return string.Join("\r\n", Regex.Split(data.ToString(index, length), RegexLineSplitter));

            return data.ToString(index, length);
        }

        /// <summary>
        /// Substrings the specified start.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public static string Substring(this StringBuilder data, int start, int count, bool withLineFeed = false)
        {
            Enumerables.CorrectLength(ref start, ref count, data.Length);
            if (withLineFeed)
                return string.Join("\r\n", Regex.Split(data.ToString(start, count), RegexLineSplitter));

            return data.ToString(start, count);
        }

        /// <summary>
        /// Substrings the specified start.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public static string Substring(this StringBuilder data, int start, ref int count, bool withLineFeed = false)
        {
            Enumerables.CorrectLength(ref start, ref count, data.Length);
            if (withLineFeed)
                return string.Join("\r\n", Regex.Split(data.ToString(start, count), RegexLineSplitter));
            return data.ToString(start, count);
        }

        /// <summary>
        /// Substrings the specified start.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>System.String.</returns>
        public static string Substring(this string data, int start, ref int count, bool withLineFeed = false)
        {
            Enumerables.CorrectLength(ref start, ref count, data.Length);
            if (withLineFeed)
                return string.Join("\r\n", Regex.Split(data.Substring(start, count), RegexLineSplitter));
            return data.Substring(start, count);
        }
        #endregion

        #region ISLINECHAR 
        public static bool IsLineChar(this char c) =>
            c == r || c == n;
        public static bool IsLineChar(this char? c) =>
            c != null && (c == r || c == n);

        public static string RemoveLineChar(this StringBuilder data)
        {
            return new Regex("(\r\n|\r|\n)").Replace(data.ToString(), "");
        }

        public static string RemoveLineChar(this StringBuilder data, int start, ref int count)
        {
            Enumerables.CorrectLength(ref start, ref count, data.Length);
            return new Regex("(\r\n|\r|\n)").Replace(data.ToString(start, count), "");
        }

        public static string RemoveLineChar(this StringBuilder data, int start, int count)
        {
            Enumerables.CorrectLength(ref start, ref count, data.Length);
            return new Regex("(\r\n|\r|\n)").Replace(data.ToString(start, count), "");
        }

        public static string RemoveLineChar(this string data)
        {
            return new Regex("(\r\n|\r|\n)").Replace(data, "");
        }

        public static string RemoveLineChar(this string data, int start, ref int count)
        {
            Enumerables.CorrectLength(ref start, ref count, data.Length);
            return new Regex("(\r\n|\r|\n)").Replace(data.Substring(start, count), "");
        }

        public static string RemoveLineChar(this string data, int start, int count)
        {
            Enumerables.CorrectLength(ref start, ref count, data.Length);
            return new Regex("(\r\n|\r|\n)").Replace(data.Substring(start, count), "");
        }

        /// <summary>
        /// Appends the line character.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>IEnumerable&lt;System.Char&gt;.</returns>
        public static IEnumerable<char> AppendLineChar(this string data)
        {
            if (string.IsNullOrEmpty(data)) return data;
            if (data[data.Length - 1] != r && data[data.Length - 1] != n)
                return Enumerables.AppendItem(data, r);
            return data;
        }
        #endregion

        #region WEB VALIDATION
        public static bool IsValidEmail(this string email)
        {
            Regex regex = new Regex(emailPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
        public static bool IsValidWebsite(this string url)
        {
            Regex regex = new Regex(websitePattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(url);
        }
        public static bool IsValidFilePath(this string path)
        {
            Regex regex = new Regex(filePathPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(path);
        }
        #endregion

        #region WORD SELECTION
        /// <summary>
        /// Selects the word.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="wordSplitter">The word splitter.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="forValidation">if set to <c>true</c> [for validation].</param>
        /// <returns>Range.</returns>
        public static Range SelectWord(this StringBuilder data, int index,
            char[] wordSplitter = null, WordSelection selection = WordSelection.Current,
            bool forValidation = false)
        {
            if (index < 0) return null;
            if (index >= data.Length) return null;

            if (wordSplitter == null || wordSplitter.Length == 0)
                wordSplitter = (forValidation) ? ValidationWordBreaker : SelectionWordBreaker;

            int start = 0;

            int i = data.ToString(0, index).LastIndexOfAny(wordSplitter);
            if (i != -1) start = i + 1;

            int end = start;
            int count = data.Length - start;
            if (count < 0) return null;

            i = data.ToString(start, count).IndexOfAny(wordSplitter);
            if (i != -1) end += i;
            else end = data.Length - 1;

            switch (selection)
            {
                case WordSelection.Current:
                default:
                    return new Range(start, end);
                case WordSelection.Previous:
                    start -= 1;
                    return data.SelectWord(start, wordSplitter, selection: WordSelection.Current);
                case WordSelection.Next:
                    end += 1;
                    return data.SelectWord(end, wordSplitter, selection: WordSelection.Current);
            }
        }

        /// <summary>
        /// Selects the word.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="result">The result.</param>
        /// <param name="wordSplitter">The word splitter.</param>
        /// <param name="intendedAsDate">if set to <c>true</c> [intended as date].</param>
        /// <param name="forValidation">if set to <c>true</c> [for validation].</param>
        public static void SelectWord<T>(this StringBuilder data, int index, out
            T result, char[] wordSplitter = null, bool intendedAsDate = false,
            bool forValidation = false) where T: IValidatedWord, new()
        {
            result = new T();
            if (index < 0) return;
            if (index >= data.Length) return;

            if (wordSplitter == null || wordSplitter.Length == 0)
                wordSplitter = (forValidation) ? ValidationWordBreaker : SelectionWordBreaker;

            int start = 0;
            int i = data.ToString(0, index + 1).LastIndexOfAny(wordSplitter);
            if (i != -1) start = i + 1;

            int end = start;
            int count = data.Length - start;
            if (count < 0) return;

            i = data.ToString(start, count).IndexOfAny(wordSplitter);

            if (i != -1) 
                end += i;
            else
                end = data.Length - 1;

            result.Start = start;
            result.End = end;
            result.Input = data.ToString(start, (end - start) + 1);

            if (intendedAsDate)
            {
                result.Output = new string( data.ToString(start, (end - start) + 1).Where(x =>
                        x == '0' ||
                        x == '1' ||
                        x == '2' ||
                        x == '3' ||
                        x == '4' ||
                        x == '5' ||
                        x == '6' ||
                        x == '7' ||
                        x == '8' ||
                        x == '9').ToArray());
            }
        }

        /// <summary>
        /// Selects the word.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="wordSplitter">The word splitter.</param>
        /// <param name="selection">The selection.</param>
        /// <param name="forValidation">if set to <c>true</c> [for validation].</param>
        /// <returns>Range.</returns>
        public static Range SelectWord(this string data, int index,
        char[] wordSplitter = null, WordSelection selection = WordSelection.Current, bool forValidation = false)
        {
            if (index < 0) return null;
            if (index >= data.Length) return null;

            if (wordSplitter == null || wordSplitter.Length == 0)
                wordSplitter = (forValidation) ? ValidationWordBreaker : SelectionWordBreaker;

            int start = 0;
            int i = data.Substring(0, index).LastIndexOfAny(wordSplitter);
            if (i != -1) start = i + 1;

            int end = start;
            int count = data.Length - start;
            if (count < 0) return null;

            i = data.Substring(start, count).IndexOfAny(wordSplitter);
            if (i != -1) end += i;
            else end = data.Length - 1;

            switch (selection)
            {
                case WordSelection.Current:
                default:
                    return new Range(start, end);
                case WordSelection.Previous:
                    start -= 1;
                    return data.SelectWord(start, wordSplitter, selection: WordSelection.Current);
                case WordSelection.Next:
                    end += 1;
                    return data.SelectWord(end, wordSplitter, selection: WordSelection.Current);
            }
        }

        /// <summary>
        /// Selects the word.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="index">The index.</param>
        /// <param name="result">The result.</param>
        /// <param name="wordSplitter">The word splitter.</param>
        /// <param name="intendedAsDate">if set to <c>true</c> [intended as date].</param>
        /// <param name="forValidation">if set to <c>true</c> [for validation].</param>
        public static void SelectWord<T>(this string data, int index, out
            T result, char[] wordSplitter = null, bool intendedAsDate = false,
            bool forValidation = false) where T:IValidatedWord, new()
        {
            result = new T();
            if (index < 0) return;
            if (index >= data.Length) return;

            if (wordSplitter == null || wordSplitter.Length == 0)
                wordSplitter = (forValidation) ? ValidationWordBreaker : SelectionWordBreaker;

            int start = 0;
            int i = data.Substring(0, index).LastIndexOfAny(wordSplitter);
            if (i != -1) start = i + 1;

            int end = start;
            int count = data.Length - start;
            if (count < 0) return;

            i = data.Substring(start, count).IndexOfAny(wordSplitter);
            if (i != -1) 
                end += i;
            else end =
                    data.Length - 1;

            result.Start = start;
            result.End = end;
            result.Input = data.Substring(start, (end - start) + 1);

            if (intendedAsDate)
            {
                result.Output = new string(data.Substring(start, (end - start) + 1).
                    Where(x => (x >= 48 && x <= 57)).ToArray());
            }
        }
        #endregion
#endif
    }
}
