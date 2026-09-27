using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace D_Dev.SaveSystem
{
    public class JsonFileSaveConfig : ISaveConfig
    {
        #region Public

        public void Save<T>(string key, T value)
        {
            string path = GetPath(key);
            string json = JsonConvert.SerializeObject(value, SaveSerializer.Settings);
            WriteAtomic(key, path, json);
        }

        public async UniTask SaveAsync<T>(string key, T value)
        {
            string path = GetPath(key);
            string json = JsonConvert.SerializeObject(value, SaveSerializer.Settings);

            await UniTask.RunOnThreadPool(() => WriteAtomic(key, path, json));
        }

        public async UniTask<T> LoadAsync<T>(string key, T defaultValue = default)
        {
            string path = GetPath(key);
            if (!File.Exists(path))
                return defaultValue;

            try
            {
                string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(path));
                return JsonConvert.DeserializeObject<T>(json, SaveSerializer.Settings);
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonFileSaveConfig] Load failed for key '{key}' - {e}");
                return defaultValue;
            }
        }

        public UniTask<bool> HasKeyAsync(string key)
            => UniTask.FromResult(File.Exists(GetPath(key)));

        public UniTask DeleteKeyAsync(string key)
        {
            try
            {
                string path = GetPath(key);
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonFileSaveConfig] Delete failed for key '{key}' - {e}");
            }

            return UniTask.CompletedTask;
        }

        public UniTask DeleteAllAsync()
        {
            try
            {
                var files = Directory.GetFiles(Application.persistentDataPath, "*.json");
                foreach (var file in files)
                    File.Delete(file);
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonFileSaveConfig] Delete all failed - {e}");
            }

            return UniTask.CompletedTask;
        }

        #endregion

        #region Private

        private void WriteAtomic(string key, string path, string json)
        {
            try
            {
                string tempPath = path + ".tmp";
                File.WriteAllText(tempPath, json);

                if (File.Exists(path))
                    File.Delete(path);

                File.Move(tempPath, path);
            }
            catch (Exception e)
            {
                Debug.LogError($"[JsonFileSaveConfig] Save failed for key '{key}' - {e}");
            }
        }

        private string GetPath(string key)
            => Path.Combine(Application.persistentDataPath, $"{key}.json");

        #endregion
    }
}
