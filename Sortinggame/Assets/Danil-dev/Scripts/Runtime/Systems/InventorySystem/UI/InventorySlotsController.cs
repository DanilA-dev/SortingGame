using System;
using System.Collections.Generic;
using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.InventorySystem.UI
{
    public class InventorySlotsController : MonoBehaviour
    {
        #region Fields

        [Title("Slots")]
        [SerializeField, Required] private InventorySlotView _slotPrefab;
        [SerializeField, Required] private RectTransform _content;
        [Title("Selection")]
        [SerializeField] private bool _selectable = true;
        [SerializeField, ShowIf(nameof(_selectable))] private bool _deselectOnRepeatClick = true;
        [SerializeReference, ShowIf(nameof(_selectable))] private PolymorphicValue<EntityInfo> _lastSelectedItem = new EntityInfoScriptableVariableValue();

        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent<InventorySlotView> OnSlotSelectedEvent;
        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent<InventorySlotView> OnSlotDeselectedEvent;

        private readonly List<InventorySlotView> _slots = new();
        private Inventory _inventory;
        private InventorySlotView _selectedSlot;

        public event Action<InventorySlotView> OnSlotSelected;
        public event Action<InventorySlotView> OnSlotDeselected;

        #endregion

        #region Properties

        public Inventory Inventory => _inventory;
        public IReadOnlyList<InventorySlotView> Slots => _slots;
        public InventorySlotView SelectedSlot => _selectedSlot;

        #endregion

        #region Monobehaviour

        private void OnDestroy()
        {
            foreach (var slot in _slots)
            {
                if (slot != null)
                    slot.OnClicked -= OnSlotClicked;
            }
        }

        #endregion

        #region Public

        public void Bind(Inventory inventory)
        {
            Unbind();

            _inventory = inventory;
            if (_inventory == null)
                return;

            var cellsCount = _inventory.Cells.Count;
            CreateMissingSlots(cellsCount);

            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                var isUsed = i < cellsCount;
                slot.gameObject.SetActive(isUsed);

                if (isUsed)
                    slot.Bind(_inventory.Cells[i]);
            }
        }

        public void Unbind()
        {
            ClearSelection();

            foreach (var slot in _slots)
                slot.Unbind();

            _inventory = null;
        }

        public void Select(InventorySlotView slot)
        {
            if (!_selectable || slot == null || slot == _selectedSlot)
                return;

            ClearSelection();

            _selectedSlot = slot;
            slot.Select();
            SetLastSelectedItem(slot.IsEmpty ? null : slot.Item.Info);
            OnSlotSelected?.Invoke(slot);
            OnSlotSelectedEvent?.Invoke(slot);
        }

        public void Deselect()
        {
            var slot = ClearSelection();
            if (slot == null)
                return;

            SetLastSelectedItem(null);
            OnSlotDeselected?.Invoke(slot);
            OnSlotDeselectedEvent?.Invoke(slot);
        }

        #endregion

        #region Private

        private InventorySlotView ClearSelection()
        {
            var slot = _selectedSlot;
            if (slot == null)
                return null;

            slot.Deselect();
            _selectedSlot = null;
            return slot;
        }

        private void SetLastSelectedItem(EntityInfo info)
        {
            if (_lastSelectedItem != null)
                _lastSelectedItem.Value = info;
        }

        private void CreateMissingSlots(int count)
        {
            for (int i = _slots.Count; i < count; i++)
            {
                var slot = Instantiate(_slotPrefab, _content);
                slot.OnClicked += OnSlotClicked;
                _slots.Add(slot);
            }
        }

        #endregion

        #region Listeners

        private void OnSlotClicked(InventorySlotView slot)
        {
            if (_deselectOnRepeatClick && slot == _selectedSlot)
                Deselect();
            else
                Select(slot);
        }

        #endregion
    }
}
