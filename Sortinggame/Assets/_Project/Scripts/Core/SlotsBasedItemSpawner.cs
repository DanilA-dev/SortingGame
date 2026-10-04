using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using D_Dev.Entity;
using D_Dev.EntitySpawner;
using D_Dev.PolymorphicValueSystem;
using D_Dev.PositionRotationConfig;
using D_Dev.PositionRotationConfig.RotationSettings;
using D_Dev.RuntimeLists;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts
{
    public class SlotsBasedItemSpawner : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _spawnOnStart;
        [SerializeReference] private BasePositionSettings[] _spawnZones = { new Vector3PositionSettings() };
        [SerializeReference] private BaseRotationSettings _spawnRotationSettings = new();
        [SerializeField] private GameObjectRuntimeList _shelvesRuntimeList;
        [SerializeField, Min(1)] private int _maxSettleSteps = 300;
        [SerializeReference] private PolymorphicValue<bool> _isSpawned = new BoolConstantValue();

        private EntitySpawnSettings _entitySpawnSettings;
        private Dictionary<EntityInfo, int> _itemsSpawnAmounts;
        private readonly Dictionary<string, EntityInfo> _itemInfosById = new();
        private readonly List<BasePositionSettings> _validZones = new();
        private readonly List<float> _zonesCumulativeWeights = new();
        private readonly List<GameObject> _spawnedItems = new();
        private bool _isCached;

        #endregion

        #region Properties

        public IReadOnlyList<GameObject> SpawnedItems => _spawnedItems;
        public bool IsSpawned => _isSpawned.Value;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _entitySpawnSettings = new();
            _isSpawned.Value = false;
        }

        private void Start()
        {
            EnsureCached();

            if(_spawnOnStart)
                SpawnItems();
        }

        private void OnValidate()
        {
            ResolveSharedZoneReferences();
        }

        #endregion

        #region Public

        public void SpawnItems()
        {
            SpawnItemsAsync().Forget();
        }

        public UniTask SpawnItemsAsync() => SpawnItemsAsync(CreateRandomItems);

        public async UniTask SpawnItemsAsync(Func<UniTask<List<GameObject>>> createItems)
        {
            EnsureCached();

            var prevSimulationMode = Physics.simulationMode;
            Physics.simulationMode = SimulationMode.Script;

            try
            {
                var items = await createItems();
                SettleItems(items);
            }
            finally
            {
                Physics.simulationMode = prevSimulationMode;
                _isSpawned.Value = true;
            }
        }

        public async UniTask<GameObject> CreateItemAsync(EntityInfo itemInfo)
        {
            EnsureCached();

            if (itemInfo == null)
                return null;

            if (_validZones.Count == 0)
            {
                Debug.LogError($"[SlotsBasedItemSpawner] No spawn zones assigned on {gameObject.name}");
                return null;
            }

            _entitySpawnSettings.Data.Value = itemInfo;
            _entitySpawnSettings.SetActiveOnStart = true;
            _entitySpawnSettings.PositionSettings = GetRandomZone();

            var item = await _entitySpawnSettings.Get();
            if (item == null)
                return null;

            item.transform.rotation = _spawnRotationSettings.GetRotation();
            _spawnedItems.Add(item);
            return item;
        }

        public bool TryGetItemInfo(string id, out EntityInfo itemInfo)
        {
            EnsureCached();

            itemInfo = null;
            return !string.IsNullOrEmpty(id) && _itemInfosById.TryGetValue(id, out itemInfo);
        }

        #endregion

        #region Private

        private void EnsureCached()
        {
            if (_isCached)
                return;

            _isCached = true;
            CacheShelvesItemsData();
            CacheSpawnZones();
        }

        private async UniTask<List<GameObject>> CreateRandomItems()
        {
            var items = new List<GameObject>();

            if(_itemsSpawnAmounts == null || _itemsSpawnAmounts.Count <= 0)
                return items;

            foreach (var (itemData, amount) in _itemsSpawnAmounts)
            {
                for (int i = 0; i < amount; i++)
                {
                    var item = await CreateItemAsync(itemData);
                    if (item != null)
                        items.Add(item);
                }
            }

            return items;
        }

        private void SettleItems(List<GameObject> items)
        {
            if (items == null || items.Count == 0)
                return;

            var bodies = new List<Rigidbody>(items.Count);
            foreach (var item in items)
            {
                if (item.TryGetComponent(out Rigidbody body))
                    bodies.Add(body);
            }

            Physics.SyncTransforms();

            bool isSettled = false;
            for (int step = 0; step < _maxSettleSteps && !isSettled; step++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
                isSettled = bodies.TrueForAll(b => b.IsSleeping());
            }

            if (!isSettled)
                Debug.LogWarning($"[SlotsBasedItemSpawner] Items did not fully settle in {_maxSettleSteps} steps on {gameObject.name}");

            foreach (var item in items)
            {
                if (item.TryGetComponent(out ItemInteractable interactable))
                    interactable.SetSettled();
            }
        }

        private void CacheShelvesItemsData()
        {
            if(_shelvesRuntimeList == null || _shelvesRuntimeList.Count <= 0)
                return;

            _itemsSpawnAmounts = new();
            _itemInfosById.Clear();

            foreach (var item in _shelvesRuntimeList.Items)
            {
                if(item == null)
                    continue;

                if(!item.TryGetComponent(out ItemSlotsContainer container))
                    continue;

                var itemInfo = container.ItemInfo;
                if (itemInfo == null)
                    continue;

                _itemsSpawnAmounts.TryAdd(itemInfo, container.Slots.Length);

                if (!string.IsNullOrEmpty(itemInfo.ID))
                    _itemInfosById.TryAdd(itemInfo.ID, itemInfo);
            }
        }

        private void CacheSpawnZones()
        {
            _validZones.Clear();
            _zonesCumulativeWeights.Clear();

            if (_spawnZones == null)
                return;

            float totalWeight = 0f;
            foreach (var zone in _spawnZones)
            {
                if (zone == null)
                    continue;

                totalWeight += GetZoneWeight(zone);
                _validZones.Add(zone);
                _zonesCumulativeWeights.Add(totalWeight);
            }
        }

        private BasePositionSettings GetRandomZone()
        {
            float roll = Random.Range(0f, _zonesCumulativeWeights[^1]);
            for (int i = 0; i < _validZones.Count; i++)
            {
                if (roll <= _zonesCumulativeWeights[i])
                    return _validZones[i];
            }

            return _validZones[^1];
        }

        private static float GetZoneWeight(BasePositionSettings zone) => zone.RandomMode switch
        {
            RandomPositionMode.Box => Mathf.Max(zone.RandomBoxSize.x * zone.RandomBoxSize.z, 0.01f),
            RandomPositionMode.Sphere => Mathf.Max(Mathf.PI * zone.RandomRadius * zone.RandomRadius, 0.01f),
            _ => 0.01f
        };

        private void ResolveSharedZoneReferences()
        {
            if (_spawnZones == null)
                return;

            for (int i = 1; i < _spawnZones.Length; i++)
            {
                var zone = _spawnZones[i];
                if (zone == null)
                    continue;

                for (int j = 0; j < i; j++)
                {
                    if (!ReferenceEquals(_spawnZones[j], zone))
                        continue;

                    _spawnZones[i] = CreateZoneCopy(zone);
                    break;
                }
            }
        }

        private static BasePositionSettings CreateZoneCopy(BasePositionSettings source)
        {
            var copy = (BasePositionSettings)Activator.CreateInstance(source.GetType());
            copy.RandomMode = source.RandomMode;
            copy.RandomBoxSize = source.RandomBoxSize;
            copy.RandomRadius = source.RandomRadius;
            copy.Axis = source.Axis;
            copy.DrawGizmos = source.DrawGizmos;
            copy.GizmoColor = source.GizmoColor;
            return copy;
        }

        #endregion
    }
}
