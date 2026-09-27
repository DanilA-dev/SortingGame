using D_Dev.Base;
using D_Dev.PositionRotationConfig;
using UnityEngine;

namespace D_Dev.Actions
{
    [System.Serializable]
    public class TransformRotateAction : BaseAction
    {
        #region Enums

        public enum RotationAxis
        {
            All,
            X,
            Y,
            Z
        }

        #endregion

        #region Fields

        [SerializeField] private Transform _transformToRotate;
        [Space]
        [SerializeReference] private BasePositionSettings _target = new();
        [SerializeField] private RotationAxis _rotationAxis = RotationAxis.All;
        [SerializeField] private float _rotationSpeed = 180f;
        [SerializeField] private float _angleThreshold = 1f;

        #endregion

        #region Overrides
        
        public override void Execute()
        {
            if (_transformToRotate == null)
                return;

            Vector3 targetPosition = _target.GetPosition();
            Vector3 direction = targetPosition - _transformToRotate.position;

            if (direction == Vector3.zero)
                return;

            Quaternion targetRotation = GetTargetRotation(direction);
            _transformToRotate.rotation = Quaternion.RotateTowards(_transformToRotate.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            if(IsTargetRotationEnded())
                IsFinished = true;
        }

        #endregion

        #region Private

        private bool IsTargetRotationEnded()
        {
            if (_transformToRotate == null)
                return true;

            Vector3 targetPosition = _target.GetPosition();
            Vector3 direction = targetPosition - _transformToRotate.position;

            if (direction == Vector3.zero)
                return true;

            Quaternion targetRotation = GetTargetRotation(direction);
            return Quaternion.Angle(_transformToRotate.rotation, targetRotation) < _angleThreshold;
        }
        
        private Quaternion GetTargetRotation(Vector3 direction)
        {
            switch (_rotationAxis)
            {
                case RotationAxis.All:
                    return Quaternion.LookRotation(direction);
                case RotationAxis.X:
                    float angleX = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;
                    return Quaternion.Euler(angleX, 0, 0);
                case RotationAxis.Y:
                    float angleY = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                    return Quaternion.Euler(0, angleY, 0);
                case RotationAxis.Z:
                    float angleZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    return Quaternion.Euler(0, 0, angleZ);
                default:
                    return Quaternion.LookRotation(direction);
            }
        }

        #endregion
    }
}
