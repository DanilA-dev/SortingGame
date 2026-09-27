using UnityEngine;

namespace D_Dev.MovementHandler
{
    [System.Serializable]
    public class TransformMovementHandler : BaseMovementHandler
    {
        #region Fields

        [SerializeField] private Transform _targetTransform;
        private Vector3 _currentVelocity;

        #endregion

        #region Overrides

        public override void OnUpdate()
        {
            if (_targetTransform == null)
                return;

            if (Direction.magnitude > 0.1f)
            {
                _currentVelocity += Direction.normalized * (Acceleration * Time.deltaTime);
                if (_currentVelocity.magnitude > MaxVelocity)
                    _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, MaxVelocity);
            }
            else
            {
                _currentVelocity = Vector3.Lerp(_currentVelocity, Vector3.zero, Time.deltaTime * Acceleration);
            }

            _targetTransform.position += _currentVelocity * Time.deltaTime;
        }

        public override void StopMovement()
        {
            _currentVelocity = Vector3.zero;
            Direction = Vector3.zero;
        }

        public override float GetVelocity() => _currentVelocity.magnitude;
        public override bool IsMoving() => _currentVelocity.magnitude > 0.1f;

        #endregion
    }
}
