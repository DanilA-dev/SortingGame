using Cysharp.Threading.Tasks;
using D_Dev.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.SaveSystem.Services
{
    public class GlobalSaveService : BaseSingleton<GlobalSaveService>
    {
        #region Fields

        [SerializeReference] private ISaveConfig _saveConfig;
        [SerializeField] private bool _useSeparateConfigForEditor;
        [ShowIf(nameof(_useSeparateConfigForEditor))]
        [SerializeReference] private ISaveConfig _editorSaveConfig;

        #endregion

        #region Monobehaviour

        protected override void Awake()
        {
            base.Awake();

            if (_saveConfig == null)
            {
                _saveConfig = new PlayerPrefsSaveConfig();
                Debug.Log("[SaveService] Using default PlayerPrefsSaveConfig");
            }

            if (_useSeparateConfigForEditor && _editorSaveConfig != null)
            {
                _saveConfig = _editorSaveConfig;
                Debug.Log("[SaveService] Using editor save config");
            }
        }

        #endregion

        #region Public

        public void Save<T>(string key, T value) => _saveConfig.Save(key, value);
        public UniTask SaveAsync<T>(string key, T value) => _saveConfig.SaveAsync(key, value);
        public UniTask<T> LoadAsync<T>(string key, T defaultValue = default) => _saveConfig.LoadAsync(key, defaultValue);
        public UniTask<bool> HasKeyAsync(string key) => _saveConfig.HasKeyAsync(key);
        public UniTask DeleteKeyAsync(string key) => _saveConfig.DeleteKeyAsync(key);
        public UniTask DeleteAllAsync() => _saveConfig.DeleteAllAsync();

        #endregion
    }
}
