/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region ITEXT
    /// <summary>
    /// Represents an object which represents a text string in a drawing context.
    /// This also has a collection of glyphs.
    /// </summary>
    public interface IText : ITextItem, IShape, IGlyphLine, IHBY 
    {
        /// <summary>
        /// Gets glyph lines in this object.
        /// </summary>
        IEnumerable<IGlyphLine> Lines { get; }

        /// <summary>
        /// Gets number of glyph lines in this object.
        /// </summary>
        int LineCount { get; }
    }
    #endregion

    #region IGLYPH-LINE
    /// <summary>
    /// Represents an object representing glyph line segment - part of big text.
    /// </summary>
    public interface IGlyphLine : IReadOnlyList<IGlyph>, ITextHolder, IObject
    {
        /// <summary>
        /// Index of the first glyph available in this line.
        /// </summary>
        int Start { get; }

        /// <summary>
        /// X co-ordinate of draw location of this line.
        /// </summary>
        int DrawX { get; }

        /// <summary>
        /// Y co-ordinate of draw location of this line.
        /// </summary>
        int DrawY { get; }

        /// <summary>
        /// Gets glyph for dot character.
        /// </summary>
        IGlyph Dot { get; }
    }
    #endregion

    #region TEXT
    /// <summary>
    /// Represents an object which represents a text string in a drawing context.
    /// This also has a collection of glyphs.
    /// </summary>
    public sealed class Text : IText, IExDraw, IPropertyBag, IExRefreshProperties, IExResizable
    {
        #region VARIABLES
        IGlyph[] Data = new IGlyph[0];
        string text;
        IGlyph dot;
        Location location;
        Size size;
        Margin margin;
        IFont Font;
        bool initialized;
        static string std = "{0}";
        #endregion

        #region CONSTRUCTORS
        public Text()
        {
        }

        /// <summary>
        /// Cretes new text object with given text using default system font.
        /// </summary>
        /// <param name="text">A text string to process to obtain glyphs collection from font</param>
        public Text(string text) : this()
        {
            this.text = text;
        }

        public Text(object any) : this()
        {
            text = string.Format(std, any);
        }

        /// <summary>
        /// Cretes new text object with given parameters.
        /// </summary>
        /// <param name="glyphs">A list of processed glyphs collection from font</param>
        /// <param name="drawStyle">A specific drawstyle to use to measure and draw glyphs if desired so</param>
        public Text(IEnumerable<IGlyph> glyphs) : this()
        {
            Data = glyphs.ToArray();
            if (glyphs is ITextHolder)
                text = ((ITextHolder)glyphs).Text;
            else
                text = new string(Data.Select(x => x.Character).ToArray());
        }

        /// <summary>
        /// Cretes new text object with given parameters.
        /// </summary>
        /// <param name="font">the font object to be used to get glyphs</param>
        /// <param name="text">A text string to process to obtain glyphs collection from font</param>
        /// <param name="drawStyle">A specific drawstyle to use to measure and draw glyphs if desired so</param>
        public Text(string text, int x, int y) : this()
        {
            location = new Location(x, y);
            this.text = text;
        }
        #endregion

        #region PROPERTIES
        public IEnumerable<IGlyphLine> Lines { get; private set; }
        public int LineCount { get; private set; }
        public bool IsOutLine { get; set; }
        public IGlyph this[int index] => Data[index];
        public int Count => Data.Length;
        public float MinHBY { get; private set; }
        public int X => location.X + margin.X;
        public int Y => location.Y + margin.Y;
        public int Width => size.Width + margin.X;
        public int Height => size.Height + margin.Y;
        public bool Valid => size.Width > 0 && size.Height > 0;
        public bool IsOriginBased => location.X == 0 && location.Y == 0;
        public IPropertyBag Properties => this;
        public string Value
        {
            get => text ?? "";
            set
            {
                text = value;
                initialized = false;
            }
        }
        public bool Initialized => initialized;
        #endregion

        #region IMPLICIT INTERFACE IMPLEMENTATION
        object IValue.Value => text ?? "";
        Location ILocationHolder.Location 
        { 
            get => location;
            set => location = value;
        }
        IGlyph IGlyphLine.Dot => dot;
        int IGlyphLine.DrawX => location.X + margin.X;
        int IGlyphLine.DrawY => location.Y + margin.Y;
        int IGlyphLine.Start => 0;
        string ITextHolder.Text => text ?? "";
        #endregion

        #region PROPERTY RELATED
        T IPropertyBag.Get<T>()
        {
            switch (typeof(T).Name)
            {
                case "TextProperty":
                case "ITextProperty":
                case "ITextHolder":
                    return (T)(object)new TextProperty(text);
                case "Font":
                case "IFont":
                    return (T)(object)(Font ?? Factory.SystemFont);
                case "Location":
                case "ILocation":
                    return (T)(object)location;
                case "Size":
                case "ISize":
                    return (T)(object)size;
                case "Margin":
                case "IMargin":
                    return (T)(object)margin;
                default:
                    break;
            }
            return default(T);
        }

        bool IPropertyBag.Set<T>(T value, bool silent)
        {
            IParameter[] parameters = new IParameter[2];
            switch (typeof(T).Name)
            {
                case "Location":
                case "ILocation":
                    location = new Location(value?.Value as ILocation);
                    return true;
                case "Size":
                case "ISize":
                    size = new Size(value?.Value as ISize);
                    return true;
                case "Margin":
                case "IMargin":
                    margin = new Margin(value?.Value as IMargin);
                    return true;
                case "Initialized":
                    initialized = (bool)value.Value;
                    return true;
                case "TextProperty":
                case "ITextProperty":
                case "ITextHolder":
                    initialized = false;
                    parameters[0] = value;
                    break;
                case "Font":
                case "IFont":
                    if (value?.Value != null)
                    {
                        initialized = false;
                        parameters[1] = value;
                    }
                    break;
                default:
                    return false;
            }
            if (!initialized)
            {
                ((IExRefreshProperties)this).RefreshProperties(parameters);
                return true;
            }
            return false;
        }

        bool IPropertyBag.Contains<T>()
        {
            switch (typeof(T).Name)
            {
                case "TextProperty":
                case "ITextProperty":
                case "ITextHolder":
                    return true;
                case "Font":
                case "IFont":
                    return true;
                case "Location":
                case "ILocation":
                    return true;
                case "Size":
                case "ISize":
                    return true;
                case "Margin":
                case "IMargin":
                    return true;
                case "Initialized":
                    return true;
                default:
                    return false;
            }
        }

        IPrimitiveList<IParameter> IExRefreshProperties.RefreshProperties(IEnumerable<IParameter> parameters)
        {
            if (parameters == null)
                return null;

            bool found = false;
            IFont font = null;
            TextCommand command = 0;
            int wrapWidth = 0;
            var remaining = new PrimitiveList<IParameter>();
            bool handled;
            foreach (var parameter in parameters)
            {
                if (parameter == null)
                    continue;
                handled = false;

                if (parameter is ILocation)
                {
                    location = new Location((ILocation)parameter);
                    handled = true;
                }
                if (parameter is IUserSize)
                {
                    size = new Size((IUserSize)parameter);
                    handled = true;
                }
                if (parameter is IMargin)
                {
                    margin = new Margin((IMargin)parameter);
                    handled = true;
                }
                if (parameter is IFont)
                {
                    font = (IFont)parameter;
                    found = true;
                    handled = true;
                }
                if (parameter is ITextHolder)
                {
                    text = ((ITextHolder)parameter).Text;
                    found = true;
                    handled = true;
                }
                if (!handled)
                    remaining.Add(parameter);
            }

            if (found)
                initialized = false;

            if (initialized)
                goto EXIT;

            if (string.IsNullOrEmpty(text))
                goto EXIT;

            Font = font ?? Factory.SystemFont;
            Data = text.Select(c => Font[c]).ToArray();

            var lines = Font.MeasureGlyphs(Data, out IRectangle area, out _, out float minHBY, parameters);
            MinHBY = minHBY;
            area.GetBounds(out int x, out int y, out int w, out int h);

            size = new Size(w, h);

            if (lines.Count > 0)
            {
                LineCount = lines.Count;
                Lines = lines.Select(l => new GlyphLine(this, l));
            }
            else
            {
                Lines = null;
                LineCount = 0;
            }
            dot = Font['.'];
            initialized = true;

            EXIT:
            if (remaining.Count > 0)
                return remaining;
            return null;
        }
        #endregion

        #region DRAW
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer graphics)
        {
            if (parameters != null)
                parameters = ((IExRefreshProperties)this).RefreshProperties(parameters);

            var dstX = location.X + margin.X;
            var dstY = location.Y + margin.Y;
            var w = size.Width + margin.X;
            var h = size.Height + margin.Y;

            if (w <= 0 || h <= 0)
                return true;

            parameters.ExtractRotationScaleParameters(out IRotation Rotation, out IScale Scale);

            var rc = new Rectangle(dstX, dstY, w, h);

            bool hasScale = Rotation?.Skew?.HasScale == true || Scale?.HasScale == true;

            if (hasScale)
                rc = rc.Scale(Rotation, Scale);

            var Parameters = parameters.SetPen(rc);

            var action = graphics.CreateRenderAction(Parameters.AppendItem(new RenderBounds(rc)));

            if (LineCount == 0)
                return this.Process(action, Parameters);
            else
                Lines.Process(action, Parameters);
            return true;
        }
        #endregion

        #region MEASURE
        public IRectangle Measure(IFont font = null, IEnumerable<IParameter> parameters = null)
        {
            var Font = font ?? Factory.SystemFont;
            var Data = text.Select(c => Font[c]).ToArray();
            var lines = Font.MeasureGlyphs(Data, out IRectangle area, out _, out _, parameters);
            return area;
        }
        #endregion

        #region REFRESH
        #endregion

        #region RESIZE
        object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            var iw = ((ISize)this).Width;
            var ih = ((ISize)this).Height;

            if
            (
               (w == iw && h == ih) ||
               (w == 0 && h == 0)
            )
            {
                return this;
            }

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

            if (SizeOnlyToFit && iw > w && ih > h)
                return this;

            if (SizeOnlyToFit)
            {
                if (w < iw)
                    w = iw;
                if (h < ih)
                    h = ih;
            }

            if (string.IsNullOrEmpty(text))
                return this;
            var font = Font ?? Factory.SystemFont;

            if (Data.Length == 0)
            {
                Data = text.Select(c => font[c]).ToArray();

            }

            var Scale = new Scale(this, w, h);
            for (int i = 0; i < Data.Length; i++)
            {
                Data[i] = Data[i].RotateAndScale(0, X + iw / 2f, Y + ih / 2f, Scale);
            }

            var lines = font.MeasureGlyphs(Data, out IRectangle area, out _, out float minHBY);
            MinHBY = minHBY;
            area.GetBounds(out int x, out int y, out w, out h);

            size = new Size(w, h);

            if (lines.Count > 0)
            {
                LineCount = lines.Count;
                Lines = lines.Select(l => new GlyphLine(this, l));
            }
            else
            {
                Lines = null;
                LineCount = 0;
            }
            success = true;
            return this;
        }
        #endregion

        #region CONTAINS
        public bool Contains(float x, float y)
        {
            return x >= location.X &&
                y >= location.Y &&
                x <= location.X + size.Width &&
                y <= location.Y + size.Height;
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            location = GWS.Location.Empty;
            return this;
        }
        #endregion

        #region IENUMERABLE<IGLYPH>
        IEnumerator<IGlyph> IEnumerable<IGlyph>.GetEnumerator() =>
            ((IEnumerable<IGlyph>)Data).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            Data.GetEnumerator();
        #endregion

        #region OPERATOR OVERLOADING
        public static implicit operator string(Text t) =>
            t.text ?? "";
        public static Text operator +(Text t, string text)
        {
            t.text += text;
            t.initialized = false;
            return t;
        }
        #endregion

        public override string ToString()
        {
            return text;
        }

        class GlyphLine: IGlyphLine
        {
            #region VARIABLES
            int start, count, drawY;
            Text Parent;
            int x, y, w, h;
            #endregion

            #region CONSTRUCTOR
            public GlyphLine(Text parent, IGlyphLineInfo info)
            {
                Parent = parent;
                start = info.Span.Start;
                count = info.Span.Count;
                x = info.X;
                y = info.Y;
                w = info.Width;
                h = info.Height;
                drawY = info.LineY.Round();
            }
            #endregion

            #region PROPERTIES
            public int Start => start;
            public int Count => count;
            public string Text
            {
                get
                {
                    return (Parent.text ?? "").Substring(start, count);
                }
            }
            public IGlyph this[int index] => 
                Parent.Data[index + start];
            public int X => x;
            public int Y => y;
            public int Width => w + Parent.margin.X;
            public int Height => h + Parent.margin.Y;
            public int DrawX => ((IGlyphLine)Parent).DrawX + x;
            public int DrawY => ((IGlyphLine)Parent).DrawY + drawY;
            public bool Valid => Width > 0 && Height > 0;
            IGlyph IGlyphLine.Dot => Parent.dot;
            #endregion

            #region GET BOUNDS
            public void GetBounds(out int x, out int y, out int w, out int h)
            {
                x = this.x;
                y = this.y;
                w = this.w;
                h = this.h;
            }
            #endregion

            #region ENUMERATOR
            public IEnumerator<IGlyph> GetEnumerator()
            {
                var count = Count;
                for (int i = 0; i < count; i++)
                {
                    yield return Parent.Data[start + i];
                }
            }
            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
            #endregion

            public override string ToString()
            {
                return Parent.text?.Substring(start, count);
            }
        }

    }
    #endregion
}
#endif
