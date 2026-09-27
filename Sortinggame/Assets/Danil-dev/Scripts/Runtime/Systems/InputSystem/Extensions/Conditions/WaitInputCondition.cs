using D_Dev.Base;
using UnityEngine;
using UnityEngine.InputSystem;

namespace D_Dev.InputSystem.Extensions.Conditions
{
    [System.Serializable]
    public class WaitInputCondition : ICondition
    {
        #region Enums

        public enum InputActionThisFrame
        {
            Pressed,
            Released
        }

        #endregion
        
        #region Fields

        [SerializeField] private InputActionReference _inputAction;
        [SerializeField] private InputActionThisFrame _desiredPhase = InputActionThisFrame.Pressed;

        private bool _isMet;
        private bool _isSubscribed;

        #endregion

        #region ICondition

        public bool IsConditionMet()
        {
            if (_inputAction?.action == null)
                return false;
            
            if(!_inputAction.action.enabled)
                _inputAction.action.Enable();
            
            return _desiredPhase switch
            {
                InputActionThisFrame.Pressed => _inputAction.action.IsPressed(),
                InputActionThisFrame.Released => !_inputAction.action.IsPressed(),
                _ => false
            };
        }

        public void Reset()
        {
        }

        #endregion
    }
}
