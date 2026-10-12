#if D_DEV_YG2 && Payments_yg
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using D_Dev.IAPService;
using Sirenix.OdinInspector;
using UnityEngine;
using YG;

namespace D_Dev.PluginYG2
{
    [Serializable]
    public class YG2IAPModule : IIAPModule
    {
        #region Fields

        [SerializeField, Min(0f), SuffixLabel("sec")] private float _catalogTimeout = 10f;
        [Tooltip("Show the price with the currency code, e.g. \"5 YAN\" instead of \"5\".")]
        [SerializeField] private bool _showCurrencyCode = true;

        private readonly Dictionary<string, Action<IAPPurchaseResult>> _callbacks = new();
        private readonly List<string> _unprocessedPurchases = new();
        private bool _isCatalogReceived;
        private bool _isSubscribed;

        #endregion

        #region Properties

        public bool IsInitialized { get; private set; }

        #endregion

        #region Public

        public async UniTask Initialize(IReadOnlyList<IAPProductInfo> products)
        {
            await UniTask.WaitUntil(() => YG2.isSDKEnabled);

            YG2.onGetPayments += OnGetPayments;
            _isCatalogReceived |= YG2.purchases != null && YG2.purchases.Length > 0;

            var timeoutCts = new CancellationTokenSource();
            var timeout = timeoutCts.CancelAfterSlim(TimeSpan.FromSeconds(_catalogTimeout), DelayType.Realtime);

            var isTimeout = await UniTask.WaitUntil(() => _isCatalogReceived, cancellationToken: timeoutCts.Token)
                .SuppressCancellationThrow();

            timeout.Dispose();
            timeoutCts.Dispose();

            YG2.onGetPayments -= OnGetPayments;

            if (isTimeout)
            {
                Debug.LogWarning("[YG2IAPModule] Payments catalog was not received");
                return;
            }

            _unprocessedPurchases.Clear();
            foreach (var purchase in YG2.purchases)
            {
                if (purchase != null && !purchase.consumed)
                    _unprocessedPurchases.Add(purchase.id);
            }

            Subscribe();
            IsInitialized = true;
        }

        public void Dispose()
        {
            Unsubscribe();
            YG2.onGetPayments -= OnGetPayments;
            _callbacks.Clear();
            IsInitialized = false;
        }

        public bool TryGetLocalizedPrice(IAPProductInfo product, out string price)
        {
            price = null;

            var purchase = product != null ? YG2.PurchaseByID(product.ProductId) : null;
            if (purchase == null)
                return false;

            price = _showCurrencyCode ? purchase.price : purchase.priceValue;
            return !string.IsNullOrEmpty(price);
        }

        public void Purchase(IAPProductInfo product, Action<IAPPurchaseResult> callback)
        {
            if (YG2.PurchaseByID(product.ProductId) == null)
            {
                callback?.Invoke(IAPPurchaseResult.UnknownProduct);
                return;
            }

            if (_callbacks.ContainsKey(product.ProductId))
            {
                callback?.Invoke(IAPPurchaseResult.AlreadyPurchasing);
                return;
            }

            _callbacks[product.ProductId] = callback;
            YG2.BuyPayments(product.ProductId);
        }

        public void Confirm(IAPProductInfo product)
        {
            if (product.Type != IAPProductType.Consumable || !_unprocessedPurchases.Remove(product.ProductId))
                return;

            YG2.ConsumePurchaseByID(product.ProductId, false);
        }

        public IReadOnlyList<string> GetUnprocessedPurchases() => new List<string>(_unprocessedPurchases);

        #endregion

        #region Private

        private void Subscribe()
        {
            if (_isSubscribed)
                return;

            _isSubscribed = true;
            YG2.onPurchaseSuccess += OnPurchaseSuccess;
            YG2.onPurchaseFailed += OnPurchaseFailed;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
                return;

            _isSubscribed = false;
            YG2.onPurchaseSuccess -= OnPurchaseSuccess;
            YG2.onPurchaseFailed -= OnPurchaseFailed;
        }

        private void Complete(string productId, IAPPurchaseResult result)
        {
            if (!_callbacks.Remove(productId, out var callback))
            {
                Debug.LogWarning($"[YG2IAPModule] Purchase result {result} for {productId} without a request");
                return;
            }

            CompleteNextFrame(callback, result).Forget();
        }

        private static async UniTaskVoid CompleteNextFrame(Action<IAPPurchaseResult> callback, IAPPurchaseResult result)
        {
            await UniTask.Yield();
            callback?.Invoke(result);
        }

        #endregion

        #region Listeners

        private void OnGetPayments() => _isCatalogReceived = true;

        private void OnPurchaseSuccess(string productId) => Complete(productId, IAPPurchaseResult.Success);

        private void OnPurchaseFailed(string productId) => Complete(productId, IAPPurchaseResult.Failed);

        #endregion
    }
}
#endif
