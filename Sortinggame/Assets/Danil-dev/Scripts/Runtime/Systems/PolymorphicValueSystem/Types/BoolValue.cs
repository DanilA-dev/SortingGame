using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class BoolValue : PolymorphicValue<bool> { }

    [System.Serializable]
    public sealed class BoolConstantValue : ConstantValue<bool>
    {
        #region Cloning

        public override PolymorphicValue<bool> Clone()
        {
            return new BoolConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class BoolScriptableVariableValue : ScriptableVariableValue<BoolScriptableVariable,bool>
    {
        #region Cloning

        public override PolymorphicValue<bool> Clone()
        {
            return new BoolScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class BoolInvertedValue : PolymorphicValue<bool>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<bool> _value = new BoolConstantValue();

        #endregion

        #region Properties

        public override bool Value
        {
            get => _value != null && !_value.Value;
            set
            {
                if (_value != null)
                    _value.Value = !value;
            }
        }

        #endregion

        #region Overrides

        protected override void SubscribeToSource()
        {
            if (_value != null)
                _value.OnValueChanged += OnSourceChanged;
        }

        protected override void UnsubscribeFromSource()
        {
            if (_value != null)
                _value.OnValueChanged -= OnSourceChanged;
        }

        #endregion

        #region Cloning

        public override PolymorphicValue<bool> Clone()
        {
            return new BoolInvertedValue { _value = _value?.Clone() };
        }

        #endregion

        #region Listeners

        private void OnSourceChanged(bool _) => RaiseValueChanged(Value);

        #endregion
    }
}
