/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region PAIR
    /// <summary>
    /// Interface IPair
    /// </summary>
    /// <seealso cref="System.IComparable" />
    public interface IPair : IComparable
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        object Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; set; }

        /// <summary>
        /// Gets the pseudo keys.
        /// </summary>
        /// <value>The pseudo keys.</value>
        IList PseudoKeys { get; }


        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, IPair entity, MatchBy option);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity1">The entity1.</param>
        /// <param name="entity2">The entity2.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(MultCriteria criteria, IPair entity1, IPair entity2, MatchBy option);

        /// <summary>
        /// Determines whether [has type equal] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if [has type equal] [the specified other]; otherwise, <c>false</c>.</returns>
        bool HasTypeEqual(IPair other);
    }

    /// <summary>
    /// Interface IPair
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <seealso cref="MnM.Collections.IPair" />
    /// <seealso cref="System.IComparable{MnM.Collections.IPair{K, V}}" />
    /// <seealso cref="System.IEquatable{MnM.Collections.IPair{K, V}}" />
    public interface IPair<K, V> : IPair, IComparable<IPair<K, V>>, IEquatable<IPair<K, V>>
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        new K Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        new V Value { get; set; }

        /// <summary>
        /// Gets the pseudo keys.
        /// </summary>
        /// <value>The pseudo keys.</value>
        new IList<K> PseudoKeys { get; }

        /// <summary>
        /// Gets a value indicating whether [read only key].
        /// </summary>
        /// <value><c>true</c> if [read only key]; otherwise, <c>false</c>.</value>
        bool ReadOnlyKey { get; }

        /// <summary>
        /// Gets a value indicating whether [read only value].
        /// </summary>
        /// <value><c>true</c> if [read only value]; otherwise, <c>false</c>.</value>
        bool ReadOnlyValue { get; }

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, IPair<K, V> entity, MatchBy option);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity1">The entity1.</param>
        /// <param name="entity2">The entity2.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(MultCriteria criteria, IPair<K, V> entity1, IPair<K, V> entity2, MatchBy option);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(MultCriteria criteria, K item1, K item2);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, K item);

        /// <summary>
        /// Matches the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(K item);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="val1">The value1.</param>
        /// <param name="val2">The value2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(MultCriteria criteria, V val1, V val2);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="val">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, V val);

        /// <summary>
        /// Matches the specified item.
        /// </summary>
        /// <param name="val">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(V val);

        /// <summary>
        /// Matches the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(IEnumerable<K> items);

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="keys">The keys.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool Match(Criteria criteria, IEnumerable<K> keys);

        /// <summary>
        /// Determines whether [has type equal] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if [has type equal] [the specified other]; otherwise, <c>false</c>.</returns>
        bool HasTypeEqual(IPair<K, V> other);
    }
    #endregion

    public abstract class _Pair<K, V> : IPair<K, V>
    {
        #region PROPERTIES
        public abstract K Key { get; set; }
        public abstract V Value { get; set; }
        public abstract IList<K> PseudoKeys { get; }
        public abstract bool ReadOnlyKey { get; }
        public abstract bool ReadOnlyValue { get; }
        #endregion

        #region MATCH
        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="other">The other.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(Criteria criteria, IPair<K, V> other, MatchBy option)
        {
            if (other == null) return false;
            switch (option)
            {
                case MatchBy.Key:
                    return this.Match(criteria, other.Key);
                case MatchBy.Value:
                    return Operations.Compare(Value, criteria, other.Value);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(Criteria criteria, IPair<K, V> other)
        {
            if (!HasTypeEqual(other))
                return false;
            return Operations.Compare(this as IPair<K, V>, criteria, other);
        }
      
        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="other1">The other1.</param>
        /// <param name="other2">The other2.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(MultCriteria criteria, IPair<K, V> other1, IPair<K, V> other2, MatchBy option)
        {
            if (other1 == null || other2 == null) return false;
            switch (option)
            {
                case MatchBy.Key:
                    return Match(criteria, other1.Key, other2.Key);
                case MatchBy.Value:
                    return Operations.CompareRange(Value, criteria, other1.Value, other2.Value);
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(Criteria criteria, K item)
        {
            if (PseudoKeys == null)
                return Operations.Compare(Key, criteria, item);
            foreach (var key in PseudoKeys)
                if (Operations.Compare(key, criteria, item)) return true;
            return false;
        }

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="item1">The item1.</param>
        /// <param name="item2">The item2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(MultCriteria criteria, K item1, K item2)
        {
            if (PseudoKeys == null)
                return Operations.CompareRange(Key, criteria, item1, item2);
            foreach (var key in PseudoKeys)
                if (Operations.CompareRange(key, criteria, item1, item2))
                    return true;
            return false;
        }

        /// <summary>
        /// Matches the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(K item)
        {
            return Match(Criteria.Equal, item);
        }

        /// <summary>
        /// Matches the specified keys.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(IEnumerable<K> keys)
        {
            if (PseudoKeys == null)
            {
                foreach (var item in keys)
                    if (Operations.Compare(this.Key, item)) return true;
            }
            else
            {
                foreach (var key in PseudoKeys)
                {
                    foreach (var item in keys)
                        if (Operations.Compare(key, item)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="keys">The keys.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Match(Criteria criteria, IEnumerable<K> keys)
        {
            if (PseudoKeys != null)
            {
                foreach (var item in keys)
                    if (Operations.Compare(Key, criteria, item)) return true;
            }
            else
            {
                foreach (var key in PseudoKeys)
                {
                    foreach (var item in keys)
                        if (Operations.Compare(key, criteria, item)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <returns></returns>
        public bool Match(MultCriteria criteria, V val1, V val2)
        {
            return (Operations.CompareRange(Value, criteria, val1, val2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Match(Criteria criteria, V val)
        {
            return (Operations.Compare(Value, criteria, val));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool Match(V val)
        {
            return (Operations.Compare(Value, 0, val));
        }
        #endregion

        #region HAS TYPE EQUAL
        /// <summary>
        /// Determines whether [has type equal] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if [has type equal] [the specified other]; otherwise, <c>false</c>.</returns>
        public virtual bool HasTypeEqual(IPair<K, V> other)
        {
            return other.VerifyAs<_Pair<K, V>>();
        }
        /// <summary>
        /// Determines whether [has type equal] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if [has type equal] [the specified other]; otherwise, <c>false</c>.</returns>
        public bool HasTypeEqual(IPair other)
        {
            if (!other.VerifyAs<IPair<K, V>>()) { return false; }
            return HasTypeEqual((IPair<K, V>)other);
        }
        #endregion

        #region EQUALITY
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        public bool Equals(IPair<K, V> other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            else if (other != null && other.HasTypeEqual(this))
            {
                return this.GetHashCode() == other.GetHashCode();
            }
            else return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public sealed override bool Equals(object other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            else if (other != null && other.VerifyAs<_Pair<K, V>>())
            {
                return this.GetHashCode() == other.GetHashCode();
            }
            else return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            int kCode = (Key != null) ? Key.GetHashCode() : 0;
            int vCode = (Value != null) ? Value.GetHashCode() : 0;
            return kCode & vCode;
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(_Pair<K, V> a, IPair<K, V> b)
        {
            bool a_ = ((object)a) == null;
            bool b_ = ((object)b) == null;

            if (a_ && b_) { return true; }
            else if (a_) { return false; }
            else return a.Equals(b);
        }
        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(_Pair<K, V> a, IPair<K, V> b)
        {
            bool a_ = ((object)a) == null;
            bool b_ = ((object)b) == null;

            if (a_ && b_) { return false; }
            else if (a_) { return true; }
            else return !a.Equals(b);
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="MultiEntry{K, V}"/> to <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="a">a.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator bool(_Pair<K, V> a)
        {
            return (a.Key != null);
        }
        #endregion

        #region INTERFACE
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        object IPair.Key
        {
            get { return Key; }
            set
            {
                if (value.VerifyAs<K>()) { this.Key = ((K)value); }
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        object IPair.Value
        {
            get { return Value; }
            set
            {
                if (value.VerifyAs<V>()) { this.Value = ((V)value); }
            }
        }

        /// <summary>
        /// Gets the pseudo keys.
        /// </summary>
        /// <value>The pseudo keys.</value>
        IList<K> IPair<K, V>.PseudoKeys => PseudoKeys;

        /// <summary>
        /// Gets the pseudo keys.
        /// </summary>
        /// <value>The pseudo keys.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        IList IPair.PseudoKeys => PseudoKeys as IList;

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.</returns>
        int IComparable.CompareTo(object obj)
        {
            if (obj.VerifyAs<IPair<K, V>>())
            {
                return ((IComparable<IPair<K, V>>)this).CompareTo((IPair<K, V>)obj);
            }
            else { return 0; }
        }

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool IPair.Match(Criteria criteria, IPair entity, MatchBy option)
        {
            if (!entity.VerifyAs<IPair<K, V>>()) return false;
            return Match(criteria, (IPair<K, V>)entity, option);
        }

        /// <summary>
        /// Matches the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="entity1">The entity1.</param>
        /// <param name="entity2">The entity2.</param>
        /// <param name="option">The option.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool IPair.Match(MultCriteria criteria, IPair entity1, IPair entity2, MatchBy option)
        {
            if (!entity1.VerifyAs<IPair<K, V>>() || !entity2.VerifyAs<IPair<K, V>>()) return false;
            return Match(criteria, (IPair<K, V>)entity1, (IPair<K, V>)entity2, option);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.</returns>
        int IComparable<IPair<K, V>>.CompareTo(IPair<K, V> other)
        {
            if (!HasTypeEqual(other)) { return 0; }

            if (Equals(other))
            {
                return 0;
            }
            else if (other != null)
            {
                int i = GetHashCode();
                int j = other.GetHashCode();
                if (i == j) { return 0; }
                else if (i > j) { return 1; }
                else if (i < j) { return -1; }
            }
            return 0;
        }
        #endregion
    }
}
