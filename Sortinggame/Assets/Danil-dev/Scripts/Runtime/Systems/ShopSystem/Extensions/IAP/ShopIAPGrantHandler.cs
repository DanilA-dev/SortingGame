using System;
using System.Collections.Generic;
using D_Dev.IAPService;
using UnityEngine;

namespace D_Dev.ShopSystem.Extensions.IAP
{
    public class ShopIAPGrantHandler : MonoBehaviour
    {
        #region Fields

        [Tooltip("Containers whose in-app items should be granted when the store reports unprocessed purchases.")]
        [SerializeField] private ShopItemsContainer[] _containers;

        private readonly List<(IAPProductInfo Product, Action Grant)> _handlers = new();

        #endregion

        #region Monobehaviour

        private void Start() => RegisterAll();

        private void OnDestroy() => UnregisterAll();

        #endregion

        #region Private

        private void RegisterAll()
        {
            var service = IAPService.IAPService.Instance;
            if (service == null)
            {
                Debug.LogWarning("[ShopIAPGrantHandler] IAPService is not found");
                return;
            }

            foreach (var container in _containers)
            {
                if (container == null)
                    continue;

                foreach (var item in container.Items)
                {
                    if (item == null)
                        continue;

                    foreach (var price in item.Prices)
                    {
                        if (price is not InAppPrice inAppPrice || inAppPrice.Product == null)
                            continue;

                        var shopItem = item;
                        Action grant = () => shopItem.GrantPurchase();
                        service.RegisterGrantHandler(inAppPrice.Product, grant);
                        _handlers.Add((inAppPrice.Product, grant));
                    }
                }
            }
        }

        private void UnregisterAll()
        {
            var service = IAPService.IAPService.Instance;
            if (service != null)
            {
                foreach (var (product, grant) in _handlers)
                    service.UnregisterGrantHandler(product, grant);
            }

            _handlers.Clear();
        }

        #endregion
    }
}
