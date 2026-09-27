using System;
using D_Dev.TweenAnimations;
using D_Dev.ValueViewProvider;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.CurrencySystem.Extensions
{
    public class CurrencyViewProvider : GenericValueViewProvider<int, IntTweenAnimation>
    {
        #region Fields

        [SerializeReference] private CurrencyInfo _currencyInfo;
        [FoldoutGroup("Events")] 
        [SerializeField] private UnityEvent OnDepositSucsses;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent OnDepositFails;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent OnWithdrawSucsses;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent OnWithdrawFails;
        
        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_currencyInfo != null && _currencyInfo.Currency != null)
                _currencyInfo.Currency.OnCurrencyUpdate += OnCurrencyUpdate;
        }

        private void Start()
        {
            if (_currencyInfo == null)
                return;

            UpdateView((int)_currencyInfo.Currency.Value);
        }

        private void OnDisable()
        {
            _currencyInfo.Currency.OnCurrencyUpdate -= OnCurrencyUpdate;
        }

        #endregion

        #region Listeners

        private void OnCurrencyUpdate(Currency.CurrencyEvent currencyEvent, long value)
        {
            switch (currencyEvent.ActionType)
            {
                case Currency.CurrencyActionType.Deposit:
                    if(currencyEvent.IsSuccess)
                        OnDepositSucsses?.Invoke();
                    else
                        OnDepositFails?.Invoke();
                    break;
                case Currency.CurrencyActionType.Withdraw:
                    if(currencyEvent.IsSuccess)
                        OnWithdrawSucsses?.Invoke();
                    else
                        OnWithdrawFails?.Invoke();
                    break;
                case Currency.CurrencyActionType.Set:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            UpdateView((int)value);
        }

        #endregion
    }
}
