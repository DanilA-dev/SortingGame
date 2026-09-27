using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions
{
    [System.Serializable]
    public class Vector3ValueExists : RuntimeValueExists<Vector3>
    {
        #region Overrides
        public override bool IsConditionMet()
        {
            var result = _value.Value != Vector3.zero;
            return result == _isExists;
        }
        #endregion
    }
}