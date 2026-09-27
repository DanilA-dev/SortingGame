using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.ProjectileSystem
{
    public class ArcProjectile : BaseProjectile
    {
        #region Fields

        [Title("Arc")]
        [SerializeField] private float _arcHeight = 2f;

        private float _currentDistance;
        private float _totalDistance;

        #endregion

        #region Override

        protected override void OnInitialized()
        {
            _currentDistance = 0f;
            _totalDistance = Mathf.Max(0.0001f, Vector3.Distance(_startPosition, _endTargetPosition));

            Vector3 initialTangent = _endTargetPosition - _startPosition;
            initialTangent.y += 4f * _arcHeight;

            if (initialTangent.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(initialTangent.normalized);
        }

        protected override void Move(float deltaTime)
        {
            if (_lockToTarget.Value)
            {
                _endTargetPosition = GetCurrentTargetPosition();
                _totalDistance = Mathf.Max(0.0001f, Vector3.Distance(_startPosition, _endTargetPosition));
            }

            _currentDistance += _speed.Value * deltaTime;
            float t = Mathf.Clamp01(_currentDistance / _totalDistance);

            Vector3 newPos = Vector3.Lerp(_startPosition, _endTargetPosition, t);
            newPos.y += 4f * _arcHeight * t * (1f - t);
            transform.position = newPos;

            Vector3 horizontalDir = _endTargetPosition - _startPosition;
            float verticalSlope = 4f * _arcHeight * (1f - 2f * t);
            Vector3 tangent = horizontalDir + Vector3.up * verticalSlope;

            if (tangent.sqrMagnitude > 0.0001f)
                _rotationHandler.SetRotation(Quaternion.LookRotation(tangent.normalized));

            if (t >= 1f && !_lockToTarget.Value)
                BeginRelease();
        }

        #endregion
    }
}
