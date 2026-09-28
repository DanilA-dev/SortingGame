using D_Dev.InputSystem;

namespace D_Dev.PlayerStateController.InputBindings
{
    [System.Serializable]
    public abstract class BaseInputBinding
    {
        #region Abstract

        public abstract void Bind(InputRouter router);
        public abstract void Unbind(InputRouter router);

        #endregion
    }
}
