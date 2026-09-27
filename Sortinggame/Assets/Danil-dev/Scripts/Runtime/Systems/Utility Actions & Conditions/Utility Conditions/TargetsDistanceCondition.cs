using System;
using D_Dev.Base;
using D_Dev.PositionRotationConfig;
using UnityEngine;

namespace D_Dev.Conditions
{
    [System.Serializable]
    public class TargetsDistanceCondition : ICondition
    {
        #region Fields

        [SerializeField] private ValueCompareType _checkType;
        [SerializeReference] private BasePositionSettings _fromTarget = new();
        [SerializeReference] private BasePositionSettings _toTarget = new();
        [SerializeField] private float _distance;

        #endregion

        #region ICondition

        public bool IsConditionMet()
        {
            return _checkType switch
            {
                ValueCompareType.Less => Vector3.Distance(_fromTarget.GetPosition(), _toTarget.GetPosition()) <
                                         _distance,
                ValueCompareType.Equal => Mathf.Approximately(
                    Vector3.Distance(_fromTarget.GetPosition(), _toTarget.GetPosition()), _distance),
                ValueCompareType.Bigger => Vector3.Distance(_fromTarget.GetPosition(), _toTarget.GetPosition()) >
                                           _distance,
                ValueCompareType.EqualOrLess => Vector3.Distance(_fromTarget.GetPosition(), _toTarget.GetPosition()) <=
                                                _distance,
                ValueCompareType.EqualOrBigger =>
                    Vector3.Distance(_fromTarget.GetPosition(), _toTarget.GetPosition()) >= _distance,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public void Reset() {}

        #endregion
    }
}
