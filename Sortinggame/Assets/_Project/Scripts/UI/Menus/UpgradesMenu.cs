using System.Collections.Generic;
using _Project.Scripts.Core.Upgrades;
using _Project.Scripts.UI.Views;
using D_Dev.MenuHandler;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class UpgradesMenu : BaseMenu
    {
        #region Fields

        [Title("Data")] 
        [SerializeField] private UpgradesInfoContainer _upgradesContainer;
        [SerializeField] private UpgradableItemView _upgradableItemViewPrefab;
        [Title("UI")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private UpgradableItemView _upgradeViewShower;
        [SerializeField] private Button _buyUpgradeButton;

        private List<UpgradableItemView> _createdUpgrades;
        
        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            InitUpgrades();
            TrySelectFirstUpgrade();
        }

        private void OnDestroy()
        {
            DisposeUpgrades();
        }

        #endregion


        #region Private

        private void InitUpgrades()
        {
            if (_createdUpgrades == null)
            {
                _createdUpgrades = new();
                foreach (var upgradeInfo in _upgradesContainer.Upgrades)
                {
                    var newUpgradeItem = Instantiate(_upgradableItemViewPrefab, _content);
                    newUpgradeItem.Init(upgradeInfo);
                    newUpgradeItem.OnUpgradableSelected += OnUpgradeViewSelected;
                    _createdUpgrades.Add(newUpgradeItem);
                }
            }
        }

        private void DisposeUpgrades()
        {
            if(_createdUpgrades == null)
                return;

            foreach (var upgradableItemView in _createdUpgrades)
                upgradableItemView.OnUpgradableSelected -= OnUpgradeViewSelected;
        }

        private void SelectUpgradeView(UpgradableItemView upgradableItem)
        {
            if(_createdUpgrades == null)
                return;
            
            if(upgradableItem == null)
                return;
            
            DeselectAll();
            _upgradeViewShower.Init(upgradableItem.UpgradeInfo);
            upgradableItem.Select();
            _priceText?.SetText(_upgradeViewShower.UpgradeInfo.NextPrice.ToString());
        }

        private void DeselectAll()
        {
            if(_createdUpgrades == null)
                return;

            foreach (var upgradableItemView in _createdUpgrades)
                upgradableItemView.Deselect();
        }
        
        private void TrySelectFirstUpgrade()
        {
            SelectUpgradeView(_createdUpgrades[0]);
        }

        #endregion

        #region Listeners

        private void OnUpgradeViewSelected(UpgradableItemView upgradableItem)
        {
            SelectUpgradeView(upgradableItem);
        }

        #endregion
        
    }
}