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
            foreach (var cell in Cells.Values)
            {
                if (!cell.CanBeOccupied())
                    continue;
                
                var item = new InventoryItem(itemInfo, remainingAmount);
                if(cell.TryAdd(item))
                    remainingAmount -= cell.Data.CurrentAmount;

                if (remainingAmount <= 0)
                    return true;
            }
            return remainingAmount < amount;
        }

        public bool RemoveItem(InventoryItemEntityInfo itemInfo, int amount)
        {
            if (itemInfo == null || amount <= 0)
                return false;

            var remainingAmount = amount;
            foreach (var cell in Cells.Values)
            {
                if(cell.IsEmpty || cell.Data.Info != itemInfo)
                    continue;
                
                if (cell.TryRemove(remainingAmount))
                    remainingAmount -= cell.Data.CurrentAmount;

                if (remainingAmount <= 0)
                    return true;
            }

            return false;
        }

        public bool HasItem(InventoryItemEntityInfo itemInfo)
        {
            if(itemInfo == null)
                return false;

            foreach (var cell in Cells.Values)
            {
                if(cell.IsEmpty)
                    continue;
                
                if (cell.Data.Info == itemInfo)
                    return true;
            }
            return false;
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
