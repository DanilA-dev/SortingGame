using UnityEngine;

namespace D_Dev.StateMachine
{
    public class DelayCondition : IStateCondition
    {
        private float _currentTime;
        private float _maxTime;
        private bool _isTimerRunning;
        
        public DelayCondition(float maxTime)
        {
            _maxTime = maxTime;
            _currentTime = _maxTime;
        }
        
        public bool IsMatched()
        {
            if(!_isTimerRunning && _currentTime <= 0)
                _currentTime = _maxTime;
            
            _isTimerRunning = _currentTime > 0;
            _currentTime -= Time.deltaTime;
            return !_isTimerRunning;
        }
    }
}
