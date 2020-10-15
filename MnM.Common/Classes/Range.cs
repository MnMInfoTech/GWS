/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if Texts || Functions
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class Range : IRange
    {
    #region variables
        [field: NonSerialized]
        protected int start;

        [field: NonSerialized]
        protected int? end;

        [field: NonSerialized]
        public event EventHandler<EventArgs> SelectionChanged;
    #endregion

    #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class.
        /// </summary>
        public Range() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public Range(int start, int end)
        {
            Enumerables.CorrectIndex(ref start, ref end);
            this.start = start;
            this.end = end;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="length">The length.</param>
        public Range(ISpan range, int length)
        {
            var tuple = Enumerables.CorrectIndex(range, length);
            this.start = tuple.Item1;
            this.end = tuple.Item2;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class.
        /// </summary>
        /// <param name="range">The range.</param>
        public Range(ISpan range)
        {
            var tuple = Enumerables.CorrectIndex(range);
            this.start = tuple.Item1;
            this.end = tuple.Item2;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="listLength">Length of the list.</param>
        public Range(int start, int end, int listLength)
        {
            Enumerables.CorrectIndex(ref start, ref end, listLength);
            this.start = start;
            this.end = end;
        }
    #endregion

    #region properties
        public virtual int Start
        {
            get => Math.Min(start, end ?? start);
            set
            {
                if (start == value)
                    return;
                Set(start: value);
            }
        }
        public virtual int End
        {
            get
            {
                if (end == null)
                    return start;
                return Math.Max(start, end.Value);
            }
            set
            {
                if (end == value)
                    return;
                Set(end: value);
            }
        }
        public virtual int Count
        {
            get
            {
                if (end == null)
                    return 0;
                return Math.Abs(end.Value - start) + 1;
            }
            set
            {
                if (end == (start + value))
                    return;
                Set(end: start + value);
            }
        }
        public int this[int index]
        {
            get
            {
                if (index > Count)
                    throw new IndexOutOfRangeException();
                return Start + index;
            }
        }
        public bool IsEmpty =>
            end == null;
        public bool Backward =>
          end != null && end < start;
        public bool Forward =>
          end != null && end > start;
    #endregion

    #region protected/internal methods
        protected internal virtual void OnSelectionChanged(EventArgs e) =>
            SelectionChanged?.Invoke(this, e);
        protected virtual void OnClear() { }
    #endregion

    #region public methods
        public void Set(int? start = null, int? end = null, bool raiseChangeEvent = true)
        {
            this.start = start ?? this.start;
            this.end = end ?? this.end;
            if (raiseChangeEvent)
                OnSelectionChanged(EventArgs.Empty);
        }
        public bool Clear(bool refresh = true, int? start = null)
        {
            bool result = end != null;
            this.start = start ?? Start;
            end = null;
            OnClear();
            if (refresh)
                OnSelectionChanged(EventArgs.Empty);
            return result;
        }

        public void CopyFrom(ISpan range)
        {
            start = range.Start;
            end = range.End;
        }
        public bool Contains(int value) =>
            end != null &&
            value >= Start && value <= End;

        public bool Contains(ISpan range) =>
            Contains(range.Start) &&
                 Contains(range.End);
        public bool SameAs(Tuple<int, int> r)
        {
            if (r == null)
                return false;
            return ((r.Item1 == this.Start &&
                r.Item2 == this.end) ||
                (r.Item2 == this.Start &&
                r.Item1 == this.end)
                );
        }
        public bool SameAs(Lot<int, int> r)
        {
            return ((r.Item1 == this.Start &&
                r.Item2 == this.end) ||
                (r.Item2 == this.Start &&
                r.Item1 == this.end)
                );
        }
        public void CorrectLength(int listLength)
        {
            if (end < listLength)
                return;
            Enumerables.CorrectIndex(this, listLength);
        }
        public override string ToString() =>
            Start + "-" + End;

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = Start; i <= End; i++)
                yield return i;
        }
        object IReadOnlyList.this[int index] =>
            this[index];
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    #endregion

    #region static members
        public static Range Empty
        {
            get
            {
                if (empty == null)
                    empty = new Range();
                return empty;
            }
        }
        static Range empty;
    #endregion
    }
#endif
}
