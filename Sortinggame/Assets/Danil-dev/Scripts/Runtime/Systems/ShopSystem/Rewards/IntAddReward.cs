using System;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.ShopSystem.Rewards
{
    [Serializable]
    public class IntAddReward : IShopReward
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<int> _target = new IntConstantValue();
        [SerializeField] private int _amount = 1;

        #endregion

        #region Public

        public void Grant()
        {
            if (_target != null)
                _target.Value += _amount;
        }

        #endregion
    }
}
