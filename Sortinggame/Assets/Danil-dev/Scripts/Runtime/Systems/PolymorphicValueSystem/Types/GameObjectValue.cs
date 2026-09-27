using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class GameObjectValue : PolymorphicValue<GameObject> { }

    [System.Serializable]
    public sealed class GameObjectConstantValue : ConstantValue<GameObject>
    {
        #region Cloning

        public override PolymorphicValue<GameObject> Clone()
        {
            return new GameObjectConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class GameObjectScriptableVariableValue : ScriptableVariableValue<GameObjectScriptableVariable,GameObject>
    {
        #region Cloning

        public override PolymorphicValue<GameObject> Clone()
        {
            return new GameObjectScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
