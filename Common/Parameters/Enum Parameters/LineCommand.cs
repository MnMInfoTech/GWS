/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    #region ILINETYPE
    /// <summary>
    /// Represents an object which has linetype information for current processing.
    /// </summary>
    public interface ILineCommand 
    {
        LineCommand LineCommand { get; }
    }
    #endregion

    #region LINE TYPE
    partial class Parameters
    {
        struct _LineCommand : ILineCommand, IInLineParameter, IProperty<LineCommand>, IModifier<LineCommand>, IModifier<ILineCommand>
        {
            readonly LineCommand Value;
            readonly ModifyCommand ModifyCommand;

            public _LineCommand(LineCommand lineType, ModifyCommand enumCommand = 0)
            {
                Value = lineType;
                ModifyCommand = enumCommand;
            }

            LineCommand IValue<LineCommand>.Value => Value;
            object IValue.Value => Value;
            LineCommand ILineCommand.LineCommand => Value;
            ModifyCommand IModifyCommandHolder.ModifyCommand => ModifyCommand;

            public ILineCommand Modify(ILineCommand other)
            {
                return new _LineCommand(Modify(other?.LineCommand ?? 0));
            }
            public LineCommand Modify(LineCommand other)
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

        #region TO LINE TYPE
        public static IInLineParameter ToParameter(this LineCommand command) =>
            new _LineCommand(command, ModifyCommand.Replace);

        public static IInLineParameter Add(this LineCommand command) =>
            new _LineCommand(command, ModifyCommand.Add);

        public static IInLineParameter Subtract(this LineCommand command) =>
            new _LineCommand(command, ModifyCommand.Remove);

        public static IInLineParameter Replace(this LineCommand command) =>
            new _LineCommand(command, ModifyCommand.Replace);
        #endregion
    }
    #endregion
}
