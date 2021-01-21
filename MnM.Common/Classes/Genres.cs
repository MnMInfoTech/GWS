/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;

using MnM.GWS.Advanced;

namespace MnM.GWS
{
    public sealed class Genres: _Lexicon<string, Type, Genre>
    {
        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="Genres"/> class.
        /// </summary>
        public Genres() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genres"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public Genres(int capacity) : base(capacity) {; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genres"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public Genres(IEnumerable<Genre> source) : base(source) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genres"/> class.
        /// </summary>
        /// <param name="types">The types.</param>
        public Genres(params Type[] types)
        {
            foreach (var item in types)
            {
                if (item != null)
                {
                    Add(new Genre(item, false));
                }
                else base.Add(null);
            }
        }

        #endregion

        #region ADD
        /// <summary>
        /// Adds the specified tp.
        /// </summary>
        /// <param name="tp">The tp.</param>
        public void Add(Type tp)
        {
            if (tp == null)
            {
                Genre g = null;
                base.Add(g);
            }
            else
            {
                Add(new Genre(tp, false));
            }
            
        }
        #endregion

        #region ADD RANGE
        public void AddRange(IEnumerable<Type> collection)
        {
            foreach (var t in collection)
            {
                Add(t);
            }
        }
        #endregion

        #region FIND GENRE FROM EXPRSSION
        public Genre GetGenre(string Expression)
        {
            Entry<Genre> seek = this.Find(Criteria.StringEqualNoCase, Expression);
            if (seek)
                return seek.Value;
            return null;
        }
        #endregion
    }
}
