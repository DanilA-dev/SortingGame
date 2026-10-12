using System;
using D_Dev.IAPService;
using D_Dev.ShopSystem.Prices;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ShopSystem.Extensions.IAP
{
    [Serializable]
    public class InAppPrice : BaseShopItemPrice
    {
        #region Fields

        [SerializeField] private IAPProductInfo _product;
        [PreviewField(50, ObjectFieldAlignment.Right)]
        [SerializeField] private Sprite _icon;

        private Action _onChanged;

        #endregion

        #region Properties

        public override ShopPriceType Type => ShopPriceType.InApp;
        public IAPProductInfo Product => _product;

        private static IAPService.IAPService Service => IAPService.IAPService.Instance;

        #endregion

        #region Public

        public override bool CanPay(ShopItemInfo item)
            => _product != null && Service != null && Service.IsInitialized && !Service.IsPurchasing(_product);

        public override void Pay(ShopItemInfo item, Action<bool> onComplete)
        {
            if (_product == null || Service == null)
            {
                onComplete?.Invoke(false);
                return;
            }

            Service.Purchase(_product, result => onComplete?.Invoke(result == IAPPurchaseResult.Success));
        }

        public override string GetLabel(ShopItemInfo item)
        {
            if (_product == null)
                return string.Empty;

            return Service != null ? Service.GetLocalizedPrice(_product) : _product.FallbackPrice?.Value;
        }

        public override Sprite GetIcon(ShopItemInfo item) => _icon;

        public override void Subscribe(Action onChanged)
        {
            if (_onChanged == null)
                IAPService.IAPService.OnInitialized += OnServiceInitialized;

            _onChanged += onChanged;
        }

        public override void Unsubscribe(Action onChanged)
        {
            _onChanged -= onChanged;

            if (_onChanged == null)
                IAPService.IAPService.OnInitialized -= OnServiceInitialized;
        }

        #endregion

        #region Listeners

        private void OnServiceInitialized() => _onChanged?.Invoke();

        #endregion
    }
}
