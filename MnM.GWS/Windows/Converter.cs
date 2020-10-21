using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MnM.GWS
{
    public class Converter: _Converter
    {
        protected override bool ProcessUnknown<T>(Type t, string expression, out T result)
        {
            result = default(T);

            #region IMAGE
            if (t.IsAssignableFrom(typeof(ICopyable)) || t.IsAssignableFrom(typeof(IBlock)))
            {
                byte[] bytes = Convert.FromBase64String(expression);

                if (t.IsAssignableFrom(typeof(ICanvas)))
                {
                    result = (T)Factory.newCanvas(bytes);
                    return true;
                }
                else if (t.IsAssignableFrom(typeof(ISurface)) ||
                    t.IsAssignableFrom(typeof(ISurface)))
                {
                    result = (T)Factory.newSurface(bytes);
                    return true;
                }
            }
            #endregion
            return false;
        }
        public unsafe override bool ConvertTo<T>(object value, out T result)
        {
            result = default(T);
            if (value == null)
                return false;
            var ttype = typeof(T);
            var stype = (value).GetType();

            if(stype.IsAssignableFrom(typeof(ICopyable)))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    var buffer = value as ICopyable;
                    IntPtr pixels;

                    int[] data = new int[buffer.Length];
                    fixed (int* d = data)
                    {
                        pixels = (IntPtr)d;
                        buffer.CopyTo(0, 0, buffer.Width, buffer.Height, pixels, buffer.Length, buffer.Width, 0, 0);
                        if(ttype == typeof(string) || ttype == typeof(byte[]))
                            Factory.ImageProcessor.Write(pixels, buffer.Width, buffer.Height, buffer.Length, 4, ms, 0);
                    }
                    if (ttype == typeof(string))
                    {
                        result = (T)(object)Convert.ToBase64String(ms.ToArray());
                        return true;
                    }
                    else if(ttype == typeof(int[]))
                    {
                        result = (T)(object)data;
                    }
                    else if(ttype == typeof(byte[]))
                    {
                        result =(T)(object) ms.ToArray();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
