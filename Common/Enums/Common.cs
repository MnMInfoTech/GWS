/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;

namespace MnM.GWS
{
    #region USAGE
    /// <summary>
    /// Enum Usage
    /// </summary>
    public enum Usage
    {
        /// <summary>
        /// Value of Field marked with this usage will get Inserted and Updated but will not be used as
        /// Criteria in Update or Delete operations.
        /// </summary>
        NonKey,
        /// <summary>
        /// Value of Field marked with this usage will get Inserted.
        /// However, the Field will only be used as Criteria for Update
        /// and Delete operations
        /// </summary>
        Key,
        /// <summary>
        /// Value of Field marked with this usage is used in Update or Delete operations
        /// as Criteria only. This field will be ignored in Insert operation.
        /// </summary>
        CriteriaOnly,
        /// <summary>
        /// Value of Field marked with this usage is used in Update or Delete operations
        /// as Criteria only. AutoIncrement Field will have this usage.
        /// </summary>
        Identity,
        /// <summary>
        /// Field marked with this usage is both AutoIncremental as well as Primary Key.
        /// Only be used in Update or Delete Operations
        /// </summary>
        IdentityKey,
        /// <summary>
        /// Value of Field marked with this usage will completely ignored in all
        /// operations. Computed field which does not exist in table can have
        /// this usage.
        /// </summary>
        None,
    }
    #endregion

    #region VALIDATE TYPE
    /// <summary>
    /// Enum ValidateType
    /// </summary>
    [Flags]
    public enum ValidateType : uint
    {
        /// <summary>
        /// The none
        /// </summary>
        None = Numbers.Flag0,

        /// <summary>
        /// Allows date in dd-MM-yyyy format
        /// </summary>
        ddMMyyyy = Numbers.Flag1,

        /// <summary>
        /// Allows date in dd-MM-yy format
        /// </summary>
        ddMMyy = Numbers.Flag2,

        /// <summary>
        /// Allows date in MM-dd-yyyy format
        /// </summary>
        MMddyyyy = Numbers.Flag3,

        /// <summary>
        /// Allows date in MM-dd-yy format
        /// </summary>
        MMddyy = Numbers.Flag4,

        /// <summary>
        /// Allows date in yyyy-MM-dd format
        /// </summary>
        yyyyMMdd = Numbers.Flag5,

        /// <summary>
        /// Allows date in yy-MM-dd format
        /// </summary>
        yyMMdd = Numbers.Flag6,

        /// <summary>
        /// Allows time in HH:mm format example 23:59
        /// </summary>
        HHmm = Numbers.Flag7,

        /// <summary>
        /// Allows time in HH:mm format with AM/PM example 11:59 PM
        /// </summary>
        hhmm12 = Numbers.Flag8,

        /// <summary>
        /// Allows time in HH:mm:SS format example 23:59:23
        /// </summary>
        HHmmss = Numbers.Flag9,

        /// <summary>
        /// Allows time in HH:mm:SS format with AM/PM example 11:59:23 PM
        /// </summary>
        hhmmss12 = Numbers.Flag10,

        /// <summary>
        /// Allows time in HH:mm:SS:MS with miliseconds format example 23:59:23:99
        /// </summary>
        HHmmssms = Numbers.Flag11,

        /// <summary>
        /// The HHMMSSMS12
        /// </summary>
        hhmmssms12 = Numbers.Flag12,

        /// <summary>
        /// Allows characters only. a to z or A to Z
        /// </summary>
        Character = Numbers.Flag13,

        /// <summary>
        /// Allows numbers only
        /// </summary>
        Number = Numbers.Flag14,

        /// <summary>
        /// Allows anything except numbers i.e. 0 to 9 can not be written
        /// </summary>
        NotNumber = Numbers.Flag15,

        /// <summary>
        /// Allows positive intiger numbers only
        /// </summary>
        PositiveNumber = Numbers.Flag16,

        /// <summary>
        /// Allows decimal number only
        /// </summary>
        DecimalNumber = Numbers.Flag17,

        /// <summary>
        /// Allows numbers with + or - signs
        /// </summary>
        NumberWithPlusAndMinus = Numbers.Flag18,

        /// <summary>
        /// Allows number with +,-,/,*,% signs
        /// </summary>
        NumberWithCalculatingSigns = Numbers.Flag19,

