using D_Dev.InputSystem;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace D_Dev.PlayerStateController
{
    [DefaultExecutionOrder(-100)]
    public class FreeLookCameraController : MonoBehaviour
    {
        #region Fields

        [Title("Camera Settings")]
        [SerializeField] private InputRouter _inputRouter;
        [SerializeField] private InputActionReference _lookAction;
        [SerializeReference] private PolymorphicValue<Transform> _cameraRoot = new TransformConstantValue();
        [SerializeReference] private PolymorphicValue<float> _topAngle = new FloatConstantValue() { Value = 80};
        [SerializeReference] private PolymorphicValue<float> _botAngle = new FloatConstantValue() { Value = -80};
        [SerializeReference] private PolymorphicValue<float> _stickRotationSpeed = new FloatConstantValue() { Value = 100};
        [SerializeReference] private PolymorphicValue<float> _mouseSensitivity = new FloatConstantValue() { Value = 3 };
        [SerializeReference] private PolymorphicValue<bool> _isLocked = new BoolConstantValue();

        private InputAction _resolvedLook;
        private Vector2 _currentLookInput;
        private bool _isLookFromPointer;

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
            if (_inputRouter == null)
                return;

            _resolvedLook = _inputRouter.Resolve(_lookAction);
            if (_resolvedLook == null)
                return;

            _resolvedLook.performed += OnLook;
            _resolvedLook.canceled += OnLook;
        }

        private void OnDestroy()
        {
            if (_resolvedLook == null)
                return;

            _resolvedLook.performed -= OnLook;
            _resolvedLook.canceled -= OnLook;
            _resolvedLook = null;
        }

        private void LateUpdate() => UpdateCameraRotation();

        #endregion

        #region Listeners

        private void OnLook(InputAction.CallbackContext context)
        {
            _isLookFromPointer = context.control?.device is Pointer;
            _currentLookInput = context.ReadValue<Vector2>();
        }

        #endregion

        #region Private

        private void UpdateCameraRotation()
        {
            if (_currentLookInput != Vector2.zero && !_isLocked.Value)
            {
                float multiplier = _isLookFromPointer
                    ? _mouseSensitivity.Value
                    : _stickRotationSpeed.Value * Time.deltaTime;

                _yaw += _currentLookInput.x * multiplier;
                _pitch += _currentLookInput.y * multiplier;
            }

            _yaw = Mathf.Repeat(_yaw, 360f);
            _pitch = Mathf.Clamp(_pitch, _botAngle.Value, _topAngle.Value);
            _cameraRoot.Value.rotation = Quaternion.Euler(_pitch, _yaw, 0.0f);
        }

        #endregion
    }
}
