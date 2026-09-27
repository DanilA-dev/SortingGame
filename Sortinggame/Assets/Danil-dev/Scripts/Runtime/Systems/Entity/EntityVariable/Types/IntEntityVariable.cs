using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class IntEntityVariable : PolymorphicEntityVariable<PolymorphicValue<int>>
    {
        #region Constructor

        public IntEntityVariable() { }
        public IntEntityVariable(StringScriptableVariable id, PolymorphicValue<int> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new IntEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}     
