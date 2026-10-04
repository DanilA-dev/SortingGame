using System.Collections.Generic;
using D_Dev.RuntimeLists;
using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts
{
    public abstract class HighlightSkill : SelectedItemSkill
    {
        #region Fields

        [Title("Targets")]
        [SerializeField] private GameObjectRuntimeList _targetsList;

        private readonly List<GameObject> _targets = new();

        #endregion

        #region Overrides

        protected override bool CanUse()
        {
            if (!base.CanUse())
                return false;

            _targets.Clear();
            foreach (var target in _targetsList.Items)
            {
                if (target != null && IsTarget(target))
                    _targets.Add(target);
            }

            return _targets.Count > 0;
        }

        protected override void OnUseStart()
        {
            foreach (var target in _targets)
                SetHighlighted(target, true);
        }

        protected override void OnUseStop()
        {
            foreach (var target in _targets)
            {
                if (target != null)
                    SetHighlighted(target, false);
            }

            _targets.Clear();
        }

        #endregion

        #region Virtual

        protected abstract bool IsTarget(GameObject target);

        protected virtual void SetHighlighted(GameObject target, bool isHighlighted)
        {
            if (target.TryGetComponent(out HighlightEffect highlight))
                highlight.highlighted = isHighlighted;
        }

        #endregion
    }
}
