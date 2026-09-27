using System;
using D_Dev.Base;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.Conditions
{
    [System.Serializable]
    public class RigidbodyLinearVelocityCompare : IFixedCondition
    {
        #region Fields

        [SerializeField] private Rigidbody _rigidbody;
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
            
            float target = _value.Value;
            switch (_compareType)
            {
                case ValueCompareType.Less:
                    return _rigidbody.linearVelocity.magnitude < target;
                case ValueCompareType.Equal:
                    return Mathf.Approximately(_rigidbody.linearVelocity.magnitude, target);
                case ValueCompareType.Bigger:
                    return _rigidbody.linearVelocity.magnitude > target;
                case ValueCompareType.EqualOrLess:
                    return _rigidbody.linearVelocity.magnitude <= target;
                case ValueCompareType.EqualOrBigger:
                    return _rigidbody.linearVelocity.magnitude >= target;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Reset() {}

        #endregion
    }
}