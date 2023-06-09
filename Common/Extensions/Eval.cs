/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    public static class Eval
    {
        #region VARIABLES
        static IEvaluator Instance;
        #endregion

        #region PROPERTIES
        public static string UsingDirectives => Instance.UsingDirectives;
        public static IKeywords Keywords => Instance.Keywords;
        public static IGenreCollection Genres => Instance.Genres;
        #endregion

        #region ATTACH
        public static void Attach(IEvaluator evaluator) =>
            Instance = evaluator;
        #endregion

        #region INIT
        public static void Init()
        {
            Instance.Init();
        }
        #endregion

        #region REFRESH
        public static void Refresh(params string[] assemblies)
        {
            Instance.Refresh(assemblies);
        }
        #endregion

        #region ADD REFERENCES
        public static void AddReferences(params string[] namespaces)
        {
            Instance.AddReferences(namespaces);
        }
        #endregion

        #region FUNCTION BY NAMESPACE
        public static Dictionary<string, PrimitiveList<string>> FunctionsByNameSpace()
        {
            return Instance.FunctionsByNameSpace();
        }
        #endregion

        #region LIST OF FUNCTIONS
        public static string[] ListOfFunctions()
        {
            return Instance.ListOfFunctions();
        }
        #endregion

        #region EVALUATE
        public static T Evaluate<T>(string value)
        {
            return Instance.Evaluate<T>(value);
        }

        public static bool Evaluate<T>(string value, out T result)
        {
            return Instance.Evaluate(value, out result);
        }
        public static object Evaluate(string value)
        {
            return Instance.Evaluate(value);
        }
        #endregion

        #region COMPILE
        public static object Compile(string code)
        {
            return Instance.Compile(code);
        }
        #endregion

        #region ADD
        public static void AddAssembley(params Type[] types)
        {
            Instance.AddAssembley(types);
        }

        public static void AddStruct<T>(params string[] structs)
        {
            Instance.AddStruct<T>(structs);
        }
        #endregion

        #region GET KEYWORD
        public static IKeyword GetKeyword(string prefix, string nameSpace)
        {
            return Instance.GetKeyword(prefix, nameSpace);
        }
        #endregion

        #region GET GENRE
        public static IGenre GetGenre(string functionName)
        {
            return Instance.GetGenre(functionName);
        }
        #endregion

        public static void Dispose()
        {
            Instance.Dispose();
        }
    }
}
