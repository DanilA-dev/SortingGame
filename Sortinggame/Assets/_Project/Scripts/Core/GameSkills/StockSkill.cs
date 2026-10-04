using UnityEngine;

namespace _Project.Scripts
{
    public class StockSkill : HighlightSkill
    {
        #region Overrides

        protected override bool IsTarget(GameObject target) =>
            target.TryGetComponent(out ItemSlotsContainer shelf)
            && !shelf.IsSorted
            && shelf.ItemInfo == SelectedInfo;

        #endregion
    }
}
