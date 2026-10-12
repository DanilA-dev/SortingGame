using UnityEngine;

namespace D_Dev.ShopSystem
{
    [CreateAssetMenu(menuName = "D-Dev/Shop/Consumable Item")]
    public class ConsumableShopItemInfo : ShopItemInfo
    {
        #region Properties

        public override bool IsConsumable => true;
        public override bool IsMaxed => false;

        #endregion

        #region Protected

        protected override void OnPurchaseCompleted() {}

        #endregion
    }
}
