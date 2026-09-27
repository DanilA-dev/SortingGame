using System.Collections.Generic;

namespace D_Dev.InventorySystem
{
    public interface IInventory
    {
        public Dictionary<int, IInventoryCell> Cells { get; }
        public bool AddItem(InventoryItemEntityInfo itemInfo, int amount);
        public bool RemoveItem(InventoryItemEntityInfo itemInfo, int amount);
        
        public bool HasItem(InventoryItemEntityInfo itemInfo);

    }
}
