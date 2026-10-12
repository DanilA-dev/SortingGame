using System;
using UnityEngine;

namespace D_Dev.InventorySystem
{
    public class InventoryCell : IInventoryCell
    {
        #region Fields

        private InventoryItem _data;

        public event Action<InventoryItem> OnContentChanged;

        #endregion

        #region Properties
        public int Index { get; set; }
        public InventoryItem Data => _data;
        public bool IsEmpty => Data.IsEmpty;
        public bool IsFull => !IsEmpty && Data.Info.IsStackLimited && Data.CurrentAmount >= Data.Info.MaxAmount;

        #endregion

        #region Public

        public bool CanAdd(InventoryItemEntityInfo itemInfo)
        {
            if (itemInfo == null)
                return false;

            return IsEmpty || (Data.Info == itemInfo && !IsFull);
        }

        public int Add(InventoryItemEntityInfo itemInfo, int amount)
        {
            if (amount <= 0 || !CanAdd(itemInfo))
                return 0;

            var currentAmount = IsEmpty ? 0 : Data.CurrentAmount;
            var addedAmount = itemInfo.IsStackLimited ? Mathf.Min(amount, itemInfo.MaxAmount - currentAmount) : amount;
            if (addedAmount <= 0)
                return 0;

            _data = new InventoryItem(itemInfo, currentAmount + addedAmount);
            OnContentChanged?.Invoke(Data);
            return addedAmount;
        }

        public int Remove(int amount)
        {
            if (IsEmpty || amount <= 0)
                return 0;

            var removedAmount = Mathf.Min(amount, Data.CurrentAmount);
            _data = Data.CurrentAmount == removedAmount 
                ? new InventoryItem(null, 0) 
                : new InventoryItem(Data.Info, Data.CurrentAmount - removedAmount);

            OnContentChanged?.Invoke(Data);
            return removedAmount;
        }

        public void Clear()
        {
            if (IsEmpty)
                return;

            _data = new InventoryItem(null, 0);
            OnContentChanged?.Invoke(Data);
        }

        #endregion
    }
}
