using D_Dev.Base;
using D_Dev.CustomEventManager;
using UnityEngine;

namespace D_Dev.SceneLoader.Extensions.Actions
{
    [System.Serializable]
    public class SceneInfoLoadAction : BaseAction
    {
        #region Fields

        [SerializeField] private SceneInfo _sceneInfo;

        #endregion

        #region Overrides

        public override void Execute()
        {
            EventManager.Invoke(EventNameConstants.SceneLoad.ToString(), _sceneInfo.SceneName);
            IsFinished = true;
        }

        #endregion
    }
}