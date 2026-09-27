using UnityEngine;

namespace D_Dev.MovementHandler
{
    public abstract class BaseMovementController : MonoBehaviour
    {
        #region Fields

        [SerializeReference] protected BaseMovementHandler _movementHandler;

        [SerializeField, Min(0f)] protected float _updateInterval = 0f;

        protected bool _isStopped;
        protected float _updateTimer;

        #endregion

        #region Properties

        protected bool IsStopped => _isStopped;

        public Rigidbody Rigidbody => _movementHandler?.Rigidbody;

        public float UpdateInterval
        {
            get => _updateInterval;
            set => _updateInterval = Mathf.Max(0f, value);
        }

        #endregion

        #region Monobehaviour

        protected virtual void Awake() => _movementHandler?.OnAwake();
        protected virtual void Start() => _movementHandler?.OnStart();

        #endregion

        #region Protected

        protected void PerformUpdate()
        {
            if (_isStopped)
                return;

            if (_updateInterval > 0f)
            {
                _updateTimer += Time.deltaTime;
                if (_updateTimer < _updateInterval)
                    return;
                _updateTimer = 0f;
            }

            _movementHandler?.OnUpdate();
        }

        protected void PerformFixedUpdate()
        {
            if (_isStopped)
                return;

            _movementHandler?.OnFixedUpdate();
        }

        #endregion

        #region Public

        public void SetDirection(Vector3 direction)
        {
            if (_movementHandler != null)
                _movementHandler.Direction = direction;
        }

        public void SetMaxVelocity(float maxVelocity)
        {
            if (_movementHandler != null)
                _movementHandler.MaxVelocity = maxVelocity;
        }

        public void SetAcceleration(float acceleration)
        {
            if (_movementHandler != null)
                _movementHandler.Acceleration = acceleration;
        }

        public void StopMovement() => _movementHandler?.StopMovement();

        public void PauseMovement()
        {
            _movementHandler?.StopMovement();
            _isStopped = true;
        }

        public void ResumeMovement()
        {
            _isStopped = false;
            _movementHandler?.ResumeMovement();
        }
        public float GetVelocity() => _movementHandler?.GetVelocity() ?? 0f;
        public bool IsMoving() => _movementHandler?.IsMoving() ?? false;

        #endregion
    }
}