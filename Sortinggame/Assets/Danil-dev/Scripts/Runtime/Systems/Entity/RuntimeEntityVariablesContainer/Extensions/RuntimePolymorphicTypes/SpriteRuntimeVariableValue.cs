using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables.Extensions
{
    [System.Serializable]
    public class SpriteRuntimeVariableValue : PolymorphicRuntimeVariableValue<SpriteEntityVariable, Sprite>
    {
        #region Clone

        public override PolymorphicValue<Sprite> Clone()
        {
            return new SpriteRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
