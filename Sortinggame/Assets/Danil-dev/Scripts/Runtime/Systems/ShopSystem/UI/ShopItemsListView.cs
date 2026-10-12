using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ShopSystem.UI
{
    public class ShopItemsListView : MonoBehaviour
    {
        #region Classes

        [Serializable]
        public class Section
        {
            public ShopItemsContainer Container;
            public RectTransform Content;
        }

        #endregion

        #region Fields

        [Title("Data")]
        [SerializeField] private Section[] _sections;
        [SerializeField] private ShopItemView _itemViewPrefab;
        [Title("Selection")]
        [SerializeField] private bool _selectable = true;
        [ShowIf(nameof(_selectable))]
        [SerializeField] private bool _selectFirstOnEnable = true;

        private readonly List<ShopItemView> _items = new();
        private ShopItemView _selectedItem;
        private bool _isCreated;

        public event Action<ShopItemView> OnItemSelected;

        #endregion

        #region Properties

        public IReadOnlyList<ShopItemView> Items => _items;
        public ShopItemView SelectedItem => _selectedItem;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            CreateItems();

            if (_selectable && _selectFirstOnEnable && (_selectedItem == null || !_selectedItem.IsShown))
                SelectFirstShown();
        }

        private void OnDestroy()
        {
            foreach (var item in _items)
            {
                if (item == null)
                    continue;

                item.OnSelected -= OnItemClicked;
                item.OnVisibilityChanged -= OnItemVisibilityChanged;
            }
        }

        #endregion

        #region Public

        public void Select(ShopItemView item)
        {
            if (!_selectable || item == null)
                return;

            foreach (var createdItem in _items)
                createdItem.Deselect();

            _selectedItem = item;
            item.Select();
            OnItemSelected?.Invoke(item);
        }

        public void SelectFirstShown()
        {
            foreach (var item in _items)
            {
                if (!item.IsShown)
                    continue;

                Select(item);
                return;
            }
        }

        #endregion

        #region Private

        private void CreateItems()
        {
            if (_isCreated)
                return;

            _isCreated = true;

            foreach (var section in _sections)
            {
                if (section?.Container == null || section.Content == null)
                    continue;

                foreach (var info in section.Container.Items)
                {
                    if (info == null)
                        continue;

                    var item = Instantiate(_itemViewPrefab, section.Content);
                    item.OnSelected += OnItemClicked;
                    item.OnVisibilityChanged += OnItemVisibilityChanged;
                    item.Init(info);
                    _items.Add(item);
                }
            }
        }

        private void SelectNearestShown(ShopItemView hiddenItem)
        {
            var parent = hiddenItem.transform.parent;
            var startIndex = hiddenItem.transform.GetSiblingIndex();

            for (int i = startIndex + 1; i < parent.childCount; i++)
            {
                if (TrySelectShown(parent.GetChild(i)))
                    return;
            }

            for (int i = startIndex - 1; i >= 0; i--)
            {
                if (TrySelectShown(parent.GetChild(i)))
                    return;
            }

            SelectFirstShown();
        }

        private bool TrySelectShown(Transform child)
        {
            if (!child.TryGetComponent(out ShopItemView item) || !item.IsShown)
                return false;

            Select(item);
            return true;
        }

        #endregion

        #region Listeners

        private void OnItemClicked(ShopItemView item) => Select(item);

        private void OnItemVisibilityChanged(ShopItemView item)
        {
            if (_selectable && item == _selectedItem && !item.IsShown)
                SelectNearestShown(item);
        }

        #endregion
    }
}
