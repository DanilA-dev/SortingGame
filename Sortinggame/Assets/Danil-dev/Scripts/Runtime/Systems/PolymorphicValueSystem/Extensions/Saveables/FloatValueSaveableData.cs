using D_Dev.PolymorphicValueSystem;
using D_Dev.SaveSystem.SaveableData;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions.Saveables
{
    [System.Serializable]
    public class FloatValueSaveableData : BaseSaveableData<float>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<float> _value = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<float> _defaultValue = new FloatConstantValue();

        #endregion

        #region Properties

        public PolymorphicValue<float> DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        #endregion

        #region Overrides

        protected override float GetTypedSaveData() => _value.Value;

        protected override void SetTypedSaveData(float data) => _value.Value = data;

        protected override float GetTypedDefaultValue() => _defaultValue.Value;

        #endregion
    }
}
