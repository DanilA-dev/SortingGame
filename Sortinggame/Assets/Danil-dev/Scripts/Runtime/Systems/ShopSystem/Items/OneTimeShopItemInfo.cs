using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ShopSystem
{
    [CreateAssetMenu(menuName = "D-Dev/Shop/One Time Item")]
    public class OneTimeShopItemInfo : ShopItemInfo
    {
        #region Fields

        [Title("Purchase")]
        [SerializeReference] private PolymorphicValue<bool> _isPurchased = new BoolConstantValue();

        #endregion

        #region Properties

        public bool IsPurchased => _isPurchased.Value;

        public override bool IsConsumable => false;
        public override bool IsOwned => IsPurchased;
        public override bool IsMaxed => IsPurchased;

        #endregion

        #region ScriptableObject

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_isPurchased != null)
                _isPurchased.OnValueChanged += OnPurchasedChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (_isPurchased != null)
                _isPurchased.OnValueChanged -= OnPurchasedChanged;
        }

        #endregion

        #region Protected

        protected override void OnPurchaseCompleted() => _isPurchased.Value = true;

        #endregion

        #region Listeners

        private void OnPurchasedChanged(bool isPurchased) => RaiseChanged();

        #endregion
    }
}
