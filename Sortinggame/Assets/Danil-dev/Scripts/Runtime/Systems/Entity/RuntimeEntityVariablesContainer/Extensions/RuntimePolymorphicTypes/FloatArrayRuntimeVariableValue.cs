using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class FloatArrayRuntimeVariableValue : PolymorphicRuntimeVariableValue<FloatArrayEntityVariable, float[]>
    {
        #region Properties

        protected override float[] DefaultValue => Array.Empty<float>();

        #endregion

        #region Clone

        public override PolymorphicValue<float[]> Clone()
        {
            return new FloatArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
