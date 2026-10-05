using Cysharp.Threading.Tasks;
using D_Dev.SaveSystem;
using Newtonsoft.Json;
using UnityEngine;
using YG;

namespace _Project.Scripts.SDK
{
    public class YG2SaveConfig : ISaveConfig
    {
        #region Public

        public void Save<T>(string key, T value)
        {
            if (!YG2.isSDKEnabled)
                Debug.LogWarning($"[YG2SaveConfig] Save '{key}' called before SDK init, data may be overwritten by cloud load");

            SetEntry(key, JsonConvert.SerializeObject(value, Formatting.None, SaveSerializer.Settings));
            Flush();
        }

        public async UniTask SaveAsync<T>(string key, T value)
        {
            await WaitForSDK();
            Save(key, value);
        }

        public async UniTask<T> LoadAsync<T>(string key, T defaultValue = default)
        {
            await WaitForSDK();
            SaveEntryYG entry = FindEntry(key);
            return entry == null
                ? defaultValue
                : JsonConvert.DeserializeObject<T>(entry.value, SaveSerializer.Settings);
        }

        public async UniTask<bool> HasKeyAsync(string key)
        {
            await WaitForSDK();
            return FindEntry(key) != null;
        }

        public async UniTask DeleteKeyAsync(string key)
        {
            await WaitForSDK();
            if (YG2.saves.entries.RemoveAll(e => e.key == key) > 0)
                Flush();
        }

        public async UniTask DeleteAllAsync()
        {
            await WaitForSDK();
            YG2.saves.entries.Clear();
            Flush();
        }

        #endregion

        #region Private

        private static UniTask WaitForSDK()
            => YG2.isSDKEnabled ? UniTask.CompletedTask : UniTask.WaitUntil(() => YG2.isSDKEnabled);

        private static SaveEntryYG FindEntry(string key)
            => YG2.saves.entries.Find(e => e.key == key);

        private static void SetEntry(string key, string json)
        {
            SaveEntryYG entry = FindEntry(key);
            if (entry != null)
                entry.value = json;
            else
                YG2.saves.entries.Add(new SaveEntryYG { key = key, value = json });
        }

        private static void Flush()
        {
            if (YG2.isSDKEnabled)
                YG2.SaveProgress();
        }

        #endregion
    }
}
