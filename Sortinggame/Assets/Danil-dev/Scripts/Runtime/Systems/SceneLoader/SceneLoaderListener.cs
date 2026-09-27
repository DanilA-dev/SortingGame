using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using D_Dev.CustomEventManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace D_Dev.SceneLoader
{
    public class SceneLoaderListener : MonoBehaviour
    {
        #region Fields

        [SerializeField] private UnityEvent _onSceneLoadStart;
        [SerializeField] private UnityEvent _onSceneLoadEnd;
        
        private CancellationTokenSource _tokenSource;

        #endregion

        #region Lifecycle

        private void Awake()
        {
            _tokenSource = new CancellationTokenSource();

            EventManager.AddListener<string>(EventNameConstants.SceneLoad.ToString(), OnSceneLoad);
            EventManager.AddListener(EventNameConstants.SceneReload.ToString(), OnSceneReload);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<string>(EventNameConstants.SceneLoad.ToString(), OnSceneLoad);
            EventManager.RemoveListener(EventNameConstants.SceneReload.ToString(), OnSceneReload);
            _tokenSource?.Cancel();
        }

        #endregion

        #region Private

        private void SetActiveScene(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                SceneManager.SetActiveScene(scene);
                Debug.Log($"[SceneLoader] Set active scene to '{sceneName}'");
                _onSceneLoadEnd?.Invoke();
            }
        }

        private string GetLastActiveUnloadableScene()
        {
            var unloadableScenes = new List<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (SceneLoader.Scenes.TryGetValue(scene.name, out var info) && info.IsUnloadable)
                    unloadableScenes.Add(scene.name);
            }
            return unloadableScenes[^1];
        }

        #endregion

        #region Listeners

        private void OnSceneLoad(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneLoader] Scene name is null or empty");
                return;
            }

            if (SceneLoader.Scenes.TryGetValue(sceneName, out var sceneInfo))
            {
                if (sceneInfo.IsUnloadable)
                {
                    SceneLoader.LoadSceneAsync(sceneName, LoadSceneMode.Additive,
                        onStart: () =>
                        {
                            EventManager.Invoke(EventNameConstants.SceneLoadStart.ToString());
                            _onSceneLoadStart?.Invoke();
                        },
                        onComplete: () => SetActiveScene(sceneName),
                        cancellationToken: _tokenSource.Token).Forget();
                    SceneLoader.UnloadUnloadableScenesExcept(sceneName).Forget();
                }
                else
                {
                    SceneLoader.LoadSceneAsync(sceneName, LoadSceneMode.Additive, cancellationToken: _tokenSource.Token).Forget();
                }
            }
            else
            {
                Debug.LogError($"[SceneLoader] Scene '{sceneName}' not found in config.");
            }
        }

        private void OnSceneReload()
        {
            ReloadSceneAsync().Forget();
        }

        private async UniTaskVoid ReloadSceneAsync()
        {
            var currentScene = GetLastActiveUnloadableScene();
            if (SceneLoader.Scenes.TryGetValue(currentScene, out var sceneInfo) && sceneInfo.IsUnloadable)
            {
                await SceneLoader.UnloadUnloadableScenesExcept(string.Empty,
                    onStart: () => _onSceneLoadStart?.Invoke());

                await SceneLoader.LoadSceneAsync(currentScene, LoadSceneMode.Additive,
                    onComplete: () => SetActiveScene(currentScene),
                    cancellationToken: _tokenSource.Token);
            }
            else
            {
                await SceneLoader.LoadSceneAsync(currentScene, LoadSceneMode.Additive,
                    cancellationToken: _tokenSource.Token);
            }
        }
      
        #endregion
    }
}