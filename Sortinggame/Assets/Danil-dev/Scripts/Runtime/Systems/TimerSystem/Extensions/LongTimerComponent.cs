using D_Dev.Base;
using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.TimerSystem
{
    public class LongTimerComponent : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _invokeOnStart;
        [SerializeField] private bool _restartOnEnable;
        [SerializeField] private bool _useUnscaledTime;
        [SerializeField] private CountDirection _direction;
        [SerializeReference] private PolymorphicValue<double> _timeValue = new DoubleConstantValue();
        [SerializeReference] private PolymorphicValue<double> _resetTime = new DoubleConstantValue();
        [ShowIf(nameof(_direction), CountDirection.Up)]
        [SerializeReference] private PolymorphicValue<double> _targetTime = new DoubleConstantValue();

        [FoldoutGroup("Events")]
        public UnityEvent OnTimerStart;
        [FoldoutGroup("Events")]
        public UnityEvent OnTimerStop;
        [FoldoutGroup("Events")]
        public UnityEvent OnTimerEnd;
        [FoldoutGroup("Events")]
        public UnityEvent<string> OnTimeTextChanged;

        private LongTimer _timer;
        private double _initialTime;
        private bool _isStarted;

        #endregion

        #region Properties

        public double CurrentTime => _timer?.CurrentTime ?? 0;
        public bool IsRunning => _timer != null && _timer.IsRunning;
        public string TimeText => _timer != null ? TimeFormatter.Format(_timer.WholeSeconds) : string.Empty;

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            _initialTime = _timeValue.Value;
            _timer = new LongTimer(_direction, _initialTime, GetTargetTime());

            _timer.OnTimerStart += OnStart;
            _timer.OnTimerStop += OnStop;
            _timer.OnTimerEnd += OnEnd;
            _timer.OnTimeChanged += OnTimeChanged;
            _timer.OnWholeSecondsChanged += OnWholeSecondsChanged;

            _timeValue.OnValueChanged += OnTimeValueChanged;
        }

        private void OnEnable()
        {
            if (!_restartOnEnable || !_isStarted)
                return;

            _timer.SetTime(_initialTime);
            OnTimeTextChanged?.Invoke(TimeText);
            StartTimer();
        }

        private void Start()
        {
            _isStarted = true;
            OnTimeTextChanged?.Invoke(TimeText);

            if (_invokeOnStart || _restartOnEnable)
                StartTimer();
        }

        private void Update()
        {
            _timer.Tick(_useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        private void OnDestroy()
        {
            if (_timer != null)
            {
                _timer.OnTimerStart -= OnStart;
                _timer.OnTimerStop -= OnStop;
                _timer.OnTimerEnd -= OnEnd;
                _timer.OnTimeChanged -= OnTimeChanged;
                _timer.OnWholeSecondsChanged -= OnWholeSecondsChanged;
            }

            if (_timeValue != null)
                _timeValue.OnValueChanged -= OnTimeValueChanged;
        }

        #endregion

        #region Public

        [FoldoutGroup("Debug"), Button]
        public void StartTimer()
        {
            _timer.TargetTime = GetTargetTime();
            _timer.Start();
        }

        [FoldoutGroup("Debug"), Button]
        public void StopTimer() => _timer.Stop();

        [FoldoutGroup("Debug"), Button]
        public void ResetTimer() => _timer.SetTime(_resetTime.Value);

        public void RestartTimer()
        {
            ResetTimer();
            StartTimer();
        }

        public void SetTime(double time) => _timer.SetTime(time);
        public void SetSeconds(float seconds) => _timer.SetTime(seconds);

        [FoldoutGroup("Debug"), Button]
        public void AddSeconds(float seconds) => _timer.AddTime(seconds);
        public void AddMinutes(float minutes) => _timer.AddTime(minutes * 60.0);
        public void AddHours(float hours) => _timer.AddTime(hours * 3600.0);

        #endregion

        #region Private

        private double GetTargetTime() =>
            _direction == CountDirection.Up && _targetTime != null ? _targetTime.Value : 0;

        #endregion

        #region Listeners

        private void OnStart() => OnTimerStart?.Invoke();
        private void OnStop() => OnTimerStop?.Invoke();
        private void OnEnd() => OnTimerEnd?.Invoke();
        private void OnTimeChanged(double time) => _timeValue.Value = time;
        private void OnWholeSecondsChanged(long seconds) => OnTimeTextChanged?.Invoke(TimeFormatter.Format(seconds));
        private void OnTimeValueChanged(double time) => _timer.SetTime(time);

        #endregion
    }
}
