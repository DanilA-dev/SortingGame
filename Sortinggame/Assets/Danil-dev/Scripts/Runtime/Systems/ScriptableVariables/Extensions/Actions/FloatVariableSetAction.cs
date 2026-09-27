using D_Dev.Base;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ScriptableVariables.Extensions.Actions
{
    [System.Serializable]
    public class FloatVariableSetAction : BaseAction
    {
        #region Enums

        private enum SetType
        {
            Add,
            Subtract,
            Multiply,
            Divide,
            Set
        }

        #endregion

        #region Fields

        [SerializeField] private FloatScriptableVariable _floatVariable;
        [SerializeField] private SetType _setType;
        [SerializeField] private float _valueToSet;
        [SerializeField] private bool _clampToMaxValue;
        [ShowIf("_clampToMaxValue")]
        [SerializeField] private float _maxValue;

        #endregion

        #region Overrides

        public override void Execute()
        {
            float newValue = _setType switch
            {
                SetType.Add => AddValue(),
                SetType.Subtract => SubtractValue(),
                SetType.Multiply => MultiplyValue(),
                SetType.Divide => DivideValue(),
                SetType.Set => SetValue(),
                _ => _floatVariable.Value
            };
            newValue = Mathf.Clamp(newValue, 0, _clampToMaxValue ? _maxValue : float.MaxValue);
            _floatVariable.Value = newValue;
            IsFinished = true;
        }

        #endregion

        #region Private

        private float AddValue() => _floatVariable.Value + _valueToSet;

        private float SetValue() => _valueToSet;

        private float SubtractValue() => _floatVariable.Value - _valueToSet;

        private float MultiplyValue() => _floatVariable.Value * _valueToSet;

        private float DivideValue() => _valueToSet != 0 ? _floatVariable.Value / _valueToSet : 0;

        #endregion
    }
}
