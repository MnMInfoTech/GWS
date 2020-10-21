/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    sealed class Text : IText
    {
        #region VARIABLES
        TextDrawStyle drawStyle, oldDrawStyle;
        IFont font;
        IList<IGlyph> Data;
        RectangleF area;
        string text;
        #endregion

        #region CONSTRUCTORS
        Text() { }

        /// <summary>
        /// Cretes new text object with given parameters.
        /// </summary>
        /// <param name="glyphs">A list of processed glyphs collection from font</param>
        /// <param name="drawStyle">A specific drawstyle to use to measure and draw glyphs if desired so</param>
        /// <param name="destX">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="destY">X cordinate of destination location where glyphs to be drawn</param>
        public Text(IList<IGlyph> glyphs, TextDrawStyle drawStyle = null, int? destX = null, int? destY = null) : this()
        {
            Data = glyphs;
            text = new string(Data.Select(x => x.Character).ToArray());

            ChangeDrawStyle(drawStyle ?? new TextDrawStyle(), false);
            if (destX != null)
                DrawX = destX.Value;
            if (destY != null)
                DrawY = destY.Value;
            ID = "Text".NewID();
        }

        /// <summary>
        /// Cretes new text object with given parameters.
        /// </summary>
        /// <param name="font">the font object to be used to get glyphs</param>
        /// <param name="text">A text string to process to obtain glyphs collection from font</param>
        /// <param name="destX">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="destY">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="drawStyle">A specific drawstyle to use to measure and draw glyphs if desired so</param>
        public Text(IFont font, string text, int destX, int destY, TextDrawStyle drawStyle = null) : this()
        {
            Initialize(font, text, drawStyle, destX, destY);
        }
        void Initialize(IFont font, string text, TextDrawStyle drawStyle = null, int? destX = null, int? destY = null)
        {
            this.font = font;
            this.text = text;
            ID = "Text".NewID();
            Change(drawStyle, destX, destY);
        }
        #endregion

        #region PROPERTIES
        public IGlyph this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
        public int Count => Data.Count;
        public TextDrawStyle DrawStyle => drawStyle;
        public bool Changed { get; private set; }
        public RectangleF Bounds
        {
            get
            {
                if (Data.Count == 0)
                    return RectangleF.Empty;
                if (!area)
                {
                    Changed = true;
                    Measure();
                    Changed = false;
                }
                return area;
            }
        }
        public int DrawX { get; private set; }
        public int DrawY { get; private set; }
        public string Name => "Text";
        public string ID { get; private set; }
        public string Value
        {
            get => text;
            set
            {
                text = value;
                Change();
            }
        }
        public IFont Font
        {
            get => font ?? Factory.SystemFont;
            set
            {
                font = value;
                Change();
            }
        }
        RectangleF IBoundsF.Bounds => Bounds;
        #endregion

        #region DRAW TO
        public bool Draw(IBuffer buffer, IReadContext context, out IPen Pen)
        {
            if (Changed)
                Measure();

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

        #region MEASURE
        public IText Measure()
        {
            if (Data.Count == 0 || !Changed)
                return this;
            MeasureText();
            Changed = false;
            return this;
        }
        public int GetKerning(int i)
        {
            if (i == 0)
                return 0;
            var c = Data[i].Character;

            if (Font.Kerning && i > 0 && c != 0)
                return Font.GetKerning(Data[i - 1].Character, c);
            return 0;
        }
        #endregion

        #region CHANGE
        public void ChangeDrawStyle(TextDrawStyle value, bool temporary = true)
        {
            if (value == null)
                return;

            Changed = Changed || value != drawStyle;
            if (!temporary)
                oldDrawStyle = value;
            drawStyle = value;
        }

        public void RestoreDrawStyle()
        {
            drawStyle = oldDrawStyle;
        }
        void Change(TextDrawStyle drawStyle = null, int? destX = null, int? destY = null)
        {
            if (drawStyle != null)
                ChangeDrawStyle(drawStyle, false);

            else if (this.drawStyle == null)
                ChangeDrawStyle(new TextDrawStyle(), false);

            if (destX != null)
                DrawX = destX.Value;
            if (destY != null)
                DrawY = destY.Value;

            Data = new IGlyph[(text + "").Length];

            var fnt = Font;
            this.drawStyle.LineHeight = fnt.Info.LineHeight.Ceiling();
            for (int i = 0; i < Data.Count; i++)
                Data[i] = fnt.GetGlyph(text[i]);

            Changed = true;
        }
        public void SetDrawXY(int? drawX = null, int? drawY = null)
        {
            if (drawX == null && drawY == null)
                return;
            if (drawX != null)
                DrawX = drawX.Value;
            if (drawY != null)
                DrawY = drawY.Value;
            Changed = true;
        }
        #endregion

        #region MEASURE TEXT
        public RectangleF MeasureText(TextDrawStyle style = null)
        {
            if (!Changed)
                return Bounds;
            ChangeDrawStyle(style);
            var info = Font.MeasureGlyphs(Data, DrawX, DrawY, DrawStyle);
            area = info.Bounds;
            Data = info.ToArray();
            RestoreDrawStyle();
            return area;
        }
        #endregion

        #region CLONE
        public object Clone()
        {
            var g = new Text();
            g.ID = Name.NewID();

            g.Data = Data.ToArray();
            g.drawStyle = drawStyle.Clone() as TextDrawStyle;
            g.oldDrawStyle = oldDrawStyle.Clone() as TextDrawStyle;
            g.DrawX = DrawX;
            g.DrawY = DrawY;
            g.font = font;
            g.area = Bounds;
            return g;
        }
        #endregion

        #region IEnumerable<IGlyph>
        IEnumerator<IGlyph> IEnumerable<IGlyph>.GetEnumerator() =>
            Data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            Data.GetEnumerator();
        #endregion
    }

}
