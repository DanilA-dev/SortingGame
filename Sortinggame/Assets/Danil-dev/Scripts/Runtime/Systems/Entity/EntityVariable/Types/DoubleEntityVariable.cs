using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class DoubleEntityVariable : PolymorphicEntityVariable<PolymorphicValue<double>>
    {
        #region Constructor

        public DoubleEntityVariable() { }
        public DoubleEntityVariable(StringScriptableVariable id, PolymorphicValue<double> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new DoubleEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}
