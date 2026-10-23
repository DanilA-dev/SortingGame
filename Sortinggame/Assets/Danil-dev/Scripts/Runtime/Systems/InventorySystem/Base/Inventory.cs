using System;
using System.Collections.Generic;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.InventorySystem
{
    public class Inventory : MonoBehaviour, IInventory
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<int> _slotsCount = new IntConstantValue();

        private readonly Dictionary<int, IInventoryCell> _cells = new();

        public event Action<InventoryItemEntityInfo, int> OnItemAdded;
        public event Action<InventoryItemEntityInfo, int> OnItemRemoved;
        public event Action OnCellsChanged;

        #endregion

        #region Properties

        public Dictionary<int, IInventoryCell> Cells => _cells;
        public bool IsFull => GetFreeCellsCount() == 0;

        #endregion

        #region Monobehaviour

        private void Awake() => ResizeCells(_slotsCount.Value);

        private void OnEnable() => _slotsCount.OnValueChanged += OnSlotsCountChanged;

        private void OnDisable() => _slotsCount.OnValueChanged -= OnSlotsCountChanged;

        #endregion

        #region Public

        public bool AddItem(InventoryItemEntityInfo itemInfo, int amount)
        {
            if (itemInfo == null || amount <= 0)
                return false;

            var remainingAmount = amount;
            remainingAmount -= AddToCells(itemInfo, remainingAmount, onlyOccupied: true);
            if (remainingAmount > 0)
                remainingAmount -= AddToCells(itemInfo, remainingAmount, onlyOccupied: false);

            var addedAmount = amount - remainingAmount;
            if (addedAmount <= 0)
                return false;

            OnItemAdded?.Invoke(itemInfo, addedAmount);
            return true;
        }

        public bool RemoveItem(InventoryItemEntityInfo itemInfo, int amount)
        {
            if (itemInfo == null || amount <= 0 || GetAmount(itemInfo) < amount)
                return false;

            var remainingAmount = amount;
            foreach (var cell in Cells.Values)
            {
                if(cell.IsEmpty || cell.Data.Info != itemInfo)
                    continue;
                
                remainingAmount -= cell.Remove(remainingAmount);
                if (remainingAmount <= 0)
                    break;
            }

            OnItemRemoved?.Invoke(itemInfo, amount);
            return true;
        }

        public bool TransferTo(Inventory target, InventoryItemEntityInfo itemInfo, int amount)
        {
            if (target == null || target == this || GetAmount(itemInfo) < amount || !target.CanAdd(itemInfo, amount))
                return false;

            return RemoveItem(itemInfo, amount) && target.AddItem(itemInfo, amount);
        }

        public bool CanAdd(InventoryItemEntityInfo itemInfo, int amount)
        {
            if (itemInfo == null || amount <= 0)
                return false;

            if (!itemInfo.IsStackLimited)
            {
                foreach (var cell in Cells.Values)
                {
                    if (cell.CanAdd(itemInfo))
                        return true;
                }
                return false;
            }

            var freeSpace = 0;
            foreach (var cell in Cells.Values)
            {
                if (!cell.CanAdd(itemInfo))
                    continue;

                freeSpace += itemInfo.MaxAmount - (cell.IsEmpty ? 0 : cell.Data.CurrentAmount);
                if (freeSpace >= amount)
                    return true;
            }
            return false;
        }

        public int GetFreeCellsCount()
        {
            var count = 0;
            foreach (var cell in Cells.Values)
            {
                if (cell.IsEmpty)
                    count++;
            }
            return count;
        }

        public bool HasItem(InventoryItemEntityInfo itemInfo) => GetAmount(itemInfo) > 0;

        public int GetAmount(InventoryItemEntityInfo itemInfo)
        {
            if(itemInfo == null)
                return 0;

            var amount = 0;
            foreach (var cell in Cells.Values)
            {
                if (!cell.IsEmpty && cell.Data.Info == itemInfo)
                    amount += cell.Data.CurrentAmount;
            }
            return amount;
        }

        #endregion

        #region Private

        private bool ResizeCells(int count)
        {
            var changed = false;

            for (int i = _cells.Count; i < count; i++)
            {
                _cells[i] = new InventoryCell { Index = i };
                changed = true;
            }

            for (int i = _cells.Count - 1; i >= Mathf.Max(count, 0); i--)
            {
                if (!_cells[i].IsEmpty)
                    break;

                _cells.Remove(i);
                changed = true;
            }

            return changed;
        }

        private int AddToCells(InventoryItemEntityInfo itemInfo, int amount, bool onlyOccupied)
        {
            var remainingAmount = amount;
            foreach (var cell in Cells.Values)
            {
                if (cell.IsEmpty == onlyOccupied)
                    continue;

                remainingAmount -= cell.Add(itemInfo, remainingAmount);
                if (remainingAmount <= 0)
                    break;
            }
            return amount - remainingAmount;
        }

        #endregion

        #region Listeners

        private void OnSlotsCountChanged(int count)
        {
            if (ResizeCells(count))
                OnCellsChanged?.Invoke();
        }

        #endregion

        #region Debug

        [FoldoutGroup("Debug")]
        [Button]
        private void DebugCells()
        {
            if(_cells.Count == 0)
                return;

            foreach (var (index, cell) in _cells)
            {
                var data = cell.IsEmpty? "Empty" : cell.Data.Info.name;
                var amount = cell.IsEmpty ? "0" : cell.Data.CurrentAmount.ToString();
                Debug.Log($"Cell : <color=blue>{index}</color> Data : <color=yellow>{data}</color> Amount : <color=red>{amount}</color> \n");
            }
        }

        #endregion
    }
}
