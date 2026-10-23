using System;
using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.InventorySystem.UI.Filters
{
    [Serializable]
    public class ItemsListSlotFilter : InventorySlotFilter
    {
        #region Enum

        public enum FilterMode
        {
            Include,
            Exclude
        }

        #endregion

        #region Fields

        [SerializeField] private FilterMode _mode = FilterMode.Include;
        [SerializeField] private List<InventoryItemEntityInfo> _items = new();

        #endregion

        #region Overrides

        public override bool IsValid(InventoryItemEntityInfo info)
        {
            var contains = info != null && _items.Contains(info);
            return _mode == FilterMode.Include ? contains : !contains;
        }

        #endregion
    }
}
