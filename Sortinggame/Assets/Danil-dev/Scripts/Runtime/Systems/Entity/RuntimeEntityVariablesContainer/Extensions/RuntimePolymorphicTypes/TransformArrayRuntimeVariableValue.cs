using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class TransformArrayRuntimeVariableValue : PolymorphicRuntimeVariableValue<TransformArrayEntityVariable, Transform[]>
    {
        #region Properties

        protected override Transform[] DefaultValue => Array.Empty<Transform>();

        #endregion

        #region Clone

        public override PolymorphicValue<Transform[]> Clone()
        {
            return new TransformArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
