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
     partial struct Glyphs : IGlyphs
    {
        #region VARIABLES
        readonly IList<IGlyph> Data;
        public readonly float MinHBY;
        public readonly bool ContainsNewLine;
        public readonly string Text;
        public readonly float X, Y, Width, Height;
        int id;
        public static readonly Glyphs Empty = new Glyphs();
        #endregion

        #region CONSTRUCTORS
        public Glyphs(string text, RectangleF area, IList<IGlyph> glyphs, float minHBY, bool containsNewLine = false): this()
        {
            X = area.X;
            Y = area.Y;
            Width = area.Width;
            Height = area.Height;
            this.Data = glyphs;
            MinHBY = minHBY;

            ContainsNewLine = containsNewLine;

            if (text == null)
                Text = new string(glyphs.Select(x => x.Character).ToArray());
            else
                Text = text;
            Name = TypeName.NewName();
        }
        #endregion

        #region PROPERTIES
        public IGlyph this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }
        public int ID
        {
            get
            {
                if (id == 0)
                    id = this.NewID();
                return id;
            }
        }
        public int Count => Data.Count;
        public string TypeName => "Text";
        public string Name { get; private set; }
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

        #region CONTAINS
        public bool Contains(float x, float y)
        {
            if (x < X || y < Y || x > X + Width || y > Y + Height)
                return false;
            return true;
        }
        #endregion

        #region TO SHAPE
        public IEnumerable<VectorF> Figure() => null;
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
#if AllHidden
    }
#endif
}
#endif
