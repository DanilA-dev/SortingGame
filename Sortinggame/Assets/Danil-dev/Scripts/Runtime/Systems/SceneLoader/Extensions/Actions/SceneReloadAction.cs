using D_Dev.Base;
using D_Dev.CustomEventManager;

namespace D_Dev.SceneLoader.Extensions.Actions
{
    [System.Serializable]
    public class SceneReloadAction : BaseAction
    {
        #region Overrides

        public override void Execute()
        {
            EventManager.Invoke(EventNameConstants.SceneReload.ToString());
            IsFinished = true;
        }

        #endregion
    }
}