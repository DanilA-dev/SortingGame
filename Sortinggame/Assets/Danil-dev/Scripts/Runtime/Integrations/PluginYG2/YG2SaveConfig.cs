#if D_DEV_YG2 && Storage_yg
using System;
using Cysharp.Threading.Tasks;
using D_Dev.SaveSystem;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using YG;

namespace D_Dev.PluginYG2
{
    [Serializable]
    public class YG2SaveConfig : ISaveConfig
    {
        #region Fields

        [Tooltip("Minimum time between cloud saves triggered by async saves. Immediate saves are sent at the end of the frame.")]
        [SuffixLabel("sec")]
        [SerializeField, Min(0f)] private float _minSaveInterval = 60f;

        private bool _isDirty;
        private bool _isFlushScheduled;
        private bool _isTimedFlushScheduled;
        private float _lastFlushTime;

        #endregion

        #region Public

        public void Save<T>(string key, T value)
        {
            if (!YG2.isSDKEnabled)
                Debug.LogWarning($"[YG2SaveConfig] Save '{key}' called before SDK init, data may be overwritten by cloud load");

            SetEntry(key, Serialize(value));
            ScheduleFlush();
        }

        public async UniTask SaveAsync<T>(string key, T value)
        {
            await WaitForSDK();
            SetEntry(key, Serialize(value));
            ScheduleTimedFlush();
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
            if (YG2.saves.entries.RemoveAll(e => e.key == key) <= 0)
                return;

            _isDirty = true;
            ScheduleFlush();
        }

        public async UniTask DeleteAllAsync()
        {
            await WaitForSDK();
            YG2.saves.entries.Clear();
            _isDirty = true;
            ScheduleFlush();
        }

        #endregion

        #region Private

        private static UniTask WaitForSDK()
            => YG2.isSDKEnabled ? UniTask.CompletedTask : UniTask.WaitUntil(() => YG2.isSDKEnabled);

        private static string Serialize<T>(T value)
            => JsonConvert.SerializeObject(value, Formatting.None, SaveSerializer.Settings);

        private static SaveEntryYG FindEntry(string key)
            => YG2.saves.entries.Find(e => e.key == key);

        private void SetEntry(string key, string json)
        {
            SaveEntryYG entry = FindEntry(key);
            if (entry == null)
            {
                YG2.saves.entries.Add(new SaveEntryYG { key = key, value = json });
                _isDirty = true;
                return;
            }

            if (entry.value == json)
                return;

            entry.value = json;
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
            Flush();
        }

        private void ScheduleTimedFlush()
        {
            if (_isTimedFlushScheduled || !_isDirty)
                return;

            _isTimedFlushScheduled = true;
            TimedFlush().Forget();
        }

        private async UniTaskVoid TimedFlush()
        {
            float wait;
            while ((wait = _lastFlushTime + _minSaveInterval - Time.unscaledTime) > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(wait), DelayType.UnscaledDeltaTime);

            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            _isTimedFlushScheduled = false;
            Flush();
        }

        private void Flush()
        {
            if (!_isDirty || !YG2.isSDKEnabled)
                return;

            _isDirty = false;
            _lastFlushTime = Time.unscaledTime;
            YG2.SaveProgress();
        }

        #endregion
    }
}
#endif
