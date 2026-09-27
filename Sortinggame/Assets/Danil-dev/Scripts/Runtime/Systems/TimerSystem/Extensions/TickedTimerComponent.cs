using D_Dev.UpdateManagerSystem;
using UnityEngine;

namespace D_Dev.TimerSystem
{
    public class TickedTimerComponent : BaseTimerComponent, ITickable
    {
        #region Monobehaviour

        private void OnEnable() => UpdateManager.Add(this);
        private void OnDisable() => UpdateManager.Remove(this);

        #endregion

        #region ITickable

        public void Tick()
        {
            UpdateTimer(Time.deltaTime);
        }

        #endregion
    }
}