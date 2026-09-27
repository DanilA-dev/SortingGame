using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class GameObjectArrayValue : PolymorphicValue<GameObject[]> { }

    [System.Serializable]
    public sealed class GameObjectArrayConstantValue : ConstantValue<GameObject[]>
    {
        #region Cloning

        public override PolymorphicValue<GameObject[]> Clone()
        {
            return new GameObjectArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class GameObjectArrayScriptableVariableValue : ScriptableVariableValue<GameObjectArrayScriptableVariable,GameObject[]>
    {
        #region Cloning

        public override PolymorphicValue<GameObject[]> Clone()
        {
            return new GameObjectArrayScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }
}
