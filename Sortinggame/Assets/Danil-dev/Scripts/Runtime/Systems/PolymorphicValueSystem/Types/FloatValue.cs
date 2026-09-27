using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class FloatValue : PolymorphicValue<float> { }

    [System.Serializable]
    public sealed class FloatConstantValue : ConstantValue<float>
    {
        #region Cloning

        public override PolymorphicValue<float> Clone()
        {
            return new FloatConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class FloatScriptableVariableValue : ScriptableVariableValue<FloatScriptableVariable,float>
    {
        #region Cloning

        public override PolymorphicValue<float> Clone()
        {
            return new FloatScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
