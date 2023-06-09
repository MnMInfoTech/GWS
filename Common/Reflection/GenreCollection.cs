/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region IGENRE-COLLECTION
    public interface IGenreCollection : ILexicon<string, Type, IGenre>
    {
        /// <summary>
        /// Adds the specified tp.
        /// </summary>
        /// <param name="tp">The tp.</param>
        void Add(Type tp);

        void AddRange(IEnumerable<Type> collection);

        IGenre GetGenre(string Expression);
    }
    #endregion

    public sealed class GenreCollection : _Lexicon<string, Type, IGenre>, IGenreCollection 
    {
        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="GenreCollection"/> class.
        /// </summary>
        public GenreCollection() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreCollection"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public GenreCollection(int capacity) : base(capacity) {; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreCollection"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public GenreCollection(IEnumerable<IGenre> source) : base(source) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreCollection"/> class.
        /// </summary>
        /// <param name="types">The types.</param>
        public GenreCollection(params Type[] types)
        {
            foreach (var item in types)
            {
                if (item != null)
                {
                    Add(Factory.newGenre(item, false));
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
                IGenre g = null;
                base.Add(g);
            }
            else
            {
                Add(Factory.newGenre(tp, false));
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
        public IGenre GetGenre(string Expression)
        {
            Entry<IGenre> seek = this.Find(Criteria.StringEqualNoCase, Expression);
            if (seek)
                return seek.Value;
            return null;
        }
        #endregion
    }
}
