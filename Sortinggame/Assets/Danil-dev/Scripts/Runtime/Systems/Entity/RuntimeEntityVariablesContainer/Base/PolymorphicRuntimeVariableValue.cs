using System;
using D_Dev.EntityVariable.Types;
using D_Dev.PolymorphicValueSystem;

namespace D_Dev.RuntimeEntityVariables
{
    [System.Serializable]
    public abstract class PolymorphicRuntimeVariableValue<TVariable, T> : RuntimeVariableValue<TVariable, T>
        where TVariable : PolymorphicEntityVariable<PolymorphicValue<T>>
    {
        #region Fields

        [NonSerialized] private PolymorphicValue<T> _subscribedValue;

        #endregion

        #region Properties

        public override T Value
        {
            get
            {
                var variable = Variable;
                return variable?.Value != null ? variable.Value.Value : DefaultValue;
            }
            set
            {
                var variable = Variable;
                if (variable?.Value != null)
                    variable.Value.Value = value;
            }
        }

        #endregion

        #region Overrides

        protected override void SubscribeToVariable(TVariable variable)
        {
            _subscribedValue = variable.Value;

            if (_subscribedValue != null)
                _subscribedValue.OnValueChanged += RaiseValueChanged;
        }

        protected override void UnsubscribeFromVariable(TVariable variable)
        {
            if (_subscribedValue != null)
                _subscribedValue.OnValueChanged -= RaiseValueChanged;

            _subscribedValue = null;
        }

        #endregion
    }
}
