using D_Dev.ColliderEvents.Extensions.ScriptableVariables;
using D_Dev.EntityVariable;
using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;
using D_Dev.ScriptableVariables;

namespace D_Dev.ColliderEvents.Extensions
{
    [System.Serializable]
    public abstract class CollisionColliderObservableValue : PolymorphicValue<CollisionColliderObservable> {}

    [System.Serializable]
    public sealed class CollisionColliderObservableConstantValue : ConstantValue<CollisionColliderObservable>
    {
        #region Cloning

        public override PolymorphicValue<CollisionColliderObservable> Clone()
        {
            return new CollisionColliderObservableConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class CollisionColliderObservableScriptableVariableValue : ScriptableVariableValue<CollisionColliderObservableScriptableVariable, CollisionColliderObservable>
    {
        #region Cloning

        public override PolymorphicValue<CollisionColliderObservable> Clone()
        {
            return new CollisionColliderObservableScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class CollisionColliderObservableRuntimeVariableValue : EntityRuntimeVariableValue<CollisionColliderObservableEntityVariable, CollisionColliderObservable>
    {
        #region Cloning

        public override PolymorphicValue<CollisionColliderObservable> Clone()
        {
            return new CollisionColliderObservableRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }

    [System.Serializable]
    public class CollisionColliderObservableEntityVariable : EntityVariable<CollisionColliderObservable>
    {
        #region Constructors

        public CollisionColliderObservableEntityVariable() {}
        
        public CollisionColliderObservableEntityVariable(StringScriptableVariable id, CollisionColliderObservable value) : base(id, value) {}

        #endregion
        
        #region Overrides
        
        public override BaseEntityVariable Clone()
        {
            return new CollisionColliderObservableEntityVariable(_variableID, _value);
        }
        
        #endregion
    }
}
