using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class Vector3Value : PolymorphicValue<Vector3> { }

    [System.Serializable]
    public sealed class Vector3ConstantValue : ConstantValue<Vector3>
    {
        #region Cloning

        public override PolymorphicValue<Vector3> Clone()
        {
            return new Vector3ConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class Vector3ScriptableVariableValue : ScriptableVariableValue<Vector3ScriptableVariable,Vector3>
    {
        #region Cloning

        public override PolymorphicValue<Vector3> Clone()
        {
            return new Vector3ScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
