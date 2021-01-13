/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    /// <summary>
    /// Represents rotation object which contains requisite information to rotate an object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rotation : IScale
    {
        #region VARIABLES
        /// <summary>
        /// Rotation value in degree.
        /// </summary>
        public readonly float Degree;

        /// <summary>
        /// 
        /// </summary>
        public readonly SkewType Skew;

        /// <summary>
        /// X co-ordinate of center of rotation.
        /// </summary>
        public readonly int Cx;

        /// <summary>
        /// Y co-ordinate of center of rotation.
        /// </summary>
        public readonly int Cy;

        /// <summary>
        /// Specifies if this object has been assigned with center or not.
        /// </summary>
        public readonly bool HasCenter;

        /// <summary>
        /// Interger value of Cosine of rotation.
        /// </summary>
        public readonly int Cosi;

        /// <summary>
        /// Interger value of Sin of rotation.
        /// </summary>
        public readonly int Sini;

        /// <summary>
        /// float value of Cosine of rotation.
        /// </summary>
        public readonly float Cos;

        /// <summary>
        /// float value of Sin of rotation.
        /// </summary>
        public readonly float Sin;

        /// <summary>
        /// Scale factor for X co-ordinte.
        /// </summary>
        public readonly float ScaleX;

        /// <summary>
        /// Scale factor for Y co-ordinte.
        /// </summary>
        public readonly float ScaleY;

        /// <summary>
        /// Validity flag
        /// </summary>
        readonly internal byte Valid;

        public static readonly Rotation Empty = new Rotation();

        static string tostr = "A:{0}, CX:{1}, CY:{2}";
        #endregion

        #region CONSTRUCTORS
        internal Rotation(float degree, float sin, float cos, int sini, int cosi, int cx, int cy, bool hasCenter, SkewType skew, float scaleX, float scaleY, byte valid)
        {
            Degree = degree;
            Sin = sin;
            Cos = cos;
            Sini = sini;
            Cosi = cosi;
            Cx = cx;
            Cy = cy;
            HasCenter = hasCenter;
            Skew = skew;
            ScaleX = scaleX;
            ScaleY = scaleY;
            Valid = valid;
        }
        /// <summary>
        /// Creates new instance of rotation using specified parameters.
        /// </summary>
        /// <param name="degree">Value of rotation in degree.</param>
        /// <param name="cx">X co-ordinate of center of rotation.</param>
        /// <param name="cy">Y co-ordinate of center of rotation.</param>
        /// <param name="hasCenter">If true then HasCenter flag is set to true and center is deemed to be permanently assigned otherwise temporarily.</param>
        /// <param name="type">Type of rotation.</param>
        public Rotation(float degree, int? cx = null, int? cy = null, bool? hasCenter = null, SkewType? skewType = null)
        {
            Degree = degree;
            Cx = cx ?? 0;
            Cy = cy ?? 0;
            HasCenter = hasCenter ?? (cx != null || cy != null);
            Angles.SinCos(Degree, out Sin, out Cos);
            Angles.SinCos(Degree, out Sini, out Cosi);

            Valid = 1;
            if (Degree == 0 || Degree == 0.001f || Degree == 360 || Degree == -360)
                Valid = 0;
            Skew = Angles.GetScale(degree, skewType, out ScaleX, out ScaleY);
        }

        /// <summary>
        /// Creates new instance of rotation using specified parameters.
        /// </summary>
        /// <param name="angle">Angle object which contains degree and center information.</param>
        /// <param name="cx">X co-ordinate of center of rotation. 
        /// If angle has center assigned then this parameter will be ignored.</param>
        /// <param name="cy">Y co-ordinate of center of rotation.
        /// If angle has center assigned then this parameter will be ignored.</param>
        public Rotation(Rotation angle, int? cx = null, int? cy = null, bool? hasCenter = null, SkewType? skewType = null) : this()
        {
            if (!angle)
                return;
            Degree = angle.Degree;
            Sin = angle.Sin;
            Cos = angle.Cos;
            Sini = angle.Sini;
            Cosi = angle.Cosi;
            Valid = angle.Valid;
            Cx = cx ?? angle.Cx;
            Cy = cy ?? angle.Cy;
            HasCenter = hasCenter ?? (cx != null || cy != null);

            if (skewType != null && skewType != angle.Skew)
                Skew = Angles.GetScale(angle.Degree, skewType, out ScaleX, out ScaleY);
            else
            {
                ScaleX = angle.ScaleX;
                ScaleY = angle.ScaleY;
                Skew = angle.Skew;
            }
        }

        /// <summary>
        /// Creates new instance of angle based on given angle parameter and specified center information.
        /// </summary>
        /// <param name="angle">Angle object to be copied.</param>
        /// <param name="center">Center of this angle to be.
        public Rotation(Rotation angle, IPointF center, bool? hasCenter = null, SkewType? skewType = null) :
            this(angle, center.X.Round(), center.Y.Round(), hasCenter, skewType)
        { }

        /// <summary>
        /// Creates new instance of angle based on given angle parameter and specified center information.
        /// </summary>
        /// <param name="angle">Value of angle in degree.</param>
        /// <param name="center">Center of this angle to be.
        public Rotation(float angle, IPointF center, bool? hasCenter = null, SkewType? skewType = null) :
            this(angle, center.X.Round(), center.Y.Round(), hasCenter, skewType)
        { }

        /// <summary>
        /// Creates new instance of angle based on given angle parameter and specified center information.
        /// </summary>
        /// <param name="angle">Value of angle in degree.</param>
        /// <param name="center">Center of this angle to be.
        public Rotation(float angle, IPoint center, bool? hasCenter = null, SkewType? skewType = null) :
            this(angle, center.X, center.Y, hasCenter, skewType)
        { }

        /// <summary>
        /// Creates new instance of angle based on given angle value and specified rectangle information.
        /// </summary>
        /// <param name="angle">Value of angle in degree.</param>
        /// <param name="area">Rectangle of which center will be used as the center of this angle.</param>
        public Rotation(float angle, IRectangleF area, bool? hasCenter = null, SkewType? skewType = null) :
            this(angle, (area.X + area.Width / 2f).Round(), (area.Y + area.Height / 2f).Round(), hasCenter, skewType)
        { }

        /// <summary>
        /// Creates new instance of angle based on given angle parameter and specified center information.
        /// </summary>
        /// <param name="angle">Value of angle in degree.</param>
        /// <param name="area"></param>
        public Rotation(float angle, IRectangle area, bool? hasCenter = null, SkewType? skewType = null) :
            this(angle, (area.X + area.Width / 2f).Round(), (area.Y + area.Height / 2f).Round(), hasCenter, skewType)
        { }
        #endregion

        #region PROPERTIES
        public bool Diagonal =>
            Skew.HasFlag(SkewType.Diagonal);
        float IScale.X => ScaleX;
        float IScale.Y => ScaleY;
        public bool HasScale => (ScaleX != 0 || ScaleY != 0);
        #endregion

        #region OPERATORS
        public static implicit operator bool(Rotation angle) =>
            angle.Valid == 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rotation operator -(Rotation a) =>
            new Rotation(-a.Degree, a.Cx, a.Cy, a.HasCenter, a.Skew);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rotation operator +(Rotation a, Rotation b)
        {
            return new Rotation(a.Degree + b.Degree, (a.Cx + b.Cx), (a.Cy + b.Cy),
                a.HasCenter || b.HasCenter, a.Skew);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rotation operator -(Rotation a, Rotation b)
        {
            return new Rotation(a.Degree - b.Degree, a.Cx - b.Cx, a.Cy - b.Cy,
                a.HasCenter || b.HasCenter, a.Skew);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rotation operator +(Rotation a, float b) =>
            new Rotation(a.Degree + b, a.Cx, a.Cy, a.HasCenter, a.Skew);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rotation operator -(Rotation a, float b) =>
            new Rotation(a.Degree - b, a.Cx, a.Cy, a.HasCenter, a.Skew);
        #endregion

        public override string ToString()
        {
            return string.Format(tostr, Degree, Cx, Cy);
        }
    }
}
