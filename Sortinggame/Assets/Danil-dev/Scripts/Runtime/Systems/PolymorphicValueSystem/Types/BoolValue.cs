using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class BoolValue : PolymorphicValue<bool> { }

    [System.Serializable]
    public sealed class BoolConstantValue : ConstantValue<bool>
    {
        #region Cloning

        public override PolymorphicValue<bool> Clone()
        {
            return new BoolConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class BoolScriptableVariableValue : ScriptableVariableValue<BoolScriptableVariable,bool>
    {
        #region Cloning

        public override PolymorphicValue<bool> Clone()
        {
            return new BoolScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
