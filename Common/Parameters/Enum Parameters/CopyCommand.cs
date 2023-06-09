/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    #region ICOPY COMMAND
    /// <summary>
    /// Represents an object which has copy command information for current processing.
    /// </summary>
    public interface ICopyCommand
    {
        CopyCommand CopyCommand { get; }
    }
    #endregion 

    #region COPY COMMAND
    partial class Parameters
    {
        struct _CopyCommand : ICopyCommand, IInLineParameter, IProperty<CopyCommand>, IModifier<CopyCommand>, IModifier<ICopyCommand>
        {
            readonly CopyCommand Value;
            readonly ModifyCommand ModifyCommand;

            public _CopyCommand(CopyCommand command, ModifyCommand enumCommand = 0)
            {
                Value = command;
                ModifyCommand = enumCommand;
            }

            CopyCommand IValue<CopyCommand>.Value => Value;
            object IValue.Value => Value;
            CopyCommand ICopyCommand.CopyCommand => Value;
            ModifyCommand IModifyCommandHolder.ModifyCommand => ModifyCommand;

            public ICopyCommand Modify(ICopyCommand other)
            {
                return new _CopyCommand(Modify(other?.CopyCommand ?? 0));
            }
            public CopyCommand Modify(CopyCommand other)
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
        public static IInLineParameter Add(this CopyCommand command) =>
            new _CopyCommand(command, ModifyCommand.Add);

        public static IInLineParameter Subtract(this CopyCommand command) =>
            new _CopyCommand(command, ModifyCommand.Remove);

        public static IInLineParameter Replace(this CopyCommand command) =>
            new _CopyCommand(command, ModifyCommand.Replace);
        #endregion
    }
    #endregion
}
