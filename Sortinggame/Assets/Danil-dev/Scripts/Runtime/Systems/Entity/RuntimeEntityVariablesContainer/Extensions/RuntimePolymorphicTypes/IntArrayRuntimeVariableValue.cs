using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class IntArrayRuntimeVariableValue : PolymorphicRuntimeVariableValue<IntArrayEntityVariable, int[]>
    {
        #region Properties

        protected override int[] DefaultValue => Array.Empty<int>();

        #endregion

        #region Clone

        public override PolymorphicValue<int[]> Clone()
        {
            return new IntArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
