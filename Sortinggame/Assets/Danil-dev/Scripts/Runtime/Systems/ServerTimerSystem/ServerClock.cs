using System;
using UnityEngine;

namespace D_Dev.ServerTimerSystem
{
    public static class ServerClock
    {
        #region Fields

        private static DateTime _syncedUtcTime;
        private static double _syncedRealtime;

        public static event Action OnSynced;
        public static event Action OnSyncFailed;

        #endregion

        #region Properties

        public static bool IsSynced { get; private set; }
        public static bool IsSyncing { get; private set; }

        public static DateTime UtcNow => _syncedUtcTime.AddSeconds(Time.realtimeSinceStartupAsDouble - _syncedRealtime);
        public static double UnixNow => (UtcNow - DateTime.UnixEpoch).TotalSeconds;

        #endregion

        #region Public

        public static async Awaitable<bool> SyncAsync(IServerTimeProvider provider)
        {
            if (provider == null || IsSyncing)
                return IsSynced;

            IsSyncing = true;

            try
            {
                DateTime? utcTime = await provider.RequestUtcTimeAsync();
                if (utcTime.HasValue)
                {
                    _syncedUtcTime = utcTime.Value;
                    _syncedRealtime = Time.realtimeSinceStartupAsDouble;
                    IsSynced = true;
                    OnSynced?.Invoke();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                IsSyncing = false;
            }

            OnSyncFailed?.Invoke();
            return false;
        }

        public static void Invalidate() => IsSynced = false;

        #endregion

        #region Private

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _syncedUtcTime = default;
            _syncedRealtime = 0;
            IsSynced = false;
            IsSyncing = false;
            OnSynced = null;
            OnSyncFailed = null;
        }

        #endregion
    }
}
