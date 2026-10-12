using System;
using Cysharp.Threading.Tasks;
using D_Dev.SaveSystem;
using GamePush;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.SDK
{
    [Serializable]
    public class GamePushSaveConfig : ISaveConfig
    {
        #region Fields

        [Tooltip("Storage used by GP_Player.Sync. Preferred uses the storage selected in the GamePush panel.")]
        [SerializeField] private SyncStorageType _storage = SyncStorageType.preferred;
        [Tooltip("Minimum time between cloud syncs triggered by async saves. Immediate saves sync at the end of the frame.")]
        [SuffixLabel("sec")]
        [SerializeField, Min(0f)] private float _minSyncInterval = 60f;

        private bool _isDirty;
        private bool _isFlushScheduled;
        private bool _isTimedSyncScheduled;
        private float _lastSyncTime;

        #endregion

        #region Public

        public void Save<T>(string key, T value)
        {
            if (!GP_Init.isReady)
                Debug.LogWarning($"[GamePushSaveConfig] Save '{key}' called before SDK init");

            SetField(key, Serialize(value));
            ScheduleFlush();
        }

        public async UniTask SaveAsync<T>(string key, T value)
        {
            await WaitForSDK();
            SetField(key, Serialize(value));
            ScheduleTimedSync();
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
            ScheduleFlush();
        }

        public async UniTask DeleteAllAsync()
        {
            await WaitForSDK();
            GP_Player.ResetPlayer();
            _isDirty = true;
            ScheduleFlush();
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

        private void ScheduleFlush()
        {
            if (_isFlushScheduled || !_isDirty)
                return;

            _isFlushScheduled = true;
            FlushAtEndOfFrame().Forget();
        }

        private async UniTaskVoid FlushAtEndOfFrame()
        {
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            _isFlushScheduled = false;
            Sync();
        }

        private void ScheduleTimedSync()
        {
            if (_isTimedSyncScheduled || !_isDirty)
                return;

            _isTimedSyncScheduled = true;
            TimedSync().Forget();
        }

        private async UniTaskVoid TimedSync()
        {
            float wait;
            while ((wait = _lastSyncTime + _minSyncInterval - Time.unscaledTime) > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(wait), DelayType.UnscaledDeltaTime);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            _isTimedSyncScheduled = false;
            Sync();
        }

        private void Sync()
        {
            if (!_isDirty || !GP_Init.isReady)
                return;

            _isDirty = false;
            _lastSyncTime = Time.unscaledTime;
            GP_Player.Sync(_storage);
        }

        #endregion
    }
}