        /// <summary>
        /// Allows true, false, 0 and -1 only
        /// </summary>
        Boolean = Numbers.Flag20,

        /// <summary>
        /// The dd m myyyy h HMM
        /// </summary>
        ddMMyyyyHHmm = ddMMyyyy | HHmm,

        /// <summary>
        /// The dd m myyyyhhmm12
        /// </summary>
        ddMMyyyyhhmm12 = ddMMyyyy | hhmm12,

        /// <summary>
        /// The dd m myyyy h HMMSS
        /// </summary>
        ddMMyyyyHHmmss = ddMMyyyy | HHmmss,

        /// <summary>
        /// The dd m myyyyhhmmss12
        /// </summary>
        ddMMyyyyhhmmss12 = ddMMyyyy | hhmmss12,

        /// <summary>
        /// The dd m myy h HMM
        /// </summary>
        ddMMyyHHmm = ddMMyy | HHmm,

        /// <summary>
        /// The dd m myyhhmm12
        /// </summary>
        ddMMyyhhmm12 = ddMMyy | hhmm12,

        /// <summary>
        /// The dd m myy h HMMSS
        /// </summary>
        ddMMyyHHmmss = ddMMyy | HHmmss,

        /// <summary>
        /// The dd m myyhhmmss12
        /// </summary>
        ddMMyyhhmmss12 = ddMMyy | hhmmss12,

        /// <summary>
        /// The yyyy m MDD h HMM
        /// </summary>
        yyyyMMddHHmm = yyyyMMdd | HHmm,

        /// <summary>
        /// The yyyy m MDDHHMM12
        /// </summary>
        yyyyMMddhhmm12 = yyyyMMdd | hhmm12,

        /// <summary>
        /// The yyyy m MDD h HMMSS
        /// </summary>
        yyyyMMddHHmmss = yyyyMMdd | HHmmss,

        /// <summary>
        /// The yyyy m MDDHHMMSS12
        /// </summary>
        yyyyMMddhhmmss12 = yyyyMMdd | hhmmss12,

        /// <summary>
        /// Allows characters and numbers only  a to z or A to Z or 0 to 9
        /// </summary>
        CharacterAndNumber = Character | Number,

        /// <summary>
        /// Allows positive decimal numbers only
        /// </summary>
        PositiveDecimalNumber = PositiveNumber | DecimalNumber,
    }
    #endregion

    #region AND/OR
    /// <summary>
    /// Enum AndOr
    /// </summary>
    public enum AndOr : byte
    {
        /// <summary>
        /// The or
        /// </summary>
        OR,
        /// <summary>
        /// The and
        /// </summary>
        AND
    }
    #endregion

    #region CRITERIA
    /// <summary>
    /// Enum Criteria
    /// </summary>
    public enum Criteria : sbyte
    {
        /// <summary>
        /// The equal
        /// </summary>
        Equal = 0,
        /// <summary>
        /// The greater than
        /// </summary>
        GreaterThan = 1,
        /// <summary>
        /// The less than
        /// </summary>
        LessThan = 2,
        /// <summary>
        /// The occurs
        /// </summary>
        Occurs = 3,
        /// <summary>
        /// The begins with
        /// </summary>
        BeginsWith = 4,
        /// <summary>
        /// The ends with
        /// </summary>
        EndsWith = 5,
        /// <summary>
        /// The occurs no case
        /// </summary>
        OccursNoCase = 6,
        /// <summary>
        /// The begins with no case
        /// </summary>
        BeginsWithNoCase = 7,
        /// <summary>
        /// The ends with no case
        /// </summary>
        EndsWithNoCase = 8,
        /// <summary>
        /// The string equal
        /// </summary>
        StringEqual = 9,
        /// <summary>
        /// The string equal no case
        /// </summary>
        StringEqualNoCase = 10,
        /// <summary>
        /// The string number greater than
        /// </summary>
        StringNumGreaterThan = 11,
        /// <summary>
        /// The string number less than
        /// </summary>
        StringNumLessThan = 12,
        /// <summary>
        /// The not equal
        /// </summary>
        NotEqual = -1,
        /// <summary>
        /// The not greater than
        /// </summary>
        NotGreaterThan = -2,
        /// <summary>
        /// The not less than
        /// </summary>
        NotLessThan = -3,
        /// <summary>
        /// The not occurs
        /// </summary>
        NotOccurs = -4,
        /// <summary>
        /// The not begins with
        /// </summary>
        NotBeginsWith = -5,
        /// <summary>
        /// The not ends with
        /// </summary>
        NotEndsWith = -6,
        /// <summary>
        /// The not occurs no case
        /// </summary>
        NotOccursNoCase = -7,
        /// <summary>
        /// The not begins with no case
        /// </summary>
        NotBeginsWithNoCase = -8,
        /// <summary>
        /// The not ends with no case
        /// </summary>
        NotEndsWithNoCase = -9,
        /// <summary>
        /// The not string equal
        /// </summary>
        NotStrEqual = -10,
        /// <summary>
        /// The not string equal no case
        /// </summary>
        NotStrEqualNoCase = -11,
        /// <summary>
        /// The not string greater than
        /// </summary>
        NotStringGreaterThan = -12,
        /// <summary>
        /// The not string less than
        /// </summary>
        NotStringLessThan = -13
    }
    #endregion

