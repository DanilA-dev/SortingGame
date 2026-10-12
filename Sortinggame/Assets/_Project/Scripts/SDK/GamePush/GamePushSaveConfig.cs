using System;
using Cysharp.Threading.Tasks;
using D_Dev.SaveSystem;
using GamePush;
using Newtonsoft.Json;
using UnityEngine;

namespace _Project.Scripts.SDK
{
    [Serializable]
    public class GamePushSaveConfig : ISaveConfig
    {
        #region Fields

        [Tooltip("Storage used by GP_Player.Sync. Preferred uses the storage selected in the GamePush panel.")]
        [SerializeField] private SyncStorageType _storage = SyncStorageType.preferred;

        private bool _isDirty;
        private bool _isSyncScheduled;

        #endregion

        #region Public

        public void Save<T>(string key, T value)
        {
            if (!GP_Init.isReady)
                Debug.LogWarning($"[GamePushSaveConfig] Save '{key}' called before SDK init");

            SetField(key, Serialize(value));
            Sync();
        }

        public async UniTask SaveAsync<T>(string key, T value)
        {
            await WaitForSDK();
            SetField(key, Serialize(value));
            ScheduleSync();
        }

        public async UniTask<T> LoadAsync<T>(string key, T defaultValue = default)
        {
            await WaitForSDK();

            var json = GetField(key);
            if (string.IsNullOrEmpty(json))
                return defaultValue;

            try
            {
                return JsonConvert.DeserializeObject<T>(json, SaveSerializer.Settings);
            }
            catch (JsonException e)
            {
                Debug.LogError($"[GamePushSaveConfig] Failed to parse '{key}': {e.Message}");
                return defaultValue;
            }
        }

        public async UniTask<bool> HasKeyAsync(string key)
        {
            await WaitForSDK();
            return !string.IsNullOrEmpty(GetField(key));
        }

        public async UniTask DeleteKeyAsync(string key)
        {
            await WaitForSDK();
            SetField(key, string.Empty);
            ScheduleSync();
        }

        public async UniTask DeleteAllAsync()
        {
            await WaitForSDK();
            GP_Player.ResetPlayer();
            _isDirty = true;
            ScheduleSync();
        }

        #endregion

        #region Private

        private static UniTask WaitForSDK()
            => GP_Init.isReady ? UniTask.CompletedTask : UniTask.WaitUntil(() => GP_Init.isReady);

        private static string Serialize<T>(T value)
            => JsonConvert.SerializeObject(value, Formatting.None, SaveSerializer.Settings);

        private static string GetField(string key)
        {
#if UNITY_EDITOR
            if (!GamePush.Native.GamePushHost.UseNativeCore && !GP_Play2Web.Enabled && !GP_Prefs.HasKey(key))
                return string.Empty;
#endif
            return GP_Player.GetString(key);
        }

        private void SetField(string key, string json)
        {
            if (GetField(key) == json)
                return;

            GP_Player.Set(key, json);
            _isDirty = true;
        }

        private void ScheduleSync()
        {
            if (_isSyncScheduled || !_isDirty)
                return;

            _isSyncScheduled = true;
            SyncAtEndOfFrame().Forget();
        }

        private async UniTaskVoid SyncAtEndOfFrame()
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            _isSyncScheduled = false;
            Sync();
        }

        private void Sync()
        {
            if (!_isDirty || !GP_Init.isReady)
                return;

            _isDirty = false;
            GP_Player.Sync(_storage);
        }

        #endregion
    }
}
