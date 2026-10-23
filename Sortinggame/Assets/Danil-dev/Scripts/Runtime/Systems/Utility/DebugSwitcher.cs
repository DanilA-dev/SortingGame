using UnityEngine;
using UnityEngine.Events;
using System;

namespace D_Dev.Utility
{
    public enum DebugMode
    {
        All,
        OnlyExceptions,
        None
    }

    public class DebugSwitcher : MonoBehaviour
    {
        #region Fields

        [SerializeField] private DebugMode _editorDebugMode = DebugMode.All;
        [SerializeField] private DebugMode _releaseDebugMode = DebugMode.OnlyExceptions;
        [SerializeField] private UnityEvent _onReleaseBuild;
        [SerializeField] private UnityEvent _onDevelopmentBuild;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            Apply(_releaseDebugMode);
            _onReleaseBuild?.Invoke();
            return;
#endif
            Apply(_editorDebugMode);

#if DEVELOPMENT_BUILD
            _onDevelopmentBuild?.Invoke();
#endif
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Apply(_editorDebugMode);
        }
#endif

        #endregion

        #region Public

        public void SetMode(DebugMode mode)
        {
            _editorDebugMode = mode;
            Apply(mode);
        }

        #endregion

        #region Private

        private void Apply(DebugMode mode)
        {
            switch (mode)
            {
                case DebugMode.All:
                    Debug.unityLogger.filterLogType = LogType.Log;
                    Debug.unityLogger.logEnabled    = true;
                    break;

                case DebugMode.OnlyExceptions:
                    Debug.unityLogger.filterLogType = LogType.Exception;
                    Debug.unityLogger.logEnabled    = true;
                    break;

                case DebugMode.None:
                    Debug.unityLogger.logEnabled = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        #endregion
    }
}