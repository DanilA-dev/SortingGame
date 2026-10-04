using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Core.Purchasable
{
    [CreateAssetMenu(menuName = "Game/Purchases/Unlock")]
    public class UnlockInfo : BasePurchasableInfo
    {
        #region Fields

        [Title("Unlock")]
        [SerializeReference] private PolymorphicValue<bool> _isLocked = new BoolConstantValue();
        [SerializeReference] private PolymorphicValue<int> _price = new IntConstantValue();

        #endregion

        #region Properties

        public bool IsLocked => _isLocked.Value;
        public override bool IsAvailable => IsLocked;
        public override bool IsMaxed => !IsLocked;
        public override int NextPrice => _price.Value;

        #endregion

        #region ScriptableObject

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_isLocked != null)
                _isLocked.OnValueChanged += OnLockChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_isLocked != null)
                _isLocked.OnValueChanged -= OnLockChanged;
        }

        #endregion

        #region Public

        public override string GetCurrentText() => string.Empty;
        public override string GetNextText() => string.Empty;

        #endregion

        #region Protected

        protected override void OnPurchased() => _isLocked.Value = false;

        #endregion

        #region Listeners

        private void OnLockChanged(bool isLocked) => RaiseChanged();

        #endregion
    }
}
