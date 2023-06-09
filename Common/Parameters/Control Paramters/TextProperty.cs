/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    #region TEXT PROPERTY
    public struct TextProperty: ITextHolder, IProperty<string>, IModifier<TextProperty>, IModifier<string>
    {
        #region VARIABLES
        public readonly string Value; 
        ModifyCommand ModifyCommand;
        #endregion

        #region CONSTRUCTORS
        public TextProperty(string text, ModifyCommand operateCommand = 0)
        {
            Value = text;
            ModifyCommand = operateCommand;
        }
        public TextProperty(object text, ModifyCommand operateCommand = 0)
        {
            Value = text?.ToString();
            ModifyCommand = operateCommand;
        }
        #endregion

        #region PROPERTIES
        object IValue.Value => Value;
        string IValue<string>.Value => Value;
        string ITextHolder.Text => Value;
        ModifyCommand IModifyCommandHolder.ModifyCommand => ModifyCommand;
        #endregion

        #region MODIFY
        public TextProperty Modify(TextProperty other)
        {
            var value = other.Value?? "";
            switch (ModifyCommand)
            {
                case ModifyCommand.Replace:
                default:
                    return this;
                case ModifyCommand.Add:
                    return new TextProperty(value + Value);
                case ModifyCommand.Remove:
                    return new TextProperty(value.Replace(Value, ""));
            }
        }
        public string Modify(string other)
        {
            switch (ModifyCommand)
            {
                case ModifyCommand.Replace:
                default:
                    return Value;
                case ModifyCommand.Add:
                    return (other + Value);
                case ModifyCommand.Remove:
                    return (other.Replace(Value, ""));
            }
        }
        #endregion
    }
    #endregion
}
