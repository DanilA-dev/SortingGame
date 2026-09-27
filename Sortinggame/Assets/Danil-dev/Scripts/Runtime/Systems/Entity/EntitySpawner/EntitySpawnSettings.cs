using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.Extensions;
using D_Dev.PolymorphicValueSystem;
using D_Dev.PositionRotationConfig;
using D_Dev.PositionRotationConfig.RotationSettings;
using D_Dev.RuntimeEntityVariables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using D_Dev.EntityPool;

namespace D_Dev.EntitySpawner
{
    [System.Serializable]
    public class EntitySpawnSettings
    {
        #region Fields

        [Title("Data")]
        [SerializeReference] private PolymorphicValue<EntityInfo> _data = new EntityInfoConstantValue();
        [SerializeField] private bool _createOnStart;
        [SerializeReference] private PolymorphicValue<int> _amount = new IntConstantValue() { Value = 1 };
        [SerializeField] private bool _setActiveOnStart;

        [FoldoutGroup("Position and Rotation")]
        [SerializeField] private bool _setParent;
        [ShowIf(nameof(_setParent))]
        [FoldoutGroup("Position and Rotation")]
        [SerializeField] private Transform _parentTransform;
        [FoldoutGroup("Position and Rotation")]
        [SerializeReference] private BasePositionSettings _positionSettings = new();
        [FoldoutGroup("Position and Rotation")]
        [SerializeField] private Vector3 _positionOffset;
        [FoldoutGroup("Position and Rotation")]
        [SerializeReference] private BaseRotationSettings _rotationSettings = new();

        [FoldoutGroup("Pool")]
        [SerializeField] private bool _usePool;
        [FoldoutGroup("Pool")]
        [ShowIf(nameof(_usePool))]
        [SerializeField] private bool _applyPosConfigOnGet;
        [FoldoutGroup("Pool")]
        [ShowIf(nameof(_usePool))]
        [SerializeField] private bool _poolCollectionCheck;
        [FoldoutGroup("Pool")]
        [ShowIf(nameof(_usePool))]
        [SerializeField] private bool _prewarm;
        [FoldoutGroup("Pool")]
        [ShowIf(nameof(ShowPrewarmAmount))]
        [SerializeField] private int _prewarmAmount;
        [FoldoutGroup("Pool")]
        [ShowIf(nameof(_usePool))]
        [SerializeField] private int _poolDefaultCapacity;
        [FoldoutGroup("Pool")]
        [ShowIf(nameof(_usePool))]
        [SerializeField] private int _poolMaxSize;

        [Title("Global Pool")]
        [FoldoutGroup("Pool")]
        [SerializeField] private PoolableDataList _globalPool;

        private ObjectPool<PoolableObject> _pool;
        private List<PoolableObject> _poolableEntities;
        private Queue<PoolableObject> _prewarmQueue;

        #endregion

        #region Properties

        public PolymorphicValue<EntityInfo> Data
        {
            get => _data;
            set => _data = value;
        }

        public bool CreateOnStart
        {
            get => _createOnStart;
            set => _createOnStart = value;
        }

        public PolymorphicValue<int> Amount
        {
            get => _amount;
            set => _amount = value;
        }

        public bool SetActiveOnStart
        {
            get => _setActiveOnStart;
            set => _setActiveOnStart = value;
        }

        public bool UsePool
        {
            get => _usePool;
            set => _usePool = value;
        }

        public bool ApplyPosConfigOnGet
        {
            get => _applyPosConfigOnGet;
            set => _applyPosConfigOnGet = value;
        }

        public bool PoolCollectionCheck
        {
            get => _poolCollectionCheck;
            set => _poolCollectionCheck = value;
        }

        public int PoolDefaultCapacity
        {
            get => _poolDefaultCapacity;
            set => _poolDefaultCapacity = value;
        }

        public int PoolMaxSize
        {
            get => _poolMaxSize;
            set => _poolMaxSize = value;
        }

        public bool Prewarm
        {
            get => _prewarm;
            set => _prewarm = value;
        }

        public int PrewarmAmount
        {
            get => _prewarmAmount;
            set => _prewarmAmount = value;
        }

        public BasePositionSettings PositionSettings
        {
            get => _positionSettings;
            set => _positionSettings = value;
        }

        public BaseRotationSettings RotationSettings
        {
            get => _rotationSettings;
            set => _rotationSettings = value;
        }

        public bool SetParent
        {
            get => _setParent;
            set => _setParent = value;
        }

        public Transform ParentTransform
        {
            get => _parentTransform;
            set => _parentTransform = value;
        }

        public Vector3 PositionOffset
        {
            get => _positionOffset;
            set => _positionOffset = value;
        }

        public List<PoolableObject> PoolableEntities => _poolableEntities;

        public PoolableDataList GlobalPool
        {
            get => _globalPool;
            set => _globalPool = value;
        }

        private bool UseGlobalPool => _globalPool != null;
        private bool ShowPrewarmAmount => _usePool && _prewarm;

        #endregion

        #region Public

        public async UniTask Init()
        {
            _data?.Value?.EntityPrefab?.ResetInstance();

            if (!UseGlobalPool && _usePool)
            {
                _poolableEntities = new List<PoolableObject>();
                _prewarmQueue = new Queue<PoolableObject>();

                if (_prewarm && _prewarmAmount > 0)
                    await PrewarmPoolAsync();

                CreatePool();
            }

            if (_createOnStart)
                await PreCreateEntities();
        }

