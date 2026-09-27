using System;
using D_Dev.PolymorphicValueSystem;
using D_Dev.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.TransformFollower
{
    public abstract class BaseTransformFollower : MonoBehaviour
    {
        #region Enum

        [Flags]
        public enum AxisUpdate
        {
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2,
            All = X | Y | Z
        }

        #endregion
        
        #region Fields

        [Title("Transforms")] 
        [SerializeReference] private PolymorphicValue<Transform> _follower = new TransformConstantValue();
        [SerializeReference] private PolymorphicValue<Transform> _objectToFollow = new TransformConstantValue();
        [Title("Settings")]
        [SerializeField] private float _tickInterval = 0.05f;
        [SerializeField] private bool _updatePosition = true;
        [SerializeField] private bool _updateRotation = true;

        [ShowIf(nameof(_updatePosition))]
        [FoldoutGroup("Update Position Settings")] 
        [SerializeField] private Vector3 _positionOffset;
        [FoldoutGroup("Update Position Settings")] 
        [SerializeField] private float _positionSpeed = 5f;
        [FoldoutGroup("Update Position Settings")] 
        [ShowIf(nameof(_updatePosition))]
        [SerializeField] private AxisUpdate _posAxisUpdate = AxisUpdate.All;
        [FoldoutGroup("Update Position Settings")]
        [ShowIf(nameof(_updatePosition))]
        [SerializeField] private bool _updatePositionOnceOnStart;
        
        [ShowIf(nameof(_updateRotation))]
        [FoldoutGroup("Update Rotation Settings")] 
        [SerializeField] private float _rotationSpeed = 5f;
        [FoldoutGroup("Update Rotation Settings")] 
        [SerializeField] private Vector3 _rotationMultiplier = Vector3.one;
        [FoldoutGroup("Update Rotation Settings")] 
        [ShowIf(nameof(_updateRotation))]
        [SerializeField] private AxisUpdate _rotAxisUpdate = AxisUpdate.All;
        [FoldoutGroup("Update Rotation Settings")]
        [ShowIf(nameof(_updateRotation))]
        [SerializeField] private bool _updateRotationOnceOnStart;
        
        private RotationHandler _rotationHandler;
        
        private float _lastTickTime;
        
        private bool _positionUpdatedOnce;
        private bool _rotationUpdatedOnce;
        
        private Vector3 _targetPosition;

        #endregion

        #region Monobehaviour

        private void Start()
        {
            _rotationHandler = new RotationHandler();
            _lastTickTime = Time.time;

            if (!IsFollowerNull())
                _rotationHandler.Initialize(_follower.Value, _rotationSpeed);
        }

        #endregion

        #region Private

        protected virtual void OnUpdate()
        {
            if (!IsObjectToFollowNull() && Time.time - _lastTickTime >= _tickInterval)
            {
                _targetPosition = _objectToFollow.Value.position;
                _lastTickTime = Time.time;
            }
            
            UpdatePosition();
            UpdateRotation();
        }

        private void UpdatePosition()
        {
            if (!_updatePosition || IsFollowerNull())
                return;

            if (_updatePositionOnceOnStart && _positionUpdatedOnce)
                return;

            Vector3 currentPos = _follower.Value.position;
            Vector3 targetPos = FilterAxis(_targetPosition + _positionOffset, currentPos, _posAxisUpdate);
            _follower.Value.position = Vector3.Lerp(currentPos, targetPos, _positionSpeed * Time.deltaTime);

            if (_updatePositionOnceOnStart)
                _positionUpdatedOnce = true;
        }

        private void UpdateRotation()
        {
            if (!_updateRotation || IsFollowerNull() || IsObjectToFollowNull())
                return;

            if (_updateRotationOnceOnStart && _rotationUpdatedOnce)
                return;

            Vector3 direction = _targetPosition - _follower.Value.position;
            Vector3 filteredDirection = FilterAxis(direction, _follower.Value.forward, _rotAxisUpdate);
            filteredDirection = Vector3.Scale(filteredDirection, _rotationMultiplier);
            _rotationHandler?.RotateTowards(filteredDirection, _rotationSpeed, false);

            if (_updateRotationOnceOnStart)
                _rotationUpdatedOnce = true;
        }

        private Vector3 FilterAxis(Vector3 target, Vector3 current, AxisUpdate axisUpdate)
        {
            Vector3 result = current;
            if ((axisUpdate & AxisUpdate.X) != 0) result.x = target.x;
            if ((axisUpdate & AxisUpdate.Y) != 0) result.y = target.y;
            if ((axisUpdate & AxisUpdate.Z) != 0) result.z = target.z;
            return result;
        }

        private bool IsFollowerNull() => _follower == null || _follower.Value == null;
        private bool IsObjectToFollowNull() => _objectToFollow == null || _objectToFollow.Value == null;

        #endregion
    }
}
