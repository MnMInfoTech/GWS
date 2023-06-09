/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region ISPAN<T>
    /// <summary>
    /// Interface ISpan
    /// </summary>
    public interface ISpan<T> : IIndexer<T>
    {
        /// <summary>
        /// Gets start of this span.
        /// </summary>
        T Start { get; }

        /// <summary>
        /// Gets end of this span.
        /// </summary>
        T End { get; }
    }
    #endregion

    #region ISPAN
    /// <summary>
    /// Interface ISpan
    /// </summary>
    public interface ISpan : ISpan<int>, IValid
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Contains(int item);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Contains(ISpan range);
    }
    #endregion

    #region SPAN-HOLDER
    /// <summary>
    /// Represents an object with portion of items of some collection.
    /// </summary>
    public interface ISpanHolder
    {
        /// <summary>
        /// Gets a span to indicate start and end of portion of items of the collection.
        /// </summary>
        Span Span { get; }
    }
    #endregion

    /// <summary>
    /// Struct Span
    /// </summary>
    public struct Span : ISpan, IComparable, IEquatable<Span>, IComparable<Span>, IValid 
    {
        #region VARIABLES
        int start, length;
        int? lineLength;
        bool lineBreaker;
        public static Span Empty = new Span();
        #endregion

        #region CONSTRUCTORS
        public Span(int start, int length, int? lineLength = null, bool lineBreaker = false)
        {
            GWSEnumerable.CorrectLength(ref start, ref length);
            this.start = start;
            this.length = length;
            this.lineLength = lineLength;
            this.lineBreaker = lineBreaker;
        }
        public Span(int start, int length, int listLength, int? lineLength = null, bool lineBreaker = false)
        {
            GWSEnumerable.CorrectLength(ref start, ref length, listLength);
            this.start = start;
            this.length = length;
            this.lineLength = lineLength;
            this.lineBreaker = lineBreaker;
        }
        public Span(int count, int? lineLength = null, bool lineBreaker = false)
        {
            this.start = 0;
            this.length = count;
            this.lineLength = lineLength;
            this.lineBreaker = lineBreaker;
        }
        public Span(ISpan range, int length, int? lineLength = null, bool lineBreaker = false)
        {
            var tuple = GWSEnumerable.CorrectLength(range, length);
            this.start = tuple.Item1;
            this.length = tuple.Item2;
            this.lineLength = lineLength;
            this.lineBreaker = lineBreaker;
        }
        public Span(ISpan range, int? lineLength = null, bool lineBreaker = false)
        {
            var tuple = GWSEnumerable.CorrectLength(range);
            this.start = tuple.Item1;
            this.length = tuple.Item2;
            this.lineLength = lineLength;
            this.lineBreaker = lineBreaker;
        }
        #endregion

        #region PROPERTIES
        public int Start
        {
            get => start;
            set => start = value;
        }
        public int Count
        {
            get => length;
            set => length = value;
        }
        public int End
        {
            get => start + length - 1;
            set
            {
                if (value >= start)
                    length = value - start + 1;
            }
        }
        public int this[int index]
        {
            get
            {
                if (index >= Count)
                    throw new IndexOutOfRangeException();
                return start + index;
            }
        }
        public int? LineLength
        {
            get => lineLength;
            set => lineLength = value;
        }
        public bool LineBreaker
        {
            get => lineBreaker;
            set => lineBreaker = value;
        }
        public bool Valid => length > 0;
        #endregion

        #region PREVIOUS
        public Span Previous()
        {
            int start = this.start - length;
            if (start < 0) start = 0;
            return new Span(start, length);
        }
        public Span Previous(int count)
        {
            int start = this.start - count;
            if (start < 0) start = 0;
            return new Span(start, count);
        }
        #endregion

        #region NEXT
        public Span Next() =>
            new Span(End + 1, length);
        public Span Next(int count) =>
            new Span(this.End + 1, count);
        #endregion

        #region CONTAINS
        public bool Contains(int value) =>
            value >= start && value <= start + length - 1;
        public bool Contains(ISpan range) =>
            Contains(range.Start) &&
                 Contains(range.End);
        #endregion

        #region TO ARRAY
        public int[] ToArray() =>
            System.Linq.Enumerable.Range(start, length).ToArray();

        #endregion

        #region SAMES
        public bool SameAs(Lot<int, int> r)
        {
            return ((r.Item1 == this.Start &&
                r.Item2 == this.End) ||
                (r.Item2 == this.Start &&
                r.Item1 == this.End)
                );
        }
        #endregion

        #region CLEAR
        public bool Clear(bool refresh = true, int? startIndex = null)
        {
            start = startIndex ?? start;
            length = 0;
            return true;
        }
        #endregion

        #region CORRECT LENGTH
        public void CorrectLength(int listLength)
        {
            if (start + length - 1 < listLength) return;
            GWSEnumerable.CorrectLength(this, listLength);
        }
        #endregion

        #region EQUALITY
        public override bool Equals(object obj)
        {
            if (obj is Span)
            {
                Span r = (Span)obj;
                return r.start == this.start && r.End == this.End;
            }
            return false;
        }
        public override int GetHashCode() =>
            start ^ length;

        bool IEquatable<Span>.Equals(Span other) =>
            this == other;
        int IComparable<Span>.CompareTo(Span other) =>
            CompareTo(other);
        int IComparable.CompareTo(object obj)
        {
            if (obj is Span)
                return this.CompareTo((Span)obj);
            return -1;
        }
        int CompareTo(Span other)
        {
            if (this == other)
                return 0;
            else if (this > other)
                return 1;
            return -1;
        }
        #endregion

        #region OPERATORS
        public static bool operator ==(Span a, Span b) =>
            (a.start == b.start && a.length == b.length);
        public static bool operator !=(Span a, Span b) =>
            (a.start != b.start && a.length != b.length);
        public static bool operator >(Span a, Span b)
        {
            if (a.start > b.start)
            {
                if (a.length >= b.length)
                    return true;
            }
            else if (a.start == b.start && a.length > b.length)
                return true;
            return false;
        }
        public static bool operator >=(Span a, Span b) =>
            !(a < b);
        public static bool operator <=(Span a, Span b) =>
            !(a > b);
        public static bool operator <(Span a, Span b)
        {
            if (a.start < b.start)
            {
                if (a.length <= b.length)
                    return true;
            }
            else if (a.start == b.start && a.length < b.length)
                return true;
            return false;
        }
        public static implicit operator bool(Span a) =>
            a.length > 0;
        #endregion

        public override string ToString() =>
    "Start: " + start + ", End: " + End + " Length: " + length.ToString();
    }
}
