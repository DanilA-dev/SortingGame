using UnityEngine;

namespace D_Dev.DistanceObserverSystem
{
    public class DistanceObserver : BaseDistanceObserver
    {
        #region Monobehaviour

        private void Update() => Check(Time.deltaTime);

        #endregion
    }
}
