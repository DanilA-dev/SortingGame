using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.PositionRotationConfig
{
    [System.Serializable]
    public class TransformArrayPositionSettings : BasePositionSettings
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<Transform[]> _values = new TransformArrayConstantValue();

        private int _currentIndex;
            
        #endregion

        #region Properties

        public PolymorphicValue<Transform[]> Values => _values;

        #endregion

        #region Overrides

        public override Vector3 OnGetPosition()
        {
            if (_values == null || _values.Value == null)
                return Vector3.zero;
            
            var currentValues = _values.Value;
            var currentPos = currentValues[_currentIndex];
            _currentIndex = (_currentIndex + 1) % currentValues.Length;
            return currentPos.position;
        }

        #endregion
    }
}