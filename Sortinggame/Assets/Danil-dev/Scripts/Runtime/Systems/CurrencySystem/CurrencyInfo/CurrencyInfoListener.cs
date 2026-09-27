using System;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.CurrencySystem
{
    public class CurrencyInfoListener : MonoBehaviour
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<CurrencyInfo> _currencyInfo;
        
        [FoldoutGroup("Deposit Events")]
        public UnityEvent<int> OnDepositSuccess;
        [FoldoutGroup("Deposit Events")]
        public UnityEvent<int> OnDepositFailed;
        
        [FoldoutGroup("Withdraw Events")]
        public UnityEvent<int> OnWithdrawSuccess;
        [FoldoutGroup("Withdraw Events")]
        public UnityEvent<int> OnWithdrawFailed;
        
        [FoldoutGroup("Set Events")]
        public UnityEvent<int> OnSetSuccess;
        [FoldoutGroup("Set Events")]
        public UnityEvent<int> OnSetFailed;
        
        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_currencyInfo != null && _currencyInfo.Value.Currency != null)
                _currencyInfo.Value.Currency.OnCurrencyUpdate += OnCurrencyUpdate;
        }

        private void OnDisable()
        {
            if (_currencyInfo != null && _currencyInfo.Value.Currency != null)
                _currencyInfo.Value.Currency.OnCurrencyUpdate -= OnCurrencyUpdate;
        }

        #endregion

        #region Listeners

        private void OnCurrencyUpdate(Currency.CurrencyEvent eventData, long value)
        {
            switch (eventData.ActionType)
            {
                case Currency.CurrencyActionType.Deposit:
                    if (eventData.IsSuccess)
                        OnDepositSuccess?.Invoke((int)value);
                    else
                        OnDepositFailed?.Invoke((int)value);
                    break;
                case Currency.CurrencyActionType.Withdraw:
                    if (eventData.IsSuccess)
                        OnWithdrawSuccess?.Invoke((int)value);
                    else
                        OnWithdrawFailed?.Invoke((int)value);
                    break;
                case Currency.CurrencyActionType.Set:
                    if (eventData.IsSuccess)
                        OnSetSuccess?.Invoke((int)value);
                    else
                        OnSetFailed?.Invoke((int)value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}
