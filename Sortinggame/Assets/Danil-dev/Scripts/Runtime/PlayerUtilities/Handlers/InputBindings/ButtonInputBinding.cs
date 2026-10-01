using D_Dev.PolymorphicValueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace D_Dev.PlayerStateController.InputBindings
{
    [System.Serializable]
    public class ButtonInputBinding : BaseInputBinding
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<bool> _output = new BoolConstantValue();

        #endregion

        #region Overrides

        protected override void Subscribe(InputAction action)
        {
            action.started += OnStarted;
            action.canceled += OnCanceled;
        }

        protected override void Unsubscribe(InputAction action)
        {
            action.started -= OnStarted;
            action.canceled -= OnCanceled;
        }

        #endregion

        #region Listeners

        private void OnStarted(InputAction.CallbackContext context) => SetOutput(true);

        private void OnCanceled(InputAction.CallbackContext context) => SetOutput(false);

        #endregion

        #region Private

        private void SetOutput(bool isPressed)
        {
            if (_output != null)
                _output.Value = isPressed;
        }

        #endregion
    }
}
