using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class IntArrayEntityVariable : PolymorphicEntityVariable<PolymorphicValue<int[]>>
    {
        #region Constructor

        public IntArrayEntityVariable() { }
        public IntArrayEntityVariable(StringScriptableVariable id, PolymorphicValue<int[]> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new IntArrayEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}
