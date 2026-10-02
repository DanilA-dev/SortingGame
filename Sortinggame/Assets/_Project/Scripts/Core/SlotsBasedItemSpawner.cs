using System.Collections.Generic;
using D_Dev.Entity;
using D_Dev.EntitySpawner;
using D_Dev.PositionRotationConfig;
using D_Dev.PositionRotationConfig.RotationSettings;
using D_Dev.RuntimeLists;
using UnityEngine;

namespace _Project.Scripts
{
    public class SlotsBasedItemSpawner : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _spawnOnStart;
        [SerializeReference] private BasePositionSettings[] _spawnZones = { new Vector3PositionSettings() };
        [SerializeReference] private BaseRotationSettings _spawnRotationSettings = new();
        [SerializeField] private GameObjectRuntimeList _shelvesRuntimeList;

        private EntitySpawnSettings _entitySpawnSettings;
        private Dictionary<EntityInfo, int> _itemsSpawnAmounts;
        private readonly List<BasePositionSettings> _validZones = new();
        private readonly List<float> _zonesCumulativeWeights = new();

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _entitySpawnSettings = new();
        }

        private void Start()
        {
            CacheShelvesItemsData();
            CacheSpawnZones();

            if(_spawnOnStart)
                SpawnItems();
        }

        private void OnValidate()
        {
            ResolveSharedZoneReferences();
        }

        #endregion

        #region Public

        public async void SpawnItems()
        {
            if(_itemsSpawnAmounts == null || _itemsSpawnAmounts.Count <= 0)
                return;

            if (_validZones.Count == 0)
            {
                Debug.LogError($"[SlotsBasedItemSpawner] No spawn zones assigned on {gameObject.name}");
                return;
            }

            foreach (var (itemData, amount) in _itemsSpawnAmounts)
            {
                _entitySpawnSettings.Data.Value = itemData;
                _entitySpawnSettings.SetActiveOnStart = true;

                for (int i = 0; i < amount; i++)
                {
                    _entitySpawnSettings.PositionSettings = GetRandomZone();
                    var item  = await _entitySpawnSettings.Get();
                    item.transform.rotation = _spawnRotationSettings.GetRotation();
                }
            }
        }

        #endregion

        #region Private

        private void CacheShelvesItemsData()
        {
            if(_shelvesRuntimeList == null || _shelvesRuntimeList.Count <= 0)
                return;

            _itemsSpawnAmounts = new();

            foreach (var item in _shelvesRuntimeList.Items)
            {
                if(item == null)
                    continue;

                if(!item.TryGetComponent(out ItemSlotsContainer container))
                    continue;

                _itemsSpawnAmounts.TryAdd(container.ItemInfo, container.Slots.Length);
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
            var copy = (BasePositionSettings)System.Activator.CreateInstance(source.GetType());
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
