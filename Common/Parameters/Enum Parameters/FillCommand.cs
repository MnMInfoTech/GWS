/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    #region ICOMMAND
    /// <summary>
    /// Represents an object which has render mode information for current processing.
    /// </summary>
    public interface IFillCommand : IProperty  
    {
        FillCommand FillCommand { get; }
    }
    #endregion

    #region DRAW MODE
    partial class Parameters
    {
        struct _FillCommand : IFillCommand, IProperty<FillCommand>, IModifier<IFillCommand>, IModifier<FillCommand>
        {
            public readonly FillCommand Value;
            readonly ModifyCommand ModifyCommand;

            public _FillCommand(FillCommand command, ModifyCommand enumCommand = 0)
            {
                Value = command;
                ModifyCommand = enumCommand;
            }

            FillCommand IValue<FillCommand>.Value => Value;
            object IValue.Value => Value;
            FillCommand IFillCommand.FillCommand => Value;
            ModifyCommand IModifyCommandHolder.ModifyCommand => ModifyCommand;

            public IFillCommand Modify(IFillCommand other)
            {
                return new _FillCommand(Modify(other?.FillCommand ?? 0));
            }
            public FillCommand Modify(FillCommand other)
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

            public static implicit operator _FillCommand(FillCommand renderMode) =>
                new _FillCommand(renderMode);

            public override string ToString() =>
                Value.ToString();
        }

        #region TO COMMAND
        public static IParameter ToParameter(this FillCommand command) =>
            new _FillCommand(command, ModifyCommand.Replace);

        public static IParameter Add(this FillCommand command) =>
            new _FillCommand(command, ModifyCommand.Add);

        public static IParameter Subtract(this FillCommand command) =>
            new _FillCommand(command, ModifyCommand.Remove);

        public static IParameter Replace(this FillCommand command) =>
            new _FillCommand(command, ModifyCommand.Replace);
        #endregion

    }
    #endregion
}
