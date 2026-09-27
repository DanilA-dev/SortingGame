using D_Dev.Base;
using D_Dev.CustomEventManager;
using D_Dev.MenuHandler;
using UnityEngine;

namespace D_Dev.Scripts.MenuHandler.Extensions.Actions
{
    [System.Serializable]
    public class OpenMenuInfoAction : BaseAction
    {
        #region Fields

        [SerializeField] private MenuInfo _menuInfo;

        #endregion

        #region IAction

        public override void Execute()
        {
            EventManager.Invoke(EventNameConstants.MenuOpen.ToString(), _menuInfo);
            IsFinished = true;
        }

        #endregion
    }
}