using D_Dev.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace D_Dev.InputSystem.Extensions.Actions
{
    [System.Serializable]
    public class WaitInputAction : BaseAction
    {
        #region Enums

        public enum InputActionPhase
        {
            Started,
            Performed,
            Canceled
        }

        #endregion

        #region Fields

        [SerializeField] private InputActionReference _inputAction;
        [SerializeField] private InputActionPhase _desiredPhase = InputActionPhase.Performed;

        private bool _isSubscribed;

        #endregion

        #region Overrides

        public override void Execute()
        {
            if (_inputAction?.action == null)
                return;

            if (!_isSubscribed)
            {
                _inputAction.action.Enable();
                switch (_desiredPhase)
                {
                    case InputActionPhase.Started:
                        _inputAction.action.started += OnInputTriggered;
                        break;
                    case InputActionPhase.Performed:
                        _inputAction.action.performed += OnInputTriggered;
                        break;
                    case InputActionPhase.Canceled:
                        _inputAction.action.canceled += OnInputTriggered;
                        break;
                }
                _isSubscribed = true;
            }
        }

        #endregion

        #region Listeners

        private void OnInputTriggered(InputAction.CallbackContext context)
        {
            IsFinished = true;
            if (_inputAction?.action != null)
            {
                switch (_desiredPhase)
                {
                    case InputActionPhase.Started:
                        _inputAction.action.started -= OnInputTriggered;
                        break;
                    case InputActionPhase.Performed:
                        _inputAction.action.performed -= OnInputTriggered;
                        break;
                    case InputActionPhase.Canceled:
                        _inputAction.action.canceled -= OnInputTriggered;
                        break;
                }
                _inputAction.action.Disable();
                _isSubscribed = false;
            }
        }

        #endregion
    }
}
