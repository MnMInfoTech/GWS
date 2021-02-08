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

#if GWS || Window
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

            #region IMAGE  
            if (ttype == typeof(IWritable) || ttype == typeof(IImage))
            {
                byte[] bytes = Convert.FromBase64String(expression);
                result = (T)Factory.newImage(bytes);
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
            var stype = (value).GetType();

            if (value is ICopyable || value is IPixels)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IntPtr pixels;
                    if (value is ICopyable)
                    {
                        var buffer = ((ICopyable)value);
                        int[] data = new int[buffer.Length];
                        fixed (int* d = data)
                        {
                            pixels = (IntPtr)d;
                            buffer.CopyTo(pixels, buffer.Length, buffer.Width, 0, 0, new Perimeter(0, 0, buffer.Width, buffer.Height), 0);
                            if (ttype == typeof(string) || ttype == typeof(byte[]))
                                Factory.ImageProcessor.Write(pixels, buffer.Width, buffer.Height, buffer.Length, 4, ms, 0);
                        }
                    }
                    else if(value is IPixels)
                    {
                        var buffer = ((IPixels)value);
                        byte[] data = new byte[buffer.Length * 4];
                        fixed (byte* d = data)
                        {
                            Blocks.CopyBlock((byte*)buffer.Source, new Perimeter( 0, 0, buffer.Width, buffer.Height), buffer.Length, 
                                buffer.Width, buffer.Height, d, 0, 0, buffer.Width, buffer.Height, 0);
                            pixels = (IntPtr)d;
                            if (ttype == typeof(string) || ttype == typeof(byte[]))
                                Factory.ImageProcessor.Write(pixels, buffer.Width, buffer.Height, buffer.Length, 4, ms, 0);
                        }
                    }
                    if (ttype == typeof(string))
                    {
                        result = (T)(object)Convert.ToBase64String(ms.ToArray());
                        return true;
                    }
                    else if (ttype == typeof(int[]))
                    {
                        result = (T)(object)value;
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