    #region MULTI CRITERIA
    /// <summary>
    /// Enum MultCriteria
    /// </summary>
    public enum MultCriteria : sbyte
    {
        /// <summary>
        /// The between
        /// </summary>
        Between = 0,
        /// <summary>
        /// The not between
        /// </summary>
        NotBetween = -1,
    }
    #endregion

    #region NUMERIC-CRITERIA
    public enum NumCriteria : sbyte
    {
        None = 0,

        /// <summary>
        /// The equal
        /// </summary>
        Equal = 1,

        /// <summary>
        /// The greater than
        /// </summary>
        GreaterThan = 2,

        /// <summary>
        /// The less than
        /// </summary>
        LessThan = 3,

        /// <summary>
        /// <summary>
        /// The not equal
        /// </summary>
        NotEqual = -1,

        /// <summary>
        /// The not greater than
        /// </summary>
        NotGreaterThan = -2,

        /// <summary>
        /// The not less than
        /// </summary>
        NotLessThan = -3,
    }
    #endregion

    #region MATCH BY
    /// <summary>
    /// Enum MatchBy
    /// </summary>
    public enum MatchBy : byte
    {
        /// <summary>
        /// The key
        /// </summary>
        Key,
        /// <summary>
        /// The value
        /// </summary>
        Value,

        Item,
    }
    #endregion

    #region BINDING FLAGS TYPE
    /// <summary>
    /// Enum BindingFlagType
    /// </summary>
    public enum BindingFlagType : byte
    {
        /// <summary>
        /// All member
        /// </summary>
        AllMember,
        /// <summary>
        /// The public instance
        /// </summary>
        PublicInstance,
        /// <summary>
        /// The non public instance
        /// </summary>
        NonPublicInstance,
        /// <summary>
        /// The public static
        /// </summary>
        PublicStatic,
        /// <summary>
        /// The non public static
        /// </summary>
        NonPublicStatic,
        /// <summary>
        /// The type initializer
        /// </summary>
        TypeInitializer,
        /// <summary>
        /// The public non inherited
        /// </summary>
        PublicNonInherited,

#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        /// <summary>
        /// The exact matching
        /// </summary>
        ExactMatching,
#endif
        /// <summary>
        /// All static
        /// </summary>
        AllStatic,
        /// <summary>
        /// All properties
        /// </summary>
        AllProperties,
    }
    #endregion

    #region EXTRACT INTERFACES
    /// <summary>
    /// Enum ExtractInterfaces
    /// </summary>
    public enum ExtractInterfaces : byte
    {
        /// <summary>
        /// The these only
        /// </summary>
        TheseOnly,
        /// <summary>
        /// The exclude these
        /// </summary>
        ExcludeThese,
        /// <summary>
        /// All prioritize these
        /// </summary>
        AllPrioritizeThese
    }
    #endregion

    #region EXCLUDE NESTED PARAMS
    /// <summary>
    /// Enum ExcludeNestedParams
    /// </summary>
    public enum ExcludeNestedParams : byte
    {
        /// <summary>
        /// The both genre
        /// </summary>
        BothGenre,
        /// <summary>
        /// The no exclusion
        /// </summary>
        NoExclusion,
        /// <summary>
        /// The host genre
        /// </summary>
        HostGenre,
        /// <summary>
        /// The other genre
        /// </summary>
        OtherGenre,
    }
    #endregion

