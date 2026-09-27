using D_Dev.InputSystem;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.PlayerStateController
{
    [DefaultExecutionOrder(-100)]
    public class FreeLookCameraController : MonoBehaviour
    {
        #region Fields

        [Title("Camera Settings")]
        [SerializeField] private InputRouter _inputRouter;
        [SerializeReference] private PolymorphicValue<Transform> _cameraRoot;
        [SerializeField] private float _topAngle = 80f;
        [SerializeField] private float _botAngle = -80f;
        [SerializeField] private float _stickRotationSpeed = 100f;
        [SerializeField] private float _mouseSensitivity = 5f;
        [SerializeField] private bool _isLocked;

        private Vector2 _currentLookInput;

        private float _yaw;
        private float _pitch;

        #endregion

        #region Properties

        public Transform CameraRoot => _cameraRoot.Value;
        public Vector3 CameraForward => _cameraRoot.Value.forward;
        public Vector3 CameraRight => _cameraRoot.Value.right;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if(_inputRouter != null)
                _inputRouter.Look += OnLook;
        }

        private void OnDestroy()
        {
            if (_inputRouter != null)
                _inputRouter.Look -= OnLook;
        }

        private void LateUpdate() => UpdateCameraRotation();

        #endregion

        #region Public

        public void LockCamera() => _isLocked = true;

        public void UnlockCamera() => _isLocked = false;

        #endregion

        #region Listeners

        private void OnLook(Vector2 delta) => _currentLookInput = delta;

        #endregion

        #region Private

        private void UpdateCameraRotation()
        {
            if (_currentLookInput != Vector2.zero && !_isLocked)
            {
                float multiplier = _inputRouter != null && _inputRouter.IsLookFromPointer
                    ? _mouseSensitivity
                    : _stickRotationSpeed * Time.deltaTime;

                _yaw += _currentLookInput.x * multiplier;
                _pitch += _currentLookInput.y * multiplier;
            }

            _yaw = Mathf.Repeat(_yaw, 360f);
            _pitch = Mathf.Clamp(_pitch, _botAngle, _topAngle);
            _cameraRoot.Value.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
        }
        
        #endregion
    }
}