using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class TransformRuntimeVariableValue : PolymorphicRuntimeVariableValue<TransformEntityVariable, Transform>
    {
        #region Clone

        public override PolymorphicValue<Transform> Clone()
        {
            return new TransformRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
