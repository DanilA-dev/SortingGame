using System.Collections.Generic;
using _Project.Scripts.Core.Upgrades;
using _Project.Scripts.UI.Views;
using D_Dev.CurrencySystem;
using D_Dev.CurrencySystem.Extensions;
using D_Dev.MenuHandler;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class UpgradesMenu : BaseMenu
    {
        #region Fields

        [Title("Data")] 
        [SerializeField] private UpgradesInfoContainer _upgradesContainer;
        [SerializeReference] private PolymorphicValue<CurrencyInfo> _currency = new CurrencyInfoConstantValue();
        [SerializeField] private UpgradableItemView _upgradableItemViewPrefab;
        [Title("UI")]
        [SerializeField] private RectTransform _content;
        [SerializeField] private TMP_Text _currentValueText;
        [SerializeField] private TMP_Text _nextLevelValueText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private UpgradableItemView _upgradeViewShower;
        [SerializeField] private Button _buyUpgradeButton;
        [SerializeField] private RewardedAdButton _adsButton;
        [SerializeField] private string _maxLevelText = "Max";

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onSuccessUpgrade;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onNotEnoughCurrency;

        private List<UpgradableItemView> _createdUpgrades;
        
        #endregion

        #region Properties

        private Currency Currency => _currency.Value.Currency;
        private BaseUpgradeInfo SelectedUpgrade => _upgradeViewShower.UpgradeInfo;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            _buyUpgradeButton.onClick.AddListener(OnBuyClicked);
            _adsButton.OnRewarded += OnAdRewarded;
            Currency.OnCurrencyUpdate += OnCurrencyUpdated;

            InitUpgrades();
            TrySelectFirstUpgrade();
        }

        private void OnDisable()
        {
            _buyUpgradeButton.onClick.RemoveListener(OnBuyClicked);
            _adsButton.OnRewarded -= OnAdRewarded;
            Currency.OnCurrencyUpdate -= OnCurrencyUpdated;
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
            UpdatePriceAndStats();
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

        private void UpdatePriceAndStats()
        {
            var upgradeInfo = SelectedUpgrade;
            var isMaxed = upgradeInfo.IsMaxed;
            var canAfford = upgradeInfo.CanAfford(Currency);

            _currentValueText?.SetText(upgradeInfo.GetValueText(upgradeInfo.Level));
            _nextLevelValueText?.SetText(isMaxed ? _maxLevelText : upgradeInfo.GetValueText(upgradeInfo.Level + 1));
            _priceText?.SetText(isMaxed ? _maxLevelText : upgradeInfo.NextPrice.ToString());

            _buyUpgradeButton.gameObject.SetActive(isMaxed || canAfford);
            _buyUpgradeButton.interactable = !isMaxed;
            _adsButton.gameObject.SetActive(!isMaxed && !canAfford);
        }

        #endregion

        #region Listeners

        private void OnUpgradeViewSelected(UpgradableItemView upgradableItem)
        {
            SelectUpgradeView(upgradableItem);
        }

        private void OnBuyClicked()
        {
            if (SelectedUpgrade.TryUpgrade(Currency))
                OnUpgraded();
            else if (!SelectedUpgrade.IsMaxed)
                _onNotEnoughCurrency?.Invoke();
        }

        private void OnAdRewarded()
        {
            if (SelectedUpgrade.TryUpgradeFree())
                OnUpgraded();
        }

        private void OnUpgraded()
        {
            UpdatePriceAndStats();
            _onSuccessUpgrade?.Invoke();
        }

        private void OnCurrencyUpdated(Currency.CurrencyEvent currencyEvent, long value) => UpdatePriceAndStats();

        #endregion
        
    }
}