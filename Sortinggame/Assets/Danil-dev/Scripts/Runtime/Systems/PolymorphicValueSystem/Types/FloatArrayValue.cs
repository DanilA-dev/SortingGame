using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class FloatArrayValue : PolymorphicValue<float[]> { }

    [System.Serializable]
    public sealed class FloatArrayConstantValue : ConstantValue<float[]>
    {
        #region Cloning

        public override PolymorphicValue<float[]> Clone()
        {
            return new FloatArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class FloatArrayScriptableVariableValue : ScriptableVariableValue<FloatArrayScriptableVariable,float[]>
    {
        #region Cloning

        public override PolymorphicValue<float[]> Clone()
        {
            return new FloatArrayScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
