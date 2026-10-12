using D_Dev.Entity;
using UnityEngine;

namespace D_Dev.InventorySystem.Extensions
{
    public class InventoryItemRemover : BaseInventoryItemHandler
    {
        #region Public

        public void Remove() => Handle();
        public void Remove(GameObject itemObject) => Handle(itemObject);
        public void Remove(EntityInfo info) => Handle(info);

        #endregion

        #region Overrides

        protected override bool Apply(Inventory inventory, InventoryItemEntityInfo itemInfo, int amount)
        {
            return inventory.RemoveItem(itemInfo, amount);
        }

        #endregion
    }
}
