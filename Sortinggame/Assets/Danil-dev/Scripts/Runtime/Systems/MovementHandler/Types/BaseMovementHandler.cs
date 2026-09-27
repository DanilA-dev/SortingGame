using UnityEngine;

namespace D_Dev.MovementHandler
{
    [System.Serializable]
    public abstract class BaseMovementHandler
    {
        #region Fields

        protected Vector3 _direction;
        protected float _acceleration;
        protected float _maxVelocity;

        #endregion

        #region Properties
        public Vector3 Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public float Acceleration
        {
            get => _acceleration;
            set => _acceleration = value;
        }

        public float MaxVelocity
        {
            get => _maxVelocity;
            set => _maxVelocity = value;
        }

        public virtual Rigidbody Rigidbody => null;

        #endregion
                
        #region Abstract
        
        public virtual void OnAwake() {}
        public virtual void OnStart() {}
        public virtual void OnUpdate() {}
        public virtual void OnFixedUpdate() {}
        public abstract void StopMovement();
        public virtual void ResumeMovement() {}
        public abstract float GetVelocity();
        public abstract bool IsMoving();

        #endregion
    }
}