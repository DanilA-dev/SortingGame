using D_Dev.Conditions;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Compare
{
    public class FloatValueCompare : BasePolymorphicValueCompare<float>
    {
        #region Fields

        [SerializeField] private ValueCompareType _compareType;

        #endregion

        #region Public

        public override bool Compare(float value, float valueTo) => _compareType switch
        {
            ValueCompareType.Less          => value < valueTo,
            ValueCompareType.Equal         => Mathf.Approximately(value, valueTo),
            ValueCompareType.Bigger        => value > valueTo,
            ValueCompareType.EqualOrLess   => value <= valueTo,
            ValueCompareType.EqualOrBigger => value >= valueTo,
            _                              => false
        };

        #endregion
    }
}
