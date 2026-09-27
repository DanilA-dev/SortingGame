using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class Vector3RuntimeVariableValue : PolymorphicRuntimeVariableValue<Vector3EntityVariable, Vector3>
    {
        #region Clone

        public override PolymorphicValue<Vector3> Clone()
        {
            return new Vector3RuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
