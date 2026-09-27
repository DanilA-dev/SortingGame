using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class StringArrayRuntimeVariableValue : PolymorphicRuntimeVariableValue<StringArrayEntityVariable, string[]>
    {
        #region Properties

        protected override string[] DefaultValue => Array.Empty<string>();

        #endregion

        #region Clone

        public override PolymorphicValue<string[]> Clone()
        {
            return new StringArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
