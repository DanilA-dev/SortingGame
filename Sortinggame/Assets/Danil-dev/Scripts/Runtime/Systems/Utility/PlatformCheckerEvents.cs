using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace D_Dev.Utility
{
    public class PlatformCheckerEvents : MonoBehaviour
    {
        #region Fields

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onMobile;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onDesktop;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onWebGLMobile;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onWebGLDesktop;
        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onConsole;

        #endregion

        #region Monobehaviour

        private void Awake() => CheckPlatform();

        #endregion

        #region Private

        private void CheckPlatform()
        {
#if UNITY_WEBGL
            InvokeWebGLPlatform();
#elif UNITY_IOS || UNITY_ANDROID
            _onMobile?.Invoke();
#elif UNITY_STANDALONE
            _onDesktop?.Invoke();
#elif UNITY_PS4 || UNITY_PS5 || UNITY_XBOXONE || UNITY_GAMECORE || UNITY_SWITCH
            _onConsole?.Invoke();
#endif
        }

        private void InvokeWebGLPlatform()
        {
            if (Application.isMobilePlatform)
                _onWebGLMobile?.Invoke();
            else
                _onWebGLDesktop?.Invoke();
        }

        #endregion
    }
}