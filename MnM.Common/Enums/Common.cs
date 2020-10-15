/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;

namespace MnM.GWS
{
    #region IMAGE FORMAT
    public enum ImageFormat
    {
        JPG = 2,
        BMP = 0,
        HDR = 3,
        TGA = 4,
        PNG = 5,
    }
    #endregion

    #region VALIDATE TYPE
    /// <summary>
    /// Enum ValidateType
    /// </summary>
    public enum ValidateType
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Allows date in dd-MM-yyyy format
        /// </summary>
        ddMMyyyy = 0x1,
        /// <summary>
        /// Allows date in dd-MM-yy format
        /// </summary>
        ddMMyy = 0x2,
        /// <summary>
        /// Allows date in MM-dd-yyyy format
        /// </summary>
        MMddyyyy = 0x4,
        /// <summary>
        /// Allows date in MM-dd-yy format
        /// </summary>
        MMddyy = 0x8,
        /// <summary>
        /// Allows date in yyyy-MM-dd format
        /// </summary>
        yyyyMMdd = 0x10,
        /// <summary>
        /// Allows date in yy-MM-dd format
        /// </summary>
        yyMMdd = 0x20,
        /// <summary>
        /// Allows time in HH:mm format example 23:59
        /// </summary>
        HHmm = 0x40,
        /// <summary>
        /// Allows time in HH:mm format with AM/PM example 11:59 PM
        /// </summary>
        hhmm12 = 0x80,
        /// <summary>
        /// Allows time in HH:mm:SS format example 23:59:23
        /// </summary>
        HHmmss = 0x100,
        /// <summary>
        /// Allows time in HH:mm:SS format with AM/PM example 11:59:23 PM
        /// </summary>
        hhmmss12 = 0x200,
        /// <summary>
        /// Allows time in HH:mm:SS:MS with miliseconds format example 23:59:23:99
        /// </summary>
        HHmmssms = 0x400,
        /// <summary>
        /// The HHMMSSMS12
        /// </summary>
        hhmmssms12 = 0x800,
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
        /// Allows characters only. a to z or A to Z
        /// </summary>
        Character = 0x1000,
        /// <summary>
        /// Allows numbers only
        /// </summary>
        Number = 0x2000,
        /// <summary>
        /// Allows characters and numbers only  a to z or A to Z or 0 to 9
        /// </summary>
        CharacterAndNumber = Character | Number,
        /// <summary>
        /// Allows anything except numbers i.e. 0 to 9 can not be written
        /// </summary>
        NotNumber = 0x4000,
        /// <summary>
        /// Allows positive intiger numbers only
        /// </summary>
        PositiveNumber = 0x8000,
        /// <summary>
        /// Allows decimal number only
        /// </summary>
        DecimalNumber = 0x10000,
        /// <summary>
        /// Allows positive decimal numbers only
        /// </summary>
        PositiveDecimalNumber = PositiveNumber | DecimalNumber,
        /// <summary>
        /// Allows numbers with + or - signs
        /// </summary>
        NumberWithPlusAndMinus = 0x20000,
        /// <summary>
        /// Allows number with +,-,/,*,% signs
        /// </summary>
        NumberWithCalculatingSigns = 0x40000,
        /// <summary>
        /// Allows true, false, 0 and -1 only
        /// </summary>
        Boolean = 0x80000
    }
    #endregion

    #region AND/OR
    /// <summary>
    /// Enum AndOr
    /// </summary>
    public enum AndOr
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
    public enum Criteria
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
    public enum MultCriteria
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

    #region MATCH BY
    /// <summary>
    /// Enum MatchBy
    /// </summary>
    public enum MatchBy
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
    public enum BindingFlagType
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
        /// <summary>
        /// The exact matching
        /// </summary>
        ExactMatching,
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
    public enum ExtractInterfaces
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
    public enum ExcludeNestedParams
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
    public enum Instance
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
    public enum GetList
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
    public enum Operand
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

    #region TIMEFORMAT
    /// <summary>
    /// Enum TimeFormat
    /// </summary>
    public enum TimeFormat
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

    #region COMPARERETURN
    /// <summary>
    /// Enum CompareReturn
    /// </summary>
    public enum CompareReturn
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
    public enum State
    {
        /// <summary>
        /// Mouse was pressed before and now relaesed
        /// </summary>
        Released = 1,

        /// <summary>
        /// Mouse was in released state and now pressed
        /// </summary>
        Pressed = 2,
    }
    #endregion

    #region EXPR TYPE
    /// <summary>
    /// Enum ExprType
    /// </summary>
    public enum ExprType
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
    public enum MathOperator
    {
        None = 0x0,

        /// <summary>
        /// The add
        /// </summary>
        Add = 0x1,

        /// <summary>
        /// The multiply
        /// </summary>
        Multiply = 0x2,

        /// <summary>
        /// The subtract
        /// </summary>
        Subtract = 0x4,

        /// <summary>
        /// The divide
        /// </summary>
        Divide = 0x8,

        /// <summary>
        /// The modulo
        /// </summary>
        Modulo = 0x10,

        /// <summary>
        /// The negate
        /// </summary>
        Negate = 0x20,

        /// <summary>
        /// The compare
        /// </summary>
        Compare = 0x40,

        All = Add | Multiply | Subtract |
            Divide | Modulo | Negate | Compare,
    }
    #endregion

    #region POSITION
    [Flags]
    public enum Position
    {
        Default = 0x0,
        Left = 0x1,
        Top = 0x2,
        Right = 0x4,
        Bottom = 0x8,
        All = Left | Top | Right | Bottom,
    }
    #endregion

    #region SKEW TYPE
    [Flags]
    public enum SkewType
    {
        None = 0x0,
        Horizontal = 0x1,
        Vertical = 0x2,
        Diagonal = 0x4,
        Downsize = Horizontal | Vertical,
    }
    #endregion
}
