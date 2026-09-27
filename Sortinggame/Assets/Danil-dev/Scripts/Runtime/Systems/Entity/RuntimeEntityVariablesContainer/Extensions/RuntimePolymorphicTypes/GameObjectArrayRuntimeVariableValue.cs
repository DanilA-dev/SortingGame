using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class GameObjectArrayRuntimeVariableValue : PolymorphicRuntimeVariableValue<GameObjectArrayEntityVariable, GameObject[]>
    {
        #region Properties

        protected override GameObject[] DefaultValue => Array.Empty<GameObject>();

        #endregion

        #region Clone

        public override PolymorphicValue<GameObject[]> Clone()
        {
            return new GameObjectArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
