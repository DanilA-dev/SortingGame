using System;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.DamageableSystem
{
    public abstract class BaseDamageableObject : MonoBehaviour, IDamageable
    {
        #region Fields

        [Title("Damageable Settings")] 
        [SerializeReference] private PolymorphicValue<bool> _isDamageable = new BoolConstantValue();
        [SerializeReference] private PolymorphicValue<float> _lastTakenDamage = new FloatConstantValue();
        [SerializeField] private bool _applyForceOnDamage;
        [ShowIf(nameof(_applyForceOnDamage))]
        [Title("Force On Damage Settings")]
        [SerializeReference] private PolymorphicValue<float> _lastTakenForce = new FloatConstantValue();
        [ShowIf(nameof(_applyForceOnDamage))]
        [SerializeReference] private PolymorphicValue<Vector3> _lastTakenDirection = new Vector3ConstantValue();
        [ShowIf(nameof(_applyForceOnDamage))]
        [SerializeField] private Rigidbody _rigidbody;
        [PropertyOrder(100)]
        [FoldoutGroup("Events")]
        public UnityEvent<DamageData> OnDamage;
        [FoldoutGroup("Events")]
        [PropertyOrder(100)]
        public UnityEvent OnDeath;
        public event Action<IDamageable> OnDeathCallback;
        
        #endregion

        #region Properties

        public bool IsDamageable => _isDamageable.Value;
        public abstract float MaxHealth { get; protected set; }
        public float CurrentHealth { get; protected set; }

        #endregion

        #region Monobehaviour

        private void Start() => Init();

        #endregion
        
        #region IDamageable

        public virtual void TakeDamage(DamageData damageData)
        {
            if(damageData.Damage <= 0)
                return;
            
            if(!_isDamageable.Value)
                return;
            
            _lastTakenDamage.Value = damageData.Damage;
            CurrentHealth -= damageData.Damage;
            if (_applyForceOnDamage && damageData.Force > 0)
            {
                _lastTakenDirection.Value = damageData.ForceDirection;
                _lastTakenForce.Value = damageData.Force;
            }
            
            OnDamage?.Invoke(damageData);
            if(CurrentHealth <= 0)
                OnDie();
        }

        public void Kill() => OnDie();
        public virtual void Restore()
        {
            CurrentHealth = MaxHealth;
        }

        #endregion

        #region Virtual
        public virtual void Init() => CurrentHealth = MaxHealth;

        #endregion
        
        #region Private
        private void OnDie()
        {
            CurrentHealth = 0;
            OnDeathCallback?.Invoke(this);
            OnDeath?.Invoke();
        }

        #endregion
    }
}