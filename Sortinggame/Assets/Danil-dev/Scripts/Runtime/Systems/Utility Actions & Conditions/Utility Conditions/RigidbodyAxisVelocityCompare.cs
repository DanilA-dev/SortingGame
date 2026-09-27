using System;
using D_Dev.Base;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.Conditions
{
    [System.Serializable]
    public class RigidbodyAxisVelocityCompare : IFixedCondition
    {
        #region Enums

        public enum VelocityAxis
        {
            X = 0,
            Y = 1,
            Z = 2
        }

        #endregion

        #region Fields

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private VelocityAxis _axis = VelocityAxis.Y;
        [SerializeField] private ValueCompareType _compareType;
        [SerializeReference] private PolymorphicValue<float> _value = new FloatConstantValue();

        #endregion

        #region ICondition

        public bool IsConditionMet()
        {
            if (_rigidbody == null)
                return false;

            if (_value == null)
                return false;

            float axisVelocity = GetAxisVelocity(_rigidbody.linearVelocity);
            float target = _value.Value;

            switch (_compareType)
            {
                case ValueCompareType.Less:
                    return axisVelocity < target;
                case ValueCompareType.Equal:
                    return Mathf.Approximately(axisVelocity, target);
                case ValueCompareType.Bigger:
                    return axisVelocity > target;
                case ValueCompareType.EqualOrLess:
                    return axisVelocity <= target;
                case ValueCompareType.EqualOrBigger:
                    return axisVelocity >= target;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Reset() {}

        #endregion

        #region Private

        private float GetAxisVelocity(Vector3 velocity)
        {
            switch (_axis)
            {
                case VelocityAxis.X:
                    return velocity.x;
                case VelocityAxis.Y:
                    return velocity.y;
                case VelocityAxis.Z:
                    return velocity.z;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
