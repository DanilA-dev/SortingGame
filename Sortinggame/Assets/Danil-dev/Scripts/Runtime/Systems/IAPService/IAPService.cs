using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using D_Dev.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.IAPService
{
    public class IAPService : BaseSingleton<IAPService>
    {
        #region Fields

        [Title("Catalog")]
        [SerializeField] private List<IAPProductInfo> _products = new();
        [Title("Modules")]
        [SerializeReference] private List<IIAPModule> _modules = new();
        [Title("Debug")]
        [SerializeField] private bool _debug;

        private readonly Dictionary<string, IAPProductInfo> _productsById = new();
        private readonly Dictionary<string, Action> _grantHandlers = new();
        private readonly List<string> _pendingGrants = new();
        private readonly HashSet<string> _purchasingIds = new();
        private IIAPModule _activeModule;

        public static event Action OnInitialized;

        #endregion

        #region Properties

        public bool IsInitialized => _activeModule != null;
        public IReadOnlyList<IAPProductInfo> Products => _products;

        #endregion

        #region Monobehaviour

        private void Start() => InitializeAsync().Forget();

        private void OnDestroy()
        {
            foreach (var module in _modules)
                module?.Dispose();

            _activeModule = null;
        }

        #endregion

        #region Public

        public bool TryGetProduct(string productId, out IAPProductInfo product)
            => _productsById.TryGetValue(productId ?? string.Empty, out product);

        public bool IsPurchasing(IAPProductInfo product) => product != null && _purchasingIds.Contains(product.ProductId);

        public string GetLocalizedPrice(IAPProductInfo product)
        {
            if (product == null)
                return string.Empty;

            if (_activeModule != null && _activeModule.TryGetLocalizedPrice(product, out var price) && !string.IsNullOrEmpty(price))
                return price;

            return product.FallbackPrice?.Value ?? string.Empty;
        }

        public void Purchase(IAPProductInfo product, Action<IAPPurchaseResult> callback = null)
        {
            if (product == null || !_productsById.ContainsKey(product.ProductId))
            {
                Log($"unknown product {product?.ProductId}");
                callback?.Invoke(IAPPurchaseResult.UnknownProduct);
                return;
            }

            if (!IsInitialized)
            {
                Log("purchase before initialization");
                callback?.Invoke(IAPPurchaseResult.NotInitialized);
                return;
            }

            if (!_purchasingIds.Add(product.ProductId))
            {
                callback?.Invoke(IAPPurchaseResult.AlreadyPurchasing);
                return;
            }

            var module = _activeModule;
            module.Purchase(product, result =>
            {
                _purchasingIds.Remove(product.ProductId);
                Log($"purchase {product.ProductId} result {result}");

                if (result != IAPPurchaseResult.Success)
                {
                    callback?.Invoke(result);
                    return;
                }

                if (callback == null)
                {
                    EnqueueGrant(product.ProductId);
                    return;
                }

                callback.Invoke(result);
                module.Confirm(product);
            });
        }

        public void RegisterGrantHandler(IAPProductInfo product, Action grant)
        {
            if (product == null || grant == null)
                return;

            _grantHandlers[product.ProductId] = grant;

            for (int i = _pendingGrants.Count - 1; i >= 0; i--)
            {
                if (_pendingGrants[i] != product.ProductId)
                    continue;

                _pendingGrants.RemoveAt(i);
                Grant(product, grant);
            }
        }

        public void UnregisterGrantHandler(IAPProductInfo product, Action grant)
        {
            if (product == null)
                return;

            if (_grantHandlers.TryGetValue(product.ProductId, out var current) && current == grant)
                _grantHandlers.Remove(product.ProductId);
        }

        #endregion

        #region Private

        private async UniTaskVoid InitializeAsync()
        {
            CacheProducts();

            foreach (var module in _modules)
            {
                if (module == null)
                    continue;

                await module.Initialize(_products);

                if (this == null)
                    return;

                if (!module.IsInitialized)
                    continue;

                _activeModule = module;
                break;
            }

            if (_activeModule == null)
            {
                Debug.LogWarning("[IAPService] No IAP module was initialized");
                return;
            }

            Log($"initialized with {_activeModule.GetType().Name}");
            OnInitialized?.Invoke();

            var unprocessed = _activeModule.GetUnprocessedPurchases();
            if (unprocessed == null)
                return;

            foreach (var productId in unprocessed)
                EnqueueGrant(productId);
        }

        private void CacheProducts()
        {
            _productsById.Clear();

            foreach (var product in _products)
            {
                if (product == null || string.IsNullOrEmpty(product.ProductId))
                    continue;

                if (!_productsById.TryAdd(product.ProductId, product))
                    Debug.LogWarning($"[IAPService] Duplicate product id {product.ProductId}");
            }
        }

        private void EnqueueGrant(string productId)
        {
            if (!TryGetProduct(productId, out var product))
            {
                Debug.LogWarning($"[IAPService] Unprocessed purchase of unknown product {productId}");
                return;
            }

            if (_grantHandlers.TryGetValue(productId, out var grant))
                Grant(product, grant);
            else
                _pendingGrants.Add(productId);
        }

        private void Grant(IAPProductInfo product, Action grant)
        {
            Log($"grant {product.ProductId}");
            grant.Invoke();
            _activeModule?.Confirm(product);
        }

        private void Log(string message)
        {
            if (_debug)
                Debug.Log($"[IAPService] {message}");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() => OnInitialized = null;

        #endregion
    }
}
