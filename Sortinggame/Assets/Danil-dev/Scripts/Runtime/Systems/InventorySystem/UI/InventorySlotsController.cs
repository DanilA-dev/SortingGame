using System;
using System.Collections.Generic;
using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.InventorySystem.UI.Filters;
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
        [SerializeField] private bool _hideEmptySlots;
        [Title("Filter")]
        [SerializeReference] private InventorySlotFilter _filter;
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
        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent<InventorySlotView> OnSlotClickedEvent;

        private readonly List<InventorySlotView> _slots = new();
        private Inventory _inventory;
        private InventorySlotView _selectedSlot;

        public event Action<InventorySlotView> OnSlotSelected;
        public event Action<InventorySlotView> OnSlotDeselected;
        public event Action<InventorySlotView> OnSlotClicked;

        #endregion

        #region Properties

        public Inventory Inventory => _inventory;
        public IReadOnlyList<InventorySlotView> Slots => _slots;
        public InventorySlotView SelectedSlot => _selectedSlot;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_lastSelectedItem != null)
                _lastSelectedItem.OnValueChanged += OnLastSelectedItemChanged;
        }

        private void OnDisable()
        {
            if (_lastSelectedItem != null)
                _lastSelectedItem.OnValueChanged -= OnLastSelectedItemChanged;
        }

        private void OnDestroy()
        {
            foreach (var slot in _slots)
            {
                if (slot != null)
                    slot.OnClicked -= OnSlotViewClicked;
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

            _inventory.OnCellsChanged += OnInventoryCellsChanged;
            _inventory.OnItemAdded += OnInventoryItemsChanged;
            _inventory.OnItemRemoved += OnInventoryItemsChanged;
            RefreshSlots();
        }

        public void Unbind()
        {
            ClearSelection();

            foreach (var slot in _slots)
                slot.Unbind();

            if (_inventory != null)
            {
                _inventory.OnCellsChanged -= OnInventoryCellsChanged;
                _inventory.OnItemAdded -= OnInventoryItemsChanged;
                _inventory.OnItemRemoved -= OnInventoryItemsChanged;
            }

            _inventory = null;
        }

        public void Select(InventorySlotView slot)
        {
            if (!_selectable || slot == null || slot == _selectedSlot)
                return;

            ApplySelection(slot);
            SetLastSelectedItem(slot.IsEmpty ? null : slot.Item.Info);
        }

        public void Deselect()
        {
            if (!ApplyDeselection())
                return;

            SetLastSelectedItem(null);
        }

        #endregion

        #region Private

        private void RefreshSlots()
        {
            var cellsCount = _inventory.Cells.Count;
            CreateMissingSlots(cellsCount);

            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                var isUsed = i < cellsCount;

                if (isUsed && slot.Cell != _inventory.Cells[i])
                    slot.Bind(_inventory.Cells[i]);
                else if (!isUsed)
                    slot.Unbind();

                slot.gameObject.SetActive(isUsed && IsSlotVisible(slot));
            }

            if (_selectedSlot != null && !_selectedSlot.gameObject.activeSelf)
                ApplyDeselection();
        }

        private bool IsSlotVisible(InventorySlotView slot)
        {
            if (slot.IsEmpty)
                return !_hideEmptySlots;

            return _filter == null || _filter.IsValid(slot.Item.Info);
        }

        private void ApplySelection(InventorySlotView slot)
        {
            ClearSelection();

            _selectedSlot = slot;
            slot.Select();
            OnSlotSelected?.Invoke(slot);
            OnSlotSelectedEvent?.Invoke(slot);
        }

        private bool ApplyDeselection()
        {
            var slot = ClearSelection();
            if (slot == null)
                return false;

            OnSlotDeselected?.Invoke(slot);
            OnSlotDeselectedEvent?.Invoke(slot);
            return true;
        }

        private InventorySlotView FindSlot(EntityInfo info)
        {
            foreach (var slot in _slots)
            {
                if (slot.gameObject.activeSelf && !slot.IsEmpty && slot.Item.Info == info)
                    return slot;
            }

            return null;
        }

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
                slot.OnClicked += OnSlotViewClicked;
                _slots.Add(slot);
            }
        }

        #endregion

        #region Listeners

        private void OnInventoryCellsChanged() => RefreshSlots();

        private void OnInventoryItemsChanged(InventoryItemEntityInfo info, int amount)
        {
            if (_hideEmptySlots || _filter != null)
                RefreshSlots();
        }

        private void OnLastSelectedItemChanged(EntityInfo info)
        {
            if (!_selectable)
                return;

            if (_selectedSlot != null && !_selectedSlot.IsEmpty && _selectedSlot.Item.Info == info)
                return;

            var slot = info != null ? FindSlot(info) : null;
            if (slot != null)
                ApplySelection(slot);
            else
                ApplyDeselection();
        }

        private void OnSlotViewClicked(InventorySlotView slot)
        {
            OnSlotClicked?.Invoke(slot);
            OnSlotClickedEvent?.Invoke(slot);

            if (_deselectOnRepeatClick && slot == _selectedSlot)
                Deselect();
            else
                Select(slot);
        }

        #endregion
    }
}
