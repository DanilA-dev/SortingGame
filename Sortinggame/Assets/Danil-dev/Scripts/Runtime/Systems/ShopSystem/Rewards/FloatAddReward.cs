using System;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.ShopSystem.Rewards
{
    [Serializable]
    public class FloatAddReward : IShopReward
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<float> _target = new FloatConstantValue();
        [SerializeField] private float _amount = 1f;

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
