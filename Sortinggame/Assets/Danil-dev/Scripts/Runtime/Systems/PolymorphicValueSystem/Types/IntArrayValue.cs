using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class IntArrayValue : PolymorphicValue<int[]> { }

    [System.Serializable]
    public sealed class IntArrayConstantValue : ConstantValue<int[]>
    {
        #region Cloning

        public override PolymorphicValue<int[]> Clone()
        {
            return new IntArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class IntArrayScriptableVariableValue : ScriptableVariableValue<IntArrayScriptableVariable,int[]>
    {
        #region Cloning

        public override PolymorphicValue<int[]> Clone()
        {
            return new IntArrayScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
