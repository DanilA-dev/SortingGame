using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ShopSystem
{
    public abstract class UpgradeShopItemInfo : ShopItemInfo
    {
        #region Fields

        [Title("Upgrade")]
        [SerializeReference] private PolymorphicValue<int> _level = new IntConstantValue();

        #endregion

        #region Properties

        public int Level => _level.Value;
        public abstract int MaxLevel { get; }

        public override bool IsConsumable => false;
        public override bool IsOwned => Level > 0;
        public override bool IsMaxed => Level >= MaxLevel;
        public override int PriceIndex => Level + 1;
        public override string LevelText => (Level + 1).ToString();

        #endregion

        #region ScriptableObject

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_level != null)
                _level.OnValueChanged += OnLevelChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_level != null)
                _level.OnValueChanged -= OnLevelChanged;
        }

        #endregion

        #region Public

        public override void Apply() => ApplyLevel(Mathf.Clamp(Level, 0, MaxLevel));

        #endregion

        #region Protected

        protected override void OnPurchaseCompleted()
        {
            _level.Value = Level + 1;
            Apply();
        }

        protected abstract void ApplyLevel(int level);

        #endregion

        #region Listeners

        private void OnLevelChanged(int level) => RaiseChanged();

        #endregion
    }
}
