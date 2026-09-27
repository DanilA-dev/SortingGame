using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class StringArrayEntityVariable : PolymorphicEntityVariable<PolymorphicValue<string[]>>
    {
        #region Constructor

        public StringArrayEntityVariable() { }
        public StringArrayEntityVariable(StringScriptableVariable id, PolymorphicValue<string[]> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new StringArrayEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}
