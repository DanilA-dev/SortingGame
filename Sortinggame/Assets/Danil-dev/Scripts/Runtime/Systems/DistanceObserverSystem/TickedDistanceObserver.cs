using D_Dev.UpdateManagerSystem;
using UnityEngine;

namespace D_Dev.DistanceObserverSystem
{
    public class TickedDistanceObserver : BaseDistanceObserver, ITickable
    {
        #region Monobehaviour

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateManager.Add(this);
        }

        private void OnDisable() => UpdateManager.Remove(this);

        #endregion

        #region ITickable

        public void Tick() => Check(Time.deltaTime);

        #endregion
    }
}
