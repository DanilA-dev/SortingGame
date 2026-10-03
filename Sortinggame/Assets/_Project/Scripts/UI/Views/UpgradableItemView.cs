using System;
using _Project.Scripts.Core.Upgrades;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Views
{
    public class UpgradableItemView : MonoBehaviour
    {
        #region Fields

        [Title("UI")]
        [SerializeField] private TMP_Text _upgradableNameText;
        [SerializeField] private TMP_Text _upgradableDescriptionText;
        [SerializeField] private TMP_Text _upgradableLevelText;
        [SerializeField] private Image _icon;
        [SerializeField] private Button _selectButton;

        [FoldoutGroup("Events")] 
        [SerializeField] private UnityEvent _onSelect;
        [FoldoutGroup("Events")] 
        [SerializeField] private UnityEvent _onDeselect;
        [FoldoutGroup("Events")] 
        [SerializeField] private UnityEvent _onMaxLevel;

        private BaseUpgradeInfo _upgradeInfo;

        public event Action<UpgradableItemView> OnUpgradableSelected;


        #endregion

        #region Properties

        public BaseUpgradeInfo UpgradeInfo => _upgradeInfo;
        public bool IsMaxed => _upgradeInfo != null && _upgradeInfo.IsMaxed;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if(_selectButton != null)
                _selectButton.onClick.AddListener(SelectUpgradableView);
        }

        private void OnDestroy()
        {
            if(_selectButton != null)
                _selectButton.onClick.RemoveListener(SelectUpgradableView);
            
            if(_upgradeInfo != null)
                _upgradeInfo.OnLevelChanged -= OnLevelUpdate;
        }

        #endregion
        
        #region Public

        public void Init(BaseUpgradeInfo upgradeInfo)
        {
            if(_upgradeInfo != null)
                _upgradeInfo.OnLevelChanged -= OnLevelUpdate;

            _upgradeInfo = upgradeInfo;
            _upgradeInfo.OnLevelChanged += OnLevelUpdate;

            UpdateData();
        }

        public void Select()
        {
            _onSelect?.Invoke();
        }

        public void Deselect()
        {
            _onDeselect?.Invoke();
        }

        #endregion

        #region Private

        private void UpdateData()
        {
            if(_upgradeInfo == null)
                return;
            
            _upgradableNameText?.SetText(_upgradeInfo.UpgradeName.Value);
            _upgradableDescriptionText?.SetText(_upgradeInfo.UpgradeDescription.Value);
            _icon.sprite = _upgradeInfo.Icon;

            UpdateLevel();
        }

        private void UpdateLevel()
        {
            _upgradableLevelText?.SetText((_upgradeInfo.Level + 1).ToString());

            if(IsMaxed)
                _onMaxLevel?.Invoke();
        }

        #endregion

        #region Listener

        private void OnLevelUpdate(int level)
        {
            UpdateLevel();
        }
        
        private void SelectUpgradableView()
        {
            OnUpgradableSelected?.Invoke(this);
        }

        #endregion
        
    }
}