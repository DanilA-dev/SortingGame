using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class TransformEntityVariable : PolymorphicEntityVariable<PolymorphicValue<Transform>>
    {
        #region Constructor

        public TransformEntityVariable() { }
        public TransformEntityVariable(StringScriptableVariable id, PolymorphicValue<Transform> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new TransformEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}