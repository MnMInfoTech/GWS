
/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if GWS || Window
using System.Collections;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region IGLYPH
    public interface IGlyph : ICloneable, IRectangleF, IHBY, IOffsetHolder, IEnumerable<IAxisLine>, IScanLine
    {
        #region PROPERTIES
        /// <summary>
        /// The character which the glyph represents.
        /// </summary>
        char Character { get; }

        new IOffset Offset { get; set; }

        /// <summary>
        /// Gets a flag indicationg if this glyph is yet processed or not.
        /// </summary>
        bool IsProcessed { get; }

        /// <summary>
        /// Gets width of this object.
        /// </summary>
        new float Width { get; }

        /// <summary>
        /// Gets height of this object.
        /// </summary>
        new float Height { get; }
        #endregion

        /// <summary>
        /// Returns rotated and scaled version of this glyph.
        /// </summary>
        /// <param name="angle">Angle of rotation to apply.</param>
        /// <param name="Cx">X co-ordinate of Center of rotation.</param>
        /// <param name="Cy">Y co-ordinate of Center of rotation.</param>
        /// <param name="scale">Scale to apply.</param>
        /// <param name="skew">Skew to apply.</param>
        /// <returns>Rotated and scaled version of this glyph.</returns>
        IGlyph RotateAndScale(float angle, float Cx, float Cy, IScale scale = null, ISkew skew = null);

        /// <summary>
        /// Refresh this glyph and populated scanline data.
        /// </summary>
        /// <param name="shapeRenderer"> Shape- renderer to generatine renderable scanlines.</param>
        void Refresh(IGlyphRenderer shapeRenderer);
    }
    #endregion

    #region IEx-GLYPH
    internal interface IExGlyph : IGlyph
    {
        new float X { get; set; }
        new float Y { get; set; }
        IGlyphSlot Slot { get; }
    }
    #endregion

    #region GLYPH
    /// <summary>
    /// Represents a glyph object which contains all the information on how a particual 
    /// character it represents should be drawn on screen.
    /// </summary>
    sealed class Glyph : IExGlyph
    {
        #region VARIABLES
        IGlyphSlot Slot;
        float x, y;
        Offset offset;
        PrimitiveList<IAxisLine> Data = new PrimitiveList<IAxisLine>(100);
        static string tostr = "{0}, x: {1}, y: {2}, w: {3}, h: {4}";
        bool DataInitialized;
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
            Slot = glyph;
            Character = Slot.Character;
        }
        #endregion

        #region PROPERTIES
        public IOffset Offset
        {
            get => offset;
            set
            {
                offset = new Offset(value);
            }
        }
        public char Character { get; private set; }
        public bool IsOutLine { get; set; }
        public bool IsProcessed => DataInitialized;
        public float X => x + Slot.X;
        public float Y => y + Slot.Y;
        public float Width => Slot.Width;
        public float Height => Slot.Height;
        public bool Valid => Width > 0 && Height > 0;
        public float MinHBY => Slot.Y;
        IGlyphSlot IExGlyph.Slot => Slot;
        float IExGlyph.X { get => x + Slot.X; set => x = value; }
        float IExGlyph.Y { get => y + Slot.Y; set => y = value; }
        #endregion

        #region REFRESH
        public void Refresh(IGlyphRenderer renderer)
        {
            if (Character == ' ')
                return;

            Data.Clear();
            int w = Slot.Width.Round();
            int h = Slot.Height.Round();

            renderer.Process(Slot.Points, Slot.Contours, ref Data, w, h, true);
            DataInitialized = true;
        }
        #endregion

        #region ROTATE
        public unsafe IGlyph RotateAndScale(float angle, float Rx, float Ry, IScale scale = null, ISkew skew = null)
        {
            if (Slot.Points == null || Slot.Points.Length == 0 ||
                char.IsWhiteSpace(Slot.Character))
            {
                DataInitialized = true;
                return this;
            }

            float minX = Slot.Min.X;
            float minY = Slot.Min.Y;
            var objX = this.x;
            var objY = this.y;

            int len = Slot.Points.Length;
            VectorF[] newPoints = new VectorF[len];
            float Sin = 0, Cos = 1;
            float sx = 0, sy = 0, skewX = 0, skewY = 0;
            bool HasScale = scale != null && scale.HasScale;
            bool HasSkewScale = skew != null && skew.HasScale;
            bool diagonal = HasSkewScale && skew.Type == SkewType.Diagonal;

            bool HasAngle = (angle != 0 && angle != 360 &&
                angle != -360 && angle != 0.001f) || diagonal;

            if (diagonal)
                angle += skew.Degree;

            if (HasAngle)
                Angles.SinCos(-angle, out Sin, out Cos);

            if (HasScale)
            {
                sx = scale.X;
                sy = scale.Y;
            }
            if (HasSkewScale)
            {
                skewX = skew.X;
                skewY = skew.Y;
            }

            float x, y, x1, y1;
            PointKind kind;

            fixed (VectorF* pt = newPoints)
            {
                fixed (VectorF* spt = Slot.Points)
                {
                    for (int i = 0; i < len; i++)
                    {
                        x = spt[i].X + minX + objX;
                        y = spt[i].Y + minY + objY;
                        kind = spt[i].Kind;

                        if (HasScale)
                        {
                            x = (x - Rx) * sx + Rx;
                            y = (y - Ry) * sy + Ry;
                        }
                        if (HasSkewScale)
                        {
                            x = (x - Rx) * skewX + Rx;
                            y = (y - Ry) * skewY + Ry;
                        }

                        if (HasAngle)
                        {
                            x -= Rx;
                            y -= Ry;
                            x1 = (x * Cos - y * Sin);
                            y1 = (x * Sin + y * Cos);
                            x1 += Rx;
                            y1 += Ry;
                            x = x1;
                            y = y1;
                        }
                        pt[i] = new VectorF(x - objX, y - objY, kind);
                    }
                }
            }
            var newSlot = new GlyphSlot(
                Slot.Character, newPoints, Slot.Contours.ToArray(), Slot.XHeight);
            var g = new Glyph(newSlot);
            g.x = objX;
            g.y = objY;
            return g;
        }
        #endregion

        #region GET BOUNDS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetBounds(out float x, out float y, out float w, out float h)
        {
            if (Width <= 0 || Height <= 0)
            {
                x = y = w = h = 0;
                return;
            }
            x = X;
            y = Y;
            w = Width;
            h = Height;
        }
        #endregion

        #region CLONE
        public object Clone()
        {
            var g = new Glyph();
            g.x = x;
            g.y = y;
            g.Slot = Slot;
            g.Data = Data;
            g.Character = Character;
            g.DataInitialized = DataInitialized;
            g.IsOutLine = IsOutLine;
            return g;
        }
        #endregion

        #region ENUMERATOR
        public IEnumerator<IAxisLine> GetEnumerator()=>
            Data.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion

        public override string ToString()
        {
            return string.Format(tostr, Slot.Character, X, Y, Slot.Width, Slot.Height);
        }
    }
    #endregion
}
#endif
