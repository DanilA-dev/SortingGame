using D_Dev.DamageableSystem;
using D_Dev.EntityVariable;
using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;
using D_Dev.ScriptableVariables;

namespace D_Dev.Extensions
{
    [System.Serializable]
    public abstract class DamageDataValue : PolymorphicValue<DamageData> {}

    [System.Serializable]
    public class DamageDataConstantValue : ConstantValue<DamageData>
    {
        #region Clone

        public override PolymorphicValue<DamageData> Clone()
        {
            return new DamageDataConstantValue { _value = Value };
        }

        #endregion
    }
    
    [System.Serializable]
    public class DamageDataRuntimeVariableValue : EntityRuntimeVariableValue<DamageDataEntityVariable, DamageData>
    {
        #region Properties

        protected override DamageData DefaultValue => new DamageData();

        #endregion

        #region Clone

        public override PolymorphicValue<DamageData> Clone()
        {
            return new DamageDataRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }

    [System.Serializable]
    public class DamageDataEntityVariable : EntityVariable<DamageData>
    {
        #region Constructors

        public DamageDataEntityVariable() {}
        
        public DamageDataEntityVariable(StringScriptableVariable id, DamageData value) : base(id, value) {}

        #endregion
        
        #region Overrides
        
        public override BaseEntityVariable Clone()
        {
            return new DamageDataEntityVariable(_variableID, _value);
        }
        
        #endregion
    }
}
