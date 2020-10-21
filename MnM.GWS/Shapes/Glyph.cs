/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if AllHidden
    partial class _Factory
    {
#else
        public
#endif
        sealed class Glyph : _Glyph, IGlyph
        {
            #region VARIABLES
            GlyphSlot slot;
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
            public Glyph(GlyphSlot glyph)
            {
                slot = glyph;
                Character = slot.Character;
            }
            void GetData()
            {
                if (Character == ' ')
                    return;
                if (Data == null)
                    Data = new Collection<AxisLine>(25);
                Data.Clear();
                var data = Data;
                FillAction<int> action = (val1, axis, horizontal, val2, alpha) =>
                {
                    data.Add(new AxisLine(val1, val2, axis, horizontal, alpha));
                };

                Factory.FontRenderer.Process(slot.Points, slot.Contours, action, Bounds.Width.Round(), Bounds.Height.Round());
                DataInitialized = true;
            }
            #endregion

            #region PROPERTIES
            public override int X
            {
                get => (x + slot.Bounds.X).Round();
                set => x = value;
            }
            public override int Y
            {
                get => (y + slot.Bounds.Y).Round();
                set => y = value;
            }
            public override RectangleF Bounds => new RectangleF(X, Y, slot.Bounds.Width, slot.Bounds.Height);
            RectangleF IBoundsF.Bounds => Bounds;
            #endregion

            #region DRAW
            public override bool Draw(IBuffer buffer, IReadContext readContext, out IPen Pen)
            {
                Pen = null;
                if (Character == ' ')
                    return true;

                if (readContext is IPen)
                    Pen = readContext as IPen;
                else
                {
                    Pen = buffer.Settings.GetPen(this, readContext);
                }
                if ((buffer.Settings.Rotation) || buffer.Settings.Scale.HasScale)
                    RotateAndScale(buffer.Settings);

                if (!DataInitialized)
                    GetData();

                var x = X;
                var y = Y;
                buffer.Settings.FillCommand |= FillCommand.DrawLineOnly;
                var aa = !buffer.Settings.LineCommand.HasFlag(LineCommand.Breshenham);

                if (aa)
                {
                    foreach (var item in Data)
                    {
                        if (item.Horizontal)
                            buffer.WriteLine(item.Val + x, item.Val + x + item.Stretch, item.Axis + y, true, Pen, item.Alpha);
                        else
                            buffer.WriteLine(item.Val + y, item.Val + y + item.Stretch, item.Axis + x, false, Pen, item.Alpha);
                    }
                }
                else
                {
                    foreach (var item in Data)
                    {
                        if (item.Horizontal)
                            buffer.WriteLine(item.Val + x, item.Val + x + item.Stretch, item.Axis + y, true, Pen, null);
                        else
                            buffer.WriteLine(item.Val + y, item.Val + y + item.Stretch, item.Axis + x, false, Pen, null);
                    }
                }
                return true;
            }
            #endregion

            public override IEnumerable<VectorF> ToShape() => null;
            
            #region ROTATE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void RotateAndScale(IDrawSettings Settings)
            {
                //parsedPoints = null;
                if (slot.Points == null || slot.Points.Count == 0 || char.IsWhiteSpace(Character))
                    return;
                var angle = Settings.Rotation;

                Rotation a = new Rotation(-angle.Degree, 0, 0, false, angle.Skew);
                IList<VectorF> pts = new VectorF[slot.Points.Count];
                bool hasScale = Settings.Scale.HasScale;
                bool hasRotation = angle;

                float sx = 1, sy = 1;
                if (hasScale)
                {
                    sx = Settings.Scale.X + 1;
                    sy = Settings.Scale.Y + 1;
                }
                for (int i = 0; i < pts.Count; i++)
                {
                    pts[i] = slot.Points[i] + slot.Min;
                    if (hasScale)
                        pts[i] = pts[i].Scale(sx, sy);
                    if(hasRotation)
                        pts[i] = a.Rotate(pts[i]);
                }

                if (hasScale)
                {
                    x = (x * sx).Round();
                    y = (y * sy).Round();
                }
                if (hasRotation)
                {
                    float cx, cy;
                    angle.EffectiveCenter(Settings.Bounds.Cx, Settings.Bounds.Cy, out cx, out cy);
                    angle.Rotate(x, y, out float fx, out float fy, cx, cy);
                    x = fx.Ceiling();
                    y = fy.Ceiling();
                }
                slot = new GlyphSlot(Character, pts, slot.Contours.ToArray(), slot.XHeight);
                DataInitialized = false;
            }

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
                   X, Y, slot.Bounds.Width, slot.Bounds.Height);
            }
        }
#if AllHidden
    }
#endif
}