using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class TransformArrayValue : PolymorphicValue<Transform[]> { }

    [System.Serializable]
    public sealed class TransformArrayConstantValue : ConstantValue<Transform[]>
    {
        #region Cloning

        public override PolymorphicValue<Transform[]> Clone()
        {
            return new TransformArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class TransformArrayScriptableVariableValue : ScriptableVariableValue<TransformArrayScriptableVariable,Transform[]>
    {
        #region Cloning

        public override PolymorphicValue<Transform[]> Clone()
        {
            return new TransformArrayScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
