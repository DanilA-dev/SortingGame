using System;
using D_Dev.Base;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ScriptableVariables.Extensions.Actions
{
    [System.Serializable]
    public class IntVariableSetAction : BaseAction
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

        [SerializeField] private IntScriptableVariable _intVariable;
        [SerializeField] private SetType _setType;
        [SerializeField] private int _valueToSet;
        [SerializeField] private bool _clampToMaxValue;
        [ShowIf("_clampToMaxValue")]
        [SerializeField] private int _maxValue;

        #endregion
        
        #region Overrides

        public override void Execute()
        {
            int newValue = _setType switch
            {
                SetType.Add => AddValue(),
                SetType.Subtract => SubtractValue(),
                SetType.Set => SetValue(),
                SetType.Multiply => MultiplyValue(),
                SetType.Divide => DivideValue(),
                _ => _intVariable.Value
            };
            newValue = Mathf.Clamp(newValue, 0, _clampToMaxValue ? _maxValue : Int32.MaxValue);
            _intVariable.Value = newValue;
            IsFinished = true;
        }

        #endregion

        #region Private

        private int AddValue() => _intVariable.Value + _valueToSet;
        private int SetValue() => _valueToSet;
        private int SubtractValue() => _intVariable.Value - _valueToSet;
        private int MultiplyValue() => _intVariable.Value * _valueToSet;
        private int DivideValue() => _valueToSet != 0 ? _intVariable.Value / _valueToSet : 0;

        #endregion
    }
}
