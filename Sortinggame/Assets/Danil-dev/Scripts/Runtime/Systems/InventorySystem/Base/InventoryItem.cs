using System;
using UnityEngine;

namespace D_Dev.InventorySystem
{
    [Serializable]
    public struct InventoryItem
    {
        #region Properties

        public InventoryItemEntityInfo Info { get; private set; }
        public int CurrentAmount { get; private set; }
        public bool IsEmpty => Info == null || CurrentAmount <= 0;

        #endregion

        #region Constructor

        public InventoryItem(InventoryItemEntityInfo info, int amount)
        {
            Info = info;
            CurrentAmount = info!= null ? Mathf.Clamp(amount, 0, info.MaxAmount) : 0;
        }

        #endregion
    }
}
