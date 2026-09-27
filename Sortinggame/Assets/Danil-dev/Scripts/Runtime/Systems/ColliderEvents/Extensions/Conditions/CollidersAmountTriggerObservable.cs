using System;
using D_Dev.Base;
using D_Dev.Conditions;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.ColliderEvents.Extensions.Conditions
{
    [System.Serializable]
    public class CollidersAmountTriggerObservable : ICondition
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<TriggerColliderObservable> _triggerObservable;
        [SerializeField] private ValueCompareType _compareType;
        [SerializeReference] private PolymorphicValue<int> _amount;

        #endregion
        
        #region ICondition

        public bool IsConditionMet()
        {
            if (_triggerObservable == null || _triggerObservable.Value == null)
                return false;
            
            return _compareType switch
            {
                ValueCompareType.Less => _triggerObservable.Value.Colliders.Count < _amount.Value,
                ValueCompareType.Equal => _triggerObservable.Value.Colliders.Count == _amount.Value,
                ValueCompareType.Bigger => _triggerObservable.Value.Colliders.Count > _amount.Value,
                ValueCompareType.EqualOrLess => _triggerObservable.Value.Colliders.Count <= _amount.Value,
                ValueCompareType.EqualOrBigger => _triggerObservable.Value.Colliders.Count >= _amount.Value,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Reset()
        {
        }

        #endregion
    }
}