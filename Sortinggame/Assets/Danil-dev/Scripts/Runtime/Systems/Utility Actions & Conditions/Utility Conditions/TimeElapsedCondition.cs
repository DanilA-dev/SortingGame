using D_Dev.Base;
using D_Dev.PolymorphicValueSystem;
using UnityEngine;

namespace D_Dev.Conditions
{
    [System.Serializable]
    public class TimeElapsedCondition : ICondition
    {
        #region Fields

        [SerializeReference] private PolymorphicValue<float> _minTime = new FloatConstantValue { Value = 1f };
        [SerializeReference] private PolymorphicValue<float> _maxTime = new FloatConstantValue { Value = 1f };
        [SerializeField] private bool _unscaledTime;

        private float _startTime;
        private float _duration;
        private bool _isStarted;

        #endregion

        #region Properties

        private float CurrentTime => _unscaledTime ? Time.unscaledTime : Time.time;

        #endregion

        #region ICondition

        public bool IsConditionMet()
        {
            if (!_isStarted)
                Reset();

            return CurrentTime - _startTime >= _duration;
        }

        public void Reset()
        {
            var minTime = _minTime.Value;
            _duration = Random.Range(minTime, Mathf.Max(minTime, _maxTime.Value));
            _startTime = CurrentTime;
            _isStarted = true;
        }

        #endregion
    }
}
