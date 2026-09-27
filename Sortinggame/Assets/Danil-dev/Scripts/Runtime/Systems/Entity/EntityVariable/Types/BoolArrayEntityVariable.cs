using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class BoolArrayEntityVariable : PolymorphicEntityVariable<PolymorphicValue<bool[]>>
    {
        #region Constructor

        public BoolArrayEntityVariable() { }
        public BoolArrayEntityVariable(StringScriptableVariable id, PolymorphicValue<bool[]> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new BoolArrayEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}