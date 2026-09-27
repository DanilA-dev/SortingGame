using UnityEngine;

namespace D_Dev.ProjectileSystem
{
    public class LinearProjectile : BaseProjectile
    {
        #region Override

        protected override void Move(float deltaTime)
        {
            Vector3 currentTarget = GetCurrentTargetPosition();
            Vector3 toTarget = currentTarget - transform.position;

            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(toTarget.normalized);
                _rotationHandler.SetRotation(lookRotation);
            }

            float step = _speed.Value * deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, step);

            if (!_lockToTarget.Value && (transform.position - currentTarget).sqrMagnitude < 0.0001f)
                BeginRelease();
        }

        #endregion
    }
}
