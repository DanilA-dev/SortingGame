using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class BoolArrayValue : PolymorphicValue<bool[]> { }

    [System.Serializable]
    public sealed class BoolArrayConstantValue : ConstantValue<bool[]>
    {
        #region Cloning

        public override PolymorphicValue<bool[]> Clone()
        {
            return new BoolArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class BoolArrayScriptableVariableValue : ScriptableVariableValue<BoolArrayScriptableVariable,bool[]>
    {
        #region Cloning

        public override PolymorphicValue<bool[]> Clone()
        {
            return new BoolArrayScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