    #region INSTANCE
    /// <summary>
    /// Enum Instance
    /// </summary>
    public enum InstanceType : byte
    {
        /// <summary>
        /// The normal
        /// </summary>
        Normal,
        /// <summary>
        /// The reference
        /// </summary>
        Reference,
        /// <summary>
        /// The array
        /// </summary>
        Array,
        /// <summary>
        /// The pointer
        /// </summary>
        Pointer
    }
    #endregion

    #region GET LIST
    /// <summary>
    /// Enum GetList
    /// </summary>
    public enum GetList : byte
    {
        /// <summary>
        /// The mn m list
        /// </summary>
        MnMList,
        /// <summary>
        /// The mn m collection
        /// </summary>
        MnMCollection,
        /// <summary>
        /// The stream
        /// </summary>
        List,
        /// <summary>
        /// The stack
        /// </summary>
        Stack,
        /// <summary>
        /// The queue
        /// </summary>
        Queue,
        /// <summary>
        /// The hash set
        /// </summary>
        HashSet
    }
    #endregion

    #region OPERAND
    /// <summary>
    /// Enum Operand
    /// </summary>
    public enum Operand : byte
    {
        /// <summary>
        /// The left
        /// </summary>
        Left,
        /// <summary>
        /// The right
        /// </summary>
        Right
    }
    #endregion

    #region TIME FORMAT
    /// <summary>
    /// Enum TimeFormat
    /// </summary>
    public enum TimeFormat : byte
    {
        /// <summary>
        /// The time24
        /// </summary>
        Time24,
        /// <summary>
        /// The time12
        /// </summary>
        Time12
    }
    #endregion

    #region COMPARE RETURN
    /// <summary>
    /// Enum CompareReturn
    /// </summary>
    public enum CompareReturn : byte
    {
        /// <summary>
        /// The small
        /// </summary>
        Small,
        /// <summary>
        /// The big
        /// </summary>
        Big,
        /// <summary>
        /// The nearest
        /// </summary>
        Nearest
    }
    #endregion

    #region PRESS STATE
    /// <summary>
    /// Mose state
    /// </summary>
    public enum InputState : byte
    {
        /// <summary>
        /// Mouse was pressed before and now relaesed
        /// </summary>
        Released = 0,

        /// <summary>
        /// Mouse was in released state and now pressed
        /// </summary>
        Pressed = 1,
    }
    #endregion

    #region EXPR TYPE
    /// <summary>
    /// Enum ExprType
    /// </summary>
    public enum ExprType : byte
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The function
        /// </summary>
        Function,

        /// <summary>
        /// The math
        /// </summary>
        Math,

        /// <summary>
        /// The keyword
        /// </summary>
        Keyword,

        /// <summary>
        /// The class
        /// </summary>
        Class,

        /// <summary>
        /// The array
        /// </summary>
        Array,

        /// <summary>
        /// The new
        /// </summary>
        New,

        /// <summary>
        /// The variable array
        /// </summary>
        VarArray,

        NumericSuffix,

