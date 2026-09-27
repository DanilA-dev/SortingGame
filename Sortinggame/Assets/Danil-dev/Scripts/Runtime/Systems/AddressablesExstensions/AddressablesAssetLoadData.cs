using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
#endif
using UnityEngine.ResourceManagement.AsyncOperations;

namespace D_Dev.AddressablesExstensions
{
    [System.Serializable]
    public class AddressablesAssetLoadData<T> where T : Object
    {
        #region Fields

        [OnValueChanged(nameof(OnUseAddressablesChanged))]
        [SerializeField] protected bool _makeAddressable;
        [HideIf(nameof(_makeAddressable))]
        [OnValueChanged(nameof(OnAssetFieldUpdate))]
        [SerializeField] protected T _asset;
        [ShowIf(nameof(_makeAddressable))]
        [SerializeField] protected AssetReference _assetReference;

        protected AsyncOperationHandle<T>? _loadHandle;
        protected bool _isLoaded = false;

        #endregion

        #region Properties

        public bool MakeAddressable => _makeAddressable;
        public T Asset => _asset;
        public AssetReference AssetReference => _assetReference;

        #endregion

        #region Public

        public async UniTask<T> LoadAsync()
        {
            if (!_makeAddressable)
                return _asset;

            if (_assetReference == null)
            {
                Debug.LogError($"AssetReference not set for {typeof(T)}");
                return null;
            }

            if (_loadHandle.HasValue && _loadHandle.Value.IsValid())
                return _loadHandle.Value.Result;

            _loadHandle = _assetReference.LoadAssetAsync<T>();
            await _loadHandle.Value.ToUniTask();

            return _loadHandle.Value.Result;
        }

        public virtual void Release()
        {
            if (_loadHandle.HasValue && _loadHandle.Value.IsValid())
            {
                Addressables.Release(_loadHandle.Value);
                _loadHandle = null;
            }
        }
        
        #endregion

        #region Private

        private void OnUseAddressablesChanged()
        {
#if UNITY_EDITOR
            if (_makeAddressable)
            {
                if (_asset != null)
                {
                    var settings = AddressableAssetSettingsDefaultObject.Settings;
                    string assetPath = AssetDatabase.GetAssetPath(_asset);
                    string guid = AssetDatabase.AssetPathToGUID(assetPath);
                    if (settings != null)
                    {
                        
                        if (settings.FindAssetEntry(guid) == null)
                        {
                            settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    _assetReference = new AssetReference(guid);
                    _asset = null;
                }
            }
            else
            {
                if (_assetReference != null && _assetReference.RuntimeKeyIsValid())
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(_assetReference.AssetGUID);
                    _asset = (T) AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
                    _assetReference = null;
                }
            }
#endif
        }

        #endregion

        #region Protected

        protected virtual void OnAssetFieldUpdate() {}

        #endregion
    }
}