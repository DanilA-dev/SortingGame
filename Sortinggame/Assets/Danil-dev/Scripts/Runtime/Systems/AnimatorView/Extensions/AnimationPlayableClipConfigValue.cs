using D_Dev.AnimatorView.AnimationPlayableHandler;
using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;

namespace D_Dev.AnimatorView.Extensions
{
    [System.Serializable]
    public abstract class AnimationPlayableClipConfigValue : PolymorphicValue<AnimationPlayableClipConfig> {}

    [System.Serializable]
    public sealed class AnimationPlayableClipConfigConstantValue : ConstantValue<AnimationPlayableClipConfig>
    {
        #region Cloning

        public override PolymorphicValue<AnimationPlayableClipConfig> Clone()
        {
            return new AnimationPlayableClipConfigConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public class AnimationPlayableClipConfigRuntimeVariableValue : EntityRuntimeVariableValue<AnimationPlayableClipConfigEntityVariable, AnimationPlayableClipConfig>
    {
        #region Clone

        public override PolymorphicValue<AnimationPlayableClipConfig> Clone()
        {
            return new AnimationPlayableClipConfigRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
