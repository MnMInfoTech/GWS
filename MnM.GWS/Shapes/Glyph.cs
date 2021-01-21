/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace MnM.GWS
{
#if AllHidden
    partial class NativeFactory
    {
#else
    public
#endif
    sealed partial class Glyph : _Glyph, IGlyph
    {
        #region VARIABLES
        IGlyphSlot slot;
        int x, y;
        IList<AxisLine> Data;
        bool DataInitialized;
        static string tostr = "Char: {0}, X: {1}, Y: {2}, W: {3}, H: {4}";
        #endregion

        #region CONSTRUCTORS
        Glyph() { }

        /// <summary>
        /// Create a new glyph object from a given slot.
        /// </summary>
        /// <param name="slot">Glyph slot made available by the font object</param>
        /// <returns></returns>
        public Glyph(IGlyphSlot glyph)
        {
            slot = glyph;
            Character = slot.Character;
        }
        void SetData()
        {
            if (Character == ' ')
                return;
            if (Data == null)
                Data = new Collection<AxisLine>(25);
            Data.Clear();
            var data = Data;
            FillAction action = (val1, axis, horizontal, val2, alpha, cmd) =>
            {
                int iVal1 = (int)val1;
                int iVal2 = (int)val2;

                if (val1 - iVal1 >= 0.5f)
                    ++iVal1;
                if (val2 - iVal2 >= 0.5f)
                    ++iVal2;

                data.Add(new AxisLine(iVal1, iVal2, axis, horizontal, alpha));
            };

            Font.Renderer.Process(slot.Points, slot.Contours, action, Width.Round(), Height.Round());
            DataInitialized = true;
        }
        #endregion

        #region PROPERTIES
        public override int X
        {
            get => (x + slot.X).Round();
            set => x = value;
        }
        public override int Y
        {
            get => (y + slot.Y).Round();
            set => y = value;
        }
        public override float Width => slot.Width;
        public override float Height => slot.Height;
        #endregion

        #region DRAW TO
        public override bool Draw(IWritable buffer, ISettings Settings)
        {
            bool success = false;
            Draw2(buffer, Settings, ref success);
            return success;
        }
        partial void Draw2(IWritable buffer, ISettings Settings, ref bool success);
        #endregion

        public override IEnumerable<VectorF> Figure() => null;

        #region CLONE
        public override object Clone()
        {
            var g = new Glyph();
            g.x = x;
            g.y = y;
            g.slot = slot;
            g.Data = Data;
            g.Character = Character;
            g.DataInitialized = DataInitialized;
            g.IsOutLine = IsOutLine;
            return g;
        }
        #endregion

        public override string ToString()
        {
            return string.Format(tostr, slot.Character,
               X, Y, slot.Width, slot.Height);
        }
    }
#if AllHidden
    }
#endif
}
#endif
