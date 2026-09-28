using D_Dev.InputSystem;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.PlayerStateController.InputBindings
{
    [System.Serializable]
    public class MoveInputBinding : BaseInputBinding
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<Vector3> _output = new Vector3ConstantValue();

        #endregion

        #region Overrides

        public override void Bind(InputRouter router) => router.Move += OnMove;

        public override void Unbind(InputRouter router) => router.Move -= OnMove;

        #endregion

        #region Listeners

        private void OnMove(Vector2 direction)
        {
            if (_output != null)
                _output.Value = new Vector3(direction.x, 0f, direction.y);
        }

        #endregion
    }
}
