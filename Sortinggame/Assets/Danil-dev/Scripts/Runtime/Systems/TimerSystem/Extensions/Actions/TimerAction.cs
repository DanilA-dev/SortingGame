using D_Dev.Base;
using UnityEngine;

namespace D_Dev.TimerSystem.Extensions.Actions
{
    [System.Serializable]
    public class TimerAction : BaseAction
    {
        #region Fields

        [SerializeField] private float _delay;
        
        private CountdownTimer _timer;

        #endregion

        #region Overrides
        
        public override void Execute()
        {
            if(_timer == null)
                _timer = new CountdownTimer(_delay);

            if (!_timer.IsRunning)
            {
                _timer.Start();
                _timer.OnTimerEnd += OnTimerEnd;
            }
            else
                _timer.Tick(Time.deltaTime);
        }

        #endregion

        #region Listeners

        private void OnTimerEnd()
        {
            _timer.OnTimerEnd -= OnTimerEnd;
            IsFinished = true;
            _timer.Reset();
            _timer.Stop();
        }

        #endregion
    }
}
