using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Core.Purchasable
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

        #endregion

        #region Public

        public override string GetCurrentText() => GetValue(Level).ToString();
        public override string GetNextText() => IsMaxed ? string.Empty : GetValue(Level + 1).ToString();

        #endregion

        #region Protected

        protected override void ApplyLevel(int level) => _target.Value = GetValue(level);

        #endregion

        #region Private

        private TValue GetValue(int level) => _values[Mathf.Clamp(level, 0, _values.Length - 1)];

        #endregion
    }
}
