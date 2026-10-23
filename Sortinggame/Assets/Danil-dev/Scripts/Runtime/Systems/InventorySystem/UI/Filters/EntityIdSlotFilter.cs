using System;
using System.Collections.Generic;
using UnityEngine;

namespace D_Dev.InventorySystem.UI.Filters
{
    [Serializable]
    public class EntityIdSlotFilter : InventorySlotFilter
    {
        #region Enum

        public enum FilterMode
        {
            Include,
            Exclude
        }

        public enum MatchMode
        {
            Equals,
            StartsWith,
            Contains
        }

        #endregion

        #region Fields

        [SerializeField] private FilterMode _mode = FilterMode.Include;
        [SerializeField] private MatchMode _match = MatchMode.Equals;
        [SerializeField] private bool _ignoreCase = true;
        [SerializeField] private List<string> _ids = new();

        #endregion

        #region Overrides

        public override bool IsValid(InventoryItemEntityInfo info)
        {
            var matches = info != null && IsMatch(info.ID);
            return _mode == FilterMode.Include ? matches : !matches;
        }

        #endregion

        #region Private

        private bool IsMatch(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;

            var comparison = _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (var pattern in _ids)
            {
                if (string.IsNullOrEmpty(pattern))
                    continue;

                var isMatch = _match switch
                {
                    MatchMode.StartsWith => id.StartsWith(pattern, comparison),
                    MatchMode.Contains => id.IndexOf(pattern, comparison) >= 0,
                    _ => string.Equals(id, pattern, comparison)
                };

                if (isMatch)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
