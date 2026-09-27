using System;
using D_Dev.ColliderChecker;
using UnityEngine;

namespace D_Dev.ColliderEvents
{
    public class CollisionColliderObservable : BaseColliderObservable
    {
        #region Monobehaviour

        private void Reset()
        {
            if (TryGetComponent(out Collider c))
                c.isTrigger = false;
        }

        #endregion
        
        #region Collision 3D

        private void OnCollisionEnter(Collision collision)
        {
            if (!_checkEnter || _collisionDimension != CollisionDimension.Collider3d)
                return;
            
            if (!_colliderChecker.IsColliderPassed(collision.collider))
                return;

            Colliders.Add(collision.collider);
            _onEnter?.Invoke(collision.collider);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (!_checkExit || _collisionDimension != CollisionDimension.Collider3d)
                return;
            
            if (!_colliderChecker.IsColliderPassed(collision.collider))
                return;

            Colliders.Remove(collision.collider);
            _onExit?.Invoke(collision.collider);
        }

        #endregion

        #region Collision 2D

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!_checkEnter || _collisionDimension != CollisionDimension.Collider2d)
                return;
            
            if (!_colliderChecker.IsColliderPassed(collision.collider))
                return;

            Colliders2D.Add(collision.collider);
            _onEnter2D?.Invoke(collision.collider);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!_checkExit || _collisionDimension != CollisionDimension.Collider2d)
                return;
            
            if (!_colliderChecker.IsColliderPassed(collision.collider))
                return;

            Colliders2D.Remove(collision.collider);
            _onExit2D?.Invoke(collision.collider);
        }

        #endregion
    }
}
