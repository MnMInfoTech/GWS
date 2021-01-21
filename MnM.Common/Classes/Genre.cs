/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    /// <summary>
    /// Class Genre. This class cannot be inherited.
    /// </summary>
    public sealed class Genre : _Pair<string, Type>
    {
        #region VARIABLES
        /// <summary>
        /// The listinterfaces
        /// </summary>
        Genres listinterfaces;

        /// <summary>
        /// The keys
        /// </summary>
        string[] keys = null;

        /// <summary>
        /// The value
        /// </summary>
        Type value;

        /// <summary>
        /// The key
        /// </summary>
        string key;

        static Type listType = typeof(Collection<>);

        /// <summary>
        /// The common
        /// </summary>
        static readonly Type[] common = new Type[]
      {
#if Functions
          typeof ( IFunction ) ,
#endif
        typeof ( IList<> ) ,
        typeof ( IList ) ,
        typeof ( IEnumerable<> ) ,
        typeof ( IEnumerable )
      };
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        public Genre() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="getbasetypes">if set to <c>true</c> [getbasetypes].</param>
        /// <param name="option">The option.</param>
        /// <param name="interfaces">The interfaces.</param>
        public Genre(Type t, bool getbasetypes, ExtractInterfaces option, params Type[] interfaces)
        {
            Initialize(t);
            Interfaces = new Genres(t.GetInterfaces(option, interfaces));

            if (getbasetypes)
                BaseTypes = new Genres(t.GetBaseTypes());

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="getbasetypes">if set to <c>true</c> [getbasetypes].</param>
        /// <param name="getinterfaces">if set to <c>true</c> [getinterfaces].</param>
        public Genre(Type t, bool getbasetypes, bool getinterfaces)
        {
            Initialize(t);
            if (getinterfaces)
            {
                Interfaces = new Genres(t.GetInterfaces());
            }
            if (getbasetypes)
            {
                BaseTypes = new Genres(t.GetBaseTypes());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="getbasetypes">if set to <c>true</c> [getbasetypes].</param>
        public Genre(Type t, bool getbasetypes)
        {
            Initialize(t);
            if (getbasetypes)
                BaseTypes = new Genres(t.GetBaseTypes());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genre"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        public Genre(Type t)
        {
            Initialize(t);
        }

        /// <summary>
        /// Initializes the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        public void Initialize(Type t)
        {
            keys = new string[4];
            if (t == null)
                return;

            if (t != null)
            {
                value = t;
                Namespace = t.Namespace; IsAbstract = t.IsAbstract;
                IsInterface = t.IsInterface; IsGeneric = t.IsGenericType;
                IsNested = t.IsNested;
                IsPrimitive = (t == typeof(string)) ? true : t.IsPrimitive;
                IsArray = t.IsArray;

                if (t.IsGenericType)
                {
                    #region process
                    StringBuilder name = new StringBuilder();
                    StringBuilder fname = new StringBuilder();
                    StringBuilder qname = new StringBuilder();

                    Type[] tp = t.GetGenericArguments();
                    Genre[] genric = new Genre[tp.Length];
                    Genres info = new Genres();

                    for (int i = 0; i < tp.Length; i++)
                    {
                        genric[i] = new Genre(tp[i], false, false);
                    }

                    if (t.IsNested)
                    {
                        int j = 0;
                        string fullname;
                        if (t.IsValueType)
                        {
                            fullname = t.DeclaringType.FullName + "+" + t.Name;
                        }
                        else
                        {
                            fullname = t.FullName;

                        }
                        string[] s = fullname.Split("+"[0]);
                        for (int i = 0; i < s.Length - 1; i++)
                        {
                            qname.Append(s[i]);
                            qname.Append("+");

                            if (s[i].IndexOf("`") != -1)
                            {
                                string[] x = s[i].Split("`"[0]);
                                fname.Append(x[0]);
                                fname.Append("<");
                                int y;
                                int.TryParse(x[1].Substring(0, System.Math.Abs(x[1].IndexOf("["))), out y);
                                if (y < 0) { y = 1; }
                                for (int k = j; k < NestedCount + y; k++, j++)
                                {
                                    if (genric[k].EvalName != null)
                                    {
                                        fname.Append(genric[k].EvalName + ",");
                                    }
                                    else
                                    {
                                        fname.Append(",");
                                    }
                                }
                                fname.Remove(fname.Length - 1, 1);
                                fname.Append(">");
                                NestedCount += y;
                            }
                            else { fname.Append(s[i]); }
                            fname.Append(".");
                        }
                    }
                    else
                    {
                        qname.Append(t.Namespace);
                        qname.Append(".");
                        fname.Append(t.Namespace);
                        fname.Append(".");
                    }
                    string nm = t.Name.Split("`"[0])[0];

                    qname.Append(t.Name);

                    if (IsNested) { keys[3] = qname.ToString(); }

                    qname.Append("[");

                    fname.Append(nm);
                    fname.Append("<");

                    name.Append(nm);
                    name.Append("<");

                    for (int i = 0; i < tp.Length; i++)
                    {
                        if (i >= NestedCount)
                        {
                            if (tp[i].FullName != null)
                            {
                                fname.Append(genric[i].EvalName + ",");
                                name.Append(genric[i].Name + ",");
                            }
                            else
                            {
                                fname.Append(",");
                                name.Append(",");
                            }
                        }

                        qname.Append(genric[i].Key + ",");
                        info.Add(genric[i]);
                    }
                    fname.Remove(fname.Length - 1, 1);
                    name.Remove(name.Length - 1, 1);
                    qname.Remove(qname.Length - 1, 1);
                    #endregion

                    fname.Append(">");
                    name.Append(">");
                    qname.Append("]");
                    keys[0] = qname.ToString();
                    keys[1] = fname.ToString();
                    keys[2] = name.ToString();
                    if (keys[3] == null)
                        keys[3] = t.Namespace + "." + t.Name;
                    GenricParams = info;
                }
                else
                {
                    keys[0] = t.ToString();
                    keys[2] = t.Name;
                    IsGeneric = false;
                }
            }
        }
        #endregion

        #region PROPRTIES
        public static Type ListType
        {
            get => listType ?? typeof(Collection<>);
            set
            {
                if (value == null || !value.IsAssignableFrom(typeof(IEnumerable)))
                    return;
                listType = value;
            }
        }
        public override string Key
        {
            get
            {
                if (keys != null && keys.Length > 0) return keys[0];
                return key;
            }
            set
            {
                if (ReadOnlyKey) throw new NotImplementedException();
                if (keys != null && keys.Length > 0) keys[0] = value;
                else key = value;
            }
        }
        public override Type Value
        {
            get { return this.value; }
            set
            {
                if (ReadOnlyValue) throw new NotImplementedException();
                this.value = value;
            }
        }
        public override bool ReadOnlyValue => true;
        public override bool ReadOnlyKey => true;
        public override IList<string> PseudoKeys => keys;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return (keys[2] != null) ? keys[2] : keys[0];
            }
        }

        /// <summary>
        /// Gets the name of the eval.
        /// </summary>
        /// <value>The name of the eval.</value>
        public string EvalName
        {
            get
            {
                return (keys[1] != null) ? keys[1] : keys[0];
            }
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName
        {
            get { return keys[0]; }
        }

        /// <summary>
        /// Gets the name of the generic.
        /// </summary>
        /// <value>The name of the generic.</value>
        public string GenericName
        {
            get { return (keys[3] != null) ? keys[3] : keys[0]; }
        }

        /// <summary>
        /// Gets the name of the creation.
        /// </summary>
        /// <value>The name of the creation.</value>
        public string CreationName
        {
            get { return TypeName.Replace('+', '.'); }
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <value>The namespace.</value>
        public string Namespace { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is generic.
        /// </summary>
        /// <value><c>true</c> if this instance is generic; otherwise, <c>false</c>.</value>
        public bool IsGeneric { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is abstract.
        /// </summary>
        /// <value><c>true</c> if this instance is abstract; otherwise, <c>false</c>.</value>
        public bool IsAbstract { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is interface.
        /// </summary>
        /// <value><c>true</c> if this instance is interface; otherwise, <c>false</c>.</value>
        public bool IsInterface { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is nested.
        /// </summary>
        /// <value><c>true</c> if this instance is nested; otherwise, <c>false</c>.</value>
        public bool IsNested { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is primitive.
        /// </summary>
        /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
        public bool IsPrimitive { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is array.
        /// </summary>
        /// <value><c>true</c> if this instance is array; otherwise, <c>false</c>.</value>
        public bool IsArray { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has i fx interface.
        /// </summary>
        /// <value><c>true</c> if this instance has i fx interface; otherwise, <c>false</c>.</value>
        public bool HasIFxInterface
        {
            get { return ListInterfaces[0] != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has t list interface.
        /// </summary>
        /// <value><c>true</c> if this instance has t list interface; otherwise, <c>false</c>.</value>
        public bool HasTListInterface
        {
            get { return this.ListInterfaces[1] != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has i list interface.
        /// </summary>
        /// <value><c>true</c> if this instance has i list interface; otherwise, <c>false</c>.</value>
        public bool HasIListInterface
        {
            get { return this.ListInterfaces[2] != null; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has t enumerable interface.
        /// </summary>
        /// <value><c>true</c> if this instance has t enumerable interface; otherwise, <c>false</c>.</value>
        public bool HasTEnumerableInterface
        {
            get { return this.ListInterfaces[3] != null; }
        }
        /// <summary>
        /// Gets a value indicating whether this instance has i enumerable interface.
        /// </summary>
        /// <value><c>true</c> if this instance has i enumerable interface; otherwise, <c>false</c>.</value>
        public bool HasIEnumerableInterface
        {
            get { return this.ListInterfaces[4] != null; }
        }

        /// <summary>
        /// Gets the nested count.
        /// </summary>
        /// <value>The nested count.</value>
        public int NestedCount { get; private set; }

        /// <summary>
        /// Gets the nested parameters.
        /// </summary>
        /// <value>The nested parameters.</value>
        public Genres NestedParams { get; private set; }

        /// <summary>
        /// Gets the genric parameters.
        /// </summary>
        /// <value>The genric parameters.</value>
        public Genres GenricParams { get; private set; }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <value>The interfaces.</value>
        public Genres Interfaces { get; set; }

        /// <summary>
        /// Gets the list interfaces.
        /// </summary>
        /// <value>The list interfaces.</value>
        public Genres ListInterfaces
        {
            get
            {
                if (listinterfaces == null)
                {
                    listinterfaces = new Genres(value.GetInterfaces
                      (ExtractInterfaces.TheseOnly, common));
                }
                return listinterfaces;
            }
        }

        /// <summary>
        /// Gets the base types.
        /// </summary>
        /// <value>The base types.</value>
        public Genres BaseTypes { get; set; }
        #endregion

        #region HYBRID GENRE
        /// <summary>
        /// Hybrids the genre.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="option">The option.</param>
        /// <returns>IGenre.</returns>
        public Genre HybridGenre(Genre other, ExcludeNestedParams option)
        {
            return new Genre(HybridGenreType(other, option), true, false);
        }

        /// <summary>
        /// Hybrids the genre.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="option">The option.</param>
        /// <param name="usethese">The usethese.</param>
        /// <returns>IGenre.</returns>
        public Genre HybridGenre(Genre other, ExcludeNestedParams option, params int[] usethese)
        {
            return new Genre(HybridGenreType(other, option, usethese), true, false);
        }

        /// <summary>
        /// Hybrids the genre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <returns>IGenre.</returns>
        public Genre HybridGenre(bool ignorenestedparams, params Genre[] paramlist)
        {
            return new Genre(HybridGenreType(ignorenestedparams, paramlist), true, false);
        }

        /// <summary>
        /// Hybrids the genre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <returns>IGenre.</returns>
        public Genre HybridGenre(bool ignorenestedparams, IList<Genre> paramlist)
        {
            return new Genre(HybridGenreType(ignorenestedparams, paramlist), true, false);
        }
        /// <summary>
        /// Hybrids the genre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <param name="usethese">The usethese.</param>
        /// <returns>IGenre.</returns>
        public Genre HybridGenre(bool ignorenestedparams, IList<Genre> paramlist, params int[] usethese)
        {
            return new Genre(HybridGenreType(ignorenestedparams, paramlist, usethese), true, false);
        }

        /// <summary>
        /// Hybrids the type of the genre.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="option">The option.</param>
        /// <param name="usethese">The usethese.</param>
        /// <returns>Type.</returns>
        public Type HybridGenreType(Genre other, ExcludeNestedParams option, params int[] usethese)
        {
            if (IsGeneric == false) { return null; }
            else if (usethese.Length == 0)
            {
                return HybridGenreType(other, option);
            }
            else
            {
                int count = GenricParams.Count;
                string[] abc = GenricParams.Keys.ToArray();
                int start, ostart;
                if (other.GenricParams != null)
                {
                    switch (option)
                    {
                        case ExcludeNestedParams.NoExclusion:
                        default:
                            start = 0; ostart = 0;
                            break;
                        case ExcludeNestedParams.HostGenre:
                            start = NestedCount; ostart = 0;
                            break;
                        case ExcludeNestedParams.OtherGenre:
                            start = 0; ostart = other.NestedCount;
                            break;
                        case ExcludeNestedParams.BothGenre:
                            start = NestedCount;
                            ostart = other.NestedCount;
                            break;
                    }

                    for (int i = 0; i < usethese.Length; i++)
                    {
                        if (start >= count) { break; }
                        int index = usethese[i];

                        if (index >= ostart && other.GenricParams.Count > index &&
                                      other.GenricParams[index] != null)
                        {
                            abc[start] = other.GenricParams[usethese[i]].Key;
                        }
                        ++start;
                    }
                }
                string s = GenericName + "[" + string.Join(",", abc) + "]";
                return Type.GetType(s, true);
            }
        }

        /// <summary>
        /// Hybrids the type of the genre.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="option">The option.</param>
        /// <returns>Type.</returns>
        public Type HybridGenreType(Genre other, ExcludeNestedParams option)
        {
            if (IsGeneric == false) { return null; }
            else
            {
                int count = GenricParams.Count;
                string[] abc = GenricParams.Keys.ToArray();
                int start, ostart;
                if (other.GenricParams != null)
                {

                    switch (option)
                    {
                        case ExcludeNestedParams.NoExclusion:
                        default:
                            start = 0; ostart = 0;
                            break;
                        case ExcludeNestedParams.HostGenre:
                            start = NestedCount; ostart = 0;
                            break;
                        case ExcludeNestedParams.OtherGenre:
                            start = 0; ostart = other.NestedCount;
                            break;
                        case ExcludeNestedParams.BothGenre:
                            start = NestedCount;
                            ostart = other.NestedCount;
                            break;
                    }
                    for (int i = start, j = ostart; i < count; i++, j++)
                    {
                        if (j < other.GenricParams.Count && other.GenricParams[j] != null)
                        {
                            abc[i] = other.GenricParams[j].Key;
                        }
                    }
                }

                string s = GenericName + "[" + string.Join(",", abc) + "]";
                return Type.GetType(s, true);
            }
        }

        /// <summary>
        /// Hybrids the type of the genre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <returns>Type.</returns>
        public Type HybridGenreType(bool ignorenestedparams, params Genre[] paramlist)
        {
            if (GenricParams == null) { return null; }

            int start = (ignorenestedparams) ? NestedCount : 0;
            int count = GenricParams.Count;
            string[] abc = GenricParams.Keys.ToArray();

            for (int i = 0; i < paramlist.Length; i++)
            {
                if (start > count) { break; }
                if (paramlist[i] != null)
                {
                    abc[start] = paramlist[i].Key;
                }
                start++;
            }
            string s = GenericName + "[" + string.Join(",", abc) + "]";
            return Type.GetType(s, true);
        }

        /// <summary>
        /// Hybrids the type of the genre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <returns>Type.</returns>
        public Type HybridGenreType(bool ignorenestedparams, IList<Genre> paramlist)
        {
            if (GenricParams == null) { return null; }

            int start = (ignorenestedparams) ? NestedCount : 0;
            int count = GenricParams.Count;
            string[] abc = GenricParams.Keys.ToArray();

            for (int i = 0; i < paramlist.Count; i++)
            {
                if (start > count) { break; }
                if (paramlist[i] != null)
                {
                    abc[start] = paramlist[i].Key;
                }
                start++;
            }
            string s = GenericName + "[" + string.Join(",", abc) + "]";
            return Type.GetType(s, true);
        }

        /// <summary>
        /// Hybrids the type of the genre.
        /// </summary>
        /// <param name="ignorenestedparams">if set to <c>true</c> [ignorenestedparams].</param>
        /// <param name="paramlist">The paramlist.</param>
        /// <param name="usethese">The usethese.</param>
        /// <returns>Type.</returns>
        public Type HybridGenreType(bool ignorenestedparams, IList<Genre> paramlist, params int[] usethese)
        {
            if (GenricParams == null) { return null; }

            int start = (ignorenestedparams) ? NestedCount : 0;
            int count = GenricParams.Count;
            string[] abc = GenricParams.Keys.ToArray();

            for (int i = 0; i < usethese.Length; i++)
            {
                if (start >= count) { break; }
                int index = usethese[i];

                if (paramlist.Count > index && paramlist[index] != null)
                {
                    abc[start] = paramlist[usethese[i]].Key;
                }
                ++start;
            }
            string s = GenericName + "[" + string.Join(",", abc) + "]";
            return Type.GetType(s, true);
        }
        #endregion

        #region CASCADE GENRE
        /// <summary>
        /// Gets the t list or i enumerable genric parameters.
        /// </summary>
        /// <param name="genericparam">The genericparam.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetTListOrIEnumerableGenricParams(out Genre genericparam)
        {
            if (ListInterfaces[1] != null && ListInterfaces[1].GenricParams != null)
            {
                genericparam = ListInterfaces[1].GenricParams[0];
                return true;
            }
            else if (ListInterfaces[3] != null && ListInterfaces[3].GenricParams != null)
            {
                genericparam = ListInterfaces[3].GenricParams[0];
                return true;
            }
            else { genericparam = null; }
            return false;
        }
        /// <summary>
        /// Cascades the genre.
        /// </summary>
        /// <returns>IGenre.</returns>
        public Genre CascadeGenre()
        {
            if (IsGeneric && HasTListInterface)
            {
                return HybridGenre(false, this);
            }
            else if (IsArray && Value.GetArrayRank() == 1)
            {
                return new Genre(Value.MakeArrayType(1), true, false);
            }
            else
            {
                return this;
            }
        }
        #endregion

        #region MATCH GENRE TLIST/ IEnumerable
        /// <summary>
        /// Matches the t list interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="GenericListParam">The generic list parameter.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchTListInterface(Genre other, out Genre GenericListParam)
        {
            bool success = ListInterfaces[1] != null &&
                           ListInterfaces[1].GenricParams[0] != null &&
                           other.ListInterfaces[1] != null &&
                           other.ListInterfaces[1].GenricParams[0] != null;
            if (success)
            {
                success = other.ListInterfaces[1].GenricParams[0] ==
                   ListInterfaces[1].GenricParams[0];
                if (success)
                {
                    GenericListParam = ListInterfaces[1].GenricParams[0];
                    return true;
                }
            }
            GenericListParam = null;
            return success;
        }

        /// <summary>
        /// Matches the t list interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchTListInterface(Genre other)
        {
            bool success = ListInterfaces[1] != null &&
                           ListInterfaces[1].GenricParams[0] != null &&
                           other.ListInterfaces[1] != null &&
                           other.ListInterfaces[1].GenricParams[0] != null;
            if (success)
            {
                success = other.ListInterfaces[1].GenricParams[0] ==
                   ListInterfaces[1].GenricParams[0];
            }
            return success;
        }

        /// <summary>
        /// Matches the t list interface parameter.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchTListInterfaceParam(Genre other)
        {
            if (ListInterfaces[1] != null &&
                 ListInterfaces[1].GenricParams[0] != null)
            {
                return ListInterfaces[1].GenricParams[0].Equals(other);
            }
            return false;
        }

        /// <summary>
        /// Matches the t enumerable interface parameter.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchTEnumerableInterfaceParam(Genre other)
        {
            if (ListInterfaces[3] != null &&
                 ListInterfaces[3].GenricParams[0] != null)
            {
                return ListInterfaces[3].GenricParams[0].Equals(other);
            }
            return false;
        }
        /// <summary>
        /// Matches the i list interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchIListInterface(Genre other)
        {
            bool success = other.ListInterfaces[2] != null &&
               ListInterfaces[2] != null;
            return success;
        }

        /// <summary>
        /// Matches the t enumerable interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="GenericListParam">The generic list parameter.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchTEnumerableInterface(Genre other, out Genre GenericListParam)
        {
            bool success = ListInterfaces[3] != null &&
                           ListInterfaces[3].GenricParams[0] != null &&
                           other.ListInterfaces[3] != null &&
                           other.ListInterfaces[3].GenricParams[0] != null;
            if (success)
            {
                success = other.ListInterfaces[3].GenricParams[0] ==
                   ListInterfaces[3].GenricParams[0];
                if (success)
                {
                    GenericListParam = ListInterfaces[3].GenricParams[0];
                    return true;
                }
            }
            GenericListParam = null;
            return success;
        }

        /// <summary>
        /// Matches the t enumerable interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchTEnumerableInterface(Genre other)
        {
            bool success =
                     ListInterfaces[3] != null &&
                     ListInterfaces[3].GenricParams[0] != null &&
                     other.ListInterfaces[3] != null &&
                     other.ListInterfaces[3].GenricParams[0] != null;
            ;
            if (success)
            {
                success = other.ListInterfaces[3].GenricParams[0] ==
                   ListInterfaces[3].GenricParams[0];
            }
            return success;
        }
        /// <summary>
        /// Matches the i fx interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchIFxInterface(Genre other)
        {
            bool success = other.ListInterfaces[0] != null &&
               ListInterfaces[0] != null;
            return success;
        }
        /// <summary>
        /// Matches the i enumerable interface.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool MatchIEnumerableInterface(Genre other)
        {
            bool success = other.ListInterfaces[4] != null &&
               ListInterfaces[4] != null;
            return success;
        }
        #endregion

        #region ARRAY FROM GENRE
        /// <summary>
        /// Mies the array.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>Array.</returns>
        public Array MyArray(int length)
        {
            return (Array)Value.MakeArrayType(1).MyInstance(length);
        }

        /// <summary>
        /// Mies the array.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>Array.</returns>
        public Array MyArray(IEnumerable items)
        {
            Type maintype = Type.GetType(Key + "[]");
            Array a = (Array)Value.MakeArrayType(1).MyInstance(items.Count());
            Genre gnr = Of(items);
            int i = 0;
            if (gnr.IsGeneric && gnr.ListInterfaces[1].GenricParams[0].Equals(this))
            {
                foreach (var item in items)
                {
                    try { a.SetValue(item, i); }
                    catch {; }
                }
            }
            return a;
        }
        #endregion

        #region MY INSTANCE
        /// <summary>
        /// Creates self instance.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Object.</returns>
        public object MyInstance(params object[] arguments)
        {
            if (Value.IsAbstract || Value.IsInterface) { return null; }
            return Value.MyInstance(arguments);
        }

        /// <summary>
        /// Creates self instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public T MyInstance<T>(params object[] arguments)
        {
            if (Value.IsAbstract || Value.IsInterface)
                return default(T);
            return (T)Value.MyInstance(arguments);
        }

        /// <summary>
        /// Creates self instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mi">The mi.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public T MyInstance<T>(Instance mi, params object[] arguments)
        {
            if (Value.IsAbstract || Value.IsInterface) { return default(T); }
            return (T)Value.MyInstance(mi, arguments.GetTypes(), arguments);
        }

        /// <summary>
        /// Creates self instance.
        /// </summary>
        /// <param name="mi">The mi.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>System.Object.</returns>
        public object MyInstance(Instance mi, params object[] arguments)
        {
            if (Value.IsAbstract || Value.IsInterface) { return null; }
            return Value.MyInstance(mi, arguments.GetTypes(), arguments);
        }
        #endregion

        #region LIST FROM SEED GENRE
        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns>IList.</returns>
        public IList MyList(GetList kind)
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + Key + "]");
            return (IList)maintype.MyInstance();
        }
        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kind">The kind.</param>
        /// <returns>T.</returns>
        public T MyList<T>(GetList kind) where T : IEnumerable
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + Key + "]");
            return maintype.MyInstance<T>();
        }
        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>IList.</returns>
        public IList MyList(GetList kind, params object[] arguments)
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + Key + "]");
            return (IList)maintype.MyInstance(arguments);
        }
        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kind">The kind.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public T MyList<T>(GetList kind, params object[] arguments) where T : IEnumerable
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + Key + "]");
            return maintype.MyInstance<T>(arguments);
        }
        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="kind">The kind.</param>
        /// <param name="t">The t.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T.</returns>
        public T MyList<T>(GetList kind, Type[] t, params object[] arguments) where T : IEnumerable
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + Key + "]");
            return maintype.MyInstance<T>(t, arguments);
        }
        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns>IList.</returns>
        public IList MyList(IEnumerable collection)
        {
            GetList kind = GetList.MnMList; bool success = false;

            if (collection is IReadOnlyList)
            {
                kind = GetList.MnMList;
                success = true;
            }
            if (success)
            {
                Type maintype = Type.GetType(GetBlank(kind) + "[" + Key + "]");
                return (IList)maintype.MyInstance(collection);
            }
            else { return null; }
        }
        /// <summary>
        /// Mies the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        public IList<T> MyList<T>(IEnumerable<T> source)
        {
            GetList kind = GetList.MnMList;

            if (source is System.Collections.Generic.List<T>)
                kind = GetList.List;
            else if (source is Stack<T>)
                kind = GetList.Stack;
            else if (source is Queue<T>)
                kind = GetList.Queue;
            else if (source is HashSet<T>)
                kind = GetList.HashSet;
            else if (source is IReadOnlyList<T>)
                return mnmList<T>((source as IReadOnlyList<T>));

            return MyList<IList<T>>(kind, source);
        }

        /// <summary>
        /// MNMs the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        IList<T> mnmList<T>(IReadOnlyList<T> source)
        {
            var instance = Genre.Of(source).MyInstance<IList<T>>(source);
            if (instance == null)
            {
                instance = Genre.Of(source).MyInstance<IList<T>>();
                if (instance != null && !instance.IsReadOnly)
                {
                    foreach (var item in source)
                        instance.Add(item);
                }
            }
            return instance;
        }
        #endregion

        #region LIST GENRE FROM GENRE
        /// <summary>
        /// Mies the list genre.
        /// </summary>
        /// <param name="kind">The kind.</param>
        /// <returns>IGenre.</returns>
        public Genre MyListGenre(GetList kind)
        {
            Type maintype = Type.GetType(GetBlank(kind) + "[" + Key + "]");
            return new Genre(maintype, false, false);
        }

        /// <summary>
        /// Mies the list genre.
        /// </summary>
        /// <returns>IGenre.</returns>
        public Genre MyListGenre()
        {
            if (IsGeneric && HasTListInterface)
            {
                return HybridGenre(this, ExcludeNestedParams.BothGenre);
            }
            else if (IsArray)
            {
                return new Genre(Value.MakeArrayType(1), false, false);
            }
            else
            {
                return MyListGenre(GetList.MnMList);
            }
        }
        #endregion

        #region GET BLANK
        public string GetBlank(GetList kind)
        {
            Type tp;
            switch (kind)
            {
                case GetList.MnMList:
                default:
                    tp = listType;
                    break;
                case GetList.List:
                    tp = typeof(List<>);
                    break;
                case GetList.Stack:
                    tp = typeof(Stack<>);
                    break;
                case GetList.Queue:
                    tp = typeof(Queue<>);
                    break;
                case GetList.HashSet:
                    tp = typeof(HashSet<>);
                    break;
            }
            return tp.Namespace + "." + tp.Name;
        }
        #endregion

        #region INSTANCE OF
        public static Genre Of<T>()
        {
            return new Genre(typeof(T), true, true);
        }
        public static Genre Of<T>(T source)
        {
            if (source is Type)
                return new Genre(source as Type, true, true);
            else if (source != null)
            {
                if (source is IEnumerable)
                    return new Genre(source.GetType(), true, true);
                else
                    return new Genre(source.GetType(), true, true);
            }
            else
                return new Genre(typeof(T), true, true);
        }
        public static Genre Of<T>(T source, bool getbasetypes, bool getinterfaces)
        {
            if (source is Type)
                return new Genre(source as Type, getbasetypes, getinterfaces);
            else if (source != null)
            {
                if (source is IEnumerable)
                    return new Genre(source.GetType(), getbasetypes, getinterfaces);
                else
                    return new Genre(source.GetType(), getbasetypes, getinterfaces);
            }
            else
                return new Genre(typeof(T), getbasetypes, getinterfaces);
        }
        #endregion
    }
}
