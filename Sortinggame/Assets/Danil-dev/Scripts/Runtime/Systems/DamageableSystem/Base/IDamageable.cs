using System;

namespace D_Dev.DamageableSystem
{
    public interface IDamageable
    {
        public event Action<IDamageable> OnDeathCallback;
        public bool IsDamageable { get; }
        public void TakeDamage(DamageData damageData);
        public void Kill();
        public void Restore();

    }
}