using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace D_Dev.AddressablesExstensions
{
    [System.Serializable]
    public class AddressablesGameObjectLoadData : AddressablesAssetLoadData<GameObject>
    {
        #region Fields

        private AsyncOperationHandle<GameObject>? _instantiateHandle;
        private bool _isInstantiated;
        private GameObject _instance;

        #endregion

        #region Public

        public async UniTask<GameObject> InstantiateAsync(Transform parent = null, bool worldPositionStays = true)
        {
            if (!MakeAddressable)
                return Instantiate(parent, worldPositionStays);

            if (_instance != null)
                return _instance;

            if (AssetReference == null)
            {
                Debug.LogError("AssetReference not set for GameObject");
                return null;
            }

            _instantiateHandle = AssetReference.InstantiateAsync(parent, worldPositionStays);
            await _instantiateHandle.Value.ToUniTask();

            _instance = _instantiateHandle.Value.Result;
            return _instance;
        }

        public GameObject Instantiate(Transform parent = null, bool worldPositionStays = true)
        {
            if (_asset == null)
                return null;

            _instance = Object.Instantiate(_asset, parent, worldPositionStays);
            return _instance;
        }
        
        public void ResetInstance()
        {
            _instance = null;
            _instantiateHandle = null;
        }

        #endregion

        #region Overrides

        public override void Release()
        {
            if (_instantiateHandle.HasValue && _instantiateHandle.Value.IsValid())
            {
                Addressables.ReleaseInstance(_instantiateHandle.Value.Result);
                _instantiateHandle = null;
            }

            _instance = null;
            base.Release();
        }

        #endregion
    }
}