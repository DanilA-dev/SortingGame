using System;
using D_Dev.CurrencySystem;
using D_Dev.CurrencySystem.Extensions;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.ShopSystem.Rewards
{
    [Serializable]
    public class CurrencyReward : IShopReward
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<CurrencyInfo> _currency = new CurrencyInfoConstantValue();
        [SerializeField, Min(0)] private long _amount;

        #endregion

        #region Public

        public void Grant()
        {
            var currencyInfo = _currency?.Value;
            if (currencyInfo == null || currencyInfo.Currency == null)
            {
                Debug.LogWarning("[CurrencyReward] Currency is not assigned");
                return;
            }

            currencyInfo.Currency.TryDeposit(_amount);
        }

        #endregion
    }
}
