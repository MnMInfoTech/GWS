/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    #region ICLEAR COMMAND
    /// <summary>
    /// Represents an object which has copy command information for current processing.
    /// </summary>
    public interface IClearCommand
    {
        ClearCommand ClearCommand { get; }
    }
    #endregion 

    #region CLEAR COMMAND
    partial class Parameters
    {
        struct _ClearCommand : IClearCommand, IParameter, IModifier<ClearCommand>, IModifier<IClearCommand>, IValue<ClearCommand>
        {
            readonly ClearCommand Value;
            readonly ModifyCommand ModifyCommand;

            public _ClearCommand(ClearCommand command, ModifyCommand enumCommand = 0)
            {
                Value = command;
                ModifyCommand = enumCommand;
            }

            ClearCommand IClearCommand.ClearCommand => Value;
            ModifyCommand IModifyCommandHolder.ModifyCommand => ModifyCommand;
            ClearCommand IValue<ClearCommand>.Value => Value;
            object IValue.Value => Value;

            public IClearCommand Modify(IClearCommand other)
            {
                return new _ClearCommand(Modify(other?.ClearCommand ?? 0));
            }
            public ClearCommand Modify(ClearCommand other)
            {
                switch (ModifyCommand)
                {
                    case ModifyCommand.Replace:
                    default:
                        return Value;
                    case ModifyCommand.Add:
                        return (other | Value);
                    case ModifyCommand.Remove:
                        return (other & ~Value);
                }
            }

            public override string ToString() =>
                Value.ToString();
        }

        #region TO UPDATE COMMAND
        public static IParameter Add(this ClearCommand command) =>
            new _ClearCommand(command, ModifyCommand.Add);

        public static IParameter Subtract(this ClearCommand command) =>
            new _ClearCommand(command, ModifyCommand.Remove);

        public static IParameter Replace(this ClearCommand command) =>
            new _ClearCommand(command, ModifyCommand.Replace);
        #endregion
    }
    #endregion
}
