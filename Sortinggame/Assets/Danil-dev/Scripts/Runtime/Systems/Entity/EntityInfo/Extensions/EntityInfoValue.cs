using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;

namespace D_Dev.Entity.Extensions
{
    [System.Serializable]
    public abstract class EntityInfoValue : PolymorphicValue<EntityInfo> {}

    [System.Serializable]
    public sealed class EntityInfoConstantValue : ConstantValue<EntityInfo>
    {
        #region Cloning

        public override PolymorphicValue<EntityInfo> Clone()
        {
            return new EntityInfoConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public class EntityInfoRuntimeVariableValue : EntityRuntimeVariableValue<EntityInfoEntityVariable, EntityInfo>
    {
        #region Clone

        public override PolymorphicValue<EntityInfo> Clone()
        {
            return new EntityInfoRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
