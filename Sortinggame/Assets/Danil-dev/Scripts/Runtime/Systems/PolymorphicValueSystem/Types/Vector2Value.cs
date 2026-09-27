using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class Vector2Value : PolymorphicValue<Vector2> { }

    [System.Serializable]
    public sealed class Vector2ConstantValue : ConstantValue<Vector2>
    {
        #region Cloning

        public override PolymorphicValue<Vector2> Clone()
        {
            return new Vector2ConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class Vector2ScriptableVariableValue : ScriptableVariableValue<Vector2ScriptableVariable,Vector2>
    {
        #region Cloning

        public override PolymorphicValue<Vector2> Clone()
        {
            return new Vector2ScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
