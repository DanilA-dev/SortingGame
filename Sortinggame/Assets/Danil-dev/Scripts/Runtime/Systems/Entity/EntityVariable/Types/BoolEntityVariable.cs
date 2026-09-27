using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class BoolEntityVariable : PolymorphicEntityVariable<PolymorphicValue<bool>>
    {
        #region Constructor

        public BoolEntityVariable() { }
        public BoolEntityVariable(StringScriptableVariable id, PolymorphicValue<bool> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new BoolEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}
