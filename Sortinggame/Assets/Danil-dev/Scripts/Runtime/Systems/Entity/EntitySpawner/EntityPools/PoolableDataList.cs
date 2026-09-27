using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using D_Dev.Entity;
using D_Dev.Entity.Extensions;
using D_Dev.EntitySpawner;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.EntityPool
{
    public abstract class PoolableDataList : ScriptableObject
    {
        #region Classes

        [System.Serializable]
        public class PoolConfig
        {
            public EntityInfo EntityData;
            public int MaxSize;
            public int DefaultCapacity;
            public bool Prewarm;
            [ShowIf(nameof(Prewarm))]
            public int PrewarmAmount;
        }

        #endregion

        #region Fields

        [SerializeField] private PoolConfig[] _poolConfigs;

        #endregion

        #region Properties

        public IReadOnlyDictionary<EntityInfo, EntitySpawnSettings> Pools => _pools;
        private Dictionary<EntityInfo, EntitySpawnSettings> _pools;

        #endregion

        #region Public

        public void Initialize()
        {
            _pools = new Dictionary<EntityInfo, EntitySpawnSettings>();

            if (_poolConfigs == null || _poolConfigs.Length == 0)
                return;

            foreach (var config in _poolConfigs)
                InitPool(config);
        }

        public void Dispose()
        {
            if (_pools == null)
                return;

            foreach (var kvp in _pools)
                kvp.Value.DisposePool();

            _pools.Clear();
        }

        public async UniTask<GameObject> Get(EntityInfo entityInfo)
        {
            if (!_pools.TryGetValue(entityInfo, out var spawnSettings))
            {
                Debug.LogError($"[PoolableDataList] No pool registered for: {entityInfo}");
                return null;
            }

            return await spawnSettings.Get();
        }

        public async UniTask<T> Get<T>(EntityInfo entityInfo) where T : Component
        {
            var obj = await Get(entityInfo);
            if (obj == null) return null;

            if (!obj.TryGetComponent<T>(out var component))
            {
                Debug.LogError($"[PoolableDataList] Component {typeof(T).Name} not found on pooled object: {obj.name}");
                return null;
            }

            return component;
        }

        public bool HasPool(EntityInfo entityInfo) => _pools.ContainsKey(entityInfo);

        public void ReleaseAll()
        {
            foreach (var keyValuePair in _pools)
                foreach (var valuePoolableEntity in keyValuePair.Value.PoolableEntities)
                    valuePoolableEntity.Release();
        }

        #endregion

        #region Private

        private async void InitPool(PoolConfig config)
        {
            var spawnSettings = new EntitySpawnSettings
            {
                Data = new EntityInfoConstantValue { Value = config.EntityData },
                UsePool = true,
                SetActiveOnStart = true,
                PoolMaxSize = config.MaxSize,
                PoolDefaultCapacity = config.DefaultCapacity,
                Prewarm = config.Prewarm,
                PrewarmAmount = config.PrewarmAmount
            };

            await spawnSettings.Init();
            _pools.Add(config.EntityData, spawnSettings);
        }

        #endregion
    }
}