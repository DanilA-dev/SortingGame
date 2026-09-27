namespace D_Dev.TimerSystem
{
    public class IncrementTimer : BaseTimer
    {
        #region Fields

        private float _targetTime;

        #endregion

        #region Construct

        public IncrementTimer(float initialTime, float targetTime) : base(0)
        {
            _targetTime = targetTime;
        }

        #endregion

        #region Override

        public override void Tick(float deltaTime)
        {
            if (!IsRunning)
                return;

            if (Time < _targetTime)
            {
                Time += deltaTime;
                if (Time > _targetTime)
                    Time = _targetTime;

                OnProgressUpdate();
            }

            if (Time >= _targetTime)
                Complete();
        }

        public override void Reset() => Time = 0;

        #endregion
    }
}
