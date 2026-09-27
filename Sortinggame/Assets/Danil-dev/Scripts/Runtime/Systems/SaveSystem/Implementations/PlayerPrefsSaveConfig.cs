using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace D_Dev.SaveSystem
{
    public class PlayerPrefsSaveConfig : ISaveConfig
    {
        #region Public

        public void Save<T>(string key, T value)
        {
            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(value, SaveSerializer.Settings));
            PlayerPrefs.Save();
        }

        public UniTask SaveAsync<T>(string key, T value)
        {
            Save(key, value);
            return UniTask.CompletedTask;
        }

        public UniTask<T> LoadAsync<T>(string key, T defaultValue = default)
        {
            if (!PlayerPrefs.HasKey(key))
                return UniTask.FromResult(defaultValue);

            var result = JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(key), SaveSerializer.Settings);
            return UniTask.FromResult(result);
        }

        public UniTask<bool> HasKeyAsync(string key)
            => UniTask.FromResult(PlayerPrefs.HasKey(key));

        public UniTask DeleteKeyAsync(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }

        public UniTask DeleteAllAsync()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            return UniTask.CompletedTask;
        }

        #endregion
    }
}