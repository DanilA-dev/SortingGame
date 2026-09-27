using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.CurrencySystem
{
    [System.Serializable]
    public class Currency
    {
        #region Enums

        public enum CurrencyActionType
        {
            Deposit = 0,
            Withdraw = 1,
            Set = 2,
        }
        
        #endregion
        
        #region Classes

        public struct CurrencyEvent
        {
            public CurrencyActionType ActionType;
            public bool IsSuccess;
        }

        #endregion
        
        #region Fields

        [SerializeField] private string _name;
        [Space]
        [SerializeField, ReadOnly] private long _value;
        [SerializeField] private long _defaultValue;
        [SerializeField] private bool _hasMaxValue;
        [ShowIf(nameof(_hasMaxValue))]
        [SerializeField] private long _maxValue;

        public event Action<CurrencyEvent,long> OnCurrencyUpdate;
            
        #endregion

        #region Properties
        public long Value => _value;

        public bool HasMaxValue
        {
            get => _hasMaxValue;
            set => _hasMaxValue = value;
        }


        public long MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public long DefaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        #endregion

        #region Constructors

        public Currency(string name, long initialValue)
        {
            _name = name;
            _value = initialValue;
        }

        #endregion
        
        #region Public

        public bool TryDeposit(long depositValue)
        {
            if (depositValue <= 0 || _hasMaxValue && _value >= _maxValue)
            {
                OnCurrencyUpdate?.Invoke(new CurrencyEvent
                {
                    ActionType = CurrencyActionType.Deposit,
                    IsSuccess = false,
                }, _value);
                Debug.Log($"[Currency : {_name}] Deposit - {depositValue}, <color=red> Failed </color>");
                return false;
            }

            var newValue = _value + depositValue;
            if (_hasMaxValue && newValue >= _maxValue)
                _value = _maxValue;
            else
                _value += depositValue;

            OnCurrencyUpdate?.Invoke(new CurrencyEvent{ActionType = CurrencyActionType.Deposit, IsSuccess = true }, _value);
            Debug.Log($"[Currency : {_name}] Deposit - {depositValue}, <color=green> Success </color>");
            return true;
        }

        public bool TryWithdraw(long withdrawValue)
        {
            if (withdrawValue <= 0 || _value < withdrawValue)
            {
                OnCurrencyUpdate?.Invoke(new CurrencyEvent
                {
                    ActionType = CurrencyActionType.Withdraw, IsSuccess = false
                }, _value);
                Debug.Log($"[Currency : {_name}] Withdraw - {withdrawValue}, <color=red> Failed </color>");
                return false;
            }

            _value -= withdrawValue;
            OnCurrencyUpdate?.Invoke(new CurrencyEvent{ActionType = CurrencyActionType.Withdraw, IsSuccess = true }, _value);
            Debug.Log($"[Currency : {_name}] Withdraw - {withdrawValue}, <color=green> Success </color>");
            return true;
        }

        public bool TrySet(long value)
        {
            if (value < 0 || _hasMaxValue && value >= _maxValue)
            {
                OnCurrencyUpdate?.Invoke(new CurrencyEvent
                {
                    ActionType = CurrencyActionType.Set, IsSuccess = false
                }, _value);
                Debug.Log($"[Currency : {_name}] Set value - {value}, <color=red> Failed </color>");
                return false;
            }

            _value = value;
            OnCurrencyUpdate?.Invoke(new CurrencyEvent{ActionType = CurrencyActionType.Set, IsSuccess = true }, _value);
            Debug.Log($"[Currency : {_name}] Set value - {value}, <color=green> Success </color>");
            return true;
        }
        
        #endregion
    }
}
