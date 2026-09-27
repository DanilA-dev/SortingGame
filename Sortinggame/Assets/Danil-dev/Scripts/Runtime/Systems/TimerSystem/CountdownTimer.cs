namespace D_Dev.TimerSystem
{
    public class CountdownTimer : BaseTimer
    {
        #region Properties
        public float RemainingTime => Time;

        #endregion

        #region Construct
        public CountdownTimer(float initialTime) : base(initialTime) {}

        #endregion

        #region Override

        public override void Tick(float deltaTime)
        {
            if (!IsRunning)
                return;

            if (Time > 0)
            {
                Time -= deltaTime;
                if (Time < 0)
                    Time = 0;

                OnProgressUpdate();
            }

            if (Time <= 0)
                Complete();
        }

        #endregion

        #region Public

        public void Reset(float newInitTime)
        {
            _initialTime = newInitTime;
            Reset();
        }

        #endregion
    }
}
