using D_Dev.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace D_Dev.PlayerStateController.InputBindings
{
    [System.Serializable]
    public abstract class BaseInputBinding
    {
        #region Fields

        [SerializeField] private InputActionReference _action;

        private InputAction _resolved;

        #endregion

        #region Public

        public void Bind(InputRouter router)
        {
            _resolved = router.Resolve(_action);
            if (_resolved != null)
                Subscribe(_resolved);
        }

        public void Unbind()
        {
            if (_resolved == null)
                return;

            Unsubscribe(_resolved);
            _resolved = null;
        }

        #endregion

        #region Abstract

        protected abstract void Subscribe(InputAction action);
        protected abstract void Unsubscribe(InputAction action);

        #endregion
    }
}
