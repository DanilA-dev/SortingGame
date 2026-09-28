using System.Collections.Generic;
using D_Dev.InputSystem;
using D_Dev.PlayerStateController.InputBindings;
using UnityEngine;

namespace D_Dev.PlayerStateController
{
    public class PlayerInputProvider : MonoBehaviour
    {
        #region Fields

        [SerializeField] private InputRouter _inputRouter;
        [SerializeReference] private List<BaseInputBinding> _bindings = new() { new MoveInputBinding() };

        #endregion

        #region Properties

        public InputRouter InputRouter => _inputRouter;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (_inputRouter == null)
                return;

            _inputRouter.Enable();

            foreach (var binding in _bindings)
                binding?.Bind(_inputRouter);
        }

        private void OnDestroy()
        {
            if (_inputRouter == null)
                return;

            foreach (var binding in _bindings)
                binding?.Unbind(_inputRouter);
        }

        #endregion

        #region Public

        public void DisableInput() => _inputRouter?.Disable();

        #endregion
    }
}
