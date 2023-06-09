/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    #region IBRUSH COMMAND
    /// <summary>
    /// Represents an object which has copy command information for current processing.
    /// </summary>
    public interface IBrushCommand
    {
        BrushCommand BrushCommand { get; }
    }
    #endregion 

    #region BRUSH COMMAND
    partial class Parameters
    {
        struct _BrushCommand : IBrushCommand, IParameter, IModifier<BrushCommand>, IModifier<IBrushCommand>, IValue<BrushCommand>
        {
            readonly BrushCommand Value;
            readonly ModifyCommand ModifyCommand;

            public _BrushCommand(BrushCommand command, ModifyCommand enumCommand = 0)
            {
                Value = command;
                ModifyCommand = enumCommand;
            }

            BrushCommand IBrushCommand.BrushCommand => Value;
            ModifyCommand IModifyCommandHolder.ModifyCommand => ModifyCommand;
            BrushCommand IValue<BrushCommand>.Value => Value;
            object IValue.Value => Value;

            public IBrushCommand Modify(IBrushCommand other)
            {
                return new _BrushCommand(Modify(other?.BrushCommand ?? 0));
            }
            public BrushCommand Modify(BrushCommand other)
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
        public static IParameter Add(this BrushCommand command) =>
            new _BrushCommand(command, ModifyCommand.Add);

        public static IParameter Subtract(this BrushCommand command) =>
            new _BrushCommand(command, ModifyCommand.Remove);

        public static IParameter Replace(this BrushCommand command) =>
            new _BrushCommand(command, ModifyCommand.Replace);
        #endregion
    }
    #endregion
}
