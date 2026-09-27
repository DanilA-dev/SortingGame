using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;

namespace D_Dev.EntityVariable.Types
{
    [System.Serializable]
    public class FloatArrayEntityVariable : PolymorphicEntityVariable<PolymorphicValue<float[]>>
    {
        #region Constructor

        public FloatArrayEntityVariable() { }
        public FloatArrayEntityVariable(StringScriptableVariable id, PolymorphicValue<float[]> value) : base(id, value) { }

        #endregion

        #region Overrides

        public override BaseEntityVariable Clone()
        {
            return new FloatArrayEntityVariable(_variableID, _value?.Clone());
        }

        #endregion
    }
}        
