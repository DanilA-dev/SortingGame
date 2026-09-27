using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class TransformArrayEntityVariable : PolymorphicEntityVariable<PolymorphicValue<Transform[]>>
    {
        #region Constructor

        public TransformArrayEntityVariable() { }
        public TransformArrayEntityVariable(StringScriptableVariable id, PolymorphicValue<Transform[]> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new TransformArrayEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}