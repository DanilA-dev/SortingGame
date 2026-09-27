using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class Vector2RuntimeVariableValue : PolymorphicRuntimeVariableValue<Vector2EntityVariable, Vector2>
    {
        #region Clone

        public override PolymorphicValue<Vector2> Clone()
        {
            return new Vector2RuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
