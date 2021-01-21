/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
#if AllHidden
    partial class NativeFactory
    {
#else
    public
#endif
        sealed partial class Text : IText
    {
        #region VARIABLES
        ITextStyle drawStyle, oldDrawStyle;
        IFont font;
        IList<IGlyph> Data;
        string text;
        float X, Y, Width, Height;
        #endregion

        #region CONSTRUCTORS
        internal Text() { }

        /// <summary>
        /// Cretes new text object with given parameters.
        /// </summary>
        /// <param name="glyphs">A list of processed glyphs collection from font</param>
        /// <param name="drawStyle">A specific drawstyle to use to measure and draw glyphs if desired so</param>
        /// <param name="destX">X cordinate of destination location where glyphs to be drawn</param>
        /// <param name="destY">X cordinate of destination location where glyphs to be drawn</param>
        public Text(IList<IGlyph> glyphs, ITextStyle drawStyle = null, int? destX = null, int? destY = null) : this()
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
        public Text(IFont font, string text, int destX, int destY, ITextStyle drawStyle = null) : this()
        {
            Initialize(font, text, drawStyle, destX, destY);
        }
        void Initialize(IFont font, string text, ITextStyle drawStyle = null, int? destX = null, int? destY = null)
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
        public ITextStyle DrawStyle => drawStyle;
        public bool Changed { get; private set; }
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
        float IPointF.X => X;
        float IPointF.Y => Y;
        float ISizeF.Width => Width;
        float ISizeF.Height => Height;
        #endregion

        #region DRAW TO
        public bool Draw(IWritable buffer, ISettings Settings)
        {
            bool success = false;
            Draw2(buffer, Settings, ref success);
            return success;
        }
        partial void Draw2(IWritable buffer, ISettings Settings, ref bool success);
        #endregion

        #region TO SHAPE
        public IEnumerable<VectorF> Figure() => null;
        #endregion

        #region CONTAINS
        public bool Contains(float x, float y)
        {
            if (x < X || y < Y || x > X + Width || y > Y + Height)
                return false;
            return true;
        }
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
        public void ChangeDrawStyle(ITextStyle value, bool temporary = true)
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
        void Change(ITextStyle drawStyle = null, int? destX = null, int? destY = null)
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
        public RectangleF MeasureText(ITextStyle style = null)
        {
            if (Changed)
            {
                ChangeDrawStyle(style);
                var info = Font.MeasureGlyphs(Data, DrawX, DrawY, DrawStyle);
                X = info.X;
                Y = info.Y;
                Width = info.Width;
                Height = info.Height;
                Data = info.ToArray();
                RestoreDrawStyle();
            }
            return new RectangleF(X, Y, Width, Height);
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
            g.X = X;
            g.Y = Y;
            g.Width = Width;
            g.Height = Height;
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
#if AllHidden
    }
#endif
}
#endif
