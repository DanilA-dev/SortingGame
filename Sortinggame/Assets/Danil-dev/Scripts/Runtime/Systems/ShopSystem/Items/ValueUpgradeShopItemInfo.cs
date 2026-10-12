using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ShopSystem
{
    public abstract class ValueUpgradeShopItemInfo<TValue> : UpgradeShopItemInfo
    {
        #region Fields

        [Title("Value")]
        [SerializeReference] private PolymorphicValue<TValue> _target;
        [SerializeField] private TValue[] _values = new TValue[0];

        #endregion

        #region Properties

        public PolymorphicValue<TValue> Target => _target;
        public override int MaxLevel => _values.Length - 1;

        #endregion

        #region Public

        public override string GetCurrentValueText() => GetValueText(Level);
        public override string GetNextValueText() => IsMaxed ? string.Empty : GetValueText(Level + 1);

        #endregion

        #region Protected

        protected override void ApplyLevel(int level)
        {
            if (_target != null && _values.Length > 0)
                _target.Value = GetValue(level);
        }

        protected virtual string FormatValue(TValue value) => value?.ToString() ?? string.Empty;

        #endregion

        #region Private

        private TValue GetValue(int level) => _values[Mathf.Clamp(level, 0, _values.Length - 1)];

        private string GetValueText(int level) => _values.Length > 0 ? FormatValue(GetValue(level)) : string.Empty;

        #endregion
    }
}
