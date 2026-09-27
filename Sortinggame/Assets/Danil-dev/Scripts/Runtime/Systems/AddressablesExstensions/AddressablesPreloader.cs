using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace D_Dev.AddressablesExstensions
{
    public class AddressablesPreloader : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _preloadOnAwake;
        [SerializeField] private AssetReference[] _assetReferences;
        [SerializeField] private AssetLabelReference[] _labelReferences;

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onPreloadComplete;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onPreloadFailed;

        private bool _preloadOnAwakeCalled;
        private bool _isPreloading;
        private readonly List<AsyncOperationHandle> _handles = new();

        #endregion

        #region Properties

        public bool IsLoaded { get; private set; }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if (_preloadOnAwake && !_preloadOnAwakeCalled)
            {
                _preloadOnAwakeCalled = true;
                PreloadAsync().Forget();
            }
        }

        private void OnDestroy()
        {
            foreach (var handle in _handles)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }
            _handles.Clear();
        }

        #endregion

        #region Public

        public void Preload() => PreloadAsync().Forget();

        public async UniTask PreloadAsync()
        {
            if (_isPreloading)
            {
                Debug.Log($"[AddressablesPreloader] Preload already in progress on '{name}'", this);
                return;
            }

            _isPreloading = true;

            foreach (var old in _handles)
            {
                if (old.IsValid())
                    Addressables.Release(old);
            }
            _handles.Clear();

            var tasks = new List<UniTask>();

            foreach (var assetReference in _assetReferences)
            {
                if (assetReference == null || string.IsNullOrEmpty(assetReference.AssetGUID))
                    continue;

                var handle = assetReference.LoadAssetAsync<Object>();
                _handles.Add(handle);
                tasks.Add(handle.ToUniTask());
            }

            foreach (var label in _labelReferences)
            {
                if (label == null || string.IsNullOrEmpty(label.labelString))
                    continue;

                var handle = Addressables.LoadAssetsAsync<Object>(label.labelString, null);
                _handles.Add(handle);
                tasks.Add(handle.ToUniTask());
            }

            bool success = true;

            try
            {
                await UniTask.WhenAll(tasks);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[AddressablesPreloader] Failed to preload assets on '{name}': {e.Message}", this);
                success = false;
            }
            finally
            {
                _isPreloading = false;
            }

            IsLoaded = true;

            if (success)
                _onPreloadComplete?.Invoke();
            else
                _onPreloadFailed?.Invoke();
        }

        #endregion
    }
}