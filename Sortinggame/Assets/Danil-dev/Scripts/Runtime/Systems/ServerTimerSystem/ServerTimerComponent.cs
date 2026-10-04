using System;
using D_Dev.Base;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.ServerTimerSystem
{
    public class ServerTimerComponent : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _autoStart;
        [SerializeField] private CountDirection _direction = CountDirection.Down;
        [SerializeReference] private PolymorphicValue<double> _startTimestamp = new DoubleConstantValue();
        [ShowIf(nameof(_direction), CountDirection.Down)]
        [SerializeReference] private PolymorphicValue<double> _duration = new DoubleConstantValue();

        [FoldoutGroup("Events")]
        public UnityEvent OnTimerStart;
        [FoldoutGroup("Events")]
        public UnityEvent OnTimerStop;
        [FoldoutGroup("Events")]
        public UnityEvent OnTimerEnd;
        [FoldoutGroup("Events")]
        public UnityEvent<string> OnTimeTextChanged;

        private bool _isStartPending;
        private bool _isEndInvoked;
        private long _lastWholeSeconds = long.MinValue;

        #endregion

        #region Properties

        public bool IsStarted => _startTimestamp.Value > 0;
        public bool IsReady => ServerClock.IsSynced;
        public bool IsFinished => IsReady && IsStarted && _direction == CountDirection.Down && GetCurrentTime() <= 0;
        public double CurrentTime => IsReady && IsStarted ? GetCurrentTime() : 0;

        #endregion

        #region Monobehaviour

        private void Awake() => _startTimestamp.OnValueChanged += OnStartTimestampChanged;

        private void Update()
        {
            if (!ServerClock.IsSynced)
                return;

            if (_isStartPending || (_autoStart && !IsStarted))
            {
                _isStartPending = false;
                ApplyStart();
            }

            if (!IsStarted)
                return;

            double time = GetCurrentTime();

            long wholeSeconds = _direction == CountDirection.Down
                ? (long)Math.Ceiling(time)
                : (long)Math.Floor(time);

            if (wholeSeconds != _lastWholeSeconds)
            {
                _lastWholeSeconds = wholeSeconds;
                OnTimeTextChanged?.Invoke(TimeFormatter.Format(wholeSeconds));
            }

            if (_direction == CountDirection.Down && time <= 0 && !_isEndInvoked)
            {
                _isEndInvoked = true;
                OnTimerEnd?.Invoke();
            }
        }

        private void OnDestroy()
        {
            if (_startTimestamp != null)
                _startTimestamp.OnValueChanged -= OnStartTimestampChanged;
        }

        #endregion

        #region Public

        [FoldoutGroup("Debug"), Button]
        public void StartTimer()
        {
            if (!ServerClock.IsSynced)
            {
                _isStartPending = true;
                return;
            }

            ApplyStart();
        }

        [FoldoutGroup("Debug"), Button]
        public void StopTimer()
        {
            _isStartPending = false;

            if (!IsStarted)
                return;

            _startTimestamp.Value = 0;
            OnTimerStop?.Invoke();
        }

        [FoldoutGroup("Debug"), Button]
        public void SkipSeconds(float seconds)
        {
            if (IsStarted)
                _startTimestamp.Value -= seconds;
        }

        public void SkipMinutes(float minutes) => SkipSeconds(minutes * 60f);
        public void SkipHours(float hours) => SkipSeconds(hours * 3600f);

        #endregion

        #region Private

        private void ApplyStart()
        {
            _startTimestamp.Value = ServerClock.UnixNow;
            _isEndInvoked = false;
            _lastWholeSeconds = long.MinValue;
            OnTimerStart?.Invoke();
        }

        private double GetCurrentTime()
        {
            double elapsed = Math.Max(0, ServerClock.UnixNow - _startTimestamp.Value);

            return _direction == CountDirection.Up
                ? elapsed
                : Math.Max(0, _duration.Value - elapsed);
        }

        #endregion

        #region Listeners

        private void OnStartTimestampChanged(double timestamp)
        {
            _lastWholeSeconds = long.MinValue;

            if (!ServerClock.IsSynced || timestamp <= 0 || GetCurrentTime() > 0)
                _isEndInvoked = false;
        }

        #endregion
    }
}
