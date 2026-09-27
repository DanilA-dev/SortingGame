using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class ColliderRuntimeVariableValue : EntityRuntimeVariableValue<ColliderEntityVariable, Collider>
    {
        #region Clone

        public override PolymorphicValue<Collider> Clone()
        {
            return new ColliderRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
