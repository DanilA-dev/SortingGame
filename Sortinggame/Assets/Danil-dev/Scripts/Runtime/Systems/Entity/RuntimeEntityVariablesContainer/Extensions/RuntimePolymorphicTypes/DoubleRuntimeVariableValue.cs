using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class DoubleRuntimeVariableValue : PolymorphicRuntimeVariableValue<DoubleEntityVariable, double>
    {
        #region Clone

        public override PolymorphicValue<double> Clone()
        {
            return new DoubleRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
