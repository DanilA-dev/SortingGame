using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class DoubleArrayValue : PolymorphicValue<double[]> { }

    [System.Serializable]
    public sealed class DoubleArrayConstantValue : ConstantValue<double[]>
    {
        #region Cloning

        public override PolymorphicValue<double[]> Clone()
        {
            return new DoubleArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class DoubleArrayScriptableVariableValue : ScriptableVariableValue<DoubleArrayScriptableVariable,double[]>
    {
        #region Cloning

        public override PolymorphicValue<double[]> Clone()
        {
            return new DoubleArrayScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
