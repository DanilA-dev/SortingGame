using D_Dev.SaveSystem.SaveableData;
using UnityEngine;

namespace D_Dev.CurrencySystem.Extensions
{
    [System.Serializable]
    public class CurrencyInfoSaveableData : BaseSaveableData<long>
    {
        #region Fields

        [SerializeField] private CurrencyInfo _currency;

        #endregion

        #region Subscription

        public override void Subscribe()
        {
            if (_currency != null && _currency.Currency != null)
                _currency.Currency.OnCurrencyUpdate += OnCurrencyUpdate;
        }

        public override void Unsubscribe()
        {
            if (_currency != null && _currency.Currency != null)
                _currency.Currency.OnCurrencyUpdate -= OnCurrencyUpdate;
        }

        private void OnCurrencyUpdate(Currency.CurrencyEvent currencyEvent, long value) => NotifyChanged();

        #endregion

        #region Overrides

        protected override long GetTypedSaveData() => _currency.Currency.Value;
        protected override void SetTypedSaveData(long data) => _currency.Currency.TrySet(data);
        protected override long GetTypedDefaultValue() => _currency.Currency.DefaultValue;

        #endregion
    }
}