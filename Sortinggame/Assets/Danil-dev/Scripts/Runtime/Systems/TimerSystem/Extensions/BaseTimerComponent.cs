using D_Dev.PolymorphicValueSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.TimerSystem
{
    public abstract class BaseTimerComponent : MonoBehaviour
    {
        #region Fields

        [SerializeField] protected bool _invokeOnStart;
        [SerializeField] protected bool _repeat;
        [SerializeReference] protected PolymorphicValue<float> _timeValue = new FloatConstantValue();

        [FoldoutGroup("Events")]
        public UnityEvent<float> OnTimerStart;
        [FoldoutGroup("Events")]
        public UnityEvent<float> OnTimerUpdate;
        [FoldoutGroup("Events")]
        public UnityEvent<float> OnTimerEnd;

        protected CountdownTimer _timer;
             
        #endregion

        #region Properties

        public bool InvokeOnStart
        {
            get => _invokeOnStart;
            set => _invokeOnStart = value;
        }

        public PolymorphicValue<float> TimeValue
        {
            get => _timeValue;
            set => _timeValue = value;
        }

        #endregion

        #region Monobehaviour

        protected virtual void Start()
        {
            _timer = new CountdownTimer(_timeValue.Value);
            
            _timer.OnTimerStart += OnStart;
            _timer.OnTimerEnd += OnEnd;
            _timer.OnTimerProgressUpdate += OnProgress;
            
            if (_invokeOnStart)
                StartTimer();
        }

        protected virtual void OnDestroy()
        {
            if (_timer != null)
            {
                _timer.OnTimerStart -= OnStart;
                _timer.OnTimerEnd -= OnEnd;
                _timer.OnTimerProgressUpdate -= OnProgress;
            }
        }

        #endregion

        #region Public

        public void UpdateTimer(float delta)
        {
            if (_timer != null)
                _timer.Tick(delta);
        }
        
        public void ResetTimer(float time)
        {
            _timer?.Reset(time);
        }

        public void ResetTimer()
        {
            _timer?.Reset(_timeValue.Value);
        }

        public void StartTimer()
        {
            ResetTimer(_timeValue.Value);
            _timer?.Start();
        }

        public void StopTimer() => _timer?.Stop();
        public void PauseTimer() => _timer?.Pause();

        #endregion

        #region Listeners

        private void OnStart() => OnTimerStart?.Invoke(_timer.RemainingTime);
        private void OnProgress(float progress) => OnTimerUpdate?.Invoke(progress);
        private void OnEnd()
        {
            OnTimerEnd?.Invoke(_timer.RemainingTime);
            
            if (_repeat)
                StartTimer();
        }

        #endregion
    }
}
