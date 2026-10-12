using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace D_Dev.LocalizationSystem
{
    public class LocalizationInitializer : MonoBehaviour
    {
        #region Fields

        [Title("Settings")]
        [SerializeField] private bool _initializeOnStart;

        [FoldoutGroup("Events")]
        [SerializeField] private UnityEvent _onInitialized;

        private bool _isInitializing;

        #endregion

        #region Properties

        public bool IsInitialized { get; private set; }

        #endregion

        #region MonoBehaviour

        private void Start()
        {
            if (_initializeOnStart)
                Initialize();
        }

        #endregion

        #region Public

        public void Initialize()
        {
            if (_isInitializing || IsInitialized)
                return;

            _isInitializing = true;
            StartCoroutine(InitializeRoutine());
        }

        #endregion

        #region Coroutine

        private IEnumerator InitializeRoutine()
        {
            yield return LocalizationSettings.InitializationOperation;
            _isInitializing = false;
            IsInitialized = true;
            _onInitialized?.Invoke();
        }

        #endregion
    }
}
