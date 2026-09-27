using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class GameObjectArrayEntityVariable : PolymorphicEntityVariable<PolymorphicValue<GameObject[]>>
    {
        #region Constructor

        public GameObjectArrayEntityVariable() { }
        public GameObjectArrayEntityVariable(StringScriptableVariable id, PolymorphicValue<GameObject[]> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new GameObjectArrayEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}