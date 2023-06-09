using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    #region ITYPE
    /// <summary>
    /// Represents an object which has an information about priority index from being wiped from screen.
    /// Very important for handling multi-threaded parallel running rendering tasks.
    /// </summary>
    public interface IType : IParameter
    {
        /// <summary>
        /// Gets GWS assigned type of the object for the purpose of handling rendering operation.
        /// </summary>
        ObjType Type { get; }
    }

    internal interface IExType : IType, IParameter { }
    #endregion

#if DevSupport
    public
#else
    internal
#endif
    struct ScrObj: IExType, IProperty<ObjType> 
    {
        public ScrObj(ObjType value)
        {
            Value = value;
        }

        public readonly ObjType Value;
        ObjType IValue<ObjType>.Value => Value;
        object IValue.Value => Value;
        ObjType IType.Type => Value;

        public static implicit operator ObjType (ScrObj objType)=>
            objType.Value;

        public static explicit operator ScrObj(ObjType type)=>
            new ScrObj(type);

        public override string ToString() => Value.ToString();
    }
}
