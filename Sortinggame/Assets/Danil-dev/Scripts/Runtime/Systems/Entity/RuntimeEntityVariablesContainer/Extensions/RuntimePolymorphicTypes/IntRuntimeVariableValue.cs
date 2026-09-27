using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class IntRuntimeVariableValue : PolymorphicRuntimeVariableValue<IntEntityVariable, int>
    {
        #region Clone

        public override PolymorphicValue<int> Clone()
        {
            return new IntRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
