using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.Character.States
{
    public class CharacterJumpState : CharacterState
    {
        #region Fields

        [Space]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeReference] private PolymorphicValue<Vector3> _jumpDirection = new Vector3ConstantValue();
        [SerializeReference] private PolymorphicValue<float> _maxJumpVelocity = new FloatConstantValue();

        #endregion

        #region State

        public override void OnEnter()
        {
            base.OnEnter();

            var jump = _jumpDirection.Value.normalized * _maxJumpVelocity.Value;
            var velocity = _rigidbody.linearVelocity;
            var charDirection = transform.TransformDirection(new Vector3(jump.x, jump.y, jump.z));
            _rigidbody.linearVelocity = new Vector3(velocity.x + charDirection.x, jump.y, velocity.z + charDirection.z);

            if (_preserveMomentum)
            {
                var horizontal = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
                _movementController.SetMaxVelocity(Mathf.Max(_maxMoveSpeed.Value, horizontal.magnitude));
            }
        }

        #endregion
    }
}