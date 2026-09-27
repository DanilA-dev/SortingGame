using D_Dev.PolymorphicValueSystem;
using D_Dev.SaveSystem.SaveableData;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions.Saveables
{
    [System.Serializable]
    public class Vector2ValueSaveableData : BaseSaveableData<Vector2>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<Vector2> _value = new Vector2ConstantValue();
        [SerializeReference] private PolymorphicValue<Vector2> _defaultValue = new Vector2ConstantValue();

        #endregion

        #region Properties

        public PolymorphicValue<Vector2> DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        #endregion

        #region Overrides

        protected override Vector2 GetTypedSaveData() => _value.Value;

        protected override void SetTypedSaveData(Vector2 data) => _value.Value = data;

        protected override Vector2 GetTypedDefaultValue() => _defaultValue.Value;

        #endregion
    }
}
