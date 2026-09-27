using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class BoolArrayRuntimeVariableValue : PolymorphicRuntimeVariableValue<BoolArrayEntityVariable, bool[]>
    {
        #region Properties

        protected override bool[] DefaultValue => Array.Empty<bool>();

        #endregion

        #region Clone

        public override PolymorphicValue<bool[]> Clone()
        {
            return new BoolArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
