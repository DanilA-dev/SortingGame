using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions
{
    [System.Serializable]
    public class Vector2ValueExists : RuntimeValueExists<Vector2>
    {
        #region Overrides

        public override bool IsConditionMet()
        {
            var result = _value.Value != Vector2.zero;
            return result == _isExists;
        }

        #endregion
    }
}