        public void DisposePool()
        {
            if (UseGlobalPool)
                return;

            if (!_usePool || _pool == null)
                return;

            if (_poolableEntities.Count <= 0)
                return;

            foreach (var poolableEntity in _poolableEntities)
            {
                poolableEntity.OnEntityRelease.RemoveListener(OnPoolableEntityReleased);
                poolableEntity.OnEntityDestroy.RemoveListener(OnPoolableEntityDestroyed);
                _pool.Release(poolableEntity);
            }

            _pool.Dispose();
            _poolableEntities.Clear();
        }

        public async UniTask<GameObject> Get()
        {
            if (UseGlobalPool)
            {
                var obj = await _globalPool.Get(_data.Value);
                if (obj != null && _applyPosConfigOnGet)
                {
                    obj.transform.position = _positionSettings.GetPosition() + _positionOffset;
                    obj.transform.rotation = _rotationSettings.GetRotation();
                }
                return obj;
            }

            GameObject returnObj = null;
            if (_usePool)
            {
                returnObj = _pool?.Get().gameObject;
                if (returnObj != null && _applyPosConfigOnGet)
                {
                    returnObj.transform.position = _positionSettings.GetPosition();
                    returnObj.transform.rotation = _rotationSettings.GetRotation();
                }
            }
            else
                returnObj = await CreateEntityAsync();

            return returnObj;
        }

        public void ReleasePoolObject(PoolableObject poolableObject)
        {
            if (UseGlobalPool)
            {
                poolableObject.gameObject.SetActive(false);
                return;
            }

            if (_usePool)
                _pool?.Release(poolableObject);
        }

        #endregion

        #region Private

        private async UniTask PreCreateEntities()
        {
            for (int i = 0; i < _amount.Value; i++)
                await CreateEntityAsync();
        }

        private async UniTask PrewarmPoolAsync()
        {
            for (int i = 0; i < _prewarmAmount; i++)
            {
                var go = await CreateEntityAsync(forceInactive: true);
                if (go != null && go.TryGetComponent(out PoolableObject poolable))
                    _prewarmQueue.Enqueue(poolable);
            }
        }

        private void CreatePool()
        {
            _pool = new ObjectPool<PoolableObject>(
                createFunc: () =>
                {
                    if (_prewarmQueue != null && _prewarmQueue.Count > 0)
                        return _prewarmQueue.Dequeue();

                    return CreateEntity().GetComponent<PoolableObject>();
                },
                actionOnGet: p =>
                {
                    if (p != null && p.gameObject != null && !p.gameObject.activeInHierarchy)
                        p.gameObject.SetActive(true);

                    p.Get();
                },
                actionOnRelease: p => { if (p != null && p.gameObject != null) p.gameObject.SetActive(false); },
                actionOnDestroy: p => { if (p != null && p.gameObject != null) Object.Destroy(p.gameObject); },
                collectionCheck: _poolCollectionCheck,
                defaultCapacity: _poolDefaultCapacity,
                maxSize: _poolMaxSize
            );
        }

        private async UniTask<GameObject> CreateEntityAsync(bool forceInactive = false)
        {
            var entityAsset = _data.Value.EntityPrefab;
            var obj = await entityAsset.InstantiateAsync();
            var entityTransform = obj.transform;

            if (_setParent && _parentTransform != null)
                entityTransform.SetParent(_parentTransform);

            obj.transform.position = _positionSettings.GetPosition() + _positionOffset;
            obj.transform.rotation = _rotationSettings.GetRotation();
            obj.SetActive(forceInactive ? false : _setActiveOnStart);

            if (obj.TryGetComponent(out RuntimeEntityVariablesContainer runtimeEntityVariablesContainer))
                runtimeEntityVariablesContainer.Init(_data.Value.Variables);

            if (_usePool && obj.TryGetComponent(out PoolableObject poolableEntity))
            {
                poolableEntity.OnEntityRelease.AddListener(OnPoolableEntityReleased);
                poolableEntity.OnEntityDestroy.AddListener(OnPoolableEntityDestroyed);
                _poolableEntities.Add(poolableEntity);
            }

            return obj;
        }

        private GameObject CreateEntity()
        {
            var entityAsset = _data.Value.EntityPrefab;
            var obj = entityAsset.Instantiate();
            var entityTransform = obj.transform;

            if (_setParent && _parentTransform != null)
                entityTransform.SetParent(_parentTransform);

            obj.transform.position = _positionSettings.GetPosition() + _positionOffset;
            obj.transform.rotation = _rotationSettings.GetRotation();
            obj.SetActive(_setActiveOnStart);

            if (obj.TryGetComponent(out RuntimeEntityVariablesContainer runtimeEntityVariablesContainer))
                runtimeEntityVariablesContainer.Init(_data.Value.Variables);

            if (_usePool && obj.TryGetComponent(out PoolableObject poolableEntity))
            {
                poolableEntity.OnEntityRelease.AddListener(OnPoolableEntityReleased);
                poolableEntity.OnEntityDestroy.AddListener(OnPoolableEntityDestroyed);
                _poolableEntities.Add(poolableEntity);
            }

            return obj;
        }

        private void OnPoolableEntityDestroyed(PoolableObject poolableObject)
        {
            poolableObject.OnEntityRelease.RemoveListener(OnPoolableEntityReleased);
            poolableObject.OnEntityDestroy.RemoveListener(OnPoolableEntityDestroyed);
            _pool.Release(poolableObject);
            _poolableEntities.TryRemove(poolableObject);
        }

        private void OnPoolableEntityReleased(PoolableObject poolableObject)
        {
            ReleasePoolObject(poolableObject);
        }

        #endregion
    }
}