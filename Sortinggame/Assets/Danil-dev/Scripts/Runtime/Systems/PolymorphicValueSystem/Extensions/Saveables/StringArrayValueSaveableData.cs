using D_Dev.PolymorphicValueSystem;
using D_Dev.SaveSystem.SaveableData;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions.Saveables
{
    [System.Serializable]
    public class StringArrayValueSaveableData : BaseSaveableData<string[]>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<string[]> _value = new StringArrayConstantValue();
        [SerializeReference] private PolymorphicValue<string[]> _defaultValue = new StringArrayConstantValue();

        #endregion

        #region Properties

        public PolymorphicValue<string[]> DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        #endregion

        #region Overrides

        protected override string[] GetTypedSaveData() => _value.Value;

        protected override void SetTypedSaveData(string[] data) => _value.Value = data;

        protected override string[] GetTypedDefaultValue() => _defaultValue.Value;

        #endregion
    }
}
