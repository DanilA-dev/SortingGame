using System;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.ShopSystem.Rewards
{
    [Serializable]
    public class BoolSetReward : IShopReward
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<bool> _target = new BoolConstantValue();
        [SerializeField] private bool _value = true;

        #endregion

        #region Public

        public void Grant()
        {
            if (_target != null)
                _target.Value = _value;
        }

        #endregion
    }
}
