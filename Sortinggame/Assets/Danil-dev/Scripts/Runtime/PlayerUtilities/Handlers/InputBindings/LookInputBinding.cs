using D_Dev.InputSystem;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.PlayerStateController.InputBindings
{
    [System.Serializable]
    public class LookInputBinding : BaseInputBinding
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<Vector2> _output = new Vector2ConstantValue();

        #endregion

        #region Overrides

        public override void Bind(InputRouter router) => router.Look += OnLook;

        public override void Unbind(InputRouter router) => router.Look -= OnLook;

        #endregion

        #region Listeners

        private void OnLook(Vector2 delta)
        {
            if (_output != null)
                _output.Value = delta;
        }

        #endregion
    }
}
