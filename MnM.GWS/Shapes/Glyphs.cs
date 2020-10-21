/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public struct Glyphs : IGlyphs
    {
        #region VARIABLES
        readonly IList<IGlyph> Data;
        public readonly float MinHBY;
        public readonly bool ContainsNewLine;
        public readonly string Text;

        public static readonly Glyphs Empty = new Glyphs();
        #endregion

        #region CONSTRUCTORS
        public Glyphs(string text, RectangleF area, IList<IGlyph> glyphs, float minHBY, bool containsNewLine = false)
        {
            ID = "Glyphs".NewID();
            Bounds = area;
            this.Data = glyphs;
            MinHBY = minHBY;

            ContainsNewLine = containsNewLine;

            if (text == null)
                Text = new string(glyphs.Select(x => x.Character).ToArray());
            else
                Text = text;
        }
        #endregion

        #region PROPERTIES
        public IGlyph this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
        public RectangleF Bounds { get; private set; }
        public string ID { get; private set; }
        public int Count => Data.Count;
        RectangleF IBoundsF.Bounds => Bounds;
        public string Name => "Text";
        #endregion

        #region DRAW TO
        public bool Draw(IBuffer buffer, IReadContext context, out IPen Pen)
        {
            RectangleF? bounds = null;
            if ((buffer.Settings.Rotation) || buffer.Settings.Scale.HasScale)
                bounds = Bounds.Scale(buffer.Settings.Rotation, buffer.Settings.Scale, out _);

            Pen = buffer.Settings.GetPen(this, context, bounds);
            foreach (var item in Data)
                item.Draw(buffer, Pen, out _);
            return true;
        }
        #endregion

        #region TO SHAPE
        public IEnumerable<VectorF> ToShape() => null;
        #endregion

        #region IENUMERABLE<VectorF>
        public IEnumerator<IGlyph> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion
    }
}
