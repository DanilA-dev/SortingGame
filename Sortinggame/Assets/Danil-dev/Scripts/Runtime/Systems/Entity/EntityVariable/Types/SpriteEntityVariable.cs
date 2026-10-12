using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class SpriteEntityVariable : PolymorphicEntityVariable<PolymorphicValue<Sprite>>
    {
        #region Constructor

        public SpriteEntityVariable() { }
        public SpriteEntityVariable(StringScriptableVariable id, PolymorphicValue<Sprite> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new SpriteEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}
