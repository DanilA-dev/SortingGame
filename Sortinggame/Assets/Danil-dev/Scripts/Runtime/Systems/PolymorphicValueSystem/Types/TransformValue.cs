using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class TransformValue : PolymorphicValue<Transform> { }

    [System.Serializable]
    public sealed class TransformConstantValue : ConstantValue<Transform>
    {
        #region Cloning

        public override PolymorphicValue<Transform> Clone()
        {
            return new TransformConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class TransformScriptableVariableValue : ScriptableVariableValue<TransformScriptableVariable,Transform>
    {
        #region Cloning

        public override PolymorphicValue<Transform> Clone()
        {
            return new TransformScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
