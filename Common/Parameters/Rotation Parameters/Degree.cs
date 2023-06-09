/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    #region IDEGREE
    public interface IDegree : IRotationParameter, IValid, ICloneable, IProperty
    {
        /// <summary>
        /// Rotation value in degree.
        /// </summary>
        float Angle { get; }

        /// <summary>
        /// Indicates if this rotation has valid degree of rotation.
        /// 0, 360 or -360 is considered invalid.
        /// </summary>
        bool HasAngle { get; }

        /// <summary>
        /// Converts this object into an instance of IRotation object.
        /// </summary>
        /// <returns>Instance of IRotation object.</returns>
        IRotation ToRotation(bool clone = false);

        /// <summary>
        /// Sets value of rotation in degree for this object.
        /// </summary>
        /// <param name="angle"></param>
        void SetDegree(float? angle);
    }
    #endregion

    #region DEGREE HOLDER
    public interface IDegreeHolder 
    {
        /// <summary>
        /// Gets Rotation object for rotate, skew and transform operations.
        /// </summary>
        IDegree Degree { get; }
    }
    #endregion


    #region DEGRE
    public struct Degree : IDegree, IModifier<IDegree>, IModifier<float>, IProperty<float>
    {
        #region VARIBALES
        readonly float Value;
        ModifyCommand ModifyCommand;
        #endregion

        #region CONSTRUCTOR
        public Degree(IDegree degree, ModifyCommand command = 0)
        {
            Value = degree?.Angle ?? 0;
            if (Value > 360 || Value < -360)
                Value %= 360;

            ModifyCommand = command;
        }
        public Degree(float degree, ModifyCommand command = 0)
        {
            Value = degree;
            if (Value > 360 || Value < -360)
                Value %= 360;

            ModifyCommand = command;
        }
        #endregion

        #region PROPERTIES
        float IDegree.Angle => Value;
        public bool HasAngle => Value != 0 && Value != 0.001f && Value != 360 && Value != -360;
        public bool Valid => Value != 0 && Value != 0.001f && Value != 360 && Value != -360;
        object IValue.Value => Value;
        float IValue<float>.Value => Value;
        ModifyCommand IModifyCommandHolder.ModifyCommand => ModifyCommand;
        #endregion

        #region MODIFY
        public float Modify(float other)
        {
            switch (ModifyCommand)
            {
                case ModifyCommand.Replace:
                default:
                    return Value;
                case ModifyCommand.Add:
                    return Value + other;
                case ModifyCommand.Remove:
                    return other - Value;
            }
        }
        public IDegree Modify(IDegree other)
        {
            return new Degree(Modify(other?.Angle ?? 0));
        }
        #endregion

        #region CLONE
        public object Clone()
        {
            return new Degree(Value, ModifyCommand);
        }
        #endregion

        #region TO ROTATION
        public IRotation ToRotation(bool clone = false)
        {
            return new Rotation(Value);
        }
        #endregion

        #region SET DEGREE
        public void SetDegree(float? angle)
        {
            this = new Degree(angle ?? 0);
        }
        #endregion

        #region OPERATOR OVERLOADING
        public static Degree operator +(Degree left, IDegree right)
        {
            if (right == null)
                return left;
            return new Degree(left.Value + right.Angle);
        }
        public static Degree operator -(Degree left, IDegree right)
        {
            if (right == null)
                return left;
            return new Degree(left.Value - right.Angle);
        }
        #endregion

        public override string ToString() =>
            Value.ToString();
    }
    #endregion
}
