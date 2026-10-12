using System;
using UnityEngine;

namespace D_Dev.ShopSystem.Prices
{
    [Serializable]
    public abstract class BaseShopItemPrice
    {
        #region Properties

        public abstract ShopPriceType Type { get; }
        public virtual bool IsDynamic => false;

        #endregion

        #region Public

        public abstract bool CanPay(ShopItemInfo item);
        public abstract void Pay(ShopItemInfo item, Action<bool> onComplete);

        public virtual bool IsFree(ShopItemInfo item) => false;
        public virtual string GetLabel(ShopItemInfo item) => string.Empty;
        public virtual Sprite GetIcon(ShopItemInfo item) => null;

        public virtual void Subscribe(Action onChanged) {}
        public virtual void Unsubscribe(Action onChanged) {}

        #endregion
    }
}
