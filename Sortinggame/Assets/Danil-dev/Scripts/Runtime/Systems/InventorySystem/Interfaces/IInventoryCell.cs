using System;

namespace D_Dev.InventorySystem
{
    public interface IInventoryCell
    {
        public int Index { get; set; }
        public bool IsEmpty { get; }
        public bool IsFull { get; }
        
        public event Action<InventoryItem> OnContentChanged;
        public InventoryItem Data { get; }
        public bool CanAdd(InventoryItemEntityInfo itemInfo);
        public int Add(InventoryItemEntityInfo itemInfo, int amount);
        public int Remove(int amount);
        public void Clear();

    }
}
