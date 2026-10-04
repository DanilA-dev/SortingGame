using D_Dev.ScriptableVariables;
using UnityEngine;

namespace D_Dev.PolymorphicValueSystem
{
    [System.Serializable]
    public abstract class IntValue : PolymorphicValue<int> { }

    [System.Serializable]
    public sealed class IntConstantValue : ConstantValue<int>
    {
        #region Cloning

        public override PolymorphicValue<int> Clone()
        {
            return new IntConstantValue { _value = _value };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class IntScriptableVariableValue : ScriptableVariableValue<IntScriptableVariable,int>
    {
        #region Cloning

        public override PolymorphicValue<int> Clone()
        {
            return new IntScriptableVariableValue { _variable = _variable };
        }

        #endregion
    }

    [System.Serializable]
    public sealed class IntMultipliedValue : PolymorphicValue<int>
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<int> _value = new IntConstantValue();
        [SerializeReference] private PolymorphicValue<float> _multiplier = new FloatConstantValue();

        #endregion

        #region Properties

        public override int Value
        {
            get
            {
                if (_value == null)
                    return default;

                return _multiplier != null ? Mathf.RoundToInt(_value.Value * _multiplier.Value) : _value.Value;
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
                _value.OnValueChanged += OnValueSourceChanged;

            if (_multiplier != null)
                _multiplier.OnValueChanged += OnMultiplierSourceChanged;
        }

        protected override void UnsubscribeFromSource()
        {
            if (_value != null)
                _value.OnValueChanged -= OnValueSourceChanged;

            if (_multiplier != null)
                _multiplier.OnValueChanged -= OnMultiplierSourceChanged;
        }

        #endregion

        #region Cloning

        public override PolymorphicValue<int> Clone()
        {
            return new IntMultipliedValue
            {
                _value = _value?.Clone(),
                _multiplier = _multiplier?.Clone()
            };
        }

        #endregion

        #region Listeners

        private void OnValueSourceChanged(int _) => RaiseValueChanged(Value);
        private void OnMultiplierSourceChanged(float _) => RaiseValueChanged(Value);

        #endregion
    }
}
