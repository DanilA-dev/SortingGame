using System;
using D_Dev.Singleton;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.ServerTimerSystem
{
    public class ServerClockManager : BaseSingleton<ServerClockManager>
    {
        #region Fields

        [SerializeField] private float _retryDelay = 5f;
        [SerializeField] private float _resyncInterval = 300f;
        [SerializeReference] private IServerTimeProvider _provider = new HttpDateServerTimeProvider();

        [FoldoutGroup("Events")]
        public UnityEvent OnSynced;
        [FoldoutGroup("Events")]
        public UnityEvent OnSyncFailed;

        private bool _isSyncLoopRunning;
        private float _lastSyncTime;

        #endregion

        #region Properties

        private bool IsActiveInstance => _instance == this;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            ServerClock.OnSynced += OnClockSynced;
            ServerClock.OnSyncFailed += OnClockSyncFailed;
        }

        private void OnDisable()
        {
            ServerClock.OnSynced -= OnClockSynced;
            ServerClock.OnSyncFailed -= OnClockSyncFailed;
        }

        private void Start()
        {
            if (IsActiveInstance)
                Sync();
        }

        private void Update()
        {
            if (!IsActiveInstance)
                return;

            if (_resyncInterval > 0 && ServerClock.IsSynced && Time.realtimeSinceStartup - _lastSyncTime >= _resyncInterval)
                Sync();
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused || !IsActiveInstance)
                return;

            ServerClock.Invalidate();
            Sync();
        }

        #endregion

        #region Public

        [FoldoutGroup("Debug"), Button]
        public async void Sync()
        {
            if (_isSyncLoopRunning)
                return;

            _isSyncLoopRunning = true;
            _lastSyncTime = Time.realtimeSinceStartup;

            try
            {
                while (!await ServerClock.SyncAsync(_provider))
                    await Awaitable.WaitForSecondsAsync(_retryDelay, destroyCancellationToken);
            }
            catch (OperationCanceledException) { }
            finally
            {
                _isSyncLoopRunning = false;
                _lastSyncTime = Time.realtimeSinceStartup;
            }
        }

        #endregion

        #region Listeners

        private void OnClockSynced() => OnSynced?.Invoke();
        private void OnClockSyncFailed() => OnSyncFailed?.Invoke();

        #endregion
    }
}
