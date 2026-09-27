using D_Dev.DamageableSystem;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Scripts.DamageableSystem.Extensions
{
    public class EntityDamageableObject : BaseDamageableObject
    {
        #region Fields

        [Title("Entity Settings")] 
        [SerializeReference] private PolymorphicValue<float> _currentHealthValue = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<float> _maxHealthValue = new FloatConstantValue();
            
        #endregion

        #region Properties

        public override float MaxHealth { get; protected set; }

        #endregion
        
        #region Overrides

        public override void Init()
        {
            MaxHealth = _maxHealthValue.Value;
            CurrentHealth = MaxHealth;
            _currentHealthValue.Value = CurrentHealth;
        }

        public override void TakeDamage(DamageData damageData)
        {
            base.TakeDamage(damageData);
            _currentHealthValue.Value = CurrentHealth;
        }

        public override void Restore()
        {
            base.Restore();
            _currentHealthValue.Value = CurrentHealth;
        }

        #endregion
    }
}