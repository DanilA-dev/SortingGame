using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class FloatRuntimeVariableValue : PolymorphicRuntimeVariableValue<FloatEntityVariable, float>
    {
        #region Clone

        public override PolymorphicValue<float> Clone()
        {
            return new FloatRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
