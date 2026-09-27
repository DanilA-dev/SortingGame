using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class BoolRuntimeVariableValue : PolymorphicRuntimeVariableValue<BoolEntityVariable, bool>
    {
        #region Clone

        public override PolymorphicValue<bool> Clone()
        {
            return new BoolRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
