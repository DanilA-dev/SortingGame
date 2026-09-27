using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class Vector2ArrayValue : PolymorphicValue<Vector2[]> { }

    [System.Serializable]
    public sealed class Vector2ArrayConstantValue : ConstantValue<Vector2[]>
    {
        #region Cloning

        public override PolymorphicValue<Vector2[]> Clone()
        {
            return new Vector2ArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class Vector2ArrayScriptableVariableValue : ScriptableVariableValue<Vector2ArrayScriptableVariable,Vector2[]>
    {
        #region Cloning

        public override PolymorphicValue<Vector2[]> Clone()
        {
            return new Vector2ArrayScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
