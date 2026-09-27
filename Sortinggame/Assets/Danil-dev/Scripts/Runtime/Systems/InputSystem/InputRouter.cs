using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputActions;

namespace D_Dev.InputSystem
{
    [CreateAssetMenu(menuName = "D-Dev/InputRouter")]
    public class InputRouter : ScriptableObject, IPlayerActions
    {
        #region Fields

        private InputActions _inputActions;
        
        public event Action<Vector2> Move;
        public event Action<Vector2> Look;
        public event Action<bool> SpacePressed;
        public event Action<bool> ShiftPressed;
        public event Action<bool> CtrlPressed;
        public event Action<bool> EscPressed;
        public event Action<bool> EPressed;
        public event Action<bool> RmbPressed;
        public event Action<bool> LmbPressed;

        #endregion

        #region Properties

        public bool IsLookFromPointer { get; private set; }

        #endregion

        #region IInputRouter

        public void Enable()
        {
            if (_inputActions == null)
            {
                _inputActions = new InputActions();
                _inputActions.Player.SetCallbacks(this);
            }
            _inputActions.Enable();
        }

        public void Disable() => _inputActions?.Disable();

        #endregion

        #region IPlayerActions

        public void OnLook(InputAction.CallbackContext context)
        {
            IsLookFromPointer = context.control?.device is Pointer;
            Look?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnMove(InputAction.CallbackContext context) => Move?.Invoke(context.ReadValue<Vector2>());

        public void OnLmb(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    LmbPressed?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    LmbPressed?.Invoke(false);
                    break;
            }
        }

        public void OnRmb(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    RmbPressed?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    RmbPressed?.Invoke(false);
                    break;
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EPressed?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    EPressed?.Invoke(false);
                    break;
            }
        }
        
        public void OnSprint(InputAction.CallbackContext context)
        {
            bool isPressed = context.ReadValueAsButton();
            ShiftPressed?.Invoke(isPressed);
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    CtrlPressed?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    CtrlPressed?.Invoke(false);
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    SpacePressed?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    SpacePressed?.Invoke(false);
                    break;
            }
        }

        public void OnEsc(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EscPressed?.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    EscPressed?.Invoke(false);
                    break;
            }
        }

        

        #endregion
      
    }
}
