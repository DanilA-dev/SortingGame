using System;
using D_Dev.CurrencySystem;
using D_Dev.CurrencySystem.Extensions;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Core.Purchasable
{
    public abstract class BasePurchasableInfo : ScriptableObject
    {
        #region Fields

        [Title("Info")]
        [SerializeReference] private PolymorphicValue<string> _displayName = new StringConstantValue();
        [SerializeReference] private PolymorphicValue<string> _description = new StringConstantValue();
        [PreviewField(75, ObjectFieldAlignment.Right)]
        [SerializeField] private Sprite _icon;
        [Title("Price")]
        [SerializeReference] private PolymorphicValue<CurrencyInfo> _currency = new CurrencyInfoConstantValue();
        [Title("Debug")]
        [SerializeField] private bool _showDebugInfo = true;

        public event Action OnChanged;
        public event Action OnNotEnoughCurrency;

        #endregion

        #region Properties

        public PolymorphicValue<string> DisplayName => _displayName;
        public PolymorphicValue<string> Description => _description;
        public Sprite Icon => _icon;
        public CurrencyInfo CurrencyInfo => _currency?.Value;

        public abstract bool IsAvailable { get; }
        public abstract bool IsMaxed { get; }
        public abstract int NextPrice { get; }
        public virtual string LevelText => string.Empty;

        private Currency Currency => CurrencyInfo != null ? CurrencyInfo.Currency : null;

        #endregion

        #region ScriptableObject

        protected virtual void OnEnable()
        {
            if (Currency != null)
                Currency.OnCurrencyUpdate += OnCurrencyUpdated;
        }

        protected virtual void OnDisable()
        {
            if (Currency != null)
                Currency.OnCurrencyUpdate -= OnCurrencyUpdated;
        }

        #endregion

        #region Public

        public bool CanAfford() => IsAvailable && !IsMaxed && Currency != null && Currency.Value >= NextPrice;

        public bool TryPurchase()
        {
            if (!CanPurchase())
                return false;

            if (Currency == null || !Currency.TryWithdraw(NextPrice))
            {
                Log("not enough currency");
                OnNotEnoughCurrency?.Invoke();
                return false;
            }

            OnPurchased();
            return true;
        }

        public bool TryPurchaseFree()
        {
            if (!CanPurchase())
                return false;

            OnPurchased();
            return true;
        }

        public virtual void Apply() {}

        public abstract string GetCurrentText();
        public abstract string GetNextText();

        #endregion

        #region Protected

        protected abstract void OnPurchased();

        protected void RaiseChanged() => OnChanged?.Invoke();

        #endregion

        #region Private

        private bool CanPurchase()
        {
            if (!IsAvailable)
            {
                Log("is not available");
                return false;
            }

            if (IsMaxed)
            {
                Log("is maxed");
                return false;
            }

            return true;
        }

        private void OnCurrencyUpdated(Currency.CurrencyEvent currencyEvent, long value) => RaiseChanged();

        private void Log(string message)
        {
            if (_showDebugInfo)
                Debug.Log($"[Purchase] {name} {message}");
        }

        #endregion
    }
}
