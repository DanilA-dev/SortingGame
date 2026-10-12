using System;
using System.Collections.Generic;

namespace D_Dev.InventorySystem
{
    public interface IInventory
    {
        public Dictionary<int, IInventoryCell> Cells { get; }
        
        public event Action<InventoryItemEntityInfo, int> OnItemAdded;
        public event Action<InventoryItemEntityInfo, int> OnItemRemoved;
        
        public bool AddItem(InventoryItemEntityInfo itemInfo, int amount);
        public bool RemoveItem(InventoryItemEntityInfo itemInfo, int amount);
        public bool HasItem(InventoryItemEntityInfo itemInfo);
        public int GetAmount(InventoryItemEntityInfo itemInfo);

    }
}
