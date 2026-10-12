using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace D_Dev.IAPService
{
    public interface IIAPModule
    {
        public bool IsInitialized { get; }
        public UniTask Initialize(IReadOnlyList<IAPProductInfo> products);
        public void Dispose();
        public bool TryGetLocalizedPrice(IAPProductInfo product, out string price);
        public void Purchase(IAPProductInfo product, Action<IAPPurchaseResult> callback);
        public void Confirm(IAPProductInfo product);
        public IReadOnlyList<string> GetUnprocessedPurchases();
    }
}
