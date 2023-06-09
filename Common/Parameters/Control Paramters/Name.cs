namespace MnM.GWS
{
    partial class Parameters
    {
        struct Name : IName, IParameter, IProperty<string>
        {
            public readonly string Value;
            public Name(string value)
            {
                Value = value;
            }
            string IName.Name => Value;
            string IValue<string>.Value => Value;
            object IValue.Value => Value;
        }

        public static IProperty<string> ToName(this string name) =>
            new Name(name);
    }
}
