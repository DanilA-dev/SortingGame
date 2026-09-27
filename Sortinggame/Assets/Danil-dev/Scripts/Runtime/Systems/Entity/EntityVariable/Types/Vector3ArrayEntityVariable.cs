using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class Vector3ArrayEntityVariable : PolymorphicEntityVariable<PolymorphicValue<Vector3[]>>
    {
        #region Constructor

        public Vector3ArrayEntityVariable() { }
        public Vector3ArrayEntityVariable(StringScriptableVariable id, PolymorphicValue<Vector3[]> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new Vector3ArrayEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}
