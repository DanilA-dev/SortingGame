using System;
using System.Collections.Generic;
using _Project.Scripts.Core.Purchasable;
using _Project.Scripts.UI.Views;
using D_Dev.MenuHandler;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts.UI
{
    public class ShopMenu : BaseMenu
    {
        #region Classes

        [Serializable]
        public class Section
        {
            public PurchasesContainer Container;
            public RectTransform Content;
        }

        #endregion

        #region Fields

        [Title("Data")]
        [SerializeField] private Section[] _sections;
        [SerializeField] private ShopItemView _itemViewPrefab;
        [Title("UI")]
        [SerializeField] private BuyShopItemView _buyItemView;

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onSuccessUpgrade;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onNotEnoughCurrency;

        private List<ShopItemView> _createdItems;
        private ShopItemView _selectedItem;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            _buyItemView.OnPurchased.AddListener(OnPurchased);
            _buyItemView.OnNotEnoughCurrency.AddListener(OnNotEnoughCurrency);

            InitItems();
            SelectFirstShown();
        }

        private void OnDisable()
        {
            _buyItemView.OnPurchased.RemoveListener(OnPurchased);
            _buyItemView.OnNotEnoughCurrency.RemoveListener(OnNotEnoughCurrency);
        }

        private void OnDestroy()
        {
            DisposeItems();
        }

        #endregion

        #region Private

        private void InitItems()
        {
            if (_createdItems != null)
                return;

            _createdItems = new();
            foreach (var section in _sections)
            {
                foreach (var info in section.Container.Purchases)
                {
                    var item = Instantiate(_itemViewPrefab, section.Content);
                    item.OnSelected += OnItemSelected;
                    item.OnAvailabilityChanged += OnItemAvailabilityChanged;
                    item.Init(info);
                    _createdItems.Add(item);
                }
            }
        }

        private void DisposeItems()
        {
            if(_createdItems == null)
                return;

            foreach (var item in _createdItems)
            {
                item.OnSelected -= OnItemSelected;
                item.OnAvailabilityChanged -= OnItemAvailabilityChanged;
            }
        }

        private void SelectItem(ShopItemView item)
        {
            if(item == null)
                return;

            foreach (var createdItem in _createdItems)
                createdItem.Deselect();

            _selectedItem = item;
            _buyItemView.Show(item.Info);
            item.Select();
        }

        private void SelectFirstShown()
        {
            foreach (var item in _createdItems)
            {
                if (!item.IsShown)
                    continue;

                SelectItem(item);
                return;
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

            SelectItem(item);
            return true;
        }

        #endregion

        #region Listeners

        private void OnItemSelected(ShopItemView item) => SelectItem(item);

        private void OnItemAvailabilityChanged(ShopItemView item)
        {
            if (item == _selectedItem && !item.IsShown)
                SelectNearestShown(item);
        }

        private void OnPurchased() => _onSuccessUpgrade?.Invoke();

        private void OnNotEnoughCurrency() => _onNotEnoughCurrency?.Invoke();

        #endregion
    }
}
