using System.Collections.Generic;
using D_Dev.EntityInfoBinder;
using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeLists;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts
{
    public class MagnetSkill : SelectedItemSkill
    {
        #region Fields

        [Title("Magnet")]
        [SerializeField] private GameObjectRuntimeList _itemsList;
        [SerializeReference] private PolymorphicValue<float> _radius = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<int> _maxItems = new IntConstantValue();
        [Title("Inventory")]
        [SerializeReference] private PolymorphicValue<int> _currentItemsAmount = new IntConstantValue();
        [SerializeReference] private PolymorphicValue<int> _maxCapacity = new IntConstantValue();

        private readonly List<ItemInteractable> _targets = new();

        #endregion

        #region Overrides

        protected override bool CanUse()
        {
            if (!base.CanUse())
                return false;

            var freeSlots = Mathf.Min(_maxItems.Value, _maxCapacity.Value - _currentItemsAmount.Value);
            if (freeSlots <= 0)
                return false;

            CollectTargets();
            if (_targets.Count > freeSlots)
                _targets.RemoveRange(freeSlots, _targets.Count - freeSlots);

            return _targets.Count > 0;
        }

        protected override void OnUseStart()
        {
            foreach (var item in _targets)
            {
                if (item != null)
                    item.StartInteract(gameObject);
            }

            _targets.Clear();
        }

        #endregion

        #region Private

        private void CollectTargets()
        {
            _targets.Clear();
            var position = transform.position;
            var sqrRadius = _radius.Value * _radius.Value;

            foreach (var target in _itemsList.Items)
            {
                if (target == null || target == SelectedItem)
                    continue;

                if ((target.transform.position - position).sqrMagnitude > sqrRadius)
                    continue;

                if (!target.TryGetComponent(out ItemInteractable item) || item.IsSorted || item.IsPicked)
                    continue;

                if (!target.TryGetComponent(out EntityInfoBinder binder) || binder.Info != SelectedInfo)
                    continue;

                _targets.Add(item);
            }

            _targets.Sort((a, b) =>
                (a.transform.position - position).sqrMagnitude.CompareTo((b.transform.position - position).sqrMagnitude));
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (_radius == null)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _radius.Value);
        }

        #endregion
    }
}
