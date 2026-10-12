using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;

namespace D_Dev.Entity.Extensions
{
    [System.Serializable]
    public abstract class EntityInfoArrayValue : PolymorphicValue<EntityInfo[]> {}

    [System.Serializable]
    public sealed class EntityInfoArrayConstantValue : ConstantValue<EntityInfo[]>
    {
        #region Cloning

        public override PolymorphicValue<EntityInfo[]> Clone()
        {
            return new EntityInfoArrayConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public class EntityInfoArrayRuntimeVariableValue : EntityRuntimeVariableValue<EntityInfoArrayEntityVariable, EntityInfo[]>
    {
        #region Clone

        public override PolymorphicValue<EntityInfo[]> Clone()
        {
            return new EntityInfoArrayRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }
}
