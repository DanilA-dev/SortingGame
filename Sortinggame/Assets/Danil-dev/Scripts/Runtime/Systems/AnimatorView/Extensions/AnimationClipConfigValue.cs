using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;

namespace D_Dev.AnimatorView.Extensions
{
    [System.Serializable]
    public abstract class AnimationClipConfigValue : PolymorphicValue<AnimationClipConfig> {}

    [System.Serializable]
    public sealed class AnimationClipConfigConstantValue : ConstantValue<AnimationClipConfig>
    {
        #region Cloning

        public override PolymorphicValue<AnimationClipConfig> Clone()
        {
            return new AnimationClipConfigConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public class AnimationClipConfigRuntimeVariableValue : EntityRuntimeVariableValue<AnimationClipConfigEntityVariable, AnimationClipConfig>
    {
        #region Clone

        public override PolymorphicValue<AnimationClipConfig> Clone()
        {
            return new AnimationClipConfigRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
