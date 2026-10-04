using System;
using D_Dev.Base;

namespace D_Dev.TimerSystem
{
    public class LongTimer
    {
        #region Fields

        private double _currentTime;
        private long _lastWholeSeconds;

        public event Action OnTimerStart;
        public event Action OnTimerStop;
        public event Action OnTimerEnd;
        public event Action<double> OnTimeChanged;
        public event Action<long> OnWholeSecondsChanged;

        #endregion

        #region Properties

        public CountDirection Direction { get; set; }
        public double TargetTime { get; set; }
        public double CurrentTime => _currentTime;
        public bool IsRunning { get; private set; }

        public long WholeSeconds => Direction == CountDirection.Down
            ? (long)Math.Ceiling(_currentTime)
            : (long)Math.Floor(_currentTime);

        public bool IsCompleted => Direction == CountDirection.Down
            ? _currentTime <= 0
            : TargetTime > 0 && _currentTime >= TargetTime;

        #endregion

        #region Construct

        public LongTimer(CountDirection direction, double currentTime = 0, double targetTime = 0)
        {
            Direction = direction;
            TargetTime = targetTime;
            _currentTime = Clamp(currentTime);
            _lastWholeSeconds = WholeSeconds;
        }

        #endregion

        #region Public

        public void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            OnTimerStart?.Invoke();
        }

        public void Stop()
        {
            if (!IsRunning)
                return;

            IsRunning = false;
            OnTimerStop?.Invoke();
        }

        public void SetTime(double time)
        {
            time = Clamp(time);
            if (time.Equals(_currentTime))
                return;

            _currentTime = time;
            OnTimeChanged?.Invoke(_currentTime);

            long wholeSeconds = WholeSeconds;
            if (wholeSeconds != _lastWholeSeconds)
            {
                _lastWholeSeconds = wholeSeconds;
                OnWholeSecondsChanged?.Invoke(wholeSeconds);
            }
        }

        public void AddTime(double time) => SetTime(_currentTime + time);

        public void Tick(double deltaTime)
        {
            if (!IsRunning || deltaTime <= 0)
                return;

            AddTime(Direction == CountDirection.Up ? deltaTime : -deltaTime);

            if (IsCompleted)
            {
                IsRunning = false;
                OnTimerEnd?.Invoke();
            }
        }

        #endregion

        #region Private

        private double Clamp(double time)
        {
            if (time < 0)
                return 0;

            if (Direction == CountDirection.Up && TargetTime > 0 && time > TargetTime)
                return TargetTime;

            return time;
        }

        #endregion
    }
}
