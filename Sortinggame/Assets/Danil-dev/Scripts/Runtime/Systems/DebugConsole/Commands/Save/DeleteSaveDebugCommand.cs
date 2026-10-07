using System;
using Cysharp.Threading.Tasks;
using D_Dev.CustomEventManager;
using D_Dev.SaveSystem.SaveableData;
using D_Dev.SaveSystem.Services;
using UnityEngine;
using Object = UnityEngine.Object;

namespace D_Dev.DebugConsole
{
    [Serializable]
    public class DeleteSaveDebugCommand : BaseDebugCommand
    {
        #region Fields

        [SerializeField] private bool _reloadScene = true;

        #endregion

        #region Overrides

        protected override Delegate GetMethod() => (Action)Execute;

        #endregion

        #region Private

        private void Execute() => ExecuteAsync().Forget();

        private async UniTaskVoid ExecuteAsync()
        {
            foreach (var handler in Object.FindObjectsByType<SaveableDataHandler>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                handler.ResetAll();

            if (GlobalSaveService.Instance != null)
                await GlobalSaveService.Instance.DeleteAllAsync();

            Debug.Log("[DeleteSaveDebugCommand] Save deleted");

            if (_reloadScene)
                EventManager.Invoke(EventNameConstants.SceneReload.ToString());
        }

        #endregion
    }
}
