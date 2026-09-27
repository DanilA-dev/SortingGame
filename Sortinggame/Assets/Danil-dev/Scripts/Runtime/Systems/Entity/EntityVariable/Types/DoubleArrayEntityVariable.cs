using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class DoubleArrayEntityVariable : PolymorphicEntityVariable<PolymorphicValue<double[]>>
    {
        #region Constructor

        public DoubleArrayEntityVariable() { }
        public DoubleArrayEntityVariable(StringScriptableVariable id, PolymorphicValue<double[]> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new DoubleArrayEntityVariable(_variableID, _value?.Clone());
        }
        #endregion
    }
}