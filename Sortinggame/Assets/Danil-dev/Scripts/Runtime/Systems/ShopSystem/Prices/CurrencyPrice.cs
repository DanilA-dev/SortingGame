using System;
using D_Dev.CurrencySystem;
using D_Dev.CurrencySystem.Extensions;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.ShopSystem.Prices
{
    [Serializable]
    public class CurrencyPrice : BaseShopItemPrice
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<CurrencyInfo> _currency = new CurrencyInfoConstantValue();
        [Tooltip("Amount per price index. For upgrades the index is the next level, the last element is used when the array is shorter.")]
        [SerializeField] private long[] _amounts = { 0 };

        private Action _onChanged;
        private Currency _subscribedCurrency;

        #endregion

        #region Properties

        public override ShopPriceType Type => ShopPriceType.Currency;
        public CurrencyInfo CurrencyInfo => _currency?.Value;
        private Currency Currency => CurrencyInfo != null ? CurrencyInfo.Currency : null;

        #endregion

        #region Public

        public long GetAmount(ShopItemInfo item)
        {
            if (_amounts == null || _amounts.Length == 0)
                return -1;

            return _amounts[Mathf.Clamp(item.PriceIndex, 0, _amounts.Length - 1)];
        }

        public override bool IsFree(ShopItemInfo item) => GetAmount(item) == 0;

        public override bool CanPay(ShopItemInfo item)
        {
            var amount = GetAmount(item);
            return amount == 0 || (amount > 0 && Currency != null && Currency.Value >= amount);
        }

        public override void Pay(ShopItemInfo item, Action<bool> onComplete)
        {
            var amount = GetAmount(item);
            onComplete?.Invoke(amount == 0 || (amount > 0 && Currency != null && Currency.TryWithdraw(amount)));
        }

        public override string GetLabel(ShopItemInfo item) => GetAmount(item).ToString();

        public override Sprite GetIcon(ShopItemInfo item) => CurrencyInfo != null ? CurrencyInfo.CurrencyIcon : null;

        public override void Subscribe(Action onChanged)
        {
            _onChanged += onChanged;

            if (_subscribedCurrency != null || Currency == null)
                return;

            _subscribedCurrency = Currency;
            _subscribedCurrency.OnCurrencyUpdate += OnCurrencyUpdate;
        }

        public override void Unsubscribe(Action onChanged)
        {
            _onChanged -= onChanged;

            if (_onChanged != null || _subscribedCurrency == null)
                return;

            _subscribedCurrency.OnCurrencyUpdate -= OnCurrencyUpdate;
            _subscribedCurrency = null;
        }

        #endregion

        #region Listeners

        private void OnCurrencyUpdate(Currency.CurrencyEvent currencyEvent, long value) => _onChanged?.Invoke();

        #endregion
    }
}
