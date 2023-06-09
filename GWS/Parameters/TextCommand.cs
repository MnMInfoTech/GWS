/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/
#if (GWS || Window)

namespace MnM.GWS
{
    public interface ITextCommand: IParameter
    {
        /// <summary>
        /// Gets text command to control text measuring and rendering operations.
        /// </summary>
        TextCommand Command { get; }
    }

    partial class Parameters
    {
        struct pTextCommand: ITextCommand
        {
            TextCommand Command;

            public pTextCommand(TextCommand command)
            {
                Command = command;
            }
            TextCommand ITextCommand.Command => Command;
        }

        public static ITextCommand ToParameter(this TextCommand textCommand) =>
            new pTextCommand(textCommand);
    }
}
#endif
