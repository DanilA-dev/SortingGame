using UnityEngine;
using UnityEngine.InputSystem;

namespace D_Dev.InputSystem
{
    [CreateAssetMenu(menuName = "D-Dev/InputRouter")]
    public class InputRouter : ScriptableObject
    {
        #region Fields

        [SerializeField] private InputActionAsset _asset;

        #endregion

        #region Properties

        public InputActionAsset Asset => _asset;

        #endregion

        #region Public

        public void Enable()
        {
            if (_asset != null)
                _asset.Enable();
        }

        public void Disable()
        {
            if (_asset != null)
                _asset.Disable();
        }

        public InputAction Resolve(InputActionReference reference)
        {
            if (_asset == null || reference == null || reference.action == null)
                return null;

            return _asset.FindAction(reference.action.id);
        }

        #endregion
    }
}
