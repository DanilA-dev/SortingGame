using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using D_Dev.Entity;
using UnityEngine;

namespace D_Dev.EntitySpawner
{
    public class EntitiesSpawner : MonoBehaviour
    {
        #region Fields

        [SerializeField] private EntitySpawnSettings[] _spawnSettings;

        #endregion

        #region Monobehaviour

        private async void Start()
        {
            if(_spawnSettings.Length <= 0)
                return;

            foreach (var entitySpawnSettings in _spawnSettings)
                await entitySpawnSettings.Init();
        }

        private void OnDisable()
        {
            if(_spawnSettings.Length <= 0)
                return;
            
            foreach (var entitySpawnSettings in _spawnSettings)
                entitySpawnSettings.DisposePool();
        }

        #endregion

        #region Public

        public async void CreateEntityAsync(int settingsIndex) => await GetEntitiesAsync(settingsIndex);
        public async void CreateEntityAsync(EntityInfo data) => await GetEntitiesAsync(data);

        public async void SpawnAll() => await SpawnAllAsync();


        #endregion

        #region Private

        private async UniTask<List<GameObject>> SpawnAllAsync()
        {
            var allResults = new List<GameObject>();
            foreach (var settings in _spawnSettings)
                for (int i = 0; i < settings.Amount.Value; i++)
                    allResults.Add(await settings.Get());
            return allResults;
        }
        
        public async UniTask<List<GameObject>> GetEntitiesAsync(EntityInfo data)                  
        {                                                 
            var spawnSettings = _spawnSettings.           
                FirstOrDefault(s => s.Data.Value == data);        
            var results = new List<GameObject>();         
                                                       
            if (spawnSettings != null)                    
                for (int i = 0; i < spawnSettings.        
                         Amount.Value; i++)                                
                    results.Add(await spawnSettings.Get());       
                                                       
            return results;                               
        }               

        public async UniTask<List<GameObject>> GetEntitiesAsync(int index)
        {
            var spawnSettings = _spawnSettings[index];
            var results = new List<GameObject>();         
                                                       
            if (spawnSettings != null)                    
                for (int i = 0; i < spawnSettings.        
                         Amount.Value; i++)                                
                    results.Add(await spawnSettings.Get());       
                                                       
            return results;                               
        }               

        #endregion
    }
}
