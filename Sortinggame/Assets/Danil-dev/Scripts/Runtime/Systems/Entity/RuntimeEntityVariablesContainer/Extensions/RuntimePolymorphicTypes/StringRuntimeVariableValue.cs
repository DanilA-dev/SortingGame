using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class StringRuntimeVariableValue : PolymorphicRuntimeVariableValue<StringEntityVariable, string>
    {
        #region Clone

        public override PolymorphicValue<string> Clone()
        {
            return new StringRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
