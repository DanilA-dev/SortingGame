using System;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ShopSystem.Prices;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace D_Dev.ShopSystem.UI
{
    public class ShopItemBuyButton : MonoBehaviour
    {
        #region Enums

        public enum ButtonState
        {
            None = 0,
            Hidden = 1,
            Unavailable = 2,
            Maxed = 3,
            Ready = 4,
            CannotPay = 5,
            Paying = 6
        }

        #endregion

        #region Fields

        private const float DynamicRefreshInterval = 0.2f;

        [Title("Item")]
        [SerializeField] private ShopItemInfo _info;
        [Title("Price")]
        [SerializeField] private ShopPriceType _priceType;
        [SerializeField, Min(0)] private int _priceTypeIndex;
        [Tooltip("Show this button only when no other price of the item can be paid.")]
        [SerializeField] private bool _showOnlyAsFallback;
        [Title("UI")]
        [Tooltip("Child object hidden when the item has no matching price. Must not be this object.")]
        [SerializeField] private GameObject _content;
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Image _icon;
        [SerializeReference] private PolymorphicValue<string> _maxedText = new StringConstantValue();
        [SerializeReference] private PolymorphicValue<string> _freeText = new StringConstantValue();

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onPurchased;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onPaymentFailed;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onReady;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onCannotPay;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onPaying;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onMaxed;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onUnavailable;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onHidden;

        private BaseShopItemPrice _price;
        private ButtonState _state;
        private bool _isSubscribed;
        private float _nextDynamicRefreshTime;

        public event Action<ShopItemInfo> OnPurchased;
        public event Action<ShopItemInfo> OnPaymentFailed;

        #endregion

        #region Properties

        public ShopItemInfo Info => _info;
        public BaseShopItemPrice Price => _price;
        public ButtonState State => _state;

        private GameObject Content => _content != null ? _content : _button != null ? _button.gameObject : null;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (_button != null)
                _button.onClick.AddListener(OnClicked);
        }

        private void OnEnable()
        {
            _price = FindPrice();
            Subscribe();
            _state = ButtonState.None;
            Refresh();
        }

        private void OnDisable() => Unsubscribe();

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnClicked);
        }

        private void Update()
        {
            if (_price == null || !_price.IsDynamic || Time.unscaledTime < _nextDynamicRefreshTime)
                return;

            _nextDynamicRefreshTime = Time.unscaledTime + DynamicRefreshInterval;
            Refresh();
        }

        #endregion

        #region Public

        public void Init(ShopItemInfo info)
        {
            Unsubscribe();
            _info = info;
            _price = FindPrice();
            _state = ButtonState.None;

            if (isActiveAndEnabled)
                Subscribe();

            Refresh();
        }

        #endregion

        #region Private

        private BaseShopItemPrice FindPrice() => _info != null ? _info.GetPrice(_priceType, _priceTypeIndex) : null;

        private void Subscribe()
        {
            if (_isSubscribed || _info == null)
                return;

            _isSubscribed = true;
            _info.OnChanged += Refresh;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
                return;

            _isSubscribed = false;

            if (_info != null)
                _info.OnChanged -= Refresh;
        }

        private void Refresh()
        {
            var state = GetState();

            var content = Content;
            if (content != null && content != gameObject)
                content.SetActive(state != ButtonState.Hidden);

            if (state != ButtonState.Hidden)
            {
                _priceText?.SetText(GetPriceText(state));

                if (_icon != null)
                {
                    var icon = _price.GetIcon(_info);
                    _icon.sprite = icon;
                    _icon.gameObject.SetActive(icon != null && state != ButtonState.Maxed && !_price.IsFree(_info));
                }
            }

            if (_button != null)
                _button.interactable = state is ButtonState.Ready or ButtonState.CannotPay;

            if (state == _state)
                return;

            _state = state;
            InvokeStateEvent(state);
        }

        private ButtonState GetState()
        {
            if (_info == null || _price == null)
                return ButtonState.Hidden;

            if (!_info.IsAvailable)
                return ButtonState.Unavailable;

            if (_info.IsMaxed)
                return ButtonState.Maxed;

            if (_info.IsPurchasing)
                return ButtonState.Paying;

            if (_showOnlyAsFallback && CanPayWithOtherPrice())
                return ButtonState.Hidden;

            return _price.CanPay(_info) ? ButtonState.Ready : ButtonState.CannotPay;
        }

        private bool CanPayWithOtherPrice()
        {
            foreach (var price in _info.Prices)
            {
                if (price != null && price != _price && price.CanPay(_info))
                    return true;
            }

            return false;
        }

        private string GetPriceText(ButtonState state)
        {
            if (state == ButtonState.Maxed)
                return _maxedText?.Value;

            if (_price.IsFree(_info))
                return _freeText?.Value;

            return _price.GetLabel(_info);
        }

        private void InvokeStateEvent(ButtonState state)
        {
            switch (state)
            {
                case ButtonState.Hidden:
                    _onHidden?.Invoke();
                    break;
                case ButtonState.Unavailable:
                    _onUnavailable?.Invoke();
                    break;
                case ButtonState.Maxed:
                    _onMaxed?.Invoke();
                    break;
                case ButtonState.Ready:
                    _onReady?.Invoke();
                    break;
                case ButtonState.CannotPay:
                    _onCannotPay?.Invoke();
                    break;
                case ButtonState.Paying:
                    _onPaying?.Invoke();
                    break;
            }
        }

        #endregion

        #region Listeners

        private void OnClicked()
        {
            if (_info == null || _price == null || _info.IsPurchasing)
                return;

            var info = _info;
            info.Purchase(_price, isSuccess =>
            {
                if (this == null)
                    return;

                if (isSuccess)
                {
                    OnPurchased?.Invoke(info);
                    _onPurchased?.Invoke();
                }
                else
                {
                    OnPaymentFailed?.Invoke(info);
                    _onPaymentFailed?.Invoke();
                }

                Refresh();
            });
        }

        #endregion
    }
}
