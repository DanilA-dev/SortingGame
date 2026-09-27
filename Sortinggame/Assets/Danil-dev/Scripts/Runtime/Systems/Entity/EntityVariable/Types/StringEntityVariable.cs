using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class StringEntityVariable : PolymorphicEntityVariable<PolymorphicValue<string>>
    {
        #region Constructor

        public StringEntityVariable() { }
        public StringEntityVariable(StringScriptableVariable id, PolymorphicValue<string> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new StringEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}
