using System;
using _Project.Scripts.Core.Purchasable;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Views
{
    public class ShopItemView : MonoBehaviour
    {
        #region Fields

        [Title("UI")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _levelLabelText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _selectButton;
        [Title("Settings")]
        [SerializeField] private bool _hideWhenUnavailable = true;

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onSelect;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onDeselect;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onMaxLevel;

        private BasePurchasableInfo _info;

        public event Action<ShopItemView> OnSelected;
        public event Action<ShopItemView> OnAvailabilityChanged;

        #endregion

        #region Properties

        public BasePurchasableInfo Info => _info;
        public bool IsShown => gameObject.activeSelf;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if(_selectButton != null)
                _selectButton.onClick.AddListener(OnSelectClicked);
        }

        private void OnDestroy()
        {
            if(_selectButton != null)
                _selectButton.onClick.RemoveListener(OnSelectClicked);

            Unsubscribe();
        }

        #endregion

        #region Public

        public void Init(BasePurchasableInfo info)
        {
            Unsubscribe();
            _info = info;
            _info.OnChanged += OnInfoChanged;

            _nameText?.SetText(_info.DisplayName.Value);
            _descriptionText?.SetText(_info.Description.Value);
            _icon.sprite = _info.Icon;

            Refresh();
        }

        public void Select() => _onSelect?.Invoke();
        public void Deselect() => _onDeselect?.Invoke();

        #endregion

        #region Private

        private void Refresh()
        {
            _levelText?.SetText(_info.LevelText);
            _levelLabelText?.gameObject.SetActive(!string.IsNullOrEmpty(_info.LevelText));
            
            if (_info.IsMaxed)
                _onMaxLevel?.Invoke();

            if (!_hideWhenUnavailable || IsShown == _info.IsAvailable)
                return;

            gameObject.SetActive(_info.IsAvailable);
            OnAvailabilityChanged?.Invoke(this);
        }

        private void Unsubscribe()
        {
            if (_info != null)
                _info.OnChanged -= OnInfoChanged;
        }

        #endregion

        #region Listeners

        private void OnInfoChanged() => Refresh();

        private void OnSelectClicked() => OnSelected?.Invoke(this);

        #endregion
    }
}
