using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace D_Dev.ShopSystem.UI
{
    public class ShopItemView : MonoBehaviour
    {
        #region Fields

        [Title("UI")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private GameObject _levelRoot;
        [SerializeField] private Button _selectButton;
        [Title("Settings")]
        [SerializeField] private bool _hideWhenUnavailable = true;
        [SerializeField] private bool _hideWhenMaxed;

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onSelect;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onDeselect;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onMaxed;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onNotMaxed;

        private ShopItemInfo _info;
        private ShopItemBuyButton[] _buyButtons;

        public event Action<ShopItemView> OnSelected;
        public event Action<ShopItemView> OnVisibilityChanged;

        #endregion

        #region Properties

        public ShopItemInfo Info => _info;
        public bool IsShown => gameObject.activeSelf;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (_selectButton != null)
                _selectButton.onClick.AddListener(OnSelectClicked);
        }

        private void OnDestroy()
        {
            if (_selectButton != null)
                _selectButton.onClick.RemoveListener(OnSelectClicked);

            Unsubscribe();
        }

        #endregion

        #region Public

        public void Init(ShopItemInfo info)
        {
            Unsubscribe();
            _info = info;

            if (_info == null)
                return;

            _info.OnChanged += Refresh;

            _nameText?.SetText(_info.DisplayName?.Value);
            _descriptionText?.SetText(_info.Description?.Value);

            if (_icon != null)
                _icon.sprite = _info.Icon;

            _buyButtons ??= GetComponentsInChildren<ShopItemBuyButton>(true);
            foreach (var buyButton in _buyButtons)
                buyButton.Init(_info);

            Refresh();
        }

        public void Select() => _onSelect?.Invoke();
        public void Deselect() => _onDeselect?.Invoke();

        #endregion

        #region Private

        private void Refresh()
        {
            var levelText = _info.LevelText;
            var hasLevel = !string.IsNullOrEmpty(levelText);
            _levelText?.SetText(levelText);

            if (_levelRoot != null)
                _levelRoot.SetActive(hasLevel);

            if (_info.IsMaxed)
                _onMaxed?.Invoke();
            else
                _onNotMaxed?.Invoke();

            var shouldShow = (!_hideWhenUnavailable || _info.IsAvailable) && (!_hideWhenMaxed || !_info.IsMaxed);
            if (IsShown == shouldShow)
                return;

            gameObject.SetActive(shouldShow);
            OnVisibilityChanged?.Invoke(this);
        }

        private void Unsubscribe()
        {
            if (_info != null)
                _info.OnChanged -= Refresh;
        }

        #endregion

        #region Listeners

        private void OnSelectClicked() => OnSelected?.Invoke(this);

        #endregion
    }
}
