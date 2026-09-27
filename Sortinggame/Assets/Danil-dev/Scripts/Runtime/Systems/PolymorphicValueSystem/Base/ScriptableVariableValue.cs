using System;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class ScriptableVariableValue<TVariable, T> : PolymorphicValue<T>
        where TVariable : BaseScriptableVariable<T>
    {
        #region Fields

        [SerializeField] protected TVariable _variable;

        [NonSerialized] private TVariable _subscribedVariable;

        #endregion

        #region Properties

        public override T Value
        {
            get
            {
                if (_variable == null)
                    return default;

                return _variable.Value;
            }
            set
            {
                if(_variable == null)
                    return;

                _variable.Value = value;
            }
        }

        #endregion

        #region Overrides

        protected override void SubscribeToSource()
        {
            _subscribedVariable = _variable;

            if (_subscribedVariable != null)
                _subscribedVariable.OnValueUpdate += RaiseValueChanged;
        }

        protected override void UnsubscribeFromSource()
        {
            if (_subscribedVariable != null)
                _subscribedVariable.OnValueUpdate -= RaiseValueChanged;

            _subscribedVariable = null;
        }

        #endregion
    }
}
