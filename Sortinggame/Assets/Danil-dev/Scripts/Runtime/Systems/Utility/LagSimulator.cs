using Sirenix.OdinInspector;
using UnityEngine;

namespace D_Dev.Utility
{
    public class LagSimulator : MonoBehaviour
    {
        #region Fields

        [Title("Settings")]
        [SerializeField] private bool _enabled = true;
        [SerializeField, MinValue(0.1f)] private float _intervalMin = 3f;
        [SerializeField, MinValue(0.1f)] private float _intervalMax = 8f;
        [SerializeField, MinValue(0f)] private float _lagDurationMin = 0.05f;
        [SerializeField, MinValue(0f)] private float _lagDurationMax = 0.3f;

        private float _nextLagTime;

        #endregion

        #region Monobehaviour

        private void Start()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ScheduleNextLag();
#endif
        }

        private void Update()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!_enabled)
                return;

            if (Time.unscaledTime >= _nextLagTime)
            {
                InvokeLag();
            }
#endif
        }

        #endregion

        #region Public

        public void InvokeLag()
        {
            SimulateLag();
            ScheduleNextLag();
        }

        #endregion
        
        #region Private

        private void SimulateLag()
        {
            var duration = Random.Range(_lagDurationMin, _lagDurationMax);
            var end = Time.realtimeSinceStartup + duration;

            while (Time.realtimeSinceStartup < end) 
            { }

            Debug.Log($"[LagSimulator] Lag simulated: {duration * 1000f:F0}ms");
        }

        private void ScheduleNextLag()
        {
            _nextLagTime = Time.unscaledTime + Random.Range(_intervalMin, _intervalMax);
        }

        #endregion
    }
}