/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
               #if !NETSTANDARD2_0
#region ICLONEABLE
    public interface ICloneable
    {
        object Clone();
    }
    #endregion
#endif
    #region ID
    /// <summary>
    /// Represents an object which has a unique ID which serves as a key to store or retrieve the object from store.
    /// </summary>
    public interface IID<T>
    {
        /// <summary>
        ///Gets a unique ID which serves as a key to store or retrieve the object from store.
        /// </summary>
        T ID { get; }
    }
    /// <summary>
    /// Represents an object which has a unique ID which serves as a key to store or retrieve the object from store.
    /// </summary>
    public interface IID : IID<int>
    { }
#endregion

#region ATTACHMENT
    /// <summary>
    /// This is a marker interface. It can be attached to the Implemnetation class of GWS.
    /// </summary>
    public interface IAttachment : IDisposable { }
#endregion

#region KEYWORD
    public interface IKeyword : IPair<string, ExprType>
    {
    }
#endregion

#region IKEYWORDS
    public interface IKeywords : ILexicon<string, ExprType, IKeyword>, ICloneable
    {
        IKeyword NewKeyword(ExprType value, IEnumerable<string> keys);
        IKeyword NewKeyword(ExprType value, string key);
#if NETSTANDARD2_0
        Genre NewGenre(Type t);
#endif
        void AddRange(IEnumerable<IKeyword> collection);
        void Trim();

        //#region add keyword
        //void Add(ExprType value, params string[] keys);

        //void Add(ExprType value, IEnumerable<string> keys);

        //void AddClass<T>(params string[] keys) where T : class;
        //void AddStruct<T>(params string[] keys) where T : struct;
        //void AddArray<T>(params string[] keys);
        //void AddEnumerable<E, T>(params string[] keys) where E : IEnumerable<T>;
        //void AddKeyword<T>(ExprType type, params string[] keys);
        //#endregion

        //#region add assemblies
        //public IEnumerable<IGenre> AddAssembley(Assembly assembly, Type whereBaseTypeIsThis = null)
        //{
        //    var types = assembly.GetTypes();
        //    Predicate<Type> p;

        //    if (whereBaseTypeIsThis != null)
        //        p = (t => t.GetInterface(whereBaseTypeIsThis.Name, true) != null
        //        && !t.IsAbstract && t.IsPublic);
        //    else
        //        p = (t => !t.IsAbstract && t.IsPublic);

        //    var glist = types.Where(x => p(x)).Select(y => y.Genre());

        //    var namespaces = glist.Select(x => x.Namespace).Distinct().
        //        Select(y => new Keyword(ExprType.NameSpace, y));

        //    AddRange(namespaces);
        //    return glist;
        //}
        //public void AddAssembley(ref IGenreList genres, Assembly assembly, Type whereBaseTypeIsThis = null)
        //{
        //    var list = AddAssembley(assembly, whereBaseTypeIsThis);
        //    genres.AddRange(list);
        //}
        //public IEnumerable<IGenre> AddAssembley(Type assemblyOfThis, Type whereBaseTypeIsThis = null) =>
        //    AddAssembley(Assembly.GetAssembly(assemblyOfThis), whereBaseTypeIsThis);
        //public IEnumerable<IGenre> AddAssembley<AssemblyOfThis, IncludeOnlyDerivedTypeOfThis>() =>
        //    AddAssembley(Assembly.GetAssembly(typeof(AssemblyOfThis)), typeof(IncludeOnlyDerivedTypeOfThis));
        //public IEnumerable<IGenre> AddAssembley<AssemblyOfThis>() =>
        //    AddAssembley(Assembly.GetAssembly(typeof(AssemblyOfThis)));
        //#endregion

        //public Keyword GetKeyword(GenreList genres, string expression, string nameSpace = null)
        //{
        //    if (!string.IsNullOrEmpty(nameSpace))
        //        expression = nameSpace + "." + expression;

        //    Entry<Keyword> key = this.Find(Criteria.StringEqualNoCase, expression);
        //    if (key)
        //        return key.Value;
        //    else
        //    {
        //        Entry<Genre> function = genres.Find(Criteria.StringEqualNoCase, expression);
        //        if (function)
        //            return new Keyword(ExprType.Function, function.Value.Key);
        //    }
        //    return null;
        //}

    }
