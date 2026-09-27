using UnityEngine;

namespace D_Dev.MovementHandler
{
    [System.Serializable]
    public class KinematicRigidbodyMovementHandler : BaseMovementHandler
    {
        #region Fields

        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private bool _useMovePosition = true;

        private Vector3 _velocity;

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
                _velocity += Direction.normalized * (Acceleration * Time.fixedDeltaTime);
                _velocity = Vector3.ClampMagnitude(_velocity, MaxVelocity);
            }
            else
                _velocity = Vector3.MoveTowards(_velocity, Vector3.zero, Acceleration * Time.fixedDeltaTime);

            Vector3 newPosition = _rigidbody.position + _velocity * Time.fixedDeltaTime;

            if (_useMovePosition)
                _rigidbody.MovePosition(newPosition);
            else
                _rigidbody.position = newPosition;
        }

        public override void StopMovement()
        {
            _velocity = Vector3.zero;
            Direction = Vector3.zero;
        }

        public override float GetVelocity() => _velocity.magnitude;
        public override bool IsMoving() => _velocity.magnitude > 0.1f;

        #endregion
    }
}
