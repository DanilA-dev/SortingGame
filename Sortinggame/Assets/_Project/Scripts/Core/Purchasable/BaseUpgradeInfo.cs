using D_Dev.ScriptableVariables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Core.Purchasable
{
    public abstract class BaseUpgradeInfo : BasePurchasableInfo
    {
        #region Fields

        [Title("Upgrade")]
        [SerializeField] private IntScriptableVariable _level;
        [SerializeField] private int[] _prices;
        [SerializeField] private UnlockInfo _requirement;

        #endregion

        #region Properties

        public int Level => _level.Value;
        public int MaxLevel => _prices.Length - 1;

        public override bool IsAvailable => _requirement == null || !_requirement.IsLocked;
        public override bool IsMaxed => Level >= MaxLevel;
        public override int NextPrice => IsMaxed ? -1 : _prices[Level + 1];
        public override string LevelText => (Level + 1).ToString();

        #endregion

        #region ScriptableObject

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_requirement != null)
                _requirement.OnChanged += RaiseChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_requirement != null)
                _requirement.OnChanged -= RaiseChanged;
        }

        #endregion

        #region Public

        public override void Apply() => ApplyLevel(Mathf.Clamp(Level, 0, MaxLevel));

        #endregion

        #region Protected

        protected override void OnPurchased()
        {
            _level.Value++;
            Apply();
            RaiseChanged();
        }

        protected abstract void ApplyLevel(int level);

        #endregion
    }
}
