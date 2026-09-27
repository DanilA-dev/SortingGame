using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class GameObjectEntityVariable : PolymorphicEntityVariable<PolymorphicValue<GameObject>>
    {
        #region Constructor

        public GameObjectEntityVariable() { }
        public GameObjectEntityVariable(StringScriptableVariable id, PolymorphicValue<GameObject> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new GameObjectEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}