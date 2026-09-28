using System;
using D_Dev.Base;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.Conditions
{
    [System.Serializable]
    public class CharacterControllerLinearVelocityCompare : ICondition
    {
        #region Fields

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private ValueCompareType _compareType;
        [SerializeReference] private PolymorphicValue<float> _value = new FloatConstantValue();

        #endregion

        #region ICondition

        public bool IsConditionMet()
        {
            if (_characterController == null)
                return false;

            if (_value == null)
                return false;

            float speed = _characterController.velocity.magnitude;
            float target = _value.Value;
            switch (_compareType)
            {
                case ValueCompareType.Less:
                    return speed < target;
                case ValueCompareType.Equal:
                    return Mathf.Approximately(speed, target);
                case ValueCompareType.Bigger:
                    return speed > target;
                case ValueCompareType.EqualOrLess:
                    return speed <= target;
                case ValueCompareType.EqualOrBigger:
                    return speed >= target;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Reset() {}

        #endregion
    }
}
