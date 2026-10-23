using D_Dev.PolymorphicValueSystem;
using D_Dev.PositionRotationConfig;
using D_Dev.StateMachineBehaviour;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Character.States
{
    public class CharacterWanderState : CharacterState, ICompletableState
    {
        #region Enums

        private enum WanderAreaMode
        {
            AroundStartPosition = 0,
            Area = 1
        }

        #endregion

        #region Fields

        [Title("Wander")]
        [SerializeReference] private PolymorphicValue<Transform> _body = new TransformConstantValue();
        [SerializeField] private WanderAreaMode _areaMode;
        [ShowIf(nameof(_areaMode), WanderAreaMode.AroundStartPosition)]
        [SerializeReference] private PolymorphicValue<float> _minDistance = new FloatConstantValue { Value = 1f };
        [ShowIf(nameof(_areaMode), WanderAreaMode.AroundStartPosition)]
        [SerializeReference] private PolymorphicValue<float> _maxDistance = new FloatConstantValue { Value = 3f };
        [ShowIf(nameof(_areaMode), WanderAreaMode.Area)]
        [SerializeReference] private BasePositionSettings _area = new();
        [SerializeReference] private PolymorphicValue<float> _stopDistance = new FloatConstantValue { Value = 0.2f };

        private Vector3 _startPosition;
        private bool _hasStartPosition;
        private Vector3 _targetPoint;
        private bool _isCompleted;

        #endregion

        #region Properties

        public bool IsCompleted => _isCompleted;
        public Vector3 TargetPoint => _targetPoint;

        #endregion

        #region State

        public override void OnEnter()
        {
            base.OnEnter();

            var body = _body.Value;
            if (body == null)
            {
                _isCompleted = true;
                return;
            }

            if (!_hasStartPosition)
            {
                _startPosition = body.position;
                _hasStartPosition = true;
            }

            _targetPoint = GetRandomPoint(body.position.y);
            _isCompleted = false;
        }

        public override void OnUpdate()
        {
            var body = _body.Value;
            if (_isCompleted || body == null)
                return;

            var direction = _targetPoint - body.position;
            direction.y = 0f;

            if (direction.magnitude <= _stopDistance.Value)
            {
                Complete();
                return;
            }

            if (_canRotate)
                _rotationHandler.RotateTowards(direction.normalized);

            if (_canMove)
                _movementController.SetDirection(direction.normalized);
        }

        public override void OnExit()
        {
            base.OnExit();
            _movementController.SetDirection(Vector3.zero);
        }

        #endregion

        #region Public

        public void ResetStartPosition() => _hasStartPosition = false;

        #endregion

        #region Private

        private Vector3 GetRandomPoint(float height)
        {
            Vector3 point;
            if (_areaMode == WanderAreaMode.Area && _area != null)
            {
                point = _area.GetPosition();
            }
            else
            {
                var minDistance = _minDistance.Value;
                var distance = Random.Range(minDistance, Mathf.Max(minDistance, _maxDistance.Value));
                var offset = Random.insideUnitCircle.normalized * distance;
                point = _startPosition + new Vector3(offset.x, 0f, offset.y);
            }

            point.y = height;
            return point;
        }

        private void Complete()
        {
            _isCompleted = true;
            _movementController.SetDirection(Vector3.zero);
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying || _isCompleted)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_targetPoint, _stopDistance.Value);
        }

        #endregion
    }
}
