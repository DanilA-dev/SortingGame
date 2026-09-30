using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using D_Dev.Entity;
using D_Dev.EntitySpawner;
using D_Dev.PositionRotationConfig;
using UnityEngine;

namespace _Project.Scripts
{
    public class ItemSpawner : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _spawnOnStart;
        [SerializeReference] private BasePositionSettings _spawnPositionSettings = new Vector3PositionSettings();
        [SerializeField] private List<EntityInfo> _itemsInfo = new();

        private EntitySpawnSettings _entitySpawnSettings;
            
        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _entitySpawnSettings = new();
        }

        private void Start()
        {
            if(_spawnOnStart)
                SpawnItems();
        }

        #endregion
        
        #region Public

        public void SpawnItems()
        {
            if(_itemsInfo.Count <= 0)
                return;

            for (var i = 0; i < _itemsInfo.Count; i++)
            {
                _entitySpawnSettings.Data.Value = _itemsInfo[i];
                _entitySpawnSettings.PositionSettings = _spawnPositionSettings;
                _entitySpawnSettings.SetActiveOnStart = true;

                _entitySpawnSettings.Get().Forget();
            }
        }

        #endregion
    }
}