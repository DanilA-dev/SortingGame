using System;
using D_Dev.CurrencySystem;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Core.Upgrades
{
    public abstract class BaseUpgradeInfo : ScriptableObject
    {
        #region Fields

        [Title("Base")]
        [SerializeReference] private PolymorphicValue<string> _upgradeName = new StringConstantValue();
        [SerializeReference] private PolymorphicValue<string> _upgradeDescription = new StringConstantValue();
        [PreviewField(75, ObjectFieldAlignment.Right)]
        [SerializeField] private Sprite _icon;
        [SerializeField] private IntScriptableVariable _level;
        [SerializeField] private int[] _prices;
        [Space]
        [Title("Debug")]
        [SerializeField] private bool _showDebugInfo = true;

        public event Action<int> OnLevelChanged;
        public event Action OnNotEnoughCurrency;

        #endregion

        #region Properties

        public int Level => _level.Value;
        public int MaxLevel => _prices.Length;
        public bool IsMaxed => Level >= MaxLevel;
        public int NextPrice => IsMaxed ? -1 : _prices[Level];

        public PolymorphicValue<string> UpgradeName => _upgradeName;

        public PolymorphicValue<string> UpgradeDescription => _upgradeDescription;

        public Sprite Icon => _icon;

        #endregion

        #region Public

        public bool TryUpgrade(Currency currency)
        {
            if (IsMaxed)
            {
                if (_showDebugInfo)
                    Debug.Log($"[Upgrade] {name} has reached max level");

                return false;
            }

            if (!currency.TryWithdraw(NextPrice))
            {
                if (_showDebugInfo)
                    Debug.Log($"[Upgrade] {name} not enough currency to next level");

                OnNotEnoughCurrency?.Invoke();
                return false;
            }

            _level.Value++;
            Apply();
            OnLevelChanged?.Invoke(Level);
            return true;
        }

        public void Apply() => ApplyLevel(Mathf.Clamp(Level, 0, MaxLevel));

        public abstract string GetValueText(int level);

        #endregion

        #region Protected

        protected abstract void ApplyLevel(int level);

        #endregion
    }
}
