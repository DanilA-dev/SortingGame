using UnityEngine;

namespace D_Dev.MovementHandler
{
    [System.Serializable]
    public class RigidbodyMovementHandler : BaseMovementHandler
    {
        #region Fields

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private ForceMode _forceMode;

        #endregion

        #region Properties

        public override Rigidbody Rigidbody => _rigidbody;

        #endregion

        #region Overrides

        public override void OnFixedUpdate()
        {
            if (_rigidbody == null)
                return;
 
            if (Direction.magnitude > 0.1f)
            {
                Vector3 force = Direction.normalized * Acceleration;
                _rigidbody.AddForce(force, _forceMode);
            }

            Vector3 velocity = _rigidbody.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0f, velocity.z);

            if (horizontalVelocity.magnitude > MaxVelocity)
            {
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, MaxVelocity);
                _rigidbody.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
            }
        }

        public override void StopMovement()
        {
            if (_rigidbody == null)
                return;
 
            _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
            Direction = Vector3.zero;
        }

        public override float GetVelocity() => _rigidbody != null ? _rigidbody.linearVelocity.magnitude : 0f;
        public override bool IsMoving() => _rigidbody != null && _rigidbody.linearVelocity.magnitude > 0.1f;

        #endregion
    }
}