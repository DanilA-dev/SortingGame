using System;
using _Project.Scripts.Core.Purchasable;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Views
{
    public class BuyShopItemView : MonoBehaviour
    {
        #region Fields

        [Title("Item")]
        [SerializeField] private ShopItemView _itemView;
        [Title("Values")]
        [SerializeField] private TMP_Text _currentValueText;
        [SerializeField] private TMP_Text _nextValueText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private string _maxLevelText = "Max";
        [Title("Buttons")]
        [SerializeField] private Button _buyButton;
        [SerializeField] private RewardedAdButton _adsButton;
        [FoldoutGroup("Events")]
        public UnityEvent OnPurchased;
        [FoldoutGroup("Events")]
        public UnityEvent OnNotEnoughCurrency;
        

        private BasePurchasableInfo _info;


        #endregion

        #region Properties

        public BasePurchasableInfo Info => _info;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            _buyButton.onClick.AddListener(OnBuyClicked);
            _adsButton.OnRewarded += OnAdRewarded;
        }

        private void OnDisable()
        {
            _buyButton.onClick.RemoveListener(OnBuyClicked);
            _adsButton.OnRewarded -= OnAdRewarded;
        }

        private void OnDestroy() => Unsubscribe();

        #endregion

        #region Public

        public void Show(BasePurchasableInfo info)
        {
            Unsubscribe();
            _info = info;
            _info.OnChanged += Refresh;

            _itemView.Init(_info);
            Refresh();
        }

        #endregion

        #region Private

        private void Refresh()
        {
            var isMaxed = _info.IsMaxed;
            var canAfford = _info.CanAfford();

            _currentValueText?.SetText(_info.GetCurrentText());
            _nextValueText?.SetText(isMaxed ? _maxLevelText : _info.GetNextText());
            _priceText?.SetText(isMaxed ? _maxLevelText : _info.NextPrice.ToString());

            _buyButton.gameObject.SetActive(!isMaxed);
            _adsButton.gameObject.SetActive(!isMaxed && !canAfford);
        }

        private void Unsubscribe()
        {
            if (_info != null)
                _info.OnChanged -= Refresh;
        }

        #endregion

        #region Listeners

        private void OnBuyClicked()
        {
            if (_info == null)
                return;

            if (_info.TryPurchase())
                OnPurchased?.Invoke();
            else if (!_info.IsMaxed)
                OnNotEnoughCurrency?.Invoke();
        }

        private void OnAdRewarded()
        {
            if (_info != null && _info.TryPurchaseFree())
                OnPurchased?.Invoke();
        }

        #endregion
    }
}
