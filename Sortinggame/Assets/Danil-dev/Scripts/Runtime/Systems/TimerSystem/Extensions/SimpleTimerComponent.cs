using UnityEngine;

namespace D_Dev.TimerSystem
{
    public class SimpleTimerComponent : BaseTimerComponent
    {
        #region Monobehaviour

        private void Update()
        {
            UpdateTimer(Time.deltaTime);
        }

        #endregion
    }
}