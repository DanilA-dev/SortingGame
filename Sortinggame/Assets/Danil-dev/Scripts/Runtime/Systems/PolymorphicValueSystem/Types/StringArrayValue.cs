using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class StringArrayValue : PolymorphicValue<string[]> { }

    [System.Serializable]
    public sealed class StringArrayConstantValue : ConstantValue<string[]>
    {
        #region Cloning

        public override PolymorphicValue<string[]> Clone()
        {
            return new StringArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class StringArrayScriptableVariableValue : ScriptableVariableValue<StringArrayScriptableVariable,string[]>
    {
        #region Cloning

        public override PolymorphicValue<string[]> Clone()
        {
            return new StringArrayScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
