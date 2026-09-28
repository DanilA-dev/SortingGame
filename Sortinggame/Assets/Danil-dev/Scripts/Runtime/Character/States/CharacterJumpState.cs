using D_Dev.MovementHandler;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Character.States
{
    public class CharacterJumpState : CharacterState
    {
        #region Enum

        private enum JumpType
        {
            RigidbodyJump = 0,
            CharacterControllerJump = 1
        }

        #endregion

        #region Fields

        [Space]
        [SerializeField] private JumpType _jumpType;
        [ShowIf(nameof(_jumpType), JumpType.RigidbodyJump)]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeReference] private PolymorphicValue<Vector3> _jumpDirection = new Vector3ConstantValue();
        [SerializeReference] private PolymorphicValue<float> _maxJumpVelocity = new FloatConstantValue();

        #endregion

        #region State

        public override void OnEnter()
        {
            base.OnEnter();

            var ccHandler = _movementController.MovementHandler as CharacterControllerMovementHandler;
            if (_jumpType == JumpType.CharacterControllerJump && ccHandler == null)
            {
                Debug.Log($"{nameof(CharacterJumpState)}: movement controller has no {nameof(CharacterControllerMovementHandler)}", this);
                return;
            }

            var jump = _jumpDirection.Value.normalized * _maxJumpVelocity.Value;
            var velocity = _jumpType == JumpType.RigidbodyJump
                ? _rigidbody.linearVelocity
                : ccHandler.Velocity;

            var charDirection = transform.TransformDirection(new Vector3(jump.x, jump.y, jump.z));
            var newVel = new Vector3(velocity.x + charDirection.x, jump.y, velocity.z + charDirection.z);
            if(_jumpType == JumpType.RigidbodyJump)
                _rigidbody.linearVelocity = newVel;
            else
                ccHandler.SetVelocity(newVel);

            if (_preserveMomentum)
            {
                var horizontal = new Vector3(newVel.x, 0f, newVel.z);
                _movementController.SetMaxVelocity(Mathf.Max(_maxMoveSpeed.Value, horizontal.magnitude));
            }
        }

        #endregion
    }
}
