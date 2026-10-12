using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class FloatValue : PolymorphicValue<float> { }

    [System.Serializable]
    public sealed class FloatConstantValue : ConstantValue<float>
    {
        #region Cloning

        public override PolymorphicValue<float> Clone()
        {
            return new FloatConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class FloatScriptableVariableValue : ScriptableVariableValue<FloatScriptableVariable,float>
    {
        #region Cloning

        public override PolymorphicValue<float> Clone()
        {
            return new FloatScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class FloatMultipliedValue : PolymorphicValue<float>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<float> _value = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<float> _multiplier = new FloatConstantValue();

        #endregion

        #region Properties

        public override float Value
        {
            get
            {
                if (_value == null)
                    return default;

                return _multiplier != null ? _value.Value * _multiplier.Value : _value.Value;
            }
            set
            {
                if (_value != null)
                    _value.Value = value;
            }
        }

        #endregion

        #region Overrides

        protected override void SubscribeToSource()
        {
            if (_value != null)
                _value.OnValueChanged += OnSourceChanged;

            if (_multiplier != null)
                _multiplier.OnValueChanged += OnSourceChanged;
        }

        protected override void UnsubscribeFromSource()
        {
            if (_value != null)
                _value.OnValueChanged -= OnSourceChanged;

            if (_multiplier != null)
                _multiplier.OnValueChanged -= OnSourceChanged;
        }

        #endregion

        #region Cloning

        public override PolymorphicValue<float> Clone()
        {
            return new FloatMultipliedValue
            {
                _value = _value?.Clone(),
                _multiplier = _multiplier?.Clone()
            };
        }

        #endregion

        #region Listeners

        private void OnSourceChanged(float _) => RaiseValueChanged(Value);

        #endregion
    }

    [System.Serializable]
    public sealed class FloatFromDoubleValue : PolymorphicValue<float>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<double> _value = new DoubleConstantValue();

        #endregion

        #region Properties

        public override float Value
        {
            get => _value != null ? (float)_value.Value : default;
            set
            {
                if (_value != null)
                    _value.Value = value;
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

        public override PolymorphicValue<float> Clone()
        {
            return new FloatFromDoubleValue { _value = _value?.Clone() };
        }

        #endregion

        #region Listeners

        private void OnSourceChanged(double _) => RaiseValueChanged(Value);

        #endregion
    }
}
