using D_Dev.ColliderChecker;
using UnityEngine;

namespace D_Dev.ColliderEvents
{
    public class TriggerColliderObservable : BaseColliderObservable
    {
        #region Monobehaviour

        private void Reset()
        {
            if (TryGetComponent(out Collider c))
                c.isTrigger = true;
        }

        #endregion
        
        #region Trigger 3D

        private void OnTriggerEnter(Collider other)
        {
            if (!_checkEnter || _collisionDimension != CollisionDimension.Collider3d)
                return;
            
            if (!_colliderChecker.IsColliderPassed(other))
                return;

            Colliders.Add(other);
            _onEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_checkExit || _collisionDimension != CollisionDimension.Collider3d)
                return;
            
            if (!_colliderChecker.IsColliderPassed(other))
                return;

            Colliders.Remove(other);
            _onExit?.Invoke(other);
        }

        #endregion

        #region Trigger 2D

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_checkEnter || _collisionDimension != CollisionDimension.Collider2d)
                return;
            
            if (!_colliderChecker.IsColliderPassed(other))
                return;

            Colliders2D.Add(other);
            _onEnter2D?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!_checkExit || _collisionDimension != CollisionDimension.Collider2d)
                return;
            
            if (!_colliderChecker.IsColliderPassed(other))
                return;

            Colliders2D.Remove(other);
            _onExit2D?.Invoke(other);
        }

        #endregion
    }
}
