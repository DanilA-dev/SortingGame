using D_Dev.PolymorphicValueSystem;
using D_Dev.TimerSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.ScreenPointers
{
    public class ScreenTargetPointer : MonoBehaviour
    {
        #region Fields

        [Title("Settings")]
        [SerializeField] private float _smoothTime = 0.15f;
        [SerializeReference] private PolymorphicValue<float> _activeTime = new FloatConstantValue();
        [SerializeReference] private PolymorphicValue<Vector3> _viewOffset = new Vector3ConstantValue();

        [FoldoutGroup("Events")] 
        [SerializeField] private UnityEvent _inCameraViewEvent;
        [FoldoutGroup("Events")] 
        [SerializeField] private UnityEvent _outCameraViewEvent;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onActivate;
        [FoldoutGroup("Events")] 
        [SerializeField] private UnityEvent _onDeactivate;
        
        private float _rotateSpeed;
        private bool _isActive;
        
        private Transform _initialTarget;
        private Transform _moveTarget;
        private Transform _rotateTarget;
        private Transform _origin;
        
        private Vector2 _screenOffset;
        private Vector3 _currentPos;
        private Vector3 _velocity;
        private Vector3 _targetPosition;
        private bool _isBehindCamera;
        
        private Camera _cam;
        private Canvas _pointerCanvas;

        private CountdownTimer _activeTimer;
        #endregion

        #region Monobehaviour

        private void OnDestroy()
        {
            if (_activeTimer != null)
            {
                _activeTimer.Stop();
                _activeTimer.OnTimerEnd -= DeactivatePointer;
            }
        }

        #endregion
        
        #region Public

        public void Init(Transform target,Vector2 screenOffset,
            float rotateSpeed, Canvas pointerCanvas, Transform origin)
        {
            _cam = Camera.main;
            _rotateSpeed = rotateSpeed;
            _screenOffset = screenOffset;
            _pointerCanvas = pointerCanvas;
            
            _initialTarget = target;
            _moveTarget = target;
            _rotateTarget = target;
            _origin = origin;
            
            _activeTimer = new CountdownTimer(_activeTime.Value);
            _activeTimer.OnTimerEnd += DeactivatePointer;
        }


        public void ActivatePointer()
        {
            if(_isActive)
                return;
            
            gameObject.SetActive(true);
            _onActivate?.Invoke();
            _activeTimer?.Start();
            
            _isActive = true;
            
            UpdateTargetPosition();
            transform.position = _targetPosition;
        }
        public void DeactivatePointer()
        {
            if(!_isActive)
                return;

            _isActive = false;
            _onDeactivate?.Invoke();
        }

        public void UpdateTargetPosition()
        {
            if (_initialTarget == null || _origin == null)
                return;

            if (!_isActive)
                return;

            var targetPos = _cam.WorldToScreenPoint(_moveTarget.position);

            _isBehindCamera = targetPos.z < 0;

            if (_isBehindCamera)
            {
                targetPos.x = Screen.width - targetPos.x;
                targetPos.y = Screen.height - targetPos.y;
            }

            if (!IsInCameraView(targetPos) || _isBehindCamera)
            {
                _targetPosition = ClampPositionOnEdge(targetPos);
            }
            else
            {
                _targetPosition = new Vector3(targetPos.x + _viewOffset.Value.x,
                    targetPos.y + _viewOffset.Value.y, targetPos.z + _viewOffset.Value.z);
            }
        }

        public void SmoothUpdate()
        {
            if (_initialTarget == null || _origin == null)
                return;

            if (!_isActive)
                return;

            _activeTimer.Tick(Time.deltaTime);

            if (!IsInCameraView(_targetPosition) || _isBehindCamera)
            {
                MovePointer(_targetPosition);
                RotatePointerTowardsTarget(_isBehindCamera);
                _outCameraViewEvent?.Invoke();
            }
            else
            {
                MovePointer(_targetPosition);
                _inCameraViewEvent?.Invoke();
            }
        }
        #endregion

        #region Private

        private void MovePointer(Vector3 pos)
        {
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref _velocity, _smoothTime);
        }
        
        private void RotatePointerTowardsTarget(bool isCameraBehind)
        {
            var target = _rotateTarget.position;
            var dir = (target - _origin.position).normalized;
            var offset = isCameraBehind ? 90 : -90;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            var targetRot = Quaternion.Euler(0, 0, angle + offset);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, _rotateSpeed);
        }
        
        private bool IsInCameraView(Vector3 pos)
        {
            return pos.x > 0 && pos.x < Screen.width &&
                   pos.y > 0 && pos.y < Screen.height;
        }
        
        private Vector2 ClampPositionOnEdge(Vector3 pointerPos)
        {
            float minY = (_screenOffset.y * _pointerCanvas.scaleFactor);
            return new Vector2(Mathf.Clamp(pointerPos.x, _screenOffset.x, Screen.width - _screenOffset.x),
                Mathf.Clamp(pointerPos.y, minY, Screen.height - _screenOffset.y));
        }
        
        #endregion
    }
}