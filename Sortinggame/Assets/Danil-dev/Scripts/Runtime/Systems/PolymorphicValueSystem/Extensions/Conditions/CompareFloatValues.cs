using System;
using D_Dev.Base;
using D_Dev.Conditions;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions
{
    [System.Serializable]
    public class CompareFloatValues : ICondition
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<float> _valueA;
        [SerializeField] private ValueCompareType _compareType;
        [SerializeReference] private PolymorphicValue<float> _valueB;
        
        #endregion

        #region IConditions

        public bool IsConditionMet()
        {
            return _compareType switch
            {
                ValueCompareType.Less => _valueA.Value < _valueB.Value,
                ValueCompareType.Equal => Mathf.Approximately(_valueA.Value, _valueB.Value),
                ValueCompareType.Bigger => _valueA.Value > _valueB.Value,
                ValueCompareType.EqualOrLess => _valueA.Value <= _valueB.Value,
                ValueCompareType.EqualOrBigger => _valueA.Value >= _valueB.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Reset() {}

        #endregion
        
    }
}