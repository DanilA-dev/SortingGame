using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;

namespace D_Dev.CurrencySystem.Extensions
{
    [System.Serializable]
    public abstract class CurrencyInfoValue : PolymorphicValue<CurrencyInfo>{}
    
    [System.Serializable]
    public class CurrencyInfoConstantValue : ConstantValue<CurrencyInfo>
    {
        #region Clone

        public override PolymorphicValue<CurrencyInfo> Clone()
        {
            return new CurrencyInfoConstantValue() { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public class CurrencyInfoRuntimeVariableValue : EntityRuntimeVariableValue<CurrencyInfoEntityVariable, CurrencyInfo>
    {
        #region Clone

        public override PolymorphicValue<CurrencyInfo> Clone()
        {
            return new CurrencyInfoRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
