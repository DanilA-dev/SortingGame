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
        [SerializeReference] private BasePositionSettings _spawnPositionSettings = new Vector3PositionSettings();
        [SerializeReference] private BaseRotationSettings _spawnRotationSettings = new();
        [SerializeField] private GameObjectRuntimeList _shelvesRuntimeList;

        private EntitySpawnSettings _entitySpawnSettings;
        private Dictionary<EntityInfo, int> _itemsSpawnAmounts;
            
        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _entitySpawnSettings = new();
        }

        private void Start()
        {
            CacheShelvesItemsData();
            
            if(_spawnOnStart)
                SpawnItems();
        }

        #endregion
        
        #region Public

        public async void SpawnItems()
        {
            if(_itemsSpawnAmounts.Count <= 0)
                return;

            foreach (var (itemData, amount) in _itemsSpawnAmounts)
            {
                _entitySpawnSettings.Data.Value = itemData;
                _entitySpawnSettings.PositionSettings = _spawnPositionSettings;
                _entitySpawnSettings.SetActiveOnStart = true;

                for (int i = 0; i < amount; i++)
                {
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

        #endregion
    }
}