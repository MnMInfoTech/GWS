/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    /// <summary>
    /// Represents an object which contains requisite information to rotate an object.
    /// </summary>
    public interface IRotation : IDegree, IDegreeHolder, ISkewHolder, ICentreHolder, IValid, 
        IRotationParameter, IMultiParamReceiver<IRotation, IRotationParameter> 
    { 
    }

    /// <summary>
    /// Represents rotation object which contains requisite information to rotate an object.
    /// </summary>
    public sealed class Rotation : IRotation, IEquatable<IRotation>
    {
        #region VARIABLES
        /// <summary>
        /// Center of this rotation.
        /// </summary>
        internal ICentre Centre;

        /// <summary>
        /// Angle value in degree.
        /// </summary>
        internal float Degree;

        /// <summary>
        /// Skew value of this rotation.
        /// </summary>
        internal ISkew Skew;

        static string toStr = "Angle: {0}, {1}, {2}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates new instance of rotation using specified parameters.
        /// </summary>
        public Rotation()
        { }

        /// <summary>
        /// Creates new instance of rotation using specified parameters.
        /// </summary>
        /// <param name="degree">Value of rotation in degree.</param>
        /// <param name="cx">X co-ordinate of center of rotation.</param>
        /// <param name="cy">Y co-ordinate of center of rotation.</param>
        /// <param name="skewType">Skew type of rotation.</param>
        public Rotation(float degree, float? cx = null, float? cy = null,
            SkewType? skewType = null, float? skewDegree = null)
        {
            Degree = degree;
            if (Degree > 360 || Degree < -360)
                Degree %= 360;
            Skew = skewType.ToSkew(skewDegree);
            if (cx != null || cy != null)
                Centre = new Centre(cx, cy);
        }

        /// <summary>
        /// Creates new instance of angle based on given angle parameter and specified center information.
        /// </summary>
        /// <param name="degree">Value of angle in degree.</param>
        /// <param name="r">Rectangle of which center will be used as the center of this angle.</param>
        /// <param name="skewType">Skew type of rotation.</param>
        /// <param name="skewDegree">Value of skew angle in degree.</param>
        /// <returns></returns>
        public Rotation(float degree, IBounds r,
            SkewType? skewType = null, float? skewDegree = null)
        {
            Degree = degree;
            if (Degree > 360 || Degree < -360)
                Degree %= 360;
            Skew = skewType.ToSkew(skewDegree);
            Centre = new Centre(r);
        }

        /// <summary>
        /// Creates new instance of angle based on given angle parameter and specified center information.
        /// </summary>
        /// <param name="degree">Value of angle in degree.</param>
        /// <param name="r">Rectangle of which center will be used as the center of this angle.</param>
        /// <param name="skewType">Skew type of rotation.</param>
        /// <param name="skewDegree">Value of skew angle in degree.</param>
        /// <returns></returns>
        public Rotation(float degree, IBoundsF r,
            SkewType? skewType = null, float? skewDegree = null)
        {
            Degree = degree;
            if (Degree > 360 || Degree < -360)
                Degree %= 360;
            Skew = skewType.ToSkew(skewDegree);
            Centre = new Centre(r);
        }

        public Rotation(IRotation rotation)
        {
            Degree = rotation.Angle;
            if (Degree > 360 || Degree < -360)
                Degree %= 360;
            if (rotation.Skew != null)
                Skew = new Skew(rotation.Skew);
            if (rotation.Centre != null)
                Centre = new Centre(rotation.Centre);
        }
        #endregion

        #region PROPERTIES
        float IDegree.Angle => Degree;
        ISkew ISkewHolder.Skew { get => Skew; }
        ICentre ICentreHolder.Centre { get => Centre; }
        IDegree IDegreeHolder.Degree => this;
        public bool Valid => (Degree != 0 && Degree != 0.001f && Degree != 360 && Degree != -360)
            || Skew?.HasScale == true;
        public bool HasAngle => Degree != 0 && Degree != 0.001f && Degree != 360 && Degree != -360;
        object IValue.Value => this;
        #endregion

        #region MODIFY
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRotation Modify(IEnumerable<IRotationParameter> rotationParams)
        {
            if (rotationParams == null)
                return this;
            foreach (var parameter in rotationParams)
            {
                if (parameter == null)
                    continue;
                if (parameter is ISkew)
                    Skew = ((ISkew)parameter);
                if (parameter is IDegree)
                {
                    if (parameter is IModifier<float>)
                        Degree = ((IModifier<float>)parameter).Modify(Degree);
                    else
                        Degree = ((IDegree)parameter).Angle;
                }
                if (parameter is ICentre)
                    Centre = (ICentre)parameter;
            }
            return this;
        }

        public IRotation Modify(IRotationParameter parameter)
        {
            if (parameter == null)
                return this;
            if (parameter is ISkew)
                Skew = ((ISkew)parameter).ToSkew();
            if (parameter is IDegree)
            {
                if (parameter is IModifier<float>)
                    Degree = ((IModifier<float>)parameter).Modify(Degree);
                else
                    Degree = ((IDegree)parameter).Angle;
            }
            if (parameter is ICentre)
                Centre = new Centre((ICentre)parameter);
            return this;
        }
        #endregion

        #region SET DEGREE
        public void SetDegree(float? angle)
        {
            Degree = angle ?? 0;
            if (Degree > 360 || Degree < -360)
                Degree %= 360;
        }
        #endregion

        #region SET SKEW
        public void SetSkew(ISkew skew)
        {
            Skew = skew;
        }
        #endregion

        #region SET CENTER
        public void SetCentre(float? x, float? y)
        {
            if (x == null && y == null)
            {
                Centre = null;
                return;
            }

            var cx = x ?? Centre?.X ?? 0;
            var cy = y ?? Centre?.Y ?? 0;
            Centre = new Centre(cx, cy);
        }
        #endregion

        #region TO ROTATION
        public IRotation ToRotation(bool clone = false)
        {
            return clone? new Rotation(this): this;
        }
        #endregion

        #region CLONE
        public object Clone()
        {
            var r = new Rotation();
            r.Degree = Degree;
            r.Skew = Skew;
            r.Centre = Centre;
            return r;
        }
        #endregion

        #region EQUALITY
        public bool Equals(IRotation other)
        {
            if (other == null)
                return false;
            return
                Degree == other.Angle &&
                Equals(Skew, other.Skew) &&
                other.Centre?.Cx == Centre?.Cx &&
                other.Centre?.Cy == Centre?.Cy;
        }
        public override int GetHashCode()
        {
            return new { Degree, Skew, Centre.Cx, Centre.Cy }.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IRotation)
                return ((IRotation)obj).Equals(this);
            return false;
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, Degree, Centre?.ToString()?? "Centre: null", Skew?.ToString()?? "Skew: null");
        }

    }
}
