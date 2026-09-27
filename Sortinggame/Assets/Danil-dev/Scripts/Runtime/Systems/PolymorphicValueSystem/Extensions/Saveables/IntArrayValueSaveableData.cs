using D_Dev.PolymorphicValueSystem;
using D_Dev.SaveSystem.SaveableData;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions.Saveables
{
    [System.Serializable]
    public class IntArrayValueSaveableData : BaseSaveableData<int[]>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<int[]> _value = new IntArrayConstantValue();
        [SerializeReference] private PolymorphicValue<int[]> _defaultValue = new IntArrayConstantValue();

        #endregion

        #region Properties

        public PolymorphicValue<int[]> DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        #endregion

        #region Overrides

        protected override int[] GetTypedSaveData() => _value.Value;

        protected override void SetTypedSaveData(int[] data) => _value.Value = data;

        protected override int[] GetTypedDefaultValue() => _defaultValue.Value;

        #endregion
    }
}
