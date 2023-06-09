/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    #region ICOMMAND
    /// <summary>
    /// Represents an object which has command information for current processing.
    /// </summary>
    internal interface ICommand : IParameter, IValue<Command>
    { }
    #endregion

    #region COMMAND
    partial class Parameters
    {
        struct pCommand : ICommand, IModifier<ICommand>, IModifier<Command>, IValue<Command>
        {
            readonly Command Value;
            readonly ModifyCommand ModifyCommand;

            public pCommand(Command command, ModifyCommand enumCommand = 0)
            {
                Value = command;
                ModifyCommand = enumCommand;
            }

            Command IValue<Command>.Value => Value;
            object IValue.Value => Value;
            ModifyCommand IModifyCommandHolder.ModifyCommand => ModifyCommand;

            public ICommand Modify(ICommand other)
            {
                return new pCommand(Modify(other?.Value ?? 0));
            }
            public Command Modify(Command other)
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

        #region TO COMMAND
        internal static IParameter ToParameter(this Command command) =>
            new pCommand(command, ModifyCommand.Replace);

        internal static IParameter Add(this Command command) =>
            new pCommand(command, ModifyCommand.Add);

        internal static IParameter Subtract(this Command command) =>
            new pCommand(command, ModifyCommand.Remove);

        internal static IParameter Replace(this Command command) =>
            new pCommand(command, ModifyCommand.Replace);
        #endregion
    }
    #endregion
}
