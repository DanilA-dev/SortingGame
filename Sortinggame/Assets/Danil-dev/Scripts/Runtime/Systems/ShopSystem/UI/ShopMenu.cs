using D_Dev.MenuHandler;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.ShopSystem.UI
{
    public class ShopMenu : BaseMenu
    {
        #region Fields

        [Title("Shop")]
        [SerializeField] private ShopItemsListView _listView;
        [SerializeField] private ShopItemDetailsView _detailsView;

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onPurchased;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onPaymentFailed;

        private bool _isSubscribed;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            if (_isSubscribed)
                return;

            _isSubscribed = true;

            if (_listView != null)
                _listView.OnItemSelected += OnItemSelected;

            if (_detailsView != null)
            {
                _detailsView.OnPurchased += OnPurchased;
                _detailsView.OnPaymentFailed += OnPaymentFailed;
            }

            if (_listView != null && _listView.SelectedItem != null && _detailsView != null)
                _detailsView.Show(_listView.SelectedItem.Info);
        }

        private void OnDisable()
        {
            if (!_isSubscribed)
                return;

            _isSubscribed = false;

            if (_listView != null)
                _listView.OnItemSelected -= OnItemSelected;

            if (_detailsView != null)
            {
                _detailsView.OnPurchased -= OnPurchased;
                _detailsView.OnPaymentFailed -= OnPaymentFailed;
            }
        }

        #endregion

        #region Listeners

        private void OnItemSelected(ShopItemView item)
        {
            if (_detailsView != null)
                _detailsView.Show(item.Info);
        }

        private void OnPurchased(ShopItemInfo info) => _onPurchased?.Invoke();

        private void OnPaymentFailed(ShopItemInfo info) => _onPaymentFailed?.Invoke();

        #endregion
    }
}
