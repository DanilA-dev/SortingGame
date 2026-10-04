using D_Dev.EntityInfoBinder;
using UnityEngine;

namespace _Project.Scripts
{
    public class VisionSkill : HighlightSkill
    {
        #region Overrides

        protected override bool IsTarget(GameObject target) =>
            target != SelectedItem
            && target.TryGetComponent(out ItemInteractable item)
            && !item.IsSorted
            && !item.IsPicked
            && target.TryGetComponent(out EntityInfoBinder binder)
            && binder.Info == SelectedInfo;

        protected override void SetHighlighted(GameObject target, bool isHighlighted)
        {
            if (target.TryGetComponent(out ItemInteractable item))
                item.SetRevealed(isHighlighted);

            base.SetHighlighted(target, isHighlighted);
        }

        #endregion
    }
}
