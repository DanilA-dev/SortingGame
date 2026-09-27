using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class Vector2ArrayEntityVariable : PolymorphicEntityVariable<PolymorphicValue<Vector2[]>>
    {
        #region Constructor

        public Vector2ArrayEntityVariable() { }
        public Vector2ArrayEntityVariable(StringScriptableVariable id, PolymorphicValue<Vector2[]> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new Vector2ArrayEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}
