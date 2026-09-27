using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class DoubleArrayRuntimeVariableValue : PolymorphicRuntimeVariableValue<DoubleArrayEntityVariable, double[]>
    {
        #region Properties

        protected override double[] DefaultValue => Array.Empty<double>();

        #endregion

        #region Clone

        public override PolymorphicValue<double[]> Clone()
        {
            return new DoubleArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
