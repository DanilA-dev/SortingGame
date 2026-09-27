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
        public bool IsFull => !IsEmpty && Data.CurrentAmount >= Data.Info.MaxAmount;

        #endregion

        #region Public

        public bool CanBeOccupied()
        {
            return IsEmpty || !IsFull;
        }

        public bool TryAdd(InventoryItem item)
        {
            if (!CanBeOccupied())
                return false;

            var canAddAmount = item.CurrentAmount;
            if (canAddAmount == 0)
                return false;
            
            _data = IsEmpty 
                ? new InventoryItem(item.Info, canAddAmount) 
                : new InventoryItem(Data.Info, Data.CurrentAmount + canAddAmount);

            OnContentChanged?.Invoke(Data);
            return true;
        }

        public bool TryRemove(int amount)
        {
            if (IsEmpty || amount <= 0)
                return false;

            var canTakeAmount = Mathf.Min(amount, Data.CurrentAmount);
            _data = Data.CurrentAmount == canTakeAmount 
                ? new InventoryItem(null, 0) 
                : new InventoryItem(Data.Info, Data.CurrentAmount - canTakeAmount);

            OnContentChanged?.Invoke(Data);
            return true;
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
