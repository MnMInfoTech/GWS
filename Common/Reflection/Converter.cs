/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

// Author: Mukesh Adhvaryu.
using System;
using System.IO;

namespace MnM.GWS
{
    public partial class Converter: _Converter
    {
        protected override bool ProcessUnknown<T>(Type t, string expression, out T result)
        {
            result = default(T);
            var ttype = typeof(T);

            #region SIZE
            if (t == (typeof(Size)) || t == typeof(ISize))
            {
                string h = "", w = "";
                bool separate = false;

                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == ',')
                    {
                        separate = true;
                        continue;
                    }
                    if (char.IsDigit(expression[i]) || expression[i] == '.')
                    {
                        if (separate) h += expression[i];
                        else w += expression[i];
                    }
                }

                if (int.TryParse(w, out int _w) && int.TryParse(h, out int _h))
                {
                    result = (T)(object)new Size(_w, _h);
                    return true;
                }
            }
            #endregion

            #region SIZEF
            else if (t == (typeof(SizeF)) || t == typeof(ISizeF))
            {
                string h = "", w = "";
                bool separate = false;

                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == ',')
                    {
                        separate = true;
                        continue;
                    }
                    if (char.IsDigit(expression[i]) || expression[i] == '.')
                    {
                        if (separate) h += expression[i];
                        else w += expression[i];
                    }
                }

                if (float.TryParse(w, out float _w) && float.TryParse(h, out float _h))
                {
                    result = (T)(object)new SizeF(_w, _h);
                    return true;
                }
                else if (double.TryParse(w, out double dw) && double.TryParse(h, out double dh))
                {
                    result = (T)(object)new SizeF((float)dw, (float)dh);
                    return true;
                }
            }
            #endregion

            #region POINT
            else if (t == (typeof(Vector)) || t == typeof(IPoint))
            {
                string h = "", w = "";
                bool separate = false;

                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == ',')
                    {
                        separate = true;
                        continue;
                    }
                    if (char.IsDigit(expression[i]) || expression[i] == '.')
                    {
                        if (separate) h += expression[i];
                        else w += expression[i];
                    }
                }
                if (int.TryParse(w, out int _w) && int.TryParse(h, out int _h))
                {
                    result = (T)(object)new Vector(_w, _h);
                    return true;
                }
            }
            #endregion

            #region POINTF
            else if (t == (typeof(VectorF)) || t == typeof(IPointF))
            {
                string h = "", w = "";
                bool separate = false;

                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == ',')
                    {
                        separate = true;
                        continue;
                    }
                    if (char.IsDigit(expression[i]) || expression[i] == '.')
                    {
                        if (separate) h += expression[i];
                        else w += expression[i];
                    }
                }
                if (float.TryParse(w, out float _w) && float.TryParse(h, out float _h))
                {
                    result = (T)(object)new VectorF(_w, _h);
                    return true;
                }
                else if (double.TryParse(w, out double dw) && double.TryParse(h, out double dh))
                {
                    result = (T)(object)new VectorF((float)dw, (float)dh);
                    return true;
                }
            }
            #endregion

            #region RECTANGLE
            else if (t == (typeof(Rectangle)) || t == typeof(IRectangle)
                || t == typeof(IBounds) || t == typeof(IBounds))
            {
                string h = "", w = "", x = "", y = "";
                int c = 0;
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == ',')
                    {
                        c++;
                        continue;
                    }
                    if (char.IsDigit(expression[i]) || expression[i] == '.')
                    {
                        switch (c)
                        {
                            case 0:
                            default:
                                x += expression[i];
                                break;
                            case 1:
                                y += expression[i];
                                break;
                            case 2:
                                w += expression[i];
                                break;
                            case 3:
                                h += expression[i];
                                break;
                        }
                    }
                }

                if (Numbers.IsNumeric(x) && Numbers.IsNumeric(y) && Numbers.IsNumeric(w) && Numbers.IsNumeric(h))
                {
                    result = (T)(object)new Rectangle(Convert.ToInt32(x), Convert.ToInt32(y),
                        Convert.ToInt32(w), Convert.ToInt32(h));
                    return true;
                }
            }
            #endregion

            #region RECTANGLEF
            else if (t == (typeof(RectangleF)) || t == typeof(IRectangleF))
            {
                string h = "", w = "", x = "", y = "";
                int c = 0;
                for (int i = 0; i < expression.Length; i++)
                {
                    if (expression[i] == ',')
                    {
                        c++;
                        continue;
                    }
                    if (char.IsDigit(expression[i]) || expression[i] == '.')
                    {
                        switch (c)
                        {
                            case 0:
                            default:
                                x += expression[i];
                                break;
                            case 1:
                                y += expression[i];
                                break;
                            case 2:
                                w += expression[i];
                                break;
                            case 3:
                                h += expression[i];
                                break;
                        }
                    }
                }

                if (Numbers.IsNumeric(x) && Numbers.IsNumeric(y) && Numbers.IsNumeric(w) && Numbers.IsNumeric(h))
                {
                    result = (T)(object)new Rectangle(Convert.ToSingle(x), Convert.ToSingle(y),
                        Convert.ToSingle(w), Convert.ToSingle(h));
                    return true;
                }
            }
            #endregion

            #region COLOR
            else if (t == (typeof(Rgba)) || t == typeof(IColour))
            {
                if (Numbers.IsNumeric(expression))
                {
                    var argb = Convert.ToInt32(expression);
                    result = (T)(object)new Rgba(argb);
                    return true;
                }
                else if (!string.IsNullOrEmpty(expression))
                {
                    try
                    {
                        result = (T)(object)Rgba.FromName(expression);
                        return true;
                    }
                    catch { }
                }
            }
            #endregion

#if GWS || Window
#region IMAGE  
            if (ttype == typeof(IImageSource))
            {
                byte[] b = Convert.FromBase64String(expression);
                result = (T)Factory.newCanvas(b);
                return true;
            }
#endregion
#endif
            return false;
        }
        public unsafe override bool ConvertTo<T>(object value, out T result)
        {
            result = default(T);
            if (value == null)
                return false;
            var ttype = typeof(T);

            if (value is IImageSource)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Factory.ImageProcessor.Write(((IImageSource)value), ms);
                    if (ttype == typeof(string))
                    {
                        result = (T)(object)Convert.ToBase64String(ms.ToArray());
                        return true;
                    }
                    else if (ttype == typeof(int[]))
                    {
                        result = (T)(object)value;
                        return true;
                    }
                    else if (ttype == typeof(byte[]))
                    {
                        result = (T)(object)ms.ToArray();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
