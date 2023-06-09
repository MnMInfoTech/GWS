using System;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region ILIMIT
    public interface ILimit : IGroup
    {
        #region PROPERTIES
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
        IReadOnlyList<int> Restrictions { get; }
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

    public struct Limit : ILimit 
    {
        #region Variables
        /// <summary>
        /// The protect
        /// </summary>
        readonly bool protect;

        /// <summary>
        /// The restrictions
        /// </summary>
        readonly int[] restrictions;

        /// <summary>
        /// The lower
        /// </summary>
        readonly int lower, upper;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Limit"/> struct.
        /// </summary>
        /// <param name="limit">The limit.</param>
        public Limit(ILimit limit)
        {
            lower = limit.Lower;
            upper = limit.Upper;
            protect = limit.Protected;
            restrictions = limit.Restrictions?.ToArray();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Limit"/> struct.
        /// </summary>
        /// <param name="constant">The constant.</param>
        /// <param name="protect">if set to <c>true</c> [protect].</param>
        public Limit(int constant, bool protect = false)
        {
            protect = false;
            restrictions = null;
            lower = constant;
            upper = constant;
            this.protect = protect;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Limit"/> struct.
        /// </summary>
        /// <param name="lower">The lower.</param>
        /// <param name="upper">The upper.</param>
        /// <param name="protect">if set to <c>true</c> [protect].</param>
        /// <param name="restrictions">The restrictions.</param>
        public Limit(int lower, int upper, bool protect = false, params int[] restrictions)
        {
            this.protect = protect;
            this.lower = System.Math.Min(lower, upper);
            this.upper = System.Math.Max(lower, upper);

            if (restrictions.Length > 0)
                this.restrictions = restrictions.ToArray();
            else this.restrictions = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Limit"/> struct.
        /// </summary>
        /// <param name="protect">if set to <c>true</c> [protect].</param>
        public Limit(bool protect)
            : this(0, protect)
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="Limit"/> struct.
        /// </summary>
        /// <param name="lower">The lower.</param>
        /// <param name="upper">The upper.</param>
        /// <param name="restrictions">The restrictions.</param>
        public Limit(int lower, int upper, params int[] restrictions)
            : this(lower, upper, false, restrictions)
        { }
        #endregion

        #region properties
        /// <summary>
        /// Gets a value indicating whether this <see cref="Limit"/> is protected.
        /// </summary>
        /// <value><c>true</c> if protected; otherwise, <c>false</c>.</value>
        public bool Protected => protect;  

        /// <summary>
        /// Gets a value indicating whether this <see cref="Limit"/> is equal.
        /// </summary>
        /// <value><c>true</c> if equal; otherwise, <c>false</c>.</value>
        public bool Equal => (lower == upper && lower > 1);  

        /// <summary>
        /// Gets the lower.
        /// </summary>
        /// <value>The lower.</value>
        public int Lower => lower; 
        
        /// <summary>
        /// Gets the upper.
        /// </summary>
        /// <value>The upper.</value>
        public int Upper => upper;

        /// <summary>
        /// Gets the restrictions.
        /// </summary>
        /// <value>The restrictions.</value>
        public IReadOnlyList<int> Restrictions => restrictions;
        #endregion

        #region methods
        /// <summary>
        /// Allowses the specified value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Allows(int Value)
        {
            if (Equal && Value == upper) { return true; }
            else if (Restricts(Value))
            {
                return false;
            }
            else if (!(lower > 1 || upper > 1)) { return true; }
            else if (Contains(Value))
            {
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Allowses the specified value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Allows(ref int Value)
        {
            if (!(lower > 1 || upper > 1)) return true;
            if (Equal && Value == upper) return true;
            if (Restricts(Value)) return false;
            if (Contains(Value)) return true;
            if (Value > 1 && Value < lower && !Restricts(lower))
            {
                Value = lower; return true;
            }
            else if (Value > 1 && Value > upper && !Restricts(upper))
            {
                Value = upper; return true;
            }
            return false;
        }
        /// <summary>
        /// Restrictses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Restricts(int value)
        {
            return restrictions != null &&
                restrictions.Contains(value);
        }
        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
        public bool Contains(int value)
        {
            return value > 1 && value
                >= lower && value <= upper;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            string s = "Limit(" + this.lower.ToString();
            s += "," + upper.ToString();
            s += "," + protect.ToString().ToLower();
            if (restrictions != null && restrictions.Length > 0)
            {
                s += ", new int[]{" + string.Join(",", restrictions) + "}";
            }
            return s + ")";
        }
        #endregion

        #region Equality Comparison
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            else if (obj is Limit)
            {
                Limit limit = (Limit)obj;
                bool ok = limit.Lower == this.lower &&
                   limit.Upper == this.upper;
                if (ok)
                {
                    if (limit.Restrictions == null && this.restrictions == null)
                    {
                        return true;
                    }
                    else if (limit.Restrictions != null && this.restrictions != null)
                    {
                        Array.Sort(limit.restrictions);
                        Array.Sort(this.restrictions);
                        return limit.restrictions.SequenceEqual(this.restrictions);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return this.lower & this.upper;
        }
        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="lt1">The LT1.</param>
        /// <param name="lt2">The LT2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Limit lt1, Limit lt2)
        {
            return lt1.Equals(lt2);
        }
        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="lt1">The LT1.</param>
        /// <param name="lt2">The LT2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Limit lt1, Limit lt2)
        {
            return !lt1.Equals(lt2);
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="Limit"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="lt">The lt.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator bool(Limit lt)
        {
            return (lt.lower > 1 || lt.upper > 1);
        }
        #endregion

        #region interface implementation
        /// <summary>
        /// Gets the series.
        /// </summary>
        /// <value>The series.</value>
        ISeries IGroup.Series => null;

        /// <summary>
        /// Gets the limit.
        /// </summary>
        /// <value>The limit.</value>
        ILimit IGroup.Limit => this;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Limit"/> is circular.
        /// </summary>
        /// <value><c>true</c> if circular; otherwise, <c>false</c>.</value>
        bool IGroup.Circular => false;
        #endregion
    }
}
