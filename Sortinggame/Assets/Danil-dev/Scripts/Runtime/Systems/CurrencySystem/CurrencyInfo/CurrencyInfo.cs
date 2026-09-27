using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.CurrencySystem
{
    [CreateAssetMenu(menuName = "D-Dev/Currencies/CurrencyData")]
    public class CurrencyInfo : ScriptableObject
    {
        #region Fields

        [Title("Currency Settings")]
        [SerializeField] private Currency _currency;

        [PreviewField(75, ObjectFieldAlignment.Right)]
        [SerializeField] private Sprite _currencyIcon;

        #endregion

        #region Properties

        public Currency Currency => _currency;

        public Sprite CurrencyIcon => _currencyIcon;

        #endregion
        
        #region Debug

        [PropertySpace(10)]
        [Button]
        private void DebugDepositValue(long depositValue) => _currency.TryDeposit(depositValue);

        [Button]
        private void DebugWithdrawValue(long withdrawValue) => _currency.TryWithdraw(withdrawValue);

        [Button]
        public void ResetToZero() => _currency.TrySet(0);
        [Button]
        public void ResetToDefault() => _currency.TrySet(_currency.DefaultValue);

        #endregion
    }
}
