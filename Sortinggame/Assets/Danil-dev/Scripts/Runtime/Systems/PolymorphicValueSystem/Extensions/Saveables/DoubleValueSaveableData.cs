using D_Dev.PolymorphicValueSystem;
using D_Dev.SaveSystem.SaveableData;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions.Saveables
{
    [System.Serializable]
    public class DoubleValueSaveableData : BaseSaveableData<double>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<double> _value = new DoubleConstantValue();
        [SerializeReference] private PolymorphicValue<double> _defaultValue = new DoubleConstantValue();

        #endregion

        #region Properties

        public PolymorphicValue<double> DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        #endregion

        #region Overrides

        protected override double GetTypedSaveData() => _value.Value;

        protected override void SetTypedSaveData(double data) => _value.Value = data;

        protected override double GetTypedDefaultValue() => _defaultValue.Value;

        #endregion
    }
}
