using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ShopSystem
{
    [CreateAssetMenu(menuName = "D-Dev/Shop/Unlock Item")]
    public class UnlockShopItemInfo : ShopItemInfo
    {
        #region Fields

        [Title("Unlock")]
        [SerializeReference] private PolymorphicValue<bool> _isLocked = new BoolConstantValue();

        #endregion

        #region Properties

        public bool IsLocked => _isLocked.Value;

        public override bool IsConsumable => false;
        public override bool IsOwned => !IsLocked;
        public override bool IsMaxed => !IsLocked;

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

        #region Protected

        protected override void OnPurchaseCompleted() => _isLocked.Value = false;

        #endregion

        #region Listeners

        private void OnLockChanged(bool isLocked) => RaiseChanged();

        #endregion
    }
}
