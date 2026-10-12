using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.MovementHandler
{
    [System.Serializable]
    public class CharacterControllerMovementHandler : BaseMovementHandler
    {
        #region Fields

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private bool _useGravity = true;
        [ShowIf(nameof(_useGravity))]
        [SerializeField] private float _gravityModifier = 1f;

        private Vector3 _currentVelocity;
        private float _verticalVelocity;

        #endregion

        #region Overrides

        public override void OnUpdate()
        {
            if (_characterController == null || !_characterController.enabled)
                return;

            if (Direction.magnitude > 0.1f)
            {
                _currentVelocity += Direction.normalized * (Acceleration * Time.deltaTime);
                if (_currentVelocity.magnitude > MaxVelocity)
                    _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, MaxVelocity);
            }
            else
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, Time.deltaTime * Acceleration);

            if (_useGravity)
            {
                if (_characterController.isGrounded && _verticalVelocity <= 0f)
                    _verticalVelocity = -0.5f;
                else
                    _verticalVelocity += Physics.gravity.y * _gravityModifier * Time.deltaTime;
            }
            else
            {
                _verticalVelocity = 0f;
            }

            Vector3 motion = _currentVelocity + Vector3.up * _verticalVelocity;
            _characterController.Move(motion * Time.deltaTime);
        }

        public override void StopMovement()
        {
            _currentVelocity = Vector3.zero;
            Direction = Vector3.zero;
        }

        public override float GetVelocity() => _currentVelocity.magnitude;
        public override bool IsMoving() => _currentVelocity.magnitude > 0.1f;

        #endregion

        #region Public

        public Vector3 Velocity => _currentVelocity + Vector3.up * _verticalVelocity;

        public void SetVelocity(Vector3 velocity)
        {
            _currentVelocity = new Vector3(velocity.x, 0f, velocity.z);
            _verticalVelocity = velocity.y;
        }

        public void SetGravity(bool enabled) => _useGravity = enabled;
        public void SetGravityModifier(float modifier) => _gravityModifier = modifier;

        #endregion
    }
}