using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class GameObjectRuntimeVariableValue : PolymorphicRuntimeVariableValue<GameObjectEntityVariable, GameObject>
    {
        #region Clone

        public override PolymorphicValue<GameObject> Clone()
        {
            return new GameObjectRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
