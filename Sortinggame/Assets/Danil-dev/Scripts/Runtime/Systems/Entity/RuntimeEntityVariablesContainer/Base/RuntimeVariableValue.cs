using System;
using D_Dev.EntityVariable;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.RuntimeEntityVariables
{
    [System.Serializable]
    public abstract class RuntimeVariableValue<TVariable, T> : PolymorphicValue<T>
        where TVariable : BaseEntityVariable
    {
        #region Fields

        [SerializeField] protected StringScriptableVariable _variableID;
        [SerializeField] protected RuntimeEntityVariablesContainer _runtimeEntityVariablesContainer;

        [NonSerialized] private TVariable _cachedVariable;
        [NonSerialized] private TVariable _subscribedVariable;
        [NonSerialized] private RuntimeEntityVariablesContainer _pendingContainer;

        #endregion

        #region Properties

        protected TVariable Variable
        {
            get
            {
                if (_cachedVariable == null && _runtimeEntityVariablesContainer != null)
                    _cachedVariable = _runtimeEntityVariablesContainer.GetVariable<TVariable>(_variableID);

                return _cachedVariable;
            }
        }

        protected virtual T DefaultValue => default;

        #endregion

        #region Overrides

        protected override void SubscribeToSource()
        {
            if (_runtimeEntityVariablesContainer == null)
                return;

            if (!_runtimeEntityVariablesContainer.IsInitialized)
            {
                _pendingContainer = _runtimeEntityVariablesContainer;
                _pendingContainer.OnInitialized.AddListener(OnContainerInitialized);
                return;
            }

            BindVariable();
        }

        protected override void UnsubscribeFromSource()
        {
            if (_pendingContainer != null)
            {
                _pendingContainer.OnInitialized.RemoveListener(OnContainerInitialized);
                _pendingContainer = null;
            }

            if (_subscribedVariable != null)
            {
                UnsubscribeFromVariable(_subscribedVariable);
                _subscribedVariable = null;
            }
        }

        #endregion

        #region Abstract

        protected abstract void SubscribeToVariable(TVariable variable);

        protected abstract void UnsubscribeFromVariable(TVariable variable);

        #endregion

        #region Private

        private void OnContainerInitialized()
        {
            if (_pendingContainer != null)
                _pendingContainer.OnInitialized.RemoveListener(OnContainerInitialized);

            _pendingContainer = null;
            BindVariable();
        }

        private void BindVariable()
        {
            _subscribedVariable = Variable;

            if (_subscribedVariable != null)
                SubscribeToVariable(_subscribedVariable);
        }

        #endregion
    }
}
