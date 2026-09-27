using D_Dev.PolymorphicValueSystem;
using D_Dev.SaveSystem.SaveableData;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions.Saveables
{
    [System.Serializable]
    public class BoolValueSaveableData : BaseSaveableData<bool>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<bool> _value = new BoolConstantValue();
        [SerializeReference] private PolymorphicValue<bool> _defaultValue = new BoolConstantValue();

        #endregion

        #region Properties

        public PolymorphicValue<bool> DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        #endregion

        #region Overrides

        protected override bool GetTypedSaveData() => _value.Value;

        protected override void SetTypedSaveData(bool data) => _value.Value = data;

        protected override bool GetTypedDefaultValue() => _defaultValue.Value;

        #endregion
    }
}
