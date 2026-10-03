using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Core.Upgrades
{
    public abstract class ValueUpgradeInfo<TValue> : BaseUpgradeInfo
    {
        #region Fields

        [Space]
        [Title("Value")]
        [SerializeReference] private PolymorphicValue<TValue> _target;
        [SerializeField] private TValue[] _values;

        #endregion

        #region Properties

        public PolymorphicValue<TValue> Target => _target;
        public TValue CurrentValue => _values[Mathf.Clamp(Level, 0, MaxLevel)];
        public TValue NextValue => IsMaxed ? CurrentValue : _values[Level + 1];

        #endregion

        #region Public

        public override string GetValueText(int level) => _values[Mathf.Clamp(level, 0, MaxLevel)].ToString();

        #endregion

        #region Protected

        protected override void ApplyLevel(int level) => _target.Value = _values[level];

        #endregion

        #region Private

        private bool IsValuesValid(TValue[] values) => values != null && values.Length == MaxLevel + 1;

        #endregion
    }
}
