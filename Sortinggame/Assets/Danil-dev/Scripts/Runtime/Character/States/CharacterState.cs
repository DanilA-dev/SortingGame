using D_Dev.MovementHandler;
using D_Dev.PolymorphicValueSystem;
using D_Dev.StateMachineBehaviour;
using D_Dev.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Character.States
{
    public class CharacterState : BaseComponentState
    {
        #region Fields

        [PropertyOrder(99)]
        [SerializeField] protected BaseMovementController _movementController;

        [Space]
        [PropertyOrder(99)]
        [SerializeField] protected bool _canMove;
        [PropertyOrder(99)]
        [SerializeField] protected bool _canRotate;

        [PropertyOrder(99)]
        [ShowIf("@_canMove || _canRotate")]
        [SerializeReference] protected PolymorphicValue<Vector3> _movementDirection = new Vector3ConstantValue();

        [PropertyOrder(99)]
        [FoldoutGroup("Movement Settings")]
        [ShowIf(nameof(_canMove))]
        [SerializeReference] protected PolymorphicValue<float> _maxMoveSpeed = new FloatConstantValue();
        [PropertyOrder(99)]
        [FoldoutGroup("Movement Settings")]
        [ShowIf(nameof(_canMove))]
        [SerializeReference] protected PolymorphicValue<float> _accelerationSpeed = new FloatConstantValue();
        [PropertyOrder(99)]
        [FoldoutGroup("Movement Settings")]
        [ShowIf(nameof(_canMove))]
        [SerializeField] protected bool _preserveMomentum;

        [PropertyOrder(99)]
        [FoldoutGroup("Rotation Settings")]
        [ShowIf(nameof(_canRotate))]
        [SerializeReference] protected PolymorphicValue<Transform> _rotateRoot = new TransformConstantValue();
        [PropertyOrder(99)]
        [FoldoutGroup("Rotation Settings")]
        [ShowIf(nameof(_canRotate))]
        [SerializeReference] protected PolymorphicValue<float> _rotateSpeed = new FloatConstantValue();

        protected RotationHandler _rotationHandler = new();

        private bool _isSubscribedToMaxMoveSpeed;

        #endregion

        #region State

        public override void OnEnter()
        {
            if (_canMove)
            {
                _movementController.ResumeMovement();

                float maxSpeed = _maxMoveSpeed.Value;
                if (_preserveMomentum)
                    maxSpeed = Mathf.Max(maxSpeed, _movementController.GetVelocity());

                _movementController.SetMaxVelocity(maxSpeed);
                _movementController.SetAcceleration(_accelerationSpeed.Value);
                SubscribeToMaxMoveSpeed();
            }
            else
            {
                _movementController.StopMovement();
            }

            if (_canRotate)
                _rotationHandler.Initialize(_rotateRoot.Value, _rotateSpeed.Value, _movementController.Rigidbody);
        }

        public override void OnExit()
        {
            UnsubscribeFromMaxMoveSpeed();
        }

        public override void OnUpdate()
        {
            if (_canRotate)
                _rotationHandler.RotateTowards(_movementDirection.Value);

            if (_canMove)
                _movementController.SetDirection(_movementDirection.Value);
        }

        #endregion

        #region Protected

        protected virtual void OnMaxMoveSpeedChanged(float maxSpeed)
        {
            if (_movementController == null)
            {
                UnsubscribeFromMaxMoveSpeed();
                return;
            }

            _movementController.SetMaxVelocity(maxSpeed);
        }

        #endregion

        #region Private

        private void SubscribeToMaxMoveSpeed()
        {
            if (_isSubscribedToMaxMoveSpeed || _maxMoveSpeed == null)
                return;

            _maxMoveSpeed.OnValueChanged += OnMaxMoveSpeedChanged;
            _isSubscribedToMaxMoveSpeed = true;
        }

        private void UnsubscribeFromMaxMoveSpeed()
        {
            if (!_isSubscribedToMaxMoveSpeed)
                return;

            _maxMoveSpeed.OnValueChanged -= OnMaxMoveSpeedChanged;
            _isSubscribedToMaxMoveSpeed = false;
        }

        #endregion
    }
}