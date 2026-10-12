using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.IAPService
{
    [Serializable]
    public class DebugIAPModule : IIAPModule
    {
        #region Fields

        [Title("Settings")]
        [SerializeField] private bool _editorOnly = true;
        [SerializeField, SuffixLabel("sec")] private float _initializeDelay;
        [SerializeField, SuffixLabel("sec")] private float _purchaseDelay = 1f;
        [Title("Results")]
        [SerializeField] private IAPPurchaseResult _purchaseResult = IAPPurchaseResult.Success;
        [Tooltip("Product ids reported as unprocessed purchases after initialization, used to test restoring.")]
        [SerializeField] private List<string> _unprocessedPurchases = new();

        private CancellationTokenSource _cts;

        #endregion

        #region Properties

        public bool IsInitialized { get; private set; }

        #endregion

        #region Public

        public async UniTask Initialize(IReadOnlyList<IAPProductInfo> products)
        {
            if (_editorOnly && !Application.isEditor)
                return;

            _cts = new CancellationTokenSource();

            if (await Wait(_initializeDelay))
                return;

            IsInitialized = true;
        }

        public void Dispose()
        {
            IsInitialized = false;
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public bool TryGetLocalizedPrice(IAPProductInfo product, out string price)
        {
            price = null;
            return false;
        }

        public void Purchase(IAPProductInfo product, Action<IAPPurchaseResult> callback) => PurchaseAsync(callback).Forget();

        public void Confirm(IAPProductInfo product) => Debug.Log($"[DebugIAPModule] Confirm {product.ProductId}");

        public IReadOnlyList<string> GetUnprocessedPurchases() => _unprocessedPurchases;

        #endregion

        #region Private

        private async UniTaskVoid PurchaseAsync(Action<IAPPurchaseResult> callback)
        {
            if (await Wait(_purchaseDelay))
                return;

            callback?.Invoke(_purchaseResult);
        }

        private async UniTask<bool> Wait(float seconds)
        {
            if (_cts == null)
                return true;

            if (seconds <= 0f)
                return false;

            return await UniTask.Delay(TimeSpan.FromSeconds(seconds), DelayType.Realtime, cancellationToken: _cts.Token)
                .SuppressCancellationThrow();
        }

        #endregion
    }
}
