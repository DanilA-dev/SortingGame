using D_Dev.InputSystem;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.PlayerStateController
{
    public class PlayerInputProvider : MonoBehaviour
    {
        #region Fields

        [SerializeField] private InputRouter _inputRouter;
        [SerializeReference] private PolymorphicValue<Vector3> _rawInputDirection = new Vector3ConstantValue();

        #endregion

        #region Properties

        public InputRouter InputRouter => _inputRouter;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _inputRouter.Enable();
            _inputRouter.Move += OnMove;
        }

        private void OnDestroy()
        {
            _inputRouter.Move -= OnMove;
        }

        #endregion

        #region Public

        public void DisableInput() => _inputRouter?.Disable();

        #endregion
        
        #region Listeners

        private void OnMove(Vector2 dir) => _rawInputDirection.Value = new Vector3(dir.x, 0f, dir.y);

        #endregion
    }
}   