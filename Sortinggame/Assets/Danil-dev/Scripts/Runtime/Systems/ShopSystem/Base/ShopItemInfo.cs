using System;
using System.Collections.Generic;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ShopSystem.Prices;
using D_Dev.ShopSystem.Rewards;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ShopSystem
{
    public abstract class ShopItemInfo : ScriptableObject
    {
        #region Fields

        [Title("Info")]
        [SerializeReference] private PolymorphicValue<string> _displayName = new StringConstantValue();
        [SerializeReference] private PolymorphicValue<string> _description = new StringConstantValue();
        [PreviewField(75, ObjectFieldAlignment.Right)]
        [SerializeField] private Sprite _icon;
        [Title("Prices")]
        [SerializeReference] private List<BaseShopItemPrice> _prices = new();
        [Title("Requirement")]
        [ValidateInput(nameof(IsRequirementValid), "Requirement must be a non-consumable item other than this one")]
        [SerializeField] private ShopItemInfo _requirement;
        [Title("Rewards")]
        [SerializeReference] private List<IShopReward> _rewards = new();
        [Title("Debug")]
        [SerializeField] private bool _showDebugInfo;

        [NonSerialized] private bool _isPurchasing;

        public event Action OnChanged;
        public event Action OnPurchased;
        public event Action<BaseShopItemPrice> OnPaymentFailed;

        #endregion

        #region Properties

        public PolymorphicValue<string> DisplayName => _displayName;
        public PolymorphicValue<string> Description => _description;
        public Sprite Icon => _icon;
        public IReadOnlyList<BaseShopItemPrice> Prices => _prices;
        public ShopItemInfo Requirement => _requirement;
        public bool IsPurchasing => _isPurchasing;

        public abstract bool IsConsumable { get; }
        public abstract bool IsMaxed { get; }
        public virtual bool IsOwned => false;
        public virtual bool IsAvailable => _requirement == null || _requirement.IsOwned;
        public virtual int PriceIndex => 0;
        public virtual string LevelText => string.Empty;

        public bool CanPurchase => IsAvailable && !IsMaxed;

        #endregion

        #region ScriptableObject

        protected virtual void OnEnable()
        {
            foreach (var price in _prices)
                price?.Subscribe(RaiseChanged);

            if (_requirement != null && _requirement != this)
                _requirement.OnChanged += RaiseChanged;
        }

        protected virtual void OnDisable()
        {
            foreach (var price in _prices)
                price?.Unsubscribe(RaiseChanged);

            if (_requirement != null && _requirement != this)
                _requirement.OnChanged -= RaiseChanged;

            _isPurchasing = false;
        }

        #endregion

        #region Public

        public BaseShopItemPrice GetPrice(ShopPriceType type, int index = 0)
        {
            var found = 0;
            foreach (var price in _prices)
            {
                if (price == null || price.Type != type)
                    continue;

                if (found == index)
                    return price;

                found++;
            }

            return null;
        }

        public bool CanPay(BaseShopItemPrice price) => CanPurchase && !_isPurchasing && price != null && price.CanPay(this);

        public void Purchase(BaseShopItemPrice price, Action<bool> onComplete = null)
        {
            if (_isPurchasing || price == null || !_prices.Contains(price) || !CheckCanPurchase())
            {
                onComplete?.Invoke(false);
                return;
            }

            if (!price.CanPay(this))
            {
                FailPayment(price);
                onComplete?.Invoke(false);
                return;
            }

            _isPurchasing = true;
            RaiseChanged();

            price.Pay(this, isSuccess =>
            {
                _isPurchasing = false;

                var isCompleted = isSuccess && CanPurchase;
                if (isCompleted)
                    CompletePurchase();
                else
                    FailPayment(price);

                onComplete?.Invoke(isCompleted);
                RaiseChanged();
            });
        }

        public bool GrantPurchase()
        {
            if (!CanPurchase)
            {
                Log("grant skipped, item cannot be purchased");
                return false;
            }

            CompletePurchase();
            RaiseChanged();
            return true;
        }

        public virtual void Apply() {}

        public virtual string GetCurrentValueText() => string.Empty;
        public virtual string GetNextValueText() => string.Empty;

        #endregion

        #region Protected

        protected abstract void OnPurchaseCompleted();

        protected void RaiseChanged() => OnChanged?.Invoke();

        #endregion

        #region Private

        private void CompletePurchase()
        {
            OnPurchaseCompleted();

            foreach (var reward in _rewards)
                reward?.Grant();

            Log("purchased");
            OnPurchased?.Invoke();
        }

        private void FailPayment(BaseShopItemPrice price)
        {
            Log($"payment failed ({price.Type})");
            OnPaymentFailed?.Invoke(price);
        }

        private bool CheckCanPurchase()
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

        private bool IsRequirementValid(ShopItemInfo requirement)
            => requirement == null || (requirement != this && !requirement.IsConsumable);

        private void Log(string message)
        {
            if (_showDebugInfo)
                Debug.Log($"[Shop] {name} {message}");
        }

        #endregion
    }
}
