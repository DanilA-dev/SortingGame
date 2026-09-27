using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class Vector2ArrayRuntimeVariableValue : PolymorphicRuntimeVariableValue<Vector2ArrayEntityVariable, Vector2[]>
    {
        #region Properties

        protected override Vector2[] DefaultValue => Array.Empty<Vector2>();

        #endregion

        #region Clone

        public override PolymorphicValue<Vector2[]> Clone()
        {
            return new Vector2ArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
