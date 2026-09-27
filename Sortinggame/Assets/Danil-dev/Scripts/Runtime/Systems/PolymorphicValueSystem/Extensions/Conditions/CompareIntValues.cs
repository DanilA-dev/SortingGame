using System;
using D_Dev.Base;
using D_Dev.Conditions;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions
{
    [System.Serializable]
    public class CompareIntValues : ICondition
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<int> _valueA;
        [SerializeField] private ValueCompareType _compareType;
        [SerializeReference] private PolymorphicValue<int> _valueB;
        
        #endregion

        #region IConditions

        public bool IsConditionMet()
        {
            return _compareType switch
            {
                ValueCompareType.Less => _valueA.Value < _valueB.Value,
                ValueCompareType.Equal => _valueA.Value == _valueB.Value,
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
