using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.InventorySystem
{
    public class Inventory : MonoBehaviour, IInventory
    {
        #region Fields

        [SerializeField] private int slotsCount;

        private readonly Dictionary<int, IInventoryCell> _cells = new();

        public event Action<InventoryItemEntityInfo, int> OnItemAdded;
        public event Action<InventoryItemEntityInfo, int> OnItemRemoved;

        #endregion

        #region Properties

        public Dictionary<int, IInventoryCell> Cells => _cells;

        #endregion

        #region Monobehaviour

        private void Awake() => InitializeCells();


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

        private void InitializeCells()
        {
            for (int i = 0; i < slotsCount; i++)
            {
                var cell = new InventoryCell { Index = i };
                _cells[i] = cell;
            }
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
