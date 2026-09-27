using D_Dev.DamageableSystem;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.Scripts.DamageableSystem.Types
{
    public class SimpleDamageableObject : BaseDamageableObject
    {
        #region Fields

        [SerializeField] private PolymorphicValue<float> _maxHealth;

        #endregion

        #region Properties

        public override float MaxHealth
        {
            get => _maxHealth.Value;
            protected set => _maxHealth.Value = value;
        }
        #endregion
    }
}