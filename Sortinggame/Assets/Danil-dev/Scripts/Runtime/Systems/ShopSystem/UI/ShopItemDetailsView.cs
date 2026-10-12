using System;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace D_Dev.ShopSystem.UI
{
    public class ShopItemDetailsView : MonoBehaviour
    {
        #region Fields

        [Title("Item")]
        [SerializeField] private ShopItemView _itemView;
        [Title("Values")]
        [SerializeField] private TMP_Text _currentValueText;
        [SerializeField] private TMP_Text _nextValueText;
        [SerializeField] private GameObject _valuesRoot;
        [SerializeReference] private PolymorphicValue<string> _maxedText = new StringConstantValue();

        private ShopItemInfo _info;
        private ShopItemBuyButton[] _buyButtons;

        public event Action<ShopItemInfo> OnPurchased;
        public event Action<ShopItemInfo> OnPaymentFailed;

        #endregion

        #region Properties

        public ShopItemInfo Info => _info;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            CacheBuyButtons();

            foreach (var buyButton in _buyButtons)
            {
                buyButton.OnPurchased += OnButtonPurchased;
                buyButton.OnPaymentFailed += OnButtonPaymentFailed;
            }
        }

        private void OnDestroy()
        {
            Unsubscribe();

            if (_buyButtons == null)
                return;

            foreach (var buyButton in _buyButtons)
            {
                if (buyButton == null)
                    continue;

                buyButton.OnPurchased -= OnButtonPurchased;
                buyButton.OnPaymentFailed -= OnButtonPaymentFailed;
            }
        }

        #endregion

        #region Public

        public void Show(ShopItemInfo info)
        {
            Unsubscribe();
            _info = info;

            if (_info != null)
                _info.OnChanged += Refresh;

            if (_itemView != null)
                _itemView.Init(_info);

            CacheBuyButtons();
            foreach (var buyButton in _buyButtons)
                buyButton.Init(_info);

            Refresh();
        }

        #endregion

        #region Private

        private void CacheBuyButtons() => _buyButtons ??= GetComponentsInChildren<ShopItemBuyButton>(true);

        private void Refresh()
        {
            if (_info == null)
                return;

            var currentText = _info.GetCurrentValueText();
            var nextText = _info.IsMaxed ? _maxedText?.Value : _info.GetNextValueText();

            _currentValueText?.SetText(currentText);
            _nextValueText?.SetText(nextText);

            if (_valuesRoot != null)
                _valuesRoot.SetActive(!string.IsNullOrEmpty(currentText));
        }

        private void Unsubscribe()
        {
            if (_info != null)
                _info.OnChanged -= Refresh;
        }

        #endregion

        #region Listeners

        private void OnButtonPurchased(ShopItemInfo info) => OnPurchased?.Invoke(info);

        private void OnButtonPaymentFailed(ShopItemInfo info) => OnPaymentFailed?.Invoke(info);

        #endregion
    }
}
