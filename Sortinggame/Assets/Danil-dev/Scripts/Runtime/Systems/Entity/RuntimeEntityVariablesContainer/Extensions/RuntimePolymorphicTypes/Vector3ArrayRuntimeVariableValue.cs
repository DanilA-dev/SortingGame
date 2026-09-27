using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class Vector3ArrayRuntimeVariableValue : PolymorphicRuntimeVariableValue<Vector3ArrayEntityVariable, Vector3[]>
    {
        #region Properties

        protected override Vector3[] DefaultValue => Array.Empty<Vector3>();

        #endregion

        #region Clone

        public override PolymorphicValue<Vector3[]> Clone()
        {
            return new Vector3ArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
