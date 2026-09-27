using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

namespace D_Dev.AddressablesExstensions
{
    [System.Serializable]
    public class AddressablesSceneLoadData
    {
        #region Fields

        [SerializeField] private bool _makeAddressable;

        [ShowIf(nameof(_makeAddressable))]
        [SerializeField] private AssetReference _sceneReference;

        private AsyncOperationHandle<SceneInstance>? _loadHandle;

        #endregion

        #region Properties

        public bool IsAddressable => _makeAddressable;
        public bool IsLoaded => _loadHandle.HasValue && _loadHandle.Value.IsValid()
                                && _loadHandle.Value.Result.Scene.isLoaded;

        #endregion

        #region Public

        public async UniTask LoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive)
        {
            if (!_makeAddressable)
            {
                var operation = SceneManager.LoadSceneAsync(sceneName, mode);
                if (operation == null)
                {
                    Debug.LogError($"[AddressablesSceneLoadData] Failed to load scene '{sceneName}'");
                    return;
                }
                operation.allowSceneActivation = false;
                await UniTask.WaitUntil(() => operation.progress >= 0.9f);
                operation.allowSceneActivation = true;
                await operation;
                return;
            }

            if (_sceneReference == null || !_sceneReference.RuntimeKeyIsValid())
            {
                Debug.LogError($"[AddressablesSceneLoadData] SceneReference is not set or invalid for '{sceneName}'");
                return;
            }

            if (_loadHandle.HasValue && _loadHandle.Value.IsValid())
            {
                if (_loadHandle.Value.Result.Scene.isLoaded)
                {
                    Debug.Log($"[AddressablesSceneLoadData] Scene '{sceneName}' is already loaded");
                    return;
                }
                Addressables.Release(_loadHandle.Value);
                _loadHandle = null;
            }

            _loadHandle = Addressables.LoadSceneAsync(_sceneReference, mode);
            await _loadHandle.Value.ToUniTask();
        }

        public async UniTask UnloadAsync(string sceneName)
        {
            if (!_makeAddressable)
            {
                await SceneManager.UnloadSceneAsync(sceneName);
                return;
            }

            if (!_loadHandle.HasValue || !_loadHandle.Value.IsValid())
            {
                Debug.Log($"[AddressablesSceneLoadData] Scene '{sceneName}' is not loaded, nothing to unload");
                return;
            }

            await Addressables.UnloadSceneAsync(_loadHandle.Value).ToUniTask();
            _loadHandle = null;
        }

        #endregion
    }
}