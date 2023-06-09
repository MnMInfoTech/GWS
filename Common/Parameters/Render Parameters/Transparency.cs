/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/
namespace MnM.GWS
{
    #region ITRANSPARENCY
    /// <summary>
    /// Represents an objet which has an information about current rendering process.
    /// Very important for handling multi-threaded parallel running rendering tasks.
    /// </summary>
    public interface ITransparency 
    {
        /// <summary>
        /// Gets Transparency value to apply on foreground while blending.
        /// </summary>
        byte Transparency { get; }
    }
    #endregion

    partial class Parameters
    {
        #region TRANSPARENCY
        struct pTransparency : ITransparency, IInLineParameter, IProperty<byte>
        {
            public readonly byte Value;

            public pTransparency(byte transparency)
            {
                Value = transparency;
            }
            public pTransparency(ITransparency transparency)
            {
                Value = transparency.Transparency;
            }
            byte ITransparency.Transparency => Value;
            object IValue.Value => this;
            byte IValue<byte>.Value => Value;

            public override string ToString() =>
                Value.ToString();
        }
        #endregion

        #region TO TRANSPARENCY
        public static IParameter ToTransparency(this byte value) =>
            new pTransparency(value);
        #endregion
    }
}
