using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class FloatEntityVariable : PolymorphicEntityVariable<PolymorphicValue<float>>
    {
        #region Constructors

        public FloatEntityVariable() {}
        public FloatEntityVariable(StringScriptableVariable id,PolymorphicValue<float> value) : base(id,value) {}


        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new FloatEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}
