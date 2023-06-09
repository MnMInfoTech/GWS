/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MnM.GWS
{
    #region IGENRE
    public interface IGenre : IPair<string, Type>
    {
        #region PROPERTIES
        string Name { get; }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        string TypeName { get; }

        /// <summary>
        /// Gets the name of the eval.
        /// </summary>
        /// <value>The name of the eval.</value>
        string EvalName { get; }

        /// <summary>
        /// Gets the name of the eval.
        /// </summary>
        /// <value>The name of the eval.</value>
        string GenericName { get; }

        /// <summary>
        /// Gets the name of the eval.
        /// </summary>
        /// <value>The name of the eval.</value>
        string CreationName { get; }

        /// <summary>
        /// Gets the name of the eval.
        /// </summary>
        /// <value>The name of the eval.</value>
        string Namespace { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is generic.
        /// </summary>
        /// <value><c>true</c> if this instance is generic; otherwise, <c>false</c>.</value>
        bool IsGeneric { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is abstract.
        /// </summary>
        /// <value><c>true</c> if this instance is abstract; otherwise, <c>false</c>.</value>
        bool IsAbstract { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is interface.
        /// </summary>
        /// <value><c>true</c> if this instance is interface; otherwise, <c>false</c>.</value>
        bool IsInterface { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is nested.
        /// </summary>
        /// <value><c>true</c> if this instance is nested; otherwise, <c>false</c>.</value>
        bool IsNested { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is primitive.
        /// </summary>
        /// <value><c>true</c> if this instance is primitive; otherwise, <c>false</c>.</value>
        bool IsPrimitive { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is array.
        /// </summary>
        /// <value><c>true</c> if this instance is array; otherwise, <c>false</c>.</value>
        bool IsArray { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has i fx interface.
        /// </summary>
        /// <value><c>true</c> if this instance has IFx interface; otherwise, <c>false</c>.</value>
        bool HasIFxInterface { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has t list interface.
        /// </summary>
        /// <value><c>true</c> if this instance has t list interface; otherwise, <c>false</c>.</value>
        bool HasTListInterface { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has i list interface.
        /// </summary>
        /// <value><c>true</c> if this instance has i list interface; otherwise, <c>false</c>.</value>
        bool HasIListInterface { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has t enumerable interface.
        /// </summary>
        /// <value><c>true</c> if this instance has t enumerable interface; otherwise, <c>false</c>.</value>
        bool HasTEnumerableInterface { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has i enumerable interface.
        /// </summary>
        /// <value><c>true</c> if this instance has i enumerable interface; otherwise, <c>false</c>.</value>
        bool HasIEnumerableInterface { get; }

        /// <summary>
        /// Gets the nested count.
        /// </summary>
        /// <value>The nested count.</value>
        int NestedCount { get; }

        /// <summary>
        /// Gets the nested parameters.
        /// </summary>
        /// <value>The nested parameters.</value>
        IGenreCollection NestedParams { get; }

        /// <summary>
        /// Gets the genric parameters.
        /// </summary>
        /// <value>The genric parameters.</value>
        IGenreCollection GenricParams { get; }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <value>The interfaces.</value>
        IGenreCollection Interfaces { get; set; }

        /// <summary>
        /// Gets the list interfaces.
        /// </summary>
        /// <value>The list interfaces.</value>
        IGenreCollection ListInterfaces { get; }

        /// <summary>
        /// Gets the base types.
        /// </summary>
        /// <value>The base types.</value>
        IGenreCollection BaseTypes { get; set; }
        #endregion

        #region INITIALIZE
        /// <summary>
        /// Initializes the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        void Initialize(Type t);
        #endregion
    }
    #endregion

    /// <summary>
    /// Class Genre. This class cannot be inherited.
    /// </summary>
    internal sealed class Genre : _Pair<string, Type>, IGenre
    {
        #region VARIABLES
        /// <summary>
        /// The listinterfaces
        /// </summary>
        GenreCollection listinterfaces;

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

        static Type listType = typeof(PrimitiveList<>);

        /// <summary>
        /// The common
        /// </summary>
        static readonly Type[] common = new Type[]
        {
            typeof ( IFunction ) ,
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
        /// <param name="getinterfaces">if set to <c>true</c> [getinterfaces].</param>
        internal Genre(Type t, bool getbasetypes, bool getinterfaces)
        {
            Initialize(t);
            TypeInfo ti = t.GetTypeInfo();
            if (getinterfaces)
            {
                Interfaces = new GenreCollection(ti.GetInterfaces());
            }
            if (getbasetypes)
            {
                BaseTypes = new GenreCollection(t.GetBaseTypes());
            }
        }

        /// <summary>
        /// Initializes the specified t.
        /// </summary>
        /// <param name="t">The t.</param>
        public void Initialize(Type type)
        {
            keys = new string[4];
            if (type == null)
                return;
            value = type;
            var t = type.GetTypeInfo();
            Namespace = t.Namespace; IsAbstract = t.IsAbstract;
            IsInterface = t.IsInterface; IsGeneric = t.IsGenericType;
            IsNested = t.IsNested;
            IsPrimitive = (type == typeof(string)) ? true : t.IsPrimitive;
            IsArray = t.IsArray;

            if (t.IsGenericType)
            {
                #region process
                StringBuilder name = new StringBuilder();
                StringBuilder fname = new StringBuilder();
                StringBuilder qname = new StringBuilder();

                Type[] tp = t.GetGenericArguments();
                Genre[] genric = new Genre[tp.Length];
                GenreCollection info = new GenreCollection();

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
        #endregion

        #region PROPRTIES
        public static Type ListType
        {
            get => listType ?? typeof(PrimitiveList<>);
            set
            {
                if (value == null || !value.GetTypeInfo().IsAssignableFrom(typeof(IEnumerable)))
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
        public IGenreCollection NestedParams { get; private set; }

        /// <summary>
        /// Gets the genric parameters.
        /// </summary>
        /// <value>The genric parameters.</value>
        public IGenreCollection GenricParams { get; private set; }

        /// <summary>
        /// Gets the interfaces.
        /// </summary>
        /// <value>The interfaces.</value>
        public IGenreCollection Interfaces { get; set; }

        /// <summary>
        /// Gets the list interfaces.
        /// </summary>
        /// <value>The list interfaces.</value>
        public IGenreCollection ListInterfaces
        {
            get
            {
                if (listinterfaces == null)
                {
                    listinterfaces = new GenreCollection(
                        value.GetInterfaces(ExtractInterfaces.TheseOnly, common));
                }
                return listinterfaces;
            }
        }

        /// <summary>
        /// Gets the base types.
        /// </summary>
        /// <value>The base types.</value>
        public IGenreCollection BaseTypes { get; set; }
        #endregion
    }
}
