using D_Dev.PolymorphicValueSystem;
using D_Dev.SaveSystem.SaveableData;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem.Extensions.Saveables
{
    [System.Serializable]
    public class Vector3ArrayValueSaveableData : BaseSaveableData<Vector3[]>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<Vector3[]> _value = new Vector3ArrayConstantValue();
        [SerializeReference] private PolymorphicValue<Vector3[]> _defaultValue = new Vector3ArrayConstantValue();

        #endregion

        #region Properties

        public PolymorphicValue<Vector3[]> DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        #endregion

        #region Overrides

        protected override Vector3[] GetTypedSaveData() => _value.Value;

        protected override void SetTypedSaveData(Vector3[] data) => _value.Value = data;

        protected override Vector3[] GetTypedDefaultValue() => _defaultValue.Value;

        #endregion
    }
}
