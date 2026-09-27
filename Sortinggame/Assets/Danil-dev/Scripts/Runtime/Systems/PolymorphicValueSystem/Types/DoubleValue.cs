using D_Dev.ScriptableVariables;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class DoubleValue : PolymorphicValue<double> { }

    [System.Serializable]
    public sealed class DoubleConstantValue : ConstantValue<double>
    {
        #region Cloning

        public override PolymorphicValue<double> Clone()
        {
            return new DoubleConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class DoubleScriptableVariableValue : ScriptableVariableValue<DoubleScriptableVariable,double>
    {
        #region Cloning

        public override PolymorphicValue<double> Clone()
        {
            return new DoubleScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
