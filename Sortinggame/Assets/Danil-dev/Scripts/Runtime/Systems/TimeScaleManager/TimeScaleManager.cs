using System;
using D_Dev.Singleton;
using UnityEngine;

namespace D_Dev.TimeScaleManager
{
    public class TimeScaleManager : BaseSingleton<TimeScaleManager>
    {
        #region Fields

        private float _previousTimeScale = 1f;
        public event Action<float> OnTimeScaleChanged;

        #endregion

        #region Public

        public void SetTimeScale(float value)
        {
            if (Mathf.Approximately(Time.timeScale, value))
                return;

            Time.timeScale = value;
            OnTimeScaleChanged?.Invoke(value);
        }

        public void RestorePrevious()
        {
            SetTimeScale(Instance._previousTimeScale);
        }

        public void Pause()
        {
            Instance._previousTimeScale = Time.timeScale;
            SetTimeScale(0f);
        }

        #endregion
    }
}
