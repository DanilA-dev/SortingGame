using D_Dev.ColliderEvents.Extensions.ScriptableVariables;
using D_Dev.EntityVariable;
using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;
using D_Dev.ScriptableVariables;

namespace D_Dev.ColliderEvents.Extensions
{
    [System.Serializable]
    public abstract class TriggerColliderObservableValue : PolymorphicValue<TriggerColliderObservable> {}

    [System.Serializable]
    public sealed class TriggerColliderObservableConstantValue : ConstantValue<TriggerColliderObservable>
    {
        #region Cloning

        public override PolymorphicValue<TriggerColliderObservable> Clone()
        {
            return new TriggerColliderObservableConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class TriggerColliderObservableScriptableVariableValue : ScriptableVariableValue<TriggerColliderObservableScriptableVariable, TriggerColliderObservable>
    {
        #region Cloning

        public override PolymorphicValue<TriggerColliderObservable> Clone()
        {
            return new TriggerColliderObservableScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class TriggerColliderObservableRuntimeVariableValue : EntityRuntimeVariableValue<TriggerColliderObservableEntityVariable, TriggerColliderObservable>
    {
        #region Cloning

        public override PolymorphicValue<TriggerColliderObservable> Clone()
        {
            return new TriggerColliderObservableRuntimeVariableValue
            {
                _variableID = _variableID,
                _runtimeEntityVariablesContainer = _runtimeEntityVariablesContainer
            };
        }

        #endregion
    }

    [System.Serializable]
    public class TriggerColliderObservableEntityVariable : EntityVariable<TriggerColliderObservable>
    {
        #region Constructors

        public TriggerColliderObservableEntityVariable() {}
        
        public TriggerColliderObservableEntityVariable(StringScriptableVariable id, TriggerColliderObservable value) : base(id, value) {}

        #endregion
        
        #region Overrides
        
        public override BaseEntityVariable Clone()
        {
            return new TriggerColliderObservableEntityVariable(_variableID, _value);
        }
        
        #endregion
    }
}
