/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    #region IUPDATE COMMAND
    /// <summary>
    /// Represents an object which has update command information for current processing.
    /// </summary>
    public interface IUpdateCommand
    {
        UpdateCommand UpdateCommand { get; }
    }
    #endregion

    #region UPDATE COMMAND
    partial class Parameters
    {
        struct _UpdateCommand : IUpdateCommand, IInLineParameter, IProperty<UpdateCommand>, IModifier<UpdateCommand>, IModifier<IUpdateCommand>
        {
            readonly UpdateCommand Value;
            readonly ModifyCommand ModifyCommand;

            public _UpdateCommand(UpdateCommand command, ModifyCommand enumCommand = 0)
            {
                Value = command;
                ModifyCommand = enumCommand;
            }

            UpdateCommand IValue<UpdateCommand>.Value => Value;
            object IValue.Value => Value;
            UpdateCommand IUpdateCommand.UpdateCommand => Value;
            ModifyCommand IModifyCommandHolder.ModifyCommand => ModifyCommand;

            public IUpdateCommand Modify(IUpdateCommand other)
            {
                return new _UpdateCommand(Modify(other?.UpdateCommand ?? 0));
            }
            public UpdateCommand Modify(UpdateCommand other)
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
        public static IInLineParameter Add(this UpdateCommand command) =>
            new _UpdateCommand(command, ModifyCommand.Add);

        public static IInLineParameter Subtract(this UpdateCommand command) =>
            new _UpdateCommand(command, ModifyCommand.Remove);

        public static IInLineParameter Replace(this UpdateCommand command) =>
            new _UpdateCommand(command, ModifyCommand.Replace);
        #endregion
    }
    #endregion
}
