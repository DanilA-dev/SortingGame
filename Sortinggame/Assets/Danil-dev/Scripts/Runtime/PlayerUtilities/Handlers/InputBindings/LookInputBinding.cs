using D_Dev.PolymorphicValueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace D_Dev.PlayerStateController.InputBindings
{
    [System.Serializable]
    public class LookInputBinding : BaseInputBinding
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<Vector2> _output = new Vector2ConstantValue();

        #endregion

        #region Overrides

        protected override void Subscribe(InputAction action)
        {
            action.performed += OnLook;
            action.canceled += OnLook;
        }

        protected override void Unsubscribe(InputAction action)
        {
            action.performed -= OnLook;
            action.canceled -= OnLook;
        }

        #endregion

        #region Listeners

        private void OnLook(InputAction.CallbackContext context)
        {
            if (_output != null)
                _output.Value = context.ReadValue<Vector2>();
        }

        #endregion
    }
}