        NameSpace,
    }
    #endregion

    #region MATH OPERATOR
    /// <summary>
    /// Enum MathOperator
    /// </summary>
    [Flags]
    public enum MathOperator : byte
    {
        None = Numbers.Flag0,

        /// <summary>
        /// The add
        /// </summary>
        Add = Numbers.Flag1,

        /// <summary>
        /// The multiply
        /// </summary>
        Multiply = Numbers.Flag2,

        /// <summary>
        /// The subtract
        /// </summary>
        Subtract = Numbers.Flag3,

        /// <summary>
        /// The divide
        /// </summary>
        Divide = Numbers.Flag4,

        /// <summary>
        /// The modulo
        /// </summary>
        Modulo = Numbers.Flag5,

        /// <summary>
        /// The negate
        /// </summary>
        Negate = Numbers.Flag6,

        /// <summary>
        /// The compare
        /// </summary>
        Compare = Numbers.Flag7,

        All = Add | Multiply | Subtract |
            Divide | Modulo | Negate | Compare,
    }
    #endregion

    #region POSITION
    [Flags]
    public enum Position : byte
    {
        Default = Numbers.Flag0,

        /// <summary>
        /// 
        /// </summary>
        Left = Numbers.Flag1,

        /// <summary>
        /// 
        /// </summary>
        Top = Numbers.Flag2,

        /// <summary>
        /// 
        /// </summary>
        Right = Numbers.Flag3,

        /// <summary>
        /// 
        /// </summary>
        Bottom = Numbers.Flag4,

        /// <summary>
        /// 
        /// </summary>
        All = Left | Top | Right | Bottom,
    }
    #endregion

    #region DIRECTION
    public enum Direction : byte
    {
        Horizontal,
        Vertical,
    }
    #endregion 

    #region STRING COMPARISON
    //
    // Summary:
    //     Specifies the culture, case, and sort rules to be used by certain overloads of
    //     the System.String.Compare(System.String,System.String) and System.String.Equals(System.Object)
    //     methods.
    public enum StringComparison
    {
        //
        // Summary:
        //     Compare strings using culture-sensitive sort rules and the current culture.
        CurrentCulture = 0,
        //
        // Summary:
        //     Compare strings using culture-sensitive sort rules, the current culture, and
        //     ignoring the case of the strings being compared.
        CurrentCultureIgnoreCase = 1,
        //
        // Summary:
        //     Compare strings using culture-sensitive sort rules and the invariant culture.
        InvariantCulture = 2,
        //
        // Summary:
        //     Compare strings using culture-sensitive sort rules, the invariant culture, and
        //     ignoring the case of the strings being compared.
        InvariantCultureIgnoreCase = 3,
        //
        // Summary:
        //     Compare strings using ordinal (binary) sort rules.
        Ordinal = 4,
        //
        // Summary:
        //     Compare strings using ordinal (binary) sort rules and ignoring the case of the
        //     strings being compared.
        OrdinalIgnoreCase = 5
    }
    #endregion

    #region SERIALIZATION STATUS
    [Flags]
    public enum SerializationStatus
    {
        None = Numbers.Flag0,
        Serialized = Numbers.Flag1,
        DeSerialized = Numbers.Flag2,
    }
    #endregion

    #region OPERATE COMMAND
    public enum ModifyCommand: byte
    {
        Replace,
        Add,
        Remove
    }
    #endregion

    #region CRITERIA MODE
    /// <summary>
    /// Enum CriteriaMode
    /// </summary>
    public enum CriteriaMode
    {
        /// <summary>
        /// The include
        /// </summary>
        Include,
        /// <summary>
        /// The exclude
        /// </summary>
        Exclude
    }
    #endregion

    #region FILTER STATUS
    /// <summary>
    /// Enum FilterStatus
    /// </summary>
    public enum FilterStatus
    {
        /// <summary>
        /// The none
        /// </summary>
        None,
        /// <summary>
        /// The started
        /// </summary>
        Started,
        /// <summary>
        /// The applied
        /// </summary>
        Applied,
    }
    #endregion

    #region COLLECTION OPERATION
    /// <summary>
    /// Enum CollectionOperation
    /// </summary>
    [Flags]
    public enum ListOperation : ushort
    {
        None = Numbers.Flag0,

        /// <summary>
        /// The end
        /// </summary>
        ProcessStart = Numbers.Flag1,

        /// <summary>
        /// The start
        /// </summary>
        ProcessEnd = Numbers.Flag2,

        /// <summary>
        /// The in progess
        /// </summary>
        ProcessContinue = Numbers.Flag3,

        /// <summary>
        /// 
        /// </summary>
        BulkOperation = Numbers.Flag4,

        /// <summary>
        /// The filter change
        /// </summary>
        FilterChange = Numbers.Flag5,

        /// <summary>
        /// The reset
        /// </summary>
        Reset = Numbers.Flag6,

        /// <summary>
        /// The update
        /// </summary>
        Update = Numbers.Flag7,

        /// <summary>
        /// 
        /// </summary>
        IndexChange = Numbers.Flag8,

        /// <summary>
        /// The check changed
        /// </summary>
        CheckChanged = Numbers.Flag9,

        /// <summary>
        /// The add
        /// </summary>
        Add = Numbers.Flag10 | FilterChange,

        /// <summary>
        /// The insert
        /// </summary>
        Insert = Numbers.Flag11 | FilterChange,

        /// <summary>
        /// The remove
        /// </summary>
        Remove = Numbers.Flag12 | FilterChange,

        /// <summary>
        /// The move
        /// </summary>
        Change = Numbers.Flag13,

        /// <summary>
        /// The change
        /// </summary>
        Swap = Numbers.Flag14,

        /// <summary>
        /// The add range
        /// </summary>
        AddRange = Add | BulkOperation,

        /// <summary>
        /// The insert range
        /// </summary>
        InsertRange = Insert | BulkOperation,

        /// <summary>
        /// The remove range
        /// </summary>
        RemoveRange = Remove | BulkOperation,

        /// <summary>
        /// The change of indices
        /// </summary>
        ChangeOfIndices = IndexChange | BulkOperation,

        /// <summary>
        /// The clear
        /// </summary>
        Clear = Remove | Reset | BulkOperation,

        ChecksChanged = CheckChanged | BulkOperation,

        Relocate =  ChangeOfIndices | Change,

        /// <summary>
        /// 
        /// </summary>
        Sort = ChangeOfIndices | Reset,
    }
    #endregion

    #region PROCESS-STATUS
    /// <summary>
    /// Enum ProcessStatus
    /// </summary>
    public enum ProcessStatus : byte
    {
        None = Numbers.Flag0,

        /// <summary>
        /// The start
        /// </summary>
        Start = Numbers.Flag1,

        /// <summary>
        /// The end
        /// </summary>
        End = Numbers.Flag2,

        /// <summary>
        /// The in progess
        /// </summary>
        InProgess = Numbers.Flag3,
    }
    #endregion

    #region SCHEDULER STATUS
    [Flags]
    public enum LoopStatus : byte
    {
        None = Numbers.Flag0,

        /// <summary>
        /// Tells GWS that the loop is running.
        /// </summary>
        IsRunning = Numbers.Flag1,

        /// <summary>
        /// 
        /// </summary>
        IsRunningProxy = Numbers.Flag2,

        /// <summary>
        /// Tells GWS that loop is busy performing the action it is specified to perform.
        /// </summary>
        IsBusy = Numbers.Flag3,

        IsSkipping = Numbers.Flag4,

        /// <summary>
        /// Tells GWS that loop is a schedule
        /// </summary>
        IsQueued = Numbers.Flag5,

        IsBusyRunning = IsBusy | IsRunning,
    }
    #endregion

    #region CHANGE TYPE
    /// <summary>
    /// Enum ChangeType
    /// </summary>
    public enum DateChangeType
    {
        /// <summary>
        /// The date replacement
        /// </summary>
        DateReplacement,
        /// <summary>
        /// The day increment
        /// </summary>
        DayIncrement,
        /// <summary>
        /// The day replacement
        /// </summary>
        DayReplacement,
        /// <summary>
        /// The month increment
        /// </summary>
        MonthIncrement,
        /// <summary>
        /// The month replacement
        /// </summary>
        MonthReplacement,
        /// <summary>
        /// The year increment
        /// </summary>
        YearIncrement,
        /// <summary>
        /// The year replacement
        /// </summary>
        YearReplacement
    }
    #endregion

    #region DATE INTERVAL
    /// <summary>
    /// Enum DateInterval
    /// </summary>
    public enum DateInterval
    {
        /// <summary>
        /// The year
        /// </summary>
        Year,
        /// <summary>
        /// The month
        /// </summary>
        Month,
        /// <summary>
        /// The weekday
        /// </summary>
        Weekday,
        /// <summary>
        /// The day
        /// </summary>
        Day,
        /// <summary>
        /// The hour
        /// </summary>
        Hour,
        /// <summary>
        /// The minute
        /// </summary>
        Minute,
        /// <summary>
        /// The second
        /// </summary>
        Second
    }
    #endregion

    #region HM Difference
    /// <summary>
    /// Enum HMDifference
    /// </summary>
    public enum HMDifference
    {
        /// <summary>
        /// As it is
        /// </summary>
        AsItIs,
        /// <summary>
        /// The positive
        /// </summary>
        Positive,
        /// <summary>
        /// The zero if negative
        /// </summary>
        ZeroIFNegative
    }
    #endregion

    #region RETURNDATE
    /// <summary>
    /// Enum ReturnDate
    /// </summary>
    public enum ReturnDate
    {
        /// <summary>
        /// The smaller
        /// </summary>
        Smaller,
        /// <summary>
        /// The bigger
        /// </summary>
        Bigger,
        /// <summary>
        /// The near match
        /// </summary>
        NearMatch
    }
    #endregion

    #region AXIS-ITEM-PROCESS-RESULT
    public enum AxisItemProcessResult : byte
    {
        Sucess,
        MoveNextPixel,
        MoveNextFragment ,
        MoveNextLine ,
        ExitLoop ,
    }
    #endregion
}
