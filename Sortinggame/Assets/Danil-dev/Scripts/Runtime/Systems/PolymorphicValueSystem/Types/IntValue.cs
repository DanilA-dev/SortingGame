using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class IntValue : PolymorphicValue<int> { }

    [System.Serializable]
    public sealed class IntConstantValue : ConstantValue<int>
    {
        #region Cloning

        public override PolymorphicValue<int> Clone()
        {
            return new IntConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class IntScriptableVariableValue : ScriptableVariableValue<IntScriptableVariable,int>
    {
        #region Cloning

        public override PolymorphicValue<int> Clone()
        {
            return new IntScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
