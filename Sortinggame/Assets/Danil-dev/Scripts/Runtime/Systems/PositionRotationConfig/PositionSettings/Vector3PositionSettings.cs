using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.PositionRotationConfig
{
    [System.Serializable]
    public class Vector3PositionSettings : BasePositionSettings
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<Vector3> _value = new Vector3ConstantValue();

        #endregion

        #region Properties

        public PolymorphicValue<Vector3> Value
        {
            get => _value;
            set => _value = value;
        }

        #endregion
        
        #region Overrides
        
        public override Vector3 OnGetPosition() => _value.Value;

        #endregion
    }
}