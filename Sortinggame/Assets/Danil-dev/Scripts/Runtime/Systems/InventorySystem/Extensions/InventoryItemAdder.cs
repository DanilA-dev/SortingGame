using D_Dev.Entity;
using UnityEngine;

namespace D_Dev.InventorySystem.Extensions
{
    public class InventoryItemAdder : BaseInventoryItemHandler
    {
        #region Public

        public void Add() => Handle();
        public void Add(GameObject itemObject) => Handle(itemObject);
        public void Add(EntityInfo info) => Handle(info);

        #endregion

        #region Overrides

        protected override bool Apply(Inventory inventory, InventoryItemEntityInfo itemInfo, int amount)
        {
            return inventory.AddItem(itemInfo, amount);
        }

        #endregion
    }
}
