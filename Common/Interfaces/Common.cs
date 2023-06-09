/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif


namespace MnM.GWS
{
    #region MyRegion
    public interface IEnumHolder<T> where T: Enum
    {
        T Kind { get; }
    }
    #endregion

    #region MyRegion
    internal interface IExEnumHolder<T>: IEnumHolder<T> where T : Enum
    {
        new T Kind { get; set; }
    }
    #endregion

    #region IPOSITION<T>
    /// <summary>
    /// Represents an object which has index which represents its position in a collection it belongs to.
    /// The position is one based as opposed tom zero based i.e it starts from 1.
    /// </summary>
    public interface IPosition<T>
        where T : struct
    {
        /// <summary>
        /// Gets an index of this object in a collection it belongs to.
        /// </summary>
        T Position { get; }
    }
    #endregion

    #region IEx POSITION<T>
    internal interface IExPosition<T> : IPosition<T>
        where T : struct
    {
        /// <summary>
        /// Gets or sets an index of this object in a collection it belongs to.
        /// </summary>
        new T Position { get; set; }
    }
    #endregion

    #region IBOUNDS
    /// <summary>
    /// Represents an object which exposes its bounds.
    /// </summary>
    public interface IBounds : IValid
    {
        /// <summary>
        /// Gets current bounds of the perimeter.
        /// </summary>
        /// <param name="x">Far left position of this object.</param>
        /// <param name="y">Far top position of this object.</param>
        /// <param name="w">Width of this object.</param>
        /// <param name="h">Height of this object.</param>
        void GetBounds(out int x, out int y, out int w, out int h);
    }
    #endregion

    #region ITYPE-BOUNDS
    public interface ITypedBounds : IBounds, IType
    { }
    #endregion

    #region IPURPOSED BOUNDS
    public interface IPurposedBounds : IBounds, IEnumHolder<Purpose>
    { }
    #endregion

    #region IBOUNDSF
    public interface IBoundsF : IValid
    {
        /// <summary>
        /// Gets current bounds of the perimeter.
        /// </summary>
        /// <param name="x">Far left position of this object.</param>
        /// <param name="y">Far top position of this object.</param>
        /// <param name="w">Width of this object.</param>
        /// <param name="h">Height of this object.</param>
        void GetBounds(out float x, out float y, out float w, out float h);
    }
    #endregion

    #region IAXIS
    /// <summary>
    /// Represents an object which has axis information.
    /// </summary>
    public interface IAxis : IScanPoint
    {
        /// <summary>
        /// Gets value of axis.
        /// </summary>
        int Axis { get; }

        /// <summary>
        /// Gets draw option for this axis.
        /// </summary>
        LineFill Draw { get; }

        /// <summary>
        /// Gets if this object represents horizontal Axis.
        /// </summary>
        bool IsHorizontal { get; }
    }
    #endregion

    #region IALPHA
    public interface IAlpha
    {
        /// <summary>
        /// Gets Alpha to apply when rendering this object.
        /// </summary>
        byte Alpha { get; }
    }
    #endregion

    #region IEND-POINTS
    public interface IEndPoints
    {
        IEnumerable<IAxisPoint> Points { get; }
    }
    #endregion

    #region IORIGIN-COMPATIBLE
    /// <summary>
    /// Represents an object which is capable of creating new version which is origin based,
    /// i.e left-top corner is always placed at (0, 0).
    /// </summary>
    public interface IOriginCompatible
    {
        /// <summary>
        /// Indicates if this object is origin based i.e left-top corner is always placed at (0, 0).
        /// </summary>
        bool IsOriginBased { get; }

        /// <summary>
        /// Gets origin-based version of this object i.e left-top corner is placed at (0, 0).
        /// </summary>
        /// <returns></returns>
        IOriginCompatible GetOriginBasedVersion();
    }
    #endregion

    #region ILASTMOUSE-POSITION
    /// <summary>
    /// Represents an object which offers last mouse position it was hovered on.
    /// </summary>
    public interface ILastMousePosition
    {
        /// <summary>
        /// Position of mouse which last hover was done on.
        /// </summary>
        IPoint LastMousePosition { get; }
    }
    #endregion

    #region ICOLOR
    public interface IColour
    {
        /// <summary>
        /// The colour this object represents.
        /// </summary>
        int Colour { get; }
    }
    #endregion

    #region ISCAN-POINT
    public interface IScanPoint
    { }
    #endregion 

    #region ISCAN-LINE
    public interface IScanLine
    { }
    #endregion

    #region IINDEPENDENT
    /// <summary>
    /// Marker interface - represents an object which is indepedent of or from any group
    /// and as such always exclude from being added to any group or collection.
    /// </summary>
    public interface IIndependent
    { }
    #endregion

    #region POINT-TYPE
    public interface IPointType
    {
        PointKind Kind { get; }
    }
    #endregion

    #region IVALID
    public interface IValid
    {
        /// <summary>
        /// Indicates if this object has valid perimeter or not.
        /// </summary>
        bool Valid { get; }
    }
    #endregion

    #region IITEMSPREAD
    public interface IItemSpread
    {
        /// <summary>
        /// Gets a direction of spread i.e horizontal or vertical. 
        /// Vertical means items will be placed from top to down and
        /// Horizontal means items will be placed from left to right.
        /// </summary>
        ItemSpread ItemSpread { get; }
    }
    #endregion

