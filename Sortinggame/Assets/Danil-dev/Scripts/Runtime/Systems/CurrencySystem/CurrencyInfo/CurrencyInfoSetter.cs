using D_Dev.CustomEventManager;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.CurrencySystem
{
    public class CurrencyInfoSetter : MonoBehaviour
    {
        #region Enums

        public enum StartCurrencySetAction
        {
            Withdraw,
            Deposit,
            Set
        }

        #endregion
        
        #region Fields

        [SerializeReference] private PolymorphicValue<CurrencyInfo> _currencyInfo;
        [SerializeReference] private PolymorphicValue<int> _amount = new IntConstantValue();
        [SerializeField] private bool _setOnEnable;
        [ShowIf("_setOnEnable")]
        [SerializeField] private StartCurrencySetAction _startCurrencySetAction;
        [FoldoutGroup("Flying Animation")]
        [SerializeField] private bool _useFlyingAnimation;
        [FoldoutGroup("Flying Animation")]
        [ShowIf(nameof(_useFlyingAnimation))] 
        [SerializeReference] private PolymorphicValue<string> _flyingAnimationEventName = new StringConstantValue();
        [FoldoutGroup("Flying Animation")]
        [ShowIf(nameof(_useFlyingAnimation))]
        [SerializeReference] private PolymorphicValue<Transform> _from;
        [FoldoutGroup("Flying Animation")]
        [ShowIf(nameof(_useFlyingAnimation))]
        [SerializeReference] private PolymorphicValue<Transform> _to;
        [Space]
        [FoldoutGroup("Deposit Events")]
        public UnityEvent<int> OnDepositSuccess;
        [FoldoutGroup("Deposit Events")]
        public UnityEvent<int> OnDepositFailed;
        [FoldoutGroup("Withdraw Events")]
        public UnityEvent<int> OnWithdrawSuccess;
        [FoldoutGroup("Withdraw Events")]
        public UnityEvent<int> OnWithdrawFailed;
        [FoldoutGroup("Set Events")]
        public UnityEvent<int> OnSetSuccess;
        [FoldoutGroup("Set Events")]
        public UnityEvent<int> OnSetFailed;
            
        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            SetCurrencyOnEnable();
        }


        #endregion
        
        #region Public

        public void TryDeposit() => TryDepositValue(_amount.Value);

        public void TryWithdraw() => TryWithdrawValue(_amount.Value);

        public void TrySet() => TrySetValue(_amount.Value);

        public bool TryDepositValue(long amount)
        {
            if (_currencyInfo != null && _currencyInfo.Value.Currency.TryDeposit(amount))
            {
                OnDepositSuccess?.Invoke((int)amount);
                if(_useFlyingAnimation)
                    EventManager.Invoke(_flyingAnimationEventName.Value, _from.Value, _to.Value, (int)amount);
                return true;
            }
            OnDepositFailed?.Invoke((int)amount);
            return false;
        }

        public bool TryWithdrawValue(long amount)
        {
            if (_currencyInfo != null && _currencyInfo.Value.Currency.TryWithdraw(amount))
            {
                OnWithdrawSuccess?.Invoke((int)amount);
                if(_useFlyingAnimation)
                    EventManager.Invoke(_flyingAnimationEventName.Value, _from.Value, _to.Value, (int)amount);
                return true;
            }
            OnWithdrawFailed?.Invoke((int)amount);
            return false;
        }

        

        public bool TrySetValue(long amount)
        {
            if (_currencyInfo != null && _currencyInfo.Value.Currency.TrySet(amount))
            {
                OnSetSuccess?.Invoke((int)amount);
                return true;
            }
            OnSetFailed?.Invoke((int)amount);
            return false;
        }

        public void SetAmount(int amount) => _amount.Value = amount;

        #endregion

        #region Private
        private void SetCurrencyOnEnable()
        {
            if (_setOnEnable)
            {
                switch (_startCurrencySetAction)
                {
                    case StartCurrencySetAction.Withdraw:
                        TryWithdraw();
                        break;
                    case StartCurrencySetAction.Deposit:
                        TryDeposit();
                        break;
                    case StartCurrencySetAction.Set:
                        TrySet();
                        break;
                }
            }
        }

        #endregion
    }
}
