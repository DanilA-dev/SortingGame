using System;
using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.InventorySystem.UI.Filters
{
    [Serializable]
    public class GroupSlotFilter : InventorySlotFilter
    {
        #region Enum

        public enum GroupMode
        {
            All,
            Any
        }

        #endregion

        #region Fields

        [SerializeField] private GroupMode _mode = GroupMode.All;
        [SerializeReference] private List<InventorySlotFilter> _filters = new();

        #endregion

        #region Overrides

        public override bool IsValid(InventoryItemEntityInfo info)
        {
            var hasAny = false;
            foreach (var filter in _filters)
            {
                if (filter == null)
                    continue;

                var isValid = filter.IsValid(info);
                if (_mode == GroupMode.All && !isValid)
                    return false;

                if (_mode == GroupMode.Any && isValid)
                    return true;

                hasAny = true;
            }

            return _mode == GroupMode.All || !hasAny;
        }

        #endregion
    }
}
