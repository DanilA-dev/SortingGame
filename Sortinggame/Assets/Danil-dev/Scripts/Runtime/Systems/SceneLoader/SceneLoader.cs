using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace D_Dev.SceneLoader
{
    public static class SceneLoader
    {
        #region Fields

        public static Dictionary<string, SceneInfo> Scenes;

        #endregion

        #region Public

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static UniTask InitializeScenesAsync()
        {
            Scenes = new();
            var path = "ProjectScenesConfig";
            var projectScenes = Resources.Load<ProjectScenesConfig>(path);
            if (projectScenes != null)
            {
                if (projectScenes.Scenes.Count <= 0)
                    return UniTask.CompletedTask;

                foreach (var scene in projectScenes.Scenes)
                {
                    Scenes[scene.SceneName] = scene;
                    if (scene.AddSceneOnStartup)
                        LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive).Forget();
                }
            }
            else
                Debug.LogError($"[SceneLoader] Failed to load project scenes config at path: Resources/{path}");

            return UniTask.CompletedTask;
        }

        public static async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single,
            Action onStart = null, Action onComplete = null, CancellationToken cancellationToken = default)
        {
            onStart?.Invoke();

            if (Scenes.TryGetValue(sceneName, out var sceneInfo) && sceneInfo.SceneLoadData != null)
            {
                await sceneInfo.SceneLoadData.LoadAsync(sceneName, mode).AttachExternalCancellation(cancellationToken);
                Debug.Log($"[SceneLoader] Scene '{sceneName}' loaded successfully");
                onComplete?.Invoke();
                return;
            }

            var operation = SceneManager.LoadSceneAsync(sceneName, mode);
            if (operation == null)
            {
                Debug.LogError($"[SceneLoader] Failed to start loading scene: {sceneName}");
                return;
            }

            operation.allowSceneActivation = false;
            await UniTask.WaitUntil(() => operation.progress >= 0.9f, cancellationToken: cancellationToken);
            operation.allowSceneActivation = true;
            await operation;

            Debug.Log($"[SceneLoader] Scene '{sceneName}' loaded successfully");
            onComplete?.Invoke();
        }

        public static async UniTask UnloadUnloadableScenesExcept(string exceptScene,
            Action onStart = null, Action onComplete = null)
        {
            var toUnload = new List<SceneInfo>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (Scenes.TryGetValue(scene.name, out var info) && info.IsUnloadable && scene.name != exceptScene)
                    toUnload.Add(info);
            }

            onStart?.Invoke();

            foreach (var info in toUnload)
            {
                if (!SceneManager.GetSceneByName(info.SceneName).isLoaded) continue;

                if (info.SceneLoadData != null && info.SceneLoadData.IsAddressable)
                {
                    await info.SceneLoadData.UnloadAsync(info.SceneName);
                }
                else
                {
                    var op = SceneManager.UnloadSceneAsync(info.SceneName);
                    if (op != null) await op;
                }

                Debug.Log($"[SceneLoader] Unloaded scene '{info.SceneName}'");
            }

            onComplete?.Invoke();
        }

        #endregion
    }
}