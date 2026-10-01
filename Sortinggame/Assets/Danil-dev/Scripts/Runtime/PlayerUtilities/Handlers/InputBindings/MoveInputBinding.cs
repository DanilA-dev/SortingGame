using D_Dev.PolymorphicValueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace D_Dev.PlayerStateController.InputBindings
{
    [System.Serializable]
    public class MoveInputBinding : BaseInputBinding
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<Vector3> _output = new Vector3ConstantValue();

        #endregion

        #region Overrides

        protected override void Subscribe(InputAction action)
        {
            action.performed += OnMove;
            action.canceled += OnMove;
        }

        protected override void Unsubscribe(InputAction action)
        {
            action.performed -= OnMove;
            action.canceled -= OnMove;
        }

        #endregion

        #region Listeners

        private void OnMove(InputAction.CallbackContext context)
        {
            if (_output == null)
                return;

            var direction = context.ReadValue<Vector2>();
            _output.Value = new Vector3(direction.x, 0f, direction.y);
        }

        #endregion
    }
}
