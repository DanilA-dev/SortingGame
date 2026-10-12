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
            CurrentAmount = info == null ? 0 
                : info.IsStackLimited ? Mathf.Clamp(amount, 0, info.MaxAmount) 
                : Mathf.Max(amount, 0);
        }

        #endregion
    }
}
