/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.


#if NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public abstract class _Evaluator : IEvaluator
    {
#region variables
        readonly Collection<string> usings = new Collection<string>(100);
        readonly Dictionary<ValidateType, string> dateFormats =
            new Dictionary<ValidateType, string>(25);

        public const char LeftRoundBrace = '(',
            LeftSquareBrace = '[',
            LeftCurlyBrace = '{';

        public const char RightRoundBrace = ')',
            RightSquareBrace = ']',
            RightCurlyBrace = '}';

        public const string dateformat = "{0}-{1}-{2}";
        public const string timeformat = "{0}:{1}:{2}.{3}";
        public const string objectArrayExpr = "new object[]{{{0}}}";
#endregion

        protected _Evaluator()
        {
            Keywords = NewKeywords();
            Genres = new Genres(100);

            AddStandardUsings(usings);
            AddStandardDataFormats(dateFormats);
            AddStandardLiteralKeywords(Keywords);
            AddLiteralArrayKeywords(Keywords);
            AddStandardMathFxKeywords(Keywords);
            Keywords.Trim();
            Genres.Trim();
        }

#region properties
        public string UsingDirectives =>
            string.Join(";", usings.Select((x => "using " + x))) + ";";
        public IKeywords Keywords { get; private set; }
        public Genres Genres { get; private set; }
#endregion

#region NEW KEYWORDS
        protected abstract IKeywords NewKeywords();
#endregion

#region REFRESH/ ADD REFERENCES
        public abstract void Init();
        public abstract void Refresh(params string[] assemblies);
        public void AddReferences(params string[] namespaces) =>
            usings.AddRange(namespaces);
#endregion

#region LIST FUNCTIONS
        public Dictionary<string, Collection<string>> FunctionsByNameSpace()
        {
            Dictionary<string, Collection<string>> h = new Dictionary<string, Collection<string>>();

            foreach (var gnr in Genres)
            {
                if (gnr != null && gnr.HasIFxInterface)
                {
                    if (h.ContainsKey(gnr.Namespace))
                    {
                        h[gnr.Namespace].Add(gnr.Name);
                    }
                    else
                    {
                        Collection<string> l = new Collection<string>();
                        l.Add(gnr.Name);
                        h.Add(gnr.Namespace, l);
                    }
                }
            }
            return h;
        }
        public string[] ListOfFunctions() =>
            Genres.Where(x => x != null && x.HasIFxInterface).Select(y => y.Name).ToArray();
#endregion

#region COMPILE & EVALUATE
        public T Evaluate<T>(string value)
        {
            try
            {
                return (T)Evaluate(value);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
        public bool Evaluate<T>(string value, out T result)
        {
            result = default(T);
            try
            {
                result = (T)Evaluate(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
#endregion

#region ABSTRACT METHODS
        public abstract object Compile(string code);
        public abstract object Evaluate(string value);
        protected abstract void AddStandardUsings(IList<string> usings);
        protected abstract void AddStandardDataFormats(Dictionary<ValidateType, string> dateFormats);
        protected abstract void AddStandardLiteralKeywords(IKeywords keywords);
        protected abstract void AddStructKeywords(IKeywords keywords);
        protected abstract void AddLiteralArrayKeywords(IKeywords keywords);
        protected abstract void AddStandardMathFxKeywords(IKeywords keywords);

        public abstract void AddAssembley(params Type[] types);
        public abstract void AddStruct<T>(params string[] structs);
        public abstract IKeyword GetKeyword(string prefix, string nameSpace);
        public abstract Genre GetGenre(string functionName);

        public virtual void Dispose() { }
#endregion
    }
}
#endif