    #region IITEMSPREAD
    public interface IItemSpreadHolder: IItemSpread
    {
        /// <summary>
        /// Gets or sets a direction of spread i.e horizontal or vertical. 
        /// Vertical means items will be placed from top to down and
        /// Horizontal means items will be placed from left to right.
        /// </summary>
        new ItemSpread ItemSpread { get; set; }
    }
    #endregion

    #region ICLONEABLE
    public interface ICloneable
    {
        object Clone();
    }
    #endregion

    #region ICLONEABLE
    public interface ICloneable<T>: ICloneable
    {
       new T Clone();
    }
    #endregion

    #region ICONTEXT
    /// <summary>
    /// This is a marker interface which represents an object which can be converted to GWS Entity.
    /// </summary>
    public interface IContext
    { }
    #endregion

    #region IID<T>
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
    #endregion

    #region IVALUE
    public interface IValue
    {
        /// <summary>
        /// Gets a boolean value.
        /// </summary>
        object Value { get; }
    }
    #endregion

    #region IVALUE<T>
    public interface IValue<T> : IValue
    {
        /// <summary>
        /// Gets a boolean value.
        /// </summary>
        new T Value { get; }
    }
    #endregion

    #region INAME
    public interface IName
    {
        string Name { get; }
    }
    #endregion

    #region INAME2
    public interface IName2 : IName
    {
        new string Name { get; set; }
    }
    #endregion

    #region ITYPEID
    public interface ITypeID
    {
        string TypeID { get; }
    }
    #endregion

    #region ISAFE-CAST
    public interface ISafeCast
    {
        V Cast<V>();
    }
    #endregion

    #region IATTACHMENT
    /// <summary>
    /// This is a marker interface. It can be attached to the Implemnetation class of GWS.
    /// </summary>
    public interface IAttachment : IDisposable { }
    #endregion

    #region IKEYWORD
    public interface IKeyword : IPair<string, ExprType>
    {
    }
    #endregion

    #region IKEYWORDS
    public interface IKeywords : ILexicon<string, ExprType, IKeyword>, ICloneable
    {
        IKeyword NewKeyword(ExprType value, IEnumerable<string> keys);
        IKeyword NewKeyword(ExprType value, string key);

        IGenre NewGenre(Type t);


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

    #region ILOOP
    public interface ILoop
    {
        /// <summary>
        /// Gets interval by which the activity should be repeated.
        /// </summary>
        long Interval { get; }

        /// <summary>
        /// Gets a current status of this object
        /// </summary>
        LoopStatus Status { get; }

        /// <summary>
        /// Starts the loop this object offers.
        /// </summary>
        bool Start();

        /// <summary>
        /// Stops the loop this object offers.
        /// </summary>
        bool Stop();
    }
    #endregion

    #region IREPEATER
    /// <summary>
    /// Represents an object which repeats some activity
    /// </summary>
    public interface IRepeater : ILoop
    {
        /// <summary>
        /// Gets or sets interval by which the activity shold be repeated.
        /// </summary>
        new int Interval { get; set; }

        /// <summary>
        /// Gets or sets last recorded time in repated process cycle.
        /// </summary>
        long TimeStamp { get; }
    }
    #endregion

    #region ICOVERT<T>
    /// <summary>
    /// Interface IConvert
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConvertible<T>
    {
        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns>T.</returns>
        T Convert();
    }
    #endregion

    #region ICOVERT<T>
    /// <summary>
    /// Interface IConvert
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConvertible<T, TInput>
    {
        /// <summary>
        /// Converts this instance.
        /// </summary>
        /// <returns>T.</returns>
        T Convert(TInput argument);
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
        IGenreCollection Genres { get; }
        #endregion

        #region refresh / add references
        void Init();
        void Refresh(params string[] assemblies);
        void AddReferences(params string[] namespaces);
        #endregion

        #region list functions
        Dictionary<string, PrimitiveList<string>> FunctionsByNameSpace();
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
        IGenre GetGenre(string functionName);
        #endregion
    }
    #endregion

    #region ISOURCE<T>
    /// <summary>
    /// Represents an object which has some data.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    public interface ISource<T> : ISize
    {
         T Source { get; }
    }
    #endregion

    #region IExtracter<T>
    /// <summary>
    /// Represents an object which has some extractable data.
    /// </summary>
    /// <typeparam name="T">Type of data.</typeparam>
    internal interface IExtracter<T> : ISize
    {
        /// <summary>
        /// Gets data of type T from this object from a particular layer and/or of particualr object specified by parameters.
        /// </summary>
        /// <param name="parameters">Parameters to get data of type T from a particular layer and/or of particualr object.</param>
        /// <returns>Pointer to colour array.</returns>
        T Extract(IExSession session, out int srcW, out int srcH);
    }
    #endregion

    #region INTERNAL INTERFACE
    /// <summary>
    /// Any interface inheriting this interface is meant to be inherited internal only.
    /// Which means even if the derived interface is public, you are not supposed to
    /// provide your own implementation through a class or structure - by implementing this interface.
    /// Any such class or structure will not work with GWS.
    /// For example, IRenderer interface is public but if you want any class to be used in GWS
    /// with IRenderer implementation, you must inherit from Renderer class which is abstract.
    /// </summary>
    public interface INotToBeImplementedOutsideGWS 
    { }
    #endregion
}
