using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class Vector3ArrayValue : PolymorphicValue<Vector3[]> { }

    [System.Serializable]
    public sealed class Vector3ArrayConstantValue : ConstantValue<Vector3[]>
    {
        #region Cloning

        public override PolymorphicValue<Vector3[]> Clone()
        {
            return new Vector3ArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class Vector3ArrayScriptableVariableValue : ScriptableVariableValue<Vector3ArrayScriptableVariable,Vector3[]>
    {
        #region Cloning

        public override PolymorphicValue<Vector3[]> Clone()
        {
            return new Vector3ArrayScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
