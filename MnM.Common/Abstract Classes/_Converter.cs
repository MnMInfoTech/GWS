/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
    public abstract class _Converter : IConverter
    {
        public bool ConvertTo<T>(string expression, out T result)
        {
            Type t = typeof(T);

            #region SIZE
            if (t == (typeof(Size)))
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
            else if (t == (typeof(SizeF)))
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
            else if (t == (typeof(Size)))
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
            else if (t == (typeof(VectorF)))
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
            else if (t == (typeof(Rectangle)))
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
            else if (t == (typeof(RectangleF)))
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
            else if (t == (typeof(Rgba)) || t == typeof(IColor))
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

            return ProcessUnknown(t, expression, out result);
        }
        protected abstract bool ProcessUnknown<T>(Type t, string expression, out T result);
        public abstract bool ConvertTo<T>(object value, out T result);
    }
}