#endregion

#region IINTPTR
    public interface IIntPtr : IDisposable, IHandle
    {
        T Instance<T>();
    }
#endregion

#region IHANDLE
    /// <summary>
    /// Represents a pointer of an object to pass about.
    /// </summary>
    public interface IHandle
    {
        /// <summary>
        /// IntPtr value represnts the information of pointer representing an object.
        /// </summary>
        IntPtr Handle { get; }
    }
#endregion

#region IOBJECTHANDLE
    /// <summary>
    /// Represents an object which has capanility to create and expose and dispose its own handle.
    /// </summary>
    public interface IHandleCreateable
    {
        IIntPtr GetHandle();
    }
#endregion

#region IEVENT-ARGS
    /// <summary>
    /// Base interface which represents EventArgs object.
    /// </summary>
    public interface IEventArgs
    {
        ///// <summary>
        ///// Gets or sets flag to indiciate whether the argument is handled or not.
        ///// </summary>
        //bool Handled { get; set; }
    }
#endregion

#region IEVENTARGS<T>
    public interface IEventArgs<T> : IEventArgs
    {
        T Args { get; }
    }
#endregion

#region IEVENTARGS<T>
    public interface IElpasedTimeEventArgs : IEventArgs
    {
        /// <summary>
        /// Gets elapsed time in miliseconds.
        /// </summary>
        uint ElapsedTime { get; }

        Unit Unit { get; }
    }
#endregion

#region INPUTEVENT-ARGS
    /// <summary>
    /// Base interface to represent GWS input arguments.
    /// </summary>
    public interface IInputEventArgs : IEventArgs
    {
        /// <summary>
        /// Gets or sets flag to indicate if enter key is pressed or mouse is entered into some region.
        /// </summary>
        bool Enter { get; set; }
    }
#endregion

#region ICANCELEVENT-ARGS
    public interface ICancelEventArgs : IEventArgs
    {
        bool Cancel { get; set; }
    }
#endregion

#region ITIMER
    /// <summary>
    /// Represents an object which allowers regualar activitiy on a specific time interval.
    /// </summary>
    public interface ITimerBase : IDisposable
    {
        /// <summary>
        /// Gets or sets interval by which the tick event gets fired.
        /// </summary>
        int Interval { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Starts this timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops this timer.
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// Represents an object which allowers regualar activitiy on a specific time interval.
    /// </summary>
    public interface ITimer : ITimerBase
    {
        Unit Unit { get; set; }

        uint LastReading { get; }

        /// <summary>
        /// Tick event which gets invoked by the interval specified.
        /// </summary>
        event EventHandler<IElpasedTimeEventArgs> Tick;
    }
#endregion

#region ICOVERT<T>
    /// <summary>
    /// Interface IConvert
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConvert<T>
    {
        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns>T.</returns>
        T Convert();
    }
#endregion

#region ICONVERTER
    public interface IConverter
    {
        bool ConvertTo<T>(string expression, out T result);
        bool ConvertTo<T>(object value, out T result);
    }
#endregion

#region EVALUATOR
    public interface IEvaluator : IAttachment
    {
#region properties
        string UsingDirectives { get; }
        IKeywords Keywords { get; }
#if NETSTANDARD2_0
        Genres Genres { get; }
#endif
#endregion

#region refresh / add references
        void Init();
        void Refresh(params string[] assemblies);
        void AddReferences(params string[] namespaces);
#endregion

#region list functions
        Dictionary<string, Collection<string>> FunctionsByNameSpace();
        string[] ListOfFunctions();
#endregion

#region compile & evaluate
        T Evaluate<T>(string value);
        bool Evaluate<T>(string value, out T result);
#endregion

#region abstract methods
        object Compile(string code);
        object Evaluate(string value);
        void AddAssembley(params Type[] types);
        void AddStruct<T>(params string[] structs);
        IKeyword GetKeyword(string prefix, string nameSpace);
#if NETSTANDARD2_0
        Genre GetGenre(string functionName);
#endif
#endregion
    }
#endregion
}
